namespace ShellFiler.UI.Dialog
{
    partial class RenameSSHDialog
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
            this.dateTimeUpdate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxFileNameCurrent = new System.Windows.Forms.TextBox();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.buttonNameCapital = new System.Windows.Forms.Button();
            this.buttonNameLower = new System.Windows.Forms.Button();
            this.buttonNameUpper = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxAttribute = new System.Windows.Forms.TextBox();
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
            this.buttonTimeAccess = new System.Windows.Forms.Button();
            this.buttonTimeUpdate = new System.Windows.Forms.Button();
            this.buttonCurrent = new System.Windows.Forms.Button();
            this.buttonNoon = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimeAccess = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBoxGroup = new System.Windows.Forms.TextBox();
            this.textBoxOwner = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // dateTimeUpdate
            // 
            this.dateTimeUpdate.CustomFormat = "yyyy/MM/dd  HH:mm:ss";
            this.dateTimeUpdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeUpdate.Location = new System.Drawing.Point(102, 19);
            this.dateTimeUpdate.Name = "dateTimeUpdate";
            this.dateTimeUpdate.Size = new System.Drawing.Size(149, 19);
            this.dateTimeUpdate.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "新しい名前(&N):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "現在の名前";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxFileNameCurrent);
            this.groupBox1.Controls.Add(this.textBoxFileName);
            this.groupBox1.Controls.Add(this.buttonNameCapital);
            this.groupBox1.Controls.Add(this.buttonNameLower);
            this.groupBox1.Controls.Add(this.buttonNameUpper);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(481, 104);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ファイル名";
            // 
            // textBoxFileNameCurrent
            // 
            this.textBoxFileNameCurrent.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxFileNameCurrent.Location = new System.Drawing.Point(25, 78);
            this.textBoxFileNameCurrent.Name = "textBoxFileNameCurrent";
            this.textBoxFileNameCurrent.ReadOnly = true;
            this.textBoxFileNameCurrent.Size = new System.Drawing.Size(348, 20);
            this.textBoxFileNameCurrent.TabIndex = 3;
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxFileName.Location = new System.Drawing.Point(25, 34);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(348, 20);
            this.textBoxFileName.TabIndex = 1;
            // 
            // buttonNameCapital
            // 
            this.buttonNameCapital.Location = new System.Drawing.Point(379, 75);
            this.buttonNameCapital.Name = "buttonNameCapital";
            this.buttonNameCapital.Size = new System.Drawing.Size(91, 23);
            this.buttonNameCapital.TabIndex = 6;
            this.buttonNameCapital.Text = "先頭大文字(&T)";
            this.buttonNameCapital.UseVisualStyleBackColor = true;
            this.buttonNameCapital.Click += new System.EventHandler(this.buttonNameCapital_Click);
            // 
            // buttonNameLower
            // 
            this.buttonNameLower.Location = new System.Drawing.Point(379, 49);
            this.buttonNameLower.Name = "buttonNameLower";
            this.buttonNameLower.Size = new System.Drawing.Size(91, 23);
            this.buttonNameLower.TabIndex = 5;
            this.buttonNameLower.Text = "小文字へ(&L)";
            this.buttonNameLower.UseVisualStyleBackColor = true;
            this.buttonNameLower.Click += new System.EventHandler(this.buttonNameLower_Click);
            // 
            // buttonNameUpper
            // 
            this.buttonNameUpper.Location = new System.Drawing.Point(379, 23);
            this.buttonNameUpper.Name = "buttonNameUpper";
            this.buttonNameUpper.Size = new System.Drawing.Size(91, 23);
            this.buttonNameUpper.TabIndex = 4;
            this.buttonNameUpper.Text = "大文字へ(&U)";
            this.buttonNameUpper.UseVisualStyleBackColor = true;
            this.buttonNameUpper.Click += new System.EventHandler(this.buttonNameUpper_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxAttribute);
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
            this.groupBox2.Location = new System.Drawing.Point(13, 125);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(175, 229);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "属性";
            // 
            // textBoxAttribute
            // 
            this.textBoxAttribute.Location = new System.Drawing.Point(60, 16);
            this.textBoxAttribute.Name = "textBoxAttribute";
            this.textBoxAttribute.Size = new System.Drawing.Size(100, 19);
            this.textBoxAttribute.TabIndex = 1;
            this.textBoxAttribute.TextChanged += new System.EventHandler(this.textBoxAttribute_TextChanged);
            this.textBoxAttribute.Leave += new System.EventHandler(this.textBoxAttribute_Leave);
            // 
            // checkBoxGroupExecute
            // 
            this.checkBoxGroupExecute.AutoSize = true;
            this.checkBoxGroupExecute.Location = new System.Drawing.Point(12, 144);
            this.checkBoxGroupExecute.Name = "checkBoxGroupExecute";
            this.checkBoxGroupExecute.Size = new System.Drawing.Size(112, 16);
            this.checkBoxGroupExecute.TabIndex = 7;
            this.checkBoxGroupExecute.Text = "グループ実行可(&3)";
            this.checkBoxGroupExecute.UseVisualStyleBackColor = true;
            this.checkBoxGroupExecute.CheckedChanged += new System.EventHandler(this.checkBoxPermission_CheckedChanged);
            // 
            // checkBoxOtherExecute
            // 
            this.checkBoxOtherExecute.AutoSize = true;
            this.checkBoxOtherExecute.Location = new System.Drawing.Point(12, 204);
            this.checkBoxOtherExecute.Name = "checkBoxOtherExecute";
            this.checkBoxOtherExecute.Size = new System.Drawing.Size(98, 16);
            this.checkBoxOtherExecute.TabIndex = 10;
            this.checkBoxOtherExecute.Text = "他人実行可(&6)";
            this.checkBoxOtherExecute.UseVisualStyleBackColor = true;
            this.checkBoxOtherExecute.CheckedChanged += new System.EventHandler(this.checkBoxPermission_CheckedChanged);
            // 
            // checkBoxGroupWrite
            // 
            this.checkBoxGroupWrite.AutoSize = true;
            this.checkBoxGroupWrite.Location = new System.Drawing.Point(12, 124);
            this.checkBoxGroupWrite.Name = "checkBoxGroupWrite";
            this.checkBoxGroupWrite.Size = new System.Drawing.Size(132, 16);
            this.checkBoxGroupWrite.TabIndex = 6;
            this.checkBoxGroupWrite.Text = "グループ書き込み可(&2)";
            this.checkBoxGroupWrite.UseVisualStyleBackColor = true;
            this.checkBoxGroupWrite.CheckedChanged += new System.EventHandler(this.checkBoxPermission_CheckedChanged);
            // 
            // checkBoxOtherWrite
            // 
            this.checkBoxOtherWrite.AutoSize = true;
            this.checkBoxOtherWrite.Location = new System.Drawing.Point(12, 184);
            this.checkBoxOtherWrite.Name = "checkBoxOtherWrite";
            this.checkBoxOtherWrite.Size = new System.Drawing.Size(118, 16);
            this.checkBoxOtherWrite.TabIndex = 9;
            this.checkBoxOtherWrite.Text = "他人書き込み可(&5)";
            this.checkBoxOtherWrite.UseVisualStyleBackColor = true;
            this.checkBoxOtherWrite.CheckedChanged += new System.EventHandler(this.checkBoxPermission_CheckedChanged);
            // 
            // checkBoxOwnerExecute
            // 
            this.checkBoxOwnerExecute.AutoSize = true;
            this.checkBoxOwnerExecute.Location = new System.Drawing.Point(12, 84);
            this.checkBoxOwnerExecute.Name = "checkBoxOwnerExecute";
            this.checkBoxOwnerExecute.Size = new System.Drawing.Size(111, 16);
            this.checkBoxOwnerExecute.TabIndex = 4;
            this.checkBoxOwnerExecute.Text = "所有者実行可(&X)";
            this.checkBoxOwnerExecute.UseVisualStyleBackColor = true;
            this.checkBoxOwnerExecute.CheckedChanged += new System.EventHandler(this.checkBoxPermission_CheckedChanged);
            // 
            // checkBoxGroupRead
            // 
            this.checkBoxGroupRead.AutoSize = true;
            this.checkBoxGroupRead.Location = new System.Drawing.Point(12, 104);
            this.checkBoxGroupRead.Name = "checkBoxGroupRead";
            this.checkBoxGroupRead.Size = new System.Drawing.Size(134, 16);
            this.checkBoxGroupRead.TabIndex = 5;
            this.checkBoxGroupRead.Text = "グループ読み込み可(&1)";
            this.checkBoxGroupRead.UseVisualStyleBackColor = true;
            this.checkBoxGroupRead.CheckedChanged += new System.EventHandler(this.checkBoxPermission_CheckedChanged);
            // 
            // checkBoxOwnerWrite
            // 
            this.checkBoxOwnerWrite.AutoSize = true;
            this.checkBoxOwnerWrite.Location = new System.Drawing.Point(12, 64);
            this.checkBoxOwnerWrite.Name = "checkBoxOwnerWrite";
            this.checkBoxOwnerWrite.Size = new System.Drawing.Size(133, 16);
            this.checkBoxOwnerWrite.TabIndex = 3;
            this.checkBoxOwnerWrite.Text = "所有者書き込み可(&W)";
            this.checkBoxOwnerWrite.UseVisualStyleBackColor = true;
            this.checkBoxOwnerWrite.CheckedChanged += new System.EventHandler(this.checkBoxPermission_CheckedChanged);
            // 
            // checkBoxOtherRead
            // 
            this.checkBoxOtherRead.AutoSize = true;
            this.checkBoxOtherRead.Location = new System.Drawing.Point(12, 164);
            this.checkBoxOtherRead.Name = "checkBoxOtherRead";
            this.checkBoxOtherRead.Size = new System.Drawing.Size(120, 16);
            this.checkBoxOtherRead.TabIndex = 8;
            this.checkBoxOtherRead.Text = "他人読み込み可(&4)";
            this.checkBoxOtherRead.UseVisualStyleBackColor = true;
            this.checkBoxOtherRead.CheckedChanged += new System.EventHandler(this.checkBoxPermission_CheckedChanged);
            // 
            // checkBoxOwnerRead
            // 
            this.checkBoxOwnerRead.AutoSize = true;
            this.checkBoxOwnerRead.Location = new System.Drawing.Point(12, 44);
            this.checkBoxOwnerRead.Name = "checkBoxOwnerRead";
            this.checkBoxOwnerRead.Size = new System.Drawing.Size(134, 16);
            this.checkBoxOwnerRead.TabIndex = 2;
            this.checkBoxOwnerRead.Text = "所有者読み込み可(&R)";
            this.checkBoxOwnerRead.UseVisualStyleBackColor = true;
            this.checkBoxOwnerRead.CheckedChanged += new System.EventHandler(this.checkBoxPermission_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "直接(&D):";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonTimeAccess);
            this.groupBox3.Controls.Add(this.buttonTimeUpdate);
            this.groupBox3.Controls.Add(this.buttonCurrent);
            this.groupBox3.Controls.Add(this.buttonNoon);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.dateTimeAccess);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.dateTimeUpdate);
            this.groupBox3.Location = new System.Drawing.Point(196, 125);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(298, 115);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "タイムスタンプ";
            // 
            // buttonTimeAccess
            // 
            this.buttonTimeAccess.Location = new System.Drawing.Point(257, 40);
            this.buttonTimeAccess.Name = "buttonTimeAccess";
            this.buttonTimeAccess.Size = new System.Drawing.Size(30, 23);
            this.buttonTimeAccess.TabIndex = 5;
            this.buttonTimeAccess.UseVisualStyleBackColor = true;
            this.buttonTimeAccess.Click += new System.EventHandler(this.buttonTimeAccess_Click);
            // 
            // buttonTimeUpdate
            // 
            this.buttonTimeUpdate.Location = new System.Drawing.Point(257, 16);
            this.buttonTimeUpdate.Name = "buttonTimeUpdate";
            this.buttonTimeUpdate.Size = new System.Drawing.Size(30, 23);
            this.buttonTimeUpdate.TabIndex = 2;
            this.buttonTimeUpdate.UseVisualStyleBackColor = true;
            this.buttonTimeUpdate.Click += new System.EventHandler(this.buttonTimeUpdate_Click);
            // 
            // buttonCurrent
            // 
            this.buttonCurrent.Location = new System.Drawing.Point(20, 77);
            this.buttonCurrent.Name = "buttonCurrent";
            this.buttonCurrent.Size = new System.Drawing.Size(79, 23);
            this.buttonCurrent.TabIndex = 6;
            this.buttonCurrent.Text = "現在時刻(&D)";
            this.buttonCurrent.UseVisualStyleBackColor = true;
            this.buttonCurrent.Click += new System.EventHandler(this.buttonCurrent_Click);
            // 
            // buttonNoon
            // 
            this.buttonNoon.Location = new System.Drawing.Point(105, 77);
            this.buttonNoon.Name = "buttonNoon";
            this.buttonNoon.Size = new System.Drawing.Size(80, 23);
            this.buttonNoon.TabIndex = 7;
            this.buttonNoon.Text = "本日正午(&O)";
            this.buttonNoon.UseVisualStyleBackColor = true;
            this.buttonNoon.Click += new System.EventHandler(this.buttonNoon_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "アクセス日時(&A):";
            // 
            // dateTimeAccess
            // 
            this.dateTimeAccess.CustomFormat = "yyyy/MM/dd  HH:mm:ss";
            this.dateTimeAccess.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeAccess.Location = new System.Drawing.Point(102, 43);
            this.dateTimeAccess.Name = "dateTimeAccess";
            this.dateTimeAccess.Size = new System.Drawing.Size(149, 19);
            this.dateTimeAccess.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "更新日時(&M):";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(419, 359);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(338, 359);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBoxGroup);
            this.groupBox4.Controls.Add(this.textBoxOwner);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Location = new System.Drawing.Point(197, 247);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(297, 106);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "オーナー";
            // 
            // textBoxGroup
            // 
            this.textBoxGroup.Location = new System.Drawing.Point(26, 76);
            this.textBoxGroup.Name = "textBoxGroup";
            this.textBoxGroup.Size = new System.Drawing.Size(260, 19);
            this.textBoxGroup.TabIndex = 2;
            // 
            // textBoxOwner
            // 
            this.textBoxOwner.Location = new System.Drawing.Point(26, 36);
            this.textBoxOwner.Name = "textBoxOwner";
            this.textBoxOwner.Size = new System.Drawing.Size(260, 19);
            this.textBoxOwner.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "所有グループ(&G):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "所有ユーザー(&E):";
            // 
            // RenameSSHDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(506, 392);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenameSSHDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "名前の変更";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenameSSHDialog_FormClosing);
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

        private System.Windows.Forms.DateTimePicker dateTimeUpdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxOwnerExecute;
        private System.Windows.Forms.CheckBox checkBoxOwnerWrite;
        private System.Windows.Forms.CheckBox checkBoxOwnerRead;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonNameLower;
        private System.Windows.Forms.Button buttonNameUpper;
        private System.Windows.Forms.Button buttonNameCapital;
        private System.Windows.Forms.Button buttonCurrent;
        private System.Windows.Forms.Button buttonNoon;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.TextBox textBoxFileNameCurrent;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.TextBox textBoxAttribute;
        private System.Windows.Forms.CheckBox checkBoxOtherExecute;
        private System.Windows.Forms.CheckBox checkBoxOtherWrite;
        private System.Windows.Forms.CheckBox checkBoxOtherRead;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBoxGroupRead;
        private System.Windows.Forms.CheckBox checkBoxGroupWrite;
        private System.Windows.Forms.CheckBox checkBoxGroupExecute;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBoxGroup;
        private System.Windows.Forms.TextBox textBoxOwner;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonTimeAccess;
        private System.Windows.Forms.Button buttonTimeUpdate;
        private System.Windows.Forms.DateTimePicker dateTimeAccess;
    }
}