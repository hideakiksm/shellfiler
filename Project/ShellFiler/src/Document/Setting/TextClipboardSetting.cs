using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Text;
using ShellFiler.Document;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：テキストビューアでのクリップボードコピー方法の設定
    //=========================================================================================
    public class TextClipboardSetting : ICloneable {
        // 改行モード
        private CopyLineBreakMode m_lineBreakMode = CopyLineBreakMode.Original;
        
        // タブモード
        private CopyTabMode m_tabMode = CopyTabMode.Original;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TextClipboardSetting() {
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
        public static bool EqualsConfig(TextClipboardSetting obj1, TextClipboardSetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_lineBreakMode != obj2.m_lineBreakMode) {
                return false;
            }
            if (obj1.m_tabMode != obj2.m_tabMode) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：クリップボードへコピーコマンドでのデフォルト設定を返す
        // 引　数：なし
        // 戻り値：デフォルト設定
        //=========================================================================================
        public static TextClipboardSetting GetDefaultSetting() {
            TextClipboardSetting setting = new TextClipboardSetting();
            setting.m_lineBreakMode = CopyLineBreakMode.Original;
            setting.m_tabMode = CopyTabMode.Original;
            return setting;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out TextClipboardSetting obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.TextClipboardSetting_TextClipboardSetting, SettingTagType.BeginObject, out fit);
            if (!success) {
                return false;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new TextClipboardSetting();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.TextClipboardSetting_TextClipboardSetting) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.TextClipboardSetting_LineBreakMode) {
                    // 改行モード
                    string stringValue = loader.StringValue;
                    if (stringValue == "Original") {
                        obj.m_lineBreakMode = CopyLineBreakMode.Original;
                    } else if (stringValue == "Cr") {
                        obj.m_lineBreakMode = CopyLineBreakMode.Cr;
                    } else if (stringValue == "CrLf") {
                        obj.m_lineBreakMode = CopyLineBreakMode.CrLf;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.TextClipboardSetting_TabMode) {
                    // タブモード
                    string stringValue = loader.StringValue;
                    if (stringValue == "Original") {
                        obj.m_tabMode = CopyTabMode.Original;
                    } else if (stringValue == "ConvertSpace") {
                        obj.m_tabMode = CopyTabMode.ConvertSpace;
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
        public static bool SaveSetting(SettingSaver saver, TextClipboardSetting obj) {
            if (obj == null) {
                return true;
            }

            // 改行モード
            string strLineBreakMode = null;
            if (obj.m_lineBreakMode == CopyLineBreakMode.Original) {
                strLineBreakMode = "Original";
            } else if (obj.m_lineBreakMode == CopyLineBreakMode.Cr) {
                strLineBreakMode = "Cr";
            } else if (obj.m_lineBreakMode == CopyLineBreakMode.CrLf) {
                strLineBreakMode = "CrLf";
            }
        
            // タブモード
            string strTabMode = null;
            if (obj.m_tabMode == CopyTabMode.Original) {
                strTabMode = "Original";
            } else if (obj.m_tabMode == CopyTabMode.ConvertSpace) {
                strTabMode = "ConvertSpace";
            }

            saver.StartObject(SettingTag.TextClipboardSetting_TextClipboardSetting);
            saver.AddString(SettingTag.TextClipboardSetting_LineBreakMode, strLineBreakMode);
            saver.AddString(SettingTag.TextClipboardSetting_TabMode, strTabMode);
            saver.EndObject(SettingTag.TextClipboardSetting_TextClipboardSetting);

            return true;
        }

        //=========================================================================================
        // プロパティ：改行モード
        //=========================================================================================
        public CopyLineBreakMode LineBreakMode {
            get {
                return m_lineBreakMode;
            }
            set {
                m_lineBreakMode = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：タブモード
        //=========================================================================================
        public CopyTabMode TabMode {
            get {
                return m_tabMode;
            }
            set {
                m_tabMode = value;
            }
        }

        //=========================================================================================
        // 列挙子：改行モード
        //=========================================================================================
        public enum CopyLineBreakMode {
            // 元のテキストの改行方法
            Original,
            // 常にCRのみ
            Cr,
            // 常にCR+LF
            CrLf,
        }

        //=========================================================================================
        // 列挙子：タブモード
        //=========================================================================================
        public enum CopyTabMode {
            // 元のタブを使用
            Original,
            // 空白に変換
            ConvertSpace
        }
    }
}
