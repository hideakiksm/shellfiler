using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.GraphicsViewer;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイルビューア＞拡大表示 の設定ページ
    //=========================================================================================
    public partial class GraphicsViewerViewPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        // アニメーションの状態（trueとfalseを2秒間隔で反転）
        private bool m_animationMode = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerViewPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;
            this.timerAnimation.Start();

            // コンフィグ値をUIに反映
            SetInitialValue(Configuration.Current);
            EnableUIItem();
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFormClosed() {
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            if (this.radioOriginal.Checked) {
                this.checkBoxShrinkOnly.Enabled = false;
            } else {
                this.checkBoxShrinkOnly.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：UIに初期値を設定する
        // 引　数：[in]config  取得対象のコンフィグ
        // 戻り値：なし
        //=========================================================================================
        private void SetInitialValue(Configuration config) {
            GraphicsViewerAutoZoomMode zoomMode = config.GraphicsViewerAutoZoomMode;
            if (zoomMode == GraphicsViewerAutoZoomMode.AlwaysOriginal) {
                this.radioOriginal.Checked = true;
            } else if (zoomMode == GraphicsViewerAutoZoomMode.AutoZoomAlways) {
                this.radioAutoZoom.Checked = true;
            } else if (zoomMode == GraphicsViewerAutoZoomMode.AutoZoom) {
                this.radioAutoZoomClick.Checked = true;
            } else if (zoomMode == GraphicsViewerAutoZoomMode.AutoZoomWideAlways) {
                this.radioAutoWide.Checked = true;
            } else if (zoomMode == GraphicsViewerAutoZoomMode.AutoZoomWide) {
                this.radioAutoWideClick.Checked = true;
            } else {
                this.radioAutoZoomClick.Checked = true;
            }
            this.checkBoxShrinkOnly.Checked = config.GraphicsViewerZoomInLarger;
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            GraphicsViewerAutoZoomMode zoomMode = GraphicsViewerAutoZoomMode.AutoZoom;
            if (this.radioOriginal.Checked) {
                zoomMode = GraphicsViewerAutoZoomMode.AlwaysOriginal;
            } else if (this.radioAutoZoom.Checked) {
                zoomMode = GraphicsViewerAutoZoomMode.AutoZoomAlways;
            } else if (this.radioAutoZoomClick.Checked) {
                zoomMode = GraphicsViewerAutoZoomMode.AutoZoom;
            } else if (this.radioAutoWide.Checked) {
                zoomMode = GraphicsViewerAutoZoomMode.AutoZoomWideAlways;
            } else if (this.radioAutoWideClick.Checked) {
                zoomMode = GraphicsViewerAutoZoomMode.AutoZoomWide;
            }

            Configuration.Current.GraphicsViewerAutoZoomMode = zoomMode;
            Configuration.Current.GraphicsViewerZoomInLarger = this.checkBoxShrinkOnly.Checked;

            this.timerAnimation.Stop();
            this.timerAnimation.Dispose();

            return true;
        }

        //=========================================================================================
        // 機　能：ページ内の設定をデフォルトに戻す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetDefault() {
            Configuration org = new Configuration();
            SetInitialValue(org);
        }

        //=========================================================================================
        // 機　能：再描画イベントが発生したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void panelSample_Paint(object sender, PaintEventArgs evt) {
            // 状態を決定
            GraphicsViewerAutoZoomMode zoomMode = null;
            if (this.radioOriginal.Checked) {
                zoomMode = GraphicsViewerAutoZoomMode.AlwaysOriginal;
            } else if (this.radioAutoZoom.Checked) {
                zoomMode = GraphicsViewerAutoZoomMode.AutoZoom;
            } else if (this.radioAutoZoomClick.Checked) {
                if (m_animationMode) {
                    zoomMode = GraphicsViewerAutoZoomMode.AlwaysOriginal;
                } else {
                    zoomMode = GraphicsViewerAutoZoomMode.AutoZoom;
                }
            } else if (this.radioAutoWide.Checked) {
                zoomMode = GraphicsViewerAutoZoomMode.AutoZoomWide;
            } else if (this.radioAutoWideClick.Checked) {
                if (m_animationMode) {
                    zoomMode = GraphicsViewerAutoZoomMode.AlwaysOriginal;
                } else {
                    zoomMode = GraphicsViewerAutoZoomMode.AutoZoomWide;
                }
            }

            // 描画
            DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, this.panelSample.Width, this.panelSample.Height);
            try {
                Graphics g = doubleBuffer.DrawingGraphics;
                g.FillRectangle(Brushes.Black, doubleBuffer.DrawingRectangle);
                if (zoomMode == GraphicsViewerAutoZoomMode.AlwaysOriginal) {
                    Bitmap image = UIIconManager.GraphicsViewer_FilterSampleZoom;
                    Rectangle rcImage = new Rectangle(0, 0, image.Width, image.Height);
                    g.DrawImage(image, rcImage);
                } else if (zoomMode == GraphicsViewerAutoZoomMode.AutoZoom) {
                    Bitmap image = UIIconManager.GraphicsViewer_FilterSample;
                    int width = (int)(((float)(this.panelSample.Height) / (float)(image.Height)) * ((float)(image.Width)));
                    Rectangle rcImage = new Rectangle(0, 0, image.Width, image.Height);
                    Rectangle rcDest = new Rectangle((this.panelSample.Width - width) / 2, 0, width, this.panelSample.Height);
                    g.DrawImage(image, rcDest, rcImage, GraphicsUnit.Pixel);
                } else {
                    Bitmap image = UIIconManager.GraphicsViewer_FilterSample;
                    Rectangle rcSrc = new Rectangle(0, 0, this.panelSample.Width, this.panelSample.Height);
                    g.DrawImage(image, rcSrc, rcSrc, GraphicsUnit.Pixel);
                }
            } finally {
                doubleBuffer.FlushScreen(0, 0);
            }
        }

        //=========================================================================================
        // 機　能：タイマーがタイムアウトしたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TimerAnimation_Tick(object sender, EventArgs evt) {
            m_animationMode = !m_animationMode;
            this.panelSample.Invalidate();
        }

        //=========================================================================================
        // 機　能：ラジオボタンの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButton_CheckedChanged(object sender, EventArgs evt) {
            this.panelSample.Invalidate();
            EnableUIItem();
        }
    }
}