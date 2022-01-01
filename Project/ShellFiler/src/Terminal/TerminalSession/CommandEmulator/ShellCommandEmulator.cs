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
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.UI;
using ShellFiler.Util;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {
    
    //=========================================================================================
    // クラス：Shellの入力のエミュレーションによるコマンドのエミュレータ（TerminalShellChannelごと）
    //=========================================================================================
    public class ShellCommandEmulator {
        // 入出力対象のシェル
        private TerminalShellChannel m_shellChannel;
        
        // エミュレータエンジンの待ち行列（先頭が次に処理するエンジン、0件のときはコンソール）
        private List<EmulatorEngineQueue> m_emulatorEngineQueue = new List<EmulatorEngineQueue>();

        // チャネルが閉じられたことを表すイベント
        private ManualResetEvent m_channelClosedEvent = new ManualResetEvent(false);

        // 直前の解析で残ったバイト列（残っていないとき長さ0の配列）
        private byte[] m_prevBuffer = new byte[0];

        // 使用中のプロンプト識別用シーケンス
        private long m_promptSequence;

        // 回復不可能な状態になったときtrue
        private bool m_recoverFailed = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]channel   入出力対象のシェル
        // 戻り値：なし
        //=========================================================================================
        public ShellCommandEmulator(TerminalShellChannel channel) {
            m_shellChannel = channel;
            ResetPrompt();
            if (m_shellChannel.State == TerminalShellChannel.ChannelState.Closed) {
                m_channelClosedEvent.Set();
            }
        }
        
        //=========================================================================================
        // 機　能：待ち行列にエンジンを登録する
        // 引　数：[in]engine  登録するエンジン
        // 戻り値：登録したキュー内のセル（回復不可能な状態で登録できないときはnull）
        // メ　モ：各処理スレッドで実行する
        //=========================================================================================
        private EmulatorEngineQueue SetEngine(IShellEmulatorEngine engine) {
            EmulatorEngineQueue queueCell;
            lock (this) {
                if (m_recoverFailed) {
                    return null;
                }
                // 待ち行列の最後に登録
                queueCell = new EmulatorEngineQueue(engine);
                m_emulatorEngineQueue.Add(queueCell);
                
                // 初めの1件ならアクティブ状態に変更
                if (m_emulatorEngineQueue.Count == 1) {
                    queueCell.MyTurnEvent.Set();
                }
            }
            return queueCell;
        }

        //=========================================================================================
        // 機　能：待ち行列の先頭の実行エンジンを削除する
        // 引　数：[in]status   エンジンのステータス
        // 戻り値：なし
        // メ　モ：エンジンの実行スレッドで実行する
        //=========================================================================================
        public void RemoveEngine(FileOperationStatus status) {
            lock (this) {
                // 待ち行列の先頭から削除
                if (m_emulatorEngineQueue.Count > 0) {
                    m_emulatorEngineQueue[0].CompletedEvent.Set();
                    m_emulatorEngineQueue[0].Status = status;
                    m_emulatorEngineQueue.RemoveAt(0);
                }

                // 残っている先頭をアクティブ状態に変更
                if (m_emulatorEngineQueue.Count >= 1) {
                    m_emulatorEngineQueue[0].MyTurnEvent.Set();
                }
            }
        }

        //=========================================================================================
        // 機　能：待ち行列の先頭の実行エンジンをリカバリエンジンに設定する
        // 引　数：[in]engine        設定するリカバリエンジン
        // 　　　　[in]failedStatus  失敗時のステータス
        // 戻り値：登録したリカバリエンジンのキュー内のセル
        // メ　モ：エンジンの実行スレッドで実行する
        //=========================================================================================
        private EmulatorEngineQueue SetRecoveryEngine(ShellEngineRecovery engine, FileOperationStatus failedStatus) {
            EmulatorEngineQueue queueCell;
            lock (this) {
                if (m_emulatorEngineQueue[0].EmulatorEngine is ShellEngineRecovery) {
                    // 2重での失敗は回復不可能
                    for (int i = 0; i < m_emulatorEngineQueue.Count; i++) {
                        m_emulatorEngineQueue[i].MyTurnEvent.Set();
                        m_emulatorEngineQueue[i].CompletedEvent.Set();
                        m_emulatorEngineQueue[i].Status = FileOperationStatus.Fail;
                    }
                    m_emulatorEngineQueue.Clear();
                    m_recoverFailed = true;
                    queueCell = null;
                } else {
                    // 待ち行列の先頭と入れ替えて登録
                    queueCell = new EmulatorEngineQueue(engine);
                    m_emulatorEngineQueue[0].CompletedEvent.Set();
                    m_emulatorEngineQueue[0].Status = failedStatus;
                    
                    // リカバリエンジンをアクティブ化
                    m_emulatorEngineQueue[0] = queueCell;
                    queueCell.MyTurnEvent.Set();
                }
            }
            return queueCell;
        }

        //=========================================================================================
        // 機　能：ターミナルが接続されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：ストリームスレッドで実行される
        //=========================================================================================
        public void OnConnect() {
            if (m_shellChannel.ForFileSystem) {
                // ファイルシステム接続のときはエンジンを設定
                IShellEmulatorEngine engine = new ShellEngineInitializeConsole(this, m_shellChannel.ParentConnection);
                EmulatorEngineQueue queueCell = SetEngine(engine);      // ここは必ず成功する
                string strRequest = engine.GetRequest();
                Encoding encoding = m_shellChannel.ParentConnection.AuthenticateSetting.Encoding;
                byte[] request = encoding.GetBytes(strRequest + ReturnCharSend);

                WindowsToSshSendData(request, 0, request.Length);
                // 待機は他のコマンドのExecute()で実施
            }
        }

        //=========================================================================================
        // 機　能：ターミナルが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：ストリームスレッドで実行される
        //=========================================================================================
        public void OnCloseStream() {
            m_channelClosedEvent.Set();
        }

        //=========================================================================================
        // 機　能：WindowsからSSH接続に向けてメッセージを送信する
        // 引　数：[in]buffer   送信するデータのバッファ
        // 　　　　[in]offset   送信データの開始オフセット
        // 　　　　[in]length   送信するデータの長さ
        // 戻り値：送信できたときtrue、すでに閉じられているときfalse
        // メ　モ：UIスレッドで実行される（アップロード処理は受信スレッド）
        //=========================================================================================
        public bool WindowsToSshSendData(byte[] buffer, int offset, int length) {
            return m_shellChannel.WindowsToSshSendData(buffer, offset, length);
        }

        //=========================================================================================
        // 機　能：SSHからのデータが受信できたときの処理を行う
        // 引　数：[in]buffer   受信したデータのバッファ
        // 戻り値：受信データを仮想画面に転送する内容（転送する内容がないときnull）
        // メ　モ：ストリームスレッドで実行される
        //=========================================================================================
        public byte[] OnReceive(byte[] buffer) {
            // バッファを結合
            buffer = ArrayUtils.AppendBuffer<byte>(m_prevBuffer, 0, m_prevBuffer.Length, buffer, 0, buffer.Length);

            while (true) {
                // 待ち行列の先頭のエンジンを取得
                EmulatorEngineQueue queueCell;
                lock (this) {
                    if (m_emulatorEngineQueue.Count == 0) {
                        queueCell = null;
                    } else {
                        queueCell = m_emulatorEngineQueue[0];
                    }
                }

                // 待機中のエンジンがなければコンソールとしてそのまま実行
                if (queueCell == null) {
                    m_prevBuffer = new byte[0];
                    return buffer;
                }

                // 受信処理を実行
                // 処理中にm_emulatorEngineが変化することがある
                byte[] nextBuffer;
                ShellEngineError error = queueCell.EmulatorEngine.OnReceive(buffer, out nextBuffer);
                buffer = nextBuffer;
                if (error.IsFailed) {
                    // コンソールが混乱：回復
                    m_prevBuffer = new byte[0];
                    RecoverEngineError(error);
                    return null;
                }

                // 同じエンジンなら次回を待つ
                if (m_emulatorEngineQueue.Count > 0 && queueCell == m_emulatorEngineQueue[0]) {
                    m_prevBuffer = buffer;
                    return null;
                }
            }
        }

        //=========================================================================================
        // 機　能：シェルファイルシステムでの処理を実行する
        // 引　数：[in]context   リクエストコンテキスト
        // 　　　　[in]engine    実行する処理のエンジン
        // 戻り値：ステータス
        // メ　モ：各処理スレッドで実行し、待ち行列への登録後、エンジンでの処理が終わるまでイベントで待機する
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext context, IShellEmulatorEngine engine) {
            try {
                // 待ち行列の最後に登録
                EmulatorEngineQueue queueCell = SetEngine(engine);
                if (queueCell == null) {
                    return FileOperationStatus.Fail;
                }
                if (m_shellChannel.State == TerminalShellChannel.ChannelState.Closed || m_shellChannel.State == TerminalShellChannel.ChannelState.Error) {
                    m_channelClosedEvent.Set();
                }

                // 処理開始イベントの発生を待つ
                int index;
                EventWaitHandle cancelEvent = context.CancelEvent;
                EventWaitHandle myTurnEvent = queueCell.MyTurnEvent;
                WaitHandle[] waitEventListBegin = { cancelEvent, myTurnEvent, m_channelClosedEvent };
                index = WaitHandle.WaitAny(waitEventListBegin);
                if (index == 2 || m_channelClosedEvent.WaitOne(0)) {
                    return FileOperationStatus.FailedConnect;               // チャネルクローズ
                } else if (index == 0 || cancelEvent.WaitOne(0)) {
                    return FileOperationStatus.Canceled;
                }

                // コマンドを送信
                SendCommand(engine);

                // 処理の完了を待つ
                EventWaitHandle completedEvent = queueCell.CompletedEvent;
                WaitHandle[] waitEventListEnd = { cancelEvent, completedEvent, m_channelClosedEvent };
                index = WaitHandle.WaitAny(waitEventListEnd);
                if (index == 2 || m_channelClosedEvent.WaitOne(0)) {
                    return FileOperationStatus.FailedConnect;               // チャネルクローズ
                } else if (index == 0 || cancelEvent.WaitOne(0)) {
                    return FileOperationStatus.Canceled;
                }

                if (m_shellChannel.ParentConnection.Closed) {
                    return FileOperationStatus.Canceled;
                }

                return queueCell.Status;
            } finally {
                engine.CloseEngine();
            }
        }

        //=========================================================================================
        // 機　能：エンジンからのコマンドを送信する
        // 引　数：[in]engine    実行する処理のエンジン
        // 戻り値：なし
        //=========================================================================================
        private void SendCommand(IShellEmulatorEngine engine) {
            string strRequest = engine.GetRequest();
            if (strRequest != "") {
                Encoding encoding = m_shellChannel.ParentConnection.AuthenticateSetting.Encoding;
                byte[] request = encoding.GetBytes(strRequest + ReturnCharSend);
                m_shellChannel.WindowsToSshSendData(request, 0, request.Length);
            }
        }

        //=========================================================================================
        // 機　能：処理エンジンでのエラーからリカバリする
        // 引　数：[in]error  発生したエラーの詳細
        // 戻り値：なし
        //=========================================================================================
        private void RecoverEngineError(ShellEngineError error) {
            // ログ出力
            if (error.ErrorMessage != null) {
                Program.LogWindow.RegistLogLineHelper(error.ErrorMessage);
                Program.LogWindow.RegistLogLineHelper(error.ErrorLine);
            }

            // 回復処理用のエンジンを登録
            ShellEngineRecovery engine = new ShellEngineRecovery(this, m_shellChannel.ParentConnection);
            EmulatorEngineQueue engineQueue = SetRecoveryEngine(engine, error.ErrorCode);
            SendCommand(engineQueue.EmulatorEngine);
        }

        //=========================================================================================
        // 機　能：プロンプト文字列をリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetPrompt() {
            m_promptSequence = Math.Max(m_promptSequence + 1, DateTime.Now.Ticks);
        }

        //=========================================================================================
        // プロパティ：入出力対象のシェル
        //=========================================================================================
        public TerminalShellChannel ShellChannel {
            get {
                return m_shellChannel;
            }
        }

        //=========================================================================================
        // プロパティ：エミュレータエンジンが使用するプロンプトの受信期待値
        //=========================================================================================
        public string PromptString {
            get {
                const string PROMPT_STR = "[ShellFiler{0}]$";
                string prompt = string.Format(PROMPT_STR, string.Format("{0:X16}", m_promptSequence));
                return prompt;
            }
        }

        //=========================================================================================
        // プロパティ：エミュレータエンジンが使用するプロンプトの送信値
        //=========================================================================================
        public string PromptStringSend {
            get {
                const string PROMPT_STR = "\t{0}[ShellFiler{1}]$";
                string userServer = m_shellChannel.ParentConnection.ShellCommandDictionary.ValuePromptUserServer;
                string prompt = string.Format(PROMPT_STR, userServer, string.Format("{0:X16}", m_promptSequence));
                return prompt;
            }
        }

        //=========================================================================================
        // プロパティ：送信時に使用する改行コード
        //=========================================================================================
        public string ReturnCharSend {
            get {
                return "\n";
            }
        }

        //=========================================================================================
        // プロパティ：受信時に使用する改行コード
        //=========================================================================================
        public string ReturnCharReceive {
            get {
                return "\r\n";
            }
        }

        //=========================================================================================
        // クラス：エミュレータエンジンの待ち行列
        //=========================================================================================
        private class EmulatorEngineQueue {
            // エミュレータエンジン
            public IShellEmulatorEngine EmulatorEngine;

            // 待ち行列の先頭になったことを表すイベント
            public ManualResetEvent MyTurnEvent = new ManualResetEvent(false);

            // 処理が完了したことを表すイベント
            public ManualResetEvent CompletedEvent = new ManualResetEvent(false);

            // 実行完了時のステータス
            public FileOperationStatus Status = FileOperationStatus.Success;

            public EmulatorEngineQueue(IShellEmulatorEngine emulatorEngine) {
                EmulatorEngine = emulatorEngine;
            }
        }
    }
}
