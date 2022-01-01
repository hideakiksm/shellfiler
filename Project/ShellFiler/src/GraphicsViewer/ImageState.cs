using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.FileViewer;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // 列挙子：イメージの状態
    //=========================================================================================
    public enum ImageState {
        Null,               // 初期状態
        Completed,          // 完了
        Loading,            // 読み込み中
    }
}
