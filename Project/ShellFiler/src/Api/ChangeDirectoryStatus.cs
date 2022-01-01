using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.Api {

    //=========================================================================================
    // 列挙子：ディレクトリ変更時のステータス
    //=========================================================================================
    public enum ChangeDirectoryStatus {
        Success,                // 成功
        Failed,                 // 失敗
        Loading,                // 読み込み中
        ArcNotInstalled,        // アーカイブの処理プログラムがインストールされていない
    }
}
