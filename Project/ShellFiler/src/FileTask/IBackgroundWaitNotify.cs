using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Util;
using ShellFiler.UI.Dialog;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：バックグラウンドタスク実行中のコールバック
    //=========================================================================================
    public interface IBackgroundWaitNotify {

        //=========================================================================================
        // 機　能：進捗率を更新する
        // 引　数：[in]ratio     進捗率（0～100）
        // 　　　　[in]filePath  展開中のファイルパス
        // 戻り値：なし
        // メ　モ：delegateによりUIスレッドで実行する
        //=========================================================================================
        void UpdateProgress(int ratio, string filePath);

        //=========================================================================================
        // 機　能：展開処理が終了したときの処理を行う
        // 引　数：[in]status  展開処理のステータス
        // 戻り値：なし
        // メ　モ：delegateによりUIスレッドで実行する
        //=========================================================================================
        void ExtractCompleted(FileOperationStatus status);
    }
}
