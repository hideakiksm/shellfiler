using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Util;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：テキストファイルの論理行１行分の情報
    //=========================================================================================
    public class TextBufferLogicalLineInfo {
        // 元ファイルのバッファ中の先頭インデックス（物理行単位）
        private int m_bufferIndex;
        
        // テキスト行バッファ（string変換済み）
        private string m_strLine;

        // 検索にヒットしたかどうかの状態
        private SearchHitState m_searchHitState = SearchHitState.Unknown;

        // 自動検索にヒットしたかどうかの状態
        private SearchHitState m_autoSearchHitFlag = SearchHitState.Unknown;

        // 改行コードまたは改行の有無
        private LineBreakChar m_lineBreakChar;

        // テキスト行の物理行番号
        private int m_physicalLineNo;

        // 検索ヒットの情報

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]bufferIndex      元ファイルのバッファ中の先頭インデックス
        // 　　　　[in]strLine          テキスト行バッファ（string変換済み）
        // 　　　　[in]lineBreakChar    改行コードまたは改行の有無
        // 　　　　[in]physicalLineNo   テキスト行の物理行番号
        // 戻り値：なし
        //=========================================================================================
        public TextBufferLogicalLineInfo(int bufferIndex, string strLine, LineBreakChar lineBreakChar, int physicalLineNo) {
            m_bufferIndex = bufferIndex;
            m_strLine = strLine;
            m_lineBreakChar = lineBreakChar;
            m_physicalLineNo = physicalLineNo;
        }

        //=========================================================================================
        // プロパティ：元ファイルのバッファ中の先頭インデックス
        //=========================================================================================
        public int BufferIndex {
            get {
                return m_bufferIndex;
            }
        }

        //=========================================================================================
        // プロパティ：テキスト行バッファ（string変換済み、タブコード変換前）
        //=========================================================================================
        public string StrLineOrg {
            get {
                return m_strLine;
            }
        }

        //=========================================================================================
        // プロパティ：検索にヒットしたときtrue
        //=========================================================================================
        public SearchHitState SearchHitState {
            get {
                return m_searchHitState;
            }
            set {
                m_searchHitState = value;
            }
        }

        //=========================================================================================
        // プロパティ：自動検索にヒットしたときtrue
        //=========================================================================================
        public SearchHitState AutoSearchHitState {
            get {
                return m_autoSearchHitFlag;
            }
            set {
                m_autoSearchHitFlag = value;
            }
        }

        //=========================================================================================
        // プロパティ：改行コードまたは改行の有無
        //=========================================================================================
        public LineBreakChar LineBreakChar {
            get {
                return m_lineBreakChar;
            }
        }

        //=========================================================================================
        // プロパティ：テキスト行の物理行番号
        //=========================================================================================
        public int PhysicalLineNo {
            get {
                return m_physicalLineNo;
            }
        }
    }
}
