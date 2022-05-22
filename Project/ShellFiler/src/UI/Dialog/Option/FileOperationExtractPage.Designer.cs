namespace ShellFiler.UI.Dialog.Option {
    partial class FileOperationExtractPage {
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
            this.radioButtonSameExist = new System.Windows.Forms.RadioButton();
            this.radioButtonAlwaysNew = new System.Windows.Forms.RadioButton();
            this.radioButtonDirect = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonSameExist);
            this.groupBox1.Controls.Add(this.radioButtonAlwaysNew);
            this.groupBox1.Controls.Add(this.radioButtonDirect);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(512, 109);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "展開先";
            // 
            // radioButtonSameExist
            // 
            this.radioButtonSameExist.AutoSize = true;
            this.radioButtonSameExist.Location = new System.Drawing.Point(9, 79);
            this.radioButtonSameExist.Name = "radioButtonSameExist";
            this.radioButtonSameExist.Size = new System.Drawing.Size(346, 19);
            this.radioButtonSameExist.TabIndex = 1;
            this.radioButtonSameExist.TabStop = true;
            this.radioButtonSameExist.Text = "格納ファイルが反対パスと混ざる場合だけフォルダを作成して展開する";
            this.radioButtonSameExist.UseVisualStyleBackColor = true;
            // 
            // radioButtonAlwaysNew
            // 
            this.radioButtonAlwaysNew.AutoSize = true;
            this.radioButtonAlwaysNew.Location = new System.Drawing.Point(9, 57);
            this.radioButtonAlwaysNew.Name = "radioButtonAlwaysNew";
            this.radioButtonAlwaysNew.Size = new System.Drawing.Size(293, 19);
            this.radioButtonAlwaysNew.TabIndex = 1;
            this.radioButtonAlwaysNew.TabStop = true;
            this.radioButtonAlwaysNew.Text = "反対パスにアーカイブ名と同じフォルダを作成して展開する";
            this.radioButtonAlwaysNew.UseVisualStyleBackColor = true;
            // 
            // radioButtonDirect
            // 
            this.radioButtonDirect.AutoSize = true;
            this.radioButtonDirect.Location = new System.Drawing.Point(9, 35);
            this.radioButtonDirect.Name = "radioButtonDirect";
            this.radioButtonDirect.Size = new System.Drawing.Size(157, 19);
            this.radioButtonDirect.TabIndex = 1;
            this.radioButtonDirect.TabStop = true;
            this.radioButtonDirect.Text = "反対パスにそのまま展開する";
            this.radioButtonDirect.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(312, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "圧縮ファイルの展開時にフォルダを作成するかどうかを指定します。";
            // 
            // FileOperationExtractPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "FileOperationExtractPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonSameExist;
        private System.Windows.Forms.RadioButton radioButtonAlwaysNew;
        private System.Windows.Forms.RadioButton radioButtonDirect;
        private System.Windows.Forms.Label label1;


    }
}
