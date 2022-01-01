using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：展開用パスワードの入力ダイアログ
    //=========================================================================================
    public partial class ArchivePasswordDialog : Form {
        // 入力したパスワード
        string m_password;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]arcFileName  アーカイブファイル名
        // 戻り値：なし
        //=========================================================================================
        public ArchivePasswordDialog(string arcFileName) {
            InitializeComponent();

            this.textBoxArchiveName.Text = arcFileName;
            this.ActiveControl = this.textBoxPassword;
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
            } else {
                this.textBoxPassword.PasswordChar = '*';
            }
        }

        //=========================================================================================
        // 機　能：管理ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonManage_Click(object sender, EventArgs evt) {
            ArchivePasswordManageDialog manageDialog = new ArchivePasswordManageDialog();
            manageDialog.ShowDialog(this);
            InfoBox.Information(this, Resources.DlgArcPass_PasswordUseNextTime);
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_password = this.textBoxPassword.Text;
            DialogResult = DialogResult.OK;
            Close();
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
