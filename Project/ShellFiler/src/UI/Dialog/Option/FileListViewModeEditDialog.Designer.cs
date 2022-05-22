namespace ShellFiler.UI.Dialog.Option {
    partial class FileListViewModeEditDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxThumbName = new System.Windows.Forms.ComboBox();
            this.comboBoxThumbSize = new System.Windows.Forms.ComboBox();
            this.panelViewSample = new System.Windows.Forms.Panel();
            this.radioButtonThumb = new System.Windows.Forms.RadioButton();
            this.labelViewName = new System.Windows.Forms.Label();
            this.labelViewThumbSize = new System.Windows.Forms.Label();
            this.radioButtonDetail = new System.Windows.Forms.RadioButton();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxTargetFolder = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxThumbName
            // 
            this.comboBoxThumbName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxThumbName.FormattingEnabled = true;
            this.comboBoxThumbName.Location = new System.Drawing.Point(48, 123);
            this.comboBoxThumbName.Name = "comboBoxThumbName";
            this.comboBoxThumbName.Size = new System.Drawing.Size(155, 23);
            this.comboBoxThumbName.TabIndex = 5;
            // 
            // comboBoxThumbSize
            // 
            this.comboBoxThumbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxThumbSize.FormattingEnabled = true;
            this.comboBoxThumbSize.Location = new System.Drawing.Point(48, 79);
            this.comboBoxThumbSize.Name = "comboBoxThumbSize";
            this.comboBoxThumbSize.Size = new System.Drawing.Size(155, 23);
            this.comboBoxThumbSize.TabIndex = 3;
            // 
            // panelViewSample
            // 
            this.panelViewSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelViewSample.Location = new System.Drawing.Point(230, 18);
            this.panelViewSample.Name = "panelViewSample";
            this.panelViewSample.Size = new System.Drawing.Size(283, 146);
            this.panelViewSample.TabIndex = 6;
            // 
            // radioButtonThumb
            // 
            this.radioButtonThumb.AutoSize = true;
            this.radioButtonThumb.Location = new System.Drawing.Point(8, 40);
            this.radioButtonThumb.Name = "radioButtonThumb";
            this.radioButtonThumb.Size = new System.Drawing.Size(168, 19);
            this.radioButtonThumb.TabIndex = 1;
            this.radioButtonThumb.TabStop = true;
            this.radioButtonThumb.Text = "サムネイル表示に変更する(&T)";
            this.radioButtonThumb.UseVisualStyleBackColor = true;
            // 
            // labelViewName
            // 
            this.labelViewName.AutoSize = true;
            this.labelViewName.Location = new System.Drawing.Point(31, 106);
            this.labelViewName.Name = "labelViewName";
            this.labelViewName.Size = new System.Drawing.Size(135, 15);
            this.labelViewName.TabIndex = 4;
            this.labelViewName.Text = "ファイル名の表示方法(&N):";
            // 
            // labelViewThumbSize
            // 
            this.labelViewThumbSize.AutoSize = true;
            this.labelViewThumbSize.Location = new System.Drawing.Point(31, 62);
            this.labelViewThumbSize.Name = "labelViewThumbSize";
            this.labelViewThumbSize.Size = new System.Drawing.Size(142, 15);
            this.labelViewThumbSize.TabIndex = 2;
            this.labelViewThumbSize.Text = "サムネイル画像の大きさ(S):";
            // 
            // radioButtonDetail
            // 
            this.radioButtonDetail.AutoSize = true;
            this.radioButtonDetail.Location = new System.Drawing.Point(8, 18);
            this.radioButtonDetail.Name = "radioButtonDetail";
            this.radioButtonDetail.Size = new System.Drawing.Size(144, 19);
            this.radioButtonDetail.TabIndex = 0;
            this.radioButtonDetail.TabStop = true;
            this.radioButtonDetail.Text = "詳細表示に変更する(&D)";
            this.radioButtonDetail.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(375, 222);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(456, 222);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "対象フォルダ(&F):";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelViewSample);
            this.groupBox1.Controls.Add(this.labelViewName);
            this.groupBox1.Controls.Add(this.radioButtonThumb);
            this.groupBox1.Controls.Add(this.labelViewThumbSize);
            this.groupBox1.Controls.Add(this.comboBoxThumbName);
            this.groupBox1.Controls.Add(this.radioButtonDetail);
            this.groupBox1.Controls.Add(this.comboBoxThumbSize);
            this.groupBox1.Location = new System.Drawing.Point(12, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(519, 178);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "使用する表示モード";
            // 
            // textBoxTargetFolder
            // 
            this.textBoxTargetFolder.Location = new System.Drawing.Point(101, 10);
            this.textBoxTargetFolder.Name = "textBoxTargetFolder";
            this.textBoxTargetFolder.Size = new System.Drawing.Size(430, 23);
            this.textBoxTargetFolder.TabIndex = 1;
            // 
            // FileListViewModeEditDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(543, 257);
            this.Controls.Add(this.textBoxTargetFolder);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileListViewModeEditDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "表示モードの追加と編集";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxThumbName;
        private System.Windows.Forms.ComboBox comboBoxThumbSize;
        private System.Windows.Forms.Panel panelViewSample;
        private System.Windows.Forms.RadioButton radioButtonThumb;
        private System.Windows.Forms.Label labelViewName;
        private System.Windows.Forms.Label labelViewThumbSize;
        private System.Windows.Forms.RadioButton radioButtonDetail;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxTargetFolder;
    }
}