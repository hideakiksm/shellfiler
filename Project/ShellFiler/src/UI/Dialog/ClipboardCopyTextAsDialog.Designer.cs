namespace ShellFiler.UI.Dialog {
    partial class ClipboardCopyNameAsDialog {
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
            this.textBoxSample = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxFullPath = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonQuoteNone = new System.Windows.Forms.RadioButton();
            this.radioButtonQuoteSpace = new System.Windows.Forms.RadioButton();
            this.radioButtonQuoteAlways = new System.Windows.Forms.RadioButton();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonSeparatorReturn = new System.Windows.Forms.RadioButton();
            this.radioButtonSeparatorComma = new System.Windows.Forms.RadioButton();
            this.radioButtonSeparatorTab = new System.Windows.Forms.RadioButton();
            this.radioButtonSeparatorSpace = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxSample
            // 
            this.textBoxSample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSample.Location = new System.Drawing.Point(0, 0);
            this.textBoxSample.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxSample.Multiline = true;
            this.textBoxSample.Name = "textBoxSample";
            this.textBoxSample.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxSample.Size = new System.Drawing.Size(584, 121);
            this.textBoxSample.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "コピー文字列(&C):";
            // 
            // checkBoxFullPath
            // 
            this.checkBoxFullPath.AutoSize = true;
            this.checkBoxFullPath.Location = new System.Drawing.Point(291, 296);
            this.checkBoxFullPath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxFullPath.Name = "checkBoxFullPath";
            this.checkBoxFullPath.Size = new System.Drawing.Size(178, 29);
            this.checkBoxFullPath.TabIndex = 4;
            this.checkBoxFullPath.Text = "フォルダ名を付加(&F)";
            this.checkBoxFullPath.UseVisualStyleBackColor = true;
            this.checkBoxFullPath.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonQuoteNone);
            this.groupBox1.Controls.Add(this.radioButtonQuoteSpace);
            this.groupBox1.Controls.Add(this.radioButtonQuoteAlways);
            this.groupBox1.Location = new System.Drawing.Point(291, 174);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(316, 112);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "引用符\" \"";
            // 
            // radioButtonQuoteNone
            // 
            this.radioButtonQuoteNone.AutoSize = true;
            this.radioButtonQuoteNone.Location = new System.Drawing.Point(10, 80);
            this.radioButtonQuoteNone.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonQuoteNone.Name = "radioButtonQuoteNone";
            this.radioButtonQuoteNone.Size = new System.Drawing.Size(88, 29);
            this.radioButtonQuoteNone.TabIndex = 2;
            this.radioButtonQuoteNone.TabStop = true;
            this.radioButtonQuoteNone.Text = "なし(&N)";
            this.radioButtonQuoteNone.UseVisualStyleBackColor = true;
            this.radioButtonQuoteNone.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonQuoteSpace
            // 
            this.radioButtonQuoteSpace.AutoSize = true;
            this.radioButtonQuoteSpace.Location = new System.Drawing.Point(10, 52);
            this.radioButtonQuoteSpace.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonQuoteSpace.Name = "radioButtonQuoteSpace";
            this.radioButtonQuoteSpace.Size = new System.Drawing.Size(204, 29);
            this.radioButtonQuoteSpace.TabIndex = 1;
            this.radioButtonQuoteSpace.TabStop = true;
            this.radioButtonQuoteSpace.Text = "空白を含む場合だけ(&S)";
            this.radioButtonQuoteSpace.UseVisualStyleBackColor = true;
            this.radioButtonQuoteSpace.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonQuoteAlways
            // 
            this.radioButtonQuoteAlways.AutoSize = true;
            this.radioButtonQuoteAlways.Location = new System.Drawing.Point(10, 26);
            this.radioButtonQuoteAlways.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonQuoteAlways.Name = "radioButtonQuoteAlways";
            this.radioButtonQuoteAlways.Size = new System.Drawing.Size(133, 29);
            this.radioButtonQuoteAlways.TabIndex = 0;
            this.radioButtonQuoteAlways.TabStop = true;
            this.radioButtonQuoteAlways.Text = "常につける(&A)";
            this.radioButtonQuoteAlways.UseVisualStyleBackColor = true;
            this.radioButtonQuoteAlways.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(374, 346);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(112, 34);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(495, 346);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(112, 34);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonSeparatorReturn);
            this.groupBox2.Controls.Add(this.radioButtonSeparatorComma);
            this.groupBox2.Controls.Add(this.radioButtonSeparatorTab);
            this.groupBox2.Controls.Add(this.radioButtonSeparatorSpace);
            this.groupBox2.Location = new System.Drawing.Point(18, 174);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(218, 142);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "セパレータ";
            // 
            // radioButtonSeparatorReturn
            // 
            this.radioButtonSeparatorReturn.AutoSize = true;
            this.radioButtonSeparatorReturn.Location = new System.Drawing.Point(10, 106);
            this.radioButtonSeparatorReturn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonSeparatorReturn.Name = "radioButtonSeparatorReturn";
            this.radioButtonSeparatorReturn.Size = new System.Drawing.Size(94, 29);
            this.radioButtonSeparatorReturn.TabIndex = 3;
            this.radioButtonSeparatorReturn.TabStop = true;
            this.radioButtonSeparatorReturn.Text = "改行(&R)";
            this.radioButtonSeparatorReturn.UseVisualStyleBackColor = true;
            this.radioButtonSeparatorReturn.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonSeparatorComma
            // 
            this.radioButtonSeparatorComma.AutoSize = true;
            this.radioButtonSeparatorComma.Location = new System.Drawing.Point(10, 80);
            this.radioButtonSeparatorComma.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonSeparatorComma.Name = "radioButtonSeparatorComma";
            this.radioButtonSeparatorComma.Size = new System.Drawing.Size(98, 29);
            this.radioButtonSeparatorComma.TabIndex = 2;
            this.radioButtonSeparatorComma.TabStop = true;
            this.radioButtonSeparatorComma.Text = "カンマ(&C)";
            this.radioButtonSeparatorComma.UseVisualStyleBackColor = true;
            this.radioButtonSeparatorComma.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonSeparatorTab
            // 
            this.radioButtonSeparatorTab.AutoSize = true;
            this.radioButtonSeparatorTab.Location = new System.Drawing.Point(10, 52);
            this.radioButtonSeparatorTab.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonSeparatorTab.Name = "radioButtonSeparatorTab";
            this.radioButtonSeparatorTab.Size = new System.Drawing.Size(83, 29);
            this.radioButtonSeparatorTab.TabIndex = 1;
            this.radioButtonSeparatorTab.TabStop = true;
            this.radioButtonSeparatorTab.Text = "タブ(&T)";
            this.radioButtonSeparatorTab.UseVisualStyleBackColor = true;
            this.radioButtonSeparatorTab.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonSeparatorSpace
            // 
            this.radioButtonSeparatorSpace.AutoSize = true;
            this.radioButtonSeparatorSpace.Location = new System.Drawing.Point(10, 26);
            this.radioButtonSeparatorSpace.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonSeparatorSpace.Name = "radioButtonSeparatorSpace";
            this.radioButtonSeparatorSpace.Size = new System.Drawing.Size(93, 29);
            this.radioButtonSeparatorSpace.TabIndex = 0;
            this.radioButtonSeparatorSpace.TabStop = true;
            this.radioButtonSeparatorSpace.Text = "空白(&P)";
            this.radioButtonSeparatorSpace.UseVisualStyleBackColor = true;
            this.radioButtonSeparatorSpace.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBoxSample);
            this.panel1.Location = new System.Drawing.Point(23, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(584, 121);
            this.panel1.TabIndex = 7;
            // 
            // ClipboardCopyNameAsDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(626, 399);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxFullPath);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClipboardCopyNameAsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "形式指定名前コピー";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSample;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxFullPath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonQuoteNone;
        private System.Windows.Forms.RadioButton radioButtonQuoteSpace;
        private System.Windows.Forms.RadioButton radioButtonQuoteAlways;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonSeparatorReturn;
        private System.Windows.Forms.RadioButton radioButtonSeparatorComma;
        private System.Windows.Forms.RadioButton radioButtonSeparatorTab;
        private System.Windows.Forms.RadioButton radioButtonSeparatorSpace;
        private System.Windows.Forms.Panel panel1;
    }
}