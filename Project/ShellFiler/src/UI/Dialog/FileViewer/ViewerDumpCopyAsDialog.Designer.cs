namespace ShellFiler.UI.Dialog.FileViewer {
    partial class ViewerDumpCopyAsDialog {
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
            this.radioButtonDump = new System.Windows.Forms.RadioButton();
            this.radioButtonText = new System.Windows.Forms.RadioButton();
            this.radioButtonBase64 = new System.Windows.Forms.RadioButton();
            this.comboBoxDumpRadix = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDumpPrefix = new System.Windows.Forms.TextBox();
            this.textBoxDumpPostfix = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxDumpSeparator = new System.Windows.Forms.TextBox();
            this.numericDumpLineWidth = new System.Windows.Forms.NumericUpDown();
            this.buttonDumpC = new System.Windows.Forms.Button();
            this.buttonDumpBasic = new System.Windows.Forms.Button();
            this.buttonDumpDefault = new System.Windows.Forms.Button();
            this.numericBase64Folding = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxSample = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.comboBoxDumpWidth = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.radioButtonScreen = new System.Windows.Forms.RadioButton();
            this.radioButtonView = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.numericDumpLineWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBase64Folding)).BeginInit();
            this.SuspendLayout();
            // 
            // radioButtonDump
            // 
            this.radioButtonDump.AutoSize = true;
            this.radioButtonDump.Location = new System.Drawing.Point(12, 10);
            this.radioButtonDump.Name = "radioButtonDump";
            this.radioButtonDump.Size = new System.Drawing.Size(71, 19);
            this.radioButtonDump.TabIndex = 0;
            this.radioButtonDump.TabStop = true;
            this.radioButtonDump.Text = "ダンプ(&D)";
            this.radioButtonDump.UseVisualStyleBackColor = true;
            // 
            // radioButtonText
            // 
            this.radioButtonText.AutoSize = true;
            this.radioButtonText.Location = new System.Drawing.Point(12, 164);
            this.radioButtonText.Name = "radioButtonText";
            this.radioButtonText.Size = new System.Drawing.Size(169, 19);
            this.radioButtonText.TabIndex = 16;
            this.radioButtonText.TabStop = true;
            this.radioButtonText.Text = "オリジナルファイルのテキスト(&T)";
            this.radioButtonText.UseVisualStyleBackColor = true;
            // 
            // radioButtonBase64
            // 
            this.radioButtonBase64.AutoSize = true;
            this.radioButtonBase64.Location = new System.Drawing.Point(13, 230);
            this.radioButtonBase64.Name = "radioButtonBase64";
            this.radioButtonBase64.Size = new System.Drawing.Size(88, 19);
            this.radioButtonBase64.TabIndex = 19;
            this.radioButtonBase64.TabStop = true;
            this.radioButtonBase64.Text = "BASE64(&B)";
            this.radioButtonBase64.UseVisualStyleBackColor = true;
            // 
            // comboBoxDumpRadix
            // 
            this.comboBoxDumpRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDumpRadix.FormattingEnabled = true;
            this.comboBoxDumpRadix.Location = new System.Drawing.Point(136, 28);
            this.comboBoxDumpRadix.Name = "comboBoxDumpRadix";
            this.comboBoxDumpRadix.Size = new System.Drawing.Size(134, 23);
            this.comboBoxDumpRadix.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "基数(&R):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "数値の桁数(&D):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "接頭/接尾文字(&P):";
            // 
            // textBoxDumpPrefix
            // 
            this.textBoxDumpPrefix.Location = new System.Drawing.Point(136, 82);
            this.textBoxDumpPrefix.Name = "textBoxDumpPrefix";
            this.textBoxDumpPrefix.Size = new System.Drawing.Size(64, 23);
            this.textBoxDumpPrefix.TabIndex = 6;
            // 
            // textBoxDumpPostfix
            // 
            this.textBoxDumpPostfix.Location = new System.Drawing.Point(206, 83);
            this.textBoxDumpPostfix.Name = "textBoxDumpPostfix";
            this.textBoxDumpPostfix.Size = new System.Drawing.Size(64, 23);
            this.textBoxDumpPostfix.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "セパレータ(&S)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "改行間隔(&L)";
            // 
            // textBoxDumpSeparator
            // 
            this.textBoxDumpSeparator.Location = new System.Drawing.Point(136, 108);
            this.textBoxDumpSeparator.Name = "textBoxDumpSeparator";
            this.textBoxDumpSeparator.Size = new System.Drawing.Size(134, 23);
            this.textBoxDumpSeparator.TabIndex = 9;
            // 
            // numericDumpLineWidth
            // 
            this.numericDumpLineWidth.Location = new System.Drawing.Point(136, 135);
            this.numericDumpLineWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericDumpLineWidth.Name = "numericDumpLineWidth";
            this.numericDumpLineWidth.Size = new System.Drawing.Size(134, 23);
            this.numericDumpLineWidth.TabIndex = 11;
            // 
            // buttonDumpC
            // 
            this.buttonDumpC.Location = new System.Drawing.Point(293, 56);
            this.buttonDumpC.Name = "buttonDumpC";
            this.buttonDumpC.Size = new System.Drawing.Size(75, 23);
            this.buttonDumpC.TabIndex = 14;
            this.buttonDumpC.Text = "C/Java(&C)";
            this.buttonDumpC.UseVisualStyleBackColor = true;
            // 
            // buttonDumpBasic
            // 
            this.buttonDumpBasic.Location = new System.Drawing.Point(293, 84);
            this.buttonDumpBasic.Name = "buttonDumpBasic";
            this.buttonDumpBasic.Size = new System.Drawing.Size(75, 23);
            this.buttonDumpBasic.TabIndex = 15;
            this.buttonDumpBasic.Text = "BASIC(&B)";
            this.buttonDumpBasic.UseVisualStyleBackColor = true;
            // 
            // buttonDumpDefault
            // 
            this.buttonDumpDefault.Location = new System.Drawing.Point(293, 28);
            this.buttonDumpDefault.Name = "buttonDumpDefault";
            this.buttonDumpDefault.Size = new System.Drawing.Size(75, 23);
            this.buttonDumpDefault.TabIndex = 13;
            this.buttonDumpDefault.Text = "デフォルト(&D)";
            this.buttonDumpDefault.UseVisualStyleBackColor = true;
            // 
            // numericBase64Folding
            // 
            this.numericBase64Folding.Location = new System.Drawing.Point(137, 249);
            this.numericBase64Folding.Name = "numericBase64Folding";
            this.numericBase64Folding.Size = new System.Drawing.Size(134, 23);
            this.numericBase64Folding.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 252);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 15);
            this.label6.TabIndex = 20;
            this.label6.Text = "フォールディング(&F):";
            // 
            // textBoxSample
            // 
            this.textBoxSample.AcceptsReturn = true;
            this.textBoxSample.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxSample.Location = new System.Drawing.Point(13, 288);
            this.textBoxSample.Multiline = true;
            this.textBoxSample.Name = "textBoxSample";
            this.textBoxSample.ReadOnly = true;
            this.textBoxSample.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxSample.Size = new System.Drawing.Size(509, 118);
            this.textBoxSample.TabIndex = 23;
            this.textBoxSample.WordWrap = false;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(366, 412);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 24;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(447, 412);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 25;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // comboBoxDumpWidth
            // 
            this.comboBoxDumpWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDumpWidth.FormattingEnabled = true;
            this.comboBoxDumpWidth.Location = new System.Drawing.Point(136, 55);
            this.comboBoxDumpWidth.Name = "comboBoxDumpWidth";
            this.comboBoxDumpWidth.Size = new System.Drawing.Size(134, 23);
            this.comboBoxDumpWidth.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(277, 251);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 15);
            this.label8.TabIndex = 22;
            this.label8.Text = "(0でフォールディングなし)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(276, 138);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 15);
            this.label7.TabIndex = 12;
            this.label7.Text = "(0で改行なし)";
            // 
            // radioButtonScreen
            // 
            this.radioButtonScreen.AutoSize = true;
            this.radioButtonScreen.Location = new System.Drawing.Point(12, 186);
            this.radioButtonScreen.Name = "radioButtonScreen";
            this.radioButtonScreen.Size = new System.Drawing.Size(155, 19);
            this.radioButtonScreen.TabIndex = 17;
            this.radioButtonScreen.TabStop = true;
            this.radioButtonScreen.Text = "画面表記と同じテキスト(&E)";
            this.radioButtonScreen.UseVisualStyleBackColor = true;
            // 
            // radioButtonView
            // 
            this.radioButtonView.AutoSize = true;
            this.radioButtonView.Location = new System.Drawing.Point(12, 208);
            this.radioButtonView.Name = "radioButtonView";
            this.radioButtonView.Size = new System.Drawing.Size(126, 19);
            this.radioButtonView.TabIndex = 18;
            this.radioButtonView.TabStop = true;
            this.radioButtonView.Text = "ダンプビューのまま(&M)";
            this.radioButtonView.UseVisualStyleBackColor = true;
            // 
            // ViewerDumpCopyAsDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(534, 447);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxSample);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonDumpDefault);
            this.Controls.Add(this.buttonDumpBasic);
            this.Controls.Add(this.buttonDumpC);
            this.Controls.Add(this.textBoxDumpPostfix);
            this.Controls.Add(this.textBoxDumpSeparator);
            this.Controls.Add(this.textBoxDumpPrefix);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericBase64Folding);
            this.Controls.Add(this.numericDumpLineWidth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxDumpWidth);
            this.Controls.Add(this.comboBoxDumpRadix);
            this.Controls.Add(this.radioButtonBase64);
            this.Controls.Add(this.radioButtonView);
            this.Controls.Add(this.radioButtonScreen);
            this.Controls.Add(this.radioButtonText);
            this.Controls.Add(this.radioButtonDump);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewerDumpCopyAsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "形式を指定してコピー";
            ((System.ComponentModel.ISupportInitialize)(this.numericDumpLineWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBase64Folding)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonDump;
        private System.Windows.Forms.RadioButton radioButtonText;
        private System.Windows.Forms.RadioButton radioButtonBase64;
        private System.Windows.Forms.ComboBox comboBoxDumpRadix;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDumpPrefix;
        private System.Windows.Forms.TextBox textBoxDumpPostfix;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxDumpSeparator;
        private System.Windows.Forms.NumericUpDown numericDumpLineWidth;
        private System.Windows.Forms.Button buttonDumpC;
        private System.Windows.Forms.Button buttonDumpBasic;
        private System.Windows.Forms.Button buttonDumpDefault;
        private System.Windows.Forms.NumericUpDown numericBase64Folding;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxSample;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxDumpWidth;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton radioButtonScreen;
        private System.Windows.Forms.RadioButton radioButtonView;
    }
}