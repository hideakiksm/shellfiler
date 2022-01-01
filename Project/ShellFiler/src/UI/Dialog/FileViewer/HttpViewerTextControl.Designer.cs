namespace ShellFiler.UI.Dialog.FileViewer {
    partial class HttpViewerTextControl {
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
            this.textBoxRequestText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxEncoding = new System.Windows.Forms.ComboBox();
            this.buttonSample = new System.Windows.Forms.Button();
            this.buttonLength = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "リクエスト内容:";
            // 
            // textBoxRequestText
            // 
            this.textBoxRequestText.AcceptsReturn = true;
            this.textBoxRequestText.Location = new System.Drawing.Point(4, 20);
            this.textBoxRequestText.MaxLength = 1048576;
            this.textBoxRequestText.Multiline = true;
            this.textBoxRequestText.Name = "textBoxRequestText";
            this.textBoxRequestText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxRequestText.Size = new System.Drawing.Size(517, 162);
            this.textBoxRequestText.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 191);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "文字コード:";
            // 
            // comboBoxEncoding
            // 
            this.comboBoxEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEncoding.FormattingEnabled = true;
            this.comboBoxEncoding.Location = new System.Drawing.Point(70, 188);
            this.comboBoxEncoding.Name = "comboBoxEncoding";
            this.comboBoxEncoding.Size = new System.Drawing.Size(165, 20);
            this.comboBoxEncoding.TabIndex = 3;
            // 
            // buttonSample
            // 
            this.buttonSample.Location = new System.Drawing.Point(342, 185);
            this.buttonSample.Name = "buttonSample";
            this.buttonSample.Size = new System.Drawing.Size(75, 23);
            this.buttonSample.TabIndex = 4;
            this.buttonSample.Text = "サンプル";
            this.buttonSample.UseVisualStyleBackColor = true;
            this.buttonSample.Click += new System.EventHandler(this.buttonSample_Click);
            // 
            // buttonLength
            // 
            this.buttonLength.Location = new System.Drawing.Point(423, 185);
            this.buttonLength.Name = "buttonLength";
            this.buttonLength.Size = new System.Drawing.Size(98, 23);
            this.buttonLength.TabIndex = 5;
            this.buttonLength.Text = "Content-Length";
            this.buttonLength.UseVisualStyleBackColor = true;
            this.buttonLength.Click += new System.EventHandler(this.buttonContentLength_Click);
            // 
            // HttpViewerTextControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonLength);
            this.Controls.Add(this.buttonSample);
            this.Controls.Add(this.comboBoxEncoding);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxRequestText);
            this.Controls.Add(this.label1);
            this.Name = "HttpViewerTextControl";
            this.Size = new System.Drawing.Size(524, 212);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxRequestText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxEncoding;
        private System.Windows.Forms.Button buttonSample;
        private System.Windows.Forms.Button buttonLength;
    }
}
