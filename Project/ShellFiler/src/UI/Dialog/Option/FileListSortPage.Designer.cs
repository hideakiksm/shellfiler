namespace ShellFiler.UI.Dialog.Option {
    partial class FileListSortPage {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxLeftPrimary = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxLeftSecondary = new System.Windows.Forms.ComboBox();
            this.checkBoxLeftPrimaryRev = new System.Windows.Forms.CheckBox();
            this.checkBoxLeftSecondaryRev = new System.Windows.Forms.CheckBox();
            this.checkBoxLeftCapital = new System.Windows.Forms.CheckBox();
            this.checkBoxLeftDirTop = new System.Windows.Forms.CheckBox();
            this.checkBoxLeftNumber = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxRightNumber = new System.Windows.Forms.CheckBox();
            this.checkBoxRightDirTop = new System.Windows.Forms.CheckBox();
            this.checkBoxRightCapital = new System.Windows.Forms.CheckBox();
            this.checkBoxRightSecondaryRev = new System.Windows.Forms.CheckBox();
            this.checkBoxRightPrimaryRev = new System.Windows.Forms.CheckBox();
            this.comboBoxRightSecondary = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxRightPrimary = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.radioButtonLeftPrev = new System.Windows.Forms.RadioButton();
            this.radioButtonLeftFix = new System.Windows.Forms.RadioButton();
            this.radioButtonRightPrev = new System.Windows.Forms.RadioButton();
            this.radioButtonRightFix = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonLeftFix);
            this.groupBox1.Controls.Add(this.radioButtonLeftPrev);
            this.groupBox1.Controls.Add(this.checkBoxLeftNumber);
            this.groupBox1.Controls.Add(this.checkBoxLeftDirTop);
            this.groupBox1.Controls.Add(this.checkBoxLeftCapital);
            this.groupBox1.Controls.Add(this.checkBoxLeftSecondaryRev);
            this.groupBox1.Controls.Add(this.checkBoxLeftPrimaryRev);
            this.groupBox1.Controls.Add(this.comboBoxLeftSecondary);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboBoxLeftPrimary);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(513, 137);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "左ウィンドウのソート方法";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "最優先のキー(&P):";
            // 
            // comboBoxLeftPrimary
            // 
            this.comboBoxLeftPrimary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLeftPrimary.FormattingEnabled = true;
            this.comboBoxLeftPrimary.Location = new System.Drawing.Point(117, 62);
            this.comboBoxLeftPrimary.Name = "comboBoxLeftPrimary";
            this.comboBoxLeftPrimary.Size = new System.Drawing.Size(121, 20);
            this.comboBoxLeftPrimary.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "同一時のキー(&S):";
            // 
            // comboBoxLeftSecondary
            // 
            this.comboBoxLeftSecondary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLeftSecondary.FormattingEnabled = true;
            this.comboBoxLeftSecondary.Location = new System.Drawing.Point(117, 88);
            this.comboBoxLeftSecondary.Name = "comboBoxLeftSecondary";
            this.comboBoxLeftSecondary.Size = new System.Drawing.Size(121, 20);
            this.comboBoxLeftSecondary.TabIndex = 6;
            // 
            // checkBoxLeftPrimaryRev
            // 
            this.checkBoxLeftPrimaryRev.AutoSize = true;
            this.checkBoxLeftPrimaryRev.Location = new System.Drawing.Point(245, 65);
            this.checkBoxLeftPrimaryRev.Name = "checkBoxLeftPrimaryRev";
            this.checkBoxLeftPrimaryRev.Size = new System.Drawing.Size(63, 16);
            this.checkBoxLeftPrimaryRev.TabIndex = 4;
            this.checkBoxLeftPrimaryRev.Text = "逆順(&E)";
            this.checkBoxLeftPrimaryRev.UseVisualStyleBackColor = true;
            // 
            // checkBoxLeftSecondaryRev
            // 
            this.checkBoxLeftSecondaryRev.AutoSize = true;
            this.checkBoxLeftSecondaryRev.Location = new System.Drawing.Point(244, 90);
            this.checkBoxLeftSecondaryRev.Name = "checkBoxLeftSecondaryRev";
            this.checkBoxLeftSecondaryRev.Size = new System.Drawing.Size(64, 16);
            this.checkBoxLeftSecondaryRev.TabIndex = 7;
            this.checkBoxLeftSecondaryRev.Text = "逆順(&V)";
            this.checkBoxLeftSecondaryRev.UseVisualStyleBackColor = true;
            // 
            // checkBoxLeftCapital
            // 
            this.checkBoxLeftCapital.AutoSize = true;
            this.checkBoxLeftCapital.Location = new System.Drawing.Point(329, 66);
            this.checkBoxLeftCapital.Name = "checkBoxLeftCapital";
            this.checkBoxLeftCapital.Size = new System.Drawing.Size(139, 16);
            this.checkBoxLeftCapital.TabIndex = 8;
            this.checkBoxLeftCapital.Text = "英大・小文字を区別(&C)";
            this.checkBoxLeftCapital.UseVisualStyleBackColor = true;
            // 
            // checkBoxLeftDirTop
            // 
            this.checkBoxLeftDirTop.AutoSize = true;
            this.checkBoxLeftDirTop.Location = new System.Drawing.Point(329, 87);
            this.checkBoxLeftDirTop.Name = "checkBoxLeftDirTop";
            this.checkBoxLeftDirTop.Size = new System.Drawing.Size(119, 16);
            this.checkBoxLeftDirTop.TabIndex = 9;
            this.checkBoxLeftDirTop.Text = "フォルダは先頭へ(&D)";
            this.checkBoxLeftDirTop.UseVisualStyleBackColor = true;
            // 
            // checkBoxLeftNumber
            // 
            this.checkBoxLeftNumber.AutoSize = true;
            this.checkBoxLeftNumber.Location = new System.Drawing.Point(329, 109);
            this.checkBoxLeftNumber.Name = "checkBoxLeftNumber";
            this.checkBoxLeftNumber.Size = new System.Drawing.Size(178, 16);
            this.checkBoxLeftNumber.TabIndex = 10;
            this.checkBoxLeftNumber.Text = "数値部分は数の大小で比較(&R)";
            this.checkBoxLeftNumber.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonRightFix);
            this.groupBox2.Controls.Add(this.radioButtonRightPrev);
            this.groupBox2.Controls.Add(this.checkBoxRightNumber);
            this.groupBox2.Controls.Add(this.checkBoxRightDirTop);
            this.groupBox2.Controls.Add(this.checkBoxRightCapital);
            this.groupBox2.Controls.Add(this.checkBoxRightSecondaryRev);
            this.groupBox2.Controls.Add(this.checkBoxRightPrimaryRev);
            this.groupBox2.Controls.Add(this.comboBoxRightSecondary);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.comboBoxRightPrimary);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(4, 151);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(513, 138);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "右ウィンドウのソート方法";
            // 
            // checkBoxRightNumber
            // 
            this.checkBoxRightNumber.AutoSize = true;
            this.checkBoxRightNumber.Location = new System.Drawing.Point(329, 109);
            this.checkBoxRightNumber.Name = "checkBoxRightNumber";
            this.checkBoxRightNumber.Size = new System.Drawing.Size(178, 16);
            this.checkBoxRightNumber.TabIndex = 10;
            this.checkBoxRightNumber.Text = "数値部分は数の大小で比較(&R)";
            this.checkBoxRightNumber.UseVisualStyleBackColor = true;
            // 
            // checkBoxRightDirTop
            // 
            this.checkBoxRightDirTop.AutoSize = true;
            this.checkBoxRightDirTop.Location = new System.Drawing.Point(329, 87);
            this.checkBoxRightDirTop.Name = "checkBoxRightDirTop";
            this.checkBoxRightDirTop.Size = new System.Drawing.Size(119, 16);
            this.checkBoxRightDirTop.TabIndex = 9;
            this.checkBoxRightDirTop.Text = "フォルダは先頭へ(&D)";
            this.checkBoxRightDirTop.UseVisualStyleBackColor = true;
            // 
            // checkBoxRightCapital
            // 
            this.checkBoxRightCapital.AutoSize = true;
            this.checkBoxRightCapital.Location = new System.Drawing.Point(329, 66);
            this.checkBoxRightCapital.Name = "checkBoxRightCapital";
            this.checkBoxRightCapital.Size = new System.Drawing.Size(139, 16);
            this.checkBoxRightCapital.TabIndex = 8;
            this.checkBoxRightCapital.Text = "英大・小文字を区別(&C)";
            this.checkBoxRightCapital.UseVisualStyleBackColor = true;
            // 
            // checkBoxRightSecondaryRev
            // 
            this.checkBoxRightSecondaryRev.AutoSize = true;
            this.checkBoxRightSecondaryRev.Location = new System.Drawing.Point(244, 90);
            this.checkBoxRightSecondaryRev.Name = "checkBoxRightSecondaryRev";
            this.checkBoxRightSecondaryRev.Size = new System.Drawing.Size(64, 16);
            this.checkBoxRightSecondaryRev.TabIndex = 7;
            this.checkBoxRightSecondaryRev.Text = "逆順(&V)";
            this.checkBoxRightSecondaryRev.UseVisualStyleBackColor = true;
            // 
            // checkBoxRightPrimaryRev
            // 
            this.checkBoxRightPrimaryRev.AutoSize = true;
            this.checkBoxRightPrimaryRev.Location = new System.Drawing.Point(245, 65);
            this.checkBoxRightPrimaryRev.Name = "checkBoxRightPrimaryRev";
            this.checkBoxRightPrimaryRev.Size = new System.Drawing.Size(63, 16);
            this.checkBoxRightPrimaryRev.TabIndex = 4;
            this.checkBoxRightPrimaryRev.Text = "逆順(&E)";
            this.checkBoxRightPrimaryRev.UseVisualStyleBackColor = true;
            // 
            // comboBoxRightSecondary
            // 
            this.comboBoxRightSecondary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRightSecondary.FormattingEnabled = true;
            this.comboBoxRightSecondary.Location = new System.Drawing.Point(117, 88);
            this.comboBoxRightSecondary.Name = "comboBoxRightSecondary";
            this.comboBoxRightSecondary.Size = new System.Drawing.Size(121, 20);
            this.comboBoxRightSecondary.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "同一時のキー(&S):";
            // 
            // comboBoxRightPrimary
            // 
            this.comboBoxRightPrimary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRightPrimary.FormattingEnabled = true;
            this.comboBoxRightPrimary.Location = new System.Drawing.Point(117, 62);
            this.comboBoxRightPrimary.Name = "comboBoxRightPrimary";
            this.comboBoxRightPrimary.Size = new System.Drawing.Size(121, 20);
            this.comboBoxRightPrimary.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "最優先のキー(&P):";
            // 
            // radioButtonLeftPrev
            // 
            this.radioButtonLeftPrev.AutoSize = true;
            this.radioButtonLeftPrev.Location = new System.Drawing.Point(6, 18);
            this.radioButtonLeftPrev.Name = "radioButtonLeftPrev";
            this.radioButtonLeftPrev.Size = new System.Drawing.Size(131, 16);
            this.radioButtonLeftPrev.TabIndex = 0;
            this.radioButtonLeftPrev.TabStop = true;
            this.radioButtonLeftPrev.Text = "前回終了時の状態(&L)";
            this.radioButtonLeftPrev.UseVisualStyleBackColor = true;
            // 
            // radioButtonLeftFix
            // 
            this.radioButtonLeftFix.AutoSize = true;
            this.radioButtonLeftFix.Location = new System.Drawing.Point(6, 40);
            this.radioButtonLeftFix.Name = "radioButtonLeftFix";
            this.radioButtonLeftFix.Size = new System.Drawing.Size(144, 16);
            this.radioButtonLeftFix.TabIndex = 1;
            this.radioButtonLeftFix.TabStop = true;
            this.radioButtonLeftFix.Text = "一覧の状態を指定する(&I)";
            this.radioButtonLeftFix.UseVisualStyleBackColor = true;
            // 
            // radioButtonRightPrev
            // 
            this.radioButtonRightPrev.AutoSize = true;
            this.radioButtonRightPrev.Location = new System.Drawing.Point(6, 18);
            this.radioButtonRightPrev.Name = "radioButtonRightPrev";
            this.radioButtonRightPrev.Size = new System.Drawing.Size(131, 16);
            this.radioButtonRightPrev.TabIndex = 0;
            this.radioButtonRightPrev.TabStop = true;
            this.radioButtonRightPrev.Text = "前回終了時の状態(&L)";
            this.radioButtonRightPrev.UseVisualStyleBackColor = true;
            // 
            // radioButtonRightFix
            // 
            this.radioButtonRightFix.AutoSize = true;
            this.radioButtonRightFix.Location = new System.Drawing.Point(6, 40);
            this.radioButtonRightFix.Name = "radioButtonRightFix";
            this.radioButtonRightFix.Size = new System.Drawing.Size(144, 16);
            this.radioButtonRightFix.TabIndex = 1;
            this.radioButtonRightFix.TabStop = true;
            this.radioButtonRightFix.Text = "一覧の状態を指定する(&I)";
            this.radioButtonRightFix.UseVisualStyleBackColor = true;
            // 
            // FileListSortPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FileListSortPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxLeftSecondaryRev;
        private System.Windows.Forms.CheckBox checkBoxLeftPrimaryRev;
        private System.Windows.Forms.ComboBox comboBoxLeftSecondary;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxLeftPrimary;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxLeftNumber;
        private System.Windows.Forms.CheckBox checkBoxLeftDirTop;
        private System.Windows.Forms.CheckBox checkBoxLeftCapital;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxRightNumber;
        private System.Windows.Forms.CheckBox checkBoxRightDirTop;
        private System.Windows.Forms.CheckBox checkBoxRightCapital;
        private System.Windows.Forms.CheckBox checkBoxRightSecondaryRev;
        private System.Windows.Forms.CheckBox checkBoxRightPrimaryRev;
        private System.Windows.Forms.ComboBox comboBoxRightSecondary;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxRightPrimary;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButtonLeftFix;
        private System.Windows.Forms.RadioButton radioButtonLeftPrev;
        private System.Windows.Forms.RadioButton radioButtonRightFix;
        private System.Windows.Forms.RadioButton radioButtonRightPrev;

    }
}
