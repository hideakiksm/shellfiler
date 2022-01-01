using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.Document.Serialize.CommandApi {
        
    //=========================================================================================
    // クラス：用途別のコマンド一覧
    //=========================================================================================
    public class XCommandScene {
        // コマンドのグループ
        public List<XCommandGroup> CommandGroup = new List<XCommandGroup>();
    }
}
