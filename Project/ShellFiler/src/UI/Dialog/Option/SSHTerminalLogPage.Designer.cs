namespace ShellFiler.UI.Dialog.Option {
    partial class SSHTerminalLogPage {
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
            this.radioFileIntegrate = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioFileEachSession = new System.Windows.Forms.RadioButton();
            this.radioFileNone = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericRotationCount = new System.Windows.Forms.NumericUpDown();
            this.numericRotationSize = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFolder = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonFolderRef = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericRotationCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericRotationSize)).BeginInit();
            this.SuspendLayout();
            // 
            // radioFileIntegrate
            // 
            this.radioFileIntegrate.AutoSize = true;
            this.radioFileIntegrate.Location = new System.Drawing.Point(6, 40);
            this.radioFileIntegrate.Name = "radioFileIntegrate";
            this.radioFileIntegrate.Size = new System.Drawing.Size(267, 16);
            this.radioFileIntegrate.TabIndex = 1;
            this.radioFileIntegrate.TabStop = true;
            this.radioFileIntegrate.Text = "ShellFilerのプロセスごとに１つのログファイルに出力(&I)";
            this.radioFileIntegrate.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioFileEachSession);
            this.groupBox1.Controls.Add(this.radioFileNone);
            this.groupBox1.Controls.Add(this.radioFileIntegrate);
            this.groupBox1.Location = new System.Drawing.Point(6, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(511, 86);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ログファイル形式";
            // 
            // radioFileEachSession
            // 
            this.radioFileEachSession.AutoSize = true;
            this.radioFileEachSession.Location = new System.Drawing.Point(6, 62);
            this.radioFileEachSession.Name = "radioFileEachSession";
            this.radioFileEachSession.Size = new System.Drawing.Size(213, 16);
            this.radioFileEachSession.TabIndex = 2;
            this.radioFileEachSession.TabStop = true;
            this.radioFileEachSession.Text = "セッションごとに別のログファイルに出力(&E)";
            this.radioFileEachSession.UseVisualStyleBackColor = true;
            // 
            // radioFileNone
            // 
            this.radioFileNone.AutoSize = true;
            this.radioFileNone.Location = new System.Drawing.Point(6, 18);
            this.radioFileNone.Name = "radioFileNone";
            this.radioFileNone.Size = new System.Drawing.Size(153, 16);
            this.radioFileNone.TabIndex = 0;
            this.radioFileNone.TabStop = true;
            this.radioFileNone.Text = "ログファイルを出力しない(&N)";
            this.radioFileNone.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericRotationCount);
            this.groupBox2.Controls.Add(this.numericRotationSize);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(6, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(511, 70);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ローテーション";
            // 
            // numericRotationCount
            // 
            this.numericRotationCount.Location = new System.Drawing.Point(165, 42);
            this.numericRotationCount.Name = "numericRotationCount";
            this.numericRotationCount.Size = new System.Drawing.Size(120, 19);
            this.numericRotationCount.TabIndex = 4;
            // 
            // numericRotationSize
            // 
            this.numericRotationSize.Location = new System.Drawing.Point(165, 17);
            this.numericRotationSize.Name = "numericRotationSize";
            this.numericRotationSize.Size = new System.Drawing.Size(120, 19);
            this.numericRotationSize.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(291, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "KB";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "最大ファイル数(&C):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "1ファイルあたりの最大サイズ(&S):";
            // 
            // textBoxFolder
            // 
            this.textBoxFolder.Location = new System.Drawing.Point(35, 188);
            this.textBoxFolder.Name = "textBoxFolder";
            this.textBoxFolder.Size = new System.Drawing.Size(401, 19);
            this.textBoxFolder.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 173);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "ログ出力フォルダ(&F):";
            // 
            // buttonFolderRef
            // 
            this.buttonFolderRef.Location = new System.Drawing.Point(442, 186);
            this.buttonFolderRef.Name = "buttonFolderRef";
            this.buttonFolderRef.Size = new System.Drawing.Size(75, 23);
            this.buttonFolderRef.TabIndex = 4;
            this.buttonFolderRef.Text = "参照(&R)";
            this.buttonFolderRef.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(439, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "指定がない場合は既定の出力先(AppData\\Local\\ShellFiler\\TerminalLog)に出力します。";
            // 
            // SSHTerminalLogPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonFolderRef);
            this.Controls.Add(this.textBoxFolder);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.groupBox1);
            this.Name = "SSHTerminalLogPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericRotationCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericRotationSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioFileIntegrate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioFileEachSession;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numericRotationCount;
        private System.Windows.Forms.NumericUpDown numericRotationSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFolder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonFolderRef;
        private System.Windows.Forms.RadioButton radioFileNone;
        private System.Windows.Forms.Label label3;

    }
}
