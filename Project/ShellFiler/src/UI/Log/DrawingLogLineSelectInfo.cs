using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;

namespace ShellFiler.UI.Log {

    //=========================================================================================
    // クラス：ログの各行を描画するときの情報
    //=========================================================================================
    public class DrawingLogLineContext {
        // 選択開始カラム（半角文字単位、全選択のとき0、選択がないとき-1）
        private int m_selectionStart;

        // 選択終了カラムの次の位置（半角文字単位、全選択のときint.MaxValue、選択がないとき-1）
        private int m_selectionEnd;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]selectionStart   選択開始カラム（半角文字単位、全選択のとき0、選択がないとき-1）
        // 　　　　[in]selectionEnd     選択終了カラムの次の位置（半角文字単位、全選択のときint.MaxValue、選択がないとき-1）
        // 戻り値：なし
        //=========================================================================================
        public DrawingLogLineContext(int selectionStart, int selectionEnd) {
            m_selectionStart = selectionStart;
            m_selectionEnd = selectionEnd;
        }

        //=========================================================================================
        // プロパティ：選択開始カラム（半角文字単位、全選択のとき0、選択がないとき-1）
        //=========================================================================================
        public int SelectionStart {
            get {
                return m_selectionStart;
            }
        }

        //=========================================================================================
        // プロパティ：選択終了カラムの次の位置（半角文字単位、全選択のときint.MaxValue、選択がないとき-1）
        //=========================================================================================
        public int SelectionEnd {
            get {
                return m_selectionEnd;
            }
        }
    }
}
