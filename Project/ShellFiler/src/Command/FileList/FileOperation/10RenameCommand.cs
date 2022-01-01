using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.FileOperation {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ファイルまたはディレクトリの、名前または属性を変更します。
    //   書式 　 Rename()
    //   引数  　なし
    //   戻り値　bool:名前の変更をバックグラウンドで開始したときtrue、変更を開始できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class RenameCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public RenameCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            FileOperationStatus status;
            if (FileListViewTarget.FileList.Files.Count == 0) {
                return false;
            }
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }

            UIFile file = FileListViewTarget.FileList.Files[FileListComponentTarget.CursorLineNo];
            if (file.FileName == "..") {
                return false;
            }
            string fullPath = FileListViewTarget.FileList.DisplayDirectoryName + file.FileName;

            // SSHシェルの場合は属性を取得
            IFile fullFileInfo = null;              // SSHシェル以外はnullのままで、UIFile fileから情報を取得する
            if (FileListViewTarget.FileList.FileSystem.FileSystemId == FileSystemID.SSHShell) {
                status = ShellGetFileInfoDialog.GetFileInfo(fullPath, FileSystemID.SSHShell, FileListViewTarget.FileList.FileListContext, out fullFileInfo);
                if (!status.Succeeded) {
                    return false;
                }
            }

            // ダイアログで新しい名前を入力
            FileSystemID srcFileSystemId = FileListViewTarget.FileList.FileSystem.FileSystemId;
            RenameFileInfo renameInfoOrg = file.CreateRenameFileInfo(srcFileSystemId, fullFileInfo);
            RenameFileInfo.IRenameFileDialog dialog = renameInfoOrg.CreateRenameDialog(srcFileSystemId);
            if (dialog == null) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_RenameNotSupported);
                return false;
            }
            DialogResult result = dialog.ShowRenameDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return false;
            }
            RenameFileInfo renameInfoNew = dialog.ModifiedRenameFileInfo;
            if (!renameInfoOrg.IsModified(renameInfoNew)) {
                return false;
            }

            // バックグラウンドタスクで名前の変更を実行
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshTarget, RefreshUITarget.RefreshOption.SpecifyCursorFile, renameInfoNew.FileName);
            IFileProviderSrc srcProvider = new FileProviderSrcDummy(srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            RenameBackgroundTask task = new RenameBackgroundTask(srcProvider, destProvider, uiTarget, fullPath, renameInfoOrg, renameInfoNew);
            Program.Document.BackgroundTaskManager.StartFileTask(task, true);
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.RenameCommand;
            }
        }
    }
}
