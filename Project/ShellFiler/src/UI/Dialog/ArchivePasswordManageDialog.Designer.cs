namespace ShellFiler.UI.Dialog {
    partial class ArchivePasswordManageDialog {
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
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.listViewPasswordList = new System.Windows.Forms.ListView();
            this.columnDisplayName = new System.Windows.Forms.ColumnHeader();
            this.columnPassword = new System.Windows.Forms.ColumnHeader();
            this.linkLabelPassword = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelFreeware = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(396, 12);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "追加(&A)...";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(396, 41);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 2;
            this.buttonDelete.Text = "削除(&D)...";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(396, 201);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "閉じる";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 165);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(380, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "パスワード付きの圧縮ファイルで、これら設定済みのパスワードを順に適用します。";
            // 
            // buttonUp
            // 
            this.buttonUp.Location = new System.Drawing.Point(396, 70);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(75, 23);
            this.buttonUp.TabIndex = 3;
            this.buttonUp.Text = "上へ(&U)";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Location = new System.Drawing.Point(396, 99);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(75, 23);
            this.buttonDown.TabIndex = 4;
            this.buttonDown.Text = "下へ(&D)";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // listViewPasswordList
            // 
            this.listViewPasswordList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnDisplayName,
            this.columnPassword});
            this.listViewPasswordList.HideSelection = false;
            this.listViewPasswordList.Location = new System.Drawing.Point(12, 12);
            this.listViewPasswordList.MultiSelect = false;
            this.listViewPasswordList.Name = "listViewPasswordList";
            this.listViewPasswordList.Size = new System.Drawing.Size(378, 150);
            this.listViewPasswordList.TabIndex = 0;
            this.listViewPasswordList.UseCompatibleStateImageBehavior = false;
            this.listViewPasswordList.View = System.Windows.Forms.View.Details;
            this.listViewPasswordList.SelectedIndexChanged += new System.EventHandler(this.listViewPasswordList_SelectedIndexChanged);
            // 
            // columnDisplayName
            // 
            this.columnDisplayName.Text = "名前";
            this.columnDisplayName.Width = 160;
            // 
            // columnPassword
            // 
            this.columnPassword.Text = "パスワード(表示されません)";
            this.columnPassword.Width = 200;
            // 
            // linkLabelPassword
            // 
            this.linkLabelPassword.AutoSize = true;
            this.linkLabelPassword.Location = new System.Drawing.Point(10, 189);
            this.linkLabelPassword.Name = "linkLabelPassword";
            this.linkLabelPassword.Size = new System.Drawing.Size(110, 12);
            this.linkLabelPassword.TabIndex = 7;
            this.linkLabelPassword.TabStop = true;
            this.linkLabelPassword.Text = "パスワード保存の注意";
            this.linkLabelPassword.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelPassword_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 177);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(225, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "クリップボードの文字列も自動的に適用します。";
            // 
            // labelFreeware
            // 
            this.labelFreeware.AutoSize = true;
            this.labelFreeware.Location = new System.Drawing.Point(12, 208);
            this.labelFreeware.Name = "labelFreeware";
            this.labelFreeware.Size = new System.Drawing.Size(0, 12);
            this.labelFreeware.TabIndex = 9;
            // 
            // ArchivePasswordManageDialog
            // 
            this.AcceptButton = this.buttonCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(482, 236);
            this.Controls.Add(this.labelFreeware);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkLabelPassword);
            this.Controls.Add(this.listViewPasswordList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArchivePasswordManageDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "展開用パスワードの管理";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.ListView listViewPasswordList;
        private System.Windows.Forms.ColumnHeader columnDisplayName;
        private System.Windows.Forms.ColumnHeader columnPassword;
        private System.Windows.Forms.LinkLabel linkLabelPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelFreeware;
    }
}