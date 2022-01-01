using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Management;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.Properties;
using ShellFiler.GraphicsViewer;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：グラフィックビューア用にファイルの読み込みをバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    public class GraphicsViewerBackgroundTask : BaseThread, IBackgroundTask {
        // リクエスト待ちにする時間[ms]
        private const int REQUEST_WAIT_TIMEOUT = 5000;

        // バックグラウンドタスクID
        private BackgroundTaskID m_backgroundTaskId;

        // キャンセル状態
        private CancelFlag m_cancelFlag = new CancelFlag();

        // キャンセルされたときシグナル状態になるイベント
        private ManualResetEvent m_cancelEvent;

        // キャンセル状態に対するロック
        private object m_cancelFlagLock = new object();

        // 現在の処理に対するコンテキスト情報（現在の処理がないときnull、キャンセルイベントはm_cancelEventをマスターとする）
        private GraphicsViewerRequestContext m_gvRequestContext = null;
        
        // 画像読み込みのリクエストが入ったことを表すイベント
        private AutoResetEvent m_requestLoadEvent = new AutoResetEvent(false);

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider   転送元ファイルの情報
        // 　　　　[in]destProvider  転送先ファイルの情報
        // 　　　　[in]refreshUi     作業完了時のUI更新方法
        // 　　　　[in]param         グラフィックビューア用画像の読み込みパラメータ
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerBackgroundTask() : base("GraphicsViewerBackgroundTask", 1) {
            m_backgroundTaskId = BackgroundTaskID.NextId();
            m_cancelEvent = new ManualResetEvent(false);

            m_backgroundTaskPathInfo = new BackgroundTaskPathInfo("", "", "", "");
        }

        //=========================================================================================
        // 機　能：タスクを開始する
        // 引　数：[in]suspend  Suspend状態で起動するときtrue
        // 戻り値：なし
        //=========================================================================================
        public void StartTask(bool suspend) {
            StartThread();          // スレッド開始
        }

        //=========================================================================================
        // 機　能：処理を中断または再開する
        // 引　数：[in]suspend  Suspend状態にするときtrue
        // 戻り値：なし
        //=========================================================================================
        public void SetSuspendState(bool suspend) {
        }

        //=========================================================================================
        // 機　能：スレッドの入り口
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンドスレッドで実行する
        //=========================================================================================
        protected override void ThreadEntry() {
            try {
                ExecuteTask();
            } catch (Exception e) {
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionTask, "GraphicsViewerBackgroundTask");
            }
            // バックグラウンドタスク稼働時はメインウィンドウを閉じる操作を禁止しているため、
            // 呼び出しは成功するはず。
            // BaseThread.InvokeProcedureByMainThreadはJoinThreadと競合するため、ここでは使用できない。
            Program.MainWindow.BeginInvoke(new OnNotifyFileTaskEndDelegate(OnNotifyFileTaskEndUI), m_backgroundTaskId);
        }
        private delegate void OnNotifyFileTaskEndDelegate(BackgroundTaskID taskId);
        private static void OnNotifyFileTaskEndUI(BackgroundTaskID taskId) {
            try {
                Program.Document.BackgroundTaskManager.OnNotifyFileTaskEnd(taskId);
            } catch (Exception e) {
                Program.Abort("UIスレッドでの処理OnNotifyFileTaskEndUIの途中でエラーが発生しました。\r\n" + e.ToString());
            }
        }

        //=========================================================================================
        // 機　能：タスクを実行する
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンドスレッドで実行する
        //=========================================================================================
        protected void ExecuteTask() {
            try {
                while (true) {
                    // リクエストを取得
                    GraphicsViewerImageLoadRequest request = Program.Document.BackgroundTaskManager.GraphicsViewerTaskManager.GetNextLoadRequest();
                    if (request == null) {
                        // イベントを待つ
                        WaitHandle[] waitEventList = { m_cancelEvent, m_requestLoadEvent };
                        int index = WaitHandle.WaitAny(waitEventList, REQUEST_WAIT_TIMEOUT);
                        if (index == 0) {                                   // 終了イベント
                            return;
                        } else if (index == WaitHandle.WaitTimeout) {       // タイムアウト
                            break;
                        }                                                   // 新規リクエスト
                        continue;
                    }

                    // コンテキストを切り替え
                    if (m_gvRequestContext == null || (m_gvRequestContext.CurrentBasePath != request.BasePath || m_gvRequestContext.CurrentFileSystem != request.TargetFileSystem || m_gvRequestContext.CurrentFileListContext != request.TargetFileListContext)) {
                        if (m_gvRequestContext != null) {
                            m_gvRequestContext.CurrentFileSystem.EndFileOperation(m_gvRequestContext.RequestContext);
                            m_gvRequestContext.RequestContext.Dispose();
                        }
                        FileOperationRequestContext reqContext = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(m_backgroundTaskId, request.TargetFileSystem.FileSystemId, FileSystemID.None, request.TargetFileListContext, null, m_cancelEvent);
                        m_gvRequestContext = new GraphicsViewerRequestContext(request.BasePath, request.TargetFileSystem, request.TargetFileListContext, reqContext);
                        m_gvRequestContext.CurrentFileSystem.BeginFileOperation(m_gvRequestContext.RequestContext, m_gvRequestContext.CurrentBasePath);
                    }

                    // 画像を読み込み
                    ImageInfo result = LoadImage(request.LoadingImage.FilePath);

                    // タスク管理に通知
                    if (IsCancel) {
                        break;
                    }
                    Program.Document.BackgroundTaskManager.GraphicsViewerTaskManager.NotifyLoadingEnd(result);
                }
            } finally {
                if (m_gvRequestContext != null) {
                    m_gvRequestContext.CurrentFileSystem.EndFileOperation(m_gvRequestContext.RequestContext);
                    m_gvRequestContext.RequestContext.Dispose();
                }
            }
        }

        //=========================================================================================
        // 機　能：画像を読み込む
        // 引　数：[in]filePath  読み込むファイルのパス
        // 戻り値：読み込んだ結果
        //=========================================================================================
        private ImageInfo LoadImage(string filePath) {
            // ファイルを読み込む
            BufferedImage image;
            long maxSize = Configuration.Current.GraphicsViewerMaxFileSize;
            FileOperationStatus status = m_gvRequestContext.CurrentFileSystem.RetrieveImage(m_gvRequestContext.RequestContext, filePath, maxSize, out image);
            if (!status.Succeeded) {
                return ImageInfo.Fail(filePath, status);
            }
            int colorBits = Image.GetPixelFormatSize(image.Image.PixelFormat);
            return ImageInfo.Success(filePath, image, colorBits);

//          // ファイルを読み込む
//          byte[] buffer;
//          long maxSize = Configuration.Current.GraphicsViewerMaxFileSize;
//          FileOperationStatus status = m_gvRequestContext.CurrentFileSystem.RetrieveFileWhole(m_gvRequestContext.RequestContext, filePath, maxSize, out buffer);
//          if (!status.Succeeded) {
//              return ImageInfo.Fail(filePath, status);
//          }
//
//          // フォーマットを変換する
//          ImageLoader loader = new ImageLoader();
//          Image image;
//          int colorBits;
//          status = loader.LoadImage(buffer, out image, out colorBits);
//          if (!status.Succeeded) {
//              return ImageInfo.Fail(filePath, status);
//          }
//
//          // 成功
//          return ImageInfo.Success(filePath, image, colorBits);
        }

        //=========================================================================================
        // 機　能：読み込みリクエストを行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RequestLoad() {
            m_requestLoadEvent.Set();
        }

        //=========================================================================================
        // 機　能：ファイル転送のタスクをキャンセルする
        // 引　数：[in]reason  キャンセルした理由
        // 戻り値：なし
        //=========================================================================================
        public void SetCancel(CancelReason reason) {
            lock (m_cancelFlagLock) {
                m_cancelEvent.Set();
                m_cancelFlag.SetCancel(reason);
                if (m_gvRequestContext != null) {
                    m_gvRequestContext.RequestContext.SetCancel(reason);
                }
            }
        }

        //=========================================================================================
        // プロパティ：転送元/転送先の情報
        //=========================================================================================
        public BackgroundTaskPathInfo PathInfo {
            get {
                return m_backgroundTaskPathInfo;
            }
        }

        //=========================================================================================
        // プロパティ：タスクID
        //=========================================================================================
        public BackgroundTaskID TaskId {
            get {
                return m_backgroundTaskId;
            }
        }

        //=========================================================================================
        // プロパティ：処理を中止するときtrue
        //=========================================================================================
        public override bool IsCancel {
            get {
                if (base.IsCancel) {
                    return true;
                }
                return m_cancelFlag.IsCancel;
            }
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.GraphicsViewer;
            }
        }

        //=========================================================================================
        // クラス：グラフィックビューアでのリクエストコンテキスト
        //=========================================================================================
        private class GraphicsViewerRequestContext {
            // 読み込み対象の画像があるディレクトリ
            public string CurrentBasePath;

            // 読み込み対象の画像があるファイルシステム
            public IFileSystem CurrentFileSystem;

            // ファイル一覧のコンテキスト情報
            public IFileListContext CurrentFileListContext;

            // 使用中のリクエストコンテキスト
            public FileOperationRequestContext RequestContext;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]basePath     読み込み対象の画像があるディレクトリ
            // 　　　　[in]fileSystem   読み込み対象の画像があるファイルシステム
            // 　　　　[in]fileListCtx  ファイル一覧のコンテキスト情報
            // 　　　　[in]context      使用中のリクエストコンテキスト
            // 戻り値：なし
            //=========================================================================================
            public GraphicsViewerRequestContext(string basePath, IFileSystem fileSystem, IFileListContext fileListCtx, FileOperationRequestContext context) {
                CurrentBasePath = basePath;
                CurrentFileListContext = fileListCtx;
                CurrentFileSystem = fileSystem;
                RequestContext = context;
            }
        }
    }
}
