namespace ShellFiler.UI.ControlBar
{
    partial class FunctionBar
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
            // FunctionBar
            // 
            this.Name = "FunctionBar";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FunctionBar_Paint);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.FunctionBar_PreviewKeyDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FunctionBar_MouseMove);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FunctionBar_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FunctionBar_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FunctionBar_MouseUp);
            this.SizeChanged += new System.EventHandler(this.FunctionBar_SizeChanged);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(FunctionBar_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FunctionBar_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
