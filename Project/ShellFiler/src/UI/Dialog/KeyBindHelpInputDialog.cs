using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：キーバインドヘルプ用キー入力ダイアログ
    //=========================================================================================
    public partial class KeyBindHelpInputDialog : Form {
        // 入力されたキー
        private Keys m_resultKeyCode;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public KeyBindHelpInputDialog() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：キーが入力されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void KeyBindHelpDialog_KeyDown(object sender, KeyEventArgs evt) {
            m_resultKeyCode = evt.KeyCode;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：パネルがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void panelInner_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        //=========================================================================================
        // コマンド：入力されたキー
        //=========================================================================================
        public Keys ResultKeyCode {
            get {
                return m_resultKeyCode;
            }
        }
    }
}
