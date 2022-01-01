using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Document;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：コマンドの戻り値
    //=========================================================================================
    public enum CommandRetType {
        Void,
        Bool,
        String,
        Int,
    }
}
