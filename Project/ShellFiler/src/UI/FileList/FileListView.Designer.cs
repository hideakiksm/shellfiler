namespace ShellFiler.UI.FileList
{
    partial class FileListView
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
            // FileListView
            // 
            this.AllowDrop = true;
            this.Name = "FileListView";
            this.Size = new System.Drawing.Size(100, 150);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.FileListView_MouseWheel);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.fileListView_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FileListView_MouseMove);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.FileListView_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FileListView_MouseDown);
            this.Resize += new System.EventHandler(this.FileListView_Resize);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FileListView_MouseUp);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileListView_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileListView_DragEnter);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
