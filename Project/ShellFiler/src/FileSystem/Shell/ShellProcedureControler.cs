using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.Terminal.TerminalSession.CommandEmulator;
using ShellFiler.Properties;
using ShellFiler.UI.Dialog;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Util;

namespace ShellFiler.FileSystem.Shell {

    //=========================================================================================
    // クラス：SSH内部処理のコントローラ
    //=========================================================================================
    class ShellProcedureControler {
        // 接続
        private SSHConnection m_connection;

        // コンテキスト情報
        private FileOperationRequestContext m_context;

        // ターミナル接続
        private TerminalShellChannel m_terminalChannel = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellProcedureControler(SSHConnection connection, FileOperationRequestContext context) {
            m_connection = connection;
            m_context = context;
        }

        //=========================================================================================
        // 機　能：プロシージャの実行に必要な準備を行う
        // 引　数：[in]targetDir   操作対象のサーバー（サーバー内パスは無視）
        // 　　　　[in]isTarget    対象パスの一覧のときtrue、反対パスのときfalse
        // 　　　　[in]initMode    初期化する状況
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Initialize(string targetDir, bool isTarget, InitializeMode initMode) {
            // 　　　　　　　　　　　　　　　            　  Contextあり        Contextなし
            // 一般の利用                                  Contextから継続    ダイアログで選択
            // ログイン経由のcd（常に新規チャネル）        新規チャネル       新規チャネル
            // ログイン経由のcd（必要時にチャネル作成）    Contextから継続    ダイアログで選択
            FileOperationStatus status;

            // 対象を解析
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(targetDir, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // 既存のチャネルを取得
            ShellFileListContext fileListContext = null;
            if (initMode == InitializeMode.LoginWithNewChannel) {
                fileListContext = null;
            } else if (isTarget) {
                fileListContext = (ShellFileListContext)(m_context.SrcFileListContext);
            } else {
                fileListContext = (ShellFileListContext)(m_context.DestFileListContext);
            }

            // コンテキストで指定されたチャネルを使用
            if (fileListContext != null) {
                ShellRequestContext shellRequestContext = m_context.ShellRequestContext;
                TerminalShellChannel cachedChannel = shellRequestContext.GetTerminalShellChannel(fileListContext.TerminalShellChannelId);
                if (cachedChannel != null) {
                    // コンテキストにキャッシュ済み
                    m_terminalChannel = cachedChannel;
                    return FileOperationStatus.Success;
                }
                cachedChannel = m_connection.GetTerminalChannelById(fileListContext.TerminalShellChannelId);
                if (cachedChannel != null) {
                    // コンテキストでは初見だが作成済み
                    m_terminalChannel = cachedChannel;
                    return FileOperationStatus.Success;
                }
            }

            // ダイアログで選択
            if (initMode != InitializeMode.LoginWithNewChannel) {
                success = SelectSSHChannel(targetDir, out m_terminalChannel);
                if (!success) {
                    return FileOperationStatus.Canceled;
                } else if (m_terminalChannel != null) {
                    return FileOperationStatus.Success;
                }
            }

            // 準備
            status = m_connection.ConnectServer();
            if (!status.Succeeded) {
                return status;
            }

            // 仮想コンソールを作成
            SSHUserAuthenticateSettingItem authSetting = m_connection.AuthenticateSetting;
            string userServer = SSHUtils.CreateUserServerShort(authSetting.UserName, authSetting.ServerName, authSetting.PortNo);
            string uniqueName = Program.Document.FileSystemFactory.SSHConnectionManager.CreateUniqueConsoleName(userServer, true);
            ConsoleScreen console = new ConsoleScreen(userServer, uniqueName, DateTime.Now);

            // 接続
            string errorDetail;
            status = ConnectTerminal(console, out errorDetail);
            if (m_connection.Closed) {
                status = FileOperationStatus.Canceled;
            }
            if (m_terminalChannel != null) {
                m_terminalChannel.NotifyConnect(status, errorDetail);
            }

            // ホームディレクトリを取得
            status = ResetHomeDirectory();
            if (!status.Succeeded) {
                return status;
            }

            return status;
        }

        //=========================================================================================
        // 機　能：使用可能なSSHチャネルから実際に使用するチャネルを選択する
        // 引　数：[in]targetDir  処理対象のディレクトリ
        // 　　　　[out]channel   使用するSSHチャネルを返す変数（新規チャネルのときnulll）
        // 戻り値：選択が確定したときtrue
        // メ　モ：戻りfalseのときキャンセル、戻りtrueでchannel==nullのとき、新規チャネル
        //=========================================================================================
        private bool SelectSSHChannel(string targetDir, out TerminalShellChannel channel) {
            channel = null;
            List<TerminalShellChannel> channelList = Program.Document.FileSystemFactory.SSHConnectionManager.GetAuthorizedTerminalChannel(targetDir);
            if (channelList.Count == 0) {
                return true;
            }
            int selected = InputSSHChannel(channelList, targetDir);
            if (selected == -1) {
                return false;
            }
            if (selected < channelList.Count) {
                channel = channelList[selected];
            } else {
                channel = null;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：使用中チャネルのホームディレクトリをリセットする
        // 引　数：なし
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus ResetHomeDirectory() {
            FileOperationStatus status;
            ShellCommandEmulator emulator = m_terminalChannel.ShellCommandEmulator;
            ShellEngineGetCurrentDirectory engine = new ShellEngineGetCurrentDirectory(emulator, m_connection);
            status = emulator.Execute(m_context, engine);
            if (!status.Succeeded) {
                return status;
            }
            m_terminalChannel.HomeDirectory = engine.CurrentDirectory;
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ダイアログで使用するチャネルを選択する
        // 引　数：[in]channelList  使用するチャネルの候補
        // 　　　　[in]targetDir    処理対象のディレクトリ
        // 戻り値：選択したチャネルのインデックス（キャンセル:-1、新規:channelList.Count）
        //=========================================================================================
        private int InputSSHChannel(List<TerminalShellChannel> channelList, string targetDir) {
            object result;
            bool success = BaseThread.InvokeFunctionByMainThread(new InputSSHChannelDelegate(InputSSHChannelUI), out result, channelList, targetDir);
            if (!success) {
                return -1;
            }
            return (int)result;
        }
        delegate int InputSSHChannelDelegate(List<TerminalShellChannel> channelList, string targetDir);
        private int InputSSHChannelUI(List<TerminalShellChannel> channelList, string targetDir) {
            try {
                SSHSelectChannelDialog dialog = new SSHSelectChannelDialog(channelList, targetDir);
                DialogResult result = dialog.ShowDialog(Program.MainWindow);
                if (result != DialogResult.OK) {
                    return -1;
                }
                return dialog.SelectedChannelIndex;
            } catch (Exception e) {
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionUI, "InputSSHChannelUI");
                return -1;
            }
        }

        //=========================================================================================
        // 機　能：ターミナルを接続する
        // 引　数：[in]console       元になる仮想コンソール
        // 　　　　[out]errorDetail  エラー発生時に詳細情報を返す変数
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus ConnectTerminal(ConsoleScreen console, out string errorDetail) {
            FileOperationStatus status;
            errorDetail = null;
            m_terminalChannel = new TerminalShellChannel(m_connection, console, true);
            status = m_terminalChannel.Connect(out errorDetail);
            if (!status.Succeeded) {
                Program.LogWindow.RegistLogLineHelper(string.Format(Resources.Log_SSHShellError, errorDetail));
                return status;
            }
            if (m_connection.Closed) {
                return FileOperationStatus.Canceled;
            }

            // 結果を格納
            m_connection.SetTerminalChannel(m_terminalChannel);
            m_context.ShellRequestContext.Add(m_terminalChannel.ID, m_terminalChannel);
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // プロパティ：キャンセルされたときtrue
        //=========================================================================================
        public bool IsCanceled {
            get {
                return m_connection.Closed;
            }
        }

        //=========================================================================================
        // プロパティ：接続
        //=========================================================================================
        public SSHConnection Connection {
            get {
                return m_connection;
            }
        }
        
        //=========================================================================================
        // プロパティ：コンテキスト情報
        //=========================================================================================
        public FileOperationRequestContext Context {
            get {
                return m_context;
            }
        }
        
        //=========================================================================================
        // プロパティ：ターミナル接続
        //=========================================================================================
        public TerminalShellChannel TerminalChannel {
            get {
                return m_terminalChannel;
            }
        }

        //=========================================================================================
        // 列挙子：初期化モード
        //=========================================================================================
        public enum InitializeMode {
            GenericOperation,               // 一般の利用
            LoginWithNewChannel,            // ログイン経由のcd（常に新規チャネル）
            LoginWithExistingChannel,       // ログイン経由のcd（必要時にチャネル作成）
        }
    }
}
