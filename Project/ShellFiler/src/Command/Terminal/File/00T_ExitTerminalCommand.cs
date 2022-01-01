using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Terminal.UI;

namespace ShellFiler.Command.Terminal.File {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ターミナルを終了します。
    //   書式 　 T_ExitTerminal()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class T_ExitTerminalCommand : TerminalActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public T_ExitTerminalCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            TerminalForm form = TerminalPanel.TerminalForm;
            if (form != null) {
                if (form.TopMost) {
                    form.FullScreen(false, false);
                    return null;
                } else {
                    form.Close();
                    return null;
                }
            }
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.T_ExitTerminalCommand;
            }
        }
    }
}
