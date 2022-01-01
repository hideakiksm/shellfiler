using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Command;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // 列挙子：変更の対象とするシーン
    //=========================================================================================
    public enum CommandUsingSceneType {
        FileList,               // ファイル一覧
        FileViewer,             // ファイルビューア
        GraphicsViewer,         // グラフィックビューア
        MonitoringViewer,       // モニタリングビューア
        Terminal,               // ターミナル
    }
}
