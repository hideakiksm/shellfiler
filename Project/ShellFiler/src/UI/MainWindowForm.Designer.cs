namespace ShellFiler.UI
{
    partial class MainWindowForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControlMain = new ShellFiler.Util.TabControlEx();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerFile = new System.Windows.Forms.SplitContainer();
            this.splitContainerLog = new System.Windows.Forms.SplitContainer();
            this.panelLeft = new ShellFiler.UI.FileList.FilePanel();
            this.panelRight = new ShellFiler.UI.FileList.FilePanel();
            this.logPanel = new ShellFiler.UI.Log.Logger.LogPanel();
            this.stateListPanel = new ShellFiler.UI.StateList.StateListPanel();
            this.functionBar = new ShellFiler.UI.ControlBar.FunctionBar();
            this.menuStripMain = new ShellFiler.UI.ControlBar.MainMenuStrip();
            this.toolStripMain = new ShellFiler.UI.ControlBar.MainToolbarStrip();
            this.tabControlMain.SuspendLayout();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.splitContainerMain.Panel1MinSize = 50;
            this.splitContainerFile.Panel1.SuspendLayout();
            this.splitContainerFile.Panel2.SuspendLayout();
            this.splitContainerFile.SuspendLayout();
            this.splitContainerLog.Panel1.SuspendLayout();
            this.splitContainerLog.Panel2.SuspendLayout();
            this.splitContainerLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Location = new System.Drawing.Point(0, 52);
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(400, 200);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.TabIndex = 0;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 51);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainerMain.Panel1.Controls.Add(this.tabControlMain);
            this.splitContainerMain.Panel2.BackColor = System.Drawing.SystemColors.MenuBar;
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerLog);
            this.splitContainerMain.Size = new System.Drawing.Size(892, 500);
            this.splitContainerMain.SplitterDistance = 362;
            this.splitContainerMain.SplitterWidth = 3;
            this.splitContainerMain.TabIndex = 0;
            this.splitContainerMain.MouseUp += new System.Windows.Forms.MouseEventHandler(SplitContainer_MouseUp);
            // 
            // splitContainerFile
            // 
            this.splitContainerFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerFile.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainerFile.Location = new System.Drawing.Point(0, 0);
            this.splitContainerFile.Name = "splitContainerFile";
            this.splitContainerFile.Panel1.Controls.Add(this.panelLeft);
            this.splitContainerFile.Panel2.Controls.Add(this.panelRight);
            this.splitContainerFile.Size = new System.Drawing.Size(892, 362);
            this.splitContainerFile.SplitterDistance = 500;
            this.splitContainerFile.SplitterWidth = 3;
            this.splitContainerFile.TabIndex = 0;
            this.splitContainerFile.MouseUp += new System.Windows.Forms.MouseEventHandler(SplitContainer_MouseUp);
            this.splitContainerFile.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(splitContainerFile_SplitterMoved);
            this.splitContainerFile.SizeChanged += new System.EventHandler(splitContainerFile_SizeChanged);
            // 
            // splitContainerLog
            // 
            this.splitContainerLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLog.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLog.Name = "splitContainerLog";
            this.splitContainerLog.Panel1.Controls.Add(this.logPanel);
            this.splitContainerLog.Panel2.Controls.Add(this.stateListPanel);
            this.splitContainerLog.Size = new System.Drawing.Size(892, 135);
            this.splitContainerLog.SplitterDistance = 670;
            this.splitContainerLog.SplitterWidth = 3;
            this.splitContainerLog.TabIndex = 0;
            this.splitContainerLog.MouseUp += new System.Windows.Forms.MouseEventHandler(SplitContainer_MouseUp);
            // 
            // panelLeft
            // 
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(500, 362);
            this.panelLeft.TabIndex = 0;
            // 
            // panelRight
            // 
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(0, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(389, 362);
            this.panelRight.TabIndex = 0;
            // 
            // logPanel
            // 
            this.logPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.logPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.logPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logPanel.Location = new System.Drawing.Point(0, 0);
            this.logPanel.Name = "logPanel";
            this.logPanel.Size = new System.Drawing.Size(892, 135);
            this.logPanel.TabIndex = 0;
            // 
            // functionBar
            // 
            this.functionBar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.functionBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.functionBar.Location = new System.Drawing.Point(0, 551);
            this.functionBar.Name = "functionBar";
            this.functionBar.Size = new System.Drawing.Size(892, 22);
            this.functionBar.TabIndex = 0;
            //
            // menuStripMain
            //
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "mainMenuStrip";
            this.menuStripMain.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.menuStripMain.Size = new System.Drawing.Size(634, 26);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // toolStripMain
            // 
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Location = new System.Drawing.Point(0, 26);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Padding = new System.Windows.Forms.Padding(0);
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStripMain.Size = new System.Drawing.Size(634, 25);
            this.toolStripMain.Stretch = true;
            this.toolStripMain.TabIndex = 1;
            this.toolStripMain.Text = "toolStripMain";
            // 
            // MainWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(892, 573);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.menuStripMain);
            this.Controls.Add(this.functionBar);
            this.Name = "MainWindowForm";
            this.Text = "ShellFiler";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindowForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindowForm_FormClosing);
            this.Activated += new System.EventHandler(this.MainWindowForm_Activated);
            this.tabControlMain.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerFile.Panel1.ResumeLayout(false);
            this.splitContainerFile.Panel2.ResumeLayout(false);
            this.splitContainerFile.ResumeLayout(false);
            this.splitContainerLog.Panel1.ResumeLayout(false);
            this.splitContainerLog.Panel2.ResumeLayout(false);
            this.splitContainerLog.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ShellFiler.UI.ControlBar.MainMenuStrip menuStripMain;
        private ShellFiler.UI.ControlBar.MainToolbarStrip toolStripMain;
        private ShellFiler.Util.TabControlEx tabControlMain;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainerFile;
        private System.Windows.Forms.SplitContainer splitContainerLog;
        private ShellFiler.UI.ControlBar.FunctionBar functionBar;
        private ShellFiler.UI.FileList.FilePanel panelLeft;
        private ShellFiler.UI.FileList.FilePanel panelRight;
        private ShellFiler.UI.StateList.StateListPanel stateListPanel;
        private Log.Logger.LogPanel logPanel;
    }
}
