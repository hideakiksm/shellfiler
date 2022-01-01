using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.FileTask.ArgumentConverter;
using ShellFiler.UI.Dialog.KeyOption;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：コマンドライン要素のキー入力ダイアログ
    //=========================================================================================
    public partial class ShellArgumentInputDialog : Form {
        // 入力領域のY幅
        private const int CY_TEXTBOX_STEP = 24;

        // コマンド
        private string m_command;

        // テキストボックスのリスト
        private List<TextBox> m_textBoxList = new List<TextBox>();

        // コマンド引数の解析結果
        private ShellCommandArgument m_argument;

        // ヘルプウィンドウ
        private CommandArgumentHelp m_commandHelp;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]command   コマンド
        // 　　　　[in]argument  コマンド引数の解析結果
        // 戻り値：なし
        //=========================================================================================
        public ShellArgumentInputDialog(string command, ShellCommandArgument argument) {
            m_command = command;
            m_argument = argument;
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：フォームが読み込まれたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ShellArgumentInputDialog_Load(object sender, EventArgs e) {
            // 入力領域を作成
            int tabIndex = 0;
            List<string> inputMessageList = m_argument.KeyInputMessage;
            for (int i = 0; i < inputMessageList.Count; i++) {
                Label labelMessage = new Label();
                TextBox textBoxParam = new TextBox();
                labelMessage.AutoSize = true;
                labelMessage.Location = new Point(33, 28 + i * CY_TEXTBOX_STEP);
                labelMessage.Size = new Size(74, 12);
                labelMessage.TabIndex = tabIndex++;
                labelMessage.Text = inputMessageList[i] + "(&" + (i + 1) + "):";
                textBoxParam.Location = new Point(160, 25 + i * CY_TEXTBOX_STEP);
                textBoxParam.Size = new Size(270, 19);
                textBoxParam.TabIndex = tabIndex++;
                textBoxParam.TextChanged += new EventHandler(this.TextBoxParam_TextChanged);
                m_textBoxList.Add(textBoxParam);
                this.Controls.Add(labelMessage);
                this.Controls.Add(textBoxParam);
            }

            // 位置を調整
            int yExpand = (inputMessageList.Count - 1) * CY_TEXTBOX_STEP;
            this.label1.TabIndex = tabIndex++;
            this.label1.Location = new Point(this.label1.Location.X, this.label1.Location.Y + yExpand);
            this.textBoxCommand.TabIndex = tabIndex++;
            this.textBoxCommand.Location = new Point(this.textBoxCommand.Location.X, this.textBoxCommand.Location.Y + yExpand);
            this.buttonOk.TabIndex = tabIndex++;
            this.buttonOk.Location = new Point(this.buttonOk.Location.X, this.buttonOk.Location.Y + yExpand);
            this.buttonCancel.TabIndex = tabIndex++;
            this.buttonCancel.Location = new Point(this.buttonCancel.Location.X, this.buttonCancel.Location.Y + yExpand);
            this.Size = new Size(this.Size.Width, this.Size.Height + yExpand);

            TextBoxParam_TextChanged(null, null);

            // ヘルプ
            m_commandHelp = new CommandArgumentHelp();
            m_commandHelp.AdjustLocation(this);
            m_commandHelp.Show(this);
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ShellArgumentInputDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            m_commandHelp.Dispose();
        }

        //=========================================================================================
        // 機　能：パラメータのテキストが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TextBoxParam_TextChanged(object sender, EventArgs evt) {
            List<string> keyInputList = new List<string>();
            for (int i = 0; i < m_textBoxList.Count; i++) {
                keyInputList.Add(m_textBoxList[i].Text);
            }
            string displayCommand = m_command + " " + m_argument.GetDisplayCommand(keyInputList);
            this.textBoxCommand.Text = displayCommand;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            string command = this.textBoxCommand.Text;
            if (command.IndexOf("$<") != -1) {
                InfoBox.Warning(this, Resources.DlgShellCommand_CommandCannotUseInput);
                return;
            }
            
            ShellCommandArgument commandArgument = new ShellCommandArgument(command, m_argument.Target, m_argument.Opposite, m_argument.TargetUseFile, m_argument.OppositeUseFile);
            bool success = commandArgument.ParseArgument();
            if (!success) {
                InfoBox.Warning(this, "{0}", commandArgument.ErrorMessage);
                return;
            }
            m_argument = commandArgument;

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：キャンセルボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCancel_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        //=========================================================================================
        // プロパティ：キー入力した結果
        //=========================================================================================
        public ShellCommandArgument InputResult {
            get {
                return m_argument;
            }
        }
    }
}
