namespace ShellFiler.UI.Dialog.Option {
    partial class InstallWorkingDirPage {
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
            this.textBoxTempDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonTempRef = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonTempFix = new System.Windows.Forms.RadioButton();
            this.radioButtonTempAuto = new System.Windows.Forms.RadioButton();
            this.textBoxTempAuto = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxTempDir
            // 
            this.textBoxTempDir.Location = new System.Drawing.Point(33, 105);
            this.textBoxTempDir.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxTempDir.Name = "textBoxTempDir";
            this.textBoxTempDir.Size = new System.Drawing.Size(465, 23);
            this.textBoxTempDir.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 145);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(457, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "作業フォルダは、圧縮ファイルの処理や、SSHサーバーからの一時的なダウンロードで使用します。";
            // 
            // buttonTempRef
            // 
            this.buttonTempRef.Location = new System.Drawing.Point(505, 102);
            this.buttonTempRef.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonTempRef.Name = "buttonTempRef";
            this.buttonTempRef.Size = new System.Drawing.Size(88, 29);
            this.buttonTempRef.TabIndex = 4;
            this.buttonTempRef.Text = "参照(&R)...";
            this.buttonTempRef.UseVisualStyleBackColor = true;
            this.buttonTempRef.Click += new System.EventHandler(this.buttonTempRef_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonTempFix);
            this.groupBox1.Controls.Add(this.radioButtonTempAuto);
            this.groupBox1.Controls.Add(this.textBoxTempAuto);
            this.groupBox1.Controls.Add(this.textBoxTempDir);
            this.groupBox1.Controls.Add(this.buttonTempRef);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(600, 200);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "作業フォルダ";
            // 
            // radioButtonTempFix
            // 
            this.radioButtonTempFix.AutoSize = true;
            this.radioButtonTempFix.Location = new System.Drawing.Point(9, 81);
            this.radioButtonTempFix.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonTempFix.Name = "radioButtonTempFix";
            this.radioButtonTempFix.Size = new System.Drawing.Size(207, 19);
            this.radioButtonTempFix.TabIndex = 2;
            this.radioButtonTempFix.TabStop = true;
            this.radioButtonTempFix.Text = "指定フォルダを作業フォルダに設定(&W)";
            this.radioButtonTempFix.UseVisualStyleBackColor = true;
            this.radioButtonTempFix.CheckedChanged += new System.EventHandler(this.radioButtonTemp_CheckedChanged);
            // 
            // radioButtonTempAuto
            // 
            this.radioButtonTempAuto.AutoSize = true;
            this.radioButtonTempAuto.Location = new System.Drawing.Point(9, 22);
            this.radioButtonTempAuto.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonTempAuto.Name = "radioButtonTempAuto";
            this.radioButtonTempAuto.Size = new System.Drawing.Size(112, 19);
            this.radioButtonTempAuto.TabIndex = 0;
            this.radioButtonTempAuto.TabStop = true;
            this.radioButtonTempAuto.Text = "自動的に設定(&A)";
            this.radioButtonTempAuto.UseVisualStyleBackColor = true;
            // 
            // textBoxTempAuto
            // 
            this.textBoxTempAuto.Location = new System.Drawing.Point(33, 46);
            this.textBoxTempAuto.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxTempAuto.Name = "textBoxTempAuto";
            this.textBoxTempAuto.ReadOnly = true;
            this.textBoxTempAuto.Size = new System.Drawing.Size(465, 23);
            this.textBoxTempAuto.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 177);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "次回起動時に有効です。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 162);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(214, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "変更時、旧フォルダは手操作で削除します。";
            // 
            // InstallWorkingDirPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "InstallWorkingDirPage";
            this.Size = new System.Drawing.Size(607, 462);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxTempDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonTempRef;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButtonTempAuto;
        private System.Windows.Forms.RadioButton radioButtonTempFix;
        private System.Windows.Forms.TextBox textBoxTempAuto;
        private System.Windows.Forms.Label label1;
    }
}
