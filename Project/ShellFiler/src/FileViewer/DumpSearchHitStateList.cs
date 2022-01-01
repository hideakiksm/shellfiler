using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.Locale;
using System.Runtime.InteropServices;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：検索にヒットしたかどうかの状態を行ごとのリストにしたもの
    //=========================================================================================
    public class DumpSearchHitStateList {
        // m_stateListの初期サイズ
        private const int INIT_STATE_LIST_SIZE = 4000;

        // 状態のリスト（３で割った上位がautoSearchの状態、下位がsearchの状態）
        private byte[] m_stateList;

        // 検索でヒットした件数
        private int m_searchHitCount;

        // 自動検索でヒットした件数
        private int m_autoHitCount;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]size  予測サイズ
        // 戻り値：なし
        //=========================================================================================
        public DumpSearchHitStateList() {
            m_stateList = new byte[0];
            m_autoHitCount = 0;
            m_searchHitCount = 0;
        }

        //=========================================================================================
        // 機　能：情報を初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearAll() {
            lock (this) {
                m_stateList = new byte[0];
                m_autoHitCount = 0;
                m_searchHitCount = 0;
            }
        }

        //=========================================================================================
        // 機　能：指定行に対する検索ヒット状態を返す
        // 引　数：[in]line     取得する行
        // 　　　　[out]search  検索ヒット状態を返す変数
        // 　　　　[out]auto    自動検索ヒット状態を返す変数
        // 戻り値：なし
        //=========================================================================================
        public void GetHistState(int line, out SearchHitState search, out SearchHitState auto) {
            lock (this) {
                if (m_stateList.Length <= line) {
                    search = SearchHitState.Unknown;
                    auto = SearchHitState.Unknown;
                } else {
                    search = (SearchHitState)(m_stateList[line] % 3);
                    auto = (SearchHitState)(m_stateList[line] / 3);
                }
            }
        }

        //=========================================================================================
        // 機　能：指定行に対する検索ヒット状態を設定する
        // 引　数：[in]line       取得する行
        // 　　　　[in]search     検索ヒット状態
        // 　　　　[in]searchMode 検索バイト列のときtrue、自動検索バイト列のときfalse
        // 戻り値：なし
        //=========================================================================================
        public void SetSearchHistState(int line, SearchHitState search, bool searchMode) {
            lock (this) {
                if (m_stateList.Length < line + 1) {
                    // 領域が足りないとき
                    int newSize = line + Math.Max(INIT_STATE_LIST_SIZE, m_stateList.Length);
                    byte[] newArray = new byte[newSize];
                    Array.Copy(m_stateList, newArray, m_stateList.Length);
                    Array.Clear(newArray, m_stateList.Length, newSize - m_stateList.Length);
                    m_stateList = newArray;
                    if (searchMode) {
                        m_stateList[line] = (byte)((int)SearchHitState.NotHit * 3 + (int)search);
                        m_searchHitCount++;
                    } else {
                        m_stateList[line] = (byte)((int)search * 3 + (int)SearchHitState.NotHit);
                        m_autoHitCount++;
                    }
                } else {
                    // 既存の領域に上書き
                    if (searchMode) {
                        SearchHitState prev = (SearchHitState)(m_stateList[line] % 3);
                        if (prev == SearchHitState.Hit) {
                            m_searchHitCount--;
                        }
                        m_stateList[line] = (byte)(m_stateList[line] / 3 * 3 + (int)search);
                        if (search == SearchHitState.Hit) {
                            m_searchHitCount++;
                        }
                    } else {
                        SearchHitState prev = (SearchHitState)(m_stateList[line] / 3);
                        if (prev == SearchHitState.Hit) {
                            m_autoHitCount--;
                        }
                        m_stateList[line] = (byte)((int)search * 3 + m_stateList[line] % 3);
                        if (search == SearchHitState.Hit) {
                            m_autoHitCount++;
                        }
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：指定行に対する検索ヒット状態を設定する
        // 引　数：[in]start      塗りつぶし開始行
        // 　　　　[in]end        塗りつぶし終了行
        // 　　　　[in]searchMode 検索バイト列のときtrue、自動検索バイト列のときfalse
        // 戻り値：なし
        //=========================================================================================
        public void FillSearchNotHit(int start, int end, bool searchMode) {
            lock (this) {
                if (m_stateList.Length <= end + 1) {
                    int newSize = end + 1;
                    byte[] newArray = new byte[newSize];
                    Array.Copy(m_stateList, newArray, m_stateList.Length);
                    Array.Clear(newArray, m_stateList.Length, newSize - m_stateList.Length);
                    m_stateList = newArray;
                }
                for (int i = start; i <= end; i++) {
                    SearchHitState searchState = (SearchHitState)(m_stateList[i] % 3);
                    SearchHitState autoState = (SearchHitState)(m_stateList[i] / 3);
                    if (searchMode) {
                        if (searchState == (int)(SearchHitState.Unknown)) {
                            m_stateList[i] = (byte)((int)autoState * 3 + (int)SearchHitState.NotHit);
                        }
                    } else {
                        if (autoState == (int)(SearchHitState.Unknown)) {
                            m_stateList[i] = (byte)((int)SearchHitState.NotHit * 3 + (int)autoState % 3);
                        }
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：レーダーバーの情報を設定する
        // 引　数：[in]lineCount      ビューアの行数
        // 　　　　[in]hitFlagSearch  検索結果ありのときtrueのフラグを書き込むビット配列
        // 　　　　[in]hitFlagAuto    自動検索結果ありのときtrueのフラグを書き込むビット配列
        // 戻り値：なし
        //=========================================================================================
        public void SetRadarBarInfo(int lineCount, BitArray hitFlagSearch, BitArray hitFlagAuto) {
            lock (this) {
                int screenSize = hitFlagSearch.Count;
                for (int i = 0; i < lineCount; i++) {
                    if (i < m_stateList.Length) {
                        if ((SearchHitState)(m_stateList[i] % 3) == SearchHitState.Hit) {
                            int pos = (int)((double)screenSize * (double)i / (double)lineCount);
                            hitFlagSearch[pos] = true;
                        }
                        if ((SearchHitState)(m_stateList[i] / 3) == SearchHitState.Hit) {
                            int pos = (int)((double)screenSize * (double)i / (double)lineCount);
                            hitFlagAuto[pos] = true;
                        }
                    }
                }
            }
        }

        //=========================================================================================
        // プロパティ：検索でヒットした件数
        //=========================================================================================
        public int SearchHitCount {
            get {
                return m_searchHitCount;
            }
        }

        //=========================================================================================
        // プロパティ：自動検索でヒットした件数
        //=========================================================================================
        public int AutoHitCount {
            get {
                return m_autoHitCount;
            }
        }
    }
}
