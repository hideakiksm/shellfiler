using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Locale;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession {

    //=========================================================================================
    // クラス：エスケープシーケンスの意味解析クラス
    //=========================================================================================
    public class EscapeSequenceParser {
        // 文字コードのパターンで使用する定義
        private static char ESC = (char)0x1b;           // ESC文字そのもの
        private static char NUM1 = (char)1;             // 任意の桁数の数値
        private static char NUM2 = (char)2;             // 任意の桁数の数値
        private static char NUMLIST = (char)3;          // 任意の桁数の数値（可変長）
        private static char DIGIT2 = (char)4;           // 2桁の数値xyでxとyそれぞれ0x20を足した数値

        // エスケープシーケンスのトライ構造
        private static TrieNode s_escapeTrie;

        // 解析時対象の文字列
        private string m_receivedString;

        //=========================================================================================
        // 機　能：スタティックイニシャライザ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        static EscapeSequenceParser() {
            s_escapeTrie = new TrieNode(ESC);

            // http://hp.vector.co.jp/authors/VA016670/escape_code.htmlより
            Add(EscapeType.Nop,                     ESC + "[>1l");                           // ファンク有り, ESC[>1l
            Add(EscapeType.Nop,                     ESC + "[>1h");                           // ファンクなし, ESC[>1h
            Add(EscapeType.Nop,                     ESC + "[>5l");                           // カーソル有り, ESC[>5l
            Add(EscapeType.Nop,                     ESC + "[>5h");                           // カーソルなし, ESC[>5h
            Add(EscapeType.SetColor,                ESC + "[" + NUMLIST + "m");              // 色をつける, ESC[色番号m 
            Add(EscapeType.MoveCursor,              ESC + "[" + NUM1 + ";" + NUM2 + "H");    // カーソルをY行目のX桁目に移動する。, ESC[Y;XH
            Add(EscapeType.MoveCursor,              ESC + "[" + NUM1 + "," + NUM2 + "H");    // カーソルをY行目のX桁目に移動する。, ESC[Y;XH
            Add(EscapeType.MoveCursor,              ESC + "[" + NUM1 + ";" + NUM2 + "f");    // カーソルをY行目のX桁目に移動する。, ESC[Y;Xf
            Add(EscapeType.MoveCursor,              ESC + "[" + NUM1 + "," + NUM2 + "f");    // カーソルをY行目のX桁目に移動する。, ESC[Y;Xf
            Add(EscapeType.SetScrollRange,          ESC + "[" + NUM1 + ";" + NUM2 + "r");    // スクロール範囲をS行目からE行目までに設定する, ESC[S;Er
//          Add(EscapeType.MoveCursor,              ESC + "=" + DIGIT2);                     // カーソルをl行c列に移動する。ただしlとcは20hが加えられた値となる, ESC=lc
            Add(EscapeType.CursorUpWith,            ESC + "[" + NUM1 + "A");                 // カーソルをY行上へ移動する。, ESC[YA
            Add(EscapeType.CursorDownWith,          ESC + "[" + NUM1 + "B");                 // カーソルをY行下へ移動する。, ESC[YB
            Add(EscapeType.CursorRightWith,         ESC + "[" + NUM1 + "C");                 // カーソルをX桁右へ移動する。行の右端より先には移動しない。 （次の行へは移動しない）, ESC[XC
            Add(EscapeType.CursorLeftWith,          ESC + "[" + NUM1 + "D");                 // カーソルをX桁左へ移動する。行の左端より先には移動しない。 （前の行へは移動しない）, ESC[XD
            Add(EscapeType.CursorHome,              ESC + "[H");                             // カーソルを左上に移動する。, ESC[H
            Add(EscapeType.DeleteBelow,             ESC + "[" + NUM1 + "M");                 // カーソル行から下へpn行削除する, ESC[pnM
//          Add(EscapeType.DeleteUpper,             ESC + "[" + NUM1 + "L");                 // カーソル行から上へpn行削除する, ESC[pnL
            Add(EscapeType.CursorNextLeft,          ESC + "E");                              // カーソルを１行下の一番左に移動する。(カーソルが最下行にある場合は１行スクロールする), ESCE
            Add(EscapeType.CursorUp,                ESC + "M");                              // カーソルをカラム位置はそのままに１行上に移動する。(カーソルが最上行にある場合は機種依存) , ESCM
            Add(EscapeType.CursorUp,                ESC + "[A");                             // カーソルをカラム位置はそのままに１行上に移動する。(カーソルが最上行にある場合は機種依存) , ESC[A
            Add(EscapeType.CursorDown,              ESC + "D");                              // カーソルをカラム位置はそのままに１行下に移動する。(カーソルが最下行にある場合は１行スクロールする) , ESCD
            Add(EscapeType.CursorDown,              ESC + "[B");                             // カーソルをカラム位置はそのままに１行下に移動する。(カーソルが最下行にある場合は１行スクロールする) , ESC[B
            Add(EscapeType.CursorRight,             ESC + "[C");                             // カーソルを１つ右に移動する。 , ESC[C
            Add(EscapeType.CursorLeft,              ESC + "[D");                             // カーソルを１つ左に移動する。 , ESC[D
            Add(EscapeType.Nop,                     ESC + "[M");                             // １上, ESC[M
            Add(EscapeType.Nop,                     ESC + "[E");                             // １下１, ESC[E
            Add(EscapeType.InsertLine,              ESC + "[L");                             // カーソル行に1行追加する, ESC[L
            Add(EscapeType.InsertLineWith,          ESC + "[" + NUM1 + "L");                 // カーソル行にX行追加する, ESC[XL
            Add(EscapeType.RegistPosition,          ESC + "[s");                             // カーソル位置と表示文字の属性をセーブする。, ESC[s
            Add(EscapeType.RestorePosition,         ESC + "[u");                             // 上でセーブした内容をロードする。セーブされていない場合はデフォルトの属性に戻し、一番左上にカーソルを移動する。(機種依存) , ESC[u
            Add(EscapeType.Nop,                     ESC + "[6n");                            // コンソール入力, ESC[6n 
            Add(EscapeType.ClearAllNext,            ESC + "[0J");                            // カーソル位置から最終行の右端まで削除する, ESC[0J
            Add(EscapeType.ClearAllNext,            ESC + "[J");                             // 後ろ消す, ESC[J
            Add(EscapeType.ClearAllPrev,            ESC + "[1J");                            // 先頭行の左端からカーソル位置まで削除する, ESC[1J
            Add(EscapeType.ClearScreen,             ESC + "[2J");                            // 画面全体を消去し、カーソルを左上に移動する, ESC[2J
            Add(EscapeType.ClearScreen,             ESC + "*");                              // 画面全体を消去し、カーソルを左上に移動する, ESC*
            Add(EscapeType.ClearLineNext,           ESC + "[0K");                            // カーソル位置から同一行の右端まで削除する, ESC[0K
            Add(EscapeType.ClearLineNext,           ESC + "[K");                             // 右消す, ESC[K
            Add(EscapeType.ClearLinePrev,           ESC + "[1K");                            // 同一行の左端からカーソル位置まで削除する, ESC[1K
            Add(EscapeType.ClearLine,               ESC + "[2K");                            // カーソルのある行全て削除する, ESC[2K 
            Add(EscapeType.Nop,                     ESC + "$B");                             // JIS KI code (JIS X 0208-1983 新JIS), ESC$B
            Add(EscapeType.Nop,                     ESC + "$@");                             // JIS KI code (JIS X 0208-1978 旧JIS), ESC$@
            Add(EscapeType.Nop,                     ESC + "$B");                             // EUC KI code (JIS X 0208-1983 UJIS), ESC$B
            Add(EscapeType.Nop,                     ESC + "$K");                             // 区点(7bit×2) KI code, ESC$K
            Add(EscapeType.Nop,                     ESC + "(J");                             // JISローマ字(7bit) KI code (JIS X 0201), ESC(J
            Add(EscapeType.Nop,                     ESC + "(B");                             // Ascii KO code, ESC(B
            Add(EscapeType.TerminalModeApplication, ESC + "=");                              // ターミナルモードをApplicationへ（poderosaより）, ESC =
            Add(EscapeType.TerminalModeNormal,      ESC + ">");                              // ターミナルモードをNormalへ（poderosaより）, ESC >
        }

/*
http://matthieu.benoit.free.fr/68hc11/vt100.htm 
c		Reset
[ ! p		Soft Reset
# 8		Fill Screen with E's
} 1 * 		Fill screen with * test
} 2		Video attribute test display
} 3		Character sets display test

KEYBOARD COMMANDS
~~~~~~~~~~~~~~~~
[ 2 h		Keyboard locked
[ 2 l		Keyboard unlocked
[ ? 8 h		Autorepeat ON
[ ? 8 l		Autorepeat OFF
[ 0 q		Lights all off on keyboard
[ * q		Light * on

PROGRAMMABLE KEY COMMANDS
~~~~~~~~~~~~~~~~~~~~~~~~
! pk		Program a programmable key (local)
@ pk		Program a programmable key (on-line)
% pk		Transmit programmable key contents

SCREEN FORMAT
~~~~~~~~~~~~
[ ? 3 h		132 Characters on
[ ? 3 l		80 Characters on
[ ? 4 h		Smooth Scroll on
[ ? 4 l		Jump Scroll on
[ *t ; *b r	Scrolling region selected, line *t to *b
[ ? 5 h		Inverse video on
[ ? 5 l		Normal video off
[ ? 7 h		Wraparound ON
[ ? 7 l		Wraparound OFF
[ ? 75 h	Screen display ON
[ ? 75 l	Screen display OFF

CHARACTER SETS AND LABELS
~~~~~~~~~~~~~~~~~~~~~~~~
( A		British 
( B		North American ASCII set
( C		Finnish
( E		Danish or Norwegian
( H		Swedish
( K		German
( Q		French Canadian
( R		Flemish or French/Belgian
( Y		Italian
( Z		Spanish
( 0		Line Drawing
( 1		Alternative Character
( 2		Alternative Line drawing
( 4		Dutch
( 5		Finnish
( 6		Danish or Norwegian
( 7		Swedish
( =		Swiss (French or German)

[Note all ( may be replaced with )]

CHARACTER SIZE
~~~~~~~~~~~~~
# 1		Double ht, single width top half chars
# 2		Double ht, single width lower half chars
# 3 		Double ht, double width top half chars
# 4		Double ht, double width lower half chars
# 5		Single ht, single width chars
# 6		Single ht, double width chars

ATTRIBUTES AND FIELDS
~~~~~~~~~~~~~~~~~~~~
[ 0 m		Clear all character attributes
[ 1 m		Alternate Intensity ON
[ 4 m		Underline ON
[ 5 m		Blink ON
[ 7 m		Inverse video ON
[ 22 m		Alternate Intensity OFF
[ 24 m		Underline OFF
[ 25 m		Blink OFF
[ 27 m		Inverse Video OFF
[ 0 }		Protected fields OFF
[ 1 } 		Protected = Alternate Intensity
[ 4 }		Protected = Underline
[ 5 }		Protected = Blinking
[ 7 }		Protected = Inverse
[ 254 }		Protected = All attributes OFF

CURSOR COMMANDS
~~~~~~~~~~~~~~
[ ? 25 l	Cursor OFF
[ ? 25 h	Cursor ON
[ ? 50 l	Cursor OFF
[ ? 50 h	Cursor ON
7		Save cursor position and character attributes
8		Restore cursor position and character attributes
D		Line feed
E		Carriage return and line feed
M		Reverse Line feed
    [ A		Cursor up one line
    [ B		Cursor down one line
    [ C		Cursor right one column
    [ D		Cursor left one column
    [ * A		Cursor up * lines
    [ * B		Cursor down * lines
    [ * C		Cursor right * columns
    [ * D		Cursor left * columns
    [ H		Cursor home
    [ *l ; *c H	Move cursor to line *l, column *c
    [ *l ; *c f	Move curosr to line *l, column *c
Y nl nc 	Direct cursor addressing (line/column number)
H		Tab set at present cursor position
[ 0 g		Clear tab at present cursor position
[ 3 g		Clear all tabs

EDIT COMMANDS
~~~~~~~~~~~~
[ 4 h		Insert mode selected
[ 4 l		Replacement mode selected
[ ? 14 h	Immediate operation of ENTER key
[ ? 14 l	Deferred operation of ENTER key
[ ? 16 h	Edit selection immediate
[ ? 16 l	Edit selection deffered
[ P		Delete character from cursor position
[ * P		Delete * chars from curosr right
[ M		Delete 1 char from cursor position
[ * M		Delete * lines from cursor line down
[ J		Erase screen from cursor to end
[ 1 J		Erase beginning of screen to cursor
[ 2 J		Erase entire screen but do not move cursor
[ K		Erase line from cursor to end
[ 1 K		Erase from beginning of line to cursor
[ 2 K		Erase entire line but do not move cursor
[ L		Insert 1 line from cursor position
[ * L		Insert * lines from cursor position
*/
        //=========================================================================================
        // 機　能：スタティックイニシャライザでパターンを追加する
        // 引　数：[in]escapeType   結果として返すエスケープシーケンスの識別子
        // 　　　　[in]template     エスケープシーケンスのテンプレート
        // 戻り値：なし
        //=========================================================================================
        private static void Add(EscapeType escapeType, string template) {
            TrieNode prevNode = s_escapeTrie;
            for (int i = 1; i < template.Length; i++) {
                char currentChar = template[i];
                TrieNode currentNode = null;                            // i番目の文字に対応するノード
                List<TrieNode> currentNodeList = prevNode.NextNode;     // i番目の文字の兄弟ノード
                for (int j = 0; j < currentNodeList.Count; j++) {
                    if (currentChar == currentNodeList[j].Char) {
                        currentNode = currentNodeList[j];
                        break;
                    }
                }
                if (currentNode == null) {
                    // 新出の場合
                    currentNode = new TrieNode(currentChar);
                    prevNode.NextNode.Add(currentNode);
                    if (i == template.Length - 1) {
                        currentNode.ResultType = escapeType;
                    }
                } else {
                    // 既出の場合
                    ;
                }
                prevNode = currentNode;
            }
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]receivedString  受信した文字列
        // 戻り値：なし
        //=========================================================================================
        public EscapeSequenceParser(string receivedString) {
            m_receivedString = receivedString;
        }

        //=========================================================================================
        // 機　能：コンストラクタで指定した受信文字列を解析する
        // 引　数：[in,out]indexOrg    文字列中の解析開始インデックス位置（文字位置はESC、次のインデックス位置を返す）
        // 　　　　[out]resultType     解析結果を返す変数
        // 　　　　[out]resultAddInfo  解析情報としての付加情報を返す変数（付加情報がないときnull）
        // 戻り値：解析に成功したときtrue、テンプレートにないシーケンスのときfalse
        //=========================================================================================
        public bool Parse(ref int indexOrg, out EscapeType resultType, out List<int> resultAddInfo) {
            resultAddInfo = null;
            List<TrieCandidate> candidateList = new List<TrieCandidate>();
            candidateList.Add(new TrieCandidate(indexOrg, s_escapeTrie, new List<int>()));

            int maxIndex = indexOrg;
            while (candidateList.Count > 0) {
                TrieCandidate parseTarget = candidateList[candidateList.Count - 1];
                candidateList.RemoveAt(candidateList.Count - 1);
                int index = parseTarget.StartIndex;
                TrieNode node = parseTarget.Node;
                List<int> addInfo;
                CompareTrieNodeResult compResult = CompareTrieNode(ref index, node.Char, out addInfo);
                maxIndex = Math.Max(maxIndex, index);
                switch (compResult) {
                    case CompareTrieNodeResult.StringShortage:
                        resultType = EscapeType.Nop;
                        return false;
                    case CompareTrieNodeResult.Success:
                        break;
                    case CompareTrieNodeResult.NotMatch:
                        continue;
                }
                List<int> addInfoNext = new List<int>();
                addInfoNext.AddRange(parseTarget.AddInfo);
                if (addInfo != null) {
                    addInfoNext.AddRange(addInfo);
                }
                if (node.NextNode.Count == 0) {
                    // ここで終わり
                    indexOrg = maxIndex;
                    resultType = node.ResultType;
                    resultAddInfo = addInfoNext;
                    return true;
                } else {
                    // 次の候補を検索
                    for (int i = node.NextNode.Count - 1; i >= 0; i--) {
                        TrieCandidate nextCandidate = new TrieCandidate(index, node.NextNode[i], addInfoNext); 
                        candidateList.Add(nextCandidate);
                    }
                }
            }
            // すべて検索しても見つからないときは不明なエスケープとして処理
            indexOrg = maxIndex;
            resultType = EscapeType.Nop;
            return true;
        }

        //=========================================================================================
        // 機　能：トライのノードと解析対象の文字列を比較する
        // 引　数：[in,out]indexOrg  文字列中の解析開始インデックス位置（次のインデックス位置を返す）
        // 　　　　[in]chNode        トライの現在の文字
        // 　　　　[out]addInfo      付加情報を返す変数（付加情報がないときnull）
        // 戻り値：ノードの解析結果
        //=========================================================================================
        private CompareTrieNodeResult CompareTrieNode(ref int index, char chNode, out List<int> addInfo) {
            CompareTrieNodeResult result;
            if (chNode == NUM1 || chNode == NUM2) {
                result = CompareTrieNodeNum1(ref index, out addInfo);
            } else if (chNode == NUMLIST) {
                result = CompareTrieNodeNumMulti(ref index, out addInfo);
            } else if (chNode == DIGIT2) {
                result = CompareTrieNodeNumDigit2(ref index, out addInfo);
            } else {
                addInfo = null;
                result = CompareTrieNodeEtc(ref index, chNode);
            }
            return result;
        }

        //=========================================================================================
        // 機　能：トライの数値ノードと解析対象の文字列を比較する
        // 引　数：[in,out]indexOrg  文字列中の解析開始インデックス位置（数値の1桁目、次のインデックス位置を返す）
        // 　　　　[out]addInfo      付加情報を返す変数
        // 戻り値：ノードの解析結果
        //=========================================================================================
        private CompareTrieNodeResult CompareTrieNodeNum1(ref int index, out List<int> addInfo) {
            addInfo = null;
            int digitNum = 0;               // 解析した数値の文字数
            int parsedValue = 0;            // 解析結果
            while (true) {
                if (index >= m_receivedString.Length) {
                    return CompareTrieNodeResult.StringShortage;
                }
                char ch = m_receivedString[index];
                if ('0' <= ch && ch <= '9') {
                    digitNum++;
                    parsedValue = parsedValue * 10 + (ch - '0');
                    if (parsedValue < 0) {
                        return CompareTrieNodeResult.NotMatch;
                    }
                    index++;
                } else {
                    break;
                }
            }
            if (digitNum == 0) {
                return CompareTrieNodeResult.NotMatch;
            }
            addInfo = new List<int>();
            addInfo.Add(parsedValue);
            return CompareTrieNodeResult.Success;
        }

        //=========================================================================================
        // 機　能：トライの複数個の数値ノードと解析対象の文字列を比較する
        // 引　数：[in,out]indexOrg  文字列中の解析開始インデックス位置（数値の1桁目、次のインデックス位置を返す）
        // 　　　　[out]addInfo      付加情報を返す変数
        // 戻り値：ノードの解析結果
        // メ　モ：「12;34;56」より、{12,34,56}を取得
        //=========================================================================================
        private CompareTrieNodeResult CompareTrieNodeNumMulti(ref int index, out List<int> addInfo) {
            addInfo = null;
            List<int> addInfoTemp = new List<int>();
            int parsedValue = 0;            // 解析結果
            while (true) {
                if (index >= m_receivedString.Length) {
                    return CompareTrieNodeResult.StringShortage;
                }
                char ch = m_receivedString[index];
                if ('0' <= ch && ch <= '9') {
                    parsedValue = parsedValue * 10 + (ch - '0');
                    if (parsedValue < 0) {
                        return CompareTrieNodeResult.NotMatch;
                    }
                    index++;
                } else if (ch == ';') {
                    addInfoTemp.Add(parsedValue);
                    parsedValue = 0;
                    index++;
                } else {
                    break;
                }
            }
            addInfoTemp.Add(parsedValue);
            addInfo = addInfoTemp;
            return CompareTrieNodeResult.Success;
        }

        //=========================================================================================
        // 機　能：トライの数値2桁（0x20シフト）より数値2桁を取得する
        // 引　数：[in,out]indexOrg  文字列中の解析開始インデックス位置（数値の1桁目、次のインデックス位置を返す）
        // 　　　　[out]addInfo      付加情報を返す変数
        // 戻り値：ノードの解析結果
        // メ　モ：「!1」より、{1,17}を取得
        //=========================================================================================
        private CompareTrieNodeResult CompareTrieNodeNumDigit2(ref int index, out List<int> addInfo) {
            addInfo = null;
            if (index + 1 >= m_receivedString.Length) {
                return CompareTrieNodeResult.NotMatch;
            }
            int data1 = Math.Max(0, m_receivedString[index++] - 0x20);
            int data2 = Math.Max(0, m_receivedString[index++] - 0x20);
            addInfo = new List<int>();
            addInfo.Add(data1);
            addInfo.Add(data2);
            return CompareTrieNodeResult.Success;
        }

        //=========================================================================================
        // 機　能：トライのその他の文字列を取得する
        // 引　数：[in,out]indexOrg  文字列中の解析開始インデックス位置（次のインデックス位置を返す）
        // 　　　　[in]chNode        トライのノードのテンプレートに設定された想定文字
        // 戻り値：ノードの解析結果
        //=========================================================================================
        private CompareTrieNodeResult CompareTrieNodeEtc(ref int index, char chNode) {
            if (index >= m_receivedString.Length) {
                return CompareTrieNodeResult.StringShortage;
            }
            if (chNode == m_receivedString[index]) {
                index++;
                return CompareTrieNodeResult.Success;
            } else {
                return CompareTrieNodeResult.NotMatch;
            }
        }

        //=========================================================================================
        // 列挙子：トライのノードの解析結果
        //=========================================================================================
        private enum CompareTrieNodeResult {
            StringShortage,                 // パターンに対して入力文字列が不足している
            Success,                        // パターンと一致した
            NotMatch,                       // パターンと一致しなかった
        }

        //=========================================================================================
        // クラス：トライのノード
        //=========================================================================================
        private class TrieNode {
            // このノードの文字
            public char Char;

            // 解析結果（NextNodeが空のとき有効）
            public EscapeType ResultType = EscapeType.Nop;
            
            // 次の文字の候補一覧
            public List<TrieNode> NextNode = new List<TrieNode>();

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]ch   ノードに設定する文字
            // 戻り値：なし
            //=========================================================================================
            public TrieNode(char ch) {
                Char = ch;
            }
        }

        //=========================================================================================
        // クラス：トライを使った解析で、次の解析の候補になっているノード群
        //=========================================================================================
        private class TrieCandidate {
            // 候補の開始位置の文字インデックス
            public int StartIndex;

            // 候補のトライノード
            public TrieNode Node;

            // 解析で取得した付加情報
            public List<int> AddInfo;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]startIndex   候補の開始位置の文字インデックス
            // 　　　　[in]node         候補のトライノード
            // 　　　　[in]addInfo      解析で取得した付加情報
            // 戻り値：なし
            //=========================================================================================
            public TrieCandidate(int startIndex, TrieNode node, List<int> addInfo) {
                StartIndex = startIndex;
                Node = node;
                AddInfo = addInfo;
            }
        }
    }
}




