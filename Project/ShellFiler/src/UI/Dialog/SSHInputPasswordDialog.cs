using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：パスワードの入力ダイアログ
    //=========================================================================================
    public partial class SSHInputPasswordDialog : Form {
        // 認証情報
        private SSHUserAuthenticateSettingItem m_authSetting;

        // 入力したパスワード
        private string m_password;

        // パスワードを保存するときtrue
        private bool m_savePassword;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]authSetting  認証情報
        // 　　　　[in]isTempAuth   認証情報が一時的なもののときtrue
        // 戻り値：なし
        //=========================================================================================
        public SSHInputPasswordDialog(SSHUserAuthenticateSettingItem authSetting, bool isTempAuth) {
            InitializeComponent();
            m_authSetting = authSetting;
            if (authSetting.KeyAuthentication) {
                this.labelPassword.Text = Resources.DlgSSHInputPass_Passphrase;
                if (authSetting.Password == null) {
                    this.labelMessage.Text = Resources.DlgSSHInputPass_NoPassword;
                } else {
                    this.labelMessage.Text = Resources.DlgSSHInputPass_InvalidPassword;
                }
            } else {
                this.labelPassword.Text = Resources.DlgSSHInputPass_Password;
                if (authSetting.Password == null) {
                    this.labelMessage.Text = Resources.DlgSSHInputPass_NoPassword;
                } else {
                    this.labelMessage.Text = Resources.DlgSSHInputPass_InvalidPassword;
                }
            }
            if (isTempAuth) {
                this.checkBoxSave.Enabled = false;
            } else {
                this.checkBoxSave.Enabled = true;
            }
            
            string userServer = SSHUtils.CreateUserServerShort(authSetting.UserName, authSetting.ServerName, authSetting.PortNo);
            this.textBoxServer.Text = userServer;
            this.ActiveControl = this.textBoxPassword;
        }

        //=========================================================================================
        // 機　能：パスワード保存のリスク説明のリンクがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            InfoBox.Information(this, Resources.Dlg_PasswordRisk);
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_password = textBoxPassword.Text;
            m_savePassword = checkBoxSave.Checked;
            DialogResult = DialogResult.OK;

            Close();
        }

        //=========================================================================================
        // プロパティ：パスワード
        //=========================================================================================
        public string Password {
            get {
                return m_password;
            }
        }

        //=========================================================================================
        // プロパティ：パスワードを保存するときtrue
        //=========================================================================================
        public bool SavePassword {
            get {
                return m_savePassword;
            }
        }
    }
}
