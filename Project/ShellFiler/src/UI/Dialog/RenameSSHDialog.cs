using System;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.SSH;
using ShellFiler.Properties;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Util;
using ShellFiler.Locale;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：SSHファイルの編集機能
    //=========================================================================================
    public partial class RenameSSHDialog : Form, RenameFileInfo.IRenameFileDialog {
        // 編集対象のファイル情報（編集前）
        RenameFileInfo m_renameFileInfoOrg;

        // 編集対照のファイル情報（編集結果）
        RenameFileInfo m_renameFileInfoModified;

        // アクセス時刻の編集を有効にするときtrue
        private bool m_enableAccessTime;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]renameInfo   編集対照のファイル情報
        // 　　　　[in]enableATime  アクセス時刻の編集を有効にするときtrue
        // 戻り値：なし
        //=========================================================================================
        public RenameSSHDialog(RenameFileInfo renameInfo, bool enableATime) {
            InitializeComponent();
            m_renameFileInfoOrg = renameInfo;
            m_enableAccessTime = enableATime;
            this.textBoxFileName.Text = m_renameFileInfoOrg.SSHInfo.FileName;
            this.textBoxFileNameCurrent.Text = m_renameFileInfoOrg.SSHInfo.FileName;
            int permissions = renameInfo.SSHInfo.Permissions;
            int dispPermission = SSHUtils.PermissionOctToDec(permissions);
            this.textBoxAttribute.Text = string.Format("{0:000}", dispPermission);
            SetPermissionCheckBox(permissions);
            this.dateTimeUpdate.CustomFormat = DateTimeFormatter.RENAME_TIME_CUSTOM_FORMAT;
            this.dateTimeUpdate.Value = renameInfo.SSHInfo.ModifiedDate;
            this.dateTimeAccess.CustomFormat = DateTimeFormatter.RENAME_TIME_CUSTOM_FORMAT;
            this.dateTimeAccess.Value = renameInfo.SSHInfo.AccessDate;
            this.dateTimeAccess.Enabled = enableATime;
            this.textBoxOwner.Text = renameInfo.SSHInfo.Owner;
            this.textBoxGroup.Text = renameInfo.SSHInfo.Group;
            this.buttonTimeUpdate.ImageList = UIIconManager.IconImageList;
            this.buttonTimeUpdate.ImageIndex = (int)IconImageListID.ButtonRenameTimeSame4;
            this.buttonTimeUpdate.Enabled = enableATime;
            this.buttonTimeAccess.ImageList = UIIconManager.IconImageList;
            this.buttonTimeAccess.ImageIndex = (int)IconImageListID.ButtonRenameTimeSame5;
            this.buttonTimeAccess.Enabled = enableATime;
            FormUtils.SetCursorPosExtension(this.textBoxFileName);
            this.ActiveControl = this.textBoxFileName;
        }

        //=========================================================================================
        // 機　能：パーミッションの直接入力文字列が正しいかどうかを確認する
        // 引　数：[in]str  確認する文字列
        // 戻り値：パーミッション文字列が正しいときtrue
        //=========================================================================================
        private bool CheckPermissionString(string str) {
            if (str.Length != 3) {
                return false;
            }
            if (str[0] < '0' || '7' < str[0]) {
                return false;
            }
            if (str[1] < '0' || '7' < str[1]) {
                return false;
            }
            if (str[2] < '0' || '7' < str[2]) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：パーミッションの直接入力からチェックボックスを設定する
        // 引　数：[in]permissions  パーミッションの８進数表現
        // 戻り値：なし
        //=========================================================================================
        private void SetPermissionCheckBox(int permissions) {
            this.checkBoxOwnerRead.Checked = ((permissions & FileAttribute.S_IRUSR) != 0);
            this.checkBoxOwnerWrite.Checked = ((permissions & FileAttribute.S_IWUSR) != 0);
            this.checkBoxOwnerExecute.Checked = ((permissions & FileAttribute.S_IXUSR) != 0);
            this.checkBoxGroupRead.Checked = ((permissions & FileAttribute.S_IRGRP) != 0);
            this.checkBoxGroupWrite.Checked = ((permissions & FileAttribute.S_IWGRP) != 0);
            this.checkBoxGroupExecute.Checked = ((permissions & FileAttribute.S_IXGRP) != 0);
            this.checkBoxOtherRead.Checked = ((permissions & FileAttribute.S_IROTH) != 0);
            this.checkBoxOtherWrite.Checked = ((permissions & FileAttribute.S_IWOTH) != 0);
            this.checkBoxOtherExecute.Checked = ((permissions & FileAttribute.S_IXOTH) != 0);
        }

        //=========================================================================================
        // 機　能：チェックボックスの状態からパーミッションの８進数表現を取得する
        // 引　数：なし
        // 戻り値：パーミッションの８進数表現
        //=========================================================================================
        private int GetAttributeIntOct() {
            int attr = 0;
            if (this.checkBoxOwnerRead.Checked) {
                attr |= FileAttribute.S_IRUSR;
            }
            if (this.checkBoxOwnerWrite.Checked) {
                attr |= FileAttribute.S_IWUSR;
            }
            if (this.checkBoxOwnerExecute.Checked) {
                attr |= FileAttribute.S_IXUSR;
            }
            if (this.checkBoxGroupRead.Checked) {
                attr |= FileAttribute.S_IRGRP;
            }
            if (this.checkBoxGroupWrite.Checked) {
                attr |= FileAttribute.S_IWGRP;
            }
            if (this.checkBoxGroupExecute.Checked) {
                attr |= FileAttribute.S_IXGRP;
            }
            if (this.checkBoxOtherRead.Checked) {
                attr |= FileAttribute.S_IROTH;
            }
            if (this.checkBoxOtherWrite.Checked) {
                attr |= FileAttribute.S_IWOTH;
            }
            if (this.checkBoxOtherExecute.Checked) {
                attr |= FileAttribute.S_IXOTH;
            }
            return attr;
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
            if (name.Length >= 1) {
                name = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
            } else {
                name = name.ToUpper();
            }
            this.textBoxFileName.Text = name;
        }

        //=========================================================================================
        // 機　能：パーミッションの直接入力から外れたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxAttribute_Leave(object sender, EventArgs evt) {
            if (!CheckPermissionString(this.textBoxAttribute.Text)) {
                checkBoxPermission_CheckedChanged(null, null);
            }
        }

        //=========================================================================================
        // 機　能：パーミッションの直接入力が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxAttribute_TextChanged(object sender, EventArgs evt) {
            if (!this.textBoxAttribute.Focused) {
                return;
            }
            if (!CheckPermissionString(this.textBoxAttribute.Text)) {
                return;
            }
            int permissions = int.Parse(this.textBoxAttribute.Text);
            SetPermissionCheckBox(SSHUtils.PermissionDecToOct(permissions));
        }

        //=========================================================================================
        // 機　能：パーミッションのチェックボックスがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void checkBoxPermission_CheckedChanged(object sender, EventArgs evt) {
            if (sender != null && 
                !this.checkBoxOwnerRead.Focused &&
                !this.checkBoxOwnerWrite.Focused &&
                !this.checkBoxOwnerExecute.Focused &&
                !this.checkBoxGroupRead.Focused &&
                !this.checkBoxGroupWrite.Focused &&
                !this.checkBoxGroupExecute.Focused &&
                !this.checkBoxOtherRead.Focused &&
                !this.checkBoxOtherWrite.Focused &&
                !this.checkBoxOtherExecute.Focused) {
                return;
            }

            int permission = GetAttributeIntOct();
            permission = SSHUtils.PermissionOctToDec(permission);
            this.textBoxAttribute.Text = string.Format("{0:000}", permission);
        }
        
        //=========================================================================================
        // 機　能：更新時刻を他の時刻に反映する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonTimeUpdate_Click(object sender, EventArgs evt) {
            this.dateTimeAccess.Value = this.dateTimeUpdate.Value;
        }

        //=========================================================================================
        // 機　能：アクセス時刻を他の時刻に反映する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonTimeAccess_Click(object sender, EventArgs evt) {
            this.dateTimeUpdate.Value = this.dateTimeAccess.Value;
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
            if (!CheckPermissionString(this.textBoxAttribute.Text)) {
                InfoBox.Warning(this, Resources.DlgRename_Permission);
                return;
            }
            if (this.textBoxOwner.Text.Length == 0) {
                InfoBox.Warning(this, Resources.DlgRename_NoOwner);
                return;
            }
            if (this.textBoxGroup.Text.Length == 0) {
                InfoBox.Warning(this, Resources.DlgRename_NoGroup);
                return;
            }

            RenameFileInfo.SSHRenameInfo sshInfo = new RenameFileInfo.SSHRenameInfo();
            sshInfo.FileName = this.textBoxFileName.Text;
            sshInfo.IsDirectory = m_renameFileInfoOrg.SSHInfo.IsDirectory;
            int permission = SSHUtils.PermissionDecToOct(int.Parse(this.textBoxAttribute.Text));
            sshInfo.Permissions = permission;
            sshInfo.ModifiedDate = this.dateTimeUpdate.Value;
            if (m_enableAccessTime) {
                sshInfo.AccessDate = this.dateTimeAccess.Value;
            } else {
                sshInfo.AccessDate = m_renameFileInfoOrg.SSHInfo.AccessDate;
            }
            sshInfo.Owner = this.textBoxOwner.Text;
            sshInfo.Group = this.textBoxGroup.Text;
            m_renameFileInfoModified = new RenameFileInfo(sshInfo);

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RenameSSHDialog_FormClosing(object sender, FormClosingEventArgs evt) {
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
