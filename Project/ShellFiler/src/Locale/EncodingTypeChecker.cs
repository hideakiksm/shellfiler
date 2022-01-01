using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Locale {

    //=========================================================================================
    // クラス：指定されたEncodingTypeに対応する文字コードかどうかを判断するクラス
    //=========================================================================================
    public abstract class EncodingTypeChecker {
        // コード定義
        private byte BYTE_TAB = 0x09;                                // TAB
        private byte BYTE_CR  = 0x0d;                                // CR
        private byte BYTE_LF  = 0x0a;                                // LF
        private byte BYTE_ESC = 0x1B;                                // ESC

        //=========================================================================================
        // 機　能：文字コードを確認する
        // 引　数：[in]src  確認対象のバイト列
        // 戻り値：このクラスの文字コードのときtrue
        //=========================================================================================
        public abstract bool CheckBytes(byte[] src);

        //=========================================================================================
        // 機　能：認識できるテキストであるかどうかを返す
        // 引　数：[in]src  確認対象のバイト列
        // 戻り値：認識できたエンコード（認識できないときnull）
        //=========================================================================================
        public static EncodingType IsText(byte[] src) {
            bool check;
            check = EncodingType.UTF8.EncodingTypeChecker.CheckBytes(src);
            if (check) {
                return EncodingType.UTF8;
            }
            check = EncodingType.SJIS.EncodingTypeChecker.CheckBytes(src);
            if (check) {
                return EncodingType.SJIS;
            }
            check = EncodingType.EUC.EncodingTypeChecker.CheckBytes(src);
            if (check) {
                return EncodingType.EUC;
            }
            check = EncodingType.JIS.EncodingTypeChecker.CheckBytes(src);
            if (check) {
                return EncodingType.JIS;
            }
            check = EncodingType.UNICODE.EncodingTypeChecker.CheckBytes(src);
            if (check) {
                return EncodingType.UNICODE;
            }
            return null;
        }

        //=========================================================================================
        // クラス：UTF-8であることを確認するクラス
        //=========================================================================================
        public class Utf8 : EncodingTypeChecker {

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public Utf8() {
            }

            //=========================================================================================
            // 機　能：文字コードを確認する
            // 引　数：[in]src  確認対象のバイト列
            // 戻り値：このクラスの文字コードのときtrue
            //=========================================================================================
            public override bool CheckBytes(byte[] src) {
                // 1バイト文字  0xxxxxxx
                // 2バイト文字  110yyyyx 10xxxxxx 
                // 3バイト文字  1110yyyy 10yxxxxx 10xxxxxx
                // 4バイト文字  11110yyy 10yyxxxx 10xxxxxx 10xxxxxx
                // 5バイト文字  111110yy 10yyyxxx 10xxxxxx 10xxxxxx 10xxxxxx
                // 6バイト文字  1111110y 10yyyyxx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx
                
                int length = src.Length;
                for (int i = 0; i < length; i++) {
                    byte first = src[i];
                    if (first < 0x20) {                                         // バイナリ
                        if (first == BYTE_TAB || first == BYTE_CR || first == BYTE_LF) {
                            ;
                        } else {
                            return false;
                        }
                    } else if ((first & 0x80) == 0) {                           // 1バイト文字
                        if (first == 0x7f) {
                            return false;
                        }
                    } else if ((first & 0xe0) == 0xc0) {                        // 2バイト文字
                        if (i < length - 1 && (src[i + 1] & 0xc0) == 0x80) {
                            i++;
                        } else {
                            return false;
                        }
                    } else if ((first & 0xf0) == 0xe0) {                        // 3バイト文字
                        if (i < length - 2 && (src[i + 1] & 0xc0) == 0x80 && (src[i + 2] & 0xc0) == 0x80) {
                            i += 2;
                        } else {
                            return false;
                        }
                    } else if ((first & 0xf8) == 0xf0) {                        // 4バイト文字
                        if (i < length - 3 && (src[i + 1] & 0xc0) == 0x80 && (src[i + 2] & 0xc0) == 0x80 && (src[i + 3] & 0xc0) == 0x80) {
                            i += 3;
                        } else {
                            return false;
                        }
                    } else if ((first & 0xfc) == 0xf8) {                        // 5バイト文字
                        if (i < length - 4 && (src[i + 1] & 0xc0) == 0x80 && (src[i + 2] & 0xc0) == 0x80 && (src[i + 3] & 0xc0) == 0x80 && (src[i + 4] & 0xc0) == 0x80) {
                            i += 4;
                        } else {
                            return false;
                        }
                    } else if ((first & 0xfe) == 0xfc) {                        // 6バイト文字
                        if (i < length - 5 && (src[i + 1] & 0xc0) == 0x80 && (src[i + 2] & 0xc0) == 0x80 && (src[i + 3] & 0xc0) == 0x80 && (src[i + 4] & 0xc0) == 0x80 && (src[i + 5] & 0xc0) == 0x80) {
                            i += 5;
                        } else {
                            return false;
                        }
                    } else {
                        return false;
                    }
                }
                return true;
            }
        }

        //=========================================================================================
        // クラス：ShiftJISであることを確認するクラス
        //=========================================================================================
        public class SJis : EncodingTypeChecker {

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public SJis() {
            }

            //=========================================================================================
            // 機　能：文字コードを確認する
            // 引　数：[in]src  確認対象のバイト列
            // 戻り値：このクラスの文字コードのときtrue
            //=========================================================================================
            public override bool CheckBytes(byte[] src) {
                // 第1バイト 81～9f  e0～ef
                // 第2バイト 40～7e  80～fc
                int length = src.Length;
                for (int i = 0; i < length; i++) {
                    byte first = src[i];
                    if (first < 0x20) {
                        if (first == BYTE_TAB || first == BYTE_CR || first == BYTE_LF) {
                            ;
                        } else {
                            return false;
                        }
                    } else if (first < 0x7f) {
                        ;
                    } else if (0x81 <= first && first <= 0x9f || 0xe0 <= first && first <= 0xef) {
                        if (i >= length - 1) {
                            return false;
                        }
                        byte second = src[i + 1];
                        if (0x40 <= second && second <= 0x7e || 0x80 <= second && second <= 0xfc) {
                            ;
                        } else {
                            return false;
                        }
                        i++;
                    } else {
                        return false;
                    }
                }
                return true;
            }
        }

        //=========================================================================================
        // クラス：EUC-JPであることを確認するクラス
        //=========================================================================================
        public class EucJp : EncodingTypeChecker {

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public EucJp() {
            }

            //=========================================================================================
            // 機　能：文字コードを確認する
            // 引　数：[in]src  確認対象のバイト列
            // 戻り値：このクラスの文字コードのときtrue
            //=========================================================================================
            public override bool CheckBytes(byte[] src) {
                // 2バイト文字のとき
                //   第1バイト a1～fe
                //   第2バイト a1～fe
                // 3バイト文字のとき
                //   第1バイト 8f
                //   第2バイト a1～fe
                //   第3バイト a1～fe
                int length = src.Length;
                for (int i = 0; i < length; i++) {
                    byte first = src[i];
                    if (first < 0x20) {
                        if (first == BYTE_TAB || first == BYTE_CR || first == BYTE_LF) {
                            ;
                        } else {
                            return false;
                        }
                    } else if (first < 0x7f) {
                        ;
                    } else if (first == 0x8f) {
                        if (i >= length - 2) {
                            return false;
                        }
                        byte second = src[i + 1];
                        byte third = src[i + 2];
                        if (0xa1 <= second && second <= 0xfe) {
                            ;
                        } else {
                            return false;
                        }
                        if (0xa1 <= third && third <= 0xfe) {
                            ;
                        } else {
                            return false;
                        }
                        i += 2;
                    } else if (0xa1 <= first && first <= 0xfe) {
                        if (i >= length - 1) {
                            return false;
                        }
                        byte second = src[i + 1];
                        if (0xa1 <= second && second <= 0xfe) {
                            ;
                        } else {
                            return false;
                        }
                        i++;
                    } else {
                        return false;
                    }
                }
                return true;
            }
        }

        //=========================================================================================
        // クラス：JISであることを確認するクラス
        //=========================================================================================
        public class Jis : EncodingTypeChecker {

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public Jis() {
            }

            //=========================================================================================
            // 機　能：文字コードを確認する
            // 引　数：[in]src  確認対象のバイト列
            // 戻り値：このクラスの文字コードのときtrue
            //=========================================================================================
            public override bool CheckBytes(byte[] src) {
                // ESC $ @ 漢字の開始（旧JIS漢字 JIS C 6226-1978） 
                // ESC $ B 漢字の開始 (新JIS漢字 JIS X 0208-1983） 
                // ESC $ ( D 漢字の開始 (JIS X 0208-1990）
                // ESC $ ( Q
                // ESC $ ( P
                // ESC ( B ASCIIの開始 
                // ESC ( J JISローマ字の開始 
                // ESC ( I 半角カタカナの開始 
                //
                // ASCII:20～7e
                // 漢字:2121～7e7e
                // 半角カナ:20～5f  a1～df
                JIS_STATE jisState = JIS_STATE.ASCII;
                int length = src.Length;
                for (int i = 0; i < length; i++) {
                    byte first = src[i];
                    if (first == BYTE_ESC) {
                        if (i < length - 2 && src[i + 1] == '$') {
                            if (src[i + 2] == '@') {                // ESC $ @
                                jisState = JIS_STATE.KANJI;
                                i += 2;
                            } else if (src[i + 2] == 'B') {         // ESC $ B
                                jisState = JIS_STATE.KANJI;
                                i += 2;
                            } else if (src[i + 2] == '(' && i < length - 3) {
                                if (src[i + 3] == 'D') {            // ESC $ ( D
                                    jisState = JIS_STATE.KANJI;
                                } else if (src[i + 3] == 'Q') {     // ESC $ ( Q
                                    jisState = JIS_STATE.KANJI;
                                } else if (src[i + 3] == 'P') {     // ESC $ ( P
                                    jisState = JIS_STATE.KANJI;
                                } else {
                                    return false;
                                }
                                i += 3;
                            } else {
                                return false;
                            }
                        } else if (i < length - 2 && src[i + 1] == '(') {
                            if (src[i + 2] == 'B') {
                                jisState = JIS_STATE.ASCII;
                            } else if (src[i + 2] == 'J') {
                                jisState = JIS_STATE.ASCII;
                            } else if (src[i + 2] == 'I') {
                                jisState = JIS_STATE.KANJI;
                            } else {
                                return false;
                            }
                        } else {
                            return false;
                        }
                    } else if (first < 0x20) {
                        // 制御文字
                        if (first == BYTE_TAB || first == BYTE_CR || first == BYTE_LF) {
                            ;
                        } else {
                            return false;
                        }
                    } else {
                        if (jisState == JIS_STATE.ASCII) {
                            // ASCII
                            if (first <= 0x7e) {
                                ;
                            } else {
                                return false;
                            }
                        } else if (jisState == JIS_STATE.KANJI) {
                            // 漢字
                            if (i >= length - 1) {
                                return false;
                            }
                            byte second = src[i + 1];
                            if (0x21 <= first && first <= 0x7e && 0x21 <= second && second <= 0x7e) {
                                ;
                            } else {
                                return false;
                            }
                        } else {
                            // 半角カナ
                            if (0x20 <= first && first <= 0x5f || 0xa1 <= first && first <= 0xdf) {
                                ;
                            } else {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }

            //=========================================================================================
            // 列挙子：JISコードの状態
            //=========================================================================================
            private enum JIS_STATE {
                KANJI,
                ASCII,
                KANA,
            }
        }

        //=========================================================================================
        // クラス：UNICODEであることを確認するクラス
        //=========================================================================================
        public class Unicode : EncodingTypeChecker {

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public Unicode() {
            }

            //=========================================================================================
            // 機　能：文字コードを確認する
            // 引　数：[in]src  確認対象のバイト列
            // 戻り値：このクラスの文字コードのときtrue
            //=========================================================================================
            public override bool CheckBytes(byte[] src) {
                if (src.Length % 2 == 1) {
                    return false;
                }
                int length = src.Length;
                for (int i = 0; i < length - 1; i += 2) {
                    if (src[i + 1] == 0) {
                        byte first = src[i];
                        if (first == BYTE_CR || first == BYTE_LF || first == BYTE_TAB) {
                            ;
                        } else {
                            return false;
                        }
                    }
                }
                return false;
            }
        }
    }
}
