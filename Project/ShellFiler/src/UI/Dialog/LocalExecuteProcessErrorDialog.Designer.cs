namespace ShellFiler.UI.Dialog {
    partial class LocalExecuteProcessErrorDialog {
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
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelErrorLine = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.listViewLocalFile = new System.Windows.Forms.ListView();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(80, 24);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(68, 12);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "編集:XXX 他";
            // 
            // labelErrorLine
            // 
            this.labelErrorLine.AutoSize = true;
            this.labelErrorLine.Location = new System.Drawing.Point(63, 50);
            this.labelErrorLine.Name = "labelErrorLine";
            this.labelErrorLine.Size = new System.Drawing.Size(267, 12);
            this.labelErrorLine.TabIndex = 1;
            this.labelErrorLine.Text = "編集用のアプリケーション{0}が起動直後に終了しました。";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(443, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "編集を破棄または反映するには、画面右下の「編集終了待ち」の一覧を右クリックしてください。";
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(43, 50);
            this.pictureBoxIcon.TabIndex = 2;
            this.pictureBoxIcon.TabStop = false;
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonOk.Location = new System.Drawing.Point(483, 227);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // listViewLocalFile
            // 
            this.listViewLocalFile.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
            this.listViewLocalFile.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewLocalFile.Location = new System.Drawing.Point(12, 120);
            this.listViewLocalFile.Name = "listViewLocalFile";
            this.listViewLocalFile.Size = new System.Drawing.Size(544, 101);
            this.listViewLocalFile.TabIndex = 4;
            this.listViewLocalFile.UseCompatibleStateImageBehavior = false;
            this.listViewLocalFile.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader
            // 
            this.columnHeader.Width = 25;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "作業ファイル:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(61, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(162, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "ファイルの更新を監視できません。";
            // 
            // LocalExecuteProcessErrorDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonOk;
            this.ClientSize = new System.Drawing.Size(573, 263);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listViewLocalFile);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelErrorLine);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LocalExecuteProcessErrorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ShellFiler";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelErrorLine;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ListView listViewLocalFile;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}