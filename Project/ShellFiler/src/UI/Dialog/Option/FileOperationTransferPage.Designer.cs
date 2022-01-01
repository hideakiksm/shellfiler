namespace ShellFiler.UI.Dialog.Option {
    partial class FileOperationTransferPage {
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
            this.radioButtonDeleteFix = new System.Windows.Forms.RadioButton();
            this.radioButtonDeletePrev = new System.Windows.Forms.RadioButton();
            this.checkBoxAttr = new System.Windows.Forms.CheckBox();
            this.checkBoxDirectory = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxSameAutoSSH = new System.Windows.Forms.ComboBox();
            this.comboBoxSameAutoWindows = new System.Windows.Forms.ComboBox();
            this.comboBoxSameOpr = new System.Windows.Forms.ComboBox();
            this.radioButtonSameFix = new System.Windows.Forms.RadioButton();
            this.radioButtonSamePrev = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonDeleteFix
            // 
            this.radioButtonDeleteFix.AutoSize = true;
            this.radioButtonDeleteFix.Location = new System.Drawing.Point(6, 40);
            this.radioButtonDeleteFix.Name = "radioButtonDeleteFix";
            this.radioButtonDeleteFix.Size = new System.Drawing.Size(107, 16);
            this.radioButtonDeleteFix.TabIndex = 1;
            this.radioButtonDeleteFix.TabStop = true;
            this.radioButtonDeleteFix.Text = "初期値を指定(&F)";
            this.radioButtonDeleteFix.UseVisualStyleBackColor = true;
            this.radioButtonDeleteFix.CheckedChanged += new System.EventHandler(this.RadioButtonDelete_CheckedChanged);
            // 
            // radioButtonDeletePrev
            // 
            this.radioButtonDeletePrev.AutoSize = true;
            this.radioButtonDeletePrev.Location = new System.Drawing.Point(6, 18);
            this.radioButtonDeletePrev.Name = "radioButtonDeletePrev";
            this.radioButtonDeletePrev.Size = new System.Drawing.Size(242, 16);
            this.radioButtonDeletePrev.TabIndex = 0;
            this.radioButtonDeletePrev.TabStop = true;
            this.radioButtonDeletePrev.Text = "直前に指定された設定を初期値として使用(&P)";
            this.radioButtonDeletePrev.UseVisualStyleBackColor = true;
            this.radioButtonDeletePrev.CheckedChanged += new System.EventHandler(this.RadioButtonDelete_CheckedChanged);
            // 
            // checkBoxAttr
            // 
            this.checkBoxAttr.AutoSize = true;
            this.checkBoxAttr.Location = new System.Drawing.Point(38, 84);
            this.checkBoxAttr.Name = "checkBoxAttr";
            this.checkBoxAttr.Size = new System.Drawing.Size(313, 16);
            this.checkBoxAttr.TabIndex = 3;
            this.checkBoxAttr.Text = "読み込み/システム属性のファイルを確認なしですべて削除(&A)";
            this.checkBoxAttr.UseVisualStyleBackColor = true;
            // 
            // checkBoxDirectory
            // 
            this.checkBoxDirectory.AutoSize = true;
            this.checkBoxDirectory.Location = new System.Drawing.Point(38, 62);
            this.checkBoxDirectory.Name = "checkBoxDirectory";
            this.checkBoxDirectory.Size = new System.Drawing.Size(190, 16);
            this.checkBoxDirectory.TabIndex = 2;
            this.checkBoxDirectory.Text = "フォルダを確認なしですべて削除(&D)";
            this.checkBoxDirectory.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.checkBoxAttr);
            this.groupBox1.Controls.Add(this.radioButtonDeletePrev);
            this.groupBox1.Controls.Add(this.checkBoxDirectory);
            this.groupBox1.Controls.Add(this.radioButtonDeleteFix);
            this.groupBox1.Location = new System.Drawing.Point(3, 185);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(514, 164);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ファイル削除の確認画面";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "設定をONにします。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(495, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "削除実行後に「フォルダを削除しますか？」のような確認を省略したい場合は、[初期値を指定]を選択して";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(316, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "削除開始時の確認画面で、これらの設定を初期値に使用します。";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.comboBoxSameAutoSSH);
            this.groupBox2.Controls.Add(this.comboBoxSameAutoWindows);
            this.groupBox2.Controls.Add(this.comboBoxSameOpr);
            this.groupBox2.Controls.Add(this.radioButtonSameFix);
            this.groupBox2.Controls.Add(this.radioButtonSamePrev);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(514, 163);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "同名のファイルの扱い";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(200, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "SSHでファイル名を自動変更する方法(&S):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(224, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "Windowsでファイル名を自動変更する方法(&W):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 148);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(479, 12);
            this.label7.TabIndex = 8;
            this.label7.Text = "コピーや移動で同じ名前のファイルが見つかった場合の、確認画面に表示される初期値を指定します。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(155, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "同名のファイルに対する操作(&O):";
            // 
            // comboBoxSameAutoSSH
            // 
            this.comboBoxSameAutoSSH.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSameAutoSSH.FormattingEnabled = true;
            this.comboBoxSameAutoSSH.Location = new System.Drawing.Point(265, 114);
            this.comboBoxSameAutoSSH.Name = "comboBoxSameAutoSSH";
            this.comboBoxSameAutoSSH.Size = new System.Drawing.Size(237, 20);
            this.comboBoxSameAutoSSH.TabIndex = 7;
            // 
            // comboBoxSameAutoWindows
            // 
            this.comboBoxSameAutoWindows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSameAutoWindows.FormattingEnabled = true;
            this.comboBoxSameAutoWindows.Location = new System.Drawing.Point(265, 88);
            this.comboBoxSameAutoWindows.Name = "comboBoxSameAutoWindows";
            this.comboBoxSameAutoWindows.Size = new System.Drawing.Size(237, 20);
            this.comboBoxSameAutoWindows.TabIndex = 5;
            // 
            // comboBoxSameOpr
            // 
            this.comboBoxSameOpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSameOpr.FormattingEnabled = true;
            this.comboBoxSameOpr.Location = new System.Drawing.Point(265, 62);
            this.comboBoxSameOpr.Name = "comboBoxSameOpr";
            this.comboBoxSameOpr.Size = new System.Drawing.Size(237, 20);
            this.comboBoxSameOpr.TabIndex = 3;
            // 
            // radioButtonSameFix
            // 
            this.radioButtonSameFix.AutoSize = true;
            this.radioButtonSameFix.Location = new System.Drawing.Point(6, 40);
            this.radioButtonSameFix.Name = "radioButtonSameFix";
            this.radioButtonSameFix.Size = new System.Drawing.Size(103, 16);
            this.radioButtonSameFix.TabIndex = 1;
            this.radioButtonSameFix.TabStop = true;
            this.radioButtonSameFix.Text = "初期値を指定(&I)";
            this.radioButtonSameFix.UseVisualStyleBackColor = true;
            this.radioButtonSameFix.CheckedChanged += new System.EventHandler(this.RadioButtonSame_CheckedChanged);
            // 
            // radioButtonSamePrev
            // 
            this.radioButtonSamePrev.AutoSize = true;
            this.radioButtonSamePrev.Location = new System.Drawing.Point(6, 18);
            this.radioButtonSamePrev.Name = "radioButtonSamePrev";
            this.radioButtonSamePrev.Size = new System.Drawing.Size(243, 16);
            this.radioButtonSamePrev.TabIndex = 0;
            this.radioButtonSamePrev.TabStop = true;
            this.radioButtonSamePrev.Text = "直前に指定された設定を初期値として使用(&R)";
            this.radioButtonSamePrev.UseVisualStyleBackColor = true;
            this.radioButtonSamePrev.CheckedChanged += new System.EventHandler(this.RadioButtonSame_CheckedChanged);
            // 
            // FileOperationTransferPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FileOperationTransferPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonDeleteFix;
        private System.Windows.Forms.RadioButton radioButtonDeletePrev;
        private System.Windows.Forms.CheckBox checkBoxAttr;
        private System.Windows.Forms.CheckBox checkBoxDirectory;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxSameAutoSSH;
        private System.Windows.Forms.ComboBox comboBoxSameAutoWindows;
        private System.Windows.Forms.ComboBox comboBoxSameOpr;
        private System.Windows.Forms.RadioButton radioButtonSameFix;
        private System.Windows.Forms.RadioButton radioButtonSamePrev;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;

    }
}
