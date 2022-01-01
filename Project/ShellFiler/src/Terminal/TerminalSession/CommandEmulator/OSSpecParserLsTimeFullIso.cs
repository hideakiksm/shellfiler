using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.OSSpec;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {

    //=========================================================================================
    // クラス：トークンの解析を行う:full-iso形式での時刻 YYYY-MM-DD HH:MM:SS.XXXXXXXXX +9999
    //=========================================================================================
    public class OSSpecParserLsTimeFullIso : IOSSpecTokenParser {

        //=========================================================================================
        // 機　能：構文解析を行う
        // 引　数：[in]line          コマンドの実行結果の1行分
        // 　　　　[in]expect        期待値の設定
        // 　　　　[in,out]parsePos  解析開始位置（次の解析位置を返す）
        // 　　　　[out]value        解析の結果取得した値
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        public bool ParseToken(string line, OSSpecColumnExpect expect, ref int parsePos, out object value) {
            value = DateTime.MinValue;
            bool success;
            // 2012-10-01 12:34:56.123456789 +0900

            // 年月日
            int dateYYYY, dateMM, dateDD;
            success = ParseNumber(line, 4, ref parsePos, out dateYYYY);
            if (!success) {
                return false;
            }
            success = SkipChar(line, ref parsePos, '-');
            if (!success) {
                return false;
            }
            success = ParseNumber(line, 2, ref parsePos, out dateMM);
            if (!success) {
                return false;
            }
            success = SkipChar(line, ref parsePos, '-');
            if (!success) {
                return false;
            }
            success = ParseNumber(line, 2, ref parsePos, out dateDD);
            if (!success) {
                return false;
            }
            success = SkipChar(line, ref parsePos, ' ');
            if (!success) {
                return false;
            }
            if (1600 <= dateYYYY && dateYYYY < 3000 && 1 <= dateMM && dateMM <= 12 && 1 <= dateDD && dateDD <= 31) {
                ;
            } else {
                return false;
            }

            // 時分秒.ナノ秒
            int timeHH, timeMM, timeSS, timeNano;
            success = ParseNumber(line, 2, ref parsePos, out timeHH);
            if (!success) {
                return false;
            }
            success = SkipChar(line, ref parsePos, ':');
            if (!success) {
                return false;
            }
            success = ParseNumber(line, 2, ref parsePos, out timeMM);
            if (!success) {
                return false;
            }
            success = SkipChar(line, ref parsePos, ':');
            if (!success) {
                return false;
            }
            success = ParseNumber(line, 2, ref parsePos, out timeSS);
            if (!success) {
                return false;
            }
            success = SkipChar(line, ref parsePos, '.');
            if (!success) {
                return false;
            }
            success = ParseNumber(line, 9, ref parsePos, out timeNano);
            if (!success) {
                return false;
            }
            if (0 <= timeHH && timeHH < 24 && 0 <= timeMM && timeMM <= 59 && 0 <= timeSS && timeSS <= 60) {
                ;
            } else {
                return false;
            }
            success = SkipChar(line, ref parsePos, ' ');
            if (!success) {
                return false;
            }

            // タイムゾーン
            if (parsePos >= line.Length) {
                return false;
            }
            if (line[parsePos] == '+' || line[parsePos] == '-') {
                parsePos++;
            } else {
                return false;
            }
            int timeZone;
            success = ParseNumber(line, 4, ref parsePos, out timeZone);
            if (!success) {
                return false;
            }

            value = new DateTime(dateYYYY, dateMM, dateDD, timeHH, timeMM, timeSS, timeNano / 1000000, DateTimeKind.Local);
            return true;
        }

        //=========================================================================================
        // 機　能：指定桁数の数値を解析する
        // 引　数：[in]line          コマンドの実行結果の1行分
        // 　　　　[in]digit         期待する桁数
        // 　　　　[in,out]parsePos  解析開始位置（次の解析位置を返す）
        // 　　　　[out]number       解析の結果取得した値
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool ParseNumber(string line, int digit, ref int parsePos, out int number) {
            number = 0;
            for (int i = 0; i < digit; i++) {
                if (parsePos >= line.Length) {
                    return false;
                }
                char ch = line[parsePos];
                if ('0' <= ch && ch <= '9') {
                    number = number * 10 + (ch - '0');
                    parsePos++;
                } else {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：指定文字をスキップする
        // 引　数：[in]line          コマンドの実行結果の1行分
        // 　　　　[in,out]parsePos  解析開始位置（次の解析位置を返す）
        // 　　　　[in]ch            スキップする文字の期待
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool SkipChar(string line, ref int parsePos, char ch) {
            if (parsePos >= line.Length) {
                return false;
            }
            if (line[parsePos] == ch) {
                parsePos++;
                return true;
            } else {
                return false;
            }
        }
    }
}
