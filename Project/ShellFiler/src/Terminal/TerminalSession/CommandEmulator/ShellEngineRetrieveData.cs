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
    // クラス：コマンド実行によりコンソールからの実行結果を取得するエミュレータエンジン
    //=========================================================================================
    public class ShellEngineRetrieveData : IShellEmulatorEngine {
        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        // バイト列取得の共通処理
        private ResultBytesParser m_resultBytesParser;

        // 出力結果のクリーンナップ処理
        private ConsoleResultCleaner m_consoleResultCleaner;

        // 実行するコマンドライン
        private string m_commandLine;

        // 結果を格納するメモリストリーム
        private MemoryStream m_resultStream;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent      所有オブジェクト
        // 　　　　[in]connection  SSHの接続
        // 　　　　[in]command     実行するコマンドライン
        // 　　　　[in]errorExpect エラー発生を検知するための期待値
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineRetrieveData(ShellCommandEmulator parent, SSHConnection connection, string command, List<OSSpecLineExpect> errorExpect) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            m_commandLine = command;
            ShellCommandDictionary dictionary = m_connection.ShellCommandDictionary;
            m_resultBytesParser = new ResultBytesParser(m_connection, this, m_shellCommandEmulator, errorExpect);
            m_consoleResultCleaner = new ConsoleResultCleaner(dictionary.ResultCrLfConvert);
            m_resultStream = new MemoryStream();
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
                return error;
            }

            if (receivedData.Length > 0) {
                byte[] cleaned = m_consoleResultCleaner.CleanReceivedData(buffer, receivedData.Offset, receivedData.Length, completed);
                m_resultStream.Write(cleaned, 0, cleaned.Length);
            }

            if (completed) {
                m_shellCommandEmulator.RemoveEngine(FileOperationStatus.Success);
            }
            return ShellEngineError.Success();
        }

        //=========================================================================================
        // プロパティ：結果のバイト列
        //=========================================================================================
        public byte[] ResultBytes {
            get {
                byte[] buffer = m_resultStream.ToArray();
                return buffer;
            }
        }
    }
}
