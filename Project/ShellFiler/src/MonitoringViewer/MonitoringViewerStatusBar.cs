using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.MonitoringViewer {

    //=========================================================================================
    // クラス：モニタリングビューアでのステータスバー
    //=========================================================================================
    public partial class MonitoringViewerStatusBar : StatusStrip {
        // モニタリングビューアのパネル
        private IMonitoringViewer m_monitoringViewer;

        // ファイル名領域
        private ToolStripStatusLabel m_fileLabel;

        // 選択中情報領域
        private ToolStripStatusLabel m_selectLabel;

        // 実行日時領域
        private ToolStripStatusLabel m_dateLabel;

        // エラーメッセージ表示の実装
        private StatusBarErrorMessageImpl m_errorMessageImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MonitoringViewerStatusBar() {
            InitializeComponent();

            this.SuspendLayout();
            
            m_fileLabel = CreateLabel(118);
            m_fileLabel.Spring = true;
            m_selectLabel = CreateLabel(50);
            m_selectLabel.AutoSize = true;
            m_dateLabel = CreateLabel(40);
            m_dateLabel.AutoSize = true;

            this.ImageList = UIIconManager.IconImageList;
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {m_fileLabel, m_selectLabel, m_dateLabel});

            m_errorMessageImpl = new StatusBarErrorMessageImpl(this, this.components, m_fileLabel, new StatusBarErrorMessageImpl.RefreshStatusBarDelegate(this.RefreshStatusBar));

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
            label.AutoSize = false;
            label.Size = new System.Drawing.Size(cx, 19);
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            label.Text = "";
            return label;
        }
        
        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]viewer  モニタリングビューアのパネル
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(IMonitoringViewer viewer) {
            m_monitoringViewer = viewer;
            RefreshStatusBar();
        }

        //=========================================================================================
        // 機　能：破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DisposeStatusBar() {
            m_errorMessageImpl.Dispose();
        }

        //=========================================================================================
        // 機　能：ステータスバーを更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshStatusBar() {
            // メイン領域
            bool done = m_errorMessageImpl.RefreshStatusBar();
            if (!done) {
                if (m_monitoringViewer.Available) {
                    m_fileLabel.Text = m_monitoringViewer.Title;
                } else {
                    m_fileLabel.Text = Resources.MonitorView_Loading;
                }
            }

            // 検索結果領域
            if (m_monitoringViewer.Available && m_monitoringViewer.MatrixData != null) {
                MatrixData data = m_monitoringViewer.MatrixData;
                if (data.SearchKeyword == null) {
                    m_selectLabel.Text = "";
                } else {
                    string select = string.Format(Resources.MonitorView_Select, data.SearchHitLineCount, data.SearchHitColumnCount);
                    m_selectLabel.Text = select;
                }
            } else {
                m_selectLabel.Text = "";
            }
        }

        //=========================================================================================
        // 機　能：エラーメッセージを更新する
        // 引　数：[in]message   エラーメッセージ
        // 　　　　[in]level     エラーのレベル
        // 　　　　[in]icon      使用するアイコン
        // 戻り値：なし
        //=========================================================================================
        public void ShowErrorMessage(string message, FileOperationStatus.LogLevel level, IconImageListID icon) {
            m_errorMessageImpl.ShowErrorMessageWorkThread(message, level, icon, StatusBarErrorMessageImpl.DisplayTime.Default);
        }
    }
}
