namespace ShellFiler.UI.Dialog {
    partial class CombineFileDialog {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CombineFileDialog));
            this.listViewTarget = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonSortName = new System.Windows.Forms.Button();
            this.buttonSortMark = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDestName = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDestFolder = new System.Windows.Forms.TextBox();
            this.pictureBoxArrow = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArrow)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewTarget
            // 
            this.listViewTarget.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTarget.FullRowSelect = true;
            this.listViewTarget.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewTarget.HideSelection = false;
            this.listViewTarget.Location = new System.Drawing.Point(0, 0);
            this.listViewTarget.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listViewTarget.MultiSelect = false;
            this.listViewTarget.Name = "listViewTarget";
            this.listViewTarget.Size = new System.Drawing.Size(615, 220);
            this.listViewTarget.TabIndex = 1;
            this.listViewTarget.UseCompatibleStateImageBehavior = false;
            this.listViewTarget.View = System.Windows.Forms.View.Details;
            this.listViewTarget.SelectedIndexChanged += new System.EventHandler(this.listViewTarget_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "ファイルの結合順(&S):";
            // 
            // buttonUp
            // 
            this.buttonUp.Location = new System.Drawing.Point(657, 42);
            this.buttonUp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(112, 34);
            this.buttonUp.TabIndex = 2;
            this.buttonUp.Text = "上へ(&U)";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Location = new System.Drawing.Point(656, 86);
            this.buttonDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(112, 34);
            this.buttonDown.TabIndex = 3;
            this.buttonDown.Text = "下へ(&D)";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonSortName
            // 
            this.buttonSortName.Location = new System.Drawing.Point(657, 189);
            this.buttonSortName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSortName.Name = "buttonSortName";
            this.buttonSortName.Size = new System.Drawing.Size(112, 34);
            this.buttonSortName.TabIndex = 4;
            this.buttonSortName.Text = "名前順(&N)";
            this.buttonSortName.UseVisualStyleBackColor = true;
            this.buttonSortName.Click += new System.EventHandler(this.buttonSortName_Click);
            // 
            // buttonSortMark
            // 
            this.buttonSortMark.Location = new System.Drawing.Point(657, 232);
            this.buttonSortMark.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSortMark.Name = "buttonSortMark";
            this.buttonSortMark.Size = new System.Drawing.Size(112, 34);
            this.buttonSortMark.TabIndex = 5;
            this.buttonSortMark.Text = "マーク順(&M)";
            this.buttonSortMark.UseVisualStyleBackColor = true;
            this.buttonSortMark.Click += new System.EventHandler(this.buttonSortMark_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 354);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 25);
            this.label2.TabIndex = 8;
            this.label2.Text = "結合先ファイル名(&D):";
            // 
            // textBoxDestName
            // 
            this.textBoxDestName.Location = new System.Drawing.Point(183, 350);
            this.textBoxDestName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxDestName.Name = "textBoxDestName";
            this.textBoxDestName.Size = new System.Drawing.Size(584, 31);
            this.textBoxDestName.TabIndex = 9;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(656, 399);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(112, 34);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(536, 399);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(112, 34);
            this.buttonOk.TabIndex = 10;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 309);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(160, 25);
            this.label3.TabIndex = 6;
            this.label3.Text = "結合先フォルダ名(&F):";
            // 
            // textBoxDestFolder
            // 
            this.textBoxDestFolder.Location = new System.Drawing.Point(182, 304);
            this.textBoxDestFolder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxDestFolder.Name = "textBoxDestFolder";
            this.textBoxDestFolder.ReadOnly = true;
            this.textBoxDestFolder.Size = new System.Drawing.Size(584, 31);
            this.textBoxDestFolder.TabIndex = 7;
            // 
            // pictureBoxArrow
            // 
            this.pictureBoxArrow.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxArrow.Image")));
            this.pictureBoxArrow.InitialImage = null;
            this.pictureBoxArrow.Location = new System.Drawing.Point(339, 273);
            this.pictureBoxArrow.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBoxArrow.Name = "pictureBoxArrow";
            this.pictureBoxArrow.Size = new System.Drawing.Size(72, 24);
            this.pictureBoxArrow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxArrow.TabIndex = 12;
            this.pictureBoxArrow.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listViewTarget);
            this.panel1.Location = new System.Drawing.Point(20, 46);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(615, 220);
            this.panel1.TabIndex = 13;
            // 
            // CombineFileDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(790, 452);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBoxArrow);
            this.Controls.Add(this.textBoxDestFolder);
            this.Controls.Add(this.textBoxDestName);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSortMark);
            this.Controls.Add(this.buttonSortName);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CombineFileDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ファイルの結合";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArrow)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewTarget;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonSortName;
        private System.Windows.Forms.Button buttonSortMark;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDestName;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDestFolder;
        private System.Windows.Forms.PictureBox pictureBoxArrow;
        private System.Windows.Forms.Panel panel1;
    }
}