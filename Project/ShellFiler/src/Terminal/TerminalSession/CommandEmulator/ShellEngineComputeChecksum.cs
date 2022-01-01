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
    // クラス：ファイルのチェックサムを取得するエミュレータエンジン
    //=========================================================================================
    public class ShellEngineComputeChecksum : IShellEmulatorEngine {
        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        // 行内解析の共通処理
        private ResultLineParser m_resultLineParser;

        // 取得対象ファイル名のローカルパス
        private string m_localPath;

        // ファイルのチェックサム（cksum CRC32）
        private uint m_cksumCrc32;

        // ファイルサイズ
        private long m_fileSize;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent     所有オブジェクト
        // 　　　　[in]connection SSHの接続
        // 　　　　[in]localPath  取得対象ファイル名のローカルパス
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineComputeChecksum(ShellCommandEmulator parent, SSHConnection connection, string localPath) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            m_localPath = localPath;

            m_resultLineParser = new ResultLineParser(m_connection, this, m_shellCommandEmulator, m_connection.ShellCommandDictionary.ExpectComputeChecksum);
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
            string strCommand = dictionary.GetCommandComputeChecksum(m_localPath);
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
                bool success = RetrieveResult(line.LineString, line.ParsedToken, line.LineType);
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
        // 機　能：SSHからのデータが受信できたときの処理を行う
        // 引　数：[in]line            解析対象の行
        // 　　　　[in]parseResult     解析の結果抽出したデータ
        // 　　　　[in]resultLineType  結果行の種類
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool RetrieveResult(string line, List<ResultLineParser.ParsedToken> parseResult, OSSpecLineType resultLineType) {
            // 無意味な行なら次へ
            if ((resultLineType & OSSpecLineType.Comment) != 0) {
                return true;
            }

            // 内容を取得
            bool crcChecked = false;
            m_fileSize = -1;

            for (int i = 0; i < parseResult.Count; i++) {
                OSSpecTokenType token = parseResult[i].TokenType;
                object resultValue = parseResult[i].ParsedData;
                if (token == OSSpecTokenType.CksumCRC) {
                    m_cksumCrc32 = (uint)resultValue;
                    crcChecked = true;
                } else if (token == OSSpecTokenType.CksumSize) {
                    m_fileSize = (long)resultValue;
                }
            }

            // 内容の確認/調整
            if (!crcChecked || m_fileSize == -1) {
                Program.Abort("属性取得の制御エラーです。crcChecked={0}, m_fileSize={1}, line={2}",
                              crcChecked, m_fileSize, line);
            }

            return true;
        }

        //=========================================================================================
        // プロパティ：ファイルのチェックサム（cksum CRC32）
        //=========================================================================================
        public uint CksumCrc32 {
            get {
                return m_cksumCrc32;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルサイズ
        //=========================================================================================
        public long FileSize {
            get {
                return m_fileSize;
            }
        }
    }
}
