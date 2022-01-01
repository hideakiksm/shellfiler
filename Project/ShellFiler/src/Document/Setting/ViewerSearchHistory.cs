using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイルビュアーでの検索文字列ヒストリ
    //=========================================================================================
    class ViewerSearchHistory {
        // 最大履歴件数
        private int m_maxHistoryCount;

        // テキストビューアの検索文字列のヒストリ（末尾ほど新しい）
        private List<string> m_textSearchHistory = new List<string>();
        
        // ダンプビューアの検索文字列のヒストリ（末尾ほど新しい）
        private List<string> m_dumpSearchHistory = new List<string>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ViewerSearchHistory() {
            LoadData();
            m_maxHistoryCount = Configuration.ViewerSearchHistoryMaxCount;
        }
        
        //=========================================================================================
        // 機　能：データを読み込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void LoadData() {
#if !FREE_VERSION
            string fileName = DirectoryManager.ViewerSearchHistorySetting;
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
        private static bool LoadSettingInternal(SettingLoader loader, ViewerSearchHistory obj) {
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
            List<string> listText = new List<string>();
            success = loader.ExpectTag(SettingTag.ViewerSearchHistory_ViewerSearchHistory, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            List<string> textList = new List<string>();
            List<string> dumpList = new List<string>();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.ViewerSearchHistory_ViewerSearchHistory) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.ViewerSearchHistory_SearchTextList) {
                    while (true) {
                        success = loader.GetTag(out tagName, out tagType);
                        if (!success) {
                            return false;
                        }
                        if (tagType == SettingTagType.EndObject && tagName == SettingTag.ViewerSearchHistory_ViewerSearchHistory) {
                            return false;
                        } else if (tagType == SettingTagType.EndObject && tagName == SettingTag.ViewerSearchHistory_SearchTextList) {
                            break;
                        } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.ViewerSearchHistory_SearchText) {
                            textList.Add(loader.StringValue);
                        }
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.ViewerSearchHistory_SearchDumpList) {
                    while (true) {
                        success = loader.GetTag(out tagName, out tagType);
                        if (!success) {
                            return false;
                        }
                        if (tagType == SettingTagType.EndObject && tagName == SettingTag.ViewerSearchHistory_ViewerSearchHistory) {
                            return false;
                        } else if (tagType == SettingTagType.EndObject && tagName == SettingTag.ViewerSearchHistory_SearchDumpList) {
                            break;
                        } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.ViewerSearchHistory_SearchDump) {
                            dumpList.Add(loader.StringValue);
                        }
                    }
                }
            }

            // 内容を置き換える
            obj.m_textSearchHistory.Clear();
            obj.m_textSearchHistory.AddRange(textList);
            obj.m_dumpSearchHistory.Clear();
            obj.m_dumpSearchHistory.AddRange(dumpList);

            return true;
        }
        
        //=========================================================================================
        // 機　能：データを書き込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SaveData() {
#if !FREE_VERSION
            if (Configuration.Current.ViewerSearchHistorySaveDisk) {
                string fileName = DirectoryManager.ViewerSearchHistorySetting;
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
            saver.StartObject(SettingTag.ViewerSearchHistory_ViewerSearchHistory);
            
            // テキストヒストリ
            saver.StartObject(SettingTag.ViewerSearchHistory_SearchTextList);
            foreach (string item in m_textSearchHistory) {
                saver.AddString(SettingTag.ViewerSearchHistory_SearchText, item);
            }
            saver.EndObject(SettingTag.ViewerSearchHistory_SearchTextList);

            // ダンプヒストリ
            saver.StartObject(SettingTag.ViewerSearchHistory_SearchDumpList);
            foreach (string item in m_dumpSearchHistory) {
                saver.AddString(SettingTag.ViewerSearchHistory_SearchDump, item);
            }
            saver.EndObject(SettingTag.ViewerSearchHistory_SearchDumpList);

            saver.EndObject(SettingTag.ViewerSearchHistory_ViewerSearchHistory);

            return saver.SaveSetting(false);
        }
        
        //=========================================================================================
        // 機　能：ファイルに保存された履歴を削除する
        // 引　数：なし
        // 戻り値：削除に成功したときtrue
        //=========================================================================================
        public bool DeleteSetting() {
            // ファイルがないときはそのまま
            string fileName = DirectoryManager.ViewerSearchHistorySetting;
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
        // 機　能：テキストビューアのヒストリに検索文字列を追加する
        // 引　数：[in]word   新しい検索文字列
        // 戻り値：なし
        //=========================================================================================
        public void AddTextSearchWord(string word) {
            AddSearchWord(m_textSearchHistory, word);
        }

        //=========================================================================================
        // 機　能：ダンプビューアのヒストリに検索文字列を追加する
        // 引　数：[in]word   新しい検索文字列
        // 戻り値：なし
        //=========================================================================================
        public void AddDumpSearchWord(string word) {
            AddSearchWord(m_dumpSearchHistory, word);
        }

        //=========================================================================================
        // 機　能：ヒストリに検索文字列を追加する
        // 引　数：[in]history  対象となるヒストリ
        // 　　　　[in]word     新しい検索文字列
        // 戻り値：なし
        //=========================================================================================
        private void AddSearchWord(List<string> history, string word) {
            for (int i = 0; i < history.Count; i++) {
                if (history[i] == word) {
                    history.RemoveAt(i);
                    break;
                }
            }
            history.Add(word);
            if (history.Count > m_maxHistoryCount) {
                history.RemoveAt(0);
            }
        }

        //=========================================================================================
        // 機　能：テキストビューアのヒストリの検索文字列をすべてクリアする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearTextSearchWord() {
            m_textSearchHistory.Clear();
        }

        //=========================================================================================
        // 機　能：ダンプビューアのヒストリの検索文字列をすべてクリアする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearDumpSearchWord() {
            m_dumpSearchHistory.Clear();
        }

        //=========================================================================================
        // プロパティ：テキストビューアの検索文字列のヒストリ
        //=========================================================================================
        public List<string> TextSearchHistory {
            get {
                return m_textSearchHistory;
            }
        }

        //=========================================================================================
        // プロパティ：ダンプビューアの検索文字列のヒストリ
        //=========================================================================================
        public List<string> DumpSearchHistory {
            get {
                return m_dumpSearchHistory;
            }
        }
    }
}
