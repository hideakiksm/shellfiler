using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.FileSystem;

namespace ShellFiler.UI.Log {

    //=========================================================================================
    // 列挙子：ログの出力色
    //=========================================================================================
    public enum LogColor {
        Normal,         // 通常色（黒）
        Error,          // エラー色（赤）
        StdError,       // 標準エラー出力色（青）
    }
}
