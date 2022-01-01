using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;

namespace ShellFiler.UI.FileList.Crawler {

    //=========================================================================================
    // クラス：バックグラウンドでファイルをクロールするスレッド
    //=========================================================================================
    public class FileCrawlThread : BaseThread {        
        // ウィンドウを再読込するためのリクエスト（添字の小さい方から処理、[0]が実行中タスクで実行完了時にクリア）
        private List<FileCrawlRequest> m_listRefreshRequest = new List<FileCrawlRequest>();

        // リクエストが入ったことを表すイベント
        private AutoResetEvent m_requestEvent = new AutoResetEvent(false);

        // 終了イベント
        private ManualResetEvent m_endEvent = new ManualResetEvent(false);

        // スレッドを終了するときtrue
        private bool m_endFlag = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileCrawlThread() : base("FileCrawlThread", 1) {
            StartThread();
        }
        
        //=========================================================================================
        // 機　能：処理を終了する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            m_endEvent.Set();
            m_endFlag = true;
            JoinThread();
        }

        //=========================================================================================
        // 機　能：再読込をリクエストする
        // 引　数：[in]fileList   再読込するファイル一覧
        // 　　　　[in]crawlType  クロールによって取得する情報
        // 　　　　[in]param      リクエストの追加パラメータ
        // 戻り値：なし
        //=========================================================================================
        public void RequestNewFileCrawl(UIFileList fileList, CrawlType crawlType, Object param) {
            lock (this) {
                for (int i = 0; i < m_listRefreshRequest.Count; i++) {
                    if (m_listRefreshRequest[i].FileList.UIFileListId == fileList.UIFileListId) {
                        m_listRefreshRequest.RemoveAt(i);
                        break;
                    }
                }
                m_listRefreshRequest.Add(new FileCrawlRequest(fileList, crawlType, param));
                m_requestEvent.Set();
            }
        }

        public void StopFileCrawl(UIFileList fileList) {
            lock (this) {
                for (int i = 0; i < m_listRefreshRequest.Count; i++) {
                    if (m_listRefreshRequest[i].FileList.UIFileListId == fileList.UIFileListId) {
                        m_listRefreshRequest.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイル一覧が削除されたときの処理を行う
        // 引　数：[in]fileList  削除されるファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public void OnDisposeUIFileList(UIFileList fileList) {
            lock (this) {
                for (int i = 0; i < m_listRefreshRequest.Count; i++) {
                    if (m_listRefreshRequest[i].FileList.UIFileListId == fileList.UIFileListId) {
                        m_listRefreshRequest.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：スレッドの入り口
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンドスレッドで実行する
        //=========================================================================================
        protected override void ThreadEntry() {
            ExecuteTask();
        }

        //=========================================================================================
        // 機　能：タスクを実行する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ExecuteTask() {
            while (true) {
                // イベントを待つ
                bool wait = false;
                lock (this) {
                    wait = (m_listRefreshRequest.Count == 0);
                }
                if (wait) {
                    WaitHandle[] waitEventList = { m_endEvent, m_requestEvent };
                    int index = WaitHandle.WaitAny(waitEventList);
                    if (index == 0) {
                        return;               // 終了イベント
                    }
                }

                // リクエストを取得
                FileCrawlRequest request = null;
                lock (this) {
                    if (m_listRefreshRequest.Count > 0) {
                        request = m_listRefreshRequest[0];
                    }
                }

                // 実行
                if (request != null) {
                    FileClawler crawler = null;
                    switch (request.CrawlType) {
                        case CrawlType.Icon:
                            crawler = new FileCrawlerExtractIcon(this);
                            break;
                        case CrawlType.Thumbnail:
                            crawler = new FileCrawlerCreateThumbnail(this);
                            break;
                    }
                    crawler.Execute(request);
                }

                lock (this) {
                    if (m_listRefreshRequest.Count > 0 && m_listRefreshRequest[0] == request) {
                        m_listRefreshRequest.RemoveAt(0);
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：リクエストがキャンセルされたかどうかを返す
        // 引　数：[in]request   現在実行しているリクエスト
        // 戻り値：リクエストがキャンセルされたときtrue
        //=========================================================================================
        public bool CheckRequestCanceled(FileCrawlRequest request) {
            bool requestCanceled;
            lock (this) {
                requestCanceled = (m_listRefreshRequest.Count > 0 && m_listRefreshRequest[0] != request);
            }
            return requestCanceled;
        }

        //=========================================================================================
        // 機　能：ファイルアイコンを描画する
        // 引　数：[in]index  logLine  登録するログ情報
        // 戻り値：なし
        // メ　モ：delegateによりUIスレッドで実行する
        //=========================================================================================
        public void DrawFileIcon(UIFileList fileList, int index, UIFile file) {
            BaseThread.InvokeProcedureByMainThread(new DrawFileIconDelegate(DrawFileIconUI), fileList, index);
        }
        delegate void DrawFileIconDelegate(UIFileList fileList, int index);
        private void DrawFileIconUI(UIFileList fileList, int index) {
            // 対象ファイルが変わっていないことを確認
            UIFileList currentFileList = Program.Document.CurrentTabPage.GetFileList(fileList.IsLeftWindow);
            if (currentFileList.LoadedAge != fileList.LoadedAge) {
                return;
            }
            if (index >= fileList.Files.Count) {
                return;
            }

            // 再描画
            IFileListViewComponent component = null;
            if (fileList.IsLeftWindow) {
                component = Program.MainWindow.LeftFileListView.FileListViewComponent;
            } else {
                component = Program.MainWindow.RightFileListView.FileListViewComponent;
            }
            component.DrawFileIcon(index);
        }

        //=========================================================================================
        // クラス：クロールを終了するときtrue
        //=========================================================================================
        public bool CrawlEnd {
            get {
                return m_endFlag;
            }
        }
    }
}
