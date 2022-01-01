using System;
using System.Reflection;
using ShellFiler.Api;
using ShellFiler.FileViewer;

namespace ShellFiler.Command {
    
    //=========================================================================================
    // クラス：ActionCommandのオプション
    //=========================================================================================
    public enum ActionCommandOption {
        // 何もしない
        None = 0,
        // 実行後、次の行に移動
        MoveNext = 0x0001,
    }
}
