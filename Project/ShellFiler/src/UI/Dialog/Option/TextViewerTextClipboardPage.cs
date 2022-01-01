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
using ShellFiler.FileViewer;
using ShellFiler.UI.Dialog.FileViewer;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイルビューア＞クリップボード(テキスト) の設定ページ
    //=========================================================================================
    public partial class TextViewerTextClipboardPage : UserControl, IOptionDialogPage {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public TextViewerTextClipboardPage(OptionSettingDialog parent) {
            InitializeComponent();
            string[] returnItems = {
                Resources.OptionTextViewClipT_ReturnOrg,            // 0:元のまま
                Resources.OptionTextViewClipT_ReturnCr,             // 1:CRだけ
                Resources.OptionTextViewClipT_ReturnCrLf,           // 2:CR+LF
            };
            this.comboBoxReturn.Items.AddRange(returnItems);
            string[] tabItems = {
                Resources.OptionTextViewClipT_TabOrg,               // TABコードのまま
                Resources.OptionTextViewClipT_TabSpace,             // 空白に変換
            };
            this.comboBoxTab.Items.AddRange(tabItems);

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
            // 参照元の設定を決める
            TextClipboardSetting setting;
            if (config.TextClipboardSettingDefault == null) {
                setting = new TextClipboardSetting();
                this.radioButtonPrev.Checked = true;
            } else {
                setting = (TextClipboardSetting)(config.TextClipboardSettingDefault.Clone());
                this.radioButtonFix.Checked = true;
            }

            switch (setting.LineBreakMode) {
                case TextClipboardSetting.CopyLineBreakMode.Original:
                    this.comboBoxReturn.SelectedIndex = 0;
                    break;
                case TextClipboardSetting.CopyLineBreakMode.Cr:
                    this.comboBoxReturn.SelectedIndex = 1;
                    break;
                case TextClipboardSetting.CopyLineBreakMode.CrLf:
                    this.comboBoxReturn.SelectedIndex = 2;
                    break;
            }
            switch (setting.TabMode) {
                case TextClipboardSetting.CopyTabMode.Original:
                    this.comboBoxTab.SelectedIndex = 0;
                    break;
                case TextClipboardSetting.CopyTabMode.ConvertSpace:
                    this.comboBoxTab.SelectedIndex = 1;
                    break;
            }
        }

        //=========================================================================================
        // 機　能：前回値か固定値かを選択するラジオボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonPrevFix_CheckedChanged(object sender, EventArgs evt) {
            if (this.radioButtonPrev.Checked) {
                this.comboBoxReturn.Enabled = false;
                this.comboBoxTab.Enabled = false;
            } else {
                this.comboBoxReturn.Enabled = true;
                this.comboBoxTab.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            TextClipboardSetting setting;
            if (this.radioButtonPrev.Checked) {
                setting = null;
            } else {
                setting = new TextClipboardSetting();
                switch (this.comboBoxReturn.SelectedIndex) {
                    case 0:
                        setting.LineBreakMode = TextClipboardSetting.CopyLineBreakMode.Original;
                        break;
                    case 1:
                        setting.LineBreakMode = TextClipboardSetting.CopyLineBreakMode.Cr;
                        break;
                    case 2:
                        setting.LineBreakMode = TextClipboardSetting.CopyLineBreakMode.CrLf;
                        break;
                }
                switch (this.comboBoxTab.SelectedIndex) {
                    case 0:
                        setting.TabMode = TextClipboardSetting.CopyTabMode.Original;
                        break;
                    case 1:
                        setting.TabMode = TextClipboardSetting.CopyTabMode.ConvertSpace;
                        break;
                }
            }

            Configuration.Current.TextClipboardSettingDefault = setting;

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