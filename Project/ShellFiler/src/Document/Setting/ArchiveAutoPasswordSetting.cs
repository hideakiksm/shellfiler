using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：アーカイブ内の自動パスワード入力の設定
    //=========================================================================================
    public class ArchiveAutoPasswordSetting {
        // 最大設定可能数
        public const int MAX_PASSWORD_SETTING_COUNT = 99;

        // クリップボード上の最大設定可能数
        public const int MAX_PASSWORD_CLIPBOARD_COUNT = 10;

        // 自動パスワードの設定
        private List<ArchiveAutoPasswordItem> m_itemList = new List<ArchiveAutoPasswordItem>();
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ArchiveAutoPasswordSetting() {
            LoadData();
        }
        
        //=========================================================================================
        // 機　能：データを読み込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void LoadData() {
#if !FREE_VERSION
            string fileName = DirectoryManager.ArchiveAutoPasswordSetting;
            SettingLoader loader = new SettingLoader(fileName);
            bool success = LoadSetting(loader, this);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotLoadSetting, fileName, loader.ErrorDetail);
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSetting(SettingLoader loader, ArchiveAutoPasswordSetting obj) {
            // ファイルがないときはそのまま
            if (!File.Exists(loader.FileName)) {
                return true;
            }

            // ファイルから読み込む
            bool success;
            success = loader.LoadSetting(false);
            if (!success) {
                return false;
            }

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.ArchivePassword_ArchivePassword, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            List<ArchiveAutoPasswordItem> itemList = new List<ArchiveAutoPasswordItem>();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.ArchivePassword_ArchivePassword) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.ArchivePassword_Item) {
                    ArchiveAutoPasswordItem item;
                    success = LoadPasswordItem(loader, out item);
                    if (!success) {
                        return false;
                    }
                    itemList.Add(item);
                }
            }
            obj.m_itemList.Clear();
            obj.m_itemList.AddRange(itemList);
            return true;
        }

        //=========================================================================================
        // 機　能：パスワードの項目を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadPasswordItem(SettingLoader loader, out ArchiveAutoPasswordItem obj) {
            bool success;
            obj = null;
            string displayName = null;
            string password = null;
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.ArchivePassword_Item) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.ArchivePassword_DisplayName) {
                    displayName = loader.StringValue;
                } else if (tagType == SettingTagType.PasswordValue && tagName == SettingTag.ArchivePassword_Password) {
                    password = loader.PasswordValue;
                }
            }
            if (displayName == null || password == null) {
                return false;
            }
            obj = new ArchiveAutoPasswordItem(displayName, password);
            return true;
        }

        //=========================================================================================
        // 機　能：データを書き込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SaveData() {
#if !FREE_VERSION
            string fileName = DirectoryManager.ArchiveAutoPasswordSetting;
            SettingSaver saver = new SettingSaver(fileName);
            bool success = SaveSetting(saver);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotSaveSetting, fileName);
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        private bool SaveSetting(SettingSaver saver) {
            saver.StartObject(SettingTag.ArchivePassword_ArchivePassword);
            foreach (ArchiveAutoPasswordItem item in m_itemList) {
                saver.StartObject(SettingTag.ArchivePassword_Item);
                saver.AddString(SettingTag.ArchivePassword_DisplayName, item.DisplayName);
                saver.AddPassword(SettingTag.ArchivePassword_Password, item.Password);
                saver.EndObject(SettingTag.ArchivePassword_Item);
            }
            saver.EndObject(SettingTag.ArchivePassword_ArchivePassword);

            return saver.SaveSetting(false);
        }

        //=========================================================================================
        // 機　能：項目を追加する
        // 引　数：[in]item  設定項目
        // 戻り値：なし
        //=========================================================================================
        public void AddItem(ArchiveAutoPasswordItem item) {
            m_itemList.Add(item);
        }

        //=========================================================================================
        // 機　能：指定した表示名が重複していないことを確認する
        // 引　数：[in]displayName  表示名
        // 戻り値：重複していないときtrue
        //=========================================================================================
        public bool CheckUniqueDisplayName(string displayName) {
            foreach (ArchiveAutoPasswordItem item in m_itemList) {
                if (item.DisplayName == displayName) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：自動パスワードの設定一覧
        //=========================================================================================
        public List<ArchiveAutoPasswordItem> AutoPasswordItemList {
            get {
                return m_itemList;
            }
        }

        //=========================================================================================
        // プロパティ：設定項目をこれ以上記憶できないときtrue
        //=========================================================================================
        public bool ItemFull {
            get {
                return (m_itemList.Count >= MAX_PASSWORD_SETTING_COUNT);
            }
        }
    }
}
