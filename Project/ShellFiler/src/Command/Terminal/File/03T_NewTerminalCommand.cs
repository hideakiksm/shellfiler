using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Command.FileList.SSH;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.UI.Dialog.Terminal;

namespace ShellFiler.Command.Terminal.File {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 新しいターミナルを開きます。
    //   書式 　 T_NewTerminal()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class T_NewTerminal : TerminalActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public T_NewTerminal() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            string dir = this.TerminalPanel.UserServer + ":/";
            CreateSSHTerminalCommand.OpenTerminal(CreateSSHTerminalCommand.OpenMode.AlwaysNew, dir);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.T_NewTerminal;
            }
        }
    }
}
