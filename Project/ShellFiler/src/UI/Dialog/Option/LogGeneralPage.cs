using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ログ＞全般 の設定ページ
    //=========================================================================================
    public partial class LogGeneralPage : UserControl, IOptionDialogPage {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public LogGeneralPage(OptionSettingDialog parent) {
            InitializeComponent();
            
            this.labelLineCount.Text = string.Format(this.labelLineCount.Text, Configuration.MIN_LOG_LINE_MAX_COUNT, Configuration.MAX_LOG_LINE_MAX_COUNT);
            this.numericLineCount.Minimum = Configuration.MIN_LOG_LINE_MAX_COUNT;
            this.numericLineCount.Maximum = Configuration.MAX_LOG_LINE_MAX_COUNT;
            
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
            this.numericLineCount.Value = config.LogLineMaxCountDefault;
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            Configuration.Current.LogLineMaxCountDefault = (int)(this.numericLineCount.Value);
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
