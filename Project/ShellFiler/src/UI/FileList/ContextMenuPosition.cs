using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileSystem;

namespace ShellFiler.UI.FileList {

    //=========================================================================================
    // クラス：コンテキストメニューの表示位置
    //=========================================================================================
    public enum ContextMenuPosition {
        OnFile,             // ファイル一覧のカーソル上
        FileListTop,        // ファイル一覧の一番上
        OnMouse,            // マウスカーソル上
    }
}
