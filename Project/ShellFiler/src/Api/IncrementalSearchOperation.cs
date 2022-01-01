using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // 列挙子：インクリメンタルサーチの操作
    //=========================================================================================
    public enum IncrementalSearchOperation {
        FromTop,            // 先頭から検索する
        MoveUp,             // 上に移動する
        MoveDown,           // 下に移動する
        Mark,               // マークする
    }
}
