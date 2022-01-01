namespace ShellFiler.UI.Dialog.Option {
    partial class InstallEditorPage {
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
            this.textBoxEditor = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxEditorSSH = new System.Windows.Forms.CheckBox();
            this.linkLabelSSHHelp = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonEditorFix = new System.Windows.Forms.RadioButton();
            this.textBoxEditorAuto = new System.Windows.Forms.TextBox();
            this.radioButtonEditorAuto = new System.Windows.Forms.RadioButton();
            this.textBoxEditorSSH = new System.Windows.Forms.TextBox();
            this.buttonEditorRefSSH = new System.Windows.Forms.Button();
            this.buttonEditorRef = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxLineSSH = new System.Windows.Forms.CheckBox();
            this.radioButtonLineSpecify = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.radioButtonLineNone = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxEditorLineNumSSH = new System.Windows.Forms.TextBox();
            this.buttonEditorLineNumRefSSH = new System.Windows.Forms.Button();
            this.textBoxEditorLineNum = new System.Windows.Forms.TextBox();
            this.buttonEditorLineNumRef = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxEditor
            // 
            this.textBoxEditor.Location = new System.Drawing.Point(150, 85);
            this.textBoxEditor.Name = "textBoxEditor";
            this.textBoxEditor.Size = new System.Drawing.Size(277, 19);
            this.textBoxEditor.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(215, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "テキストエディタのコマンドラインを指定します。";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxEditorSSH);
            this.groupBox2.Controls.Add(this.linkLabelSSHHelp);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.radioButtonEditorFix);
            this.groupBox2.Controls.Add(this.textBoxEditorAuto);
            this.groupBox2.Controls.Add(this.radioButtonEditorAuto);
            this.groupBox2.Controls.Add(this.textBoxEditorSSH);
            this.groupBox2.Controls.Add(this.textBoxEditor);
            this.groupBox2.Controls.Add(this.buttonEditorRefSSH);
            this.groupBox2.Controls.Add(this.buttonEditorRef);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(514, 168);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "テキストエディタ";
            // 
            // checkBoxEditorSSH
            // 
            this.checkBoxEditorSSH.AutoSize = true;
            this.checkBoxEditorSSH.Location = new System.Drawing.Point(28, 112);
            this.checkBoxEditorSSH.Name = "checkBoxEditorSSH";
            this.checkBoxEditorSSH.Size = new System.Drawing.Size(116, 16);
            this.checkBoxEditorSSH.TabIndex = 6;
            this.checkBoxEditorSSH.Text = "SSHは別に指定(&S)";
            this.checkBoxEditorSSH.UseVisualStyleBackColor = true;
            this.checkBoxEditorSSH.CheckedChanged += new System.EventHandler(this.checkBoxEditorSSH_CheckedChanged);
            // 
            // linkLabelSSHHelp
            // 
            this.linkLabelSSHHelp.AutoSize = true;
            this.linkLabelSSHHelp.Location = new System.Drawing.Point(407, 134);
            this.linkLabelSSHHelp.Name = "linkLabelSSHHelp";
            this.linkLabelSSHHelp.Size = new System.Drawing.Size(101, 12);
            this.linkLabelSSHHelp.TabIndex = 11;
            this.linkLabelSSHHelp.TabStop = true;
            this.linkLabelSSHHelp.Text = "SSH用のエディタとは";
            this.linkLabelSSHHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSSHHelp_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "通常のファイル(&G):";
            // 
            // radioButtonEditorFix
            // 
            this.radioButtonEditorFix.AutoSize = true;
            this.radioButtonEditorFix.Location = new System.Drawing.Point(8, 65);
            this.radioButtonEditorFix.Name = "radioButtonEditorFix";
            this.radioButtonEditorFix.Size = new System.Drawing.Size(212, 16);
            this.radioButtonEditorFix.TabIndex = 2;
            this.radioButtonEditorFix.TabStop = true;
            this.radioButtonEditorFix.Text = "テキストエディタのコマンドラインを指定(&E)";
            this.radioButtonEditorFix.UseVisualStyleBackColor = true;
            this.radioButtonEditorFix.CheckedChanged += new System.EventHandler(this.radioButtonEditor_CheckedChanged);
            // 
            // textBoxEditorAuto
            // 
            this.textBoxEditorAuto.Location = new System.Drawing.Point(28, 40);
            this.textBoxEditorAuto.Name = "textBoxEditorAuto";
            this.textBoxEditorAuto.ReadOnly = true;
            this.textBoxEditorAuto.Size = new System.Drawing.Size(399, 19);
            this.textBoxEditorAuto.TabIndex = 1;
            // 
            // radioButtonEditorAuto
            // 
            this.radioButtonEditorAuto.AutoSize = true;
            this.radioButtonEditorAuto.Location = new System.Drawing.Point(8, 18);
            this.radioButtonEditorAuto.Name = "radioButtonEditorAuto";
            this.radioButtonEditorAuto.Size = new System.Drawing.Size(179, 16);
            this.radioButtonEditorAuto.TabIndex = 0;
            this.radioButtonEditorAuto.TabStop = true;
            this.radioButtonEditorAuto.Text = "拡張子.txtの関連付けを使用(&T)";
            this.radioButtonEditorAuto.UseVisualStyleBackColor = true;
            this.radioButtonEditorAuto.CheckedChanged += new System.EventHandler(this.radioButtonEditor_CheckedChanged);
            // 
            // textBoxEditorSSH
            // 
            this.textBoxEditorSSH.Location = new System.Drawing.Point(150, 110);
            this.textBoxEditorSSH.Name = "textBoxEditorSSH";
            this.textBoxEditorSSH.Size = new System.Drawing.Size(277, 19);
            this.textBoxEditorSSH.TabIndex = 7;
            // 
            // buttonEditorRefSSH
            // 
            this.buttonEditorRefSSH.Location = new System.Drawing.Point(433, 108);
            this.buttonEditorRefSSH.Name = "buttonEditorRefSSH";
            this.buttonEditorRefSSH.Size = new System.Drawing.Size(75, 23);
            this.buttonEditorRefSSH.TabIndex = 8;
            this.buttonEditorRefSSH.Text = "参照(&H)...";
            this.buttonEditorRefSSH.UseVisualStyleBackColor = true;
            this.buttonEditorRefSSH.Click += new System.EventHandler(this.buttonEditorRef_Click);
            // 
            // buttonEditorRef
            // 
            this.buttonEditorRef.Location = new System.Drawing.Point(433, 83);
            this.buttonEditorRef.Name = "buttonEditorRef";
            this.buttonEditorRef.Size = new System.Drawing.Size(75, 23);
            this.buttonEditorRef.TabIndex = 5;
            this.buttonEditorRef.Text = "参照(&C)...";
            this.buttonEditorRef.UseVisualStyleBackColor = true;
            this.buttonEditorRef.Click += new System.EventHandler(this.buttonEditorRef_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(245, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "{0}の位置に編集対象のファイル名を埋め込みます。";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxLineSSH);
            this.groupBox1.Controls.Add(this.radioButtonLineSpecify);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.radioButtonLineNone);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxEditorLineNumSSH);
            this.groupBox1.Controls.Add(this.buttonEditorLineNumRefSSH);
            this.groupBox1.Controls.Add(this.textBoxEditorLineNum);
            this.groupBox1.Controls.Add(this.buttonEditorLineNumRef);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(3, 190);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(514, 149);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ファイルビューアからの編集";
            // 
            // checkBoxLineSSH
            // 
            this.checkBoxLineSSH.AutoSize = true;
            this.checkBoxLineSSH.Location = new System.Drawing.Point(28, 89);
            this.checkBoxLineSSH.Name = "checkBoxLineSSH";
            this.checkBoxLineSSH.Size = new System.Drawing.Size(117, 16);
            this.checkBoxLineSSH.TabIndex = 5;
            this.checkBoxLineSSH.Text = "SSHは別に指定(&O)";
            this.checkBoxLineSSH.UseVisualStyleBackColor = true;
            this.checkBoxLineSSH.CheckedChanged += new System.EventHandler(this.checkBoxEditorSSH_CheckedChanged);
            // 
            // radioButtonLineSpecify
            // 
            this.radioButtonLineSpecify.AutoSize = true;
            this.radioButtonLineSpecify.Location = new System.Drawing.Point(8, 40);
            this.radioButtonLineSpecify.Name = "radioButtonLineSpecify";
            this.radioButtonLineSpecify.Size = new System.Drawing.Size(198, 16);
            this.radioButtonLineSpecify.TabIndex = 1;
            this.radioButtonLineSpecify.TabStop = true;
            this.radioButtonLineSpecify.Text = "行番号付きのコマンドラインを指定(&L)";
            this.radioButtonLineSpecify.UseVisualStyleBackColor = true;
            this.radioButtonLineSpecify.CheckedChanged += new System.EventHandler(this.radioButtonEditor_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "通常のファイル(&A):";
            // 
            // radioButtonLineNone
            // 
            this.radioButtonLineNone.AutoSize = true;
            this.radioButtonLineNone.Location = new System.Drawing.Point(8, 18);
            this.radioButtonLineNone.Name = "radioButtonLineNone";
            this.radioButtonLineNone.Size = new System.Drawing.Size(332, 16);
            this.radioButtonLineNone.TabIndex = 0;
            this.radioButtonLineNone.TabStop = true;
            this.radioButtonLineNone.Text = "テキストエディタと同じコマンドラインにして、行番号を指定しない(&N)";
            this.radioButtonLineNone.UseVisualStyleBackColor = true;
            this.radioButtonLineNone.CheckedChanged += new System.EventHandler(this.radioButtonEditor_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(353, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "{0}の位置に編集対象のファイル名を、{1}の位置に行番号を埋め込みます。";
            // 
            // textBoxEditorLineNumSSH
            // 
            this.textBoxEditorLineNumSSH.Location = new System.Drawing.Point(150, 87);
            this.textBoxEditorLineNumSSH.Name = "textBoxEditorLineNumSSH";
            this.textBoxEditorLineNumSSH.Size = new System.Drawing.Size(277, 19);
            this.textBoxEditorLineNumSSH.TabIndex = 6;
            // 
            // buttonEditorLineNumRefSSH
            // 
            this.buttonEditorLineNumRefSSH.Location = new System.Drawing.Point(433, 85);
            this.buttonEditorLineNumRefSSH.Name = "buttonEditorLineNumRefSSH";
            this.buttonEditorLineNumRefSSH.Size = new System.Drawing.Size(75, 23);
            this.buttonEditorLineNumRefSSH.TabIndex = 7;
            this.buttonEditorLineNumRefSSH.Text = "参照(&F)...";
            this.buttonEditorLineNumRefSSH.UseVisualStyleBackColor = true;
            this.buttonEditorLineNumRefSSH.Click += new System.EventHandler(this.buttonEditorRef_Click);
            // 
            // textBoxEditorLineNum
            // 
            this.textBoxEditorLineNum.Location = new System.Drawing.Point(150, 62);
            this.textBoxEditorLineNum.Name = "textBoxEditorLineNum";
            this.textBoxEditorLineNum.Size = new System.Drawing.Size(277, 19);
            this.textBoxEditorLineNum.TabIndex = 3;
            // 
            // buttonEditorLineNumRef
            // 
            this.buttonEditorLineNumRef.Location = new System.Drawing.Point(433, 60);
            this.buttonEditorLineNumRef.Name = "buttonEditorLineNumRef";
            this.buttonEditorLineNumRef.Size = new System.Drawing.Size(75, 23);
            this.buttonEditorLineNumRef.TabIndex = 4;
            this.buttonEditorLineNumRef.Text = "参照(&R)...";
            this.buttonEditorLineNumRef.UseVisualStyleBackColor = true;
            this.buttonEditorLineNumRef.Click += new System.EventHandler(this.buttonEditorRef_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(386, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "ファイルビューアで表示中の行付近から編集するためのコマンドラインを指定します。";
            // 
            // InstallEditorPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "InstallEditorPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxEditor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonEditorRef;
        private System.Windows.Forms.RadioButton radioButtonEditorFix;
        private System.Windows.Forms.TextBox textBoxEditorAuto;
        private System.Windows.Forms.RadioButton radioButtonEditorAuto;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonLineSpecify;
        private System.Windows.Forms.RadioButton radioButtonLineNone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxEditorLineNum;
        private System.Windows.Forms.Button buttonEditorLineNumRef;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxEditorSSH;
        private System.Windows.Forms.LinkLabel linkLabelSSHHelp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxEditorSSH;
        private System.Windows.Forms.Button buttonEditorRefSSH;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxLineSSH;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxEditorLineNumSSH;
        private System.Windows.Forms.Button buttonEditorLineNumRefSSH;
    }
}
