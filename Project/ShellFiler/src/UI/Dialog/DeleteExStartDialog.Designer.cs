﻿namespace ShellFiler.UI.Dialog
{
    partial class DeleteExStartDialog
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
            this.listViewTarget = new System.Windows.Forms.ListView();
            this.checkBoxDirectory = new System.Windows.Forms.CheckBox();
            this.checkBoxAttr = new System.Windows.Forms.CheckBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            this.checkBoxWithRecycle = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonSetting = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonWild = new System.Windows.Forms.RadioButton();
            this.textBoxWildCard = new System.Windows.Forms.TextBox();
            this.checkedListCondition = new System.Windows.Forms.CheckedListBox();
            this.checkBoxCondition = new System.Windows.Forms.CheckBox();
            this.buttonSetting = new System.Windows.Forms.Button();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.checkBoxSuspend = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewTarget
            // 
            this.listViewTarget.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewTarget.HideSelection = false;
            this.listViewTarget.Location = new System.Drawing.Point(18, 48);
            this.listViewTarget.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listViewTarget.Name = "listViewTarget";
            this.listViewTarget.Size = new System.Drawing.Size(628, 160);
            this.listViewTarget.TabIndex = 1;
            this.listViewTarget.UseCompatibleStateImageBehavior = false;
            this.listViewTarget.View = System.Windows.Forms.View.Details;
            // 
            // checkBoxDirectory
            // 
            this.checkBoxDirectory.AutoSize = true;
            this.checkBoxDirectory.Location = new System.Drawing.Point(18, 219);
            this.checkBoxDirectory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxDirectory.Name = "checkBoxDirectory";
            this.checkBoxDirectory.Size = new System.Drawing.Size(285, 29);
            this.checkBoxDirectory.TabIndex = 2;
            this.checkBoxDirectory.Text = "フォルダを確認なしですべて削除(&D)";
            this.checkBoxDirectory.UseVisualStyleBackColor = true;
            // 
            // checkBoxAttr
            // 
            this.checkBoxAttr.AutoSize = true;
            this.checkBoxAttr.Location = new System.Drawing.Point(18, 248);
            this.checkBoxAttr.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxAttr.Name = "checkBoxAttr";
            this.checkBoxAttr.Size = new System.Drawing.Size(464, 29);
            this.checkBoxAttr.TabIndex = 3;
            this.checkBoxAttr.Text = "読み込み/システム属性のファイルを確認なしですべて削除(&A)";
            this.checkBoxAttr.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(414, 692);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(112, 34);
            this.buttonOk.TabIndex = 7;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(536, 692);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(112, 34);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
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
            // checkBoxWithRecycle
            // 
            this.checkBoxWithRecycle.AutoSize = true;
            this.checkBoxWithRecycle.Location = new System.Drawing.Point(18, 276);
            this.checkBoxWithRecycle.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxWithRecycle.Name = "checkBoxWithRecycle";
            this.checkBoxWithRecycle.Size = new System.Drawing.Size(198, 29);
            this.checkBoxWithRecycle.TabIndex = 4;
            this.checkBoxWithRecycle.Text = "ごみ箱を使って削除(&R)";
            this.checkBoxWithRecycle.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.radioButtonSetting);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioButtonWild);
            this.groupBox1.Controls.Add(this.textBoxWildCard);
            this.groupBox1.Controls.Add(this.checkBoxCondition);
            this.groupBox1.Controls.Add(this.buttonSetting);
            this.groupBox1.Location = new System.Drawing.Point(18, 358);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(630, 324);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // radioButtonSetting
            // 
            this.radioButtonSetting.AutoSize = true;
            this.radioButtonSetting.Location = new System.Drawing.Point(12, 33);
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
            this.label2.Location = new System.Drawing.Point(39, 286);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(346, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "「:」区切りで複数指定可能、ファイルだけが対象";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(501, 225);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "↑↓で切り替え";
            // 
            // radioButtonWild
            // 
            this.radioButtonWild.AutoSize = true;
            this.radioButtonWild.Location = new System.Drawing.Point(12, 213);
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
            this.textBoxWildCard.Location = new System.Drawing.Point(42, 250);
            this.textBoxWildCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxWildCard.Name = "textBoxWildCard";
            this.textBoxWildCard.Size = new System.Drawing.Size(577, 31);
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
            this.checkedListCondition.Size = new System.Drawing.Size(575, 137);
            this.checkedListCondition.TabIndex = 2;
            // 
            // checkBoxCondition
            // 
            this.checkBoxCondition.AutoSize = true;
            this.checkBoxCondition.Location = new System.Drawing.Point(12, 0);
            this.checkBoxCondition.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxCondition.Name = "checkBoxCondition";
            this.checkBoxCondition.Size = new System.Drawing.Size(166, 29);
            this.checkBoxCondition.TabIndex = 0;
            this.checkBoxCondition.Text = "条件付き削除(&O)";
            this.checkBoxCondition.UseVisualStyleBackColor = true;
            // 
            // buttonSetting
            // 
            this.buttonSetting.Location = new System.Drawing.Point(508, 22);
            this.buttonSetting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSetting.Name = "buttonSetting";
            this.buttonSetting.Size = new System.Drawing.Size(112, 34);
            this.buttonSetting.TabIndex = 3;
            this.buttonSetting.Text = "設定(&S)...";
            this.buttonSetting.UseVisualStyleBackColor = true;
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Location = new System.Drawing.Point(18, 15);
            this.pictureBoxIcon.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(24, 24);
            this.pictureBoxIcon.TabIndex = 8;
            this.pictureBoxIcon.TabStop = false;
            // 
            // checkBoxSuspend
            // 
            this.checkBoxSuspend.AutoSize = true;
            this.checkBoxSuspend.Location = new System.Drawing.Point(18, 304);
            this.checkBoxSuspend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxSuspend.Name = "checkBoxSuspend";
            this.checkBoxSuspend.Size = new System.Drawing.Size(244, 29);
            this.checkBoxSuspend.TabIndex = 5;
            this.checkBoxSuspend.Text = "待機状態のタスクを作成(&W)";
            this.checkBoxSuspend.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkedListCondition);
            this.panel1.Location = new System.Drawing.Point(44, 69);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(575, 137);
            this.panel1.TabIndex = 8;
            // 
            // DeleteExStartDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(670, 744);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.checkBoxSuspend);
            this.Controls.Add(this.checkBoxWithRecycle);
            this.Controls.Add(this.checkBoxAttr);
            this.Controls.Add(this.checkBoxDirectory);
            this.Controls.Add(this.listViewTarget);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeleteExStartDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ファイル削除の確認";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DeleteStartDialog_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewTarget;
        private System.Windows.Forms.CheckBox checkBoxDirectory;
        private System.Windows.Forms.CheckBox checkBoxAttr;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.CheckBox checkBoxWithRecycle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonSetting;
        private System.Windows.Forms.CheckBox checkBoxCondition;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.CheckedListBox checkedListCondition;
        private System.Windows.Forms.RadioButton radioButtonSetting;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonWild;
        private System.Windows.Forms.TextBox textBoxWildCard;
        private System.Windows.Forms.CheckBox checkBoxSuspend;
        private System.Windows.Forms.Panel panel1;
    }
}