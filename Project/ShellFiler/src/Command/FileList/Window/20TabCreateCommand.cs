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
    // 新しいタブを開きます。
    //   書式 　 TabCreate()
    //   引数  　なし
    //   戻り値　bool:タブを開いたときtrue、開けなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class TabCreateCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TabCreateCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (Program.Document.TabPageList.Count >= TabPageList.MAX_TAB_PAGE_COUNT) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_TabTooMany);
                return false;
            }

            Program.MainWindow.TabControlImpl.DuplicateCurrentTab();
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.TabCreateCommand;
            }
        }
    }
}
