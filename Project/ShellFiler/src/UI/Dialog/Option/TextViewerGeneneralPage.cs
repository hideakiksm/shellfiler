using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイルビューア＞全般 の設定ページ
    //=========================================================================================
    public partial class TextViewerGeneralPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public TextViewerGeneralPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;
            this.numericMaxSize.Minimum = Configuration.MIN_TEXT_VIEWER_MAX_FILE_SIZE / 1024 / 1024;
            this.numericMaxSize.Maximum = Configuration.MAX_TEXT_VIEWER_MAX_FILE_SIZE / 1024 / 1024;
            this.labelMaxSize.Text = string.Format(this.labelMaxSize.Text, this.numericMaxSize.Minimum, this.numericMaxSize.Maximum);

            this.numericMaxLine.Minimum = Configuration.MIN_TEXT_VIEWER_MAX_LINE_COUNT;
            this.numericMaxLine.Maximum = Configuration.MAX_TEXT_VIEWER_MAX_LINE_COUNT;
            this.labelMaxLine.Text = string.Format(this.labelMaxLine.Text, this.numericMaxLine.Minimum, this.numericMaxLine.Maximum);

            string[] itemList = new string[] {
                Resources.OptionTextViewGeneral_CompPrev,
                Resources.OptionTextViewGeneral_CompDel,
                Resources.OptionTextViewGeneral_CompNext,
            };
            this.comboBoxCompare.Items.AddRange(itemList);

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
            this.numericMaxSize.Value = config.TextViewerMaxFileSize / 1024 / 1024;
            this.numericMaxLine.Value = config.TextViewerMaxLineCount;
            if (config.TextViewerClearCompareBufferDefault == null) {
                this.comboBoxCompare.SelectedIndex = 0;
            } else if (config.TextViewerClearCompareBufferDefault.Value == true) {
                this.comboBoxCompare.SelectedIndex = 1;
            } else {
                this.comboBoxCompare.SelectedIndex = 2;
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            // 最大サイズ
            int maxSize = (int)(this.numericMaxSize.Value) * 1024 * 1024;
            success = Configuration.CheckTextViewerMaxFileSize(ref maxSize, m_parent);
            if (!success) {
                return false;
            }

            // 最大行数
            int maxCount = (int)(this.numericMaxLine.Value);
            success = Configuration.CheckTextViewerMaxLineCount(ref maxCount, m_parent);
            if (!success) {
                return false;
            }

            // 選択範囲の比較
            BooleanFlag clearComp;
            int index = this.comboBoxCompare.SelectedIndex;
            if (index == 0) {
                clearComp = null;
            } else if (index == 1) {
                clearComp = new BooleanFlag(true);
            } else {
                clearComp = new BooleanFlag(false);
            }

            Configuration.Current.TextViewerMaxFileSize = maxSize;
            Configuration.Current.TextViewerMaxLineCount = maxCount;
            Configuration.Current.TextViewerClearCompareBufferDefault = clearComp;

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