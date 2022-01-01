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
using ShellFiler.UI.FileList;
using ShellFiler.UI.ControlBar;

namespace ShellFiler.Command.FileList.Tools {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 実行メニューを表示します。
    //   書式 　 ExecuteMenu()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class ExecuteMenuCommand : FileListActionCommand {
        // メニューのX位置
        private const int MENU_POSITION_X = 100;

        // メニューのY位置
        private const int MENU_POSITION_Y = 50;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ExecuteMenuCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // delegateしてキー入力をキャンセル
            List<MenuItemSetting> menu = Program.Document.MenuSetting.ExecuteMenu;
            Program.MainWindow.BeginInvoke(new ExecutePostDelegate(ExecutePost), FileListViewTarget, menu);
            return null;
        }
        public delegate void ExecutePostDelegate(FileListView fileListView, List<MenuItemSetting> menu);
        public static void ExecutePost(FileListView fileListView, List<MenuItemSetting> menu) {
            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.FileViewerMenu, Program.MainWindow);
            menuImpl.AddItemsFromSetting(cms, cms.Items, menu, Program.Document.KeySetting.FileListKeyItemList, false, null);
            menuImpl.RefreshToolbarStatus(new UIItemRefreshContext());
            fileListView.ContextMenuStrip = cms;
            fileListView.ContextMenuStrip.Show(fileListView, new Point(fileListView.Left + MENU_POSITION_X, fileListView.Top + MENU_POSITION_Y));
            fileListView.ContextMenuStrip = null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ExecuteMenuCommand;
            }
        }
    }
}
