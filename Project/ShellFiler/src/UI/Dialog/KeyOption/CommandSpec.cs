using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：コマンドの定義一覧
    //=========================================================================================
    public class CommandSpec {
        // ファイル一覧のコマンド一覧
        public CommandScene FileList = new CommandScene();

        // ファイルビューアのコマンド一覧
        public CommandScene FileViewer = new CommandScene();

        // グラフィックビューアのコマンド一覧
        public CommandScene GraphicsViewer = new CommandScene();
    }
}
