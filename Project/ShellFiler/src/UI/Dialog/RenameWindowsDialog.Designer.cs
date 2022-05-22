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
            this.dateTimeUpdate.Location = new System.Drawing.Point(190, 36);
            this.dateTimeUpdate.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.dateTimeUpdate.Name = "dateTimeUpdate";
            this.dateTimeUpdate.Size = new System.Drawing.Size(292, 35);
            this.dateTimeUpdate.TabIndex = 1;
            // 
            // dateTimeCreate
            // 
            this.dateTimeCreate.CustomFormat = "yyyy/MM/dd  HH:mm:ss";
            this.dateTimeCreate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeCreate.Location = new System.Drawing.Point(190, 83);
            this.dateTimeCreate.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.dateTimeCreate.Name = "dateTimeCreate";
            this.dateTimeCreate.Size = new System.Drawing.Size(292, 35);
            this.dateTimeCreate.TabIndex = 4;
            // 
            // dateTimeAccess
            // 
            this.dateTimeAccess.CustomFormat = "yyyy/MM/dd  HH:mm:ss";
            this.dateTimeAccess.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeAccess.Location = new System.Drawing.Point(190, 131);
            this.dateTimeAccess.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.dateTimeAccess.Name = "dateTimeAccess";
            this.dateTimeAccess.Size = new System.Drawing.Size(292, 35);
            this.dateTimeAccess.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 33);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "新しい名前(&N):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 114);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 30);
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
            this.groupBox1.Location = new System.Drawing.Point(23, 23);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox1.Size = new System.Drawing.Size(808, 191);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ファイル名";
            // 
            // textBoxFileNameCurrent
            // 
            this.textBoxFileNameCurrent.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxFileNameCurrent.Location = new System.Drawing.Point(44, 142);
            this.textBoxFileNameCurrent.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.textBoxFileNameCurrent.Name = "textBoxFileNameCurrent";
            this.textBoxFileNameCurrent.ReadOnly = true;
            this.textBoxFileNameCurrent.Size = new System.Drawing.Size(585, 30);
            this.textBoxFileNameCurrent.TabIndex = 3;
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxFileName.Location = new System.Drawing.Point(44, 63);
            this.textBoxFileName.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(585, 30);
            this.textBoxFileName.TabIndex = 1;
            // 
            // buttonNameCapital
            // 
            this.buttonNameCapital.Location = new System.Drawing.Point(639, 131);
            this.buttonNameCapital.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonNameCapital.Name = "buttonNameCapital";
            this.buttonNameCapital.Size = new System.Drawing.Size(159, 40);
            this.buttonNameCapital.TabIndex = 6;
            this.buttonNameCapital.Text = "先頭大文字(&T)";
            this.buttonNameCapital.UseVisualStyleBackColor = true;
            this.buttonNameCapital.Click += new System.EventHandler(this.buttonNameCapital_Click);
            // 
            // buttonNameLower
            // 
            this.buttonNameLower.Location = new System.Drawing.Point(639, 84);
            this.buttonNameLower.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonNameLower.Name = "buttonNameLower";
            this.buttonNameLower.Size = new System.Drawing.Size(159, 40);
            this.buttonNameLower.TabIndex = 5;
            this.buttonNameLower.Text = "小文字へ(&L)";
            this.buttonNameLower.UseVisualStyleBackColor = true;
            this.buttonNameLower.Click += new System.EventHandler(this.buttonNameLower_Click);
            // 
            // buttonNameUpper
            // 
            this.buttonNameUpper.Location = new System.Drawing.Point(639, 37);
            this.buttonNameUpper.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonNameUpper.Name = "buttonNameUpper";
            this.buttonNameUpper.Size = new System.Drawing.Size(159, 40);
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
            this.groupBox2.Location = new System.Drawing.Point(23, 224);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox2.Size = new System.Drawing.Size(243, 238);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "属性";
            // 
            // linkLabelEtcAttr
            // 
            this.linkLabelEtcAttr.AutoSize = true;
            this.linkLabelEtcAttr.Location = new System.Drawing.Point(21, 189);
            this.linkLabelEtcAttr.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.linkLabelEtcAttr.Name = "linkLabelEtcAttr";
            this.linkLabelEtcAttr.Size = new System.Drawing.Size(129, 30);
            this.linkLabelEtcAttr.TabIndex = 4;
            this.linkLabelEtcAttr.TabStop = true;
            this.linkLabelEtcAttr.Text = "詳細の表示...";
            this.linkLabelEtcAttr.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEtcAttr_LinkClicked);
            // 
            // checkBoxSystem
            // 
            this.checkBoxSystem.AutoSize = true;
            this.checkBoxSystem.Location = new System.Drawing.Point(21, 149);
            this.checkBoxSystem.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.checkBoxSystem.Name = "checkBoxSystem";
            this.checkBoxSystem.Size = new System.Drawing.Size(127, 34);
            this.checkBoxSystem.TabIndex = 3;
            this.checkBoxSystem.Text = "システム(&S)";
            this.checkBoxSystem.UseVisualStyleBackColor = true;
            // 
            // checkBoxArchive
            // 
            this.checkBoxArchive.AutoSize = true;
            this.checkBoxArchive.Location = new System.Drawing.Point(21, 110);
            this.checkBoxArchive.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.checkBoxArchive.Name = "checkBoxArchive";
            this.checkBoxArchive.Size = new System.Drawing.Size(141, 34);
            this.checkBoxArchive.TabIndex = 2;
            this.checkBoxArchive.Text = "アーカイブ(&A)";
            this.checkBoxArchive.UseVisualStyleBackColor = true;
            // 
            // checkBoxHidden
            // 
            this.checkBoxHidden.AutoSize = true;
            this.checkBoxHidden.Location = new System.Drawing.Point(21, 72);
            this.checkBoxHidden.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.checkBoxHidden.Name = "checkBoxHidden";
            this.checkBoxHidden.Size = new System.Drawing.Size(161, 34);
            this.checkBoxHidden.TabIndex = 1;
            this.checkBoxHidden.Text = "隠しファイル(&H)";
            this.checkBoxHidden.UseVisualStyleBackColor = true;
            // 
            // checkBoxReadOnly
            // 
            this.checkBoxReadOnly.AutoSize = true;
            this.checkBoxReadOnly.Location = new System.Drawing.Point(21, 33);
            this.checkBoxReadOnly.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.checkBoxReadOnly.Name = "checkBoxReadOnly";
            this.checkBoxReadOnly.Size = new System.Drawing.Size(181, 34);
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
            this.groupBox3.Location = new System.Drawing.Point(276, 224);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox3.Size = new System.Drawing.Size(554, 238);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "タイムスタンプ";
            // 
            // buttonTimeAccess
            // 
            this.buttonTimeAccess.Location = new System.Drawing.Point(492, 130);
            this.buttonTimeAccess.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonTimeAccess.Name = "buttonTimeAccess";
            this.buttonTimeAccess.Size = new System.Drawing.Size(52, 40);
            this.buttonTimeAccess.TabIndex = 8;
            this.buttonTimeAccess.UseVisualStyleBackColor = true;
            this.buttonTimeAccess.Click += new System.EventHandler(this.buttonTimeAccess_Click);
            // 
            // buttonTimeCreate
            // 
            this.buttonTimeCreate.Location = new System.Drawing.Point(492, 82);
            this.buttonTimeCreate.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonTimeCreate.Name = "buttonTimeCreate";
            this.buttonTimeCreate.Size = new System.Drawing.Size(52, 40);
            this.buttonTimeCreate.TabIndex = 5;
            this.buttonTimeCreate.UseVisualStyleBackColor = true;
            this.buttonTimeCreate.Click += new System.EventHandler(this.buttonTimeCreate_Click);
            // 
            // buttonTimeUpdate
            // 
            this.buttonTimeUpdate.Location = new System.Drawing.Point(492, 35);
            this.buttonTimeUpdate.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonTimeUpdate.Name = "buttonTimeUpdate";
            this.buttonTimeUpdate.Size = new System.Drawing.Size(52, 40);
            this.buttonTimeUpdate.TabIndex = 2;
            this.buttonTimeUpdate.UseVisualStyleBackColor = true;
            this.buttonTimeUpdate.Click += new System.EventHandler(this.buttonTimeUpdate_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 136);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(148, 30);
            this.label5.TabIndex = 6;
            this.label5.Text = "アクセス日時(&E):";
            // 
            // buttonCurrent
            // 
            this.buttonCurrent.Location = new System.Drawing.Point(23, 184);
            this.buttonCurrent.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonCurrent.Name = "buttonCurrent";
            this.buttonCurrent.Size = new System.Drawing.Size(138, 40);
            this.buttonCurrent.TabIndex = 9;
            this.buttonCurrent.Text = "現在時刻(&D)";
            this.buttonCurrent.UseVisualStyleBackColor = true;
            this.buttonCurrent.Click += new System.EventHandler(this.buttonCurrent_Click);
            // 
            // buttonNoon
            // 
            this.buttonNoon.Location = new System.Drawing.Point(172, 184);
            this.buttonNoon.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonNoon.Name = "buttonNoon";
            this.buttonNoon.Size = new System.Drawing.Size(140, 40);
            this.buttonNoon.TabIndex = 10;
            this.buttonNoon.Text = "本日正午(&O)";
            this.buttonNoon.UseVisualStyleBackColor = true;
            this.buttonNoon.Click += new System.EventHandler(this.buttonNoon_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 89);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 30);
            this.label4.TabIndex = 3;
            this.label4.Text = "作成日時(&C):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 42);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 30);
            this.label3.TabIndex = 0;
            this.label3.Text = "更新日時(&M):";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(700, 472);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(131, 40);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(559, 472);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(131, 40);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // RenameWindowsDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(844, 537);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
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