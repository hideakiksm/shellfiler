using System;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Condition;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.FileList;

namespace ShellFiler.Command.FileList.FileOperation {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マークされたファイルを削除します。
    //   書式 　 Delete()
    //   引数  　なし
    //   戻り値　bool:削除をバックグラウンドで開始したときtrue、削除を開始できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class DeleteCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DeleteCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            return DeleteCommand.DeleteExecute(FileListViewTarget, FileListViewOpposite, true, false);
        }

        //=========================================================================================
        // 機　能：削除を実行する
        // 引　数：[in]fileListViewTarget   対象パスのファイル一覧
        // 　　　　[in]fileListViewOpposite 反対パスのファイル一覧
        // 　　　　[in]withRecycle          ごみ箱を使って削除するときtrue
        // 　　　　[in]deleteEx             拡張削除モードにするときtrue
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public static bool DeleteExecute(FileListView fileListViewTarget, FileListView fileListViewOpposite, bool withRecycle, bool deleteEx) {
            // マークを確認
            bool markOk = BackgroundTaskCommandUtil.CheckMarkState(fileListViewTarget, Configuration.Current.MarklessDelete);
            if (!markOk) {
                return false;
            }
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, fileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }

            // ファイル削除の確認
            bool startTask = true;
            CompareCondition condition = null;
            DeleteFileOption deleteOpt;
            bool delWithRecy;
            if (!deleteEx) {
                DeleteStartDialog dialog = new DeleteStartDialog(withRecycle, fileListViewTarget.FileList.FileSystem.FileSystemId);
                dialog.InitializeByMarkFile(fileListViewTarget.FileList);
                DialogResult result = dialog.ShowDialog(Program.MainWindow);
                if (result == DialogResult.Cancel) {
                    return false;
                }
                deleteOpt = (DeleteFileOption)(dialog.DeleteFileOption.Clone());
                delWithRecy = dialog.DeleteWithRecycle;
                Program.Document.UserGeneralSetting.DeleteFileOption = (DeleteFileOption)(deleteOpt.Clone());
            } else {
                DeleteExStartDialog dialog = new DeleteExStartDialog(withRecycle, fileListViewTarget.FileList.FileSystem.FileSystemId);
                dialog.InitializeByMarkFile(fileListViewTarget.FileList);
                DialogResult result = dialog.ShowDialog(Program.MainWindow);
                if (result == DialogResult.Cancel) {
                    return false;
                }
                deleteOpt = (DeleteFileOption)(dialog.DeleteFileOption.Clone());
                delWithRecy = dialog.DeleteWithRecycle;
                startTask = !dialog.SuspededTask;
                Program.Document.UserGeneralSetting.DeleteFileOption = (DeleteFileOption)(deleteOpt.Clone());
                condition = dialog.TransferCondition;
            }

            // 処理開始条件をチェック
            canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(DeleteBackgroundTask));
            if (!canStart) {
                return false;
            }

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(fileListViewTarget, fileListViewOpposite, RefreshUITarget.RefreshMode.RefreshTarget, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(fileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(fileListViewTarget);               // fileListContext:ライフサイクル管理
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(fileListViewTarget.FileList, fileListViewTarget.FileList.MarkFiles, srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            AttributeSetMode attributeSetMode = (AttributeSetMode)(Configuration.Current.TransferAttributeSetMode.Clone());
            CopyMoveDeleteOption option = new CopyMoveDeleteOption(null, null, condition, attributeSetMode, null, null, false);
            DeleteBackgroundTask task = new DeleteBackgroundTask(srcProvider, destProvider, uiTarget, deleteOpt, delWithRecy, option, null);
            Program.Document.BackgroundTaskManager.StartFileTask(task, startTask);
            fileListViewTarget.FileListViewComponent.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.DeleteCommand;
            }
        }
    }
}
