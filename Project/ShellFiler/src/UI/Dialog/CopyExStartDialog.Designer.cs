namespace ShellFiler.UI.Dialog
{
    partial class CopyExStartDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonSetting = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonWild = new System.Windows.Forms.RadioButton();
            this.textBoxWildCard = new System.Windows.Forms.TextBox();
            this.checkedListCondition = new System.Windows.Forms.CheckedListBox();
            this.buttonSetting = new System.Windows.Forms.Button();
            this.checkBoxCondition = new System.Windows.Forms.CheckBox();
            this.textBoxDest = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelMessage = new System.Windows.Forms.Label();
            this.listViewTarget = new System.Windows.Forms.ListView();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelAttrCopy = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxSuspend = new System.Windows.Forms.CheckBox();
            this.checkBoxAttrCopy = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(687, 609);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(112, 34);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(808, 609);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(112, 34);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.radioButtonSetting);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioButtonWild);
            this.groupBox1.Controls.Add(this.textBoxWildCard);
            this.groupBox1.Controls.Add(this.buttonSetting);
            this.groupBox1.Controls.Add(this.checkBoxCondition);
            this.groupBox1.Location = new System.Drawing.Point(18, 260);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(546, 338);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // radioButtonSetting
            // 
            this.radioButtonSetting.AutoSize = true;
            this.radioButtonSetting.Location = new System.Drawing.Point(10, 36);
            this.radioButtonSetting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonSetting.Name = "radioButtonSetting";
            this.radioButtonSetting.Size = new System.Drawing.Size(229, 29);
            this.radioButtonSetting.TabIndex = 1;
            this.radioButtonSetting.TabStop = true;
            this.radioButtonSetting.Text = "設定済みの条件で選択(&C)";
            this.radioButtonSetting.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 292);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(346, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "「:」区切りで複数指定可能、ファイルだけが対象";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(408, 228);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "↑↓で切り替え";
            // 
            // radioButtonWild
            // 
            this.radioButtonWild.AutoSize = true;
            this.radioButtonWild.Location = new System.Drawing.Point(10, 216);
            this.radioButtonWild.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonWild.Name = "radioButtonWild";
            this.radioButtonWild.Size = new System.Drawing.Size(323, 29);
            this.radioButtonWild.TabIndex = 4;
            this.radioButtonWild.TabStop = true;
            this.radioButtonWild.Text = "ファイル名のワイルドカードで簡易指定(&Q)";
            this.radioButtonWild.UseVisualStyleBackColor = true;
            // 
            // textBoxWildCard
            // 
            this.textBoxWildCard.Location = new System.Drawing.Point(40, 254);
            this.textBoxWildCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxWildCard.Name = "textBoxWildCard";
            this.textBoxWildCard.Size = new System.Drawing.Size(486, 31);
            this.textBoxWildCard.TabIndex = 5;
            // 
            // checkedListCondition
            // 
            this.checkedListCondition.CheckOnClick = true;
            this.checkedListCondition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListCondition.FormattingEnabled = true;
            this.checkedListCondition.Location = new System.Drawing.Point(0, 0);
            this.checkedListCondition.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkedListCondition.Name = "checkedListCondition";
            this.checkedListCondition.Size = new System.Drawing.Size(483, 115);
            this.checkedListCondition.TabIndex = 2;
            // 
            // buttonSetting
            // 
            this.buttonSetting.Location = new System.Drawing.Point(416, 30);
            this.buttonSetting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSetting.Name = "buttonSetting";
            this.buttonSetting.Size = new System.Drawing.Size(112, 34);
            this.buttonSetting.TabIndex = 3;
            this.buttonSetting.Text = "設定(&S)...";
            this.buttonSetting.UseVisualStyleBackColor = true;
            // 
            // checkBoxCondition
            // 
            this.checkBoxCondition.AutoSize = true;
            this.checkBoxCondition.Location = new System.Drawing.Point(15, 0);
            this.checkBoxCondition.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxCondition.Name = "checkBoxCondition";
            this.checkBoxCondition.Size = new System.Drawing.Size(168, 29);
            this.checkBoxCondition.TabIndex = 0;
            this.checkBoxCondition.Text = "条件付きコピー(&O)";
            this.checkBoxCondition.UseVisualStyleBackColor = true;
            // 
            // textBoxDest
            // 
            this.textBoxDest.Location = new System.Drawing.Point(117, 219);
            this.textBoxDest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxDest.Name = "textBoxDest";
            this.textBoxDest.ReadOnly = true;
            this.textBoxDest.Size = new System.Drawing.Size(802, 31);
            this.textBoxDest.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 224);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "転送先(&T):";
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(51, 16);
            this.labelMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(82, 25);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "message";
            // 
            // listViewTarget
            // 
            this.listViewTarget.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewTarget.HideSelection = false;
            this.listViewTarget.Location = new System.Drawing.Point(18, 48);
            this.listViewTarget.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listViewTarget.Name = "listViewTarget";
            this.listViewTarget.Size = new System.Drawing.Size(901, 160);
            this.listViewTarget.TabIndex = 1;
            this.listViewTarget.UseCompatibleStateImageBehavior = false;
            this.listViewTarget.View = System.Windows.Forms.View.Details;
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxIcon.Location = new System.Drawing.Point(18, 15);
            this.pictureBoxIcon.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(24, 24);
            this.pictureBoxIcon.TabIndex = 9;
            this.pictureBoxIcon.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelAttrCopy);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.checkBoxSuspend);
            this.groupBox2.Controls.Add(this.checkBoxAttrCopy);
            this.groupBox2.Location = new System.Drawing.Point(573, 260);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(348, 224);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "その他の設定";
            // 
            // labelAttrCopy
            // 
            this.labelAttrCopy.AutoSize = true;
            this.labelAttrCopy.Location = new System.Drawing.Point(28, 84);
            this.labelAttrCopy.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAttrCopy.Name = "labelAttrCopy";
            this.labelAttrCopy.Size = new System.Drawing.Size(223, 25);
            this.labelAttrCopy.TabIndex = 2;
            this.labelAttrCopy.Text = "現在のオプションは「{0}」です。";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 178);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(249, 25);
            this.label5.TabIndex = 5;
            this.label5.Text = "その処理の完了後に開始します。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 154);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(253, 25);
            this.label4.TabIndex = 4;
            this.label4.Text = "ほかに実行中の処理がある場合、";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 63);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(301, 25);
            this.label6.TabIndex = 1;
            this.label6.Text = "未設定のときはオプションにしたがいます。";
            // 
            // checkBoxSuspend
            // 
            this.checkBoxSuspend.AutoSize = true;
            this.checkBoxSuspend.Location = new System.Drawing.Point(10, 126);
            this.checkBoxSuspend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxSuspend.Name = "checkBoxSuspend";
            this.checkBoxSuspend.Size = new System.Drawing.Size(244, 29);
            this.checkBoxSuspend.TabIndex = 3;
            this.checkBoxSuspend.Text = "待機状態のタスクを作成(&W)";
            this.checkBoxSuspend.UseVisualStyleBackColor = true;
            // 
            // checkBoxAttrCopy
            // 
            this.checkBoxAttrCopy.AutoSize = true;
            this.checkBoxAttrCopy.Location = new System.Drawing.Point(10, 32);
            this.checkBoxAttrCopy.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxAttrCopy.Name = "checkBoxAttrCopy";
            this.checkBoxAttrCopy.Size = new System.Drawing.Size(274, 29);
            this.checkBoxAttrCopy.TabIndex = 0;
            this.checkBoxAttrCopy.Text = "転送後にすべての属性をコピー(&A)";
            this.checkBoxAttrCopy.ThreeState = true;
            this.checkBoxAttrCopy.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkedListCondition);
            this.panel1.Location = new System.Drawing.Point(43, 73);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(483, 115);
            this.panel1.TabIndex = 8;
            // 
            // CopyExStartDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(940, 662);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.listViewTarget);
            this.Controls.Add(this.textBoxDest);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CopyExStartDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "方式を指定してコピー";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DeleteStartDialog_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxDest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxCondition;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.ListView listViewTarget;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.RadioButton radioButtonSetting;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonWild;
        private System.Windows.Forms.TextBox textBoxWildCard;
        private System.Windows.Forms.CheckedListBox checkedListCondition;
        private System.Windows.Forms.Button buttonSetting;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelAttrCopy;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxAttrCopy;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxSuspend;
        private System.Windows.Forms.Panel panel1;
    }
}