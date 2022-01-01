namespace ShellFiler.UI.Dialog.FileViewer {
    partial class ViewerCompareSuccessDialog {
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
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButtonDispose = new System.Windows.Forms.RadioButton();
            this.radioButtonNextUse = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "差分表示ツールを起動しました。";
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(43, 50);
            this.pictureBoxIcon.TabIndex = 2;
            this.pictureBoxIcon.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(325, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "このダイアログを閉じると、比較に使用した一時ファイルを削除します。";
            // 
            // radioButtonDispose
            // 
            this.radioButtonDispose.AutoSize = true;
            this.radioButtonDispose.Location = new System.Drawing.Point(9, 18);
            this.radioButtonDispose.Name = "radioButtonDispose";
            this.radioButtonDispose.Size = new System.Drawing.Size(160, 16);
            this.radioButtonDispose.TabIndex = 0;
            this.radioButtonDispose.TabStop = true;
            this.radioButtonDispose.Text = "登録された文字列を破棄(&D)";
            this.radioButtonDispose.UseVisualStyleBackColor = true;
            // 
            // radioButtonNextUse
            // 
            this.radioButtonNextUse.AutoSize = true;
            this.radioButtonNextUse.Location = new System.Drawing.Point(9, 40);
            this.radioButtonNextUse.Name = "radioButtonNextUse";
            this.radioButtonNextUse.Size = new System.Drawing.Size(206, 16);
            this.radioButtonNextUse.TabIndex = 1;
            this.radioButtonNextUse.TabStop = true;
            this.radioButtonNextUse.Text = "そのまま残して次回の比較にも使用(&N)";
            this.radioButtonNextUse.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonDispose);
            this.groupBox1.Controls.Add(this.radioButtonNextUse);
            this.groupBox1.Location = new System.Drawing.Point(63, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 63);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "登録済みの選択範囲の文字列";
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(321, 128);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "閉じる";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // ViewerCompareSuccessDialog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(408, 163);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewerCompareSuccessDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "選択範囲を差分表示ツールで比較";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioButtonDispose;
        private System.Windows.Forms.RadioButton radioButtonNextUse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonClose;
    }
}