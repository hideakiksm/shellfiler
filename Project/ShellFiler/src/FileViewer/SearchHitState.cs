using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.Locale;
using System.Runtime.InteropServices;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：検索にヒットしたかどうかの状態
    //=========================================================================================
    public enum SearchHitState {
        // 不明
        Unknown = 0,

        // ヒットした
        Hit = 1,

        // ヒットしなかった
        NotHit = 2,
    }
}
