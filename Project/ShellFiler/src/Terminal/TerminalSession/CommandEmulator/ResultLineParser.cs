using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Shell;
using ShellFiler.UI;
using ShellFiler.Util;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {
    
    //=========================================================================================
    // クラス：結果行の解析エンジン
    //=========================================================================================
    public class ResultLineParser {
        // 使用中のSSH接続
        private SSHConnection m_connection;

        // 使用中の解析エンジン
        private IShellEmulatorEngine m_emulatorEngine;

        // コマンドのエミュレータ
        private ShellCommandEmulator m_shellCommandEmulator;

        // 結果行の解析ルール
        private List<OSSpecLineExpect> m_parseRule;

        // 応答行の先頭にあるコマンドのエコーを受信したときtrue
        private bool m_skippedCommandEcho = false;

        // 現在解析中の行に対するm_parseRuleの位置
        private int m_parseRulePosition = 0;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection  使用中のSSH接続
        // 　　　　[in]engine      使用中の解析エンジン
        // 　　　　[in]emlator     コマンドのエミュレータ
        // 　　　　[in]parseRule   結果行の解析ルール
        // 戻り値：なし
        //=========================================================================================
        public ResultLineParser(SSHConnection connection, IShellEmulatorEngine engine, ShellCommandEmulator emulator, List<OSSpecLineExpect> parseRule) {
            m_connection = connection;
            m_emulatorEngine = engine;
            m_shellCommandEmulator = emulator;
            m_parseRule = parseRule;
        }

        //=========================================================================================
        // 機　能：SSHからのデータが受信できたときの処理を行う
        // 引　数：[in]buffer        受信したデータのバッファ
        // 　　　　[out]resultLine   解析結果の行単位リストを返す変数
        // 　　　　[out]completed    解析が完了したときtrue
        // 　　　　[out]remainBytes  受信データを仮想画面に転送する内容（エラーのときnull）
        // 戻り値：エラーコード
        // メ　モ：ストリームスレッドで実行される
        //=========================================================================================
        public ShellEngineError OnReceive(byte[] buffer, out List<ParsedLine> resultLine, out bool completed, out byte[] remainBytes) {
            resultLine = new List<ParsedLine>();

            // 行単位に分解
            ShellEngineError error;
            List<string> parsedLine;
            error = SplitLine(buffer, out parsedLine, out completed, out remainBytes);
            if (error.IsFailed) {
                return error;
            }

            // 行からトークンを取得
            for (int i = 0; i < parsedLine.Count; i++) {
                List<ResultLineParser.ParsedToken> resultToken;
                OSSpecLineType resultLineType;
                int hitRule;
                error = ParseEachLine(parsedLine[i], m_parseRule, ref m_parseRulePosition, out hitRule, out resultToken, out resultLineType);
                if (error.IsFailed) {
                    return error;
                }
                resultLine.Add(new ParsedLine(parsedLine[i], resultLineType, resultToken));
            }
            return ShellEngineError.Success();
        }

        //=========================================================================================
        // 機　能：受信したバイト列を行単位に切り分ける
        // 引　数：[in]buffer        受信したデータのバッファ
        // 　　　　[out]resultLine   解析結果の行単位リストを返す変数
        // 　　　　[out]completed    解析が完了したときtrue
        // 　　　　[out]remainBytes  受信データを仮想画面に転送する内容（エラーのときnull）
        // 戻り値：エラーコード
        //=========================================================================================
        private ShellEngineError SplitLine(byte[] buffer, out List<string> resultLine, out bool completed, out byte[] remainBytes) {
            Encoding encoding = m_connection.AuthenticateSetting.Encoding;
            byte[] prompt = encoding.GetBytes(m_shellCommandEmulator.PromptString);

            // 行に分解
            // 最後が改行だと、split.Offsetがbufferの領域を超える点に注意
            BufferRange[] split = CommandReplyChecker.ParseLinePosiiton(buffer, m_shellCommandEmulator.ReturnCharReceive);
            if (split.Length == 1) {
                // 行の途中で止まっている
                int dummyPromptLength, dummyNextCheckIndex;
                string userServer;
                bool found = CommandReplyChecker.CheckPrompt(encoding, buffer, 0, buffer.Length, prompt, false, out userServer, out dummyPromptLength, out dummyNextCheckIndex);
                if (found) {
                    // プロンプトを発見したら終了
                    resultLine = new List<string>();
                    completed = true;
                    remainBytes = new byte[0];
                    m_shellCommandEmulator.ShellChannel.ActiveUserServer = userServer;
                    return ShellEngineError.Success();
                } else {
                    // 次の受信と合わせて解析
                    resultLine = new List<string>();
                    completed = false;
                    remainBytes = buffer;
                    return ShellEngineError.Success();
                }
            }

            // コマンドエコーの解析
            int bodyStartIndex = 0;         // コマンド実行結果本体が含まれるsplitのインデックス
            if (!m_skippedCommandEcho) {
                // コマンドのエコーをスキップ
                // リクエスト文字列と同じものが先頭行で受信しているはず
                string strCommand = m_emulatorEngine.GetRequest();
                byte[] commandEcho = m_connection.AuthenticateSetting.Encoding.GetBytes(strCommand);
                int echoNext = CommandReplyChecker.CheckCommandEcho(buffer, split[0].Offset, split[0].Length, commandEcho);
                if (echoNext == CommandReplyChecker.ECHO_NOT_MATCH) {
                    // 先頭がコマンドエコーではない
                    resultLine = new List<string>();
                    completed = false;
                    remainBytes = new byte[0];
                    return ShellEngineError.Fail(FileOperationStatus.Fail, Resources.ShellError_CommandEcho, m_connection.AuthenticateSetting.Encoding.GetString(buffer, split[0].Offset, split[0].Length));
                } else if (echoNext == CommandReplyChecker.ECHO_SHOTAGE) {
                    resultLine = new List<string>();
                    completed = false;
                    remainBytes = ArrayUtils.CreateCleanedBuffer<byte>(buffer, 0, buffer.Length);
                    return ShellEngineError.Success();
                }
                m_skippedCommandEcho = true;
                bodyStartIndex = 1;
            }

            // 応答本体の解析
            resultLine = new List<string>();
            completed = false;                     // 0回実行のデフォルト（split.Length > 2より実際はない）
            remainBytes = new byte[0];             // 0回実行のデフォルト（split.Length > 2より実際はない）
            for (int i = bodyStartIndex; i < split.Length; i++) {
                if (i == split.Length - 1) {
                    // 最終行の解析
                    int dummyPromptLength, dummyNextCheckIndex;
                    string userServer;
                    bool found = CommandReplyChecker.CheckPrompt(encoding, buffer, split[i].Offset, split[i].Length, prompt, false, out userServer, out dummyPromptLength, out dummyNextCheckIndex);
                    if (found) {
                        // 最終行でプロンプトを発見したら終了
                        completed = true;
                        remainBytes = new byte[0];
                        m_shellCommandEmulator.ShellChannel.ActiveUserServer = userServer;
                    } else {
                        // 最終行がプロンプト以外の場合はまだ受信し切れていない
                        completed = false;
                        remainBytes = ArrayUtils.CreateCleanedBuffer<byte>(buffer, split[i].Offset, buffer.Length - split[i].Offset);
                    }
                } else {
                    // 途中の有効な行の解析
                    string strLine = BytesToString(buffer, split[i].Offset, split[i].Length, m_connection.AuthenticateSetting.Encoding);
                    resultLine.Add(strLine);
                }
            }
            return ShellEngineError.Success();
        }

        //=========================================================================================
        // 機　能：バイト列を文字列に変換する
        // 引　数：[in]buffer    受信したデータのバッファ
        // 　　　　[in]offset    解析の開始オフセット
        // 　　　　[in]length    解析する長さ
        // 　　　　[in]encoding  文字列のエンコード方式
        // 戻り値：変換した文字列
        //=========================================================================================
        public static string BytesToString(byte[] buffer, int offset, int length, Encoding encoding) {
            if (length == 0) {
                return "";
            }
            string result = encoding.GetString(buffer, offset, length);
            result = EscapeSequenceEraser.Execute(result);
            return result;
        }

        //=========================================================================================
        // 機　能：行の内容をトークンに分解する
        // 引　数：[in]line             解析対象の行
        // 　　　　[in]parseRule        解析ルール
        // 　　　　[in,out]parseRulePos 解析ルールの開始位置（次の位置を帰す）
        // 　　　　[out]hitRule         ヒットした解析ルールのインデックスを返す変数（ヒットしなかったとき-1）
        // 　　　　[out]result          解析結果を返す変数
        // 　　　　[out]lineType        結果行の種類を返す変数
        // 戻り値：解析に成功したときtrue
        // メ　モ：バックグラウンド処理スレッドで実行される
        // 　　　　ここに次のプロンプトが来ることはない
        //=========================================================================================
        public static ShellEngineError ParseEachLine(string line, List<OSSpecLineExpect> parseRule, ref int parseRulePos, out int hitRule, out List<ParsedToken> result, out OSSpecLineType lineType) {
            result = new List<ParsedToken>();
            lineType = OSSpecLineType.None;

            // 候補すべてに対して解析
            int parseStartPosition = parseRulePos;
            while (parseRulePos < parseRule.Count) {
                OSSpecLineExpect lineExpect = parseRule[parseRulePos];
                lineType = lineExpect.LineType;
                bool success = ParseOneLine(line, lineExpect, out result);
                if (success) {
                    // 期待値に一致したとき
                    hitRule = parseRulePos;
                    parseRulePos = NextParseSuccessPosition(parseRule, parseRulePos, parseStartPosition);
                    if ((lineType & OSSpecLineType.ErrorLine) != 0) {
                        return ShellEngineError.Fail(FileOperationStatus.Fail, Resources.ShellError_ErrorLine, line);
                    }
                    return ShellEngineError.Success();
                }

                // 期待値に一致しなかったときは次へ
                int next = parseRulePos + 1;
                if (next < parseRule.Count && (parseRule[next].LineType & OSSpecLineType.OrPrev) != 0) {
                    // 次の行はOR指定のため継続
                    parseRulePos++;
                } else {
                    // すべてのOR指定で失敗した
                    break;
                }
            }
            hitRule = -1;
            return ShellEngineError.Fail(FileOperationStatus.Fail, Resources.ShellError_LineParse_OSSPEC, line);
        }

        //=========================================================================================
        // 機　能：パターンマッチング成功時、次回の解析位置を探す
        // 引　数：[in]parseRule  解析ルール
        // 　　　　[in]current    成功時の解析位置
        // 　　　　[in]start      成功時の候補群の先頭位置（OR指定の先頭行のインデックス）
        // 戻り値：次回の解析位置（最大でparseRule.Countまで進む）
        //=========================================================================================
        private static int NextParseSuccessPosition(List<OSSpecLineExpect> parseRule, int current, int start) {
            OSSpecLineType lineType = parseRule[current].LineType;
            if ((lineType & OSSpecLineType.Repeat) != 0) {
                // 現在行がリピートなら、先頭位置まで戻す
                return start;
            }
            int next = current;
            while (true) {
                if (next + 1 < parseRule.Count && (parseRule[next + 1].LineType & OSSpecLineType.OrPrev) != 0) {
                    // 次の行がORなら次の行に進める
                    next++;
                } else {
                    // 次の行がOR以外ならその行に進めて終わる
                    next++;
                    break;
                }
            }
            return next;
        }

        //=========================================================================================
        // 機　能：1行の解析を行う
        // 引　数：[in]line         解析対象の行
        // 　　　　[in]lineExpect   行の内容の期待値
        // 　　　　[out]result      解析したトークンのリストを返す変数
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private static bool ParseOneLine(string line, OSSpecLineExpect lineExpect, out List<ParsedToken> result) {
            result = new List<ParsedToken>();
            // 各行の先頭から解析
            int expectColumnPos = 0;
            int length = line.Length;
            int parsePos = 0;
            while (parsePos < length) {
                // トークン用の解析クラスを作成
                if (expectColumnPos >= lineExpect.ColumnExpect.Count) {
                    return false;
                }
                OSSpecColumnExpect columnExpect = lineExpect.ColumnExpect[expectColumnPos];
                IOSSpecTokenParser parser = columnExpect.TokenType.CreateParser();

                // 各トークンを解析
                object columnValue;
                bool success = parser.ParseToken(line, columnExpect, ref parsePos, out columnValue);
                if (!success) {
                    return false;
                }
                result.Add(new ParsedToken(columnExpect.TokenType, columnValue));
                expectColumnPos++;
            }

            // 最後まで解析できれば成功
            if (expectColumnPos == lineExpect.ColumnExpect.Count) {
                return true;
            } else if (line == "" && lineExpect.ColumnExpect.Count == 1 && lineExpect.ColumnExpect[0].TokenType == OSSpecTokenType.StrAll) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // プロパティ：応答行の先頭にあるコマンドのエコーを受信したときtrue
        //=========================================================================================
        public bool SkippedCommandEcho {
            get {
                return m_skippedCommandEcho;
            }
        }

        //=========================================================================================
        // クラス：解析結果1行分の情報
        //=========================================================================================
        public class ParsedLine {
            // 切り分けた行
            private string m_lineString;

            // 行の内容
            private OSSpecLineType m_lineType;

            // 行内のトークンの並び
            private List<ParsedToken> m_parsedToken;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parsedLine   切り分けた行
            // 　　　　[in]lineType     行の内容
            // 　　　　[in]parsedToken  行内のトークンの並び
            // 戻り値：なし
            //=========================================================================================
            public ParsedLine(string lineString, OSSpecLineType lineType, List<ParsedToken> parsedToken) {
                m_lineString = lineString;
                m_lineType = lineType;
                m_parsedToken = parsedToken;
            }

            //=========================================================================================
            // プロパティ：切り分けた行
            //=========================================================================================
            public string LineString {
                get {
                    return m_lineString;
                }
            }

            //=========================================================================================
            // プロパティ：行の内容
            //=========================================================================================
            public OSSpecLineType LineType {
                get {
                    return m_lineType;
                }
            }

            //=========================================================================================
            // プロパティ：行内のトークンの並び
            //=========================================================================================
            public List<ParsedToken> ParsedToken {
                get {
                    return m_parsedToken;
                }
            }
        }

        //=========================================================================================
        // クラス：行内のトークン
        //=========================================================================================
        public class ParsedToken {
            // 抽出したトークンの種類
            private OSSpecTokenType m_tokenType;

            // 解析済みデータ
            private object m_parsedData;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parseRule  解析ルール
            // 　　　　[in]emlator    コマンドのエミュレータ
            // 戻り値：なし
            //=========================================================================================
            public ParsedToken(OSSpecTokenType tokenType, object parsedData) {
                m_tokenType = tokenType;
                m_parsedData = parsedData;
            }

            //=========================================================================================
            // プロパティ：抽出したトークンの種類
            //=========================================================================================
            public OSSpecTokenType TokenType {
                get {
                    return m_tokenType;
                }
            }

            //=========================================================================================
            // プロパティ：解析済みデータ
            //=========================================================================================
            public object ParsedData {
                get {
                    return m_parsedData;
                }
            }
        }
    }
}
