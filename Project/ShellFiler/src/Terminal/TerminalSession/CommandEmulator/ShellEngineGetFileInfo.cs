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
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Shell;
using ShellFiler.UI;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {
    
    //=========================================================================================
    // クラス：ファイル情報を取得するエミュレータエンジン
    //=========================================================================================
    public class ShellEngineGetFileInfo : IShellEmulatorEngine {
        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        // 行内解析の共通処理
        private ResultLineParser m_resultLineParser;

        // 属性の取得結果（見つからなかったときnull）
        private ShellFile m_resultFileInfo = null;

        // 指定されたファイルが見つからなかったときtrue
        private bool m_fileNotFound = false;

        // 取得対象ファイル名のローカルパス
        private string m_localPath;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent     所有オブジェクト
        // 　　　　[in]connection SSHの接続
        // 　　　　[in]localPath  取得対象ファイル名のローカルパス
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineGetFileInfo(ShellCommandEmulator parent, SSHConnection connection, string localPath) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            m_localPath = localPath;

            m_resultLineParser = new ResultLineParser(m_connection, this, m_shellCommandEmulator, m_connection.ShellCommandDictionary.ExpectGetFileInfo);
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
            string strCommand = dictionary.GetCommandGetFileInfo(m_localPath);
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
                bool success = RetrieveResult(line);
                if (!success) {
                    return ShellEngineError.Fail(FileOperationStatus.Fail, Resources.ShellError_LineParse_OSSPEC, line.LineString);
                }
            }
            if (completed) {
                if (m_fileNotFound) {
                    m_resultFileInfo = null;
                } else if (m_resultFileInfo == null) {
                    return ShellEngineError.Fail(FileOperationStatus.Fail, Resources.ShellError_EmptyResult_OSSPEC, GetRequest());
                }
                m_shellCommandEmulator.RemoveEngine(FileOperationStatus.Success);
            }
            return ShellEngineError.Success();
        }

        //=========================================================================================
        // 機　能：SSHからのデータが受信できたときの処理を行う
        // 引　数：[in]line   解析対象の行
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool RetrieveResult(ResultLineParser.ParsedLine line) {
            // 無意味な行なら次へ
            if ((line.LineType & OSSpecLineType.Comment) != 0) {
                return true;
            }

            // ファイルが存在しているか確認
            List<ResultLineParser.ParsedToken> tokenList = line.ParsedToken;
            for (int i = 0; i < tokenList.Count; i++) {
                if (tokenList[i].TokenType == OSSpecTokenType.StatNotFound) {
                    m_fileNotFound = true;
                    return true;
                }
            }

            // statの実行結果を取得
            ShellFile fileInfo;
            bool success = ShellEngineGetFileList.RetrieveFileInfo(line.LineString, line.ParsedToken, line.LineType, out fileInfo);
            if (success == false || fileInfo == null) {
                return false;
            }
            m_resultFileInfo = fileInfo;
            return true;
        }

        //=========================================================================================
        // プロパティ：属性の取得結果（見つからなかったときnull）
        //=========================================================================================
        public ShellFile ResultFileInfo {
            get {
                return m_resultFileInfo;
            }
        }
    }
}
