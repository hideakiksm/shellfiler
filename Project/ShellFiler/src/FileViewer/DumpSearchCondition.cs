using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Util;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：ダンプビューアでの検索条件
    //=========================================================================================
    public class DumpSearchCondition {
        // 検索キーワードの最大文字数
        public const int MAX_SEARCH_BYTES_LENGTH = 64;

        // 検索文字バイト列（検索しないときnull）
        private byte[] m_searchBytes;
        
        // 自動検索文字バイト列（検索しないときnull）
        private byte[] m_autoSearchBytes;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DumpSearchCondition() {
            m_searchBytes = null;
            m_autoSearchBytes = null;
        }
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]src  コピー元
        // 戻り値：なし
        //=========================================================================================
        public DumpSearchCondition(DumpSearchCondition src) {
            m_searchBytes = src.m_searchBytes;
            m_autoSearchBytes = src.m_autoSearchBytes;
        }
        
        //=========================================================================================
        // 機　能：検索語句を最大長でトリミングする
        // 引　数：[in]data      元のデータ
        // 　　　　[out]trimmed  トリミングしたときtrueを返す変数
        // 戻り値：なし
        //=========================================================================================
        public static byte[] TrimBySearchLength(byte[] data, out bool trimmed) {
            if (data == null) {
                trimmed = false;
                return data;
            } else if (data.Length > MAX_SEARCH_BYTES_LENGTH) {
                trimmed = true;
                byte[] trimData = new byte[MAX_SEARCH_BYTES_LENGTH];
                Array.Copy(data, trimData, trimData.Length);
                return trimData;
            } else {
                trimmed = false;
                return data;
            }
        }

        //=========================================================================================
        // プロパティ：検索文字バイト列（検索しないときnull）
        //=========================================================================================
        public byte[] SearchBytes {
            get {
                return m_searchBytes;
            }
            set {
                m_searchBytes = value;
            }
        }

        //=========================================================================================
        // プロパティ：自動検索文字バイト列（検索しないときnull）
        //=========================================================================================
        public byte[] AutoSearchBytes {
            get {
                return m_autoSearchBytes;
            }
            set {
                m_autoSearchBytes = value;
            }
        }
    }
}
