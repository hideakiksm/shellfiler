using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.FileTask;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイルコンペアの設定
    //=========================================================================================
    public class FileCompareSetting : ICloneable {
        // 更新時刻の比較モード
        private FileTimeCompareMode m_fileTimeMode = FileTimeCompareMode.MarkExactly;
        
        // ファイルサイズの比較モード
        private FileSizeCompareMode m_fileSizeMode = FileSizeCompareMode.MarkExactly;

        // フォルダを除外
        private bool m_exceptFolder = true;

        // 内容を判別
        private bool m_checkContents = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileCompareSetting() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            return MemberwiseClone();
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileCompareSetting obj1, FileCompareSetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_fileTimeMode != obj2.m_fileTimeMode) {
                return false;
            }
            if (obj1.m_fileSizeMode != obj2.m_fileSizeMode) {
                return false;
            }
            if (obj1.m_exceptFolder != obj2.m_exceptFolder) {
                return false;
            }
            if (obj1.m_checkContents != obj2.m_checkContents) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out FileCompareSetting obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.FileCompareSetting_FileCompareSetting, SettingTagType.BeginObject, out fit);
            if (!success) {
                return false;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new FileCompareSetting();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileCompareSetting_FileCompareSetting) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileCompareSetting_FileTimeMode) {
                    string stringValue = loader.StringValue;
                    // 更新時刻の比較モード
                    if (stringValue == "MarkExactly") {
                        obj.m_fileTimeMode = FileTimeCompareMode.MarkExactly;
                    } else if (stringValue == "MarkNewer") {
                        obj.m_fileTimeMode = FileTimeCompareMode.MarkNewer;
                    } else if (stringValue == "MarkOlder") {
                        obj.m_fileTimeMode = FileTimeCompareMode.MarkOlder;
                    } else if (stringValue == "Ignore") {
                        obj.m_fileTimeMode = FileTimeCompareMode.Ignore;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileCompareSetting_FileSizeMode) {
                    // ファイルサイズの比較モード
                    string stringValue = loader.StringValue;
                    if (stringValue == "MarkExactly") {
                        obj.m_fileSizeMode = FileSizeCompareMode.MarkExactly;
                    } else if (stringValue == "MarkBigger") {
                        obj.m_fileSizeMode = FileSizeCompareMode.MarkBigger;
                    } else if (stringValue == "MarkSmaller") {
                        obj.m_fileSizeMode = FileSizeCompareMode.MarkSmaller;
                    } else if (stringValue == "Ignore") {
                        obj.m_fileSizeMode = FileSizeCompareMode.Ignore;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCompareSetting_ExceptFolder) {
                    // フォルダを除外
                    obj.m_exceptFolder = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCompareSetting_CheckContents) {
                    // 内容を判別
                    obj.m_checkContents = loader.BoolValue;
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
        public static bool SaveSetting(SettingSaver saver, FileCompareSetting obj) {
            if (obj == null) {
                return true;
            }

            // 更新時刻の比較モード
            string strFileTimeMode = null;
            if (obj.m_fileTimeMode == FileTimeCompareMode.MarkExactly) {
                strFileTimeMode = "MarkExactly";
            } else if (obj.m_fileTimeMode == FileTimeCompareMode.MarkNewer) {
                strFileTimeMode = "MarkNewer";
            } else if (obj.m_fileTimeMode == FileTimeCompareMode.MarkOlder) {
                strFileTimeMode = "MarkOlder";
            } else if (obj.m_fileTimeMode == FileTimeCompareMode.Ignore) {
                strFileTimeMode = "Ignore";
            }

            // ファイルサイズの比較モード
            string strFileSizeMode = null;
            if (obj.m_fileSizeMode == FileSizeCompareMode.MarkExactly) {
                strFileSizeMode = "MarkExactly";
            } else if (obj.m_fileSizeMode == FileSizeCompareMode.MarkBigger) {
                strFileSizeMode = "MarkBigger";
            } else if (obj.m_fileSizeMode == FileSizeCompareMode.MarkSmaller) {
                strFileSizeMode = "MarkSmaller";
            } else if (obj.m_fileSizeMode == FileSizeCompareMode.Ignore) {
                strFileSizeMode = "Ignore";
            }

            saver.StartObject(SettingTag.FileCompareSetting_FileCompareSetting);
            saver.AddString(SettingTag.FileCompareSetting_FileTimeMode, strFileTimeMode);
            saver.AddString(SettingTag.FileCompareSetting_FileSizeMode, strFileSizeMode);
            saver.AddBool(SettingTag.FileCompareSetting_ExceptFolder, obj.m_exceptFolder);
            saver.AddBool(SettingTag.FileCompareSetting_CheckContents, obj.m_checkContents);
            saver.EndObject(SettingTag.FileCompareSetting_FileCompareSetting);

            return true;
        }

        //=========================================================================================
        // プロパティ：更新時刻の比較モード
        //=========================================================================================
        public FileTimeCompareMode FileTimeMode {
            get {
                return m_fileTimeMode;
            }
            set {
                m_fileTimeMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルサイズの比較モード
        //=========================================================================================
        public FileSizeCompareMode FileSizeMode {
            get {
                return m_fileSizeMode;
            }
            set {
                m_fileSizeMode = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：フォルダを除外
        //=========================================================================================
        public bool ExceptFolder {
            get {
                return m_exceptFolder;
            }
            set {
                m_exceptFolder = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：内容を判別
        //=========================================================================================
        public bool CheckContents {
            get {
                return m_checkContents;
            }
            set {
                m_checkContents = value;
            }
        }

        //=========================================================================================
        // 列挙子：更新時刻の比較モード
        //=========================================================================================
        public enum FileTimeCompareMode {
            MarkExactly,            // 同じものをマーク
            MarkNewer,              // 新しいものをマーク
            MarkOlder,              // 古いものをマーク
            Ignore,                 // 考慮しない
        }

        //=========================================================================================
        // 列挙子：ファイルサイズの比較モード
        //=========================================================================================
        public enum FileSizeCompareMode {
            MarkExactly,            // 同じものをマーク
            MarkBigger,             // 大きいものをマーク
            MarkSmaller,            // 小さいものをマーク
            Ignore,                 // 考慮しない
        }
    }
}
