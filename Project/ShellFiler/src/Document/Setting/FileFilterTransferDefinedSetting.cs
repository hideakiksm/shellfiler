using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Text;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：転送用定義済みファイルフィルターの設定
    //=========================================================================================
    public class FileFilterTransferDefinedSetting : ICloneable {
        // 定義済み設定での使用する拡張子のマスク
        private string m_definedTargetFileMask = "*";

        // 定義済み設定でのその他のファイルの扱い
        private FileFilterListTransferOtherMode m_definedOtherMode = FileFilterListTransferOtherMode.SkipTransfer;

        // 定義済み設定の選択項目
        private FileFilterDefinedMode m_selectedItem = FileFilterDefinedMode.ShiftJISToUTF8;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterTransferDefinedSetting() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileFilterTransferDefinedSetting obj = new FileFilterTransferDefinedSetting();
            obj.m_definedTargetFileMask = m_definedTargetFileMask;
            obj.m_definedOtherMode = m_definedOtherMode;
            obj.m_selectedItem = m_selectedItem;

            return obj;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileFilterTransferDefinedSetting obj1, FileFilterTransferDefinedSetting obj2) {
            if (obj1.m_definedTargetFileMask != obj2.m_definedTargetFileMask) {
                return false;
            }
            if (obj1.m_definedOtherMode != obj2.m_definedOtherMode) {
                return false;
            }
            if (obj1.m_selectedItem != obj2.m_selectedItem) {
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
        public static bool LoadSetting(SettingLoader loader, out FileFilterTransferDefinedSetting obj) {
            bool success;
            obj = new FileFilterTransferDefinedSetting();

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.FileFilter_DefinedItem, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileFilter_DefinedItem) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_DefinedTargetFileMask) {
                    obj.m_definedTargetFileMask = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_DefinedOtherMode) {
                    obj.m_definedOtherMode = FileFilterListTransferOtherMode.StringToTransferMode(loader.StringValue, obj.m_definedOtherMode);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_DefinedSelectedItem) {
                    obj.m_selectedItem = FileFilterDefinedMode.FromString(loader.StringValue);
                    if (obj.m_selectedItem == null) {
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
        public static bool SaveSetting(SettingSaver saver, FileFilterTransferDefinedSetting obj) {
            saver.StartObject(SettingTag.FileFilter_DefinedItem);

            saver.AddString(SettingTag.FileFilter_DefinedTargetFileMask, obj.m_definedTargetFileMask);
            saver.AddString(SettingTag.FileFilter_DefinedOtherMode, FileFilterListTransferOtherMode.TransferModeToString(obj.m_definedOtherMode));
            saver.AddString(SettingTag.FileFilter_DefinedSelectedItem, obj.m_selectedItem.StringName);
            
            saver.EndObject(SettingTag.FileFilter_DefinedItem);
            
            return true;
        }

        //=========================================================================================
        // 機　能：転送用設定に変換する
        // 引　数：[in]quickList  クイック設定のリスト
        // 戻り値：転送用設定
        //=========================================================================================
        public FileFilterTransferSetting CreateTransferSetting(List<FileFilterListClipboard> quickList) {
            // フィルターを作成
            List<FileFilterItem> filterItemList = new List<FileFilterItem>();
            FileFilterItem filterItem = new FileFilterItem();
            filterItemList.Add(filterItem);
            if (m_selectedItem == FileFilterDefinedMode.ShiftJISToUTF8 ||
                    m_selectedItem == FileFilterDefinedMode.ShiftJISToEUC ||
                    m_selectedItem == FileFilterDefinedMode.UTF8ToShiftJIS ||
                    m_selectedItem == FileFilterDefinedMode.EUCToShiftJIS) {
                filterItem.FileFilterClassPath = typeof(FileFilterCharsetConvert).FullName;
                filterItem.PropertyList = FileFilterCharsetConvert.CreateProperty(m_selectedItem);
            } else if (m_selectedItem == FileFilterDefinedMode.ReturnCRLF ||
                    m_selectedItem == FileFilterDefinedMode.ReturnLF) {
                filterItem.FileFilterClassPath = typeof(FileFilterConvertCrLf).FullName;
                filterItem.PropertyList = FileFilterConvertCrLf.CreateProperty(m_selectedItem);
            } else if (m_selectedItem == FileFilterDefinedMode.DeleteEmptyLine) {
                filterItem.FileFilterClassPath = typeof(FileFilterDeleteMultiCrLf).FullName;
                filterItem.PropertyList = FileFilterDeleteMultiCrLf.CreateProperty(m_selectedItem);
            } else if (m_selectedItem == FileFilterDefinedMode.ShiftJIS4TabSpace ||
                    m_selectedItem == FileFilterDefinedMode.ShiftJISSpace4Tab ||
                    m_selectedItem == FileFilterDefinedMode.ShiftJIS8TabSpace ||
                    m_selectedItem == FileFilterDefinedMode.ShiftJISSpace8Tab ||
                    m_selectedItem == FileFilterDefinedMode.UTF84TabSpace ||
                    m_selectedItem == FileFilterDefinedMode.UTF8Space4Tab ||
                    m_selectedItem == FileFilterDefinedMode.UTF88TabSpace ||
                    m_selectedItem == FileFilterDefinedMode.UTF8Space8Tab) {
                filterItem.FileFilterClassPath = typeof(FileFilterConvertTabSpace).FullName;
                filterItem.PropertyList = FileFilterConvertTabSpace.CreateProperty(m_selectedItem);
            } else if (m_selectedItem == FileFilterDefinedMode.ShellFilerDump) {
                filterItem.FileFilterClassPath = typeof(FileFilterShellFilerDump).FullName;
                filterItem.PropertyList = FileFilterShellFilerDump.CreateProperty();
            } else if (m_selectedItem == FileFilterDefinedMode.Quick1) {
                filterItemList.Clear();
                filterItemList.AddRange(quickList[0].FilterList);
            } else if (m_selectedItem == FileFilterDefinedMode.Quick2) {
                filterItemList.Clear();
                filterItemList.AddRange(quickList[1].FilterList);
            } else if (m_selectedItem == FileFilterDefinedMode.Quick3) {
                filterItemList.Clear();
                filterItemList.AddRange(quickList[2].FilterList);
            } else if (m_selectedItem == FileFilterDefinedMode.Quick4) {
                filterItemList.Clear();
                filterItemList.AddRange(quickList[3].FilterList);
            } else {
                Program.Abort("フィルターを作成できません。");
            }

            // 戻り値に整形
            FileFilterTransferSetting transfer = new FileFilterTransferSetting();
            transfer.OtherMode = m_definedOtherMode;
            FileFilterListTransfer transferItem = new FileFilterListTransfer();
            transferItem.FilterName = "";
            transferItem.TargetFileMask = m_definedTargetFileMask;
            transferItem.UseFilter = true;
            transferItem.FilterList.AddRange(filterItemList);
            transfer.TransferList.Add(transferItem);
            return transfer;
        }
        
        //=========================================================================================
        // プロパティ：定義済み設定での使用する拡張子のマスク
        //=========================================================================================
        public string DefinedTargetFileMask {
            get {
                return m_definedTargetFileMask;
            }
            set {
                m_definedTargetFileMask = value;
            }
        }

        //=========================================================================================
        // プロパティ：定義済み設定でのその他のファイルの扱い
        //=========================================================================================
        public FileFilterListTransferOtherMode DefinedOtherMode {
            get {
                return m_definedOtherMode;
            }
            set {
                m_definedOtherMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：定義済み設定の選択項目
        //=========================================================================================
        public FileFilterDefinedMode SelectedItem {
            get {
                return m_selectedItem;
            }
            set {
                m_selectedItem = value;
            }
        }
    }
}
