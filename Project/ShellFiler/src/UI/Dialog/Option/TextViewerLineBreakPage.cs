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
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Dialog.FileViewer;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイルビューア＞表示 の設定ページ
    //=========================================================================================
    public partial class TextViewerLineBreakPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        // ダイアログの実装
        private ViewerLineWidthDialog.Impl m_impl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public TextViewerLineBreakPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;

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
            TextViewerLineBreakSetting setting;
            if (Configuration.Current.TextViewerLineBreakDefault == null) {
                setting = new TextViewerLineBreakSetting();
                prev = true;
            } else {
                setting = (TextViewerLineBreakSetting)(Configuration.Current.TextViewerLineBreakDefault.Clone());
                prev = false;
            }
            m_impl = new ViewerLineWidthDialog.Impl(m_parent, setting);
            m_impl.Initialize(this.radioButtonChar, this.radioButtonPixel, this.radioButtonNone,
                              this.numericChar, this.numericPixel);

            // UIに反映
            if (prev) {
                this.radioButtonLinePrev.Checked = true;
            } else {
                this.radioButtonLineFix.Checked = true;
            }
            RadioButtonPrevFix_CheckedChanged(null, null);

            this.numericByteCount.Minimum = TextViewerLineBreakSetting.MIN_DUMP_LINE_BYTE_COUNT;
            this.numericByteCount.Maximum = TextViewerLineBreakSetting.MAX_DUMP_LINE_BYTE_COUNT;
            this.numericByteCount.Value = setting.DumpLineByteCount;

            // 拡張子
            this.textBoxTab4Ext.Text = Configuration.Current.TextViewerTab4Extension;
        }

        //=========================================================================================
        // 機　能：前回値か固定値かを選択するラジオボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonPrevFix_CheckedChanged(object sender, EventArgs evt) {
            if (this.radioButtonLinePrev.Checked) {
                this.radioButtonChar.Enabled = false;
                this.radioButtonPixel.Enabled = false;
                this.radioButtonNone.Enabled = false;
                this.numericChar.Enabled = false;
                this.numericPixel.Enabled = false;
                this.numericByteCount.Enabled = false;
            } else {
                this.radioButtonChar.Enabled = true;
                this.radioButtonPixel.Enabled = true;
                this.radioButtonNone.Enabled = true;
                this.numericByteCount.Enabled = true;
                m_impl.OnRadioButtonCheckedChanged();
            }
        }

        //=========================================================================================
        // 機　能：前回値か固定値かを選択するラジオボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonLineBreak_CheckedChanged(object sender, EventArgs evt) {
            m_impl.OnRadioButtonCheckedChanged();
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            TextViewerLineBreakSetting setting;
            if (this.radioButtonLinePrev.Checked) {
                setting = null;
            } else {
                success = m_impl.OnOk();
                if (!success) {
                    return false;
                }
                setting = m_impl.Setting;
                setting.DumpLineByteCount = (int)(this.numericByteCount.Value);
            }

            Configuration.Current.TextViewerLineBreakDefault = setting;
            Configuration.Current.TextViewerTab4Extension = this.textBoxTab4Ext.Text;
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
