namespace ShellFiler.UI.Dialog {
    partial class KeyBindHelpInputDialog {
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
            this.label1 = new System.Windows.Forms.Label();
            this.panelInner = new System.Windows.Forms.Panel();
            this.panelInner.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(82, 42);
            this.label1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(363, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "割り当てを確認したいキーを押してください。";
            this.label1.Click += new System.EventHandler(this.panelInner_Click);
            // 
            // panelInner
            // 
            this.panelInner.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelInner.Controls.Add(this.label1);
            this.panelInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelInner.Location = new System.Drawing.Point(0, 0);
            this.panelInner.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.panelInner.Name = "panelInner";
            this.panelInner.Size = new System.Drawing.Size(541, 127);
            this.panelInner.TabIndex = 2;
            this.panelInner.Click += new System.EventHandler(this.panelInner_Click);
            // 
            // KeyBindHelpInputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(541, 127);
            this.ControlBox = false;
            this.Controls.Add(this.panelInner);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeyBindHelpInputDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "キー割り当ての確認";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyBindHelpDialog_KeyDown);
            this.panelInner.ResumeLayout(false);
            this.panelInner.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelInner;
    }
}