namespace ShellFiler.UI.Dialog
{
    partial class MakeDirectoryDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDirName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCurrent = new System.Windows.Forms.TextBox();
            this.checkBoxMoveCurrent = new System.Windows.Forms.CheckBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "作成するフォルダ名(&N):";
            // 
            // textBoxDirName
            // 
            this.textBoxDirName.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxDirName.Location = new System.Drawing.Point(26, 29);
            this.textBoxDirName.Name = "textBoxDirName";
            this.textBoxDirName.Size = new System.Drawing.Size(279, 20);
            this.textBoxDirName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "親フォルダ:";
            // 
            // textBoxCurrent
            // 
            this.textBoxCurrent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxCurrent.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxCurrent.Location = new System.Drawing.Point(26, 78);
            this.textBoxCurrent.Name = "textBoxCurrent";
            this.textBoxCurrent.ReadOnly = true;
            this.textBoxCurrent.Size = new System.Drawing.Size(279, 13);
            this.textBoxCurrent.TabIndex = 3;
            // 
            // checkBoxMoveCurrent
            // 
            this.checkBoxMoveCurrent.AutoSize = true;
            this.checkBoxMoveCurrent.Location = new System.Drawing.Point(15, 101);
            this.checkBoxMoveCurrent.Name = "checkBoxMoveCurrent";
            this.checkBoxMoveCurrent.Size = new System.Drawing.Size(150, 19);
            this.checkBoxMoveCurrent.TabIndex = 4;
            this.checkBoxMoveCurrent.Text = "作成したフォルダを開く(&M)";
            this.checkBoxMoveCurrent.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(149, 128);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(230, 128);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // MakeDirectoryDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(317, 163);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.checkBoxMoveCurrent);
            this.Controls.Add(this.textBoxCurrent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxDirName);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MakeDirectoryDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "フォルダの作成";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDirName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCurrent;
        private System.Windows.Forms.CheckBox checkBoxMoveCurrent;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
    }
}