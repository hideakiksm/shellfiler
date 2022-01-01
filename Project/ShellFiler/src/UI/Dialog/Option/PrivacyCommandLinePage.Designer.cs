namespace ShellFiler.UI.Dialog.Option {
    partial class PrivacyCommandLinePage {
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
            this.buttonCommandDel = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxCommandSave = new System.Windows.Forms.CheckBox();
            this.numericCommandNum = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericCommandNum)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCommandDel
            // 
            this.buttonCommandDel.Location = new System.Drawing.Point(8, 111);
            this.buttonCommandDel.Name = "buttonCommandDel";
            this.buttonCommandDel.Size = new System.Drawing.Size(95, 23);
            this.buttonCommandDel.TabIndex = 5;
            this.buttonCommandDel.Text = "履歴の削除(&M)";
            this.buttonCommandDel.UseVisualStyleBackColor = true;
            this.buttonCommandDel.Click += new System.EventHandler(this.buttonCommandDel_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(251, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(140, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "(変更は次回起動時に有効)";
            // 
            // checkBoxCommandSave
            // 
            this.checkBoxCommandSave.AutoSize = true;
            this.checkBoxCommandSave.Location = new System.Drawing.Point(8, 78);
            this.checkBoxCommandSave.Name = "checkBoxCommandSave";
            this.checkBoxCommandSave.Size = new System.Drawing.Size(196, 16);
            this.checkBoxCommandSave.TabIndex = 3;
            this.checkBoxCommandSave.Text = "コマンド履歴をディスクに保存する(&D):";
            this.checkBoxCommandSave.UseVisualStyleBackColor = true;
            // 
            // numericCommandNum
            // 
            this.numericCommandNum.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericCommandNum.Location = new System.Drawing.Point(107, 53);
            this.numericCommandNum.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericCommandNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericCommandNum.Name = "numericCommandNum";
            this.numericCommandNum.Size = new System.Drawing.Size(120, 19);
            this.numericCommandNum.TabIndex = 1;
            this.numericCommandNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "最大記憶件数(&C):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "コマンドラインの履歴";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 23);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(170, 12);
            this.label10.TabIndex = 7;
            this.label10.Text = "入力したコマンドラインの履歴です。";
            // 
            // PrivacyCommandLinePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonCommandDel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkBoxCommandSave);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericCommandNum);
            this.Name = "PrivacyCommandLinePage";
            this.Size = new System.Drawing.Size(520, 370);
            ((System.ComponentModel.ISupportInitialize)(this.numericCommandNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericCommandNum;
        private System.Windows.Forms.Button buttonCommandDel;
        private System.Windows.Forms.CheckBox checkBoxCommandSave;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label10;


    }
}
