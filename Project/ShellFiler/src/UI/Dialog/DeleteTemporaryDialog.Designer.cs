namespace ShellFiler.UI.Dialog {
    partial class DeleteTemporaryDialog {
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
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.listBoxFolder = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Location = new System.Drawing.Point(21, 21);
            this.pictureBoxIcon.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(75, 88);
            this.pictureBoxIcon.TabIndex = 2;
            this.pictureBoxIcon.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(108, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(468, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "次のフォルダに使用されていない一時ファイルがあります。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(107, 66);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(702, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "ShellFilerが異常終了した場合などに作業していたファイルを抽出することができます。";
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(467, 326);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(186, 40);
            this.buttonDelete.TabIndex = 4;
            this.buttonDelete.Text = "削除して続行(&D)";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(663, 326);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(187, 40);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "そのまま続行(&C)";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // listBoxFolder
            // 
            this.listBoxFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxFolder.FormattingEnabled = true;
            this.listBoxFolder.ItemHeight = 30;
            this.listBoxFolder.Location = new System.Drawing.Point(0, 0);
            this.listBoxFolder.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.listBoxFolder.Name = "listBoxFolder";
            this.listBoxFolder.Size = new System.Drawing.Size(829, 166);
            this.listBoxFolder.TabIndex = 3;
            this.listBoxFolder.DoubleClick += new System.EventHandler(this.listBoxFolder_DoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(107, 98);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(432, 30);
            this.label3.TabIndex = 2;
            this.label3.Text = "一覧をダブルクリックすると作業フォルダを開きます。";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listBoxFolder);
            this.panel1.Location = new System.Drawing.Point(21, 152);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(829, 166);
            this.panel1.TabIndex = 6;
            // 
            // DeleteTemporaryDialog
            // 
            this.AcceptButton = this.buttonDelete;
            this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(872, 387);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxIcon);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeleteTemporaryDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "未使用の一時ファイル";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ListBox listBoxFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
    }
}