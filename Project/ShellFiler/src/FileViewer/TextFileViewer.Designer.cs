namespace ShellFiler.FileViewer {
    partial class TextFileViewer {
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
            this.components = new System.ComponentModel.Container();
            this.visualBellTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // visualBellTimer
            // 
            this.visualBellTimer.Interval = 20;
            this.visualBellTimer.Tick += new System.EventHandler(this.visualBellTimer_Tick);
            // 
            // TextFileViewer
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TextFileViewer_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TextFileViewer_MouseMove);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TextFileViewer_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextFileViewer_MouseDown);
            this.Resize += new System.EventHandler(this.TextFileViewer_Resize);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TextFileViewer_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer visualBellTimer;
    }
}
