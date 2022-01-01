using System;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Management;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.FileList;

namespace ShellFiler.Command.FileList.Internal {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 内部処理用に指定された再試行情報を使って削除します。
    //   書式 　 DeleteRetryInternal(FileErrorRetryInfo retryInfo)
    //   引数  　retryInfo:再試行情報
    //   戻り値　bool:削除をバックグラウンドで開始したときtrue、削除を開始できなかったときfalseを返します。
    //=========================================================================================
    class DeleteRetryInternalCommand : FileListActionCommand {
        // 再試行情報
        private FileErrorRetryInfo m_retryInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DeleteRetryInternalCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_retryInfo = (FileErrorRetryInfo)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            bool withRecycle = (m_retryInfo.TaskType == BackgroundTaskType.Delete);
            return DeleteExecute(withRecycle, m_retryInfo);
        }

        //=========================================================================================
        // 機　能：削除を実行する
        // 引　数：[in]withRecycle          ごみ箱を使って削除するときtrue
        // 　　　　[in]retryInfo            再試行情報
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        private static bool DeleteExecute(bool withRecycle, FileErrorRetryInfo retryInfo) {
            // 処理開始条件をチェック
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(DeleteBackgroundTask));
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, retryInfo.SrcFileSystem, true);
            if (!canStart) {
                return false;
            }

            // ファイル削除の確認
            DeleteStartDialog dialog = new DeleteStartDialog(withRecycle, retryInfo.SrcFileSystem);
            dialog.InitializeByRetryInfo(retryInfo);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result == DialogResult.Cancel) {
                return false;
            }
            DeleteFileOption deleteOpt = (DeleteFileOption)(dialog.DeleteFileOption.Clone());
            bool delWithRecy = dialog.DeleteWithRecycle;
            Program.Document.UserGeneralSetting.DeleteFileOption = (DeleteFileOption)(deleteOpt.Clone());

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(null, null, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(retryInfo.SrcFileSystem);
            IFileProviderSrc srcProvider = new FileProviderSrcSpecified(retryInfo.RetryFileList, srcFileSystem, null);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            DeleteBackgroundTask task = new DeleteBackgroundTask(srcProvider, destProvider, uiTarget, deleteOpt, delWithRecy, retryInfo.Option, retryInfo.RetryApiList);
            Program.Document.BackgroundTaskManager.StartFileTask(task, true);
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
