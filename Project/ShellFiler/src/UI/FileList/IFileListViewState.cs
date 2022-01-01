using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Command.FileList;
using ShellFiler.FileSystem;

namespace ShellFiler.UI.FileList {

    //=========================================================================================
    // インターフェース：ファイル一覧の状態を記憶しておくためのインターフェース
    // 　　　　　　　　　インターフェース名だけを定義し、詳細は実装クラスが決める。
    //=========================================================================================
    public interface IFileListViewState : ICloneable {
    }
}
