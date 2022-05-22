namespace ShellFiler.UI.Dialog {
    partial class MarkWithConditionsDialog {
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
            this.radioButtonSetting = new System.Windows.Forms.RadioButton();
            this.radioButtonWild = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonSelectClear = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.radioButtonSelectSet = new System.Windows.Forms.RadioButton();
            this.radioButtonSelectRevert = new System.Windows.Forms.RadioButton();
            this.checkedListCondition = new System.Windows.Forms.CheckedListBox();
            this.buttonSetting = new System.Windows.Forms.Button();
            this.textBoxWildCard = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonSetting
            // 
            this.radioButtonSetting.AutoSize = true;
            this.radioButtonSetting.Location = new System.Drawing.Point(15, 27);
            this.radioButtonSetting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonSetting.Name = "radioButtonSetting";
            this.radioButtonSetting.Size = new System.Drawing.Size(229, 29);
            this.radioButtonSetting.TabIndex = 0;
            this.radioButtonSetting.TabStop = true;
            this.radioButtonSetting.Text = "設定済みの条件で選択(&C)";
            this.radioButtonSetting.UseVisualStyleBackColor = true;
            // 
            // radioButtonWild
            // 
            this.radioButtonWild.AutoSize = true;
            this.radioButtonWild.Location = new System.Drawing.Point(15, 204);
            this.radioButtonWild.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonWild.Name = "radioButtonWild";
            this.radioButtonWild.Size = new System.Drawing.Size(323, 29);
            this.radioButtonWild.TabIndex = 3;
            this.radioButtonWild.TabStop = true;
            this.radioButtonWild.Text = "ファイル名のワイルドカードで簡易指定(&Q)";
            this.radioButtonWild.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonSelectClear);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.radioButtonSelectSet);
            this.groupBox1.Controls.Add(this.radioButtonSelectRevert);
            this.groupBox1.Location = new System.Drawing.Point(585, 66);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(300, 171);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "選択状態の操作";
            // 
            // radioButtonSelectClear
            // 
            this.radioButtonSelectClear.AutoSize = true;
            this.radioButtonSelectClear.Location = new System.Drawing.Point(16, 94);
            this.radioButtonSelectClear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonSelectClear.Name = "radioButtonSelectClear";
            this.radioButtonSelectClear.Size = new System.Drawing.Size(182, 29);
            this.radioButtonSelectClear.TabIndex = 2;
            this.radioButtonSelectClear.TabStop = true;
            this.radioButtonSelectClear.Text = "選択状態をクリア(&L)";
            this.radioButtonSelectClear.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 142);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(272, 25);
            this.label5.TabIndex = 6;
            this.label5.Text = "ファイルとフォルダの両方が対象です。";
            // 
            // radioButtonSelectSet
            // 
            this.radioButtonSelectSet.AutoSize = true;
            this.radioButtonSelectSet.Location = new System.Drawing.Point(16, 62);
            this.radioButtonSelectSet.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonSelectSet.Name = "radioButtonSelectSet";
            this.radioButtonSelectSet.Size = new System.Drawing.Size(178, 29);
            this.radioButtonSelectSet.TabIndex = 1;
            this.radioButtonSelectSet.TabStop = true;
            this.radioButtonSelectSet.Text = "選択状態に設定(&T)";
            this.radioButtonSelectSet.UseVisualStyleBackColor = true;
            // 
            // radioButtonSelectRevert
            // 
            this.radioButtonSelectRevert.AutoSize = true;
            this.radioButtonSelectRevert.Location = new System.Drawing.Point(16, 28);
            this.radioButtonSelectRevert.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonSelectRevert.Name = "radioButtonSelectRevert";
            this.radioButtonSelectRevert.Size = new System.Drawing.Size(180, 29);
            this.radioButtonSelectRevert.TabIndex = 0;
            this.radioButtonSelectRevert.TabStop = true;
            this.radioButtonSelectRevert.Text = "選択状態を反転(&R)";
            this.radioButtonSelectRevert.UseVisualStyleBackColor = true;
            // 
            // checkedListCondition
            // 
            this.checkedListCondition.CheckOnClick = true;
            this.checkedListCondition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListCondition.FormattingEnabled = true;
            this.checkedListCondition.Location = new System.Drawing.Point(0, 0);
            this.checkedListCondition.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkedListCondition.Name = "checkedListCondition";
            this.checkedListCondition.Size = new System.Drawing.Size(487, 117);
            this.checkedListCondition.TabIndex = 1;
            // 
            // buttonSetting
            // 
            this.buttonSetting.Location = new System.Drawing.Point(420, 22);
            this.buttonSetting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSetting.Name = "buttonSetting";
            this.buttonSetting.Size = new System.Drawing.Size(112, 34);
            this.buttonSetting.TabIndex = 2;
            this.buttonSetting.Text = "設定(&S)...";
            this.buttonSetting.UseVisualStyleBackColor = true;
            // 
            // textBoxWildCard
            // 
            this.textBoxWildCard.Location = new System.Drawing.Point(45, 238);
            this.textBoxWildCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxWildCard.Name = "textBoxWildCard";
            this.textBoxWildCard.Size = new System.Drawing.Size(486, 31);
            this.textBoxWildCard.TabIndex = 4;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(776, 374);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(112, 34);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(654, 374);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(112, 34);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(412, 213);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 25);
            this.label1.TabIndex = 5;
            this.label1.Text = "↑↓で切り替え";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.radioButtonSetting);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.radioButtonWild);
            this.groupBox2.Controls.Add(this.textBoxWildCard);
            this.groupBox2.Controls.Add(this.buttonSetting);
            this.groupBox2.Location = new System.Drawing.Point(22, 66);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(554, 315);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "選択状態を変えるファイルの条件";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 278);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(384, 25);
            this.label2.TabIndex = 6;
            this.label2.Text = "「:」区切りで複数指定可能、ファイルとフォルダが対象";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 14);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(453, 25);
            this.label3.TabIndex = 6;
            this.label3.Text = "指定の条件に一致するファイルをまとめて選択状態にできます。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 34);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(559, 25);
            this.label4.TabIndex = 6;
            this.label4.Text = "たとえば、今日の12時以降に更新された*.bakだけをマークすることができます。";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkedListCondition);
            this.panel1.Location = new System.Drawing.Point(44, 62);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(487, 117);
            this.panel1.TabIndex = 7;
            // 
            // MarkWithConditionsDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(906, 426);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MarkWithConditionsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "条件を指定して選択";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MarkWithConditionsDialog_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonSetting;
        private System.Windows.Forms.RadioButton radioButtonWild;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox checkedListCondition;
        private System.Windows.Forms.Button buttonSetting;
        private System.Windows.Forms.RadioButton radioButtonSelectClear;
        private System.Windows.Forms.RadioButton radioButtonSelectSet;
        private System.Windows.Forms.RadioButton radioButtonSelectRevert;
        private System.Windows.Forms.TextBox textBoxWildCard;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
    }
}