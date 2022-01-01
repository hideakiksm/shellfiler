using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Archive;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.Setting;
using ShellFiler.Command.FileList.Window;
using ShellFiler.Command.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：関連付け1種類についてのカスタマイズ項目（キー1つについての関連付け）
    //=========================================================================================
    public class AssociateKeySetting : ICloneable {
        // 拡張子のセパレータ
        private const string EXT_SEPARATOR = ";";

        // 関連付け名
        private string m_displayName = "";

        // 拡張子に対する関連付け（フォルダに対する関連づけのときExtListはnull）
        private List<AssociateInfo> m_associateExtList = new List<AssociateInfo>();

        // デフォルトの関連付け（設定がない場合はnull）
        private ActionCommandMoniker m_defaultCommand = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public AssociateKeySetting() {
        }

        //=========================================================================================
        // 機　能：設定を破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearSetting() {
            m_displayName = "";
            m_associateExtList.Clear();
            m_defaultCommand = null;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(AssociateKeySetting obj1, AssociateKeySetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 != null && obj2 != null) {
                ;
            } else {
                return false;
            }
            if (obj1.m_associateExtList.Count != obj2.m_associateExtList.Count) {
                return false;
            }
            for (int i = 0; i < obj1.m_associateExtList.Count; i++) {
                if (obj1.m_displayName != obj2.m_displayName) {
                    return false;
                }
                if (obj1.m_associateExtList[i].ExtList == null && obj2.m_associateExtList[i].ExtList == null) {
                    ;
                } else if (obj1.m_associateExtList[i].ExtList == null && obj2.m_associateExtList[i].ExtList != null) {
                    return false;
                } else if (obj1.m_associateExtList[i].ExtList != null && obj2.m_associateExtList[i].ExtList == null) {
                    return false;
                } else {
                    if (obj1.m_associateExtList[i].ExtList.Length != obj2.m_associateExtList[i].ExtList.Length) {
                        return false;
                    }
                    for (int j = 0; j < obj1.m_associateExtList[i].ExtList.Length; j++) {
                        if (obj1.m_associateExtList[i].ExtList[j] != obj2.m_associateExtList[i].ExtList[j]) {
                            return false;
                        }
                    }
                }
                if (!ActionCommandMoniker.Equals(obj1.m_associateExtList[i].CommandMoniker, obj2.m_associateExtList[i].CommandMoniker)) {
                    return false;
                }
            }
            if (!ActionCommandMoniker.Equals(obj1.m_defaultCommand, obj2.m_defaultCommand)) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            AssociateKeySetting clone = new AssociateKeySetting();
            clone.m_displayName = m_displayName;
            for (int i = 0; i < m_associateExtList.Count; i++) {
                AssociateInfo info = new AssociateInfo();
                info.ExtList = m_associateExtList[i].ExtList;
                info.CommandMoniker = (ActionCommandMoniker)(m_associateExtList[i].CommandMoniker.Clone());
                info.FileSystem = m_associateExtList[i].FileSystem;
                clone.m_associateExtList.Add(info);
            }
            if (m_defaultCommand == null) {
                clone.m_defaultCommand = null;
            } else {
                clone.m_defaultCommand = (ActionCommandMoniker)(m_defaultCommand.Clone());
            }
            return clone;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader      読み込み用クラス
        // 　　　　[in]obj         読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, AssociateKeySetting obj) {
            bool success;
            success = loader.ExpectTag(SettingTag.KeySetting_AssocKeySetting, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.KeySetting_AssocKeySetting) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.KeySetting_AssocKeyDisplayName) {
                    obj.m_displayName = loader.StringValue;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.KeySetting_AssocKeyExtList) {
                    success = LoadAssociateInfo(loader, obj.m_associateExtList, obj.m_displayName);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.KeySetting_AssocKeyExtList, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.KeySetting_AssocKeyDefaultCommand) {
                    success = loader.GetTag(out tagName, out tagType);
                    if (!success) {
                        return false;
                    }
                    string className;
                    success = KeyItemSetting.LoadActionCommandMoniker(tagType, tagName, SettingTag.KeySetting_AssocKeyDefaultCommand, loader, out obj.m_defaultCommand, out className);
                    if (!success) {
                        // 読み込み失敗時はそのまま回復
                        obj.m_defaultCommand = null;
                        loader.SetWarning(Resources.SettingLoader_LoadKeySettingAssocMonikerDefault, obj.m_displayName);
                    }
                }
            }

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader     読み込み用クラス
        // 　　　　[in]obj        読み込み対象のオブジェクト
        // 　　　　[in]assocName  関連づけの名前
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadAssociateInfo(SettingLoader loader, List<AssociateInfo> obj, string assocName) {
            bool success;
            while (true) {
                bool fit;
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.PeekTag(SettingTag.KeySetting_AssocKeyInfo, SettingTagType.BeginObject, out fit);
                if (!fit) {
                    break;
                }
                loader.NextTag();
                AssociateInfo info = new AssociateInfo();
                info.ExtList = null;
                info.FileSystem = null;
                info.CommandMoniker = null;
                while (true) {
                    success = loader.GetTag(out tagName, out tagType);
                    if (!success) {
                        return false;
                    }
                    if (tagType == SettingTagType.EndObject && tagName == SettingTag.KeySetting_AssocKeyInfo) {
                        break;
                    } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.KeySetting_AssocKeyInfoExt) {
                        string ext = loader.StringValue;
                        info.ExtList = ext.Split(EXT_SEPARATOR[0]);
                    } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.KeySetting_AssocKeyInfoFileSystem) {
                        string fileSystem = loader.StringValue;
                        info.FileSystem = FileSystemID.FromString(fileSystem, true);
                    } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.KeySetting_AssocKeyInfoCommand) {
                        success = loader.GetTag(out tagName, out tagType);
                        if (!success) {
                            return false;
                        }
                        ActionCommandMoniker moniker;
                        string className;
                        success = KeyItemSetting.LoadActionCommandMoniker(tagType, tagName, SettingTag.KeySetting_AssocKeyInfoCommand, loader, out moniker, out className);
                        info.CommandMoniker = moniker;
                        if (!success || info.CommandMoniker == null) {
                            // 読み込み失敗時は無効コマンド
                            info.CommandMoniker = null;
                            if (info.ExtList == null) {
                                loader.SetWarning(Resources.SettingLoader_LoadKeySettingAssocMonikerFolder, assocName);
                            } else {
                                loader.SetWarning(Resources.SettingLoader_LoadKeySettingAssocMonikerExt, assocName, info.ExtList);
                            }
                        }
                    }
                }
                if (info.FileSystem != null && info.CommandMoniker != null) {
                    obj.Add(info);
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
        public static bool SaveSetting(SettingSaver saver, AssociateKeySetting obj) {
            bool success;
            saver.StartObject(SettingTag.KeySetting_AssocKeySetting);
            saver.AddString(SettingTag.KeySetting_AssocKeyDisplayName, obj.m_displayName);

            saver.StartObject(SettingTag.KeySetting_AssocKeyExtList);
            for (int i = 0; i < obj.m_associateExtList.Count; i++) {
                success = SaveAssociateInfo(saver, obj.m_associateExtList[i]);
                if (!success) {
                    return false;
                }
            }
            saver.EndObject(SettingTag.KeySetting_AssocKeyExtList);

            saver.StartObject(SettingTag.KeySetting_AssocKeyDefaultCommand);
            if (obj.m_defaultCommand != null) {
                success = KeyItemSetting.SaveActionCommandMoniker(saver, obj.m_defaultCommand);
                if (!success) {
                    return false;
                }
            }
            saver.EndObject(SettingTag.KeySetting_AssocKeyDefaultCommand);

            saver.EndObject(SettingTag.KeySetting_AssocKeySetting);

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        private static bool SaveAssociateInfo(SettingSaver saver, AssociateInfo obj) {
            bool success;
            saver.StartObject(SettingTag.KeySetting_AssocKeyInfo);

            if (obj.ExtList != null) {
                saver.AddString(SettingTag.KeySetting_AssocKeyInfoExt, StringUtils.CombineStringArray(obj.ExtList, EXT_SEPARATOR));
            }
            saver.AddString(SettingTag.KeySetting_AssocKeyInfoFileSystem, obj.FileSystem.StringId);
            
            saver.StartObject(SettingTag.KeySetting_AssocKeyInfoCommand);
            success = KeyItemSetting.SaveActionCommandMoniker(saver, obj.CommandMoniker);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.KeySetting_AssocKeyInfoCommand);

            saver.EndObject(SettingTag.KeySetting_AssocKeyInfo);

            return true;
        }

        //=========================================================================================
        // 機　能：関連付け情報をソートする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SortSetting() {
            // 拡張子のソート
            for (int i = 0; i < m_associateExtList.Count; i++) {
                if (m_associateExtList[i].ExtList != null) {
                    List<string> extList = new List<string>();
                    extList.AddRange(m_associateExtList[i].ExtList);
                    extList.Sort();
                    m_associateExtList[i].ExtList = extList.ToArray();
                }
            }

            // 全体のソート
            AssociateSettingSorter sorter = new AssociateSettingSorter();
            m_associateExtList.Sort(sorter);
        }

        //=========================================================================================
        // 機　能：関連付け情報を追加する
        // 引　数：[in]systemId  実行対象のファイルシステム（関係ないときNone）
        // 　　　　[in]extList   関連付ける拡張子の一覧（フォルダに対する関連づけのときnull）
        // 　　　　[in]command   関連付けるコマンド
        // 戻り値：なし
        //=========================================================================================
        public void AddAssociate(FileSystemID systemId, string[] extList, ActionCommandMoniker command) {
            AssociateInfo info = new AssociateInfo();
            string[] extLow;
            if (extList == null) {
                extLow = null;
            } else {
                extLow = new string[extList.Length];
                for (int i = 0; i < extList.Length; i++) {
                    extLow[i] = extList[i].ToLower();
                }
            }
            info.ExtList = extLow;
            info.FileSystem = systemId;
            info.CommandMoniker = command;
            m_associateExtList.Add(info);
        }

        //=========================================================================================
        // 機　能：デフォルトの関連づけを設定する
        // 引　数：[in]command    関連付けるコマンド
        // 戻り値：なし
        //=========================================================================================
        public void SetDefaultCommand(ActionCommandMoniker command) {
            m_defaultCommand = command;
        }
        
        //=========================================================================================
        // 機　能：フォルダの関連づけを取得する
        // 引　数：[in]fileSystem  ファイルシステム
        // 戻り値：フォルダの関連づけ（設定がないときはnull）
        //=========================================================================================
        public ActionCommandMoniker GetFolderCommand(FileSystemID fileSystem) {
            foreach (AssociateInfo info in m_associateExtList) {
                if (info.ExtList == null) {
                    if (info.FileSystem == FileSystemID.None || info.FileSystem == fileSystem) {
                        return info.CommandMoniker;
                    }
                }
            }
            return null;
        }
        
        //=========================================================================================
        // 機　能：ファイル名に対応した関連づけを取得する
        // 引　数：[in]fileName    ファイル名
        // 　　　　[in]fileSystem  ファイルシステム
        // 戻り値：ファイル名に対応した関連づけ（設定がないときはnull）
        //=========================================================================================
        public ActionCommandMoniker GetAssociateCommand(string fileName, FileSystemID fileSystem) {
            string fileLow = fileName.ToLower();
            foreach (AssociateInfo info in m_associateExtList) {
                if (info.ExtList == null) {
                    continue;
                }
                foreach (string ext in info.ExtList) {
                    if (ext.StartsWith(".")) {
                        if (fileLow.EndsWith(ext)) {
                            if (info.FileSystem == FileSystemID.None || info.FileSystem == fileSystem) {
                               return info.CommandMoniker;
                            }
                        }
                    } else {
                        if (fileLow == ext) {
                            if (info.FileSystem == FileSystemID.None || info.FileSystem == fileSystem) {
                               return info.CommandMoniker;
                            }
                        }
                    }
                }
            }
            return null;
        }
       
        //=========================================================================================
        // 機　能：デフォルトの関連づけを取得する
        // 引　数：なし
        // 戻り値：デフォルトの関連づけ（設定がないときはnull）
        //=========================================================================================
        public ActionCommandMoniker GetDefaultCommand() {
            return m_defaultCommand;
        }

        //=========================================================================================
        // プロパティ：関連付け名
        //=========================================================================================
        public string DislayName {
            get {
                return m_displayName;
            }
            set {
                m_displayName = value;
            }
        }

        //=========================================================================================
        // プロパティ：拡張子に対する関連付け
        //=========================================================================================
        public List<AssociateInfo> AssociateExtList {
            get {
                return m_associateExtList;
            }
        }
        
        //=========================================================================================
        // クラス：関連づけソートのための比較クラス
        //=========================================================================================
        private class AssociateSettingSorter : IComparer<AssociateInfo> {

            //=========================================================================================
            // 機　能：2つの設定を比較する
            // 引　数：[in]info1  比較対象の設定１
            // 　　　　[in]info2  比較対象の設定２
            // 戻り値：比較結果
            //=========================================================================================
            public int Compare(AssociateInfo info1, AssociateInfo info2) {
                if (info1.ExtList == null && info2.ExtList == null) {
                    // フォルダ同士
                    return info1.FileSystem.IntId - info2.FileSystem.IntId;
                } else if (info1.ExtList == null && info2.ExtList != null) {
                    return -1;
                } else if (info1.ExtList != null && info2.ExtList == null) {
                    return -1;
                } else {
                    // ファイル同士
                    if (info1.FileSystem != info2.FileSystem) {
                        return info1.FileSystem.IntId - info2.FileSystem.IntId;
                    }
                    int count = Math.Max(info1.ExtList.Length, info2.ExtList.Length);
                    for (int i = 0; i < count; i++) {
                        string ext1;
                        if (i < info1.ExtList.Length) {
                            ext1 = info1.ExtList[i];
                        } else {
                            ext1 = "";
                        }
                        string ext2;
                        if (i < info2.ExtList.Length) {
                            ext2 = info2.ExtList[i];
                        } else {
                            ext2 = "";
                        }
                        if (ext1 != ext2) {
                            return string.Compare(ext1, ext2);
                        }
                    }
                }
                return 0;
            }
        }

        //=========================================================================================
        // クラス：関連づけの設定
        //=========================================================================================
        public class AssociateInfo : ICloneable {
            // 関連付ける拡張子の一覧（小文字、フォルダやデフォルトに対する関連づけのときnull）
            private string[] m_extList;

            // 実行対象のファイルシステム（関係ないときNone）
            private FileSystemID m_fileSystem;

            // 関連付けるコマンド
            private ActionCommandMoniker m_commandMoniker;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public AssociateInfo() {
            }

            //=========================================================================================
            // 機　能：クローンを作成する
            // 引　数：なし
            // 戻り値：作成したクローン
            //=========================================================================================
            public object Clone() {
                AssociateInfo obj = new AssociateInfo();
                if (m_extList == null) {
                    obj.m_extList = null;
                } else {
                    obj.m_extList = new string[m_extList.Length];
                    for (int i = 0; i < m_extList.Length; i++) {
                        obj.m_extList[i] = m_extList[i];
                    }
                }
                obj.m_fileSystem = m_fileSystem;
                obj.m_commandMoniker = (ActionCommandMoniker)(m_commandMoniker.Clone());
                return obj;
            }

            //=========================================================================================
            // プロパティ：関連付ける拡張子の一覧（小文字、フォルダやデフォルトに対する関連づけのときnull）
            //=========================================================================================
            public string[] ExtList {
                get {
                    return m_extList;
                }
                set {
                    m_extList = value;
                }
            }

            //=========================================================================================
            // プロパティ：実行対象のファイルシステム（関係ないときNone）
            //=========================================================================================
            public FileSystemID FileSystem {
                get {
                    return m_fileSystem;
                }
                set {
                    m_fileSystem = value;
                }
            }

            //=========================================================================================
            // プロパティ：関連付けるコマンド
            //=========================================================================================
            public ActionCommandMoniker CommandMoniker {
                get {
                    return m_commandMoniker;
                }
                set {
                    m_commandMoniker = value;
                }
            }
        }
    }
}
