namespace ShellFiler.UI.Dialog.Option {
    partial class SSHTerminalPage {
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
            this.radioShellWin = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioShellSSH = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioCloseStaySilent = new System.Windows.Forms.RadioButton();
            this.radioCloseStayMsg = new System.Windows.Forms.RadioButton();
            this.radioCloseClose = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelLineCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericLineCount = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericLineCount)).BeginInit();
            this.SuspendLayout();
            // 
            // radioShellWin
            // 
            this.radioShellWin.AutoSize = true;
            this.radioShellWin.Location = new System.Drawing.Point(6, 18);
            this.radioShellWin.Name = "radioShellWin";
            this.radioShellWin.Size = new System.Drawing.Size(246, 16);
            this.radioShellWin.TabIndex = 0;
            this.radioShellWin.TabStop = true;
            this.radioShellWin.Text = "常にWindowsのコマンドプロンプトを起動する(&W)";
            this.radioShellWin.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioShellSSH);
            this.groupBox1.Controls.Add(this.radioShellWin);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(6, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(511, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SSHフォルダからのシェルの起動方法";
            // 
            // radioShellSSH
            // 
            this.radioShellSSH.AutoSize = true;
            this.radioShellSSH.Location = new System.Drawing.Point(6, 40);
            this.radioShellSSH.Name = "radioShellSSH";
            this.radioShellSSH.Size = new System.Drawing.Size(257, 16);
            this.radioShellSSH.TabIndex = 1;
            this.radioShellSSH.TabStop = true;
            this.radioShellSSH.Text = "SSHの一覧の場合はSSHターミナルを起動する(&S)";
            this.radioShellSSH.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(126, 12);
            this.label7.TabIndex = 3;
            this.label7.Text = "実行したときの設定です。";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(27, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(442, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "ファイル一覧の画面でSSHのフォルダを表示しているときに、[実行]メニューから[シェル]コマンドを";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioCloseStaySilent);
            this.groupBox3.Controls.Add(this.radioCloseStayMsg);
            this.groupBox3.Controls.Add(this.radioCloseClose);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(6, 109);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(511, 130);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ターミナルウィンドウを閉じたときの動作";
            // 
            // radioCloseStaySilent
            // 
            this.radioCloseStaySilent.AutoSize = true;
            this.radioCloseStaySilent.Location = new System.Drawing.Point(6, 62);
            this.radioCloseStaySilent.Name = "radioCloseStaySilent";
            this.radioCloseStaySilent.Size = new System.Drawing.Size(381, 16);
            this.radioCloseStaySilent.TabIndex = 2;
            this.radioCloseStaySilent.TabStop = true;
            this.radioCloseStaySilent.Text = "ウィンドウを閉じてもシェルチャネルはそのままで、閉じるときのメッセージなし(&M)";
            this.radioCloseStaySilent.UseVisualStyleBackColor = true;
            // 
            // radioCloseStayMsg
            // 
            this.radioCloseStayMsg.AutoSize = true;
            this.radioCloseStayMsg.Location = new System.Drawing.Point(6, 40);
            this.radioCloseStayMsg.Name = "radioCloseStayMsg";
            this.radioCloseStayMsg.Size = new System.Drawing.Size(240, 16);
            this.radioCloseStayMsg.TabIndex = 1;
            this.radioCloseStayMsg.TabStop = true;
            this.radioCloseStayMsg.Text = "ウィンドウを閉じてもシェルチャネルはそのまま(&T)";
            this.radioCloseStayMsg.UseVisualStyleBackColor = true;
            // 
            // radioCloseClose
            // 
            this.radioCloseClose.AutoSize = true;
            this.radioCloseClose.Location = new System.Drawing.Point(6, 18);
            this.radioCloseClose.Name = "radioCloseClose";
            this.radioCloseClose.Size = new System.Drawing.Size(241, 16);
            this.radioCloseClose.TabIndex = 0;
            this.radioCloseClose.TabStop = true;
            this.radioCloseClose.Text = "ウィンドウを閉じたときシェルチャネルも閉じる(&C)";
            this.radioCloseClose.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(415, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "ターミナルを起動するときに既存のシェルチャネルを選択すれば元の画面を復元できます。";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(27, 81);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(404, 12);
            this.label8.TabIndex = 3;
            this.label8.Text = "1つのSSHセッションでは、複数のシェルチャネルを接続してターミナルから操作できます。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(420, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "ターミナルウィンドウを閉じてもシェルチャネルとコンソールの画面を残しておくことができます。";
            // 
            // labelLineCount
            // 
            this.labelLineCount.AutoSize = true;
            this.labelLineCount.Location = new System.Drawing.Point(272, 256);
            this.labelLineCount.Name = "labelLineCount";
            this.labelLineCount.Size = new System.Drawing.Size(65, 12);
            this.labelLineCount.TabIndex = 4;
            this.labelLineCount.Text = "行（{0}～{1}）";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 276);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(279, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "変更した場合は、次回に起動したターミナルから有効です。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 256);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "バックログの記憶行数(&L):";
            // 
            // numericLineCount
            // 
            this.numericLineCount.Location = new System.Drawing.Point(146, 254);
            this.numericLineCount.Name = "numericLineCount";
            this.numericLineCount.Size = new System.Drawing.Size(120, 19);
            this.numericLineCount.TabIndex = 3;
            // 
            // SSHTerminalPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelLineCount);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericLineCount);
            this.Name = "SSHTerminalPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericLineCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioShellWin;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioShellSSH;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioCloseStayMsg;
        private System.Windows.Forms.RadioButton radioCloseClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioCloseStaySilent;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelLineCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericLineCount;

    }
}
