using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：正規表現テストダイアログ
    //=========================================================================================
    public partial class RegexTestDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public RegexTestDialog() {
            InitializeComponent();

            this.webBrowser.DocumentText = Resources.HtmlTransferConditionFileName;
            this.textBoxRegex.Text = "^[0-9]*$";
            this.textBoxTarget.Text = "12345";
        }

        //=========================================================================================
        // 機　能：テキストが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBox_TextChanged(object sender, EventArgs evt) {
            Regex regex;
            try {
                regex = new Regex(this.textBoxRegex.Text);
            } catch (Exception) {
                this.labelMessage.Text = Resources.DlgRegexTest_RegexError;
                return;
            }
            if (regex.IsMatch(this.textBoxTarget.Text)) {
                this.labelMessage.Text = Resources.DlgRegexTest_RegexMatch;
            } else {
                this.labelMessage.Text = "";
            }
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RegexTestDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            this.webBrowser.Dispose();
        }

        //=========================================================================================
        // 機　能：閉じるボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonClose_Click(object sender, EventArgs evt) {
            Close();
        }
    }
}
