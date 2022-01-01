using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.UI.Dialog.FileViewer;
using ShellFiler.FileViewer;
using ShellFiler.FileViewer.HTTPResponseViewer;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.GraphicsViewer;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.Locale;

namespace ShellFiler.Command.FileList.Tools {

    //=========================================================================================
    // クラス：コマンドを実行する
    // HTTPリクエストのプロトコルテストを実行し、結果のレスポンスをファイルビューアで表示します。
    //   書式 　 HTTPResponseViewer()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.0.0
    //=========================================================================================
    class HTTPResponseViewerCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public HTTPResponseViewerCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // ファイル参照に使用するデフォルトフォルダを決定
            string targetPath;
            if (FileSystemID.IsWindows(FileListViewTarget.FileList.FileSystem.FileSystemId)) {
                targetPath = FileListViewTarget.FileList.DisplayDirectoryName;
            } else {
                targetPath = UIFileList.InitialFolder;
            }

            // リクエスト条件を入力
            ResponseViewerRequestSetting defaultValue = Program.Document.UserSetting.ResponseViewerDefaultSetting;
            HttpViewerRequestDialog dialog = new HttpViewerRequestDialog(defaultValue, targetPath);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return null;
            }
            ResponseViewerRequest request = dialog.ResponseViewerRequest;
            ResponseViewerRequestSetting nextDefaultRequest = (ResponseViewerRequestSetting)(request.RequestSetting.Clone());
            Program.Document.UserSetting.ResponseViewerDefaultSetting.SelectedMode = nextDefaultRequest.SelectedMode;
            if (nextDefaultRequest.SelectedMode == ResponseViewerMode.HttpMode) {
                Program.Document.UserSetting.ResponseViewerDefaultSetting.HttpModeRequest = nextDefaultRequest.HttpModeRequest;
            } else {
                Program.Document.UserSetting.ResponseViewerDefaultSetting.TcpModeRequest = nextDefaultRequest.TcpModeRequest;
            }
            

            // 読み込み対象を準備
            string dispUrl;
            if (request.RequestSetting.SelectedMode == ResponseViewerMode.HttpMode) {
                dispUrl = request.RequestSetting.HttpModeRequest.RequestUrl;
            } else {
                dispUrl = request.RequestSetting.TcpModeRequest.ServerName;
            }
            dispUrl = dispUrl.Split(new char[] {'?'}, 2)[0];
            int maxSize = Configuration.Current.TextViewerMaxFileSize;
            AccessibleFile accessibleFile = new AccessibleFile("", FileSystemID.None, dispUrl, true, 8, maxSize, true, 0, EncodingType.UTF8);

            // バックグラウンドタスクを準備
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            IFileProviderSrc srcProvider = new FileProviderSrcDummy(FileListViewTarget.FileList.FileSystem, null);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            HttpResponseViewerBackgroundTask task = new HttpResponseViewerBackgroundTask(srcProvider, destProvider, uiTarget, accessibleFile, request);
            accessibleFile.LoadingTaskId = task.TaskId;

            // ファイルビューアを開く
            FileViewerForm fileViewer = new FileViewerForm(FileViewerForm.ViewerMode.TextView, accessibleFile, task.TaskId);
            fileViewer.Show();

            // 読み込み開始
            Program.Document.BackgroundTaskManager.StartFileTask(task, true);

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.HTTPResponseViewerCommand;
            }
        }
    }
}
