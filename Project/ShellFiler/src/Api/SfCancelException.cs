using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Properties;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：アプリケーションとしての例外（キャンセル）
    //=========================================================================================
    public class SfCancelException : SfException {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SfCancelException() : base(SfException.Canceled) {
        }
    }
}
