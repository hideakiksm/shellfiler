using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.UI.Log;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;
using ShellFiler.FileSystem;
using ShellFiler.Terminal.TerminalSession;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.Document.SSH {

    //=========================================================================================
    // クラス：SSHサーバとの接続
    //=========================================================================================
    public class SSHConnection {
        // コネクションマネージャ
        private SSHConnectionManager m_manager;

        // Sessionのマルチスレッド同期用
        private Object m_lockSession = new Object();

        // 接続に使用する認証情報
        private SSHUserAuthenticateSettingItem m_authSetting;

        // 接続に使用する認証情報が一時的なもののときtrue
        private bool m_isTemporaryAuthSetting;

        // SSHライブラリのルート(null:接続していない)
        private JSch m_jsch = null;

        // SSHのセッション(null:接続していない)
        private Session m_sshSession = null;

        // ターミナル用のチャネル
        private List<TerminalShellChannel> m_terminalChannel = new List<TerminalShellChannel>();

        // コマンドの辞書
        private ShellCommandDictionary m_shellCommandDictionary;

        // この接続のSFTP接続でのホームディレクトリ
        private string m_sftpHomeDirectory;

        // この接続で直前に表示されたディレクトリ
        private string m_previousDirectory = "~/";

        // 接続を使用中のときtrue
        private bool m_inUse;

        // 接続でエラーが発生したときtrue
        private bool m_isError;

        // 接続が切れたときtrue
        private bool m_isClosed;

        // 接続が切れたときシグナル状態になるイベント
        private ManualResetEvent m_closedEvent = new ManualResetEvent(false);

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent       所有元のコネクションマネージャ
        // 　　　　[in]authSetting  接続に使用する認証情報
        // 　　　　[in]isTempAuth   認証情報が一時的なもののときtrue
        // 戻り値：なし
        //=========================================================================================
        public SSHConnection(SSHConnectionManager parent, SSHUserAuthenticateSettingItem authSetting, bool isTempAuth) {
            m_manager = parent;
            m_authSetting = authSetting;
            m_isTemporaryAuthSetting = isTempAuth;
            m_inUse = false;
            m_isError = false;
            m_isClosed = false;
            m_shellCommandDictionary = Program.Document.OSSpecSetting.GetShellCommandDictionary(authSetting.TargetOS);
        }
        
        //=========================================================================================
        // 機　能：サーバーに接続する
        // 引　数：なし
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus ConnectServer() {
            FileOperationStatus status;
            lock (this) {
                status = InternalConnectServer();
            }
            if (status != FileOperationStatus.Success) {
                m_manager.CloseServer(m_authSetting.ServerName, m_authSetting.UserName, m_authSetting.PortNo);
            }
            return status;
        }

        //=========================================================================================
        // 機　能：サーバー接続の内部処理を行う
        // 引　数：なし
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus InternalConnectServer() {
            // すでに接続している場合はエラーチェック
            if (m_sshSession != null) {
                if (m_isError || m_isClosed) {
                    return FileOperationStatus.FailedConnect;
                }
                return FileOperationStatus.Success;
            }
            
            // ログに接続中メッセージ出力
            string userServer = SSHUtils.CreateUserServerShort(m_authSetting.UserName, m_authSetting.ServerName, m_authSetting.PortNo);
            Program.LogWindow.RegistLogLineHelper(string.Format(Resources.Log_SSHTryConnect, userServer));

            // パスワード
            string password = m_authSetting.Password;
            
            // SSHセッションを開始
            string keyAuthError = null;                     // 公開鍵認証でエラーが発生したときのメッセージ
            lock (m_lockSession) {
                m_jsch = new JSch();
                if (m_isClosed) {
                    return FileOperationStatus.FailedConnect;
                }
                if (m_authSetting.KeyAuthentication && File.Exists(m_authSetting.PrivateKeyFilePath)) {
                    try {
                        m_jsch.addIdentity(m_authSetting.PrivateKeyFilePath);
                    } catch (Exception e) {
                        keyAuthError = e.Message;
                    }
                }
                m_sshSession = m_jsch.getSession(m_authSetting.UserName, m_authSetting.ServerName, m_authSetting.PortNo);
            }
            if (keyAuthError != null) {
                Program.LogWindow.RegistLogLineHelper(string.Format(Resources.Log_SSHPrivateKey, m_authSetting.PrivateKeyFilePath, keyAuthError));
                return FileOperationStatus.FailedConnect;
            }

            UserInfo ui = new SSHConnectUserInfo(m_authSetting, password, m_isTemporaryAuthSetting);
            m_sshSession.setUserInfo(ui);
            try {
                m_sshSession.setTimeout(5);
                m_sshSession.connect(m_authSetting.Timeout);
                
                // SFTPコネクションを取得
                ChannelSftp channel = (ChannelSftp)(m_sshSession.openChannel("sftp"));
                try {
                    channel.connect();
                    m_sftpHomeDirectory = GenericFileStringUtils.CompleteDirectoryName(channel.pwd(), "/");
                    m_previousDirectory = m_sftpHomeDirectory;
                } finally {
                    channel.disconnect();
                }

                // ログに完了メッセージ出力
                Program.LogWindow.RegistLogLineHelper(Resources.Log_SSHConnected);

                return FileOperationStatus.Success;
            } catch (JSchAuthCancelException) {
                m_isError = true;
                m_isClosed = true;
                return FileOperationStatus.Canceled;
            } catch (Exception e) {
                string dispMessage = string.Format(Resources.Log_SSHConnectFail, userServer, e.Message);
                Program.LogWindow.RegistLogLineHelper(dispMessage);
                return FileOperationStatus.FailedConnect;
            }
        }

        //=========================================================================================
        // 機　能：パスワードを入力する
        // 引　数：[in]authSetting  対象の認証情報
        // 　　　　[in]isTempAuth   認証情報が一時的なもののときtrue
        // 戻り値：パスワード（キャンセルされたときnull）
        //=========================================================================================
        private static string InputPassword(SSHUserAuthenticateSettingItem authSetting, bool isTempAuth) {
            object result;
            bool success = BaseThread.InvokeFunctionByMainThread(new InputPasswordDelegate(InputPasswordDelegateUI), out result, authSetting, isTempAuth);
            if (!success) {
                return null;
            }
            return (string)result;
        }
        private delegate string InputPasswordDelegate(SSHUserAuthenticateSettingItem authSetting, bool isTempAuth);
        private static string InputPasswordDelegateUI(SSHUserAuthenticateSettingItem authSetting, bool isTempAuth) {
            try {
                // パスワードを入力
                SSHInputPasswordDialog dialog = new SSHInputPasswordDialog(authSetting, isTempAuth);
                DialogResult dialogResult = dialog.ShowDialog(Program.MainWindow);
                if (dialogResult != DialogResult.OK) {
                    return null;
                }

                // パスワードを保存
                if (dialog.SavePassword) {
                    authSetting.Password = dialog.Password;
                    SSHUserAuthenticateSetting setting = Program.Document.SSHUserAuthenticateSetting;
                    setting.SaveData();
                }

                return dialog.Password;
            } catch (Exception e) {
                Program.Abort("InputPasswordDelegateUI()の処理中、予期しない例外が発生しました。\n{0}", e.ToString());
                return null;
            }
        }

        //=========================================================================================
        // 機　能：例外発生時の処理を行う
        // 引　数：[in]e  発生した例外オブジェクト
        // 戻り値：通信エラー時のステータス
        //=========================================================================================
        public FileOperationStatus OnException(Exception e, string message) {
            lock (this) {
                m_isError = true;
            }
            m_manager.CloseServer(m_authSetting.ServerName, m_authSetting.UserName, m_authSetting.PortNo);

            string userServer = SSHUtils.CreateUserServerShort(m_authSetting.UserName, m_authSetting.ServerName, m_authSetting.PortNo);
            string dispMessage = string.Format(message, userServer, e.Message);
            Program.LogWindow.RegistLogLineHelper(dispMessage);
            return FileOperationStatus.FailedConnect;
        }

        //=========================================================================================
        // 機　能：サーバーとの接続を完全に閉じる
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public virtual void DisposeConnection() {
            lock (m_lockSession) {
                m_isClosed = true;
                m_closedEvent.Set();

                for (int i = 0; i < m_terminalChannel.Count; i++) {
                    try {
                        m_terminalChannel[i].Close();
                    } catch (Exception) {
                        // 終了時の例外は無視
                    }
                }
                m_terminalChannel.Clear();

                try {
                    if (m_sshSession != null) {
                        m_sshSession.disconnect();
                    }
                } catch (Exception) {
                    // 終了時の例外は無視
                }
            }

            // 接続終了のログを出力
            string userServer = SSHUtils.CreateUserServerShort(m_authSetting.UserName, m_authSetting.ServerName, m_authSetting.PortNo);
            string message = string.Format(ShellFiler.Properties.Resources.Log_SSHDisconnect, userServer);
            Program.LogWindow.RegistLogLineHelper(message);
        }

        //=========================================================================================
        // 機　能：リクエスト処理開始を通知する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void StartRequest() {
            lock (this) {
                m_inUse = true;
            }
        }

        //=========================================================================================
        // 機　能：リクエスト処理完了を通知する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CompleteRequest() {
            lock (this) {
                m_inUse = false;
            }
        }

        //=========================================================================================
        // 機　能：ターミナル用のチャネルを設定する
        // 引　数：[in]terminalChannel  ターミナル用のチャネル
        // 戻り値：なし
        //=========================================================================================
        public void SetTerminalChannel(TerminalShellChannel terminalChannel) {
            lock (m_lockSession) {
                m_terminalChannel.Add(terminalChannel);
            }
        }

        //=========================================================================================
        // 機　能：ターミナル用のチャネルを削除する
        // 引　数：[in]terminalChannel  ターミナル用のチャネル
        // 戻り値：なし
        //=========================================================================================
        public void RemoveTerminalChannel(TerminalShellChannel terminalChannel) {
            lock (m_lockSession) {
                m_terminalChannel.Remove(terminalChannel);
                try {
                    terminalChannel.Close();
                } catch (Exception) {
                    // 終了時の例外は無視
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイルシステム用のターミナルチャネルを取得する
        // 引　数：[in]id  シェルチャネルのID
        // 戻り値：ターミナルチャネル（未登録のときnull）
        //=========================================================================================
        public TerminalShellChannel GetTerminalChannelById(TerminalShellChannelID id) {
            lock (m_lockSession) {
                for (int i = 0; i < m_terminalChannel.Count; i++) {
                    if (m_terminalChannel[i].ID == id) {
                        return m_terminalChannel[i];
                    }
                }
            }
            return null;
        }

        //=========================================================================================
        // 機　能：byte配列をstringに変換する
        // 引　数：[in]buf  byte配列
        // 戻り値：文字列表現
        //=========================================================================================
        public string ByteToString(byte[] buf) {
            byte[] bufCrLf = new byte[buf.Length * 2];
            int lengthCrLf = 0;
            for (int i = 0; i < buf.Length; i++) {
                if (buf[i] == '\x0a') {
                    bufCrLf[lengthCrLf++] = (byte)'\x0d';
                    bufCrLf[lengthCrLf++] = (byte)'\x0a';
                } else {
                    bufCrLf[lengthCrLf++] = buf[i];
                }
            }
            string result = m_authSetting.Encoding.GetString(bufCrLf, 0, lengthCrLf);
            return result;
        }

        //=========================================================================================
        // プロパティ：コマンドの辞書
        //=========================================================================================
        public ShellCommandDictionary ShellCommandDictionary {
            get {
                return m_shellCommandDictionary;
            }
        }

        //=========================================================================================
        // プロパティ：コネクションマネージャ
        //=========================================================================================
        public SSHConnectionManager ConnectionManager {
            get {
                return m_manager;
            }
        }

        //=========================================================================================
        // プロパティ：接続先の認証情報
        //=========================================================================================
        public SSHUserAuthenticateSettingItem AuthenticateSetting {
            get {
                return m_authSetting;
            }
        }

        //=========================================================================================
        // プロパティ：SSHのセッション（閉じられた後は無効なセッションが返る）
        //=========================================================================================
        public Session SSHSession {
            get {
                return m_sshSession;
            }
        }

        //=========================================================================================
        // プロパティ：ターミナル用のチャネル
        //=========================================================================================
        public List<TerminalShellChannel> TerminalChannel {
            get {
                return m_terminalChannel;
            }
        }

        //=========================================================================================
        // プロパティ：SFTP接続でのホームディレクトリ
        //=========================================================================================
        public string SFTPHomeDirectory {
            get {
                return m_sftpHomeDirectory;
            }
        }

        //=========================================================================================
        // プロパティ：この接続で直前に表示されたディレクトリ
        //=========================================================================================
        public string PreviousDirectory {
            get {
                return m_previousDirectory;
            }
            set {
                m_previousDirectory = value;
            }
        }

        //=========================================================================================
        // プロパティ：接続でエラーが発生したときtrue
        //=========================================================================================
        public bool IsError {
            get {
                return m_isError;
            }
        }

        //=========================================================================================
        // プロパティ：接続が切れたときtrue
        //=========================================================================================
        public bool Closed {
            get {
                return m_isClosed;
            }
        }

        //=========================================================================================
        // プロパティ：接続が切れたときシグナル状態になるイベント
        //=========================================================================================
        public ManualResetEvent ClosedEvent {
            get {
                return m_closedEvent;
            }
        }

        //=========================================================================================
        // プロパティ：接続中のときtrue
        //=========================================================================================
        public bool InUse {
            get {
                return m_inUse;
            }
        }

        //=========================================================================================
        // クラス：SSH接続時、ライブラリに対してユーザー情報を提供するクラス
        //=========================================================================================
        private class SSHConnectUserInfo : UserInfo {
            // 認証情報
            private SSHUserAuthenticateSettingItem m_authSetting;

            // 接続に賞する認証情報が一時的なもののときtrue
            private bool m_isTemporaryAuthSetting;

            // パスワード
            private string m_password;

            // パスワード入力を再試行した回数
            private int m_passwordRetryCount;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]authSetting  認証情報
            // 　　　　[in]password     パスワード
            // 　　　　[in]isTempAuth   認証情報が一時的なもののときtrue
            // 戻り値：なし
            //=========================================================================================
            public SSHConnectUserInfo(SSHUserAuthenticateSettingItem authSetting, string password, bool isTempAuth) {
                m_authSetting = authSetting;
                m_password = password;
                m_passwordRetryCount = 0;
                m_isTemporaryAuthSetting = isTempAuth;
            }

            //=========================================================================================
            // 機　能：パスワードを取得する
            // 引　数：なし
            // 戻り値：パスワード
            //=========================================================================================
            public string getPassword() {
                if (m_authSetting.KeyAuthentication) {
                    return null;
                } else {
                    if (m_password == null || m_passwordRetryCount > 0) {
                        m_password = InputPassword(m_authSetting, m_isTemporaryAuthSetting);
                    }
                    m_passwordRetryCount++;
                    return m_password;
                }
            }

            public bool promptYesNo(string message) {
                message += "[YES]";
                Program.LogWindow.RegistLogLineHelper(message);
                return true;
            }
            public string getPassphrase() {
                if (!m_authSetting.KeyAuthentication) {
                    return null;
                } else {
                    if (m_password == null || m_passwordRetryCount > 0) {
                        m_password = InputPassword(m_authSetting, m_isTemporaryAuthSetting);
                    }
                    m_passwordRetryCount++;
                    return m_password;
                }
            }
            public bool promptPassphrase(string message) {
                return true;
            }
            public bool promptPassword(string message) {
                message += "[********]";
                Program.LogWindow.RegistLogLineHelper(message);
                return true;
            }
            public void showMessage(string message) {
                Program.LogWindow.RegistLogLineHelper(message);
            }
        }
    }
}
