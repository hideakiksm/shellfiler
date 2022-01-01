namespace ShellFiler.UI.Dialog {
    partial class DropActionInternalDialog {
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonMove = new System.Windows.Forms.Button();
            this.buttonShortcut = new System.Windows.Forms.Button();
            this.buttonChdir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "反対パスからドロップされました。";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(138, 140);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonCopy
            // 
            this.buttonCopy.Location = new System.Drawing.Point(13, 24);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(200, 23);
            this.buttonCopy.TabIndex = 1;
            this.buttonCopy.Text = "このフォルダにコピー(&C)";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonMove
            // 
            this.buttonMove.Location = new System.Drawing.Point(13, 53);
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.Size = new System.Drawing.Size(200, 23);
            this.buttonMove.TabIndex = 2;
            this.buttonMove.Text = "このフォルダに移動(&M)";
            this.buttonMove.UseVisualStyleBackColor = true;
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // buttonShortcut
            // 
            this.buttonShortcut.Location = new System.Drawing.Point(13, 82);
            this.buttonShortcut.Name = "buttonShortcut";
            this.buttonShortcut.Size = new System.Drawing.Size(200, 23);
            this.buttonShortcut.TabIndex = 3;
            this.buttonShortcut.Text = "このフォルダにショートカットを作成(&S)";
            this.buttonShortcut.UseVisualStyleBackColor = true;
            this.buttonShortcut.Click += new System.EventHandler(this.buttonShortcut_Click);
            // 
            // buttonChdir
            // 
            this.buttonChdir.Location = new System.Drawing.Point(13, 111);
            this.buttonChdir.Name = "buttonChdir";
            this.buttonChdir.Size = new System.Drawing.Size(200, 23);
            this.buttonChdir.TabIndex = 4;
            this.buttonChdir.Text = "転送元のフォルダを表示(&V)";
            this.buttonChdir.UseVisualStyleBackColor = true;
            this.buttonChdir.Click += new System.EventHandler(this.buttonChdir_Click);
            // 
            // DropActionInternalDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(227, 177);
            this.Controls.Add(this.buttonChdir);
            this.Controls.Add(this.buttonShortcut);
            this.Controls.Add(this.buttonMove);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DropActionInternalDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ドロップ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Button buttonMove;
        private System.Windows.Forms.Button buttonShortcut;
        private System.Windows.Forms.Button buttonChdir;
    }
}