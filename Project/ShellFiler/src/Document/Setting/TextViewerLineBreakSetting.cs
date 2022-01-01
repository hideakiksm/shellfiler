using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // プロパティ：テキストビューアでの折り返し設定
    //=========================================================================================
    public class TextViewerLineBreakSetting : ICloneable {
        // テキスト 文字数で改行する設定の最小値
        public const int MIN_TEXT_BREAK_CHAR_COUNT = 20;

        // テキスト 文字数で改行する設定の最大値
        public const int MAX_TEXT_BREAK_CHAR_COUNT = 10000;

        // テキスト ピクセル数で改行する設定の最小値
        public const int MIN_TEXT_BREAK_PIXEL_COUNT = 200;

        // テキスト ピクセル数で改行する設定の最大値
        public const int MAX_TEXT_BREAK_PIXEL_COUNT = 10000;

        // ダンプ １行あたりの設定の最小値
        public const int MIN_DUMP_LINE_BYTE_COUNT = 8;

        // ダンプ １行あたりの設定の最大値
        public const int MAX_DUMP_LINE_BYTE_COUNT = 64;

        // 折り返しモード
        private TextViewerLineBreakMode m_lineBreakMode = TextViewerLineBreakSetting.TextViewerLineBreakMode.BreakByChar;
        
        // ピクセル数での折り返し幅（0:ウィンドウに合わせる、1:画面幅に合わせる、2:マルチモニタに合わせる）
        private int m_breakPixel = 1;

        // 文字数での折り返し幅（0:ウィンドウに合わせる、1:画面幅に合わせる、2:マルチモニタに合わせる）
        private int m_breakCharCount = 1;

        // ダンプ表示で１行に表示するバイト数
        private int m_dumpLineByteCount = 16;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TextViewerLineBreakSetting() {
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
        public static bool EqualsConfig(TextViewerLineBreakSetting obj1, TextViewerLineBreakSetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_lineBreakMode != obj2.m_lineBreakMode) {
                return false;
            }
            if (obj1.m_breakPixel != obj2.m_breakPixel) {
                return false;
            }
            if (obj1.m_breakCharCount != obj2.m_breakCharCount) {
                return false;
            }
            if (obj1.m_dumpLineByteCount != obj2.m_dumpLineByteCount) {
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
        public static bool LoadSetting(SettingLoader loader, out TextViewerLineBreakSetting obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.TextViewerLineBreak_TextViewerLineBreak, SettingTagType.BeginObject, out fit);
            if (!success) {
                return false;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new TextViewerLineBreakSetting();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.TextViewerLineBreak_TextViewerLineBreak) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.TextViewerLineBreak_LineBreakMode) {
                    // 折り返しモード
                    string stringValue = loader.StringValue;
                    if (stringValue == "NoBreak") {
                        obj.m_lineBreakMode = TextViewerLineBreakMode.NoBreak;
                    } else if (stringValue == "BreakByPixel") {
                        obj.m_lineBreakMode = TextViewerLineBreakMode.BreakByPixel;
                    } else if (stringValue == "BreakByChar") {
                        obj.m_lineBreakMode = TextViewerLineBreakMode.BreakByChar;
                    } else {
                        obj.m_lineBreakMode = TextViewerLineBreakMode.BreakByChar;
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.TextViewerLineBreak_BreakPixel) {
                    // ピクセル数での折り返し幅（0:ウィンドウに合わせる、1:画面幅に合わせる、2:マルチモニタに合わせる）
                    int intValue = loader.IntValue;
                    if (intValue <= 2 || MIN_TEXT_BREAK_PIXEL_COUNT <= intValue && intValue <= MAX_TEXT_BREAK_PIXEL_COUNT) {
                        obj.m_breakPixel = intValue;
                    } else {
                        obj.m_breakPixel = new TextViewerLineBreakSetting().m_breakPixel;
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.TextViewerLineBreak_BreakCharCount) {
                    // 文字数での折り返し幅（0:ウィンドウに合わせる、1:画面幅に合わせる、2:マルチモニタに合わせる）
                    int intValue = loader.IntValue;
                    if (intValue <= 2 || MIN_TEXT_BREAK_CHAR_COUNT <= intValue && intValue <= MAX_TEXT_BREAK_PIXEL_COUNT) {
                        obj.m_breakCharCount = intValue;
                    } else {
                        obj.m_breakCharCount = new TextViewerLineBreakSetting().m_breakCharCount;
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.TextViewerLineBreak_DumpLineByteCount) {
                    // ダンプ表示で１行に表示するバイト数
                    int intValue = loader.IntValue;
                    if (MIN_DUMP_LINE_BYTE_COUNT <= intValue && intValue <= MAX_DUMP_LINE_BYTE_COUNT) {
                        obj.m_dumpLineByteCount = intValue;
                    } else {
                        obj.m_dumpLineByteCount = new TextViewerLineBreakSetting().m_dumpLineByteCount;
                    }
                    obj.m_dumpLineByteCount = loader.IntValue;
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
        public static bool SaveSetting(SettingSaver saver, TextViewerLineBreakSetting obj) {
            if (obj == null) {
                return true;
            }

            // 折り返しモード
            string strLineBreakMode = null;
            if (obj.m_lineBreakMode == TextViewerLineBreakMode.NoBreak) {
                strLineBreakMode = "NoBreak";
            } else if (obj.m_lineBreakMode == TextViewerLineBreakMode.BreakByPixel) {
                strLineBreakMode = "BreakByPixel";
            } else if (obj.m_lineBreakMode == TextViewerLineBreakMode.BreakByChar) {
                strLineBreakMode = "BreakByChar";
            }

            saver.StartObject(SettingTag.TextViewerLineBreak_TextViewerLineBreak);
            saver.AddString(SettingTag.TextViewerLineBreak_LineBreakMode, strLineBreakMode);
            saver.AddInt(SettingTag.TextViewerLineBreak_BreakPixel, obj.m_breakPixel);
            saver.AddInt(SettingTag.TextViewerLineBreak_BreakCharCount, obj.m_breakCharCount);
            saver.AddInt(SettingTag.TextViewerLineBreak_DumpLineByteCount, obj.m_dumpLineByteCount);
            saver.EndObject(SettingTag.TextViewerLineBreak_TextViewerLineBreak);

            return true;
        }

        //=========================================================================================
        // プロパティ：折り返しモード
        //=========================================================================================
        public TextViewerLineBreakMode LineBreakMode {
            get {
                return m_lineBreakMode;
            }
            set {
                m_lineBreakMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：ピクセル数での折り返し幅（0:ウィンドウに合わせる、1:画面幅に合わせる、2:マルチモニタに合わせる）
        //=========================================================================================
        public int BreakPixel {
            get {
                return m_breakPixel;
            }
            set {
                m_breakPixel = value;
            }
        }

        //=========================================================================================
        // プロパティ：文字数での折り返し幅（0:ウィンドウに合わせる、1:画面幅に合わせる、2:マルチモニタに合わせる）
        //=========================================================================================
        public int BreakCharCount {
            get {
                return m_breakCharCount;
            }
            set {
                m_breakCharCount = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダンプ表示で１行に表示するバイト数
        //=========================================================================================
        public int DumpLineByteCount {
            get {
                return m_dumpLineByteCount;
            }
            set {
                m_dumpLineByteCount = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューアでの折り返しモード
        //=========================================================================================
        public enum TextViewerLineBreakMode {
            NoBreak,                        // 折り返しなし
            BreakByPixel,                   // ピクセル数で折り返し
            BreakByChar,                    // 半角文字数で折り返し
        }
    }
}
