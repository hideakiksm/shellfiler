using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Document.Setting;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.GraphicsViewer.Filter;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.Util;

namespace ShellFiler.GraphicsViewer {

    //=========================================================================================
    // クラス：ファイルビューアでのステータスバー
    //=========================================================================================
    public partial class GraphicsViewerStatusBar : StatusStrip {
        // グラフィックビューアのパネル
        private GraphicsViewerPanel m_graphicsViewer;

        // ファイル名領域
        private ToolStripStatusLabel m_fileLabel;

        // マーク状態領域
        private ToolStripStatusLabel m_markStatusLabel;

        // フィルター領域
        private ToolStripStatusLabel m_filterLabel;

        // ズーム領域
        private ToolStripStatusLabel m_zoomLabel;

        // 画像サイズ領域
        private ToolStripStatusLabel m_sizeLabel;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerStatusBar() {
            InitializeComponent();

            this.SuspendLayout();
            
            this.BackColor = SystemColors.Control;
            this.ImageList = UIIconManager.IconImageList;
            m_fileLabel = CreateLabel(118);
            m_fileLabel.Spring = true;
            m_markStatusLabel = CreateLabel(40);
            m_markStatusLabel.Click += new EventHandler(MarkLabel_Click);
            m_filterLabel = CreateLabel(40);
            m_filterLabel.Click += new EventHandler(FilterLabel_Click);
            m_zoomLabel = CreateLabel(40);
            m_zoomLabel.Click += new EventHandler(ZoomLabel_Click);
            m_sizeLabel = CreateLabel(60);

            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {m_fileLabel, m_markStatusLabel, m_filterLabel, m_zoomLabel, m_sizeLabel});

            this.ResumeLayout(false);
        }

        //=========================================================================================
        // 機　能：ステータスバーのラベルを作成する
        // 引　数：[in]cx  幅の初期値
        // 戻り値：作成したラベル
        //=========================================================================================
        private ToolStripStatusLabel CreateLabel(int cx) {
            ToolStripStatusLabel label = new ToolStripStatusLabel();
            label.Margin = new System.Windows.Forms.Padding(10, 3, 0, 0);
            label.AutoSize = true;
            label.Size = new System.Drawing.Size(cx, 19);
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            label.Text = "";
            return label;
        }
        
        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]viewer  グラフィックビューアのパネル
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(GraphicsViewerPanel viewer) {
            m_graphicsViewer = viewer;
            RefreshStatusBar();
        }

        //=========================================================================================
        // 機　能：ステータスバーを更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshStatusBar() {
            ImageInfo imageInfo = m_graphicsViewer.CurrentImage;
            if (imageInfo == null) {
                // 画像を読み込み中
                m_fileLabel.Text = Resources.GraphicsViewer_StatusLoading;
                m_filterLabel.BackColor = SystemColors.Control;
                m_markStatusLabel.Text = string.Format(Resources.GraphicsViewer_StatusMark, "-");
                m_markStatusLabel.ImageIndex = 0;
                m_filterLabel.Text = string.Format(Resources.GraphicsViewer_StatusFilter, "---");
                m_filterLabel.ToolTipText = null;
                m_filterLabel.BackColor = SystemColors.Control;
                m_zoomLabel.Text = string.Format(Resources.GraphicsViewer_StatusZoom, "---");
                m_zoomLabel.BackColor = SystemColors.Control;
                m_sizeLabel.Text = string.Format(Resources.GraphicsViewer_StatusSize, "---", "---", "-");
            } else if (imageInfo.Image == null) {
                // 読み込み失敗
                m_fileLabel.Text = StatusBarFormatter.CreateSlideShowPosition(m_graphicsViewer) + imageInfo.FilePath;
                m_filterLabel.BackColor = SystemColors.Control;
                m_markStatusLabel.Text = string.Format(Resources.GraphicsViewer_StatusMark, "-");
                m_markStatusLabel.ImageIndex = 0;
                m_filterLabel.Text = string.Format(Resources.GraphicsViewer_StatusFilter, "---");
                m_filterLabel.ToolTipText = null;
                m_filterLabel.BackColor = SystemColors.Control;
                m_zoomLabel.Text = string.Format(Resources.GraphicsViewer_StatusZoom, "---");
                m_zoomLabel.BackColor = SystemColors.Control;
                m_sizeLabel.Text = string.Format(Resources.GraphicsViewer_StatusSize, "---", "---", "-");
            } else {
                // 読み込み完了
                StatusBarFormatter info = new StatusBarFormatter(m_graphicsViewer, imageInfo);
                m_fileLabel.Text = info.SlideShowPosition + info.FilePath;
                m_markStatusLabel.Text = info.MarkState;
                m_markStatusLabel.ImageIndex = info.MarkImageIndex;
                m_filterLabel.Text = info.FilterText;
                m_filterLabel.ToolTipText = info.FilterHint;
                m_filterLabel.BackColor = info.FilterBackColor;
                m_zoomLabel.Text = info.ZoomRatio;
                m_zoomLabel.BackColor = info.ZoomRatioBackColor;
                m_sizeLabel.Text = info.SizeInfo;
            }
        }

        //=========================================================================================
        // 機　能：ステータスバーのマーク領域がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MarkLabel_Click(object sender, EventArgs evt) {
            ImageInfo imageInfo = m_graphicsViewer.CurrentImage;
            if (imageInfo == null || imageInfo.Image == null) {
                return;
            }

            // コンテキストメニューを表示
            List<MenuItemSetting> menu = Program.Document.MenuSetting.GraphicsViewerMarkMenuList;
            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.GraphicsViewerMenu, m_graphicsViewer);
            menuImpl.AddItemsFromSetting(cms, cms.Items, menu, Program.Document.KeySetting.GraphicsViewerKeyItemList, false, null);
            menuImpl.RefreshToolbarStatus(new UIItemRefreshContext());
            ContextMenuStrip = cms;
            ContextMenuStrip.Show(this, new Point(m_markStatusLabel.Bounds.Left, m_markStatusLabel.Bounds.Top - cms.Height - 20));
            ContextMenuStrip = null;
        }

        //=========================================================================================
        // 機　能：ステータスバーのフィルター領域がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FilterLabel_Click(object sender, EventArgs evt) {
            ImageInfo imageInfo = m_graphicsViewer.CurrentImage;
            if (imageInfo == null || imageInfo.Image == null) {
                return;
            }

            // コンテキストメニューを表示
            List<MenuItemSetting> menu = Program.Document.MenuSetting.GraphicsViewerFilterMenuList;
            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.GraphicsViewerMenu, m_graphicsViewer);
            menuImpl.AddItemsFromSetting(cms, cms.Items, menu, Program.Document.KeySetting.GraphicsViewerKeyItemList, false, null);
            menuImpl.RefreshToolbarStatus(new UIItemRefreshContext());
            ContextMenuStrip = cms;
            ContextMenuStrip.Show(this, new Point(m_filterLabel.Bounds.Left, m_filterLabel.Bounds.Top - cms.Height - 20));
            ContextMenuStrip = null;
        }

        //=========================================================================================
        // 機　能：ステータスバーのズーム領域がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ZoomLabel_Click(object sender, EventArgs evt) {
            ImageInfo imageInfo = m_graphicsViewer.CurrentImage;
            if (imageInfo == null || imageInfo.Image == null) {
                return;
            }

            // コンテキストメニューを表示
            List<MenuItemSetting> menu = Program.Document.MenuSetting.GraphicsViewerZoomMenuList;
            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.GraphicsViewerMenu, m_graphicsViewer);
            menuImpl.AddItemsFromSetting(cms, cms.Items, menu, Program.Document.KeySetting.GraphicsViewerKeyItemList, false, null);
            menuImpl.RefreshToolbarStatus(new UIItemRefreshContext());
            ContextMenuStrip = cms;
            ContextMenuStrip.Show(this, new Point(m_zoomLabel.Bounds.Left, m_zoomLabel.Bounds.Top - cms.Height - 20));
            ContextMenuStrip = null;
        }
    }
}
