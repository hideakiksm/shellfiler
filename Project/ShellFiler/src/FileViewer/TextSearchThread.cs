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
    // クラス：バックグラウンドでテキストを検索するスレッド
    //=========================================================================================
    public class TextSearchThread : BaseThread {
        // 所有しているビューア
        private TextFileViewer m_textFileViewer;

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
        // 引　数：[in]viewer  所有しているビューア
        // 戻り値：なし
        //=========================================================================================
        public TextSearchThread(TextFileViewer viewer) : base("TextSearchThread", 1) {
            m_textFileViewer = viewer;
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
        // 　　　　[in]wait       はじめにヒットするまで待機するときtrue
        // 戻り値：検索ではじめにヒットした位置
        //=========================================================================================
        public int RequestSearch(TextSearchCondition condition, bool forward, int startLine, bool wait) {
            lock (this) {
                m_searchRequest = new SearchRequest(new TextSearchCondition(condition), forward, startLine);
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
            }

            // 実行
            if (request.Forward) {
                m_loadingUncompleted = !(m_textFileViewer.TextBufferLineInfo.CompletedLoading);
                ExecuteSearchForward(request, request.StartLine);
                if (!m_endFlag) {
                    ExecuteSearchReverse(request, request.StartLine);
                }
            } else {
                ExecuteSearchReverse(request, request.StartLine);
                m_loadingUncompleted = !(m_textFileViewer.TextBufferLineInfo.CompletedLoading);
                if (!m_endFlag) {
                    ExecuteSearchForward(request, request.StartLine);
                }
            }
            if (!m_endFlag) {
                NotifySearchEnd();
            }
        }

        //=========================================================================================
        // 機　能：前方に向かって検索を実行する
        // 引　数：[in]request  リクエスト
        // 　　　　[in]line     検索開始行
        // 戻り値：なし
        //=========================================================================================
        private void ExecuteSearchForward(SearchRequest request, int line) {
            TextBufferLineInfoList lineInfoList = m_textFileViewer.TextBufferLineInfo;
            int lineCount = lineInfoList.LogicalLineCount;
            while (line < lineCount) {
                if (m_endFlag) {
                    break;
                }

                // 連続行を探してstrLineListに入れる
                List<string> strLineList = new List<string>();
                TextBufferLogicalLineInfo startLineInfo = lineInfoList.GetLineInfo(line);
                strLineList.Add(startLineInfo.StrLineOrg);
                if (line + 1 < lineCount) {
                    for (int i = line + 1; i < lineCount; i++) {
                        TextBufferLogicalLineInfo lineInfo = lineInfoList.GetLineInfo(i);
                        if (startLineInfo.PhysicalLineNo != lineInfo.PhysicalLineNo) {
                            break;
                        }
                        strLineList.Add(lineInfo.StrLineOrg);
                    }
                }

                // 文字列にまとめる
                StringBuilder strCombine = new StringBuilder();
                for (int i = 0; i < strLineList.Count; i++) {
                    strCombine.Append(strLineList[i]);
                }

                // 検索を実行
                List<int> hitPosSearch, hitLengthSearch, hitPosAuto, hitLengthAuto;
                TextSearchCore searchCore = new TextSearchCore();
                searchCore.SearchLine(strCombine.ToString(), request.SearchCondition, out hitPosSearch, out hitLengthSearch, out hitPosAuto, out hitLengthAuto);
                int hitLine = SetHitPosition(line, strLineList, hitPosSearch, request.SearchCondition.SearchString, true);
                SetHitPosition(line, strLineList, hitPosAuto, request.SearchCondition.AutoSearchString, false);
                if (hitPosSearch != null && hitPosSearch.Count > 0 || hitPosAuto != null && hitPosAuto.Count > 0) {
                    if (m_searchHitLine == -1 && request.Forward) {
                        m_searchHitLine = line + hitLine;
                        m_resultFixedEvent.Set();
                    }
                }
                line += strLineList.Count;
            }
            m_resultFixedEvent.Set();
        }

        //=========================================================================================
        // 機　能：後方に向かって検索を実行する
        // 引　数：[in]request  リクエスト
        // 　　　　[in]line     検索開始行
        // 戻り値：なし
        //=========================================================================================
        private void ExecuteSearchReverse(SearchRequest request, int line) {
            TextBufferLineInfoList lineInfoList = m_textFileViewer.TextBufferLineInfo;
            int lineCount = lineInfoList.LogicalLineCount;
            while (line >= 0) {
                if (m_endFlag) {
                    break;
                }

                // 連続行を探してstrLineListに入れる
                List<string> strLineList = new List<string>();
                TextBufferLogicalLineInfo startLineInfo = lineInfoList.GetLineInfo(line);
                strLineList.Add(startLineInfo.StrLineOrg);
                if (line - 1 >= 0) {
                    for (int i = line - 1; i >= 0; i--) {
                        TextBufferLogicalLineInfo lineInfo = lineInfoList.GetLineInfo(i);
                        if (startLineInfo.PhysicalLineNo != lineInfo.PhysicalLineNo) {
                            break;
                        }
                        strLineList.Add(lineInfo.StrLineOrg);
                    }
                }

                // 文字列にまとめる
                strLineList.Reverse();
                StringBuilder strCombine = new StringBuilder();
                for (int i = 0; i < strLineList.Count; i++) {
                    strCombine.Append(strLineList[i]);
                }

                // 検索を実行
                List<int> hitPosSearch, hitLengthSearch, hitPosAuto, hitLengthAuto;
                TextSearchCore searchCore = new TextSearchCore();
                searchCore.SearchLine(strCombine.ToString(), request.SearchCondition, out hitPosSearch, out hitLengthSearch, out hitPosAuto, out hitLengthAuto);
                int hitLine = SetHitPosition(line - strLineList.Count + 1, strLineList, hitPosSearch, request.SearchCondition.SearchString, true);
                SetHitPosition(line - strLineList.Count + 1, strLineList, hitPosAuto, request.SearchCondition.AutoSearchString, false);
                if (hitPosSearch != null && hitPosSearch.Count > 0 || hitPosAuto != null && hitPosAuto.Count > 0) {
                    if (m_searchHitLine == -1 && !request.Forward) {
                        m_searchHitLine = line + hitLine;
                        m_resultFixedEvent.Set();
                    }
                }
                line = line - strLineList.Count;
            }
            m_resultFixedEvent.Set();
        }

        //=========================================================================================
        // 機　能：検索結果を格納する
        // 引　数：[in]line        格納開始の論理行番号
        // 　　　　[in]strLineList 物理行１行に対する各行の文字列（line, line+1, line+2…）
        // 　　　　[in]hitPos      strLineListを１つの文字列としたときのヒット位置のインデックス配列（null:検索対象外）
        // 　　　　[in]keyword     検索キーワード（hitPosがnullのときnull）
        // 　　　　[in]isSearch    検索語句指定の結果のときtrue、自動検索の結果のときfalse
        // 戻り値：検索結果（文字列中、はじめにヒットしたインデックスのリスト）
        //=========================================================================================
        private int SetHitPosition(int line, List<string> strLineList, List<int> hitPos, String keyword, bool isSearch) {
            if (hitPos == null) {
                return - 1;
            }
            int hitStrLineIndex = 0;
            int keywordLen = keyword.Length;
            int lineTopIndex = 0;
            int idxHitPos = 0;
            TextBufferLineInfoList lineInfoList = m_textFileViewer.TextBufferLineInfo;
            for (int i = 0; i < strLineList.Count; i++) {
                if (idxHitPos >= hitPos.Count) {
                    break;
                }

                TextBufferLogicalLineInfo lineInfo = lineInfoList.GetLineInfo(line + i);
                if (hitPos[idxHitPos] < lineTopIndex + strLineList[i].Length) {
                    if (isSearch) {
                        if (lineInfo.SearchHitState != SearchHitState.Hit) {
                            lineInfo.SearchHitState = SearchHitState.Hit;
                            lineInfoList.AddSearchHitCount(true, 1);
                        }
                    } else {
                        if (lineInfo.AutoSearchHitState != SearchHitState.Hit) {
                            lineInfo.AutoSearchHitState = SearchHitState.Hit;
                            lineInfoList.AddSearchHitCount(false, 1);
                        }
                    }
                    hitStrLineIndex = i;
                } else {
                    if (isSearch) {
                        if (lineInfo.SearchHitState == SearchHitState.Hit) {
                            lineInfoList.AddSearchHitCount(true, -1);
                        }
                        lineInfo.SearchHitState = SearchHitState.NotHit;
                    } else {
                        if (lineInfo.AutoSearchHitState == SearchHitState.Hit) {
                            lineInfoList.AddSearchHitCount(false, -1);
                        }
                        lineInfo.AutoSearchHitState = SearchHitState.NotHit;
                    }
                }
                while (idxHitPos < hitPos.Count && hitPos[idxHitPos] < lineTopIndex + strLineList[i].Length && hitPos[idxHitPos] + keywordLen <= lineTopIndex + strLineList[i].Length) {
                    idxHitPos++;
                }
                lineTopIndex += strLineList[i].Length;
            }
            return hitStrLineIndex;
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
            m_textFileViewer.NotifySearchEnd(true);
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
        public TextSearchCondition SearchCondition {
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
            public TextSearchCondition SearchCondition;

            // 前方検索するときtrue
            public bool Forward;

            // 検索開始論理行
            public int StartLine;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]condition  検索条件
            // 　　　　[in]forward    前方検索するときtrue
            // 　　　　[in]startLine  検索開始行
            // 戻り値：なし
            //=========================================================================================
            public SearchRequest(TextSearchCondition condition, bool forward, int startLine) {
                SearchCondition = condition;
                Forward = forward;
                StartLine = startLine;
            }
        }
    }
}
