using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Util;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // 列挙子：テキストビューアでの検索モード
    //=========================================================================================
    public enum TextSearchMode {
        // 通常：大小文字を同一視
        NormalIgnoreCase,

        // 通常：大小文字を区別
        NormalCaseSensitive,

        // ワイルドカード：大小文字を同一視
        WildCardIgnoreCase,

        // ワイルドカード：大小文字を区別
        WildCardCaseSensitive,

        // 正規表現
        RegularExpression,
    }
}
