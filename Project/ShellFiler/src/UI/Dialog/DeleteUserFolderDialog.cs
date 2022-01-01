using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.Virtual;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ユーザーフォルダの削除ダイアログ
    //=========================================================================================
    public partial class DeleteUserFolderDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DeleteUserFolderDialog() {
            InitializeComponent();
            this.textBoxWork.Text = DirectoryManager.TemporaryRoot;
            this.textBoxSetting.Text = DirectoryManager.ApplicationDataRoot;
        }

        //=========================================================================================
        // 機　能：設定フォルダの開くボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonSetting_Click(object sender, EventArgs evt) {
            string folder = DirectoryManager.ApplicationDataRoot;
            OpenFolder(folder);
        }

        //=========================================================================================
        // 機　能：作業フォルダの開くボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonWork_Click(object sender, EventArgs evt) {
            string folder = DirectoryManager.TemporaryRoot;
            OpenFolder(folder);
        }

        //=========================================================================================
        // 機　能：指定されたフォルダをエクスプローラで開く
        // 引　数：[in]folder   開くフォルダ
        // 戻り値：なし
        //=========================================================================================
        private void OpenFolder(string folder) {
            try {
                Process process = Process.Start(folder);
                if (process != null) {
                    process.Dispose();
                }
            } catch (Exception) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_ExplorerCannotOpen);
            }
        }
    }
}
