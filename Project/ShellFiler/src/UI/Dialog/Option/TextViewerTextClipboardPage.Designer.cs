namespace ShellFiler.UI.Dialog.Option {
    partial class TextViewerTextClipboardPage {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxTab = new System.Windows.Forms.ComboBox();
            this.comboBoxReturn = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(283, 15);
            this.label11.TabIndex = 0;
            this.label11.Text = "形式を指定してクリップボードにコピー（テキスト）の初期値";
            // 
            // radioButtonFix
            // 
            this.radioButtonFix.AutoSize = true;
            this.radioButtonFix.Location = new System.Drawing.Point(5, 47);
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
            this.radioButtonPrev.Location = new System.Drawing.Point(5, 25);
            this.radioButtonPrev.Name = "radioButtonPrev";
            this.radioButtonPrev.Size = new System.Drawing.Size(247, 19);
            this.radioButtonPrev.TabIndex = 1;
            this.radioButtonPrev.TabStop = true;
            this.radioButtonPrev.Text = "直前に指定された設定を初期値として使用(&R)";
            this.radioButtonPrev.UseVisualStyleBackColor = true;
            this.radioButtonPrev.CheckedChanged += new System.EventHandler(this.RadioButtonPrevFix_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxTab);
            this.groupBox1.Controls.Add(this.comboBoxReturn);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(28, 73);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(489, 75);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "テキストビューアでのコピー形式";
            // 
            // comboBoxTab
            // 
            this.comboBoxTab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTab.FormattingEnabled = true;
            this.comboBoxTab.Location = new System.Drawing.Point(95, 44);
            this.comboBoxTab.Name = "comboBoxTab";
            this.comboBoxTab.Size = new System.Drawing.Size(121, 23);
            this.comboBoxTab.TabIndex = 3;
            // 
            // comboBoxReturn
            // 
            this.comboBoxReturn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxReturn.FormattingEnabled = true;
            this.comboBoxReturn.Location = new System.Drawing.Point(95, 18);
            this.comboBoxReturn.Name = "comboBoxReturn";
            this.comboBoxReturn.Size = new System.Drawing.Size(121, 23);
            this.comboBoxReturn.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "タブ文字(&T):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "改行コード(&L):";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(307, 64);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(109, 15);
            this.label13.TabIndex = 11;
            this.label13.Text = "決められた形式です。";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(307, 51);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(211, 15);
            this.label12.TabIndex = 10;
            this.label12.Text = "[クリップボードにコピー]の形式は、あらかじめ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(307, 38);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 15);
            this.label10.TabIndex = 9;
            this.label10.Text = "初期値を決めます。";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(307, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(209, 15);
            this.label9.TabIndex = 8;
            this.label9.Text = "[形式を指定してクリップボードにコピ－]での";
            // 
            // TextViewerTextClipboardPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.radioButtonFix);
            this.Controls.Add(this.radioButtonPrev);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "TextViewerTextClipboardPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RadioButton radioButtonFix;
        private System.Windows.Forms.RadioButton radioButtonPrev;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxTab;
        private System.Windows.Forms.ComboBox comboBoxReturn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;





    }
}
