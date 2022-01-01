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
    // クラス：タイトル編集ダイアログ
    //=========================================================================================
    public partial class TerminalEditTitleDialog : Form {
        // 入力結果
        private string m_resultTitle;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]userServer   ユーザ名@サーバ名
        //         [in]title        編集対象の名前
        // 戻り値：なし
        //=========================================================================================
        public TerminalEditTitleDialog(string userServer, string title) {
            InitializeComponent();
            this.labelMessage.Text = string.Format(this.labelMessage.Text, userServer);
            this.textBoxName.Text = title;
        }

        //=========================================================================================
        // 機　能：フォームが読み込まれたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_resultTitle = this.textBoxName.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：入力結果
        //=========================================================================================
        public string ResultTitle {
            get {
                return m_resultTitle;
            }
        }
    }
}
