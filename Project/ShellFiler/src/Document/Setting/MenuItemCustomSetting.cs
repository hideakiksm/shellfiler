using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Dialog.KeyOption;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.Open;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.Setting;
using ShellFiler.Command.FileList.Window;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Cursor;
using ShellFiler.Command.FileViewer.Edit;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.Command.GraphicsViewer;
using ShellFiler.Command.GraphicsViewer.Edit;
using ShellFiler.Command.GraphicsViewer.File;
using ShellFiler.Command.GraphicsViewer.View;
using ShellFiler.Command.GraphicsViewer.Filter;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：メニューのカスタマイズ情報
    //=========================================================================================
    public class MenuItemCustomSetting : ICloneable {
        // メニュー項目の並び
        private List<MenuItemCustomRoot> m_customMenuList = new List<MenuItemCustomRoot>();

        // 定義済み項目の表示スイッチ（項目がないものはON）
        private Dictionary<string, bool> m_definedMenuFlag = new Dictionary<string, bool>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MenuItemCustomSetting() {
        }

        //=========================================================================================
        // 機　能：設定を初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(List<MenuItemSetting> definedMenu) {
            m_customMenuList.Clear();
            m_definedMenuFlag.Clear();
            for (int i = 0; i < definedMenu.Count; i++) {
                m_definedMenuFlag.Add(definedMenu[i].ItemNameInput, true);
            }
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            MenuItemCustomSetting obj = new MenuItemCustomSetting();
            for (int i = 0; i < m_customMenuList.Count; i++) {
                obj.m_customMenuList.Add((MenuItemCustomRoot)(m_customMenuList[i].Clone()));
            }
            foreach (string definedKey in m_definedMenuFlag.Keys) {
                obj.m_definedMenuFlag.Add(definedKey, m_definedMenuFlag[definedKey]);
            }
            return obj;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(MenuItemCustomSetting obj1, MenuItemCustomSetting obj2) {
            if (obj1.m_customMenuList.Count != obj2.m_customMenuList.Count) {
                return false;
            }
            for (int i = 0; i < obj1.m_customMenuList.Count; i++) {
                if (!MenuItemCustomRoot.EqualsConfig(obj1.m_customMenuList[i], obj2.m_customMenuList[i])) {
                    return false;
                }
            }
            if (obj1.m_definedMenuFlag.Count != obj2.m_definedMenuFlag.Count) {
                return false;
            }
            foreach (string key in obj1.m_definedMenuFlag.Keys) {
                if (!obj2.m_definedMenuFlag.ContainsKey(key)) {
                    return false;
                }
                if (obj1.m_definedMenuFlag[key] != obj2.m_definedMenuFlag[key]) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader      読み込み用クラス
        // 　　　　[in]definedList 定義済みメニュー
        // 　　　　[out]obj        読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, List<MenuItemSetting> definedList, out MenuItemCustomSetting obj) {
            bool success;
            obj = new MenuItemCustomSetting();
            obj.Initialize(definedList);

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.MenuSetting_MenuCustom, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.MenuSetting_MenuCustom) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.MenuSetting_MenuCustomRootList) {
                    List<MenuItemCustomRoot> list = new List<MenuItemCustomRoot>();
                    success = LoadSettingRootList(loader, list);
                    if (!success) {
                        return false;
                    }
                    obj.m_customMenuList = list;
                    success = loader.ExpectTag(SettingTag.MenuSetting_MenuCustomRootList, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.MenuSetting_MenuCustomDefinedOffList) {
                    success = LoadSettingDefinedFlag(loader, definedList, obj.m_definedMenuFlag);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.MenuSetting_MenuCustomDefinedOffList, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：カスタム設定のリスト項目を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]list    読み込んだ結果を返すリスト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSettingRootList(SettingLoader loader, List<MenuItemCustomRoot> list) {
            while (true) {
                bool success;
                bool fit;
                success = loader.PeekTag(SettingTag.MenuSetting_MenuCustomRootList, SettingTagType.EndObject, out fit);
                if (!success) {
                    return success;
                }
                if (fit) {
                    return true;
                }

                MenuItemCustomRoot item;
                success = MenuItemCustomRoot.LoadSetting(loader, out item);
                if (!success) {
                    return false;
                }
                if (item != null) {
                    list.Add(item);             // エラーの場合は登録を回避
                }
            }
        }

        //=========================================================================================
        // 機　能：カスタム設定の定義済み項目表示フラグを読み込む
        // 引　数：[in]loader      読み込み用クラス
        // 　　　　[in]definedList 定義済みメニュー
        // 　　　　[in]list        読み込んだ結果を返すリスト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSettingDefinedFlag(SettingLoader loader, List<MenuItemSetting> definedList, Dictionary<string, bool> list) {
            while (true) {
                bool success;
                bool fit;
                success = loader.PeekTag(SettingTag.MenuSetting_MenuCustomDefinedOffList, SettingTagType.EndObject, out fit);
                if (!success) {
                    return success;
                }
                if (fit) {
                    return true;
                }

                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.StringValue && tagName == SettingTag.MenuSetting_MenuCustomDefinedOff) {
                    string key = loader.StringValue;
                    if (list.ContainsKey(key)) {
                        list[key] = false;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, MenuItemCustomSetting obj) {
            saver.StartObject(SettingTag.MenuSetting_MenuCustom);

            saver.StartObject(SettingTag.MenuSetting_MenuCustomRootList);
            foreach (MenuItemCustomRoot item in obj.m_customMenuList) {
                MenuItemCustomRoot.SaveSetting(saver, item);
            }
            saver.EndObject(SettingTag.MenuSetting_MenuCustomRootList);

            saver.StartObject(SettingTag.MenuSetting_MenuCustomDefinedOffList);
            foreach (string key in obj.m_definedMenuFlag.Keys) {
                if (!obj.m_definedMenuFlag[key]) {
                    saver.AddString(SettingTag.MenuSetting_MenuCustomDefinedOff, key);
                }
            }
            saver.EndObject(SettingTag.MenuSetting_MenuCustomDefinedOffList);

            saver.EndObject(SettingTag.MenuSetting_MenuCustom);

            return true;
        }

        //=========================================================================================
        // 機　能：設定をリセットする
        // 引　数：[in]newCustomList    新しいカスタム項目
        // 　　　　[in]definedMenuFlag  定義済み項目を表示するかどうかの設定
        // 戻り値：なし
        //=========================================================================================
        public void ResetCustomMenuSetting(List<MenuItemCustomRoot> newCustomList, Dictionary<string, bool> definedMenuFlag) {
            m_customMenuList = newCustomList;
            m_definedMenuFlag = definedMenuFlag;
        }

        //=========================================================================================
        // 機　能：メニューを表示する必要があるかどうかを返す
        // 引　数：[in]menuName   調べるメニュー
        // 戻り値：メニューを表示するときtrue
        //=========================================================================================
        public bool IsDisplayMenu(string menuName) {
            if (m_definedMenuFlag.ContainsKey(menuName)) {
                return m_definedMenuFlag[menuName];
            } else {
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：メニュー項目の並び
        //=========================================================================================
        public List<MenuItemCustomRoot> CustomRootList {
            get {
                return m_customMenuList;
            }
        }
    }
}
