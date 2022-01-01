using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：メニュー設定ダイアログ
    //=========================================================================================
    public partial class MenuSettingDialog : Form {
        // メニュー設定（コンストラクタのインスタンスを編集して返る）
        private MenuSetting m_menuSetting;

        // コマンド仕様のXML解析結果
        private CommandSpec m_commandSpec;

        // メニューの利用シーン
        private CommandUsingSceneType m_scene;

        // サンプルパネル
        private MenuSamplePanel m_samplePanel;

        // 編集中メニューはDataGridViewRow.Tagに入っているものをマスターとする。
        // OKがクリックされたとき、Tagの情報を再整理してMenuSettingとする。

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]menuSetting   メニュー設定（コンストラクタのインスタンスを編集して返る）
        // 　　　　[in]commandSpec   コマンド仕様のXML解析結果
        // 　　　　[in]scene         メニューの利用シーン
        // 戻り値：なし
        //=========================================================================================
        public MenuSettingDialog(MenuSetting menuSetting, CommandSpec commandSpec, CommandUsingSceneType scene) {
            InitializeComponent();
            m_menuSetting = menuSetting;
            m_commandSpec = commandSpec;
            m_scene = scene;
            
            m_samplePanel = new MenuSamplePanel();
            m_samplePanel.Dock = DockStyle.Fill;
            this.panelSample.Controls.Add(m_samplePanel);

            // 項目を追加
            List<MenuItemSetting> menuItems = GetMenuItemListOriginal(m_menuSetting, m_scene);
            List<MenuItemCustomRoot> customItems = new List<MenuItemCustomRoot>();
            MenuItemCustomSetting customList = GetMenuItemCustom(m_menuSetting, m_scene);
            customItems.AddRange(customList.CustomRootList);
            string prevMenu = "";
            for (int i = 0; i < menuItems.Count; i++) {
                for (int j = 0; j < customItems.Count; j++) {
                    if (customItems[j] != null && customItems[j].PrevItemName == prevMenu) {
                        AddListBoxItem(null, null, customItems[j], -1);
                        customItems[j] = null;
                    }
                }
                AddListBoxItem(menuItems[i], customList, null, -1);
                prevMenu = menuItems[i].ItemNameInput;
            }
            for (int i = 0; i < customItems.Count; i++) {
                if (customItems[i] != null) {
                    AddListBoxItem(null, null, customItems[i], -1);
                }
            }
            this.dataGridView.Rows[0].Cells[0].Selected = true;
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：定義済みメニューの一覧を返す
        // 引　数：[in]menuSetting   メニュー設定
        // 　　　　[in]scene         メニューの利用シーン
        // 戻り値：定義済みメニューのルート項目のリスト
        //=========================================================================================
        private List<MenuItemSetting> GetMenuItemListOriginal(MenuSetting menuSetting, CommandUsingSceneType scene) {
            List<MenuItemSetting> menuItems = null;
            if (scene == CommandUsingSceneType.FileList) {
                menuItems = menuSetting.MainMenuItemList;
            }
            return menuItems;
        }

        //=========================================================================================
        // 機　能：カスタムメニューの一覧を返す
        // 引　数：[in]menuSetting   メニュー設定
        // 　　　　[in]scene         メニューの利用シーン
        // 戻り値：カスタムメニューのルート項目のリスト
        //=========================================================================================
        private MenuItemCustomSetting GetMenuItemCustom(MenuSetting menuSetting, CommandUsingSceneType scene) {
            MenuItemCustomSetting custom = null;
            if (scene == CommandUsingSceneType.FileList) {
                custom = menuSetting.MainMenuCustom; 
            }
            return custom;
        }

        //=========================================================================================
        // 機　能：定義済み/カスタムを混在させた状態で編集内容からメニューの一覧を返す
        // 引　数：なし
        // 戻り値：ルート項目のリスト
        //=========================================================================================
        private List<MenuItemSetting> GetMenuItemListAll() {
            List<MenuItemSetting> menuItems = new List<MenuItemSetting>();
            int count = this.dataGridView.Rows.Count;
            for (int i = 0; i < count; i++) {
                MenuSettingRowTag tag = (MenuSettingRowTag)(this.dataGridView.Rows[i].Tag);
                if (tag.FixedMenuItem != null) {
                    menuItems.Add(tag.FixedMenuItem);
                } else {
                    menuItems.Add(tag.CustomItem.MenuSetting);
                }
            }
            return menuItems;
        }

        //=========================================================================================
        // 機　能：XML解析済みのコマンド定義を返す
        // 引　数：[in]commandSpec   XMLの解析結果
        // 　　　　[in]scene         コマンドの利用シーン
        // 戻り値：コマンドの利用シーンに応じたコマンド一覧
        //=========================================================================================
        private CommandScene GetCommandScene(CommandSpec commandSpec, CommandUsingSceneType scene) {
            CommandScene commandScene = null;
            if (scene == CommandUsingSceneType.FileList) {
                commandScene = commandSpec.FileList;
            }
            return commandScene;
        }

        //=========================================================================================
        // 機　能：リストボックスの項目を追加する
        // 引　数：[in]menuItem    編集対象の定義済みメニュー（カスタムメニューのときnull）
        // 　　　　[in]customList  メニューのカスタム設定全体（定義済みの表示非表示判定用、カスタムメニューのときnull）
        // 　　　　[in]customItem  編集対象のカスタムメニュー（定義済みメニューのときnull）
        // 　　　　[in]index       項目のインデックス
        // 戻り値：なし
        //=========================================================================================
        private void AddListBoxItem(MenuItemSetting menuItem, MenuItemCustomSetting customList, MenuItemCustomRoot customItem, int index) {
            bool checkValue, checkReadOnly;
            string itemValue, itemType;
            MenuSettingRowTag tag;
            if (menuItem != null) {
                // 定義済み項目
                if (menuItem.ItemNameInput == MenuSetting.OptionMenuName) {
                    checkReadOnly = true;
                    checkValue = true;
                } else {
                    checkReadOnly = false;
                    checkValue = customList.IsDisplayMenu(menuItem.ItemNameInput);
                }
                itemValue = menuItem.ItemName.Replace("&", "");
                itemType = Resources.DlgMenuSetting_TypeFixed;
                tag = new MenuSettingRowTag(menuItem, null);
            } else {
                // カスタム項目
                checkValue = customItem.DisplayRoot;
                checkReadOnly = false;
                itemValue = customItem.MenuSetting.ItemName.Replace("&", "");
                itemType = Resources.DlgMenuSetting_TypeCustom;
                tag = new MenuSettingRowTag(null, customItem);
            }

            // 項目を追加
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewCheckBoxCell checkBox = new DataGridViewCheckBoxCell(checkValue);
            checkBox.Value = checkValue;
            checkBox.ThreeState = false;
            row.Cells.Add(checkBox);
            DataGridViewTextBoxCell textCell1 = new DataGridViewTextBoxCell();
            textCell1.Value = itemValue;
            row.Cells.Add(textCell1);
            DataGridViewTextBoxCell textCell2 = new DataGridViewTextBoxCell();
            textCell2.Value = itemType;
            row.Cells.Add(textCell2);
            row.Tag = tag;

            if (index == - 1) {
                this.dataGridView.Rows.Add(row);
            } else {
                this.dataGridView.Rows.Insert(index, row);
            }
            row.Cells[0].ReadOnly = checkReadOnly;
            row.Cells[1].ReadOnly = true;
            row.Cells[2].ReadOnly = true;
        }

        //=========================================================================================
        // 機　能：UIの有効／無効状態を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            DataGridViewRow selectedRow = GetSelectedRow();
            if (selectedRow == null) {
                // 選択なし
                this.buttonAdd.Enabled = true;
                this.buttonEdit.Enabled = false;
                this.buttonDelete.Enabled = false;
                this.buttonUp.Enabled = false;
                this.buttonDown.Enabled = false;
            } else if (((MenuSettingRowTag)(selectedRow.Tag)).FixedMenuItem != null) {
                // 定義済み
                this.buttonAdd.Enabled = true;
                this.buttonEdit.Enabled = false;
                this.buttonDelete.Enabled = false;
                this.buttonUp.Enabled = false;
                this.buttonDown.Enabled = false;
            } else {
                // カスタム
                this.buttonAdd.Enabled = true;
                this.buttonEdit.Enabled = true;
                this.buttonDelete.Enabled = true;
                this.buttonUp.Enabled = (selectedRow.Index != 0);
                this.buttonDown.Enabled = (selectedRow.Index != this.dataGridView.Rows.Count - 1);
            }
        }

        //=========================================================================================
        // 機　能：データグリッドビューの選択中のセルが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void dataGridView_SelectionChanged(object sender, EventArgs evt) {
            DataGridViewRow selectedRow = GetSelectedRow();
            if (selectedRow == null) {
                return;
            }
            MenuSettingRowTag item = (MenuSettingRowTag)(selectedRow.Tag);
            if (item.FixedMenuItem != null) {
                m_samplePanel.Initialize(item.FixedMenuItem, true);
            } else {
                m_samplePanel.Initialize(item.CustomItem.MenuSetting, true);
            }
            m_samplePanel.Invalidate();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：データグリッドビューの選択中セルを返す
        // 引　数：なし
        // 戻り値：選択中セルの行（選択中ではないときnull）
        //=========================================================================================
        private DataGridViewRow GetSelectedRow() {
            if (this.dataGridView.SelectedCells.Count == 0) {
                return null;
            }
            int rowIndex = this.dataGridView.SelectedCells[0].RowIndex;
            DataGridViewRow selectedRow = this.dataGridView.Rows[rowIndex];
            return selectedRow;
        }

        //=========================================================================================
        // 機　能：追加ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonAdd_Click(object sender, EventArgs evt) {
            int index;
            DataGridViewRow row = GetSelectedRow();
            if (row == null) {
                index = 0;
            } else {
                index = row.Index;
            }
            string menuName = CreateOriginalMenu();
            MenuItemSetting menuItem = new MenuItemSetting(menuName, '*');
            List<MenuItemSetting> parentMenuList = GetMenuItemListAll();
            MenuItemCustomRoot item = new MenuItemCustomRoot(true, "", menuItem);
            CommandScene commandScene = GetCommandScene(m_commandSpec, m_scene);
            MenuListSettingDialog dialog = new MenuListSettingDialog(commandScene, item, parentMenuList);
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }
            AddListBoxItem(null, null, dialog.ResultMenuItem, index + 1);
            this.dataGridView.Rows[index + 1].Selected = true;
        }

        //=========================================================================================
        // 機　能：ユニークなメニュー項目名を作成する
        // 引　数：なし
        // 戻り値：新しいメニュー項目名
        //=========================================================================================
        private string CreateOriginalMenu() {
            List<MenuItemSetting> menuItems = GetMenuItemListAll();
            string menuName;
            for (int i = 1;; i++) {
                if (i == 1) {
                    menuName = Resources.DlgMenuSetting_CustomMenuRootName;
                } else {
                    menuName = Resources.DlgMenuSetting_CustomMenuRootName + i;
                }
                bool found = false;
                for (int j = 0; j < menuItems.Count; j++) {
                    if (menuItems[j].ItemNameInput == menuName) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    return menuName;
                }
            }
        }

        //=========================================================================================
        // 機　能：編集ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonEdit_Click(object sender, EventArgs evt) {
            DataGridViewRow row = GetSelectedRow();
            if (row == null) {
                return;
            }
            MenuSettingRowTag tag = (MenuSettingRowTag)(row.Tag);
            if (tag.CustomItem == null) {
                return;
            }
            MenuItemCustomRoot prevItem = (MenuItemCustomRoot)(tag.CustomItem.Clone());
            List<MenuItemSetting> parentMenuList = GetMenuItemListAll();
            CommandScene commandScene = GetCommandScene(m_commandSpec, m_scene);
            MenuListSettingDialog dialog = new MenuListSettingDialog(commandScene, tag.CustomItem, parentMenuList);
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                tag.CustomItem = prevItem;
                return;
            }
            row.Cells[1].Value = tag.CustomItem.MenuSetting.ItemName.Replace("&", "");
            m_samplePanel.Initialize(tag.CustomItem.MenuSetting, true);
            this.panelSample.Invalidate();
        }

        //=========================================================================================
        // 機　能：削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            DataGridViewRow row = GetSelectedRow();
            if (row == null) {
                return;
            }
            MenuSettingRowTag tag = (MenuSettingRowTag)(row.Tag);
            if (tag.CustomItem == null) {
                return;
            }
            DialogResult result = InfoBox.Question(this, MessageBoxButtons.OKCancel, Resources.DlgMenuSetting_DeleteMenu);
            if (result != DialogResult.OK) {
                return;
            }
            int nextSelectIndex = Math.Min(row.Index, this.dataGridView.Rows.Count - 1);
            this.dataGridView.Rows.Remove(row);
            this.dataGridView.Rows[nextSelectIndex].Selected = true;
        }

        //=========================================================================================
        // 機　能：上へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonUp_Click(object sender, EventArgs evt) {
            DataGridViewRow row = GetSelectedRow();
            if (row == null) {
                return;
            }
            MenuSettingRowTag tag = (MenuSettingRowTag)(row.Tag);
            if (tag.CustomItem == null) {
                return;
            }
            int index = row.Index;
            if (index == 0) {
                return;
            }
            this.dataGridView.Rows.Remove(row);
            this.dataGridView.Rows.Insert(index - 1, row);
            row.Selected = true;
        }

        //=========================================================================================
        // 機　能：下へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDown_Click(object sender, EventArgs evt) {
            DataGridViewRow row = GetSelectedRow();
            if (row == null) {
                return;
            }
            MenuSettingRowTag tag = (MenuSettingRowTag)(row.Tag);
            if (tag.CustomItem == null) {
                return;
            }
            int index = row.Index;
            if (index == this.dataGridView.Rows.Count - 1) {
                return;
            }
            this.dataGridView.Rows.Remove(row);
            this.dataGridView.Rows.Insert(index + 1, row);
            row.Selected = true;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            MenuItemCustomSetting custom = GetMenuItemCustom(m_menuSetting, m_scene);
            int rowCount = this.dataGridView.Rows.Count;

            // 新しいカスタム項目を作成
            Dictionary<string, bool> definedDispList = new Dictionary<string, bool>();
            HashSet<string> nameSet = new HashSet<string>();
            List<string> duplicatedNameList = new List<string>();
            for (int i = 0; i < rowCount; i++) {
                DataGridViewRow row = this.dataGridView.Rows[i];
                MenuSettingRowTag tag = (MenuSettingRowTag)(row.Tag);
                bool check = (bool)(row.Cells[0].Value);
                string itemName;
                if (tag.CustomItem == null) {
                    itemName = tag.FixedMenuItem.ItemNameInput;
                    definedDispList.Add(itemName, check); 
                } else {
                    itemName = tag.CustomItem.MenuSetting.ItemNameInput;
                    tag.CustomItem.DisplayRoot = check;
                }
                if (nameSet.Contains(itemName)) {
                    duplicatedNameList.Add(itemName);
                } else {
                    nameSet.Add(itemName);
                }
            }

            // 項目名の重複を警告
            if (duplicatedNameList.Count > 0) {
                string message;
                if (duplicatedNameList.Count == 1) {
                    message = Resources.DlgMenuSetting_DuplicateMenuName;
                } else {
                    message = Resources.DlgMenuSetting_DuplicateMenuNameMulti;
                }
                DialogResult result = InfoBox.Message(this, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, message, duplicatedNameList[0], duplicatedNameList.Count);
                if (result != DialogResult.OK) {
                    return;
                }
            }

            // 項目を登録
            List<MenuItemCustomRoot> newCustomList = new List<MenuItemCustomRoot>();
            string prevMenu = "";
            for (int i = 0; i < rowCount; i++) {
                DataGridViewRow row = this.dataGridView.Rows[i];
                MenuSettingRowTag tag = (MenuSettingRowTag)(row.Tag);
                if (tag.CustomItem != null) {
                    tag.CustomItem.PrevItemName = prevMenu;
                    newCustomList.Add(tag.CustomItem);
                } else {
                    prevMenu = tag.FixedMenuItem.ItemNameInput;
                }
            }
            custom.ResetCustomMenuSetting(newCustomList, definedDispList);

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // クラス：データグリッドビューの行に設定されるTag
        //=========================================================================================
        private class MenuSettingRowTag {
            // 定義済みのメニュー項目（カスタムのメニュー項目のときnull）
            public MenuItemSetting FixedMenuItem;

            // カスタムのメニュー項目（定義済みのメニュー項目のときnull）
            public MenuItemCustomRoot CustomItem;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]fixedMenuItem  定義済みのメニュー項目（カスタムのメニュー項目のときnull）
            // 　　　　[in]customItem     カスタムのメニュー項目（定義済みのメニュー項目のときnull）
            // 戻り値：なし
            //=========================================================================================
            public MenuSettingRowTag(MenuItemSetting fixedMenuItem, MenuItemCustomRoot customItem) {
                FixedMenuItem = fixedMenuItem;
                CustomItem  = customItem;
            }
        }
    }
}
