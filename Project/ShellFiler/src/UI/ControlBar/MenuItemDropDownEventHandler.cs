using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // 機　能：メニュー項目が開かれたことを通知するイベント
    // 引　数：[in]sender   イベントの送信元
    // 　　　　[in]evt      送信イベント
    // 戻り値：なし
    //=========================================================================================
    public delegate void MenuItemDropDownEventHandler(object sender, EventArgs evt); 
}
