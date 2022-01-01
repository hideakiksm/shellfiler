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
    // バックログの表示と非表示を切り替えます。
    //   書式 　 T_ToggleBackLogView()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class T_ToggleBackLogViewCommand : TerminalActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public T_ToggleBackLogViewCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            TerminalPanel.VisibleBackLog = !TerminalPanel.VisibleBackLog;
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.T_ToggleBackLogViewCommand;
            }
        }
    }
}
