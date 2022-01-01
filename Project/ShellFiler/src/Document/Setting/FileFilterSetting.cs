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
    // クラス：ファイルフィルターの設定
    //=========================================================================================
    public class FileFilterSetting : ICloneable {
        // クリップボード用の設定
        private FileFilterClipboardSetting m_clipboardSetting = new FileFilterClipboardSetting();

        // 前回使用した転送用フィルターのモード
        private FileFilterMode m_transferLastFilterMode = FileFilterMode.DefinedMode;
        
        // 転送用の設定（詳細モード）
        private FileFilterTransferSetting m_transferDetailSetting = new FileFilterTransferSetting();
        
        // 転送用の設定（クイックモード）
        private FileFilterTransferQuickSetting m_transferQuickSetting = new FileFilterTransferQuickSetting();

        // 転送用の設定（定義済み設定）
        private FileFilterTransferDefinedSetting m_transferDefinedSetting = new FileFilterTransferDefinedSetting();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterSetting() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileFilterSetting obj = new FileFilterSetting();
            obj.m_clipboardSetting = (FileFilterClipboardSetting)(m_clipboardSetting.Clone());
            obj.m_transferLastFilterMode = m_transferLastFilterMode;
            obj.m_transferDetailSetting = (FileFilterTransferSetting)(m_transferDetailSetting.Clone());
            obj.m_transferQuickSetting = (FileFilterTransferQuickSetting)(m_transferQuickSetting.Clone());
            obj.m_transferDefinedSetting = (FileFilterTransferDefinedSetting)(m_transferDefinedSetting.Clone());
            return obj;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileFilterSetting obj1, FileFilterSetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (!FileFilterClipboardSetting.EqualsConfig(obj1.m_clipboardSetting, obj2.m_clipboardSetting)) {
                return false;
            }
            if (obj1.m_transferLastFilterMode != obj2.m_transferLastFilterMode) {
                return false;
            }
            if (!FileFilterTransferSetting.EqualsConfig(obj1.m_transferDetailSetting, obj2.m_transferDetailSetting)) {
                return false;
            }
            if (!FileFilterTransferQuickSetting.EqualsConfig(obj1.m_transferQuickSetting, obj2.m_transferQuickSetting)) {
                return false;
            }
            if (!FileFilterTransferDefinedSetting.EqualsConfig(obj1.m_transferDefinedSetting, obj2.m_transferDefinedSetting)) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：デフォルト値を設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetDefaultSetting() {
            m_clipboardSetting = new FileFilterClipboardSetting();
            m_transferLastFilterMode = FileFilterMode.DefinedMode;
            m_transferDetailSetting = new FileFilterTransferSetting();
            m_transferQuickSetting = new FileFilterTransferQuickSetting();
            m_transferDefinedSetting = new FileFilterTransferDefinedSetting();
        }

        //=========================================================================================
        // 機　能：データを読み込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void LoadData() {
#if !FREE_VERSION
            string fileName = DirectoryManager.FileFilterSetting;
            SettingLoader loader = new SettingLoader(fileName);
            bool success = LoadSettingInternal(loader);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotLoadSetting, fileName, loader.ErrorDetail);
                SetDefaultSetting();
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool LoadSettingInternal(SettingLoader loader) {
            // ファイルがないときはデフォルト
            if (!File.Exists(loader.FileName)) {
                SetDefaultSetting();
                return true;
            }

            // ファイルから読み込む
            bool success;
            success = loader.LoadSetting(false);
            if (!success) {
                return false;
            }

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.FileFilter_FileFilter, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileFilter_FileFilter) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileFilter_ClipboardSetting) {
                    success = FileFilterClipboardSetting.LoadSetting(loader, out m_clipboardSetting);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.FileFilter_ClipboardSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_TransferDetailSettingMode) {
                    m_transferLastFilterMode = FileFilterMode.FromString(loader.StringValue);
                    if (m_transferLastFilterMode == null) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileFilter_TransferDetailSetting) {
                    success = FileFilterTransferSetting.LoadSetting(loader, out m_transferDetailSetting);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.FileFilter_TransferDetailSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileFilter_TransferQuickSetting) {
                    success = FileFilterTransferQuickSetting.LoadSetting(loader, out m_transferQuickSetting);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.FileFilter_TransferQuickSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileFilter_TransferDefinedSetting) {
                    success = FileFilterTransferDefinedSetting.LoadSetting(loader, out m_transferDefinedSetting);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.FileFilter_TransferDefinedSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：データを書き込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SaveData() {
#if !FREE_VERSION
            string fileName = DirectoryManager.FileFilterSetting;
            SettingSaver saver = new SettingSaver(fileName);
            bool success = SaveSettingInternal(saver);
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
        private bool SaveSettingInternal(SettingSaver saver) {
            saver.StartObject(SettingTag.FileFilter_FileFilter);

            saver.StartObject(SettingTag.FileFilter_ClipboardSetting);
            FileFilterClipboardSetting.SaveSetting(saver, m_clipboardSetting);
            saver.EndObject(SettingTag.FileFilter_ClipboardSetting);
            
            saver.AddString(SettingTag.FileFilter_TransferDetailSettingMode, m_transferLastFilterMode.StringName);

            saver.StartObject(SettingTag.FileFilter_TransferDetailSetting);
            FileFilterTransferSetting.SaveSetting(saver, m_transferDetailSetting);
            saver.EndObject(SettingTag.FileFilter_TransferDetailSetting);

            saver.StartObject(SettingTag.FileFilter_TransferQuickSetting);
            FileFilterTransferQuickSetting.SaveSetting(saver, m_transferQuickSetting);
            saver.EndObject(SettingTag.FileFilter_TransferQuickSetting);

            saver.StartObject(SettingTag.FileFilter_TransferDefinedSetting);
            FileFilterTransferDefinedSetting.SaveSetting(saver, m_transferDefinedSetting);
            saver.EndObject(SettingTag.FileFilter_TransferDefinedSetting);

            saver.EndObject(SettingTag.FileFilter_FileFilter);
            return saver.SaveSetting(false);
        }

        //=========================================================================================
        // プロパティ：クリップボード用設定
        //=========================================================================================
        public FileFilterClipboardSetting ClipboardSetting {
            get {
                return m_clipboardSetting;
            }
            set {
                m_clipboardSetting = value;
            }
        }

        //=========================================================================================
        // プロパティ：前回使用した転送用フィルターのモード
        //=========================================================================================
        public FileFilterMode TransferLastFilterMode {
            get {
                return m_transferLastFilterMode;
            }
            set {
                m_transferLastFilterMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：転送用の設定（詳細モード）
        //=========================================================================================
        public FileFilterTransferSetting TransferDetailSetting {
            get {
                return m_transferDetailSetting;
            }
            set {
                m_transferDetailSetting = value;
            }
        }

        //=========================================================================================
        // プロパティ：転送用の設定（クイックモード）
        //=========================================================================================
        public FileFilterTransferQuickSetting TransferQuickSetting {
            get {
                return m_transferQuickSetting;
            }
            set {
                m_transferQuickSetting = value;
            }
        }
 
        //=========================================================================================
        // プロパティ：転送用の設定（定義済み設定）
        //=========================================================================================
        public FileFilterTransferDefinedSetting TransferDefinedSetting {
            get {
                return m_transferDefinedSetting;
            }
            set {
                m_transferDefinedSetting = value;
            }
        }
    }
}
