using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：SSH接続先ダイアログ
    //=========================================================================================
    public partial class SSHConnectionDialog : Form {
        // パスワードのデフォルト（これと同じ文字列なら変更なし）
        private static readonly string PASSWORD_HIDE_DEFAULT = "\t*******";

        // 編集対象の認証情報
        private SSHUserAuthenticateSettingItem m_setting;

        // 自動的に再接続するときtrue
        private bool m_autoRetry = false;

        // ログインの方法
        private ConnectMode m_loginMode;

        // 入力が確定状態のときtrue
        private bool m_dialogSuccess = false;

        // 認証方式のコンボボックス実装
        private LasyComboBoxImpl m_comboBoxAuthMethod;

        // 文字コードのコンボボックス実装
        private LasyComboBoxImpl m_comboBoxEncode;

        // OSのコンボボックス実装
        private LasyComboBoxImpl m_comboBoxOs;


        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting     編集対象の認証情報（新規のときnull）
        // 　　　　[in]loginMode   ログインの方法
        // 戻り値：なし
        //=========================================================================================
        public SSHConnectionDialog(SSHUserAuthenticateSettingItem setting, ConnectMode loginMode) {
            InitializeComponent();
            m_loginMode = loginMode;

            bool isNewSetting = false;
            if (setting != null) {
                m_setting = setting;
            } else {
                m_setting = new SSHUserAuthenticateSettingItem();
                isNewSetting = true;
            }

            List<string> listHostName = new List<string>();
            List<string> listUserName = new List<string>();

            // ツリーに項目を追加
            SSHUserAuthenticateSetting authDatabase = Program.Document.SSHUserAuthenticateSetting;
            for (int i = 0; i < authDatabase.UserAuthenticateSettingList.Count; i++) {
                SSHUserAuthenticateSettingItem auth = authDatabase.UserAuthenticateSettingList[i];
                if (!listHostName.Contains(auth.ServerName)) {
                    listHostName.Add(auth.ServerName);
                }
                if (!listUserName.Contains(auth.UserName)) {
                    listUserName.Add(auth.UserName);
                }
            }

            // 各項目を初期化
            listHostName.Sort();
            listUserName.Sort();
            this.comboBoxHost.Items.AddRange(listHostName.ToArray());
            this.comboBoxUser.Items.AddRange(listUserName.ToArray());
            this.comboBoxPort.Items.Add(Resources.DlgLoginDir_SSHPortSSH);
            this.comboBoxPort.Items.Add(Resources.DlgLoginDir_SSHPortTelnet);

            string[] authMethodList = new string[] {
                Resources.DlgLoginDir_SSHAuthPassword,
                Resources.DlgLoginDir_SSHAuthKey,
            };
            int authMethodIndex = (m_setting.KeyAuthentication ? 1 : 0);
            m_comboBoxAuthMethod = new LasyComboBoxImpl(this.comboBoxAuthMethod, authMethodList, authMethodIndex);
            m_comboBoxAuthMethod.SelectedIndexChanged += new EventHandler(comboBoxAuthMethod_SelectedIndexChanged);

            string[] encodeList = new string[] {
                Resources.DlgLoginDir_SSHEncodeUTF8,
                Resources.DlgLoginDir_SSHEncodeEUC,
                Resources.DlgLoginDir_SSHEncodeSJIS,
                Resources.DlgLoginDir_SSHEncodeISO,
            };
            int encodeIndex = SSHUserAuthenticateSettingItem.EncodingToIndex(m_setting.Encoding);
            m_comboBoxEncode = new LasyComboBoxImpl(this.comboBoxEncode, encodeList, encodeIndex);

            string[] osList = new string[] {
                Resources.DlgLoginDir_SSHOSDefault,
            };
            m_comboBoxOs = new LasyComboBoxImpl(this.comboBoxOS, osList, 0);

            if (m_setting.KeyAuthentication) {
                this.textBoxPrivateKey.Text = m_setting.PrivateKeyFilePath;
            }

            // 編集内容を初期化
            this.comboBoxHost.Text = m_setting.ServerName;
            this.comboBoxPort.Text = TcpPortToString(m_setting.PortNo);
            this.comboBoxUser.Text = m_setting.UserName;

            switch (loginMode) {
                case ConnectMode.EditForSave:
                    if (isNewSetting) {
                        this.textBoxPass.Text = "";
                        this.checkBoxSavePassword.Checked = true;
                    } else if (m_setting.Password != null) {
                        this.textBoxPass.Text = PASSWORD_HIDE_DEFAULT;
                        this.checkBoxSavePassword.Checked = true;
                    } else {
                        this.checkBoxSavePassword.Checked = false;
                        this.textBoxPass.Text = "";
                    }
                    this.checkBoxSavePassword.Visible = true;
                    this.checkBoxAutoRetry.Visible = false;
                    break;
                case ConnectMode.TempNewConnection:
                    this.textBoxPass.Text = "";
                    this.checkBoxSavePassword.Checked = true;
                    this.checkBoxSavePassword.Visible = false;
                    this.checkBoxAutoRetry.Visible = false;
                    break;
                case ConnectMode.UserSererFixed:
                    this.textBoxPass.Text = "";
                    this.comboBoxHost.Enabled = false;
                    this.comboBoxPort.Enabled = false;
                    this.comboBoxUser.Enabled = false;
                    this.checkBoxSavePassword.Checked = true;
                    this.checkBoxSavePassword.Visible = false;
                    this.checkBoxAutoRetry.Visible = true;
                    break;
            }

            EnableUIItem();
            this.ActiveControl = this.comboBoxHost;
        }

        //=========================================================================================
        // 機　能：UIの有効/無効状態を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            bool enablePassword;
            bool enableKey;
            string passwordLabel;
            if (m_comboBoxAuthMethod.SelectedIndex == 0) {
                // パスワード認証
                enablePassword = this.checkBoxSavePassword.Checked;
                enableKey = false;
                passwordLabel = Resources.DlgSSHInputPass_Password;
            } else {
                // 公開鍵認証
                enablePassword = this.checkBoxSavePassword.Checked;
                enableKey = true;
                passwordLabel = Resources.DlgSSHInputPass_Passphrase;
            }
            this.textBoxPass.Enabled = enablePassword;
            this.textBoxPrivateKey.Enabled = enableKey;
            this.buttonPrivateKeyRef.Enabled = enableKey;
            this.labelPassword.Text = passwordLabel;
        }

        //=========================================================================================
        // 機　能：TCPポート番号を文字列に変換する
        // 引　数：[in]portNo  ポート番号
        // 戻り値：ポート番号の文字列表現
        //=========================================================================================
        private string TcpPortToString(int portNo) {
            if (portNo == SSHUtils.SSH_DEFAULT_PORT) {
                return Resources.DlgLoginDir_SSHPortSSH;
            } else if (portNo == SSHUtils.TELNET_DEFAULT_PORT) {
                return Resources.DlgLoginDir_SSHPortTelnet;
            } else {
                return "" + portNo;
            }
        }

        //=========================================================================================
        // 機　能：文字列をTCPポート番号に変換する
        // 引　数：[in]strPort  ポート番号の文字列表現
        // 戻り値：ポート番号（-1:エラー）
        //=========================================================================================
        private int StringToTcpPort(string strPort) {
            if (strPort == Resources.DlgLoginDir_SSHPortSSH) {
                return 22;
            } else if (strPort == Resources.DlgLoginDir_SSHPortTelnet) {
                return 23;
            } else {
                int port;
                if (int.TryParse(strPort, out port)) {
                    if (port <= 0 || port > 65536) {
                        return -1;
                    } else {
                        return port;
                    }
                } else {
                    return -1;
                }
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_dialogSuccess = false;

            // サーバー名
            string serverName = this.comboBoxHost.Text;
            if (serverName == "") {
                InfoBox.Warning(this, Resources.DlgSSHConn_Host);
                return;
            }

            // ポート
            int port = StringToTcpPort(this.comboBoxPort.Text);
            if (port < 0) {
                InfoBox.Warning(this, Resources.DlgSSHConn_Port);
                return;
            }

            // ユーザー名
            string userName = this.comboBoxUser.Text;
            if (userName == "") {
                InfoBox.Warning(this, Resources.DlgSSHConn_User);
                return;
            }

            // 同名のものがないかチェック
            SSHUserAuthenticateSetting authDatabase = Program.Document.SSHUserAuthenticateSetting;
            foreach (SSHUserAuthenticateSettingItem auth in authDatabase.UserAuthenticateSettingList) {
                if (auth == m_setting) {
                    continue;
                }
                if (auth.UserName == userName && auth.ServerName == serverName && auth.PortNo == port) {
                    InfoBox.Warning(this, Resources.DlgSSHConn_SameSetting);
                    return;
                }
            }

            // パスワード
            bool savePassword = this.checkBoxSavePassword.Checked;
            string password = null;
            if (!savePassword) {
                password = null;
            } else {
                if (this.textBoxPass.Text == PASSWORD_HIDE_DEFAULT) {
                    password = m_setting.Password;
                } else {
                    password = this.textBoxPass.Text;
                }
            }

            // 文字コード
            Encoding encoding = SSHUserAuthenticateSettingItem.IndexToEncoding(m_comboBoxEncode.SelectedIndex);

            // 表示名を作成
            string displayName = SSHUtils.CreateUserServerShort(userName, serverName, port);
            int index = 2;
            while (true) {
                bool exist = false;
                foreach (SSHUserAuthenticateSettingItem auth in authDatabase.UserAuthenticateSettingList) {
                    if (auth == m_setting) {
                        continue;
                    }
                    if (auth.DisplayName == displayName) {
                        exist = true;
                        break;
                    }
                }
                if (!exist) {
                    break;
                }
                displayName = SSHUtils.CreateUserServerShort(userName, serverName, port) + "[" + index + "]";
                index++;
            }

            // 自動再接続するか
            switch (m_loginMode) {
                case ConnectMode.EditForSave:
                    m_autoRetry = true;
                    break;
                case ConnectMode.TempNewConnection:
                    m_autoRetry = false;
                    break;
                case ConnectMode.UserSererFixed:
                    m_autoRetry = this.checkBoxAutoRetry.Checked;
                    break;
            }

            // 秘密鍵
            bool keyAuthentication;
            string privateKey;
            if (m_comboBoxAuthMethod.SelectedIndex == 1) {
                keyAuthentication = true;
                privateKey = this.textBoxPrivateKey.Text;
                if (!File.Exists(privateKey)) {
                    InfoBox.Warning(this, Resources.DlgLoginDir_SSHPrivateKeyError);
                    return;
                }
            } else {
                keyAuthentication = false;
                privateKey = null;
            }

            // 読み込んだ値を設定
            m_setting.ServerName = serverName;
            m_setting.PortNo = port;
            m_setting.UserName = userName;
            m_setting.KeyAuthentication = keyAuthentication;
            m_setting.Password = password;
            m_setting.PrivateKeyFilePath = privateKey;
            m_setting.Encoding = encoding;
            m_setting.TargetOS = SSHUserAuthenticateSettingItem.OSType.DefaultOS;
            m_setting.DisplayName = displayName;

            m_dialogSuccess = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void SSHConnectionDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && !m_dialogSuccess) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // 機　能：秘密鍵の参照ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonKeyRef_Click(object sender, EventArgs evt) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = Resources.DlgLoginDir_SSHPrivateKeyRefTitle;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ofd.Filter = Resources.DlgLoginDir_SSHPrivateKeyRefFilter;
            ofd.RestoreDirectory = true;
            DialogResult dr = ofd.ShowDialog(this);
            if (dr == DialogResult.OK) {
                string filePath = ofd.FileName;
                this.textBoxPrivateKey.Text = filePath;
            }
        }

        //=========================================================================================
        // 機　能：認証方式が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxAuthMethod_SelectedIndexChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：パスワードを保存するかどうかが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void checkBoxSavePassword_CheckedChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：リンクがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            if (sender == this.linkLabelKeyGuide) {
                // 秘密鍵ガイド
                HelpMessageDialog dialog = new HelpMessageDialog(Resources.DlgLoginDir_SSHPrivateKey, NativeResources.HtmlSSHPrivateKey, null);
                dialog.ShowDialog(this);
            } else if (sender == this.linkLabelPassword) {
                // パスワード保存
                InfoBox.Information(this, Resources.Dlg_PasswordRisk);
            }
        }

        //=========================================================================================
        // プロパティ：認証情報の編集結果（新規の場合のみ、編集の場合は対象がすでに書き換わっている）
        //=========================================================================================
        public SSHUserAuthenticateSettingItem AuthenticateSetting {
            get {
                return m_setting;
            }
        }

        //=========================================================================================
        // プロパティ：自動的に再接続するときtrue
        //=========================================================================================
        public bool AutoRetry {
            get {
                return m_autoRetry;
            }
        }

        //=========================================================================================
        // 列挙子：ログイン方法
        //=========================================================================================
        public enum ConnectMode {
            EditForSave,            // 保存用の認証情報の編集
            TempNewConnection,      // 一時接続用の新規認証情報の編集
            UserSererFixed,         // ユーザー名とサーバー名が決定済みでのその他オプションの編集
        }
    }
}
