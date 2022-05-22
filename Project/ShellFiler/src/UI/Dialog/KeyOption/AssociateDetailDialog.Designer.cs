namespace ShellFiler.UI.Dialog.KeyOption {
    partial class AssociateDetailDialog {
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
            this.comboBoxExt = new System.Windows.Forms.ComboBox();
            this.comboBoxFileSystem = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonAssign = new System.Windows.Forms.Button();
            this.textBoxExplanation = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.treeViewCommand = new System.Windows.Forms.TreeView();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelFreeware = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxExt
            // 
            this.comboBoxExt.FormattingEnabled = true;
            this.comboBoxExt.Location = new System.Drawing.Point(136, 44);
            this.comboBoxExt.Name = "comboBoxExt";
            this.comboBoxExt.Size = new System.Drawing.Size(435, 23);
            this.comboBoxExt.TabIndex = 3;
            // 
            // comboBoxFileSystem
            // 
            this.comboBoxFileSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFileSystem.FormattingEnabled = true;
            this.comboBoxFileSystem.Location = new System.Drawing.Point(136, 18);
            this.comboBoxFileSystem.Name = "comboBoxFileSystem";
            this.comboBoxFileSystem.Size = new System.Drawing.Size(145, 23);
            this.comboBoxFileSystem.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(577, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "セミコロン「;」区切り";
            // 
            // buttonAssign
            // 
            this.buttonAssign.Enabled = false;
            this.buttonAssign.Location = new System.Drawing.Point(521, 353);
            this.buttonAssign.Name = "buttonAssign";
            this.buttonAssign.Size = new System.Drawing.Size(87, 23);
            this.buttonAssign.TabIndex = 2;
            this.buttonAssign.Text = "割り当て(&A)...";
            this.buttonAssign.UseVisualStyleBackColor = true;
            this.buttonAssign.Click += new System.EventHandler(this.buttonAssign_Click);
            // 
            // textBoxExplanation
            // 
            this.textBoxExplanation.Location = new System.Drawing.Point(341, 52);
            this.textBoxExplanation.Multiline = true;
            this.textBoxExplanation.Name = "textBoxExplanation";
            this.textBoxExplanation.ReadOnly = true;
            this.textBoxExplanation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxExplanation.Size = new System.Drawing.Size(326, 192);
            this.textBoxExplanation.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(102, 15);
            this.label9.TabIndex = 0;
            this.label9.Text = "ファイルシステム(&Y):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "ファイルの拡張子(&E):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "機能(&F):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(341, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "説明:";
            // 
            // treeViewCommand
            // 
            this.treeViewCommand.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeViewCommand.HideSelection = false;
            this.treeViewCommand.Location = new System.Drawing.Point(8, 52);
            this.treeViewCommand.Name = "treeViewCommand";
            this.treeViewCommand.Size = new System.Drawing.Size(327, 192);
            this.treeViewCommand.TabIndex = 2;
            this.treeViewCommand.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewCommand_BeforeExpand);
            this.treeViewCommand.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeViewCommand_DrawNode);
            this.treeViewCommand.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCommand_AfterSelect);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(614, 353);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelFreeware
            // 
            this.labelFreeware.AutoSize = true;
            this.labelFreeware.Location = new System.Drawing.Point(10, 548);
            this.labelFreeware.Name = "labelFreeware";
            this.labelFreeware.Size = new System.Drawing.Size(0, 15);
            this.labelFreeware.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxFileSystem);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.comboBoxExt);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(677, 76);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "関連付けるファイルの条件";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.treeViewCommand);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBoxExplanation);
            this.groupBox2.Location = new System.Drawing.Point(12, 94);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(677, 253);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "実行する機能";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(354, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "条件に一致するファイルで関連づけを実行すると、次の機能を実行します。";
            // 
            // AssociateDetailDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(698, 388);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelFreeware);
            this.Controls.Add(this.buttonAssign);
            this.Controls.Add(this.buttonCancel);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssociateDetailDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "割り当てる機能の追加と編集";
            this.Load += new System.EventHandler(this.AssociateSettingDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAssign;
        private System.Windows.Forms.TextBox textBoxExplanation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView treeViewCommand;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBoxFileSystem;
        private System.Windows.Forms.ComboBox comboBoxExt;
        private System.Windows.Forms.Label labelFreeware;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
    }
}