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
using ShellFiler.Locale;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイルビューア＞クリップボード(ダンプ) の設定ページ
    //=========================================================================================
    public partial class TextViewerDumpClipboardPage : UserControl, IOptionDialogPage {
        // ダイアログの実装
        private ViewerDumpCopyAsDialog.Impl m_impl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public TextViewerDumpClipboardPage(OptionSettingDialog parent) {
            InitializeComponent();

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
            bool prev;
            DumpClipboardSetting setting;
            if (config.DumpClipboardSettingDefault == null) {
                setting = new DumpClipboardSetting();
                prev = true;
            } else {
                setting = (DumpClipboardSetting)(config.DumpClipboardSettingDefault.Clone());
                prev = false;
            }

            // UIに反映
            byte[] buffer = new byte[256 - 32];
            for (int i = 0; i < buffer.Length; i++) {
                buffer[i] = (byte)(i + 32);
            }
            m_impl = new ViewerDumpCopyAsDialog.Impl(setting, buffer, 0, buffer.Length, 16, EncodingType.SJIS);
            m_impl.Initialize(this.radioButtonDump, this.radioButtonText, this.radioButtonScreen, this.radioButtonView, this.radioButtonBase64,
                              this.comboBoxDumpRadix, this.comboBoxDumpWidth, this.textBoxDumpPrefix, this.textBoxDumpPostfix,
                              this.textBoxDumpSeparator, this.numericDumpLineWidth, this.numericBase64Folding, this.textBoxSample,
                              this.buttonDumpDefault, this.buttonDumpC, this.buttonDumpBasic);
            if (prev) {
                this.radioButtonPrev.Checked = true;
            } else {
                this.radioButtonFix.Checked = true;
            }
            RadioButtonPrevFix_CheckedChanged(null, null);
        }

        //=========================================================================================
        // 機　能：前回値か固定値かを選択するラジオボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonPrevFix_CheckedChanged(object sender, EventArgs evt) {
            if (this.radioButtonPrev.Checked) {
                this.radioButtonDump.Enabled = false;
                this.radioButtonText.Enabled = false;
                this.radioButtonScreen.Enabled = false;
                this.radioButtonBase64.Enabled = false;
                this.comboBoxDumpRadix.Enabled = false;
                this.comboBoxDumpWidth.Enabled = false;
                this.textBoxDumpPrefix.Enabled = false;
                this.textBoxDumpPostfix.Enabled = false;
                this.textBoxDumpSeparator.Enabled = false;
                this.numericDumpLineWidth.Enabled = false;
                this.numericBase64Folding.Enabled = false;
                this.textBoxSample.Enabled = false;
                this.buttonDumpDefault.Enabled = false;
                this.buttonDumpC.Enabled = false;
                this.buttonDumpBasic.Enabled = false;
            } else {
                this.radioButtonDump.Enabled = true;
                this.radioButtonText.Enabled = true;
                this.radioButtonScreen.Enabled = true;
                this.radioButtonBase64.Enabled = true;
                this.textBoxSample.Enabled = true;
                m_impl.ChangeEnabled();
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            m_impl.OnOk();
            
            DumpClipboardSetting setting;
            if (this.radioButtonPrev.Checked) {
                setting = null;
            } else {
                setting = m_impl.Setting;
            }
            Configuration.Current.DumpClipboardSettingDefault = setting;

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