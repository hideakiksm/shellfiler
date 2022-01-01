using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：Win32APIのRECT構造体
    //=========================================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(Rectangle rect) {
            this.Bottom = rect.Bottom;
            this.Left = rect.Left;
            this.Right = rect.Right;
            this.Top = rect.Top;
        }

        public RECT(int left, int top, int right, int bottom) {
            this.Bottom = bottom;
            this.Left = left;
            this.Right = right;
            this.Top = top;
        }
    }
}
