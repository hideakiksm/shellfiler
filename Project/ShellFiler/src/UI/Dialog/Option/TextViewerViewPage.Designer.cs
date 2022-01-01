namespace ShellFiler.UI.Dialog.Option {
    partial class TextViewerViewPage {
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
            this.checkBoxCrLf = new System.Windows.Forms.CheckBox();
            this.checkBoxCtrl = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkBoxCrLf
            // 
            this.checkBoxCrLf.AutoSize = true;
            this.checkBoxCrLf.Location = new System.Drawing.Point(3, 3);
            this.checkBoxCrLf.Name = "checkBoxCrLf";
            this.checkBoxCrLf.Size = new System.Drawing.Size(126, 16);
            this.checkBoxCrLf.TabIndex = 0;
            this.checkBoxCrLf.Text = "行番号を表示する(&L)";
            this.checkBoxCrLf.UseVisualStyleBackColor = true;
            // 
            // checkBoxCtrl
            // 
            this.checkBoxCtrl.AutoSize = true;
            this.checkBoxCtrl.Location = new System.Drawing.Point(3, 45);
            this.checkBoxCtrl.Name = "checkBoxCtrl";
            this.checkBoxCtrl.Size = new System.Drawing.Size(140, 16);
            this.checkBoxCtrl.TabIndex = 2;
            this.checkBoxCtrl.Text = "制御文字を表示する(&C)";
            this.checkBoxCtrl.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(290, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "制御文字として、次の情報を表示するかどうかを指定します。";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(211, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "・各行の末端の改行コード(CRまたはCRLF)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "・ファイル末端の[EOF]マーク";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(399, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "・表示されたファイルの末尾以降に読み込まれていない部分があることを示す[...]マーク";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "変更結果は次に起動するビューアから有効です。";
            // 
            // TextViewerViewPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxCtrl);
            this.Controls.Add(this.checkBoxCrLf);
            this.Name = "TextViewerViewPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxCrLf;
        private System.Windows.Forms.CheckBox checkBoxCtrl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;




    }
}
