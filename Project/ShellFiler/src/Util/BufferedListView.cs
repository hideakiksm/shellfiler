using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ShellFiler.Api;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：ダブルバッファリングされたリストビュー
    //=========================================================================================
    public class BufferedListView : System.Windows.Forms.ListView {

        protected override bool DoubleBuffered {
            get {
                return true;
            }
            set {
            }
        }
    }
}
