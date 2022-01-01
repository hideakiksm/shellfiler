using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.Util;
using ShellFiler.FileTask;

namespace ShellFiler.FileTask.Management {
 
    //=========================================================================================
    // 列挙子：タスクの実行種別
    //=========================================================================================
    public enum BackgroundTaskRunningType {
        Waitable,           // 待機可能
        Waiting,            // 待機中
        WaitingOver,        // 待機中個数オーバー
        Limited,            // 待機不可能
        LimitedOver,        // 待機不可能個数オーバー
        Unlimited,          // 多重実行可能
        None,               // 状態なし（終了など）
    }
}
