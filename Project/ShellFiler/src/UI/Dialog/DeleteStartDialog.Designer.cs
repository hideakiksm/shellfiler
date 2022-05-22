namespace ShellFiler.UI.Dialog
{
    partial class DeleteStartDialog
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
            this.listViewTarget = new System.Windows.Forms.ListView();
            this.checkBoxDirectory = new System.Windows.Forms.CheckBox();
            this.checkBoxAttr = new System.Windows.Forms.CheckBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            this.checkBoxWithRecycle = new System.Windows.Forms.CheckBox();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // listViewTarget
            // 
            this.listViewTarget.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewTarget.HideSelection = false;
            this.listViewTarget.Location = new System.Drawing.Point(12, 32);
            this.listViewTarget.Name = "listViewTarget";
            this.listViewTarget.Size = new System.Drawing.Size(420, 150);
            this.listViewTarget.TabIndex = 1;
            this.listViewTarget.UseCompatibleStateImageBehavior = false;
            this.listViewTarget.View = System.Windows.Forms.View.Details;
            // 
            // checkBoxDirectory
            // 
            this.checkBoxDirectory.AutoSize = true;
            this.checkBoxDirectory.Location = new System.Drawing.Point(12, 188);
            this.checkBoxDirectory.Name = "checkBoxDirectory";
            this.checkBoxDirectory.Size = new System.Drawing.Size(195, 19);
            this.checkBoxDirectory.TabIndex = 2;
            this.checkBoxDirectory.Text = "フォルダを確認なしですべて削除(&D)";
            this.checkBoxDirectory.UseVisualStyleBackColor = true;
            // 
            // checkBoxAttr
            // 
            this.checkBoxAttr.AutoSize = true;
            this.checkBoxAttr.Location = new System.Drawing.Point(12, 206);
            this.checkBoxAttr.Name = "checkBoxAttr";
            this.checkBoxAttr.Size = new System.Drawing.Size(316, 19);
            this.checkBoxAttr.TabIndex = 3;
            this.checkBoxAttr.Text = "読み込み/システム属性のファイルを確認なしですべて削除(&A)";
            this.checkBoxAttr.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(276, 246);
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
            this.buttonCancel.Location = new System.Drawing.Point(357, 246);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(34, 11);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(59, 15);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "message";
            // 
            // checkBoxWithRecycle
            // 
            this.checkBoxWithRecycle.AutoSize = true;
            this.checkBoxWithRecycle.Location = new System.Drawing.Point(12, 224);
            this.checkBoxWithRecycle.Name = "checkBoxWithRecycle";
            this.checkBoxWithRecycle.Size = new System.Drawing.Size(138, 19);
            this.checkBoxWithRecycle.TabIndex = 4;
            this.checkBoxWithRecycle.Text = "ごみ箱を使って削除(&R)";
            this.checkBoxWithRecycle.UseVisualStyleBackColor = true;
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Location = new System.Drawing.Point(12, 10);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxIcon.TabIndex = 9;
            this.pictureBoxIcon.TabStop = false;
            // 
            // DeleteStartDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(444, 281);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.checkBoxWithRecycle);
            this.Controls.Add(this.checkBoxAttr);
            this.Controls.Add(this.checkBoxDirectory);
            this.Controls.Add(this.listViewTarget);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeleteStartDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ファイル削除の確認";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DeleteStartDialog_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewTarget;
        private System.Windows.Forms.CheckBox checkBoxDirectory;
        private System.Windows.Forms.CheckBox checkBoxAttr;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.CheckBox checkBoxWithRecycle;
        private System.Windows.Forms.PictureBox pictureBoxIcon;

    }
}