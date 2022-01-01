using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Util;

namespace ShellFiler.Api {

    //=========================================================================================
    // インターフェース：２ストロークキーの入力に対応したウィンドウ
    //=========================================================================================
    public interface ITwoStrokeKeyForm {

        //=========================================================================================
        // 機　能：2ストロークキーの状態が変わったことを通知する
        // 引　数：[in]newState  新しい状態
        // 戻り値：なし
        //=========================================================================================
        void TwoStrokeKeyStateChanged(TwoStrokeType newState);
    }
}
