using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.Virtual;

namespace ShellFiler.Command.FileList.FileOperationEtc {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 差分表示ツールでマークファイルまたはディレクトリと比較します。
    //   書式 　 DiffMark()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.1.0
    //=========================================================================================
    class DiffMarkCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DiffMarkCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 差分表示ツールのコマンドラインを取得
            string command = GetDiffToolCommand(false);
            if (command == null) {
                DiffToolDownloadDialog dialog = new DiffToolDownloadDialog();
                dialog.ShowDialog(Program.MainWindow);
                return null;
            }

            // ファイル構成をチェック
            List<UIFile> fileList = FileListViewTarget.FileList.MarkFiles;
            if (fileList.Count == 0) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_DiffNoMark);
                return null;
            } else if (fileList.Count > 3) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_DiffTooManyMark);
                return null;
            }
            bool isDir = false;
            bool isFile = false;
            foreach (UIFile file in fileList) {
                if (file.Attribute.IsDirectory) {
                    isDir = true;
                } else {
                    isFile = true;
                }
            }
            if (isDir && isFile) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_DiffFileDir);
                return null;
            }

            // 仮想フォルダの場合は実行準備
            bool success = PrepareVirtualFolderCommand.ReadyForVirtualFolder(FileListViewTarget);
            if (!success) {
                return null;
            }

            // 表示名を作成
            List<UIFile> markedList = FileListViewTarget.FileList.MarkFiles;
            string dispName = string.Format(Resources.LocalExec_DisplayNameDiffMark, markedList[0].FileName);
            TemporarySpaceDisplayName nameInfo = new TemporarySpaceDisplayName(dispName, IconImageListID.FileList_DiffMark);

            // コマンドラインを作成
            // command="C:\…\DiffTool.exe {0}"
            // program="C:\…\DiffTool.exe"
            // argument="{0}";
            IFileListContext fileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // virtualInfo:ライフサイクル管理
            string program, argument;
            GenericFileStringUtils.SplitCommandLine(command, out program, out argument);    
            LocalExecuteBackgroundTask.GetProgramArgumentDelegate argDelegate = delegate(List<string> srcPathList, List<string> destPathList) {
                if (fileListContext != null) {
                    srcPathList = fileListContext.GetExecuteLocalPathList(srcPathList);
                }
                string fileListCommand = GenericFileStringUtils.CreateCommandFiles(srcPathList, false);
                return string.Format(argument, fileListCommand);
            };

            // バックグラウンドで開始
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
        // 機　能：差分表示ツールのコマンドラインを取得する
        // 引　数：[in]isDirectory  ディレクトリが対象のときtrue
        // 戻り値：コマンドライン（取得できなかったときnull）
        //=========================================================================================
        public static string GetDiffToolCommand(bool isDirectory) {
            // 設定から取得
            string pathValue;
            if (isDirectory) {
                pathValue = Configuration.Current.DiffDirectoryCommandLine;
                if (pathValue.Trim() != "") {
                    return pathValue;
                }
            } else {
                pathValue = Configuration.Current.DiffCommandLine;
                if (pathValue.Trim() != "") {
                    return pathValue;
                }
            }

            // ショートカットを確認
            string lnkPath = Path.Combine(Program.InstallPath, "diff.lnk");
            if (!File.Exists(lnkPath)) {
                return null;
            }
            pathValue = Win32API.ResolveShortcut(lnkPath);
            if (pathValue == null) {
                return null;
            }
            pathValue = "\""+ pathValue + "\" {0}";

            return pathValue;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.DiffMarkCommand;
            }
        }
    }
}
