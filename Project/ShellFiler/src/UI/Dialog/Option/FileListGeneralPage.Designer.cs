namespace ShellFiler.UI.Dialog.Option {
    partial class FileListGeneralPage {
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
            this.numericAutoUpdate = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxRefreshTabChange = new System.Windows.Forms.CheckBox();
            this.checkBoxRefreshSSH = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericAutoUpdate)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "自動更新までの時間(&A):";
            // 
            // numericAutoUpdate
            // 
            this.numericAutoUpdate.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericAutoUpdate.Location = new System.Drawing.Point(133, 3);
            this.numericAutoUpdate.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericAutoUpdate.Name = "numericAutoUpdate";
            this.numericAutoUpdate.Size = new System.Drawing.Size(120, 23);
            this.numericAutoUpdate.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(259, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "ミリ秒";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(387, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "0～5000の範囲で、500ms単位で設定できます。0のときは自動更新しません。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(396, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "ファイルシステムで更新を検出した後、ファイル一覧に反映するまでの待ち時間です。";
            // 
            // checkBoxRefreshTabChange
            // 
            this.checkBoxRefreshTabChange.AutoSize = true;
            this.checkBoxRefreshTabChange.Location = new System.Drawing.Point(5, 71);
            this.checkBoxRefreshTabChange.Name = "checkBoxRefreshTabChange";
            this.checkBoxRefreshTabChange.Size = new System.Drawing.Size(213, 19);
            this.checkBoxRefreshTabChange.TabIndex = 5;
            this.checkBoxRefreshTabChange.Text = "タブを切り替えたとき一覧を更新する(&T)";
            this.checkBoxRefreshTabChange.UseVisualStyleBackColor = true;
            // 
            // checkBoxRefreshSSH
            // 
            this.checkBoxRefreshSSH.AutoSize = true;
            this.checkBoxRefreshSSH.Location = new System.Drawing.Point(27, 91);
            this.checkBoxRefreshSSH.Name = "checkBoxRefreshSSH";
            this.checkBoxRefreshSSH.Size = new System.Drawing.Size(131, 19);
            this.checkBoxRefreshSSH.TabIndex = 6;
            this.checkBoxRefreshSSH.Text = "SSHでも更新する(&S)";
            this.checkBoxRefreshSSH.UseVisualStyleBackColor = true;
            // 
            // FileListGeneralPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.checkBoxRefreshSSH);
            this.Controls.Add(this.checkBoxRefreshTabChange);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericAutoUpdate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "FileListGeneralPage";
            this.Size = new System.Drawing.Size(520, 370);
            ((System.ComponentModel.ISupportInitialize)(this.numericAutoUpdate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericAutoUpdate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxRefreshTabChange;
        private System.Windows.Forms.CheckBox checkBoxRefreshSSH;
    }
}
