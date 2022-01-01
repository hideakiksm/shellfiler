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
    // クラス：ファイルビューアで選択した範囲の比較用バッファ
    //=========================================================================================
    public class FileViewerSelectionCompareBuffer {
        // 左側に表示するテキスト（未登録のときnull）
        private string m_leftString = null;
        
        // 左側に表示するテキストの開始行番号（未登録のとき-1）
        private int m_leftStartLineNum = -1;

        // 右側に表示するテキスト（未登録のときnull）
        private string m_rightString = null;

        // 右側に表示するテキストの開始行番号（未登録のとき-1）
        private int m_rightStartLineNum = -1;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileViewerSelectionCompareBuffer() {
        }

        //=========================================================================================
        // プロパティ：左側に表示するテキスト（未登録のときnull）
        //=========================================================================================
        public string LeftString {
            get {
                return m_leftString;
            }
            set {
                m_leftString = value;
            }
        }

        //=========================================================================================
        // プロパティ：左側に表示するテキストの開始行番号（未登録のとき-1）
        //=========================================================================================
        public int LeftStartLineNum {
            get {
                return m_leftStartLineNum;
            }
            set {
                m_leftStartLineNum = value;
            }
        }

        //=========================================================================================
        // プロパティ：右側に表示するテキスト（未登録のときnull）
        //=========================================================================================
        public string RightString {
            get {
                return m_rightString;
            }
            set {
                m_rightString = value;
            }
        }

        //=========================================================================================
        // プロパティ：右側に表示するテキストの開始行番号（未登録のとき-1）
        //=========================================================================================
        public int RightStartLineNum {
            get {
                return m_rightStartLineNum;
            }
            set {
                m_rightStartLineNum = value;
            }
        }
    }
}
