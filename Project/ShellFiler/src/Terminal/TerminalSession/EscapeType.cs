using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Locale;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession {

    //=========================================================================================
    // 列挙子：エスケープシーケンスのコマンド種別
    //=========================================================================================
    public enum EscapeType {
        Nop,                    // 何もしない
        SetColor,               // 色をつける, ESC[色番号m 
        MoveCursor,             // カーソルをY行目のX桁目に移動する。, ESC[Y;XH
        DeleteBelow,            // カーソル行から下へpn行削除する, ESC[pnM
        DeleteUpper,            // カーソル行から上へpn行削除する, ESC[pnL
        CursorDown,             // カーソルをカラム位置はそのままに１行下に移動する。(カーソルが最下行にある場合は１行スクロールする) , ESCD
        CursorNextLeft,         // カーソルを１行下の一番左に移動する。(カーソルが最下行にある場合は１行スクロールする), ESCE
        CursorUp,               // カーソルをカラム位置はそのままに１行上に移動する。(カーソルが最上行にある場合は機種依存) , ESCM
        CursorLeft,             // カーソルを１つ左に移動する。 , ESC[D
        CursorRight,            // カーソルを１つ右に移動する。 , ESC[C
        InsertLine,             // カーソル行に1行追加する, ESC[L
        InsertLineWith,         // カーソル行にX行追加する, ESC[XL
        CursorUpWith,           // カーソルをY行上へ移動する。, ESC[YA
        CursorDownWith,         // カーソルをY行下へ移動する。, ESC[YB
        CursorRightWith,        // カーソルをX桁右へ移動する。行の右端より先には移動しない。 （次の行へは移動しない）, ESC[XC
        CursorLeftWith,         // カーソルをX桁左へ移動する。行の左端より先には移動しない。 （前の行へは移動しない）, ESC[XD
        CursorHome,             // カーソルを左上に移動する。, ESC[H
        RegistPosition,         // 位置記憶, ESC[s
        RestorePosition,        // 位置移動, ESC[u
        ClearAllNext,           // カーソル位置から最終行の右端まで削除する, ESC[0J
        ClearAllPrev,           // 先頭行の左端からカーソル位置まで削除する, ESC[1J
        ClearScreen,            // 画面全体を消去し、カーソルを左上に移動する, ESC*
        ClearLineNext,          // カーソル位置から同一行の右端まで削除する, ESC[0K
        ClearLinePrev,          // 同一行の左端からカーソル位置まで削除する, ESC[1K
        ClearLine,              // カーソルのある行全て削除する, ESC[2K 
        SetScrollRange,         // スクロール範囲をS行目からE行目までに設定する, ESC[S;Er
        TerminalModeApplication,// ターミナルモードをApplicationへ（poderosaより）, ESC =
        TerminalModeNormal,     // ターミナルモードをNormalへ（poderosaより）, ESC >
    }
}
