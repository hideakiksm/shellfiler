namespace ShellFiler.UI.Dialog {
    partial class LicenseAgreeDialog {
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
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonAgree = new System.Windows.Forms.RadioButton();
            this.radioButtonDisagree = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(551, 300);
            this.webBrowser.TabIndex = 0;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(492, 357);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 340);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "使用許諾に同意しますか？";
            // 
            // radioButtonAgree
            // 
            this.radioButtonAgree.AutoSize = true;
            this.radioButtonAgree.Location = new System.Drawing.Point(154, 338);
            this.radioButtonAgree.Name = "radioButtonAgree";
            this.radioButtonAgree.Size = new System.Drawing.Size(86, 19);
            this.radioButtonAgree.TabIndex = 3;
            this.radioButtonAgree.Text = "同意する(&A)";
            this.radioButtonAgree.UseVisualStyleBackColor = true;
            // 
            // radioButtonDisagree
            // 
            this.radioButtonDisagree.AutoSize = true;
            this.radioButtonDisagree.Checked = true;
            this.radioButtonDisagree.Location = new System.Drawing.Point(248, 338);
            this.radioButtonDisagree.Name = "radioButtonDisagree";
            this.radioButtonDisagree.Size = new System.Drawing.Size(97, 19);
            this.radioButtonDisagree.TabIndex = 4;
            this.radioButtonDisagree.TabStop = true;
            this.radioButtonDisagree.Text = "同意しない(&D)";
            this.radioButtonDisagree.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.webBrowser);
            this.panel1.Location = new System.Drawing.Point(12, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(555, 304);
            this.panel1.TabIndex = 1;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(10, 11);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(55, 15);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "使用許諾";
            // 
            // LicenseAgreeDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(579, 392);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.radioButtonDisagree);
            this.Controls.Add(this.radioButtonAgree);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOk);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LicenseAgreeDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ShellFilerの使用許諾";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LicenseAgreeDialog_FormClosed);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonAgree;
        private System.Windows.Forms.RadioButton radioButtonDisagree;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelTitle;
    }
}