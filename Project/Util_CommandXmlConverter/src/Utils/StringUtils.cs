using System;
using System.Collections.Generic;
using System.Text;

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
        // 引　数：[in]str         取得する文字列
        // 　　　　[in]ignoreEmpty 空行を無視するきとtrue
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
        // 機　能：文字列の最後に含まれる改行をすべて削除する
        // 引　数：[in]str  対象文字列
        // 戻り値：改行を削除した文字列
        //=========================================================================================
        public static string RemoveLastLineBreak(string str) {
            return str.TrimEnd('\r', '\n');
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
            StringBuilder result = new StringBuilder();
            bool first = true;
            foreach (string str in list) {
                result.Append(str);
                if (first) {
                    first = false;
                } else {
                    result.Append(separator);
                }
            }
            return result.ToString();
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
                strSize = String.Format("{0}.{1,000}T", size / TERA, ((size / GIGA) % 1024) * 1000 / 1024);
            } else if (size > GIGA) {
                strSize = String.Format("{0}.{1,000}G", size / GIGA, ((size / MEGA) % 1024) * 1000 / 1024);
            } else if (size > MEGA) {
                strSize = String.Format("{0}.{1,000}M", size / MEGA, ((size / KIRO) % 1024) * 1000 / 1024);
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
        // 機　能：オプションの文字列を空白区切りで連結する
        // 引　数：[in]optionList  オプションの文字列（省略時はnullまたは""）
        // 戻り値：オプションを空白区切りで連結した文字列
        //=========================================================================================
        public static string AppendOption(params string[] optionList) {
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
    }
}
