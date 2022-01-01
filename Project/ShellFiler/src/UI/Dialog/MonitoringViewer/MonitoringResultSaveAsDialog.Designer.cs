namespace ShellFiler.UI.Dialog.MonitoringViewer {
    partial class MonitoringResultSaveAsDialog {
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
            this.radioFormatCsv = new System.Windows.Forms.RadioButton();
            this.radioFormatTab = new System.Windows.Forms.RadioButton();
            this.radioFormatOrg = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.buttonFileNameRef = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioFormatCsv);
            this.groupBox1.Controls.Add(this.radioFormatTab);
            this.groupBox1.Controls.Add(this.radioFormatOrg);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(225, 89);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "保存形式";
            // 
            // radioFormatCsv
            // 
            this.radioFormatCsv.AutoSize = true;
            this.radioFormatCsv.Location = new System.Drawing.Point(6, 63);
            this.radioFormatCsv.Name = "radioFormatCsv";
            this.radioFormatCsv.Size = new System.Drawing.Size(120, 16);
            this.radioFormatCsv.TabIndex = 2;
            this.radioFormatCsv.TabStop = true;
            this.radioFormatCsv.Text = "表示内容のCSV(&C)";
            this.radioFormatCsv.UseVisualStyleBackColor = true;
            // 
            // radioFormatTab
            // 
            this.radioFormatTab.AutoSize = true;
            this.radioFormatTab.Location = new System.Drawing.Point(7, 41);
            this.radioFormatTab.Name = "radioFormatTab";
            this.radioFormatTab.Size = new System.Drawing.Size(181, 16);
            this.radioFormatTab.TabIndex = 1;
            this.radioFormatTab.TabStop = true;
            this.radioFormatTab.Text = "表示内容のタブ区切りテキスト(&T)";
            this.radioFormatTab.UseVisualStyleBackColor = true;
            // 
            // radioFormatOrg
            // 
            this.radioFormatOrg.AutoSize = true;
            this.radioFormatOrg.Location = new System.Drawing.Point(7, 19);
            this.radioFormatOrg.Name = "radioFormatOrg";
            this.radioFormatOrg.Size = new System.Drawing.Size(177, 16);
            this.radioFormatOrg.TabIndex = 0;
            this.radioFormatOrg.TabStop = true;
            this.radioFormatOrg.Text = "コマンドの実行結果のテキスト(&X)";
            this.radioFormatOrg.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "ファイル名(&F):";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(22, 123);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(322, 19);
            this.textBoxFileName.TabIndex = 2;
            // 
            // buttonFileNameRef
            // 
            this.buttonFileNameRef.Location = new System.Drawing.Point(350, 121);
            this.buttonFileNameRef.Name = "buttonFileNameRef";
            this.buttonFileNameRef.Size = new System.Drawing.Size(75, 23);
            this.buttonFileNameRef.TabIndex = 3;
            this.buttonFileNameRef.Text = "参照(&R)";
            this.buttonFileNameRef.UseVisualStyleBackColor = true;
            this.buttonFileNameRef.Click += new System.EventHandler(this.buttonFileNameRef_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(350, 163);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(269, 163);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // MonitoringResultSaveAsDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(437, 198);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonFileNameRef);
            this.Controls.Add(this.textBoxFileName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MonitoringResultSaveAsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "名前を付けて保存";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioFormatTab;
        private System.Windows.Forms.RadioButton radioFormatOrg;
        private System.Windows.Forms.RadioButton radioFormatCsv;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Button buttonFileNameRef;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
    }
}