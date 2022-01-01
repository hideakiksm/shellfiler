using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：コマンドの入力履歴
    //=========================================================================================
    public class CommandHistory {
        // コマンドヒストリの最大記憶件数
        private int m_maxCount;

        // コマンドの入力履歴のリスト
        private List<CommandHistoryItem> m_historyItemList = new List<CommandHistoryItem>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CommandHistory() {
            m_maxCount = Configuration.CommandHistoryMaxCount;
            LoadData();
        }
        
        //=========================================================================================
        // 機　能：データを読み込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void LoadData() {
#if !FREE_VERSION
            string fileName = DirectoryManager.CommandHistorySetting;
            SettingLoader loader = new SettingLoader(fileName);
            bool success = LoadSettingInternal(loader, this);
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
        private static bool LoadSettingInternal(SettingLoader loader, CommandHistory obj) {
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
            success = loader.ExpectTag(SettingTag.CommandHistory_CommandHistory, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            List<CommandHistoryItem> itemList = new List<CommandHistoryItem>();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.CommandHistory_CommandHistory) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.CommandHistory_Item) {
                    CommandHistoryItem item;
                    success = LoadHistoryItem(loader, out item);
                    if (!success) {
                        return false;
                    }
                    itemList.Add(item);
                }
            }
            obj.m_historyItemList.Clear();
            obj.m_historyItemList.AddRange(itemList);
            return true;
        }

        //=========================================================================================
        // 機　能：パスワードの項目を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadHistoryItem(SettingLoader loader, out CommandHistoryItem obj) {
            bool success;
            obj = null;
            string command = null;
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.CommandHistory_Item) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.CommandHistory_Command) {
                    command = loader.StringValue;
                }
            }
            if (command == null) {
                return false;
            }
            obj = new CommandHistoryItem(command);
            return true;
        }

        //=========================================================================================
        // 機　能：データを書き込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SaveData() {
#if !FREE_VERSION
            if (Configuration.Current.CommandHistorySaveDisk) {
                string fileName = DirectoryManager.CommandHistorySetting;
                SettingSaver saver = new SettingSaver(fileName);
                bool success = SaveSettingInternal(saver);
                if (!success) {
                    InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotSaveSetting, fileName);
                }
            } else {
                DeleteSetting();
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        private bool SaveSettingInternal(SettingSaver saver) {
            saver.StartObject(SettingTag.CommandHistory_CommandHistory);
            foreach (CommandHistoryItem item in m_historyItemList) {
                saver.StartObject(SettingTag.CommandHistory_Item);
                saver.AddString(SettingTag.CommandHistory_Command, item.CommandString);
                saver.EndObject(SettingTag.CommandHistory_Item);
            }
            saver.EndObject(SettingTag.CommandHistory_CommandHistory);

            return saver.SaveSetting(false);
        }

        //=========================================================================================
        // 機　能：ファイルに保存された履歴を削除する
        // 引　数：なし
        // 戻り値：削除に成功したときtrue
        //=========================================================================================
        public bool DeleteSetting() {
            // ファイルがないときはそのまま
            string fileName = DirectoryManager.CommandHistorySetting;
            if (!File.Exists(fileName)) {
                return true;
            }
            try {
                File.Delete(fileName);
            } catch (Exception) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：項目を追加する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void AddHistory(CommandHistoryItem item) {
            for (int i = m_historyItemList.Count - 1; i >= 0; i--) {
                if (item.EqualsCommand(m_historyItemList[i])) {
                    // 同じものがあったときは最新位置と入れ替え
                    for (int j = i; j < m_historyItemList.Count - 1; j++) {
                        m_historyItemList[j] = m_historyItemList[j + 1];
                    }
                    m_historyItemList[m_historyItemList.Count - 1] = item;
                    return;
                }
            }
            // 同じもがなかったときは最後に追加
            if (m_historyItemList.Count < m_maxCount) {
                m_historyItemList.Add(item);
            } else {
                m_historyItemList.RemoveAt(0);
                m_historyItemList.Add(item);
            }
        }

        //=========================================================================================
        // 機　能：すべての項目を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearAllHistory() {
            m_historyItemList.Clear();
        }

        //=========================================================================================
        // プロパティ：コマンドの入力履歴
        //=========================================================================================
        public List<CommandHistoryItem> ItemList {
            get {
                return m_historyItemList;
            }
        }
    }
}
