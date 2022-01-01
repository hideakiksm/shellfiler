namespace ShellFiler.UI.Dialog.Option {
    partial class InstallDiffPage {
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDiffRef = new System.Windows.Forms.Button();
            this.textBoxDiffCommand = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxDiffDirectoryCommand = new System.Windows.Forms.TextBox();
            this.buttonDiffDirectoryRef = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(470, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "差分表示ツールをShellFilerと別にインストールして登録しておくと、ファイル一覧やファイルビューアから";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "ファイル用差分表示ツール";
            // 
            // buttonDiffRef
            // 
            this.buttonDiffRef.Location = new System.Drawing.Point(424, 17);
            this.buttonDiffRef.Name = "buttonDiffRef";
            this.buttonDiffRef.Size = new System.Drawing.Size(75, 23);
            this.buttonDiffRef.TabIndex = 2;
            this.buttonDiffRef.Text = "参照(&R)...";
            this.buttonDiffRef.UseVisualStyleBackColor = true;
            this.buttonDiffRef.Click += new System.EventHandler(this.buttonDiffRef_Click);
            // 
            // textBoxDiffCommand
            // 
            this.textBoxDiffCommand.Location = new System.Drawing.Point(19, 19);
            this.textBoxDiffCommand.Name = "textBoxDiffCommand";
            this.textBoxDiffCommand.Size = new System.Drawing.Size(399, 19);
            this.textBoxDiffCommand.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(244, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "簡単にテキストの相違点を表示することができます。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(422, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "入力文字列の{0}の位置に、比較対象のファイルやフォルダを複数個指定して起動します。";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 184);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(478, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "差分表示ツールの指定がない場合、ShellFiler\\binフォルダにdiff.lnkというショートカットを作っておくと、";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 196);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(380, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "リンク先を差分表示ツールとして起動します（無料版ではこちらをお使いください）。";
            // 
            // textBoxDiffDirectoryCommand
            // 
            this.textBoxDiffDirectoryCommand.Location = new System.Drawing.Point(19, 72);
            this.textBoxDiffDirectoryCommand.Name = "textBoxDiffDirectoryCommand";
            this.textBoxDiffDirectoryCommand.Size = new System.Drawing.Size(399, 19);
            this.textBoxDiffDirectoryCommand.TabIndex = 9;
            // 
            // buttonDiffDirectoryRef
            // 
            this.buttonDiffDirectoryRef.Location = new System.Drawing.Point(424, 70);
            this.buttonDiffDirectoryRef.Name = "buttonDiffDirectoryRef";
            this.buttonDiffDirectoryRef.Size = new System.Drawing.Size(75, 23);
            this.buttonDiffDirectoryRef.TabIndex = 10;
            this.buttonDiffDirectoryRef.Text = "参照(&E)...";
            this.buttonDiffDirectoryRef.UseVisualStyleBackColor = true;
            this.buttonDiffDirectoryRef.Click += new System.EventHandler(this.buttonDiffDirectoryRef_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(143, 12);
            this.label7.TabIndex = 8;
            this.label7.Text = "ディレクトリ用差分表示ツール";
            // 
            // InstallDiffPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxDiffDirectoryCommand);
            this.Controls.Add(this.buttonDiffDirectoryRef);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxDiffCommand);
            this.Controls.Add(this.buttonDiffRef);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Name = "InstallDiffPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonDiffRef;
        private System.Windows.Forms.TextBox textBoxDiffCommand;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxDiffDirectoryCommand;
        private System.Windows.Forms.Button buttonDiffDirectoryRef;
        private System.Windows.Forms.Label label7;

    }
}
