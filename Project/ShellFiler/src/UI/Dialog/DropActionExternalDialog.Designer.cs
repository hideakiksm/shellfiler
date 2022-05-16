namespace ShellFiler.UI.Dialog {
    partial class DropActionExternalDialog {
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
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonMove = new System.Windows.Forms.Button();
            this.buttonShortcut = new System.Windows.Forms.Button();
            this.buttonChdir = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelFileCount = new System.Windows.Forms.Label();
            this.labelFolderCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(340, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "次のファイルまたはフォルダがドロップされました。";
            // 
            // listViewFiles
            // 
            this.listViewFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewFiles.HideSelection = false;
            this.listViewFiles.Location = new System.Drawing.Point(20, 39);
            this.listViewFiles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(792, 196);
            this.listViewFiles.TabIndex = 1;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(700, 387);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(112, 34);
            this.buttonCancel.TabIndex = 13;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonCopy
            // 
            this.buttonCopy.Location = new System.Drawing.Point(18, 256);
            this.buttonCopy.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(300, 34);
            this.buttonCopy.TabIndex = 3;
            this.buttonCopy.Text = "このフォルダにコピー(&C)";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonMove
            // 
            this.buttonMove.Location = new System.Drawing.Point(18, 300);
            this.buttonMove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.Size = new System.Drawing.Size(300, 34);
            this.buttonMove.TabIndex = 4;
            this.buttonMove.Text = "このフォルダに移動(&M)";
            this.buttonMove.UseVisualStyleBackColor = true;
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // buttonShortcut
            // 
            this.buttonShortcut.Location = new System.Drawing.Point(18, 344);
            this.buttonShortcut.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonShortcut.Name = "buttonShortcut";
            this.buttonShortcut.Size = new System.Drawing.Size(300, 34);
            this.buttonShortcut.TabIndex = 5;
            this.buttonShortcut.Text = "このフォルダにショートカットを作成(&S)";
            this.buttonShortcut.UseVisualStyleBackColor = true;
            this.buttonShortcut.Click += new System.EventHandler(this.buttonShortcut_Click);
            // 
            // buttonChdir
            // 
            this.buttonChdir.Location = new System.Drawing.Point(18, 387);
            this.buttonChdir.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonChdir.Name = "buttonChdir";
            this.buttonChdir.Size = new System.Drawing.Size(300, 34);
            this.buttonChdir.TabIndex = 6;
            this.buttonChdir.Text = "転送元のフォルダを表示(&V)";
            this.buttonChdir.UseVisualStyleBackColor = true;
            this.buttonChdir.Click += new System.EventHandler(this.buttonChdir_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(675, 256);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 25);
            this.label3.TabIndex = 7;
            this.label3.Text = "ファイル数:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(675, 283);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 25);
            this.label4.TabIndex = 9;
            this.label4.Text = "フォルダ数:";
            // 
            // labelFileCount
            // 
            this.labelFileCount.Location = new System.Drawing.Point(756, 252);
            this.labelFileCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFileCount.Name = "labelFileCount";
            this.labelFileCount.Size = new System.Drawing.Size(57, 34);
            this.labelFileCount.TabIndex = 8;
            this.labelFileCount.Text = "0";
            this.labelFileCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelFolderCount
            // 
            this.labelFolderCount.Location = new System.Drawing.Point(756, 278);
            this.labelFolderCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFolderCount.Name = "labelFolderCount";
            this.labelFolderCount.Size = new System.Drawing.Size(57, 34);
            this.labelFolderCount.TabIndex = 10;
            this.labelFolderCount.Text = "0";
            this.labelFolderCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DropActionExternalDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(831, 440);
            this.Controls.Add(this.labelFolderCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelFileCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonChdir);
            this.Controls.Add(this.buttonShortcut);
            this.Controls.Add(this.buttonMove);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.listViewFiles);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DropActionExternalDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ドロップ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DropActionDialog_FormClosed);
            this.Load += new System.EventHandler(this.DropActionDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Button buttonMove;
        private System.Windows.Forms.Button buttonShortcut;
        private System.Windows.Forms.Button buttonChdir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelFileCount;
        private System.Windows.Forms.Label labelFolderCount;
    }
}