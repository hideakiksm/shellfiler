namespace ShellFiler.UI.Dialog.Option {
    partial class FileListFolderSizePage {
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
            this.numericSpecify = new System.Windows.Forms.NumericUpDown();
            this.radioButtonSpecify = new System.Windows.Forms.RadioButton();
            this.radioButtonOpposite = new System.Windows.Forms.RadioButton();
            this.radioButtonTarget = new System.Windows.Forms.RadioButton();
            this.radioButtonOriginal = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.labelLowerMessage = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxLowerCache = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonFix = new System.Windows.Forms.RadioButton();
            this.radioButtonPrev = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericMaxFolder = new System.Windows.Forms.NumericUpDown();
            this.numericMaxDepth = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSpecify)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxFolder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxDepth)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericSpecify);
            this.groupBox1.Controls.Add(this.radioButtonSpecify);
            this.groupBox1.Controls.Add(this.radioButtonOpposite);
            this.groupBox1.Controls.Add(this.radioButtonTarget);
            this.groupBox1.Controls.Add(this.radioButtonOriginal);
            this.groupBox1.Location = new System.Drawing.Point(32, 103);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(427, 115);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "サイズの計算方法";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(291, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "bytes単位に切り上げ";
            // 
            // numericSpecify
            // 
            this.numericSpecify.Location = new System.Drawing.Point(182, 85);
            this.numericSpecify.Name = "numericSpecify";
            this.numericSpecify.Size = new System.Drawing.Size(103, 23);
            this.numericSpecify.TabIndex = 6;
            // 
            // radioButtonSpecify
            // 
            this.radioButtonSpecify.AutoSize = true;
            this.radioButtonSpecify.Location = new System.Drawing.Point(7, 85);
            this.radioButtonSpecify.Name = "radioButtonSpecify";
            this.radioButtonSpecify.Size = new System.Drawing.Size(120, 19);
            this.radioButtonSpecify.TabIndex = 3;
            this.radioButtonSpecify.TabStop = true;
            this.radioButtonSpecify.Text = "指定サイズ単位(&S)";
            this.radioButtonSpecify.UseVisualStyleBackColor = true;
            // 
            // radioButtonOpposite
            // 
            this.radioButtonOpposite.AutoSize = true;
            this.radioButtonOpposite.Location = new System.Drawing.Point(7, 63);
            this.radioButtonOpposite.Name = "radioButtonOpposite";
            this.radioButtonOpposite.Size = new System.Drawing.Size(159, 19);
            this.radioButtonOpposite.TabIndex = 2;
            this.radioButtonOpposite.TabStop = true;
            this.radioButtonOpposite.Text = "反対パスのクラスタサイズ(&O)";
            this.radioButtonOpposite.UseVisualStyleBackColor = true;
            // 
            // radioButtonTarget
            // 
            this.radioButtonTarget.AutoSize = true;
            this.radioButtonTarget.Location = new System.Drawing.Point(7, 41);
            this.radioButtonTarget.Name = "radioButtonTarget";
            this.radioButtonTarget.Size = new System.Drawing.Size(157, 19);
            this.radioButtonTarget.TabIndex = 1;
            this.radioButtonTarget.TabStop = true;
            this.radioButtonTarget.Text = "対象パスのクラスタサイズ(&P)";
            this.radioButtonTarget.UseVisualStyleBackColor = true;
            // 
            // radioButtonOriginal
            // 
            this.radioButtonOriginal.AutoSize = true;
            this.radioButtonOriginal.Location = new System.Drawing.Point(7, 19);
            this.radioButtonOriginal.Name = "radioButtonOriginal";
            this.radioButtonOriginal.Size = new System.Drawing.Size(143, 19);
            this.radioButtonOriginal.TabIndex = 0;
            this.radioButtonOriginal.TabStop = true;
            this.radioButtonOriginal.Text = "ファイルサイズそのもの(&F)";
            this.radioButtonOriginal.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(46, 275);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(334, 15);
            this.label6.TabIndex = 8;
            this.label6.Text = "フォルダを切り替えると、すぐに取得済みのフォルダサイズを表示します。";
            // 
            // labelLowerMessage
            // 
            this.labelLowerMessage.AutoSize = true;
            this.labelLowerMessage.Location = new System.Drawing.Point(46, 260);
            this.labelLowerMessage.Name = "labelLowerMessage";
            this.labelLowerMessage.Size = new System.Drawing.Size(313, 15);
            this.labelLowerMessage.TabIndex = 7;
            this.labelLowerMessage.Text = "最大で下記の階層/フォルダ分のフォルダサイズをキャッシュします。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 245);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(347, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "サイズの取得後、[フォルダサイズ取得結果のクリア]を実行するまでの間、";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(315, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "マークされたフォルダ配下にあるファイルサイズの合計を計算します。";
            // 
            // checkBoxLowerCache
            // 
            this.checkBoxLowerCache.AutoSize = true;
            this.checkBoxLowerCache.Location = new System.Drawing.Point(32, 226);
            this.checkBoxLowerCache.Name = "checkBoxLowerCache";
            this.checkBoxLowerCache.Size = new System.Drawing.Size(158, 19);
            this.checkBoxLowerCache.TabIndex = 5;
            this.checkBoxLowerCache.Text = "下位階層の結果を保持(&L)";
            this.checkBoxLowerCache.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "フォルダサイズの取得";
            // 
            // radioButtonFix
            // 
            this.radioButtonFix.AutoSize = true;
            this.radioButtonFix.Location = new System.Drawing.Point(3, 81);
            this.radioButtonFix.Name = "radioButtonFix";
            this.radioButtonFix.Size = new System.Drawing.Size(109, 19);
            this.radioButtonFix.TabIndex = 3;
            this.radioButtonFix.TabStop = true;
            this.radioButtonFix.Text = "初期値を指定(&I)";
            this.radioButtonFix.UseVisualStyleBackColor = true;
            this.radioButtonFix.CheckedChanged += new System.EventHandler(this.radioButtonPrevFixed_CheckedChanged);
            // 
            // radioButtonPrev
            // 
            this.radioButtonPrev.AutoSize = true;
            this.radioButtonPrev.Location = new System.Drawing.Point(3, 45);
            this.radioButtonPrev.Name = "radioButtonPrev";
            this.radioButtonPrev.Size = new System.Drawing.Size(247, 19);
            this.radioButtonPrev.TabIndex = 2;
            this.radioButtonPrev.TabStop = true;
            this.radioButtonPrev.Text = "直前に指定された設定を初期値として使用(&R)";
            this.radioButtonPrev.UseVisualStyleBackColor = true;
            this.radioButtonPrev.CheckedChanged += new System.EventHandler(this.radioButtonPrevFixed_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericMaxFolder);
            this.groupBox2.Controls.Add(this.numericMaxDepth);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(5, 298);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(512, 69);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "下位階層の結果を保持する最大件数";
            // 
            // numericMaxFolder
            // 
            this.numericMaxFolder.Location = new System.Drawing.Point(89, 43);
            this.numericMaxFolder.Name = "numericMaxFolder";
            this.numericMaxFolder.Size = new System.Drawing.Size(91, 23);
            this.numericMaxFolder.TabIndex = 3;
            // 
            // numericMaxDepth
            // 
            this.numericMaxDepth.Location = new System.Drawing.Point(89, 18);
            this.numericMaxDepth.Name = "numericMaxDepth";
            this.numericMaxDepth.Size = new System.Drawing.Size(91, 23);
            this.numericMaxDepth.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 15);
            this.label7.TabIndex = 2;
            this.label7.Text = "フォルダ数(&N):";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(211, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(284, 15);
            this.label10.TabIndex = 6;
            this.label10.Text = "フォルダ数を超えると、深い階層のサイズは記憶されません。";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(211, 29);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(241, 15);
            this.label9.TabIndex = 5;
            this.label9.Text = "値以上の階層のフォルダサイズは記憶されません。";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(211, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(250, 15);
            this.label8.TabIndex = 4;
            this.label8.Text = "マークしたフォルダの階層が1、その下が2と数えます。";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "階層(&D):";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(30, 65);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(392, 15);
            this.label11.TabIndex = 1;
            this.label11.Text = "SSHで実行すると、サイズの計算方法は対象パスのクラスタサイズに設定されます。";
            // 
            // FileListFolderSizePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.radioButtonFix);
            this.Controls.Add(this.radioButtonPrev);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelLowerMessage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxLowerCache);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "FileListFolderSizePage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSpecify)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxFolder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxDepth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericSpecify;
        private System.Windows.Forms.RadioButton radioButtonSpecify;
        private System.Windows.Forms.RadioButton radioButtonOpposite;
        private System.Windows.Forms.RadioButton radioButtonTarget;
        private System.Windows.Forms.RadioButton radioButtonOriginal;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelLowerMessage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxLowerCache;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonFix;
        private System.Windows.Forms.RadioButton radioButtonPrev;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericMaxFolder;
        private System.Windows.Forms.NumericUpDown numericMaxDepth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;

    }
}
