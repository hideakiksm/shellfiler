namespace ShellFiler.UI.Dialog {
    partial class FileCompareDialog {
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonTimeIgnore = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeOld = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeNew = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeSame = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonSizeIgnore = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeSmall = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeBig = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeSame = new System.Windows.Forms.RadioButton();
            this.checkBoxExceptFolder = new System.Windows.Forms.CheckBox();
            this.buttonNameOnly = new System.Windows.Forms.Button();
            this.buttonExactly = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(289, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "左右の一覧中、同じ名前のファイルを自動的にマークします。";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonTimeIgnore);
            this.groupBox1.Controls.Add(this.radioButtonTimeOld);
            this.groupBox1.Controls.Add(this.radioButtonTimeNew);
            this.groupBox1.Controls.Add(this.radioButtonTimeSame);
            this.groupBox1.Location = new System.Drawing.Point(13, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(152, 112);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "更新日時";
            // 
            // radioButtonTimeIgnore
            // 
            this.radioButtonTimeIgnore.AutoSize = true;
            this.radioButtonTimeIgnore.Location = new System.Drawing.Point(6, 85);
            this.radioButtonTimeIgnore.Name = "radioButtonTimeIgnore";
            this.radioButtonTimeIgnore.Size = new System.Drawing.Size(91, 16);
            this.radioButtonTimeIgnore.TabIndex = 3;
            this.radioButtonTimeIgnore.TabStop = true;
            this.radioButtonTimeIgnore.Text = "考慮しない(&T)";
            this.radioButtonTimeIgnore.UseVisualStyleBackColor = true;
            // 
            // radioButtonTimeOld
            // 
            this.radioButtonTimeOld.AutoSize = true;
            this.radioButtonTimeOld.Location = new System.Drawing.Point(6, 63);
            this.radioButtonTimeOld.Name = "radioButtonTimeOld";
            this.radioButtonTimeOld.Size = new System.Drawing.Size(116, 16);
            this.radioButtonTimeOld.TabIndex = 2;
            this.radioButtonTimeOld.TabStop = true;
            this.radioButtonTimeOld.Text = "古いものをマーク(&O)";
            this.radioButtonTimeOld.UseVisualStyleBackColor = true;
            // 
            // radioButtonTimeNew
            // 
            this.radioButtonTimeNew.AutoSize = true;
            this.radioButtonTimeNew.Location = new System.Drawing.Point(6, 41);
            this.radioButtonTimeNew.Name = "radioButtonTimeNew";
            this.radioButtonTimeNew.Size = new System.Drawing.Size(126, 16);
            this.radioButtonTimeNew.TabIndex = 1;
            this.radioButtonTimeNew.TabStop = true;
            this.radioButtonTimeNew.Text = "新しいものをマーク(&W)";
            this.radioButtonTimeNew.UseVisualStyleBackColor = true;
            // 
            // radioButtonTimeSame
            // 
            this.radioButtonTimeSame.AutoSize = true;
            this.radioButtonTimeSame.Location = new System.Drawing.Point(7, 19);
            this.radioButtonTimeSame.Name = "radioButtonTimeSame";
            this.radioButtonTimeSame.Size = new System.Drawing.Size(114, 16);
            this.radioButtonTimeSame.TabIndex = 0;
            this.radioButtonTimeSame.TabStop = true;
            this.radioButtonTimeSame.Text = "同じものをマーク(&S)";
            this.radioButtonTimeSame.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonSizeIgnore);
            this.groupBox2.Controls.Add(this.radioButtonSizeSmall);
            this.groupBox2.Controls.Add(this.radioButtonSizeBig);
            this.groupBox2.Controls.Add(this.radioButtonSizeSame);
            this.groupBox2.Location = new System.Drawing.Point(171, 33);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(152, 112);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ファイルサイズ";
            // 
            // radioButtonSizeIgnore
            // 
            this.radioButtonSizeIgnore.AutoSize = true;
            this.radioButtonSizeIgnore.Location = new System.Drawing.Point(6, 85);
            this.radioButtonSizeIgnore.Name = "radioButtonSizeIgnore";
            this.radioButtonSizeIgnore.Size = new System.Drawing.Size(92, 16);
            this.radioButtonSizeIgnore.TabIndex = 3;
            this.radioButtonSizeIgnore.TabStop = true;
            this.radioButtonSizeIgnore.Text = "考慮しない(&G)";
            this.radioButtonSizeIgnore.UseVisualStyleBackColor = true;
            // 
            // radioButtonSizeSmall
            // 
            this.radioButtonSizeSmall.AutoSize = true;
            this.radioButtonSizeSmall.Location = new System.Drawing.Point(6, 63);
            this.radioButtonSizeSmall.Name = "radioButtonSizeSmall";
            this.radioButtonSizeSmall.Size = new System.Drawing.Size(125, 16);
            this.radioButtonSizeSmall.TabIndex = 2;
            this.radioButtonSizeSmall.TabStop = true;
            this.radioButtonSizeSmall.Text = "小さいものをマーク(&M)";
            this.radioButtonSizeSmall.UseVisualStyleBackColor = true;
            // 
            // radioButtonSizeBig
            // 
            this.radioButtonSizeBig.AutoSize = true;
            this.radioButtonSizeBig.Location = new System.Drawing.Point(6, 41);
            this.radioButtonSizeBig.Name = "radioButtonSizeBig";
            this.radioButtonSizeBig.Size = new System.Drawing.Size(125, 16);
            this.radioButtonSizeBig.TabIndex = 1;
            this.radioButtonSizeBig.TabStop = true;
            this.radioButtonSizeBig.Text = "大きいものをマーク(&B)";
            this.radioButtonSizeBig.UseVisualStyleBackColor = true;
            // 
            // radioButtonSizeSame
            // 
            this.radioButtonSizeSame.AutoSize = true;
            this.radioButtonSizeSame.Location = new System.Drawing.Point(7, 19);
            this.radioButtonSizeSame.Name = "radioButtonSizeSame";
            this.radioButtonSizeSame.Size = new System.Drawing.Size(110, 16);
            this.radioButtonSizeSame.TabIndex = 0;
            this.radioButtonSizeSame.TabStop = true;
            this.radioButtonSizeSame.Text = "同じものをマーク(&I)";
            this.radioButtonSizeSame.UseVisualStyleBackColor = true;
            // 
            // checkBoxExceptFolder
            // 
            this.checkBoxExceptFolder.AutoSize = true;
            this.checkBoxExceptFolder.Location = new System.Drawing.Point(13, 156);
            this.checkBoxExceptFolder.Name = "checkBoxExceptFolder";
            this.checkBoxExceptFolder.Size = new System.Drawing.Size(107, 16);
            this.checkBoxExceptFolder.TabIndex = 5;
            this.checkBoxExceptFolder.Text = "フォルダを除外(&F)";
            this.checkBoxExceptFolder.UseVisualStyleBackColor = true;
            // 
            // buttonNameOnly
            // 
            this.buttonNameOnly.Location = new System.Drawing.Point(229, 178);
            this.buttonNameOnly.Name = "buttonNameOnly";
            this.buttonNameOnly.Size = new System.Drawing.Size(94, 23);
            this.buttonNameOnly.TabIndex = 4;
            this.buttonNameOnly.Text = "名前のみ(&N)";
            this.buttonNameOnly.UseVisualStyleBackColor = true;
            this.buttonNameOnly.Click += new System.EventHandler(this.buttonNameOnly_Click);
            // 
            // buttonExactly
            // 
            this.buttonExactly.Location = new System.Drawing.Point(229, 151);
            this.buttonExactly.Name = "buttonExactly";
            this.buttonExactly.Size = new System.Drawing.Size(94, 23);
            this.buttonExactly.TabIndex = 3;
            this.buttonExactly.Text = "同一ファイル(&A)";
            this.buttonExactly.UseVisualStyleBackColor = true;
            this.buttonExactly.Click += new System.EventHandler(this.buttonExactly_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(249, 218);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(168, 218);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // FileCompareDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(336, 253);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonExactly);
            this.Controls.Add(this.buttonNameOnly);
            this.Controls.Add(this.checkBoxExceptFolder);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileCompareDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ファイル一覧の比較";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FileCompareDialog_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonTimeIgnore;
        private System.Windows.Forms.RadioButton radioButtonTimeOld;
        private System.Windows.Forms.RadioButton radioButtonTimeNew;
        private System.Windows.Forms.RadioButton radioButtonTimeSame;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonSizeIgnore;
        private System.Windows.Forms.RadioButton radioButtonSizeSmall;
        private System.Windows.Forms.RadioButton radioButtonSizeBig;
        private System.Windows.Forms.RadioButton radioButtonSizeSame;
        private System.Windows.Forms.CheckBox checkBoxExceptFolder;
        private System.Windows.Forms.Button buttonNameOnly;
        private System.Windows.Forms.Button buttonExactly;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
    }
}