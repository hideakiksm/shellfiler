namespace ShellFiler.MonitoringViewer {
    partial class MatrixDataView {
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
            this.listView = new ShellFiler.Util.BufferedListView();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.OwnerDraw = true;
            this.listView.Size = new System.Drawing.Size(150, 150);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.VirtualMode = true;
            this.listView.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView_DrawColumnHeader);
            this.listView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView_DrawItem);
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            this.listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            this.listView.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ProcessView_PreviewKeyDown);
            this.listView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listView_RetrieveVirtualItem);
            this.listView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ProcessView_KeyUp);
            this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ProcessView_KeyDown);
            this.listView.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView_DrawSubItem);
            // 
            // MatrixDataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView);
            this.Name = "MatrixDataView";
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ProcessView_PreviewKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ProcessView_KeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ProcessView_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private ShellFiler.Util.BufferedListView listView;



    }
}
