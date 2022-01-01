using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.FileSystem;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：フォルダサイズの取得条件
    //=========================================================================================
    public class RetrieveFolderSizeCondition {
        // フォルダサイズの取得モード
        private FolderSizeMode m_folderSizeMode = FolderSizeMode.TargetPathCluster;

        // フォルダサイズの取得単位（常に有効、1以上）
        private int m_folderSizeUnit = 1;

        // 取得結果をキャッシュするときtrue
        private bool m_useCache = true;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public RetrieveFolderSizeCondition() {
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
        public static bool EqualsConfig(RetrieveFolderSizeCondition obj1, RetrieveFolderSizeCondition obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_folderSizeMode != obj2.m_folderSizeMode) {
                return false;
            }
            if (obj1.m_folderSizeUnit != obj2.m_folderSizeUnit) {
                return false;
            }
            if (obj1.m_useCache != obj2.m_useCache) {
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
        public static bool LoadSetting(SettingLoader loader, out RetrieveFolderSizeCondition obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.RetrieveFolderSizeInfo, SettingTagType.BeginObject, out fit);
            if (!success) {
                return success;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new RetrieveFolderSizeCondition();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return success;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.RetrieveFolderSizeInfo) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.RetrieveFolderSizeInfo_Mode) {
                    obj.m_folderSizeMode = FolderSizeMode.FromString(loader.StringValue);
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.RetrieveFolderSizeInfo_SizeUnit) {
                    obj.m_folderSizeUnit = Math.Max(1, loader.IntValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.RetrieveFolderSizeInfo_UseCache) {
                    obj.m_useCache = loader.BoolValue;
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
        public static bool SaveSetting(SettingSaver saver, RetrieveFolderSizeCondition obj) {
            if (obj == null) {
                return true;
            }
            saver.StartObject(SettingTag.RetrieveFolderSizeInfo);
            saver.AddString(SettingTag.RetrieveFolderSizeInfo_Mode, obj.m_folderSizeMode.StringName);
            saver.AddInt(SettingTag.RetrieveFolderSizeInfo_SizeUnit, obj.m_folderSizeUnit);
            saver.AddBool(SettingTag.RetrieveFolderSizeInfo_UseCache, obj.m_useCache);
            saver.EndObject(SettingTag.RetrieveFolderSizeInfo);
            return true;
        }

        //=========================================================================================
        // プロパティ：フォルダサイズの取得モード
        //=========================================================================================
        public FolderSizeMode SizeMode {
            get {
                return m_folderSizeMode;
            }
            set {
                m_folderSizeMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：フォルダサイズの取得単位（常に有効、1以上）
        //=========================================================================================
        public int FolderSizeUnit {
            get {
                return m_folderSizeUnit;
            }
            set {
                m_folderSizeUnit = value;
            }
        }

        //=========================================================================================
        // プロパティ：取得結果をキャッシュするときtrue
        //=========================================================================================
        public bool UseCache {
            get {
                return m_useCache;
            }
            set {
                m_useCache = value;
            }
        }

        //=========================================================================================
        // クラス：フォルダサイズの取得モード
        //=========================================================================================
        public class FolderSizeMode {
            // 文字列表現からシンボルへのMap
            private static Dictionary<string, FolderSizeMode> m_nameToSymbol = new Dictionary<string, FolderSizeMode>();

            public static readonly FolderSizeMode OriginalSize        = new FolderSizeMode("OriginalSize");             // 元のサイズ
            public static readonly FolderSizeMode TargetPathCluster   = new FolderSizeMode("TargetPathCluster");        // 対象パスのクラスタサイズ
            public static readonly FolderSizeMode OppositePathCluster = new FolderSizeMode("OppositePathCluster");      // 反対パスのクラスタサイズ
            public static readonly FolderSizeMode SpecifiedSize       = new FolderSizeMode("SpecifiedSize");            // 指定値

            // 文字列表現
            private string m_stringName;
        
            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]stringName   文字列表現
            // 戻り値：なし
            //=========================================================================================
            public FolderSizeMode(string stringName) {
                m_stringName = stringName;
                m_nameToSymbol.Add(stringName, this);
            }
            
            //=========================================================================================
            // 機　能：文字列をシンボルに変換する
            // 引　数：[in]stringName   文字列表現
            // 戻り値：シンボル（対応するシンボルがないときnull）
            //=========================================================================================
            public static FolderSizeMode FromString(string stringName) {
                if (m_nameToSymbol.ContainsKey(stringName)) {
                    return m_nameToSymbol[stringName];
                } else {
                    return null;
                }
            }

            //=========================================================================================
            // プロパティ：文字列表現
            //=========================================================================================
            public string StringName {
                get {
                    return m_stringName;
                }
            }
        }
    }
}
