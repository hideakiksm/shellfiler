namespace ShellFiler.UI.FileList {
    partial class AddressBarStrip {
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
            // AddressBarStrip
            // 
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AddressBarStrip_MouseClick);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.AddressBarStrip_Layout);
            this.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.AddressBarStrip_ItemClicked);
            this.ResumeLayout(false);

        }

        #endregion


    }
}
