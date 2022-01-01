using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Dialog.KeyOption;
using ShellFiler.Command;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：メニューのカスタマイズ情報（ルートメニューの項目1つ）
    //=========================================================================================
    public class MenuItemCustomRoot : ICloneable {
        // メニュー設定
        private MenuItemSetting m_menuSetting;
        
        // ルートに表示するときtrue
        private bool m_displayRoot = true;

        // 直前に表示される項目名（未確定のとき""で、[実行]の次に表示）
        private string m_prevItemName = "";

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]displayRoot   ルートに表示するときtrue
        // 　　　　[in]prevItemName  直前に表示される項目名（未確定のとき""で、[実行]の次に表示）
        // 　　　　
        // 戻り値：なし
        //=========================================================================================
        public MenuItemCustomRoot(bool displayRoot, string prevItemName, MenuItemSetting menuSetting) {
            m_displayRoot = displayRoot;
            m_prevItemName = prevItemName;
            m_menuSetting = menuSetting;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            MenuItemCustomRoot obj = new MenuItemCustomRoot(m_displayRoot, m_prevItemName, null);
            obj.m_menuSetting = (MenuItemSetting)(m_menuSetting.Clone());
            return obj;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(MenuItemCustomRoot obj1, MenuItemCustomRoot obj2) {
            if (!MenuItemSetting.EqualsConfig(obj1.m_menuSetting, obj2.m_menuSetting)) {
                return false;
            }
            if (obj1.m_displayRoot != obj2.m_displayRoot) {
                return false;
            }
            if (obj1.m_prevItemName != obj2.m_prevItemName) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト（部分的に失敗したときnull）
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out MenuItemCustomRoot obj) {
            obj = null;
            bool success;
            MenuItemSetting menuSetting = null;
            BooleanFlag displayRoot = null;
            string prevItemName = null;

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.MenuSetting_MenuCustomRoot, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.MenuSetting_MenuCustomRoot) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.MenuSetting_MenuCustomSetting) {
                    success = MenuItemSetting.LoadSetting(loader, out menuSetting, null);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.MenuSetting_MenuCustomSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.MenuSetting_MenuCustomDisplayRoot) {
                    displayRoot = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.MenuSetting_MenuCustomPrevItemName) {
                    prevItemName = loader.StringValue;
                }
            }
            if (menuSetting == null || displayRoot == null || prevItemName == null) {
                obj = null;
                loader.SetWarning(Resources.SettingLoader_LoadMenuCustomRoot);
                return true;
            }
            obj = new MenuItemCustomRoot(displayRoot.Value, prevItemName, menuSetting);
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, MenuItemCustomRoot obj) {
            saver.StartObject(SettingTag.MenuSetting_MenuCustomRoot);

            saver.StartObject(SettingTag.MenuSetting_MenuCustomSetting);
            MenuItemSetting.SaveSetting(saver, obj.m_menuSetting);
            saver.EndObject(SettingTag.MenuSetting_MenuCustomSetting);
            saver.AddBool(SettingTag.MenuSetting_MenuCustomDisplayRoot, obj.m_displayRoot);
            saver.AddString(SettingTag.MenuSetting_MenuCustomPrevItemName, obj.m_prevItemName);

            saver.EndObject(SettingTag.MenuSetting_MenuCustomRoot);

            return true;
        }

        //=========================================================================================
        // プロパティ：ルートに表示するときtrue
        //=========================================================================================
        public bool DisplayRoot {
            get {
                return m_displayRoot;
            }
            set {
                m_displayRoot = value;
            }
        }

        //=========================================================================================
        // プロパティ：直前に表示される項目名（未確定のとき""で、[実行]の次に表示）
        //=========================================================================================
        public string PrevItemName {
            get {
                return m_prevItemName;
            }
            set {
                m_prevItemName = value;
            }
        }

        //=========================================================================================
        // プロパティ：メニュー設定
        //=========================================================================================
        public MenuItemSetting MenuSetting {
            get {
                return m_menuSetting;
            }
            set {
                m_menuSetting = value;
            }
        }
    }
}
