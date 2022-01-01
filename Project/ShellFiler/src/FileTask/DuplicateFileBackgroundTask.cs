using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.Management;
using ShellFiler.FileTask.Provider;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイルコピーをバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class DuplicateFileBackgroundTask : CopyBackgroundTask {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider  転送元ファイルの情報
        // 　　　　[in]destProvider 転送先ファイルの情報
        // 　　　　[in]refreshUi    作業完了時のUI更新方法
        // 　　　　[in]option       コピーと移動のオプション
        // 戻り値：なし
        //=========================================================================================
        public DuplicateFileBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, CopyMoveDeleteOption option) : base(srcProvider, destProvider, refreshUi, option, null) {
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.DuplicateFile;
            }
        }
    }
}
