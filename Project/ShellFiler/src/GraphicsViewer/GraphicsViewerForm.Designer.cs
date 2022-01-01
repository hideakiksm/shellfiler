namespace ShellFiler.GraphicsViewer {
    partial class GraphicsViewerForm {
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
            this.menuStrip = new GraphicsViewerMenuStrip();
            this.toolStrip = new GraphicsViewerToolbarStrip();
            this.statusStrip = new GraphicsViewerStatusBar();
            this.viewerPanel = new GraphicsViewerPanel();
            this.functionBar = new ShellFiler.UI.ControlBar.FunctionBar();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip1";
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
            // statusStrip1
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 599);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(920, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            this.statusStrip.SizingGrip = false;
            this.statusStrip.GripMargin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            // 
            // functionBar
            // 
            this.functionBar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.functionBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.functionBar.Location = new System.Drawing.Point(0, 553);
            this.functionBar.Name = "panelFuncKey";
            this.functionBar.Size = new System.Drawing.Size(892, 20);
            this.functionBar.TabIndex = 0;
            // 
            // viewerPanel
            // 
            this.viewerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewerPanel.Location = new System.Drawing.Point(0, 49);
            this.viewerPanel.Name = "viewerPanel";
            this.viewerPanel.Size = new System.Drawing.Size(920, 550);
            this.viewerPanel.TabIndex = 3;
            // 
            // FileViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 621);
            this.Controls.Add(this.viewerPanel);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.functionBar);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FileViewerForm";
            this.Text = "FileViewer";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GraphicsViewerForm_FormClosing);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(GraphicsViewerForm_PreviewKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(GraphicsViewerForm_KeyUp);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GraphicsViewerMenuStrip menuStrip;
        private GraphicsViewerToolbarStrip toolStrip;
        private ShellFiler.UI.ControlBar.FunctionBar functionBar;
        private GraphicsViewerStatusBar statusStrip;
        private GraphicsViewerPanel viewerPanel;
    }
}