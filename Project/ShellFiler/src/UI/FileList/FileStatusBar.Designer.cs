namespace ShellFiler.UI
{
    partial class FileStatusBar
    {

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerAnimation = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerAnimation
            // 
            this.timerAnimation.Interval = 80;
            this.timerAnimation.Tick += new System.EventHandler(this.timerAnimation_Tick);
            // 
            // FileStatusBar
            // 
            this.DoubleClick += new System.EventHandler(this.FileStatusBar_DoubleClick);
            this.Click += new System.EventHandler(this.FileStatusBar_Click);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Timer timerAnimation;
    }
}
