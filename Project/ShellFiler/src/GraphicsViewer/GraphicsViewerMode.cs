using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // 列挙子：グラフィックビューアの起動モード
    //=========================================================================================
    public enum GraphicsViewerMode {
        GraphicsViewer,         // グラフィックビューア    
        SlideShow,              // スライドショー
        ClipboardViewer,        // クリップボードビューア
    }
}
