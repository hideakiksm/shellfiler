using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.Window {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 他のすべてのタブを閉じます。
    //   書式 　 TabDeleteOther()
    //   引数  　なし
    //   戻り値　bool:タブを閉じたときtrue、閉じられなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class TabDeleteOtherCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TabDeleteOtherCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (Program.Document.TabPageList.Count == 1) {
                return false;
            }
            Program.MainWindow.TabControlImpl.DeleteOtherTab();
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.TabDeleteOtherCommand;
            }
        }
    }
}
