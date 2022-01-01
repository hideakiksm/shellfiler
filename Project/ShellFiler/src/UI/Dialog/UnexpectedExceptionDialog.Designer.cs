namespace ShellFiler.UI.Dialog {
    partial class UnexpectedExceptionDialog {
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
            this.labelMessage = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textBoxDetail = new System.Windows.Forms.TextBox();
            this.labelManualClose = new System.Windows.Forms.Label();
            this.labelAutoClose = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(62, 12);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(74, 12);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "labelMessage";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(328, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "申し訳ありませんが、この例外により内部状態が不安定になったため、";
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(44, 42);
            this.pictureBoxIcon.TabIndex = 1;
            this.pictureBoxIcon.TabStop = false;
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(391, 258);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "閉じる";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // textBoxDetail
            // 
            this.textBoxDetail.Location = new System.Drawing.Point(12, 87);
            this.textBoxDetail.Multiline = true;
            this.textBoxDetail.Name = "textBoxDetail";
            this.textBoxDetail.ReadOnly = true;
            this.textBoxDetail.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDetail.Size = new System.Drawing.Size(454, 165);
            this.textBoxDetail.TabIndex = 3;
            this.textBoxDetail.WordWrap = false;
            // 
            // labelManualClose
            // 
            this.labelManualClose.AutoSize = true;
            this.labelManualClose.Location = new System.Drawing.Point(62, 53);
            this.labelManualClose.Name = "labelManualClose";
            this.labelManualClose.Size = new System.Drawing.Size(157, 12);
            this.labelManualClose.TabIndex = 0;
            this.labelManualClose.Text = "ShellFilerを再起動してください。";
            // 
            // labelAutoClose
            // 
            this.labelAutoClose.AutoSize = true;
            this.labelAutoClose.Location = new System.Drawing.Point(62, 53);
            this.labelAutoClose.Name = "labelAutoClose";
            this.labelAutoClose.Size = new System.Drawing.Size(134, 12);
            this.labelAutoClose.TabIndex = 0;
            this.labelAutoClose.Text = "ShellFilerを再起動します。";
            // 
            // UnexpectedExceptionDialog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(478, 293);
            this.Controls.Add(this.textBoxDetail);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.labelAutoClose);
            this.Controls.Add(this.labelManualClose);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UnexpectedExceptionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "プログラムに問題がありました";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TextBox textBoxDetail;
        private System.Windows.Forms.Label labelManualClose;
        private System.Windows.Forms.Label labelAutoClose;
    }
}