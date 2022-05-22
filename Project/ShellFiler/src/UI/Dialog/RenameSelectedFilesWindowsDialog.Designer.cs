namespace ShellFiler.UI.Dialog {
    partial class RenameSelectedFilesWindowsDialog {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonSequential = new System.Windows.Forms.Button();
            this.buttonNameLower = new System.Windows.Forms.Button();
            this.buttonNameUpper = new System.Windows.Forms.Button();
            this.comboBoxNameExt = new System.Windows.Forms.ComboBox();
            this.comboBoxNameBody = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxSystem = new System.Windows.Forms.CheckBox();
            this.checkBoxArchive = new System.Windows.Forms.CheckBox();
            this.checkBoxHidden = new System.Windows.Forms.CheckBox();
            this.checkBoxReadOnly = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTimeAccess = new System.Windows.Forms.MaskedTextBox();
            this.textBoxTimeCreate = new System.Windows.Forms.MaskedTextBox();
            this.textBoxTimeUpdate = new System.Windows.Forms.MaskedTextBox();
            this.buttonTimeAccess = new System.Windows.Forms.Button();
            this.buttonTimeCreate = new System.Windows.Forms.Button();
            this.buttonTimeUpdate = new System.Windows.Forms.Button();
            this.buttonCurrent = new System.Windows.Forms.Button();
            this.buttonNoon = new System.Windows.Forms.Button();
            this.dateTimeDateAccess = new System.Windows.Forms.DateTimePicker();
            this.checkBoxTimeAccess = new System.Windows.Forms.CheckBox();
            this.dateTimeDateCreate = new System.Windows.Forms.DateTimePicker();
            this.checkBoxTimeCreate = new System.Windows.Forms.CheckBox();
            this.checkBoxDateAccess = new System.Windows.Forms.CheckBox();
            this.dateTimeDateUpdate = new System.Windows.Forms.DateTimePicker();
            this.checkBoxDateCreate = new System.Windows.Forms.CheckBox();
            this.checkBoxTimeUpdate = new System.Windows.Forms.CheckBox();
            this.checkBoxDateUpdate = new System.Windows.Forms.CheckBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.comboBoxFolder = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonSequential);
            this.groupBox1.Controls.Add(this.buttonNameLower);
            this.groupBox1.Controls.Add(this.buttonNameUpper);
            this.groupBox1.Controls.Add(this.comboBoxNameExt);
            this.groupBox1.Controls.Add(this.comboBoxNameBody);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(359, 120);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ファイル名";
            // 
            // buttonSequential
            // 
            this.buttonSequential.Location = new System.Drawing.Point(171, 74);
            this.buttonSequential.Name = "buttonSequential";
            this.buttonSequential.Size = new System.Drawing.Size(75, 23);
            this.buttonSequential.TabIndex = 6;
            this.buttonSequential.Text = "連番(&N)...";
            this.buttonSequential.UseVisualStyleBackColor = true;
            this.buttonSequential.Click += new System.EventHandler(this.buttonSequential_Click);
            // 
            // buttonNameLower
            // 
            this.buttonNameLower.Location = new System.Drawing.Point(90, 74);
            this.buttonNameLower.Name = "buttonNameLower";
            this.buttonNameLower.Size = new System.Drawing.Size(75, 23);
            this.buttonNameLower.TabIndex = 5;
            this.buttonNameLower.Text = "小文字(&L)";
            this.buttonNameLower.UseVisualStyleBackColor = true;
            this.buttonNameLower.Click += new System.EventHandler(this.buttonNameLower_Click);
            // 
            // buttonNameUpper
            // 
            this.buttonNameUpper.Location = new System.Drawing.Point(9, 74);
            this.buttonNameUpper.Name = "buttonNameUpper";
            this.buttonNameUpper.Size = new System.Drawing.Size(75, 23);
            this.buttonNameUpper.TabIndex = 4;
            this.buttonNameUpper.Text = "大文字(&P)";
            this.buttonNameUpper.UseVisualStyleBackColor = true;
            this.buttonNameUpper.Click += new System.EventHandler(this.buttonNameUpper_Click);
            // 
            // comboBoxNameExt
            // 
            this.comboBoxNameExt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNameExt.FormattingEnabled = true;
            this.comboBoxNameExt.Location = new System.Drawing.Point(106, 45);
            this.comboBoxNameExt.Name = "comboBoxNameExt";
            this.comboBoxNameExt.Size = new System.Drawing.Size(238, 23);
            this.comboBoxNameExt.TabIndex = 3;
            // 
            // comboBoxNameBody
            // 
            this.comboBoxNameBody.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNameBody.FormattingEnabled = true;
            this.comboBoxNameBody.Location = new System.Drawing.Point(106, 19);
            this.comboBoxNameBody.Name = "comboBoxNameBody";
            this.comboBoxNameBody.Size = new System.Drawing.Size(238, 23);
            this.comboBoxNameBody.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "拡張子(&E):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "ファイル名主部(&F):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxSystem);
            this.groupBox2.Controls.Add(this.checkBoxArchive);
            this.groupBox2.Controls.Add(this.checkBoxHidden);
            this.groupBox2.Controls.Add(this.checkBoxReadOnly);
            this.groupBox2.Location = new System.Drawing.Point(382, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(121, 120);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "属性";
            // 
            // checkBoxSystem
            // 
            this.checkBoxSystem.AutoSize = true;
            this.checkBoxSystem.Checked = true;
            this.checkBoxSystem.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxSystem.Location = new System.Drawing.Point(6, 88);
            this.checkBoxSystem.Name = "checkBoxSystem";
            this.checkBoxSystem.Size = new System.Drawing.Size(82, 19);
            this.checkBoxSystem.TabIndex = 3;
            this.checkBoxSystem.Text = "システム(&S)";
            this.checkBoxSystem.ThreeState = true;
            this.checkBoxSystem.UseVisualStyleBackColor = true;
            // 
            // checkBoxArchive
            // 
            this.checkBoxArchive.AutoSize = true;
            this.checkBoxArchive.Checked = true;
            this.checkBoxArchive.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxArchive.Location = new System.Drawing.Point(6, 65);
            this.checkBoxArchive.Name = "checkBoxArchive";
            this.checkBoxArchive.Size = new System.Drawing.Size(90, 19);
            this.checkBoxArchive.TabIndex = 2;
            this.checkBoxArchive.Text = "アーカイブ(&A)";
            this.checkBoxArchive.ThreeState = true;
            this.checkBoxArchive.UseVisualStyleBackColor = true;
            // 
            // checkBoxHidden
            // 
            this.checkBoxHidden.AutoSize = true;
            this.checkBoxHidden.Checked = true;
            this.checkBoxHidden.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxHidden.Location = new System.Drawing.Point(6, 42);
            this.checkBoxHidden.Name = "checkBoxHidden";
            this.checkBoxHidden.Size = new System.Drawing.Size(100, 19);
            this.checkBoxHidden.TabIndex = 1;
            this.checkBoxHidden.Text = "隠しファイル(&H)";
            this.checkBoxHidden.ThreeState = true;
            this.checkBoxHidden.UseVisualStyleBackColor = true;
            // 
            // checkBoxReadOnly
            // 
            this.checkBoxReadOnly.AutoSize = true;
            this.checkBoxReadOnly.Checked = true;
            this.checkBoxReadOnly.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxReadOnly.Location = new System.Drawing.Point(6, 19);
            this.checkBoxReadOnly.Name = "checkBoxReadOnly";
            this.checkBoxReadOnly.Size = new System.Drawing.Size(111, 19);
            this.checkBoxReadOnly.TabIndex = 0;
            this.checkBoxReadOnly.Text = "読み取り専用(&R)";
            this.checkBoxReadOnly.ThreeState = true;
            this.checkBoxReadOnly.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.textBoxTimeAccess);
            this.groupBox3.Controls.Add(this.textBoxTimeCreate);
            this.groupBox3.Controls.Add(this.textBoxTimeUpdate);
            this.groupBox3.Controls.Add(this.buttonTimeAccess);
            this.groupBox3.Controls.Add(this.buttonTimeCreate);
            this.groupBox3.Controls.Add(this.buttonTimeUpdate);
            this.groupBox3.Controls.Add(this.buttonCurrent);
            this.groupBox3.Controls.Add(this.buttonNoon);
            this.groupBox3.Controls.Add(this.dateTimeDateAccess);
            this.groupBox3.Controls.Add(this.checkBoxTimeAccess);
            this.groupBox3.Controls.Add(this.dateTimeDateCreate);
            this.groupBox3.Controls.Add(this.checkBoxTimeCreate);
            this.groupBox3.Controls.Add(this.checkBoxDateAccess);
            this.groupBox3.Controls.Add(this.dateTimeDateUpdate);
            this.groupBox3.Controls.Add(this.checkBoxDateCreate);
            this.groupBox3.Controls.Add(this.checkBoxTimeUpdate);
            this.groupBox3.Controls.Add(this.checkBoxDateUpdate);
            this.groupBox3.Location = new System.Drawing.Point(13, 139);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(509, 135);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "タイムスタンプ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "アクセス日時(&A)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "作成日時(&C)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "更新日時(&U)";
            // 
            // textBoxTimeAccess
            // 
            this.textBoxTimeAccess.Location = new System.Drawing.Point(386, 71);
            this.textBoxTimeAccess.Mask = "90:00:00";
            this.textBoxTimeAccess.Name = "textBoxTimeAccess";
            this.textBoxTimeAccess.Size = new System.Drawing.Size(66, 23);
            this.textBoxTimeAccess.TabIndex = 16;
            this.textBoxTimeAccess.Text = "120000";
            // 
            // textBoxTimeCreate
            // 
            this.textBoxTimeCreate.Location = new System.Drawing.Point(386, 44);
            this.textBoxTimeCreate.Mask = "90:00:00";
            this.textBoxTimeCreate.Name = "textBoxTimeCreate";
            this.textBoxTimeCreate.Size = new System.Drawing.Size(66, 23);
            this.textBoxTimeCreate.TabIndex = 10;
            this.textBoxTimeCreate.Text = "120000";
            // 
            // textBoxTimeUpdate
            // 
            this.textBoxTimeUpdate.Location = new System.Drawing.Point(386, 19);
            this.textBoxTimeUpdate.Mask = "90:00:00";
            this.textBoxTimeUpdate.Name = "textBoxTimeUpdate";
            this.textBoxTimeUpdate.Size = new System.Drawing.Size(66, 23);
            this.textBoxTimeUpdate.TabIndex = 4;
            this.textBoxTimeUpdate.Text = "120000";
            // 
            // buttonTimeAccess
            // 
            this.buttonTimeAccess.Location = new System.Drawing.Point(458, 71);
            this.buttonTimeAccess.Name = "buttonTimeAccess";
            this.buttonTimeAccess.Size = new System.Drawing.Size(30, 23);
            this.buttonTimeAccess.TabIndex = 17;
            this.buttonTimeAccess.UseVisualStyleBackColor = true;
            this.buttonTimeAccess.Click += new System.EventHandler(this.buttonTimeAccess_Click);
            // 
            // buttonTimeCreate
            // 
            this.buttonTimeCreate.Location = new System.Drawing.Point(458, 44);
            this.buttonTimeCreate.Name = "buttonTimeCreate";
            this.buttonTimeCreate.Size = new System.Drawing.Size(30, 23);
            this.buttonTimeCreate.TabIndex = 11;
            this.buttonTimeCreate.UseVisualStyleBackColor = true;
            this.buttonTimeCreate.Click += new System.EventHandler(this.buttonTimeCreate_Click);
            // 
            // buttonTimeUpdate
            // 
            this.buttonTimeUpdate.Location = new System.Drawing.Point(458, 17);
            this.buttonTimeUpdate.Name = "buttonTimeUpdate";
            this.buttonTimeUpdate.Size = new System.Drawing.Size(30, 23);
            this.buttonTimeUpdate.TabIndex = 5;
            this.buttonTimeUpdate.UseVisualStyleBackColor = true;
            this.buttonTimeUpdate.Click += new System.EventHandler(this.buttonTimeUpdate_Click);
            // 
            // buttonCurrent
            // 
            this.buttonCurrent.Location = new System.Drawing.Point(9, 101);
            this.buttonCurrent.Name = "buttonCurrent";
            this.buttonCurrent.Size = new System.Drawing.Size(79, 23);
            this.buttonCurrent.TabIndex = 18;
            this.buttonCurrent.Text = "現在時刻(&D)";
            this.buttonCurrent.UseVisualStyleBackColor = true;
            this.buttonCurrent.Click += new System.EventHandler(this.buttonCurrent_Click);
            // 
            // buttonNoon
            // 
            this.buttonNoon.Location = new System.Drawing.Point(94, 101);
            this.buttonNoon.Name = "buttonNoon";
            this.buttonNoon.Size = new System.Drawing.Size(80, 23);
            this.buttonNoon.TabIndex = 19;
            this.buttonNoon.Text = "本日正午(&O)";
            this.buttonNoon.UseVisualStyleBackColor = true;
            this.buttonNoon.Click += new System.EventHandler(this.buttonNoon_Click);
            // 
            // dateTimeDateAccess
            // 
            this.dateTimeDateAccess.CustomFormat = "yyyy/MM/dd";
            this.dateTimeDateAccess.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeDateAccess.Location = new System.Drawing.Point(183, 71);
            this.dateTimeDateAccess.Name = "dateTimeDateAccess";
            this.dateTimeDateAccess.Size = new System.Drawing.Size(101, 23);
            this.dateTimeDateAccess.TabIndex = 14;
            // 
            // checkBoxTimeAccess
            // 
            this.checkBoxTimeAccess.AutoSize = true;
            this.checkBoxTimeAccess.Location = new System.Drawing.Point(304, 73);
            this.checkBoxTimeAccess.Name = "checkBoxTimeAccess";
            this.checkBoxTimeAccess.Size = new System.Drawing.Size(83, 19);
            this.checkBoxTimeAccess.TabIndex = 15;
            this.checkBoxTimeAccess.Text = "時刻を変更";
            this.checkBoxTimeAccess.UseVisualStyleBackColor = true;
            this.checkBoxTimeAccess.CheckedChanged += new System.EventHandler(this.CheckBoxDateTime_CheckedChanged);
            // 
            // dateTimeDateCreate
            // 
            this.dateTimeDateCreate.CustomFormat = "yyyy/MM/dd";
            this.dateTimeDateCreate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeDateCreate.Location = new System.Drawing.Point(183, 44);
            this.dateTimeDateCreate.Name = "dateTimeDateCreate";
            this.dateTimeDateCreate.Size = new System.Drawing.Size(101, 23);
            this.dateTimeDateCreate.TabIndex = 8;
            // 
            // checkBoxTimeCreate
            // 
            this.checkBoxTimeCreate.AutoSize = true;
            this.checkBoxTimeCreate.Location = new System.Drawing.Point(304, 46);
            this.checkBoxTimeCreate.Name = "checkBoxTimeCreate";
            this.checkBoxTimeCreate.Size = new System.Drawing.Size(83, 19);
            this.checkBoxTimeCreate.TabIndex = 9;
            this.checkBoxTimeCreate.Text = "時刻を変更";
            this.checkBoxTimeCreate.UseVisualStyleBackColor = true;
            this.checkBoxTimeCreate.CheckedChanged += new System.EventHandler(this.CheckBoxDateTime_CheckedChanged);
            // 
            // checkBoxDateAccess
            // 
            this.checkBoxDateAccess.AutoSize = true;
            this.checkBoxDateAccess.Location = new System.Drawing.Point(100, 73);
            this.checkBoxDateAccess.Name = "checkBoxDateAccess";
            this.checkBoxDateAccess.Size = new System.Drawing.Size(83, 19);
            this.checkBoxDateAccess.TabIndex = 13;
            this.checkBoxDateAccess.Text = "日付を変更";
            this.checkBoxDateAccess.UseVisualStyleBackColor = true;
            this.checkBoxDateAccess.CheckedChanged += new System.EventHandler(this.CheckBoxDateTime_CheckedChanged);
            // 
            // dateTimeDateUpdate
            // 
            this.dateTimeDateUpdate.CustomFormat = "yyyy/MM/dd";
            this.dateTimeDateUpdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeDateUpdate.Location = new System.Drawing.Point(183, 19);
            this.dateTimeDateUpdate.Name = "dateTimeDateUpdate";
            this.dateTimeDateUpdate.Size = new System.Drawing.Size(101, 23);
            this.dateTimeDateUpdate.TabIndex = 2;
            // 
            // checkBoxDateCreate
            // 
            this.checkBoxDateCreate.AutoSize = true;
            this.checkBoxDateCreate.Location = new System.Drawing.Point(100, 46);
            this.checkBoxDateCreate.Name = "checkBoxDateCreate";
            this.checkBoxDateCreate.Size = new System.Drawing.Size(83, 19);
            this.checkBoxDateCreate.TabIndex = 7;
            this.checkBoxDateCreate.Text = "日付を変更";
            this.checkBoxDateCreate.UseVisualStyleBackColor = true;
            this.checkBoxDateCreate.CheckedChanged += new System.EventHandler(this.CheckBoxDateTime_CheckedChanged);
            // 
            // checkBoxTimeUpdate
            // 
            this.checkBoxTimeUpdate.AutoSize = true;
            this.checkBoxTimeUpdate.Location = new System.Drawing.Point(304, 21);
            this.checkBoxTimeUpdate.Name = "checkBoxTimeUpdate";
            this.checkBoxTimeUpdate.Size = new System.Drawing.Size(83, 19);
            this.checkBoxTimeUpdate.TabIndex = 3;
            this.checkBoxTimeUpdate.Text = "時刻を変更";
            this.checkBoxTimeUpdate.UseVisualStyleBackColor = true;
            this.checkBoxTimeUpdate.CheckedChanged += new System.EventHandler(this.CheckBoxDateTime_CheckedChanged);
            // 
            // checkBoxDateUpdate
            // 
            this.checkBoxDateUpdate.AutoSize = true;
            this.checkBoxDateUpdate.Location = new System.Drawing.Point(100, 21);
            this.checkBoxDateUpdate.Name = "checkBoxDateUpdate";
            this.checkBoxDateUpdate.Size = new System.Drawing.Size(83, 19);
            this.checkBoxDateUpdate.TabIndex = 1;
            this.checkBoxDateUpdate.Text = "日付を変更";
            this.checkBoxDateUpdate.UseVisualStyleBackColor = true;
            this.checkBoxDateUpdate.CheckedChanged += new System.EventHandler(this.CheckBoxDateTime_CheckedChanged);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(370, 343);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(73, 23);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(449, 343);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(73, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.comboBoxFolder);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Location = new System.Drawing.Point(13, 280);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(509, 56);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "処理対象";
            // 
            // comboBoxFolder
            // 
            this.comboBoxFolder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFolder.FormattingEnabled = true;
            this.comboBoxFolder.Location = new System.Drawing.Point(130, 18);
            this.comboBoxFolder.Name = "comboBoxFolder";
            this.comboBoxFolder.Size = new System.Drawing.Size(214, 23);
            this.comboBoxFolder.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(123, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "フォルダの処理方法(&V):";
            // 
            // RenameSelectedFilesWindowsDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(534, 378);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenameSelectedFilesWindowsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ファイル情報の一括編集";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenameSelectedFilesWindowsDialog_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonNameLower;
        private System.Windows.Forms.Button buttonNameUpper;
        private System.Windows.Forms.ComboBox comboBoxNameExt;
        private System.Windows.Forms.ComboBox comboBoxNameBody;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxDateUpdate;
        private System.Windows.Forms.CheckBox checkBoxSystem;
        private System.Windows.Forms.CheckBox checkBoxArchive;
        private System.Windows.Forms.CheckBox checkBoxHidden;
        private System.Windows.Forms.CheckBox checkBoxReadOnly;
        private System.Windows.Forms.Button buttonTimeAccess;
        private System.Windows.Forms.Button buttonTimeCreate;
        private System.Windows.Forms.Button buttonTimeUpdate;
        private System.Windows.Forms.Button buttonCurrent;
        private System.Windows.Forms.Button buttonNoon;
        private System.Windows.Forms.DateTimePicker dateTimeDateUpdate;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.MaskedTextBox textBoxTimeUpdate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxTimeUpdate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MaskedTextBox textBoxTimeAccess;
        private System.Windows.Forms.MaskedTextBox textBoxTimeCreate;
        private System.Windows.Forms.DateTimePicker dateTimeDateAccess;
        private System.Windows.Forms.CheckBox checkBoxTimeAccess;
        private System.Windows.Forms.DateTimePicker dateTimeDateCreate;
        private System.Windows.Forms.CheckBox checkBoxTimeCreate;
        private System.Windows.Forms.CheckBox checkBoxDateAccess;
        private System.Windows.Forms.CheckBox checkBoxDateCreate;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBoxFolder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonSequential;
    }
}