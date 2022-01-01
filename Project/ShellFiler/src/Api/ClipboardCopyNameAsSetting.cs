using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：クリップボードにコピーする設定
    //=========================================================================================
    public class ClipboardCopyNameAsSetting : ICloneable {
        // セパレータのモード
        private SeparatorMode m_separatorMode = SeparatorMode.SeparatorReturn;

        // 引用符のモード
        private QuoteMode m_quoteMode = QuoteMode.QuoteNone;

        // フォルダ名を付加するときtrue
        private bool m_fullPath = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ClipboardCopyNameAsSetting() {
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
        public static bool EqualsConfig(ClipboardCopyNameAsSetting obj1, ClipboardCopyNameAsSetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_separatorMode != obj2.m_separatorMode) {
                return false;
            }
            if (obj1.m_quoteMode != obj2.m_quoteMode) {
                return false;
            }
            if (obj1.m_fullPath != obj2.m_fullPath) {
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
        public static bool LoadSetting(SettingLoader loader, out ClipboardCopyNameAsSetting obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.ClipboardCopyNameAs_ClipboardCopyNameAs, SettingTagType.BeginObject, out fit);
            if (!success) {
                return false;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new ClipboardCopyNameAsSetting();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.ClipboardCopyNameAs_ClipboardCopyNameAs) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.ClipboardCopyNameAs_SeparatorMode) {
                    // セパレータのモード
                    string stringValue = loader.StringValue;
                    if (stringValue == "SeparatorSpace") {
                        obj.m_separatorMode = SeparatorMode.SeparatorSpace;
                    } else if (stringValue == "SeparatorTab") {
                        obj.m_separatorMode = SeparatorMode.SeparatorTab;
                    } else if (stringValue == "SeparatorComma") {
                        obj.m_separatorMode = SeparatorMode.SeparatorComma;
                    } else if (stringValue == "SeparatorReturn") {
                        obj.m_separatorMode = SeparatorMode.SeparatorReturn;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.ClipboardCopyNameAs_QuotaMode) {
                    // 引用符のモード
                    string stringValue = loader.StringValue;
                    if (stringValue == "QuoteAlways") {
                        obj.m_quoteMode = QuoteMode.QuoteAlways;
                    } else if (stringValue == "QuoteSpace") {
                        obj.m_quoteMode = QuoteMode.QuoteSpace;
                    } else if (stringValue == "QuoteNone") {
                        obj.m_quoteMode = QuoteMode.QuoteNone;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.ClipboardCopyNameAs_FullPath) {
                    // フォルダ名を付加するときtrue
                    obj.m_fullPath = loader.BoolValue;
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
        public static bool SaveSetting(SettingSaver saver, ClipboardCopyNameAsSetting obj) {
            if (obj == null) {
                return true;
            }

            // セパレータのモード
            string strSeparatorMode = null;
            if (obj.m_separatorMode == SeparatorMode.SeparatorSpace) {
                strSeparatorMode = "SeparatorSpace";
            } else if (obj.m_separatorMode == SeparatorMode.SeparatorTab) {
                strSeparatorMode = "SeparatorTab";
            } else if (obj.m_separatorMode == SeparatorMode.SeparatorComma) {
                strSeparatorMode = "SeparatorComma";
            } else if (obj.m_separatorMode == SeparatorMode.SeparatorReturn) {
                strSeparatorMode = "SeparatorReturn";
            }

            // 引用符のモード
            string strQuotaMode = null;
            if (obj.m_quoteMode == QuoteMode.QuoteAlways) {
                strQuotaMode = "QuoteAlways";
            } else if (obj.m_quoteMode == QuoteMode.QuoteSpace) {
                strQuotaMode = "QuoteSpace";
            } else if (obj.m_quoteMode == QuoteMode.QuoteNone) {
                strQuotaMode = "QuoteNone";
            }

            saver.StartObject(SettingTag.ClipboardCopyNameAs_ClipboardCopyNameAs);
            saver.AddString(SettingTag.ClipboardCopyNameAs_SeparatorMode, strSeparatorMode);
            saver.AddString(SettingTag.ClipboardCopyNameAs_QuotaMode, strQuotaMode);
            saver.AddBool(SettingTag.ClipboardCopyNameAs_FullPath, obj.m_fullPath);
            saver.EndObject(SettingTag.ClipboardCopyNameAs_ClipboardCopyNameAs);

            return true;
        }

        //=========================================================================================
        // プロパティ：セパレータのモード
        //=========================================================================================
        public SeparatorMode Separator {
            get {
                return m_separatorMode;
            }
            set {
                m_separatorMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：引用符のモード
        //=========================================================================================
        public QuoteMode Quote {
            get {
                return m_quoteMode;
            }
            set {
                m_quoteMode = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：フォルダ名を付加するときtrue
        //=========================================================================================
        public bool FullPath {
            get {
                return m_fullPath;
            }
            set {
                m_fullPath = value;
            }
        }

        //=========================================================================================
        // 列挙子：セパレータのモード
        //=========================================================================================
        public enum SeparatorMode {
            // 空白
            SeparatorSpace,
            // タブ
            SeparatorTab,
            // カンマ
            SeparatorComma,
            // 改行
            SeparatorReturn,
        }

        //=========================================================================================
        // 列挙子：引用符のモード
        //=========================================================================================
        public enum QuoteMode {
            // 常につける
            QuoteAlways,
            // 空白を含む場合のみ
            QuoteSpace,
            // 常になし
            QuoteNone,
        }
    }
}
