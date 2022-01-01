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
using ShellFiler.FileTask.DataObject;
using ShellFiler.UI;
using ShellFiler.Util;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {
    
    //=========================================================================================
    // クラス：ボリューム情報を取得するエミュレータエンジン
    //=========================================================================================
    public class ShellEngineGetVolumeInfo : IShellEmulatorEngine {
        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        // 一覧を取得するディレクトリ
        private string m_directory;

        // 行内解析の共通処理
        private ResultLineParser m_resultLineParser;

        // ボリューム情報の取得結果
        private VolumeInfo m_resultVolumeInfo;

        // 結果行のすべて
        private StringBuilder m_resultAllLine = new StringBuilder();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent     所有オブジェクト
        // 　　　　[in]connection SSHの接続
        // 　　　　[in]directory  一覧を取得するディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineGetVolumeInfo(ShellCommandEmulator parent, SSHConnection connection, string directory) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            m_directory = directory;
            m_resultLineParser = new ResultLineParser(m_connection, this, m_shellCommandEmulator, m_connection.ShellCommandDictionary.ExpectGetVolumeInfo);
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
            string strCommand = dictionary.GetCommandGetVolumeInfo(m_directory);
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
                m_resultAllLine.Append(line.LineString + "\r\n");

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
            long usedSize = -1;
            long freeSize = -1;
            int percent = -1;

            for (int i = 0; i < parseResult.Count; i++) {
                OSSpecTokenType token = parseResult[i].TokenType;
                object resultValue = parseResult[i].ParsedData;
                if (token == OSSpecTokenType.DfUsed) {
                    usedSize = (long)resultValue;
                } else if (token == OSSpecTokenType.DfFree) {
                    freeSize = (long)resultValue;
                } else if (token == OSSpecTokenType.DfUsedP) {
                    percent = (int)resultValue;
                }
            }

            // 内容の確認/調整
            if (usedSize == -1 || freeSize == -1 || percent == -1) {
                Program.Abort("属性取得の制御エラーです。usedSize={0}, freeSize={1}, percent={2}, line={3}",
                              usedSize, freeSize, percent, line);
            }

            m_resultVolumeInfo = new VolumeInfo();
            m_resultVolumeInfo.UsedDiskSize = usedSize * 1024;
            m_resultVolumeInfo.FreeSize = freeSize * 1024;
            m_resultVolumeInfo.FreeRatio = 100 - percent;
            m_resultVolumeInfo.ClusterSize = 1;
            m_resultVolumeInfo.VolumeLabel = "";
            m_resultVolumeInfo.DriveEtcInfo = m_resultAllLine.ToString();

            return true;
        }

        //=========================================================================================
        // プロパティ：ボリューム情報の取得結果
        //=========================================================================================
        public VolumeInfo ResultVolumeInfo {
            get {
                return m_resultVolumeInfo;
            }
        }
    }
}

