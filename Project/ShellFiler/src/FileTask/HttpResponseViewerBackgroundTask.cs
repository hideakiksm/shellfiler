using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileViewer.HTTPResponseViewer;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.Management;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;
using ShellFiler.Util;
using ShellFiler.Properties;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：HTTPレスポンスビューア用の通信をバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class HttpResponseViewerBackgroundTask : AbstractFileBackgroundTask {
        // ファイルアクセスの結果を返すオブジェクト
        private AccessibleFile m_accessibleFile;

        // リクエスト
        private ResponseViewerRequest m_request;

        // HTTPモードでの処理
        private HttpModeProcedure m_httpModeProcedure;

        // TCPモードでの処理
        private TcpModeProcedure m_tcpModeProcedure;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider   転送元ファイルの情報
        // 　　　　[in]destProvider  転送先ファイルの情報
        // 　　　　[in]refreshUi     作業完了時のUI更新方法
        // 　　　　[in]accessWrapper ファイルアクセスの結果を返すオブジェクト
        // 戻り値：なし
        //=========================================================================================
        public HttpResponseViewerBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, AccessibleFile accessWrapper, ResponseViewerRequest request)
            : base(srcProvider, destProvider, refreshUi) {
            m_accessibleFile = accessWrapper;
            m_request = request;
            if (m_request.RequestSetting.SelectedMode == ResponseViewerMode.HttpMode) {
                m_httpModeProcedure = new HttpModeProcedure(m_request.RequestSetting.HttpModeRequest, m_request.RequestBody, m_accessibleFile);
            } else {
                m_tcpModeProcedure = new TcpModeProcedure(m_request.RequestSetting.TcpModeRequest, m_request.RequestBody, m_accessibleFile);
            }
            CreateBackgroundTaskPathInfo();
        }

        //=========================================================================================
        // 機　能：転送元/転送先の情報を作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CreateBackgroundTaskPathInfo() {
            // 転送元
            string dispName = m_accessibleFile.DisplayName;
            string srcShort = dispName;
            string srcDetail = dispName;

            // 転送先
            string destShort = "";
            string destDetail = "";

            m_backgroundTaskPathInfo = new BackgroundTaskPathInfo(srcShort, srcDetail, destShort, destDetail);
        }

        //=========================================================================================
        // 機　能：タスクを実行する
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンドスレッドで実行する
        //=========================================================================================
        protected override void ExecuteTask() {
            // リクエスト前にビューアを初期化
            // （ビューアを初期化してキャンセルを受け付けられるようにする）
            m_accessibleFile.AddData(new byte[0], 0, 0);
            m_accessibleFile.FireEvent(false);

            // リクエスト
            if (m_httpModeProcedure != null) {
                m_httpModeProcedure.Execute();
            } else {
                m_tcpModeProcedure.Execute();
            }
        }

        //=========================================================================================
        // 機　能：ファイル転送のタスクをキャンセルする
        // 引　数：[in]reason  キャンセルした理由
        // 戻り値：なし
        //=========================================================================================
        public override void SetCancel(CancelReason reason) {
            base.SetCancel(reason);
            if (m_httpModeProcedure != null) {
                m_httpModeProcedure.CancelRequest();
            } else {
                m_tcpModeProcedure.CancelRequest();
            }
        }


        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.Retrieve;
            }
        }

        //=========================================================================================
        // プロパティ：転送元/転送先の情報
        //=========================================================================================
        public override BackgroundTaskPathInfo PathInfo {
            get {
                return m_backgroundTaskPathInfo;
            }
        }

        //=========================================================================================
        // クラス：HTTPによるリクエスト要求スレッド
        //=========================================================================================
        public class HttpModeProcedure {
            // 通信に使用するリクエスト
            private ResponseViewerHttpRequest m_request;

            // HTTP通信（null:リクエストが開始されていない）
            private HttpWebRequest m_webRequest = null;

            // HTTPボディの電文
            private byte[] m_requestBody;

            // レスポンス電文の書き込み先
            private AccessibleFile m_accessibleFile;

            // リクエストがキャンセルされたときtrue
            private bool m_canceled = false;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]request         通信に使用するリクエスト
            // 　　　　[in]requestBody     HTTPボディの電文
            // 　　　　[in]accessibleFile  レスポンス電文の書き込み先
            // 戻り値：なし
            //=========================================================================================
            internal HttpModeProcedure(ResponseViewerHttpRequest request, byte[] requestBody, AccessibleFile accessibleFile) {
                m_request = request;
                m_requestBody = requestBody;
                m_accessibleFile = accessibleFile;
            }

            //=========================================================================================
            // 機　能：処理の入り口
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Execute() {
                try {
                    Request();
                    if (!m_canceled) {
                        m_accessibleFile.AddCompleted(RetrieveDataLoadStatus.CompletedAll, null);
                    } else {
                        m_accessibleFile.AddCompleted(RetrieveDataLoadStatus.CompletedPart, null);
                    }
                } catch (Exception e) {
                    if (!m_canceled) {
                        string message = string.Format(Resources.FileViewer_HttpResError, e.Message);
                        m_accessibleFile.AddCompleted(RetrieveDataLoadStatus.Failed, message);
                    } else {
                        m_accessibleFile.AddCompleted(RetrieveDataLoadStatus.CompletedPart, null);
                    }
                }
                m_accessibleFile.FireEvent(true);
            }

            //=========================================================================================
            // 機　能：サーバーと通信する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void Request() {
                // 準備
                lock (this) {
                    if (m_canceled) {
                        return;
                    }
                    m_webRequest = (HttpWebRequest)(HttpWebRequest.Create(m_request.RequestUrl));
                }
                m_webRequest.Method = m_request.RequestMethod;

                // プロキシ
                if (m_request.ProxyUrl != null) {
                    m_webRequest.Proxy = new WebProxy(m_request.ProxyUrl);
                }

                // HTTPヘッダを準備
                m_webRequest.ContentLength = m_requestBody.Length;
                foreach (KeyValuePair<string, string> header in m_request.HttpHeader) {
                    string headerKey = header.Key;
                    string headerValue = header.Value;
                    if (headerKey == "Content-Type") {
                        m_webRequest.ContentType = headerValue;
                    } else if (headerKey == "Accept") {
                        m_webRequest.Accept = headerValue;
                    } else if (headerKey == "User-Agent") {
                        m_webRequest.UserAgent = headerValue;
                    } else if (headerKey == "Cache-Control") {
                        m_webRequest.Headers.Add(HttpRequestHeader.CacheControl, headerValue);
                    } else {
                        m_webRequest.Headers.Add(headerKey, headerValue);
                    }
                }

                // リクエスト本体を送信
                DateTime startTime = DateTime.Now;
                DateTime endTimeFirstChunk = DateTime.MinValue;
                DateTime endTimeLastChunk = DateTime.MinValue;
                m_webRequest.Timeout = 30 * 60 * 1000;
                if (m_request.RequestMethod == "GET" || m_request.RequestMethod == "HEAD") {
                    ;
                } else {
                    Stream reqStream = m_webRequest.GetRequestStream();
                    reqStream.Write(m_requestBody, 0, m_requestBody.Length);
                    reqStream.Close();
                }

                // レスポンスを受信
                WebResponse res = m_webRequest.GetResponse();
                Stream resStream = res.GetResponseStream();
                try {
                    byte[] buffer = new byte[4096];
                    while (true) {
                        int readSize = resStream.Read(buffer, 0, buffer.Length);
                        if (endTimeFirstChunk == DateTime.MinValue) {
                            endTimeFirstChunk = DateTime.Now;
                        }
                        if (readSize == 0) {
                            endTimeLastChunk = DateTime.Now;
                            break;
                        }
                        m_accessibleFile.AddData(buffer, 0, readSize);
                        m_accessibleFile.FireEvent(false);
                    }
                } finally {
                    resStream.Close();
                }

                // 実行時間を記録
                double responseTime = (endTimeFirstChunk - startTime).TotalMilliseconds;
                double turnAroundTime = (endTimeLastChunk - startTime).TotalMilliseconds;
                if (responseTime > int.MaxValue) {
                    m_accessibleFile.LoadingResponseTime = -1;
                } else {
                    m_accessibleFile.LoadingResponseTime = (int)responseTime;
                }
                if (turnAroundTime > int.MaxValue) {
                    m_accessibleFile.LoadingTurnAroundTime = -1;
                } else {
                    m_accessibleFile.LoadingTurnAroundTime = (int)turnAroundTime;
                }
            }

            //=========================================================================================
            // 機　能：リクエストをキャンセルする
            // 引　数：なし
            // 戻り値：なし
            // メ　モ：UIスレッドから実行する
            //=========================================================================================
            internal void CancelRequest() {
                lock (this) {
                    if (m_webRequest != null) {
                        m_webRequest.Abort();
                    }
                    m_canceled = true;
                }
            }
        }

        //=========================================================================================
        // クラス：TCPによるリクエスト要求スレッド
        //=========================================================================================
        public class TcpModeProcedure {
            // 通信に使用するリクエスト
            private ResponseViewerTcpRequest m_request;

            // TCPクライアント（null:リクエストが開始されていない）
            private TcpClient m_tcpClient = null;

            // HTTPボディの電文
            private byte[] m_requestBody;

            // レスポンス電文の書き込み先
            private AccessibleFile m_accessibleFile;

            // リクエストがキャンセルされたときtrue
            private bool m_canceled = false;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]request         通信に使用するリクエスト
            // 　　　　[in]requestBody     HTTPボディの電文
            // 　　　　[in]accessibleFile  レスポンス電文の書き込み先
            // 戻り値：なし
            //=========================================================================================
            internal TcpModeProcedure(ResponseViewerTcpRequest request, byte[] requestBody, AccessibleFile accessibleFile) {
                m_request = request;
                m_requestBody = requestBody;
                m_accessibleFile = accessibleFile;
            }

            //=========================================================================================
            // 機　能：処理の入り口
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Execute() {
                try {
                    Request();
                    if (!m_canceled) {
                        m_accessibleFile.AddCompleted(RetrieveDataLoadStatus.CompletedAll, null);
                    } else {
                        m_accessibleFile.AddCompleted(RetrieveDataLoadStatus.CompletedPart, null);
                    }
                } catch (Exception e) {
                    if (!m_canceled) {
                        string message = string.Format(Resources.FileViewer_HttpResError, e.Message);
                        m_accessibleFile.AddCompleted(RetrieveDataLoadStatus.Failed, message);
                    } else {
                        m_accessibleFile.AddCompleted(RetrieveDataLoadStatus.CompletedPart, null);
                    }
                }
                m_accessibleFile.FireEvent(true);
                try {
                    if (m_tcpClient != null && !m_canceled) {
                        m_tcpClient.Close();
                        m_tcpClient = null;
                    }
                } catch (Exception) {
                }
            }

            //=========================================================================================
            // 機　能：サーバーと通信する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void Request() {
                // 準備
                lock (this) {
                    if (m_canceled) {
                        return;
                    }
                    m_tcpClient = new TcpClient(m_request.ServerName, m_request.PortNo);
                }

                DateTime startTime = DateTime.Now;
                DateTime endTimeFirstChunk = DateTime.MinValue;
                DateTime endTimeLastChunk = DateTime.MinValue;

                // リクエスト本体を送信
                NetworkStream stream = m_tcpClient.GetStream();
                try {
                    stream.Write(m_requestBody, 0, m_requestBody.Length);

                    // レスポンスを受信
                    byte[] buffer = new byte[4096];
                    while (true) {
                        int readSize = stream.Read(buffer, 0, buffer.Length);
                        if (endTimeFirstChunk == DateTime.MinValue) {
                            endTimeFirstChunk = DateTime.Now;
                        }
                        if (readSize == 0) {
                            endTimeLastChunk = DateTime.Now;
                            break;
                        }
                        m_accessibleFile.AddData(buffer, 0, readSize);
                        m_accessibleFile.FireEvent(false);
                    }
                } finally {
                    stream.Close();
                }

                // 実行時間を記録
                double responseTime = (endTimeFirstChunk - startTime).TotalMilliseconds;
                double turnAroundTime = (endTimeLastChunk - startTime).TotalMilliseconds;
                if (responseTime > int.MaxValue) {
                    m_accessibleFile.LoadingResponseTime = -1;
                } else {
                    m_accessibleFile.LoadingResponseTime = (int)responseTime;
                }
                if (turnAroundTime > int.MaxValue) {
                    m_accessibleFile.LoadingTurnAroundTime = -1;
                } else {
                    m_accessibleFile.LoadingTurnAroundTime = (int)turnAroundTime;
                }
            }

            //=========================================================================================
            // 機　能：リクエストをキャンセルする
            // 引　数：なし
            // 戻り値：なし
            // メ　モ：UIスレッドから実行する
            //=========================================================================================
            internal void CancelRequest() {
                lock (this) {
                    if (m_tcpClient != null) {
                        m_tcpClient.Close();
                    }
                    m_canceled = true;
                }
            }
        }
    }
}
