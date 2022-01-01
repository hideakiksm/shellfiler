using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Text;
using ShellFiler.Api;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：バックグラウンドでダンプモードでのバイト列を検索するスレッド
    //=========================================================================================
    public class DumpSearchThread : BaseThread {
        // 所有しているビューア
        private TextFileViewer m_textFileViewer;

        // 検索結果の格納先
        private DumpSearchHitStateList m_searchHitStateList;

        // 検索スレッドに対するリクエスト（新しいリクエストがないときnull）
        private SearchRequest m_searchRequest = null;

        // リクエストが入ったことを表すイベント
        private AutoResetEvent m_requestEvent = new AutoResetEvent(false);

        // はじめの検索結果が確定したイベント
        private ManualResetEvent m_resultFixedEvent = new ManualResetEvent(false);

        // 検索完了イベント
        private ManualResetEvent m_searchCompletedEvent = new ManualResetEvent(false);

        // 終了イベント
        private ManualResetEvent m_endEvent = new ManualResetEvent(false);

        // スレッドを終了するときtrue
        private bool m_endFlag = false;

        // 検索ではじめにヒットした位置（-1:ヒットしない）
        private int m_searchHitLine = -1;

        // ロード中で未完了の状態にあるときtrue
        private bool m_loadingUncompleted = true;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]viewer     所有しているビューア
        // 　　　　[in]stateList  検索結果の格納先
        // 戻り値：なし
        //=========================================================================================
        public DumpSearchThread(TextFileViewer viewer, DumpSearchHitStateList stateList) : base("TextSearchThread", 1) {
            m_textFileViewer = viewer;
            m_searchHitStateList = stateList;
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
        // 機　能：テキスト検索をリクエストする
        // 引　数：[in]condition  検索条件
        // 　　　　[in]forward    前方検索するときtrue
        // 　　　　[in]startLine  検索開始行
        // 　　　　[in]lineBytes  1行に表示するバイト数
        // 　　　　[in]wait       はじめにヒットするまで待機するときtrue
        // 戻り値：検索ではじめにヒットした位置
        //=========================================================================================
        public int RequestSearch(DumpSearchCondition condition, bool forward, int startLine, int lineBytes, bool wait) {
            lock (this) {
                m_searchRequest = new SearchRequest(new DumpSearchCondition(condition), forward, startLine, lineBytes);
                m_requestEvent.Set();
            }
            if (wait) {
                m_resultFixedEvent.WaitOne();
                return m_searchHitLine;
            } else {
                return -1;
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
            // イベントを待つ
            WaitHandle[] waitEventList = { m_endEvent, m_requestEvent };
            int index = WaitHandle.WaitAny(waitEventList);
            if (index == 0) {
                return;               // 終了イベント
            }

            // リクエストを取得
            SearchRequest request = null;
            lock (this) {
                request = m_searchRequest;
                m_searchRequest = null;
            }

            // 実行
            if (request.Forward) {
                // 前方検索
                m_loadingUncompleted = !(m_textFileViewer.TextBufferLineInfo.CompletedLoading);
                ExecuteSearchForward(request.SearchCondition.SearchBytes, request.StartLine, request.LineBytes, request.Forward, true);
                if (m_endFlag) {
                    return;
                }
                ExecuteSearchForward(request.SearchCondition.AutoSearchBytes, request.StartLine, request.LineBytes, request.Forward, false);
                m_resultFixedEvent.Set();
                if (m_endFlag) {
                    return;
                }
                ExecuteSearchReverse(request.SearchCondition.SearchBytes, request.StartLine, request.LineBytes, request.Forward, true);
                if (m_endFlag) {
                    return;
                }
                ExecuteSearchReverse(request.SearchCondition.AutoSearchBytes, request.StartLine, request.LineBytes, request.Forward, false);
            } else {
                // 後方検索
                ExecuteSearchReverse(request.SearchCondition.SearchBytes, request.StartLine, request.LineBytes, request.Forward, true);
                if (m_endFlag) {
                    return;
                }
                ExecuteSearchReverse(request.SearchCondition.AutoSearchBytes, request.StartLine, request.LineBytes, request.Forward, false);
                if (m_endFlag) {
                    return;
                }
                m_loadingUncompleted = !(m_textFileViewer.TextBufferLineInfo.CompletedLoading);
                ExecuteSearchForward(request.SearchCondition.SearchBytes, request.StartLine, request.LineBytes, request.Forward, true);
                if (m_endFlag) {
                    return;
                }
                ExecuteSearchForward(request.SearchCondition.AutoSearchBytes, request.StartLine, request.LineBytes, request.Forward, false);
                m_resultFixedEvent.Set();
            }

            // 終了通知
            if (!m_endFlag) {
                NotifySearchEnd();
            }
        }

        //=========================================================================================
        // 機　能：前方に向かって検索を実行する
        // 引　数：[in]searchBytes  検索バイト列
        // 　　　　[in]startLine    開始行
        // 　　　　[in]lineBytes    1行あたりのバイト数
        // 　　　　[in]forward      前方検索の指定の時true
        // 　　　　[in]searchMode   検索バイト列のときtrue、自動検索バイト列のときfalse
        // 戻り値：なし
        //=========================================================================================
        private void ExecuteSearchForward(byte[] searchBytes, int startLine, int lineBytes, bool forward, bool searchMode) {
            if (searchBytes == null || searchBytes.Length == 0) {
                return;
            }
            DumpSearchCore searchCore = new DumpSearchCore();
            byte[] targetBuffer;
            int fileSize;
            m_textFileViewer.TextBufferLineInfo.TargetFile.GetBuffer(out targetBuffer, out fileSize);
            
            // 検索バイト列
            int lastHitLine = startLine;
            int startAddressSearch = startLine * lineBytes;
            int endAddressSearch = fileSize - searchBytes.Length;
            while (startAddressSearch <= endAddressSearch) {
                if (m_endFlag) {
                    break;
                }

                // 検索を実行
                int hitAddressSearch = searchCore.ForwardSearchHitAddress(searchBytes, targetBuffer, startAddressSearch, fileSize, fileSize);
                if (hitAddressSearch == -1) {
                    int lastLine = (fileSize - 1) / lineBytes;
                    if (lastHitLine <= lastLine) {
                        m_searchHitStateList.FillSearchNotHit(lastHitLine, lastLine, searchMode);
                    }
                    break;
                }

                // ヒット時の処理
                int hitStartLine = hitAddressSearch / lineBytes;
                int hitEndLine = (hitAddressSearch + searchBytes.Length - 1) / lineBytes;
                if (lastHitLine <= hitStartLine - 1) {
                    m_searchHitStateList.FillSearchNotHit(lastHitLine, hitStartLine - 1, searchMode);
                }
                for (int i = hitStartLine; i <= hitEndLine; i++) {
                    m_searchHitStateList.SetSearchHistState(i, SearchHitState.Hit, searchMode);
                }
                if (m_searchHitLine == -1 && forward) {
                    m_searchHitLine = hitStartLine;
                    m_resultFixedEvent.Set();
                }
                startAddressSearch = Math.Max((hitAddressSearch + searchBytes.Length - 1) + 1, (hitStartLine + 1) * lineBytes);
                lastHitLine = hitEndLine + 1;
            }
        }

        //=========================================================================================
        // 機　能：後方に向かって検索を実行する
        // 引　数：[in]searchBytes  検索バイト列
        // 　　　　[in]startLine    開始行
        // 　　　　[in]lineBytes    1行あたりのバイト数
        // 　　　　[in]forward      前方検索の指定の時true
        // 　　　　[in]searchMode   検索バイト列のときtrue、自動検索バイト列のときfalse
        // 戻り値：なし
        //=========================================================================================
        private void ExecuteSearchReverse(byte[] searchBytes, int startLine, int lineBytes, bool forward, bool searchMode) {
            if (searchBytes == null || searchBytes.Length == 0) {
                return;
            }
            DumpSearchCore searchCore = new DumpSearchCore();
            byte[] targetBuffer;
            int fileSize;
            m_textFileViewer.TextBufferLineInfo.TargetFile.GetBuffer(out targetBuffer, out fileSize);
            
            // 検索バイト列
            int lastHitLine = startLine;
            int startAddressSearch = startLine * lineBytes + lineBytes - 1;
            int endAddressSearch = 0 + searchBytes.Length;
            while (startAddressSearch >= endAddressSearch) {
                if (m_endFlag) {
                    break;
                }

                // 検索を実行
                // ヒットした末端アドレスが返る
                int hitAddressSearch = searchCore.ReverseSearchHitAddress(searchBytes, targetBuffer, startAddressSearch, fileSize);
                if (hitAddressSearch == -1) {
                    if (lastHitLine >= 0) {
                        m_searchHitStateList.FillSearchNotHit(0, lastHitLine, searchMode);
                    }
                    break;
                }

                // ヒット時の処理
                int hitStartLine = hitAddressSearch / lineBytes;                            // 末尾（大きいアドレス）
                int hitEndLine = (hitAddressSearch - searchBytes.Length + 1) / lineBytes;   // 先頭（小さいアドレス）
                if (lastHitLine >= hitStartLine + 1) {
                    m_searchHitStateList.FillSearchNotHit(hitStartLine + 1, lastHitLine, searchMode);
                }
                for (int i = hitEndLine; i <= hitStartLine; i++) {
                    m_searchHitStateList.SetSearchHistState(i, SearchHitState.Hit, searchMode);
                }
                if (m_searchHitLine == -1 && !forward) {
                    m_searchHitLine = hitStartLine;
                    m_resultFixedEvent.Set();
                }
                startAddressSearch = Math.Max((hitAddressSearch - searchBytes.Length + 1) - 1, hitEndLine * lineBytes - 1);
                lastHitLine = hitStartLine - 1;
            }
        }

        //=========================================================================================
        // 機　能：検索が終了したことを通知する
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：delegateによりUIスレッドで実行する
        //=========================================================================================
        private void NotifySearchEnd() {
            m_searchCompletedEvent.Set();
            Program.MainWindow.BeginInvoke(new NotifySearchEndDelegate(NotifySearchEndUI));
        }
        delegate void NotifySearchEndDelegate();
        private void NotifySearchEndUI() {
            m_textFileViewer.NotifySearchEnd(false);
        }

        //=========================================================================================
        // プロパティ：検索完了イベント
        //=========================================================================================
        public ManualResetEvent SearchCompletedEvent {
            get {
                return m_searchCompletedEvent;
            }
        }
        
        //=========================================================================================
        // プロパティ：前回の検索条件
        //=========================================================================================
        public DumpSearchCondition SearchCondition {
            get {
                return m_searchRequest.SearchCondition;
            }
        }

        //=========================================================================================
        // プロパティ：ロード中で未完了の状態にあるときtrue
        //=========================================================================================
        public bool LoadingUncompleted {
            get {
                return m_loadingUncompleted;
            }
        }

        //=========================================================================================
        // クラス：ファイル一覧をクロールするためのリクエスト
        //=========================================================================================
        private class SearchRequest {
            // 検索条件
            public DumpSearchCondition SearchCondition;

            // 前方検索するときtrue
            public bool Forward;

            // 検索開始論理行
            public int StartLine;

            // 1行に表示するバイト数
            public int LineBytes;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]condition  検索条件
            // 　　　　[in]forward    前方検索するときtrue
            // 　　　　[in]startLine  検索開始行
            // 　　　　[in]lineBytes  1行に表示するバイト数
            // 戻り値：なし
            //=========================================================================================
            public SearchRequest(DumpSearchCondition condition, bool forward, int startLine, int lineBytes) {
                SearchCondition = condition;
                Forward = forward;
                StartLine = startLine;
                LineBytes = lineBytes;
            }
        }
    }
}
