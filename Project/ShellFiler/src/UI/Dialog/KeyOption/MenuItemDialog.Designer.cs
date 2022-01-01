namespace ShellFiler.UI.Dialog.KeyOption {
    partial class MenuItemDialog {
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.treeViewCommand = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxExplanation = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxItemName = new System.Windows.Forms.CheckBox();
            this.labelShortcutWarning = new System.Windows.Forms.Label();
            this.comboBoxShortcut = new System.Windows.Forms.ComboBox();
            this.textBoxItemName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonAssign = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.treeViewCommand);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBoxExplanation);
            this.groupBox2.Location = new System.Drawing.Point(10, 142);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(677, 234);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "実行する機能";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "機能(&F):";
            // 
            // treeViewCommand
            // 
            this.treeViewCommand.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeViewCommand.HideSelection = false;
            this.treeViewCommand.Location = new System.Drawing.Point(6, 30);
            this.treeViewCommand.Name = "treeViewCommand";
            this.treeViewCommand.Size = new System.Drawing.Size(327, 192);
            this.treeViewCommand.TabIndex = 1;
            this.treeViewCommand.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeViewCommand_DrawNode);
            this.treeViewCommand.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewCommand_BeforeExpand);
            this.treeViewCommand.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCommand_AfterSelect);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(339, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "説明:";
            // 
            // textBoxExplanation
            // 
            this.textBoxExplanation.Location = new System.Drawing.Point(339, 30);
            this.textBoxExplanation.Multiline = true;
            this.textBoxExplanation.Name = "textBoxExplanation";
            this.textBoxExplanation.ReadOnly = true;
            this.textBoxExplanation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxExplanation.Size = new System.Drawing.Size(326, 192);
            this.textBoxExplanation.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(306, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "メニューの項目名と、決定したときに実行する機能を設定します。";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxItemName);
            this.groupBox1.Controls.Add(this.labelShortcutWarning);
            this.groupBox1.Controls.Add(this.comboBoxShortcut);
            this.groupBox1.Controls.Add(this.textBoxItemName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(12, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(677, 100);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "メニュー項目";
            // 
            // checkBoxItemName
            // 
            this.checkBoxItemName.AutoSize = true;
            this.checkBoxItemName.Location = new System.Drawing.Point(112, 20);
            this.checkBoxItemName.Name = "checkBoxItemName";
            this.checkBoxItemName.Size = new System.Drawing.Size(142, 16);
            this.checkBoxItemName.TabIndex = 1;
            this.checkBoxItemName.Text = "項目名をカスタマイズする";
            this.checkBoxItemName.UseVisualStyleBackColor = true;
            // 
            // labelShortcutWarning
            // 
            this.labelShortcutWarning.AutoSize = true;
            this.labelShortcutWarning.BackColor = System.Drawing.Color.Yellow;
            this.labelShortcutWarning.Location = new System.Drawing.Point(239, 68);
            this.labelShortcutWarning.Name = "labelShortcutWarning";
            this.labelShortcutWarning.Size = new System.Drawing.Size(172, 12);
            this.labelShortcutWarning.TabIndex = 5;
            this.labelShortcutWarning.Text = "ショートカットキーの重複があります。";
            // 
            // comboBoxShortcut
            // 
            this.comboBoxShortcut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxShortcut.FormattingEnabled = true;
            this.comboBoxShortcut.Location = new System.Drawing.Point(112, 65);
            this.comboBoxShortcut.Name = "comboBoxShortcut";
            this.comboBoxShortcut.Size = new System.Drawing.Size(121, 20);
            this.comboBoxShortcut.TabIndex = 4;
            // 
            // textBoxItemName
            // 
            this.textBoxItemName.Location = new System.Drawing.Point(129, 40);
            this.textBoxItemName.Name = "textBoxItemName";
            this.textBoxItemName.Size = new System.Drawing.Size(229, 19);
            this.textBoxItemName.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 12);
            this.label6.TabIndex = 3;
            this.label6.Text = "ショートカットキー(&S):";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "項目名(&N):";
            // 
            // buttonAssign
            // 
            this.buttonAssign.Enabled = false;
            this.buttonAssign.Location = new System.Drawing.Point(519, 382);
            this.buttonAssign.Name = "buttonAssign";
            this.buttonAssign.Size = new System.Drawing.Size(87, 23);
            this.buttonAssign.TabIndex = 3;
            this.buttonAssign.Text = "割り当て(&A)...";
            this.buttonAssign.UseVisualStyleBackColor = true;
            this.buttonAssign.Click += new System.EventHandler(this.buttonAssign_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(612, 382);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // MenuItemDialog
            // 
            this.AcceptButton = this.buttonAssign;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(701, 418);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonAssign);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MenuItemDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "メニュー項目の追加と編集";
            this.Load += new System.EventHandler(this.MenuItemDialog_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TreeView treeViewCommand;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxExplanation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonAssign;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxShortcut;
        private System.Windows.Forms.TextBox textBoxItemName;
        private System.Windows.Forms.CheckBox checkBoxItemName;
        private System.Windows.Forms.Label labelShortcutWarning;
    }
}