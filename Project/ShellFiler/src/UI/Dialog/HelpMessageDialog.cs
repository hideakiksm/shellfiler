using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ヘルプメッセージの表示用ダイアログ
    //=========================================================================================
    public partial class HelpMessageDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]title    ダイアログのタイトル
        // 　　　　[in]urlFile  表示するファイルのダイアログリソース（HTML指定のときnull）
        // 　　　　[in]html     表示するHTML（URL指定のときnull）
        // 戻り値：なし
        //=========================================================================================
        public HelpMessageDialog(string title, string urlFile, string html) {
            InitializeComponent();

            this.Text = title;
            if (urlFile != null) {
                string url = "res://" + Assembly.GetExecutingAssembly().Location + "/" + urlFile;
                this.webBrowser.Navigate(url);
            } else {
                this.webBrowser.DocumentText = html;
            }
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void HelpMessageDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            this.webBrowser.Dispose();
        }

        //=========================================================================================
        // 機　能：閉じるボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonClose_Click(object sender, EventArgs evt) {
            Close();
        }
    }
}
