namespace ShellFiler.UI.Dialog {
    partial class DiffToolOppositeTargetDialog {
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
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonName = new System.Windows.Forms.RadioButton();
            this.radioButtonMark = new System.Windows.Forms.RadioButton();
            this.textBoxTarget = new System.Windows.Forms.TextBox();
            this.textBoxOppositeName = new System.Windows.Forms.TextBox();
            this.textBoxOppositeMark = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(43, 50);
            this.pictureBoxIcon.TabIndex = 2;
            this.pictureBoxIcon.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "どちらのファイルとの差分を確認しますか？";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "対象ファイル:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(62, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "反対ファイル:";
            // 
            // radioButtonName
            // 
            this.radioButtonName.AutoSize = true;
            this.radioButtonName.Location = new System.Drawing.Point(3, 3);
            this.radioButtonName.Name = "radioButtonName";
            this.radioButtonName.Size = new System.Drawing.Size(91, 16);
            this.radioButtonName.TabIndex = 0;
            this.radioButtonName.TabStop = true;
            this.radioButtonName.Text = "同名のファイル";
            this.radioButtonName.UseVisualStyleBackColor = true;
            // 
            // radioButtonMark
            // 
            this.radioButtonMark.AutoSize = true;
            this.radioButtonMark.Location = new System.Drawing.Point(3, 28);
            this.radioButtonMark.Name = "radioButtonMark";
            this.radioButtonMark.Size = new System.Drawing.Size(84, 16);
            this.radioButtonMark.TabIndex = 1;
            this.radioButtonMark.TabStop = true;
            this.radioButtonMark.Text = "マークファイル";
            this.radioButtonMark.UseVisualStyleBackColor = true;
            // 
            // textBoxTarget
            // 
            this.textBoxTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxTarget.Location = new System.Drawing.Point(185, 39);
            this.textBoxTarget.Name = "textBoxTarget";
            this.textBoxTarget.ReadOnly = true;
            this.textBoxTarget.Size = new System.Drawing.Size(360, 19);
            this.textBoxTarget.TabIndex = 2;
            // 
            // textBoxOppositeName
            // 
            this.textBoxOppositeName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOppositeName.Location = new System.Drawing.Point(185, 81);
            this.textBoxOppositeName.Name = "textBoxOppositeName";
            this.textBoxOppositeName.ReadOnly = true;
            this.textBoxOppositeName.Size = new System.Drawing.Size(360, 19);
            this.textBoxOppositeName.TabIndex = 5;
            // 
            // textBoxOppositeMark
            // 
            this.textBoxOppositeMark.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOppositeMark.Location = new System.Drawing.Point(185, 106);
            this.textBoxOppositeMark.Name = "textBoxOppositeMark";
            this.textBoxOppositeMark.ReadOnly = true;
            this.textBoxOppositeMark.Size = new System.Drawing.Size(360, 19);
            this.textBoxOppositeMark.TabIndex = 6;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(472, 136);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(391, 136);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 7;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButtonName);
            this.panel1.Controls.Add(this.radioButtonMark);
            this.panel1.Location = new System.Drawing.Point(75, 78);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(104, 52);
            this.panel1.TabIndex = 4;
            // 
            // DiffToolOppositeTargetDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(559, 171);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxOppositeMark);
            this.Controls.Add(this.textBoxOppositeName);
            this.Controls.Add(this.textBoxTarget);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxIcon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiffToolOppositeTargetDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "反対パスのファイルと差分表示";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonName;
        private System.Windows.Forms.RadioButton radioButtonMark;
        private System.Windows.Forms.TextBox textBoxTarget;
        private System.Windows.Forms.TextBox textBoxOppositeName;
        private System.Windows.Forms.TextBox textBoxOppositeMark;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Panel panel1;
    }
}