namespace ShellFiler.UI.Dialog.Option {
    partial class FileListViewModePage {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxLeftThumbName = new System.Windows.Forms.ComboBox();
            this.comboBoxLeftThumbSize = new System.Windows.Forms.ComboBox();
            this.panelLeftViewSample = new System.Windows.Forms.Panel();
            this.radioButtonLeftThumb = new System.Windows.Forms.RadioButton();
            this.labelViewName = new System.Windows.Forms.Label();
            this.labelViewThumbSize = new System.Windows.Forms.Label();
            this.radioButtonLeftDetail = new System.Windows.Forms.RadioButton();
            this.radioButtonLeftPrev = new System.Windows.Forms.RadioButton();
            this.listViewFolder = new System.Windows.Forms.ListView();
            this.columnHeaderFolder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderViewMode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.buttonFolderDelete = new System.Windows.Forms.Button();
            this.buttonFolderAdd = new System.Windows.Forms.Button();
            this.buttonFolderEdit = new System.Windows.Forms.Button();
            this.buttonFolderDown = new System.Windows.Forms.Button();
            this.buttonFolderUp = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxLeftThumbName);
            this.groupBox1.Controls.Add(this.comboBoxLeftThumbSize);
            this.groupBox1.Controls.Add(this.panelLeftViewSample);
            this.groupBox1.Controls.Add(this.radioButtonLeftThumb);
            this.groupBox1.Controls.Add(this.labelViewName);
            this.groupBox1.Controls.Add(this.labelViewThumbSize);
            this.groupBox1.Controls.Add(this.radioButtonLeftDetail);
            this.groupBox1.Controls.Add(this.radioButtonLeftPrev);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(513, 176);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "フォルダ切り替え時の表示モード";
            // 
            // comboBoxLeftThumbName
            // 
            this.comboBoxLeftThumbName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLeftThumbName.FormattingEnabled = true;
            this.comboBoxLeftThumbName.Location = new System.Drawing.Point(48, 141);
            this.comboBoxLeftThumbName.Name = "comboBoxLeftThumbName";
            this.comboBoxLeftThumbName.Size = new System.Drawing.Size(155, 23);
            this.comboBoxLeftThumbName.TabIndex = 6;
            // 
            // comboBoxLeftThumbSize
            // 
            this.comboBoxLeftThumbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLeftThumbSize.FormattingEnabled = true;
            this.comboBoxLeftThumbSize.Location = new System.Drawing.Point(48, 99);
            this.comboBoxLeftThumbSize.Name = "comboBoxLeftThumbSize";
            this.comboBoxLeftThumbSize.Size = new System.Drawing.Size(155, 23);
            this.comboBoxLeftThumbSize.TabIndex = 4;
            // 
            // panelLeftViewSample
            // 
            this.panelLeftViewSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLeftViewSample.Location = new System.Drawing.Point(224, 18);
            this.panelLeftViewSample.Name = "panelLeftViewSample";
            this.panelLeftViewSample.Size = new System.Drawing.Size(283, 146);
            this.panelLeftViewSample.TabIndex = 7;
            // 
            // radioButtonLeftThumb
            // 
            this.radioButtonLeftThumb.AutoSize = true;
            this.radioButtonLeftThumb.Location = new System.Drawing.Point(6, 62);
            this.radioButtonLeftThumb.Name = "radioButtonLeftThumb";
            this.radioButtonLeftThumb.Size = new System.Drawing.Size(168, 19);
            this.radioButtonLeftThumb.TabIndex = 2;
            this.radioButtonLeftThumb.TabStop = true;
            this.radioButtonLeftThumb.Text = "サムネイル表示に変更する(&T)";
            this.radioButtonLeftThumb.UseVisualStyleBackColor = true;
            // 
            // labelViewName
            // 
            this.labelViewName.AutoSize = true;
            this.labelViewName.Location = new System.Drawing.Point(31, 124);
            this.labelViewName.Name = "labelViewName";
            this.labelViewName.Size = new System.Drawing.Size(135, 15);
            this.labelViewName.TabIndex = 5;
            this.labelViewName.Text = "ファイル名の表示方法(&N):";
            // 
            // labelViewThumbSize
            // 
            this.labelViewThumbSize.AutoSize = true;
            this.labelViewThumbSize.Location = new System.Drawing.Point(31, 82);
            this.labelViewThumbSize.Name = "labelViewThumbSize";
            this.labelViewThumbSize.Size = new System.Drawing.Size(142, 15);
            this.labelViewThumbSize.TabIndex = 3;
            this.labelViewThumbSize.Text = "サムネイル画像の大きさ(S):";
            // 
            // radioButtonLeftDetail
            // 
            this.radioButtonLeftDetail.AutoSize = true;
            this.radioButtonLeftDetail.Location = new System.Drawing.Point(6, 40);
            this.radioButtonLeftDetail.Name = "radioButtonLeftDetail";
            this.radioButtonLeftDetail.Size = new System.Drawing.Size(144, 19);
            this.radioButtonLeftDetail.TabIndex = 1;
            this.radioButtonLeftDetail.TabStop = true;
            this.radioButtonLeftDetail.Text = "詳細表示に変更する(&D)";
            this.radioButtonLeftDetail.UseVisualStyleBackColor = true;
            // 
            // radioButtonLeftPrev
            // 
            this.radioButtonLeftPrev.AutoSize = true;
            this.radioButtonLeftPrev.Location = new System.Drawing.Point(6, 18);
            this.radioButtonLeftPrev.Name = "radioButtonLeftPrev";
            this.radioButtonLeftPrev.Size = new System.Drawing.Size(152, 19);
            this.radioButtonLeftPrev.TabIndex = 0;
            this.radioButtonLeftPrev.TabStop = true;
            this.radioButtonLeftPrev.Text = "直前の状態を保持する(&L)";
            this.radioButtonLeftPrev.UseVisualStyleBackColor = true;
            // 
            // listViewFolder
            // 
            this.listViewFolder.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFolder,
            this.columnHeaderViewMode});
            this.listViewFolder.FullRowSelect = true;
            this.listViewFolder.HideSelection = false;
            this.listViewFolder.Location = new System.Drawing.Point(3, 213);
            this.listViewFolder.MultiSelect = false;
            this.listViewFolder.Name = "listViewFolder";
            this.listViewFolder.Size = new System.Drawing.Size(427, 154);
            this.listViewFolder.TabIndex = 2;
            this.listViewFolder.UseCompatibleStateImageBehavior = false;
            this.listViewFolder.View = System.Windows.Forms.View.Details;
            this.listViewFolder.SelectedIndexChanged += new System.EventHandler(this.listViewFolder_SelectedIndexChanged);
            // 
            // columnHeaderFolder
            // 
            this.columnHeaderFolder.Text = "フォルダ";
            this.columnHeaderFolder.Width = 270;
            // 
            // columnHeaderViewMode
            // 
            this.columnHeaderViewMode.Text = "表示方法";
            this.columnHeaderViewMode.Width = 130;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 196);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "特定フォルダでの切り替え(&F):";
            // 
            // buttonFolderDelete
            // 
            this.buttonFolderDelete.Location = new System.Drawing.Point(436, 271);
            this.buttonFolderDelete.Name = "buttonFolderDelete";
            this.buttonFolderDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonFolderDelete.TabIndex = 5;
            this.buttonFolderDelete.Text = "削除(&M)";
            this.buttonFolderDelete.UseVisualStyleBackColor = true;
            this.buttonFolderDelete.Click += new System.EventHandler(this.buttonFolderDelete_Click);
            // 
            // buttonFolderAdd
            // 
            this.buttonFolderAdd.Location = new System.Drawing.Point(436, 213);
            this.buttonFolderAdd.Name = "buttonFolderAdd";
            this.buttonFolderAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonFolderAdd.TabIndex = 3;
            this.buttonFolderAdd.Text = "追加(&A)...";
            this.buttonFolderAdd.UseVisualStyleBackColor = true;
            this.buttonFolderAdd.Click += new System.EventHandler(this.buttonFolderAdd_Click);
            // 
            // buttonFolderEdit
            // 
            this.buttonFolderEdit.Location = new System.Drawing.Point(436, 242);
            this.buttonFolderEdit.Name = "buttonFolderEdit";
            this.buttonFolderEdit.Size = new System.Drawing.Size(75, 23);
            this.buttonFolderEdit.TabIndex = 4;
            this.buttonFolderEdit.Text = "編集(&E)...";
            this.buttonFolderEdit.UseVisualStyleBackColor = true;
            this.buttonFolderEdit.Click += new System.EventHandler(this.buttonFolderEdit_Click);
            // 
            // buttonFolderDown
            // 
            this.buttonFolderDown.Location = new System.Drawing.Point(436, 344);
            this.buttonFolderDown.Name = "buttonFolderDown";
            this.buttonFolderDown.Size = new System.Drawing.Size(75, 23);
            this.buttonFolderDown.TabIndex = 7;
            this.buttonFolderDown.Text = "下へ(&W)";
            this.buttonFolderDown.UseVisualStyleBackColor = true;
            this.buttonFolderDown.Click += new System.EventHandler(this.buttonFolderDown_Click);
            // 
            // buttonFolderUp
            // 
            this.buttonFolderUp.Location = new System.Drawing.Point(436, 315);
            this.buttonFolderUp.Name = "buttonFolderUp";
            this.buttonFolderUp.Size = new System.Drawing.Size(75, 23);
            this.buttonFolderUp.TabIndex = 6;
            this.buttonFolderUp.Text = "上へ(&U)";
            this.buttonFolderUp.UseVisualStyleBackColor = true;
            this.buttonFolderUp.Click += new System.EventHandler(this.buttonFolderUp_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(304, 196);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "上から順に評価されます。";
            // 
            // FileListViewModePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.buttonFolderEdit);
            this.Controls.Add(this.buttonFolderAdd);
            this.Controls.Add(this.buttonFolderUp);
            this.Controls.Add(this.buttonFolderDown);
            this.Controls.Add(this.buttonFolderDelete);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewFolder);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "FileListViewModePage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonLeftDetail;
        private System.Windows.Forms.RadioButton radioButtonLeftPrev;
        private System.Windows.Forms.Label labelViewName;
        private System.Windows.Forms.Label labelViewThumbSize;
        private System.Windows.Forms.RadioButton radioButtonLeftThumb;
        private System.Windows.Forms.Panel panelLeftViewSample;
        private System.Windows.Forms.ComboBox comboBoxLeftThumbName;
        private System.Windows.Forms.ComboBox comboBoxLeftThumbSize;
        private System.Windows.Forms.ListView listViewFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonFolderDelete;
        private System.Windows.Forms.Button buttonFolderAdd;
        private System.Windows.Forms.Button buttonFolderEdit;
        private System.Windows.Forms.Button buttonFolderDown;
        private System.Windows.Forms.Button buttonFolderUp;
        private System.Windows.Forms.ColumnHeader columnHeaderFolder;
        private System.Windows.Forms.ColumnHeader columnHeaderViewMode;
        private System.Windows.Forms.Label label2;

    }
}
