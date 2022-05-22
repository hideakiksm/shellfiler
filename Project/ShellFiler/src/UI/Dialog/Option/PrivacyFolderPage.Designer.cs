namespace ShellFiler.UI.Dialog.Option {
    partial class PrivacyFolderPage {
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
            this.linkLabelInitFolder = new System.Windows.Forms.LinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonFolderDel = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericFolderNum = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxFolderSave = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericWholeFolder = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericFolderNum)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericWholeFolder)).BeginInit();
            this.SuspendLayout();
            // 
            // linkLabelInitFolder
            // 
            this.linkLabelInitFolder.AutoSize = true;
            this.linkLabelInitFolder.Location = new System.Drawing.Point(26, 104);
            this.linkLabelInitFolder.Name = "linkLabelInitFolder";
            this.linkLabelInitFolder.Size = new System.Drawing.Size(132, 15);
            this.linkLabelInitFolder.TabIndex = 6;
            this.linkLabelInitFolder.TabStop = true;
            this.linkLabelInitFolder.Text = "起動時のフォルダの設定へ";
            this.linkLabelInitFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelInitFolder_LinkClicked);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 89);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(506, 15);
            this.label8.TabIndex = 5;
            this.label8.Text = "起動時のフォルダが[前回終了時のフォルダ]に設定されていると、終了時のフォルダが履歴となって残ります。";
            // 
            // buttonFolderDel
            // 
            this.buttonFolderDel.Location = new System.Drawing.Point(8, 323);
            this.buttonFolderDel.Name = "buttonFolderDel";
            this.buttonFolderDel.Size = new System.Drawing.Size(95, 23);
            this.buttonFolderDel.TabIndex = 4;
            this.buttonFolderDel.Text = "履歴の削除(&L)";
            this.buttonFolderDel.UseVisualStyleBackColor = true;
            this.buttonFolderDel.Click += new System.EventHandler(this.buttonFolderDel_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(253, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(144, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "(変更は次回起動時に有効)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(268, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "フォルダの変更を行ったときに履歴を記憶する方法です。";
            // 
            // numericFolderNum
            // 
            this.numericFolderNum.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericFolderNum.Location = new System.Drawing.Point(109, 58);
            this.numericFolderNum.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericFolderNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericFolderNum.Name = "numericFolderNum";
            this.numericFolderNum.Size = new System.Drawing.Size(120, 23);
            this.numericFolderNum.TabIndex = 3;
            this.numericFolderNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "最大記憶件数(&F):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "フォルダ履歴";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.numericFolderNum);
            this.groupBox1.Controls.Add(this.linkLabelInitFolder);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(3, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(514, 125);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "画面ごとの履歴";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(219, 15);
            this.label10.TabIndex = 0;
            this.label10.Text = "[次へ][戻る]で操作できるフォルダ履歴です。";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 31);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(186, 15);
            this.label9.TabIndex = 1;
            this.label9.Text = "左右それぞれの画面で記憶されます。";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(233, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "ShellFiler全体で記憶されるフォルダ履歴です。";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxFolderSave);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.numericWholeFolder);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(5, 201);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(512, 104);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "全体での履歴";
            // 
            // checkBoxFolderSave
            // 
            this.checkBoxFolderSave.AutoSize = true;
            this.checkBoxFolderSave.Location = new System.Drawing.Point(6, 78);
            this.checkBoxFolderSave.Name = "checkBoxFolderSave";
            this.checkBoxFolderSave.Size = new System.Drawing.Size(199, 19);
            this.checkBoxFolderSave.TabIndex = 5;
            this.checkBoxFolderSave.Text = "フォルダ履歴をディスクに保存する(&D)";
            this.checkBoxFolderSave.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(251, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "(変更は次回起動時に有効)";
            // 
            // numericWholeFolder
            // 
            this.numericWholeFolder.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericWholeFolder.Location = new System.Drawing.Point(107, 53);
            this.numericWholeFolder.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericWholeFolder.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericWholeFolder.Name = "numericWholeFolder";
            this.numericWholeFolder.Size = new System.Drawing.Size(120, 23);
            this.numericWholeFolder.TabIndex = 3;
            this.numericWholeFolder.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "最大記憶件数(&A):";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 31);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(459, 15);
            this.label11.TabIndex = 1;
            this.label11.Text = "[フォルダ履歴]コマンドでの一覧表示や、フォルダ切り替え時のカーソル位置の復元に使用します。";
            // 
            // PrivacyFolderPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonFolderDel);
            this.Controls.Add(this.label4);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "PrivacyFolderPage";
            this.Size = new System.Drawing.Size(520, 370);
            ((System.ComponentModel.ISupportInitialize)(this.numericFolderNum)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericWholeFolder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonFolderDel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericFolderNum;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel linkLabelInitFolder;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericWholeFolder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkBoxFolderSave;


    }
}
