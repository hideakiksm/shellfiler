using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス；コマンド引数
    //=========================================================================================
    public partial class CommandArgumentHelp : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CommandArgumentHelp() {
            InitializeComponent();
            this.ControlBox = false;

            string urlFile = NativeResources.HtmlCommandArgument;
            string url = "res://" + Assembly.GetExecutingAssembly().Location + "/" + urlFile;
            this.webBrowser.Navigate(url);
        }

        //=========================================================================================
        // 機　能：表示位置を調整する
        // 引　数：[in]parent  基準となるフォーム
        // 戻り値：なし
        //=========================================================================================
        public void AdjustLocation(Form parent) {
            // 親の右横に移動
            this.Location = new Point(parent.Right, parent.Top);
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void CommandArgumentHelp_FormClosed(object sender, FormClosedEventArgs evt) {
            this.webBrowser.Dispose();
        }
    }
}
