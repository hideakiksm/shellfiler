using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;
using System.Runtime.InteropServices;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：テキスト検索のエンジン
    // 　　　　1つのビューアに付き、1つのインスタンスが生成される
    //=========================================================================================
    public class TextSearchEngine {
        // 所有しているビューア
        private TextFileViewer m_textFileViewer;

        // ステータスバー
        private FileViewerStatusBar m_statusBar;

        // 対象となるテキストの行情報
        private TextBufferLineInfoList m_lineInfo;

        // ダンプビューアでの検索ヒット情報
        private DumpSearchHitStateList m_dumpHitStateList = new DumpSearchHitStateList();

        // バックグラウンドでのテキスト検索スレッド（検索中でないときはnull、検索完了後はそのまま保持）
        private TextSearchThread m_searchTextThread = null;

        // バックグラウンドでのダンプ検索スレッド（検索中でないときはnull、検索完了後はそのまま保持）
        private DumpSearchThread m_searchDumpThread = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]viewer    所有しているビューア
        // 　　　　[in]statusBar ステータスバー
        // 　　　　[in]lineInfo  対象となるテキストの行情報
        // 戻り値：なし
        //=========================================================================================
        public TextSearchEngine(TextFileViewer viewer, FileViewerStatusBar statusBar, TextBufferLineInfoList lineInfo) {
            m_lineInfo = lineInfo;
            m_textFileViewer = viewer;
            m_statusBar = statusBar;
        }

        //=========================================================================================
        // 機　能：処理を終了する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            StopSearchThread();
        }

        //=========================================================================================
        // 機　能：検索処理を停止する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void StopSearchThread() {
            if (m_searchTextThread != null) {
                m_searchTextThread.Dispose();
                m_searchTextThread = null;
            }
            if (m_searchDumpThread != null) {
                m_searchDumpThread.Dispose();
                m_searchDumpThread = null;
            }
        }

        //=========================================================================================
        // 機　能：新規にテキストでの検索を実行する
        // 引　数：[in]condition  検索条件
        // 　　　　[in]forward    前方検索するときtrue
        // 　　　　[in]startLine  検索開始行
        // 戻り値：検索にヒットした行（-1:見つからない）
        //=========================================================================================
        public int SearchTextNew(TextSearchCondition condition, bool forward, int startLine) {
            if (condition == null || condition.SearchString == null) {
                return -1;
            }
            // 検索ヒットフラグをクリア
            ClearSearchResult();
            m_statusBar.SetSearchHitCount(m_lineInfo.SearchHitCount, m_lineInfo.AutoSearchHitCount);

            // 検索を実行
            StopSearchThread();
            m_searchTextThread = new TextSearchThread(m_textFileViewer);
            int hitPos = m_searchTextThread.RequestSearch(condition, forward, startLine, true);
            m_textFileViewer.Invalidate();
            if (m_searchTextThread.LoadingUncompleted) {
                m_textFileViewer.ShowStatusbarMessage(Resources.FileViewer_SearchNowLoading, FileOperationStatus.LogLevel.Info, IconImageListID.FileViewer_SearchGeneric);
            }

            return hitPos;
        }

        //=========================================================================================
        // 機　能：前回の続きからテキストでの検索を実行する
        // 引　数：[in]forward    前方検索するときtrue
        // 　　　　[in]startLine  検索開始行
        // 戻り値：検索にヒットした行（-1:見つからない）
        //=========================================================================================
        public int SearchTextNext(bool forward, int startLine) {
            // 検索完了まで待つ
            if (m_searchTextThread == null) {
                return -1;
            }
            if (m_searchTextThread.LoadingUncompleted) {
                return SearchTextNew(m_searchTextThread.SearchCondition, forward, startLine);
            }
            m_searchTextThread.SearchCompletedEvent.WaitOne();

            int lineCount = m_lineInfo.LogicalLineCount;
            if (forward) {
                // 前方方向に検索
                if (startLine + 1 < lineCount) {
                    for (int i = startLine + 1; i < lineCount; i++) {
                        TextBufferLogicalLineInfo lineInfo = m_lineInfo.GetLineInfo(i);
                        if (lineInfo.SearchHitState == SearchHitState.Hit) {
                            return i;
                        }
                    }
                }
                return -1;
            } else {
                // 後方方向に検索
                if (startLine - 1 >= 0) {
                    for (int i = startLine - 1; i >= 0; i--) {
                        TextBufferLogicalLineInfo lineInfo = m_lineInfo.GetLineInfo(i);
                        if (lineInfo.SearchHitState == SearchHitState.Hit) {
                            return i;
                        }
                    }
                }
                return -1;
            }
        }

        //=========================================================================================
        // 機　能：新規にテキストでの自動検索を実行する
        // 引　数：[in]condition  検索条件
        // 戻り値：なし
        //=========================================================================================
        public void SearchTextAutoNew(TextSearchCondition condition) {
            // 検索ヒットフラグをクリア
            ClearSearchResult();
            m_statusBar.SetSearchHitCount(m_lineInfo.SearchHitCount, m_lineInfo.AutoSearchHitCount);

            // 検索を実行
            StopSearchThread();

            m_searchTextThread = new TextSearchThread(m_textFileViewer);
            m_searchTextThread.RequestSearch(condition, true, 0, false);
        }
        
        //=========================================================================================
        // 機　能：新規にダンプでの検索を実行する
        // 引　数：[in]condition  検索条件
        // 　　　　[in]forward    前方検索するときtrue
        // 　　　　[in]startLine  検索開始行
        // 　　　　[in]lineBytes  1行に表示するバイト数
        // 戻り値：検索にヒットした行（-1:見つからない）
        //=========================================================================================
        public int SearchDumpNew(DumpSearchCondition condition, bool forward, int startLine, int lineBytes) {
            if (condition == null || condition.SearchBytes == null) {
                return -1;
            }
            // 検索ヒットフラグをクリア
            ClearSearchResult();
            m_statusBar.SetSearchHitCount(m_lineInfo.SearchHitCount, m_lineInfo.AutoSearchHitCount);

            // 検索を実行
            StopSearchThread();

            m_searchDumpThread = new DumpSearchThread(m_textFileViewer, m_dumpHitStateList);
            int hitPos = m_searchDumpThread.RequestSearch(condition, forward, startLine, lineBytes, true);
            m_textFileViewer.Invalidate();
            if (m_searchDumpThread.LoadingUncompleted) {
                m_textFileViewer.ShowStatusbarMessage(Resources.FileViewer_SearchNowLoading, FileOperationStatus.LogLevel.Info, IconImageListID.FileViewer_SearchGeneric);
            }

            return hitPos;
        }

        //=========================================================================================
        // 機　能：前回の続きからダンプでの検索を実行する
        // 引　数：[in]forward    前方検索するときtrue
        // 　　　　[in]startLine  検索開始行
        // 　　　　[in]lineBytes  1行に表示するバイト数
        // 戻り値：検索にヒットした行（-1:見つからない）
        //=========================================================================================
        public int SearchDumpNext(bool forward, int startLine, int lineBytes) {
            // 検索完了まで待つ
            if (m_searchDumpThread == null) {
                return -1;
            }
            m_searchDumpThread.SearchCompletedEvent.WaitOne();

            byte[] readBuffer;
            int readSize;
            m_lineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);
            int lineCount = (readSize - 1) / lineBytes + 1;
            if (forward) {
                // 前方方向に検索
                if (startLine + 1 < lineCount) {
                    SearchHitState search, auto;
                    for (int i = startLine + 1; i < lineCount; i++) {
                        m_dumpHitStateList.GetHistState(i, out search, out auto);
                        if (search == SearchHitState.Hit) {
                            return i;
                        }
                    }
                }
                return -1;
            } else {
                // 後方方向に検索
                if (startLine - 1 >= 0) {
                    SearchHitState search, auto;
                    for (int i = startLine - 1; i >= 0; i--) {
                        m_dumpHitStateList.GetHistState(i, out search, out auto);
                        if (search == SearchHitState.Hit) {
                            return i;
                        }
                    }
                }
                return -1;
            }
        }

        //=========================================================================================
        // 機　能：新規にダンプでの自動検索を実行する
        // 引　数：[in]condition  検索条件
        // 　　　　[in]lineBytes  1行に表示するバイト数
        // 戻り値：なし
        //=========================================================================================
        public void SearchDumpAutoNew(DumpSearchCondition condition, int lineBytes) {
            // 検索ヒットフラグをクリア
            ClearSearchResult();
            m_statusBar.SetSearchHitCount(m_lineInfo.SearchHitCount, m_lineInfo.AutoSearchHitCount);

            // 検索を実行
            StopSearchThread();

            m_searchDumpThread = new DumpSearchThread(m_textFileViewer, m_dumpHitStateList);
            m_searchDumpThread.RequestSearch(condition, true, 0, lineBytes, false);
        }
        
        //=========================================================================================
        // 機　能：検索ヒットフラグをクリアする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearSearchResult() {
            for (int i = 0; i < m_lineInfo.LogicalLineCount; i++) {
                m_lineInfo.GetLineInfo(i).AutoSearchHitState = SearchHitState.Unknown;
                m_lineInfo.GetLineInfo(i).SearchHitState = SearchHitState.Unknown;
            }
            m_lineInfo.SetSearchHitCount(true, 0);
            m_lineInfo.SetSearchHitCount(false, 0);
            m_dumpHitStateList.ClearAll();
        }

        //=========================================================================================
        // プロパティ：ダンプビューアでの検索ヒット情報
        //=========================================================================================
        public DumpSearchHitStateList DumpSearchHitStateList {
            get {
                return m_dumpHitStateList;
            }
        }
    }
}
