namespace ShellFiler.UI.Dialog {
    partial class BookmarkSettingDialog {
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
            this.treeViewSetting = new System.Windows.Forms.TreeView();
            this.buttonNewFolder = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDispName = new System.Windows.Forms.TextBox();
            this.textBoxFolder = new System.Windows.Forms.TextBox();
            this.buttonFolderRef = new System.Windows.Forms.Button();
            this.comboBoxShortcut = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNewGroup = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonInitial = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeViewSetting
            // 
            this.treeViewSetting.HideSelection = false;
            this.treeViewSetting.Location = new System.Drawing.Point(13, 13);
            this.treeViewSetting.Name = "treeViewSetting";
            this.treeViewSetting.ShowNodeToolTips = true;
            this.treeViewSetting.ShowPlusMinus = false;
            this.treeViewSetting.ShowRootLines = false;
            this.treeViewSetting.Size = new System.Drawing.Size(351, 198);
            this.treeViewSetting.TabIndex = 0;
            this.treeViewSetting.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewSetting_BeforeCollapse);
            this.treeViewSetting.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewSetting_BeforeSelect);
            this.treeViewSetting.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSetting_AfterSelect);
            // 
            // buttonNewFolder
            // 
            this.buttonNewFolder.Location = new System.Drawing.Point(370, 41);
            this.buttonNewFolder.Name = "buttonNewFolder";
            this.buttonNewFolder.Size = new System.Drawing.Size(93, 23);
            this.buttonNewFolder.TabIndex = 2;
            this.buttonNewFolder.Text = "新規フォルダ(&N)";
            this.buttonNewFolder.UseVisualStyleBackColor = true;
            this.buttonNewFolder.Click += new System.EventHandler(this.buttonNewFolder_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(370, 70);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(93, 23);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "削除(&D)";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Location = new System.Drawing.Point(370, 99);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(93, 23);
            this.buttonUp.TabIndex = 4;
            this.buttonUp.Text = "上へ(&U)";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Location = new System.Drawing.Point(370, 128);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(93, 23);
            this.buttonDown.TabIndex = 5;
            this.buttonDown.Text = "下へ(&B)";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 220);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "項目名(&M):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 245);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "フォルダ(&F):";
            // 
            // textBoxDispName
            // 
            this.textBoxDispName.Location = new System.Drawing.Point(100, 217);
            this.textBoxDispName.Name = "textBoxDispName";
            this.textBoxDispName.Size = new System.Drawing.Size(161, 23);
            this.textBoxDispName.TabIndex = 8;
            // 
            // textBoxFolder
            // 
            this.textBoxFolder.Location = new System.Drawing.Point(100, 242);
            this.textBoxFolder.Name = "textBoxFolder";
            this.textBoxFolder.Size = new System.Drawing.Size(282, 23);
            this.textBoxFolder.TabIndex = 10;
            // 
            // buttonFolderRef
            // 
            this.buttonFolderRef.Location = new System.Drawing.Point(388, 240);
            this.buttonFolderRef.Name = "buttonFolderRef";
            this.buttonFolderRef.Size = new System.Drawing.Size(75, 23);
            this.buttonFolderRef.TabIndex = 11;
            this.buttonFolderRef.Text = "参照(&R)...";
            this.buttonFolderRef.UseVisualStyleBackColor = true;
            this.buttonFolderRef.Click += new System.EventHandler(this.buttonFolderRef_Click);
            // 
            // comboBoxShortcut
            // 
            this.comboBoxShortcut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxShortcut.FormattingEnabled = true;
            this.comboBoxShortcut.Location = new System.Drawing.Point(100, 268);
            this.comboBoxShortcut.Name = "comboBoxShortcut";
            this.comboBoxShortcut.Size = new System.Drawing.Size(161, 23);
            this.comboBoxShortcut.TabIndex = 13;
            this.comboBoxShortcut.DropDown += new System.EventHandler(this.comboBoxShortcut_DropDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 271);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 15);
            this.label3.TabIndex = 12;
            this.label3.Text = "ショートカット(&S):";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(388, 292);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 15;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonNewGroup
            // 
            this.buttonNewGroup.Location = new System.Drawing.Point(370, 12);
            this.buttonNewGroup.Name = "buttonNewGroup";
            this.buttonNewGroup.Size = new System.Drawing.Size(93, 23);
            this.buttonNewGroup.TabIndex = 1;
            this.buttonNewGroup.Text = "新規グループ(&G)";
            this.buttonNewGroup.UseVisualStyleBackColor = true;
            this.buttonNewGroup.Click += new System.EventHandler(this.buttonNewGroup_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(307, 292);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 14;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonInitial
            // 
            this.buttonInitial.Location = new System.Drawing.Point(370, 157);
            this.buttonInitial.Name = "buttonInitial";
            this.buttonInitial.Size = new System.Drawing.Size(93, 23);
            this.buttonInitial.TabIndex = 6;
            this.buttonInitial.Text = "初期値挿入(&I)";
            this.buttonInitial.UseVisualStyleBackColor = true;
            this.buttonInitial.Click += new System.EventHandler(this.buttonInitial_Click);
            // 
            // BookmarkSettingDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(475, 327);
            this.Controls.Add(this.comboBoxShortcut);
            this.Controls.Add(this.textBoxFolder);
            this.Controls.Add(this.textBoxDispName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonFolderRef);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonInitial);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonNewGroup);
            this.Controls.Add(this.buttonNewFolder);
            this.Controls.Add(this.treeViewSetting);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BookmarkSettingDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ブックマークの設定";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BookmarkSettingDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewSetting;
        private System.Windows.Forms.Button buttonNewFolder;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDispName;
        private System.Windows.Forms.TextBox textBoxFolder;
        private System.Windows.Forms.Button buttonFolderRef;
        private System.Windows.Forms.ComboBox comboBoxShortcut;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNewGroup;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonInitial;
    }
}