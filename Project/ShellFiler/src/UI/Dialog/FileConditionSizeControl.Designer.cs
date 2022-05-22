namespace ShellFiler.UI.Dialog {
    partial class FileConditionSizeControl {
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
            this.numericStart = new System.Windows.Forms.NumericUpDown();
            this.numericEnd = new System.Windows.Forms.NumericUpDown();
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
            this.checkBoxStart.Location = new System.Drawing.Point(149, 2);
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
            this.checkBoxEnd.Location = new System.Drawing.Point(349, 2);
            this.checkBoxEnd.Name = "checkBoxEnd";
            this.checkBoxEnd.Size = new System.Drawing.Size(48, 19);
            this.checkBoxEnd.TabIndex = 3;
            this.checkBoxEnd.Text = "含む";
            this.checkBoxEnd.UseVisualStyleBackColor = true;
            this.checkBoxEnd.CheckedChanged += new System.EventHandler(this.checkBoxStartEnd_CheckedChanged);
            // 
            // numericStart
            // 
            this.numericStart.Increment = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numericStart.Location = new System.Drawing.Point(3, 1);
            this.numericStart.Maximum = new decimal(new int[] {
            0,
            4096,
            0,
            0});
            this.numericStart.Name = "numericStart";
            this.numericStart.Size = new System.Drawing.Size(145, 23);
            this.numericStart.TabIndex = 0;
            this.numericStart.ThousandsSeparator = true;
            this.numericStart.ValueChanged += new System.EventHandler(this.numericStartEnd_ValueChanged);
            // 
            // numericEnd
            // 
            this.numericEnd.Increment = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numericEnd.Location = new System.Drawing.Point(202, 1);
            this.numericEnd.Maximum = new decimal(new int[] {
            0,
            4096,
            0,
            0});
            this.numericEnd.Name = "numericEnd";
            this.numericEnd.Size = new System.Drawing.Size(145, 23);
            this.numericEnd.TabIndex = 2;
            this.numericEnd.ThousandsSeparator = true;
            this.numericEnd.ValueChanged += new System.EventHandler(this.numericStartEnd_ValueChanged);
            // 
            // FileConditionSizeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.numericEnd);
            this.Controls.Add(this.numericStart);
            this.Controls.Add(this.checkBoxEnd);
            this.Controls.Add(this.checkBoxStart);
            this.Controls.Add(this.panelRange);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "FileConditionSizeControl";
            this.Size = new System.Drawing.Size(398, 48);
            ((System.ComponentModel.ISupportInitialize)(this.numericStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEnd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelRange;
        private System.Windows.Forms.CheckBox checkBoxStart;
        private System.Windows.Forms.CheckBox checkBoxEnd;
        private System.Windows.Forms.NumericUpDown numericStart;
        private System.Windows.Forms.NumericUpDown numericEnd;
    }
}
