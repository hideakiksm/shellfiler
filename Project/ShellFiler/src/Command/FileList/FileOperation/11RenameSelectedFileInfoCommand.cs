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
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.FileOperation {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ファイルまたはディレクトリの、名前または属性を一括で変更します。
    //   書式 　 RenameSelectedFileInfo()
    //   引数  　なし
    //   戻り値　bool:属性の変更をバックグラウンドで開始したときtrue、変更を開始できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class RenameSelectedFileInfoCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public RenameSelectedFileInfoCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // マークを確認
            bool markOk = BackgroundTaskCommandUtil.CheckMarkState(FileListViewTarget, Configuration.Current.MarklessAttribute);
            if (!markOk) {
                return false;
            }
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }

            // ダイアログで新しい名前を入力
            FileSystemID fileSystemID = FileListViewTarget.FileList.FileSystem.FileSystemId;
            RenameSelectedFileInfo.IRenameSelectedFileDialog dialog = RenameSelectedFileInfo.CreateRenameDialog(fileSystemID, FileListViewTarget.FileList);
            if (dialog == null) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_RenameNotSupported);
                return false;
            }
            DialogResult result = dialog.ShowRenameDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return false;
            }
            RenameSelectedFileInfo renameInfo = dialog.RenameSelectedFileInfo;
            if (!renameInfo.IsModified()) {
                return false;
            }

            // バックグラウンドタスクで名前の変更を実行
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshTarget, RefreshUITarget.RefreshOption.None);
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(FileListViewTarget.FileList, FileListViewTarget.FileList.MarkFiles, srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            RenameSelectedFileInfoBackgroundTask task = new RenameSelectedFileInfoBackgroundTask(srcProvider, destProvider, uiTarget, renameInfo);
            Program.Document.BackgroundTaskManager.StartFileTask(task, true);
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.RenameSelectedFileInfoCommand;
            }
        }
    }
}
