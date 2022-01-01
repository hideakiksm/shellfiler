using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.UI.Log;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;

namespace ShellFiler.FileTask.Condition {

    //=========================================================================================
    // クラス：ファイル条件用にワイルドカードを変換するクラス
    //=========================================================================================
    public class WildCardConverter {

        //=========================================================================================
        // 機　能：ワイルドカード文字列を正規表現文字列に変換する
        // 引　数：[in]wildCard  ワイルドカード文字列
        // 戻り値：正規表現文字列（変換できなかったときnull）
        //=========================================================================================
        public static string ConvertWildCardToRegexString(string wildCard) {
            StringBuilder result = new StringBuilder();
            result.Append("^(");
            string[] wildCardList = wildCard.Split(':');
            for (int i = 0; i < wildCardList.Length; i++) {
                string wildCardItem = wildCardList[i];
                if (wildCardItem == "") {
                    continue;
                }
                if (wildCardItem.IndexOf('*') == -1 && wildCardItem.IndexOf('?') == -1) {
                    return null;
                }
                if (i != 0) {
                    result.Append("|");
                }
                result.Append(StringUtils.ConvertWildcardToRegexString(wildCardItem));
            }
            result.Append(")$");
            return result.ToString();
        }

        //=========================================================================================
        // 機　能：正規表現文字列をワイルドカード文字列に変換する
        // 引　数：[in]regex   正規表現文字列
        // 戻り値：ワイルドカード文字列（変換できなかったときnull）
        //=========================================================================================
        public static string ConvertRegexStringToWildCard(string wildCard) {
            if (wildCard.StartsWith("^(") && wildCard.EndsWith(")$")) {
                ;
            } else {
                return null;
            }
            string escapeCharList = @"\*+?{[()^$# ";
            string wildCardPart = wildCard.Substring(2, wildCard.Length - 4);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < wildCardPart.Length; i++) {
                char ch = wildCardPart[i];
                if (ch == '\\') {
                    if (i < wildCardPart.Length - 1) {
                        sb.Append(wildCardPart[i + 1]);
                        i++;
                    } else {
                        return null;
                    }
                } else if (ch == '|') {
                    sb.Append(':');
                } else if (ch == '.') {
                    if (i < wildCardPart.Length - 1) {
                        if (wildCardPart[i + 1] == '*') {
                            sb.Append('*');
                            i++;
                        } else {
                            sb.Append('?');
                        }
                    } else {
                        sb.Append('?');
                    }
                } else if (escapeCharList.IndexOf(ch) != -1) {
                    return null;
                } else {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }
    }
}
