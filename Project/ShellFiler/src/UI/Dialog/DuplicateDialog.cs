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
    // クラス：Windowsファイルの編集機能
    //=========================================================================================
    public partial class DuplicateDialog : Form {
        // 対象がディレクトリのときtrue
        private bool m_isDirectory;

        // 新しいファイル名
        private string m_newFileName;

        // ファイルシステム
        private FileSystemID m_fileSystem;

        // 対象パスのファイル一覧
        private List<UIFile> m_targetList;

        // ２重化対象のファイル情報（編集結果）
        private DuplicateFileInfo m_duplicateFileInfoModified;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]isDirectory   対象がディレクトリのときtrue
        // 　　　　[in]newFileName   新しいファイル名
        // 　　　　[in]fileSystem    ファイルシステム
        // 　　　　[in]targetList    対象パスのファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public DuplicateDialog(bool isDirectory, string newFileName, FileSystemID fileSystem, List<UIFile> targetList) {
            InitializeComponent();
            m_isDirectory = isDirectory;
            m_newFileName = newFileName;
            m_fileSystem = fileSystem;
            m_targetList = targetList;
            this.textBoxFileName.Text = m_newFileName;
            this.textBoxFileNameCurrent.Text = m_newFileName;

            if (FileSystemID.IsSSH(fileSystem)) {
                this.checkBoxCopyAttr.Checked = Configuration.Current.TransferAttributeSetMode.SshSetAtributeAll;
            } else {
                this.checkBoxCopyAttr.Checked = Configuration.Current.TransferAttributeSetMode.WindowsSetAttributeAll;
            }
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
            m_duplicateFileInfoModified = null;

            // ファイル名の入力があるか？
            string fileName = this.textBoxFileName.Text;
            if (fileName.Length == 0) {
                InfoBox.Warning(this, Resources.DlgDuplicate_NoFileName);
                return;
            }

            // ファイル名の重複確認
            bool foundSame = false;
            if (FileSystemID.IgnoreCaseFolderPath(m_fileSystem)) {
                for (int i = 0; i < m_targetList.Count; i++) {
                    if (m_targetList[i].FileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)) {
                        foundSame = true;
                    }
                }
            } else {
                for (int i = 0; i < m_targetList.Count; i++) {
                    if (m_targetList[i].FileName == fileName) {
                        foundSame = true;
                    }
                }
            }
            if (foundSame) {
                InfoBox.Warning(this, Resources.DlgDuplicate_SameFileName);
                return;
            }

            // 値を取り込む
            AttributeSetMode attrMode = new AttributeSetMode();
            attrMode.WindowsSetAttributeAll = this.checkBoxCopyAttr.Checked;
            attrMode.SshSetAtributeAll = this.checkBoxCopyAttr.Checked;
            m_duplicateFileInfoModified = new DuplicateFileInfo(m_isDirectory, this.textBoxFileName.Text, attrMode);

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
            if (DialogResult == DialogResult.OK && m_duplicateFileInfoModified == null) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // プロパティ：２重化対象のファイル情報（編集結果）
        //=========================================================================================
        public DuplicateFileInfo DuplicateFileInfoModified {
            get {
                return m_duplicateFileInfoModified;
            }
        }
    }
}
