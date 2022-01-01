using System;
using System.Collections.Generic;
using ShellFiler.Api;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：メニューのカスタマイズ情報
    //=========================================================================================
    public class MenuItemSetting : ICloneable {
        // ショートカットなし
        public const char SHORTCUT_KEY_NONE = '*';

        // 項目の種類
        private ItemType m_itemType;

        // 実行するコマンド（ItemType.MenuItem以外はnull）
        private ActionCommandMoniker m_commandMoniker;

        // コマンドのUI表現
        private UIResource m_uiResource;

        // 項目名の表記（カスタマイズしないときnull）
        private string m_itemName;

        // ショートカットキー（なしのとき'*'）
        private char m_shortcutKey = SHORTCUT_KEY_NONE;

        // サブメニューのリスト（ItemType.MenuItemはnull）
        private List<MenuItemSetting> m_subMenuList = null;

        //=========================================================================================
        // 機　能：コンストラクタ（セパレータ）
        // 引　数：[in]type  アイコンの種類
        // 戻り値：なし
        //=========================================================================================
        public MenuItemSetting(ItemType type) {
            m_itemType = type;
            m_itemName = "";
        }

        //=========================================================================================
        // 機　能：コンストラクタ（ポップアップ）
        // 引　数：[in]itemName   メニュー項目の表記
        // 　　　　[in]shortcut   ショートカットキー
        // 戻り値：なし
        //=========================================================================================
        public MenuItemSetting(string itemName, char shortcut) {
            m_itemType = ItemType.SubMenu;
            m_itemName = itemName;
            m_shortcutKey = shortcut;
            m_subMenuList = new List<MenuItemSetting>();
        }

        //=========================================================================================
        // 機　能：コンストラクタ（ボタン）
        // 引　数：[in]moniker       実行するコマンド
        // 　　　　[in]shortcut      ショートカットキー
        // 　　　　[in]itemName      項目名の表記（カスタマイズしないときnull）
        // 戻り値：なし
        //=========================================================================================
        public MenuItemSetting(ActionCommandMoniker moniker, char shortcut, string itemName) {
            m_commandMoniker = moniker;
            m_itemType = ItemType.MenuItem;
            ActionCommand actionCommand = moniker.CreateActionCommand();
            m_uiResource = actionCommand.UIResource;
            m_itemName = itemName;
            m_shortcutKey = shortcut;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（内部）
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private MenuItemSetting() {
        }

        //=========================================================================================
        // 機　能：メニューの内容を差し替える（ポップアップ）
        // 引　数：[in]itemName   メニュー項目の表記
        // 　　　　[in]shortcut   ショートカットキー
        // 戻り値：なし
        //=========================================================================================
        public void ReplaceMenuSettingItem(string itemName, char shortcut) {
            m_itemType = ItemType.SubMenu;
            m_itemName = itemName;
            m_shortcutKey = shortcut;
        }

        //=========================================================================================
        // 機　能：メニューの内容を差し替える（ボタン）
        // 引　数：[in]moniker       実行するコマンド
        // 　　　　[in]shortcut      ショートカットキー
        // 　　　　[in]itemName      項目名の表記（カスタマイズしないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void ReplaceMenuSettingItem(ActionCommandMoniker moniker, char shortcut, string itemName) {
            m_commandMoniker = moniker;
            m_itemType = ItemType.MenuItem;
            ActionCommand actionCommand = moniker.CreateActionCommand();
            m_uiResource = actionCommand.UIResource;
            m_itemName = itemName;
            m_shortcutKey = shortcut;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            MenuItemSetting obj = new MenuItemSetting();
            obj.m_itemType = m_itemType;
            if (m_commandMoniker == null) {
                obj.m_commandMoniker = null;
            } else {
                obj.m_commandMoniker = (ActionCommandMoniker)(m_commandMoniker.Clone());
            }
            obj.m_uiResource = m_uiResource;
            obj.m_shortcutKey = m_shortcutKey;
            obj.m_itemName = m_itemName;
            if (m_subMenuList == null) {
                obj.m_subMenuList = null;
            } else {
                obj.m_subMenuList = new List<MenuItemSetting>();
                for (int i = 0; i < m_subMenuList.Count; i++) {
                    obj.m_subMenuList.Add((MenuItemSetting)m_subMenuList[i].Clone());
                }
            }
            return obj;
        }

        //=========================================================================================
        // 機　能：サブメニューを追加する
        // 引　数：[in]item  サブメニュー
        // 戻り値：なし
        //=========================================================================================
        public void AddSubMenu(MenuItemSetting item) {
            m_itemType = ItemType.SubMenu;
            m_subMenuList.Add(item);
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(MenuItemSetting obj1, MenuItemSetting obj2) {
            if (obj1.m_itemType != obj2.m_itemType) {
                return false;
            }
            if (!ActionCommandMoniker.Equals(obj1.m_commandMoniker, obj2.m_commandMoniker)) {
                return false;
            }
            if (obj1.m_uiResource != obj2.m_uiResource) {
                return false;
            }
            if (obj1.m_shortcutKey != obj2.m_shortcutKey) {
                return false;
            }
            if (obj1.m_itemName != obj2.m_itemName) {
                return false;
            }
            if (obj1.m_subMenuList == null && obj2.m_subMenuList == null) {
                ;
            } else if (obj1.m_subMenuList == null || obj2.m_subMenuList == null) {
                return false;
            } else {
                if (obj1.m_subMenuList.Count != obj2.m_subMenuList.Count) {
                    return false;
                }
                for (int i = 0; i < obj1.m_subMenuList.Count; i++) {
                    if (!EqualsConfig(obj1.m_subMenuList[i], obj2.m_subMenuList[i])) {
                        return false;
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader      読み込み用クラス
        // 　　　　[in]obj         読み込み対象のオブジェクト（部分的に失敗したときnull）
        // 　　　　[in]parentName  親の項目名
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out MenuItemSetting obj, string parentName) {
            obj = null;
            bool success;

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.MenuSetting_MenuItemSetting, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }

            ItemType itemType = null;
            ActionCommandMoniker moniker = null;
            string monikerClass = null;
            string itemName = null;
            char shortcutKey = '\x00';
            List<MenuItemSetting> subMenuList = null;
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.MenuSetting_MenuItemSetting) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.MenuSetting_MenuItemType) {
                    itemType = ItemType.FromSerializedData(loader.StringValue);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.MenuSetting_MenuItemName) {
                    itemName = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.MenuSetting_MenuItemShortcut) {
                    string shortcutKeyStr = loader.StringValue;
                    shortcutKey = (shortcutKeyStr.Length > 0) ? shortcutKeyStr[0] : SHORTCUT_KEY_NONE;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.MenuSetting_MenuItemCommand) {
                    success = KeyItemSetting.LoadActionCommandMoniker(SettingTagType.EndObject, null, SettingTag.MenuSetting_MenuItemCommand, loader, out moniker, out monikerClass);
                    if (!success) {
                        // 読み込み失敗時はここで回復
                        moniker = null;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.MenuSetting_MenuItemSubMenuList) {
                    subMenuList = new List<MenuItemSetting>();
                    success = LoadSubmenu(loader, subMenuList, itemName);
                    if (!success) {
                        return false;
                    }
                }
            }

            // メニューを作成
            if (itemType == null) {
                loader.SetWarning(Resources.SettingLoader_LoadKeySettingKeyMenuFailed);
                obj = null;
            } else if (itemType == ItemType.SubMenu) {
                if (itemName == null || shortcutKey == '\x00' || subMenuList == null) {
                    loader.SetWarning(Resources.SettingLoader_LoadKeySettingKeyMenuFailed);
                    obj = null;
                } else {
                    obj = new MenuItemSetting(itemName, shortcutKey);
                    foreach (MenuItemSetting subMenu in subMenuList) {
                        obj.AddSubMenu(subMenu);
                    }
                }
            } else if (itemType == ItemType.Separator) {
                obj = new MenuItemSetting(ItemType.Separator);
            } else if (itemType == ItemType.MenuItem) {
                if (moniker == null) {
                    string dispParent = (parentName == null) ? "???" : parentName;
                    string dispShortcut = ConvertShortcutKeyDisplay(shortcutKey, "???");
                    string dispMoniker = (monikerClass == null) ? "???" : monikerClass;
                    loader.SetWarning(Resources.SettingLoader_LoadKeySettingMenuCommandFailed, dispParent, dispShortcut, dispMoniker);
                    obj = null;
                } else if (shortcutKey == '\x00') {
                    loader.SetWarning(Resources.SettingLoader_LoadKeySettingKeyMenuFailed);
                    obj = null;
                } else {
                    obj = new MenuItemSetting(moniker, shortcutKey, itemName);
                }
            } else {
                loader.SetWarning(Resources.SettingLoader_LoadKeySettingKeyMenuFailed);
                obj = null;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：ショートカットキーを表示名に変換する
        // 引　数：[in]shortcutKey  ショートカットキー
        // 　　　　[in]defautName   未設定(0x00)の場合のデフォルト
        // 戻り値：ショートカットキーの表示名
        //=========================================================================================
        private static string ConvertShortcutKeyDisplay(char shortcutKey, string defautName) {
            string displayName;
            if (shortcutKey == '\x00') {
                displayName = defautName;
            } else if (shortcutKey == SHORTCUT_KEY_NONE) {
                displayName = Resources.DlgMenuSetting_ShortcutNone;
            } else {
                displayName = "" + shortcutKey;
            }
            return displayName;
        }

        //=========================================================================================
        // 機　能：サブメニューの項目をファイルから読み込む
        // 引　数：[in]loader       読み込み用クラス
        // 　　　　[in]subMenuList  読み込んだサブメニューを返す配列
        // 　　　　[in]parentName   親のメニュー項目名
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSubmenu(SettingLoader loader, List<MenuItemSetting> subMenuList, string parentName) {
            bool success;
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.MenuSetting_MenuItemSubMenuList) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.MenuSetting_MenuItemSubMenu) {
                    MenuItemSetting obj;
                    success = LoadSetting(loader, out obj, parentName);
                    if (!success) {
                        return false;
                    }
                    if (obj != null) {              // エラー時はここで復活
                        subMenuList.Add(obj);
                    }
                    success = loader.ExpectTag(SettingTag.MenuSetting_MenuItemSubMenu, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                }
            }

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, MenuItemSetting obj) {
            saver.StartObject(SettingTag.MenuSetting_MenuItemSetting);

            saver.AddString(SettingTag.MenuSetting_MenuItemType, ItemType.ToSerializedData(obj.m_itemType));
            if (obj.m_itemName != null) {
                saver.AddString(SettingTag.MenuSetting_MenuItemName, obj.m_itemName);
            }
            saver.AddString(SettingTag.MenuSetting_MenuItemShortcut, "" + obj.m_shortcutKey);
            if (obj.m_commandMoniker != null) {
                saver.StartObject(SettingTag.MenuSetting_MenuItemCommand);
                KeyItemSetting.SaveActionCommandMoniker(saver, obj.m_commandMoniker);
                saver.EndObject(SettingTag.MenuSetting_MenuItemCommand);
            }

            if (obj.m_subMenuList != null) {
                saver.StartObject(SettingTag.MenuSetting_MenuItemSubMenuList);
                foreach (MenuItemSetting subMenu in obj.m_subMenuList) {
                    saver.StartObject(SettingTag.MenuSetting_MenuItemSubMenu);
                    SaveSetting(saver, subMenu);
                    saver.EndObject(SettingTag.MenuSetting_MenuItemSubMenu);
                }
                saver.EndObject(SettingTag.MenuSetting_MenuItemSubMenuList);
            }

            saver.EndObject(SettingTag.MenuSetting_MenuItemSetting);

            return true;
        }

        //=========================================================================================
        // プロパティ：項目の種類
        //=========================================================================================
        public ItemType Type {
            get {
                return m_itemType;
            }
        }

        //=========================================================================================
        // プロパティ：実行するコマンド
        //=========================================================================================
        public ActionCommandMoniker ActionCommandMoniker {
            get {
                return m_commandMoniker;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public UIResource UIResource {
            get {
                return m_uiResource;
            }
        }

        //=========================================================================================
        // プロパティ：項目を有効にする条件
        //=========================================================================================
        public UIEnableCondition UIEnableCondition {
            get {
                return m_uiResource.UIEnableCondition;;
            }
        }

        //=========================================================================================
        // プロパティ：ショートカットキー
        //=========================================================================================
        public char ShortcutKey {
            get {
                return m_shortcutKey;
            }
            set {
                m_shortcutKey = value;
            }
        }

        //=========================================================================================
        // プロパティ：メニューの表示名（入力値、入力がないときnull）
        //=========================================================================================
        public string ItemNameInput {
            get {
                return m_itemName;
            }
            set {
                m_itemName = value;
            }
        }

        //=========================================================================================
        // プロパティ：メニューの表示名（ショートカットを含む）
        //=========================================================================================
        public string ItemName {
            get {
                string itemName;
                if (m_itemName != null) {
                    itemName = m_itemName;
                } else {
                    itemName = m_uiResource.Hint;
                }
                if (m_shortcutKey != '*') {
                    itemName += "(&" + m_shortcutKey + ")";
                }
                if (m_uiResource != null && m_uiResource.IsOpenDialog) {
                    itemName += "...";
                }
                return itemName;
            }
        }

        //=========================================================================================
        // プロパティ：サブメニューのリスト
        //=========================================================================================
        public List<MenuItemSetting> SubMenuList {
            get {
                return m_subMenuList;
            }
        }

        //=========================================================================================
        // 列挙子：項目の種類
        //=========================================================================================
        public class ItemType {
            public static readonly ItemType MenuItem = new ItemType();       // メニュー項目
            public static readonly ItemType Separator = new ItemType();      // セパレータ
            public static readonly ItemType SubMenu = new ItemType();        // サブメニュー

            //=========================================================================================
            // 機　能：シリアライズされたデータからオブジェクトを復元する
            // 引　数：[in]serialized  シリアライズされたデータ
            // 戻り値：復元したショートカットの種類
            //=========================================================================================
            public static ItemType FromSerializedData(string serialized) {
                if (serialized == "MenuItem") {
                    return MenuItem;
                } else if (serialized == "Separator") {
                    return Separator;
                } else if (serialized == "SubMenu") {
                    return SubMenu;
                } else {
                    return null;
                }
            }

            //=========================================================================================
            // 機　能：オブジェクトからシリアライズされたデータを作成する
            // 引　数：[in]obj   ショートカットの種類
            // 戻り値：シリアライズされたデータ
            //=========================================================================================
            public static string ToSerializedData(ItemType obj) {
                if (obj == MenuItem) {
                    return "MenuItem";
                } else if (obj == Separator) {
                    return "Separator";
                } else if (obj == SubMenu) {
                    return "SubMenu";
                } else {
                    Program.Abort("未定義のItemType");
                    return null;
                }
            }
        }
    }
}