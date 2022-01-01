using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.Terminal;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.Log;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;

namespace ShellFiler.Terminal.UI {
    
    //=========================================================================================
    // クラス：ターミナルのフォーム
    //=========================================================================================
    public partial class TerminalForm : Form, IKeyEventIntegrator, ITwoStrokeKeyForm {
        // ターミナルビューア
        private TerminalPanel m_terminalPanel;

        // ステータスバー
        private TerminalStatusBar m_statusStrip;

        // 通常状態のウィンドウサイズ
        private Rectangle m_normalWindowSize = Rectangle.Empty;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]userServer   このターミナルの接続先user@server
        // 　　　　[in]channel      使用する接続（未接続のときnull）
        // 戻り値：なし
        //=========================================================================================
        public TerminalForm(string userServer, ConsoleScreen console, TerminalShellChannel channel) {
            InitializeComponent();

            this.SuspendLayout();
            this.Icon = Resources.ShellFilerTerm;

            m_statusStrip = new TerminalStatusBar();
            m_terminalPanel = new TerminalPanel(this, this, this, console, channel, userServer, m_statusStrip);
            m_terminalPanel.Dock = DockStyle.Fill;

            m_statusStrip.Location = new Point(0, 599);
            m_statusStrip.Size = new Size(920, 22);
            m_statusStrip.GripMargin = new Padding(0, 2, 0, 2);
            m_statusStrip.Initialize(m_terminalPanel);
            this.functionBar.Initialize(this, UICommandSender.TerminalFunctionBar, m_terminalPanel, Program.Document.KeySetting.TerminalKeyItemList, false);
            this.Controls.Add(m_terminalPanel);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(m_statusStrip);
            this.Controls.Add(this.functionBar);
            this.Controls.Add(this.menuStrip);

            this.menuStrip.Initialize(m_terminalPanel);
            this.toolStrip.Initialize(m_terminalPanel);
            this.ResumeLayout(true);
            this.PerformLayout();

            Program.Document.UserSetting.InitialSetting.InitializeTerminal(this);
            Program.WindowManager.OnOpenViewer(this);
            SetWindowTitle(console.DisplayName);
        }

        //=========================================================================================
        // 機　能：フォームが読み込まれたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TerminalForm_Load(object sender, EventArgs evt) {
            m_terminalPanel.OnFormLoad();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられるときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MonitoringViewerForm_FormClosing(object sender, FormClosingEventArgs evt) {
            TerminalCloseConfirmMode mode = Configuration.Current.TerminalCloseConfirmMode;
            if (m_terminalPanel.ConsoleScreen.ShellChannel == null) {
                // すでにクローズ済み
            } else if (mode == TerminalCloseConfirmMode.KeepChannelSilent) {
                // 無条件にそのまま
            } else {
                // ダイアログで確認
                TerminalCloseConfirmDialog dialog = new TerminalCloseConfirmDialog(mode, m_terminalPanel.ConsoleScreen.UserServer);
                DialogResult result = dialog.ShowDialog(this);
                if (result != DialogResult.OK) {
                    evt.Cancel = true;
                    return;
                }
                if (dialog.CloseChannel) {
                    if (m_terminalPanel.ConsoleScreen.ShellChannel != null) {
                        m_terminalPanel.ConsoleScreen.ShellChannel.Close();
                    }
                }
            }

            // クローズ処理
            m_terminalPanel.OnCloseView();
            Program.Document.UserSetting.InitialSetting.OnCloseTerminal(this);
            Program.WindowManager.OnCloseViewer(this);
        }

        //=========================================================================================
        // 機　能：ウィンドウタイトルを設定する
        // 引　数：[in]title  タイトル
        // 戻り値：なし
        //=========================================================================================
        public void SetWindowTitle(string title) {
            this.Text = string.Format(Resources.WindowTitleTerminal, title);
        }

        //=========================================================================================
        // 機　能：キーが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void MonitoringViewerForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs evt) {
            OnKeyDown(new KeyCommand(evt));
        }
        
        //=========================================================================================
        // 機　能：キーが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void MonitoringViewerForm_KeyUp(object sender, KeyEventArgs evt) {
            OnKeyUp(new KeyCommand(evt));
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]key  キーコマンド
        // 戻り値：キー入力したときtrue
        //=========================================================================================
        public bool OnKeyDown(KeyCommand key) {
            this.functionBar.OnKeyChange(key);
            return m_terminalPanel.OnKeyDown(key);
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う（OnKeyDown処理後）
        // 引　数：[in]key  入力した文字
        // 戻り値：キー入力したときtrue
        //=========================================================================================
        public bool OnKeyChar(char key) {
            return m_terminalPanel.OnKeyChar(key);
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]key  キーコマンド
        // 戻り値：なし
        //=========================================================================================
        public void OnKeyUp(KeyCommand key) {
            this.functionBar.OnKeyChange(key);
        }

        //=========================================================================================
        // 機　能：ビューアを最大化する
        // 引　数：[in]maximize  最大化するときtrue、元に戻すときfalse
        // 　　　　[in]allScreen 全スクリーンを対象にするときtrue、カレントスクリーンを対象にするときfalse
        // 戻り値：なし
        //=========================================================================================
        public void FullScreen(bool maximize, bool allScreen) {
            if (maximize) {
                if (allScreen) {
                    // 最大化
                    if (m_normalWindowSize == Rectangle.Empty) {
                        if (this.WindowState != FormWindowState.Normal) {
                            m_normalWindowSize = RestoreBounds;
                        } else {
                            m_normalWindowSize = new Rectangle(Left, Top, Width, Height);
                        }
                    }
                    Rectangle rcAll = FormUtils.GetAllScreenRectangle();
                    this.WindowState = FormWindowState.Normal;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.TopMost = true;
                    this.Location = new Point(rcAll.Left, rcAll.Top);
                    this.Size = new Size(rcAll.Width, rcAll.Height);
                    this.menuStrip.Hide();
                    this.functionBar.Hide();
                } else {
                    // 最大化
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.TopMost = true;
                    this.WindowState = FormWindowState.Maximized;
                    this.menuStrip.Hide();
                    this.functionBar.Hide();
                }
            } else {
                // 最大化の解除
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.TopMost = false;
                this.WindowState = FormWindowState.Normal;
                this.menuStrip.Show();
                this.functionBar.Show();
                if (m_normalWindowSize != Rectangle.Empty) {
                    this.Size = new Size(m_normalWindowSize.Width, m_normalWindowSize.Height);
                    this.Location = new Point(m_normalWindowSize.Left, m_normalWindowSize.Top);
                    m_normalWindowSize = Rectangle.Empty;
                }
            }
        }

        //=========================================================================================
        // 機　能：フォームがアクティブになったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TerminalForm_Activated(object sender, EventArgs evt) {
            m_terminalPanel.OnWindowActivateChanged(true);
        }

        //=========================================================================================
        // 機　能：フォームが他のウィンドウによって隠されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TerminalForm_Deactivate(object sender, EventArgs evt) {
            m_terminalPanel.OnWindowActivateChanged(false);
        }

        //=========================================================================================
        // 機　能：2ストロークキーの状態が変わったことを通知する
        // 引　数：[in]newState  新しい状態
        // 戻り値：なし
        //=========================================================================================
        public void TwoStrokeKeyStateChanged(TwoStrokeType newState) {
        }

        //=========================================================================================
        // プロパティ：仮想コンソール
        //=========================================================================================
        public ConsoleScreen ConsoleScreen {
            get {
                return m_terminalPanel.ConsoleScreen;
            }
        }

        //=========================================================================================
        // プロパティ：ファンクションバー
        //=========================================================================================
        public FunctionBar FunctionBar {
            get {
                return this.functionBar;
            }
        }

        //=========================================================================================
        // プロパティ：ステータスバー
        //=========================================================================================
        public TerminalStatusBar StatusBar {
            get {
                return m_statusStrip;
            }
        }

        //=========================================================================================
        // プロパティ：ツールバー
        //=========================================================================================
        public TerminalToolBarStrip Toolbar {
            get {
                return this.toolStrip;
            }
        }
    }
}
