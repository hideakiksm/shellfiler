namespace ShellFiler.UI.Dialog.Option {
    partial class TextViewerGeneralPage {
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericMaxSize = new System.Windows.Forms.NumericUpDown();
            this.numericMaxLine = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelMaxSize = new System.Windows.Forms.Label();
            this.labelMaxLine = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxCompare = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxLine)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "読み込めるテキストの最大サイズ(&S):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "読み込めるテキストの最大行数(&L):";
            // 
            // numericMaxSize
            // 
            this.numericMaxSize.Location = new System.Drawing.Point(200, 4);
            this.numericMaxSize.Name = "numericMaxSize";
            this.numericMaxSize.Size = new System.Drawing.Size(120, 19);
            this.numericMaxSize.TabIndex = 1;
            // 
            // numericMaxLine
            // 
            this.numericMaxLine.Location = new System.Drawing.Point(200, 29);
            this.numericMaxLine.Name = "numericMaxLine";
            this.numericMaxLine.Size = new System.Drawing.Size(120, 19);
            this.numericMaxLine.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(326, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "MB";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(326, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "行";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(251, 12);
            this.label6.TabIndex = 8;
            this.label6.Text = "設定されたサイズを超えた部分は読み込まれません。";
            // 
            // labelMaxSize
            // 
            this.labelMaxSize.AutoSize = true;
            this.labelMaxSize.Location = new System.Drawing.Point(354, 6);
            this.labelMaxSize.Name = "labelMaxSize";
            this.labelMaxSize.Size = new System.Drawing.Size(58, 12);
            this.labelMaxSize.TabIndex = 3;
            this.labelMaxSize.Text = "{0}～{1}MB";
            // 
            // labelMaxLine
            // 
            this.labelMaxLine.AutoSize = true;
            this.labelMaxLine.Location = new System.Drawing.Point(354, 31);
            this.labelMaxLine.Name = "labelMaxLine";
            this.labelMaxLine.Size = new System.Drawing.Size(41, 12);
            this.labelMaxLine.TabIndex = 7;
            this.labelMaxLine.Text = "{0}～{1}";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(26, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(397, 12);
            this.label8.TabIndex = 9;
            this.label8.Text = "間違って大きなファイルを開いたとき、読み込みに時間がかかる問題を回避できます。";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(296, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "選択範囲の比較で差分表示ツールを起動した後の登録テキスト(&S):";
            // 
            // comboBoxCompare
            // 
            this.comboBoxCompare.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCompare.FormattingEnabled = true;
            this.comboBoxCompare.Location = new System.Drawing.Point(200, 130);
            this.comboBoxCompare.Name = "comboBoxCompare";
            this.comboBoxCompare.Size = new System.Drawing.Size(195, 20);
            this.comboBoxCompare.TabIndex = 11;
            // 
            // label15
            // 
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label15.Location = new System.Drawing.Point(59, 97);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(400, 3);
            this.label15.TabIndex = 12;
            // 
            // TextViewerGeneralPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label15);
            this.Controls.Add(this.comboBoxCompare);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericMaxLine);
            this.Controls.Add(this.numericMaxSize);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelMaxLine);
            this.Controls.Add(this.labelMaxSize);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Name = "TextViewerGeneralPage";
            this.Size = new System.Drawing.Size(520, 370);
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxLine)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericMaxSize;
        private System.Windows.Forms.NumericUpDown numericMaxLine;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelMaxSize;
        private System.Windows.Forms.Label labelMaxLine;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxCompare;
        private System.Windows.Forms.Label label15;


    }
}
