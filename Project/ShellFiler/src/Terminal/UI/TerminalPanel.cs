using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.Command.Terminal;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.Log;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;

namespace ShellFiler.Terminal.UI {

    //=========================================================================================
    // クラス：ターミナルの機能を提供するパネル（ターミナルの機能単位を提供）
    //=========================================================================================
    public partial class TerminalPanel : UserControl, IUICommandTarget, IConsoleScreenEvent {
        // スクロールの際にページ単位でスクロールする指定
        public const int TERMINAL_SCROLL_PAGE = int.MaxValue - 1;

        // このターミナルを格納しているフォーム（フォーム配下にないときnull）
        public TerminalForm m_terminalForm;

        // このターミナルの接続先user@server
        private string m_userServer;

        // ステータスバー
        private TerminalStatusBar m_statusBar;

        // 2ストロークキーの制御フォーム
        private ITwoStrokeKeyForm m_twoStrokeKeyForm;

        // ターミナルのコンソール画面(このクラスで所有、TerminalViewからも参照)
        private ConsoleScreen m_consoleScreen;

        // 接続対象のターミナル（新規に接続するときnull）
        private TerminalShellChannel m_targetTerminal;

        // 入力用のビュー
        private TerminalView m_terminalViewCurrent;
        
        // バックログのビュー
        private TerminalView m_terminalViewBackLog;

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]form          このターミナルを格納しているフォーム（フォーム配下にないときnull）
        // 　　　　[in]keyForm       2ストロークキーの制御フォーム
        // 　　　　[in]keyIntegrator キー入力の統合先
        // 　　　　[in]console       コンソール画面
        // 　　　　[in]terminal      接続対象のターミナル（新規に接続するときnull）
        // 　　　　[in]userServer    このターミナルの接続先user@server
        // 　　　　[in]statusBar     ステータスバー
        // 戻り値：なし
        //=========================================================================================
        public TerminalPanel(TerminalForm form, ITwoStrokeKeyForm keyForm, IKeyEventIntegrator keyIntegrator, ConsoleScreen console, TerminalShellChannel terminal, string userServer, TerminalStatusBar statusBar) {
            InitializeComponent();
            
            m_terminalForm = form;
            m_twoStrokeKeyForm = keyForm;
            m_consoleScreen = console;
            m_targetTerminal = terminal;
            m_userServer = userServer;
            m_statusBar = statusBar;

            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();

            m_terminalViewCurrent = new TerminalView(keyIntegrator, this, m_consoleScreen, m_userServer, m_statusBar, true);
            m_terminalViewBackLog = new TerminalView(keyIntegrator, this, m_consoleScreen, m_userServer, m_statusBar, false);
            m_terminalViewCurrent.InitializeOppositeView(m_terminalViewBackLog);
            m_terminalViewBackLog.InitializeOppositeView(m_terminalViewCurrent);
            m_terminalViewCurrent.Dock = DockStyle.Fill;
            m_terminalViewCurrent.Location = new Point(0, 0);
            m_terminalViewBackLog.Dock = DockStyle.Fill;
            m_terminalViewBackLog.Location = new Point(0, 0);
            m_terminalViewCurrent.MouseWheel += new MouseEventHandler(this.LogPanel_MouseWheel);
            m_terminalViewBackLog.MouseWheel += new MouseEventHandler(this.LogPanel_MouseWheel);

            ShowLastPosition(m_terminalViewCurrent);
            ShowLastPosition(m_terminalViewBackLog);
            this.splitContainer.Panel1.Controls.Add(m_terminalViewBackLog);
            this.splitContainer.Panel2.Controls.Add(m_terminalViewCurrent);
            this.ActiveControl = m_terminalViewCurrent;
            VisibleBackLog = false;
            m_consoleScreen.Event.AddEventHandler(this);            // VisualBellのため、View登録後に登録

            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        //=========================================================================================
        // 機　能：フォームが読み込まれたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFormLoad() {
            Size charSize = m_terminalViewCurrent.ScreenCharSize;
            Size pixelSize = m_terminalViewCurrent.ClientRectangle.Size;
            string targetPath = "sftp:" + m_userServer + ":/~";
            TerminalShellChannel channel;
            List<TerminalShellChannel> channelList = Program.Document.FileSystemFactory.SSHConnectionManager.GetAuthorizedTerminalChannel(targetPath);
            if (m_targetTerminal != null && channelList.Contains(m_targetTerminal)) {
                // まだ接続中
                // 接続済みの画面を初期化
                channel = m_targetTerminal;
                m_consoleScreen.SetConsoleSize(charSize, pixelSize);
                channel.SetPtySize(charSize, pixelSize);
                m_terminalViewCurrent.SetCaretPosition();
                m_terminalViewBackLog.SetCaretPosition();
            } else {
                // これからチャネルを接続
                m_consoleScreen.SetConsoleSize(charSize, pixelSize);
                FileOperationRequestContext context = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(BackgroundTaskID.NullId, FileSystemID.SFTP, FileSystemID.None, null, null, null);
                InitializeTerminalArg arg = new InitializeTerminalArg(context, m_consoleScreen);
                Program.Document.UIRequestBackgroundThread.Request(arg, targetPath);
                m_statusBar.ShowErrorMessage(Resources.Terminal_StatusConnecting, FileOperationStatus.LogLevel.Null, IconImageListID.None, StatusBarErrorMessageImpl.DisplayTime.Infinite);
            }
            this.Resize += new EventHandler(TerminalPanel_Resize);
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnCloseView() {
            m_terminalViewCurrent.OnCloseView();
            m_terminalViewBackLog.OnCloseView();
            m_consoleScreen.Event.DeleteEventHandler(this);
        }

        //=========================================================================================
        // 機　能：ウィンドウサイズが変更されたときの処理を行う
        // 引　数：[in]sender    　 対象の画面
        // 　　　　[in]status       ステータス
        // 　　　　[in]errorDetail  詳細エラー情報
        // 戻り値：なし
        //=========================================================================================
        private void TerminalPanel_Resize(object sender, EventArgs evt) {
            Size charSize = m_terminalViewCurrent.ScreenCharSize;
            Size pixelSize = m_terminalViewCurrent.ClientRectangle.Size;
            m_consoleScreen.SetConsoleSize(charSize, pixelSize);
            TerminalShellChannel channel = m_consoleScreen.ShellChannel;
            if (channel != null) {
                channel.SetPtySize(charSize, pixelSize);
            }
        }

        //=========================================================================================
        // 機　能：接続処理が完了したときの処理を行う
        // 引　数：[in]sender    　 対象の画面
        // 　　　　[in]status       ステータス
        // 　　　　[in]errorDetail  詳細エラー情報
        // 戻り値：なし
        //=========================================================================================
        public void OnConnect(ConsoleScreen sender, FileOperationStatus status, string errorDetail) {
            if (status.Succeeded) {
                m_statusBar.ShowErrorMessage(Resources.Terminal_StatusConnected, FileOperationStatus.LogLevel.Null, IconImageListID.None, StatusBarErrorMessageImpl.DisplayTime.Default);
            } else {
                string message = string.Format(Resources.Terminal_StatusConnectFailed, errorDetail);
                m_statusBar.ShowErrorMessage(message, FileOperationStatus.LogLevel.Error, IconImageListID.None, StatusBarErrorMessageImpl.DisplayTime.Default);
            }
            m_terminalViewCurrent.Invalidate();
            m_terminalViewBackLog.Invalidate();
        }

        //=========================================================================================
        // 機　能：データが追加されたときの処理を行う
        // 引　数：[in]sender      対象の画面
        // 　　　　[in]changeLog   変化内容
        // 戻り値：なし
        //=========================================================================================
        public void OnAddData(ConsoleScreen sender, LogLineChangeLog changeLog) {
            if (changeLog.RequestBeep) {
                TimeSpanWait wait = new TimeSpanWait();
                for (int i = 0; i < 1; i++) {
                    if (i != 0) {
                        wait.Sleep(30);
                    }
                    if (this.splitContainer.Panel1Collapsed) {
                        m_terminalViewCurrent.ShowVisualBell(true);
                        wait.Sleep(30);
                        m_terminalViewCurrent.ShowVisualBell(false);
                    } else {
                        m_terminalViewBackLog.ShowVisualBell(true);
                        m_terminalViewCurrent.ShowVisualBell(true);
                        wait.Sleep(30);
                        m_terminalViewBackLog.ShowVisualBell(false);
                        m_terminalViewCurrent.ShowVisualBell(false);
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：SSHの接続が閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnSSHClose() {
            m_statusBar.ShowErrorMessage(Resources.Terminal_StatusDisconnected, FileOperationStatus.LogLevel.Null, IconImageListID.None, StatusBarErrorMessageImpl.DisplayTime.Default);
        }

        //=========================================================================================
        // 機　能：選択された範囲のテキストを返す
        // 引　数：[in]getAll     全範囲を取得するときtrue、選択範囲を取得するときfalse
        // 　　　　[in]crlf       改行コード
        // 　　　　[out]text      選択された範囲のテキストを返す変数（選択中ではないときnull）
        // 　　　　[out]createAll すべて選択できたときtrueを返す変数
        // 戻り値：なし
        //=========================================================================================
        public void GetSelectedText(bool getAll, string crlf, out string text, out bool createAll) {
            if (m_terminalViewCurrent.SelectionRange != null) {
                m_terminalViewCurrent.GetSelectedText(getAll, crlf, out text, out createAll);
            } else {
                m_terminalViewBackLog.GetSelectedText(getAll, crlf, out text, out createAll);
            }
        }

        //=========================================================================================
        // 機　能：テキストをすべて選択する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SelectAllText() {
            m_terminalViewCurrent.SelectAllText();
            m_terminalViewCurrent.Invalidate();
            m_terminalViewBackLog.Invalidate();
        }

        //=========================================================================================
        // 機　能：バックログをクリアする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearBackLog() {
            m_consoleScreen.ClearBackLog();
            m_terminalViewCurrent.ClearBackLog();
            m_terminalViewBackLog.ClearBackLog();
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]evt   送信イベント
        // 戻り値：キー入力したときtrue
        //=========================================================================================
        public bool OnKeyDown(KeyCommand evt) {
            TerminalActionCommand command = Program.Document.CommandFactory.CreateTerminalCommandFromKeyInput(evt, this);
            if (command == null) {
                return false;
            }
            TerminalCommandRuntime runtime = new TerminalCommandRuntime(command);
            runtime.Execute();
            return true;
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う（OnKeyDown処理後）
        // 引　数：[in]key  入力した文字
        // 戻り値：キー入力したときtrue
        //=========================================================================================
        public bool OnKeyChar(char key) {
            // 選択中の場合は解除
            if (m_terminalViewCurrent.SelectionRange != null || m_terminalViewBackLog.SelectionRange != null) {
                m_terminalViewCurrent.ClearSelection();
                m_terminalViewBackLog.ClearSelection();
            }

            // 文字を送信
            SendCommand(new string(new char[] {key}));
            return true;
        }

        //=========================================================================================
        // 機　能：マウスホイールが操作されたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LogPanel_MouseWheel(object sender, MouseEventArgs evt) {
            if (this.splitContainer.Panel1Collapsed) {
                m_terminalViewCurrent.LogPanel_MouseWheel(sender, evt);
            } else {
                m_terminalViewBackLog.LogPanel_MouseWheel(sender, evt);
            }
        }

        //=========================================================================================
        // 機　能：SSHコンソールに文字列を送信する
        // 引　数：[in]str  送信する文字列
        // 戻り値：なし
        //=========================================================================================
        public void SendCommand(string str) {
            if (m_consoleScreen.ShellChannel != null) {
                byte[] buffer = m_consoleScreen.Encoding.GetBytes(str);
                SendBytes(buffer);
            }
        }

        //=========================================================================================
        // 機　能：SSHコンソールにデータを送信する
        // 引　数：[in]buffer  送信するデータ
        // 戻り値：なし
        //=========================================================================================
        private void SendBytes(byte[] buffer) {
            TerminalShellChannel channel = m_consoleScreen.ShellChannel;
            if (channel != null) {
                ShowLastPosition(m_terminalViewCurrent);
                channel.WindowsToSshSendData(buffer, 0, buffer.Length);
            }
        }

        //=========================================================================================
        // 機　能：UIでのコマンドが発生したことを通知する
        // 引　数：[in]sender  イベント発生原因の送信元の種別
        // 　　　　[in]item    発生したイベントの項目
        // 戻り値：なし
        //=========================================================================================
        public void OnUICommand(UICommandSender sender, UICommandItem item) {
            // 項目に対応するコマンドを取得して実行
            TerminalActionCommand command = Program.Document.CommandFactory.CreateTerminalCommandFromUICommand(item, this);
            if (command == null) {
                return;
            }
            TerminalCommandRuntime runtime = new TerminalCommandRuntime(command);
            runtime.Execute();
        }

        //=========================================================================================
        // 機　能：ウィンドウのアクティブ状態が変化したときの処理を行う
        // 引　数：[in]isActive  ウィンドウがアクティブになったときtrue、隠されたときfalse
        // 戻り値：なし
        //=========================================================================================
        public void OnWindowActivateChanged(bool isActive) {
            if (isActive) {
                m_terminalViewCurrent.OnFocusChange(true);
                this.ActiveControl = m_terminalViewCurrent;
            } else {
                m_terminalViewCurrent.OnFocusChange(false);
            }
        }

        //=========================================================================================
        // 機　能：ログビューをスクロールさせる
        // 引　数：[in]mainLog   入力画面をスクロールするときtrue、バックログをスクロールするときfalse
        // 　　　　[in]line      スクロールする行数（下方向がプラス、TERMINAL_SCROLL_PAGEのときページ単位）
        // 戻り値：なし
        //=========================================================================================
        public void ScrollLog(bool mainLog, int line) {
            if (!mainLog && !VisibleBackLog) {
                return;
            }
            if (line == 0) {
                return;
            }

            TerminalView targetView;
            if (mainLog) {
                targetView = m_terminalViewCurrent;
            } else {
                targetView = m_terminalViewBackLog;
            }

            if (line == TERMINAL_SCROLL_PAGE) {
                line = targetView.ScreenCharSize.Height - 1;
            } else if (line == -TERMINAL_SCROLL_PAGE) {
                line = -(targetView.ScreenCharSize.Height - 1);
            }

            targetView.ScrollLine(line);
        }

        //=========================================================================================
        // 機　能：最終行を表示する
        // 引　数：[in]view   対象のビュー
        // 戻り値：なし
        //=========================================================================================
        private void ShowLastPosition(TerminalView view) {
            view.ScrollLine(int.MaxValue / 2);
        }

        //=========================================================================================
        // 機　能：選択範囲を点滅させる
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void FlashSelection() {
            if (this.SelectionRange == null) {
                return;
            }
            LogViewSelectionRange selection = this.SelectionRange;
            this.SelectionRange = null;
            m_terminalViewCurrent.SelectionRange = null;
            m_terminalViewBackLog.SelectionRange = null;
            m_terminalViewCurrent.Invalidate();
            m_terminalViewCurrent.Update();
            m_terminalViewBackLog.Invalidate();
            m_terminalViewBackLog.Update();
            Thread.Sleep(100);
            m_terminalViewCurrent.SelectionRange = selection;
            m_terminalViewBackLog.SelectionRange = selection;
            m_terminalViewCurrent.Invalidate();
            m_terminalViewBackLog.Invalidate();
        }

        //=========================================================================================
        // 機　能：スプリットの境界上でマウスのボタンが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void splitContainer_MouseUp(object sender, MouseEventArgs evt) {
            this.ActiveControl = m_terminalViewCurrent;
        }

        //=========================================================================================
        // 機　能：スプリットパネルの境界が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs evt) {
            if (this.splitContainer.SplitterDistance < 8) {
                VisibleBackLog = false;
            }
        }

        //=========================================================================================
        // 機　能：ウィンドウタイトルをリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetWindowTitle() {
            if (m_terminalForm != null) {
                m_terminalForm.SetWindowTitle(m_consoleScreen.DisplayName);
            }
        }

        //=========================================================================================
        // プロパティ：このターミナルを格納しているフォーム（フォーム配下にないときnull）
        //=========================================================================================
        public TerminalForm TerminalForm {
            get {
                return m_terminalForm;
            }
        }

        //=========================================================================================
        // プロパティ：ダイアログの親となるフォーム
        //=========================================================================================
        public Form DialogParentForm {
            get {
                if (m_terminalForm != null) {
                    return m_terminalForm;
                }
                return null;
            }
        }

        //=========================================================================================
        // プロパティ：このターミナルの接続先user@server
        //=========================================================================================
        public string UserServer {
            get {
                return m_userServer;
            }
        }

        //=========================================================================================
        // プロパティ：仮想コンソール
        //=========================================================================================
        public ConsoleScreen ConsoleScreen {
            get {
                return m_consoleScreen;
            }
        }

        //=========================================================================================
        // プロパティ：2ストロークキーの制御フォーム
        //=========================================================================================
        public ITwoStrokeKeyForm TwoStrokeKeyForm {
            get {
                return m_twoStrokeKeyForm;
            }
        }

        //=========================================================================================
        // プロパティ：バックログを表示するときtrue
        //=========================================================================================
        public bool VisibleBackLog {
            get {
                return !this.splitContainer.Panel1Collapsed;
            }
            set {
                if (value != this.splitContainer.Panel1Collapsed) {
                    return;
                }
                if (value) {
                    // OFF→ON
                    if (this.splitContainer.SplitterDistance < 30) {
                        this.splitContainer.SplitterDistance = this.splitContainer.Height / 2;
                    }
                    this.splitContainer.Panel1Collapsed = false;
                    ShowLastPosition(m_terminalViewCurrent);
                } else {
                    // ON→OFF
                    this.splitContainer.Panel1Collapsed = true;
                }
            }
        }
        
        //=========================================================================================
        // プロパティ：選択範囲をドラッグ中のときtrue
        //=========================================================================================
        public bool IsMouseDrag {
            get {
                return m_terminalViewBackLog.IsMouseDrag || m_terminalViewCurrent.IsMouseDrag;
            }
        }

        //=========================================================================================
        // プロパティ：選択中の範囲（選択がないときnull）
        //=========================================================================================
        public LogViewSelectionRange SelectionRange {
            get {
                if (m_terminalViewBackLog.SelectionRange != null) {
                    return m_terminalViewBackLog.SelectionRange;
                }
                if (m_terminalViewCurrent.SelectionRange != null) {
                    return m_terminalViewCurrent.SelectionRange;
                }
                return null;
            }
            set {
                m_terminalViewBackLog.SelectionRange = value;
                m_terminalViewCurrent.SelectionRange = value;
            }
        }
    }
}
