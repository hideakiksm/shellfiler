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
using ShellFiler.Terminal.UI;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession {

    //=========================================================================================
    // クラス：ターミナルログの内容変化のリスト
    //=========================================================================================
    public class LogLineChangeLog : ChangeLog<LogLineTerminal> {
        // BEEP音を鳴らすときtrue
        private bool m_requestBeep = false;

        // 全データのリフレッシュが適切なときtrue
        private bool m_refreshAll = false;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public LogLineChangeLog() {
        }

        //=========================================================================================
        // プロパティ：BEEP音を鳴らすときtrue
        //=========================================================================================
        public bool RequestBeep {
            get {
                return m_requestBeep;
            }
            set {
                m_requestBeep = value;
            }
        }

        //=========================================================================================
        // プロパティ：全データのリフレッシュが適切なときtrue
        //=========================================================================================
        public bool RefreshAll {
            get {
                return m_refreshAll;
            }
            set {
                m_refreshAll = value;
            }
        }
    }
}
