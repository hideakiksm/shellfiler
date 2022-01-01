namespace ShellFiler.Terminal.UI {
    partial class TerminalForm {
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
            this.menuStrip = new ShellFiler.Terminal.UI.TerminalMenuStrip();
            this.toolStrip = new ShellFiler.Terminal.UI.TerminalToolBarStrip();
            this.functionBar = new ShellFiler.UI.ControlBar.FunctionBar();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(920, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // toolStrip
            // 
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(920, 25);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip";
            // 
            // functionBar
            // 
            this.functionBar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.functionBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.functionBar.Location = new System.Drawing.Point(0, 553);
            this.functionBar.Name = "functionBar";
            this.functionBar.Size = new System.Drawing.Size(892, 20);
            this.functionBar.TabIndex = 0;
            // 
            // TerminalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 621);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "TerminalForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "FileViewer";
            this.Deactivate += new System.EventHandler(this.TerminalForm_Deactivate);
            this.Load += new System.EventHandler(this.TerminalForm_Load);
            this.Activated += new System.EventHandler(this.TerminalForm_Activated);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MonitoringViewerForm_KeyUp);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MonitoringViewerForm_FormClosing);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.MonitoringViewerForm_PreviewKeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private ShellFiler.Terminal.UI.TerminalMenuStrip menuStrip;
        private ShellFiler.Terminal.UI.TerminalToolBarStrip toolStrip;
        private ShellFiler.UI.ControlBar.FunctionBar functionBar;
    }
}