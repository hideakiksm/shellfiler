namespace ShellFiler.UI.Dialog.MonitoringViewer {
    partial class MonitoringResultCopyAsDialog {
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.textBoxSample = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioFormatCsv);
            this.groupBox1.Controls.Add(this.radioFormatTab);
            this.groupBox1.Controls.Add(this.radioFormatOrg);
            this.groupBox1.Location = new System.Drawing.Point(12, 139);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(225, 89);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "コピー形式";
            // 
            // radioFormatCsv
            // 
            this.radioFormatCsv.AutoSize = true;
            this.radioFormatCsv.Location = new System.Drawing.Point(6, 63);
            this.radioFormatCsv.Name = "radioFormatCsv";
            this.radioFormatCsv.Size = new System.Drawing.Size(125, 19);
            this.radioFormatCsv.TabIndex = 2;
            this.radioFormatCsv.TabStop = true;
            this.radioFormatCsv.Text = "表示内容のCSV(&C)";
            this.radioFormatCsv.UseVisualStyleBackColor = true;
            this.radioFormatCsv.CheckedChanged += new System.EventHandler(this.RadioFormat_CheckedChanged);
            // 
            // radioFormatTab
            // 
            this.radioFormatTab.AutoSize = true;
            this.radioFormatTab.Location = new System.Drawing.Point(7, 41);
            this.radioFormatTab.Name = "radioFormatTab";
            this.radioFormatTab.Size = new System.Drawing.Size(186, 19);
            this.radioFormatTab.TabIndex = 1;
            this.radioFormatTab.TabStop = true;
            this.radioFormatTab.Text = "表示内容のタブ区切りテキスト(&T)";
            this.radioFormatTab.UseVisualStyleBackColor = true;
            this.radioFormatTab.CheckedChanged += new System.EventHandler(this.RadioFormat_CheckedChanged);
            // 
            // radioFormatOrg
            // 
            this.radioFormatOrg.AutoSize = true;
            this.radioFormatOrg.Location = new System.Drawing.Point(7, 19);
            this.radioFormatOrg.Name = "radioFormatOrg";
            this.radioFormatOrg.Size = new System.Drawing.Size(181, 19);
            this.radioFormatOrg.TabIndex = 0;
            this.radioFormatOrg.TabStop = true;
            this.radioFormatOrg.Text = "コマンドの実行結果のテキスト(&X)";
            this.radioFormatOrg.UseVisualStyleBackColor = true;
            this.radioFormatOrg.CheckedChanged += new System.EventHandler(this.RadioFormat_CheckedChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(350, 230);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(269, 230);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // textBoxSample
            // 
            this.textBoxSample.Location = new System.Drawing.Point(15, 30);
            this.textBoxSample.Multiline = true;
            this.textBoxSample.Name = "textBoxSample";
            this.textBoxSample.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxSample.Size = new System.Drawing.Size(410, 103);
            this.textBoxSample.TabIndex = 6;
            this.textBoxSample.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "コピー文字列(&S):";
            // 
            // MonitoringResultCopyAsDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(437, 265);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxSample);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MonitoringResultCopyAsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "形式を指定してコピー";
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
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.TextBox textBoxSample;
        private System.Windows.Forms.Label label1;
    }
}