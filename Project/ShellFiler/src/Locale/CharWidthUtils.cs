using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using ShellFiler.Util;

namespace ShellFiler.Locale {

    //=========================================================================================
    // クラス：文字の幅（半角/全角）についてのユーティリティ
    //=========================================================================================
    class CharWidthUtils {

        //=========================================================================================
        // 機　能：半角での文字列長を返す
        // 引　数：[in]g            フォントサイズを計測するためのグラフィックス（未初期化の文字がないときnull可）
        // 　　　　[in]str          計測対象の文字列
        // 　　　　[in]unknownWidth 不明文字の幅
        // 戻り値：半角での文字列長
        //=========================================================================================
        public static int StringHalfCharLength(Graphics g, string str, int unknownWidth) {
            CharWidth charWidth = new CharWidth();
            int width = 0;
            for (int i = 0; i < str.Length; i++) {
                CharWidth.CharType type = charWidth.GetCharType(g, str[i]);
                if (type == CharWidth.CharType.FullWidth) {
                    width += 2;
                } else if (type == CharWidth.CharType.HalfWidth) {
                    width += 1;
                } else {
                    width += unknownWidth;
                }
            }
            return width;
        }

        //=========================================================================================
        // 機　能：半角での文字列長により部分文字列を返す
        // 引　数：[in]g            フォントサイズを計測するためのグラフィックス（未初期化の文字がないときnull可）
        // 　　　　[in]str          計測対象の文字列
        // 　　　　[in]unknownWidth 不明文字の幅（1または2）
        // 　　　　[in]startWidth   開始位置の半角ベース文字幅
        // 　　　　[in]lengthWidth  取得文字列長の半角ベースの文字幅
        // 　　　　[in]padding1     開始位置が全角で分割された場合に使用するパディング
        // 　　　　[in]padding2     終了位置が全角で分割された場合に使用するパディング
        // 戻り値：半角での部分文字列取得結果
        //=========================================================================================
        public static string SubstringHalfChar(Graphics g, string str, int unknownWidth, int startWidth, int lengthWidth, char padding1, char padding2) {
            if (lengthWidth == 0) {
                return "";
            }
            CharWidth charWidth = new CharWidth();
            int substrStart = -1;                   // 開始位置（文字列上のインデックス）
            int substrLength = -1;                  // 切り出す長さ（文字列上のインデックス）
            bool usePadding1 = false;               // padding1を使用するときtrue
            bool usePadding2 = false;               // padding2を使用するときtrue

            int width = 0;
            for (int i = 0; i < str.Length; i++) {
                // 文字幅を計測
                int prevWidth = width;
                CharWidth.CharType type = charWidth.GetCharType(g, str[i]);
                if (type == CharWidth.CharType.FullWidth) {
                    width += 2;
                } else if (type == CharWidth.CharType.HalfWidth) {
                    width += 1;
                } else {
                    width += unknownWidth;
                }

                // 開始位置
                if (substrStart == -1) {
                    if (width > startWidth) {
                        substrStart = i;
                        if (prevWidth != startWidth) {
                            usePadding1 = true;
                            substrStart++;
                        }
                    }
                }

                // 終了位置
                if (substrLength == -1) {
                    if (width >= startWidth + lengthWidth) {
                        substrLength = i - substrStart + 1;
                        if (width != startWidth + lengthWidth) {
                            usePadding2 = true;
                            substrLength--;
                        }
                        break;
                    }
                }
            }

            // 結果を作成
            string result;
            if (substrStart == -1) {
                // 開始位置がはみ出ている
                result = "";
            } else if (substrLength == -1) {
                // 終了位置がはみ出ている
                if (usePadding1) {
                    result = padding1 + str.Substring(substrStart);
                } else {
                    result = str.Substring(substrStart);
                }
            } else {
                // 通常
                if (usePadding1) {
                    result = new string(padding1, 1);
                } else {
                    result = "";
                }
                result += str.Substring(substrStart, substrLength);
                if (usePadding2) {
                    result += padding2;
                }
            }
            return result;
        }
    }
}
