namespace ShellFiler.UI.Dialog.FileViewer {
    partial class ViewerDumpSearchDialog {
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
            this.buttonInputHelp = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panelParent = new System.Windows.Forms.Panel();
            this.panelScroll = new System.Windows.Forms.Panel();
            this.textBoxText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.panelParent.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "検索するバイト列(&S):";
            // 
            // buttonInputHelp
            // 
            this.buttonInputHelp.Location = new System.Drawing.Point(304, 25);
            this.buttonInputHelp.Name = "buttonInputHelp";
            this.buttonInputHelp.Size = new System.Drawing.Size(26, 20);
            this.buttonInputHelp.TabIndex = 2;
            this.buttonInputHelp.Text = ">>";
            this.buttonInputHelp.UseVisualStyleBackColor = true;
            this.buttonInputHelp.Click += new System.EventHandler(this.buttonInputHelp_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(255, 146);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 7;
            this.buttonClose.Text = "閉じる";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(190, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "16進数、カンマ等での区切り";
            // 
            // panelParent
            // 
            this.panelParent.AutoScroll = true;
            this.panelParent.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelParent.Controls.Add(this.panelScroll);
            this.panelParent.Location = new System.Drawing.Point(12, 88);
            this.panelParent.Name = "panelParent";
            this.panelParent.Size = new System.Drawing.Size(318, 54);
            this.panelParent.TabIndex = 6;
            // 
            // panelScroll
            // 
            this.panelScroll.BackColor = System.Drawing.SystemColors.Window;
            this.panelScroll.Location = new System.Drawing.Point(0, 0);
            this.panelScroll.Name = "panelScroll";
            this.panelScroll.Size = new System.Drawing.Size(300, 50);
            this.panelScroll.TabIndex = 0;
            this.panelScroll.Paint += new System.Windows.Forms.PaintEventHandler(this.panelScroll_Paint);
            // 
            // textBoxText
            // 
            this.textBoxText.Location = new System.Drawing.Point(12, 63);
            this.textBoxText.Name = "textBoxText";
            this.textBoxText.Size = new System.Drawing.Size(286, 19);
            this.textBoxText.TabIndex = 5;
            this.textBoxText.TextChanged += new System.EventHandler(this.textBoxText_TextChanged);
            this.textBoxText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxInput_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "文字列で指定(&T):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(218, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "F1:先頭、F2:末尾、↑↓:次候補、→:メニュー";
            // 
            // textBoxInput
            // 
            this.textBoxInput.Location = new System.Drawing.Point(14, 25);
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(284, 19);
            this.textBoxInput.TabIndex = 1;
            this.textBoxInput.TextChanged += new System.EventHandler(this.textBoxInput_TextChanged);
            this.textBoxInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxInput_KeyDown);
            // 
            // ViewerDumpSearchDialog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(342, 183);
            this.Controls.Add(this.textBoxInput);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxText);
            this.Controls.Add(this.panelParent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonInputHelp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewerDumpSearchDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "文字列の検索";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ViewerDumpSearchDialog_FormClosed);
            this.panelParent.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonInputHelp;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelParent;
        private System.Windows.Forms.Panel panelScroll;
        private System.Windows.Forms.TextBox textBoxText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxInput;
    }
}