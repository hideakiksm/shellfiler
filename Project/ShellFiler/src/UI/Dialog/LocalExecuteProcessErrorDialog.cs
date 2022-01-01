using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Virtual;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ローカル実行プロセス監視エラーダイアログ
    //=========================================================================================
    public partial class LocalExecuteProcessErrorDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]nameInfo  ローカル実行の表示名
        // 　　　　[in]program   ローカル実行するプログラム名
        // 　　　　[in]message   エラーメッセージ
        // 　　　　[in]fileList  ローカルのファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public LocalExecuteProcessErrorDialog(TemporarySpaceDisplayName nameInfo, string program, string message, List<LocalFileInfo> fileList) {
            InitializeComponent();
            this.pictureBoxIcon.Image = SystemIcons.Exclamation.ToBitmap();
            this.labelTitle.Text = nameInfo.Text;
            this.labelErrorLine.Text = message;
            this.columnHeader.Width = -1;
            foreach (LocalFileInfo fileInfo in fileList) {
                this.listViewLocalFile.Items.Add(fileInfo.FilePath);
            }
        }
    }
}
