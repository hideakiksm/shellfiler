namespace ShellFiler.UI.Dialog
{
    partial class TaskManagerDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewSSH = new System.Windows.Forms.ListView();
            this.buttonDisconnectAll = new System.Windows.Forms.Button();
            this.buttonSSHDisconnect = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonCacnelAll = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listViewTask = new System.Windows.Forms.ListView();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageBackground = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonPriorityMinus = new System.Windows.Forms.Button();
            this.buttonPriorityPlus = new System.Windows.Forms.Button();
            this.buttonResume = new System.Windows.Forms.Button();
            this.buttonSuspend = new System.Windows.Forms.Button();
            this.buttonCancelWait = new System.Windows.Forms.Button();
            this.listViewWait = new System.Windows.Forms.ListView();
            this.tabPageSSHConnection = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.tabPageBackground.SuspendLayout();
            this.tabPageSSHConnection.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewSSH
            // 
            this.listViewSSH.HideSelection = false;
            this.listViewSSH.Location = new System.Drawing.Point(6, 6);
            this.listViewSSH.MultiSelect = false;
            this.listViewSSH.Name = "listViewSSH";
            this.listViewSSH.Size = new System.Drawing.Size(429, 266);
            this.listViewSSH.TabIndex = 4;
            this.listViewSSH.UseCompatibleStateImageBehavior = false;
            this.listViewSSH.View = System.Windows.Forms.View.Details;
            // 
            // buttonDisconnectAll
            // 
            this.buttonDisconnectAll.Location = new System.Drawing.Point(441, 35);
            this.buttonDisconnectAll.Name = "buttonDisconnectAll";
            this.buttonDisconnectAll.Size = new System.Drawing.Size(83, 23);
            this.buttonDisconnectAll.TabIndex = 6;
            this.buttonDisconnectAll.Text = "すべて切断(&S)";
            this.buttonDisconnectAll.UseVisualStyleBackColor = true;
            // 
            // buttonSSHDisconnect
            // 
            this.buttonSSHDisconnect.Location = new System.Drawing.Point(441, 6);
            this.buttonSSHDisconnect.Name = "buttonSSHDisconnect";
            this.buttonSSHDisconnect.Size = new System.Drawing.Size(83, 23);
            this.buttonSSHDisconnect.TabIndex = 5;
            this.buttonSSHDisconnect.Text = "切断(&D)";
            this.buttonSSHDisconnect.UseVisualStyleBackColor = true;
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(458, 324);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(83, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "閉じる";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // buttonCacnelAll
            // 
            this.buttonCacnelAll.Location = new System.Drawing.Point(441, 251);
            this.buttonCacnelAll.Name = "buttonCacnelAll";
            this.buttonCacnelAll.Size = new System.Drawing.Size(83, 23);
            this.buttonCacnelAll.TabIndex = 15;
            this.buttonCacnelAll.Text = "すべて中止(&A)";
            this.buttonCacnelAll.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(441, 18);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(83, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "中止(&S)";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // listViewTask
            // 
            this.listViewTask.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewTask.FullRowSelect = true;
            this.listViewTask.HideSelection = false;
            this.listViewTask.HoverSelection = true;
            this.listViewTask.Location = new System.Drawing.Point(3, 18);
            this.listViewTask.MultiSelect = false;
            this.listViewTask.Name = "listViewTask";
            this.listViewTask.Size = new System.Drawing.Size(404, 90);
            this.listViewTask.TabIndex = 1;
            this.listViewTask.UseCompatibleStateImageBehavior = false;
            this.listViewTask.View = System.Windows.Forms.View.Details;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageBackground);
            this.tabControl.Controls.Add(this.tabPageSSHConnection);
            this.tabControl.Location = new System.Drawing.Point(13, 13);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(538, 304);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageBackground
            // 
            this.tabPageBackground.Controls.Add(this.label3);
            this.tabPageBackground.Controls.Add(this.label2);
            this.tabPageBackground.Controls.Add(this.label4);
            this.tabPageBackground.Controls.Add(this.label7);
            this.tabPageBackground.Controls.Add(this.label6);
            this.tabPageBackground.Controls.Add(this.label5);
            this.tabPageBackground.Controls.Add(this.label1);
            this.tabPageBackground.Controls.Add(this.buttonCacnelAll);
            this.tabPageBackground.Controls.Add(this.buttonPriorityMinus);
            this.tabPageBackground.Controls.Add(this.buttonPriorityPlus);
            this.tabPageBackground.Controls.Add(this.buttonResume);
            this.tabPageBackground.Controls.Add(this.buttonSuspend);
            this.tabPageBackground.Controls.Add(this.buttonCancelWait);
            this.tabPageBackground.Controls.Add(this.buttonCancel);
            this.tabPageBackground.Controls.Add(this.listViewWait);
            this.tabPageBackground.Controls.Add(this.listViewTask);
            this.tabPageBackground.Location = new System.Drawing.Point(4, 22);
            this.tabPageBackground.Name = "tabPageBackground";
            this.tabPageBackground.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBackground.Size = new System.Drawing.Size(530, 278);
            this.tabPageBackground.TabIndex = 0;
            this.tabPageBackground.Text = "バックグラウンドタスク";
            this.tabPageBackground.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 256);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(409, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "プロセスの実行やファイルの取得など、ファイル操作以外のタスクは待機中にできません。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 244);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(388, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "ファイル操作のタスクが終了すると、待機中のタスクの1つが自動的に再開されます。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "実行中:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(408, 225);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 12);
            this.label7.TabIndex = 9;
            this.label7.Text = "低";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(408, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 8;
            this.label6.Text = "高";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(394, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "優先度";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "待機中:";
            // 
            // buttonPriorityMinus
            // 
            this.buttonPriorityMinus.Location = new System.Drawing.Point(441, 201);
            this.buttonPriorityMinus.Name = "buttonPriorityMinus";
            this.buttonPriorityMinus.Size = new System.Drawing.Size(83, 23);
            this.buttonPriorityMinus.TabIndex = 12;
            this.buttonPriorityMinus.Text = "優先度-1(&D)";
            this.buttonPriorityMinus.UseVisualStyleBackColor = true;
            // 
            // buttonPriorityPlus
            // 
            this.buttonPriorityPlus.Location = new System.Drawing.Point(441, 172);
            this.buttonPriorityPlus.Name = "buttonPriorityPlus";
            this.buttonPriorityPlus.Size = new System.Drawing.Size(83, 23);
            this.buttonPriorityPlus.TabIndex = 11;
            this.buttonPriorityPlus.Text = "優先度+1(&U)";
            this.buttonPriorityPlus.UseVisualStyleBackColor = true;
            // 
            // buttonResume
            // 
            this.buttonResume.Location = new System.Drawing.Point(221, 114);
            this.buttonResume.Name = "buttonResume";
            this.buttonResume.Size = new System.Drawing.Size(83, 23);
            this.buttonResume.TabIndex = 4;
            this.buttonResume.Text = "↑再開(&R)";
            this.buttonResume.UseVisualStyleBackColor = true;
            // 
            // buttonSuspend
            // 
            this.buttonSuspend.Location = new System.Drawing.Point(132, 114);
            this.buttonSuspend.Name = "buttonSuspend";
            this.buttonSuspend.Size = new System.Drawing.Size(83, 23);
            this.buttonSuspend.TabIndex = 3;
            this.buttonSuspend.Text = "↓中断(&S)";
            this.buttonSuspend.UseVisualStyleBackColor = true;
            // 
            // buttonCancelWait
            // 
            this.buttonCancelWait.Location = new System.Drawing.Point(441, 143);
            this.buttonCancelWait.Name = "buttonCancelWait";
            this.buttonCancelWait.Size = new System.Drawing.Size(83, 23);
            this.buttonCancelWait.TabIndex = 10;
            this.buttonCancelWait.Text = "中止(&Q)";
            this.buttonCancelWait.UseVisualStyleBackColor = true;
            // 
            // listViewWait
            // 
            this.listViewWait.FullRowSelect = true;
            this.listViewWait.HideSelection = false;
            this.listViewWait.Location = new System.Drawing.Point(3, 143);
            this.listViewWait.MultiSelect = false;
            this.listViewWait.Name = "listViewWait";
            this.listViewWait.Size = new System.Drawing.Size(404, 94);
            this.listViewWait.TabIndex = 6;
            this.listViewWait.UseCompatibleStateImageBehavior = false;
            this.listViewWait.View = System.Windows.Forms.View.Details;
            // 
            // tabPageSSHConnection
            // 
            this.tabPageSSHConnection.Controls.Add(this.buttonDisconnectAll);
            this.tabPageSSHConnection.Controls.Add(this.listViewSSH);
            this.tabPageSSHConnection.Controls.Add(this.buttonSSHDisconnect);
            this.tabPageSSHConnection.Location = new System.Drawing.Point(4, 22);
            this.tabPageSSHConnection.Name = "tabPageSSHConnection";
            this.tabPageSSHConnection.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSSHConnection.Size = new System.Drawing.Size(530, 278);
            this.tabPageSSHConnection.TabIndex = 1;
            this.tabPageSSHConnection.Text = "SSHセッション";
            this.tabPageSSHConnection.UseVisualStyleBackColor = true;
            // 
            // TaskManagerDialog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(563, 359);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TaskManagerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "バックグラウンドマネージャ";
            this.tabControl.ResumeLayout(false);
            this.tabPageBackground.ResumeLayout(false);
            this.tabPageBackground.PerformLayout();
            this.tabPageSSHConnection.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSSHDisconnect;
        private System.Windows.Forms.ListView listViewSSH;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonDisconnectAll;
        private System.Windows.Forms.Button buttonCacnelAll;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListView listViewTask;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageBackground;
        private System.Windows.Forms.TabPage tabPageSSHConnection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonPriorityMinus;
        private System.Windows.Forms.Button buttonPriorityPlus;
        private System.Windows.Forms.Button buttonSuspend;
        private System.Windows.Forms.Button buttonResume;
        private System.Windows.Forms.ListView listViewWait;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCancelWait;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;

    }
}