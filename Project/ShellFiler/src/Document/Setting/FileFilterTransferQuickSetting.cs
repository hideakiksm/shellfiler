using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Text;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：転送用クイックファイルフィルターの設定
    //=========================================================================================
    public class FileFilterTransferQuickSetting : ICloneable {
        // クイック設定での使用する拡張子のマスク
        private string m_quickTargetFileMask = "*";

        // クイック設定でのその他のファイルの扱い
        private FileFilterListTransferOtherMode m_quickOtherMode = FileFilterListTransferOtherMode.SkipTransfer;

        // クイック設定でのフィルター（選択がないときnull）
        private FileFilterItem m_quickFilterItem = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterTransferQuickSetting() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileFilterTransferQuickSetting obj = new FileFilterTransferQuickSetting();
            obj.m_quickTargetFileMask = m_quickTargetFileMask;
            obj.m_quickOtherMode = m_quickOtherMode;
            if (m_quickFilterItem == null) {
                obj.m_quickFilterItem = null;
            } else {
                obj.m_quickFilterItem = (FileFilterItem)(m_quickFilterItem.Clone());
            }

            return obj;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileFilterTransferQuickSetting obj1, FileFilterTransferQuickSetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_quickTargetFileMask != obj2.m_quickTargetFileMask) {
                return false;
            }
            if (obj1.m_quickOtherMode != obj2.m_quickOtherMode) {
                return false;
            }
            if (!FileFilterItem.EqualsConfig(obj1.m_quickFilterItem, obj2.m_quickFilterItem)) {
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
        public static bool LoadSetting(SettingLoader loader, out FileFilterTransferQuickSetting obj) {
            bool success;
            obj = new FileFilterTransferQuickSetting();

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.FileFilter_QuickItem, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileFilter_QuickItem) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_QuickTargetFileMask) {
                    obj.m_quickTargetFileMask = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_QuickOtherMode) {
                    obj.m_quickOtherMode = FileFilterListTransferOtherMode.StringToTransferMode(loader.StringValue, obj.m_quickOtherMode);
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileFilter_QuickFilterItem) {
                    success = FileFilterItem.LoadSetting(loader, out obj.m_quickFilterItem);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.FileFilter_QuickFilterItem, SettingTagType.EndObject);
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
        public static bool SaveSetting(SettingSaver saver, FileFilterTransferQuickSetting obj) {
            saver.StartObject(SettingTag.FileFilter_QuickItem);

            saver.AddString(SettingTag.FileFilter_QuickTargetFileMask, obj.m_quickTargetFileMask);
            saver.AddString(SettingTag.FileFilter_QuickOtherMode, FileFilterListTransferOtherMode.TransferModeToString(obj.m_quickOtherMode));
            saver.StartObject(SettingTag.FileFilter_QuickFilterItem);
            FileFilterItem.SaveSetting(saver, obj.m_quickFilterItem);
            saver.EndObject(SettingTag.FileFilter_QuickFilterItem);

            saver.EndObject(SettingTag.FileFilter_QuickItem);
            
            return true;
        }

        //=========================================================================================
        // 機　能：転送用設定に変換する
        // 引　数：なし
        // 戻り値：転送用設定
        //=========================================================================================
        public FileFilterTransferSetting CreateTransferSetting() {
            FileFilterTransferSetting transfer = new FileFilterTransferSetting();
            transfer.OtherMode = m_quickOtherMode;
            FileFilterListTransfer item = new FileFilterListTransfer();
            item.FilterName = "";
            item.TargetFileMask = m_quickTargetFileMask;
            item.UseFilter = true;
            item.FilterList.Add(m_quickFilterItem);
            transfer.TransferList.Add(item);
            return transfer;
        }
        
        //=========================================================================================
        // プロパティ：クイック設定での使用する拡張子のマスク
        //=========================================================================================
        public string QuickTargetFileMask {
            get {
                return m_quickTargetFileMask;
            }
            set {
                m_quickTargetFileMask = value;
            }
        }

        //=========================================================================================
        // プロパティ：クイック設定でのその他のファイルの扱い
        //=========================================================================================
        public FileFilterListTransferOtherMode QuickOtherMode {
            get {
                return m_quickOtherMode;
            }
            set {
                m_quickOtherMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：クイック設定でのフィルター（選択がないときnull）
        //=========================================================================================
        public FileFilterItem QuickFilterItem {
            get {
                return m_quickFilterItem;
            }
            set {
                m_quickFilterItem = value;
            }
        }
    }
}
