namespace ShellFiler.UI.Dialog.Option {
    partial class PrivacyFileViewerPage {
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
            this.buttonViewerDel = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.checkBoxViewerSave = new System.Windows.Forms.CheckBox();
            this.numericViewerNum = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericViewerNum)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonViewerDel
            // 
            this.buttonViewerDel.Location = new System.Drawing.Point(8, 133);
            this.buttonViewerDel.Name = "buttonViewerDel";
            this.buttonViewerDel.Size = new System.Drawing.Size(95, 23);
            this.buttonViewerDel.TabIndex = 7;
            this.buttonViewerDel.Text = "履歴の削除(&E)";
            this.buttonViewerDel.UseVisualStyleBackColor = true;
            this.buttonViewerDel.Click += new System.EventHandler(this.buttonViewerDel_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(244, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(144, 15);
            this.label9.TabIndex = 5;
            this.label9.Text = "(変更は次回起動時に有効)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 23);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(250, 15);
            this.label10.TabIndex = 1;
            this.label10.Text = "ファイルビューアで入力した検索文字列の履歴です。";
            // 
            // checkBoxViewerSave
            // 
            this.checkBoxViewerSave.AutoSize = true;
            this.checkBoxViewerSave.Location = new System.Drawing.Point(8, 101);
            this.checkBoxViewerSave.Name = "checkBoxViewerSave";
            this.checkBoxViewerSave.Size = new System.Drawing.Size(188, 19);
            this.checkBoxViewerSave.TabIndex = 6;
            this.checkBoxViewerSave.Text = "検索履歴をディスクに保存する(&D)";
            this.checkBoxViewerSave.UseVisualStyleBackColor = true;
            // 
            // numericViewerNum
            // 
            this.numericViewerNum.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericViewerNum.Location = new System.Drawing.Point(110, 74);
            this.numericViewerNum.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericViewerNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericViewerNum.Name = "numericViewerNum";
            this.numericViewerNum.Size = new System.Drawing.Size(120, 23);
            this.numericViewerNum.TabIndex = 4;
            this.numericViewerNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 76);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(102, 15);
            this.label11.TabIndex = 3;
            this.label11.Text = "最大記憶件数(&S):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "ファイルビューアの検索履歴";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(209, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "テキストとダンプのそれぞれで記憶されます。";
            // 
            // PrivacyFileViewerPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.buttonViewerDel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.checkBoxViewerSave);
            this.Controls.Add(this.numericViewerNum);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "PrivacyFileViewerPage";
            this.Size = new System.Drawing.Size(520, 370);
            ((System.ComponentModel.ISupportInitialize)(this.numericViewerNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonViewerDel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox checkBoxViewerSave;
        private System.Windows.Forms.NumericUpDown numericViewerNum;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;


    }
}
