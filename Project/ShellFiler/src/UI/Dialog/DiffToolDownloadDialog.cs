using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using ShellFiler.Locale;
using ShellFiler.Command.FileList.Setting;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：差分表示ツールダウンロードの案内ダイアログ
    //=========================================================================================
    public partial class DiffToolDownloadDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DiffToolDownloadDialog() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：リンクボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            if (sender == this.linkLabelOption) {
                OptionSettingCommand.OpenOptionSettingDialog(this);
            } else if (sender == this.linkLabelWeb) {
                Process.Start(KnownUrl.DiffToolUrl);
            }
        }
    }
}
