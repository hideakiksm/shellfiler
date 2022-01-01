using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommandXmlConverter.Properties;
using ShellFiler.Document.Serialize.CommandApi;
using ShellFiler.Util;

namespace CommandXmlConverter {

    //=========================================================================================
    // クラス：Commandクラスのソース解析器
    // 次の形式を解析
    //    //=========================================================================================
    //    // クラス：コマンドを実行する
    //    // カーソルを上に{0}行だけ移動します。
    //    //   書式 　 CursorUp(int line)
    //    //   引数  　line:移動する行数
    //    // 　　　　　line-default:1
    //    // 　　　　　line-range:1,999999
    //    //   戻り値　なし
    //    //   対応Ver 1.0.0
    //    //=========================================================================================
    //    class CursorUpCommand : FileListActionCommand {
    //        public override void SetParameter(params object[] param) {
    //=========================================================================================
    public class CommandSourceParser {
        // 解析した内容
        private XCommandApi m_commandApi = new XCommandApi();

        // 解析対象のファイル名
        private readonly string m_fileName;

        // 解析対象のソースファイルの各行
        private readonly string[] m_sourceLine;
        
        // 0スタートの解析中行番号
        private int m_currentLine = 0;

        // 状態の管理
        private ParseState m_parseState = ParseState.Initial;
        
        // SetParameter行があったときtrue
        private bool m_existSetParameter = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileName       解析対象のファイル名
        // 　　　　[in]sourceFileList 解析対象のソースファイルの各行
        // 戻り値：なし
        //=========================================================================================
        public CommandSourceParser(string fileName, string[] sourceFileList) {
            m_fileName = fileName;
            m_sourceLine = sourceFileList;
        }

        //=========================================================================================
        // 機　能：ソースファイルを解析する
        // 引　数：なし
        // 戻り値：解析の結果、変換したコマンドの情報（スキップするときnull）
        //=========================================================================================
        public XCommandApi Parse() {
            for (m_currentLine = 0; m_currentLine < m_sourceLine.Length; m_currentLine++) {
                switch (m_parseState) {
                    case ParseState.Initial:                    // 初期状態
                        ParseClass();
                        break;
                    case ParseState.HeaderClassParsed:          // 「// クラス：」読み込み済み
                        ParseHeaderComment();
                        break;
                    case ParseState.HeaderCommentParsed:        // 説明読み込み済み
                        ParseHeaderPrototype();
                        break;
                    case ParseState.HeaderPrototypeParsed:      // プロトタイプ読み込み済み
                        ParseHeaderArgument();
                        break;
                    case ParseState.HeaderCompleted:            // ヘッダ読み込み完了
                        ParseClassDef();
                        break;
                    case ParseState.ClassDefParsed:             // クラス定義読み込み済み
                        ParseSetParameter();
                        break;
                }
                if (m_parseState == ParseState.Completed) {
                    break;
                }
            }
            if (m_existSetParameter && m_commandApi.ArgumentList.Count == 0) {
                throw NewException("SetParameterメソッドがあるのに引数のコメント定義がありません。");
            }
            if (!m_existSetParameter && m_commandApi.ArgumentList.Count > 0) {
                throw NewException("SetParameterメソッドがないのに引数のコメント定義があります。");
            }
            if (m_parseState == ParseState.Ignore) {
                m_commandApi = null;
            } else if (m_parseState == ParseState.ClassDefParsed || m_parseState == ParseState.Completed) {
                ;       // OK
            } else {
                throw NewException("ソースのコメント形式の不正により途中までしか解析できませんでした。");
            }
            return m_commandApi;
        }

        //=========================================================================================
        // 機　能：解析エラー発生時の例外を投げる
        // 引　数：[in]message  例外のメッセージ
        // 戻り値：なし
        //=========================================================================================
        private Exception NewException(string message) {
            throw new Exception(message + m_fileName + ":" + (m_currentLine + 1));
        }

        //=========================================================================================
        // 機　能：クラス名のコメントを解析する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ParseClass() {
            string source = m_sourceLine[m_currentLine].Trim();
            if (source.StartsWith("// クラス：")) {
                m_parseState = ParseState.HeaderClassParsed;
            } else if (source.StartsWith("/* クラス：")) {
                m_parseState = ParseState.Ignore;
            }
        }

        //=========================================================================================
        // 機　能：機能説明のコメントを解析する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ParseHeaderComment() {
            string source = m_sourceLine[m_currentLine].Trim();
            if (!source.StartsWith("// ")) {
                throw NewException("コメントにコマンドの説明がありません。");
            }
            source = source.Substring(3).Trim(' ', '\t', '　');
            m_commandApi.Comment = source;
            m_parseState = ParseState.HeaderCommentParsed;
        }

        //=========================================================================================
        // 機　能：APIの書式のコメントを解析する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ParseHeaderPrototype() {
            string source = m_sourceLine[m_currentLine].Trim();
            if (!source.StartsWith("// ")) {
                throw NewException("コメントに書式の説明がありません。");
            }
            source = source.Substring(3).Trim(' ', '\t', '　');
            if (!source.StartsWith("書式")) {
                throw NewException("コメントにキーワード「書式」がありません。");
            }
            source = source.Substring(2).Trim(' ', '\t', '　');
            string[] methodParam = source.Split('(');
            if (methodParam.Length != 2 || !methodParam[1].EndsWith(")")) {
                throw NewException("書式のコメントがXxxx()形式になっていません。");
            }
            m_commandApi.CommandName = methodParam[0];
            methodParam[1] = methodParam[1].Substring(0, methodParam[1].Length - 1).Trim();
            if (methodParam[1] != "") {
                string[] paramList = methodParam[1].Split(',');
                for (int i = 0; i < paramList.Length; i++) {
                    XCommandArgument arg = ParseCommandArgument(i, paramList[i]);
                    m_commandApi.ArgumentList.Add(arg);
                }
            }
            m_parseState = ParseState.HeaderPrototypeParsed;
        }

        //=========================================================================================
        // 機　能：APIの書式のうち、引数部分のコメントを解析する
        // 引　数：[in]index        対象となる引数のインデックス
        // 　　　　[in]paramSource  解析対象となる引数部分のソース
        // 戻り値：なし
        //=========================================================================================
        private XCommandArgument ParseCommandArgument(int index, string paramSource) {
            string[] paramList = StringUtils.SeparateBySpace(paramSource);
            if (paramList.Length != 2) {
                throw NewException("引数リストの" + (index + 1) + "番目が型＋値になっていません。");
            }
            XCommandArgument arg = new XCommandArgument();
            if (paramList[0] != "int" && paramList[0] != "string" && paramList[0] != "bool" && paramList[0] != "menu") {
                throw NewException("引数リストの" + (index + 1) + "番目が不明な型です。");
            }
            arg.ArgumentType = paramList[0];
            arg.ArgumentName = paramList[1];
            return arg;
        }

        //=========================================================================================
        // 機　能：引数説明のコメントを解析する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ParseHeaderArgument() {
            string source = m_sourceLine[m_currentLine].Trim();
            if (!source.StartsWith("// ")) {
                throw NewException("引数の説明がありません。");
            }
            source = source.Substring(3).Trim(' ', '\t', '　');
            if (source.StartsWith("戻り値")) {
                CheckArgumentList();
                m_parseState = ParseState.HeaderCompleted;
                return;
            }
            if (source.StartsWith("引数")) {
                source = source.Substring(2).Trim(' ', '\t', '　');
            }
            if (source == "なし") {
                m_parseState = ParseState.HeaderCompleted;
                return;
            }
            string[] paramValue = source.Split(new char[] {':'}, 2);            // "line-default", "1"
            if (paramValue.Length != 2) {
                throw NewException("引数の定義が「なし」または「キーワード:値」の形式になっていません。");
            }
            string[] paramMode = paramValue[0].Split('-');      // "line", "default"
            if (paramMode.Length >= 3) {
                throw NewException("引数の定義が「キーワード:値」の形式になっていません。");
            }
            XCommandArgument argument = GetCommandArgument(paramMode[0]);
            if (argument == null) {
                throw NewException("引数の説明で未知の仮引数" + paramMode[0] + "が使用されました。");
            }
            if (paramMode.Length == 1) {
                argument.ArgumentComment = paramValue[1];
            } else {
                if (paramMode[1] == "default") {
                    if (argument.ArgumentType == "string") {
                        argument.DefaultValue = paramValue[1];
                    } else if (argument.ArgumentType == "int") {
                        if (!int.TryParse(paramValue[1], out _)) {
                            throw NewException("引数の定義" + paramValue[0] + "でint型のデフォルト値が数値ではありません。");
                        }
                        argument.DefaultValue = paramValue[1];
                    } else if (argument.ArgumentType == "bool") {
                        if (paramValue[1] != "true" && paramValue[1] != "false") {
                            throw NewException("引数の定義" + paramValue[0] + "でbool型のデフォルト値がtrue/falseではありません。");
                        }
                        argument.DefaultValue = paramValue[1];
                    } else if (argument.ArgumentType == "menu") {
                        argument.DefaultValue = "";
                    }
                } else if (paramMode[1] == "range") {
                    if (argument.ArgumentType == "int") {
                        string[] range = paramValue[1].Split(',');
                        if (range.Length != 2) {
                            throw NewException("引数の定義" + paramValue[0] + "で数値の範囲が正しく指定されていません。");
                        }
                        if (!int.TryParse(range[0], out _) || !int.TryParse(range[1], out _)) {
                            throw NewException("引数の定義" + paramValue[0] + "で数値の範囲が数値で指定されていません。");
                        }
                        argument.ValueRange = paramValue[1];
                    } else if (argument.ArgumentType == "string") {
                        if (paramValue[1] != "") {
                            string[] rangeList = paramValue[1].Split(',');
                            foreach (string rangeItem in rangeList) {
                                if (rangeItem.Split('=').Length != 2) {
                                    throw NewException("引数の定義" + paramValue[0] + "で文字列の範囲が「キー=説明」で指定されていません。");
                                }
                            }
                        }
                        argument.ValueRange = paramValue[1];
                    } else if (argument.ArgumentType == "bool") {
                        argument.ValueRange = "";
                    } else if (argument.ArgumentType == "menu") {
                        argument.ValueRange = "";
                    }
                } else {
                    throw NewException("引数の定義「キーワード:値」のうち、キーワードで未知の指定" + paramMode[1] + "が指定されました。");
                }
            }
        }

        //=========================================================================================
        // 機　能：指定された解析済みの引数データを取得する
        // 引　数：[in]argName  取得したい引数の変数名
        // 戻り値：引数の解析済みデータ（変数が見つからない場合はnull）
        //=========================================================================================
        private XCommandArgument GetCommandArgument(string argName) {
            foreach (XCommandArgument arg in m_commandApi.ArgumentList) {
                if (arg.ArgumentName == argName) {
                    return arg;
                }
            }
            return null;
        }

        //=========================================================================================
        // 機　能：APIの引数の解析結果に異常がないか確認する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CheckArgumentList() {
            foreach (XCommandArgument arg in m_commandApi.ArgumentList) {
                if (arg.ArgumentComment == null) {
                    throw NewException("引数" + arg.ArgumentName + "の説明がありません。");
                }
                if (arg.DefaultValue == null) {
                    throw NewException("引数" + arg.ArgumentName + "のデフォルト値がありません。");
                }
                if (arg.ValueRange == null) {
                    throw NewException("引数" + arg.ArgumentName + "の範囲の定義がありません。");
                }
            }
            int argCount = m_commandApi.ArgumentList.Count;
            if (argCount == 0) {
                if (m_commandApi.Comment.IndexOf("{") != -1) {
                    throw NewException("引数がないのにコメント内に文字'{'があります。");
                }
            } else {
                for (int i = 0; i < argCount; i++) {
                    if (m_commandApi.Comment.IndexOf("{" + i + "}") == -1) {
                        throw NewException("機能説明のコメントに引数" + i + "を埋め込む位置{" + i + "}がありません。");
                    }
                }
                int braceCount = StringUtils.GetCharCount(m_commandApi.Comment, '{');
                if (argCount != braceCount) {
                    throw NewException("機能説明のコメント中の'{'の数が引数の数と一致しません。");
                }
            }
        }

        //=========================================================================================
        // 機　能：クラスの開始行を解析する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ParseClassDef() {
            string source = m_sourceLine[m_currentLine].Trim();
            if (source.StartsWith("//")) {
                return;
            }
            string[] tokenList = StringUtils.SeparateBySpace(source);
            int indexClass = -1;
            int indexCommand = -1;
            for (int i = 0; i < tokenList.Length; i++) {
                if (tokenList[i] == "class") {
                    indexClass = i;
                }
                if (tokenList[i].EndsWith("Command") && indexCommand == -1) {
                    indexCommand = i;
                }
            }
            if (indexClass == -1 || indexCommand == -1) {
                throw NewException("コメントの直後にclass XxxxCommandの定義がありません");
            }
            string commandName = tokenList[indexCommand].Substring(0, tokenList[indexCommand].Length - 7);  // "Command"を取り除く
            if (m_commandApi.CommandName != commandName) {
                throw NewException("コメントの定義" + m_commandApi.CommandName + "とソースのclass内のクラス名" + commandName + "が一致しません。");
            }
            m_parseState = ParseState.ClassDefParsed;
        }

        //=========================================================================================
        // 機　能：パラメータセットのメソッドを解析する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ParseSetParameter() {
            string source = m_sourceLine[m_currentLine].Trim();
            if (source.IndexOf("public override void SetParameter(") != -1) {
                m_existSetParameter = true;
                m_parseState = ParseState.Completed;
            }
        }

        //=========================================================================================
        // 列挙子：解析中の状態
        //=========================================================================================
        private enum ParseState {
            Initial,                    // 初期状態
            HeaderClassParsed,          // 「// クラス：」読み込み済み
            HeaderCommentParsed,        // 説明読み込み済み
            HeaderPrototypeParsed,      // プロトタイプ読み込み済み
            HeaderCompleted,            // ヘッダ読み込み完了
            ClassDefParsed,             // クラス定義読み込み済み
            Completed,                  // すべて解析完了
            Ignore,                     // 無視するコマンド
        }
    }
}
