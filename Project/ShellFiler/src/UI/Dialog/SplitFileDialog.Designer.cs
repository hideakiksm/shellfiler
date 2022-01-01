namespace ShellFiler.UI.Dialog {
    partial class SplitFileDialog {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplitFileDialog));
            this.listViewTarget = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.pictureBoxArrow = new System.Windows.Forms.PictureBox();
            this.textBoxDestFolder = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonCD700 = new System.Windows.Forms.Button();
            this.buttonCD650 = new System.Windows.Forms.Button();
            this.buttonFD = new System.Windows.Forms.Button();
            this.comboBoxSizeUnit = new System.Windows.Forms.ComboBox();
            this.numericNum = new System.Windows.Forms.NumericUpDown();
            this.numericSize = new System.Windows.Forms.NumericUpDown();
            this.radioButtonNum = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.radioButtonSize = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxRadix = new System.Windows.Forms.ComboBox();
            this.numericIncrease = new System.Windows.Forms.NumericUpDown();
            this.comboBoxWidth = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericStart = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxSample = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.labelMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArrow)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericIncrease)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStart)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewTarget
            // 
            this.listViewTarget.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewTarget.FullRowSelect = true;
            this.listViewTarget.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewTarget.HideSelection = false;
            this.listViewTarget.Location = new System.Drawing.Point(12, 28);
            this.listViewTarget.MultiSelect = false;
            this.listViewTarget.Name = "listViewTarget";
            this.listViewTarget.Size = new System.Drawing.Size(538, 50);
            this.listViewTarget.TabIndex = 2;
            this.listViewTarget.UseCompatibleStateImageBehavior = false;
            this.listViewTarget.View = System.Windows.Forms.View.Details;
            this.listViewTarget.SelectedIndexChanged += new System.EventHandler(this.listViewTarget_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "分解対象ファイル(&S):";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(475, 409);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(394, 409);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // pictureBoxArrow
            // 
            this.pictureBoxArrow.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxArrow.Image")));
            this.pictureBoxArrow.InitialImage = null;
            this.pictureBoxArrow.Location = new System.Drawing.Point(257, 84);
            this.pictureBoxArrow.Name = "pictureBoxArrow";
            this.pictureBoxArrow.Size = new System.Drawing.Size(48, 16);
            this.pictureBoxArrow.TabIndex = 13;
            this.pictureBoxArrow.TabStop = false;
            // 
            // textBoxDestFolder
            // 
            this.textBoxDestFolder.Location = new System.Drawing.Point(121, 106);
            this.textBoxDestFolder.Name = "textBoxDestFolder";
            this.textBoxDestFolder.ReadOnly = true;
            this.textBoxDestFolder.Size = new System.Drawing.Size(429, 19);
            this.textBoxDestFolder.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "分割先フォルダ名(&F):";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonCD700);
            this.groupBox1.Controls.Add(this.buttonCD650);
            this.groupBox1.Controls.Add(this.buttonFD);
            this.groupBox1.Controls.Add(this.comboBoxSizeUnit);
            this.groupBox1.Controls.Add(this.numericNum);
            this.groupBox1.Controls.Add(this.numericSize);
            this.groupBox1.Controls.Add(this.radioButtonNum);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.radioButtonSize);
            this.groupBox1.Location = new System.Drawing.Point(12, 131);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(538, 101);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "分割条件";
            // 
            // buttonCD700
            // 
            this.buttonCD700.Location = new System.Drawing.Point(184, 67);
            this.buttonCD700.Name = "buttonCD700";
            this.buttonCD700.Size = new System.Drawing.Size(75, 23);
            this.buttonCD700.TabIndex = 5;
            this.buttonCD700.Text = "CD(&700M)";
            this.buttonCD700.UseVisualStyleBackColor = true;
            this.buttonCD700.Click += new System.EventHandler(this.buttonCD700_Click);
            // 
            // buttonCD650
            // 
            this.buttonCD650.Location = new System.Drawing.Point(103, 67);
            this.buttonCD650.Name = "buttonCD650";
            this.buttonCD650.Size = new System.Drawing.Size(75, 23);
            this.buttonCD650.TabIndex = 4;
            this.buttonCD650.Text = "CD(&650M)";
            this.buttonCD650.UseVisualStyleBackColor = true;
            this.buttonCD650.Click += new System.EventHandler(this.buttonCD650_Click);
            // 
            // buttonFD
            // 
            this.buttonFD.Location = new System.Drawing.Point(22, 67);
            this.buttonFD.Name = "buttonFD";
            this.buttonFD.Size = new System.Drawing.Size(75, 23);
            this.buttonFD.TabIndex = 3;
            this.buttonFD.Text = "FD(&1.4M)";
            this.buttonFD.UseVisualStyleBackColor = true;
            this.buttonFD.Click += new System.EventHandler(this.buttonFD_Click);
            // 
            // comboBoxSizeUnit
            // 
            this.comboBoxSizeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSizeUnit.FormattingEnabled = true;
            this.comboBoxSizeUnit.Location = new System.Drawing.Point(184, 41);
            this.comboBoxSizeUnit.Name = "comboBoxSizeUnit";
            this.comboBoxSizeUnit.Size = new System.Drawing.Size(103, 20);
            this.comboBoxSizeUnit.TabIndex = 2;
            this.comboBoxSizeUnit.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSizeUnit_SelectedIndexChanged);
            // 
            // numericNum
            // 
            this.numericNum.Location = new System.Drawing.Point(343, 41);
            this.numericNum.Name = "numericNum";
            this.numericNum.Size = new System.Drawing.Size(91, 19);
            this.numericNum.TabIndex = 7;
            this.numericNum.ValueChanged += new System.EventHandler(this.NumericSplit_ValueChanged);
            // 
            // numericSize
            // 
            this.numericSize.Location = new System.Drawing.Point(22, 42);
            this.numericSize.Name = "numericSize";
            this.numericSize.Size = new System.Drawing.Size(156, 19);
            this.numericSize.TabIndex = 1;
            this.numericSize.ValueChanged += new System.EventHandler(this.NumericSplit_ValueChanged);
            // 
            // radioButtonNum
            // 
            this.radioButtonNum.AutoSize = true;
            this.radioButtonNum.Location = new System.Drawing.Point(331, 19);
            this.radioButtonNum.Name = "radioButtonNum";
            this.radioButtonNum.Size = new System.Drawing.Size(119, 16);
            this.radioButtonNum.TabIndex = 6;
            this.radioButtonNum.TabStop = true;
            this.radioButtonNum.Text = "ファイル数で分割(&N)";
            this.radioButtonNum.UseVisualStyleBackColor = true;
            this.radioButtonNum.CheckedChanged += new System.EventHandler(this.RadioButtonSplitSize_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(440, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "個";
            // 
            // radioButtonSize
            // 
            this.radioButtonSize.AutoSize = true;
            this.radioButtonSize.Location = new System.Drawing.Point(7, 19);
            this.radioButtonSize.Name = "radioButtonSize";
            this.radioButtonSize.Size = new System.Drawing.Size(101, 16);
            this.radioButtonSize.TabIndex = 0;
            this.radioButtonSize.TabStop = true;
            this.radioButtonSize.Text = "サイズで分割(&S)";
            this.radioButtonSize.UseVisualStyleBackColor = true;
            this.radioButtonSize.CheckedChanged += new System.EventHandler(this.RadioButtonSplitSize_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(391, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(159, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "それぞれのファイルを分解します。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "基数(&R):";
            // 
            // comboBoxRadix
            // 
            this.comboBoxRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRadix.FormattingEnabled = true;
            this.comboBoxRadix.Location = new System.Drawing.Point(112, 106);
            this.comboBoxRadix.Name = "comboBoxRadix";
            this.comboBoxRadix.Size = new System.Drawing.Size(120, 20);
            this.comboBoxRadix.TabIndex = 8;
            // 
            // numericIncrease
            // 
            this.numericIncrease.Location = new System.Drawing.Point(112, 81);
            this.numericIncrease.Name = "numericIncrease";
            this.numericIncrease.Size = new System.Drawing.Size(120, 19);
            this.numericIncrease.TabIndex = 6;
            // 
            // comboBoxWidth
            // 
            this.comboBoxWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWidth.FormattingEnabled = true;
            this.comboBoxWidth.Location = new System.Drawing.Point(112, 131);
            this.comboBoxWidth.Name = "comboBoxWidth";
            this.comboBoxWidth.Size = new System.Drawing.Size(120, 20);
            this.comboBoxWidth.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "数値の桁数(&D):";
            // 
            // numericStart
            // 
            this.numericStart.Location = new System.Drawing.Point(112, 56);
            this.numericStart.Name = "numericStart";
            this.numericStart.Size = new System.Drawing.Size(120, 19);
            this.numericStart.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "増分(&I):";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 12);
            this.label8.TabIndex = 3;
            this.label8.Text = "開始番号(&S):";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(112, 14);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(120, 19);
            this.textBoxFileName.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "ファイル名後方(&F):";
            // 
            // textBoxSample
            // 
            this.textBoxSample.Location = new System.Drawing.Point(252, 29);
            this.textBoxSample.Multiline = true;
            this.textBoxSample.Name = "textBoxSample";
            this.textBoxSample.ReadOnly = true;
            this.textBoxSample.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxSample.Size = new System.Drawing.Size(280, 130);
            this.textBoxSample.TabIndex = 12;
            this.textBoxSample.WordWrap = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(250, 14);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(137, 12);
            this.label10.TabIndex = 11;
            this.label10.Text = "ファイル名20件分のサンプル:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.textBoxSample);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBoxFileName);
            this.groupBox2.Controls.Add(this.numericStart);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.comboBoxWidth);
            this.groupBox2.Controls.Add(this.comboBoxRadix);
            this.groupBox2.Controls.Add(this.numericIncrease);
            this.groupBox2.Location = new System.Drawing.Point(12, 238);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(538, 165);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ファイルの命名ルール";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(91, 36);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(141, 12);
            this.label11.TabIndex = 2;
            this.label11.Text = "「?」を数値として結合します。";
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(309, 88);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(68, 12);
            this.labelMessage.TabIndex = 3;
            this.labelMessage.Text = "---個に分割";
            // 
            // SplitFileDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(562, 444);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxDestFolder);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.pictureBoxArrow);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewTarget);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplitFileDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ファイルの分割";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArrow)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericIncrease)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStart)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewTarget;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.PictureBox pictureBoxArrow;
        private System.Windows.Forms.TextBox textBoxDestFolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonCD700;
        private System.Windows.Forms.Button buttonCD650;
        private System.Windows.Forms.Button buttonFD;
        private System.Windows.Forms.ComboBox comboBoxSizeUnit;
        private System.Windows.Forms.NumericUpDown numericSize;
        private System.Windows.Forms.RadioButton radioButtonSize;
        private System.Windows.Forms.NumericUpDown numericNum;
        private System.Windows.Forms.RadioButton radioButtonNum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxRadix;
        private System.Windows.Forms.NumericUpDown numericIncrease;
        private System.Windows.Forms.ComboBox comboBoxWidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericStart;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxSample;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label labelMessage;
    }
}