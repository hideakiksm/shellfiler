using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.Document.Serialize.CommandApi {
        
    //=========================================================================================
    // クラス：コマンドの機能１つ
    //=========================================================================================
    public class XCommandApi {
        // コマンド名
        public string CommandName;

        // コマンドの説明
        public string Comment;

        // 引数一覧
        public List<XCommandArgument> ArgumentList = new List<XCommandArgument>();
    }
}
