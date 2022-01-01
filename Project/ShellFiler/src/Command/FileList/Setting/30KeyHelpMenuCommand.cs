using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command.FileList.Internal;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;

namespace ShellFiler.Command.FileList.Setting {

    //=========================================================================================
    // クラス：コマンドを実行する
    // キーヘルプメニューを表示します。
    //   書式 　 KeyListMenu()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class KeyListMenuCommand : FileListActionCommand {
        // メニューのX位置
        private const int MENU_POSITION_X = 100;

        // メニューのY位置
        private const int MENU_POSITION_Y = 50;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public KeyListMenuCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // delegateしてキー入力をキャンセル
            Program.MainWindow.BeginInvoke(new ExecutePostDelegate(ExecutePost));
            return null;
        }
        private delegate void ExecutePostDelegate();
        private void ExecutePost() {
            // A～Zのキーに対応する項目を動的生成
            Keys[] targetKeyList = {
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
                Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
            };
            KeyItemSettingList keyList = Program.Document.KeySetting.FileListKeyItemList;
            List<MenuItemSetting> menu = new List<MenuItemSetting>();
            foreach (Keys key in targetKeyList) {
                KeyItemSetting keyItem = keyList.GetSettingFromKey(new KeyState(key, false, false, false));
                if (keyItem != null) {
                    ActionCommandMoniker moniker = keyItem.ActionCommandMoniker;
                    menu.Add(new MenuItemSetting(moniker, key.ToString()[0], null));
                }
            }
            if (menu.Count == 0) {
                InfoBox.Information(Program.MainWindow, Resources.KeyHelpMenu_NoKeySetting);
                return;
            }

            // 説明を追加
            menu.Add(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(NopCommand)), '*', Resources.KeyHelpMenu_Message1));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(NopCommand)), '*', Resources.KeyHelpMenu_Message2));

            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.FileViewerMenu, Program.MainWindow);
            menuImpl.AddItemsFromSetting(cms, cms.Items, menu, Program.Document.KeySetting.FileListKeyItemList, false, null);
            menuImpl.RefreshToolbarStatus(new UIItemRefreshContext());
            FileListViewTarget.ContextMenuStrip = cms;
            FileListViewTarget.ContextMenuStrip.Show(FileListViewTarget, new Point(FileListViewTarget.Left + MENU_POSITION_X, FileListViewTarget.Top + MENU_POSITION_Y));
            FileListViewTarget.ContextMenuStrip = null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.KeyListMenuCommand;
            }
        }
    }
}
