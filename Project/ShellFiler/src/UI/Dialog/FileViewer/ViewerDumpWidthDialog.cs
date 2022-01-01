using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.FileViewer {

    //=========================================================================================
    // クラス：折り返し設定の変更ダイアログ
    //=========================================================================================
    public partial class ViewerDumpWidthDialog : Form {
        // 元の設定情報
        private TextViewerLineBreakSetting m_orgSetting;

        // 編集した折り返し設定（コンストラクタとは別インスタンス）
        private TextViewerLineBreakSetting m_setting;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting  設定情報
        // 戻り値：なし
        //=========================================================================================
        public ViewerDumpWidthDialog(TextViewerLineBreakSetting setting) {
            InitializeComponent();
            m_orgSetting = setting;
            m_setting = (TextViewerLineBreakSetting)setting.Clone();
            this.numericByteCount.Minimum = TextViewerLineBreakSetting.MIN_DUMP_LINE_BYTE_COUNT;
            this.numericByteCount.Maximum = TextViewerLineBreakSetting.MAX_DUMP_LINE_BYTE_COUNT;
            this.numericByteCount.Value = m_setting.DumpLineByteCount;
            this.numericByteCount.Select(0, (this.numericByteCount.Value).ToString().Length);
        }

        //=========================================================================================
        // 機　能：OKボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_setting.DumpLineByteCount = (int)(this.numericByteCount.Value);
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：編集した折り返し設定（コンストラクタとは別インスタンス）
        //=========================================================================================
        public TextViewerLineBreakSetting Setting {
            get {
                return m_setting;
            }
        }
    }
}
