namespace ShellFiler.UI.Dialog.MonitoringViewer {
    partial class KillConfirmDialog {
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
            this.labelMessageKill = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.buttonNo = new System.Windows.Forms.Button();
            this.buttonYes = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPid = new System.Windows.Forms.TextBox();
            this.textBoxCommand = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labelMessageForce = new System.Windows.Forms.Label();
            this.labelMessageForce2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMessageKill
            // 
            this.labelMessageKill.AutoSize = true;
            this.labelMessageKill.Location = new System.Drawing.Point(36, 12);
            this.labelMessageKill.Name = "labelMessageKill";
            this.labelMessageKill.Size = new System.Drawing.Size(132, 15);
            this.labelMessageKill.TabIndex = 0;
            this.labelMessageKill.Text = "次のコマンドを終了します。";
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(12, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(18, 18);
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // buttonNo
            // 
            this.buttonNo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonNo.Location = new System.Drawing.Point(341, 130);
            this.buttonNo.Name = "buttonNo";
            this.buttonNo.Size = new System.Drawing.Size(75, 23);
            this.buttonNo.TabIndex = 7;
            this.buttonNo.Text = "キャンセル";
            this.buttonNo.UseVisualStyleBackColor = true;
            // 
            // buttonYes
            // 
            this.buttonYes.Location = new System.Drawing.Point(260, 130);
            this.buttonYes.Name = "buttonYes";
            this.buttonYes.Size = new System.Drawing.Size(75, 23);
            this.buttonYes.TabIndex = 6;
            this.buttonYes.Text = "終了(&Y)";
            this.buttonYes.UseVisualStyleBackColor = true;
            this.buttonYes.Click += new System.EventHandler(this.buttonYes_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "PID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "COMMAND:";
            // 
            // textBoxPid
            // 
            this.textBoxPid.Location = new System.Drawing.Point(90, 52);
            this.textBoxPid.Name = "textBoxPid";
            this.textBoxPid.ReadOnly = true;
            this.textBoxPid.Size = new System.Drawing.Size(322, 23);
            this.textBoxPid.TabIndex = 3;
            // 
            // textBoxCommand
            // 
            this.textBoxCommand.Location = new System.Drawing.Point(89, 79);
            this.textBoxCommand.Name = "textBoxCommand";
            this.textBoxCommand.ReadOnly = true;
            this.textBoxCommand.Size = new System.Drawing.Size(323, 23);
            this.textBoxCommand.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "よろしいですか？";
            // 
            // labelMessageForce
            // 
            this.labelMessageForce.AutoSize = true;
            this.labelMessageForce.Location = new System.Drawing.Point(36, 12);
            this.labelMessageForce.Name = "labelMessageForce";
            this.labelMessageForce.Size = new System.Drawing.Size(156, 15);
            this.labelMessageForce.TabIndex = 0;
            this.labelMessageForce.Text = "次のコマンドを強制終了します。";
            // 
            // labelMessageForce2
            // 
            this.labelMessageForce2.AutoSize = true;
            this.labelMessageForce2.BackColor = System.Drawing.Color.Yellow;
            this.labelMessageForce2.Location = new System.Drawing.Point(12, 109);
            this.labelMessageForce2.Name = "labelMessageForce2";
            this.labelMessageForce2.Size = new System.Drawing.Size(400, 15);
            this.labelMessageForce2.TabIndex = 1;
            this.labelMessageForce2.Text = "強制終了する前に「プロセス終了」コマンドで通常の終了ができないか試してください。";
            // 
            // KillConfirmDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonNo;
            this.ClientSize = new System.Drawing.Size(428, 165);
            this.Controls.Add(this.textBoxCommand);
            this.Controls.Add(this.textBoxPid);
            this.Controls.Add(this.buttonYes);
            this.Controls.Add(this.buttonNo);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelMessageForce2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelMessageForce);
            this.Controls.Add(this.labelMessageKill);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KillConfirmDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "コマンドの終了";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMessageKill;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button buttonNo;
        private System.Windows.Forms.Button buttonYes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPid;
        private System.Windows.Forms.TextBox textBoxCommand;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelMessageForce;
        private System.Windows.Forms.Label labelMessageForce2;
    }
}