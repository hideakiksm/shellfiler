using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：文字列ライブラリ
    //=========================================================================================
    public class StringUtils {

        //=========================================================================================
        // 機　能：指定された文字列を繰り返した文字列を作成する
        // 引　数：[in]str    繰り返しを作成する文字列
        // 　　　　[in]count  繰り返し回数
        // 戻り値：作成済みの文字列
        //=========================================================================================
        public static string Repeat(string str, int count) {
            StringBuilder builder = new StringBuilder(str.Length * count);
            for (int i = 0; i < count; i++) {
                builder.Append(str);
            }
            return builder.ToString();
        }

        //=========================================================================================
        // 機　能：最終行を取得する
        // 引　数：[in]str         取得する文字列
        // 　　　　[in]ignoreEmpty 空行を無視するきとtrue
        // 戻り値：最終行の文字列
        //=========================================================================================
        public static string GetLastLine(string str, bool ignoreEmpty) {
            string[] lineList = str.Split('\x0a');
            string lastLine = null;
            if (ignoreEmpty) {
                lastLine = lineList[0];
                for (int i = lineList.Length - 1; i >= 0; i--) {
                    if (lineList[i].Length == 0 || lineList[i] == "\x0d") {
                        ;
                    } else {
                        lastLine = lineList[i];
                        break;
                    }
                }
            } else {
                lastLine = lineList[lineList.Length - 1];
            }
            if (lastLine.EndsWith("\x0d")) {
                lastLine = lastLine.Substring(0, lastLine.Length - 1);
            }
            return lastLine;
        }

        //=========================================================================================
        // 機　能：空白を元に文字列を分割する
        // 引　数：[in]str   取得する文字列
        // 戻り値：分解した文字列
        //=========================================================================================
        public static string[] SeparateBySpace(string str) {
            List<string> result = new List<string>();
            StringBuilder value = new StringBuilder();
            for (int i = 0; i < str.Length; i++) {
                if (str[i] == ' ') {
                    if (value.Length > 0) {
                        result.Add(value.ToString());
                        value = new StringBuilder();
                    }
                } else {
                    value.Append(str[i]);
                }
            }
            if (value.Length > 0) {
                result.Add(value.ToString());
            }
            return result.ToArray();
        }

        //=========================================================================================
        // 機　能：空白を元に文字列を分割する
        // 引　数：[in]str     取得する文字列
        // 　　　　[in]length  最大長
        // 戻り値：分解した文字列
        //=========================================================================================
        public static string[] SeparateBySpace(string str, int length) {
            List<string> result = new List<string>();
            StringBuilder value = new StringBuilder();
            int i = 0;
            while (i < str.Length) {
                if (str[i] == ' ' && result.Count < length - 1) {
                    if (value.Length > 0) {
                        result.Add(value.ToString());
                        value = new StringBuilder();
                    }
                    while (str[i] == ' ' && i < str.Length) {
                        i++;
                    }
                } else {
                    value.Append(str[i]);
                    i++;
                }
            }
            if (value.Length > 0) {
                result.Add(value.ToString());
            }
            return result.ToArray();
        }

        //=========================================================================================
        // 機　能：文字列を行単位に分割する
        // 引　数：[in]str   取得する文字列
        // 戻り値：分解した文字列
        //=========================================================================================
        public static string[] SeparateLine(string str) {
            string[] lines = str.Split('\n');
            string[] result = new string[lines.Length];
            for (int i = 0; i < lines.Length; i++) {
                result[i] = lines[i].TrimEnd('\r');
            }
            return result;
        }

        //=========================================================================================
        // 機　能：行単位の文字列の配列を１つにまとめる
        // 引　数：[in]strArray   行単位の文字列の配列
        // 　　　　[in]cr         改行コード
        // 戻り値：まとめた結果
        //=========================================================================================
        public static string ConbineLine(string[] strArray, string cr) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strArray.Length; i++) {
                if (i != 0) {
                    sb.Append(cr);
                }
                sb.Append(strArray[i]);
            }
            return sb.ToString();
        }
        
        //=========================================================================================
        // 機　能：CR LFをLFに変換する
        // 引　数：[in]src   元の文字列
        // 戻り値：変換後の文字列
        //=========================================================================================
        public static string ConvertCrLfToLf(string src) {
            char CR = '\r';
            char LF = '\n';
            StringBuilder sb = new StringBuilder();
            int length = src.Length;
            for (int i = 0; i < length; i++) {
                bool addRet = false;
                if (src[i] == CR) {
                    addRet = true;
                    if (i + 1 < length && src[i + 1] == LF) {
                        i++;
                    }
                } else if (src[i] == LF) {
                    addRet = true;
                }
                if (addRet) {
                    sb.Append(LF);
                } else {
                    sb.Append(src[i]);
                }
            }
            return sb.ToString(); 
        }

        //=========================================================================================
        // 機　能：バッファの内容を文字列に変換する
        // 引　数：[in]encoding      エンコード方式
        // 　　　　[in]buffer        受信したデータのバッファ
        // 　　　　[in,out]offset    受信したデータの開始オフセット（未処理データのオフセットを返す）
        // 　　　　[in,out]length    受信したデータの長さ（未処理データの長さを返す）
        // 戻り値：変換した文字列
        //=========================================================================================
        public static string ConvertUncheckedByteToString(Encoding encoding, byte[] buffer, ref int offset, ref int length) {
            const char ILLEGAL_CHAR = (char)0xfffd;
            StringBuilder str = new StringBuilder();
            while (length > 0) {
                try {
                    string strOnce = encoding.GetString(buffer, offset, length);
                    str.Append(strOnce);
                    offset += length;
                    length = 0;
                } catch (DecoderFallbackException e) {
                    // 不明な文字
                    int errorIndex = e.Index + e.BytesUnknown.Length;       // offsetを0としたエラー位置
                    int errorLength = e.BytesUnknown.Length;
                    char errorRecoverChar = ILLEGAL_CHAR;
                    if (e.BytesUnknown[0] == 0x1b) {
                        // ESC
                        errorLength = 1;
                        errorRecoverChar = (char)0x1b;
                    } else if (errorIndex + e.BytesUnknown.Length == length) {
                        // 最終位置が変換不可：ここでwhileを抜けて終わり
                        break;
                    }
                    if (errorIndex != 0) {
                        // エラー発生位置までを通常通り変換
                        string strRecover = encoding.GetString(buffer, offset, errorIndex);
                        str.Append(strRecover);
                        offset += errorIndex;
                        length -= errorIndex;
                    }
                    // エラー発生位置の文字を変換
                    str.Append(errorRecoverChar);
                    offset += errorLength;
                    length -= errorLength;
                }
            }
            return str.ToString();
        }

        //=========================================================================================
        // 機　能：文字列の最後に含まれる改行をすべて削除する
        // 引　数：[in]str  対象文字列
        // 戻り値：改行を削除した文字列
        //=========================================================================================
        public static string RemoveLastLineBreak(string str) {
            return str.TrimEnd('\r', '\n');
        }
        
        //=========================================================================================
        // 機　能：文字列から引用符を外す
        // 引　数：[in]str  対象文字列
        // 戻り値：引用符を外した文字列
        //=========================================================================================
        public static string RemoveStringQuote(string str) {
            if (str.StartsWith("\"") && str.EndsWith("\"")) {
                return str.Substring(1, str.Length - 2);
            } else if (str.StartsWith("'") && str.EndsWith("'")) {
                return str.Substring(1, str.Length - 2);
            } else {
                return str;
            }
        }
        
        //=========================================================================================
        // 機　能：文字列に引用符をつける
        // 引　数：[in]str    対象文字列
        // 　　　　[in]quote  引用符の文字
        // 戻り値：引用符をつけた文字列
        //=========================================================================================
        public static string AddStringQuote(string str, string quote) {
            if (str.StartsWith("\"") && str.EndsWith("\"")) {
                return str;
            } else if (str.StartsWith("'") && str.EndsWith("'")) {
                return str;
            } else {
                return quote + str + quote;
            }
        }

        //=========================================================================================
        // 機　能：省略文字列を作成する
        // 引　数：[in]str    対象文字列
        // 　　　　[in]length 最大の長さ
        // 戻り値：省略文字列
        //=========================================================================================
        public static string MakeOmittedString(string str, int length) {
            if (str.Length <= length || str.Length <= 4) {
                return str;
            } else {
                return str.Substring(0, length - 3) + "...";
            }
        }

        //=========================================================================================
        // 機　能：文字列の最後に含まれる改行を1個分削除する
        // 引　数：[in]str  対象文字列
        // 戻り値：改行を削除した文字列
        //=========================================================================================
        public static string RemoveSingleLastLineBreak(string str) {
            if (str.EndsWith("\r\n")) {
                return str.Substring(0, str.Length - 2);
            } else if (str.EndsWith("\n")) {
                return str.Substring(0, str.Length - 1);
            } else {
                return str;
            }
        }

        //=========================================================================================
        // 機　能：文字列を指定のセパレータを加えて連結する
        // 引　数：[in]list       連結対象の文字列
        // 　　　　[in]separator  連結に使用するセパレータ
        // 戻り値：連結した文字列
        //=========================================================================================
        public static string CombineStringArray(List<string> list, string separator) {
            return CombineStringArray(list.ToArray(), 0, list.Count, separator);
        }

        //=========================================================================================
        // 機　能：文字列を指定のセパレータを加えて連結する
        // 引　数：[in]list       連結対象の文字列
        // 　　　　[in]separator  連結に使用するセパレータ
        // 戻り値：連結した文字列
        //=========================================================================================
        public static string CombineStringArray(string[] list, string separator) {
            return CombineStringArray(list, 0, list.Length, separator);
        }

        //=========================================================================================
        // 機　能：文字列を指定のセパレータを加えて連結する
        // 引　数：[in]list       連結対象の文字列
        // 　　　　[in]start      配列中の開始インデックス
        // 　　　　[in]count      配列中の連結する数
        // 　　　　[in]separator  連結に使用するセパレータ
        // 戻り値：連結した文字列
        //=========================================================================================
        public static string CombineStringArray(string[] list, int start, int count, string separator) {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < count; i++) {
                string str = list[start + i];
                if (i != 0) {
                    result.Append(separator);
                }
                result.Append(str);
            }
            return result.ToString();
        }

        //=========================================================================================
        // 機　能：文字列の英字部分の先頭のみを大文字にする（ABC123def.gHI→Abc123Def.Ghi）
        // 引　数：[in]str  変換対象の文字列
        // 戻り値：変換した文字列
        //=========================================================================================
        public static string CapitalString(String str) {
            char[] strChar = str.ToCharArray();
            bool firstChar = true;
            for (int i = 0; i < strChar.Length; i++) {
                char ch = strChar[i];
                if ('A' <= ch && ch <= 'Z') {
                    if (!firstChar) {
                        strChar[i] = (char)('a' + ch - 'A');
                    }
                    firstChar = false;
                } else if ('a' <= ch && ch <= 'z') {
                    if (firstChar) {
                        strChar[i] = (char)('A' + ch - 'a');
                    }
                    firstChar = false;
                } else {
                    firstChar = true;
                }
            }
            return new string(strChar);
        }

        //=========================================================================================
        // 機　能：ファイルサイズを文字列に変換する
        // 引　数：[in]size  ファイルサイズ
        // 戻り値：変換後のファイルサイズ
        //=========================================================================================
        public static string FileSizeToString(long size) {
            const long TERA = 1024L * 1024L * 1024L * 1024L;
            const long GIGA = 1024L * 1024L * 1024L;
            const long MEGA = 1024L * 1024L;
            const long KIRO = 1024L;
            String strSize;
            if (size > TERA) {
                strSize = String.Format("{0}.{1:000}T", size / TERA, ((size / GIGA) % 1024) * 1000 / 1024);
            } else if (size > GIGA) {
                strSize = String.Format("{0}.{1:000}G", size / GIGA, ((size / MEGA) % 1024) * 1000 / 1024);
            } else if (size > MEGA) {
                strSize = String.Format("{0}.{1:000}M", size / MEGA, ((size / KIRO) % 1024) * 1000 / 1024);
            } else {
                strSize = String.Format("{0}", size);
            }
            return strSize;
        }

        //=========================================================================================
        // 機　能：文字列中の指定文字の開始位置をすべて返す
        // 引　数：[in]str   検索される文字列
        // 　　　　[in]key   キーとなる文字列
        // 戻り値：ヒットした位置の配列
        //=========================================================================================
        public static int[] SearchAllPosition(string str, string key) {
            List<int> result = new List<int>();
            int startPos = 0;
            while (startPos < str.Length) {
                int hitPos = str.IndexOf(key, startPos);
                if (hitPos == -1) {
                    break;
                }
                result.Add(hitPos);
                startPos = hitPos + key.Length;
            }
            return result.ToArray();
        }

        //=========================================================================================
        // 機　能：文字配列から指定された文字列と同じ要素のインデックスを返す
        // 引　数：[in]array  文字配列
        // 　　　　[in]key    キーとなる文字列
        // 戻り値：ヒットした要素のインデックス（ヒットしないとき-1）
        //=========================================================================================
        public static int SearchElementIndex(string[] array, string key) {
            for (int i = 0; i < array.Length; i++) {
                if (array[i] == key) {
                    return i;
                }
            }
            return -1;
        }

        //=========================================================================================
        // 機　能：文字列中の指定範囲の文字がはじめに見つかった位置を返す
        // 引　数：[in]str     検索される文字列
        // 　　　　[in]chStart 開始文字
        // 　　　　[in]chEnd   終了文字
        // 戻り値：はじめに見つかった位置（-1:見つからない）
        //=========================================================================================
        public static int IndexOfRangeIgnoreCase(string str, char chStart, char chEnd) {
            if ('A' <= chStart && chStart <= 'Z') {
                chStart = (char)(chStart - 'A' + 'a');
            }
            if ('A' <= chEnd && chEnd <= 'Z') {
                chEnd = (char)(chEnd - 'A' + 'a');
            }
            str = str.ToLower();
            for (int i = 0; i < str.Length; i++) {
                char ch = str[i];
                if (chStart <= ch && ch <= chEnd) {
                    return i;
                }
            }
            return -1;
        }

        //=========================================================================================
        // 機　能：配列を空白区切りで連結する
        // 引　数：[in]optionList  文字列（省略時はnullまたは""）
        // 戻り値：空白区切りで連結した文字列
        //=========================================================================================
        public static string AppendStringOption(params string[] optionList) {
            StringBuilder sb = new StringBuilder();
            foreach (string option in optionList) {
                if (option != null && option != "") {
                    if (sb.Length > 0) {
                        sb.Append(' ');
                    }
                    sb.Append(option);
                }
            }
            return sb.ToString();
        }

        //=========================================================================================
        // 機　能：配列を空白区切りで連結する
        // 引　数：[in]optionList  文字列（省略時はnullまたは""）
        // 戻り値：空白区切りで連結した文字列
        //=========================================================================================
        public static string AppendString(string[] optionList, int startIndex, int length) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++) {
                if (i != 0) {
                    sb.Append(' ');
                }
                sb.Append(optionList[startIndex + i]);
            }
            return sb.ToString();
        }

        //=========================================================================================
        // 機　能：項目一覧のうち、指定された項目のインデックスを返す
        // 引　数：[in]itemList   項目一覧
        // 　　　　[in]key        探す項目
        // 戻り値：keyが見つかった位置のインデックス（見つからないとき0）
        // メ　モ：itemList={"aaa","bbb","ccc"}、key="bbb"のとき、戻り値=1
        //=========================================================================================
        public static int GetStringArrayIndex(string[] itemList, string key) {
            for (int i = 0; i < itemList.Length; i++) {
                if (itemList[i] == key) {
                    return i;
                }
            }
            return 0;
        }

        //=========================================================================================
        // 機　能：文字列中に指定された文字がいくつあるかをカウントする
        // 引　数：[in]str  文字列
        // 　　　　[in]ch   調べる文字
        // 戻り値：chが存在した個数
        //=========================================================================================
        public static int GetCharCount(string str, char ch) {
            int count = 0;
            for (int i = 0; i < str.Length; i++) {
                if (str[i] == ch) {
                    count++;
                }
            }
            return count;
        }

        //=========================================================================================
        // 機　能：文字列中の英字部分を先頭大文字に変換する
        // 引　数：[in]str  文字列（null可）
        // 戻り値：変換後の文字列（入力がnullのときnull）
        //=========================================================================================
        public static string ToCapital(string org) {
            if (org == null) {
                return null;
            }
            bool toUpper = true;
            char[] chOrg = org.ToCharArray();
            for (int i = 0; i < chOrg.Length; i++) {
                char ch = chOrg[i];
                if ('A' <= ch && ch <= 'Z') {
                    if (toUpper) {
                        toUpper = false;
                    } else {
                        chOrg[i] = (char)(ch - 'A' + 'a');
                    }
                } else if ('a' <= ch && ch <= 'z') {
                    if (toUpper) {
                        chOrg[i] = (char)(ch - 'a' + 'A');
                        toUpper = false;
                    } else {
                        ;
                    }
                } else {
                    toUpper = true;
                }
            }
            return new string(chOrg);
        }

        //=========================================================================================
        // 機　能：HTMLパラメータを分解する
        // 引　数：[in]param     分解前のHTMLパラメータ（key=value&key=value形式）
        // 　　　　[in]encoding  文字列のエンコード方式
        // 戻り値：分解したパラメータ
        //=========================================================================================
        public static  List<KeyValuePair<string, string>> SplitHtmlParameter(string param, Encoding encoding) {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            string[] paramValueList = param.Split('&');
            foreach (string paramValue in paramValueList) {
                string[] paramAndValue = paramValue.Split(new char[] {'='}, 2);
                string encodedValue;
                if (paramAndValue.Length == 1) {
                    encodedValue = "";
                } else {
                    try {
                        encodedValue = HttpUtility.UrlDecode(paramAndValue[1], encoding);
                    } catch (Exception) {
                        return null;
                    }
                }
                result.Add(new KeyValuePair<string, string>(paramAndValue[0], encodedValue));
            }
            return result;
        }

        //=========================================================================================
        // 機　能：HTMLパラメータを結合する
        // 引　数：[in]paramList  結合前のHTMLパラメータ
        // 　　　　[in]encoding   文字列のエンコード方式
        // 戻り値：結合したパラメータ（key=value&key=value形式）
        //=========================================================================================
        public static string AppendHtmlParameter(List<KeyValuePair<string, string>> paramList, Encoding encoding) {
            StringBuilder result = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValue in paramList) {
                if (result.Length > 0) {
                    result.Append('&');
                }
                string encoded = HttpUtility.UrlEncode(keyValue.Value, encoding);
                result.Append(keyValue.Key);
                result.Append('=');
                result.Append(encoded);
            }
            return result.ToString();
        }

        //=========================================================================================
        // 機　能：ワイルドカード文字列を正規表現エンジンに変換して返す
        // 引　数：[in]wildcard  ワイルドカード文字列（大文字小文字の区別あり）
        // 戻り値：正規表現オブジェクト
        //=========================================================================================
        public static Regex ConvertWildcardToRegex(string wildcard) {
            string rPattern = Regex.Replace(wildcard, ".", new MatchEvaluator(WildCardMatchEvaluator)); 
            return new Regex(rPattern);
        }

        //=========================================================================================
        // 機　能：ワイルドカード文字列を正規表現に変換して返す
        // 引　数：[in]wildcard  ワイルドカード文字列（大文字小文字の区別あり）
        // 戻り値：正規表現文字列
        //=========================================================================================
        public static string ConvertWildcardToRegexString(string wildcard) {
            string rPattern = Regex.Replace(wildcard, ".", new MatchEvaluator(WildCardMatchEvaluator)); 
            return rPattern;
        }

        //=========================================================================================
        // 機　能：ワイルドカード用に正規表現のマッチ条件を評価する
        // 引　数：[in]m  マッチ条件
        // 戻り値：新しい条件
        //=========================================================================================
        private static string WildCardMatchEvaluator(Match m) {
            string s = m.Value;
            if (s.Equals("?")) {
                return ".";
            } else if (s.Equals("*")) {
                return ".*";
            } else {
                return Regex.Escape(s);
            }
        }

        //=========================================================================================
        // 機　能：指定されたバイト列からキーワードとなるバイト列の先頭位置を返す
        // 引　数：[in]data     検索対象のバイト列
        // 　　　　[in]keyword  キーワードとなるバイト列
        // 　　　　[in]start    検索開始のインデックス
        // 戻り値：検索にヒットした位置（-1:ヒットしない）
        //=========================================================================================
        public static int FindBytes(byte[] data, byte[] keyword, int start) {
            int end = data.Length - keyword.Length + 1;
            for (int i = start; i < end; i++) {
                bool hit = true;
                for (int j = 0; j < keyword.Length; j++) {
                    if (data[i + j] != keyword[j]) {
                        hit = false;
                        break;
                    }
                }
                if (hit) {
                    return i;
                }
            }
            return -1;
        }

        //=========================================================================================
        // 機　能：文字列中の特定の文字の数を返す
        // 引　数：[in]str   文字列
        // 　　　　[in]ch    数える文字
        // 戻り値：文字列中の文字の数
        //=========================================================================================
        public static int CountCharOf(string str, char ch) {
            int count = 0;
            int length = str.Length;
            for (int i = 0; i < length; i++) {
                if (str[i] == ch) {
                    count++;
                }
            }
            return count;
        }
    }
}
