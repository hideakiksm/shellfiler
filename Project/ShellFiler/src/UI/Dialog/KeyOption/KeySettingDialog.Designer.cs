namespace ShellFiler.UI.Dialog.KeyOption {
    partial class KeySettingDialog {
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
            this.treeViewAllKey = new System.Windows.Forms.TreeView();
            this.tabControlKeyList = new System.Windows.Forms.TabControl();
            this.tabPageAll = new System.Windows.Forms.TabPage();
            this.tabPageAssigned = new System.Windows.Forms.TabPage();
            this.listViewDefinedKey = new System.Windows.Forms.ListView();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.treeViewCommand = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonRelease = new System.Windows.Forms.Button();
            this.buttonAssign = new System.Windows.Forms.Button();
            this.textBoxExplanation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDefault = new System.Windows.Forms.Button();
            this.labelFreeware = new System.Windows.Forms.Label();
            this.tabControlKeyList.SuspendLayout();
            this.tabPageAll.SuspendLayout();
            this.tabPageAssigned.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewAllKey
            // 
            this.treeViewAllKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewAllKey.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeViewAllKey.HideSelection = false;
            this.treeViewAllKey.Location = new System.Drawing.Point(3, 3);
            this.treeViewAllKey.Name = "treeViewAllKey";
            this.treeViewAllKey.Size = new System.Drawing.Size(259, 300);
            this.treeViewAllKey.TabIndex = 0;
            this.treeViewAllKey.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeViewKeyList_DrawNode);
            this.treeViewAllKey.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewKeyList_BeforeExpand);
            this.treeViewAllKey.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewKeyList_AfterSelect);
            // 
            // tabControlKeyList
            // 
            this.tabControlKeyList.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControlKeyList.Controls.Add(this.tabPageAll);
            this.tabControlKeyList.Controls.Add(this.tabPageAssigned);
            this.tabControlKeyList.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlKeyList.Location = new System.Drawing.Point(6, 18);
            this.tabControlKeyList.Multiline = true;
            this.tabControlKeyList.Name = "tabControlKeyList";
            this.tabControlKeyList.SelectedIndex = 0;
            this.tabControlKeyList.Size = new System.Drawing.Size(273, 332);
            this.tabControlKeyList.TabIndex = 0;
            this.tabControlKeyList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControlKeyList_DrawItem);
            this.tabControlKeyList.SelectedIndexChanged += new System.EventHandler(this.tabControlKeyList_SelectedIndexChanged);
            // 
            // tabPageAll
            // 
            this.tabPageAll.Controls.Add(this.treeViewAllKey);
            this.tabPageAll.Location = new System.Drawing.Point(4, 4);
            this.tabPageAll.Name = "tabPageAll";
            this.tabPageAll.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAll.Size = new System.Drawing.Size(265, 306);
            this.tabPageAll.TabIndex = 0;
            this.tabPageAll.Text = "すべてのキー";
            this.tabPageAll.UseVisualStyleBackColor = true;
            // 
            // tabPageAssigned
            // 
            this.tabPageAssigned.Controls.Add(this.listViewDefinedKey);
            this.tabPageAssigned.Location = new System.Drawing.Point(4, 4);
            this.tabPageAssigned.Name = "tabPageAssigned";
            this.tabPageAssigned.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAssigned.Size = new System.Drawing.Size(265, 306);
            this.tabPageAssigned.TabIndex = 1;
            this.tabPageAssigned.Text = "割り当て済みのキー";
            this.tabPageAssigned.UseVisualStyleBackColor = true;
            // 
            // listViewDefinedKey
            // 
            this.listViewDefinedKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDefinedKey.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.listViewDefinedKey.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewDefinedKey.HideSelection = false;
            this.listViewDefinedKey.Location = new System.Drawing.Point(3, 3);
            this.listViewDefinedKey.Name = "listViewDefinedKey";
            this.listViewDefinedKey.Size = new System.Drawing.Size(259, 300);
            this.listViewDefinedKey.TabIndex = 0;
            this.listViewDefinedKey.UseCompatibleStateImageBehavior = false;
            this.listViewDefinedKey.View = System.Windows.Forms.View.Details;
            this.listViewDefinedKey.SelectedIndexChanged += new System.EventHandler(this.listViewDefinedKey_SelectedIndexChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(568, 387);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(487, 387);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // treeViewCommand
            // 
            this.treeViewCommand.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeViewCommand.HideSelection = false;
            this.treeViewCommand.Location = new System.Drawing.Point(6, 17);
            this.treeViewCommand.Name = "treeViewCommand";
            this.treeViewCommand.Size = new System.Drawing.Size(327, 207);
            this.treeViewCommand.TabIndex = 0;
            this.treeViewCommand.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeViewCommand_DrawNode);
            this.treeViewCommand.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewCommand_BeforeExpand);
            this.treeViewCommand.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCommand_AfterSelect);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tabControlKeyList);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(285, 371);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "キー";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 353);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(218, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "太字のキーには機能が割り当てられています。";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonRelease);
            this.groupBox2.Controls.Add(this.buttonAssign);
            this.groupBox2.Controls.Add(this.textBoxExplanation);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.treeViewCommand);
            this.groupBox2.Location = new System.Drawing.Point(304, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(339, 371);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "キーに割り当てる機能";
            // 
            // buttonRelease
            // 
            this.buttonRelease.Enabled = false;
            this.buttonRelease.Location = new System.Drawing.Point(113, 342);
            this.buttonRelease.Name = "buttonRelease";
            this.buttonRelease.Size = new System.Drawing.Size(96, 23);
            this.buttonRelease.TabIndex = 4;
            this.buttonRelease.Text = "割り当て解除(&D)";
            this.buttonRelease.UseVisualStyleBackColor = true;
            this.buttonRelease.Click += new System.EventHandler(this.buttonRelease_Click);
            // 
            // buttonAssign
            // 
            this.buttonAssign.Enabled = false;
            this.buttonAssign.Location = new System.Drawing.Point(6, 342);
            this.buttonAssign.Name = "buttonAssign";
            this.buttonAssign.Size = new System.Drawing.Size(101, 23);
            this.buttonAssign.TabIndex = 3;
            this.buttonAssign.Text = "<<割り当て(&A)...";
            this.buttonAssign.UseVisualStyleBackColor = true;
            this.buttonAssign.Click += new System.EventHandler(this.buttonAssign_Click);
            // 
            // textBoxExplanation
            // 
            this.textBoxExplanation.Location = new System.Drawing.Point(7, 252);
            this.textBoxExplanation.Multiline = true;
            this.textBoxExplanation.Name = "textBoxExplanation";
            this.textBoxExplanation.ReadOnly = true;
            this.textBoxExplanation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxExplanation.Size = new System.Drawing.Size(326, 84);
            this.textBoxExplanation.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 237);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "機能の詳細説明";
            // 
            // buttonDefault
            // 
            this.buttonDefault.Location = new System.Drawing.Point(397, 387);
            this.buttonDefault.Name = "buttonDefault";
            this.buttonDefault.Size = new System.Drawing.Size(84, 23);
            this.buttonDefault.TabIndex = 2;
            this.buttonDefault.Text = "規定値に戻す";
            this.buttonDefault.UseVisualStyleBackColor = true;
            this.buttonDefault.Click += new System.EventHandler(this.buttonDefault_Click);
            // 
            // labelFreeware
            // 
            this.labelFreeware.AutoSize = true;
            this.labelFreeware.Location = new System.Drawing.Point(12, 386);
            this.labelFreeware.Name = "labelFreeware";
            this.labelFreeware.Size = new System.Drawing.Size(0, 12);
            this.labelFreeware.TabIndex = 7;
            // 
            // KeySettingDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(655, 422);
            this.Controls.Add(this.labelFreeware);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonDefault);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeySettingDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "キー割り当ての変更";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.KeySettingDialog_FormClosed);
            this.tabControlKeyList.ResumeLayout(false);
            this.tabPageAll.ResumeLayout(false);
            this.tabPageAssigned.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewAllKey;
        private System.Windows.Forms.TabControl tabControlKeyList;
        private System.Windows.Forms.TabPage tabPageAll;
        private System.Windows.Forms.TabPage tabPageAssigned;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.TreeView treeViewCommand;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonRelease;
        private System.Windows.Forms.Button buttonAssign;
        private System.Windows.Forms.TextBox textBoxExplanation;
        private System.Windows.Forms.ListView listViewDefinedKey;
        private System.Windows.Forms.Button buttonDefault;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelFreeware;
    }
}