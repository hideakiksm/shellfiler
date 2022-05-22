namespace ShellFiler.UI.Dialog {
    partial class RetrieveFolderSizeDialog {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelOpposite = new System.Windows.Forms.Label();
            this.labelTarget = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericSpecify = new System.Windows.Forms.NumericUpDown();
            this.radioButtonSpecify = new System.Windows.Forms.RadioButton();
            this.radioButtonOpposite = new System.Windows.Forms.RadioButton();
            this.radioButtonTarget = new System.Windows.Forms.RadioButton();
            this.radioButtonOriginal = new System.Windows.Forms.RadioButton();
            this.checkBoxLowerCache = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labelLowerMessage = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.textBoxDeleteShortcut = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSpecify)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelOpposite);
            this.groupBox1.Controls.Add(this.labelTarget);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericSpecify);
            this.groupBox1.Controls.Add(this.radioButtonSpecify);
            this.groupBox1.Controls.Add(this.radioButtonOpposite);
            this.groupBox1.Controls.Add(this.radioButtonTarget);
            this.groupBox1.Controls.Add(this.radioButtonOriginal);
            this.groupBox1.Location = new System.Drawing.Point(13, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(427, 115);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "サイズの計算方法";
            // 
            // labelOpposite
            // 
            this.labelOpposite.AutoSize = true;
            this.labelOpposite.Location = new System.Drawing.Point(180, 65);
            this.labelOpposite.Name = "labelOpposite";
            this.labelOpposite.Size = new System.Drawing.Size(135, 15);
            this.labelOpposite.TabIndex = 5;
            this.labelOpposite.Text = "{0}bytes単位に切り上げ";
            // 
            // labelTarget
            // 
            this.labelTarget.AutoSize = true;
            this.labelTarget.Location = new System.Drawing.Point(180, 43);
            this.labelTarget.Name = "labelTarget";
            this.labelTarget.Size = new System.Drawing.Size(135, 15);
            this.labelTarget.TabIndex = 4;
            this.labelTarget.Text = "{0}bytes単位に切り上げ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(291, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "bytes単位に切り上げ";
            // 
            // numericSpecify
            // 
            this.numericSpecify.Location = new System.Drawing.Point(182, 85);
            this.numericSpecify.Name = "numericSpecify";
            this.numericSpecify.Size = new System.Drawing.Size(103, 23);
            this.numericSpecify.TabIndex = 6;
            // 
            // radioButtonSpecify
            // 
            this.radioButtonSpecify.AutoSize = true;
            this.radioButtonSpecify.Location = new System.Drawing.Point(7, 85);
            this.radioButtonSpecify.Name = "radioButtonSpecify";
            this.radioButtonSpecify.Size = new System.Drawing.Size(120, 19);
            this.radioButtonSpecify.TabIndex = 3;
            this.radioButtonSpecify.TabStop = true;
            this.radioButtonSpecify.Text = "指定サイズ単位(&S)";
            this.radioButtonSpecify.UseVisualStyleBackColor = true;
            // 
            // radioButtonOpposite
            // 
            this.radioButtonOpposite.AutoSize = true;
            this.radioButtonOpposite.Location = new System.Drawing.Point(7, 63);
            this.radioButtonOpposite.Name = "radioButtonOpposite";
            this.radioButtonOpposite.Size = new System.Drawing.Size(159, 19);
            this.radioButtonOpposite.TabIndex = 2;
            this.radioButtonOpposite.TabStop = true;
            this.radioButtonOpposite.Text = "反対パスのクラスタサイズ(&O)";
            this.radioButtonOpposite.UseVisualStyleBackColor = true;
            // 
            // radioButtonTarget
            // 
            this.radioButtonTarget.AutoSize = true;
            this.radioButtonTarget.Location = new System.Drawing.Point(7, 41);
            this.radioButtonTarget.Name = "radioButtonTarget";
            this.radioButtonTarget.Size = new System.Drawing.Size(157, 19);
            this.radioButtonTarget.TabIndex = 1;
            this.radioButtonTarget.TabStop = true;
            this.radioButtonTarget.Text = "対象パスのクラスタサイズ(&P)";
            this.radioButtonTarget.UseVisualStyleBackColor = true;
            // 
            // radioButtonOriginal
            // 
            this.radioButtonOriginal.AutoSize = true;
            this.radioButtonOriginal.Location = new System.Drawing.Point(7, 19);
            this.radioButtonOriginal.Name = "radioButtonOriginal";
            this.radioButtonOriginal.Size = new System.Drawing.Size(143, 19);
            this.radioButtonOriginal.TabIndex = 0;
            this.radioButtonOriginal.TabStop = true;
            this.radioButtonOriginal.Text = "ファイルサイズそのもの(&F)";
            this.radioButtonOriginal.UseVisualStyleBackColor = true;
            // 
            // checkBoxLowerCache
            // 
            this.checkBoxLowerCache.AutoSize = true;
            this.checkBoxLowerCache.Location = new System.Drawing.Point(13, 169);
            this.checkBoxLowerCache.Name = "checkBoxLowerCache";
            this.checkBoxLowerCache.Size = new System.Drawing.Size(158, 19);
            this.checkBoxLowerCache.TabIndex = 1;
            this.checkBoxLowerCache.Text = "下位階層の結果を保持(&L)";
            this.checkBoxLowerCache.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 190);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(347, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "サイズの取得後、[フォルダサイズ取得結果のクリア]を実行するまでの間、";
            // 
            // labelLowerMessage
            // 
            this.labelLowerMessage.AutoSize = true;
            this.labelLowerMessage.Location = new System.Drawing.Point(38, 206);
            this.labelLowerMessage.Name = "labelLowerMessage";
            this.labelLowerMessage.Size = new System.Drawing.Size(316, 15);
            this.labelLowerMessage.TabIndex = 3;
            this.labelLowerMessage.Text = "最大で{0}階層{1}フォルダ分のフォルダサイズをキャッシュします。";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 222);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(334, 15);
            this.label6.TabIndex = 4;
            this.label6.Text = "フォルダを切り替えると、すぐに取得済みのフォルダサイズを表示します。";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(365, 267);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(284, 267);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 7;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(315, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "マークされたフォルダ配下にあるファイルサイズの合計を計算します。";
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(40, 244);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(95, 23);
            this.buttonDelete.TabIndex = 5;
            this.buttonDelete.Text = "結果を削除(&D)";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // textBoxDeleteShortcut
            // 
            this.textBoxDeleteShortcut.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxDeleteShortcut.Location = new System.Drawing.Point(141, 249);
            this.textBoxDeleteShortcut.Name = "textBoxDeleteShortcut";
            this.textBoxDeleteShortcut.ReadOnly = true;
            this.textBoxDeleteShortcut.Size = new System.Drawing.Size(299, 16);
            this.textBoxDeleteShortcut.TabIndex = 6;
            this.textBoxDeleteShortcut.Text = "キー割り当て:{0}";
            // 
            // RetrieveFolderSizeDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(452, 302);
            this.Controls.Add(this.textBoxDeleteShortcut);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelLowerMessage);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkBoxLowerCache);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RetrieveFolderSizeDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "フォルダサイズの取得";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSpecify)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelOpposite;
        private System.Windows.Forms.Label labelTarget;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericSpecify;
        private System.Windows.Forms.RadioButton radioButtonSpecify;
        private System.Windows.Forms.RadioButton radioButtonOpposite;
        private System.Windows.Forms.RadioButton radioButtonTarget;
        private System.Windows.Forms.RadioButton radioButtonOriginal;
        private System.Windows.Forms.CheckBox checkBoxLowerCache;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelLowerMessage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TextBox textBoxDeleteShortcut;
    }
}