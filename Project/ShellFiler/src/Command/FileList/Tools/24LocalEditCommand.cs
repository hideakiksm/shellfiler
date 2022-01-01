using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Virtual;

namespace ShellFiler.Command.FileList.Tools {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マーク済みのファイルまたはカーソル位置のファイルをローカルPCで編集します。
    //   書式 　 LocalEdit()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class LocalEditCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public LocalEditCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (FileListViewTarget.FileList.Files.Count == 0) {
                return null;
            }
            // マークを確認
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAllFolder, true, null);
            bool markOk = BackgroundTaskCommandUtil.CheckMarkState(FileListViewTarget, Configuration.Current.MarklessEdit);
            if (!markOk) {
                return null;
            }
            if (FileListViewTarget.FileList.MarkedFileCount == 0) {
                return null;
            }

            // 仮想フォルダの場合は実行準備
            bool success = PrepareVirtualFolderCommand.ReadyForVirtualFolder(FileListViewTarget);
            if (!success) {
                return null;
            }

            // 表示名を作成
            List<UIFile> markedList = FileListViewTarget.FileList.MarkFiles;
            string dispName;
            if (markedList.Count == 1) {
                dispName = string.Format(Resources.LocalExec_DisplayNameEdit1, markedList[0].FileName);
            } else {
                dispName = string.Format(Resources.LocalExec_DisplayNameEditN, markedList[0].FileName);
            }
            TemporarySpaceDisplayName nameInfo = new TemporarySpaceDisplayName(dispName, IconImageListID.FileList_LocalEdit);

            // コマンドラインを作成
            // command="C:\…\Editor.exe -a -b {0}"
            // program="C:\…\Editor.exe"
            // argument="-a -b {0}";
            string command = GetEditorCommandLine(FileListViewTarget.FileList.FileSystem.FileSystemId);
            if (command == null) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_EditorCommandLineAssoc);
                return null;
            }
            string program, argument;
            GenericFileStringUtils.SplitCommandLine(command, out program, out argument);    
            IFileListContext fileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
            LocalExecuteBackgroundTask.GetProgramArgumentDelegate argDelegate = delegate(List<string> srcPathList, List<string> destPathList) {
                if (fileListContext != null) {
                    srcPathList = fileListContext.GetExecuteLocalPathList(srcPathList);
                }
                string fileListCommand = GenericFileStringUtils.CreateCommandFiles(srcPathList, false);
                return string.Format(argument, fileListCommand);
            };

            // バックグラウンドタスクで編集を実行
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(FileListViewTarget.FileList, FileListViewTarget.FileList.MarkFiles, srcFileSystem, fileListContext);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            string targetDir = FileListViewTarget.FileList.DisplayDirectoryName;
            LocalExecuteBackgroundTask task = new LocalExecuteBackgroundTask(srcProvider, destProvider, uiTarget, program, argDelegate, nameInfo, targetDir, null);
            Program.Document.BackgroundTaskManager.StartFileTask(task, true);
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return null;
        }

        //=========================================================================================
        // 機　能：エディタのコマンドラインを返す
        // 引　数：[in]fileSystem  編集対象のファイルシステム
        // 戻り値：エディタのコマンドライン（.txtの関連付けから取得できなかったときはnull）
        //=========================================================================================
        public static string GetEditorCommandLine(FileSystemID fileSystem) {
            string commandLine = Configuration.Current.TextEditorCommandLine;
            string commandLineSSH = Configuration.Current.TextEditorCommandLineSSH;
            string result;
            if (commandLine == "") {
                result = GetTextAssocCommandLine();
            } else {
                if (FileSystemID.IsSSH(fileSystem) && commandLineSSH != "") {
                    result = commandLineSSH;
                } else {
                    result = commandLine;
                }
            }
            return result;
        }

        //=========================================================================================
        // 機　能：行番号付きのエディタのコマンドラインを返す
        // 引　数：[in]fileSystem  編集対象のファイルシステム
        // 　　　　[out]withLine   行番号付きのコマンドラインを返したときtrue
        // 戻り値：エディタのコマンドライン（.txtの関連付けから取得できなかったときはnull）
        //=========================================================================================
        public static string GetEditorCommandLineWithLineNum(FileSystemID fileSystem, out bool withLine) {
            string commandLine = Configuration.Current.TextEditorCommandLineWithLineNumber;
            string commandLineSSH = Configuration.Current.TextEditorCommandLineWithLineNumberSSH;
            string result;
            if (commandLine == "") {
                withLine = false;
                result = GetEditorCommandLine(fileSystem);
            } else {
                withLine = true;
                if (FileSystemID.IsSSH(fileSystem) && commandLineSSH != "") {
                    result = commandLineSSH;
                } else {
                    result = commandLine;
                }
            }
            return result;
        }

        //=========================================================================================
        // 機　能：拡張子.txtに関連付けられたコマンドラインを返す
        // 引　数：なし
        // 戻り値：エディタのコマンドライン（.txtの関連付けから取得できなかったときはnull）
        //=========================================================================================
        public static string GetTextAssocCommandLine() {
            // .txtの関連づけを取得
            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(".txt");
            if (regKey == null) {
                return null;
            }
            string fileType = (string)regKey.GetValue("");
            regKey.Close();

            RegistryKey regKey2 = Registry.ClassesRoot.OpenSubKey(string.Format(@"{0}\shell\{1}\command", fileType, "open"));
            if (regKey2 == null) {
                regKey2 = Registry.ClassesRoot.OpenSubKey(string.Format(@"{0}\shell\{1}\command", fileType, "edit"));
                if (regKey2 == null) {
                    return null;
                }
            }
            string command = (string)regKey2.GetValue("");
            regKey2.Close();

            // "%1"を{0}に置換
            int dqp1 = command.IndexOf("\"%1\"");
            if (dqp1 != -1) {
                command = command.Substring(0, dqp1) + "{0}" + command.Substring(dqp1 + 4);
            } else {
                int p1 = command.IndexOf("%1");
                if (p1 != -1) {
                    command = command.Substring(0, p1) + "{0}" + command.Substring(p1 + 2);
                }
            }
            return command;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.LocalEditCommand;
            }
        }
    }
}
