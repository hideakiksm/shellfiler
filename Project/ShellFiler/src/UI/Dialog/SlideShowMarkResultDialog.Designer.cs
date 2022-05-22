namespace ShellFiler.UI.Dialog {
    partial class SlideShowMarkResultDialog {
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
            this.textBoxFolder = new System.Windows.Forms.TextBox();
            this.buttonChdir = new System.Windows.Forms.Button();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.columnMark = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnFiles = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonMark1 = new System.Windows.Forms.Button();
            this.buttonMark2 = new System.Windows.Forms.Button();
            this.buttonMark3 = new System.Windows.Forms.Button();
            this.buttonMark4 = new System.Windows.Forms.Button();
            this.buttonMark5 = new System.Windows.Forms.Button();
            this.buttonMark6 = new System.Windows.Forms.Button();
            this.buttonMark7 = new System.Windows.Forms.Button();
            this.buttonMark8 = new System.Windows.Forms.Button();
            this.buttonMark9 = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "対象フォルダ:";
            // 
            // textBoxFolder
            // 
            this.textBoxFolder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxFolder.Location = new System.Drawing.Point(85, 18);
            this.textBoxFolder.Name = "textBoxFolder";
            this.textBoxFolder.ReadOnly = true;
            this.textBoxFolder.Size = new System.Drawing.Size(270, 16);
            this.textBoxFolder.TabIndex = 1;
            // 
            // buttonChdir
            // 
            this.buttonChdir.Location = new System.Drawing.Point(361, 12);
            this.buttonChdir.Name = "buttonChdir";
            this.buttonChdir.Size = new System.Drawing.Size(75, 23);
            this.buttonChdir.TabIndex = 2;
            this.buttonChdir.Text = "切り替え(&D)";
            this.buttonChdir.UseVisualStyleBackColor = true;
            this.buttonChdir.Click += new System.EventHandler(this.buttonChdir_Click);
            // 
            // listViewFiles
            // 
            this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnMark,
            this.columnFiles});
            this.listViewFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewFiles.HideSelection = false;
            this.listViewFiles.Location = new System.Drawing.Point(12, 42);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(424, 137);
            this.listViewFiles.TabIndex = 3;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            // 
            // columnMark
            // 
            this.columnMark.Text = "マーク";
            this.columnMark.Width = 90;
            // 
            // columnFiles
            // 
            this.columnFiles.Text = "ファイル名";
            this.columnFiles.Width = 300;
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(361, 244);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 14;
            this.buttonClose.Text = "閉じる";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonMark1
            // 
            this.buttonMark1.Location = new System.Drawing.Point(12, 186);
            this.buttonMark1.Name = "buttonMark1";
            this.buttonMark1.Size = new System.Drawing.Size(84, 23);
            this.buttonMark1.TabIndex = 4;
            this.buttonMark1.Text = "マークNo.&1";
            this.buttonMark1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonMark1.UseVisualStyleBackColor = true;
            // 
            // buttonMark2
            // 
            this.buttonMark2.Location = new System.Drawing.Point(102, 186);
            this.buttonMark2.Name = "buttonMark2";
            this.buttonMark2.Size = new System.Drawing.Size(84, 23);
            this.buttonMark2.TabIndex = 5;
            this.buttonMark2.Text = "マークNo.&2";
            this.buttonMark2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonMark2.UseVisualStyleBackColor = true;
            // 
            // buttonMark3
            // 
            this.buttonMark3.Location = new System.Drawing.Point(192, 186);
            this.buttonMark3.Name = "buttonMark3";
            this.buttonMark3.Size = new System.Drawing.Size(84, 23);
            this.buttonMark3.TabIndex = 6;
            this.buttonMark3.Text = "マークNo.&3";
            this.buttonMark3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonMark3.UseVisualStyleBackColor = true;
            // 
            // buttonMark4
            // 
            this.buttonMark4.Location = new System.Drawing.Point(12, 215);
            this.buttonMark4.Name = "buttonMark4";
            this.buttonMark4.Size = new System.Drawing.Size(84, 23);
            this.buttonMark4.TabIndex = 7;
            this.buttonMark4.Text = "マークNo.&4";
            this.buttonMark4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonMark4.UseVisualStyleBackColor = true;
            // 
            // buttonMark5
            // 
            this.buttonMark5.Location = new System.Drawing.Point(102, 215);
            this.buttonMark5.Name = "buttonMark5";
            this.buttonMark5.Size = new System.Drawing.Size(84, 23);
            this.buttonMark5.TabIndex = 8;
            this.buttonMark5.Text = "マークNo.&5";
            this.buttonMark5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonMark5.UseVisualStyleBackColor = true;
            // 
            // buttonMark6
            // 
            this.buttonMark6.Location = new System.Drawing.Point(192, 215);
            this.buttonMark6.Name = "buttonMark6";
            this.buttonMark6.Size = new System.Drawing.Size(84, 23);
            this.buttonMark6.TabIndex = 9;
            this.buttonMark6.Text = "マークNo.&6";
            this.buttonMark6.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonMark6.UseVisualStyleBackColor = true;
            // 
            // buttonMark7
            // 
            this.buttonMark7.Location = new System.Drawing.Point(12, 244);
            this.buttonMark7.Name = "buttonMark7";
            this.buttonMark7.Size = new System.Drawing.Size(84, 23);
            this.buttonMark7.TabIndex = 10;
            this.buttonMark7.Text = "マークNo.&7";
            this.buttonMark7.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonMark7.UseVisualStyleBackColor = true;
            // 
            // buttonMark8
            // 
            this.buttonMark8.Location = new System.Drawing.Point(102, 244);
            this.buttonMark8.Name = "buttonMark8";
            this.buttonMark8.Size = new System.Drawing.Size(84, 23);
            this.buttonMark8.TabIndex = 11;
            this.buttonMark8.Text = "マークNo.&8";
            this.buttonMark8.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonMark8.UseVisualStyleBackColor = true;
            // 
            // buttonMark9
            // 
            this.buttonMark9.Location = new System.Drawing.Point(192, 244);
            this.buttonMark9.Name = "buttonMark9";
            this.buttonMark9.Size = new System.Drawing.Size(84, 23);
            this.buttonMark9.TabIndex = 12;
            this.buttonMark9.Text = "マークNo.&9";
            this.buttonMark9.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonMark9.UseVisualStyleBackColor = true;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(282, 186);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(82, 23);
            this.buttonClear.TabIndex = 13;
            this.buttonClear.Text = "選択解除(&C)";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // SlideShowMarkResultDialog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(447, 276);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonMark5);
            this.Controls.Add(this.buttonMark9);
            this.Controls.Add(this.buttonMark8);
            this.Controls.Add(this.buttonMark4);
            this.Controls.Add(this.buttonMark7);
            this.Controls.Add(this.buttonMark3);
            this.Controls.Add(this.buttonMark6);
            this.Controls.Add(this.buttonMark2);
            this.Controls.Add(this.buttonMark1);
            this.Controls.Add(this.listViewFiles);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonChdir);
            this.Controls.Add(this.textBoxFolder);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SlideShowMarkResultDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "スライドショーのマーク結果";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.SlideShowMarkResultDialog_HelpButtonClicked);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SlideShowMarkResultDialog_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFolder;
        private System.Windows.Forms.Button buttonChdir;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ColumnHeader columnMark;
        private System.Windows.Forms.ColumnHeader columnFiles;
        private System.Windows.Forms.Button buttonMark1;
        private System.Windows.Forms.Button buttonMark2;
        private System.Windows.Forms.Button buttonMark3;
        private System.Windows.Forms.Button buttonMark4;
        private System.Windows.Forms.Button buttonMark5;
        private System.Windows.Forms.Button buttonMark6;
        private System.Windows.Forms.Button buttonMark7;
        private System.Windows.Forms.Button buttonMark8;
        private System.Windows.Forms.Button buttonMark9;
        private System.Windows.Forms.Button buttonClear;
    }
}