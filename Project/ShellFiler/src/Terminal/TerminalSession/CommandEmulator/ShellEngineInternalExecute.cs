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
    // クラス：ShellFiler内部のコマンドを実行するエミュレータエンジン
    //=========================================================================================
    public class ShellEngineInternalExecute : IShellEmulatorEngine {
        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        // 行内解析の共通処理
        private ResultLineParser m_resultLineParser;
        
        // 実行するコマンドライン
        private string m_commandLine;

        // データ処理のためのフック（未登録のときnull）
        private RemainDataHookDelegate m_remainDataHook = null;

        //=========================================================================================
        // 機　能：SSHからのデータが受信できたときの処理を行う
        // 引　数：[in]parsedLine      行として認識した情報
        // 　　　　[in,out]completed   コマンドを完了したときtrue
        //         [in,out]remain      行になっていない受信内容（受信データを仮想画面に転送する内容）
        // 戻り値：エラーコード
        // メ　モ：ストリームスレッドで実行される
        //=========================================================================================
        public delegate ShellEngineError RemainDataHookDelegate(List<ResultLineParser.ParsedLine> parsedLine, ref bool completed, ref byte[] remain);

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent      所有オブジェクト
        // 　　　　[in]connection  SSHの接続
        // 　　　　[in]command     実行するコマンドライン
        // 　　　　[in]errorExpect エラー発生を検知するための期待値
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineInternalExecute(ShellCommandEmulator parent, SSHConnection connection, string command, List<OSSpecLineExpect> errorExpect) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            m_commandLine = command;
            m_resultLineParser = new ResultLineParser(m_connection, this, m_shellCommandEmulator, errorExpect);
        }

        //=========================================================================================
        // 機　能：データフックをセットする
        // 引　数：[in]hoook   フックプロシージャ
        // 戻り値：なし
        //=========================================================================================
        public void SetDataHook(RemainDataHookDelegate hook) {
            m_remainDataHook = hook;
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
            List<ResultLineParser.ParsedLine> parsedLine;
            bool completed;
            error = m_resultLineParser.OnReceive(buffer, out parsedLine, out completed, out remain);
            if (error.IsFailed) {
                return error;
            }
            if (m_remainDataHook != null) {
                error = m_remainDataHook(parsedLine, ref completed, ref remain);
            }
            if (completed) {
                m_shellCommandEmulator.RemoveEngine(FileOperationStatus.Success);
            }
            return ShellEngineError.Success();
        }

        //=========================================================================================
        // 機　能：SSHにデータを送信する
        // 引　数：[in]buffer   送信するデータ
        // 戻り値：なし
        //=========================================================================================
        public void SendData(byte[] buffer) {
            m_shellCommandEmulator.WindowsToSshSendData(buffer, 0, buffer.Length);
        }
    }
}
