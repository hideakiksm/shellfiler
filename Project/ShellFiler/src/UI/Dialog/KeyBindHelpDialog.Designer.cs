namespace ShellFiler.UI.Dialog {
    partial class KeyBindHelpDialog {
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
            this.listViewKey = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxExplanation = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonRetry = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewKey
            // 
            this.listViewKey.HideSelection = false;
            this.listViewKey.Location = new System.Drawing.Point(21, 66);
            this.listViewKey.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.listViewKey.MultiSelect = false;
            this.listViewKey.Name = "listViewKey";
            this.listViewKey.Size = new System.Drawing.Size(724, 167);
            this.listViewKey.TabIndex = 0;
            this.listViewKey.UseCompatibleStateImageBehavior = false;
            this.listViewKey.View = System.Windows.Forms.View.Details;
            this.listViewKey.SelectedIndexChanged += new System.EventHandler(this.listViewKey_SelectedIndexChanged);
            this.listViewKey.DoubleClick += new System.EventHandler(this.listViewKey_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "割り当てられているキー(&K):";
            // 
            // textBoxExplanation
            // 
            this.textBoxExplanation.Location = new System.Drawing.Point(21, 247);
            this.textBoxExplanation.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.textBoxExplanation.Multiline = true;
            this.textBoxExplanation.Name = "textBoxExplanation";
            this.textBoxExplanation.ReadOnly = true;
            this.textBoxExplanation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxExplanation.Size = new System.Drawing.Size(724, 106);
            this.textBoxExplanation.TabIndex = 3;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(472, 363);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(131, 40);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "実行";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(614, 363);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(131, 40);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonRetry
            // 
            this.buttonRetry.Location = new System.Drawing.Point(614, 14);
            this.buttonRetry.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonRetry.Name = "buttonRetry";
            this.buttonRetry.Size = new System.Drawing.Size(131, 40);
            this.buttonRetry.TabIndex = 4;
            this.buttonRetry.Text = "再入力(&R)";
            this.buttonRetry.UseVisualStyleBackColor = true;
            this.buttonRetry.Click += new System.EventHandler(this.buttonRetry_Click);
            // 
            // KeyBindHelpDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(759, 429);
            this.Controls.Add(this.buttonRetry);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxExplanation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewKey);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeyBindHelpDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "キー割り当ての確認";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.KeyBindHelpDialog_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxExplanation;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonRetry;
    }
}