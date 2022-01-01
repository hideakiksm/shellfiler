using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.Util;
using ShellFiler.FileTask;
using ShellFiler.GraphicsViewer;

namespace ShellFiler.FileTask.Management {
    
    //=========================================================================================
    // クラス：グラフィックビューア内でのタスク管理のクラス
    //=========================================================================================
    public class GraphicsViewerTaskManager {
        // 所有クラス
        private BackgroundTaskManager m_taskManager;

        // ファイル操作でのバックグラウンドタスク（null:まだ起動していない）
        private GraphicsViewerBackgroundTask m_gvBackgroundTask = null;

        // リクエスト後の処理待ちキューの一覧
        private List<GraphicsViewerImageLoadRequest> m_requestQueue = new List<GraphicsViewerImageLoadRequest>();

        // バックグラウンドで処理中のリクエスト
        private GraphicsViewerImageLoadRequest m_responseWait = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]taskManager  所有クラス
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerTaskManager(BackgroundTaskManager taskManager) {
            m_taskManager = taskManager;
        }
        
        //=========================================================================================
        // 機　能：画像読み込みのリクエストを登録する
        // 引　数：[in]request  読み込みのリクエスト
        // 戻り値：なし
        //=========================================================================================
        public void RequestLoadImage(GraphicsViewerImageLoadRequest request) {
            lock (this) {
                // 逆向きのリクエストを消す
                int index = 0;
                while (index < m_requestQueue.Count) {
                    if (request.GraphicsViewerForm == m_requestQueue[index].GraphicsViewerForm && request.Forward != m_requestQueue[index].Forward) {
                        m_requestQueue.RemoveAt(index);
                    } else {
                        index++;
                    }
                }
                if (m_responseWait != null && request.GraphicsViewerForm == m_responseWait.GraphicsViewerForm && request.Forward != m_responseWait.Forward) {
                    m_responseWait = null;
                }

                // 処理待ちに登録
                m_requestQueue.Add(request);
            }

            // ロードタスクを起動
            if (m_gvBackgroundTask == null) {
                m_gvBackgroundTask = new GraphicsViewerBackgroundTask();
                m_taskManager.StartFileTask(m_gvBackgroundTask, true);
            } else {
                m_gvBackgroundTask.RequestLoad();
            }
        }

        //=========================================================================================
        // 機　能：グラフィックビューア用のタスクが終了したときの処理を行う
        // 引　数：なし
        // 戻り値：次の画像のリクエスト
        //=========================================================================================
        public void OnTerminateTask() {
            m_gvBackgroundTask = null;
        }

        //=========================================================================================
        // 機　能：待機中の画像一覧から、次の画像を読み込むリクエストを取得する
        // 引　数：なし
        // 戻り値：次の画像のリクエスト
        //=========================================================================================
        public GraphicsViewerImageLoadRequest GetNextLoadRequest() {
            GraphicsViewerImageLoadRequest request = null;
            lock (this) {
                if (m_requestQueue.Count > 0) {
                    request = m_requestQueue[0];
                    m_requestQueue.RemoveAt(0);
                }
                m_responseWait = request;
            }
            return request;
        }

        //=========================================================================================
        // 機　能：進捗状態に更新があったときのイベントを発行する
        // 引　数：[in]taskId   更新があったタスクのID
        // 戻り値：なし
        // メ　モ：delegateによりユーザインターフェーススレッドで実行される
        //=========================================================================================
        public void NotifyLoadingEnd(ImageInfo result) {
            BaseThread.InvokeProcedureByMainThread(new NotifyLoadingEndDelegate(NotifyLoadingEndUI), result);
        }
        delegate void NotifyLoadingEndDelegate(ImageInfo result);
        private void NotifyLoadingEndUI(ImageInfo result) {
            GraphicsViewerForm form = null;
            lock (this) {
                if (m_responseWait == null || result.FilePath != m_responseWait.LoadingImage.FilePath) {
                    form = null;
                } else {
                    form = m_responseWait.GraphicsViewerForm;
                    m_responseWait = null;
                }
            }
            if (form != null) {
                form.NotifyFileLoad(result);
            }
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]form   閉じられたフォーム
        // 戻り値：なし
        //=========================================================================================
        public void OnCloseForm(GraphicsViewerForm form) {
            // このフォームのリクエストを消す
            lock (this) {
                int index = 0;
                while (index < m_requestQueue.Count) {
                    if (m_requestQueue[index].GraphicsViewerForm == form) {
                        m_requestQueue.RemoveAt(index);
                    } else {
                        index++;
                    }
                }
                if (m_responseWait != null && m_responseWait.GraphicsViewerForm == form) {
                    m_responseWait = null;
                }
            }
            if (m_gvBackgroundTask != null && m_requestQueue.Count == 0 && m_responseWait == null) {
                m_gvBackgroundTask.SetCancel(CancelReason.User);
                m_gvBackgroundTask = null;
            }
        }
    }
}
