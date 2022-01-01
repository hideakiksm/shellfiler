namespace ShellFiler.UI.FileList
{
    partial class FilePanel
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
            this.fileViewStatus = new ShellFiler.UI.FileStatusBar();
            this.fileListView = new ShellFiler.UI.FileList.FileListView();
            this.addressBar = new ShellFiler.UI.FileList.AddressBarStrip();
            this.fileViewStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileViewStatus
            // 
            this.fileViewStatus.Location = new System.Drawing.Point(0, 311);
            this.fileViewStatus.Name = "fileViewStatus";
            this.fileViewStatus.Size = new System.Drawing.Size(523, 22);
            this.fileViewStatus.SizingGrip = false;
            this.fileViewStatus.TabIndex = 1;
            this.fileViewStatus.Text = "statusStrip";
            // 
            // fileListView
            // 
            this.fileListView.BackColor = System.Drawing.SystemColors.Window;
            this.fileListView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.fileListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileListView.Location = new System.Drawing.Point(0, 25);
            this.fileListView.Name = "fileListView";
            this.fileListView.Size = new System.Drawing.Size(523, 286);
            this.fileListView.TabIndex = 3;
            // 
            // addressBar
            // 
            this.addressBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.addressBar.Location = new System.Drawing.Point(0, 0);
            this.addressBar.Name = "addressBar";
            this.addressBar.Size = new System.Drawing.Size(523, 25);
            this.addressBar.TabIndex = 0;
            this.addressBar.Text = "toolStrip1";
            // 
            // FilePanel
            // 
            this.Controls.Add(this.fileListView);
            this.Controls.Add(this.fileViewStatus);
            this.Controls.Add(this.addressBar);
            this.Name = "FilePanel";
            this.Size = new System.Drawing.Size(523, 333);
            this.fileViewStatus.ResumeLayout(false);
            this.fileViewStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AddressBarStrip addressBar;
        private ShellFiler.UI.FileStatusBar fileViewStatus;
        private FileListView fileListView;
    }
}
