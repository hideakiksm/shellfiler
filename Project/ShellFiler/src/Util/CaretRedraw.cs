using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Util;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：カレットの再描画中消去ユーティリティ
    //=========================================================================================
    public class CaretRedraw {
        // 対象のカレット（処理不要のときnull）
        private Win32Caret m_caret = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]caret  対象のカレット（処理不要のときnull）
        // 戻り値：なし
        //=========================================================================================
        public CaretRedraw(Win32Caret caret) {
            if (caret != null && caret.Visible) {
                m_caret = caret;
                m_caret.Visible = false;
            }
        }

        //=========================================================================================
        // 機　能：カレットを元に戻す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Resume() {
            if (m_caret != null) {
                m_caret.Visible = true;
            }
        }
    }
}
