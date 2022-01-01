using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;
using ShellFiler.FileViewer;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイルビューア＞表示 の設定ページ
    //=========================================================================================
    public partial class TextViewerSearchPage : UserControl, IOptionDialogPage {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public TextViewerSearchPage(OptionSettingDialog parent) {
            InitializeComponent();

            string[] optionItemList = {
                Resources.DlgViewerSearch_Option1,
                Resources.DlgViewerSearch_Option2,
                Resources.DlgViewerSearch_Option3,
                Resources.DlgViewerSearch_Option4,
                Resources.DlgViewerSearch_Option5,
            };
            this.comboBoxOption.Items.AddRange(optionItemList);

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
            TextSearchOption option;
            if (config.TextViewerSearchOptionDefault == null) {
                option = new TextSearchOption();
                this.radioButtonPrev.Checked = true;
            } else {
                option = (TextSearchOption)(config.TextViewerSearchOptionDefault.Clone());
                this.radioButtonFix.Checked = true;
            }

            switch (option.SearchMode) {
                case TextSearchMode.NormalIgnoreCase:
                    this.comboBoxOption.SelectedIndex = 0;
                    break;
                case TextSearchMode.NormalCaseSensitive:
                    this.comboBoxOption.SelectedIndex = 1;
                    break;
                case TextSearchMode.WildCardIgnoreCase:
                    this.comboBoxOption.SelectedIndex = 2;
                    break;
                case TextSearchMode.WildCardCaseSensitive:
                    this.comboBoxOption.SelectedIndex = 3;
                    break;
                case TextSearchMode.RegularExpression:
                    this.comboBoxOption.SelectedIndex = 4;
                    break;
            }
            this.checkBoxWord.Checked = option.SearchWord;
        }
        
        //=========================================================================================
        // 機　能：前回値か固定値かを選択するラジオボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonPrevFix_CheckedChanged(object sender, EventArgs evt) {
            if (radioButtonPrev.Checked) {
                this.comboBoxOption.Enabled = false;
                this.checkBoxWord.Enabled = false;
            } else {
                this.comboBoxOption.Enabled = true;
                this.checkBoxWord.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            TextSearchOption option = new TextSearchOption();
            if (this.radioButtonPrev.Checked) {
                option = null;
            } else {
                switch (this.comboBoxOption.SelectedIndex) {
                    case 0:
                        option.SearchMode = TextSearchMode.NormalIgnoreCase;
                        break;
                    case 1:
                        option.SearchMode = TextSearchMode.NormalCaseSensitive;
                        break;
                    case 2:
                        option.SearchMode = TextSearchMode.WildCardIgnoreCase;
                        break;
                    case 3:
                        option.SearchMode = TextSearchMode.WildCardCaseSensitive;
                        break;
                    case 4:
                        option.SearchMode = TextSearchMode.RegularExpression;
                        break;
                }
                option.SearchWord = this.checkBoxWord.Checked;
            }
            Configuration.Current.TextViewerSearchOptionDefault = option;
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