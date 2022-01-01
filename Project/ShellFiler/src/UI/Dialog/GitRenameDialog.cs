using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.Locale;
using ShellFiler.Api;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：gitコマンドのリネーム機能
    //=========================================================================================
    public partial class GitRenameDialog : Form {
        // 新しいファイル名（編集結果）
        private string m_newFileName;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileName   ファイル名
        // 戻り値：なし
        //=========================================================================================
        public GitRenameDialog(string fileName) {
            InitializeComponent();

            this.textBoxFileName.Text = fileName;
            this.textBoxFileNameCurrent.Text = fileName;

            FormUtils.SetCursorPosExtension(this.textBoxFileName);
            this.ActiveControl = this.textBoxFileName;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_newFileName = this.textBoxFileName.Text;
            if (m_newFileName.Length == 0) {
                m_newFileName = null;
                InfoBox.Warning(this, Resources.DlgGit_NoFileName);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RenameWindowsDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && m_newFileName == null) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // プロパティ：新しいファイル名（編集結果）
        //=========================================================================================
        public string NewFileName {
            get {
                return m_newFileName;
            }
        }
    }
}
