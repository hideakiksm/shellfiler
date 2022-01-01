using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ShellFiler.UI.Dialog.MonitoringViewer {

    //=========================================================================================
    // クラス：プロセス終了の確認ダイアログ
    //=========================================================================================
    public partial class KillConfirmDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]pid      終了するプロセスのPID
        // 　　　　[in]command  終了するプロセスのコマンドライン
        // 　　　　[in]force    強制終了するときtrue
        // 戻り値：なし
        //=========================================================================================
        public KillConfirmDialog(int pid, string command, bool force) {
            InitializeComponent();

            int imageIndex;
            if (force) {
                imageIndex = (int)(IconImageListID.MonitoringViewer_PsForceTerm);
                this.labelMessageKill.Hide();
            } else {
                imageIndex = (int)(IconImageListID.MonitoringViewer_PsKill);
                this.labelMessageForce.Hide();
                this.labelMessageForce2.Hide();
            }
            this.pictureBox.Image = UIIconManager.IconImageList.Images[imageIndex];

            this.textBoxPid.Text = pid.ToString();
            this.textBoxCommand.Text = command;
            this.ActiveControl = this.buttonNo;
        }

        //=========================================================================================
        // 機　能：終了ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonYes_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.Yes;
            Close();
        }

        //=========================================================================================
        // 機　能：キャンセルボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNo_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}
