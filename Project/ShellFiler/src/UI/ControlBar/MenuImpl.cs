using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileViewer;
using ShellFiler.Util;

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // クラス：メニューの項目一覧の実装
    //=========================================================================================
    class MenuImpl {
        // ダミーのメニューにつくタグ（このインスタンスが設定されているメニューはドロップダウン時に削除）
        private object DummyMenuTag = new object();

        // コマンドの送信元の識別用
        private UICommandSender m_commandSender;
        
        // コマンドの送信先
        private IUICommandTarget m_commandTarget;

        // ツールバーボタンの項目一覧（ドロップダウン項目/セパレータを除く）
        private List<ToolStripMenuItem> m_menuItemList = new List<ToolStripMenuItem>();
        
        // ボタンの状態（マークされている状態として更新されているときtrue）
        private bool m_uiStatusMarked = false;

        // ボタンの状態（直前のパスヒストリが有効として更新されているときtrue）
        private bool m_uiStatusPathHisPrev = true;

        // ボタンの状態（直後のパスヒストリが有効として更新されているときtrue）
        private bool m_uiStatusPathHisNext = true;

        // ドロップダウンのメニュー項目が開かれたことを通知するイベント
        public event MenuItemDropDownEventHandler MenuItemDropDownEvent; 

        // 作成しようとしているルート項目の追加が適切なときtrueを返すdelegate
        public delegate bool IsValidMenuItemDelegate(MenuItemSetting item);

        // キー設定
        private KeyItemSettingList m_keySetting;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]commandSender コマンドの送信元の識別用
        // 　　　　[in]commandTarget コマンドの送信先
        // 戻り値：なし
        //=========================================================================================
        public MenuImpl(UICommandSender commandSender, IUICommandTarget commandTarget) {
            m_commandSender = commandSender;
            m_commandTarget = commandTarget;
            m_uiStatusMarked = true;
            m_uiStatusPathHisPrev = true;
            m_uiStatusPathHisNext = true;
        }

        //=========================================================================================
        // 機　能：すべてのメニュー項目を削除する
        // 引　数：[in]menuBase    処理対象のメニュー
        // 　　　　[in]itemSettingList  項目の設定
        // 戻り値：なし
        //=========================================================================================
        public void ClearItemsFromSetting(ToolStrip menuBase, ToolStripItemCollection menuItems) {
            int itemCount = menuItems.Count;
            for (int i = itemCount - 1; i >= 0; i--) {
                ToolStripItem item = menuItems[i];
                if (item is ToolStripMenuItem) {
                    ToolStripMenuItem menuItem = (ToolStripMenuItem)item;
                    ClearItemsFromSetting(menuBase, menuItem.DropDownItems);
                }
                item.Dispose();
            }
            menuItems.Clear();
        }

        //=========================================================================================
        // 機　能：設定から項目を追加する
        // 引　数：[in]menuBase         処理対象のメニュー
        // 　　　　[in]menuItems        項目を作成するメニューのItemCollection
        // 　　　　[in]itemSettingList  項目の設定
        // 　　　　[in]keySetting       キーの設定
        // 　　　　[in]hasMenuBar       メニューバーを持っている形式のときtrue、ポップアップのときfalse
        // 　　　　[in]isValid          作成しようとしているルート項目の追加が適切なときtrueを返すdelegate（常に作成するときnull）
        // 戻り値：なし
        //=========================================================================================
        public void AddItemsFromSetting(ToolStrip menuBase, ToolStripItemCollection menuItems, List<MenuItemSetting> itemSettingList, KeyItemSettingList keySetting, bool hasMenuBar, IsValidMenuItemDelegate isValid) {
            m_keySetting = keySetting;
            m_menuItemList.Clear();
            m_uiStatusMarked = true;
            m_uiStatusPathHisPrev = true;
            m_uiStatusPathHisNext = true;
            menuBase.ImageList = UIIconManager.IconImageList;
            if (hasMenuBar) {
                foreach (MenuItemSetting itemSetting in itemSettingList) {
                    if (isValid != null && !isValid(itemSetting)) {
                        continue;
                    }
                    ToolStripMenuItem dropDownItem = new ToolStripMenuItem();
                    menuItems.Add(dropDownItem);
                    dropDownItem.Size = new System.Drawing.Size(85, 22);
                    dropDownItem.Text = itemSetting.ItemName;
                    dropDownItem.Tag = itemSetting;
                    dropDownItem.DropDown.ImageList = UIIconManager.IconImageList;
                    dropDownItem.DropDownOpening += new EventHandler(DropDownItem_Opening);
                    if (itemSetting.SubMenuList.Count > 0) {
                        // 何かメニューがないとAlt+?で開かないため、ダミーを作成
                        ToolStripMenuItem dummy = new ToolStripMenuItem();
                        dummy.Tag = DummyMenuTag;
                        dropDownItem.DropDownItems.Add(dummy);
                    }
                    // 下位の階層はDropDownItem_Openingで作成
                }
            } else {
                CreateDropdownMenuItem(menuItems, itemSettingList, keySetting);
            }
        }
        
        //=========================================================================================
        // 機　能：ドロップダウンメニューの設定から配下の項目を作成する
        // 引　数：[in]targetItems     追加対象のUI項目のItemCollection
        // 　　　　[in]itemSettingList 追加するドロップダウンメニュー項目のリスト
        // 　　　　[in]keySetting      キーの設定
        // 戻り値：なし
        //=========================================================================================
        public void CreateDropdownMenuItem(ToolStripItemCollection targetItems, List<MenuItemSetting> itemSettingList, KeyItemSettingList keySetting) {
            foreach (MenuItemSetting itemSetting in itemSettingList) {
                if (itemSetting.Type == MenuItemSetting.ItemType.MenuItem) {
                    CreateMenuItem(targetItems, itemSetting, keySetting);
                } else if (itemSetting.Type == MenuItemSetting.ItemType.Separator) {
                    targetItems.Add(new ToolStripSeparator());
                } else if (itemSetting.Type == MenuItemSetting.ItemType.SubMenu) {
                    ToolStripMenuItem dropDownItem = new ToolStripMenuItem();
                    dropDownItem.Size = new System.Drawing.Size(85, 22);
                    dropDownItem.Text = itemSetting.ItemName;
                    dropDownItem.DropDown.ImageList = UIIconManager.IconImageList;
                    dropDownItem.DropDownOpening += new EventHandler(DropDownItem_Opening);
                    targetItems.Add(dropDownItem);
                    CreateDropdownMenuItem(dropDownItem.DropDownItems, itemSetting.SubMenuList, keySetting);
                }
            }
        }

        //=========================================================================================
        // 機　能：メニューの設定から配下の項目を作成する
        // 引　数：[in]targetItems     追加対象のUI項目
        // 　　　　[in]menuSetting     追加するメニューの項目の設定
        // 　　　　[in]keySetting      キーの設定
        // 戻り値：なし
        //=========================================================================================
        private void CreateMenuItem(ToolStripItemCollection targetItems, MenuItemSetting menuSetting, KeyItemSettingList keySetting) {
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Size = new System.Drawing.Size(85, 22);
            item.Text = menuSetting.ItemName;
            if (menuSetting.UIResource.IconIdLeft != IconImageListID.None) {
                item.ImageIndex = (int)menuSetting.UIResource.IconIdLeft;
            }
            item.Tag = new MenuItemTag(menuSetting, menuSetting.UIResource.IconIdLeft, menuSetting.UIResource.IconIdRight);
            item.ShortcutKeyDisplayString = ToolBarImpl.CreateShortcutDisplayString(keySetting, menuSetting.ActionCommandMoniker);
            item.Click += new EventHandler(menuItemClicked);
            m_menuItemList.Add(item);
            targetItems.Add(item);
        }

        //=========================================================================================
        // 機　能：項目がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void menuItemClicked(object sender, EventArgs evt) {
            MenuItemTag tag = null;
            tag = (MenuItemTag)(((ToolStripMenuItem)sender).Tag);
            MenuItemSetting setting = tag.ItemSetting;
            UICommandItem item = new UICommandItem(setting.ActionCommandMoniker);
            m_commandTarget.OnUICommand(m_commandSender, item);
        }

        //=========================================================================================
        // 機　能：ドロップダウン項目がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void DropDownItem_Opening(object sender, EventArgs evt) {
            ToolStripMenuItem dropDownItem = (ToolStripMenuItem)sender;
            MenuItemSetting itemSetting = (MenuItemSetting)(dropDownItem.Tag);
            if (dropDownItem.DropDownItems.Count == 1 && dropDownItem.DropDownItems[0].Tag == DummyMenuTag) {
                dropDownItem.DropDownItems.Clear();
                CreateDropdownMenuItem(dropDownItem.DropDownItems, itemSetting.SubMenuList, m_keySetting);
            }
            if (MenuItemDropDownEvent != null) {
                MenuItemDropDownEvent(sender, evt);
            }
        }

        //=========================================================================================
        // 機　能：メインウィンドウのカーソルの左右に変化が生じたときの処理を行う
        // 引　数：[in]isLeft  カーソルが左側にあるときtrue
        // 戻り値：なし
        //=========================================================================================
        public void OnCursorLRChanged(bool isLeft) {
            foreach (ToolStripMenuItem item in m_menuItemList) {
                if (item.Tag == null) {
                    continue;
                }
                MenuItemTag tag = (MenuItemTag)(item.Tag);
                if (tag.LeftIconId != IconImageListID.None) {
                    if (tag.LeftIconId != tag.RightIconId) {
                        if (isLeft) {
                            item.ImageIndex = (int)tag.LeftIconId;
                        } else {
                            item.ImageIndex = (int)tag.RightIconId;
                        }
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：ツールバーのステータス状態を更新する
        // 引　数：[in]context   ツールバーの更新状態のコンテキスト
        // 戻り値：なし
        //=========================================================================================
        public void RefreshToolbarStatus(UIItemRefreshContext context) {
            // マーク中のみ有効
            if (m_uiStatusMarked != context.IsMarked) {                
                m_uiStatusMarked = context.IsMarked;
                foreach (ToolStripMenuItem item in m_menuItemList) {
                    MenuItemSetting itemSetting = ((MenuItemTag)(item.Tag)).ItemSetting;
                    if (UIEnableCondition.CheckMark(itemSetting.UIResource.UIEnableCondition)) {
                        item.Enabled = m_uiStatusMarked;
                    }
                }
            }

            // 前のパスヒストリが有効
            if (m_uiStatusPathHisPrev != context.GetPathHistoryPrev(m_commandSender)) {
                m_uiStatusPathHisPrev = context.GetPathHistoryPrev(m_commandSender);
                foreach (ToolStripMenuItem item in m_menuItemList) {
                    MenuItemSetting itemSetting = ((MenuItemTag)(item.Tag)).ItemSetting;
                    if (itemSetting.UIResource.UIEnableCondition == UIEnableCondition.PathHistP) {
                        item.Enabled = m_uiStatusPathHisPrev;
                    }
                }
            }

            // 次のパスヒストリが有効
            if (m_uiStatusPathHisNext != context.GetPathHistoryNext(m_commandSender)) {
                m_uiStatusPathHisNext = context.GetPathHistoryNext(m_commandSender);
                foreach (ToolStripMenuItem item in m_menuItemList) {
                    MenuItemSetting itemSetting = ((MenuItemTag)(item.Tag)).ItemSetting;
                    if (itemSetting.UIResource.UIEnableCondition == UIEnableCondition.PathHistN) {
                        item.Enabled = m_uiStatusPathHisNext;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：メニューの項目名を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void UpdateItemName() {
            // OpenAssociateコマンド
            List<UIResource> assocItems = UIResource.OpenFileAssociateItems;
            foreach (ToolStripMenuItem item in m_menuItemList) {
                if (item.Tag != null) {
                    MenuItemTag tag = (MenuItemTag)(item.Tag);
                    UIResource uiResource = tag.ItemSetting.UIResource;
                    int assocIndex = assocItems.IndexOf(uiResource);
                    if (assocIndex != -1) {
                        item.Text = Program.Document.KeySetting.AssociateSetting.AssocSettingList[assocIndex].DislayName + "(" + tag.ItemSetting.ShortcutKey + ")";
                    }
                }
            }
        }

        //=========================================================================================
        // プロパティ：メニュー項目のTagプロパティに保持する情報
        //=========================================================================================
        public class MenuItemTag {
            // ツールバーの設定情報
            public MenuItemSetting ItemSetting;

            // 左側にカーソルがあるときのアイコン
            public IconImageListID LeftIconId;
            
            // 右側にカーソルがあるときのアイコン
            public IconImageListID RightIconId;

            public MenuItemTag(MenuItemSetting itemSetting, IconImageListID leftIconId, IconImageListID rightIconId) {
                this.ItemSetting = itemSetting;
                this.LeftIconId = leftIconId;
                this.RightIconId = rightIconId;
            }
        }
    }
}
