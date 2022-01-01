using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.FileTask;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.UI;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.Terminal;

namespace ShellFiler.Command.FileList.SSH {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 接続済みのSSHのシェルチャネルを開きます。
    //   書式 　 SelectSSHTerminal()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class SelectSSHTerminalCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SelectSSHTerminalCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            CreateSSHTerminalCommand.OpenTerminal(CreateSSHTerminalCommand.OpenMode.SelectExisting, null);

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.SelectSSHTerminalCommand;
            }
        }
    }
}
