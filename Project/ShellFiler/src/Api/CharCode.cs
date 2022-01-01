using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：文字コードの定義
    //=========================================================================================
    public class CharCode {
        public const char CH_ESC = (char)0x1b;              // ESC
        public const char CH_CR = (char)0x0d;               // CR
        public const char CH_LF = (char)0x0a;               // LF
        public const char CH_TAB = (char)0x09;              // TAB

        public const byte BY_ESC = (byte)0x1b;              // ESC
        public const byte BY_CR = (byte)0x0d;               // CR
        public const byte BY_LF = (byte)0x0a;               // LF
        public const byte BY_TAB = (byte)0x09;              // TAB

        public const string STR_CRLF = "\r\n";              // CR LF
        public const string STR_LF = "\n";                  // LF
    }
}
