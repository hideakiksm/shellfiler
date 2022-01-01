using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.Locale;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：圧縮ダイアログ（ダウンロードの案内）
    //=========================================================================================
    public partial class ArchiveDownloadDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ArchiveDownloadDialog() {
            InitializeComponent();
            this.labelBit1.Text = string.Format(this.labelBit1.Text, OSUtils.GetCurrentProcessBits());
            this.labelBit2.Text = string.Format(this.labelBit2.Text, OSUtils.GetCurrentProcessBits());
        }

        //=========================================================================================
        // 機　能：リンクボタンが押されたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelDL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            InfoBox.Information(Program.MainWindow, Resources.DlgArchiveDL_DownLoadInfo, OSUtils.GetCurrentProcessBits());
            Process.Start(KnownUrl.SevenZipUrl);
        }
    }
}
