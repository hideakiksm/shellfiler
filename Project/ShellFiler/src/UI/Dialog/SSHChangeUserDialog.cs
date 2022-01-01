using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.GraphicsViewer;
using ShellFiler.Virtual;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.TerminalSession.CommandEmulator;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ユーザーの切り替えダイアログ
    //=========================================================================================
    public partial class SSHChangeUserDialog : Form {
        // ユーザー変更の情報
        private SSHChangeUserInfo m_changeUserInfo;
        
        // 現在実行中のユーザーがスーパーユーザーのときtrue
        private bool m_isSuperUserNow;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]directory     現在のディレクトリ（suパスを含む）
        // 　　　　[in]userServer    現在の操作ユーザー（ユーザー名@サーバー名）
        // 　　　　[in]rootUserName  ルートとなるユーザー名
        // 　　　　[in]dictionary    現在のディレクトリに対するコマンド辞書
        // 戻り値：なし
        //=========================================================================================
        public SSHChangeUserDialog(string directory, string userServer, string rootUserName, ShellCommandDictionary dictionary) {
            InitializeComponent();

            m_isSuperUserNow = SSHUtils.IsSuperUser(userServer, rootUserName);
            if (m_isSuperUserNow) {
                this.textBoxCurrent.Text = userServer + Resources.DlgSSHChangeUser_SuperUserNow;
            } else {
                this.textBoxCurrent.Text = userServer;
            }
            this.checkBoxLoginShell.Checked = dictionary.ValueChangeUserLoginShell;
            this.radioButtonSu.Checked = true;
            this.ActiveControl = this.textBoxUser;
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            if (this.radioButtonSu.Checked) {
                this.textBoxUser.Enabled = true;
                this.textBoxPassword.Enabled = !m_isSuperUserNow;
                this.checkBoxLoginShell.Enabled = true;
            } else {
                this.textBoxUser.Enabled = false;
                this.textBoxPassword.Enabled = false;
                this.checkBoxLoginShell.Enabled = false;
            }
        }
        
        //=========================================================================================
        // 機　能：チェックボタンの状態が切り替えられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void radioButton_CheckedChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.radioButtonSu.Checked) {
                string userName = this.textBoxUser.Text;
                string password = this.textBoxPassword.Text;
                bool useLoginShell = this.checkBoxLoginShell.Checked;
                m_changeUserInfo = new SSHChangeUserInfo(SSHChangeUserInfo.ChangeUserMode.Su, m_isSuperUserNow, userName, password, useLoginShell);
            } else {
                m_changeUserInfo = new SSHChangeUserInfo(SSHChangeUserInfo.ChangeUserMode.Exit, false, null, null, false);
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：ユーザー変更の情報
        //=========================================================================================
        public SSHChangeUserInfo ChangeUserInfo {
            get {
                return m_changeUserInfo;
            }
        }
    }
}
