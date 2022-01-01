using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {
    
    //=========================================================================================
    // クラス：設定の反映確認ダイアログ
    //=========================================================================================
    public partial class ConfirmSettingUpdatedDialog : Form {
        // 他のプロセスの結果を読み込むときtrue
        private bool m_loadExternalConfig = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]dispName  確認する設定の表示名
        // 戻り値：なし
        //=========================================================================================
        public ConfirmSettingUpdatedDialog(string dispName) {
            InitializeComponent();
            this.Text = string.Format(this.Text, dispName);
            this.labelMessage.Text = string.Format(this.labelMessage.Text, dispName);
            this.pictureBoxIcon.Image = SystemIcons.Question.ToBitmap();
            this.radioButtonLoad.Checked = true;
            this.ActiveControl = this.radioButtonLoad;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.radioButtonLoad.Checked) {
                m_loadExternalConfig = true;
            } else {
                m_loadExternalConfig = false;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：他のプロセスの結果を読み込むときtrue
        //=========================================================================================
        public bool LoadExternalConfig {
            get {
                return m_loadExternalConfig;
            }
        }
    }
}
