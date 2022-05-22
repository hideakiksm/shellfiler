namespace ShellFiler.UI.Dialog
{
    partial class FileIncrementalSearchDialog
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
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.checkBoxCompareHead = new System.Windows.Forms.CheckBox();
            this.panelInformation = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonMark = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "検索するファイル名(&F):";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(25, 30);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(240, 23);
            this.textBoxFileName.TabIndex = 1;
            this.textBoxFileName.TextChanged += new System.EventHandler(this.textBoxFileName_TextChanged);
            this.textBoxFileName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFileName_KeyDown);
            // 
            // checkBoxCompareHead
            // 
            this.checkBoxCompareHead.AutoSize = true;
            this.checkBoxCompareHead.Location = new System.Drawing.Point(15, 57);
            this.checkBoxCompareHead.Name = "checkBoxCompareHead";
            this.checkBoxCompareHead.Size = new System.Drawing.Size(166, 19);
            this.checkBoxCompareHead.TabIndex = 2;
            this.checkBoxCompareHead.Text = "ファイル名の先頭から比較(&T)";
            this.checkBoxCompareHead.UseVisualStyleBackColor = true;
            this.checkBoxCompareHead.Enter += new System.EventHandler(this.SubControl_Enter);
            // 
            // panelInformation
            // 
            this.panelInformation.Location = new System.Drawing.Point(13, 79);
            this.panelInformation.Name = "panelInformation";
            this.panelInformation.Size = new System.Drawing.Size(252, 16);
            this.panelInformation.TabIndex = 4;
            this.panelInformation.Paint += new System.Windows.Forms.PaintEventHandler(this.panelInformation_Paint);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(253, 100);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "戻る";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Location = new System.Drawing.Point(284, 12);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(44, 23);
            this.buttonUp.TabIndex = 0;
            this.buttonUp.Text = "↑";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            this.buttonUp.Enter += new System.EventHandler(this.SubControl_Enter);
            // 
            // buttonDown
            // 
            this.buttonDown.Location = new System.Drawing.Point(284, 38);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(44, 23);
            this.buttonDown.TabIndex = 0;
            this.buttonDown.Text = "↓";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            this.buttonDown.Enter += new System.EventHandler(this.SubControl_Enter);
            // 
            // buttonMark
            // 
            this.buttonMark.Location = new System.Drawing.Point(284, 65);
            this.buttonMark.Name = "buttonMark";
            this.buttonMark.Size = new System.Drawing.Size(44, 23);
            this.buttonMark.TabIndex = 0;
            this.buttonMark.Text = "選択";
            this.buttonMark.UseVisualStyleBackColor = true;
            this.buttonMark.Click += new System.EventHandler(this.buttonMark_Click);
            this.buttonMark.Enter += new System.EventHandler(this.SubControl_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(190, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Ctrlで切り替え";
            // 
            // FileIncrementalSearchDialog
            // 
            this.AcceptButton = this.buttonCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(340, 135);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonMark);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.panelInformation);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.checkBoxCompareHead);
            this.Controls.Add(this.textBoxFileName);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileIncrementalSearchDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "インクリメンタルサーチ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.CheckBox checkBoxCompareHead;
        private System.Windows.Forms.Panel panelInformation;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonMark;
        private System.Windows.Forms.Label label2;
    }
}