using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Properties;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.UI;
using ShellFiler.Util;
using ShellFiler.UI.Dialog;
using ShellFiler.Locale;

namespace ShellFiler.UI {

    //=========================================================================================
    // クラス：ウィンドウの管理クラス
    //=========================================================================================
    public class WindowManager {
        // ビューアの一覧
        private List<Form> m_fileViewerList = new List<Form>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public WindowManager() {
        }

        //=========================================================================================
        // 機　能：ビューアが開かれたときの処理を行う
        // 引　数：[in]form        開かれたフォーム
        // 戻り値：なし
        //=========================================================================================
        public void OnOpenViewer(Form form) {
            m_fileViewerList.Add(form);
        }

        //=========================================================================================
        // 機　能：ビューアが閉じられたときの処理を行う
        // 引　数：[in]form   開かれたフォーム
        // 戻り値：なし
        //=========================================================================================
        public void OnCloseViewer(Form form) {
            m_fileViewerList.Remove(form);
        }

        //=========================================================================================
        // 機　能：指定されたユーザーとサーバーに対するターミナルウィンドウを返す
        // 引　数：[in]console  検索対象のコンソール（null可、nullの場合はメソッドからnullを返す）
        // 戻り値：ターミナルウィンドウ(対応するウィンドウがなかったときnull)
        //=========================================================================================
        public TerminalForm GetTerminalForm(ConsoleScreen console) {
            if (console == null) {
                return null;
            }
            for (int i = 0; i < m_fileViewerList.Count; i++) {
                if (m_fileViewerList[i] is TerminalForm) {
                    TerminalForm terminal = (TerminalForm)(m_fileViewerList[i]);
                    if (terminal.ConsoleScreen == console) {
                        return terminal;
                    }
                }
            }
            return null;
        }

        //=========================================================================================
        // プロパティ：ファイルビューアの一覧
        //=========================================================================================
        public List<Form> FileViewerList {
            get {
                return m_fileViewerList;
            }
        }
    }
}
