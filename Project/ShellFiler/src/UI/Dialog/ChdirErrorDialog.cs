using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.UI;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：フォルダ変更エラーダイアログ
    //=========================================================================================
    public partial class ChdirErrorDialog : Form {
        // 再試行時のフォルダ
        private string m_retryFolder;

        // 直前のフォルダ（直前のフォルダがないときnull）
        private string m_prevFolder;

        // ルートフォルダ（ルートフォルダが取得できないときnull）
        private string m_rootFolder;

        // 親フォルダ（親フォルダがないときnull）
        private string m_parentFolder;

        // ホームフォルダ
        private string m_homeFolder;

        // Windowsデスクトップ
        private string m_desktopFolder;

        // 次回のフォルダ変更条件
        private ChangeDirectoryParam m_chdirParam;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]chdirParam  失敗したフォルダ変更条件
        // 　　　　[in]history     失敗した一覧のパスヒストリ
        // 戻り値：なし
        //=========================================================================================
        public ChdirErrorDialog(ChangeDirectoryParam chdirParam, PathHistory history) {
            InitializeComponent();

            m_chdirParam = chdirParam;
            this.pictureBoxIcon.Image = SystemIcons.Warning.ToBitmap();
            if (chdirParam is ChangeDirectoryParam.Refresh) {
                this.labelMessage.Text = Resources.Msg_FailedRefreshDir;
            } else {
                this.labelMessage.Text = Resources.Msg_FailedChangeDir;
            }
            this.textBoxFail.Text = chdirParam.TargetDirectory;
            this.textBoxNext.Text = chdirParam.TargetDirectory;

            // フォルダ
            m_retryFolder = chdirParam.TargetDirectory;
            
            if (chdirParam is ChangeDirectoryParam.Refresh) {
                if (history.EnablePrev) {
                    m_prevFolder = history.HistoryList[history.CurrentIndex - 1].Directory;
                }
            } else {
                if (history.CurrentIndex < history.HistoryList.Count) {
                    m_prevFolder = history.HistoryList[history.CurrentIndex].Directory;
                }
            }

            FileSystemID fileSystemId = Program.Document.FileSystemFactory.GetFileSystemFromRootPath(chdirParam.TargetDirectory);
            if (fileSystemId != FileSystemID.None) {
                IFileSystem fileSystem = Program.Document.FileSystemFactory.CreateFileSystemForFileOperation(fileSystemId);
                string dummySub;
                fileSystem.SplitRootPath(chdirParam.TargetDirectory, out m_rootFolder, out dummySub);
                m_parentFolder = fileSystem.GetFullPath(chdirParam.TargetDirectory + fileSystem.GetPathSeparator(null) + "..");
                m_homeFolder = fileSystem.GetHomePath(chdirParam.TargetDirectory);
            }

            m_desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            EnableUIItem();
            this.radioButtonRetry.Checked = true;
            this.ActiveControl = this.radioButtonRetry;
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            this.radioButtonRetry.Enabled = (m_retryFolder != null);
            this.radioButtonPrev.Enabled = (m_prevFolder != null);
            this.radioButtonRoot.Enabled = (m_rootFolder != null);
            this.radioButtonParent.Enabled = (m_parentFolder != null);
            this.radioButtonHome.Enabled = (m_homeFolder != null);
            this.radioButtonDesktop.Enabled = (m_desktopFolder != null);
        }

        //=========================================================================================
        // 機　能：ラジオボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButton_CheckedChanged(object sender, EventArgs evt) {
            this.textBoxNext.Text = GetNextFolder();
        }

        //=========================================================================================
        // 機　能：UIの入力値から次に表示するフォルダを取得する
        // 引　数：なし
        // 戻り値：フォルダ名
        //=========================================================================================
        private string GetNextFolder() {
            string folder;
            if (this.radioButtonRetry.Checked) {
                folder = m_retryFolder;
            } else if (this.radioButtonPrev.Checked) {
                folder = m_prevFolder;
            } else if (this.radioButtonRoot.Checked) {
                folder = m_rootFolder;
            } else if (this.radioButtonParent.Checked) {
                folder = m_parentFolder;
            } else if (this.radioButtonHome.Checked) {
                folder = m_homeFolder;
            } else {
                folder = m_desktopFolder;
            }
            return folder;
        }

        //=========================================================================================
        // 機　能：OKボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.radioButtonRetry.Checked) {
                ;
            } else {
                string folder = GetNextFolder();
                m_chdirParam = new ChangeDirectoryParam.Direct(this.textBoxNext.Text);
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：次回のフォルダ変更条件
        //=========================================================================================
        public ChangeDirectoryParam NextChangeDirParam {
            get {
                return m_chdirParam;
            }
        }
    }
}
