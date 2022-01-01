namespace ShellFiler.UI.Dialog.Option {
    partial class PrivacyGeneralPage {
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
            this.checkBoxFolder = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxViewer = new System.Windows.Forms.CheckBox();
            this.checkBoxCommand = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBoxFolder
            // 
            this.checkBoxFolder.AutoSize = true;
            this.checkBoxFolder.Location = new System.Drawing.Point(6, 28);
            this.checkBoxFolder.Name = "checkBoxFolder";
            this.checkBoxFolder.Size = new System.Drawing.Size(98, 16);
            this.checkBoxFolder.TabIndex = 1;
            this.checkBoxFolder.Text = "フォルダ履歴(&F)";
            this.checkBoxFolder.UseVisualStyleBackColor = true;
            this.checkBoxFolder.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(312, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "削除する履歴情報を選択して、[削除]ボタンをクリックしてください。";
            // 
            // checkBoxViewer
            // 
            this.checkBoxViewer.AutoSize = true;
            this.checkBoxViewer.Location = new System.Drawing.Point(6, 74);
            this.checkBoxViewer.Name = "checkBoxViewer";
            this.checkBoxViewer.Size = new System.Drawing.Size(109, 16);
            this.checkBoxViewer.TabIndex = 3;
            this.checkBoxViewer.Text = "ファイルビューア(&V)";
            this.checkBoxViewer.UseVisualStyleBackColor = true;
            this.checkBoxViewer.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBoxCommand
            // 
            this.checkBoxCommand.AutoSize = true;
            this.checkBoxCommand.Location = new System.Drawing.Point(6, 120);
            this.checkBoxCommand.Name = "checkBoxCommand";
            this.checkBoxCommand.Size = new System.Drawing.Size(99, 16);
            this.checkBoxCommand.TabIndex = 5;
            this.checkBoxCommand.Text = "コマンド履歴(&C)";
            this.checkBoxCommand.UseVisualStyleBackColor = true;
            this.checkBoxCommand.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(212, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "ファイル一覧で表示したフォルダの履歴です。";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(306, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "ファイルビューアの検索キーワードで入力した文字列の履歴です。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(168, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "実行したコマンドの入力履歴です。";
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(6, 167);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 7;
            this.buttonDelete.Text = "削除(&D)";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // PrivacyGeneralPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxCommand);
            this.Controls.Add(this.checkBoxViewer);
            this.Controls.Add(this.checkBoxFolder);
            this.Name = "PrivacyGeneralPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxViewer;
        private System.Windows.Forms.CheckBox checkBoxCommand;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonDelete;



    }
}
