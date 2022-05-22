namespace ShellFiler.UI.Dialog.Option {
    partial class TextViewerLineBreakPage {
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
            this.textBoxTab4Ext = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericPixel = new System.Windows.Forms.NumericUpDown();
            this.numericChar = new System.Windows.Forms.NumericUpDown();
            this.radioButtonNone = new System.Windows.Forms.RadioButton();
            this.radioButtonPixel = new System.Windows.Forms.RadioButton();
            this.radioButtonChar = new System.Windows.Forms.RadioButton();
            this.numericByteCount = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButtonLinePrev = new System.Windows.Forms.RadioButton();
            this.radioButtonLineFix = new System.Windows.Forms.RadioButton();
            this.groupBoxText = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBoxDump = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericPixel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericByteCount)).BeginInit();
            this.groupBoxText.SuspendLayout();
            this.groupBoxDump.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxTab4Ext
            // 
            this.textBoxTab4Ext.Location = new System.Drawing.Point(32, 323);
            this.textBoxTab4Ext.Name = "textBoxTab4Ext";
            this.textBoxTab4Ext.Size = new System.Drawing.Size(288, 23);
            this.textBoxTab4Ext.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(326, 326);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 15);
            this.label6.TabIndex = 8;
            this.label6.Text = "空白区切り";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(30, 347);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(423, 15);
            this.label7.TabIndex = 7;
            this.label7.Text = "タブ幅は、「各行の折返し表示」の設定が「文字数指定」になっている場合だけ有効です。";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 305);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "4タブ表示するファイルの拡張子(&E):";
            // 
            // label15
            // 
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label15.Location = new System.Drawing.Point(59, 286);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(400, 3);
            this.label15.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(217, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(247, 15);
            this.label5.TabIndex = 11;
            this.label5.Text = "折返しなしでは、タブ文字が空白1文字になります。";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(217, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(268, 15);
            this.label8.TabIndex = 8;
            this.label8.Text = "ピクセル数指定では、タブ文字が空白1文字になります。";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(217, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(203, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "文字数指定では、タブ幅を指定できます。";
            // 
            // numericPixel
            // 
            this.numericPixel.Location = new System.Drawing.Point(20, 84);
            this.numericPixel.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericPixel.Name = "numericPixel";
            this.numericPixel.Size = new System.Drawing.Size(77, 23);
            this.numericPixel.TabIndex = 6;
            // 
            // numericChar
            // 
            this.numericChar.Location = new System.Drawing.Point(21, 38);
            this.numericChar.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericChar.Name = "numericChar";
            this.numericChar.Size = new System.Drawing.Size(76, 23);
            this.numericChar.TabIndex = 1;
            // 
            // radioButtonNone
            // 
            this.radioButtonNone.AutoSize = true;
            this.radioButtonNone.Location = new System.Drawing.Point(6, 109);
            this.radioButtonNone.Name = "radioButtonNone";
            this.radioButtonNone.Size = new System.Drawing.Size(104, 19);
            this.radioButtonNone.TabIndex = 10;
            this.radioButtonNone.TabStop = true;
            this.radioButtonNone.Text = "折り返しなし(&N)";
            this.radioButtonNone.UseVisualStyleBackColor = true;
            this.radioButtonNone.CheckedChanged += new System.EventHandler(this.RadioButtonLineBreak_CheckedChanged);
            // 
            // radioButtonPixel
            // 
            this.radioButtonPixel.AutoSize = true;
            this.radioButtonPixel.Location = new System.Drawing.Point(6, 63);
            this.radioButtonPixel.Name = "radioButtonPixel";
            this.radioButtonPixel.Size = new System.Drawing.Size(115, 19);
            this.radioButtonPixel.TabIndex = 5;
            this.radioButtonPixel.TabStop = true;
            this.radioButtonPixel.Text = "ピクセル数指定(&P)";
            this.radioButtonPixel.UseVisualStyleBackColor = true;
            this.radioButtonPixel.CheckedChanged += new System.EventHandler(this.RadioButtonLineBreak_CheckedChanged);
            // 
            // radioButtonChar
            // 
            this.radioButtonChar.AutoSize = true;
            this.radioButtonChar.Location = new System.Drawing.Point(6, 18);
            this.radioButtonChar.Name = "radioButtonChar";
            this.radioButtonChar.Size = new System.Drawing.Size(103, 19);
            this.radioButtonChar.TabIndex = 0;
            this.radioButtonChar.TabStop = true;
            this.radioButtonChar.Text = "文字数指定(&C)";
            this.radioButtonChar.UseVisualStyleBackColor = true;
            this.radioButtonChar.CheckedChanged += new System.EventHandler(this.RadioButtonLineBreak_CheckedChanged);
            // 
            // numericByteCount
            // 
            this.numericByteCount.Location = new System.Drawing.Point(148, 18);
            this.numericByteCount.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numericByteCount.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericByteCount.Name = "numericByteCount";
            this.numericByteCount.Size = new System.Drawing.Size(92, 23);
            this.numericByteCount.TabIndex = 1;
            this.numericByteCount.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "1行に表示するバイト数(&B):";
            // 
            // radioButtonLinePrev
            // 
            this.radioButtonLinePrev.AutoSize = true;
            this.radioButtonLinePrev.Location = new System.Drawing.Point(3, 24);
            this.radioButtonLinePrev.Name = "radioButtonLinePrev";
            this.radioButtonLinePrev.Size = new System.Drawing.Size(247, 19);
            this.radioButtonLinePrev.TabIndex = 1;
            this.radioButtonLinePrev.TabStop = true;
            this.radioButtonLinePrev.Text = "直前に指定された設定を初期値として使用(&R)";
            this.radioButtonLinePrev.UseVisualStyleBackColor = true;
            this.radioButtonLinePrev.CheckedChanged += new System.EventHandler(this.RadioButtonPrevFix_CheckedChanged);
            // 
            // radioButtonLineFix
            // 
            this.radioButtonLineFix.AutoSize = true;
            this.radioButtonLineFix.Location = new System.Drawing.Point(3, 46);
            this.radioButtonLineFix.Name = "radioButtonLineFix";
            this.radioButtonLineFix.Size = new System.Drawing.Size(109, 19);
            this.radioButtonLineFix.TabIndex = 2;
            this.radioButtonLineFix.TabStop = true;
            this.radioButtonLineFix.Text = "初期値を指定(&I)";
            this.radioButtonLineFix.UseVisualStyleBackColor = true;
            this.radioButtonLineFix.CheckedChanged += new System.EventHandler(this.RadioButtonPrevFix_CheckedChanged);
            // 
            // groupBoxText
            // 
            this.groupBoxText.Controls.Add(this.label12);
            this.groupBoxText.Controls.Add(this.label4);
            this.groupBoxText.Controls.Add(this.label13);
            this.groupBoxText.Controls.Add(this.radioButtonChar);
            this.groupBoxText.Controls.Add(this.radioButtonPixel);
            this.groupBoxText.Controls.Add(this.radioButtonNone);
            this.groupBoxText.Controls.Add(this.numericChar);
            this.groupBoxText.Controls.Add(this.numericPixel);
            this.groupBoxText.Controls.Add(this.label10);
            this.groupBoxText.Controls.Add(this.label5);
            this.groupBoxText.Controls.Add(this.label3);
            this.groupBoxText.Controls.Add(this.label9);
            this.groupBoxText.Controls.Add(this.label8);
            this.groupBoxText.Location = new System.Drawing.Point(23, 68);
            this.groupBoxText.Name = "groupBoxText";
            this.groupBoxText.Size = new System.Drawing.Size(493, 150);
            this.groupBoxText.TabIndex = 3;
            this.groupBoxText.TabStop = false;
            this.groupBoxText.Text = "テキストビューア";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(103, 88);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(102, 15);
            this.label12.TabIndex = 7;
            this.label12.Text = "ピクセル(0:画面幅)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(217, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(258, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "表示幅が半角/全角サイズの文字だけ表示できます。";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(103, 42);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(89, 15);
            this.label13.TabIndex = 2;
            this.label13.Text = "文字(0:画面幅)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(217, 128);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(149, 15);
            this.label10.TabIndex = 12;
            this.label10.Text = "すべての文字を表示できます。";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(217, 86);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(149, 15);
            this.label9.TabIndex = 9;
            this.label9.Text = "すべての文字を表示できます。";
            // 
            // groupBoxDump
            // 
            this.groupBoxDump.Controls.Add(this.label2);
            this.groupBoxDump.Controls.Add(this.numericByteCount);
            this.groupBoxDump.Location = new System.Drawing.Point(23, 224);
            this.groupBoxDump.Name = "groupBoxDump";
            this.groupBoxDump.Size = new System.Drawing.Size(493, 46);
            this.groupBoxDump.TabIndex = 4;
            this.groupBoxDump.TabStop = false;
            this.groupBoxDump.Text = "ダンプビューア";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(2, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(98, 15);
            this.label11.TabIndex = 0;
            this.label11.Text = "各行の折返し表示";
            // 
            // TextViewerLineBreakPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label11);
            this.Controls.Add(this.groupBoxDump);
            this.Controls.Add(this.groupBoxText);
            this.Controls.Add(this.radioButtonLineFix);
            this.Controls.Add(this.radioButtonLinePrev);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.textBoxTab4Ext);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "TextViewerLineBreakPage";
            this.Size = new System.Drawing.Size(520, 370);
            ((System.ComponentModel.ISupportInitialize)(this.numericPixel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericByteCount)).EndInit();
            this.groupBoxText.ResumeLayout(false);
            this.groupBoxText.PerformLayout();
            this.groupBoxDump.ResumeLayout(false);
            this.groupBoxDump.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxTab4Ext;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericPixel;
        private System.Windows.Forms.NumericUpDown numericChar;
        private System.Windows.Forms.RadioButton radioButtonNone;
        private System.Windows.Forms.RadioButton radioButtonPixel;
        private System.Windows.Forms.RadioButton radioButtonChar;
        private System.Windows.Forms.NumericUpDown numericByteCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioButtonLinePrev;
        private System.Windows.Forms.RadioButton radioButtonLineFix;
        private System.Windows.Forms.GroupBox groupBoxText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBoxDump;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;





    }
}
