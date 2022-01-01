using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Globalization;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：SSH用のファイル名一括編集条件の入力ダイアログ
    //=========================================================================================
    public partial class RenameSelectedFilesSSHDialog : Form, RenameSelectedFileInfo.IRenameSelectedFileDialog {
        // 編集結果
        private RenameSelectedFileInfo m_renameSelectedFileInfo;

        // ファイル一覧
        private UIFileList m_fileList;

        // 共通部分の実装
        private RenameSelectedFilesWindowsDialog.CommonImpl m_commonImpl;

        // アクセス日時の更新を有効にするときtrue
        private bool m_enableAccessTime;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileList          更新しようとしているファイル一覧（ファイル名重複の可能性を確認）
        // 　　　　[in]enableAccessTime  アクセス日時の更新を有効にするときtrue
        // 戻り値：なし
        //=========================================================================================
        public RenameSelectedFilesSSHDialog(UIFileList fileList, bool enableAccessTime) {
            InitializeComponent();
            m_fileList = fileList;
            m_enableAccessTime = enableAccessTime;

            // 実装
            m_commonImpl = new RenameSelectedFilesWindowsDialog.CommonImpl(this, this.comboBoxNameBody, this.comboBoxNameExt, this.comboBoxFolder);

            // 日時
            this.dateTimeDateUpdate.CustomFormat = DateTimeFormatter.RENAME_DATE_CUSTOM_FORMAT;

            // その他
            EnableUIItem();
            SetAttributeString();
        }

        //=========================================================================================
        // 機　能：UIの有効/無効状態を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            // 更新日時
            this.dateTimeDateUpdate.Enabled = this.checkBoxDateUpdate.Checked;
            this.textBoxTimeUpdate.Enabled = this.checkBoxTimeUpdate.Checked;
            
            // アクセス日時
            if (m_enableAccessTime) {
                this.dateTimeDateAccess.Enabled = this.checkBoxDateAccess.Checked;
                this.textBoxTimeAccess.Enabled = this.checkBoxTimeAccess.Checked;
            } else {
                this.dateTimeDateAccess.Enabled = false;
                this.textBoxTimeAccess.Enabled = false;
                this.checkBoxDateAccess.Enabled = false;
                this.checkBoxTimeAccess.Enabled = false;
            }

            // オーナー
            this.textBoxOwner.Enabled = this.checkBoxOwner.Checked;
            this.textBoxGroup.Enabled = this.checkBoxGroup.Checked;
        }

        //=========================================================================================
        // 機　能：現在時刻の設定ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCurrent_Click(object sender, EventArgs evt) {
            DateTime current = DateTime.Now;
            string time = string.Format("{0:00}{1:00}{2:00}", current.Hour, current.Minute, current.Second);

            this.dateTimeDateUpdate.Value = current;
            this.textBoxTimeUpdate.Text = time;
            this.checkBoxDateUpdate.Checked = true;
            this.checkBoxTimeUpdate.Checked = true;

            if (m_enableAccessTime) {
                this.dateTimeDateAccess.Value = current;
                this.textBoxTimeAccess.Text = time;
                this.checkBoxDateAccess.Checked = true;
                this.checkBoxTimeAccess.Checked = true;
            }
        }

        //=========================================================================================
        // 機　能：本日正午の設定ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNoon_Click(object sender, EventArgs evt) {
            DateTime current = DateTime.Now;
            string time = "120000";

            this.dateTimeDateUpdate.Value = current;
            this.textBoxTimeUpdate.Text = time;
            this.checkBoxDateUpdate.Checked = true;
            this.checkBoxTimeUpdate.Checked = true;

            if (m_enableAccessTime) {
                this.dateTimeDateAccess.Value = current;
                this.textBoxTimeAccess.Text = time;
                this.checkBoxDateAccess.Checked = true;
                this.checkBoxTimeAccess.Checked = true;
            }
        }

        //=========================================================================================
        // 機　能：ファイル名の大文字化ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNameUpper_Click(object sender, EventArgs evt) {
            m_commonImpl.SelectToUpper();
        }

        //=========================================================================================
        // 機　能：ファイル名の小文字化ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNameLower_Click(object sender, EventArgs evt) {
            m_commonImpl.SelectToLower();
        }

        //=========================================================================================
        // 機　能：ファイル名の連番指定ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonSequential_Click(object sender, EventArgs evt) {
            m_commonImpl.SelectSequential();
        }

        //=========================================================================================
        // 機　能：日時設定のチェックボックスの状態が変わったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void CheckBoxDateTime_CheckedChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：属性のチェックボックスの状態が変わったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void checkBoxAttribute_CheckedChanged(object sender, EventArgs evt) {
            SetAttributeString();
        }

        //=========================================================================================
        // 機　能：チェックボックスの状態から直接入力の属性文字列を作成してUIに反映する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void SetAttributeString() {
            string textAttr = GetAttributeMaskString(this.checkBoxOwnerRead, this.checkBoxOwnerWrite, this.checkBoxOwnerExecute) +
                              GetAttributeMaskString(this.checkBoxGroupRead, this.checkBoxGroupWrite, this.checkBoxGroupExecute) +
                              GetAttributeMaskString(this.checkBoxOtherRead, this.checkBoxOtherWrite, this.checkBoxOtherExecute);
            this.textBoxAttirbute.Text = textAttr;
        }

        //=========================================================================================
        // 機　能：チェックボックス(Read/Warite/Execute)から、直接入力の8進数1桁の文字列を作成する
        // 引　数：[in]checkBoxRead     Readのチェックボックス
        // 　　　　[in]checkBoxWrite    Writeのチェックボックス
        // 　　　　[in]checkBoxExecute  Executeのチェックボックス
        // 戻り値：直接入力の8進数1桁、チェックボックスに中間状態があるときは「_」
        //=========================================================================================
        private string GetAttributeMaskString(CheckBox checkBoxRead, CheckBox checkBoxWrite, CheckBox checkBoxExecute) {
            string textAttr;
            if (checkBoxRead.CheckState == CheckState.Indeterminate || checkBoxWrite.CheckState == CheckState.Indeterminate || checkBoxExecute.CheckState == CheckState.Indeterminate) {
                textAttr = "_";
            } else {
                int attr = 0;
                if (checkBoxRead.CheckState == CheckState.Checked) {
                    attr |= 4;
                }
                if (checkBoxWrite.CheckState == CheckState.Checked) {
                    attr |= 2;
                }
                if (checkBoxExecute.CheckState == CheckState.Checked) {
                    attr |= 1;
                }
                textAttr = attr.ToString();
            }
            return textAttr;
        }

        //=========================================================================================
        // 機　能：属性の直接入力のテキストボックスからフォーカスが外れたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxAttirbute_Leave(object sender, EventArgs evt) {
            SetAttributeString();
        }
        
        //=========================================================================================
        // 機　能：属性の直接入力のテキストボックスからフォーカスが外れたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxAttirbute_TextChanged(object sender, EventArgs evt) {
            string textAttr = this.textBoxAttirbute.Text;
            if (textAttr.Length >= 1 && '0' <= textAttr[0] && textAttr[0] <= '7') {
                SetAttributeFromMaskString(textAttr[0], this.checkBoxOwnerRead, this.checkBoxOwnerWrite, this.checkBoxOwnerExecute);
            }
            if (textAttr.Length >= 2 && '0' <= textAttr[1] && textAttr[1] <= '7') {
                SetAttributeFromMaskString(textAttr[1], this.checkBoxGroupRead, this.checkBoxGroupWrite, this.checkBoxGroupExecute);
            }
            if (textAttr.Length >= 3 && '0' <= textAttr[2] && textAttr[2] <= '7') {
                SetAttributeFromMaskString(textAttr[2], this.checkBoxOtherRead, this.checkBoxOtherWrite, this.checkBoxOtherExecute);
            }
        }

        //=========================================================================================
        // 機　能：属性の直接入力の文字をUIに反映する
        // 引　数：[in]attrChar   属性の直接入力の文字（'0'～'7'）
        // 　　　　[in]checkBoxRead     Readのチェックボックス
        // 　　　　[in]checkBoxWrite    Writeのチェックボックス
        // 　　　　[in]checkBoxExecute  Executeのチェックボックス
        // 戻り値：なし
        //=========================================================================================
        private void SetAttributeFromMaskString(char attrChar, CheckBox checkBoxRead, CheckBox checkBoxWrite, CheckBox checkBoxExecute) {
            int attr = attrChar - '0';
            if ((attr & 4) != 0) {
                checkBoxRead.CheckState = CheckState.Checked;
            } else {
                checkBoxRead.CheckState = CheckState.Unchecked;
            }
            if ((attr & 2) != 0) {
                checkBoxWrite.CheckState = CheckState.Checked;
            } else {
                checkBoxWrite.CheckState = CheckState.Unchecked;
            }
            if ((attr & 1) != 0) {
                checkBoxExecute.CheckState = CheckState.Checked;
            } else {
                checkBoxExecute.CheckState = CheckState.Unchecked;
            }
        }

        //=========================================================================================
        // 機　能：オーナーのチェックボックスの状態が変わったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void checkBoxOwner_CheckedChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_renameSelectedFileInfo = null;
            RenameSelectedFileInfo.SSHRenameInfo renameInfo = new RenameSelectedFileInfo.SSHRenameInfo();

            // ファイル名
            RenameSelectedFileInfo.ModifyFileNameInfo fileNameInfo = m_commonImpl.GetFileNameInfo();
            renameInfo.ModifyFileNameInfo = fileNameInfo;

            // 属性
            renameInfo.AttributeOwnerRead    = RenameSelectedFilesWindowsDialog.CommonImpl.GetAttributeFromUI(this.checkBoxOwnerRead);
            renameInfo.AttributeOwnerWrite   = RenameSelectedFilesWindowsDialog.CommonImpl.GetAttributeFromUI(this.checkBoxOwnerWrite);
            renameInfo.AttributeOwnerExecute = RenameSelectedFilesWindowsDialog.CommonImpl.GetAttributeFromUI(this.checkBoxOwnerExecute);
            renameInfo.AttributeGroupRead    = RenameSelectedFilesWindowsDialog.CommonImpl.GetAttributeFromUI(this.checkBoxGroupRead);
            renameInfo.AttributeGroupWrite   = RenameSelectedFilesWindowsDialog.CommonImpl.GetAttributeFromUI(this.checkBoxGroupWrite);
            renameInfo.AttributeGroupExecute = RenameSelectedFilesWindowsDialog.CommonImpl.GetAttributeFromUI(this.checkBoxGroupExecute);
            renameInfo.AttributeOtherRead    = RenameSelectedFilesWindowsDialog.CommonImpl.GetAttributeFromUI(this.checkBoxOtherRead);
            renameInfo.AttributeOtherWrite   = RenameSelectedFilesWindowsDialog.CommonImpl.GetAttributeFromUI(this.checkBoxOtherWrite);
            renameInfo.AttributeOtherExecute = RenameSelectedFilesWindowsDialog.CommonImpl.GetAttributeFromUI(this.checkBoxOtherExecute);

            // 更新日時
            bool success;
            DateInfo dateInfo;
            TimeInfo timeInfo;

            success = RenameSelectedFilesWindowsDialog.CommonImpl.GetTimestamp(this.checkBoxDateUpdate, this.checkBoxTimeUpdate, this.dateTimeDateUpdate, this.textBoxTimeUpdate, out dateInfo, out timeInfo);
            if (!success) {
                InfoBox.Warning(this, Resources.DlgRenameSel_ErrorUpdateTime);
                return;
            }
            renameInfo.UpdateDate = dateInfo;
            renameInfo.UpdateTime = timeInfo;
            
            if (m_enableAccessTime) {
                success = RenameSelectedFilesWindowsDialog.CommonImpl.GetTimestamp(this.checkBoxDateAccess, this.checkBoxTimeAccess, this.dateTimeDateAccess, this.textBoxTimeAccess, out dateInfo, out timeInfo);
                if (!success) {
                    InfoBox.Warning(this, Resources.DlgRenameSel_ErrorAccessTime);
                    return;
                }
                renameInfo.AccessDate = dateInfo;
                renameInfo.AccessTime = timeInfo;
            } else {
                renameInfo.AccessDate = null;
                renameInfo.AccessTime = null;
            }

            // フォルダの処理方法
            renameInfo.TargetFolder = m_commonImpl.GetTargetFolder();

            // オーナー
            if (this.checkBoxOwner.Checked) {
                if (this.textBoxOwner.Text == "") {
                    InfoBox.Warning(this, Resources.DlgRenameSel_ErrorOwnerUser);
                    return;
                }
                renameInfo.Owner = this.textBoxOwner.Text;
            } else {
                renameInfo.Owner = null;
            }
            if (this.checkBoxGroup.Checked) {
                if (this.textBoxGroup.Text == "") {
                    InfoBox.Warning(this, Resources.DlgRenameSel_ErrorOwnerGroup);
                    return;
                }
                renameInfo.Group = this.textBoxGroup.Text;
            } else {
                renameInfo.Group = null;
            }

            // 組み合わせを確認
            success = RenameSelectedFilesWindowsDialog.CommonImpl.CheckRenameAndFolder(renameInfo.ModifyFileNameInfo.RenameModeFileBody, renameInfo.ModifyFileNameInfo.RenameModeFileExt, renameInfo.TargetFolder);
            if (!success) {
                InfoBox.Warning(this, Resources.DlgRenameSel_ErrorTargetFolder);
                return;
            }
            success = RenameSelectedFilesWindowsDialog.CommonImpl.CheckFileName(m_fileList, renameInfo.ModifyFileNameInfo);
            if (!success) {
                DialogResult yesNo = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgRenameSel_ErrorDuplicateFile);
                if (yesNo != DialogResult.Yes) {
                    return;
                }
            }

            m_renameSelectedFileInfo = new RenameSelectedFileInfo(renameInfo);
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RenameSelectedFilesSSHDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && m_renameSelectedFileInfo == null) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // 機　能：編集ダイアログを表示する
        // 引　数：[in]parent  親ウィンドウ
        // 戻り値：ダイアログの結果
        //=========================================================================================
        public DialogResult ShowRenameDialog(Form parent) {
            return ShowDialog(parent);
        }

        //=========================================================================================
        // プロパティ：編集対象のリネーム情報（編集結果）
        //=========================================================================================
        public RenameSelectedFileInfo RenameSelectedFileInfo {
            get {
                return m_renameSelectedFileInfo;
            }
        }
    }
}
