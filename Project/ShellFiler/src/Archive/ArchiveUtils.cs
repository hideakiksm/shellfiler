using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：圧縮／展開のユーティリティ
    //=========================================================================================
    public class ArchiveUtils {

        //=========================================================================================
        // 機　能：展開非サポート時のログを表示する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public static void ShowExtractError() {
            LogLineSimple log1 = new LogLineSimple(LogColor.Error, Resources.Log_NotAvailableExtract1, OSUtils.GetCurrentProcessBits());
            Program.LogWindow.RegistLogLine(log1);
            LogLineSimple log2 = new LogLineSimple(LogColor.Error, Resources.Log_NotAvailableExtract2);
            Program.LogWindow.RegistLogLine(log2);
        }
    }
}
