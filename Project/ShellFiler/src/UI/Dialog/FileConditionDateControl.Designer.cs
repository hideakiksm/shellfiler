namespace ShellFiler.UI.Dialog {
    partial class FileConditionDateControl {
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
            this.dateTimeStart = new System.Windows.Forms.DateTimePicker();
            this.checkBoxStart = new System.Windows.Forms.CheckBox();
            this.dateTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.checkBoxEnd = new System.Windows.Forms.CheckBox();
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
            // dateTimeStart
            // 
            this.dateTimeStart.CustomFormat = "yyyy/MM/dd  HH:mm:ss";
            this.dateTimeStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeStart.Location = new System.Drawing.Point(3, 3);
            this.dateTimeStart.Name = "dateTimeStart";
            this.dateTimeStart.Size = new System.Drawing.Size(145, 19);
            this.dateTimeStart.TabIndex = 0;
            this.dateTimeStart.ValueChanged += new System.EventHandler(this.dateTimeStartEnd_ValueChanged);
            // 
            // checkBoxStart
            // 
            this.checkBoxStart.AutoSize = true;
            this.checkBoxStart.Location = new System.Drawing.Point(150, 5);
            this.checkBoxStart.Name = "checkBoxStart";
            this.checkBoxStart.Size = new System.Drawing.Size(46, 16);
            this.checkBoxStart.TabIndex = 1;
            this.checkBoxStart.Text = "含む";
            this.checkBoxStart.UseVisualStyleBackColor = true;
            this.checkBoxStart.CheckedChanged += new System.EventHandler(this.checkBoxStartEnd_CheckedChanged);
            // 
            // dateTimeEnd
            // 
            this.dateTimeEnd.CustomFormat = "yyyy/MM/dd  HH:mm:ss";
            this.dateTimeEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeEnd.Location = new System.Drawing.Point(206, 3);
            this.dateTimeEnd.Name = "dateTimeEnd";
            this.dateTimeEnd.Size = new System.Drawing.Size(145, 19);
            this.dateTimeEnd.TabIndex = 2;
            this.dateTimeEnd.ValueChanged += new System.EventHandler(this.dateTimeStartEnd_ValueChanged);
            // 
            // checkBoxEnd
            // 
            this.checkBoxEnd.AutoSize = true;
            this.checkBoxEnd.Location = new System.Drawing.Point(353, 5);
            this.checkBoxEnd.Name = "checkBoxEnd";
            this.checkBoxEnd.Size = new System.Drawing.Size(46, 16);
            this.checkBoxEnd.TabIndex = 3;
            this.checkBoxEnd.Text = "含む";
            this.checkBoxEnd.UseVisualStyleBackColor = true;
            this.checkBoxEnd.CheckedChanged += new System.EventHandler(this.checkBoxStartEnd_CheckedChanged);
            // 
            // FileConditionDateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.checkBoxEnd);
            this.Controls.Add(this.dateTimeEnd);
            this.Controls.Add(this.checkBoxStart);
            this.Controls.Add(this.dateTimeStart);
            this.Controls.Add(this.panelRange);
            this.Name = "FileConditionDateControl";
            this.Size = new System.Drawing.Size(398, 48);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelRange;
        private System.Windows.Forms.DateTimePicker dateTimeStart;
        private System.Windows.Forms.CheckBox checkBoxStart;
        private System.Windows.Forms.DateTimePicker dateTimeEnd;
        private System.Windows.Forms.CheckBox checkBoxEnd;
    }
}
