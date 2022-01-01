using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：拡張子入力ダイアログ
    //=========================================================================================
    public partial class RenameSelectedExtensionDialog : Form {
        // 入力した拡張子
        private string m_extension;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]extension  元の拡張子
        // 戻り値：なし
        //=========================================================================================
        public RenameSelectedExtensionDialog(string extension) {
            InitializeComponent();
            this.textBoxExt.Text = extension;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.textBoxExt.Text == "") {
                InfoBox.Warning(this, Resources.DlgRenameSel_ExtErrorNoInput);
                m_extension = null;
                return;
            }
            m_extension = this.textBoxExt.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RenameSelectedExtensionDialog_FormClosing(object sender, FormClosingEventArgs evt) {
                        // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && m_extension == null) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // プロパティ：入力した拡張子
        //=========================================================================================
        public string Extension {
            get {
                return m_extension;
            }
        }
    }
}
