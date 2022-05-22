namespace ShellFiler.UI.Dialog.FileViewer {
    partial class ViewerCompareDisplayDialog {
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
            this.buttonDiffTool = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxLeft = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonLeftClear = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBoxRight = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonRightClear = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonDiffTool
            // 
            this.buttonDiffTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDiffTool.Location = new System.Drawing.Point(457, 12);
            this.buttonDiffTool.Name = "buttonDiffTool";
            this.buttonDiffTool.Size = new System.Drawing.Size(92, 23);
            this.buttonDiffTool.TabIndex = 2;
            this.buttonDiffTool.Text = "差分を表示(&W)";
            this.buttonDiffTool.UseVisualStyleBackColor = true;
            this.buttonDiffTool.Click += new System.EventHandler(this.buttonDiffTool_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(474, 359);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "閉じる";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(12, 40);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.panel1);
            this.splitContainer.Panel1MinSize = 120;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panel2);
            this.splitContainer.Panel2MinSize = 120;
            this.splitContainer.Size = new System.Drawing.Size(537, 315);
            this.splitContainer.SplitterDistance = 120;
            this.splitContainer.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.textBoxLeft);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.buttonLeftClear);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(120, 315);
            this.panel1.TabIndex = 0;
            // 
            // textBoxLeft
            // 
            this.textBoxLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLeft.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxLeft.Location = new System.Drawing.Point(6, 29);
            this.textBoxLeft.Multiline = true;
            this.textBoxLeft.Name = "textBoxLeft";
            this.textBoxLeft.ReadOnly = true;
            this.textBoxLeft.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLeft.Size = new System.Drawing.Size(107, 279);
            this.textBoxLeft.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "左側";
            // 
            // buttonLeftClear
            // 
            this.buttonLeftClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLeftClear.Location = new System.Drawing.Point(55, 3);
            this.buttonLeftClear.Name = "buttonLeftClear";
            this.buttonLeftClear.Size = new System.Drawing.Size(58, 23);
            this.buttonLeftClear.TabIndex = 1;
            this.buttonLeftClear.Text = "消去(&L)";
            this.buttonLeftClear.UseVisualStyleBackColor = true;
            this.buttonLeftClear.Click += new System.EventHandler(this.buttonLeftClear_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.textBoxRight);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.buttonRightClear);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(413, 315);
            this.panel2.TabIndex = 0;
            // 
            // textBoxRight
            // 
            this.textBoxRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRight.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxRight.Location = new System.Drawing.Point(5, 31);
            this.textBoxRight.Multiline = true;
            this.textBoxRight.Name = "textBoxRight";
            this.textBoxRight.ReadOnly = true;
            this.textBoxRight.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxRight.Size = new System.Drawing.Size(401, 277);
            this.textBoxRight.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "右側";
            // 
            // buttonRightClear
            // 
            this.buttonRightClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRightClear.Location = new System.Drawing.Point(348, 3);
            this.buttonRightClear.Name = "buttonRightClear";
            this.buttonRightClear.Size = new System.Drawing.Size(58, 23);
            this.buttonRightClear.TabIndex = 1;
            this.buttonRightClear.Text = "消去(&R)";
            this.buttonRightClear.UseVisualStyleBackColor = true;
            this.buttonRightClear.Click += new System.EventHandler(this.buttonRightClear_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(216, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "比較対象として登録されているテキストです。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(271, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "[選択範囲を左側/右側として差分表示]で登録します。";
            // 
            // ViewerCompareDisplayDialog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(561, 392);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonDiffTool);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "ViewerCompareDisplayDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "比較対象を確認";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonDiffTool;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox textBoxRight;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonLeftClear;
        private System.Windows.Forms.TextBox textBoxLeft;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonRightClear;
        private System.Windows.Forms.Label label4;
    }
}