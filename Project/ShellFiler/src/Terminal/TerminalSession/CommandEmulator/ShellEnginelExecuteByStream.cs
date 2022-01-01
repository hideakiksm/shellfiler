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
using Tamir.SharpSsh.jsch;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {
    
    //=========================================================================================
    // クラス：ShellFiler内部のコマンドを実行し、結果をストリームに格納するエミュレータエンジン
    //=========================================================================================
    public class ShellEngineExecuteByStream : IShellEmulatorEngine {
        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        // 行内解析の共通処理
        private ResultBytesParser m_resultBytesParser;

        // 出力結果のクリーンナップ処理
        private ConsoleResultCleaner m_consoleResultCleaner;

        // 実行するコマンドライン
        private string m_commandLine;

        // 受信したデータの登録先
        private IRetrieveFileDataTarget m_dataTarget;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent      所有オブジェクト
        // 　　　　[in]connection  SSHの接続
        // 　　　　[in]command     実行するコマンドライン
        // 　　　　[in]errorExpect エラー発生を検知するための期待値
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineExecuteByStream(ShellCommandEmulator parent, SSHConnection connection, string current, string command, List<OSSpecLineExpect> errorExpect, IRetrieveFileDataTarget dataTarget) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            m_commandLine = command;
            m_dataTarget = dataTarget;
            ShellCommandDictionary dictionary = m_connection.ShellCommandDictionary;
            m_resultBytesParser = new ResultBytesParser(m_connection, this, m_shellCommandEmulator, errorExpect);
            m_consoleResultCleaner = new ConsoleResultCleaner(dictionary.ResultCrLfConvert);
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
            return m_commandLine;
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
            BufferRange receivedData;
            bool completed;
            error = m_resultBytesParser.OnReceive(buffer, out receivedData, out completed, out remain);
            if (error.IsFailed) {
                m_dataTarget.AddCompleted(RetrieveDataLoadStatus.Failed, error.ErrorMessage);
                return error;
            }

            if (receivedData.Length > 0) {
                byte[] cleaned = m_consoleResultCleaner.CleanReceivedData(buffer, receivedData.Offset, receivedData.Length, completed);
                m_dataTarget.AddData(cleaned, 0, cleaned.Length);
                m_dataTarget.FireEvent(false);
            }

            if (completed) {
                m_dataTarget.AddCompleted(RetrieveDataLoadStatus.CompletedAll, null);
                m_shellCommandEmulator.RemoveEngine(FileOperationStatus.Success);
            }
            return ShellEngineError.Success();
        }
    }
}
