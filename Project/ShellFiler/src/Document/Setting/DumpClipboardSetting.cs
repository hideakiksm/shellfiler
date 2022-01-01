using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ダンプビューアでのクリップボードコピー方法の設定
    //=========================================================================================
    public class DumpClipboardSetting : ICloneable {
        // ダンプ：桁数 の最大値
        public const int MAX_DUMP_WIDTH = 8;

        // ダンプ：改行間隔 の最大値
        public const int MAX_DUMP_LINE_WIDTH = 1000;

        // BASE64：フォールディングサイズ の最大値
        public const int MAX_BASE64_FOLDING_WIDTH = 1000;

        // ダンプのモード
        private DumpMode m_dumpMode = DumpMode.Dump;
        
        // テキスト：エンコーディング
        private Encoding m_textEncoding = Encoding.UTF8;
        
        // ダンプ：基数
        private NumericRadix m_dumpRadix = NumericRadix.Radix16Upper;
        
        // ダンプ：桁数
        private int m_dumpWidth = 2;

        // ダンプ：先頭に0をつけるときtrue
        private bool m_dumpZeroPadding = true;

        // ダンプ：接頭文字
        private string m_dumpPrefixString = "";

        // ダンプ：接尾文字
        private string m_dumpPostfixString = "";

        // ダンプ：セパレータ
        private string m_dumpSeparator = ", ";

        // ダンプ：改行間隔
        private int m_dumpLineWidth = 16;

        // BASE64：フォールディングサイズ
        private int m_base64FoldingWidth = 80;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DumpClipboardSetting() {
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
        public static bool EqualsConfig(DumpClipboardSetting obj1, DumpClipboardSetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_dumpMode != obj2.m_dumpMode) {
                return false;
            }
            if (obj1.m_dumpRadix != obj2.m_dumpRadix) {
                return false;
            }
            if (obj1.m_dumpWidth != obj2.m_dumpWidth) {
                return false;
            }
            if (obj1.m_dumpZeroPadding != obj2.m_dumpZeroPadding) {
                return false;
            }
            if (obj1.m_dumpPrefixString != obj2.m_dumpPrefixString) {
                return false;
            }
            if (obj1.m_dumpPostfixString != obj2.m_dumpPostfixString) {
                return false;
            }
            if (obj1.m_dumpSeparator != obj2.m_dumpSeparator) {
                return false;
            }
            if (obj1.m_dumpLineWidth != obj2.m_dumpLineWidth) {
                return false;
            }
            if (obj1.m_base64FoldingWidth != obj2.m_base64FoldingWidth) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：値を検証して修正する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ModifyValue() {
            if (m_dumpWidth > MAX_DUMP_WIDTH) {
                m_dumpWidth = MAX_DUMP_WIDTH;
            } else if (m_dumpWidth < 0) {
                m_dumpWidth = 0;
            }

            if (m_dumpLineWidth > MAX_DUMP_LINE_WIDTH) {
                m_dumpLineWidth = MAX_DUMP_LINE_WIDTH;
            } else if (m_dumpLineWidth < 0) {
                m_dumpLineWidth = 0;
            }

            if (m_base64FoldingWidth > MAX_BASE64_FOLDING_WIDTH) {
                m_base64FoldingWidth = MAX_BASE64_FOLDING_WIDTH;
            } else if (m_base64FoldingWidth < 0) {
                m_base64FoldingWidth = 0;
            }
        }

        //=========================================================================================
        // 機　能：クリップボードへコピーコマンド（テキストモード）でのデフォルト設定を返す
        // 引　数：[in]encoding  エンコード方式の設定
        // 戻り値：デフォルト設定
        //=========================================================================================
        public static DumpClipboardSetting GetTextDefaultSetting(Encoding encoding) {
            DumpClipboardSetting setting = new DumpClipboardSetting();
            setting.m_dumpMode = DumpMode.Text;
            setting.m_textEncoding = encoding;
            return setting;
        }

        //=========================================================================================
        // 機　能：クリップボードへコピーコマンド（ダンプモード）でのデフォルト設定を返す
        // 引　数：なし
        // 戻り値：デフォルト設定
        //=========================================================================================
        public static DumpClipboardSetting GetDumpDefaultSetting() {
            DumpClipboardSetting setting = new DumpClipboardSetting();
            setting.m_dumpMode = DumpMode.Dump;
            setting.m_dumpRadix = NumericRadix.Radix16Upper;
            setting.m_dumpWidth = 2;
            setting.m_dumpZeroPadding = true;
            setting.m_dumpPrefixString = "";
            setting.m_dumpPostfixString = "";
            setting.m_dumpSeparator = ",";
            setting.m_dumpLineWidth = 16;
            return setting;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out DumpClipboardSetting obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.DumpClipboardSetting_DumpClipboardSetting, SettingTagType.BeginObject, out fit);
            if (!success) {
                return false;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new DumpClipboardSetting();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.DumpClipboardSetting_DumpClipboardSetting) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.DumpClipboardSetting_DumpMode) {
                    // ダンプのモード
                    string stringValue = loader.StringValue;
                    if (stringValue == "Text") {
                        obj.m_dumpMode = DumpMode.Text;
                    } else if (stringValue == "Dump") {
                        obj.m_dumpMode = DumpMode.Dump;
                    } else if (stringValue == "Screen") {
                        obj.m_dumpMode = DumpMode.Screen;
                    } else if (stringValue == "Base64") {
                        obj.m_dumpMode = DumpMode.Base64;
                    } else if (stringValue == "View") {
                        obj.m_dumpMode = DumpMode.View;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.DumpClipboardSetting_DumpRadix) {
                    // ダンプ：基数
                    string stringValue = loader.StringValue;
                    if (stringValue == "Radix8") {
                        obj.m_dumpRadix = NumericRadix.Radix8;
                    } else if (stringValue == "Radix10") {
                        obj.m_dumpRadix = NumericRadix.Radix10;
                    } else if (stringValue == "Radix16Lower") {
                        obj.m_dumpRadix = NumericRadix.Radix16Lower;
                    } else if (stringValue == "Radix16Upper") {
                        obj.m_dumpRadix = NumericRadix.Radix16Upper;
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.DumpClipboardSetting_DumpWidth) {
                    // ダンプ：桁数
                    int intValue = loader.IntValue;
                    if (0 <= intValue && intValue <= MAX_DUMP_WIDTH) {
                        obj.m_dumpWidth = intValue;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.DumpClipboardSetting_DumpZeroPadding) {
                    // ダンプ：先頭に0をつけるときtrue
                    obj.m_dumpZeroPadding = loader.BoolValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.DumpClipboardSetting_DumpPrefixString) {
                    // ダンプ：接頭文字
                    obj.m_dumpPrefixString = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.DumpClipboardSetting_DumpPostfixString) {
                    // ダンプ：接尾文字
                    obj.m_dumpPostfixString = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.DumpClipboardSetting_DumpPostfixString) {
                    // ダンプ：セパレータ
                    obj.m_dumpSeparator = loader.StringValue;
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.DumpClipboardSetting_DumpLineWidth) {
                    // ダンプ：改行間隔
                    int intValue = loader.IntValue;
                    if (0 <= intValue && intValue <= MAX_DUMP_LINE_WIDTH) {
                        obj.m_dumpLineWidth = intValue;
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.DumpClipboardSetting_Base64FoldingWidth) {
                    // BASE64：フォールディングサイズ の最大値
                    int intValue = loader.IntValue;
                    if (0 <= intValue && intValue <= MAX_BASE64_FOLDING_WIDTH) {
                        obj.m_base64FoldingWidth = intValue;
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
        public static bool SaveSetting(SettingSaver saver, DumpClipboardSetting obj) {
            if (obj == null) {
                return true;
            }

            // ダンプのモード
            string strDumpMode = null;
            if (obj.m_dumpMode == DumpMode.Text) {
                strDumpMode = "Text";
            } else if (obj.m_dumpMode == DumpMode.Dump) {
                strDumpMode = "Dump";
            } else if (obj.m_dumpMode == DumpMode.Screen) {
                strDumpMode = "Screen";
            } else if (obj.m_dumpMode == DumpMode.Base64) {
                strDumpMode = "Base64";
            } else if (obj.m_dumpMode == DumpMode.View) {
                strDumpMode = "View";
            }

            // ダンプ：基数
            string strDumpRadix = null;
            if (obj.m_dumpRadix == NumericRadix.Radix8) {
                strDumpRadix = "Radix8";
            } else if (obj.m_dumpRadix == NumericRadix.Radix10) {
                strDumpRadix = "Radix10";
            } else if (obj.m_dumpRadix == NumericRadix.Radix16Lower) {
                strDumpRadix = "Radix16Lower";
            } else if (obj.m_dumpRadix == NumericRadix.Radix16Upper) {
                strDumpRadix = "Radix16Upper";
            }

            saver.StartObject(SettingTag.DumpClipboardSetting_DumpClipboardSetting);
            saver.AddString(SettingTag.DumpClipboardSetting_DumpMode, strDumpMode);
            saver.AddString(SettingTag.DumpClipboardSetting_DumpRadix, strDumpRadix);
            saver.AddInt(SettingTag.DumpClipboardSetting_DumpWidth, obj.m_dumpWidth);
            saver.AddBool(SettingTag.DumpClipboardSetting_DumpZeroPadding, obj.m_dumpZeroPadding);
            saver.AddString(SettingTag.DumpClipboardSetting_DumpPrefixString, obj.m_dumpPrefixString);
            saver.AddString(SettingTag.DumpClipboardSetting_DumpPostfixString, obj.m_dumpPostfixString);
            saver.AddString(SettingTag.DumpClipboardSetting_DumpSeparator, obj.m_dumpSeparator);
            saver.AddInt(SettingTag.DumpClipboardSetting_DumpLineWidth, obj.m_dumpLineWidth);
            saver.AddInt(SettingTag.DumpClipboardSetting_Base64FoldingWidth, obj.m_base64FoldingWidth);
            saver.EndObject(SettingTag.DumpClipboardSetting_DumpClipboardSetting);

            return true;
        }

        //=========================================================================================
        // プロパティ：ダンプのモード
        //=========================================================================================
        public DumpMode Mode {
            get {
                return m_dumpMode;
            }
            set {
                m_dumpMode = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：テキスト：エンコーディング
        //=========================================================================================
        public Encoding Encoding {
            get {
                return m_textEncoding;
            }
            set {
                m_textEncoding = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ダンプ：基数
        //=========================================================================================
        public NumericRadix DumpRadix {
            get {
                return m_dumpRadix;
            }
            set {
                m_dumpRadix = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ダンプ：桁数
        //=========================================================================================
        public int DumpWidth {
            get {
                return m_dumpWidth;
            }
            set {
                m_dumpWidth = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダンプ：先頭に0をつけるときtrue
        //=========================================================================================
        public bool DumpZeroPadding {
            get {
                return m_dumpZeroPadding;
            }
            set {
                m_dumpZeroPadding = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダンプ：接頭文字
        //=========================================================================================
        public string DumpPrefixString {
            get {
                return m_dumpPrefixString;
            }
            set {
                m_dumpPrefixString = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダンプ：接尾文字
        //=========================================================================================
        public string DumpPostfixString {
            get {
                return m_dumpPostfixString;
            }
            set {
                m_dumpPostfixString = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダンプ：セパレータ
        //=========================================================================================
        public string DumpSeparator {
            get {
                return m_dumpSeparator;
            }
            set {
                m_dumpSeparator = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダンプ：改行間隔
        //=========================================================================================
        public int DumpLineWidth {
            get {
                return m_dumpLineWidth;
            }
            set {
                m_dumpLineWidth = value;
            }
        }

        //=========================================================================================
        // プロパティ：BASE64：フォールディングサイズ
        //=========================================================================================
        public int Base64FoldingWidth {
            get {
                return m_base64FoldingWidth;
            }
            set {
                m_base64FoldingWidth = value;
            }
        }
    }

    //=========================================================================================
    // 列挙子：ダンプのモード
    //=========================================================================================
    public enum DumpMode {
        Text,               // テキスト
        Dump,               // ダンプ
        Screen,             // 画面表記と同じテキスト
        View,               // Viewイメージ
        Base64,             // BASE64
    }
}
