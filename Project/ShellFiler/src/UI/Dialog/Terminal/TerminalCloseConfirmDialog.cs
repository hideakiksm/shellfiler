using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.Terminal;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.Log;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;


namespace ShellFiler.UI.Dialog.Terminal {

    //=========================================================================================
    // クラス：ターミナルウィンドウの終了確認ダイアログ
    //=========================================================================================
    public partial class TerminalCloseConfirmDialog : Form {
        // チャネルを閉じる選択を行ったときtrue
        private bool m_closeChannel;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]mode        ダイアログを閉じるときのモード
        // 　　　　[in]userServer  閉じる接続のユーザー名@サーバー名
        // 戻り値：なし
        //=========================================================================================
        public TerminalCloseConfirmDialog(TerminalCloseConfirmMode mode, string userServer) {
            InitializeComponent();
            this.pictureBoxIcon.Image = SystemIcons.Information.ToBitmap();
            if (mode == TerminalCloseConfirmMode.KeepChannelConfirm) {
                this.radioButtonKeep.Checked = true;
            } else {
                this.radioButtonClose.Checked = true;
            }
            this.labelMessage.Text = string.Format(this.labelMessage.Text, userServer);
        }

        //=========================================================================================
        // 機　能：ラジオボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButton_CheckedChanged(object sender, EventArgs evt) {
            if (this.radioButtonKeep.Checked) {
                this.labelWarning.Visible = true;
            } else {
                this.labelWarning.Visible = false;
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_closeChannel = (this.radioButtonClose.Checked);
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：チャネルを閉じる選択を行ったときtrue
        //=========================================================================================
        public bool CloseChannel {
            get {
                return m_closeChannel;
            }
        }
    }
}
