using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Util;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：一時ファイル削除ダイアログ
    //=========================================================================================
    public partial class DeleteTemporaryDialog : Form {
        // 見つかった一時フォルダの一覧
        private List<string> m_garbageList;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]garbageList   見つかった一時フォルダの一覧
        // 戻り値：なし
        //=========================================================================================
        public DeleteTemporaryDialog(List<string> garbageList) {
            InitializeComponent();
            m_garbageList = garbageList;

            this.pictureBoxIcon.Image = SystemIcons.Warning.ToBitmap();
            this.listBoxFolder.Items.AddRange(garbageList.ToArray());
        }

        //=========================================================================================
        // 機　能：一覧がダブルクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxFolder_DoubleClick(object sender, EventArgs evt) {
            int index = this.listBoxFolder.SelectedIndex;
            if (index == -1) {
                return;
            }
            string folder = m_garbageList[index];
            try {
                Process process = OSUtils.ProcessStartCommandLine(folder, folder);
                if (process != null) {
                    process.Dispose();
                }
            } catch (Exception) {
                InfoBox.Warning(this, Resources.DlgDeleteTemp_FailedOpenFolder);
            }
        }

        //=========================================================================================
        // 機　能：削除して閉じるボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：そのまま閉じるボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonClose_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
