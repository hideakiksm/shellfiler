using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // 列挙子：キャンセルした理由
    //=========================================================================================
    public enum CancelReason {
        None,           // 継続
        User,           // ユーザーによるキャンセル
        Connection,     // 接続エラーによるキャンセル
        DiskFull,       // ディスクフルによるキャンセル
    }
}
