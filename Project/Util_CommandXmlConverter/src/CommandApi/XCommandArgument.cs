using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.Document.Serialize.CommandApi {

    //=========================================================================================
    // クラス：引数の定義
    //=========================================================================================
    public class XCommandArgument {
        // 仮引数名
        public string ArgumentName;

        // 引数の型
        public string ArgumentType;

        // 引数の説明
        public string ArgumentComment;

        // 引数のデフォルト値
        public string DefaultValue;

        // 引数の範囲
        public string ValueRange;
    }
}
