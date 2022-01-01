using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Text;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Terminal.TerminalSession.CommandEmulator;
using ShellFiler.UI;
using ShellFiler.Util;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.Terminal.TerminalSession {
    
    //=========================================================================================
    // クラス：ターミナル用のshellチャネルのラッパ
    //=========================================================================================
    public class TerminalShellChannel {
        // 1つのコネクションで開けるターミナルの数
        public const int MAX_CONSOLE_COUNT = 30;

        // ID
        private TerminalShellChannelID m_terminalShellChannelId;

        // 親となるShellFilerのコネクション
        private SSHConnection m_parentConnection;

        // SharpSSHのshellチャネル
        private ChannelShell m_shellChannel;

        // チャネルが接続された時刻
        private DateTime m_channelStartTime;

        // Windows→SSHのデータ保存領域
        private TerminalWindowsToSSHStream m_windowsToSSHStream = null;

        // SSH→Windowsのデータ保存領域
        private TerminalSSHToWindowsStream m_sshToWindowsStream = null;

        // ターミナルデータを扱う仮想画面
        private ConsoleScreen m_consoleScreen;

        // チャネルの状態
        private ChannelState m_channelState = ChannelState.Initial;

        // ファイルシステム用の接続のときtrue
        private bool m_forFileSystem;

        // コマンドのエミュレータ
        private ShellCommandEmulator m_commandEmulator;

        // コンソールを操作している「ユーザー@サーバー」（初期化完了まではnull）
        private string m_userServer = null;

        // ホームディレクトリ
        private string m_homeDirectory = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent         親となるShellFilerのコネクション
        // 　　　　[in]console        ターミナルデータを扱う仮想画面
        // 　　　　[in]forFileSystem  ファイルシステム用の接続のときtrue
        // 戻り値：なし
        //=========================================================================================
        public TerminalShellChannel(SSHConnection parent, ConsoleScreen console, bool forFileSystem) {
            m_terminalShellChannelId = TerminalShellChannelID.NextId();
            m_parentConnection = parent;
            m_consoleScreen = console;
            m_forFileSystem = forFileSystem;
            m_commandEmulator = new ShellCommandEmulator(this);
            m_channelStartTime = DateTime.Now;
        }
        
        //=========================================================================================
        // 機　能：コンソールのサイズを設定する
        // 引　数：[in]charSize   文字単位のサイズ
        // 　　　　[in]pixelSize  ピクセル単位のサイズ
        // 戻り値：なし
        // メ　モ：UIスレッドで実行される
        //=========================================================================================
        public void SetPtySize(Size charSize, Size pixelSize) {
            if (m_channelState == ChannelState.Connected) {
                try {
                    m_shellChannel.setPtySize(charSize.Width, charSize.Height, pixelSize.Width, pixelSize.Height);
                } catch (JSchException) {
                    // ここでのエラーは無視
                }
            }
        }

        //=========================================================================================
        // 機　能：ターミナルチャネルを接続する
        // 引　数：[out]errorDetail  エラー発生時、詳細情報を返す変数（エラーではないときnullを返す）
        // 戻り値：ステータス
        // メ　モ：通信スレッドで実行される
        //=========================================================================================
        public FileOperationStatus Connect(out string errorDetail) {
            errorDetail = null;
            m_windowsToSSHStream = new TerminalWindowsToSSHStream(this, m_parentConnection);
            m_sshToWindowsStream = new TerminalSSHToWindowsStream(this, m_parentConnection);

            try {
                m_shellChannel = (ChannelShell)(m_parentConnection.SSHSession.openChannel("shell"));
                m_shellChannel.setInputStream(m_windowsToSSHStream);
                m_shellChannel.setOutputStream(m_sshToWindowsStream);
                m_shellChannel.connect();
                m_shellChannel.setPtySize(m_consoleScreen.CharSize.Width, m_consoleScreen.CharSize.Height, m_consoleScreen.PixelSize.Width,  m_consoleScreen.PixelSize.Height);
                m_consoleScreen.ShellChannel = this;
                if (m_channelState == ChannelState.Initial) {
                    m_channelState = ChannelState.Connected;
                }
            } catch (JSchException e) {
                errorDetail = e.Message;
                return FileOperationStatus.Fail;
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：チャネルを閉じる
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：通信スレッドで実行される
        //=========================================================================================
        public void Close() {
            lock (this) {
                m_consoleScreen.ShellChannel = null;
                if (m_windowsToSSHStream != null) {
                    m_windowsToSSHStream.ConnectionClose();
                    m_windowsToSSHStream = null;
                }
                if (m_sshToWindowsStream != null) {
                    m_sshToWindowsStream.ConnectionClose();
                    m_sshToWindowsStream = null;
                }
                m_shellChannel.disconnect();
                m_shellChannel = null;
            }
            FireOnSSHClose();
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
            lock (this) {
                if (m_windowsToSSHStream == null) {
                    return false;
                }
                m_windowsToSSHStream.WinUIAddData(buffer, offset, length);
                return true;
            }
        }

        //=========================================================================================
        // 機　能：接続が完了したことを通知する
        // 引　数：[in]status       接続のステータス
        // 　　　　[in]errorDetail  エラーが発生したとき、その詳細情報（エラーではないときnull）
        // 戻り値：送信できたときtrue、すでに閉じられているときfalse
        // メ　モ：通信スレッドで実行される
        //=========================================================================================
        public void NotifyConnect(FileOperationStatus status, string errorDetail) {
            if (status.Succeeded) {
                m_commandEmulator.OnConnect();
            } else {
                m_channelState = ChannelState.Error;
            }
        }

        //=========================================================================================
        // 機　能：SSHからのデータが受信できたときの処理を行う
        // 引　数：[in]buffer   受信したデータのバッファ
        // 　　　　[in]offset   受信データの開始オフセット
        // 　　　　[in]length   受信したデータの長さ
        // 戻り値：なし
        // メ　モ：ストリームスレッドで実行される
        //=========================================================================================
        public void OnSSHAddData(byte[] buffer, int offset, int length) {
            // bufferはSSH実装で再利用されるため、別スレッドに渡す前にコピー
            byte[] forConsole;
            try {
                byte[] clone = ArrayUtils.CreateCleanedBuffer<byte>(buffer, offset, length);
                forConsole = m_commandEmulator.OnReceive(clone);
                if (forConsole == null) {
                    return;
                }
            } catch (Exception e) {
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionThread, "OnSSHAddData");
                return;
            }
            Program.MainWindow.BeginInvoke(new OnSSHAddDataDelegate(OnSSHAddDataUI), forConsole, 0, forConsole.Length);
        }
        delegate void OnSSHAddDataDelegate(byte[] buffer, int offset, int length);
        private void OnSSHAddDataUI(byte[] buffer, int offset, int length) {
            try {
                m_consoleScreen.SSHOnDataReceived(buffer, offset, length);
            } catch (Exception e) {
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionUI, "OnSSHAddDataUI");
            }
        }

        //=========================================================================================
        // 機　能：ターミナルが閉じられたときの処理を行う
        // 引　数：[in]sshToWin  SSH→Windowsのストリームのときtrue、Windows→SSHのときfalse
        // 戻り値：なし
        // メ　モ：ストリームスレッドで実行される
        //=========================================================================================
        public void OnCloseStream(bool sshToWin) {
            if (!sshToWin) {
                return;
            }
            m_channelState = ChannelState.Closed;
            m_commandEmulator.OnCloseStream();
            Program.MainWindow.BeginInvoke(new OnCloseStreamDelegate(OnCloseStreamUI));
        }
        delegate void OnCloseStreamDelegate();
        private void OnCloseStreamUI() {
            try {
                m_parentConnection.RemoveTerminalChannel(this);
                OnSSHCloseUI();
            } catch (Exception e) {
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionUI, "OnCloseStreamUI");
            }
        }

        //=========================================================================================
        // 機　能：SSHの接続が閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：通信スレッドで実行される
        //=========================================================================================
        public void FireOnSSHClose() {
            Program.MainWindow.BeginInvoke(new OnSSHCloseDelegate(OnSSHCloseUI));
        }
        delegate void OnSSHCloseDelegate();
        private void OnSSHCloseUI() {       // privateでも呼び出す
            try {
                m_consoleScreen.SSHOnClose();
            } catch (Exception e) {
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionUI, "OnSSHClose");
            }
        }

        //=========================================================================================
        // プロパティ：ID
        //=========================================================================================
        public TerminalShellChannelID ID {
            get {
                return m_terminalShellChannelId;
            }
        }

        //=========================================================================================
        // プロパティ：チャネルが接続された時刻
        //=========================================================================================
        public DateTime ChannelStartTime {
            get {
                return m_channelStartTime;
            }
        }

        //=========================================================================================
        // プロパティ：親となるShellFilerのコネクション
        //=========================================================================================
        public SSHConnection ParentConnection {
            get {
                return m_parentConnection;
            }
        }

        //=========================================================================================
        // プロパティ：ターミナルデータを扱う仮想画面
        //=========================================================================================
        public ConsoleScreen ConsoleScreen {
            get {
                return m_consoleScreen;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイルシステム用の接続のときtrue
        //=========================================================================================
        public bool ForFileSystem {
            get {
                return m_forFileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのエミュレータ
        //=========================================================================================
        public ShellCommandEmulator ShellCommandEmulator {
            get {
                return m_commandEmulator;
            }
        }

        //=========================================================================================
        // プロパティ：チャネルの状態
        //=========================================================================================
        public ChannelState State {
            get {
                return m_channelState;
            }
        }

        //=========================================================================================
        // プロパティ：コンソールを操作している「ユーザー@サーバー」（初期化完了まではnull）
        //=========================================================================================
        public string ActiveUserServer {
            get {
                return m_userServer;
            }
            set {
                m_userServer = value;
            }
        }

        //=========================================================================================
        // プロパティ：ホームディレクトリ（初期化完了まではnull）
        //=========================================================================================
        public string HomeDirectory {
            get {
                return m_homeDirectory;
            }
            set {
                m_homeDirectory = value;
            }
        }

        //=========================================================================================
        // 列挙子：チャネルの状態
        //=========================================================================================
        public enum ChannelState {
            Initial,                // 初期状態
            Connected,              // 接続済み
            Closed,                 // 切断済み
            Error,                  // エラー発生
        }
    }
}
