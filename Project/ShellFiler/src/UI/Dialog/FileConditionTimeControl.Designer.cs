namespace ShellFiler.UI.Dialog {
    partial class FileConditionTimeControl {
        /// <summary> 
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.panelRange = new System.Windows.Forms.Panel();
            this.checkBoxStart = new System.Windows.Forms.CheckBox();
            this.checkBoxEnd = new System.Windows.Forms.CheckBox();
            this.maskedStart = new System.Windows.Forms.MaskedTextBox();
            this.maskedEnd = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // panelRange
            // 
            this.panelRange.BackColor = System.Drawing.SystemColors.Window;
            this.panelRange.Location = new System.Drawing.Point(0, 23);
            this.panelRange.Name = "panelRange";
            this.panelRange.Size = new System.Drawing.Size(399, 24);
            this.panelRange.TabIndex = 4;
            this.panelRange.Paint += new System.Windows.Forms.PaintEventHandler(this.panelRange_Paint);
            // 
            // checkBoxStart
            // 
            this.checkBoxStart.AutoSize = true;
            this.checkBoxStart.Location = new System.Drawing.Point(101, 2);
            this.checkBoxStart.Name = "checkBoxStart";
            this.checkBoxStart.Size = new System.Drawing.Size(48, 19);
            this.checkBoxStart.TabIndex = 1;
            this.checkBoxStart.Text = "含む";
            this.checkBoxStart.UseVisualStyleBackColor = true;
            this.checkBoxStart.CheckedChanged += new System.EventHandler(this.checkBoxStartEnd_CheckedChanged);
            // 
            // checkBoxEnd
            // 
            this.checkBoxEnd.AutoSize = true;
            this.checkBoxEnd.Location = new System.Drawing.Point(314, 3);
            this.checkBoxEnd.Name = "checkBoxEnd";
            this.checkBoxEnd.Size = new System.Drawing.Size(48, 19);
            this.checkBoxEnd.TabIndex = 3;
            this.checkBoxEnd.Text = "含む";
            this.checkBoxEnd.UseVisualStyleBackColor = true;
            this.checkBoxEnd.CheckedChanged += new System.EventHandler(this.checkBoxStartEnd_CheckedChanged);
            // 
            // maskedStart
            // 
            this.maskedStart.Location = new System.Drawing.Point(39, 0);
            this.maskedStart.Mask = "90:00:00";
            this.maskedStart.Name = "maskedStart";
            this.maskedStart.Size = new System.Drawing.Size(60, 23);
            this.maskedStart.TabIndex = 0;
            this.maskedStart.Text = "120000";
            this.maskedStart.TextChanged += new System.EventHandler(this.dateTimeStartEnd_ValueChanged);
            // 
            // maskedEnd
            // 
            this.maskedEnd.Location = new System.Drawing.Point(252, 0);
            this.maskedEnd.Mask = "90:00:00";
            this.maskedEnd.Name = "maskedEnd";
            this.maskedEnd.Size = new System.Drawing.Size(60, 23);
            this.maskedEnd.TabIndex = 2;
            this.maskedEnd.Text = "120000";
            this.maskedEnd.TextChanged += new System.EventHandler(this.dateTimeStartEnd_ValueChanged);
            // 
            // FileConditionTimeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.maskedEnd);
            this.Controls.Add(this.maskedStart);
            this.Controls.Add(this.checkBoxEnd);
            this.Controls.Add(this.checkBoxStart);
            this.Controls.Add(this.panelRange);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "FileConditionTimeControl";
            this.Size = new System.Drawing.Size(398, 48);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelRange;
        private System.Windows.Forms.CheckBox checkBoxStart;
        private System.Windows.Forms.CheckBox checkBoxEnd;
        private System.Windows.Forms.MaskedTextBox maskedStart;
        private System.Windows.Forms.MaskedTextBox maskedEnd;
    }
}
