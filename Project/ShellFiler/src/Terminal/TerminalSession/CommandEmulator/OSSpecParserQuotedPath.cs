using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.OSSpec;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {

    //=========================================================================================
    // クラス：トークンの解析を行う:数値0～9の並び（戻り=int:値）
    //=========================================================================================
    public class OSSpecParserQuotedPath : IOSSpecTokenParser {

        //=========================================================================================
        // 機　能：構文解析を行う
        // 引　数：[in]line          コマンドの実行結果の1行分
        // 　　　　[in]expect        期待値の設定
        // 　　　　[in,out]parsePos  解析開始位置（次の解析位置を返す）
        // 　　　　[out]value        解析の結果取得した値
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        public bool ParseToken(string line, OSSpecColumnExpect expect, ref int parsePos, out object value) {
            value = "";
            int startPos = parsePos;
            int length = line.Length;
            if (parsePos >= length) {
                return false;
            }

            // "～"を読み込む
            if (!IsQuoteChar(line[parsePos])) {
                return false;
            }
            parsePos++;
            while (!IsQuoteChar(line[parsePos])) {
                if (parsePos >= length) {
                    return false;
                }
                parsePos++;
            }
            parsePos++;

            if (parsePos - startPos <= 2) {
                return false;
            }
            value = line.Substring(startPos + 1, parsePos - startPos - 2);

            // ディレクトリが「"."/」のようになっていることがある
            if (parsePos < line.Length && line[parsePos] == '/') {
                parsePos++;
                value += "/";
            } else if (parsePos < line.Length && line[parsePos] == '*') {
                parsePos++;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：引用符の文字かどうかを返す
        // 引　数：[in]ch   調べる文字
        // 戻り値：引用符の文字であるときtrue
        //=========================================================================================
        private bool IsQuoteChar(char ch) {
            if (ch == '\"' || ch == '`' || ch == '\'') {
                return true;
            } else {
                return false;
            }
        }
    }
}
