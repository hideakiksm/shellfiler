namespace ShellFiler.UI.Dialog {
    partial class FileFilterClipboardDialog {
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
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.listBoxUse = new System.Windows.Forms.ListBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.listBoxAllFilter = new System.Windows.Forms.ListBox();
            this.panelFilterProp = new System.Windows.Forms.Panel();
            this.buttonRef = new System.Windows.Forms.Button();
            this.buttonQuick1 = new System.Windows.Forms.Button();
            this.buttonQuick2 = new System.Windows.Forms.Button();
            this.buttonQuick3 = new System.Windows.Forms.Button();
            this.buttonQuickSetting = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonQuick4 = new System.Windows.Forms.Button();
            this.comboBoxCharset = new System.Windows.Forms.ComboBox();
            this.labelCharset = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelFreeware = new System.Windows.Forms.Label();
            this.radioButtonFile = new System.Windows.Forms.RadioButton();
            this.textBoxDest = new System.Windows.Forms.TextBox();
            this.radioButtonClipboard = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "保存先:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "使用するフィルター(&F):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(327, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(105, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "すべてのフィルター(&N):";
            // 
            // listBoxUse
            // 
            this.listBoxUse.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBoxUse.FormattingEnabled = true;
            this.listBoxUse.ItemHeight = 12;
            this.listBoxUse.Location = new System.Drawing.Point(12, 99);
            this.listBoxUse.Name = "listBoxUse";
            this.listBoxUse.Size = new System.Drawing.Size(230, 148);
            this.listBoxUse.TabIndex = 9;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(248, 99);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 10;
            this.buttonAdd.Text = "<< 追加(&A)";
            this.buttonAdd.UseVisualStyleBackColor = true;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(248, 128);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.Text = "削除(&L) >>";
            this.buttonDelete.UseVisualStyleBackColor = true;
            // 
            // buttonUp
            // 
            this.buttonUp.Location = new System.Drawing.Point(248, 195);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(75, 23);
            this.buttonUp.TabIndex = 12;
            this.buttonUp.Text = "上へ(&U)";
            this.buttonUp.UseVisualStyleBackColor = true;
            // 
            // buttonDown
            // 
            this.buttonDown.Location = new System.Drawing.Point(248, 224);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(75, 23);
            this.buttonDown.TabIndex = 13;
            this.buttonDown.Text = "下へ(&D)";
            this.buttonDown.UseVisualStyleBackColor = true;
            // 
            // listBoxAllFilter
            // 
            this.listBoxAllFilter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxAllFilter.FormattingEnabled = true;
            this.listBoxAllFilter.ItemHeight = 16;
            this.listBoxAllFilter.Location = new System.Drawing.Point(329, 99);
            this.listBoxAllFilter.Name = "listBoxAllFilter";
            this.listBoxAllFilter.Size = new System.Drawing.Size(230, 148);
            this.listBoxAllFilter.TabIndex = 15;
            // 
            // panelFilterProp
            // 
            this.panelFilterProp.BackColor = System.Drawing.SystemColors.Window;
            this.panelFilterProp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFilterProp.Location = new System.Drawing.Point(12, 253);
            this.panelFilterProp.Name = "panelFilterProp";
            this.panelFilterProp.Size = new System.Drawing.Size(656, 156);
            this.panelFilterProp.TabIndex = 21;
            // 
            // buttonRef
            // 
            this.buttonRef.Location = new System.Drawing.Point(593, 30);
            this.buttonRef.Name = "buttonRef";
            this.buttonRef.Size = new System.Drawing.Size(75, 23);
            this.buttonRef.TabIndex = 6;
            this.buttonRef.Text = "参照(&R)...";
            this.buttonRef.UseVisualStyleBackColor = true;
            this.buttonRef.Click += new System.EventHandler(this.buttonRef_Click);
            // 
            // buttonQuick1
            // 
            this.buttonQuick1.Location = new System.Drawing.Point(565, 99);
            this.buttonQuick1.Name = "buttonQuick1";
            this.buttonQuick1.Size = new System.Drawing.Size(103, 23);
            this.buttonQuick1.TabIndex = 16;
            this.buttonQuick1.Text = "クイック1(&1)";
            this.buttonQuick1.UseVisualStyleBackColor = true;
            this.buttonQuick1.Click += new System.EventHandler(this.buttonQuick_Click);
            // 
            // buttonQuick2
            // 
            this.buttonQuick2.Location = new System.Drawing.Point(565, 128);
            this.buttonQuick2.Name = "buttonQuick2";
            this.buttonQuick2.Size = new System.Drawing.Size(103, 23);
            this.buttonQuick2.TabIndex = 17;
            this.buttonQuick2.Text = "クイック2(&2)";
            this.buttonQuick2.UseVisualStyleBackColor = true;
            this.buttonQuick2.Click += new System.EventHandler(this.buttonQuick_Click);
            // 
            // buttonQuick3
            // 
            this.buttonQuick3.Location = new System.Drawing.Point(565, 157);
            this.buttonQuick3.Name = "buttonQuick3";
            this.buttonQuick3.Size = new System.Drawing.Size(103, 23);
            this.buttonQuick3.TabIndex = 18;
            this.buttonQuick3.Text = "クイック3(&3)";
            this.buttonQuick3.UseVisualStyleBackColor = true;
            this.buttonQuick3.Click += new System.EventHandler(this.buttonQuick_Click);
            // 
            // buttonQuickSetting
            // 
            this.buttonQuickSetting.Location = new System.Drawing.Point(565, 224);
            this.buttonQuickSetting.Name = "buttonQuickSetting";
            this.buttonQuickSetting.Size = new System.Drawing.Size(103, 23);
            this.buttonQuickSetting.TabIndex = 20;
            this.buttonQuickSetting.Text = "クイック設定(&S)...";
            this.buttonQuickSetting.UseVisualStyleBackColor = true;
            this.buttonQuickSetting.Click += new System.EventHandler(this.buttonQuickSetting_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(593, 415);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 23;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(512, 415);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 22;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonQuick4
            // 
            this.buttonQuick4.Location = new System.Drawing.Point(565, 186);
            this.buttonQuick4.Name = "buttonQuick4";
            this.buttonQuick4.Size = new System.Drawing.Size(103, 23);
            this.buttonQuick4.TabIndex = 19;
            this.buttonQuick4.Text = "クイック4(&4)";
            this.buttonQuick4.UseVisualStyleBackColor = true;
            this.buttonQuick4.Click += new System.EventHandler(this.buttonQuick_Click);
            // 
            // comboBoxCharset
            // 
            this.comboBoxCharset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCharset.FormattingEnabled = true;
            this.comboBoxCharset.Location = new System.Drawing.Point(94, 6);
            this.comboBoxCharset.Name = "comboBoxCharset";
            this.comboBoxCharset.Size = new System.Drawing.Size(112, 20);
            this.comboBoxCharset.TabIndex = 1;
            // 
            // labelCharset
            // 
            this.labelCharset.AutoSize = true;
            this.labelCharset.Location = new System.Drawing.Point(10, 9);
            this.labelCharset.Name = "labelCharset";
            this.labelCharset.Size = new System.Drawing.Size(74, 12);
            this.labelCharset.TabIndex = 0;
            this.labelCharset.Text = "文字コード(&C):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(212, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(396, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "クリップボードの文字列をこの文字コードにしてから、フィルターを適用して保存します。";
            // 
            // labelFreeware
            // 
            this.labelFreeware.AutoSize = true;
            this.labelFreeware.Location = new System.Drawing.Point(12, 426);
            this.labelFreeware.Name = "labelFreeware";
            this.labelFreeware.Size = new System.Drawing.Size(0, 12);
            this.labelFreeware.TabIndex = 24;
            // 
            // radioButtonFile
            // 
            this.radioButtonFile.AutoSize = true;
            this.radioButtonFile.Location = new System.Drawing.Point(94, 33);
            this.radioButtonFile.Name = "radioButtonFile";
            this.radioButtonFile.Size = new System.Drawing.Size(68, 16);
            this.radioButtonFile.TabIndex = 4;
            this.radioButtonFile.TabStop = true;
            this.radioButtonFile.Text = "ファイル(&I)";
            this.radioButtonFile.UseVisualStyleBackColor = true;
            this.radioButtonFile.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // textBoxDest
            // 
            this.textBoxDest.Location = new System.Drawing.Point(191, 32);
            this.textBoxDest.Name = "textBoxDest";
            this.textBoxDest.Size = new System.Drawing.Size(396, 19);
            this.textBoxDest.TabIndex = 5;
            // 
            // radioButtonClipboard
            // 
            this.radioButtonClipboard.AutoSize = true;
            this.radioButtonClipboard.Location = new System.Drawing.Point(94, 54);
            this.radioButtonClipboard.Name = "radioButtonClipboard";
            this.radioButtonClipboard.Size = new System.Drawing.Size(98, 16);
            this.radioButtonClipboard.TabIndex = 7;
            this.radioButtonClipboard.TabStop = true;
            this.radioButtonClipboard.Text = "クリップボード(&P)";
            this.radioButtonClipboard.UseVisualStyleBackColor = true;
            this.radioButtonClipboard.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // FileFilterClipboardDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(680, 448);
            this.Controls.Add(this.textBoxDest);
            this.Controls.Add(this.radioButtonClipboard);
            this.Controls.Add(this.radioButtonFile);
            this.Controls.Add(this.labelFreeware);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelCharset);
            this.Controls.Add(this.comboBoxCharset);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonQuickSetting);
            this.Controls.Add(this.buttonQuick4);
            this.Controls.Add(this.buttonQuick3);
            this.Controls.Add(this.buttonQuick2);
            this.Controls.Add(this.buttonQuick1);
            this.Controls.Add(this.buttonRef);
            this.Controls.Add(this.panelFilterProp);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.listBoxAllFilter);
            this.Controls.Add(this.listBoxUse);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileFilterClipboardDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "クリップボードをテキスト変換";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox listBoxUse;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.ListBox listBoxAllFilter;
        private System.Windows.Forms.Panel panelFilterProp;
        private System.Windows.Forms.Button buttonRef;
        private System.Windows.Forms.Button buttonQuick1;
        private System.Windows.Forms.Button buttonQuick2;
        private System.Windows.Forms.Button buttonQuick3;
        private System.Windows.Forms.Button buttonQuickSetting;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonQuick4;
        private System.Windows.Forms.ComboBox comboBoxCharset;
        private System.Windows.Forms.Label labelCharset;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelFreeware;
        private System.Windows.Forms.RadioButton radioButtonFile;
        private System.Windows.Forms.TextBox textBoxDest;
        private System.Windows.Forms.RadioButton radioButtonClipboard;
    }
}