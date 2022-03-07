namespace ShellFiler.UI.Dialog {
    partial class CommandInputDialog {
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
            this.comboBoxCommand = new ShellFiler.UI.Dialog.CommandInputDialog.CommandComboBox();
            this.buttonDeleteHistory = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.comboBoxResultMode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "コマンドライン(&C):";
            // 
            // comboBoxCommand
            // 
            this.comboBoxCommand.FormattingEnabled = true;
            this.comboBoxCommand.Location = new System.Drawing.Point(15, 33);
            this.comboBoxCommand.Name = "comboBoxCommand";
            this.comboBoxCommand.Size = new System.Drawing.Size(455, 23);
            this.comboBoxCommand.TabIndex = 1;
            this.comboBoxCommand.TextChanged += new System.EventHandler(this.comboBoxCommand_TextChanged);
            this.comboBoxCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxCommand_KeyDown);
            this.comboBoxCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBoxCommand_KeyPress);
            this.comboBoxCommand.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxCommand_KeyUp);
            this.comboBoxCommand.Leave += new System.EventHandler(this.comboBoxCommand_Leave);
            // 
            // buttonDeleteHistory
            // 
            this.buttonDeleteHistory.Location = new System.Drawing.Point(389, 63);
            this.buttonDeleteHistory.Name = "buttonDeleteHistory";
            this.buttonDeleteHistory.Size = new System.Drawing.Size(81, 23);
            this.buttonDeleteHistory.TabIndex = 2;
            this.buttonDeleteHistory.Text = "履歴削除(&H)";
            this.buttonDeleteHistory.UseVisualStyleBackColor = true;
            this.buttonDeleteHistory.Click += new System.EventHandler(this.buttonDeleteHistory_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(389, 115);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(81, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(302, 115);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(81, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // comboBoxResultMode
            // 
            this.comboBoxResultMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxResultMode.FormattingEnabled = true;
            this.comboBoxResultMode.Location = new System.Drawing.Point(15, 82);
            this.comboBoxResultMode.Name = "comboBoxResultMode";
            this.comboBoxResultMode.Size = new System.Drawing.Size(214, 23);
            this.comboBoxResultMode.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "オプション(&O):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(338, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Ctrl+Spaceで入力ヒント";
            // 
            // CommandInputDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(482, 150);
            this.Controls.Add(this.comboBoxResultMode);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonDeleteHistory);
            this.Controls.Add(this.comboBoxCommand);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommandInputDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "コマンドの実行";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CommandInputDialog_FormClosing);
            this.Move += new System.EventHandler(this.CommandInputDialog_Move);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonDeleteHistory;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private CommandInputDialog.CommandComboBox comboBoxCommand;
        private System.Windows.Forms.ComboBox comboBoxResultMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}