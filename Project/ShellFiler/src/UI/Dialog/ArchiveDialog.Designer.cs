namespace ShellFiler.UI.Dialog {
    partial class ArchiveDialog {
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
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxArcFileName = new System.Windows.Forms.TextBox();
            this.tabPageLocal7z = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonLTgz = new System.Windows.Forms.RadioButton();
            this.radioButtonL7z = new System.Windows.Forms.RadioButton();
            this.radioButtonLTar = new System.Windows.Forms.RadioButton();
            this.radioButtonLTbz2 = new System.Windows.Forms.RadioButton();
            this.radioButtonLZip = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panelLWarning = new System.Windows.Forms.Panel();
            this.buttonLRecommend = new System.Windows.Forms.Button();
            this.trackBarLLevel = new System.Windows.Forms.TrackBar();
            this.comboBoxLEncMethod = new System.Windows.Forms.ComboBox();
            this.comboBoxLMethod = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxLPassword = new System.Windows.Forms.ComboBox();
            this.buttonPasswordLManage = new System.Windows.Forms.Button();
            this.checkBoxLEncrypt = new System.Windows.Forms.CheckBox();
            this.checkBoxLTimeStamp = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageRemote = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.labelRCommand = new System.Windows.Forms.Label();
            this.textBoxRCommand = new System.Windows.Forms.TextBox();
            this.buttonRRecommend = new System.Windows.Forms.Button();
            this.trackBarRLevel = new System.Windows.Forms.TrackBar();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.checkBoxRTimeStamp = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonRTar = new System.Windows.Forms.RadioButton();
            this.radioButtonRTgz = new System.Windows.Forms.RadioButton();
            this.radioButtonRTbz2 = new System.Windows.Forms.RadioButton();
            this.radioButtonRZip = new System.Windows.Forms.RadioButton();
            this.textBoxDirectory = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tabPageLocal7z.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLLevel)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPageRemote.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRLevel)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(400, 340);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(481, 340);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "作成先フォルダ:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "書庫ファイル名(&N):";
            // 
            // textBoxArcFileName
            // 
            this.textBoxArcFileName.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxArcFileName.Location = new System.Drawing.Point(126, 40);
            this.textBoxArcFileName.Name = "textBoxArcFileName";
            this.textBoxArcFileName.Size = new System.Drawing.Size(321, 20);
            this.textBoxArcFileName.TabIndex = 3;
            this.textBoxArcFileName.TextChanged += new System.EventHandler(this.textBoxArcFileName_TextChanged);
            // 
            // tabPageLocal7z
            // 
            this.tabPageLocal7z.Controls.Add(this.groupBox1);
            this.tabPageLocal7z.Controls.Add(this.groupBox2);
            this.tabPageLocal7z.Location = new System.Drawing.Point(4, 24);
            this.tabPageLocal7z.Name = "tabPageLocal7z";
            this.tabPageLocal7z.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLocal7z.Size = new System.Drawing.Size(536, 241);
            this.tabPageLocal7z.TabIndex = 0;
            this.tabPageLocal7z.Text = "このコンピュータで圧縮";
            this.tabPageLocal7z.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonLTgz);
            this.groupBox1.Controls.Add(this.radioButtonL7z);
            this.groupBox1.Controls.Add(this.radioButtonLTar);
            this.groupBox1.Controls.Add(this.radioButtonLTbz2);
            this.groupBox1.Controls.Add(this.radioButtonLZip);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(113, 132);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "圧縮形式";
            // 
            // radioButtonLTgz
            // 
            this.radioButtonLTgz.AutoSize = true;
            this.radioButtonLTgz.Location = new System.Drawing.Point(6, 62);
            this.radioButtonLTgz.Name = "radioButtonLTgz";
            this.radioButtonLTgz.Size = new System.Drawing.Size(76, 19);
            this.radioButtonLTgz.TabIndex = 2;
            this.radioButtonLTgz.TabStop = true;
            this.radioButtonLTgz.Text = "tar.gz(&G)";
            this.radioButtonLTgz.UseVisualStyleBackColor = true;
            // 
            // radioButtonL7z
            // 
            this.radioButtonL7z.AutoSize = true;
            this.radioButtonL7z.Location = new System.Drawing.Point(6, 40);
            this.radioButtonL7z.Name = "radioButtonL7z";
            this.radioButtonL7z.Size = new System.Drawing.Size(73, 19);
            this.radioButtonL7z.TabIndex = 1;
            this.radioButtonL7z.TabStop = true;
            this.radioButtonL7z.Text = "7-Zip(&S)";
            this.radioButtonL7z.UseVisualStyleBackColor = true;
            // 
            // radioButtonLTar
            // 
            this.radioButtonLTar.AutoSize = true;
            this.radioButtonLTar.Location = new System.Drawing.Point(6, 106);
            this.radioButtonLTar.Name = "radioButtonLTar";
            this.radioButtonLTar.Size = new System.Drawing.Size(60, 19);
            this.radioButtonLTar.TabIndex = 4;
            this.radioButtonLTar.TabStop = true;
            this.radioButtonLTar.Text = "tar(&T)";
            this.radioButtonLTar.UseVisualStyleBackColor = true;
            // 
            // radioButtonLTbz2
            // 
            this.radioButtonLTbz2.AutoSize = true;
            this.radioButtonLTbz2.Location = new System.Drawing.Point(6, 84);
            this.radioButtonLTbz2.Name = "radioButtonLTbz2";
            this.radioButtonLTbz2.Size = new System.Drawing.Size(81, 19);
            this.radioButtonLTbz2.TabIndex = 3;
            this.radioButtonLTbz2.TabStop = true;
            this.radioButtonLTbz2.Text = "tar.bz2(&2)";
            this.radioButtonLTbz2.UseVisualStyleBackColor = true;
            // 
            // radioButtonLZip
            // 
            this.radioButtonLZip.AutoSize = true;
            this.radioButtonLZip.Location = new System.Drawing.Point(6, 18);
            this.radioButtonLZip.Name = "radioButtonLZip";
            this.radioButtonLZip.Size = new System.Drawing.Size(63, 19);
            this.radioButtonLZip.TabIndex = 0;
            this.radioButtonLZip.TabStop = true;
            this.radioButtonLZip.Text = "ZIP(&Z)";
            this.radioButtonLZip.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panelLWarning);
            this.groupBox2.Controls.Add(this.buttonLRecommend);
            this.groupBox2.Controls.Add(this.trackBarLLevel);
            this.groupBox2.Controls.Add(this.comboBoxLEncMethod);
            this.groupBox2.Controls.Add(this.comboBoxLMethod);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.comboBoxLPassword);
            this.groupBox2.Controls.Add(this.buttonPasswordLManage);
            this.groupBox2.Controls.Add(this.checkBoxLEncrypt);
            this.groupBox2.Controls.Add(this.checkBoxLTimeStamp);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(125, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(408, 231);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "オプション";
            // 
            // panelLWarning
            // 
            this.panelLWarning.Location = new System.Drawing.Point(30, 209);
            this.panelLWarning.Name = "panelLWarning";
            this.panelLWarning.Size = new System.Drawing.Size(372, 16);
            this.panelLWarning.TabIndex = 15;
            // 
            // buttonLRecommend
            // 
            this.buttonLRecommend.Location = new System.Drawing.Point(327, 51);
            this.buttonLRecommend.Name = "buttonLRecommend";
            this.buttonLRecommend.Size = new System.Drawing.Size(75, 23);
            this.buttonLRecommend.TabIndex = 14;
            this.buttonLRecommend.Text = "推奨(&C)";
            this.buttonLRecommend.UseVisualStyleBackColor = true;
            // 
            // trackBarLLevel
            // 
            this.trackBarLLevel.BackColor = System.Drawing.SystemColors.Window;
            this.trackBarLLevel.LargeChange = 2;
            this.trackBarLLevel.Location = new System.Drawing.Point(147, 91);
            this.trackBarLLevel.Maximum = 9;
            this.trackBarLLevel.Name = "trackBarLLevel";
            this.trackBarLLevel.Size = new System.Drawing.Size(118, 45);
            this.trackBarLLevel.TabIndex = 6;
            // 
            // comboBoxLEncMethod
            // 
            this.comboBoxLEncMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLEncMethod.FormattingEnabled = true;
            this.comboBoxLEncMethod.Location = new System.Drawing.Point(114, 179);
            this.comboBoxLEncMethod.Name = "comboBoxLEncMethod";
            this.comboBoxLEncMethod.Size = new System.Drawing.Size(198, 23);
            this.comboBoxLEncMethod.TabIndex = 13;
            // 
            // comboBoxLMethod
            // 
            this.comboBoxLMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLMethod.FormattingEnabled = true;
            this.comboBoxLMethod.Location = new System.Drawing.Point(114, 65);
            this.comboBoxLMethod.Name = "comboBoxLMethod";
            this.comboBoxLMethod.Size = new System.Drawing.Size(198, 23);
            this.comboBoxLMethod.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(271, 92);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 15);
            this.label7.TabIndex = 7;
            this.label7.Text = "高圧縮";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(112, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 15);
            this.label6.TabIndex = 5;
            this.label6.Text = "高速";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "圧縮率(&R):";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(28, 182);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 15);
            this.label9.TabIndex = 12;
            this.label9.Text = "暗号化方式(&E):";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(28, 68);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "アルゴリズム(&A):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "圧縮方法";
            // 
            // comboBoxLPassword
            // 
            this.comboBoxLPassword.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLPassword.FormattingEnabled = true;
            this.comboBoxLPassword.Location = new System.Drawing.Point(114, 153);
            this.comboBoxLPassword.Name = "comboBoxLPassword";
            this.comboBoxLPassword.Size = new System.Drawing.Size(198, 23);
            this.comboBoxLPassword.TabIndex = 10;
            // 
            // buttonPasswordLManage
            // 
            this.buttonPasswordLManage.Location = new System.Drawing.Point(327, 151);
            this.buttonPasswordLManage.Name = "buttonPasswordLManage";
            this.buttonPasswordLManage.Size = new System.Drawing.Size(75, 23);
            this.buttonPasswordLManage.TabIndex = 11;
            this.buttonPasswordLManage.Text = "管理(&M)...";
            this.buttonPasswordLManage.UseVisualStyleBackColor = true;
            // 
            // checkBoxLEncrypt
            // 
            this.checkBoxLEncrypt.AutoSize = true;
            this.checkBoxLEncrypt.Location = new System.Drawing.Point(6, 137);
            this.checkBoxLEncrypt.Name = "checkBoxLEncrypt";
            this.checkBoxLEncrypt.Size = new System.Drawing.Size(108, 19);
            this.checkBoxLEncrypt.TabIndex = 8;
            this.checkBoxLEncrypt.Text = "暗号化を行う(&P)";
            this.checkBoxLEncrypt.UseVisualStyleBackColor = true;
            // 
            // checkBoxLTimeStamp
            // 
            this.checkBoxLTimeStamp.AutoSize = true;
            this.checkBoxLTimeStamp.Location = new System.Drawing.Point(6, 18);
            this.checkBoxLTimeStamp.Name = "checkBoxLTimeStamp";
            this.checkBoxLTimeStamp.Size = new System.Drawing.Size(219, 19);
            this.checkBoxLTimeStamp.TabIndex = 0;
            this.checkBoxLTimeStamp.Text = "書庫の時刻を最新ファイルに合わせる(&T)";
            this.checkBoxLTimeStamp.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 156);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "パスワード(&W):";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageLocal7z);
            this.tabControl.Controls.Add(this.tabPageRemote);
            this.tabControl.Location = new System.Drawing.Point(12, 65);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(544, 269);
            this.tabControl.TabIndex = 4;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Selecting);
            // 
            // tabPageRemote
            // 
            this.tabPageRemote.Controls.Add(this.groupBox4);
            this.tabPageRemote.Controls.Add(this.groupBox3);
            this.tabPageRemote.Location = new System.Drawing.Point(4, 24);
            this.tabPageRemote.Name = "tabPageRemote";
            this.tabPageRemote.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRemote.Size = new System.Drawing.Size(536, 241);
            this.tabPageRemote.TabIndex = 1;
            this.tabPageRemote.Text = "リモートサーバーで圧縮";
            this.tabPageRemote.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.labelRCommand);
            this.groupBox4.Controls.Add(this.textBoxRCommand);
            this.groupBox4.Controls.Add(this.buttonRRecommend);
            this.groupBox4.Controls.Add(this.trackBarRLevel);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.label21);
            this.groupBox4.Controls.Add(this.label23);
            this.groupBox4.Controls.Add(this.label22);
            this.groupBox4.Controls.Add(this.label20);
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Controls.Add(this.checkBoxRTimeStamp);
            this.groupBox4.Location = new System.Drawing.Point(125, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(408, 185);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "オプション";
            // 
            // labelRCommand
            // 
            this.labelRCommand.AutoSize = true;
            this.labelRCommand.Location = new System.Drawing.Point(24, 126);
            this.labelRCommand.Name = "labelRCommand";
            this.labelRCommand.Size = new System.Drawing.Size(37, 15);
            this.labelRCommand.TabIndex = 7;
            this.labelRCommand.Text = "unzip";
            // 
            // textBoxRCommand
            // 
            this.textBoxRCommand.Location = new System.Drawing.Point(61, 123);
            this.textBoxRCommand.Name = "textBoxRCommand";
            this.textBoxRCommand.Size = new System.Drawing.Size(109, 23);
            this.textBoxRCommand.TabIndex = 8;
            // 
            // buttonRRecommend
            // 
            this.buttonRRecommend.Location = new System.Drawing.Point(327, 58);
            this.buttonRRecommend.Name = "buttonRRecommend";
            this.buttonRRecommend.Size = new System.Drawing.Size(75, 23);
            this.buttonRRecommend.TabIndex = 5;
            this.buttonRRecommend.Text = "推奨(&C)";
            this.buttonRRecommend.UseVisualStyleBackColor = true;
            // 
            // trackBarRLevel
            // 
            this.trackBarRLevel.BackColor = System.Drawing.SystemColors.Window;
            this.trackBarRLevel.LargeChange = 2;
            this.trackBarRLevel.Location = new System.Drawing.Point(123, 58);
            this.trackBarRLevel.Maximum = 9;
            this.trackBarRLevel.Name = "trackBarRLevel";
            this.trackBarRLevel.Size = new System.Drawing.Size(118, 45);
            this.trackBarRLevel.TabIndex = 3;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(247, 59);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(43, 15);
            this.label17.TabIndex = 4;
            this.label17.Text = "高圧縮";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(88, 59);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(31, 15);
            this.label18.TabIndex = 2;
            this.label18.Text = "高速";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(176, 126);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(213, 15);
            this.label21.TabIndex = 9;
            this.label21.Text = "<書庫ファイル名> <対象ファイルの一覧>";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(24, 167);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(268, 15);
            this.label23.TabIndex = 11;
            this.label23.Text = "コマンドの実行前に既存の書庫ファイルは削除されます。";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(24, 150);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(348, 15);
            this.label22.TabIndex = 10;
            this.label22.Text = "コマンドを誤って変更すると、関係ないファイルが破損する場合があります。";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 103);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(107, 15);
            this.label20.TabIndex = 6;
            this.label20.Text = "実行するコマンド(&C):";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(4, 58);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(66, 15);
            this.label19.TabIndex = 1;
            this.label19.Text = "圧縮率(&R):";
            // 
            // checkBoxRTimeStamp
            // 
            this.checkBoxRTimeStamp.AutoSize = true;
            this.checkBoxRTimeStamp.Location = new System.Drawing.Point(6, 18);
            this.checkBoxRTimeStamp.Name = "checkBoxRTimeStamp";
            this.checkBoxRTimeStamp.Size = new System.Drawing.Size(219, 19);
            this.checkBoxRTimeStamp.TabIndex = 0;
            this.checkBoxRTimeStamp.Text = "書庫の時刻を最新ファイルに合わせる(&T)";
            this.checkBoxRTimeStamp.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonRTar);
            this.groupBox3.Controls.Add(this.radioButtonRTgz);
            this.groupBox3.Controls.Add(this.radioButtonRTbz2);
            this.groupBox3.Controls.Add(this.radioButtonRZip);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(113, 110);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "圧縮形式";
            // 
            // radioButtonRTar
            // 
            this.radioButtonRTar.AutoSize = true;
            this.radioButtonRTar.Location = new System.Drawing.Point(6, 84);
            this.radioButtonRTar.Name = "radioButtonRTar";
            this.radioButtonRTar.Size = new System.Drawing.Size(66, 19);
            this.radioButtonRTar.TabIndex = 3;
            this.radioButtonRTar.TabStop = true;
            this.radioButtonRTar.Text = "TAR(&T)";
            this.radioButtonRTar.UseVisualStyleBackColor = true;
            // 
            // radioButtonRTgz
            // 
            this.radioButtonRTgz.AutoSize = true;
            this.radioButtonRTgz.Location = new System.Drawing.Point(6, 40);
            this.radioButtonRTgz.Name = "radioButtonRTgz";
            this.radioButtonRTgz.Size = new System.Drawing.Size(88, 19);
            this.radioButtonRTgz.TabIndex = 1;
            this.radioButtonRTgz.TabStop = true;
            this.radioButtonRTgz.Text = "TAR.GZ(&G)";
            this.radioButtonRTgz.UseVisualStyleBackColor = true;
            // 
            // radioButtonRTbz2
            // 
            this.radioButtonRTbz2.AutoSize = true;
            this.radioButtonRTbz2.Location = new System.Drawing.Point(6, 62);
            this.radioButtonRTbz2.Name = "radioButtonRTbz2";
            this.radioButtonRTbz2.Size = new System.Drawing.Size(92, 19);
            this.radioButtonRTbz2.TabIndex = 2;
            this.radioButtonRTbz2.TabStop = true;
            this.radioButtonRTbz2.Text = "TAR.BZ2(&2)";
            this.radioButtonRTbz2.UseVisualStyleBackColor = true;
            // 
            // radioButtonRZip
            // 
            this.radioButtonRZip.AutoSize = true;
            this.radioButtonRZip.Location = new System.Drawing.Point(6, 18);
            this.radioButtonRZip.Name = "radioButtonRZip";
            this.radioButtonRZip.Size = new System.Drawing.Size(63, 19);
            this.radioButtonRZip.TabIndex = 0;
            this.radioButtonRZip.TabStop = true;
            this.radioButtonRZip.Text = "ZIP(&Z)";
            this.radioButtonRZip.UseVisualStyleBackColor = true;
            // 
            // textBoxDirectory
            // 
            this.textBoxDirectory.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxDirectory.Location = new System.Drawing.Point(126, 13);
            this.textBoxDirectory.Name = "textBoxDirectory";
            this.textBoxDirectory.ReadOnly = true;
            this.textBoxDirectory.Size = new System.Drawing.Size(321, 20);
            this.textBoxDirectory.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(271, 99);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 7;
            this.label10.Text = "高圧縮";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(112, 99);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 12);
            this.label11.TabIndex = 5;
            this.label11.Text = "高速";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(28, 98);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 12);
            this.label12.TabIndex = 4;
            this.label12.Text = "圧縮率(&R):";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(28, 189);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(82, 12);
            this.label13.TabIndex = 12;
            this.label13.Text = "暗号化方式(&E):";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(28, 75);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(78, 12);
            this.label14.TabIndex = 2;
            this.label14.Text = "アルゴリズム(&A):";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 57);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 12);
            this.label15.TabIndex = 1;
            this.label15.Text = "圧縮方法";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(28, 163);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(71, 12);
            this.label16.TabIndex = 9;
            this.label16.Text = "パスワード(&W):";
            // 
            // ArchiveDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(568, 375);
            this.Controls.Add(this.textBoxDirectory);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.textBoxArcFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArchiveDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ファイルの圧縮";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ArchiveDialog_FormClosing);
            this.tabPageLocal7z.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLLevel)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabPageRemote.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRLevel)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxArcFileName;
        private System.Windows.Forms.TabPage tabPageLocal7z;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonLTgz;
        private System.Windows.Forms.RadioButton radioButtonL7z;
        private System.Windows.Forms.RadioButton radioButtonLTar;
        private System.Windows.Forms.RadioButton radioButtonLTbz2;
        private System.Windows.Forms.RadioButton radioButtonLZip;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBoxLPassword;
        private System.Windows.Forms.Button buttonPasswordLManage;
        private System.Windows.Forms.CheckBox checkBoxLEncrypt;
        private System.Windows.Forms.CheckBox checkBoxLTimeStamp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TextBox textBoxDirectory;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar trackBarLLevel;
        private System.Windows.Forms.ComboBox comboBoxLMethod;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonLRecommend;
        private System.Windows.Forms.ComboBox comboBoxLEncMethod;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TabPage tabPageRemote;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button buttonRRecommend;
        private System.Windows.Forms.TrackBar trackBarRLevel;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.CheckBox checkBoxRTimeStamp;
        private System.Windows.Forms.RadioButton radioButtonRTar;
        private System.Windows.Forms.RadioButton radioButtonRTgz;
        private System.Windows.Forms.RadioButton radioButtonRTbz2;
        private System.Windows.Forms.RadioButton radioButtonRZip;
        private System.Windows.Forms.TextBox textBoxRCommand;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Panel panelLWarning;
        private System.Windows.Forms.Label labelRCommand;
    }
}