using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using ShellFiler.Util;

namespace ShellFiler.Locale {

    //=========================================================================================
    // クラス：文字種別を判断するクラス
    //=========================================================================================
    class StringCategory {
        // 全角記号のリスト
        private const string FULL_SYMBOL = "、。，．・：；？！゛゜´｀¨＾￣＿〇―‐／＼～∥｜…‥‘’“”（）〔〕［］｛｝〈〉《》" +
                                           "「」『』【】＋－±×÷＝≠＜＞≦≧∞∴♂♀°′″℃￥＄￠￡％＃＆＊＠§☆★○●◎◇◆□" +
                                           "■△▲▽▼※〒→←↑↓〓∈∋⊆⊇⊂⊃∪∩∧∨￢⇒⇔∀∃∠⊥⌒∂∇≡≒≪≫√∽∝∵∫∬Å" +
                                           "‰♯♭♪†‡¶◯─│┌┐┘└├┬┤┴┼━┃┏┓┛┗┣┳┫┻╋┠┯┨┷┿┝┰┥┸╂①②③" +
                                           "④⑤⑥⑦⑧⑨⑩⑪⑫⑬⑭⑮⑯⑰⑱⑲⑳ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩ㍉㌔㌢㍍㌘㌧㌃㌶㍑㍗㌍㌦㌣㌫㍊㌻" +
                                           "㎜㎝㎞㎎㎏㏄㎡㍻〝〟№㏍℡㊤㊥㊦㊧㊨㈱㈲㈹㍾㍽㍼≒≡∫∮∑√⊥∠∟⊿∵∩∪";

        //=========================================================================================
        // 機　能：大文字を小文字にする
        // 引　数：[in]str  元の文字列
        // 戻り値：小文字にした文字列
        // メ　モ：文字列長を保証したいとき、カルチャーの影響を避けたいときに使用する
        //=========================================================================================
        public static string ToLower(string str) {
            StringBuilder result = new StringBuilder(str.Length);
            int length = str.Length;
            for (int i = 0; i < length; i++) {
                char ch = str[i];
                if ('A' <= ch && ch <= 'Z') {
                    result.Append((char)(ch - 'A' + 'a'));
                } else {
                    result.Append(ch);
                }
            }
            return result.ToString();
        }

        //=========================================================================================
        // 機　能：ファイルビューアでの文字種別を返す
        // 引　数：[in]ch  調べる文字
        // 戻り値：文字種別
        //=========================================================================================
        public static ViewerCharType GetViewerCharType(char ch) {
            if ('A' <= ch && ch <= 'Z') {
                return ViewerCharType.HalfAlphaNum;
            } else if ('a' <= ch && ch <= 'z') {
                return ViewerCharType.HalfAlphaNum;
            } else if ('0' <= ch && ch <= '9') {
                return ViewerCharType.HalfAlphaNum;
            } else if (ch == '_') {
                return ViewerCharType.HalfAlphaNum;
            } else if (ch == '\t' || ch == ' ' || ch == '　') {
                return ViewerCharType.Space;
            } else if (ch < 0x7f) {
                return ViewerCharType.HalfSymbol;
            } else if (FULL_SYMBOL.IndexOf(ch) != -1) {
                return ViewerCharType.FullSymbol;
            } else {
                return ViewerCharType.FullChar;
            }
        }

        //=========================================================================================
        // 機　能：ダンプビューアでの文字種別を返す
        // 引　数：[in]data  調べるバイト
        // 戻り値：文字種別
        //=========================================================================================
        public static ViewerCharType GetViewerByteType(byte data) {
            char ch = (char)data;
            if ('A' <= ch && ch <= 'Z') {
                return ViewerCharType.HalfAlphaNum;
            } else if ('a' <= ch && ch <= 'z') {
                return ViewerCharType.HalfAlphaNum;
            } else if ('0' <= ch && ch <= '9') {
                return ViewerCharType.HalfAlphaNum;
            } else if (ch == '_') {
                return ViewerCharType.HalfAlphaNum;
            } else if (ch == '\t' || ch == ' ') {
                return ViewerCharType.Space;
            } else if (ch == 0x00) {
                return ViewerCharType.Zero;
            } else if (ch < 0x20) {
                return ViewerCharType.Binary;
            } else if (ch < 0x7f) {
                return ViewerCharType.HalfSymbol;
            } else {
                return ViewerCharType.Binary;
            }
        }
    }
}
