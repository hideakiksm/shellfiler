using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.OSSpec;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {

    //=========================================================================================
    // クラス：トークンの解析を行う:行末までの文字列の並び
    //=========================================================================================
    public class OSSpecParserStringAll : IOSSpecTokenParser {

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
            if (parsePos >= line.Length) {
                return false;
            }
            value = line.Substring(parsePos);
            parsePos = line.Length;
            return true;
        }
    }
}
