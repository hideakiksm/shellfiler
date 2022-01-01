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
    // クラス：同名ファイルの扱い方を表現するための設定
    //=========================================================================================
    public class SameFileOption : ICloneable {
        // 同名ファイルのモード
        private SameFileOperation.SameFileTransferMode m_sameFileMode = SameFileOperation.SameFileTransferMode.RenameNew;

        // 自動的に更新するときのモード(Windows)
        private SameFileOperation.SameFileAutoUpdateMode m_autoUpdateModeWindows = SameFileOperation.SameFileAutoUpdateMode.AddParentheses;
        
        // 自動的に更新するときのモード(SSH)
        private SameFileOperation.SameFileAutoUpdateMode m_autoUpdateModeSSH = SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SameFileOption() {
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
        public static bool EqualsConfig(SameFileOption obj1, SameFileOption obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_sameFileMode != obj2.m_sameFileMode) {
                return false;
            }
            if (obj1.m_autoUpdateModeWindows != obj2.m_autoUpdateModeWindows) {
                return false;
            }
            if (obj1.m_autoUpdateModeSSH != obj2.m_autoUpdateModeSSH) {
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
        public static bool LoadSetting(SettingLoader loader, out SameFileOption obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.SameFileOption_SameFileOption, SettingTagType.BeginObject, out fit);
            if (!success) {
                return false;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();

            obj = new SameFileOption();
            string strValue;
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.SameFileOption_SameFileOption) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.SameFileOption_SameFileMode) {
                    // 同名ファイルのモード
                    strValue = loader.StringValue;
                    if (strValue == "ForceOverwrite") {
                        obj.m_sameFileMode = SameFileOperation.SameFileTransferMode.ForceOverwrite;
                    } else if (strValue == "OverwriteIfNewer") {
                        obj.m_sameFileMode = SameFileOperation.SameFileTransferMode.OverwriteIfNewer;
                    } else if (strValue == "RenameNew") {
                        obj.m_sameFileMode = SameFileOperation.SameFileTransferMode.RenameNew;
                    } else if (strValue == "NotOverwrite") {
                        obj.m_sameFileMode = SameFileOperation.SameFileTransferMode.NotOverwrite;
                    } else if (strValue == "AutoRename") {
                        obj.m_sameFileMode = SameFileOperation.SameFileTransferMode.AutoRename;
                    } else if (strValue == "FullAutoTransfer") {
                        obj.m_sameFileMode = SameFileOperation.SameFileTransferMode.FullAutoTransfer;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.SameFileOption_AutoUpdateModeWindows) {
                    // 自動的に更新するときのモード(Windows)
                    strValue = loader.StringValue;
                    if (strValue == "AddUnderBarNumber") {
                        obj.m_autoUpdateModeWindows = SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber;
                    } else if (strValue == "AddParentheses") {
                        obj.m_autoUpdateModeWindows = SameFileOperation.SameFileAutoUpdateMode.AddParentheses;
                    } else if (strValue == "AddBracket") {
                        obj.m_autoUpdateModeWindows = SameFileOperation.SameFileAutoUpdateMode.AddBracket;
                    } else if (strValue == "AddUnderBar") {
                        obj.m_autoUpdateModeWindows = SameFileOperation.SameFileAutoUpdateMode.AddUnderBar;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.SameFileOption_AutoUpdateModeSSH) {
                    // 自動的に更新するときのモード(SSH)
                    strValue = loader.StringValue;
                    if (strValue == "AddUnderBarNumber") {
                        obj.m_autoUpdateModeSSH = SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber;
                    } else if (strValue == "AddUnderBar") {
                        obj.m_autoUpdateModeSSH = SameFileOperation.SameFileAutoUpdateMode.AddUnderBar;
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
        public static bool SaveSetting(SettingSaver saver, SameFileOption obj) {
            if (obj == null) {
                return true;
            }

            // 同名ファイルのモード
            string strSameFileMode = null;
            if (obj.m_sameFileMode == SameFileOperation.SameFileTransferMode.ForceOverwrite) {
                strSameFileMode = "ForceOverwrite";
            } else if (obj.m_sameFileMode == SameFileOperation.SameFileTransferMode.OverwriteIfNewer) {
                strSameFileMode = "OverwriteIfNewer";
            } else if (obj.m_sameFileMode == SameFileOperation.SameFileTransferMode.RenameNew) {
                strSameFileMode = "RenameNew";
            } else if (obj.m_sameFileMode == SameFileOperation.SameFileTransferMode.NotOverwrite) {
                strSameFileMode = "NotOverwrite";
            } else if (obj.m_sameFileMode == SameFileOperation.SameFileTransferMode.AutoRename) {
                strSameFileMode = "AutoRename";
            } else if (obj.m_sameFileMode == SameFileOperation.SameFileTransferMode.FullAutoTransfer) {
                strSameFileMode = "FullAutoTransfer";
            }

            // 自動的に更新するときのモード(Windows)
            string strAutoUpdateModeWindows = null;
            if (obj.m_autoUpdateModeWindows == SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber) {
                strAutoUpdateModeWindows = "AddUnderBarNumber";
            } else if (obj.m_autoUpdateModeWindows == SameFileOperation.SameFileAutoUpdateMode.AddParentheses) {
                strAutoUpdateModeWindows = "AddParentheses";
            } else if (obj.m_autoUpdateModeWindows == SameFileOperation.SameFileAutoUpdateMode.AddBracket) {
                strAutoUpdateModeWindows = "AddBracket";
            } else if (obj.m_autoUpdateModeWindows == SameFileOperation.SameFileAutoUpdateMode.AddUnderBar) {
                strAutoUpdateModeWindows = "AddUnderBar";
            }

            // 自動的に更新するときのモード(SSH)
            string strAutoUpdateModeSSH = null;
            if (obj.m_autoUpdateModeSSH == SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber) {
                strAutoUpdateModeSSH = "AddUnderBarNumber";
            } else if (obj.m_autoUpdateModeSSH == SameFileOperation.SameFileAutoUpdateMode.AddUnderBar) {
                strAutoUpdateModeSSH = "AddUnderBar";
            }

            // 保存
            saver.StartObject(SettingTag.SameFileOption_SameFileOption);
            saver.AddString(SettingTag.SameFileOption_SameFileMode, strSameFileMode);
            saver.AddString(SettingTag.SameFileOption_AutoUpdateModeWindows, strAutoUpdateModeWindows);
            saver.AddString(SettingTag.SameFileOption_AutoUpdateModeSSH, strAutoUpdateModeSSH);
            saver.EndObject(SettingTag.SameFileOption_SameFileOption);

            return true;
        }

        //=========================================================================================
        // プロパティ：同名ファイルのモード
        //=========================================================================================
        public SameFileOperation.SameFileTransferMode SameFileMode {
            get {
                return m_sameFileMode;
            }
            set {
                m_sameFileMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：自動的に更新するときのモード(Windows)
        //=========================================================================================
        public SameFileOperation.SameFileAutoUpdateMode AutoUpdateModeWindows {
            get {
                return m_autoUpdateModeWindows;
            }
            set {
                m_autoUpdateModeWindows = value;
            }
        }

        //=========================================================================================
        // プロパティ：自動的に更新するときのモード(SSH)
        //=========================================================================================
        public SameFileOperation.SameFileAutoUpdateMode AutoUpdateModeSSH {
            get {
                return m_autoUpdateModeSSH;
            }
            set {
                m_autoUpdateModeSSH = value;
            }
        }
    }
}
