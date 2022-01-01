namespace ShellFiler.UI.Dialog
{
    partial class LogInDirectoryDialog
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
            this.tabPages = new System.Windows.Forms.TabControl();
            this.tabPageDrive = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewDrive = new System.Windows.Forms.ListView();
            this.tabPageRegDir = new System.Windows.Forms.TabPage();
            this.labelFreewareBook = new System.Windows.Forms.Label();
            this.buttonSetting = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxRegDir = new System.Windows.Forms.TextBox();
            this.listViewRegDir = new System.Windows.Forms.ListView();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxRegDirGroup = new System.Windows.Forms.ComboBox();
            this.tabPageSSH = new System.Windows.Forms.TabPage();
            this.linkLabelHelp = new System.Windows.Forms.LinkLabel();
            this.checkBoxSSHNewChannel = new System.Windows.Forms.CheckBox();
            this.radioButtonSSHShell = new System.Windows.Forms.RadioButton();
            this.radioButtonSSHSFTP = new System.Windows.Forms.RadioButton();
            this.labelProtocol = new System.Windows.Forms.Label();
            this.labelFreewareSSH = new System.Windows.Forms.Label();
            this.buttonSSHTemp = new System.Windows.Forms.Button();
            this.textBoxSSHFolder = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.buttonSSHEdit = new System.Windows.Forms.Button();
            this.buttonSSHDelete = new System.Windows.Forms.Button();
            this.buttonSSHNew = new System.Windows.Forms.Button();
            this.treeViewSSH = new System.Windows.Forms.TreeView();
            this.tabPageSSHDisable = new System.Windows.Forms.TabPage();
            this.label23 = new System.Windows.Forms.Label();
            this.linkLabelSSHDL = new System.Windows.Forms.LinkLabel();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.tabPageHistory = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.listBoxHistory = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabPages.SuspendLayout();
            this.tabPageDrive.SuspendLayout();
            this.tabPageRegDir.SuspendLayout();
            this.tabPageSSH.SuspendLayout();
            this.tabPageSSHDisable.SuspendLayout();
            this.tabPageHistory.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPages
            // 
            this.tabPages.Controls.Add(this.tabPageDrive);
            this.tabPages.Controls.Add(this.tabPageRegDir);
            this.tabPages.Controls.Add(this.tabPageSSH);
            this.tabPages.Controls.Add(this.tabPageSSHDisable);
            this.tabPages.Controls.Add(this.tabPageHistory);
            this.tabPages.Location = new System.Drawing.Point(13, 13);
            this.tabPages.Name = "tabPages";
            this.tabPages.SelectedIndex = 0;
            this.tabPages.Size = new System.Drawing.Size(474, 329);
            this.tabPages.TabIndex = 0;
            this.tabPages.SelectedIndexChanged += new System.EventHandler(this.tabPages_SelectedIndexChanged);
            // 
            // tabPageDrive
            // 
            this.tabPageDrive.Controls.Add(this.label5);
            this.tabPageDrive.Controls.Add(this.buttonDisconnect);
            this.tabPageDrive.Controls.Add(this.buttonConnect);
            this.tabPageDrive.Controls.Add(this.label1);
            this.tabPageDrive.Controls.Add(this.listViewDrive);
            this.tabPageDrive.Location = new System.Drawing.Point(4, 22);
            this.tabPageDrive.Name = "tabPageDrive";
            this.tabPageDrive.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDrive.Size = new System.Drawing.Size(466, 303);
            this.tabPageDrive.TabIndex = 0;
            this.tabPageDrive.Text = "ローカルドライブ";
            this.tabPageDrive.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(265, 289);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(190, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "A～Zで直接指定、←→でタブ切り替え";
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Location = new System.Drawing.Point(9, 240);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(125, 23);
            this.buttonDisconnect.TabIndex = 2;
            this.buttonDisconnect.Text = "ネットワークの切断(&U)...";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(9, 211);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(125, 23);
            this.buttonConnect.TabIndex = 2;
            this.buttonConnect.Text = "ネットワークの追加(&N)...";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "変更先ドライブ(&D):";
            // 
            // listViewDrive
            // 
            this.listViewDrive.HideSelection = false;
            this.listViewDrive.Location = new System.Drawing.Point(9, 22);
            this.listViewDrive.MultiSelect = false;
            this.listViewDrive.Name = "listViewDrive";
            this.listViewDrive.Size = new System.Drawing.Size(446, 183);
            this.listViewDrive.TabIndex = 1;
            this.listViewDrive.UseCompatibleStateImageBehavior = false;
            this.listViewDrive.View = System.Windows.Forms.View.Details;
            this.listViewDrive.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewDrive_MouseStateChange);
            this.listViewDrive.DoubleClick += new System.EventHandler(this.listViewDrive_DoubleClick);
            this.listViewDrive.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listViewDrive_MouseStateChange);
            this.listViewDrive.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewDrive_MouseStateChange);
            // 
            // tabPageRegDir
            // 
            this.tabPageRegDir.Controls.Add(this.labelFreewareBook);
            this.tabPageRegDir.Controls.Add(this.buttonSetting);
            this.tabPageRegDir.Controls.Add(this.label6);
            this.tabPageRegDir.Controls.Add(this.textBoxRegDir);
            this.tabPageRegDir.Controls.Add(this.listViewRegDir);
            this.tabPageRegDir.Controls.Add(this.label4);
            this.tabPageRegDir.Controls.Add(this.label3);
            this.tabPageRegDir.Controls.Add(this.label2);
            this.tabPageRegDir.Controls.Add(this.comboBoxRegDirGroup);
            this.tabPageRegDir.Location = new System.Drawing.Point(4, 22);
            this.tabPageRegDir.Name = "tabPageRegDir";
            this.tabPageRegDir.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRegDir.Size = new System.Drawing.Size(466, 303);
            this.tabPageRegDir.TabIndex = 1;
            this.tabPageRegDir.Text = "ブックマーク";
            this.tabPageRegDir.UseVisualStyleBackColor = true;
            // 
            // labelFreewareBook
            // 
            this.labelFreewareBook.AutoSize = true;
            this.labelFreewareBook.Location = new System.Drawing.Point(7, 289);
            this.labelFreewareBook.Name = "labelFreewareBook";
            this.labelFreewareBook.Size = new System.Drawing.Size(0, 12);
            this.labelFreewareBook.TabIndex = 10;
            // 
            // buttonSetting
            // 
            this.buttonSetting.Location = new System.Drawing.Point(380, 42);
            this.buttonSetting.Name = "buttonSetting";
            this.buttonSetting.Size = new System.Drawing.Size(75, 23);
            this.buttonSetting.TabIndex = 9;
            this.buttonSetting.Text = "設定(&S)...";
            this.buttonSetting.UseVisualStyleBackColor = true;
            this.buttonSetting.Click += new System.EventHandler(this.buttonSetting_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(265, 289);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(190, 12);
            this.label6.TabIndex = 8;
            this.label6.Text = "A～Zで直接指定、←→でタブ切り替え";
            // 
            // textBoxRegDir
            // 
            this.textBoxRegDir.Location = new System.Drawing.Point(5, 251);
            this.textBoxRegDir.Name = "textBoxRegDir";
            this.textBoxRegDir.Size = new System.Drawing.Size(450, 19);
            this.textBoxRegDir.TabIndex = 7;
            // 
            // listViewRegDir
            // 
            this.listViewRegDir.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewRegDir.Location = new System.Drawing.Point(5, 71);
            this.listViewRegDir.MultiSelect = false;
            this.listViewRegDir.Name = "listViewRegDir";
            this.listViewRegDir.Size = new System.Drawing.Size(450, 152);
            this.listViewRegDir.TabIndex = 5;
            this.listViewRegDir.UseCompatibleStateImageBehavior = false;
            this.listViewRegDir.View = System.Windows.Forms.View.Details;
            this.listViewRegDir.SelectedIndexChanged += new System.EventHandler(this.listViewRegDir_SelectedIndexChanged);
            this.listViewRegDir.DoubleClick += new System.EventHandler(this.listViewRegDir_DoubleClick);
            this.listViewRegDir.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listViewRegDir_MouseStateChange);
            this.listViewRegDir.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewRegDir_MouseStateChange);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 236);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "ジャンプ先のフォルダ名(&P):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "ジャンプ先(&S):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "グループ(&G):";
            // 
            // comboBoxRegDirGroup
            // 
            this.comboBoxRegDirGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRegDirGroup.FormattingEnabled = true;
            this.comboBoxRegDirGroup.Location = new System.Drawing.Point(5, 23);
            this.comboBoxRegDirGroup.Name = "comboBoxRegDirGroup";
            this.comboBoxRegDirGroup.Size = new System.Drawing.Size(232, 20);
            this.comboBoxRegDirGroup.TabIndex = 1;
            this.comboBoxRegDirGroup.SelectedValueChanged += new System.EventHandler(this.comboBoxRegDirGroup_SelectedValueChanged);
            // 
            // tabPageSSH
            // 
            this.tabPageSSH.Controls.Add(this.linkLabelHelp);
            this.tabPageSSH.Controls.Add(this.checkBoxSSHNewChannel);
            this.tabPageSSH.Controls.Add(this.radioButtonSSHShell);
            this.tabPageSSH.Controls.Add(this.radioButtonSSHSFTP);
            this.tabPageSSH.Controls.Add(this.labelProtocol);
            this.tabPageSSH.Controls.Add(this.labelFreewareSSH);
            this.tabPageSSH.Controls.Add(this.buttonSSHTemp);
            this.tabPageSSH.Controls.Add(this.textBoxSSHFolder);
            this.tabPageSSH.Controls.Add(this.label7);
            this.tabPageSSH.Controls.Add(this.label18);
            this.tabPageSSH.Controls.Add(this.buttonSSHEdit);
            this.tabPageSSH.Controls.Add(this.buttonSSHDelete);
            this.tabPageSSH.Controls.Add(this.buttonSSHNew);
            this.tabPageSSH.Controls.Add(this.treeViewSSH);
            this.tabPageSSH.Location = new System.Drawing.Point(4, 22);
            this.tabPageSSH.Name = "tabPageSSH";
            this.tabPageSSH.Size = new System.Drawing.Size(466, 303);
            this.tabPageSSH.TabIndex = 2;
            this.tabPageSSH.Text = "SSH";
            this.tabPageSSH.UseVisualStyleBackColor = true;
            // 
            // linkLabelHelp
            // 
            this.linkLabelHelp.AutoSize = true;
            this.linkLabelHelp.Location = new System.Drawing.Point(47, 248);
            this.linkLabelHelp.Name = "linkLabelHelp";
            this.linkLabelHelp.Size = new System.Drawing.Size(49, 12);
            this.linkLabelHelp.TabIndex = 13;
            this.linkLabelHelp.TabStop = true;
            this.linkLabelHelp.Text = "プロトコル";
            // 
            // checkBoxSSHNewChannel
            // 
            this.checkBoxSSHNewChannel.AutoSize = true;
            this.checkBoxSSHNewChannel.Location = new System.Drawing.Point(223, 264);
            this.checkBoxSSHNewChannel.Name = "checkBoxSSHNewChannel";
            this.checkBoxSSHNewChannel.Size = new System.Drawing.Size(163, 16);
            this.checkBoxSSHNewChannel.TabIndex = 10;
            this.checkBoxSSHNewChannel.Text = "常に新しいチャネルで接続(&C)";
            this.checkBoxSSHNewChannel.UseVisualStyleBackColor = true;
            // 
            // radioButtonSSHShell
            // 
            this.radioButtonSSHShell.AutoSize = true;
            this.radioButtonSSHShell.Location = new System.Drawing.Point(122, 264);
            this.radioButtonSSHShell.Name = "radioButtonSSHShell";
            this.radioButtonSSHShell.Size = new System.Drawing.Size(88, 16);
            this.radioButtonSSHShell.TabIndex = 9;
            this.radioButtonSSHShell.TabStop = true;
            this.radioButtonSSHShell.Text = "SSHシェル(&H)";
            this.radioButtonSSHShell.UseVisualStyleBackColor = true;
            // 
            // radioButtonSSHSFTP
            // 
            this.radioButtonSSHSFTP.AutoSize = true;
            this.radioButtonSSHSFTP.Location = new System.Drawing.Point(122, 246);
            this.radioButtonSSHSFTP.Name = "radioButtonSSHSFTP";
            this.radioButtonSSHSFTP.Size = new System.Drawing.Size(66, 16);
            this.radioButtonSSHSFTP.TabIndex = 8;
            this.radioButtonSSHSFTP.TabStop = true;
            this.radioButtonSSHSFTP.Text = "SFTP(&F)";
            this.radioButtonSSHSFTP.UseVisualStyleBackColor = true;
            // 
            // labelProtocol
            // 
            this.labelProtocol.AutoSize = true;
            this.labelProtocol.Location = new System.Drawing.Point(3, 248);
            this.labelProtocol.Name = "labelProtocol";
            this.labelProtocol.Size = new System.Drawing.Size(48, 12);
            this.labelProtocol.TabIndex = 7;
            this.labelProtocol.Text = "使用する";
            // 
            // labelFreewareSSH
            // 
            this.labelFreewareSSH.AutoSize = true;
            this.labelFreewareSSH.Location = new System.Drawing.Point(7, 289);
            this.labelFreewareSSH.Name = "labelFreewareSSH";
            this.labelFreewareSSH.Size = new System.Drawing.Size(0, 12);
            this.labelFreewareSSH.TabIndex = 11;
            // 
            // buttonSSHTemp
            // 
            this.buttonSSHTemp.Location = new System.Drawing.Point(372, 92);
            this.buttonSSHTemp.Name = "buttonSSHTemp";
            this.buttonSSHTemp.Size = new System.Drawing.Size(83, 23);
            this.buttonSSHTemp.TabIndex = 4;
            this.buttonSSHTemp.Text = "一時接続(&T)";
            this.buttonSSHTemp.UseVisualStyleBackColor = true;
            this.buttonSSHTemp.Click += new System.EventHandler(this.buttonSSHTemp_Click);
            // 
            // textBoxSSHFolder
            // 
            this.textBoxSSHFolder.Location = new System.Drawing.Point(5, 220);
            this.textBoxSSHFolder.Name = "textBoxSSHFolder";
            this.textBoxSSHFolder.Size = new System.Drawing.Size(360, 19);
            this.textBoxSSHFolder.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 205);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "接続先(&P):";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(358, 289);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(97, 12);
            this.label18.TabIndex = 12;
            this.label18.Text = "←→でタブ切り替え";
            // 
            // buttonSSHEdit
            // 
            this.buttonSSHEdit.Location = new System.Drawing.Point(371, 62);
            this.buttonSSHEdit.Name = "buttonSSHEdit";
            this.buttonSSHEdit.Size = new System.Drawing.Size(84, 23);
            this.buttonSSHEdit.TabIndex = 3;
            this.buttonSSHEdit.Text = "編集(&E)...";
            this.buttonSSHEdit.UseVisualStyleBackColor = true;
            // 
            // buttonSSHDelete
            // 
            this.buttonSSHDelete.Location = new System.Drawing.Point(371, 33);
            this.buttonSSHDelete.Name = "buttonSSHDelete";
            this.buttonSSHDelete.Size = new System.Drawing.Size(84, 23);
            this.buttonSSHDelete.TabIndex = 2;
            this.buttonSSHDelete.Text = "削除(&D)";
            this.buttonSSHDelete.UseVisualStyleBackColor = true;
            // 
            // buttonSSHNew
            // 
            this.buttonSSHNew.Location = new System.Drawing.Point(371, 4);
            this.buttonSSHNew.Name = "buttonSSHNew";
            this.buttonSSHNew.Size = new System.Drawing.Size(84, 23);
            this.buttonSSHNew.TabIndex = 1;
            this.buttonSSHNew.Text = "新規(&N)...";
            this.buttonSSHNew.UseVisualStyleBackColor = true;
            // 
            // treeViewSSH
            // 
            this.treeViewSSH.HideSelection = false;
            this.treeViewSSH.Indent = 14;
            this.treeViewSSH.Location = new System.Drawing.Point(5, 4);
            this.treeViewSSH.Name = "treeViewSSH";
            this.treeViewSSH.ShowRootLines = false;
            this.treeViewSSH.Size = new System.Drawing.Size(360, 194);
            this.treeViewSSH.TabIndex = 0;
            // 
            // tabPageSSHDisable
            // 
            this.tabPageSSHDisable.Controls.Add(this.label23);
            this.tabPageSSHDisable.Controls.Add(this.linkLabelSSHDL);
            this.tabPageSSHDisable.Controls.Add(this.label22);
            this.tabPageSSHDisable.Controls.Add(this.label21);
            this.tabPageSSHDisable.Controls.Add(this.label8);
            this.tabPageSSHDisable.Controls.Add(this.label20);
            this.tabPageSSHDisable.Location = new System.Drawing.Point(4, 22);
            this.tabPageSSHDisable.Name = "tabPageSSHDisable";
            this.tabPageSSHDisable.Size = new System.Drawing.Size(466, 303);
            this.tabPageSSHDisable.TabIndex = 2;
            this.tabPageSSHDisable.Text = "SSH";
            this.tabPageSSHDisable.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(358, 289);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(97, 12);
            this.label23.TabIndex = 4;
            this.label23.Text = "←→でタブ切り替え";
            // 
            // linkLabelSSHDL
            // 
            this.linkLabelSSHDL.AutoSize = true;
            this.linkLabelSSHDL.Location = new System.Drawing.Point(53, 143);
            this.linkLabelSSHDL.Name = "linkLabelSSHDL";
            this.linkLabelSSHDL.Size = new System.Drawing.Size(100, 12);
            this.linkLabelSSHDL.TabIndex = 4;
            this.linkLabelSSHDL.TabStop = true;
            this.linkLabelSSHDL.Text = "ダウンロードページへ";
            this.linkLabelSSHDL.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.linkLabelSSHDL_PreviewKeyDown);
            this.linkLabelSSHDL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSSHDL_LinkClicked);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(32, 117);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(343, 12);
            this.label22.TabIndex = 3;
            this.label22.Text = "すべてのDLLをShellFilerインストール先のbinフォルダにコピーしてください。";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(32, 95);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(247, 12);
            this.label21.TabIndex = 2;
            this.label21.Text = "SSHを使用するには、SharpSSHをダウンロードして、";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(32, 62);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(356, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "SSHを使うと、Linuxサーバなどに接続してファイルを操作することができます。";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(32, 38);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(223, 12);
            this.label20.TabIndex = 0;
            this.label20.Text = "この環境ではSSHを使用することができません。";
            // 
            // tabPageHistory
            // 
            this.tabPageHistory.Controls.Add(this.label10);
            this.tabPageHistory.Controls.Add(this.buttonDelete);
            this.tabPageHistory.Controls.Add(this.listBoxHistory);
            this.tabPageHistory.Controls.Add(this.label9);
            this.tabPageHistory.Location = new System.Drawing.Point(4, 22);
            this.tabPageHistory.Name = "tabPageHistory";
            this.tabPageHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHistory.Size = new System.Drawing.Size(466, 303);
            this.tabPageHistory.TabIndex = 3;
            this.tabPageHistory.Text = "フォルダ履歴";
            this.tabPageHistory.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(358, 289);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 12);
            this.label10.TabIndex = 8;
            this.label10.Text = "←→でタブ切り替え";
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(375, 253);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(85, 23);
            this.buttonDelete.TabIndex = 5;
            this.buttonDelete.Text = "履歴削除(&D)";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // listBoxHistory
            // 
            this.listBoxHistory.FormattingEnabled = true;
            this.listBoxHistory.ItemHeight = 12;
            this.listBoxHistory.Location = new System.Drawing.Point(6, 18);
            this.listBoxHistory.Name = "listBoxHistory";
            this.listBoxHistory.Size = new System.Drawing.Size(454, 232);
            this.listBoxHistory.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 12);
            this.label9.TabIndex = 3;
            this.label9.Text = "履歴(&F):";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(331, 348);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(412, 348);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // LogInDirectoryDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(497, 383);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.tabPages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogInDirectoryDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "フォルダを変更";
            this.Load += new System.EventHandler(this.LogInDirectoryDialog_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogInDirectoryDialog_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogInDirectoryDialog_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LogInDirectoryDialog_KeyDown);
            this.tabPages.ResumeLayout(false);
            this.tabPageDrive.ResumeLayout(false);
            this.tabPageDrive.PerformLayout();
            this.tabPageRegDir.ResumeLayout(false);
            this.tabPageRegDir.PerformLayout();
            this.tabPageSSH.ResumeLayout(false);
            this.tabPageSSH.PerformLayout();
            this.tabPageSSHDisable.ResumeLayout(false);
            this.tabPageSSHDisable.PerformLayout();
            this.tabPageHistory.ResumeLayout(false);
            this.tabPageHistory.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabPages;
        private System.Windows.Forms.TabPage tabPageDrive;
        private System.Windows.Forms.TabPage tabPageRegDir;
        private System.Windows.Forms.ListView listViewDrive;
        private System.Windows.Forms.TabPage tabPageSSH;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxRegDirGroup;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxRegDir;
        private System.Windows.Forms.ListView listViewRegDir;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TreeView treeViewSSH;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button buttonSSHDelete;
        private System.Windows.Forms.Button buttonSSHNew;
        private System.Windows.Forms.TabPage tabPageSSHDisable;
        private System.Windows.Forms.LinkLabel linkLabelSSHDL;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button buttonSSHEdit;
        private System.Windows.Forms.TextBox textBoxSSHFolder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonSetting;
        private System.Windows.Forms.Button buttonSSHTemp;
        private System.Windows.Forms.Label labelFreewareBook;
        private System.Windows.Forms.Label labelFreewareSSH;
        private System.Windows.Forms.TabPage tabPageHistory;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.ListBox listBoxHistory;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelProtocol;
        private System.Windows.Forms.RadioButton radioButtonSSHShell;
        private System.Windows.Forms.RadioButton radioButtonSSHSFTP;
        private System.Windows.Forms.CheckBox checkBoxSSHNewChannel;
        private System.Windows.Forms.LinkLabel linkLabelHelp;
    }
}