using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShellFiler.UI.Dialog.Terminal {

    //=========================================================================================
    // クラス：保存範囲の指定ダイアログ
    //=========================================================================================
    public partial class TerminalSaveTypeDialog : Form {
        // すべての範囲を保存するときtrue
        private bool m_saveAll = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TerminalSaveTypeDialog() {
            InitializeComponent();
            this.radioButtonSelection.Checked = true;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOK_Click(object sender, EventArgs evt) {
            if (this.radioButtonSelection.Checked) {
                m_saveAll = false;
            } else {
                m_saveAll = true;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：すべての範囲を保存するときtrue
        //=========================================================================================
        public bool SaveAll {
            get {
                return m_saveAll;
            }
        }
    }
}
