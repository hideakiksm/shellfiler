using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.MonitoringViewer;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.UI.Dialog.Terminal;

namespace ShellFiler.Command.Terminal.File {

    //=========================================================================================
    // クラス：コマンドを実行する
    // このターミナルの表示名を編集します。
    //   書式 　 T_EditDisplayName()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class T_EditDisplayNameCommand : TerminalActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public T_EditDisplayNameCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            ConsoleScreen console = TerminalPanel.ConsoleScreen;
            TerminalEditTitleDialog dialog = new TerminalEditTitleDialog(TerminalPanel.UserServer, console.DisplayName);
            DialogResult result = dialog.ShowDialog(TerminalPanel.DialogParentForm);
            if (result != DialogResult.OK) {
                return null;
            }
            console.DisplayName = dialog.ResultTitle;
            TerminalPanel.ResetWindowTitle();
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.T_EditDisplayNameCommand;
            }
        }
    }
}
