namespace ShellFiler.GraphicsViewer {
    partial class GraphicsViewerPanel {
        /// <summary> 
        /// 必要なデザイナ変数です。
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

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // GraphicsViewerPanel
            // 
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MouseLeave += new System.EventHandler(this.GraphicsViewerPanel_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GraphicsViewerPanel_MouseMove);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.GraphicsViewerPanel_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphicsViewerPanel_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GraphicsViewerPanel_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
