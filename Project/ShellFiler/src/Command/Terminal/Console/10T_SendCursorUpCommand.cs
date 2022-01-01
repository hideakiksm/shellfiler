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

namespace ShellFiler.Command.Terminal.Console {

    //=========================================================================================
    // クラス：コマンドを実行する
    // カーソルを上に移動します。
    //   書式 　 T_SendCursorUp()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class T_SendCursorUpCommand : TerminalActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public T_SendCursorUpCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            string command;
            if (TerminalPanel.ConsoleScreen.TerminalMode == TerminalModeType.Normal) {
                command = ConsoleScreen.CH_ESC + "[A";
            } else {
                command = ConsoleScreen.CH_ESC + "OA";
            }
            TerminalPanel.SendCommand(command);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.T_SendCursorUpCommand;
            }
        }
    }
}
