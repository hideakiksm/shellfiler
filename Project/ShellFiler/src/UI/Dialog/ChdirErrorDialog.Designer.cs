namespace ShellFiler.UI.Dialog {
    partial class ChdirErrorDialog {
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
            this.textBoxFail = new System.Windows.Forms.TextBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.radioButtonRetry = new System.Windows.Forms.RadioButton();
            this.radioButtonParent = new System.Windows.Forms.RadioButton();
            this.radioButtonHome = new System.Windows.Forms.RadioButton();
            this.radioButtonDesktop = new System.Windows.Forms.RadioButton();
            this.textBoxNext = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonRoot = new System.Windows.Forms.RadioButton();
            this.radioButtonPrev = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxFail
            // 
            this.textBoxFail.Location = new System.Drawing.Point(127, 34);
            this.textBoxFail.Name = "textBoxFail";
            this.textBoxFail.ReadOnly = true;
            this.textBoxFail.Size = new System.Drawing.Size(345, 19);
            this.textBoxFail.TabIndex = 2;
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(56, 13);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(157, 12);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "フォルダを変更できませんでした。";
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Location = new System.Drawing.Point(13, 13);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxIcon.TabIndex = 2;
            this.pictureBoxIcon.TabStop = false;
            // 
            // radioButtonRetry
            // 
            this.radioButtonRetry.AutoSize = true;
            this.radioButtonRetry.Location = new System.Drawing.Point(6, 18);
            this.radioButtonRetry.Name = "radioButtonRetry";
            this.radioButtonRetry.Size = new System.Drawing.Size(75, 16);
            this.radioButtonRetry.TabIndex = 0;
            this.radioButtonRetry.TabStop = true;
            this.radioButtonRetry.Text = "再試行(&R)";
            this.radioButtonRetry.UseVisualStyleBackColor = true;
            this.radioButtonRetry.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonParent
            // 
            this.radioButtonParent.AutoSize = true;
            this.radioButtonParent.Location = new System.Drawing.Point(6, 84);
            this.radioButtonParent.Name = "radioButtonParent";
            this.radioButtonParent.Size = new System.Drawing.Size(119, 16);
            this.radioButtonParent.TabIndex = 3;
            this.radioButtonParent.TabStop = true;
            this.radioButtonParent.Text = "親フォルダへ移動(&P)";
            this.radioButtonParent.UseVisualStyleBackColor = true;
            this.radioButtonParent.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonHome
            // 
            this.radioButtonHome.AutoSize = true;
            this.radioButtonHome.Location = new System.Drawing.Point(6, 106);
            this.radioButtonHome.Name = "radioButtonHome";
            this.radioButtonHome.Size = new System.Drawing.Size(138, 16);
            this.radioButtonHome.TabIndex = 4;
            this.radioButtonHome.TabStop = true;
            this.radioButtonHome.Text = "ホームフォルダへ移動(&H)";
            this.radioButtonHome.UseVisualStyleBackColor = true;
            this.radioButtonHome.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonDesktop
            // 
            this.radioButtonDesktop.AutoSize = true;
            this.radioButtonDesktop.Location = new System.Drawing.Point(6, 126);
            this.radioButtonDesktop.Name = "radioButtonDesktop";
            this.radioButtonDesktop.Size = new System.Drawing.Size(168, 16);
            this.radioButtonDesktop.TabIndex = 5;
            this.radioButtonDesktop.TabStop = true;
            this.radioButtonDesktop.Text = "Windowsデスクトップへ移動(&D)";
            this.radioButtonDesktop.UseVisualStyleBackColor = true;
            this.radioButtonDesktop.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // textBoxNext
            // 
            this.textBoxNext.Location = new System.Drawing.Point(126, 60);
            this.textBoxNext.Name = "textBoxNext";
            this.textBoxNext.Size = new System.Drawing.Size(345, 19);
            this.textBoxNext.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(57, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "失敗：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(56, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "再試行(&F):";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(396, 241);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(315, 241);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonRetry);
            this.groupBox1.Controls.Add(this.radioButtonRoot);
            this.groupBox1.Controls.Add(this.radioButtonPrev);
            this.groupBox1.Controls.Add(this.radioButtonParent);
            this.groupBox1.Controls.Add(this.radioButtonHome);
            this.groupBox1.Controls.Add(this.radioButtonDesktop);
            this.groupBox1.Location = new System.Drawing.Point(13, 86);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(219, 149);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "操作";
            // 
            // radioButtonRoot
            // 
            this.radioButtonRoot.AutoSize = true;
            this.radioButtonRoot.Location = new System.Drawing.Point(6, 62);
            this.radioButtonRoot.Name = "radioButtonRoot";
            this.radioButtonRoot.Size = new System.Drawing.Size(135, 16);
            this.radioButtonRoot.TabIndex = 2;
            this.radioButtonRoot.TabStop = true;
            this.radioButtonRoot.Text = "ルートフォルダへ移動(&T)";
            this.radioButtonRoot.UseVisualStyleBackColor = true;
            this.radioButtonRoot.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonPrev
            // 
            this.radioButtonPrev.AutoSize = true;
            this.radioButtonPrev.Location = new System.Drawing.Point(6, 40);
            this.radioButtonPrev.Name = "radioButtonPrev";
            this.radioButtonPrev.Size = new System.Drawing.Size(142, 16);
            this.radioButtonPrev.TabIndex = 1;
            this.radioButtonPrev.TabStop = true;
            this.radioButtonPrev.Text = "直前のフォルダへ移動(&V)";
            this.radioButtonPrev.UseVisualStyleBackColor = true;
            this.radioButtonPrev.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // ChdirErrorDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(483, 276);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.textBoxNext);
            this.Controls.Add(this.textBoxFail);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChdirErrorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ShellFiler";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxFail;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.RadioButton radioButtonRetry;
        private System.Windows.Forms.RadioButton radioButtonParent;
        private System.Windows.Forms.RadioButton radioButtonHome;
        private System.Windows.Forms.RadioButton radioButtonDesktop;
        private System.Windows.Forms.TextBox textBoxNext;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonPrev;
        private System.Windows.Forms.RadioButton radioButtonRoot;
    }
}