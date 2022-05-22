namespace ShellFiler.UI.Dialog.FileViewer {
    partial class HttpViewerRequestDialog {
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageHTTP = new System.Windows.Forms.TabPage();
            this.checkBoxAutoContent = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.ColumnCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnHeaderName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnHeaderValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelHTTPBody = new System.Windows.Forms.Panel();
            this.comboBoxHTTPMethod = new System.Windows.Forms.ComboBox();
            this.radioButtonHTTPBodyFile = new System.Windows.Forms.RadioButton();
            this.radioButtonHTTPBodyText = new System.Windows.Forms.RadioButton();
            this.buttonHTTPHeaderDelete = new System.Windows.Forms.Button();
            this.buttonHTTPHeaderAdd = new System.Windows.Forms.Button();
            this.buttonHTTPParamEdit = new System.Windows.Forms.Button();
            this.textBoxHTTPProxyUrl = new System.Windows.Forms.TextBox();
            this.textBoxHTTPUrl = new System.Windows.Forms.TextBox();
            this.checkBoxHTTPProxyUrl = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageTCP = new System.Windows.Forms.TabPage();
            this.numericPortNum = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.radioButtonTCPReqFile = new System.Windows.Forms.RadioButton();
            this.radioButtonTCPReqText = new System.Windows.Forms.RadioButton();
            this.panelTCPRequest = new System.Windows.Forms.Panel();
            this.textBoxTCPServer = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabPageHTTP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.tabPageTCP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPortNum)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageHTTP);
            this.tabControl.Controls.Add(this.tabPageTCP);
            this.tabControl.Location = new System.Drawing.Point(13, 13);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(755, 551);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageHTTP
            // 
            this.tabPageHTTP.Controls.Add(this.checkBoxAutoContent);
            this.tabPageHTTP.Controls.Add(this.label16);
            this.tabPageHTTP.Controls.Add(this.dataGridView);
            this.tabPageHTTP.Controls.Add(this.panelHTTPBody);
            this.tabPageHTTP.Controls.Add(this.comboBoxHTTPMethod);
            this.tabPageHTTP.Controls.Add(this.radioButtonHTTPBodyFile);
            this.tabPageHTTP.Controls.Add(this.radioButtonHTTPBodyText);
            this.tabPageHTTP.Controls.Add(this.buttonHTTPHeaderDelete);
            this.tabPageHTTP.Controls.Add(this.buttonHTTPHeaderAdd);
            this.tabPageHTTP.Controls.Add(this.buttonHTTPParamEdit);
            this.tabPageHTTP.Controls.Add(this.textBoxHTTPProxyUrl);
            this.tabPageHTTP.Controls.Add(this.textBoxHTTPUrl);
            this.tabPageHTTP.Controls.Add(this.checkBoxHTTPProxyUrl);
            this.tabPageHTTP.Controls.Add(this.label4);
            this.tabPageHTTP.Controls.Add(this.label2);
            this.tabPageHTTP.Controls.Add(this.label6);
            this.tabPageHTTP.Controls.Add(this.label7);
            this.tabPageHTTP.Controls.Add(this.label3);
            this.tabPageHTTP.Controls.Add(this.label13);
            this.tabPageHTTP.Controls.Add(this.label12);
            this.tabPageHTTP.Controls.Add(this.label5);
            this.tabPageHTTP.Controls.Add(this.label1);
            this.tabPageHTTP.Location = new System.Drawing.Point(4, 24);
            this.tabPageHTTP.Name = "tabPageHTTP";
            this.tabPageHTTP.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHTTP.Size = new System.Drawing.Size(747, 523);
            this.tabPageHTTP.TabIndex = 0;
            this.tabPageHTTP.Text = "HTTPモード";
            this.tabPageHTTP.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoContent
            // 
            this.checkBoxAutoContent.AutoSize = true;
            this.checkBoxAutoContent.Location = new System.Drawing.Point(304, 102);
            this.checkBoxAutoContent.Name = "checkBoxAutoContent";
            this.checkBoxAutoContent.Size = new System.Drawing.Size(307, 19);
            this.checkBoxAutoContent.TabIndex = 9;
            this.checkBoxAutoContent.Text = "GET/HEAD/POSTでContent-Typeを自動設定する(&T)";
            this.checkBoxAutoContent.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(203, 506);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(402, 15);
            this.label16.TabIndex = 21;
            this.label16.Text = "ヘッダとボディの区切りとなる「CR LF CR LF」の次の位置からのデータを指定します。";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnCheck,
            this.ColumnHeaderName,
            this.ColumnHeaderValue});
            this.dataGridView.Location = new System.Drawing.Point(134, 125);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowHeadersWidth = 4;
            this.dataGridView.RowTemplate.Height = 21;
            this.dataGridView.Size = new System.Drawing.Size(470, 152);
            this.dataGridView.TabIndex = 11;
            // 
            // ColumnCheck
            // 
            this.ColumnCheck.HeaderText = "";
            this.ColumnCheck.Name = "ColumnCheck";
            this.ColumnCheck.Width = 20;
            // 
            // ColumnHeaderName
            // 
            this.ColumnHeaderName.HeaderText = "ヘッダ名";
            this.ColumnHeaderName.Name = "ColumnHeaderName";
            this.ColumnHeaderName.Width = 150;
            // 
            // ColumnHeaderValue
            // 
            this.ColumnHeaderValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnHeaderValue.HeaderText = "値";
            this.ColumnHeaderValue.Name = "ColumnHeaderValue";
            // 
            // panelHTTPBody
            // 
            this.panelHTTPBody.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelHTTPBody.Location = new System.Drawing.Point(203, 288);
            this.panelHTTPBody.Name = "panelHTTPBody";
            this.panelHTTPBody.Size = new System.Drawing.Size(538, 206);
            this.panelHTTPBody.TabIndex = 20;
            // 
            // comboBoxHTTPMethod
            // 
            this.comboBoxHTTPMethod.FormattingEnabled = true;
            this.comboBoxHTTPMethod.Location = new System.Drawing.Point(134, 99);
            this.comboBoxHTTPMethod.Name = "comboBoxHTTPMethod";
            this.comboBoxHTTPMethod.Size = new System.Drawing.Size(164, 23);
            this.comboBoxHTTPMethod.TabIndex = 8;
            // 
            // radioButtonHTTPBodyFile
            // 
            this.radioButtonHTTPBodyFile.AutoSize = true;
            this.radioButtonHTTPBodyFile.Location = new System.Drawing.Point(114, 310);
            this.radioButtonHTTPBodyFile.Name = "radioButtonHTTPBodyFile";
            this.radioButtonHTTPBodyFile.Size = new System.Drawing.Size(83, 19);
            this.radioButtonHTTPBodyFile.TabIndex = 19;
            this.radioButtonHTTPBodyFile.TabStop = true;
            this.radioButtonHTTPBodyFile.Text = "ファイル参照";
            this.radioButtonHTTPBodyFile.UseVisualStyleBackColor = true;
            // 
            // radioButtonHTTPBodyText
            // 
            this.radioButtonHTTPBodyText.AutoSize = true;
            this.radioButtonHTTPBodyText.Location = new System.Drawing.Point(114, 288);
            this.radioButtonHTTPBodyText.Name = "radioButtonHTTPBodyText";
            this.radioButtonHTTPBodyText.Size = new System.Drawing.Size(85, 19);
            this.radioButtonHTTPBodyText.TabIndex = 18;
            this.radioButtonHTTPBodyText.TabStop = true;
            this.radioButtonHTTPBodyText.Text = "テキスト入力";
            this.radioButtonHTTPBodyText.UseVisualStyleBackColor = true;
            // 
            // buttonHTTPHeaderDelete
            // 
            this.buttonHTTPHeaderDelete.Location = new System.Drawing.Point(611, 157);
            this.buttonHTTPHeaderDelete.Name = "buttonHTTPHeaderDelete";
            this.buttonHTTPHeaderDelete.Size = new System.Drawing.Size(75, 26);
            this.buttonHTTPHeaderDelete.TabIndex = 13;
            this.buttonHTTPHeaderDelete.Text = "削除(&D)";
            this.buttonHTTPHeaderDelete.UseVisualStyleBackColor = true;
            // 
            // buttonHTTPHeaderAdd
            // 
            this.buttonHTTPHeaderAdd.Location = new System.Drawing.Point(611, 125);
            this.buttonHTTPHeaderAdd.Name = "buttonHTTPHeaderAdd";
            this.buttonHTTPHeaderAdd.Size = new System.Drawing.Size(75, 26);
            this.buttonHTTPHeaderAdd.TabIndex = 12;
            this.buttonHTTPHeaderAdd.Text = "追加(&A)";
            this.buttonHTTPHeaderAdd.UseVisualStyleBackColor = true;
            // 
            // buttonHTTPParamEdit
            // 
            this.buttonHTTPParamEdit.Location = new System.Drawing.Point(611, 48);
            this.buttonHTTPParamEdit.Name = "buttonHTTPParamEdit";
            this.buttonHTTPParamEdit.Size = new System.Drawing.Size(75, 23);
            this.buttonHTTPParamEdit.TabIndex = 4;
            this.buttonHTTPParamEdit.Text = "編集(&E)...";
            this.buttonHTTPParamEdit.UseVisualStyleBackColor = true;
            // 
            // textBoxHTTPProxyUrl
            // 
            this.textBoxHTTPProxyUrl.Location = new System.Drawing.Point(134, 74);
            this.textBoxHTTPProxyUrl.Name = "textBoxHTTPProxyUrl";
            this.textBoxHTTPProxyUrl.Size = new System.Drawing.Size(296, 23);
            this.textBoxHTTPProxyUrl.TabIndex = 6;
            // 
            // textBoxHTTPUrl
            // 
            this.textBoxHTTPUrl.Location = new System.Drawing.Point(134, 49);
            this.textBoxHTTPUrl.Name = "textBoxHTTPUrl";
            this.textBoxHTTPUrl.Size = new System.Drawing.Size(471, 23);
            this.textBoxHTTPUrl.TabIndex = 3;
            // 
            // checkBoxHTTPProxyUrl
            // 
            this.checkBoxHTTPProxyUrl.AutoSize = true;
            this.checkBoxHTTPProxyUrl.Location = new System.Drawing.Point(9, 77);
            this.checkBoxHTTPProxyUrl.Name = "checkBoxHTTPProxyUrl";
            this.checkBoxHTTPProxyUrl.Size = new System.Drawing.Size(106, 19);
            this.checkBoxHTTPProxyUrl.TabIndex = 5;
            this.checkBoxHTTPProxyUrl.Text = "プロキシURL(&X)";
            this.checkBoxHTTPProxyUrl.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(394, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "HTTPサーバーへの接続テストを行い、応答電文をファイルビューアで確認できます。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(506, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "HTTPモードでは、指定されたパラメータを組み合わせてHTTPリクエストを自動生成し、サーバーに送ります。";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 106);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 15);
            this.label6.TabIndex = 7;
            this.label6.Text = "メソッド(&M):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 288);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 15);
            this.label7.TabIndex = 17;
            this.label7.Text = "HTTPボディ(&B):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(610, 234);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "太字のヘッダは一般的に";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(610, 248);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(133, 15);
            this.label13.TabIndex = 15;
            this.label13.Text = "使用される(独自ではない)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(610, 262);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 15);
            this.label12.TabIndex = 16;
            this.label12.Text = "HTTPヘッダです。";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "HTTPヘッダ(&H):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "URL+?+パラメータ(&R):";
            // 
            // tabPageTCP
            // 
            this.tabPageTCP.Controls.Add(this.numericPortNum);
            this.tabPageTCP.Controls.Add(this.label14);
            this.tabPageTCP.Controls.Add(this.label9);
            this.tabPageTCP.Controls.Add(this.radioButtonTCPReqFile);
            this.tabPageTCP.Controls.Add(this.radioButtonTCPReqText);
            this.tabPageTCP.Controls.Add(this.panelTCPRequest);
            this.tabPageTCP.Controls.Add(this.textBoxTCPServer);
            this.tabPageTCP.Controls.Add(this.label15);
            this.tabPageTCP.Controls.Add(this.label11);
            this.tabPageTCP.Controls.Add(this.label10);
            this.tabPageTCP.Controls.Add(this.label8);
            this.tabPageTCP.Location = new System.Drawing.Point(4, 24);
            this.tabPageTCP.Name = "tabPageTCP";
            this.tabPageTCP.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTCP.Size = new System.Drawing.Size(747, 523);
            this.tabPageTCP.TabIndex = 1;
            this.tabPageTCP.Text = "TCPモード";
            this.tabPageTCP.UseVisualStyleBackColor = true;
            // 
            // numericPortNum
            // 
            this.numericPortNum.Location = new System.Drawing.Point(114, 75);
            this.numericPortNum.Name = "numericPortNum";
            this.numericPortNum.Size = new System.Drawing.Size(120, 23);
            this.numericPortNum.TabIndex = 5;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(201, 321);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(333, 15);
            this.label14.TabIndex = 10;
            this.label14.Text = "HTTPプロトコルを使用する場合はヘッダとボディの両方を指定します。";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(394, 15);
            this.label9.TabIndex = 0;
            this.label9.Text = "HTTPサーバーへの接続テストを行い、応答電文をファイルビューアで確認できます。";
            // 
            // radioButtonTCPReqFile
            // 
            this.radioButtonTCPReqFile.AutoSize = true;
            this.radioButtonTCPReqFile.Location = new System.Drawing.Point(114, 122);
            this.radioButtonTCPReqFile.Name = "radioButtonTCPReqFile";
            this.radioButtonTCPReqFile.Size = new System.Drawing.Size(83, 19);
            this.radioButtonTCPReqFile.TabIndex = 8;
            this.radioButtonTCPReqFile.TabStop = true;
            this.radioButtonTCPReqFile.Text = "ファイル参照";
            this.radioButtonTCPReqFile.UseVisualStyleBackColor = true;
            // 
            // radioButtonTCPReqText
            // 
            this.radioButtonTCPReqText.AutoSize = true;
            this.radioButtonTCPReqText.Location = new System.Drawing.Point(114, 100);
            this.radioButtonTCPReqText.Name = "radioButtonTCPReqText";
            this.radioButtonTCPReqText.Size = new System.Drawing.Size(85, 19);
            this.radioButtonTCPReqText.TabIndex = 7;
            this.radioButtonTCPReqText.TabStop = true;
            this.radioButtonTCPReqText.Text = "テキスト入力";
            this.radioButtonTCPReqText.UseVisualStyleBackColor = true;
            // 
            // panelTCPRequest
            // 
            this.panelTCPRequest.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelTCPRequest.Location = new System.Drawing.Point(203, 102);
            this.panelTCPRequest.Name = "panelTCPRequest";
            this.panelTCPRequest.Size = new System.Drawing.Size(538, 216);
            this.panelTCPRequest.TabIndex = 9;
            // 
            // textBoxTCPServer
            // 
            this.textBoxTCPServer.Location = new System.Drawing.Point(114, 49);
            this.textBoxTCPServer.Name = "textBoxTCPServer";
            this.textBoxTCPServer.Size = new System.Drawing.Size(240, 23);
            this.textBoxTCPServer.TabIndex = 3;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 77);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(81, 15);
            this.label15.TabIndex = 4;
            this.label15.Text = "ポート番号(&P):";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 102);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(91, 15);
            this.label11.TabIndex = 6;
            this.label11.Text = "リクエスト電文(&Q)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 52);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 15);
            this.label10.TabIndex = 2;
            this.label10.Text = "サーバー名(&S):";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(582, 15);
            this.label8.TabIndex = 1;
            this.label8.Text = "TCPモードでは、ヘッダとボディのリクエスト電文そのものを直接指定することで、より低レベルでのプロトコルの検証を行えます。";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(693, 570);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(612, 570);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "接続";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // HttpViewerRequestDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(780, 605);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HttpViewerRequestDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "HTTPレスポンスビューア";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HttpViewerRequestDialog_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabPageHTTP.ResumeLayout(false);
            this.tabPageHTTP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.tabPageTCP.ResumeLayout(false);
            this.tabPageTCP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPortNum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageHTTP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPageTCP;
        private System.Windows.Forms.TextBox textBoxHTTPProxyUrl;
        private System.Windows.Forms.TextBox textBoxHTTPUrl;
        private System.Windows.Forms.CheckBox checkBoxHTTPProxyUrl;
        private System.Windows.Forms.Button buttonHTTPParamEdit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxHTTPMethod;
        private System.Windows.Forms.RadioButton radioButtonHTTPBodyFile;
        private System.Windows.Forms.RadioButton radioButtonHTTPBodyText;
        private System.Windows.Forms.Button buttonHTTPHeaderDelete;
        private System.Windows.Forms.Button buttonHTTPHeaderAdd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panelHTTPBody;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.RadioButton radioButtonTCPReqFile;
        private System.Windows.Forms.RadioButton radioButtonTCPReqText;
        private System.Windows.Forms.Panel panelTCPRequest;
        private System.Windows.Forms.TextBox textBoxTCPServer;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnHeaderName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnHeaderValue;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numericPortNum;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxAutoContent;

    }
}