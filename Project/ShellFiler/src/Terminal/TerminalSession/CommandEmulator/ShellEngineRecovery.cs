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
using ShellFiler.FileSystem.SFTP;
using ShellFiler.UI;
using ShellFiler.Util;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {
    
    //=========================================================================================
    // クラス：内部の矛盾発生時にリカバリ処理を行うエンジン
    //=========================================================================================
    public class ShellEngineRecovery : IShellEmulatorEngine {
        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent   所有オブジェクト
        // 　　　　[in]connection SSHの接続
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineRecovery(ShellCommandEmulator parent, SSHConnection connection) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            ResetPrompt();
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
        // 機　能：プロンプト文字列をリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetPrompt() {
            m_shellCommandEmulator.ResetPrompt();
        }

        //=========================================================================================
        // 機　能：コマンドラインに送信する文字列を取得する
        // 引　数：なし
        // 戻り値：送信するコマンドライン
        // メ　モ：バックグラウンド処理スレッドで実行される
        //=========================================================================================
        public string GetRequest() {
            ShellCommandDictionary dictionary = m_connection.ShellCommandDictionary;
            string strCommand = dictionary.GetBreakCode() + m_shellCommandEmulator.ReturnCharSend + dictionary.GetEnterCode() + dictionary.GetCommandChangePrompt(m_shellCommandEmulator.PromptStringSend);
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
            Encoding encoding = m_connection.AuthenticateSetting.Encoding;
            byte[] prompt = encoding.GetBytes(m_shellCommandEmulator.PromptString);
            int promptLength, nextCheckIndex;
            string userServer;
            bool found = CommandReplyChecker.CheckPrompt(encoding, buffer, 0, buffer.Length, prompt, true, out userServer, out promptLength, out nextCheckIndex);
            if (!found) {
                 m_shellCommandEmulator.ShellChannel.ActiveUserServer = userServer;
                remain = buffer;
                return ShellEngineError.Success();
            }
            byte[] nextBuffer = ArrayUtils.CreateCleanedBuffer<byte>(buffer, nextCheckIndex, buffer.Length - nextCheckIndex);
            m_shellCommandEmulator.RemoveEngine(FileOperationStatus.Success);
            remain = nextBuffer;
            return ShellEngineError.Success();
        }
    }
}
