using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
    // クラス：削除コマンドを実行するエミュレータエンジン
    //=========================================================================================
    public class ShellEngineDelete : IShellEmulatorEngine {
        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        // 行内解析の共通処理
        private ResultLineParser m_resultLineParser;
        
        // 実行するコマンドライン
        private string m_commandLine;

        // 削除確認のためのプロンプト文字列の期待値
        private string m_confirmPrompt;

        // 削除確認に入力する応答
        private string m_confirmAnswer;

        // 応答のステータスコード（削除してよいかプロンプトで聞かれたら失敗）
        private FileOperationStatus m_resultStatus = FileOperationStatus.Success;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent      所有オブジェクト
        // 　　　　[in]connection  SSHの接続
        // 　　　　[in]command     実行するコマンドライン
        // 　　　　[in]errorExpect エラー発生を検知するための期待値
        // 　　　　[in]prompt      削除確認のためのプロンプト文字列の期待値
        // 　　　　[in]answer      削除確認のためのプロンプトに対する応答
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineDelete(ShellCommandEmulator parent, SSHConnection connection, string command, List<OSSpecLineExpect> errorExpect, string prompt, string answer) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            m_commandLine = command;
            m_confirmPrompt = prompt;
            m_confirmAnswer = answer;
            ShellCommandDictionary dictionary = m_connection.ShellCommandDictionary;
            m_resultLineParser = new ResultLineParser(m_connection, this, m_shellCommandEmulator, errorExpect);
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
            // 受信バイト列を解析
            ShellEngineError error;
            List<ResultLineParser.ParsedLine> parsedLine;
            bool completed;
            error = m_resultLineParser.OnReceive(buffer, out parsedLine, out completed, out remain);
            if (error.IsFailed) {
                return error;
            }

            // 期待値（エラー）を解析
            if (remain.Length > 0) {
                bool found = CheckPrompt(remain);
                if (found) {
                    remain = new byte[0];

                    string answer = m_confirmAnswer + m_shellCommandEmulator.ReturnCharSend;
                    byte[] sendData = m_connection.AuthenticateSetting.Encoding.GetBytes(answer);
                    m_shellCommandEmulator.WindowsToSshSendData(sendData, 0, sendData.Length);
                    m_resultStatus = FileOperationStatus.Fail;
                }
            }

            if (completed) {
                m_shellCommandEmulator.RemoveEngine(m_resultStatus);
            }
            return ShellEngineError.Success();
        }

        //=========================================================================================
        // 機　能：yes/noのプロンプトの応答を処理する
        // 引　数：[in]buffer   受信データ（行の先頭から）
        // 戻り値：ヒアドキュメントのプロンプトを検出したときtrue
        //=========================================================================================
        private bool CheckPrompt(byte[] buffer) {
            if (!m_resultLineParser.SkippedCommandEcho) {
                return false;
            }
            string receiveData = m_connection.AuthenticateSetting.Encoding.GetString(buffer);
            Regex regex = new Regex(m_confirmPrompt);
            if (regex.IsMatch(receiveData)) {
                return true;
            } else {
                return false;
            }
        }

    }
}
