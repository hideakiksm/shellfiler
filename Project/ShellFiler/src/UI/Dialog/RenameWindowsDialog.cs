using System;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.Locale;
using ShellFiler.Api;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：Windowsファイルの編集機能
    //=========================================================================================
    public partial class RenameWindowsDialog : Form, RenameFileInfo.IRenameFileDialog {
        // 編集対象のファイル情報（編集前）
        private RenameFileInfo m_renameFileInfoOrg;

        // 編集対象のファイル情報（編集結果）
        private RenameFileInfo m_renameFileInfoModified;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]renameInfo  編集対象のファイル情報
        // 戻り値：なし
        //=========================================================================================
        public RenameWindowsDialog(RenameFileInfo renameInfo) {
            InitializeComponent();
            m_renameFileInfoOrg = renameInfo;
            this.textBoxFileName.Text = m_renameFileInfoOrg.WindowsInfo.FileName;
            this.textBoxFileNameCurrent.Text = m_renameFileInfoOrg.WindowsInfo.FileName;
            this.checkBoxArchive.Checked = ((renameInfo.WindowsInfo.FileAttributes & FileAttributes.Archive) != 0);
            this.checkBoxHidden.Checked = ((renameInfo.WindowsInfo.FileAttributes & FileAttributes.Hidden) != 0);
            this.checkBoxReadOnly.Checked = ((renameInfo.WindowsInfo.FileAttributes & FileAttributes.ReadOnly) != 0);
            this.checkBoxSystem.Checked = ((renameInfo.WindowsInfo.FileAttributes & FileAttributes.System) != 0);
            this.dateTimeUpdate.CustomFormat = DateTimeFormatter.RENAME_TIME_CUSTOM_FORMAT;
            this.dateTimeUpdate.Value = renameInfo.WindowsInfo.ModifiedDate;
            this.dateTimeCreate.CustomFormat = DateTimeFormatter.RENAME_TIME_CUSTOM_FORMAT;
            this.dateTimeCreate.Value = renameInfo.WindowsInfo.CreationDate;
            this.dateTimeAccess.CustomFormat = DateTimeFormatter.RENAME_TIME_CUSTOM_FORMAT;
            this.dateTimeAccess.Value = renameInfo.WindowsInfo.AccessDate;
            FormUtils.SetCursorPosExtension(this.textBoxFileName);
            this.buttonTimeUpdate.ImageList = UIIconManager.IconImageList;
            this.buttonTimeUpdate.ImageIndex = (int)IconImageListID.ButtonRenameTimeSame1;
            this.buttonTimeCreate.ImageList = UIIconManager.IconImageList;
            this.buttonTimeCreate.ImageIndex = (int)IconImageListID.ButtonRenameTimeSame2;
            this.buttonTimeAccess.ImageList = UIIconManager.IconImageList;
            this.buttonTimeAccess.ImageIndex = (int)IconImageListID.ButtonRenameTimeSame3;
            this.ActiveControl = this.textBoxFileName;
        }

        //=========================================================================================
        // 機　能：大文字へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNameUpper_Click(object sender, EventArgs evt) {
            this.textBoxFileName.Text = this.textBoxFileName.Text.ToUpper();
        }

        //=========================================================================================
        // 機　能：小文字へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNameLower_Click(object sender, EventArgs evt) {
            this.textBoxFileName.Text = this.textBoxFileName.Text.ToLower();
        }

        //=========================================================================================
        // 機　能：先頭大文字ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNameCapital_Click(object sender, EventArgs evt) {
            string name = this.textBoxFileName.Text;
            this.textBoxFileName.Text = StringUtils.ToCapital(name);
        }

        //=========================================================================================
        // 機　能：属性の詳細リンクがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelEtcAttr_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            AttributeDisplayDialog dialog = new AttributeDisplayDialog(this.m_renameFileInfoOrg.WindowsInfo.FileAttributes);
            dialog.ShowDialog(this);
        }

        //=========================================================================================
        // 機　能：本日正午ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNoon_Click(object sender, EventArgs evt) {
            DateTime now = DateTime.Now;
            DateTime noon = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0, 0, now.Kind);
            this.dateTimeUpdate.Value = noon;
            this.dateTimeCreate.Value = noon;
            this.dateTimeAccess.Value = noon;
        }

        //=========================================================================================
        // 機　能：現在時刻ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCurrent_Click(object sender, EventArgs evt) {
            DateTime now = DateTime.Now;
            this.dateTimeUpdate.Value = now;
            this.dateTimeCreate.Value = now;
            this.dateTimeAccess.Value = now;
        }

        //=========================================================================================
        // 機　能：更新時刻を他の時刻に反映する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonTimeUpdate_Click(object sender, EventArgs evt) {
            this.dateTimeCreate.Value = this.dateTimeUpdate.Value;
            this.dateTimeAccess.Value = this.dateTimeUpdate.Value;
        }

        //=========================================================================================
        // 機　能：作成時刻を他の時刻に反映する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonTimeCreate_Click(object sender, EventArgs evt) {
            this.dateTimeUpdate.Value = this.dateTimeCreate.Value;
            this.dateTimeAccess.Value = this.dateTimeCreate.Value;
        }

        //=========================================================================================
        // 機　能：アクセス時刻を他の時刻に反映する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonTimeAccess_Click(object sender, EventArgs evt) {
            this.dateTimeUpdate.Value = this.dateTimeAccess.Value;
            this.dateTimeCreate.Value = this.dateTimeAccess.Value;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_renameFileInfoModified = null;
            if (this.textBoxFileName.Text.Length == 0) {
                InfoBox.Warning(this, Resources.DlgRename_NoFileName);
                return;
            }

            RenameFileInfo.WindowsRenameInfo winInfo = new RenameFileInfo.WindowsRenameInfo();
            winInfo.FileName = this.textBoxFileName.Text;
            FileAttributes attr = m_renameFileInfoOrg.WindowsInfo.FileAttributes;
            if (this.checkBoxArchive.Checked) {
                attr |= FileAttributes.Archive;
            } else {
                attr &= ~FileAttributes.Archive;
            }
            if (this.checkBoxHidden.Checked) {
                attr |= FileAttributes.Hidden;
            } else {
                attr &= ~FileAttributes.Hidden;
            }
            if (this.checkBoxReadOnly.Checked) {
                attr |= FileAttributes.ReadOnly;
            } else {
                attr &= ~FileAttributes.ReadOnly;
            }
            if (this.checkBoxSystem.Checked) {
                attr |= FileAttributes.System;
            } else {
                attr &= ~FileAttributes.System;
            }
            winInfo.FileAttributes = attr;
            winInfo.ModifiedDate = this.dateTimeUpdate.Value;
            winInfo.CreationDate = this.dateTimeCreate.Value;
            winInfo.AccessDate = this.dateTimeAccess.Value;
            m_renameFileInfoModified = new RenameFileInfo(winInfo);

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
            if (DialogResult == DialogResult.OK && m_renameFileInfoModified == null) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // 機　能：名前の変更ダイアログを表示する
        // 引　数：[in]parent  ダイアログの親となるフォーム
        // 戻り値：なし
        //=========================================================================================
        public DialogResult ShowRenameDialog(Form parent) {
            return ShowDialog(parent);
        }

        //=========================================================================================
        // プロパティ：編集対象のファイル情報（編集前）
        //=========================================================================================
        public RenameFileInfo OriginalRenameFileInfo {
            get {
                return m_renameFileInfoOrg;
            }
        }

        //=========================================================================================
        // プロパティ：編集対象のファイル情報（編集結果）
        //=========================================================================================
        public RenameFileInfo ModifiedRenameFileInfo {
            get {
                return m_renameFileInfoModified;
            }
        }
    }
}
