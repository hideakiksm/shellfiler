namespace ShellFiler.UI.Dialog {
    partial class RenameSelectedFilesSSHDialog {
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
            this.textBoxAttirbute = new System.Windows.Forms.MaskedTextBox();
            this.checkBoxGroupExecute = new System.Windows.Forms.CheckBox();
            this.checkBoxOtherExecute = new System.Windows.Forms.CheckBox();
            this.checkBoxGroupWrite = new System.Windows.Forms.CheckBox();
            this.checkBoxOtherWrite = new System.Windows.Forms.CheckBox();
            this.checkBoxOwnerExecute = new System.Windows.Forms.CheckBox();
            this.checkBoxGroupRead = new System.Windows.Forms.CheckBox();
            this.checkBoxOwnerWrite = new System.Windows.Forms.CheckBox();
            this.checkBoxOtherRead = new System.Windows.Forms.CheckBox();
            this.checkBoxOwnerRead = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTimeUpdate = new System.Windows.Forms.MaskedTextBox();
            this.buttonCurrent = new System.Windows.Forms.Button();
            this.buttonNoon = new System.Windows.Forms.Button();
            this.dateTimeDateUpdate = new System.Windows.Forms.DateTimePicker();
            this.checkBoxTimeUpdate = new System.Windows.Forms.CheckBox();
            this.checkBoxDateUpdate = new System.Windows.Forms.CheckBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.comboBoxFolder = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxGroup = new System.Windows.Forms.CheckBox();
            this.checkBoxOwner = new System.Windows.Forms.CheckBox();
            this.textBoxGroup = new System.Windows.Forms.TextBox();
            this.textBoxOwner = new System.Windows.Forms.TextBox();
            this.checkBoxDateAccess = new System.Windows.Forms.CheckBox();
            this.checkBoxTimeAccess = new System.Windows.Forms.CheckBox();
            this.dateTimeDateAccess = new System.Windows.Forms.DateTimePicker();
            this.textBoxTimeAccess = new System.Windows.Forms.MaskedTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
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
            this.groupBox1.Size = new System.Drawing.Size(473, 102);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ファイル名";
            // 
            // buttonSequential
            // 
            this.buttonSequential.Location = new System.Drawing.Point(168, 71);
            this.buttonSequential.Name = "buttonSequential";
            this.buttonSequential.Size = new System.Drawing.Size(75, 23);
            this.buttonSequential.TabIndex = 6;
            this.buttonSequential.Text = "連番(&N)...";
            this.buttonSequential.UseVisualStyleBackColor = true;
            this.buttonSequential.Click += new System.EventHandler(this.buttonSequential_Click);
            // 
            // buttonNameLower
            // 
            this.buttonNameLower.Location = new System.Drawing.Point(87, 71);
            this.buttonNameLower.Name = "buttonNameLower";
            this.buttonNameLower.Size = new System.Drawing.Size(75, 23);
            this.buttonNameLower.TabIndex = 5;
            this.buttonNameLower.Text = "小文字(&L)";
            this.buttonNameLower.UseVisualStyleBackColor = true;
            this.buttonNameLower.Click += new System.EventHandler(this.buttonNameLower_Click);
            // 
            // buttonNameUpper
            // 
            this.buttonNameUpper.Location = new System.Drawing.Point(6, 71);
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
            this.comboBoxNameExt.Size = new System.Drawing.Size(238, 20);
            this.comboBoxNameExt.TabIndex = 3;
            // 
            // comboBoxNameBody
            // 
            this.comboBoxNameBody.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNameBody.FormattingEnabled = true;
            this.comboBoxNameBody.Location = new System.Drawing.Point(106, 19);
            this.comboBoxNameBody.Name = "comboBoxNameBody";
            this.comboBoxNameBody.Size = new System.Drawing.Size(238, 20);
            this.comboBoxNameBody.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "拡張子(&E):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "ファイル名主部(&F):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxAttirbute);
            this.groupBox2.Controls.Add(this.checkBoxGroupExecute);
            this.groupBox2.Controls.Add(this.checkBoxOtherExecute);
            this.groupBox2.Controls.Add(this.checkBoxGroupWrite);
            this.groupBox2.Controls.Add(this.checkBoxOtherWrite);
            this.groupBox2.Controls.Add(this.checkBoxOwnerExecute);
            this.groupBox2.Controls.Add(this.checkBoxGroupRead);
            this.groupBox2.Controls.Add(this.checkBoxOwnerWrite);
            this.groupBox2.Controls.Add(this.checkBoxOtherRead);
            this.groupBox2.Controls.Add(this.checkBoxOwnerRead);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(12, 121);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(169, 228);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "属性";
            // 
            // textBoxAttirbute
            // 
            this.textBoxAttirbute.Location = new System.Drawing.Point(59, 18);
            this.textBoxAttirbute.Mask = "000";
            this.textBoxAttirbute.Name = "textBoxAttirbute";
            this.textBoxAttirbute.Size = new System.Drawing.Size(100, 19);
            this.textBoxAttirbute.TabIndex = 11;
            this.textBoxAttirbute.Leave += new System.EventHandler(this.textBoxAttirbute_Leave);
            this.textBoxAttirbute.TextChanged += new System.EventHandler(this.textBoxAttirbute_TextChanged);
            // 
            // checkBoxGroupExecute
            // 
            this.checkBoxGroupExecute.AutoSize = true;
            this.checkBoxGroupExecute.Checked = true;
            this.checkBoxGroupExecute.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxGroupExecute.Location = new System.Drawing.Point(11, 144);
            this.checkBoxGroupExecute.Name = "checkBoxGroupExecute";
            this.checkBoxGroupExecute.Size = new System.Drawing.Size(112, 16);
            this.checkBoxGroupExecute.TabIndex = 7;
            this.checkBoxGroupExecute.Text = "グループ実行可(&3)";
            this.checkBoxGroupExecute.ThreeState = true;
            this.checkBoxGroupExecute.UseVisualStyleBackColor = true;
            this.checkBoxGroupExecute.CheckStateChanged += new System.EventHandler(this.checkBoxAttribute_CheckedChanged);
            // 
            // checkBoxOtherExecute
            // 
            this.checkBoxOtherExecute.AutoSize = true;
            this.checkBoxOtherExecute.Checked = true;
            this.checkBoxOtherExecute.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxOtherExecute.Location = new System.Drawing.Point(11, 204);
            this.checkBoxOtherExecute.Name = "checkBoxOtherExecute";
            this.checkBoxOtherExecute.Size = new System.Drawing.Size(98, 16);
            this.checkBoxOtherExecute.TabIndex = 10;
            this.checkBoxOtherExecute.Text = "他人実行可(&6)";
            this.checkBoxOtherExecute.ThreeState = true;
            this.checkBoxOtherExecute.UseVisualStyleBackColor = true;
            this.checkBoxOtherExecute.CheckStateChanged += new System.EventHandler(this.checkBoxAttribute_CheckedChanged);
            // 
            // checkBoxGroupWrite
            // 
            this.checkBoxGroupWrite.AutoSize = true;
            this.checkBoxGroupWrite.Checked = true;
            this.checkBoxGroupWrite.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxGroupWrite.Location = new System.Drawing.Point(11, 124);
            this.checkBoxGroupWrite.Name = "checkBoxGroupWrite";
            this.checkBoxGroupWrite.Size = new System.Drawing.Size(132, 16);
            this.checkBoxGroupWrite.TabIndex = 6;
            this.checkBoxGroupWrite.Text = "グループ書き込み可(&2)";
            this.checkBoxGroupWrite.ThreeState = true;
            this.checkBoxGroupWrite.UseVisualStyleBackColor = true;
            this.checkBoxGroupWrite.CheckStateChanged += new System.EventHandler(this.checkBoxAttribute_CheckedChanged);
            // 
            // checkBoxOtherWrite
            // 
            this.checkBoxOtherWrite.AutoSize = true;
            this.checkBoxOtherWrite.Checked = true;
            this.checkBoxOtherWrite.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxOtherWrite.Location = new System.Drawing.Point(11, 184);
            this.checkBoxOtherWrite.Name = "checkBoxOtherWrite";
            this.checkBoxOtherWrite.Size = new System.Drawing.Size(118, 16);
            this.checkBoxOtherWrite.TabIndex = 9;
            this.checkBoxOtherWrite.Text = "他人書き込み可(&5)";
            this.checkBoxOtherWrite.ThreeState = true;
            this.checkBoxOtherWrite.UseVisualStyleBackColor = true;
            this.checkBoxOtherWrite.CheckStateChanged += new System.EventHandler(this.checkBoxAttribute_CheckedChanged);
            // 
            // checkBoxOwnerExecute
            // 
            this.checkBoxOwnerExecute.AutoSize = true;
            this.checkBoxOwnerExecute.Checked = true;
            this.checkBoxOwnerExecute.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxOwnerExecute.Location = new System.Drawing.Point(11, 84);
            this.checkBoxOwnerExecute.Name = "checkBoxOwnerExecute";
            this.checkBoxOwnerExecute.Size = new System.Drawing.Size(111, 16);
            this.checkBoxOwnerExecute.TabIndex = 4;
            this.checkBoxOwnerExecute.Text = "所有者実行可(&X)";
            this.checkBoxOwnerExecute.ThreeState = true;
            this.checkBoxOwnerExecute.UseVisualStyleBackColor = true;
            this.checkBoxOwnerExecute.CheckStateChanged += new System.EventHandler(this.checkBoxAttribute_CheckedChanged);
            // 
            // checkBoxGroupRead
            // 
            this.checkBoxGroupRead.AutoSize = true;
            this.checkBoxGroupRead.Checked = true;
            this.checkBoxGroupRead.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxGroupRead.Location = new System.Drawing.Point(11, 104);
            this.checkBoxGroupRead.Name = "checkBoxGroupRead";
            this.checkBoxGroupRead.Size = new System.Drawing.Size(134, 16);
            this.checkBoxGroupRead.TabIndex = 5;
            this.checkBoxGroupRead.Text = "グループ読み込み可(&1)";
            this.checkBoxGroupRead.ThreeState = true;
            this.checkBoxGroupRead.UseVisualStyleBackColor = true;
            this.checkBoxGroupRead.CheckStateChanged += new System.EventHandler(this.checkBoxAttribute_CheckedChanged);
            // 
            // checkBoxOwnerWrite
            // 
            this.checkBoxOwnerWrite.AutoSize = true;
            this.checkBoxOwnerWrite.Checked = true;
            this.checkBoxOwnerWrite.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxOwnerWrite.Location = new System.Drawing.Point(11, 64);
            this.checkBoxOwnerWrite.Name = "checkBoxOwnerWrite";
            this.checkBoxOwnerWrite.Size = new System.Drawing.Size(133, 16);
            this.checkBoxOwnerWrite.TabIndex = 3;
            this.checkBoxOwnerWrite.Text = "所有者書き込み可(&W)";
            this.checkBoxOwnerWrite.ThreeState = true;
            this.checkBoxOwnerWrite.UseVisualStyleBackColor = true;
            this.checkBoxOwnerWrite.CheckStateChanged += new System.EventHandler(this.checkBoxAttribute_CheckedChanged);
            // 
            // checkBoxOtherRead
            // 
            this.checkBoxOtherRead.AutoSize = true;
            this.checkBoxOtherRead.Checked = true;
            this.checkBoxOtherRead.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxOtherRead.Location = new System.Drawing.Point(11, 164);
            this.checkBoxOtherRead.Name = "checkBoxOtherRead";
            this.checkBoxOtherRead.Size = new System.Drawing.Size(120, 16);
            this.checkBoxOtherRead.TabIndex = 8;
            this.checkBoxOtherRead.Text = "他人読み込み可(&4)";
            this.checkBoxOtherRead.ThreeState = true;
            this.checkBoxOtherRead.UseVisualStyleBackColor = true;
            this.checkBoxOtherRead.CheckStateChanged += new System.EventHandler(this.checkBoxAttribute_CheckedChanged);
            // 
            // checkBoxOwnerRead
            // 
            this.checkBoxOwnerRead.AutoSize = true;
            this.checkBoxOwnerRead.Checked = true;
            this.checkBoxOwnerRead.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxOwnerRead.Location = new System.Drawing.Point(11, 44);
            this.checkBoxOwnerRead.Name = "checkBoxOwnerRead";
            this.checkBoxOwnerRead.Size = new System.Drawing.Size(134, 16);
            this.checkBoxOwnerRead.TabIndex = 2;
            this.checkBoxOwnerRead.Text = "所有者読み込み可(&R)";
            this.checkBoxOwnerRead.ThreeState = true;
            this.checkBoxOwnerRead.UseVisualStyleBackColor = true;
            this.checkBoxOwnerRead.CheckStateChanged += new System.EventHandler(this.checkBoxAttribute_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "直接(&D):";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.textBoxTimeAccess);
            this.groupBox3.Controls.Add(this.textBoxTimeUpdate);
            this.groupBox3.Controls.Add(this.buttonCurrent);
            this.groupBox3.Controls.Add(this.buttonNoon);
            this.groupBox3.Controls.Add(this.dateTimeDateAccess);
            this.groupBox3.Controls.Add(this.checkBoxTimeAccess);
            this.groupBox3.Controls.Add(this.dateTimeDateUpdate);
            this.groupBox3.Controls.Add(this.checkBoxDateAccess);
            this.groupBox3.Controls.Add(this.checkBoxTimeUpdate);
            this.groupBox3.Controls.Add(this.checkBoxDateUpdate);
            this.groupBox3.Location = new System.Drawing.Point(187, 127);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(299, 146);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "タイムスタンプ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "更新日時(&U)";
            // 
            // textBoxTimeUpdate
            // 
            this.textBoxTimeUpdate.Location = new System.Drawing.Point(183, 44);
            this.textBoxTimeUpdate.Mask = "90:00:00";
            this.textBoxTimeUpdate.Name = "textBoxTimeUpdate";
            this.textBoxTimeUpdate.Size = new System.Drawing.Size(101, 19);
            this.textBoxTimeUpdate.TabIndex = 4;
            this.textBoxTimeUpdate.Text = "120000";
            // 
            // buttonCurrent
            // 
            this.buttonCurrent.Location = new System.Drawing.Point(5, 117);
            this.buttonCurrent.Name = "buttonCurrent";
            this.buttonCurrent.Size = new System.Drawing.Size(79, 23);
            this.buttonCurrent.TabIndex = 10;
            this.buttonCurrent.Text = "現在時刻(&D)";
            this.buttonCurrent.UseVisualStyleBackColor = true;
            this.buttonCurrent.Click += new System.EventHandler(this.buttonCurrent_Click);
            // 
            // buttonNoon
            // 
            this.buttonNoon.Location = new System.Drawing.Point(90, 117);
            this.buttonNoon.Name = "buttonNoon";
            this.buttonNoon.Size = new System.Drawing.Size(80, 23);
            this.buttonNoon.TabIndex = 11;
            this.buttonNoon.Text = "本日正午(&O)";
            this.buttonNoon.UseVisualStyleBackColor = true;
            this.buttonNoon.Click += new System.EventHandler(this.buttonNoon_Click);
            // 
            // dateTimeDateUpdate
            // 
            this.dateTimeDateUpdate.CustomFormat = "yyyy/MM/dd";
            this.dateTimeDateUpdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeDateUpdate.Location = new System.Drawing.Point(183, 19);
            this.dateTimeDateUpdate.Name = "dateTimeDateUpdate";
            this.dateTimeDateUpdate.Size = new System.Drawing.Size(101, 19);
            this.dateTimeDateUpdate.TabIndex = 2;
            // 
            // checkBoxTimeUpdate
            // 
            this.checkBoxTimeUpdate.AutoSize = true;
            this.checkBoxTimeUpdate.Location = new System.Drawing.Point(100, 46);
            this.checkBoxTimeUpdate.Name = "checkBoxTimeUpdate";
            this.checkBoxTimeUpdate.Size = new System.Drawing.Size(81, 16);
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
            this.checkBoxDateUpdate.Size = new System.Drawing.Size(81, 16);
            this.checkBoxDateUpdate.TabIndex = 1;
            this.checkBoxDateUpdate.Text = "日付を変更";
            this.checkBoxDateUpdate.UseVisualStyleBackColor = true;
            this.checkBoxDateUpdate.CheckedChanged += new System.EventHandler(this.CheckBoxDateTime_CheckedChanged);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(332, 408);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(73, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(413, 408);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(73, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.comboBoxFolder);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Location = new System.Drawing.Point(12, 355);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(474, 47);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "処理対象";
            // 
            // comboBoxFolder
            // 
            this.comboBoxFolder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFolder.FormattingEnabled = true;
            this.comboBoxFolder.Location = new System.Drawing.Point(130, 18);
            this.comboBoxFolder.Name = "comboBoxFolder";
            this.comboBoxFolder.Size = new System.Drawing.Size(214, 20);
            this.comboBoxFolder.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "フォルダの処理方法(&V):";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBoxGroup);
            this.groupBox5.Controls.Add(this.checkBoxOwner);
            this.groupBox5.Controls.Add(this.textBoxGroup);
            this.groupBox5.Controls.Add(this.textBoxOwner);
            this.groupBox5.Location = new System.Drawing.Point(187, 279);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(299, 70);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "オーナー";
            // 
            // checkBoxGroup
            // 
            this.checkBoxGroup.AutoSize = true;
            this.checkBoxGroup.Location = new System.Drawing.Point(6, 45);
            this.checkBoxGroup.Name = "checkBoxGroup";
            this.checkBoxGroup.Size = new System.Drawing.Size(102, 16);
            this.checkBoxGroup.TabIndex = 2;
            this.checkBoxGroup.Text = "所有グループ(&G)";
            this.checkBoxGroup.UseVisualStyleBackColor = true;
            this.checkBoxGroup.CheckedChanged += new System.EventHandler(this.checkBoxOwner_CheckedChanged);
            // 
            // checkBoxOwner
            // 
            this.checkBoxOwner.AutoSize = true;
            this.checkBoxOwner.Location = new System.Drawing.Point(6, 20);
            this.checkBoxOwner.Name = "checkBoxOwner";
            this.checkBoxOwner.Size = new System.Drawing.Size(103, 16);
            this.checkBoxOwner.TabIndex = 0;
            this.checkBoxOwner.Text = "所有ユーザー(&E)";
            this.checkBoxOwner.UseVisualStyleBackColor = true;
            this.checkBoxOwner.CheckedChanged += new System.EventHandler(this.checkBoxOwner_CheckedChanged);
            // 
            // textBoxGroup
            // 
            this.textBoxGroup.Location = new System.Drawing.Point(123, 43);
            this.textBoxGroup.Name = "textBoxGroup";
            this.textBoxGroup.Size = new System.Drawing.Size(161, 19);
            this.textBoxGroup.TabIndex = 3;
            // 
            // textBoxOwner
            // 
            this.textBoxOwner.Location = new System.Drawing.Point(123, 18);
            this.textBoxOwner.Name = "textBoxOwner";
            this.textBoxOwner.Size = new System.Drawing.Size(161, 19);
            this.textBoxOwner.TabIndex = 1;
            // 
            // checkBoxDateAccess
            // 
            this.checkBoxDateAccess.AutoSize = true;
            this.checkBoxDateAccess.Location = new System.Drawing.Point(100, 71);
            this.checkBoxDateAccess.Name = "checkBoxDateAccess";
            this.checkBoxDateAccess.Size = new System.Drawing.Size(81, 16);
            this.checkBoxDateAccess.TabIndex = 6;
            this.checkBoxDateAccess.Text = "日付を変更";
            this.checkBoxDateAccess.UseVisualStyleBackColor = true;
            this.checkBoxDateAccess.CheckedChanged += new System.EventHandler(this.CheckBoxDateTime_CheckedChanged);
            // 
            // checkBoxTimeAccess
            // 
            this.checkBoxTimeAccess.AutoSize = true;
            this.checkBoxTimeAccess.Location = new System.Drawing.Point(100, 96);
            this.checkBoxTimeAccess.Name = "checkBoxTimeAccess";
            this.checkBoxTimeAccess.Size = new System.Drawing.Size(81, 16);
            this.checkBoxTimeAccess.TabIndex = 8;
            this.checkBoxTimeAccess.Text = "時刻を変更";
            this.checkBoxTimeAccess.UseVisualStyleBackColor = true;
            this.checkBoxTimeAccess.CheckedChanged += new System.EventHandler(this.CheckBoxDateTime_CheckedChanged);
            // 
            // dateTimeDateAccess
            // 
            this.dateTimeDateAccess.CustomFormat = "yyyy/MM/dd";
            this.dateTimeDateAccess.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeDateAccess.Location = new System.Drawing.Point(183, 69);
            this.dateTimeDateAccess.Name = "dateTimeDateAccess";
            this.dateTimeDateAccess.Size = new System.Drawing.Size(101, 19);
            this.dateTimeDateAccess.TabIndex = 7;
            // 
            // textBoxTimeAccess
            // 
            this.textBoxTimeAccess.Location = new System.Drawing.Point(183, 94);
            this.textBoxTimeAccess.Mask = "90:00:00";
            this.textBoxTimeAccess.Name = "textBoxTimeAccess";
            this.textBoxTimeAccess.Size = new System.Drawing.Size(101, 19);
            this.textBoxTimeAccess.TabIndex = 9;
            this.textBoxTimeAccess.Text = "120000";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "アクセス日時(&A)";
            // 
            // RenameSelectedFilesSSHDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(501, 443);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenameSelectedFilesSSHDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ファイル情報の一括編集";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenameSelectedFilesSSHDialog_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
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
        private System.Windows.Forms.Button buttonCurrent;
        private System.Windows.Forms.Button buttonNoon;
        private System.Windows.Forms.DateTimePicker dateTimeDateUpdate;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.MaskedTextBox textBoxTimeUpdate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxTimeUpdate;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBoxFolder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonSequential;
        private System.Windows.Forms.CheckBox checkBoxGroupExecute;
        private System.Windows.Forms.CheckBox checkBoxOtherExecute;
        private System.Windows.Forms.CheckBox checkBoxGroupWrite;
        private System.Windows.Forms.CheckBox checkBoxOtherWrite;
        private System.Windows.Forms.CheckBox checkBoxOwnerExecute;
        private System.Windows.Forms.CheckBox checkBoxGroupRead;
        private System.Windows.Forms.CheckBox checkBoxOwnerWrite;
        private System.Windows.Forms.CheckBox checkBoxOtherRead;
        private System.Windows.Forms.CheckBox checkBoxOwnerRead;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBoxGroup;
        private System.Windows.Forms.TextBox textBoxOwner;
        private System.Windows.Forms.CheckBox checkBoxGroup;
        private System.Windows.Forms.CheckBox checkBoxOwner;
        private System.Windows.Forms.MaskedTextBox textBoxAttirbute;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MaskedTextBox textBoxTimeAccess;
        private System.Windows.Forms.DateTimePicker dateTimeDateAccess;
        private System.Windows.Forms.CheckBox checkBoxTimeAccess;
        private System.Windows.Forms.CheckBox checkBoxDateAccess;
    }
}