namespace ShellFiler.UI.Dialog
{
    partial class RenameWindowsDialog
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
            this.dateTimeCreate = new System.Windows.Forms.DateTimePicker();
            this.dateTimeAccess = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxFileNameCurrent = new System.Windows.Forms.TextBox();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.buttonNameCapital = new System.Windows.Forms.Button();
            this.buttonNameLower = new System.Windows.Forms.Button();
            this.buttonNameUpper = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.linkLabelEtcAttr = new System.Windows.Forms.LinkLabel();
            this.checkBoxSystem = new System.Windows.Forms.CheckBox();
            this.checkBoxArchive = new System.Windows.Forms.CheckBox();
            this.checkBoxHidden = new System.Windows.Forms.CheckBox();
            this.checkBoxReadOnly = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonTimeAccess = new System.Windows.Forms.Button();
            this.buttonTimeCreate = new System.Windows.Forms.Button();
            this.buttonTimeUpdate = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonCurrent = new System.Windows.Forms.Button();
            this.buttonNoon = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            // dateTimeCreate
            // 
            this.dateTimeCreate.CustomFormat = "yyyy/MM/dd  HH:mm:ss";
            this.dateTimeCreate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeCreate.Location = new System.Drawing.Point(102, 44);
            this.dateTimeCreate.Name = "dateTimeCreate";
            this.dateTimeCreate.Size = new System.Drawing.Size(149, 19);
            this.dateTimeCreate.TabIndex = 4;
            // 
            // dateTimeAccess
            // 
            this.dateTimeAccess.CustomFormat = "yyyy/MM/dd  HH:mm:ss";
            this.dateTimeAccess.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeAccess.Location = new System.Drawing.Point(102, 69);
            this.dateTimeAccess.Name = "dateTimeAccess";
            this.dateTimeAccess.Size = new System.Drawing.Size(149, 19);
            this.dateTimeAccess.TabIndex = 7;
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
            this.label2.Location = new System.Drawing.Point(10, 63);
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
            this.groupBox1.Size = new System.Drawing.Size(434, 105);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ファイル名";
            // 
            // textBoxFileNameCurrent
            // 
            this.textBoxFileNameCurrent.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxFileNameCurrent.Location = new System.Drawing.Point(25, 79);
            this.textBoxFileNameCurrent.Name = "textBoxFileNameCurrent";
            this.textBoxFileNameCurrent.ReadOnly = true;
            this.textBoxFileNameCurrent.Size = new System.Drawing.Size(306, 20);
            this.textBoxFileNameCurrent.TabIndex = 3;
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxFileName.Location = new System.Drawing.Point(25, 34);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(306, 20);
            this.textBoxFileName.TabIndex = 1;
            // 
            // buttonNameCapital
            // 
            this.buttonNameCapital.Location = new System.Drawing.Point(337, 76);
            this.buttonNameCapital.Name = "buttonNameCapital";
            this.buttonNameCapital.Size = new System.Drawing.Size(91, 23);
            this.buttonNameCapital.TabIndex = 6;
            this.buttonNameCapital.Text = "先頭大文字(&T)";
            this.buttonNameCapital.UseVisualStyleBackColor = true;
            this.buttonNameCapital.Click += new System.EventHandler(this.buttonNameCapital_Click);
            // 
            // buttonNameLower
            // 
            this.buttonNameLower.Location = new System.Drawing.Point(337, 50);
            this.buttonNameLower.Name = "buttonNameLower";
            this.buttonNameLower.Size = new System.Drawing.Size(91, 23);
            this.buttonNameLower.TabIndex = 5;
            this.buttonNameLower.Text = "小文字へ(&L)";
            this.buttonNameLower.UseVisualStyleBackColor = true;
            this.buttonNameLower.Click += new System.EventHandler(this.buttonNameLower_Click);
            // 
            // buttonNameUpper
            // 
            this.buttonNameUpper.Location = new System.Drawing.Point(337, 24);
            this.buttonNameUpper.Name = "buttonNameUpper";
            this.buttonNameUpper.Size = new System.Drawing.Size(91, 23);
            this.buttonNameUpper.TabIndex = 4;
            this.buttonNameUpper.Text = "大文字へ(&U)";
            this.buttonNameUpper.UseVisualStyleBackColor = true;
            this.buttonNameUpper.Click += new System.EventHandler(this.buttonNameUpper_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.linkLabelEtcAttr);
            this.groupBox2.Controls.Add(this.checkBoxSystem);
            this.groupBox2.Controls.Add(this.checkBoxArchive);
            this.groupBox2.Controls.Add(this.checkBoxHidden);
            this.groupBox2.Controls.Add(this.checkBoxReadOnly);
            this.groupBox2.Location = new System.Drawing.Point(13, 128);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(125, 136);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "属性";
            // 
            // linkLabelEtcAttr
            // 
            this.linkLabelEtcAttr.AutoSize = true;
            this.linkLabelEtcAttr.Location = new System.Drawing.Point(12, 108);
            this.linkLabelEtcAttr.Name = "linkLabelEtcAttr";
            this.linkLabelEtcAttr.Size = new System.Drawing.Size(69, 12);
            this.linkLabelEtcAttr.TabIndex = 4;
            this.linkLabelEtcAttr.TabStop = true;
            this.linkLabelEtcAttr.Text = "詳細の表示...";
            this.linkLabelEtcAttr.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEtcAttr_LinkClicked);
            // 
            // checkBoxSystem
            // 
            this.checkBoxSystem.AutoSize = true;
            this.checkBoxSystem.Location = new System.Drawing.Point(12, 85);
            this.checkBoxSystem.Name = "checkBoxSystem";
            this.checkBoxSystem.Size = new System.Drawing.Size(77, 16);
            this.checkBoxSystem.TabIndex = 3;
            this.checkBoxSystem.Text = "システム(&S)";
            this.checkBoxSystem.UseVisualStyleBackColor = true;
            // 
            // checkBoxArchive
            // 
            this.checkBoxArchive.AutoSize = true;
            this.checkBoxArchive.Location = new System.Drawing.Point(12, 63);
            this.checkBoxArchive.Name = "checkBoxArchive";
            this.checkBoxArchive.Size = new System.Drawing.Size(86, 16);
            this.checkBoxArchive.TabIndex = 2;
            this.checkBoxArchive.Text = "アーカイブ(&A)";
            this.checkBoxArchive.UseVisualStyleBackColor = true;
            // 
            // checkBoxHidden
            // 
            this.checkBoxHidden.AutoSize = true;
            this.checkBoxHidden.Location = new System.Drawing.Point(12, 41);
            this.checkBoxHidden.Name = "checkBoxHidden";
            this.checkBoxHidden.Size = new System.Drawing.Size(95, 16);
            this.checkBoxHidden.TabIndex = 1;
            this.checkBoxHidden.Text = "隠しファイル(&H)";
            this.checkBoxHidden.UseVisualStyleBackColor = true;
            // 
            // checkBoxReadOnly
            // 
            this.checkBoxReadOnly.AutoSize = true;
            this.checkBoxReadOnly.Location = new System.Drawing.Point(12, 19);
            this.checkBoxReadOnly.Name = "checkBoxReadOnly";
            this.checkBoxReadOnly.Size = new System.Drawing.Size(107, 16);
            this.checkBoxReadOnly.TabIndex = 0;
            this.checkBoxReadOnly.Text = "読み取り専用(&R)";
            this.checkBoxReadOnly.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonTimeAccess);
            this.groupBox3.Controls.Add(this.buttonTimeCreate);
            this.groupBox3.Controls.Add(this.buttonTimeUpdate);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.buttonCurrent);
            this.groupBox3.Controls.Add(this.buttonNoon);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.dateTimeUpdate);
            this.groupBox3.Controls.Add(this.dateTimeCreate);
            this.groupBox3.Controls.Add(this.dateTimeAccess);
            this.groupBox3.Location = new System.Drawing.Point(148, 128);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(299, 136);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "タイムスタンプ";
            // 
            // buttonTimeAccess
            // 
            this.buttonTimeAccess.Location = new System.Drawing.Point(257, 67);
            this.buttonTimeAccess.Name = "buttonTimeAccess";
            this.buttonTimeAccess.Size = new System.Drawing.Size(30, 23);
            this.buttonTimeAccess.TabIndex = 8;
            this.buttonTimeAccess.UseVisualStyleBackColor = true;
            this.buttonTimeAccess.Click += new System.EventHandler(this.buttonTimeAccess_Click);
            // 
            // buttonTimeCreate
            // 
            this.buttonTimeCreate.Location = new System.Drawing.Point(257, 42);
            this.buttonTimeCreate.Name = "buttonTimeCreate";
            this.buttonTimeCreate.Size = new System.Drawing.Size(30, 23);
            this.buttonTimeCreate.TabIndex = 5;
            this.buttonTimeCreate.UseVisualStyleBackColor = true;
            this.buttonTimeCreate.Click += new System.EventHandler(this.buttonTimeCreate_Click);
            // 
            // buttonTimeUpdate
            // 
            this.buttonTimeUpdate.Location = new System.Drawing.Point(257, 17);
            this.buttonTimeUpdate.Name = "buttonTimeUpdate";
            this.buttonTimeUpdate.Size = new System.Drawing.Size(30, 23);
            this.buttonTimeUpdate.TabIndex = 2;
            this.buttonTimeUpdate.UseVisualStyleBackColor = true;
            this.buttonTimeUpdate.Click += new System.EventHandler(this.buttonTimeUpdate_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "アクセス日時(&E):";
            // 
            // buttonCurrent
            // 
            this.buttonCurrent.Location = new System.Drawing.Point(13, 99);
            this.buttonCurrent.Name = "buttonCurrent";
            this.buttonCurrent.Size = new System.Drawing.Size(79, 23);
            this.buttonCurrent.TabIndex = 9;
            this.buttonCurrent.Text = "現在時刻(&D)";
            this.buttonCurrent.UseVisualStyleBackColor = true;
            this.buttonCurrent.Click += new System.EventHandler(this.buttonCurrent_Click);
            // 
            // buttonNoon
            // 
            this.buttonNoon.Location = new System.Drawing.Point(98, 99);
            this.buttonNoon.Name = "buttonNoon";
            this.buttonNoon.Size = new System.Drawing.Size(80, 23);
            this.buttonNoon.TabIndex = 10;
            this.buttonNoon.Text = "本日正午(&O)";
            this.buttonNoon.UseVisualStyleBackColor = true;
            this.buttonNoon.Click += new System.EventHandler(this.buttonNoon_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "作成日時(&C):";
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
            this.buttonCancel.Location = new System.Drawing.Point(372, 273);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(291, 273);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // RenameWindowsDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(459, 308);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenameWindowsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "名前の変更";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenameWindowsDialog_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimeUpdate;
        private System.Windows.Forms.DateTimePicker dateTimeCreate;
        private System.Windows.Forms.DateTimePicker dateTimeAccess;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxSystem;
        private System.Windows.Forms.CheckBox checkBoxArchive;
        private System.Windows.Forms.CheckBox checkBoxHidden;
        private System.Windows.Forms.CheckBox checkBoxReadOnly;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonNameLower;
        private System.Windows.Forms.Button buttonNameUpper;
        private System.Windows.Forms.Button buttonNameCapital;
        private System.Windows.Forms.Button buttonCurrent;
        private System.Windows.Forms.Button buttonNoon;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.LinkLabel linkLabelEtcAttr;
        private System.Windows.Forms.TextBox textBoxFileNameCurrent;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Button buttonTimeUpdate;
        private System.Windows.Forms.Button buttonTimeAccess;
        private System.Windows.Forms.Button buttonTimeCreate;
    }
}