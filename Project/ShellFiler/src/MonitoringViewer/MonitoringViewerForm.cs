using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.Command.MonitoringViewer;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;

namespace ShellFiler.MonitoringViewer {
    
    //=========================================================================================
    // クラス：モニタリングビューアのフォーム
    //=========================================================================================
    public partial class MonitoringViewerForm : Form, IKeyEventIntegrator, ITwoStrokeKeyForm, IUICommandTarget {
        // モニタリングファイルビューア
        private IMonitoringViewer m_monitoringViewer;

        // 通常状態のウィンドウサイズ
        private Rectangle m_normalWindowSize = Rectangle.Empty;

        // 読み込みタスクのID
        private BackgroundTaskID m_readBackgroundTaskId;

        // インクリメンタルサーチの実装
        private IncrementalSearchLogic m_searchLogic;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]viewer  ビューア本体
        // 　　　　[in]taskId  読み込みタスクのID
        // 戻り値：なし
        //=========================================================================================
        public MonitoringViewerForm(IMonitoringViewer viewer, BackgroundTaskID taskId) {
            m_monitoringViewer = viewer;
            m_readBackgroundTaskId = taskId;
            m_searchLogic = new IncrementalSearchLogic(this, viewer);

            InitializeViewer();
            Program.Document.UserSetting.InitialSetting.InitializeMonitoringViewer(this);
            Program.WindowManager.OnOpenViewer(this);
            this.Text = string.Format(Resources.MonitorView_Title, viewer.Title);
        }

        //=========================================================================================
        // 機　能：モニタリングビューアとして初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void InitializeViewer() {
            this.SuspendLayout();

            InitializeComponent();
            this.Icon = Resources.ShellFilerMView;

            UserControl panel = m_monitoringViewer.CreateMonitorPanel(this);
            this.statusStrip.Initialize(m_monitoringViewer);
            this.functionBar.Initialize(this, UICommandSender.MonitoringViewerFunctionBar, m_monitoringViewer.MonitoringViewerForm, Program.Document.KeySetting.MonitoringViewerKeyItemList, false);
            panel.Dock = DockStyle.Fill;
            this.viewerPanel.Controls.Add(panel);

            this.menuStrip.Initialize(m_monitoringViewer);
            this.toolStrip.Initialize(m_monitoringViewer);

            this.ResumeLayout(true);
            this.PerformLayout();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられるときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void MonitoringViewerForm_FormClosing(object sender, FormClosingEventArgs evt) {
            // クローズ処理
            m_searchLogic.CloseViewer();
            m_monitoringViewer.OnFormClosed();
            if (m_readBackgroundTaskId != BackgroundTaskID.NullId) {
                Program.Document.BackgroundTaskManager.CancelBackgroundTask(m_readBackgroundTaskId, true);
            }
            Program.Document.UserSetting.InitialSetting.OnCloseMonitoringViewer(this);
            Program.WindowManager.OnCloseViewer(this);
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
            if (m_monitoringViewer != null) {
                if (m_monitoringViewer.MatrixDataView == null) {
                    return false;
                }

                // キーに対応するコマンドを取得して実行
                MonitoringViewerActionCommand command = Program.Document.CommandFactory.CreateFromKeyInput(key, m_monitoringViewer);
                if (command != null) {
                    MonitoringViewerCommandRuntime runtime = new MonitoringViewerCommandRuntime(command);
                    runtime.Execute();
                    return true;
                } else {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う（OnKeyDown処理後）
        // 引　数：[in]key  入力した文字
        // 戻り値：キー入力したときtrue
        //=========================================================================================
        public bool OnKeyChar(char key) {
            return false;
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
        // 機　能：UIでのコマンドが発生したことを通知する
        // 引　数：[in]sender  イベント発生原因の送信元の種別
        // 　　　　[in]item    発生したイベントの項目
        // 戻り値：なし
        //=========================================================================================
        public void OnUICommand(UICommandSender sender, UICommandItem item) {
            if (m_monitoringViewer.MatrixDataView == null) {
                return;
            }

            // 項目に対応するコマンドを取得して実行
            MonitoringViewerActionCommand command = Program.Document.CommandFactory.CreateMonitoringViewerCommandFromUICommand(sender, item, m_monitoringViewer);
            if (command != null) {
                MonitoringViewerCommandRuntime runtime = new MonitoringViewerCommandRuntime(command);
                runtime.Execute();
            }
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
        // 機　能：ビューを更新してデータの内容を反映する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshView() {
            m_monitoringViewer.MatrixDataView.RefreshView();
        }

        //=========================================================================================
        // 機　能：2ストロークキーの状態が変わったことを通知する
        // 引　数：[in]newState  新しい状態
        // 戻り値：なし
        //=========================================================================================
        public void TwoStrokeKeyStateChanged(TwoStrokeType newState) {
        }

        //=========================================================================================
        // 機　能：検索ダイアログを表示する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ShowSearchDialog() {
            m_searchLogic.OpenSearchDialog();
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
        public MonitoringViewerStatusBar StatusBar {
            get {
                return this.statusStrip;
            }
        }

        //=========================================================================================
        // プロパティ：ツールバー
        //=========================================================================================
        public MonitoringViewerToolbarStrip Toolbar {
            get {
                return this.toolStrip;
            }
        }

        //=========================================================================================
        // プロパティ：インクリメンタルサーチの実装
        //=========================================================================================
        public IncrementalSearchLogic IncrementalSearchLogic {
            get {
                return m_searchLogic;
            }
        }
    }
}
