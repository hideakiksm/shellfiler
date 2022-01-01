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
    // ファンクションキー＞全般 の設定ページ
    //=========================================================================================
    public partial class FuncGeneralPage : UserControl, IOptionDialogPage {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FuncGeneralPage(OptionSettingDialog parent) {
            InitializeComponent();
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
            int split = config.FunctionBarSplitCount;
            switch (split) {
                case 0:
                    this.radioButtonSplit0.Checked = true;
                    break;
                case 4:
                    this.radioButtonSplit4.Checked = true;
                    break;
                case 5:
                    this.radioButtonSplit5.Checked = true;
                    break;
            }
            this.checkBoxOverray.Checked = config.FunctionBarUseOverrayIcon;
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            int split = 0;
            if (this.radioButtonSplit0.Checked) {
                split = 0;
            } else if (this.radioButtonSplit4.Checked) {
                split = 4;
            } else {
                split = 5;
            }
            Configuration.Current.FunctionBarSplitCount = split;
            Configuration.Current.FunctionBarUseOverrayIcon = this.checkBoxOverray.Checked;
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
