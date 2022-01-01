namespace ShellFiler.FileViewer {
    partial class RadarBar {
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
            // RadarBar
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Name = "TextFileViewer";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.RadarBar_Paint);
            this.SizeChanged += new System.EventHandler(this.RadarBar_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
