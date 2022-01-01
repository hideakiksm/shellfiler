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
    // 列挙子：改行文字
    //=========================================================================================
    public enum LineBreakChar {
        CrLf,               // CR LFで改行
        Cr,                 // CRで改行
        Eof,                // EOF
        EofContinue,        // ...
        None,               // 改行しない
    }
}
