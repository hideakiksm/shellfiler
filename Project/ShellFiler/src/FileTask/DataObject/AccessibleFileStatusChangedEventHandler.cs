using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShellFiler.FileTask.DataObject {

    //=========================================================================================
    // 機　能：ファイルの状態が変わったことを通知するイベント
    // 引　数：[in]sender   イベントの送信元
    // 　　　　[in]evt      送信イベント
    // 戻り値：なし
    //=========================================================================================
    public delegate void AccessibleFileStatusChangedEventHandler(object sender, AccessibleFileStatusChangedEventArgs evt); 
}
