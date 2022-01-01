using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // 列挙子：読み込みのステータス
    //=========================================================================================
    public enum FileListLoadingStatus {
        Initial,                // 読み込み前、Completed/Failedに遷移予定
        InitialExpectLoading,   // 読み込み前、Loadingに遷移予定
        Loading,                // 読み込み中
        Completed,              // 読み込み完了
        Failed,                 // 読み込みに失敗
        // 途中まで読み込んだ状態はない。
        // 読み込み中は常に0件、読み込み完了は常に全件ありの状態になる。
    }
}
