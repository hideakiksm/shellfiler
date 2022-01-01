using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // SSH＞全般 の設定ページ
    //=========================================================================================
    public partial class SSHTerminalPage : UserControl, IOptionDialogPage {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public SSHTerminalPage(OptionSettingDialog parent) {
            InitializeComponent();

            this.labelLineCount.Text = string.Format(this.labelLineCount.Text, Configuration.MIN_TERMINAL_LOG_LINE_COUNT, Configuration.MAX_TERMINAL_LOG_LINE_COUNT);
            this.numericLineCount.Minimum = Configuration.MIN_TERMINAL_LOG_LINE_COUNT;
            this.numericLineCount.Maximum = Configuration.MAX_TERMINAL_LOG_LINE_COUNT;
            
            // コンフィグ値をUIに反映
            SetInitialValue(Configuration.Current);
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFormClosed() {
        }

        //=========================================================================================
        // 機　能：UIに初期値を設定する
        // 引　数：[in]config  取得対象のコンフィグ
        // 戻り値：なし
        //=========================================================================================
        private void SetInitialValue(Configuration config) {
            this.numericLineCount.Value = config.TerminalLogLineCount;
            if (config.TerminalShellCommandSSH) {
                this.radioShellSSH.Checked = true;
            } else {
                this.radioShellWin.Checked = true;
            }
            if (config.TerminalCloseConfirmMode == TerminalCloseConfirmMode.ShellClose) {
                this.radioCloseClose.Checked = true;
            } else if (config.TerminalCloseConfirmMode == TerminalCloseConfirmMode.KeepChannelConfirm) {
                this.radioCloseStayMsg.Checked = true;
            } else {
                this.radioCloseStaySilent.Checked = true;
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            Configuration.Current.TerminalLogLineCount = (int)(this.numericLineCount.Value);
            if (this.radioShellSSH.Checked) {
                Configuration.Current.TerminalShellCommandSSH = true;
            } else {
                Configuration.Current.TerminalShellCommandSSH = false;
            }
            if (this.radioCloseClose.Checked) {
                Configuration.Current.TerminalCloseConfirmMode = TerminalCloseConfirmMode.ShellClose;
            } else if (this.radioCloseStayMsg.Checked) {
                Configuration.Current.TerminalCloseConfirmMode = TerminalCloseConfirmMode.KeepChannelConfirm;
            } else {
                Configuration.Current.TerminalCloseConfirmMode = TerminalCloseConfirmMode.KeepChannelSilent;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ページ内の設定をデフォルトに戻す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetDefault() {
            Configuration org = new Configuration();
            SetInitialValue(org);
        }
    }
}