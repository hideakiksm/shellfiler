namespace ShellFiler.UI.Dialog.KeyOption {
    partial class VersionupKeyDiffDialog {
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
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.listViewFileList = new System.Windows.Forms.ListView();
            this.columnListCommand = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnListKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnListStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.listViewFileViewer = new System.Windows.Forms.ListView();
            this.columnFvCommand = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnFvKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnFvStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewGraphicsViewer = new System.Windows.Forms.ListView();
            this.columnGvCommand = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnGvKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnGvStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonCopy = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxInformation = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(313, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "今までお使いのShellFilerから、次のキー設定が追加されました。";
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(43, 43);
            this.pictureBoxIcon.TabIndex = 2;
            this.pictureBoxIcon.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "ファイル一覧(&F):";
            // 
            // listViewFileList
            // 
            this.listViewFileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnListCommand,
            this.columnListKey,
            this.columnListStatus});
            this.listViewFileList.HideSelection = false;
            this.listViewFileList.Location = new System.Drawing.Point(131, 138);
            this.listViewFileList.Name = "listViewFileList";
            this.listViewFileList.Size = new System.Drawing.Size(524, 97);
            this.listViewFileList.TabIndex = 5;
            this.listViewFileList.UseCompatibleStateImageBehavior = false;
            this.listViewFileList.View = System.Windows.Forms.View.Details;
            // 
            // columnListCommand
            // 
            this.columnListCommand.Text = "コマンド";
            this.columnListCommand.Width = 250;
            // 
            // columnListKey
            // 
            this.columnListKey.Text = "キー";
            this.columnListKey.Width = 120;
            // 
            // columnListStatus
            // 
            this.columnListStatus.Text = "対処";
            this.columnListStatus.Width = 130;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 241);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "ファイルビューア(&V):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 344);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "グラフィックビューア(&G):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(61, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(446, 15);
            this.label6.TabIndex = 1;
            this.label6.Text = "設定を自動でマージできなかった機能は、キー設定の変更またはリセットを行うと使用できます。";
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(562, 447);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(93, 23);
            this.buttonClose.TabIndex = 11;
            this.buttonClose.Text = "反映して継続";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // listViewFileViewer
            // 
            this.listViewFileViewer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnFvCommand,
            this.columnFvKey,
            this.columnFvStatus});
            this.listViewFileViewer.HideSelection = false;
            this.listViewFileViewer.Location = new System.Drawing.Point(131, 241);
            this.listViewFileViewer.Name = "listViewFileViewer";
            this.listViewFileViewer.Size = new System.Drawing.Size(524, 97);
            this.listViewFileViewer.TabIndex = 7;
            this.listViewFileViewer.UseCompatibleStateImageBehavior = false;
            this.listViewFileViewer.View = System.Windows.Forms.View.Details;
            // 
            // columnFvCommand
            // 
            this.columnFvCommand.Text = "コマンド";
            this.columnFvCommand.Width = 250;
            // 
            // columnFvKey
            // 
            this.columnFvKey.Text = "キー";
            this.columnFvKey.Width = 120;
            // 
            // columnFvStatus
            // 
            this.columnFvStatus.Text = "対処";
            this.columnFvStatus.Width = 130;
            // 
            // listViewGraphicsViewer
            // 
            this.listViewGraphicsViewer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnGvCommand,
            this.columnGvKey,
            this.columnGvStatus});
            this.listViewGraphicsViewer.HideSelection = false;
            this.listViewGraphicsViewer.Location = new System.Drawing.Point(131, 344);
            this.listViewGraphicsViewer.Name = "listViewGraphicsViewer";
            this.listViewGraphicsViewer.Size = new System.Drawing.Size(524, 97);
            this.listViewGraphicsViewer.TabIndex = 9;
            this.listViewGraphicsViewer.UseCompatibleStateImageBehavior = false;
            this.listViewGraphicsViewer.View = System.Windows.Forms.View.Details;
            // 
            // columnGvCommand
            // 
            this.columnGvCommand.Text = "コマンド";
            this.columnGvCommand.Width = 250;
            // 
            // columnGvKey
            // 
            this.columnGvKey.Text = "キー";
            this.columnGvKey.Width = 120;
            // 
            // columnGvStatus
            // 
            this.columnGvStatus.Text = "対処";
            this.columnGvStatus.Width = 130;
            // 
            // buttonCopy
            // 
            this.buttonCopy.Location = new System.Drawing.Point(12, 447);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(101, 23);
            this.buttonCopy.TabIndex = 10;
            this.buttonCopy.Text = "クリップボードへ(&C)";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "お知らせ(&I):";
            // 
            // textBoxInformation
            // 
            this.textBoxInformation.Location = new System.Drawing.Point(131, 58);
            this.textBoxInformation.Multiline = true;
            this.textBoxInformation.Name = "textBoxInformation";
            this.textBoxInformation.ReadOnly = true;
            this.textBoxInformation.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxInformation.Size = new System.Drawing.Size(524, 74);
            this.textBoxInformation.TabIndex = 3;
            // 
            // VersionupKeyDiffDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(667, 479);
            this.ControlBox = false;
            this.Controls.Add(this.textBoxInformation);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.listViewGraphicsViewer);
            this.Controls.Add(this.listViewFileViewer);
            this.Controls.Add(this.listViewFileList);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VersionupKeyDiffDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "バージョンアップありがとうございます";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VersionupKeyDiffDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView listViewFileList;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ColumnHeader columnListCommand;
        private System.Windows.Forms.ColumnHeader columnListKey;
        private System.Windows.Forms.ColumnHeader columnListStatus;
        private System.Windows.Forms.ListView listViewFileViewer;
        private System.Windows.Forms.ColumnHeader columnFvCommand;
        private System.Windows.Forms.ColumnHeader columnFvKey;
        private System.Windows.Forms.ColumnHeader columnFvStatus;
        private System.Windows.Forms.ListView listViewGraphicsViewer;
        private System.Windows.Forms.ColumnHeader columnGvCommand;
        private System.Windows.Forms.ColumnHeader columnGvKey;
        private System.Windows.Forms.ColumnHeader columnGvStatus;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxInformation;
    }
}