namespace ShellFiler.UI.Dialog {
    partial class FileConditionNoneControl {
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
            this.labelNone = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelNone
            // 
            this.labelNone.AutoSize = true;
            this.labelNone.Location = new System.Drawing.Point(140, 18);
            this.labelNone.Name = "labelNone";
            this.labelNone.Size = new System.Drawing.Size(118, 12);
            this.labelNone.TabIndex = 3;
            this.labelNone.Text = "条件指定はありません。";
            // 
            // FileConditionNoneControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.labelNone);
            this.Name = "FileConditionNoneControl";
            this.Size = new System.Drawing.Size(398, 48);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelNone;
    }
}
