using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Command.FileViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;

namespace ShellFiler.Locale {

    //=========================================================================================
    // クラス：ファイルビューアのダンプ表示に使用する文字列の変換クラス
    //=========================================================================================
    class DumpTextFormatter {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent   所有ビュー
        // 　　　　[in]fontSize 半角１文字分の大きさの期待値
        // 戻り値：なし
        //=========================================================================================
        public DumpTextFormatter() {
        }

        //=========================================================================================
        // 機　能：バイト列の指定範囲を文字列に変換する
        // 引　数：[in]encoding    解釈に使用するエンコーディング
        // 　　　　[in]buffer      変換対象のバッファ
        // 　　　　[in]start       変換開始位置
        // 　　　　[in]length      変換された長さ
        // 　　　　[out]result     変換した結果の文字列を返す変数
        // 　　　　[out]charToByte 文字位置をバイト位置に変換する位置情報を返す変数
        // 　　　　[out]byteToChar バイト位置を文字位置に変換する位置情報を返す変数
        // 戻り値：なし
        // メ　モ：変換対象が82 A0 41のとき、result="あA"、charToByte={0,2}、byteToChar={0,0,1}
        //=========================================================================================
        public void Convert(EncodingType encoding, byte[] buffer, int start, int length, out string result, out List<int> charToByte, out List<int> byteToChar) {
            if (encoding == EncodingType.UTF8) {
                ConvertBytesToUtf8String(buffer, start, length, out result, out charToByte, out byteToChar);
            } else if (encoding == EncodingType.SJIS) {
                ConvertBytesToSjisString(buffer, start, length, out result, out charToByte, out byteToChar);
            } else if (encoding == EncodingType.EUC) {
                ConvertBytesToEucString(buffer, start, length, out result, out charToByte, out byteToChar);
            } else if (encoding == EncodingType.UNICODE) {
                ConvertBytesToUnicodeString(buffer, start, length, out result, out charToByte, out byteToChar);
            } else {
                ConvertBytesToAsciiString(buffer, start, length, out result, out charToByte, out byteToChar);
            }
        }

        //=========================================================================================
        // 機　能：バイト列の指定範囲をAscii文字列に変換する
        // 引　数：[in]buffer      変換対象のバッファ
        // 　　　　[in]start       変換開始位置
        // 　　　　[in]length      変換された長さ
        // 　　　　[out]result     変換した結果の文字列を返す変数
        // 　　　　[out]charToByte 文字位置をバイト位置に変換する位置情報を返す変数
        // 　　　　[out]byteToChar バイト位置を文字位置に変換する位置情報を返す変数
        // 戻り値：なし
        //=========================================================================================
        public void ConvertBytesToAsciiString(byte[] buffer, int start, int length, out string result, out List<int> charToByte, out List<int> byteToChar) {
            charToByte = new List<int>();
            byteToChar = new List<int>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++) {
                byte ch = buffer[start + i];
                if (ch < 0x20 || ch >= 0x7f) {
                    sb.Append('.');
                } else {
                    sb.Append((char)ch);
                }
                charToByte.Add(i);
                byteToChar.Add(i);
            }
            result = sb.ToString();
        }

        //=========================================================================================
        // 機　能：バイト列の指定範囲をUNICODE文字列に変換する
        // 引　数：[in]buffer      変換対象のバッファ
        // 　　　　[in]start       変換開始位置
        // 　　　　[in]length      変換された長さ
        // 　　　　[out]result     変換した結果の文字列を返す変数
        // 　　　　[out]charToByte 文字位置をバイト位置に変換する位置情報を返す変数
        // 　　　　[out]byteToChar バイト位置を文字位置に変換する位置情報を返す変数
        // 戻り値：なし
        //=========================================================================================
        public void ConvertBytesToUnicodeString(byte[] buffer, int start, int length, out string result, out List<int> charToByte, out List<int> byteToChar) {
            charToByte = new List<int>();
            byteToChar = new List<int>();
            StringBuilder sb = new StringBuilder();
            int index = 0;
            if (start % 2 == 1) {
                sb.Append('.');
                charToByte.Add(index);
                byteToChar.Add(sb.Length);
            }
            for (; index < length - 1; index += 2) {
                int ch = ((int)(buffer[start + index + 1]) << 8) | ((int)(buffer[start + index]));
                if (ch < 0x20) {
                    ch = '.';
                }
                sb.Append((char)ch);
                charToByte.Add(index);
                byteToChar.Add(sb.Length);
                byteToChar.Add(sb.Length);
            }
            if (index == length - 1) {
                sb.Append('.');
                charToByte.Add(start + index);
                byteToChar.Add(sb.Length);
            }
            result = sb.ToString();
        }

        //=========================================================================================
        // 機　能：バイト列の指定範囲をUTF-8文字列に変換する
        // 引　数：[in]buffer      変換対象のバッファ
        // 　　　　[in]start       変換開始位置
        // 　　　　[in]length      変換された長さ
        // 　　　　[out]result     変換した結果の文字列を返す変数
        // 　　　　[out]charToByte 文字位置をバイト位置に変換する位置情報を返す変数
        // 　　　　[out]byteToChar バイト位置を文字位置に変換する位置情報を返す変数
        // 戻り値：なし
        //=========================================================================================
        public void ConvertBytesToUtf8String(byte[] buffer, int start, int length, out string result, out List<int> charToByte, out List<int> byteToChar) {
            charToByte = new List<int>();
            byteToChar = new List<int>();
            StringBuilder sb = new StringBuilder();
            int index = 0;                      // 走査ポインタ（0～length-1）
            int charStartIndex = 0;             // 文字が始まるbufferのインデックス
            int charRemainLength = 0;           // 直前の複数バイト文字の残り文字数
            while (index < length) {
                byte ch = buffer[start + index];
                bool error = false;                             // 異常シーケンスのときtrue
                if (ch < 0x80) {
                    char writeCh;
                    if (ch < 0x20 || ch == 0x7f) {              // バイナリコード
                        writeCh = '.';
                    } else {                                    // ASCII
                        writeCh = (char)ch;
                    }
                    // 直前が複数バイト文字なら、未処理の複数バイト文字分を不正に設定
                    if (charRemainLength > 0) {
                        for (int i = charStartIndex; i < index; i++) {
                            byteToChar.Add(sb.Length);
                            charToByte.Add(i);
                            sb.Append('.');
                        }
                    }
                    // 0x80未満の文字を設定
                    charToByte.Add(index);
                    byteToChar.Add(sb.Length);
                    sb.Append(writeCh);
                    charRemainLength = 0;
                    charStartIndex = index + 1;
                } else if ((ch & 0xc0) == 0x80) {               // UTF8継続文字10xx xxxx
                    if (charRemainLength == 0) {
                        // 残り0バイトなのに継続しようとした
                        error = true;
                    } else if (charRemainLength == 1) {
                        // 残り1バイトの最後の文字
                        charToByte.Add(charStartIndex);
                        for (int i = charStartIndex; i <= index; i++) {
                            byteToChar.Add(sb.Length);
                        }
                        string str = EncodingType.UTF8.Encoding.GetString(buffer, start + charStartIndex, index - charStartIndex + 1);
                        sb.Append(str[0]);
                        charRemainLength = 0;
                        charStartIndex = index + 1;
                    } else {
                        // 複数バイトの中間位置（3バイト文字の2バイト目）
                        charRemainLength--;
                    }
                } else if ((ch & 0xe0) == 0xc0) {               // 2バイト開始文字110x xxxx
                    if (charRemainLength == 0) {
                        charRemainLength = 1;
                        charStartIndex = index;
                    } else {
                        error = true;
                    }
                } else if ((ch & 0xf0) == 0xe0) {               // 3バイト開始文字1110 xxxx
                    if (charRemainLength == 0) {
                        charRemainLength = 2;
                        charStartIndex = index;
                    } else {
                        error = true;
                    }
                } else if ((ch & 0xf8) == 0xf0) {               // 4バイト開始文字1111 0xxx
                    if (charRemainLength == 0) {
                        charRemainLength = 3;
                        charStartIndex = index;
                    } else {
                        error = true;
                    }
                } else if ((ch & 0xfc) == 0xf8) {               // 5バイト開始文字1111 10xx
                    if (charRemainLength == 0) {
                        charRemainLength = 4;
                        charStartIndex = index;
                    } else {
                        error = true;
                    }
                } else if ((ch & 0xfe) == 0xfc) {               // 6バイト開始文字1111 110x
                    if (charRemainLength == 0) {
                        charRemainLength = 5;
                        charStartIndex = index;
                    } else {
                        error = true;
                    }
                } else {
                    error = true;
                }

                // 複数バイト文字のエラーの場合
                if (error) {
                    // 未処理の複数バイト文字を、index番目まで含めて処理
                    for (int i = charStartIndex; i <= index; i++) {
                        byteToChar.Add(sb.Length);
                        charToByte.Add(i);
                        sb.Append('.');
                    }
                    charRemainLength = 0;
                    charStartIndex = index + 1;
                }
                index++;
            }
            // ループ終了後、未処理文字があればそれを不正文字として登録
            if (charRemainLength > 0) {
                for (int i = charStartIndex; i < length; i++) {
                    charToByte.Add(i);
                    byteToChar.Add(sb.Length);
                    sb.Append('.');
                }
            }

            result = sb.ToString();
        }

        //=========================================================================================
        // 機　能：バイト列の指定範囲をSJIS文字列に変換する
        // 引　数：[in]buffer      変換対象のバッファ
        // 　　　　[in]start       変換開始位置
        // 　　　　[in]length      変換された長さ
        // 　　　　[out]result     変換した結果の文字列を返す変数
        // 　　　　[out]charToByte 文字位置をバイト位置に変換する位置情報を返す変数
        // 　　　　[out]byteToChar バイト位置を文字位置に変換する位置情報を返す変数
        // 戻り値：なし
        //=========================================================================================
        public void ConvertBytesToSjisString(byte[] buffer, int start, int length, out string result, out List<int> charToByte, out List<int> byteToChar) {
            charToByte = new List<int>();
            byteToChar = new List<int>();
            StringBuilder sb = new StringBuilder();
            int index = 0;                      // 走査ポインタ（0～length-1）
            int charStartIndex = 0;             // 文字が始まるbufferのインデックス
            int charRemainLength = 0;           // 直前の複数バイト文字の残り文字数
            while (index < length) {
                byte ch = buffer[start + index];
                bool error = false;                             // 異常シーケンスのときtrue
                if (charRemainLength > 0) {
                    if ((0x40 <= ch && ch <= 0x7e) || (0x80 <= ch && ch <= 0xfc)) {     // 下位バイト(0x40～0x7e、0x80～0xfc)
                        // 残り1バイトの最後の文字
                        charToByte.Add(charStartIndex);
                        for (int i = charStartIndex; i <= index; i++) {
                            byteToChar.Add(sb.Length);
                        }
                        string str = EncodingType.SJIS.Encoding.GetString(buffer, start + charStartIndex, index - charStartIndex + 1);
                        sb.Append(str[0]);
                        charRemainLength = 0;
                        charStartIndex = index + 1;
                    } else {
                        error = true;
                    }
                } else if (ch < 0x80) {
                    char writeCh;
                    if (ch < 0x20 || ch == 0x7f) {              // バイナリコード
                        writeCh = '.';
                    } else {                                    // ASCII
                        writeCh = (char)ch;
                    }
                    // 0x80未満の文字を設定
                    charToByte.Add(index);
                    byteToChar.Add(sb.Length);
                    sb.Append(writeCh);
                    charRemainLength = 0;
                    charStartIndex = index + 1;
                } else if ((0x81 <= ch && ch <= 0x9f) || (0xe0 <= ch && ch <= 0xef)) {      // 上位バイト(0x81～0x9f、0xe0～0xef)
                    // 残り0バイトの場合は開始
                    charRemainLength = 1;
                    charStartIndex = index;
                } else if ((0xa1 <= ch) && (ch <= 0xdf)) {
                    // 半角カナを設定
                    charToByte.Add(charStartIndex);
                    for (int i = charStartIndex; i <= index; i++) {
                        byteToChar.Add(sb.Length);
                    }
                    string str = EncodingType.SJIS.Encoding.GetString(buffer, start + index, 1);
                    sb.Append(str[0]);
                    charRemainLength = 0;
                    charStartIndex = index + 1;
                } else {
                    error = true;
                }

                if (error) {
                    // 未処理の複数バイト文字を、index番目まで含めて処理
                    for (int i = charStartIndex; i <= index; i++) {
                        byteToChar.Add(sb.Length);
                        charToByte.Add(i);
                        sb.Append('.');
                    }
                    charRemainLength = 0;
                    charStartIndex = index + 1;
                }
                index++;
            }
            // ループ終了後、未処理文字があればそれを不正文字として登録
            if (charRemainLength > 0) {
                charToByte.Add(charStartIndex);
                byteToChar.Add(sb.Length);
                sb.Append('.');
            }

            result = sb.ToString();
        }

        //=========================================================================================
        // 機　能：バイト列の指定範囲をEUC-JP文字列に変換する
        // 引　数：[in]buffer      変換対象のバッファ
        // 　　　　[in]start       変換開始位置
        // 　　　　[in]length      変換された長さ
        // 　　　　[out]result     変換した結果の文字列を返す変数
        // 　　　　[out]charToByte 文字位置をバイト位置に変換する位置情報を返す変数
        // 　　　　[out]byteToChar バイト位置を文字位置に変換する位置情報を返す変数
        // 戻り値：なし
        //=========================================================================================
        public void ConvertBytesToEucString(byte[] buffer, int start, int length, out string result, out List<int> charToByte, out List<int> byteToChar) {
            charToByte = new List<int>();
            byteToChar = new List<int>();
            StringBuilder sb = new StringBuilder();
            int index = 0;                      // 走査ポインタ（0～length-1）
            int charStartIndex = 0;             // 文字が始まるbufferのインデックス
            int charRemainLength = 0;           // 直前の複数バイト文字の残り文字数
            while (index < length) {
                byte ch = buffer[start + index];
                bool skipStep2 = false;                         // ステップ2の処理をスキップするときtrue
                bool error = false;                             // 異常シーケンスのときtrue
                if (charRemainLength == 1) {
                    if (0xa1 <= ch && ch <= 0xfe) {                 // 第1/第2バイト
                        // 残り1バイトの最後の文字
                        charToByte.Add(charStartIndex);
                        for (int i = charStartIndex; i <= index; i++) {
                            byteToChar.Add(sb.Length);
                        }
                        string str = EncodingType.EUC.Encoding.GetString(buffer, start + charStartIndex, index - charStartIndex + 1);
                        sb.Append(str[0]);
                        charRemainLength = 0;
                        charStartIndex = index + 1;
                        skipStep2 = true;
                    } else {
                        error = true;
                    }
                } else if (charRemainLength == 2) {                 // 第2バイト
                    if (ch >= 0xa1) {
                        charRemainLength--;
                        skipStep2 = true;
                    } else {
                        error = true;
                    }
                }
                if (error) {                                    // エラー
                    // 未処理の複数バイト文字を、index直前まで処理
                    for (int i = charStartIndex; i < index; i++) {
                        byteToChar.Add(sb.Length);
                        charToByte.Add(i);
                        sb.Append('.');
                    }
                    charRemainLength = 0;
                    charStartIndex = index + 1;
                }
                if (skipStep2) {                                // 処理済み
                    ;
                } else if (ch == 0x8f) {                        // 補助漢字 SS3（シングルシフトスリー）
                    charRemainLength = 2;
                    charStartIndex = index;
                } else if (ch == 0x8e || ch >= 0xa1 && ch <= 0xfe) {
                    charRemainLength = 1;
                    charStartIndex = index;
                } else {
                    char writeCh;
                    if (ch >= 0x20 && ch <= 0x7e) {             // ASCII
                        writeCh = (char)ch;
                    } else {                                    // バイナリコード
                        writeCh = '.';
                    }
                    // 0x80未満の文字を設定
                    charToByte.Add(index);
                    byteToChar.Add(sb.Length);
                    sb.Append(writeCh);
                    charRemainLength = 0;
                    charStartIndex = index + 1;
                }
                index++;
            }
            // ループ終了後、未処理文字があればそれを不正文字として登録
            if (charRemainLength > 0) {
                for (int i = charStartIndex; i < length; i++) {
                    charToByte.Add(i);
                    byteToChar.Add(sb.Length);
                    sb.Append('.');
                }
            }

            result = sb.ToString();
        }
    }
}
