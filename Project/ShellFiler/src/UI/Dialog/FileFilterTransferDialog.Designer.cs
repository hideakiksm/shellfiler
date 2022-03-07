namespace ShellFiler.UI.Dialog {
    partial class FileFilterTransferDialog {
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
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDest = new System.Windows.Forms.TextBox();
            this.textBoxSrc = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageFixed = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.radioShellFilerDump = new System.Windows.Forms.RadioButton();
            this.comboBoxDefinedTargetExt = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBoxDefinedOther = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.radioDeleteEmptyLine = new System.Windows.Forms.RadioButton();
            this.radioReturnLF = new System.Windows.Forms.RadioButton();
            this.radioReturnCRLF = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.radioQuick4 = new System.Windows.Forms.RadioButton();
            this.radioQuick3 = new System.Windows.Forms.RadioButton();
            this.radioQuick2 = new System.Windows.Forms.RadioButton();
            this.radioQuick1 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.radioUTF8Space8Tab = new System.Windows.Forms.RadioButton();
            this.radioUTF8Space4Tab = new System.Windows.Forms.RadioButton();
            this.radioShiftJISSpace8Tab = new System.Windows.Forms.RadioButton();
            this.radioUTF88TabSpace = new System.Windows.Forms.RadioButton();
            this.radioUTF84TabSpace = new System.Windows.Forms.RadioButton();
            this.radioShiftJIS8TabSpace = new System.Windows.Forms.RadioButton();
            this.radioShiftJISSpace4Tab = new System.Windows.Forms.RadioButton();
            this.radioShiftJIS4TabSpace = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.radioEUCToShiftJIS = new System.Windows.Forms.RadioButton();
            this.radioUTF8ToShiftJIS = new System.Windows.Forms.RadioButton();
            this.radioShiftJISToEUC = new System.Windows.Forms.RadioButton();
            this.radioShiftJISToUTF8 = new System.Windows.Forms.RadioButton();
            this.tabPageQuick = new System.Windows.Forms.TabPage();
            this.comboBoxQuickTargetExt = new System.Windows.Forms.ComboBox();
            this.comboBoxQuickOther = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxQuickFilter = new System.Windows.Forms.ListBox();
            this.panelQuickComponent = new System.Windows.Forms.Panel();
            this.tabPageDetail = new System.Windows.Forms.TabPage();
            this.comboBoxDetailOther = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonDetailSelectAll = new System.Windows.Forms.Button();
            this.buttonDetailSelectClear = new System.Windows.Forms.Button();
            this.buttonDetailDelete = new System.Windows.Forms.Button();
            this.buttonDetailEdit = new System.Windows.Forms.Button();
            this.buttonDetailAdd = new System.Windows.Forms.Button();
            this.listBoxDetailFileType = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelSrcFileCount = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelFreeware = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPageFixed.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageQuick.SuspendLayout();
            this.tabPageDetail.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "転送元(&S):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "転送先(&T):";
            // 
            // textBoxDest
            // 
            this.textBoxDest.Location = new System.Drawing.Point(78, 37);
            this.textBoxDest.Name = "textBoxDest";
            this.textBoxDest.ReadOnly = true;
            this.textBoxDest.Size = new System.Drawing.Size(400, 23);
            this.textBoxDest.TabIndex = 4;
            // 
            // textBoxSrc
            // 
            this.textBoxSrc.Location = new System.Drawing.Point(78, 12);
            this.textBoxSrc.Name = "textBoxSrc";
            this.textBoxSrc.ReadOnly = true;
            this.textBoxSrc.Size = new System.Drawing.Size(400, 23);
            this.textBoxSrc.TabIndex = 1;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageFixed);
            this.tabControl.Controls.Add(this.tabPageQuick);
            this.tabControl.Controls.Add(this.tabPageDetail);
            this.tabControl.Location = new System.Drawing.Point(15, 69);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(676, 300);
            this.tabControl.TabIndex = 5;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPageFixed
            // 
            this.tabPageFixed.Controls.Add(this.groupBox5);
            this.tabPageFixed.Controls.Add(this.comboBoxDefinedTargetExt);
            this.tabPageFixed.Controls.Add(this.label16);
            this.tabPageFixed.Controls.Add(this.comboBoxDefinedOther);
            this.tabPageFixed.Controls.Add(this.label17);
            this.tabPageFixed.Controls.Add(this.label14);
            this.tabPageFixed.Controls.Add(this.label18);
            this.tabPageFixed.Controls.Add(this.groupBox2);
            this.tabPageFixed.Controls.Add(this.groupBox4);
            this.tabPageFixed.Controls.Add(this.groupBox3);
            this.tabPageFixed.Controls.Add(this.groupBox1);
            this.tabPageFixed.Location = new System.Drawing.Point(4, 24);
            this.tabPageFixed.Name = "tabPageFixed";
            this.tabPageFixed.Size = new System.Drawing.Size(668, 272);
            this.tabPageFixed.TabIndex = 2;
            this.tabPageFixed.Text = "定義済み変換";
            this.tabPageFixed.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label19);
            this.groupBox5.Controls.Add(this.radioShellFilerDump);
            this.groupBox5.Location = new System.Drawing.Point(212, 211);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(203, 60);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "ダンプ";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 37);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(88, 15);
            this.label19.TabIndex = 1;
            this.label19.Text = "※UTF-8を想定";
            // 
            // radioShellFilerDump
            // 
            this.radioShellFilerDump.AutoSize = true;
            this.radioShellFilerDump.Location = new System.Drawing.Point(6, 18);
            this.radioShellFilerDump.Name = "radioShellFilerDump";
            this.radioShellFilerDump.Size = new System.Drawing.Size(162, 19);
            this.radioShellFilerDump.TabIndex = 0;
            this.radioShellFilerDump.TabStop = true;
            this.radioShellFilerDump.Text = "ShellFilerダンプ形式に加工";
            this.radioShellFilerDump.UseVisualStyleBackColor = true;
            // 
            // comboBoxDefinedTargetExt
            // 
            this.comboBoxDefinedTargetExt.FormattingEnabled = true;
            this.comboBoxDefinedTargetExt.Location = new System.Drawing.Point(442, 174);
            this.comboBoxDefinedTargetExt.Name = "comboBoxDefinedTargetExt";
            this.comboBoxDefinedTargetExt.Size = new System.Drawing.Size(221, 23);
            this.comboBoxDefinedTargetExt.TabIndex = 6;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(470, 259);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(197, 15);
            this.label16.TabIndex = 10;
            this.label16.Text = "固定の設定でフィルターを適用できます。";
            // 
            // comboBoxDefinedOther
            // 
            this.comboBoxDefinedOther.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDefinedOther.FormattingEnabled = true;
            this.comboBoxDefinedOther.Location = new System.Drawing.Point(442, 215);
            this.comboBoxDefinedOther.Name = "comboBoxDefinedOther";
            this.comboBoxDefinedOther.Size = new System.Drawing.Size(221, 23);
            this.comboBoxDefinedOther.TabIndex = 8;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(421, 199);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(106, 15);
            this.label17.TabIndex = 7;
            this.label17.Text = "その他のファイル(&O):";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(470, 243);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(195, 15);
            this.label14.TabIndex = 9;
            this.label14.Text = "定義済み変換ではあらかじめ定められた";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(421, 157);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(88, 15);
            this.label18.TabIndex = 5;
            this.label18.Text = "対象ファイル(&T):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.radioDeleteEmptyLine);
            this.groupBox2.Controls.Add(this.radioReturnLF);
            this.groupBox2.Controls.Add(this.radioReturnCRLF);
            this.groupBox2.Location = new System.Drawing.Point(3, 155);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(203, 116);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "テキスト変換";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 90);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(169, 15);
            this.label11.TabIndex = 3;
            this.label11.Text = "※マルチバイト系文字コードを想定";
            // 
            // radioDeleteEmptyLine
            // 
            this.radioDeleteEmptyLine.AutoSize = true;
            this.radioDeleteEmptyLine.Location = new System.Drawing.Point(6, 62);
            this.radioDeleteEmptyLine.Name = "radioDeleteEmptyLine";
            this.radioDeleteEmptyLine.Size = new System.Drawing.Size(82, 19);
            this.radioDeleteEmptyLine.TabIndex = 2;
            this.radioDeleteEmptyLine.TabStop = true;
            this.radioDeleteEmptyLine.Text = "空行を削除";
            this.radioDeleteEmptyLine.UseVisualStyleBackColor = true;
            // 
            // radioReturnLF
            // 
            this.radioReturnLF.AutoSize = true;
            this.radioReturnLF.Location = new System.Drawing.Point(6, 40);
            this.radioReturnLF.Name = "radioReturnLF";
            this.radioReturnLF.Size = new System.Drawing.Size(105, 19);
            this.radioReturnLF.TabIndex = 1;
            this.radioReturnLF.TabStop = true;
            this.radioReturnLF.Text = "改行をLFに変換";
            this.radioReturnLF.UseVisualStyleBackColor = true;
            // 
            // radioReturnCRLF
            // 
            this.radioReturnCRLF.AutoSize = true;
            this.radioReturnCRLF.Location = new System.Drawing.Point(6, 18);
            this.radioReturnCRLF.Name = "radioReturnCRLF";
            this.radioReturnCRLF.Size = new System.Drawing.Size(131, 19);
            this.radioReturnCRLF.TabIndex = 0;
            this.radioReturnCRLF.TabStop = true;
            this.radioReturnCRLF.Text = "改行をCR+LFに変換";
            this.radioReturnCRLF.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.radioQuick4);
            this.groupBox4.Controls.Add(this.radioQuick3);
            this.groupBox4.Controls.Add(this.radioQuick2);
            this.groupBox4.Controls.Add(this.radioQuick1);
            this.groupBox4.Location = new System.Drawing.Point(421, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(244, 146);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "クイック設定";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 127);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(227, 15);
            this.label15.TabIndex = 5;
            this.label15.Text = "   コマンドで設定したフィルターを使用できます。";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 113);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(200, 15);
            this.label13.TabIndex = 4;
            this.label13.Text = "※「クリップボードをフィルター経由で保存」";
            // 
            // radioQuick4
            // 
            this.radioQuick4.AutoSize = true;
            this.radioQuick4.Location = new System.Drawing.Point(6, 84);
            this.radioQuick4.Name = "radioQuick4";
            this.radioQuick4.Size = new System.Drawing.Size(64, 19);
            this.radioQuick4.TabIndex = 3;
            this.radioQuick4.TabStop = true;
            this.radioQuick4.Text = "クイック4";
            this.radioQuick4.UseVisualStyleBackColor = true;
            // 
            // radioQuick3
            // 
            this.radioQuick3.AutoSize = true;
            this.radioQuick3.Location = new System.Drawing.Point(6, 62);
            this.radioQuick3.Name = "radioQuick3";
            this.radioQuick3.Size = new System.Drawing.Size(64, 19);
            this.radioQuick3.TabIndex = 2;
            this.radioQuick3.TabStop = true;
            this.radioQuick3.Text = "クイック3";
            this.radioQuick3.UseVisualStyleBackColor = true;
            // 
            // radioQuick2
            // 
            this.radioQuick2.AutoSize = true;
            this.radioQuick2.Location = new System.Drawing.Point(6, 40);
            this.radioQuick2.Name = "radioQuick2";
            this.radioQuick2.Size = new System.Drawing.Size(64, 19);
            this.radioQuick2.TabIndex = 1;
            this.radioQuick2.TabStop = true;
            this.radioQuick2.Text = "クイック2";
            this.radioQuick2.UseVisualStyleBackColor = true;
            // 
            // radioQuick1
            // 
            this.radioQuick1.AutoSize = true;
            this.radioQuick1.Location = new System.Drawing.Point(6, 18);
            this.radioQuick1.Name = "radioQuick1";
            this.radioQuick1.Size = new System.Drawing.Size(69, 19);
            this.radioQuick1.TabIndex = 0;
            this.radioQuick1.TabStop = true;
            this.radioQuick1.Text = "クイック１";
            this.radioQuick1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.radioUTF8Space8Tab);
            this.groupBox3.Controls.Add(this.radioUTF8Space4Tab);
            this.groupBox3.Controls.Add(this.radioShiftJISSpace8Tab);
            this.groupBox3.Controls.Add(this.radioUTF88TabSpace);
            this.groupBox3.Controls.Add(this.radioUTF84TabSpace);
            this.groupBox3.Controls.Add(this.radioShiftJIS8TabSpace);
            this.groupBox3.Controls.Add(this.radioShiftJISSpace4Tab);
            this.groupBox3.Controls.Add(this.radioShiftJIS4TabSpace);
            this.groupBox3.Location = new System.Drawing.Point(212, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(203, 206);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tab-Space変換";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 184);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(166, 15);
            this.label12.TabIndex = 8;
            this.label12.Text = "※文字コード不一致はエラー停止";
            // 
            // radioUTF8Space8Tab
            // 
            this.radioUTF8Space8Tab.AutoSize = true;
            this.radioUTF8Space8Tab.Location = new System.Drawing.Point(6, 158);
            this.radioUTF8Space8Tab.Name = "radioUTF8Space8Tab";
            this.radioUTF8Space8Tab.Size = new System.Drawing.Size(151, 19);
            this.radioUTF8Space8Tab.TabIndex = 7;
            this.radioUTF8Space8Tab.TabStop = true;
            this.radioUTF8Space8Tab.Text = "UTF-8 空白→8tab変換";
            this.radioUTF8Space8Tab.UseVisualStyleBackColor = true;
            // 
            // radioUTF8Space4Tab
            // 
            this.radioUTF8Space4Tab.AutoSize = true;
            this.radioUTF8Space4Tab.Location = new System.Drawing.Point(6, 118);
            this.radioUTF8Space4Tab.Name = "radioUTF8Space4Tab";
            this.radioUTF8Space4Tab.Size = new System.Drawing.Size(151, 19);
            this.radioUTF8Space4Tab.TabIndex = 5;
            this.radioUTF8Space4Tab.TabStop = true;
            this.radioUTF8Space4Tab.Text = "UTF-8 空白→4tab変換";
            this.radioUTF8Space4Tab.UseVisualStyleBackColor = true;
            // 
            // radioShiftJISSpace8Tab
            // 
            this.radioShiftJISSpace8Tab.AutoSize = true;
            this.radioShiftJISSpace8Tab.Location = new System.Drawing.Point(6, 78);
            this.radioShiftJISSpace8Tab.Name = "radioShiftJISSpace8Tab";
            this.radioShiftJISSpace8Tab.Size = new System.Drawing.Size(160, 19);
            this.radioShiftJISSpace8Tab.TabIndex = 3;
            this.radioShiftJISSpace8Tab.TabStop = true;
            this.radioShiftJISSpace8Tab.Text = "ShiftJIS 空白→8tab変換";
            this.radioShiftJISSpace8Tab.UseVisualStyleBackColor = true;
            // 
            // radioUTF88TabSpace
            // 
            this.radioUTF88TabSpace.AutoSize = true;
            this.radioUTF88TabSpace.Location = new System.Drawing.Point(6, 138);
            this.radioUTF88TabSpace.Name = "radioUTF88TabSpace";
            this.radioUTF88TabSpace.Size = new System.Drawing.Size(152, 19);
            this.radioUTF88TabSpace.TabIndex = 6;
            this.radioUTF88TabSpace.TabStop = true;
            this.radioUTF88TabSpace.Text = "UTF-8 8tab→空白変換";
            this.radioUTF88TabSpace.UseVisualStyleBackColor = true;
            // 
            // radioUTF84TabSpace
            // 
            this.radioUTF84TabSpace.AutoSize = true;
            this.radioUTF84TabSpace.Location = new System.Drawing.Point(6, 98);
            this.radioUTF84TabSpace.Name = "radioUTF84TabSpace";
            this.radioUTF84TabSpace.Size = new System.Drawing.Size(152, 19);
            this.radioUTF84TabSpace.TabIndex = 4;
            this.radioUTF84TabSpace.TabStop = true;
            this.radioUTF84TabSpace.Text = "UTF-8 4tab→空白変換";
            this.radioUTF84TabSpace.UseVisualStyleBackColor = true;
            // 
            // radioShiftJIS8TabSpace
            // 
            this.radioShiftJIS8TabSpace.AutoSize = true;
            this.radioShiftJIS8TabSpace.Location = new System.Drawing.Point(6, 58);
            this.radioShiftJIS8TabSpace.Name = "radioShiftJIS8TabSpace";
            this.radioShiftJIS8TabSpace.Size = new System.Drawing.Size(161, 19);
            this.radioShiftJIS8TabSpace.TabIndex = 2;
            this.radioShiftJIS8TabSpace.TabStop = true;
            this.radioShiftJIS8TabSpace.Text = "ShiftJIS 8tab→空白変換";
            this.radioShiftJIS8TabSpace.UseVisualStyleBackColor = true;
            // 
            // radioShiftJISSpace4Tab
            // 
            this.radioShiftJISSpace4Tab.AutoSize = true;
            this.radioShiftJISSpace4Tab.Location = new System.Drawing.Point(6, 38);
            this.radioShiftJISSpace4Tab.Name = "radioShiftJISSpace4Tab";
            this.radioShiftJISSpace4Tab.Size = new System.Drawing.Size(160, 19);
            this.radioShiftJISSpace4Tab.TabIndex = 1;
            this.radioShiftJISSpace4Tab.TabStop = true;
            this.radioShiftJISSpace4Tab.Text = "ShiftJIS 空白→4tab変換";
            this.radioShiftJISSpace4Tab.UseVisualStyleBackColor = true;
            // 
            // radioShiftJIS4TabSpace
            // 
            this.radioShiftJIS4TabSpace.AutoSize = true;
            this.radioShiftJIS4TabSpace.Location = new System.Drawing.Point(6, 18);
            this.radioShiftJIS4TabSpace.Name = "radioShiftJIS4TabSpace";
            this.radioShiftJIS4TabSpace.Size = new System.Drawing.Size(161, 19);
            this.radioShiftJIS4TabSpace.TabIndex = 0;
            this.radioShiftJIS4TabSpace.TabStop = true;
            this.radioShiftJIS4TabSpace.Text = "ShiftJIS 4tab→空白変換";
            this.radioShiftJIS4TabSpace.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.radioEUCToShiftJIS);
            this.groupBox1.Controls.Add(this.radioUTF8ToShiftJIS);
            this.groupBox1.Controls.Add(this.radioShiftJISToEUC);
            this.groupBox1.Controls.Add(this.radioShiftJISToUTF8);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 146);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "文字コードの変換";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 115);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(140, 15);
            this.label10.TabIndex = 4;
            this.label10.Text = "※変換失敗時はエラー停止";
            // 
            // radioEUCToShiftJIS
            // 
            this.radioEUCToShiftJIS.AutoSize = true;
            this.radioEUCToShiftJIS.Location = new System.Drawing.Point(6, 84);
            this.radioEUCToShiftJIS.Name = "radioEUCToShiftJIS";
            this.radioEUCToShiftJIS.Size = new System.Drawing.Size(106, 19);
            this.radioEUCToShiftJIS.TabIndex = 3;
            this.radioEUCToShiftJIS.TabStop = true;
            this.radioEUCToShiftJIS.Text = "EUC→ShiftJIS";
            this.radioEUCToShiftJIS.UseVisualStyleBackColor = true;
            // 
            // radioUTF8ToShiftJIS
            // 
            this.radioUTF8ToShiftJIS.AutoSize = true;
            this.radioUTF8ToShiftJIS.Location = new System.Drawing.Point(6, 62);
            this.radioUTF8ToShiftJIS.Name = "radioUTF8ToShiftJIS";
            this.radioUTF8ToShiftJIS.Size = new System.Drawing.Size(118, 19);
            this.radioUTF8ToShiftJIS.TabIndex = 2;
            this.radioUTF8ToShiftJIS.TabStop = true;
            this.radioUTF8ToShiftJIS.Text = "UTF-8→ShiftJIS";
            this.radioUTF8ToShiftJIS.UseVisualStyleBackColor = true;
            // 
            // radioShiftJISToEUC
            // 
            this.radioShiftJISToEUC.AutoSize = true;
            this.radioShiftJISToEUC.Location = new System.Drawing.Point(6, 40);
            this.radioShiftJISToEUC.Name = "radioShiftJISToEUC";
            this.radioShiftJISToEUC.Size = new System.Drawing.Size(106, 19);
            this.radioShiftJISToEUC.TabIndex = 1;
            this.radioShiftJISToEUC.TabStop = true;
            this.radioShiftJISToEUC.Text = "ShiftJIS→EUC";
            this.radioShiftJISToEUC.UseVisualStyleBackColor = true;
            // 
            // radioShiftJISToUTF8
            // 
            this.radioShiftJISToUTF8.AutoSize = true;
            this.radioShiftJISToUTF8.Location = new System.Drawing.Point(6, 18);
            this.radioShiftJISToUTF8.Name = "radioShiftJISToUTF8";
            this.radioShiftJISToUTF8.Size = new System.Drawing.Size(118, 19);
            this.radioShiftJISToUTF8.TabIndex = 0;
            this.radioShiftJISToUTF8.TabStop = true;
            this.radioShiftJISToUTF8.Text = "ShiftJIS→UTF-8";
            this.radioShiftJISToUTF8.UseVisualStyleBackColor = true;
            // 
            // tabPageQuick
            // 
            this.tabPageQuick.Controls.Add(this.comboBoxQuickTargetExt);
            this.tabPageQuick.Controls.Add(this.comboBoxQuickOther);
            this.tabPageQuick.Controls.Add(this.label4);
            this.tabPageQuick.Controls.Add(this.label7);
            this.tabPageQuick.Controls.Add(this.label5);
            this.tabPageQuick.Controls.Add(this.label2);
            this.tabPageQuick.Controls.Add(this.listBoxQuickFilter);
            this.tabPageQuick.Controls.Add(this.panelQuickComponent);
            this.tabPageQuick.Location = new System.Drawing.Point(4, 24);
            this.tabPageQuick.Name = "tabPageQuick";
            this.tabPageQuick.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageQuick.Size = new System.Drawing.Size(668, 272);
            this.tabPageQuick.TabIndex = 1;
            this.tabPageQuick.Text = "簡易モード";
            this.tabPageQuick.UseVisualStyleBackColor = true;
            // 
            // comboBoxQuickTargetExt
            // 
            this.comboBoxQuickTargetExt.FormattingEnabled = true;
            this.comboBoxQuickTargetExt.Location = new System.Drawing.Point(420, 61);
            this.comboBoxQuickTargetExt.Name = "comboBoxQuickTargetExt";
            this.comboBoxQuickTargetExt.Size = new System.Drawing.Size(242, 23);
            this.comboBoxQuickTargetExt.TabIndex = 4;
            // 
            // comboBoxQuickOther
            // 
            this.comboBoxQuickOther.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxQuickOther.FormattingEnabled = true;
            this.comboBoxQuickOther.Location = new System.Drawing.Point(420, 87);
            this.comboBoxQuickOther.Name = "comboBoxQuickOther";
            this.comboBoxQuickOther.Size = new System.Drawing.Size(242, 23);
            this.comboBoxQuickOther.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(316, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "その他のファイル(&O):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(316, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 15);
            this.label7.TabIndex = 3;
            this.label7.Text = "対象ファイル(&T):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(353, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(311, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "簡易モードでは、ファイルに1段階だけのフィルターを適用できます。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "実行するフィルター(&F):";
            // 
            // listBoxQuickFilter
            // 
            this.listBoxQuickFilter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxQuickFilter.FormattingEnabled = true;
            this.listBoxQuickFilter.ItemHeight = 16;
            this.listBoxQuickFilter.Location = new System.Drawing.Point(6, 21);
            this.listBoxQuickFilter.Name = "listBoxQuickFilter";
            this.listBoxQuickFilter.Size = new System.Drawing.Size(288, 84);
            this.listBoxQuickFilter.TabIndex = 1;
            // 
            // panelQuickComponent
            // 
            this.panelQuickComponent.BackColor = System.Drawing.SystemColors.Window;
            this.panelQuickComponent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelQuickComponent.Location = new System.Drawing.Point(6, 113);
            this.panelQuickComponent.Name = "panelQuickComponent";
            this.panelQuickComponent.Size = new System.Drawing.Size(656, 156);
            this.panelQuickComponent.TabIndex = 7;
            // 
            // tabPageDetail
            // 
            this.tabPageDetail.Controls.Add(this.comboBoxDetailOther);
            this.tabPageDetail.Controls.Add(this.label8);
            this.tabPageDetail.Controls.Add(this.buttonDetailSelectAll);
            this.tabPageDetail.Controls.Add(this.buttonDetailSelectClear);
            this.tabPageDetail.Controls.Add(this.buttonDetailDelete);
            this.tabPageDetail.Controls.Add(this.buttonDetailEdit);
            this.tabPageDetail.Controls.Add(this.buttonDetailAdd);
            this.tabPageDetail.Controls.Add(this.listBoxDetailFileType);
            this.tabPageDetail.Controls.Add(this.label6);
            this.tabPageDetail.Controls.Add(this.label9);
            this.tabPageDetail.Location = new System.Drawing.Point(4, 24);
            this.tabPageDetail.Name = "tabPageDetail";
            this.tabPageDetail.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDetail.Size = new System.Drawing.Size(668, 272);
            this.tabPageDetail.TabIndex = 0;
            this.tabPageDetail.Text = "詳細モード";
            this.tabPageDetail.UseVisualStyleBackColor = true;
            // 
            // comboBoxDetailOther
            // 
            this.comboBoxDetailOther.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDetailOther.FormattingEnabled = true;
            this.comboBoxDetailOther.Location = new System.Drawing.Point(115, 243);
            this.comboBoxDetailOther.Name = "comboBoxDetailOther";
            this.comboBoxDetailOther.Size = new System.Drawing.Size(199, 23);
            this.comboBoxDetailOther.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 246);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(106, 15);
            this.label8.TabIndex = 8;
            this.label8.Text = "その他のファイル(&O):";
            // 
            // buttonDetailSelectAll
            // 
            this.buttonDetailSelectAll.Location = new System.Drawing.Point(587, 180);
            this.buttonDetailSelectAll.Name = "buttonDetailSelectAll";
            this.buttonDetailSelectAll.Size = new System.Drawing.Size(75, 23);
            this.buttonDetailSelectAll.TabIndex = 6;
            this.buttonDetailSelectAll.Text = "全選択(&S)";
            this.buttonDetailSelectAll.UseVisualStyleBackColor = true;
            // 
            // buttonDetailSelectClear
            // 
            this.buttonDetailSelectClear.Location = new System.Drawing.Point(587, 209);
            this.buttonDetailSelectClear.Name = "buttonDetailSelectClear";
            this.buttonDetailSelectClear.Size = new System.Drawing.Size(75, 23);
            this.buttonDetailSelectClear.TabIndex = 7;
            this.buttonDetailSelectClear.Text = "全解除(&C)";
            this.buttonDetailSelectClear.UseVisualStyleBackColor = true;
            // 
            // buttonDetailDelete
            // 
            this.buttonDetailDelete.Location = new System.Drawing.Point(587, 82);
            this.buttonDetailDelete.Name = "buttonDetailDelete";
            this.buttonDetailDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDetailDelete.TabIndex = 5;
            this.buttonDetailDelete.Text = "削除(&D)";
            this.buttonDetailDelete.UseVisualStyleBackColor = true;
            // 
            // buttonDetailEdit
            // 
            this.buttonDetailEdit.Location = new System.Drawing.Point(587, 53);
            this.buttonDetailEdit.Name = "buttonDetailEdit";
            this.buttonDetailEdit.Size = new System.Drawing.Size(75, 23);
            this.buttonDetailEdit.TabIndex = 4;
            this.buttonDetailEdit.Text = "編集(&E)...";
            this.buttonDetailEdit.UseVisualStyleBackColor = true;
            // 
            // buttonDetailAdd
            // 
            this.buttonDetailAdd.Location = new System.Drawing.Point(587, 24);
            this.buttonDetailAdd.Name = "buttonDetailAdd";
            this.buttonDetailAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonDetailAdd.TabIndex = 3;
            this.buttonDetailAdd.Text = "追加(&A)...";
            this.buttonDetailAdd.UseVisualStyleBackColor = true;
            // 
            // listBoxDetailFileType
            // 
            this.listBoxDetailFileType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBoxDetailFileType.FormattingEnabled = true;
            this.listBoxDetailFileType.ItemHeight = 12;
            this.listBoxDetailFileType.Location = new System.Drawing.Point(6, 24);
            this.listBoxDetailFileType.Name = "listBoxDetailFileType";
            this.listBoxDetailFileType.Size = new System.Drawing.Size(575, 208);
            this.listBoxDetailFileType.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(334, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(329, 15);
            this.label6.TabIndex = 1;
            this.label6.Text = "詳細モードでは拡張子ごとに、設定済みのフィルター群を適用します。";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 4);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(139, 15);
            this.label9.TabIndex = 0;
            this.label9.Text = "使用するフィルター設定(&F):";
            // 
            // labelSrcFileCount
            // 
            this.labelSrcFileCount.AutoSize = true;
            this.labelSrcFileCount.Location = new System.Drawing.Point(484, 15);
            this.labelSrcFileCount.Name = "labelSrcFileCount";
            this.labelSrcFileCount.Size = new System.Drawing.Size(164, 15);
            this.labelSrcFileCount.TabIndex = 2;
            this.labelSrcFileCount.Text = "ファイル数:{0}   フォルダ数:{1}";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(530, 375);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(80, 23);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "転送開始(&S)";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(616, 375);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelFreeware
            // 
            this.labelFreeware.AutoSize = true;
            this.labelFreeware.Location = new System.Drawing.Point(13, 386);
            this.labelFreeware.Name = "labelFreeware";
            this.labelFreeware.Size = new System.Drawing.Size(0, 15);
            this.labelFreeware.TabIndex = 23;
            // 
            // FileFilterTransferDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(702, 412);
            this.Controls.Add(this.labelFreeware);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.labelSrcFileCount);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.textBoxDest);
            this.Controls.Add(this.textBoxSrc);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileFilterTransferDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "テキスト変換";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FileFilterTransferDialog_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabPageFixed.ResumeLayout(false);
            this.tabPageFixed.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageQuick.ResumeLayout(false);
            this.tabPageQuick.PerformLayout();
            this.tabPageDetail.ResumeLayout(false);
            this.tabPageDetail.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDest;
        private System.Windows.Forms.TextBox textBoxSrc;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageDetail;
        private System.Windows.Forms.Label labelSrcFileCount;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TabPage tabPageQuick;
        private System.Windows.Forms.ListBox listBoxQuickFilter;
        private System.Windows.Forms.Panel panelQuickComponent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonDetailDelete;
        private System.Windows.Forms.Button buttonDetailEdit;
        private System.Windows.Forms.Button buttonDetailAdd;
        private System.Windows.Forms.ListBox listBoxDetailFileType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBoxDetailOther;
        private System.Windows.Forms.ComboBox comboBoxQuickOther;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxQuickTargetExt;
        private System.Windows.Forms.Button buttonDetailSelectAll;
        private System.Windows.Forms.Button buttonDetailSelectClear;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelFreeware;
        private System.Windows.Forms.TabPage tabPageFixed;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioShiftJISToEUC;
        private System.Windows.Forms.RadioButton radioShiftJISToUTF8;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RadioButton radioDeleteEmptyLine;
        private System.Windows.Forms.RadioButton radioReturnLF;
        private System.Windows.Forms.RadioButton radioReturnCRLF;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton radioQuick4;
        private System.Windows.Forms.RadioButton radioQuick3;
        private System.Windows.Forms.RadioButton radioQuick2;
        private System.Windows.Forms.RadioButton radioQuick1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.RadioButton radioShiftJISSpace8Tab;
        private System.Windows.Forms.RadioButton radioShiftJIS8TabSpace;
        private System.Windows.Forms.RadioButton radioShiftJISSpace4Tab;
        private System.Windows.Forms.RadioButton radioShiftJIS4TabSpace;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.RadioButton radioEUCToShiftJIS;
        private System.Windows.Forms.RadioButton radioUTF8ToShiftJIS;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.RadioButton radioUTF8Space8Tab;
        private System.Windows.Forms.RadioButton radioUTF8Space4Tab;
        private System.Windows.Forms.RadioButton radioUTF88TabSpace;
        private System.Windows.Forms.RadioButton radioUTF84TabSpace;
        private System.Windows.Forms.ComboBox comboBoxDefinedTargetExt;
        private System.Windows.Forms.ComboBox comboBoxDefinedOther;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.RadioButton radioShellFilerDump;
    }
}