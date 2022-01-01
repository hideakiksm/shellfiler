using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイル一覧ビューのモード自動切り替えの設定
    //=========================================================================================
    public class FileListViewModeAutoSetting {
        // 設定内容
        private List<ModeEntry> m_folderSetting = new List<ModeEntry>();

        //=========================================================================================
        // 機　能：コンストラクタ（デフォルト設定で初期化する）
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileListViewModeAutoSetting() {
        }
    
        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileListViewModeAutoSetting obj = new FileListViewModeAutoSetting();
            for (int i = 0; i < m_folderSetting.Count; i++) {
                obj.m_folderSetting.Add(new ModeEntry(m_folderSetting[i].FolderName, m_folderSetting[i].ViewMode));
            }
            return obj;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileListViewModeAutoSetting obj1, FileListViewModeAutoSetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_folderSetting.Count != obj2.m_folderSetting.Count) {
                return false;
            }
            for (int i = 0; i < obj1.m_folderSetting.Count; i++) {
                if (obj1.m_folderSetting[i].FolderName != obj2.m_folderSetting[i].FolderName) {
                    return false;
                }
                if (!FileListViewMode.EqualsConfig(obj1.m_folderSetting[i].ViewMode, obj2.m_folderSetting[i].ViewMode)) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out FileListViewModeAutoSetting obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.FileListViewMode_FileListViewModeAuto, SettingTagType.BeginObject, out fit);
            if (!success) {
                return false;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            bool hasError = false;
            obj = new FileListViewModeAutoSetting();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileListViewMode_FileListViewModeAuto) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileListViewMode_FileListViewModeItem) {
                    ModeEntry entry;
                    success = LoadSettingEntry(loader, SettingTag.FileListViewMode_FileListViewModeItem, out entry);
                    if (!success) {
                        return false;
                    }
                    if (entry == null) {
                        hasError = true;
                    }
                }
            }
            if (hasError) {
                obj = null;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルからエントリ１件分を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]endTag  終了タグの期待値
        // 　　　　[in]obj     読み込み対象のオブジェクト（フィルターのどれかにエラーがあった場合はnull）
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSettingEntry(SettingLoader loader, SettingTag endTag, out ModeEntry obj) {
            bool success;
            obj = new ModeEntry(null, null);
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == endTag) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileListViewMode_FileListViewModeFolder) {
                    obj.FolderName = loader.StringValue;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileListViewMode_FileListViewModeSetting) {
                    FileListViewMode viewMode;
                    success = FileListViewMode.LoadSetting(loader, out viewMode);
                    if (!success) {
                        return false;
                    }
                    obj.ViewMode = viewMode;
                    success = loader.ExpectTag(SettingTag.FileListViewMode_FileListViewModeSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                }
            }
            if (obj.FolderName == null || obj.ViewMode == null) {
                obj = null;
            }
            return true;
        }
            
        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, FileListViewModeAutoSetting obj) {
            if (obj == null) {
                return true;
            }
            saver.StartObject(SettingTag.FileListViewMode_FileListViewModeAuto);
            for (int i = 0; i < obj.m_folderSetting.Count; i++) {
                saver.StartObject(SettingTag.FileListViewMode_FileListViewModeItem);
                saver.AddString(SettingTag.FileListViewMode_FileListViewModeFolder, obj.m_folderSetting[i].FolderName);
                saver.StartObject(SettingTag.FileListViewMode_FileListViewModeSetting);
                FileListViewMode.SaveSetting(saver,  obj.m_folderSetting[i].ViewMode);
                saver.EndObject(SettingTag.FileListViewMode_FileListViewModeSetting);
                saver.EndObject(SettingTag.FileListViewMode_FileListViewModeItem);
            }
            saver.EndObject(SettingTag.FileListViewMode_FileListViewModeAuto);
            return true;
        }

        //=========================================================================================
        // プロパティ：設定内容
        //=========================================================================================
        public List<ModeEntry> FolderSetting {
            get {
                return m_folderSetting;
            }
        }

        //=========================================================================================
        // クラス：登録内容のエントリ
        //=========================================================================================
        public class ModeEntry {
            // フォルダ名
            private string m_folderName;

            // 表示モード
            private FileListViewMode m_viewMode;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]folderName  フォルダ名
            // 　　　　[in]viewMode    表示モード
            // 戻り値：なし
            //=========================================================================================
            public ModeEntry(string folderName, FileListViewMode viewMode) {
                m_folderName = folderName;
                m_viewMode = viewMode;
            }

            //=========================================================================================
            // 機　能：クローンを作成する
            // 引　数：なし
            // 戻り値：作成したクローン
            //=========================================================================================
            public object Clone() {
                ModeEntry obj = new ModeEntry(m_folderName, (FileListViewMode)(m_viewMode.Clone()));
                return obj;
            }

            //=========================================================================================
            // プロパティ：フォルダ名
            //=========================================================================================
            public string FolderName {
                get {
                    return m_folderName;
                }
                set {
                    m_folderName = value;
                }
            }

            //=========================================================================================
            // プロパティ：表示モード
            //=========================================================================================
            public FileListViewMode ViewMode {
                get {
                    return m_viewMode;
                }
                set {
                    m_viewMode = value;
                }
            }
        }
    }
}
