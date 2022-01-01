using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using ShellFiler.Util;

namespace ShellFiler.Locale {

    //=========================================================================================
    // クラス：ファイルビューアでの文字の種類
    //=========================================================================================
    public enum ViewerCharType {
        // 半角英数字
        HalfAlphaNum,
        // 半角記号
        HalfSymbol,
        // 全角文字
        FullChar,
        // 全角記号
        FullSymbol,
        // 空白類
        Space,
        // 0x00
        Zero,
        // その他バイナリ
        Binary,
    }
}
