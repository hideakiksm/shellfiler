using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ShellFiler全体でのフォルダ履歴
    //=========================================================================================
    public class FolderHistoryWhole : ICloneable {
        // フォルダ履歴の最大記憶件数
        private int m_maxFolderHistoryCount;

        // フォルダ履歴のリスト（添字が大きいほど新しい）
        private List<PathHistoryItem> m_historyList = new List<PathHistoryItem>();

        // ディレクトリから履歴項目へのMap（GetDirectoryIndex()で作成）
        // ・ディレクトリは最後のセパレータを削除
        // ・Windowsでは小文字でのキーとする
        private Dictionary<string, PathHistoryItem> m_directoryIndex = new Dictionary<string,PathHistoryItem>();

        // Cloneをサポート

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FolderHistoryWhole() {
             m_maxFolderHistoryCount = Configuration.PathHistoryWholeMaxCount;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FolderHistoryWhole pathHistory = new FolderHistoryWhole();
            pathHistory.m_historyList = new List<PathHistoryItem>();
            foreach (PathHistoryItem item in m_historyList) {
                pathHistory.m_historyList.Add((PathHistoryItem)item.Clone());
            }
            return pathHistory;
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
#if !FREE_VERSION
            string fileName = DirectoryManager.FolderHistoryWhole;
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
        private static bool LoadSetting(SettingLoader loader, FolderHistoryWhole obj) {
            // ファイルがないときはデフォルト
            if (!File.Exists(loader.FileName)) {
                return true;
            }
            obj.m_directoryIndex.Clear();
            obj.m_historyList.Clear();

            // ファイルから読み込む
            bool success;
            success = loader.LoadSetting(false);
            if (!success) {
                return false;
            }

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.FolderHistory_FolderHistory, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FolderHistory_FolderHistory) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FolderHistory_HistoryItem) {
                    success = LoadHistoryItem(loader, obj);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.FolderHistory_HistoryItem, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：履歴の項目1件をファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadHistoryItem(SettingLoader loader, FolderHistoryWhole obj) {
            bool success;

            // タグを読み込む
            List<string> listText = new List<string>();
            success = loader.ExpectTag(SettingTag.FolderHistory_HistoryInfo, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            string folderName = null;
            string fileName = null;
            string fileSystem = null;
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FolderHistory_HistoryInfo) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FolderHistory_FolderName) {
                    folderName = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FolderHistory_FileName) {
                    fileName = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FolderHistory_FileSystem) {
                    fileSystem = loader.StringValue;
                }
            }

            // 内容を登録する
            if (folderName != null && fileName != null && fileSystem != null) {
                FileSystemID fileSystemId = FileSystemID.FromString(fileSystem, false);
                if (fileSystemId != null) {
                    obj.AddItem(folderName, fileName, fileSystemId);
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：設定の保存を行う
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：メインウィンドウのクローズ処理中に呼ばれる
        //=========================================================================================
        public void SaveSetting() {
#if !FREE_VERSION
            if (Configuration.Current.PathHistoryWholeSaveDisk) {
                string fileName = DirectoryManager.FolderHistoryWhole;
                SettingSaver saver = new SettingSaver(fileName);
                bool success = SaveSetting(saver, this);
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
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        private static bool SaveSetting(SettingSaver saver, FolderHistoryWhole obj) {
            bool success;

            saver.StartObject(SettingTag.FolderHistory_FolderHistory);

            // 古い方から新しい方へ保存（読み込み時に新しい方で上書き）
            foreach (PathHistoryItem historyItem in obj.m_historyList) {
                saver.StartObject(SettingTag.FolderHistory_HistoryItem);
                success = SaveHistoryItem(saver, historyItem);
                if (!success) {
                    return false;
                }
                saver.EndObject(SettingTag.FolderHistory_HistoryItem);
            }

            saver.EndObject(SettingTag.FolderHistory_FolderHistory);

            return saver.SaveSetting(false);
        }

        //=========================================================================================
        // 機　能：履歴の項目1件をファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        private static bool SaveHistoryItem(SettingSaver saver, PathHistoryItem obj) {
            saver.StartObject(SettingTag.FolderHistory_HistoryInfo);

            saver.AddString(SettingTag.FolderHistory_FolderName, obj.Directory);
            saver.AddString(SettingTag.FolderHistory_FileName, obj.FileName);
            saver.AddString(SettingTag.FolderHistory_FileSystem, obj.FileSystemID.StringId);

            saver.EndObject(SettingTag.FolderHistory_HistoryInfo);

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに保存された履歴を削除する
        // 引　数：なし
        // 戻り値：削除に成功したときtrue
        //=========================================================================================
        public bool DeleteSetting() {
            // ファイルがないときはそのまま
            string fileName = DirectoryManager.FolderHistoryWhole;
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
        // 機　能：すべての項目を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearAllHistory() {
            m_historyList.Clear();
            m_directoryIndex.Clear();
        }

        //=========================================================================================
        // 機　能：ディレクトリ名からディレクトリ名検索用のインデックス文字列を取得する
        // 引　数：[in]directory    取得するディレクトリ
        // 　　　　[in]ignoreCase   大文字小文字を無視するときtrue
        // 戻り値：ディレクトリ名検索用のインデックス文字列
        //=========================================================================================
        private string GetDirectoryIndex(string directory, bool ignoreCase) {
            directory = GenericFileStringUtils.RemoveLastDirectorySeparator(directory);
            if (ignoreCase) {
                directory = directory.ToLower();
            }
            return directory;
        }

        //=========================================================================================
        // 機　能：項目を追加する
        // 引　数：[in]directory    ディレクトリ
        // 　　　　[in]fileName     ファイル名
        // 　　　　[in]fileSystemId ファイルシステムのID
        // 戻り値：なし
        //=========================================================================================
        public void AddItem(string directory, string fileName, FileSystemID fileSystemId) {
            PathHistoryItem item = new PathHistoryItem(directory, fileName, fileSystemId);

            bool ignoreCase = FileSystemID.IgnoreCaseFolderPath(fileSystemId);
            directory = GetDirectoryIndex(directory, ignoreCase);

            // 既存項目にあるかどうかを確認
            if (m_directoryIndex.ContainsKey(directory)) {
                // 既存項目にある場合は削除
                PathHistoryItem oldItem = m_directoryIndex[directory];
                m_historyList.Remove(oldItem);
                m_directoryIndex.Remove(directory);
            }

            // 追加しようとしたが、最大件数の場合は古いものを削除
            if (m_historyList.Count >= m_maxFolderHistoryCount) {
                PathHistoryItem delItem = m_historyList[0];
                m_historyList.RemoveAt(0);
                string delDir = GetDirectoryIndex(delItem.Directory, FileSystemID.IgnoreCaseFolderPath(delItem.FileSystemID));
                m_directoryIndex.Remove(delDir);
            }

            // 追加
            m_historyList.Add(item);
            m_directoryIndex[directory] = item;
        }

        //=========================================================================================
        // 機　能：フォルダ履歴の項目を取得する
        // 引　数：[in]directory    取得するディレクトリ
        // 　　　　[in]ignoreCase   大文字小文字を無視するときtrue
        // 戻り値：パスヒストリの項目（見つからないときはnull）
        //=========================================================================================
        public PathHistoryItem GetHistoryItem(string directory, bool ignoreCase) {
            directory = GetDirectoryIndex(directory, ignoreCase);
            if (m_directoryIndex.ContainsKey(directory)) {
                return m_directoryIndex[directory];
            } else {
                return null;
            }
        }

        //=========================================================================================
        // プロパティ：フォルダ履歴のリスト（添字が大きいほど新しい）
        //=========================================================================================
        public List<PathHistoryItem> HistoryList {
            get {
                return m_historyList;
            }
        }
    }
}
