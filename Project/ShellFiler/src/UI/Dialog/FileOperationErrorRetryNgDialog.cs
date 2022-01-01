using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイルエラー時の再試行確認ダイアログ
    // 　　　　DialogResultで入力結果を反映  Yes:フォルダ込みで再試行、No:ファイルだけ再試行
    //=========================================================================================
    public partial class FileOperationErrorRetryNgDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]enableFile   ファイルだけ再試行を有効にするときtrue
        // 戻り値：なし
        //=========================================================================================
        public FileOperationErrorRetryNgDialog(bool enableFile) {
            InitializeComponent();
            this.pictureBoxIcon.Image = SystemIcons.Question.ToBitmap();
            this.ActiveControl = this.buttonRetry;
            this.buttonFile.Enabled = enableFile;
        }

        //=========================================================================================
        // 機　能：再試行ボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonRetry_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.Yes;
            Close();
        }

        //=========================================================================================
        // 機　能：ファイルだけ再試行ボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonFile_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}
