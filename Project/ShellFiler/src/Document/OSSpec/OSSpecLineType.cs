using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;

namespace ShellFiler.Document.OSSpec {

    //=========================================================================================
    // 列挙子：サーバーコマンドの結果の期待値 各行の種類
    //=========================================================================================
    public enum OSSpecLineType {
        None = 0,               // null状態
        OrPrev = 1,             // |(パイプ)  直前の行に対するOR
        Repeat = 2,             // +          この行の１回以上の繰り返し
        Comment = 4,            // c          コメント行
        ErrorLine = 8,          // e          エラー行（この行が認識されたらエラー）
    }
}
