namespace ShellFiler.UI.Dialog {
    partial class FileFilterQuickDialog {
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
            this.listBoxSetting = new System.Windows.Forms.ListBox();
            this.buttonWrite = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBoxSetting
            // 
            this.listBoxSetting.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBoxSetting.FormattingEnabled = true;
            this.listBoxSetting.ItemHeight = 12;
            this.listBoxSetting.Location = new System.Drawing.Point(12, 33);
            this.listBoxSetting.Name = "listBoxSetting";
            this.listBoxSetting.Size = new System.Drawing.Size(380, 184);
            this.listBoxSetting.TabIndex = 0;
            this.listBoxSetting.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxSetting_DrawItem);
            this.listBoxSetting.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBoxSetting_MeasureItem);
            this.listBoxSetting.SelectedIndexChanged += new System.EventHandler(this.listBoxSetting_SelectedIndexChanged);
            // 
            // buttonWrite
            // 
            this.buttonWrite.Location = new System.Drawing.Point(398, 33);
            this.buttonWrite.Name = "buttonWrite";
            this.buttonWrite.Size = new System.Drawing.Size(83, 23);
            this.buttonWrite.TabIndex = 1;
            this.buttonWrite.Text = "上書き(&W)";
            this.buttonWrite.UseVisualStyleBackColor = true;
            this.buttonWrite.Click += new System.EventHandler(this.buttonWrite_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(398, 62);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(83, 23);
            this.buttonDelete.TabIndex = 2;
            this.buttonDelete.Text = "削除(&D)";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Location = new System.Drawing.Point(398, 91);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(83, 23);
            this.buttonEdit.TabIndex = 3;
            this.buttonEdit.Text = "名前変更(&R)...";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Location = new System.Drawing.Point(398, 165);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(83, 23);
            this.buttonUp.TabIndex = 4;
            this.buttonUp.Text = "上へ(&U)";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Location = new System.Drawing.Point(398, 194);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(83, 23);
            this.buttonDown.TabIndex = 5;
            this.buttonDown.Text = "下へ(&W)";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(406, 230);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 7;
            this.buttonClose.Text = "閉じる";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(451, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "現在のフィルター一覧をクイック設定に登録すると、ボタン1つで登録済みの設定を使用できます。";
            // 
            // FileFilterQuickDialog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(494, 268);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonWrite);
            this.Controls.Add(this.listBoxSetting);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileFilterQuickDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "クイック設定";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxSetting;
        private System.Windows.Forms.Button buttonWrite;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label1;
    }
}