namespace ShellFiler.UI.Dialog.Option {
    partial class TextViewerDumpClipboardPage {
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
            this.label11 = new System.Windows.Forms.Label();
            this.radioButtonFix = new System.Windows.Forms.RadioButton();
            this.radioButtonPrev = new System.Windows.Forms.RadioButton();
            this.textBoxSample = new System.Windows.Forms.TextBox();
            this.buttonDumpDefault = new System.Windows.Forms.Button();
            this.buttonDumpBasic = new System.Windows.Forms.Button();
            this.buttonDumpC = new System.Windows.Forms.Button();
            this.textBoxDumpPostfix = new System.Windows.Forms.TextBox();
            this.textBoxDumpSeparator = new System.Windows.Forms.TextBox();
            this.textBoxDumpPrefix = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericBase64Folding = new System.Windows.Forms.NumericUpDown();
            this.numericDumpLineWidth = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxDumpWidth = new System.Windows.Forms.ComboBox();
            this.comboBoxDumpRadix = new System.Windows.Forms.ComboBox();
            this.radioButtonBase64 = new System.Windows.Forms.RadioButton();
            this.radioButtonText = new System.Windows.Forms.RadioButton();
            this.radioButtonDump = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonView = new System.Windows.Forms.RadioButton();
            this.radioButtonScreen = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericBase64Folding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericDumpLineWidth)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(274, 15);
            this.label11.TabIndex = 0;
            this.label11.Text = "形式を指定してクリップボードにコピー（ダンプ）の初期値";
            // 
            // radioButtonFix
            // 
            this.radioButtonFix.AutoSize = true;
            this.radioButtonFix.Location = new System.Drawing.Point(5, 46);
            this.radioButtonFix.Name = "radioButtonFix";
            this.radioButtonFix.Size = new System.Drawing.Size(109, 19);
            this.radioButtonFix.TabIndex = 2;
            this.radioButtonFix.TabStop = true;
            this.radioButtonFix.Text = "初期値を指定(&I)";
            this.radioButtonFix.UseVisualStyleBackColor = true;
            this.radioButtonFix.CheckedChanged += new System.EventHandler(this.RadioButtonPrevFix_CheckedChanged);
            // 
            // radioButtonPrev
            // 
            this.radioButtonPrev.AutoSize = true;
            this.radioButtonPrev.Location = new System.Drawing.Point(5, 24);
            this.radioButtonPrev.Name = "radioButtonPrev";
            this.radioButtonPrev.Size = new System.Drawing.Size(247, 19);
            this.radioButtonPrev.TabIndex = 1;
            this.radioButtonPrev.TabStop = true;
            this.radioButtonPrev.Text = "直前に指定された設定を初期値として使用(&X)";
            this.radioButtonPrev.UseVisualStyleBackColor = true;
            this.radioButtonPrev.CheckedChanged += new System.EventHandler(this.RadioButtonPrevFix_CheckedChanged);
            // 
            // textBoxSample
            // 
            this.textBoxSample.AcceptsReturn = true;
            this.textBoxSample.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxSample.Location = new System.Drawing.Point(6, 212);
            this.textBoxSample.Multiline = true;
            this.textBoxSample.Name = "textBoxSample";
            this.textBoxSample.ReadOnly = true;
            this.textBoxSample.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxSample.Size = new System.Drawing.Size(477, 84);
            this.textBoxSample.TabIndex = 23;
            this.textBoxSample.WordWrap = false;
            // 
            // buttonDumpDefault
            // 
            this.buttonDumpDefault.Location = new System.Drawing.Point(401, 18);
            this.buttonDumpDefault.Name = "buttonDumpDefault";
            this.buttonDumpDefault.Size = new System.Drawing.Size(75, 23);
            this.buttonDumpDefault.TabIndex = 13;
            this.buttonDumpDefault.Text = "デフォルト(&D)";
            this.buttonDumpDefault.UseVisualStyleBackColor = true;
            // 
            // buttonDumpBasic
            // 
            this.buttonDumpBasic.Location = new System.Drawing.Point(401, 77);
            this.buttonDumpBasic.Name = "buttonDumpBasic";
            this.buttonDumpBasic.Size = new System.Drawing.Size(75, 23);
            this.buttonDumpBasic.TabIndex = 15;
            this.buttonDumpBasic.Text = "BASIC(&B)";
            this.buttonDumpBasic.UseVisualStyleBackColor = true;
            // 
            // buttonDumpC
            // 
            this.buttonDumpC.Location = new System.Drawing.Point(401, 47);
            this.buttonDumpC.Name = "buttonDumpC";
            this.buttonDumpC.Size = new System.Drawing.Size(75, 23);
            this.buttonDumpC.TabIndex = 14;
            this.buttonDumpC.Text = "C/Java(&C)";
            this.buttonDumpC.UseVisualStyleBackColor = true;
            // 
            // textBoxDumpPostfix
            // 
            this.textBoxDumpPostfix.Location = new System.Drawing.Point(287, 67);
            this.textBoxDumpPostfix.Name = "textBoxDumpPostfix";
            this.textBoxDumpPostfix.Size = new System.Drawing.Size(64, 23);
            this.textBoxDumpPostfix.TabIndex = 7;
            // 
            // textBoxDumpSeparator
            // 
            this.textBoxDumpSeparator.Location = new System.Drawing.Point(217, 91);
            this.textBoxDumpSeparator.Name = "textBoxDumpSeparator";
            this.textBoxDumpSeparator.Size = new System.Drawing.Size(134, 23);
            this.textBoxDumpSeparator.TabIndex = 9;
            // 
            // textBoxDumpPrefix
            // 
            this.textBoxDumpPrefix.Location = new System.Drawing.Point(217, 68);
            this.textBoxDumpPrefix.Name = "textBoxDumpPrefix";
            this.textBoxDumpPrefix.Size = new System.Drawing.Size(64, 23);
            this.textBoxDumpPrefix.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(108, 190);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 15);
            this.label6.TabIndex = 20;
            this.label6.Text = "フォールディング(&F):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(108, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "改行間隔(&L)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(108, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "セパレータ(&S)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(108, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "接頭/接尾文字(&P):";
            // 
            // numericBase64Folding
            // 
            this.numericBase64Folding.Location = new System.Drawing.Point(217, 187);
            this.numericBase64Folding.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericBase64Folding.Name = "numericBase64Folding";
            this.numericBase64Folding.Size = new System.Drawing.Size(134, 23);
            this.numericBase64Folding.TabIndex = 21;
            // 
            // numericDumpLineWidth
            // 
            this.numericDumpLineWidth.Location = new System.Drawing.Point(217, 115);
            this.numericDumpLineWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericDumpLineWidth.Name = "numericDumpLineWidth";
            this.numericDumpLineWidth.Size = new System.Drawing.Size(134, 23);
            this.numericDumpLineWidth.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(108, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "数値の桁数(&D):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(108, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "基数(&R):";
            // 
            // comboBoxDumpWidth
            // 
            this.comboBoxDumpWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDumpWidth.FormattingEnabled = true;
            this.comboBoxDumpWidth.Location = new System.Drawing.Point(217, 42);
            this.comboBoxDumpWidth.Name = "comboBoxDumpWidth";
            this.comboBoxDumpWidth.Size = new System.Drawing.Size(134, 23);
            this.comboBoxDumpWidth.TabIndex = 4;
            // 
            // comboBoxDumpRadix
            // 
            this.comboBoxDumpRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDumpRadix.FormattingEnabled = true;
            this.comboBoxDumpRadix.Location = new System.Drawing.Point(217, 18);
            this.comboBoxDumpRadix.Name = "comboBoxDumpRadix";
            this.comboBoxDumpRadix.Size = new System.Drawing.Size(134, 23);
            this.comboBoxDumpRadix.TabIndex = 2;
            // 
            // radioButtonBase64
            // 
            this.radioButtonBase64.AutoSize = true;
            this.radioButtonBase64.Location = new System.Drawing.Point(6, 188);
            this.radioButtonBase64.Name = "radioButtonBase64";
            this.radioButtonBase64.Size = new System.Drawing.Size(88, 19);
            this.radioButtonBase64.TabIndex = 19;
            this.radioButtonBase64.TabStop = true;
            this.radioButtonBase64.Text = "BASE64(&B)";
            this.radioButtonBase64.UseVisualStyleBackColor = true;
            // 
            // radioButtonText
            // 
            this.radioButtonText.AutoSize = true;
            this.radioButtonText.Location = new System.Drawing.Point(6, 135);
            this.radioButtonText.Name = "radioButtonText";
            this.radioButtonText.Size = new System.Drawing.Size(169, 19);
            this.radioButtonText.TabIndex = 16;
            this.radioButtonText.TabStop = true;
            this.radioButtonText.Text = "オリジナルファイルのテキスト(&T)";
            this.radioButtonText.UseVisualStyleBackColor = true;
            // 
            // radioButtonDump
            // 
            this.radioButtonDump.AutoSize = true;
            this.radioButtonDump.Location = new System.Drawing.Point(6, 18);
            this.radioButtonDump.Name = "radioButtonDump";
            this.radioButtonDump.Size = new System.Drawing.Size(71, 19);
            this.radioButtonDump.TabIndex = 0;
            this.radioButtonDump.TabStop = true;
            this.radioButtonDump.Text = "ダンプ(&D)";
            this.radioButtonDump.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonView);
            this.groupBox1.Controls.Add(this.radioButtonDump);
            this.groupBox1.Controls.Add(this.textBoxSample);
            this.groupBox1.Controls.Add(this.radioButtonScreen);
            this.groupBox1.Controls.Add(this.radioButtonText);
            this.groupBox1.Controls.Add(this.buttonDumpDefault);
            this.groupBox1.Controls.Add(this.radioButtonBase64);
            this.groupBox1.Controls.Add(this.buttonDumpBasic);
            this.groupBox1.Controls.Add(this.comboBoxDumpRadix);
            this.groupBox1.Controls.Add(this.buttonDumpC);
            this.groupBox1.Controls.Add(this.comboBoxDumpWidth);
            this.groupBox1.Controls.Add(this.textBoxDumpPostfix);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxDumpSeparator);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxDumpPrefix);
            this.groupBox1.Controls.Add(this.numericDumpLineWidth);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.numericBase64Folding);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(28, 68);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(489, 302);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ダンプビューアでのコピー形式";
            // 
            // radioButtonView
            // 
            this.radioButtonView.AutoSize = true;
            this.radioButtonView.Location = new System.Drawing.Point(6, 171);
            this.radioButtonView.Name = "radioButtonView";
            this.radioButtonView.Size = new System.Drawing.Size(126, 19);
            this.radioButtonView.TabIndex = 18;
            this.radioButtonView.TabStop = true;
            this.radioButtonView.Text = "ダンプビューのまま(&M)";
            this.radioButtonView.UseVisualStyleBackColor = true;
            // 
            // radioButtonScreen
            // 
            this.radioButtonScreen.AutoSize = true;
            this.radioButtonScreen.Location = new System.Drawing.Point(6, 153);
            this.radioButtonScreen.Name = "radioButtonScreen";
            this.radioButtonScreen.Size = new System.Drawing.Size(155, 19);
            this.radioButtonScreen.TabIndex = 17;
            this.radioButtonScreen.TabStop = true;
            this.radioButtonScreen.Text = "画面表記と同じテキスト(&E)";
            this.radioButtonScreen.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(357, 189);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 15);
            this.label8.TabIndex = 22;
            this.label8.Text = "(0でフォールディングなし)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(357, 118);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 15);
            this.label7.TabIndex = 12;
            this.label7.Text = "(0で改行なし)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(298, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(209, 15);
            this.label9.TabIndex = 4;
            this.label9.Text = "[形式を指定してクリップボードにコピ－]での";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(298, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 15);
            this.label10.TabIndex = 5;
            this.label10.Text = "初期値を決めます。";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(298, 42);
            this.label12.Name = "label12";
            this.label12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label12.Size = new System.Drawing.Size(211, 15);
            this.label12.TabIndex = 6;
            this.label12.Text = "[クリップボードにコピー]の形式は、あらかじめ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(298, 55);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(109, 15);
            this.label13.TabIndex = 7;
            this.label13.Text = "決められた形式です。";
            // 
            // TextViewerDumpClipboardPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.radioButtonFix);
            this.Controls.Add(this.radioButtonPrev);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "TextViewerDumpClipboardPage";
            this.Size = new System.Drawing.Size(520, 370);
            ((System.ComponentModel.ISupportInitialize)(this.numericBase64Folding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericDumpLineWidth)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RadioButton radioButtonFix;
        private System.Windows.Forms.RadioButton radioButtonPrev;
        private System.Windows.Forms.TextBox textBoxSample;
        private System.Windows.Forms.Button buttonDumpDefault;
        private System.Windows.Forms.Button buttonDumpBasic;
        private System.Windows.Forms.Button buttonDumpC;
        private System.Windows.Forms.TextBox textBoxDumpPostfix;
        private System.Windows.Forms.TextBox textBoxDumpSeparator;
        private System.Windows.Forms.TextBox textBoxDumpPrefix;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericBase64Folding;
        private System.Windows.Forms.NumericUpDown numericDumpLineWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxDumpWidth;
        private System.Windows.Forms.ComboBox comboBoxDumpRadix;
        private System.Windows.Forms.RadioButton radioButtonBase64;
        private System.Windows.Forms.RadioButton radioButtonText;
        private System.Windows.Forms.RadioButton radioButtonDump;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton radioButtonScreen;
        private System.Windows.Forms.RadioButton radioButtonView;





    }
}
