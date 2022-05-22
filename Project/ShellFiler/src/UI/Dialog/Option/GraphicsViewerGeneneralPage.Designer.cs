namespace ShellFiler.UI.Dialog.Option {
    partial class GraphicsViewerGeneralPage {
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
            this.numericMaxSize = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.labelMaxSize = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.trackBarBraking = new System.Windows.Forms.TrackBar();
            this.radioButtonUse = new System.Windows.Forms.RadioButton();
            this.radioButtonLocal = new System.Windows.Forms.RadioButton();
            this.radioButtonNotUse = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButtonFilterAll = new System.Windows.Forms.RadioButton();
            this.radioButtonFilterWindow = new System.Windows.Forms.RadioButton();
            this.radioButtonFilterImage = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxFullAlwaysHide = new System.Windows.Forms.CheckBox();
            this.checkBoxFullInfo = new System.Windows.Forms.CheckBox();
            this.checkBoxFullCursor = new System.Windows.Forms.CheckBox();
            this.trackBarFullHide = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxSize)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBraking)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFullHide)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(205, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "読み込める画像ファイルの最大サイズ(&S):";
            // 
            // numericMaxSize
            // 
            this.numericMaxSize.Location = new System.Drawing.Point(211, 4);
            this.numericMaxSize.Name = "numericMaxSize";
            this.numericMaxSize.Size = new System.Drawing.Size(120, 23);
            this.numericMaxSize.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(337, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "MB";
            // 
            // labelMaxSize
            // 
            this.labelMaxSize.AutoSize = true;
            this.labelMaxSize.Location = new System.Drawing.Point(365, 6);
            this.labelMaxSize.Name = "labelMaxSize";
            this.labelMaxSize.Size = new System.Drawing.Size(79, 15);
            this.labelMaxSize.TabIndex = 3;
            this.labelMaxSize.Text = "{0}～{1}MB";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trackBarBraking);
            this.groupBox1.Controls.Add(this.radioButtonUse);
            this.groupBox1.Controls.Add(this.radioButtonLocal);
            this.groupBox1.Controls.Add(this.radioButtonNotUse);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(5, 44);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(476, 91);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ドラッグでのスクロール中の慣性";
            // 
            // trackBarBraking
            // 
            this.trackBarBraking.Location = new System.Drawing.Point(322, 37);
            this.trackBarBraking.Name = "trackBarBraking";
            this.trackBarBraking.Size = new System.Drawing.Size(116, 45);
            this.trackBarBraking.TabIndex = 5;
            // 
            // radioButtonUse
            // 
            this.radioButtonUse.AutoSize = true;
            this.radioButtonUse.Location = new System.Drawing.Point(6, 63);
            this.radioButtonUse.Name = "radioButtonUse";
            this.radioButtonUse.Size = new System.Drawing.Size(107, 19);
            this.radioButtonUse.TabIndex = 2;
            this.radioButtonUse.TabStop = true;
            this.radioButtonUse.Text = "常に使用する(&A)";
            this.radioButtonUse.UseVisualStyleBackColor = true;
            // 
            // radioButtonLocal
            // 
            this.radioButtonLocal.AutoSize = true;
            this.radioButtonLocal.Location = new System.Drawing.Point(6, 41);
            this.radioButtonLocal.Name = "radioButtonLocal";
            this.radioButtonLocal.Size = new System.Drawing.Size(204, 19);
            this.radioButtonLocal.TabIndex = 1;
            this.radioButtonLocal.TabStop = true;
            this.radioButtonLocal.Text = "リモートデスクトップ以外で使用する(&L)";
            this.radioButtonLocal.UseVisualStyleBackColor = true;
            // 
            // radioButtonNotUse
            // 
            this.radioButtonNotUse.AutoSize = true;
            this.radioButtonNotUse.Location = new System.Drawing.Point(7, 19);
            this.radioButtonNotUse.Name = "radioButtonNotUse";
            this.radioButtonNotUse.Size = new System.Drawing.Size(97, 19);
            this.radioButtonNotUse.TabIndex = 0;
            this.radioButtonNotUse.TabStop = true;
            this.radioButtonNotUse.Text = "使用しない(&N)";
            this.radioButtonNotUse.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(440, 41);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(19, 15);
            this.label9.TabIndex = 6;
            this.label9.Text = "強";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(303, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(19, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "弱";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(303, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 15);
            this.label5.TabIndex = 3;
            this.label5.Text = "ブレーキのきき具合(&B):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.radioButtonFilterAll);
            this.groupBox2.Controls.Add(this.radioButtonFilterWindow);
            this.groupBox2.Controls.Add(this.radioButtonFilterImage);
            this.groupBox2.Location = new System.Drawing.Point(5, 151);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(476, 109);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "フィルター";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(438, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "画像に適用するフィルター（明るさ調整やぼかし効果など）を有効にする範囲を指定します。";
            // 
            // radioButtonFilterAll
            // 
            this.radioButtonFilterAll.AutoSize = true;
            this.radioButtonFilterAll.Location = new System.Drawing.Point(5, 85);
            this.radioButtonFilterAll.Name = "radioButtonFilterAll";
            this.radioButtonFilterAll.Size = new System.Drawing.Size(207, 19);
            this.radioButtonFilterAll.TabIndex = 3;
            this.radioButtonFilterAll.TabStop = true;
            this.radioButtonFilterAll.Text = "すべての画像にフィルターを適用する(&I)";
            this.radioButtonFilterAll.UseVisualStyleBackColor = true;
            // 
            // radioButtonFilterWindow
            // 
            this.radioButtonFilterWindow.AutoSize = true;
            this.radioButtonFilterWindow.Location = new System.Drawing.Point(5, 63);
            this.radioButtonFilterWindow.Name = "radioButtonFilterWindow";
            this.radioButtonFilterWindow.Size = new System.Drawing.Size(288, 19);
            this.radioButtonFilterWindow.TabIndex = 2;
            this.radioButtonFilterWindow.TabStop = true;
            this.radioButtonFilterWindow.Text = "設定したウィンドウの画像だけにフィルターを適用する(&W)";
            this.radioButtonFilterWindow.UseVisualStyleBackColor = true;
            // 
            // radioButtonFilterImage
            // 
            this.radioButtonFilterImage.AutoSize = true;
            this.radioButtonFilterImage.Location = new System.Drawing.Point(6, 41);
            this.radioButtonFilterImage.Name = "radioButtonFilterImage";
            this.radioButtonFilterImage.Size = new System.Drawing.Size(236, 19);
            this.radioButtonFilterImage.TabIndex = 1;
            this.radioButtonFilterImage.TabStop = true;
            this.radioButtonFilterImage.Text = "表示中の画像だけにフィルターを適用する(&C)";
            this.radioButtonFilterImage.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxFullAlwaysHide);
            this.groupBox3.Controls.Add(this.checkBoxFullInfo);
            this.groupBox3.Controls.Add(this.checkBoxFullCursor);
            this.groupBox3.Controls.Add(this.trackBarFullHide);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(5, 277);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(476, 86);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "全画面表示";
            // 
            // checkBoxFullAlwaysHide
            // 
            this.checkBoxFullAlwaysHide.AutoSize = true;
            this.checkBoxFullAlwaysHide.Location = new System.Drawing.Point(9, 63);
            this.checkBoxFullAlwaysHide.Name = "checkBoxFullAlwaysHide";
            this.checkBoxFullAlwaysHide.Size = new System.Drawing.Size(198, 19);
            this.checkBoxFullAlwaysHide.TabIndex = 2;
            this.checkBoxFullAlwaysHide.Text = "ファイル情報は常に非表示にする(&D)";
            this.checkBoxFullAlwaysHide.UseVisualStyleBackColor = true;
            this.checkBoxFullAlwaysHide.CheckedChanged += new System.EventHandler(this.checkBoxFullAlwaysHide_CheckedChanged);
            // 
            // checkBoxFullInfo
            // 
            this.checkBoxFullInfo.AutoSize = true;
            this.checkBoxFullInfo.Location = new System.Drawing.Point(9, 41);
            this.checkBoxFullInfo.Name = "checkBoxFullInfo";
            this.checkBoxFullInfo.Size = new System.Drawing.Size(243, 19);
            this.checkBoxFullInfo.TabIndex = 1;
            this.checkBoxFullInfo.Text = "一定時間後にファイル情報を非表示にする(&F)";
            this.checkBoxFullInfo.UseVisualStyleBackColor = true;
            // 
            // checkBoxFullCursor
            // 
            this.checkBoxFullCursor.AutoSize = true;
            this.checkBoxFullCursor.Location = new System.Drawing.Point(9, 19);
            this.checkBoxFullCursor.Name = "checkBoxFullCursor";
            this.checkBoxFullCursor.Size = new System.Drawing.Size(258, 19);
            this.checkBoxFullCursor.TabIndex = 0;
            this.checkBoxFullCursor.Text = "一定時間後にマウスポインターを非表示にする(&P)";
            this.checkBoxFullCursor.UseVisualStyleBackColor = true;
            // 
            // trackBarFullHide
            // 
            this.trackBarFullHide.Location = new System.Drawing.Point(322, 32);
            this.trackBarFullHide.Name = "trackBarFullHide";
            this.trackBarFullHide.Size = new System.Drawing.Size(116, 45);
            this.trackBarFullHide.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(303, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "非表示までの時間(T)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(440, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 15);
            this.label8.TabIndex = 6;
            this.label8.Text = "長";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(303, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(19, 15);
            this.label6.TabIndex = 4;
            this.label6.Text = "短";
            // 
            // GraphicsViewerGeneralPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.numericMaxSize);
            this.Controls.Add(this.labelMaxSize);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "GraphicsViewerGeneralPage";
            this.Size = new System.Drawing.Size(520, 370);
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxSize)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBraking)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFullHide)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericMaxSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelMaxSize;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TrackBar trackBarBraking;
        private System.Windows.Forms.RadioButton radioButtonUse;
        private System.Windows.Forms.RadioButton radioButtonLocal;
        private System.Windows.Forms.RadioButton radioButtonNotUse;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioButtonFilterAll;
        private System.Windows.Forms.RadioButton radioButtonFilterWindow;
        private System.Windows.Forms.RadioButton radioButtonFilterImage;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxFullAlwaysHide;
        private System.Windows.Forms.CheckBox checkBoxFullInfo;
        private System.Windows.Forms.CheckBox checkBoxFullCursor;
        private System.Windows.Forms.TrackBar trackBarFullHide;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;


    }
}
