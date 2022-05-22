namespace ShellFiler.UI.Dialog {
    partial class SSHChangeUserDialog {
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.checkBoxLoginShell = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.radioButtonSu = new System.Windows.Forms.RadioButton();
            this.radioButtonExit = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxCurrent = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "変更先ユーザー(&U):";
            // 
            // textBoxUser
            // 
            this.textBoxUser.Location = new System.Drawing.Point(146, 61);
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.Size = new System.Drawing.Size(132, 23);
            this.textBoxUser.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "パスワード(&P):";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(146, 88);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(132, 23);
            this.textBoxPassword.TabIndex = 7;
            // 
            // checkBoxLoginShell
            // 
            this.checkBoxLoginShell.AutoSize = true;
            this.checkBoxLoginShell.Location = new System.Drawing.Point(38, 117);
            this.checkBoxLoginShell.Name = "checkBoxLoginShell";
            this.checkBoxLoginShell.Size = new System.Drawing.Size(198, 19);
            this.checkBoxLoginShell.TabIndex = 9;
            this.checkBoxLoginShell.Text = "ログインシェルを使用して切り替え(&L)";
            this.checkBoxLoginShell.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(325, 195);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(244, 195);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 11;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(284, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "rootは入力なし";
            // 
            // radioButtonSu
            // 
            this.radioButtonSu.AutoSize = true;
            this.radioButtonSu.Location = new System.Drawing.Point(13, 40);
            this.radioButtonSu.Name = "radioButtonSu";
            this.radioButtonSu.Size = new System.Drawing.Size(173, 19);
            this.radioButtonSu.TabIndex = 2;
            this.radioButtonSu.TabStop = true;
            this.radioButtonSu.Text = "su:別のユーザーに切り替え(&S)";
            this.radioButtonSu.UseVisualStyleBackColor = true;
            this.radioButtonSu.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonExit
            // 
            this.radioButtonExit.AutoSize = true;
            this.radioButtonExit.Location = new System.Drawing.Point(13, 146);
            this.radioButtonExit.Name = "radioButtonExit";
            this.radioButtonExit.Size = new System.Drawing.Size(182, 19);
            this.radioButtonExit.TabIndex = 10;
            this.radioButtonExit.TabStop = true;
            this.radioButtonExit.Text = "exit:元のユーザーに切り替え(&X)";
            this.radioButtonExit.UseVisualStyleBackColor = true;
            this.radioButtonExit.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "現在のユーザー:";
            // 
            // textBoxCurrent
            // 
            this.textBoxCurrent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxCurrent.Location = new System.Drawing.Point(99, 12);
            this.textBoxCurrent.Name = "textBoxCurrent";
            this.textBoxCurrent.ReadOnly = true;
            this.textBoxCurrent.Size = new System.Drawing.Size(301, 16);
            this.textBoxCurrent.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 167);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(276, 15);
            this.label6.TabIndex = 8;
            this.label6.Text = "はじめのユーザーで実行すると、シェルの接続が切れます。";
            // 
            // SSHChangeUserDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(412, 230);
            this.Controls.Add(this.radioButtonExit);
            this.Controls.Add(this.radioButtonSu);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.checkBoxLoginShell);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxCurrent);
            this.Controls.Add(this.textBoxUser);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SSHChangeUserDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SSH操作ユーザーの変更";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.CheckBox checkBoxLoginShell;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButtonSu;
        private System.Windows.Forms.RadioButton radioButtonExit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxCurrent;
        private System.Windows.Forms.Label label6;
    }
}