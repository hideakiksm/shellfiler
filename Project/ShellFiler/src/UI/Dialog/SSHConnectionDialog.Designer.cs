namespace ShellFiler.UI.Dialog {
    partial class SSHConnectionDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label17 = new System.Windows.Forms.Label();
            this.comboBoxOS = new System.Windows.Forms.ComboBox();
            this.comboBoxEncode = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textBoxPass = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.comboBoxUser = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxPort = new System.Windows.Forms.ComboBox();
            this.comboBoxHost = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonCacnel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonPrivateKeyRef = new System.Windows.Forms.Button();
            this.linkLabelKeyGuide = new System.Windows.Forms.LinkLabel();
            this.linkLabelPassword = new System.Windows.Forms.LinkLabel();
            this.checkBoxSavePassword = new System.Windows.Forms.CheckBox();
            this.comboBoxAuthMethod = new System.Windows.Forms.ComboBox();
            this.textBoxPrivateKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxAutoRetry = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(11, 50);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(48, 15);
            this.label17.TabIndex = 2;
            this.label17.Text = "OS(&O):";
            // 
            // comboBoxOS
            // 
            this.comboBoxOS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOS.FormattingEnabled = true;
            this.comboBoxOS.Location = new System.Drawing.Point(111, 47);
            this.comboBoxOS.Name = "comboBoxOS";
            this.comboBoxOS.Size = new System.Drawing.Size(134, 23);
            this.comboBoxOS.TabIndex = 3;
            // 
            // comboBoxEncode
            // 
            this.comboBoxEncode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEncode.FormattingEnabled = true;
            this.comboBoxEncode.Location = new System.Drawing.Point(111, 18);
            this.comboBoxEncode.Name = "comboBoxEncode";
            this.comboBoxEncode.Size = new System.Drawing.Size(134, 23);
            this.comboBoxEncode.TabIndex = 1;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(11, 22);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(79, 15);
            this.label16.TabIndex = 0;
            this.label16.Text = "文字コード(&E):";
            // 
            // textBoxPass
            // 
            this.textBoxPass.Location = new System.Drawing.Point(111, 130);
            this.textBoxPass.Name = "textBoxPass";
            this.textBoxPass.PasswordChar = '*';
            this.textBoxPass.Size = new System.Drawing.Size(292, 23);
            this.textBoxPass.TabIndex = 9;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(10, 133);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(75, 15);
            this.labelPassword.TabIndex = 8;
            this.labelPassword.Text = "パスワード(&P):";
            // 
            // comboBoxUser
            // 
            this.comboBoxUser.FormattingEnabled = true;
            this.comboBoxUser.Location = new System.Drawing.Point(111, 74);
            this.comboBoxUser.Name = "comboBoxUser";
            this.comboBoxUser.Size = new System.Drawing.Size(292, 23);
            this.comboBoxUser.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 77);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 15);
            this.label9.TabIndex = 4;
            this.label9.Text = "ユーザー名:";
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.FormattingEnabled = true;
            this.comboBoxPort.Location = new System.Drawing.Point(111, 46);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(141, 23);
            this.comboBoxPort.TabIndex = 3;
            // 
            // comboBoxHost
            // 
            this.comboBoxHost.FormattingEnabled = true;
            this.comboBoxHost.Location = new System.Drawing.Point(111, 18);
            this.comboBoxHost.Name = "comboBoxHost";
            this.comboBoxHost.Size = new System.Drawing.Size(292, 23);
            this.comboBoxHost.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "ポート(&T):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "ホスト名(&H):";
            // 
            // buttonCacnel
            // 
            this.buttonCacnel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCacnel.Location = new System.Drawing.Point(346, 347);
            this.buttonCacnel.Name = "buttonCacnel";
            this.buttonCacnel.Size = new System.Drawing.Size(75, 23);
            this.buttonCacnel.TabIndex = 4;
            this.buttonCacnel.Text = "キャンセル";
            this.buttonCacnel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(265, 347);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonPrivateKeyRef);
            this.groupBox1.Controls.Add(this.linkLabelKeyGuide);
            this.groupBox1.Controls.Add(this.linkLabelPassword);
            this.groupBox1.Controls.Add(this.checkBoxSavePassword);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.comboBoxHost);
            this.groupBox1.Controls.Add(this.comboBoxPort);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.comboBoxAuthMethod);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.comboBoxUser);
            this.groupBox1.Controls.Add(this.textBoxPrivateKey);
            this.groupBox1.Controls.Add(this.textBoxPass);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.labelPassword);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(409, 239);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "接続先";
            // 
            // buttonPrivateKeyRef
            // 
            this.buttonPrivateKeyRef.Location = new System.Drawing.Point(328, 158);
            this.buttonPrivateKeyRef.Name = "buttonPrivateKeyRef";
            this.buttonPrivateKeyRef.Size = new System.Drawing.Size(75, 23);
            this.buttonPrivateKeyRef.TabIndex = 13;
            this.buttonPrivateKeyRef.Text = "参照(&R)...";
            this.buttonPrivateKeyRef.UseVisualStyleBackColor = true;
            this.buttonPrivateKeyRef.Click += new System.EventHandler(this.buttonKeyRef_Click);
            // 
            // linkLabelKeyGuide
            // 
            this.linkLabelKeyGuide.AutoSize = true;
            this.linkLabelKeyGuide.Location = new System.Drawing.Point(72, 164);
            this.linkLabelKeyGuide.Name = "linkLabelKeyGuide";
            this.linkLabelKeyGuide.Size = new System.Drawing.Size(13, 15);
            this.linkLabelKeyGuide.TabIndex = 11;
            this.linkLabelKeyGuide.TabStop = true;
            this.linkLabelKeyGuide.Text = "?";
            this.linkLabelKeyGuide.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // linkLabelPassword
            // 
            this.linkLabelPassword.AutoSize = true;
            this.linkLabelPassword.Location = new System.Drawing.Point(293, 210);
            this.linkLabelPassword.Name = "linkLabelPassword";
            this.linkLabelPassword.Size = new System.Drawing.Size(111, 15);
            this.linkLabelPassword.TabIndex = 16;
            this.linkLabelPassword.TabStop = true;
            this.linkLabelPassword.Text = "パスワード保存の注意";
            this.linkLabelPassword.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // checkBoxSavePassword
            // 
            this.checkBoxSavePassword.AutoSize = true;
            this.checkBoxSavePassword.Location = new System.Drawing.Point(111, 209);
            this.checkBoxSavePassword.Name = "checkBoxSavePassword";
            this.checkBoxSavePassword.Size = new System.Drawing.Size(127, 19);
            this.checkBoxSavePassword.TabIndex = 15;
            this.checkBoxSavePassword.Text = "パスワードを保存(&W)";
            this.checkBoxSavePassword.UseVisualStyleBackColor = true;
            this.checkBoxSavePassword.CheckedChanged += new System.EventHandler(this.checkBoxSavePassword_CheckedChanged);
            // 
            // comboBoxAuthMethod
            // 
            this.comboBoxAuthMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAuthMethod.FormattingEnabled = true;
            this.comboBoxAuthMethod.Location = new System.Drawing.Point(112, 102);
            this.comboBoxAuthMethod.Name = "comboBoxAuthMethod";
            this.comboBoxAuthMethod.Size = new System.Drawing.Size(291, 23);
            this.comboBoxAuthMethod.TabIndex = 7;
            // 
            // textBoxPrivateKey
            // 
            this.textBoxPrivateKey.Location = new System.Drawing.Point(111, 158);
            this.textBoxPrivateKey.Name = "textBoxPrivateKey";
            this.textBoxPrivateKey.Size = new System.Drawing.Size(211, 23);
            this.textBoxPrivateKey.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "認証方法(&M):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(110, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(203, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "指定したファイルは認証ごとに参照します。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 164);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "秘密鍵(&K):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.comboBoxEncode);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.comboBoxOS);
            this.groupBox2.Location = new System.Drawing.Point(12, 257);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(409, 81);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "詳細設定";
            // 
            // checkBoxAutoRetry
            // 
            this.checkBoxAutoRetry.AutoSize = true;
            this.checkBoxAutoRetry.Location = new System.Drawing.Point(12, 347);
            this.checkBoxAutoRetry.Name = "checkBoxAutoRetry";
            this.checkBoxAutoRetry.Size = new System.Drawing.Size(155, 19);
            this.checkBoxAutoRetry.TabIndex = 2;
            this.checkBoxAutoRetry.Text = "設定を保存して再接続(&A)";
            this.checkBoxAutoRetry.UseVisualStyleBackColor = true;
            // 
            // SSHConnectionDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCacnel;
            this.ClientSize = new System.Drawing.Size(433, 382);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCacnel);
            this.Controls.Add(this.checkBoxAutoRetry);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SSHConnectionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SSHログイン";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SSHConnectionDialog_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox comboBoxOS;
        private System.Windows.Forms.ComboBox comboBoxEncode;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBoxPass;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.ComboBox comboBoxUser;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBoxPort;
        private System.Windows.Forms.ComboBox comboBoxHost;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonCacnel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxSavePassword;
        private System.Windows.Forms.LinkLabel linkLabelPassword;
        private System.Windows.Forms.CheckBox checkBoxAutoRetry;
        private System.Windows.Forms.Button buttonPrivateKeyRef;
        private System.Windows.Forms.LinkLabel linkLabelKeyGuide;
        private System.Windows.Forms.ComboBox comboBoxAuthMethod;
        private System.Windows.Forms.TextBox textBoxPrivateKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}