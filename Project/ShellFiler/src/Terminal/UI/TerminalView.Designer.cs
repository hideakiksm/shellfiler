namespace ShellFiler.Terminal.UI
{
    partial class TerminalView
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TerminalView
            // 
            this.AllowDrop = true;
            this.Size = new System.Drawing.Size(130, 146);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TerminalView_Paint);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TerminalView_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TerminalView_MouseClick);
            this.Resize += new System.EventHandler(this.TerminalView_Resize);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TerminalView_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TerminalView_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
