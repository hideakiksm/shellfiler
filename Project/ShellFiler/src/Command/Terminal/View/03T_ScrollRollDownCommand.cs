using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Terminal;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.UI;

namespace ShellFiler.Command.Terminal.View {

    //=========================================================================================
    // クラス：コマンドを実行する
    // バックログを下に{0}ページ分だけスクロールします。
    //   書式 　 T_ScrollRollDown()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class T_ScrollRollDownCommand : TerminalActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public T_ScrollRollDownCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (!TerminalPanel.VisibleBackLog) {
                TerminalPanel.VisibleBackLog = true;
            }
            TerminalPanel.ScrollLog(false, TerminalPanel.TERMINAL_SCROLL_PAGE);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.T_ScrollRollDownCommand;
            }
        }
    }
}
