namespace ShellFiler.UI.Dialog.Option {
    partial class FileOperationMarklessPage {
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
            this.checkBoxCopy = new System.Windows.Forms.CheckBox();
            this.checkBoxMove = new System.Windows.Forms.CheckBox();
            this.checkBoxDelete = new System.Windows.Forms.CheckBox();
            this.checkBoxShortcut = new System.Windows.Forms.CheckBox();
            this.checkBoxAttribute = new System.Windows.Forms.CheckBox();
            this.checkBoxPack = new System.Windows.Forms.CheckBox();
            this.checkBoxUnpack = new System.Windows.Forms.CheckBox();
            this.checkBoxEdit = new System.Windows.Forms.CheckBox();
            this.labelCopy = new System.Windows.Forms.Label();
            this.labelMove = new System.Windows.Forms.Label();
            this.labelDelete = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxFolderSize = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "マークなし操作";
            // 
            // checkBoxCopy
            // 
            this.checkBoxCopy.AutoSize = true;
            this.checkBoxCopy.Location = new System.Drawing.Point(17, 20);
            this.checkBoxCopy.Name = "checkBoxCopy";
            this.checkBoxCopy.Size = new System.Drawing.Size(212, 19);
            this.checkBoxCopy.TabIndex = 1;
            this.checkBoxCopy.Text = "コピーでマークなし操作を有効にする(&C)";
            this.checkBoxCopy.UseVisualStyleBackColor = true;
            this.checkBoxCopy.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBoxMove
            // 
            this.checkBoxMove.AutoSize = true;
            this.checkBoxMove.Location = new System.Drawing.Point(17, 42);
            this.checkBoxMove.Name = "checkBoxMove";
            this.checkBoxMove.Size = new System.Drawing.Size(211, 19);
            this.checkBoxMove.TabIndex = 3;
            this.checkBoxMove.Text = "移動でマークなし操作を有効にする(&M)";
            this.checkBoxMove.UseVisualStyleBackColor = true;
            this.checkBoxMove.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBoxDelete
            // 
            this.checkBoxDelete.AutoSize = true;
            this.checkBoxDelete.Location = new System.Drawing.Point(17, 64);
            this.checkBoxDelete.Name = "checkBoxDelete";
            this.checkBoxDelete.Size = new System.Drawing.Size(210, 19);
            this.checkBoxDelete.TabIndex = 5;
            this.checkBoxDelete.Text = "削除でマークなし操作を有効にする(&D)";
            this.checkBoxDelete.UseVisualStyleBackColor = true;
            this.checkBoxDelete.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBoxShortcut
            // 
            this.checkBoxShortcut.AutoSize = true;
            this.checkBoxShortcut.Location = new System.Drawing.Point(17, 86);
            this.checkBoxShortcut.Name = "checkBoxShortcut";
            this.checkBoxShortcut.Size = new System.Drawing.Size(277, 19);
            this.checkBoxShortcut.TabIndex = 7;
            this.checkBoxShortcut.Text = "ショートカットの作成でマークなし操作を有効にする(&T)";
            this.checkBoxShortcut.UseVisualStyleBackColor = true;
            // 
            // checkBoxAttribute
            // 
            this.checkBoxAttribute.AutoSize = true;
            this.checkBoxAttribute.Location = new System.Drawing.Point(17, 108);
            this.checkBoxAttribute.Name = "checkBoxAttribute";
            this.checkBoxAttribute.Size = new System.Drawing.Size(301, 19);
            this.checkBoxAttribute.TabIndex = 8;
            this.checkBoxAttribute.Text = "ファイル属性の一括編集でマークなし操作を有効にする(&A)";
            this.checkBoxAttribute.UseVisualStyleBackColor = true;
            // 
            // checkBoxPack
            // 
            this.checkBoxPack.AutoSize = true;
            this.checkBoxPack.Location = new System.Drawing.Point(17, 130);
            this.checkBoxPack.Name = "checkBoxPack";
            this.checkBoxPack.Size = new System.Drawing.Size(208, 19);
            this.checkBoxPack.TabIndex = 9;
            this.checkBoxPack.Text = "圧縮でマークなし操作を有効にする(&P)";
            this.checkBoxPack.UseVisualStyleBackColor = true;
            // 
            // checkBoxUnpack
            // 
            this.checkBoxUnpack.AutoSize = true;
            this.checkBoxUnpack.Location = new System.Drawing.Point(17, 152);
            this.checkBoxUnpack.Name = "checkBoxUnpack";
            this.checkBoxUnpack.Size = new System.Drawing.Size(210, 19);
            this.checkBoxUnpack.TabIndex = 10;
            this.checkBoxUnpack.Text = "展開でマークなし操作を有効にする(&U)";
            this.checkBoxUnpack.UseVisualStyleBackColor = true;
            // 
            // checkBoxEdit
            // 
            this.checkBoxEdit.AutoSize = true;
            this.checkBoxEdit.Location = new System.Drawing.Point(17, 174);
            this.checkBoxEdit.Name = "checkBoxEdit";
            this.checkBoxEdit.Size = new System.Drawing.Size(208, 19);
            this.checkBoxEdit.TabIndex = 11;
            this.checkBoxEdit.Text = "編集でマークなし操作を有効にする(&E)";
            this.checkBoxEdit.UseVisualStyleBackColor = true;
            // 
            // labelCopy
            // 
            this.labelCopy.AutoSize = true;
            this.labelCopy.BackColor = System.Drawing.Color.Yellow;
            this.labelCopy.Location = new System.Drawing.Point(242, 21);
            this.labelCopy.Name = "labelCopy";
            this.labelCopy.Size = new System.Drawing.Size(264, 15);
            this.labelCopy.TabIndex = 2;
            this.labelCopy.Text = "誤操作を防止するため、設定のOFFをおすすめします。";
            this.labelCopy.Visible = false;
            // 
            // labelMove
            // 
            this.labelMove.AutoSize = true;
            this.labelMove.BackColor = System.Drawing.Color.Yellow;
            this.labelMove.Location = new System.Drawing.Point(242, 43);
            this.labelMove.Name = "labelMove";
            this.labelMove.Size = new System.Drawing.Size(264, 15);
            this.labelMove.TabIndex = 4;
            this.labelMove.Text = "誤操作を防止するため、設定のOFFをおすすめします。";
            this.labelMove.Visible = false;
            // 
            // labelDelete
            // 
            this.labelDelete.AutoSize = true;
            this.labelDelete.BackColor = System.Drawing.Color.Yellow;
            this.labelDelete.Location = new System.Drawing.Point(242, 65);
            this.labelDelete.Name = "labelDelete";
            this.labelDelete.Size = new System.Drawing.Size(264, 15);
            this.labelDelete.TabIndex = 6;
            this.labelDelete.Text = "誤操作を防止するため、設定のOFFをおすすめします。";
            this.labelDelete.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 226);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(481, 15);
            this.label3.TabIndex = 13;
            this.label3.Text = "マークなし操作を有効にすると、ファイルを選択状態にしていなくても、カーソル位置のファイルを選択した";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 242);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 15);
            this.label2.TabIndex = 14;
            this.label2.Text = "ものとして処理を実行できます。";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // checkBoxFolderSize
            // 
            this.checkBoxFolderSize.AutoSize = true;
            this.checkBoxFolderSize.Location = new System.Drawing.Point(17, 196);
            this.checkBoxFolderSize.Name = "checkBoxFolderSize";
            this.checkBoxFolderSize.Size = new System.Drawing.Size(283, 19);
            this.checkBoxFolderSize.TabIndex = 12;
            this.checkBoxFolderSize.Text = "フォルダサイズの表示でマークなし操作を有効にする(&S)";
            this.checkBoxFolderSize.UseVisualStyleBackColor = true;
            // 
            // FileOperationMarklessPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.checkBoxFolderSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelDelete);
            this.Controls.Add(this.labelMove);
            this.Controls.Add(this.labelCopy);
            this.Controls.Add(this.checkBoxEdit);
            this.Controls.Add(this.checkBoxUnpack);
            this.Controls.Add(this.checkBoxPack);
            this.Controls.Add(this.checkBoxAttribute);
            this.Controls.Add(this.checkBoxShortcut);
            this.Controls.Add(this.checkBoxDelete);
            this.Controls.Add(this.checkBoxMove);
            this.Controls.Add(this.checkBoxCopy);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "FileOperationMarklessPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxCopy;
        private System.Windows.Forms.CheckBox checkBoxMove;
        private System.Windows.Forms.CheckBox checkBoxDelete;
        private System.Windows.Forms.CheckBox checkBoxShortcut;
        private System.Windows.Forms.CheckBox checkBoxAttribute;
        private System.Windows.Forms.CheckBox checkBoxPack;
        private System.Windows.Forms.CheckBox checkBoxUnpack;
        private System.Windows.Forms.CheckBox checkBoxEdit;
        private System.Windows.Forms.Label labelCopy;
        private System.Windows.Forms.Label labelMove;
        private System.Windows.Forms.Label labelDelete;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxFolderSize;


    }
}
