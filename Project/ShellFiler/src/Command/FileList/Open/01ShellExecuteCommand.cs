using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.OSSpec;
using ShellFiler.Document.SSH;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Locale;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.FileTask;
using ShellFiler.Properties;
using ShellFiler.Command.FileList;
using ShellFiler.FileViewer;

namespace ShellFiler.Command.FileList.Open {

    //=========================================================================================
    // クラス：コマンドを実行する
    // シェルコマンドを実行する。
    //   書式 　 ShellExecuteMenu()
    //   引数  　なし
    //   戻り値　bool:コマンドの実行開始を試行したときtrue、キャンセルしたときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ShellExecuteMenuCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ShellExecuteMenuCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // 処理開始条件をチェック
            if (FileListViewTarget.FileList.Files.Count == 0) {
                return false;
            }

            // 準備
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);

            // コマンドラインを入力
            CommandHistory commandHistory = Program.Document.UserSetting.CommandHistory;
            CommandNameDictionary commandDictionary = Program.Document.GetCommandNameDictionary(srcFileSystem.FileSystemId);
            int cursorNo = FileListViewTarget.FileListViewComponent.CursorLineNo;
            ShellExecuteRelayMode relayMode;
            Encoding encoding;
            if (FileSystemID.IsWindows(srcFileSystem.FileSystemId)) {
                if (Configuration.Current.ShellExecuteRelayModeWindowsDefault == null) {
                    relayMode = Program.Document.UserGeneralSetting.ShellExecuteRelayModeWindows;
                } else {
                    relayMode = Configuration.Current.ShellExecuteRelayModeWindowsDefault;
                }
                encoding = EncodingType.WindowsDefaultEncoding;
            } else if (FileSystemID.IsSSH(srcFileSystem.FileSystemId)) {
                if (Configuration.Current.ShellExecuteRelayModeSSHDefault == null) {
                    relayMode = Program.Document.UserGeneralSetting.ShellExecuteRelayModeSSH;
                } else {
                    relayMode = Configuration.Current.ShellExecuteRelayModeSSHDefault;
                }
                encoding = SSHUtils.GetEncoding(FileListViewTarget.FileList.FileListContext);
            } else if (FileSystemID.IsVirtual(srcFileSystem.FileSystemId)) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CanNotExecuteFromVirtualDir);
                return false;
            } else {
                FileSystemID.NotSupportError(srcFileSystem.FileSystemId);
                return false;
            }
            CommandInputDialog dialog = new CommandInputDialog(commandHistory, commandDictionary, FileListViewTarget.FileList, cursorNo, relayMode);
            DialogResult dialogResult = dialog.ShowDialog(Program.MainWindow);
            if (dialogResult != DialogResult.OK) {
                return false;
            }

            // 入力結果を取得＆修正
            string command = dialog.Command;
            relayMode = dialog.RelayMode;
            if (FileSystemID.IsWindows(srcFileSystem.FileSystemId)) {
                Program.Document.UserGeneralSetting.ShellExecuteRelayModeWindows = relayMode;
            } else if (FileSystemID.IsSSH(srcFileSystem.FileSystemId)) {
                Program.Document.UserGeneralSetting.ShellExecuteRelayModeSSH = relayMode;
            } else {
                FileSystemID.NotSupportError(srcFileSystem.FileSystemId);
                return false;
            }
            List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
            if (srcFileSystem.FileSystemId == FileSystemID.SSHShell) {
                command = SSHUtils.GetShellCommandDictionary(FileListViewTarget.FileList.FileListContext).ModifyShellExecuteCommand(command, FileListViewTarget.FileList.DisplayDirectoryName);
                expect = SSHUtils.GetShellCommandDictionary(FileListViewTarget.FileList.FileListContext).ExpectShellExecute;
            }

            // 処理開始条件をチェック
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(ShellExecuteBackgroundTask));
            if (!canStart) {
                return false;
            }

            // 取得結果の格納先
            IRetrieveFileDataTarget dataTarget;
            bool relayStdout;
            if (relayMode == ShellExecuteRelayMode.RelayFileViewer) {
                string filePath = FileListViewTarget.FileList.DisplayDirectoryName + FileListViewTarget.FileList.Files[FileListComponentTarget.CursorLineNo].FileName;
                int maxSize = Configuration.Current.TextViewerMaxFileSize;
                dataTarget = new AccessibleFile(null, FileSystemID.None, command, true, 8, maxSize, false, 0, srcFileSystem.DefaultEncoding);
                relayStdout = true;
            } else if (relayMode == ShellExecuteRelayMode.RelayLogWindow) {
                dataTarget = new RetrieveFileDataTargetShellExecute(encoding);
                relayStdout = true;
            } else {
                dataTarget = new RetrieveFileDataTargetShellExecuteDummy();
                relayStdout = false;
            }

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshTarget, RefreshUITarget.RefreshOption.None);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
            IFileProviderSrc srcProvider = new FileProviderSrcDummy(srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            ShellExecuteBackgroundTask execTask = new ShellExecuteBackgroundTask(srcProvider, destProvider, uiTarget, FileListViewTarget.FileList.DisplayDirectoryName, command, expect, relayStdout, relayStdout, dataTarget);
            
            // ファイルビューアを開く
            if (relayMode == ShellExecuteRelayMode.RelayFileViewer) {
                ((AccessibleFile)dataTarget).LoadingTaskId = execTask.TaskId;
                FileViewerForm fileViewer = new FileViewerForm(FileViewerForm.ViewerMode.Auto, (AccessibleFile)dataTarget, execTask.TaskId);
                fileViewer.Show();
            }

            // タスク開始
            Program.Document.BackgroundTaskManager.StartFileTask(execTask, true);
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);

            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ShellExecuteMenuCommand;
            }
        }
    }
}
