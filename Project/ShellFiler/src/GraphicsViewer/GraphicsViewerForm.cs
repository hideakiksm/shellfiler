using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.FileTask;
using ShellFiler.UI.ControlBar;
using ShellFiler.GraphicsViewer.Filter;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.GraphicsViewer;

namespace ShellFiler.GraphicsViewer {

    //=========================================================================================
    // クラス：グラフィックビューアのフォーム
    //=========================================================================================
    public partial class GraphicsViewerForm : Form, IKeyEventIntegrator {
        // 通常状態のウィンドウサイズ
        private Rectangle m_normalWindowSize = Rectangle.Empty;

        // グラフィックビューアの起動パラメータ（はじめに起動したときの状態）
        private GraphicsViewerParameter m_graphicsViewerParameter;

        // フォームが閉じられたときtrue
        private bool m_formClosed = false;

        // フィルター選択ダイアログ（表示中でないときはnull）
        private SelectFilterDialog m_selectFilterSettingDialog = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]gvParam  グラフィックビューアの起動パラメータ
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerForm(GraphicsViewerParameter gvParam) {
            InitializeComponent();
            m_graphicsViewerParameter = gvParam;
            this.viewerPanel.Initialize(this, this.statusStrip, gvParam);
            this.statusStrip.Initialize(this.viewerPanel);
            this.functionBar.Initialize(this, UICommandSender.GraphicsViewerFunctionBar, this.viewerPanel, Program.Document.KeySetting.GraphicsViewerKeyItemList, false);
            this.BackColor = Configuration.Current.GraphicsViewerBackColor;

            Program.Document.UserSetting.InitialSetting.InitializeViewer(this);
            Program.WindowManager.OnOpenViewer(this);

            this.menuStrip.Initialize(this.viewerPanel);
            this.toolStrip.Initialize(this.viewerPanel);

            RefreshTitle();
            this.Icon = Resources.ShellFilerGView;
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
                    this.viewerPanel.ShowScrollBar = false;
                } else {
                    // 最大化
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.TopMost = true;
                    this.WindowState = FormWindowState.Maximized;
                    this.menuStrip.Hide();
                    this.toolStrip.Hide();
                    this.statusStrip.Hide();
                    this.functionBar.Hide();
                    this.viewerPanel.ShowScrollBar = false;
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
                this.viewerPanel.ShowScrollBar = true;
                if (m_normalWindowSize != Rectangle.Empty) {
                    this.Size = new Size(m_normalWindowSize.Width, m_normalWindowSize.Height);
                    this.Location = new Point(m_normalWindowSize.Left, m_normalWindowSize.Top);
                    m_normalWindowSize = Rectangle.Empty;
                }
            }
            this.viewerPanel.OnFullScreenChanged(maximize);
        }

        //=========================================================================================
        // 機　能：フォームが閉じられるときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void GraphicsViewerForm_FormClosing(object sender, FormClosingEventArgs evt) {
            // スライドショー情報の確認
            SlideShowMarkResult newResult = this.viewerPanel.GetMarkResult();
            SlideShowMarkResult oldResult = Program.Document.SlideShowMarkResult;
            if (newResult != null && oldResult != null) {
                DialogResult result = InfoBox.Question(this, MessageBoxButtons.OKCancel, Resources.Msg_GraphicsViewerMarkResult);
                if (result != DialogResult.OK) {
                    evt.Cancel = true;
                    return;
                }
            }
            if (newResult != null) {
                Program.MainWindow.ShowSlideShowResult(true, newResult);
            }

            // 終了処理を実行
            Program.Document.BackgroundTaskManager.GraphicsViewerTaskManager.OnCloseForm(this);
            m_formClosed = true;
            this.viewerPanel.DisposeComponent();
            Program.Document.UserSetting.InitialSetting.OnCloseViewer(this);
            Program.WindowManager.OnCloseViewer(this);
        }

        //=========================================================================================
        // 機　能：キーが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void GraphicsViewerForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs evt) {
            OnKeyDown(new KeyCommand(evt));
        }
        
        //=========================================================================================
        // 機　能：キーが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void GraphicsViewerForm_KeyUp(object sender, KeyEventArgs evt) {
            OnKeyUp(new KeyCommand(evt));
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]key  キーコマンド
        // 戻り値：キー入力したときtrue
        //=========================================================================================
        public bool OnKeyDown(KeyCommand key) {
            this.functionBar.OnKeyChange(key);
            if (this.viewerPanel != null) {
                this.viewerPanel.OnKeyDown(key);
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
        // 機　能：ファイルの読み込みの結果を通知する
        // 引　数：[in]imageInfo 読み込んだイメージの結果
        // 戻り値：なし
        //=========================================================================================
        public void NotifyFileLoad(ImageInfo imageInfo) {
            if (m_formClosed) {
                return;
            }
            // ビューアに通知
            this.viewerPanel.NotifyFileLoad(imageInfo);
        }

        //=========================================================================================
        // 機　能：タイトルを更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshTitle() {
            string filePath = this.viewerPanel.CurrentImage.FilePath;
            if (filePath == "") {
                this.Text = Resources.GraphicsViewer_Title;
            } else {
                this.Text = filePath + " - " + Resources.GraphicsViewer_Title;
            }
        }

        //=========================================================================================
        // 機　能：フィルター設定のダイアログを開く
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OpenFilterSettingDialog() {
            if (m_selectFilterSettingDialog == null) {
                m_selectFilterSettingDialog = new SelectFilterDialog(this.viewerPanel);
                m_selectFilterSettingDialog.Show(this);
            } else {
                m_selectFilterSettingDialog.Focus();
            }
        }

        //=========================================================================================
        // 機　能：フィルター設定のダイアログが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnCloseFilterSettingDialog() {
            m_selectFilterSettingDialog.Dispose();
            m_selectFilterSettingDialog = null;
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアのツールバー
        //=========================================================================================
        public GraphicsViewerToolbarStrip Toolbar {
            get {
                return this.toolStrip;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアのパネル
        //=========================================================================================
        public GraphicsViewerPanel GraphicsViewerPanel {
            get {
                return this.viewerPanel;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアの起動パラメータ（はじめに起動したときの状態）
        //=========================================================================================
        public GraphicsViewerParameter GraphicsViewerParameter {
            get {
                return m_graphicsViewerParameter;
            }
        }

        //=========================================================================================
        // プロパティ：スライドショーモードのときtrue
        //=========================================================================================
        public bool SlideShowMode {
            get {
                return this.viewerPanel.SlideShowList.IsSlideShowMode;
            }
        }
    }
}
