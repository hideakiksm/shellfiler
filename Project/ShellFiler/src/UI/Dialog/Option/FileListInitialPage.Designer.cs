namespace ShellFiler.UI.Dialog.Option {
    partial class FileListInitialPage {
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
            this.buttonLeftRef = new System.Windows.Forms.Button();
            this.textBoxLeftFolder = new System.Windows.Forms.TextBox();
            this.radioButtonLeftFix = new System.Windows.Forms.RadioButton();
            this.radioButtonLeftPrev = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonWndRef = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numericWndY2 = new System.Windows.Forms.NumericUpDown();
            this.numericWndX2 = new System.Windows.Forms.NumericUpDown();
            this.numericWndY1 = new System.Windows.Forms.NumericUpDown();
            this.numericWndX1 = new System.Windows.Forms.NumericUpDown();
            this.radioButtonWndFix = new System.Windows.Forms.RadioButton();
            this.radioButtonWndPrev = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonRightRef = new System.Windows.Forms.Button();
            this.textBoxRightFolder = new System.Windows.Forms.TextBox();
            this.radioButtonRightFix = new System.Windows.Forms.RadioButton();
            this.radioButtonRightPrev = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.trackBarSplash = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericWndY2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWndX2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWndY1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWndX1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSplash)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonLeftRef);
            this.groupBox1.Controls.Add(this.textBoxLeftFolder);
            this.groupBox1.Controls.Add(this.radioButtonLeftFix);
            this.groupBox1.Controls.Add(this.radioButtonLeftPrev);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(514, 90);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "起動時の左フォルダ";
            // 
            // buttonLeftRef
            // 
            this.buttonLeftRef.Location = new System.Drawing.Point(433, 59);
            this.buttonLeftRef.Name = "buttonLeftRef";
            this.buttonLeftRef.Size = new System.Drawing.Size(75, 23);
            this.buttonLeftRef.TabIndex = 3;
            this.buttonLeftRef.Text = "参照(&1)...";
            this.buttonLeftRef.UseVisualStyleBackColor = true;
            this.buttonLeftRef.Click += new System.EventHandler(this.buttonLeftRef_Click);
            // 
            // textBoxLeftFolder
            // 
            this.textBoxLeftFolder.Location = new System.Drawing.Point(28, 59);
            this.textBoxLeftFolder.Name = "textBoxLeftFolder";
            this.textBoxLeftFolder.Size = new System.Drawing.Size(399, 23);
            this.textBoxLeftFolder.TabIndex = 2;
            // 
            // radioButtonLeftFix
            // 
            this.radioButtonLeftFix.AutoSize = true;
            this.radioButtonLeftFix.Location = new System.Drawing.Point(7, 41);
            this.radioButtonLeftFix.Name = "radioButtonLeftFix";
            this.radioButtonLeftFix.Size = new System.Drawing.Size(163, 19);
            this.radioButtonLeftFix.TabIndex = 1;
            this.radioButtonLeftFix.TabStop = true;
            this.radioButtonLeftFix.Text = "指定のWindowsフォルダ(&F)";
            this.radioButtonLeftFix.UseVisualStyleBackColor = true;
            this.radioButtonLeftFix.CheckedChanged += new System.EventHandler(this.RadioButtonLeft_CheckedChanged);
            // 
            // radioButtonLeftPrev
            // 
            this.radioButtonLeftPrev.AutoSize = true;
            this.radioButtonLeftPrev.Location = new System.Drawing.Point(7, 19);
            this.radioButtonLeftPrev.Name = "radioButtonLeftPrev";
            this.radioButtonLeftPrev.Size = new System.Drawing.Size(147, 19);
            this.radioButtonLeftPrev.TabIndex = 0;
            this.radioButtonLeftPrev.TabStop = true;
            this.radioButtonLeftPrev.Text = "前回終了時のフォルダ(&L)";
            this.radioButtonLeftPrev.UseVisualStyleBackColor = true;
            this.radioButtonLeftPrev.CheckedChanged += new System.EventHandler(this.RadioButtonLeft_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.buttonWndRef);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.numericWndY2);
            this.groupBox3.Controls.Add(this.numericWndX2);
            this.groupBox3.Controls.Add(this.numericWndY1);
            this.groupBox3.Controls.Add(this.numericWndX1);
            this.groupBox3.Controls.Add(this.radioButtonWndFix);
            this.groupBox3.Controls.Add(this.radioButtonWndPrev);
            this.groupBox3.Location = new System.Drawing.Point(3, 195);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(514, 88);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "起動時のウィンドウサイズ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(290, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "右下座標";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(126, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "左上座標";
            // 
            // buttonWndRef
            // 
            this.buttonWndRef.Location = new System.Drawing.Point(433, 57);
            this.buttonWndRef.Name = "buttonWndRef";
            this.buttonWndRef.Size = new System.Drawing.Size(75, 23);
            this.buttonWndRef.TabIndex = 7;
            this.buttonWndRef.Text = "選択(&W)...";
            this.buttonWndRef.UseVisualStyleBackColor = true;
            this.buttonWndRef.Click += new System.EventHandler(this.buttonWndRef_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(269, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "～";
            // 
            // numericWndY2
            // 
            this.numericWndY2.Location = new System.Drawing.Point(368, 58);
            this.numericWndY2.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericWndY2.Name = "numericWndY2";
            this.numericWndY2.Size = new System.Drawing.Size(59, 23);
            this.numericWndY2.TabIndex = 6;
            // 
            // numericWndX2
            // 
            this.numericWndX2.Location = new System.Drawing.Point(303, 58);
            this.numericWndX2.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericWndX2.Name = "numericWndX2";
            this.numericWndX2.Size = new System.Drawing.Size(59, 23);
            this.numericWndX2.TabIndex = 5;
            // 
            // numericWndY1
            // 
            this.numericWndY1.Location = new System.Drawing.Point(204, 58);
            this.numericWndY1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericWndY1.Name = "numericWndY1";
            this.numericWndY1.Size = new System.Drawing.Size(59, 23);
            this.numericWndY1.TabIndex = 3;
            // 
            // numericWndX1
            // 
            this.numericWndX1.Location = new System.Drawing.Point(139, 58);
            this.numericWndX1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericWndX1.Name = "numericWndX1";
            this.numericWndX1.Size = new System.Drawing.Size(59, 23);
            this.numericWndX1.TabIndex = 2;
            // 
            // radioButtonWndFix
            // 
            this.radioButtonWndFix.AutoSize = true;
            this.radioButtonWndFix.Location = new System.Drawing.Point(7, 42);
            this.radioButtonWndFix.Name = "radioButtonWndFix";
            this.radioButtonWndFix.Size = new System.Drawing.Size(96, 19);
            this.radioButtonWndFix.TabIndex = 1;
            this.radioButtonWndFix.TabStop = true;
            this.radioButtonWndFix.Text = "固定サイズ(&X)";
            this.radioButtonWndFix.UseVisualStyleBackColor = true;
            this.radioButtonWndFix.CheckedChanged += new System.EventHandler(this.RadioButtonWnd_CheckedChanged);
            // 
            // radioButtonWndPrev
            // 
            this.radioButtonWndPrev.AutoSize = true;
            this.radioButtonWndPrev.Location = new System.Drawing.Point(7, 19);
            this.radioButtonWndPrev.Name = "radioButtonWndPrev";
            this.radioButtonWndPrev.Size = new System.Drawing.Size(141, 19);
            this.radioButtonWndPrev.TabIndex = 0;
            this.radioButtonWndPrev.TabStop = true;
            this.radioButtonWndPrev.Text = "前回終了時のサイズ(&P)";
            this.radioButtonWndPrev.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonRightRef);
            this.groupBox2.Controls.Add(this.textBoxRightFolder);
            this.groupBox2.Controls.Add(this.radioButtonRightFix);
            this.groupBox2.Controls.Add(this.radioButtonRightPrev);
            this.groupBox2.Location = new System.Drawing.Point(3, 99);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(514, 90);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "起動時の左フォルダ";
            // 
            // buttonRightRef
            // 
            this.buttonRightRef.Location = new System.Drawing.Point(433, 60);
            this.buttonRightRef.Name = "buttonRightRef";
            this.buttonRightRef.Size = new System.Drawing.Size(75, 23);
            this.buttonRightRef.TabIndex = 3;
            this.buttonRightRef.Text = "参照(&2)...";
            this.buttonRightRef.UseVisualStyleBackColor = true;
            this.buttonRightRef.Click += new System.EventHandler(this.buttonRightRef_Click);
            // 
            // textBoxRightFolder
            // 
            this.textBoxRightFolder.Location = new System.Drawing.Point(28, 60);
            this.textBoxRightFolder.Name = "textBoxRightFolder";
            this.textBoxRightFolder.Size = new System.Drawing.Size(399, 23);
            this.textBoxRightFolder.TabIndex = 2;
            // 
            // radioButtonRightFix
            // 
            this.radioButtonRightFix.AutoSize = true;
            this.radioButtonRightFix.Location = new System.Drawing.Point(7, 41);
            this.radioButtonRightFix.Name = "radioButtonRightFix";
            this.radioButtonRightFix.Size = new System.Drawing.Size(165, 19);
            this.radioButtonRightFix.TabIndex = 1;
            this.radioButtonRightFix.TabStop = true;
            this.radioButtonRightFix.Text = "指定のWindowsフォルダ(&O)";
            this.radioButtonRightFix.UseVisualStyleBackColor = true;
            this.radioButtonRightFix.CheckedChanged += new System.EventHandler(this.RadioButtonRight_CheckedChanged);
            // 
            // radioButtonRightPrev
            // 
            this.radioButtonRightPrev.AutoSize = true;
            this.radioButtonRightPrev.Location = new System.Drawing.Point(7, 19);
            this.radioButtonRightPrev.Name = "radioButtonRightPrev";
            this.radioButtonRightPrev.Size = new System.Drawing.Size(148, 19);
            this.radioButtonRightPrev.TabIndex = 0;
            this.radioButtonRightPrev.TabStop = true;
            this.radioButtonRightPrev.Text = "前回終了時のフォルダ(&R)";
            this.radioButtonRightPrev.UseVisualStyleBackColor = true;
            this.radioButtonRightPrev.CheckedChanged += new System.EventHandler(this.RadioButtonRight_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.trackBarSplash);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Location = new System.Drawing.Point(3, 289);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(513, 68);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "タイトル画面でのウェイト";
            // 
            // trackBarSplash
            // 
            this.trackBarSplash.Location = new System.Drawing.Point(45, 20);
            this.trackBarSplash.Name = "trackBarSplash";
            this.trackBarSplash.Size = new System.Drawing.Size(104, 45);
            this.trackBarSplash.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(202, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(235, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "処理が長引いた場合はそれ以上短くなりません。";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(202, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(278, 15);
            this.label6.TabIndex = 3;
            this.label6.Text = "起動時にタイトルが表示されてから消えるまでの時間です。";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(152, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "長い";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "短い";
            // 
            // FileListInitialPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "FileListInitialPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericWndY2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWndX2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWndY1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWndX1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSplash)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonLeftRef;
        private System.Windows.Forms.TextBox textBoxLeftFolder;
        private System.Windows.Forms.RadioButton radioButtonLeftFix;
        private System.Windows.Forms.RadioButton radioButtonLeftPrev;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonRightRef;
        private System.Windows.Forms.TextBox textBoxRightFolder;
        private System.Windows.Forms.RadioButton radioButtonRightFix;
        private System.Windows.Forms.RadioButton radioButtonRightPrev;
        private System.Windows.Forms.RadioButton radioButtonWndFix;
        private System.Windows.Forms.RadioButton radioButtonWndPrev;
        private System.Windows.Forms.Button buttonWndRef;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericWndY2;
        private System.Windows.Forms.NumericUpDown numericWndX2;
        private System.Windows.Forms.NumericUpDown numericWndY1;
        private System.Windows.Forms.NumericUpDown numericWndX1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TrackBar trackBarSplash;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;

    }
}
