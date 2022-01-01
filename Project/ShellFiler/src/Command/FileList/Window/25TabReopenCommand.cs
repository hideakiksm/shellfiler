using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.Window {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 直前に閉じたタブを開き直します。
    //   書式 　 TabReopen()
    //   引数  　なし
    //   戻り値　bool:タブを開いたときtrue、開けなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class TabReopenCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TabReopenCommand() {
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
            if (!Program.MainWindow.TabControlImpl.ExistReopenInfo) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_TabNotExistReopen);
                return false;
            }

            Program.MainWindow.TabControlImpl.ReopenTab();
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.TabReopenCommand;
            }
        }
    }
}
