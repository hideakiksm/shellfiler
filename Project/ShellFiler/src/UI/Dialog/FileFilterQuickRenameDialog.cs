using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイルフィルターのクイック設定改名ダイアログ
    //=========================================================================================
    public partial class FileFilterQuickRenameDialog : Form {
        // 新しい名前（終了時にエラーとなってクローズを抑制するときnull）
        private string m_newName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]oldName  現在の名前
        // 戻り値：なし
        //=========================================================================================
        public FileFilterQuickRenameDialog(string oldName) {
            InitializeComponent();
            this.textBoxName.Text = oldName;
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileFilterQuickRenameDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            if (DialogResult == DialogResult.OK && m_newName == null) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.textBoxName.Text == "") {
                InfoBox.Warning(this, Resources.DlgFileFilter_QuickRenameNoName);
                m_newName = null;
                return;
            }
            m_newName = this.textBoxName.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：新しい名前
        //=========================================================================================
        public string NewName {
            get {
                return m_newName;
            }
        }
    }
}
