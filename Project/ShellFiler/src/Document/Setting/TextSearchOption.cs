using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Util;
using ShellFiler.FileViewer;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：テキストビューアでの検索オプション
    //=========================================================================================
    public class TextSearchOption : ICloneable {
        // 検索方法
        private TextSearchMode m_searchMode = TextSearchMode.NormalIgnoreCase;
        
        // 語句単位にするときtrue
        private bool m_searchWord = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TextSearchOption() {
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
        public static bool EqualsConfig(TextSearchOption obj1, TextSearchOption obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_searchMode != obj2.m_searchMode) {
                return false;
            }
            if (obj1.m_searchWord != obj2.m_searchWord) {
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
        public static bool LoadSetting(SettingLoader loader, out TextSearchOption obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.TextSearchOption_TextSearchOption, SettingTagType.BeginObject, out fit);
            if (!success) {
                return false;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new TextSearchOption();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.TextSearchOption_TextSearchOption) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.TextSearchOption_TextSearchOption) {
                    // 検索方法
                    string stringValue = loader.StringValue;
                    if (stringValue == "NormalIgnoreCase") {
                        obj.m_searchMode = TextSearchMode.NormalIgnoreCase;
                    } else if (stringValue == "NormalCaseSensitive") {
                        obj.m_searchMode = TextSearchMode.NormalCaseSensitive;
                    } else if (stringValue == "WildCardIgnoreCase") {
                        obj.m_searchMode = TextSearchMode.WildCardIgnoreCase;
                    } else if (stringValue == "WildCardCaseSensitive") {
                        obj.m_searchMode = TextSearchMode.WildCardCaseSensitive;
                    } else if (stringValue == "RegularExpression") {
                        obj.m_searchMode = TextSearchMode.RegularExpression;
                    } else {
                        obj.m_searchMode = new TextSearchOption().m_searchMode;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.TextSearchOption_SearchWord) {
                    // 語句単位にするときtrue
                    obj.m_searchWord = loader.BoolValue;
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
        public static bool SaveSetting(SettingSaver saver, TextSearchOption obj) {
            if (obj == null) {
                return true;
            }

            // 検索方法
            string strSearchMode = null;
            if (obj.m_searchMode == TextSearchMode.NormalIgnoreCase) {
                strSearchMode = "NormalIgnoreCase";
            } else if (obj.m_searchMode == TextSearchMode.NormalCaseSensitive) {
                strSearchMode = "NormalCaseSensitive";
            } else if (obj.m_searchMode == TextSearchMode.WildCardIgnoreCase) {
                strSearchMode = "WildCardIgnoreCase";
            } else if (obj.m_searchMode == TextSearchMode.WildCardCaseSensitive) {
                strSearchMode = "WildCardCaseSensitive";
            } else if (obj.m_searchMode == TextSearchMode.RegularExpression) {
                strSearchMode = "RegularExpression";
            }

            saver.StartObject(SettingTag.TextSearchOption_TextSearchOption);
            saver.AddString(SettingTag.TextSearchOption_SearchMode, strSearchMode);
            saver.AddBool(SettingTag.TextSearchOption_SearchWord, obj.m_searchWord);
            saver.EndObject(SettingTag.TextSearchOption_TextSearchOption);

            return true;
        }

        //=========================================================================================
        // プロパティ：検索方法
        //=========================================================================================
        public TextSearchMode SearchMode {
            get {
                return m_searchMode;
            }
            set {
                m_searchMode = value;
            }
        }
        
        //=========================================================================================
        // 語句単位にするときtrue
        //=========================================================================================
        public bool SearchWord {
            get {
                return m_searchWord;
            }
            set {
                m_searchWord = value;
            }
        }
    }
}
