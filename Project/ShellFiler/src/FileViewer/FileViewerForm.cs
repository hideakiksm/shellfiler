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
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;

namespace ShellFiler.FileViewer {
    
    //=========================================================================================
    // クラス：ファイルビューアのフォーム
    //=========================================================================================
    public partial class FileViewerForm : Form, IKeyEventIntegrator {
        // ファイルビューアの表示モード
        private ViewerMode m_viewerMode;

        // 表示中の対象ファイル
        private IFileViewerDataSource m_accessibleFile;

        // テキストファイルビューア
        private TextFileViewer m_textFileViewer;

        // テキストファイルビューアのレーダーバー
        private RadarBar m_radarBar;

        // 通常状態のウィンドウサイズ
        private Rectangle m_normalWindowSize = Rectangle.Empty;

        // 読み込みタスクのID
        private BackgroundTaskID m_readBackgroundTaskId;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]mode   ファイルビューアの初期モード
        // 　　　　[in]file   表示対象のファイル（読み込み中を前提）
        // 　　　　[in]taskId 読み込みタスクのID
        // 戻り値：なし
        //=========================================================================================
        public FileViewerForm(ViewerMode mode, IFileViewerDataSource file, BackgroundTaskID taskId) {
            m_viewerMode = mode;
            m_accessibleFile = file;
            m_readBackgroundTaskId = taskId;

            InitializeFileViewer();
            Program.Document.UserSetting.InitialSetting.InitializeViewer(this);
            Program.WindowManager.OnOpenViewer(this);

            // タイトル
            string filePath;
            if (m_textFileViewer.TextBufferLineInfo.TargetFile.DisplayName == null) {
                filePath = m_textFileViewer.TextBufferLineInfo.TargetFile.FilePath;
            } else {
                filePath = m_textFileViewer.TextBufferLineInfo.TargetFile.DisplayName;
            }
            this.Text = string.Format(Resources.FileViewer_Title, filePath);

            // すべての初期化が完了した後、読み込み通知イベントをつなげる
            m_accessibleFile.StatusChangedEvent += new AccessibleFileStatusChangedEventHandler(FileViewerFile_StatusChanged);
        }

        //=========================================================================================
        // 機　能：ファイルビューアとして初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void InitializeFileViewer() {
            this.SuspendLayout();

            InitializeComponent();
            this.Icon = Resources.ShellFilerFView;

            m_radarBar = new RadarBar();
            m_radarBar.Dock = System.Windows.Forms.DockStyle.Right;
            m_radarBar.Location = new System.Drawing.Point(0, 51);
            m_radarBar.Name = "radarbar";
            m_radarBar.Size = new System.Drawing.Size(6, 384);
            m_radarBar.TabIndex = 3;

            m_textFileViewer = new TextFileViewer(this, this.statusStrip, m_radarBar);
            this.statusStrip.Initialize(m_textFileViewer);
            this.functionBar.Initialize(this, UICommandSender.FileViewerFunctionBar, m_textFileViewer, Program.Document.KeySetting.FileViewerKeyItemList, false);
            m_textFileViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            m_textFileViewer.Location = new System.Drawing.Point(0, 51);
            m_textFileViewer.Name = "panel1";
            m_textFileViewer.Size = new System.Drawing.Size(765, 384);
            m_textFileViewer.TabIndex = 3;
            this.viewerPanel.Controls.Add(m_textFileViewer);

            this.menuStrip.Initialize(ViewerMode.TextView, m_textFileViewer);
            this.toolStrip.Initialize(ViewerMode.TextView, m_textFileViewer);
            m_radarBar.Initialize(m_textFileViewer);

            this.viewerPanel.Controls.Add(m_radarBar);
            this.ResumeLayout(true);
            this.PerformLayout();
        }

        //=========================================================================================
        // 機　能：読み込み中ファイルの状態が変わったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileViewerFile_StatusChanged(object sender, AccessibleFileStatusChangedEventArgs evt) {
            if (m_textFileViewer != null) {
                m_textFileViewer.OnFileStatusChanged(evt.ChunkLoaded, evt.StatusChanged);
            }
        }

        //=========================================================================================
        // 機　能：フォームが閉じられるときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void FileViewerForm_FormClosing(object sender, FormClosingEventArgs evt) {
            // ファイル保存
            if (m_accessibleFile.SaveRequired) {
                DialogResult result = InfoBox.Question(this, MessageBoxButtons.YesNoCancel, Resources.FileViewer_ConfirmSaveRequired);
                if (result == DialogResult.Yes) {
                    evt.Cancel = true;
                    UICommandItem commandItem = new UICommandItem(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SaveTextAsCommand)));
                    FileViewerActionCommand command = Program.Document.CommandFactory.CreateFileViewerCommandFromUICommand(commandItem, m_textFileViewer);
                    command.Execute();
                    return;
                } else if (result == DialogResult.Cancel) {
                    evt.Cancel = true;
                    return;
                }
            }

            // クローズ処理
            this.statusStrip.DisposeStatusBar();
            m_textFileViewer.OnFormClosed();
            if (m_accessibleFile.LoadingTaskId != BackgroundTaskID.NullId) {
                Program.Document.BackgroundTaskManager.CancelBackgroundTask(m_accessibleFile.LoadingTaskId, true);
            }
            Program.Document.UserSetting.InitialSetting.OnCloseViewer(this);
            Program.WindowManager.OnCloseViewer(this);
        }

        //=========================================================================================
        // 機　能：キーが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void FileViewerForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs evt) {
            OnKeyDown(new KeyCommand(evt));
        }
        
        //=========================================================================================
        // 機　能：キーが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void FileViewerForm_KeyUp(object sender, KeyEventArgs evt) {
            OnKeyUp(new KeyCommand(evt));
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]key  キーコマンド
        // 戻り値：キー入力したときtrue
        //=========================================================================================
        public bool OnKeyDown(KeyCommand key) {
            this.functionBar.OnKeyChange(key);
            if (m_textFileViewer != null) {
                m_textFileViewer.OnKeyDown(key);
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
                    this.toolStrip.Hide();
                    this.statusStrip.Hide();
                    this.functionBar.Hide();
                    this.viewerPanel.Show();
                    this.viewerPanel.Update();
                } else {
                    // 最大化
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.TopMost = true;
                    this.WindowState = FormWindowState.Maximized;
                    this.menuStrip.Hide();
                    this.toolStrip.Hide();
                    this.statusStrip.Hide();
                    this.functionBar.Hide();
                    this.viewerPanel.Show();
                    this.viewerPanel.Update();
                }
            } else {
                // 最大化の解除
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.TopMost = false;
                this.WindowState = FormWindowState.Normal;
                this.menuStrip.Show();
                this.toolStrip.Show();
                this.statusStrip.Show();
                this.functionBar.Show();
                if (m_normalWindowSize != Rectangle.Empty) {
                    this.Size = new Size(m_normalWindowSize.Width, m_normalWindowSize.Height);
                    this.Location = new Point(m_normalWindowSize.Left, m_normalWindowSize.Top);
                    m_normalWindowSize = Rectangle.Empty;
                }
            }
        }
        
        //=========================================================================================
        // 機　能：再描画する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Redraw() {
            m_textFileViewer.Invalidate();
        }

        //=========================================================================================
        // プロパティ：現在表示中のファイル
        //=========================================================================================
        public IFileViewerDataSource CurrentFile {
            get {
                return m_accessibleFile;
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
        // プロパティ：ツールバー
        //=========================================================================================
        public FileViewerToolbarStrip Toolbar {
            get {
                return this.toolStrip;
            }
        }

        //=========================================================================================
        // 列挙子：ファイルビューアのモード
        //=========================================================================================
        public enum ViewerMode {
            TextView,               // テキスト表示
            DumpView,               // ダンプ表示
            Auto,                   // 自動判別
        }
    }
}
