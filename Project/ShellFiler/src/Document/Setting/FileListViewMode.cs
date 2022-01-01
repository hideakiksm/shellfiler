using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイル一覧ビューのモード
    //=========================================================================================
    public class FileListViewMode {
        // サムネイルモードにするときtrue
        private bool m_thumbnailModeSwitch = false;

        // サムネイルモードの画像サイズ
        private FileListViewIconSize m_thumbnailSize = FileListViewIconSize.IconSize64;

        // サムネイルモードのファイル名
        private FileListViewThumbnailName m_thumbnailName = FileListViewThumbnailName.ThumbNameSpearate;

        //=========================================================================================
        // 機　能：コンストラクタ（デフォルト設定で初期化する）
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileListViewMode() {
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
        // 機　能：表示用文字列に変換する
        // 引　数：なし
        // 戻り値：表示用文字列
        //=========================================================================================
        public string ToDisplayString() {
            string result;
            if (!m_thumbnailModeSwitch) {
                result = Resources.FileListViewMode_DispDetail;
            } else {
                result = string.Format(Resources.FileListViewMode_DispThumb, m_thumbnailSize.ImageSize, m_thumbnailSize.ImageSize);
                if (m_thumbnailName == FileListViewThumbnailName.ThumbNameSpearate) {
                    result += Resources.FileListViewMode_DispNameSep;
                } else if (m_thumbnailName == FileListViewThumbnailName.ThumbNameSpearate) {
                    result += Resources.FileListViewMode_DispNameOver;
                } else {
                    result += Resources.FileListViewMode_DispNameNone;
                }
            }
            return result;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileListViewMode obj1, FileListViewMode obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_thumbnailModeSwitch != obj2.m_thumbnailModeSwitch) {
                return false;
            }
            if (obj1.m_thumbnailSize != obj2.m_thumbnailSize) {
                return false;
            }
            if (obj1.m_thumbnailName != obj2.m_thumbnailName) {
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
        public static bool LoadSetting(SettingLoader loader, out FileListViewMode obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.FileListViewMode_FileListViewMode, SettingTagType.BeginObject, out fit);
            if (!success) {
                return success;
            }
            if (!fit) {
                return true;
            }
            obj = new FileListViewMode();
            loader.NextTag();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return success;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileListViewMode_FileListViewMode) {
                    break;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileListViewMode_ThumbModeSwitch) {
                    obj.m_thumbnailModeSwitch = loader.BoolValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileListViewMode_ThumbnailSize) {
                    obj.m_thumbnailSize = FileListViewIconSize.FromString(loader.StringValue);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileListViewMode_ThumbnailName) {
                    obj.m_thumbnailName = FileListViewThumbnailName.FromString(loader.StringValue);
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
        public static bool SaveSetting(SettingSaver saver, FileListViewMode obj) {
            if (obj == null) {
                return true;
            }
            saver.StartObject(SettingTag.FileListViewMode_FileListViewMode);
            saver.AddBool(SettingTag.FileListViewMode_ThumbModeSwitch, obj.m_thumbnailModeSwitch);
            saver.AddString(SettingTag.FileListViewMode_ThumbnailSize, obj.m_thumbnailSize.StringName);
            saver.AddString(SettingTag.FileListViewMode_ThumbnailName, obj.m_thumbnailName.StringName);
            saver.EndObject(SettingTag.FileListViewMode_FileListViewMode);
            return true;
        }

        //=========================================================================================
        // 機　能：デフォルトファイル一覧用の設定を返す
        // 引　数：なし
        // 戻り値：デフォルトファイル一覧用の設定
        //=========================================================================================
        public static FileListViewMode DefaultFileList() {
            FileListViewMode obj = new FileListViewMode();
            obj.m_thumbnailModeSwitch = false;
            return obj;
        }

        //=========================================================================================
        // プロパティ：サムネイルモードにするときtrue
        //=========================================================================================
        public bool ThumbnailModeSwitch {
            get {
                return m_thumbnailModeSwitch;
            }
            set {
                m_thumbnailModeSwitch = value;
            }
        }

        //=========================================================================================
        // プロパティ：サムネイルモードの画像サイズ
        //=========================================================================================
        public FileListViewIconSize ThumbnailSize {
            get {
                return m_thumbnailSize;
            }
            set {
                m_thumbnailSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：サムネイルモードのファイル名
        //=========================================================================================
        public FileListViewThumbnailName ThumbnailName {
            get {
                return m_thumbnailName;
            }
            set {
                m_thumbnailName = value;
            }
        }
    }
}
