using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.Document.Serialize.CommandApi {

    //=========================================================================================
    // クラス：コマンドの定義一覧
    //=========================================================================================
    public class XCommandSpec {
        // ファイル一覧のコマンド一覧
        public XCommandScene FileList = new XCommandScene();

        // ファイルビューアのコマンド一覧
        public XCommandScene FileViewer = new XCommandScene();

        // グラフィックビューアのコマンド一覧
        public XCommandScene GraphicsViewer = new XCommandScene();
    }
}
