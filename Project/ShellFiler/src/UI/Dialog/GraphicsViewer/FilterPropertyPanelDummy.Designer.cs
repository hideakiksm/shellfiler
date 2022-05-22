namespace ShellFiler.UI.Dialog.GraphicsViewer {
    partial class FilterPropertyPanelDummy {
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(165, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "フィルターが選択されていません。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(125, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(237, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "左画面で、適用するフィルターを追加してください。";
            // 
            // FilterPropertyPanelDummy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FilterPropertyPanelDummy";
            this.Size = new System.Drawing.Size(487, 122);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;

    }
}
