using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;

namespace ShellFiler.UI.Log {

    //=========================================================================================
    // クラス：ステータスバー用に選択状態が変わったときの通知
    //=========================================================================================
    public enum LogViewContainerStatusType {
        StartSelection,             // 選択が開始された（引数：null）
        SelectionChange,            // 選択が変更された（引数：選択範囲）
        CancelSelection,            // 選択がキャンセルされた（引数：null）
    }
}
