namespace ShellFiler.UI.Dialog {
    partial class MirrorCopyDialog {
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
            this.labelMessage = new System.Windows.Forms.Label();
            this.listViewTarget = new System.Windows.Forms.ListView();
            this.radioButtonSizeDate = new System.Windows.Forms.RadioButton();
            this.radioButtonNotOverwrite = new System.Windows.Forms.RadioButton();
            this.radioButtonIfNewer = new System.Windows.Forms.RadioButton();
            this.radioButtonForce = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxExcept = new System.Windows.Forms.TextBox();
            this.labelAttrMessage = new System.Windows.Forms.Label();
            this.checkBoxAttrCopy = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxWithRecycle = new System.Windows.Forms.CheckBox();
            this.checkBoxAttr = new System.Windows.Forms.CheckBox();
            this.checkBoxDirectory = new System.Windows.Forms.CheckBox();
            this.labelAttr = new System.Windows.Forms.Label();
            this.labelDirectory = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.textBoxDest = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(34, 13);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(50, 12);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "message";
            // 
            // listViewTarget
            // 
            this.listViewTarget.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewTarget.Location = new System.Drawing.Point(12, 40);
            this.listViewTarget.Name = "listViewTarget";
            this.listViewTarget.Size = new System.Drawing.Size(602, 108);
            this.listViewTarget.TabIndex = 2;
            this.listViewTarget.UseCompatibleStateImageBehavior = false;
            this.listViewTarget.View = System.Windows.Forms.View.Details;
            // 
            // radioButtonSizeDate
            // 
            this.radioButtonSizeDate.AutoSize = true;
            this.radioButtonSizeDate.Location = new System.Drawing.Point(13, 78);
            this.radioButtonSizeDate.Name = "radioButtonSizeDate";
            this.radioButtonSizeDate.Size = new System.Drawing.Size(198, 16);
            this.radioButtonSizeDate.TabIndex = 3;
            this.radioButtonSizeDate.Text = "サイズまたは日付が違うとき上書き(&S)";
            this.radioButtonSizeDate.UseVisualStyleBackColor = true;
            // 
            // radioButtonNotOverwrite
            // 
            this.radioButtonNotOverwrite.AutoSize = true;
            this.radioButtonNotOverwrite.Location = new System.Drawing.Point(13, 58);
            this.radioButtonNotOverwrite.Name = "radioButtonNotOverwrite";
            this.radioButtonNotOverwrite.Size = new System.Drawing.Size(92, 16);
            this.radioButtonNotOverwrite.TabIndex = 2;
            this.radioButtonNotOverwrite.Text = "転送しない(&N)";
            this.radioButtonNotOverwrite.UseVisualStyleBackColor = true;
            // 
            // radioButtonIfNewer
            // 
            this.radioButtonIfNewer.AutoSize = true;
            this.radioButtonIfNewer.Location = new System.Drawing.Point(13, 38);
            this.radioButtonIfNewer.Name = "radioButtonIfNewer";
            this.radioButtonIfNewer.Size = new System.Drawing.Size(158, 16);
            this.radioButtonIfNewer.TabIndex = 1;
            this.radioButtonIfNewer.Text = "自分が新しければ上書き(&U)";
            this.radioButtonIfNewer.UseVisualStyleBackColor = true;
            // 
            // radioButtonForce
            // 
            this.radioButtonForce.AutoSize = true;
            this.radioButtonForce.Location = new System.Drawing.Point(13, 18);
            this.radioButtonForce.Name = "radioButtonForce";
            this.radioButtonForce.Size = new System.Drawing.Size(117, 16);
            this.radioButtonForce.TabIndex = 0;
            this.radioButtonForce.TabStop = true;
            this.radioButtonForce.Text = "強制的に上書き(&O)";
            this.radioButtonForce.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonForce);
            this.groupBox1.Controls.Add(this.radioButtonIfNewer);
            this.groupBox1.Controls.Add(this.radioButtonNotOverwrite);
            this.groupBox1.Controls.Add(this.radioButtonSizeDate);
            this.groupBox1.Location = new System.Drawing.Point(14, 179);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(226, 118);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "上書き条件";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxExcept);
            this.groupBox2.Controls.Add(this.labelAttrMessage);
            this.groupBox2.Controls.Add(this.checkBoxAttrCopy);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(246, 179);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(370, 118);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "コピー条件";
            // 
            // textBoxExcept
            // 
            this.textBoxExcept.Location = new System.Drawing.Point(8, 76);
            this.textBoxExcept.Name = "textBoxExcept";
            this.textBoxExcept.Size = new System.Drawing.Size(358, 19);
            this.textBoxExcept.TabIndex = 3;
            // 
            // labelAttrMessage
            // 
            this.labelAttrMessage.AutoSize = true;
            this.labelAttrMessage.Location = new System.Drawing.Point(21, 36);
            this.labelAttrMessage.Name = "labelAttrMessage";
            this.labelAttrMessage.Size = new System.Drawing.Size(253, 12);
            this.labelAttrMessage.TabIndex = 1;
            this.labelAttrMessage.Text = "未設定のときはオプションの設定「{0}」にしたがいます。";
            // 
            // checkBoxAttrCopy
            // 
            this.checkBoxAttrCopy.AutoSize = true;
            this.checkBoxAttrCopy.Location = new System.Drawing.Point(7, 19);
            this.checkBoxAttrCopy.Name = "checkBoxAttrCopy";
            this.checkBoxAttrCopy.Size = new System.Drawing.Size(184, 16);
            this.checkBoxAttrCopy.TabIndex = 0;
            this.checkBoxAttrCopy.Text = "転送後にすべての属性をコピー(&A)";
            this.checkBoxAttrCopy.ThreeState = true;
            this.checkBoxAttrCopy.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(183, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(205, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "「:」区切り、オプションで既定値を指定可能";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(311, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "次のファイルやフォルダが転送元にあっても存在しないことにする(&E):";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxWithRecycle);
            this.groupBox3.Controls.Add(this.checkBoxAttr);
            this.groupBox3.Controls.Add(this.checkBoxDirectory);
            this.groupBox3.Controls.Add(this.labelAttr);
            this.groupBox3.Controls.Add(this.labelDirectory);
            this.groupBox3.Location = new System.Drawing.Point(14, 303);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(604, 80);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "反対パスからの削除条件";
            // 
            // checkBoxWithRecycle
            // 
            this.checkBoxWithRecycle.AutoSize = true;
            this.checkBoxWithRecycle.Checked = true;
            this.checkBoxWithRecycle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWithRecycle.Location = new System.Drawing.Point(6, 54);
            this.checkBoxWithRecycle.Name = "checkBoxWithRecycle";
            this.checkBoxWithRecycle.Size = new System.Drawing.Size(134, 16);
            this.checkBoxWithRecycle.TabIndex = 2;
            this.checkBoxWithRecycle.Text = "ごみ箱を使って削除(&R)";
            this.checkBoxWithRecycle.UseVisualStyleBackColor = true;
            // 
            // checkBoxAttr
            // 
            this.checkBoxAttr.AutoSize = true;
            this.checkBoxAttr.Location = new System.Drawing.Point(6, 36);
            this.checkBoxAttr.Name = "checkBoxAttr";
            this.checkBoxAttr.Size = new System.Drawing.Size(313, 16);
            this.checkBoxAttr.TabIndex = 1;
            this.checkBoxAttr.Text = "読み込み/システム属性のファイルを確認なしですべて削除(&A)";
            this.checkBoxAttr.UseVisualStyleBackColor = true;
            this.checkBoxAttr.CheckedChanged += new System.EventHandler(this.checkBoxDelete_CheckedChanged);
            // 
            // checkBoxDirectory
            // 
            this.checkBoxDirectory.AutoSize = true;
            this.checkBoxDirectory.Location = new System.Drawing.Point(6, 18);
            this.checkBoxDirectory.Name = "checkBoxDirectory";
            this.checkBoxDirectory.Size = new System.Drawing.Size(190, 16);
            this.checkBoxDirectory.TabIndex = 0;
            this.checkBoxDirectory.Text = "フォルダを確認なしですべて削除(&D)";
            this.checkBoxDirectory.UseVisualStyleBackColor = true;
            this.checkBoxDirectory.CheckedChanged += new System.EventHandler(this.checkBoxDelete_CheckedChanged);
            // 
            // labelAttr
            // 
            this.labelAttr.AutoSize = true;
            this.labelAttr.BackColor = System.Drawing.Color.Yellow;
            this.labelAttr.Location = new System.Drawing.Point(328, 37);
            this.labelAttr.Name = "labelAttr";
            this.labelAttr.Size = new System.Drawing.Size(244, 12);
            this.labelAttr.TabIndex = 4;
            this.labelAttr.Text = "削除中に確認メッセージが表示されることがあります";
            // 
            // labelDirectory
            // 
            this.labelDirectory.AutoSize = true;
            this.labelDirectory.BackColor = System.Drawing.Color.Yellow;
            this.labelDirectory.Location = new System.Drawing.Point(328, 19);
            this.labelDirectory.Name = "labelDirectory";
            this.labelDirectory.Size = new System.Drawing.Size(244, 12);
            this.labelDirectory.TabIndex = 3;
            this.labelDirectory.Text = "削除中に確認メッセージが表示されることがあります";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(543, 394);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(462, 394);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // textBoxDest
            // 
            this.textBoxDest.Location = new System.Drawing.Point(78, 154);
            this.textBoxDest.Name = "textBoxDest";
            this.textBoxDest.ReadOnly = true;
            this.textBoxDest.Size = new System.Drawing.Size(536, 19);
            this.textBoxDest.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "転送先(&T):";
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxIcon.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxIcon.TabIndex = 10;
            this.pictureBoxIcon.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(493, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "フォルダをコピーしたとき、そのフォルダ配下で転送元にないファイルやフォルダは、転送先から削除されます。";
            // 
            // MirrorCopyDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(628, 429);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.textBoxDest);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listViewTarget);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MirrorCopyDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ミラーコピー";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MirrorCopyDialog_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.ListView listViewTarget;
        private System.Windows.Forms.RadioButton radioButtonSizeDate;
        private System.Windows.Forms.RadioButton radioButtonNotOverwrite;
        private System.Windows.Forms.RadioButton radioButtonIfNewer;
        private System.Windows.Forms.RadioButton radioButtonForce;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxExcept;
        private System.Windows.Forms.Label labelAttrMessage;
        private System.Windows.Forms.CheckBox checkBoxAttrCopy;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxWithRecycle;
        private System.Windows.Forms.CheckBox checkBoxAttr;
        private System.Windows.Forms.CheckBox checkBoxDirectory;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.TextBox textBoxDest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelAttr;
        private System.Windows.Forms.Label labelDirectory;
    }
}