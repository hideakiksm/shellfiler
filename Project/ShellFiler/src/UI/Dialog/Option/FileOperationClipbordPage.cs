using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Api;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル操作＞クリップボード の設定ページ
    //=========================================================================================
    public partial class FileOperationClipboardPage : UserControl, IOptionDialogPage {
        // UIの実装
        private ClipboardCopyNameAsDialog.Impl m_impl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileOperationClipboardPage(OptionSettingDialog parent) {
            InitializeComponent();
            List<string> fileList = new List<string>();
            fileList.Add("ReadMe.doc");
            fileList.Add("My Memo.txt");
            fileList.Add("UserList.xls");
            m_impl = new ClipboardCopyNameAsDialog.Impl(
                            @"C:\User\MyAccount", fileList, this.textBoxSample,
                            this.radioButtonSeparatorSpace, this.radioButtonSeparatorTab, this.radioButtonSeparatorComma, this.radioButtonSeparatorReturn,
                            this.radioButtonQuoteAlways, this.radioButtonQuoteSpace, this.radioButtonQuoteNone, this.checkBoxFullPath);

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
            if (config.ClipboardCopyNameAsSettingDefault == null) {
                this.radioButtonPrev.Checked = true;
                m_impl.Initialize(new ClipboardCopyNameAsSetting());
            } else {
                this.radioButtonFix.Checked = true;
                m_impl.Initialize(config.ClipboardCopyNameAsSettingDefault);
            }
            RadioButtonPrevFix_CheckedChanged(null, null);
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            ClipboardCopyNameAsSetting setting;
            if (this.radioButtonPrev.Checked) {
                setting = null;
            } else {
                setting = m_impl.Setting;
            }
            Configuration.Current.ClipboardCopyNameAsSettingDefault = setting;

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

        //=========================================================================================
        // 機　能：前回値使用か固定かのラジオボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonPrevFix_CheckedChanged(object sender, EventArgs evt) {
            bool enabled;
            if (this.radioButtonPrev.Checked) {
                enabled = false;
            } else {
                enabled = true;
            }
            this.textBoxSample.Enabled = enabled;
            this.radioButtonSeparatorSpace.Enabled = enabled;
            this.radioButtonSeparatorTab.Enabled = enabled;
            this.radioButtonSeparatorComma.Enabled = enabled;
            this.radioButtonSeparatorReturn.Enabled = enabled;
            this.radioButtonQuoteAlways.Enabled = enabled;
            this.radioButtonQuoteSpace.Enabled = enabled;
            this.radioButtonQuoteNone.Enabled = enabled;
            this.checkBoxFullPath.Enabled = enabled;
        }

        //=========================================================================================
        // 機　能：ラジオボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonSetting_CheckedChanged(object sender, EventArgs evt) {
            m_impl.OnUiChanged();
        }

        //=========================================================================================
        // 機　能：チェックボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void CheckBoxSetting_CheckedChanged(object sender, EventArgs evt) {
            m_impl.OnUiChanged();
        }
    }
}
