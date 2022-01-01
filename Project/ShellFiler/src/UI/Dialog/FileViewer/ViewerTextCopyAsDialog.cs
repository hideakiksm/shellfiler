using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.FileViewer;
using ShellFiler.Document.Setting;

namespace ShellFiler.UI.Dialog.FileViewer {
    //=========================================================================================
    // クラス：テキストビューアでの形式を指定してコピーダイアログ
    //=========================================================================================
    public partial class ViewerTextCopyAsDialog : Form {
        // 入力した設定情報（コンストラクタとは別インスタンス）
        private TextClipboardSetting m_setting;
        
        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]setting  デフォルト値に使用する設定情報
        // 戻り値：なし
        //=========================================================================================
        public ViewerTextCopyAsDialog(TextClipboardSetting setting) {
            InitializeComponent();
            switch (setting.LineBreakMode) {
                case TextClipboardSetting.CopyLineBreakMode.Original:
                    this.radioCrOrg.Checked = true;
                    break;
                case TextClipboardSetting.CopyLineBreakMode.Cr:
                    this.radioCrCr.Checked = true;
                    break;
                case TextClipboardSetting.CopyLineBreakMode.CrLf:
                    this.radioCrCrLf.Checked = true;
                    break;
            }
            switch (setting.TabMode) {
                case TextClipboardSetting.CopyTabMode.Original:
                    this.radioTabOrg.Checked = true;
                    break;
                case TextClipboardSetting.CopyTabMode.ConvertSpace:
                    this.radioTabSpace.Checked = true;
                    break;
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_setting = new TextClipboardSetting();
            if (this.radioCrOrg.Checked) {
                m_setting.LineBreakMode = TextClipboardSetting.CopyLineBreakMode.Original;
            } else if (this.radioCrCr.Checked) {
                m_setting.LineBreakMode = TextClipboardSetting.CopyLineBreakMode.Cr;
            } else {
                m_setting.LineBreakMode = TextClipboardSetting.CopyLineBreakMode.CrLf;
            }
            if (this.radioTabOrg.Checked) {
                m_setting.TabMode = TextClipboardSetting.CopyTabMode.Original;
            } else {
                m_setting.TabMode = TextClipboardSetting.CopyTabMode.ConvertSpace;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：入力した設定情報（コンストラクタとは別インスタンス）
        //=========================================================================================
        public TextClipboardSetting Setting {
            get {
                return m_setting;
            }
        }
    }
}
