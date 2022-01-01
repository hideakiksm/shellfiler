using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command.FileList.Internal;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.ControlBar;

namespace ShellFiler.Command.FileList.Setting {

    //=========================================================================================
    // クラス：コマンドを実行する
    // キーに割り当てられたコマンドを確認して実行します。
    //   書式 　 KeyBindHelp()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class KeyBindHelpCommand : FileListActionCommand {
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public KeyBindHelpCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            KeyBindHelpDialog dialog = new KeyBindHelpDialog();
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return null;
            }
            KeyState key = dialog.ResultKey;
            FileListActionCommand command = Program.Document.CommandFactory.CreateFromKeyInputDirect(key, this.FileListViewTarget);
            if (command != null) {
                FileListCommandRuntime runtime = new FileListCommandRuntime(command);
                runtime.Execute();
            }

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.KeyBindHelpCommand;
            }
        }
    }
}
