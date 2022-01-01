using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.KeyOption;
using ShellFiler.UI.ControlBar;

namespace ShellFiler.Command.FileList.Tools {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ポップアップメニュー{0}を表示します。
    //   書式 　 PopupMenu(menu menu)
    //   引数  　menu:表示するメニューのルート
    // 　　　　　menu-default:
    // 　　　　　menu-range:
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class PopupMenuCommand : FileListActionCommand {
        // 表示するメニューのルート
        private string m_rootName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public PopupMenuCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        // メ　モ：[0]:メニュー項目名（「ファイル」など）
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_rootName = (string)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // delegateしてキー入力をキャンセル
            List<MenuItemSetting> menu = null;
            List<MenuItemSetting> rootMenuList = Program.Document.MenuSetting.CreateMenuCustomizedList(CommandUsingSceneType.FileList);
            for (int i = 0; i < rootMenuList.Count; i++) {
                if (rootMenuList[i].ItemNameInput == m_rootName) {
                    menu = rootMenuList[i].SubMenuList;
                }
            }
            if (menu == null) {
                return null;
            }
            Program.MainWindow.BeginInvoke(new ExecuteMenuCommand.ExecutePostDelegate(ExecuteMenuCommand.ExecutePost), FileListViewTarget, menu);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.PopupMenuCommand;
            }
        }
    }
}
