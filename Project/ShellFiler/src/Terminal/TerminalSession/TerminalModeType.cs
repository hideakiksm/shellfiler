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
    // 列挙子：ターミナルのモード
    // キー入力のエスケープシーケンスを変更させる目的のよう（poderosaより）
    //=========================================================================================
    public enum TerminalModeType {
        Normal,
        Application,
    }
}
