namespace ShellFiler.UI.Dialog.MonitoringViewer {
    partial class MonitoringSearchDialog {
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
            this.textBoxSearchStr = new System.Windows.Forms.TextBox();
            this.checkBoxCompareHead = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "検索する文字列(&F):";
            // 
            // textBoxSearchStr
            // 
            this.textBoxSearchStr.Location = new System.Drawing.Point(26, 28);
            this.textBoxSearchStr.Name = "textBoxSearchStr";
            this.textBoxSearchStr.Size = new System.Drawing.Size(186, 19);
            this.textBoxSearchStr.TabIndex = 1;
            this.textBoxSearchStr.TextChanged += new System.EventHandler(this.textBoxSearchStr_TextChanged);
            this.textBoxSearchStr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSearchStr_KeyDown);
            // 
            // checkBoxCompareHead
            // 
            this.checkBoxCompareHead.AutoSize = true;
            this.checkBoxCompareHead.Location = new System.Drawing.Point(15, 54);
            this.checkBoxCompareHead.Name = "checkBoxCompareHead";
            this.checkBoxCompareHead.Size = new System.Drawing.Size(154, 16);
            this.checkBoxCompareHead.TabIndex = 2;
            this.checkBoxCompareHead.Text = "各カラムの先頭から検索(&T)";
            this.checkBoxCompareHead.UseVisualStyleBackColor = true;
            this.checkBoxCompareHead.Enter += new System.EventHandler(this.SubControl_Enter);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(224, 77);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "戻る";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            this.buttonCancel.Enter += new System.EventHandler(this.SubControl_Enter);
            // 
            // buttonUp
            // 
            this.buttonUp.Location = new System.Drawing.Point(255, 12);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(44, 23);
            this.buttonUp.TabIndex = 4;
            this.buttonUp.Text = "↑";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            this.buttonUp.Enter += new System.EventHandler(this.SubControl_Enter);
            // 
            // buttonDown
            // 
            this.buttonDown.Location = new System.Drawing.Point(255, 41);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(44, 23);
            this.buttonDown.TabIndex = 5;
            this.buttonDown.Text = "↓";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            this.buttonDown.Enter += new System.EventHandler(this.SubControl_Enter);
            // 
            // MonitoringSearchDialog
            // 
            this.AcceptButton = this.buttonCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(311, 112);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.checkBoxCompareHead);
            this.Controls.Add(this.textBoxSearchStr);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MonitoringSearchDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "インクリメンタルサーチ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MonitoringSearchDialog_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSearchStr;
        private System.Windows.Forms.CheckBox checkBoxCompareHead;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
    }
}