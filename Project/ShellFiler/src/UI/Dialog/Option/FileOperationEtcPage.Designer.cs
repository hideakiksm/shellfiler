namespace ShellFiler.UI.Dialog.Option {
    partial class FileOperationEtcPage {
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
            this.comboBoxIncremental = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxMkDirMove = new System.Windows.Forms.ComboBox();
            this.textBoxMkDirSSH = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxMkDirWindows = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBoxSSHExecOutput = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxWindowsExecOutput = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonResetMirror = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxMirrorExcept = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxIncremental);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(514, 50);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "インクリメンタルサーチ";
            // 
            // comboBoxIncremental
            // 
            this.comboBoxIncremental.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIncremental.FormattingEnabled = true;
            this.comboBoxIncremental.Location = new System.Drawing.Point(227, 18);
            this.comboBoxIncremental.Name = "comboBoxIncremental";
            this.comboBoxIncremental.Size = new System.Drawing.Size(255, 23);
            this.comboBoxIncremental.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(204, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "[ファイル名を先頭から比較]の初期値(&I):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBoxMkDirMove);
            this.groupBox2.Controls.Add(this.textBoxMkDirSSH);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxMkDirWindows);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(3, 59);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(514, 102);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "フォルダの作成";
            // 
            // comboBoxMkDirMove
            // 
            this.comboBoxMkDirMove.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMkDirMove.FormattingEnabled = true;
            this.comboBoxMkDirMove.Location = new System.Drawing.Point(227, 72);
            this.comboBoxMkDirMove.Name = "comboBoxMkDirMove";
            this.comboBoxMkDirMove.Size = new System.Drawing.Size(255, 23);
            this.comboBoxMkDirMove.TabIndex = 5;
            // 
            // textBoxMkDirSSH
            // 
            this.textBoxMkDirSSH.Location = new System.Drawing.Point(227, 45);
            this.textBoxMkDirSSH.Name = "textBoxMkDirSSH";
            this.textBoxMkDirSSH.Size = new System.Drawing.Size(255, 23);
            this.textBoxMkDirSSH.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(192, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "[作成したフォルダを開く]の初期値(&M):";
            // 
            // textBoxMkDirWindows
            // 
            this.textBoxMkDirWindows.Location = new System.Drawing.Point(227, 18);
            this.textBoxMkDirWindows.Name = "textBoxMkDirWindows";
            this.textBoxMkDirWindows.Size = new System.Drawing.Size(255, 23);
            this.textBoxMkDirWindows.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(192, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "SSHでの新規フォルダ名の初期値(&S):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(223, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Windowsでの新規フォルダ名の初期値(&W):";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBoxSSHExecOutput);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.comboBoxWindowsExecOutput);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(3, 166);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(514, 76);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "コマンドの実行結果";
            // 
            // comboBoxSSHExecOutput
            // 
            this.comboBoxSSHExecOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSSHExecOutput.FormattingEnabled = true;
            this.comboBoxSSHExecOutput.Location = new System.Drawing.Point(227, 46);
            this.comboBoxSSHExecOutput.Name = "comboBoxSSHExecOutput";
            this.comboBoxSSHExecOutput.Size = new System.Drawing.Size(255, 23);
            this.comboBoxSSHExecOutput.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(169, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "SSHでの標準出力の初期値(&T):";
            // 
            // comboBoxWindowsExecOutput
            // 
            this.comboBoxWindowsExecOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWindowsExecOutput.FormattingEnabled = true;
            this.comboBoxWindowsExecOutput.Location = new System.Drawing.Point(227, 18);
            this.comboBoxWindowsExecOutput.Name = "comboBoxWindowsExecOutput";
            this.comboBoxWindowsExecOutput.Size = new System.Drawing.Size(255, 23);
            this.comboBoxWindowsExecOutput.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(197, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "Windowsでの標準出力の初期値(&O):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 331);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(453, 15);
            this.label6.TabIndex = 4;
            this.label6.Text = "いずれの項目も、それぞれの機能を実行したときに表示されるダイアログの初期値を指定します。";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonResetMirror);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.textBoxMirrorExcept);
            this.groupBox4.Location = new System.Drawing.Point(3, 246);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(514, 78);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ミラーコピー";
            // 
            // buttonResetMirror
            // 
            this.buttonResetMirror.Location = new System.Drawing.Point(433, 41);
            this.buttonResetMirror.Name = "buttonResetMirror";
            this.buttonResetMirror.Size = new System.Drawing.Size(75, 23);
            this.buttonResetMirror.TabIndex = 2;
            this.buttonResetMirror.Text = "既定値(&R)";
            this.buttonResetMirror.UseVisualStyleBackColor = true;
            this.buttonResetMirror.Click += new System.EventHandler(this.buttonResetMirror_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(25, 54);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(391, 15);
            this.label10.TabIndex = 4;
            this.label10.Text = "転送先にあった場合は削除されます。「:」区切りでワイルドカード指定が可能です。";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(25, 40);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(391, 15);
            this.label9.TabIndex = 3;
            this.label9.Text = "転送元にこれらのファイルやフォルダが存在しても、存在しなかったものとみなします。";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(150, 15);
            this.label8.TabIndex = 0;
            this.label8.Text = "除外するファイルとフォルダ(&X):";
            // 
            // textBoxMirrorExcept
            // 
            this.textBoxMirrorExcept.Location = new System.Drawing.Point(227, 16);
            this.textBoxMirrorExcept.Name = "textBoxMirrorExcept";
            this.textBoxMirrorExcept.Size = new System.Drawing.Size(281, 23);
            this.textBoxMirrorExcept.TabIndex = 1;
            // 
            // FileOperationEtcPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "FileOperationEtcPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBoxIncremental;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxMkDirMove;
        private System.Windows.Forms.TextBox textBoxMkDirSSH;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxMkDirWindows;
        private System.Windows.Forms.ComboBox comboBoxWindowsExecOutput;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxSSHExecOutput;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button buttonResetMirror;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxMirrorExcept;

    }
}
