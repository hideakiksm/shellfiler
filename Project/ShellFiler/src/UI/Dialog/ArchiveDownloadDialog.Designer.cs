namespace ShellFiler.UI.Dialog {
    partial class ArchiveDownloadDialog {
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
            this.labelBit1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelEx1 = new System.Windows.Forms.Label();
            this.linkLabelDL = new System.Windows.Forms.LinkLabel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelEx2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelBit2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(267, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "この環境では圧縮と展開の機能を使うことができません。";
            // 
            // labelBit1
            // 
            this.labelBit1.AutoSize = true;
            this.labelBit1.Location = new System.Drawing.Point(10, 37);
            this.labelBit1.Name = "labelBit1";
            this.labelBit1.Size = new System.Drawing.Size(283, 12);
            this.labelBit1.TabIndex = 1;
            this.labelBit1.Text = "圧縮ファイルを扱うには、7-Zipの{0}bit版をダウンロードして、";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(208, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "このコンピューターにインストールしてください。";
            // 
            // labelEx1
            // 
            this.labelEx1.AutoSize = true;
            this.labelEx1.Location = new System.Drawing.Point(10, 106);
            this.labelEx1.Name = "labelEx1";
            this.labelEx1.Size = new System.Drawing.Size(312, 12);
            this.labelEx1.TabIndex = 4;
            this.labelEx1.Text = "7-Zipのインストールが完了すると、ShellFilerは自動的に認識して";
            // 
            // linkLabelDL
            // 
            this.linkLabelDL.AutoSize = true;
            this.linkLabelDL.Location = new System.Drawing.Point(29, 69);
            this.linkLabelDL.Name = "linkLabelDL";
            this.linkLabelDL.Size = new System.Drawing.Size(100, 12);
            this.linkLabelDL.TabIndex = 3;
            this.linkLabelDL.TabStop = true;
            this.linkLabelDL.Text = "ダウンロードページへ";
            this.linkLabelDL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelDL_LinkClicked);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(287, 192);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "閉じる";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // labelEx2
            // 
            this.labelEx2.AutoSize = true;
            this.labelEx2.Location = new System.Drawing.Point(10, 122);
            this.labelEx2.Name = "labelEx2";
            this.labelEx2.Size = new System.Drawing.Size(187, 12);
            this.labelEx2.TabIndex = 5;
            this.labelEx2.Text = "7-ZipのDLLで圧縮と展開を行います。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(300, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "7-Zipをインストールしても、この画面が表示され続ける場合は、";
            // 
            // labelBit2
            // 
            this.labelBit2.AutoSize = true;
            this.labelBit2.Location = new System.Drawing.Point(10, 166);
            this.labelBit2.Name = "labelBit2";
            this.labelBit2.Size = new System.Drawing.Size(290, 12);
            this.labelBit2.TabIndex = 5;
            this.labelBit2.Text = "インストールした7-Zipが{0}bit版であることを確認してください。";
            // 
            // ArchiveDownloadDialog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(374, 227);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.linkLabelDL);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelBit1);
            this.Controls.Add(this.labelBit2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelEx2);
            this.Controls.Add(this.labelEx1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArchiveDownloadDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ファイルの圧縮と展開";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelBit1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelEx1;
        private System.Windows.Forms.LinkLabel linkLabelDL;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelEx2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelBit2;
    }
}