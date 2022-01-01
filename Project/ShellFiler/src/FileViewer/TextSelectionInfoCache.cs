using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Command.FileViewer;
using ShellFiler.Util;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：テキスト状態で選択した位置のキャッシュ
    //=========================================================================================
    public class TextSelectionInfoCache {
        // 開始物理行（キャッシュしていないとき-1）
        private int m_startPhysicalLine = -1;

        // 開始桁位置（キャッシュしていないとき-1）
        private int m_startColumn = -1;

        // 開始桁位置のキャッシュ結果（キャッシュしていないとき-1）
        private int m_startResultColumn = -1;

        // 終了物理行（キャッシュしていないとき-1）
        private int m_endPhysicalLine = -1;

        // 終了桁位置（キャッシュしていないとき-1）
        private int m_endColumn = -1;

        // 終了桁位置のキャッシュ（キャッシュしていないとき-1）
        private int m_endResultColumn = -1;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TextSelectionInfoCache() {
        }

        //=========================================================================================
        // 機　能：結果を設定する
        // 引　数：[in]startPhysicalLine  開始物理行のキャッシュ
        // 　　　　[in]startColumn        開始桁位置のキャッシュ
        // 　　　　[in]startResultColumn  開始桁位置のキャッシュ結果（キャッシュしていないとき-1）
        // 　　　　[in]endPhysicalLine    終了物理行のキャッシュ
        // 　　　　[in]endColumn          終了桁位置のキャッシュ
        // 　　　　[in]endResultColumn    終了桁位置のキャッシュ（キャッシュしていないとき-1）
        // 戻り値：なし
        //=========================================================================================
        public void SetResult(int startPhysicalLine, int startColumn, int startResultColumn, int endPhysicalLine, int endColumn, int endResultColumn) {
            m_startPhysicalLine = startPhysicalLine;
            m_startColumn = startColumn;
            m_startResultColumn = startResultColumn;
            m_endPhysicalLine = endPhysicalLine;
            m_endColumn = endColumn;
            m_endResultColumn = endResultColumn;
        }

        //=========================================================================================
        // 機　能：キャッシュをクリアする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetCache() {
            m_startPhysicalLine = -1;
            m_startResultColumn = -1;
            m_startColumn = -1;
            m_endPhysicalLine = -1;
            m_endColumn = -1;
            m_endResultColumn = -1;
        }

        //=========================================================================================
        // プロパティ：開始物理行（キャッシュしていないとき-1）
        //=========================================================================================
        public int StartPhysicalLine {
            get {
                return m_startPhysicalLine;
            }
        }

        //=========================================================================================
        // プロパティ：開始桁位置（キャッシュしていないとき-1）
        //=========================================================================================
        public int StartColumn {
            get {
                return m_startColumn;
            }
        }
        
        //=========================================================================================
        // プロパティ：開始桁位置のキャッシュ結果（キャッシュしていないとき-1）
        //=========================================================================================
        public int StartResultColumn {
            get {
                return m_startResultColumn;
            }
        }

        //=========================================================================================
        // プロパティ：終了物理行（キャッシュしていないとき-1）
        //=========================================================================================
        public int EndPhysicalLine {
            get {
                return m_endPhysicalLine;
            }
        }

        //=========================================================================================
        // プロパティ：終了桁位置（キャッシュしていないとき-1）
        //=========================================================================================
        public int EndColumn {
            get {
                return m_endColumn;
            }
        }

        //=========================================================================================
        // プロパティ：終了桁位置のキャッシュ（キャッシュしていないとき-1）
        //=========================================================================================
        public int EndResultColumn {
            get {
                return m_endResultColumn;
            }
        }
    }
}
