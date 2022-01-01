using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Util;

namespace ShellFiler.Document.OSSpec {

    //=========================================================================================
    // インターフェイス：トークンの構文解析クラス
    //=========================================================================================
    public interface IOSSpecTokenParser {

        //=========================================================================================
        // 機　能：構文解析を行う
        // 引　数：[in]line          コマンドの実行結果の1行分
        // 　　　　[in]expect        期待値の設定
        // 　　　　[in,out]parsePos  解析開始位置（次の解析位置を返す）
        // 　　　　[out]value        解析の結果取得した値
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        bool ParseToken(string line, OSSpecColumnExpect expect, ref int parsePos, out object value);
    }
}
