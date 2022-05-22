namespace ShellFiler.UI.Dialog.Option {
    partial class FileListComparePage {
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
            this.radioButtonFix = new System.Windows.Forms.RadioButton();
            this.radioButtonPrev = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonTimeIgnore = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeOld = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeNew = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeSame = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonSizeIgnore = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeSmall = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeBig = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeSame = new System.Windows.Forms.RadioButton();
            this.checkBoxExceptFolder = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonExactly = new System.Windows.Forms.Button();
            this.buttonNameOnly = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "ファイル一覧の比較";
            // 
            // radioButtonFix
            // 
            this.radioButtonFix.AutoSize = true;
            this.radioButtonFix.Location = new System.Drawing.Point(3, 47);
            this.radioButtonFix.Name = "radioButtonFix";
            this.radioButtonFix.Size = new System.Drawing.Size(109, 19);
            this.radioButtonFix.TabIndex = 2;
            this.radioButtonFix.TabStop = true;
            this.radioButtonFix.Text = "初期値を指定(&I)";
            this.radioButtonFix.UseVisualStyleBackColor = true;
            // 
            // radioButtonPrev
            // 
            this.radioButtonPrev.AutoSize = true;
            this.radioButtonPrev.Location = new System.Drawing.Point(3, 25);
            this.radioButtonPrev.Name = "radioButtonPrev";
            this.radioButtonPrev.Size = new System.Drawing.Size(247, 19);
            this.radioButtonPrev.TabIndex = 1;
            this.radioButtonPrev.TabStop = true;
            this.radioButtonPrev.Text = "直前に指定された設定を初期値として使用(&R)";
            this.radioButtonPrev.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonTimeIgnore);
            this.groupBox1.Controls.Add(this.radioButtonTimeOld);
            this.groupBox1.Controls.Add(this.radioButtonTimeNew);
            this.groupBox1.Controls.Add(this.radioButtonTimeSame);
            this.groupBox1.Location = new System.Drawing.Point(24, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(152, 112);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "更新日時";
            // 
            // radioButtonTimeIgnore
            // 
            this.radioButtonTimeIgnore.AutoSize = true;
            this.radioButtonTimeIgnore.Location = new System.Drawing.Point(6, 85);
            this.radioButtonTimeIgnore.Name = "radioButtonTimeIgnore";
            this.radioButtonTimeIgnore.Size = new System.Drawing.Size(96, 19);
            this.radioButtonTimeIgnore.TabIndex = 3;
            this.radioButtonTimeIgnore.TabStop = true;
            this.radioButtonTimeIgnore.Text = "考慮しない(&T)";
            this.radioButtonTimeIgnore.UseVisualStyleBackColor = true;
            // 
            // radioButtonTimeOld
            // 
            this.radioButtonTimeOld.AutoSize = true;
            this.radioButtonTimeOld.Location = new System.Drawing.Point(6, 63);
            this.radioButtonTimeOld.Name = "radioButtonTimeOld";
            this.radioButtonTimeOld.Size = new System.Drawing.Size(121, 19);
            this.radioButtonTimeOld.TabIndex = 2;
            this.radioButtonTimeOld.TabStop = true;
            this.radioButtonTimeOld.Text = "古いものをマーク(&O)";
            this.radioButtonTimeOld.UseVisualStyleBackColor = true;
            // 
            // radioButtonTimeNew
            // 
            this.radioButtonTimeNew.AutoSize = true;
            this.radioButtonTimeNew.Location = new System.Drawing.Point(6, 41);
            this.radioButtonTimeNew.Name = "radioButtonTimeNew";
            this.radioButtonTimeNew.Size = new System.Drawing.Size(133, 19);
            this.radioButtonTimeNew.TabIndex = 1;
            this.radioButtonTimeNew.TabStop = true;
            this.radioButtonTimeNew.Text = "新しいものをマーク(&W)";
            this.radioButtonTimeNew.UseVisualStyleBackColor = true;
            // 
            // radioButtonTimeSame
            // 
            this.radioButtonTimeSame.AutoSize = true;
            this.radioButtonTimeSame.Location = new System.Drawing.Point(7, 19);
            this.radioButtonTimeSame.Name = "radioButtonTimeSame";
            this.radioButtonTimeSame.Size = new System.Drawing.Size(119, 19);
            this.radioButtonTimeSame.TabIndex = 0;
            this.radioButtonTimeSame.TabStop = true;
            this.radioButtonTimeSame.Text = "同じものをマーク(&S)";
            this.radioButtonTimeSame.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonSizeIgnore);
            this.groupBox2.Controls.Add(this.radioButtonSizeSmall);
            this.groupBox2.Controls.Add(this.radioButtonSizeBig);
            this.groupBox2.Controls.Add(this.radioButtonSizeSame);
            this.groupBox2.Location = new System.Drawing.Point(182, 69);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(152, 112);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ファイルサイズ";
            // 
            // radioButtonSizeIgnore
            // 
            this.radioButtonSizeIgnore.AutoSize = true;
            this.radioButtonSizeIgnore.Location = new System.Drawing.Point(6, 85);
            this.radioButtonSizeIgnore.Name = "radioButtonSizeIgnore";
            this.radioButtonSizeIgnore.Size = new System.Drawing.Size(97, 19);
            this.radioButtonSizeIgnore.TabIndex = 3;
            this.radioButtonSizeIgnore.TabStop = true;
            this.radioButtonSizeIgnore.Text = "考慮しない(&G)";
            this.radioButtonSizeIgnore.UseVisualStyleBackColor = true;
            // 
            // radioButtonSizeSmall
            // 
            this.radioButtonSizeSmall.AutoSize = true;
            this.radioButtonSizeSmall.Location = new System.Drawing.Point(6, 63);
            this.radioButtonSizeSmall.Name = "radioButtonSizeSmall";
            this.radioButtonSizeSmall.Size = new System.Drawing.Size(130, 19);
            this.radioButtonSizeSmall.TabIndex = 2;
            this.radioButtonSizeSmall.TabStop = true;
            this.radioButtonSizeSmall.Text = "小さいものをマーク(&M)";
            this.radioButtonSizeSmall.UseVisualStyleBackColor = true;
            // 
            // radioButtonSizeBig
            // 
            this.radioButtonSizeBig.AutoSize = true;
            this.radioButtonSizeBig.Location = new System.Drawing.Point(6, 41);
            this.radioButtonSizeBig.Name = "radioButtonSizeBig";
            this.radioButtonSizeBig.Size = new System.Drawing.Size(129, 19);
            this.radioButtonSizeBig.TabIndex = 1;
            this.radioButtonSizeBig.TabStop = true;
            this.radioButtonSizeBig.Text = "大きいものをマーク(&B)";
            this.radioButtonSizeBig.UseVisualStyleBackColor = true;
            // 
            // radioButtonSizeSame
            // 
            this.radioButtonSizeSame.AutoSize = true;
            this.radioButtonSizeSame.Location = new System.Drawing.Point(7, 19);
            this.radioButtonSizeSame.Name = "radioButtonSizeSame";
            this.radioButtonSizeSame.Size = new System.Drawing.Size(116, 19);
            this.radioButtonSizeSame.TabIndex = 0;
            this.radioButtonSizeSame.TabStop = true;
            this.radioButtonSizeSame.Text = "同じものをマーク(&I)";
            this.radioButtonSizeSame.UseVisualStyleBackColor = true;
            // 
            // checkBoxExceptFolder
            // 
            this.checkBoxExceptFolder.AutoSize = true;
            this.checkBoxExceptFolder.Location = new System.Drawing.Point(24, 187);
            this.checkBoxExceptFolder.Name = "checkBoxExceptFolder";
            this.checkBoxExceptFolder.Size = new System.Drawing.Size(111, 19);
            this.checkBoxExceptFolder.TabIndex = 7;
            this.checkBoxExceptFolder.Text = "フォルダを除外(&F)";
            this.checkBoxExceptFolder.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 206);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(291, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "左右の一覧中、同じ名前のファイルを自動的にマークします。";
            // 
            // buttonExactly
            // 
            this.buttonExactly.Location = new System.Drawing.Point(340, 76);
            this.buttonExactly.Name = "buttonExactly";
            this.buttonExactly.Size = new System.Drawing.Size(94, 23);
            this.buttonExactly.TabIndex = 5;
            this.buttonExactly.Text = "同一ファイル(&A)";
            this.buttonExactly.UseVisualStyleBackColor = true;
            // 
            // buttonNameOnly
            // 
            this.buttonNameOnly.Location = new System.Drawing.Point(340, 103);
            this.buttonNameOnly.Name = "buttonNameOnly";
            this.buttonNameOnly.Size = new System.Drawing.Size(94, 23);
            this.buttonNameOnly.TabIndex = 6;
            this.buttonNameOnly.Text = "名前のみ(&N)";
            this.buttonNameOnly.UseVisualStyleBackColor = true;
            // 
            // FileListComparePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.buttonExactly);
            this.Controls.Add(this.buttonNameOnly);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxExceptFolder);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radioButtonFix);
            this.Controls.Add(this.radioButtonPrev);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "FileListComparePage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonFix;
        private System.Windows.Forms.RadioButton radioButtonPrev;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonTimeIgnore;
        private System.Windows.Forms.RadioButton radioButtonTimeOld;
        private System.Windows.Forms.RadioButton radioButtonTimeNew;
        private System.Windows.Forms.RadioButton radioButtonTimeSame;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonSizeIgnore;
        private System.Windows.Forms.RadioButton radioButtonSizeSmall;
        private System.Windows.Forms.RadioButton radioButtonSizeBig;
        private System.Windows.Forms.RadioButton radioButtonSizeSame;
        private System.Windows.Forms.CheckBox checkBoxExceptFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonExactly;
        private System.Windows.Forms.Button buttonNameOnly;


    }
}
