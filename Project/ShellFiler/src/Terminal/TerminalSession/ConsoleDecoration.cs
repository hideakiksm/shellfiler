using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Text;
using ShellFiler.Api;

namespace ShellFiler.Terminal.TerminalSession {
    
    //=========================================================================================
    // クラス：コンソール1行分の文字装飾情報
    //=========================================================================================
    public class ConsoleDecoration {
        public const short NORMAL     = 0;
        public const short COLOR_MASK = 0x000f;             // 0000_0000_0000_1111  文字の色
        public const short COLOR_NOT  = 0x7ff0;             // 0111_1111_1111_0000  文字の色(NOT)
        public const short BACK_MASK  = 0x00f0;             // 0000_0000_1111_0000  背景色
        public const short BACK_NOT   = 0x7f0f;             // 0111_1111_0000_1111  背景色(NOT)
        public const short UNDER_LINE = 0x0100;             // 0000_0001_0000_0000  下線
        public const short BLINK      = 0x0200;             // 0000_0010_0000_0000  点滅（無視）
        public const short REVERSE    = 0x0400;             // 0000_0100_0000_0000  リバース
        public const short HIDE       = 0x0800;             // 0000_1000_0000_0000  非表示
        public const short BOLD       = 0x1000;             // 0001_0000_0000_0000  強調
        public const short VERTIVAL   = 0x2000;             // 0010_0000_0000_0000  垂線（無視）

        // 文字の色
        public const short COLOR_DEF_FORE   = 0x0000;       // デフォルト
        public const short COLOR_DEF_BACK   = 0x0009;       // デフォルト
        public const short COLOR_BLACK      = 0x0001;       // 30 黒
        public const short COLOR_RED        = 0x0002;       // 17, 31 赤
        public const short COLOR_BLUE       = 0x0003;       // 18, 34 青
        public const short COLOR_MAGENTA    = 0x0004;       // 19, 35 紫
        public const short COLOR_GREEN      = 0x0005;       // 20, 32 緑
        public const short COLOR_CYAN       = 0x0006;       // 22, 36 水
        public const short COLOR_YELLOW     = 0x0007;       // 21, 33 黄
        public const short COLOR_WHITE      = 0x0008;       // 23, 37 白

        // 文字の背景色
        public const short BACK_DEFAULT     = 0x0000;       // デフォルト
        public const short BACK_BLACK       = 0x0001;       // 40 黒地
        public const short BACK_RED         = 0x0002;       // 41 赤地
        public const short BACK_BLUE        = 0x0003;       // 44 青地
        public const short BACK_MAGENTA     = 0x0004;       // 45 紫地
        public const short BACK_GREEN       = 0x0005;       // 42 緑地
        public const short BACK_CYAN        = 0x0006;       // 46 水地
        public const short BACK_YELLOW      = 0x0007;       // 43 黄地
        public const short BACK_WHITE       = 0x0008;       // 47 白地
        public const short BACK_SHIFT = 4;                  // 背景色へのシフト量
    }
}
