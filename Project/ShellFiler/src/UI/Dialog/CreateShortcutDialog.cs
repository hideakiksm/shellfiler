using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ショートカットの種類指定ダイアログ
    //=========================================================================================
    public partial class CreateShortcutDialog : Form {
        // ショートカットの種類
        private ShortcutType m_shortcutType;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]type    ショートカットの種類
        // 戻り値：なし
        //=========================================================================================
        public CreateShortcutDialog(ShortcutType type) {
            InitializeComponent();
            if (type == ShortcutType.SymbolicLink) {
                this.radioButtonSymbolic.Checked = true;
            } else {
                this.radioButtonHard.Checked = true;
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.radioButtonSymbolic.Checked) {
                m_shortcutType = ShortcutType.SymbolicLink;
            } else {
                m_shortcutType = ShortcutType.HardLink;
            }
            this.DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：ショートカットの種類
        //=========================================================================================
        public ShortcutType ShortcutType {
            get {
                return m_shortcutType;
            }
        }
    }
}
