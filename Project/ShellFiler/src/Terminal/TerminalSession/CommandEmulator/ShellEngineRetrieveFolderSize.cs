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
    // クラス：フォルダ内のファイルサイズを取得するエミュレータエンジン
    //=========================================================================================
    public class ShellEngineRetrieveFolderSize : IShellEmulatorEngine {
        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        // 行内解析の共通処理
        private ResultLineParser m_resultLineParser;

        // 取得するディレクトリ
        private string m_directory;
        
        // 取得条件
        private RetrieveFolderSizeCondition m_condition;

        // 取得結果を返す変数
        private RetrieveFolderSizeResult m_result;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent     所有オブジェクト
        // 　　　　[in]connection SSHの接続
        // 　　　　[in]directory  取得するディレクトリ
        // 　　　　[in]condition  取得条件
        // 　　　　[in]result     取得結果を返す変数
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineRetrieveFolderSize(ShellCommandEmulator parent, SSHConnection connection, string directory, RetrieveFolderSizeCondition condition, RetrieveFolderSizeResult result) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            m_directory = directory;
            m_condition = condition;
            m_result = result;
            m_resultLineParser = new ResultLineParser(m_connection, this, m_shellCommandEmulator, m_connection.ShellCommandDictionary.ExpectRetrieveFolderSize);
        }

        //=========================================================================================
        // 機　能：エンジンの処理を閉じる
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンド処理スレッドで実行される
        //=========================================================================================
        public void CloseEngine() {
        }

        //=========================================================================================
        // 機　能：コマンドラインに送信する文字列を取得する
        // 引　数：なし
        // 戻り値：送信するコマンドライン
        // メ　モ：バックグラウンド処理スレッドで実行される
        //=========================================================================================
        public string GetRequest() {
            ShellCommandDictionary dictionary = m_connection.ShellCommandDictionary;
            string strCommand = dictionary.GetCommandRetrieveFolderSize(m_directory);
            return strCommand;
        }

        //=========================================================================================
        // 機　能：SSHからのデータが受信できたときの処理を行う
        // 引　数：[in]buffer   受信したデータのバッファ
        //         [in]remain   受信データを仮想画面に転送する内容（エラーのときnull）
        // 戻り値：エラーコード
        // メ　モ：ストリームスレッドで実行される
        //=========================================================================================
        public ShellEngineError OnReceive(byte[] buffer, out byte[] remain) {
            ShellEngineError error;
            List<ResultLineParser.ParsedLine> parsedLine;
            bool completed;
            error = m_resultLineParser.OnReceive(buffer, out parsedLine, out completed, out remain);
            if (error.IsFailed) {
                return error;
            }
            for (int i = 0; i < parsedLine.Count; i++) {
                ResultLineParser.ParsedLine line = parsedLine[i];
                bool success = RetrieveFolderSize(line.LineString, line.ParsedToken, line.LineType);
                if (!success) {
                    return ShellEngineError.Fail(FileOperationStatus.Fail, Resources.ShellError_LineParse_OSSPEC, line.LineString);
                }
            }
            if (completed) {
                m_shellCommandEmulator.RemoveEngine(FileOperationStatus.Success);
            }
            return ShellEngineError.Success();
        }

        //=========================================================================================
        // 機　能：SSHからの応答データからファイル情報1件分を取得する
        // 引　数：[in]line            解析対象の行
        // 　　　　[in]parseResult     解析の結果抽出したデータ
        // 　　　　[in]resultLineType  結果行の種類
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        public bool RetrieveFolderSize(string line, List<ResultLineParser.ParsedToken> parseResult, OSSpecLineType resultLineType) {
            // 無意味な行なら次へ
            if ((resultLineType & OSSpecLineType.Comment) != 0) {
                return true;
            }

            // 内容を取得
            long folderSize = -1;
            string folderPath = null;

            for (int i = 0; i < parseResult.Count; i++) {
                OSSpecTokenType token = parseResult[i].TokenType;
                object resultValue = parseResult[i].ParsedData;
                if (token == OSSpecTokenType.DuDirSize) {
                    folderSize = (long)resultValue;
                } else if (token == OSSpecTokenType.DuDirPath) {
                    folderPath = (string)resultValue;
                }
            }

            // 内容の確認/調整
            if (folderSize == -1 || folderPath == null) {
                Program.Abort("サイズ取得の制御エラーです。folderSize={0}, folderPath={1}, line={2}", folderSize, folderPath, line);
            }

            folderPath = GenericFileStringUtils.CompleteDirectoryName(folderPath, "/");
            if (!folderPath.StartsWith(m_directory)) {
                return false;
            }
            string basePath = GenericFileStringUtils.CompleteDirectoryName(GenericFileStringUtils.GetDirectoryName(m_directory, '/'), "/");
            string subPath = folderPath.Substring(basePath.Length);
            if (subPath.Length > 0) {
                subPath = subPath.Substring(0, subPath.Length - 1);
            }
            int depth = StringUtils.CountCharOf(subPath, '/') + 1;
            m_result.AddResult(subPath, depth, folderSize);

            return true;
        }
    }
}
