namespace ShellFiler.UI.Dialog {
    partial class FileConditionRelativeDateControl {
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
            this.maskedStart = new System.Windows.Forms.MaskedTextBox();
            this.numericStart = new System.Windows.Forms.NumericUpDown();
            this.labelStart = new System.Windows.Forms.Label();
            this.checkBoxEnd = new System.Windows.Forms.CheckBox();
            this.maskedEnd = new System.Windows.Forms.MaskedTextBox();
            this.numericEnd = new System.Windows.Forms.NumericUpDown();
            this.labelEnd = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEnd)).BeginInit();
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
            this.checkBoxStart.Location = new System.Drawing.Point(136, 5);
            this.checkBoxStart.Name = "checkBoxStart";
            this.checkBoxStart.Size = new System.Drawing.Size(46, 16);
            this.checkBoxStart.TabIndex = 1;
            this.checkBoxStart.Text = "含む";
            this.checkBoxStart.UseVisualStyleBackColor = true;
            this.checkBoxStart.CheckedChanged += new System.EventHandler(this.checkBoxStartEnd_CheckedChanged);
            // 
            // maskedStart
            // 
            this.maskedStart.Location = new System.Drawing.Point(83, 3);
            this.maskedStart.Mask = "90:00:00";
            this.maskedStart.Name = "maskedStart";
            this.maskedStart.Size = new System.Drawing.Size(51, 19);
            this.maskedStart.TabIndex = 5;
            this.maskedStart.Text = "120000";
            this.maskedStart.TextChanged += new System.EventHandler(this.dateTimeStartEnd_ValueChanged);
            // 
            // numericStart
            // 
            this.numericStart.Location = new System.Drawing.Point(10, 3);
            this.numericStart.Name = "numericStart";
            this.numericStart.Size = new System.Drawing.Size(40, 19);
            this.numericStart.TabIndex = 7;
            this.numericStart.ValueChanged += new System.EventHandler(this.dateTimeStartEnd_ValueChanged);
            // 
            // labelStart
            // 
            this.labelStart.AutoSize = true;
            this.labelStart.Location = new System.Drawing.Point(52, 6);
            this.labelStart.Name = "labelStart";
            this.labelStart.Size = new System.Drawing.Size(29, 12);
            this.labelStart.TabIndex = 8;
            this.labelStart.Text = "日前";
            // 
            // checkBoxEnd
            // 
            this.checkBoxEnd.AutoSize = true;
            this.checkBoxEnd.Location = new System.Drawing.Point(342, 5);
            this.checkBoxEnd.Name = "checkBoxEnd";
            this.checkBoxEnd.Size = new System.Drawing.Size(46, 16);
            this.checkBoxEnd.TabIndex = 1;
            this.checkBoxEnd.Text = "含む";
            this.checkBoxEnd.UseVisualStyleBackColor = true;
            this.checkBoxEnd.CheckedChanged += new System.EventHandler(this.checkBoxStartEnd_CheckedChanged);
            // 
            // maskedEnd
            // 
            this.maskedEnd.Location = new System.Drawing.Point(289, 3);
            this.maskedEnd.Mask = "90:00:00";
            this.maskedEnd.Name = "maskedEnd";
            this.maskedEnd.Size = new System.Drawing.Size(51, 19);
            this.maskedEnd.TabIndex = 5;
            this.maskedEnd.Text = "120000";
            this.maskedEnd.TextChanged += new System.EventHandler(this.dateTimeStartEnd_ValueChanged);
            // 
            // numericEnd
            // 
            this.numericEnd.Location = new System.Drawing.Point(216, 3);
            this.numericEnd.Name = "numericEnd";
            this.numericEnd.Size = new System.Drawing.Size(40, 19);
            this.numericEnd.TabIndex = 7;
            this.numericEnd.ValueChanged += new System.EventHandler(this.dateTimeStartEnd_ValueChanged);
            // 
            // labelEnd
            // 
            this.labelEnd.AutoSize = true;
            this.labelEnd.Location = new System.Drawing.Point(258, 6);
            this.labelEnd.Name = "labelEnd";
            this.labelEnd.Size = new System.Drawing.Size(29, 12);
            this.labelEnd.TabIndex = 8;
            this.labelEnd.Text = "日前";
            // 
            // FileConditionRelativeDateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.labelEnd);
            this.Controls.Add(this.labelStart);
            this.Controls.Add(this.numericEnd);
            this.Controls.Add(this.maskedEnd);
            this.Controls.Add(this.numericStart);
            this.Controls.Add(this.checkBoxEnd);
            this.Controls.Add(this.maskedStart);
            this.Controls.Add(this.checkBoxStart);
            this.Controls.Add(this.panelRange);
            this.Name = "FileConditionRelativeDateControl";
            this.Size = new System.Drawing.Size(398, 48);
            ((System.ComponentModel.ISupportInitialize)(this.numericStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEnd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelRange;
        private System.Windows.Forms.CheckBox checkBoxStart;
        private System.Windows.Forms.MaskedTextBox maskedStart;
        private System.Windows.Forms.NumericUpDown numericStart;
        private System.Windows.Forms.Label labelStart;
        private System.Windows.Forms.CheckBox checkBoxEnd;
        private System.Windows.Forms.MaskedTextBox maskedEnd;
        private System.Windows.Forms.NumericUpDown numericEnd;
        private System.Windows.Forms.Label labelEnd;
    }
}
