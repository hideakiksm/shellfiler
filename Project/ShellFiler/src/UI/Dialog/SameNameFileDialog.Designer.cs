namespace ShellFiler.UI.Dialog {
    partial class SameNameFileDialog {
        /// <summary>
        /// 必要なデザイナー変数です。
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

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.radioButtonForce = new System.Windows.Forms.RadioButton();
            this.radioButtonIfNewer = new System.Windows.Forms.RadioButton();
            this.radioButtonRename = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonNotOverwrite = new System.Windows.Forms.RadioButton();
            this.radioButtonAuto = new System.Windows.Forms.RadioButton();
            this.radioButtonFullAuto = new System.Windows.Forms.RadioButton();
            this.comboBoxAutoRename = new System.Windows.Forms.ComboBox();
            this.textBoxRename = new System.Windows.Forms.TextBox();
            this.checkBoxApplyAll = new System.Windows.Forms.CheckBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelAutoRename = new System.Windows.Forms.Label();
            this.linkLabelFullAuto = new System.Windows.Forms.LinkLabel();
            this.sameNameFileInfo = new ShellFiler.UI.Dialog.SameNameFileInfoControl();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonForce
            // 
            this.radioButtonForce.AutoSize = true;
            this.radioButtonForce.Location = new System.Drawing.Point(13, 18);
            this.radioButtonForce.Name = "radioButtonForce";
            this.radioButtonForce.Size = new System.Drawing.Size(122, 19);
            this.radioButtonForce.TabIndex = 0;
            this.radioButtonForce.TabStop = true;
            this.radioButtonForce.Text = "強制的に上書き(&O)";
            this.radioButtonForce.UseVisualStyleBackColor = true;
            this.radioButtonForce.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonIfNewer
            // 
            this.radioButtonIfNewer.AutoSize = true;
            this.radioButtonIfNewer.Location = new System.Drawing.Point(13, 38);
            this.radioButtonIfNewer.Name = "radioButtonIfNewer";
            this.radioButtonIfNewer.Size = new System.Drawing.Size(163, 19);
            this.radioButtonIfNewer.TabIndex = 0;
            this.radioButtonIfNewer.Text = "自分が新しければ上書き(&U)";
            this.radioButtonIfNewer.UseVisualStyleBackColor = true;
            this.radioButtonIfNewer.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonRename
            // 
            this.radioButtonRename.AutoSize = true;
            this.radioButtonRename.Location = new System.Drawing.Point(13, 58);
            this.radioButtonRename.Name = "radioButtonRename";
            this.radioButtonRename.Size = new System.Drawing.Size(142, 19);
            this.radioButtonRename.TabIndex = 0;
            this.radioButtonRename.Text = "名前を変更して転送(&R)";
            this.radioButtonRename.UseVisualStyleBackColor = true;
            this.radioButtonRename.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonForce);
            this.groupBox1.Controls.Add(this.radioButtonIfNewer);
            this.groupBox1.Controls.Add(this.radioButtonRename);
            this.groupBox1.Controls.Add(this.radioButtonNotOverwrite);
            this.groupBox1.Controls.Add(this.radioButtonAuto);
            this.groupBox1.Controls.Add(this.radioButtonFullAuto);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(301, 169);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "操作(SHIFT:すべてに適用)";
            // 
            // radioButtonNotOverwrite
            // 
            this.radioButtonNotOverwrite.AutoSize = true;
            this.radioButtonNotOverwrite.Location = new System.Drawing.Point(13, 104);
            this.radioButtonNotOverwrite.Name = "radioButtonNotOverwrite";
            this.radioButtonNotOverwrite.Size = new System.Drawing.Size(97, 19);
            this.radioButtonNotOverwrite.TabIndex = 0;
            this.radioButtonNotOverwrite.Text = "転送しない(&N)";
            this.radioButtonNotOverwrite.UseVisualStyleBackColor = true;
            this.radioButtonNotOverwrite.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonAuto
            // 
            this.radioButtonAuto.AutoSize = true;
            this.radioButtonAuto.Location = new System.Drawing.Point(13, 124);
            this.radioButtonAuto.Name = "radioButtonAuto";
            this.radioButtonAuto.Size = new System.Drawing.Size(209, 19);
            this.radioButtonAuto.TabIndex = 0;
            this.radioButtonAuto.Text = "ファイル名を自動的に変更して転送(&A)";
            this.radioButtonAuto.UseVisualStyleBackColor = true;
            this.radioButtonAuto.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonFullAuto
            // 
            this.radioButtonFullAuto.AutoSize = true;
            this.radioButtonFullAuto.Location = new System.Drawing.Point(13, 146);
            this.radioButtonFullAuto.Name = "radioButtonFullAuto";
            this.radioButtonFullAuto.Size = new System.Drawing.Size(266, 19);
            this.radioButtonFullAuto.TabIndex = 0;
            this.radioButtonFullAuto.Text = "状況判断でファイル名を自動的に変更して転送(&F)";
            this.radioButtonFullAuto.UseVisualStyleBackColor = true;
            this.radioButtonFullAuto.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // comboBoxAutoRename
            // 
            this.comboBoxAutoRename.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAutoRename.FormattingEnabled = true;
            this.comboBoxAutoRename.Location = new System.Drawing.Point(30, 207);
            this.comboBoxAutoRename.Name = "comboBoxAutoRename";
            this.comboBoxAutoRename.Size = new System.Drawing.Size(272, 23);
            this.comboBoxAutoRename.TabIndex = 2;
            // 
            // textBoxRename
            // 
            this.textBoxRename.Location = new System.Drawing.Point(30, 91);
            this.textBoxRename.Name = "textBoxRename";
            this.textBoxRename.Size = new System.Drawing.Size(272, 23);
            this.textBoxRename.TabIndex = 1;
            // 
            // checkBoxApplyAll
            // 
            this.checkBoxApplyAll.AutoSize = true;
            this.checkBoxApplyAll.Location = new System.Drawing.Point(12, 239);
            this.checkBoxApplyAll.Name = "checkBoxApplyAll";
            this.checkBoxApplyAll.Size = new System.Drawing.Size(150, 19);
            this.checkBoxApplyAll.TabIndex = 3;
            this.checkBoxApplyAll.Text = "すべてのファイルに適用(&Z)";
            this.checkBoxApplyAll.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(406, 317);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(487, 317);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "転送中止";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelAutoRename
            // 
            this.labelAutoRename.AutoSize = true;
            this.labelAutoRename.Location = new System.Drawing.Point(12, 189);
            this.labelAutoRename.Name = "labelAutoRename";
            this.labelAutoRename.Size = new System.Drawing.Size(174, 15);
            this.labelAutoRename.TabIndex = 6;
            this.labelAutoRename.Text = "ファイル名を自動的に変更する方法";
            // 
            // linkLabelFullAuto
            // 
            this.linkLabelFullAuto.AutoSize = true;
            this.linkLabelFullAuto.Location = new System.Drawing.Point(292, 160);
            this.linkLabelFullAuto.Name = "linkLabelFullAuto";
            this.linkLabelFullAuto.Size = new System.Drawing.Size(13, 15);
            this.linkLabelFullAuto.TabIndex = 2;
            this.linkLabelFullAuto.TabStop = true;
            this.linkLabelFullAuto.Text = "?";
            this.linkLabelFullAuto.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelFullAuto_LinkClicked);
            // 
            // sameNameFileInfo
            // 
            this.sameNameFileInfo.BackColor = System.Drawing.SystemColors.Control;
            this.sameNameFileInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sameNameFileInfo.Location = new System.Drawing.Point(342, 12);
            this.sameNameFileInfo.Name = "sameNameFileInfo";
            this.sameNameFileInfo.Size = new System.Drawing.Size(220, 298);
            this.sameNameFileInfo.TabIndex = 5;
            this.sameNameFileInfo.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 263);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(321, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "O,U,N,A,F:操作を決定   SHIFT+決定:すべてのファイルに適用";
            // 
            // SameNameFileDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(576, 352);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxRename);
            this.Controls.Add(this.linkLabelFullAuto);
            this.Controls.Add(this.labelAutoRename);
            this.Controls.Add(this.comboBoxAutoRename);
            this.Controls.Add(this.checkBoxApplyAll);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.sameNameFileInfo);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SameNameFileDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "同名のファイル";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SameNameFileDialog_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SameNameFileDialog_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonForce;
        private System.Windows.Forms.RadioButton radioButtonIfNewer;
        private System.Windows.Forms.RadioButton radioButtonRename;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxAutoRename;
        private System.Windows.Forms.RadioButton radioButtonFullAuto;
        private System.Windows.Forms.RadioButton radioButtonAuto;
        private System.Windows.Forms.RadioButton radioButtonNotOverwrite;
        private System.Windows.Forms.TextBox textBoxRename;
        private System.Windows.Forms.CheckBox checkBoxApplyAll;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private SameNameFileInfoControl sameNameFileInfo;
        private System.Windows.Forms.LinkLabel linkLabelFullAuto;
        private System.Windows.Forms.Label labelAutoRename;
        private System.Windows.Forms.Label label1;
    }
}