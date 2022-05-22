namespace ShellFiler.UI.Dialog.Option {
    partial class FuncGeneralPage {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonSplit5 = new System.Windows.Forms.RadioButton();
            this.radioButtonSplit4 = new System.Windows.Forms.RadioButton();
            this.radioButtonSplit0 = new System.Windows.Forms.RadioButton();
            this.checkBoxOverray = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonSplit5);
            this.groupBox1.Controls.Add(this.radioButtonSplit4);
            this.groupBox1.Controls.Add(this.radioButtonSplit0);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(165, 91);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ファンクションキーのスタイル";
            // 
            // radioButtonSplit5
            // 
            this.radioButtonSplit5.AutoSize = true;
            this.radioButtonSplit5.Location = new System.Drawing.Point(6, 62);
            this.radioButtonSplit5.Name = "radioButtonSplit5";
            this.radioButtonSplit5.Size = new System.Drawing.Size(150, 19);
            this.radioButtonSplit5.TabIndex = 2;
            this.radioButtonSplit5.TabStop = true;
            this.radioButtonSplit5.Text = "F1～F12で5個区切り(&5)";
            this.radioButtonSplit5.UseVisualStyleBackColor = true;
            // 
            // radioButtonSplit4
            // 
            this.radioButtonSplit4.AutoSize = true;
            this.radioButtonSplit4.Location = new System.Drawing.Point(6, 40);
            this.radioButtonSplit4.Name = "radioButtonSplit4";
            this.radioButtonSplit4.Size = new System.Drawing.Size(150, 19);
            this.radioButtonSplit4.TabIndex = 1;
            this.radioButtonSplit4.TabStop = true;
            this.radioButtonSplit4.Text = "F1～F12で4個区切り(&4)";
            this.radioButtonSplit4.UseVisualStyleBackColor = true;
            // 
            // radioButtonSplit0
            // 
            this.radioButtonSplit0.AutoSize = true;
            this.radioButtonSplit0.Location = new System.Drawing.Point(6, 18);
            this.radioButtonSplit0.Name = "radioButtonSplit0";
            this.radioButtonSplit0.Size = new System.Drawing.Size(150, 19);
            this.radioButtonSplit0.TabIndex = 0;
            this.radioButtonSplit0.TabStop = true;
            this.radioButtonSplit0.Text = "F1～F12で区切りなし(&0)";
            this.radioButtonSplit0.UseVisualStyleBackColor = true;
            // 
            // checkBoxOverray
            // 
            this.checkBoxOverray.AutoSize = true;
            this.checkBoxOverray.Location = new System.Drawing.Point(4, 101);
            this.checkBoxOverray.Name = "checkBoxOverray";
            this.checkBoxOverray.Size = new System.Drawing.Size(230, 19);
            this.checkBoxOverray.TabIndex = 1;
            this.checkBoxOverray.Text = "オーバーレイアイコンでキー名を表記する(&K)";
            this.checkBoxOverray.UseVisualStyleBackColor = true;
            // 
            // FuncGeneralPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.checkBoxOverray);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "FuncGeneralPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonSplit5;
        private System.Windows.Forms.RadioButton radioButtonSplit4;
        private System.Windows.Forms.RadioButton radioButtonSplit0;
        private System.Windows.Forms.CheckBox checkBoxOverray;

    }
}
