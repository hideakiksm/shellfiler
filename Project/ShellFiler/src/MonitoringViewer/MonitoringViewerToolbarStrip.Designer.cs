﻿namespace ShellFiler.MonitoringViewer {
    partial class MonitoringViewerToolbarStrip {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // FileViewerToolbarStrip
            // 
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FileViewerToolbarStrip_Paint);
            this.SizeChanged += new System.EventHandler(this.FileViewerToolbarStrip_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
