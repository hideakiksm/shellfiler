using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.Window {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 一つ左のタブを選択します。
    //   書式 　 TabSelectLeft()
    //   引数  　なし
    //   戻り値　bool:タブを選択したときtrue、最も左のときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class TabSelectLeftCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TabSelectLeftCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            int currentIndex = Program.MainWindow.TabControlImpl.GetCurrentTabIndex();
            if (currentIndex <= 0) {
                return false;
            }

            Program.MainWindow.TabControlImpl.SelectTabDirect(currentIndex - 1);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.TabSelectLeftCommand;
            }
        }
    }
}
