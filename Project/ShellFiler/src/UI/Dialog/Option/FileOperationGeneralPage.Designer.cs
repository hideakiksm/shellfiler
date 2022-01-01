namespace ShellFiler.UI.Dialog.Option {
    partial class FileOperationGeneralPage {
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
            this.numericBackgroundTask = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericEtcTask = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericBackgroundTask)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericEtcTask)).BeginInit();
            this.SuspendLayout();
            // 
            // numericBackgroundTask
            // 
            this.numericBackgroundTask.Location = new System.Drawing.Point(212, 18);
            this.numericBackgroundTask.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericBackgroundTask.Name = "numericBackgroundTask";
            this.numericBackgroundTask.Size = new System.Drawing.Size(120, 19);
            this.numericBackgroundTask.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericEtcTask);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numericBackgroundTask);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(5, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(512, 166);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "バックグラウンドタスクの最大同時実行数";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(174, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "ファイル操作の最大同時実行数(&T):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(35, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(451, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "ファイルのコピー、移動、圧縮、属性の一括変更などのファイル操作を同時に実行できる数です。";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(35, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(331, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "最大実行数を超えたものは、実行中の処理が終わるまで待機します。";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(171, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "その他処理の最大同時実行数(&E):";
            // 
            // numericEtcTask
            // 
            this.numericEtcTask.Location = new System.Drawing.Point(212, 94);
            this.numericEtcTask.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericEtcTask.Name = "numericEtcTask";
            this.numericEtcTask.Size = new System.Drawing.Size(120, 19);
            this.numericEtcTask.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(35, 118);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(444, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "フォルダ作成や、SSHでのアップロード／ダウンロード、HTTPレスポンスビューアの応答待ちなど、";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(35, 130);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(255, 12);
            this.label9.TabIndex = 2;
            this.label9.Text = "ファイル操作以外の処理を同時に実行できる数です。";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(35, 142);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(218, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "同時実行数を超える処理は開始できません。";
            // 
            // FileOperationGeneralPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "FileOperationGeneralPage";
            this.Size = new System.Drawing.Size(520, 370);
            ((System.ComponentModel.ISupportInitialize)(this.numericBackgroundTask)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericEtcTask)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericBackgroundTask;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericEtcTask;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
    }
}
