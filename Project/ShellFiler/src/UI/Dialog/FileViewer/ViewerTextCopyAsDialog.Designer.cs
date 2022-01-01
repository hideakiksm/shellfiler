namespace ShellFiler.UI.Dialog.FileViewer {
    partial class ViewerTextCopyAsDialog {
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
            this.radioCrOrg = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioCrCrLf = new System.Windows.Forms.RadioButton();
            this.radioCrCr = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioTabSpace = new System.Windows.Forms.RadioButton();
            this.radioTabOrg = new System.Windows.Forms.RadioButton();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioCrOrg
            // 
            this.radioCrOrg.AutoSize = true;
            this.radioCrOrg.Location = new System.Drawing.Point(10, 18);
            this.radioCrOrg.Name = "radioCrOrg";
            this.radioCrOrg.Size = new System.Drawing.Size(79, 16);
            this.radioCrOrg.TabIndex = 0;
            this.radioCrOrg.TabStop = true;
            this.radioCrOrg.Text = "元のまま(&O)";
            this.radioCrOrg.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioCrCrLf);
            this.groupBox1.Controls.Add(this.radioCrCr);
            this.groupBox1.Controls.Add(this.radioCrOrg);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(102, 87);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "改行コード";
            // 
            // radioCrCrLf
            // 
            this.radioCrCrLf.AutoSize = true;
            this.radioCrCrLf.Location = new System.Drawing.Point(10, 62);
            this.radioCrCrLf.Name = "radioCrCrLf";
            this.radioCrCrLf.Size = new System.Drawing.Size(72, 16);
            this.radioCrCrLf.TabIndex = 2;
            this.radioCrCrLf.TabStop = true;
            this.radioCrCrLf.Text = "CR+LF(L)";
            this.radioCrCrLf.UseVisualStyleBackColor = true;
            // 
            // radioCrCr
            // 
            this.radioCrCr.AutoSize = true;
            this.radioCrCr.Location = new System.Drawing.Point(10, 40);
            this.radioCrCr.Name = "radioCrCr";
            this.radioCrCr.Size = new System.Drawing.Size(74, 16);
            this.radioCrCr.TabIndex = 1;
            this.radioCrCr.TabStop = true;
            this.radioCrCr.Text = "CRだけ(&C)";
            this.radioCrCr.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioTabSpace);
            this.groupBox2.Controls.Add(this.radioTabOrg);
            this.groupBox2.Location = new System.Drawing.Point(122, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(144, 87);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "タブ文字";
            // 
            // radioTabSpace
            // 
            this.radioTabSpace.AutoSize = true;
            this.radioTabSpace.Location = new System.Drawing.Point(11, 41);
            this.radioTabSpace.Name = "radioTabSpace";
            this.radioTabSpace.Size = new System.Drawing.Size(95, 16);
            this.radioTabSpace.TabIndex = 1;
            this.radioTabSpace.TabStop = true;
            this.radioTabSpace.Text = "空白に変換(&S)";
            this.radioTabSpace.UseVisualStyleBackColor = true;
            // 
            // radioTabOrg
            // 
            this.radioTabOrg.AutoSize = true;
            this.radioTabOrg.Location = new System.Drawing.Point(11, 19);
            this.radioTabOrg.Name = "radioTabOrg";
            this.radioTabOrg.Size = new System.Drawing.Size(116, 16);
            this.radioTabOrg.TabIndex = 0;
            this.radioTabOrg.TabStop = true;
            this.radioTabOrg.Text = "TABコードのまま(&T)";
            this.radioTabOrg.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(110, 106);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(191, 106);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // ViewerTextCopyAsDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(278, 142);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewerTextCopyAsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "形式を指定してコピー";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioCrOrg;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioCrCrLf;
        private System.Windows.Forms.RadioButton radioCrCr;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioTabSpace;
        private System.Windows.Forms.RadioButton radioTabOrg;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
    }
}