using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：圧縮用パスワードの入力ダイアログ
    //=========================================================================================
    public partial class ArchivePasswordNewDialog : Form {
        // 入力したパスワード
        private string m_password;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ArchivePasswordNewDialog() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：パスワードの表示チェックボックスが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void checkBoxShowPassword_CheckedChanged(object sender, EventArgs evt) {
            if (this.checkBoxShowPassword.Checked) {
                this.textBoxPassword.PasswordChar = '\0';
                this.textBoxConfirm.PasswordChar = '\0';
            } else {
                this.textBoxPassword.PasswordChar = '*';
                this.textBoxConfirm.PasswordChar = '*';
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_password = null;
            string password = this.textBoxPassword.Text;
            if (password == "") {
                InfoBox.Warning(this, Resources.DlgArcPassAdd_EmptyPassword);
                return;
            }
            if (password != this.textBoxConfirm.Text) {
                InfoBox.Warning(this, Resources.DlgArcPassAdd_ConfirmNotEquals);
                return;
            }
            m_password = password;

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ArchivePasswordNewDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && m_password == null) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // プロパティ：入力したパスワード
        //=========================================================================================
        public string Password {
            get {
                return m_password;
            }
        }
    }
}
