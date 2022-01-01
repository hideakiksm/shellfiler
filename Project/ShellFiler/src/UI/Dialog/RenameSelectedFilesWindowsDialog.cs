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
    // クラス：Windows用のファイル名一括編集条件の入力ダイアログ
    //=========================================================================================
    public partial class RenameSelectedFilesWindowsDialog : Form, RenameSelectedFileInfo.IRenameSelectedFileDialog {
        // 編集結果
        private RenameSelectedFileInfo m_renameSelectedFileInfo;

        // ファイル一覧
        private UIFileList m_fileList;

        // 共通部分の実装
        private CommonImpl m_commonImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileList  更新しようとしているファイル一覧（ファイル名重複の可能性を確認）
        // 戻り値：なし
        //=========================================================================================
        public RenameSelectedFilesWindowsDialog(UIFileList fileList) {
            InitializeComponent();
            m_fileList = fileList;

            // 実装
            m_commonImpl = new CommonImpl(this, this.comboBoxNameBody, this.comboBoxNameExt, this.comboBoxFolder);

            // 日時
            this.buttonTimeUpdate.ImageList = UIIconManager.IconImageList;
            this.buttonTimeUpdate.ImageIndex = (int)IconImageListID.ButtonRenameTimeSame1;
            this.buttonTimeCreate.ImageList = UIIconManager.IconImageList;
            this.buttonTimeCreate.ImageIndex = (int)IconImageListID.ButtonRenameTimeSame2;
            this.buttonTimeAccess.ImageList = UIIconManager.IconImageList;
            this.buttonTimeAccess.ImageIndex = (int)IconImageListID.ButtonRenameTimeSame3;
            this.dateTimeDateUpdate.CustomFormat = DateTimeFormatter.RENAME_DATE_CUSTOM_FORMAT;
            this.dateTimeDateCreate.CustomFormat = DateTimeFormatter.RENAME_DATE_CUSTOM_FORMAT;
            this.dateTimeDateAccess.CustomFormat = DateTimeFormatter.RENAME_DATE_CUSTOM_FORMAT;

            // その他
            EnableUIItem();
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
            this.buttonTimeUpdate.Enabled = (this.checkBoxDateUpdate.Checked || this.checkBoxTimeUpdate.Checked);
            
            // 作成日時
            this.dateTimeDateCreate.Enabled = this.checkBoxDateCreate.Checked;
            this.textBoxTimeCreate.Enabled = this.checkBoxTimeCreate.Checked;
            this.buttonTimeCreate.Enabled = (this.checkBoxDateCreate.Checked || this.checkBoxTimeCreate.Checked);

            // アクセス日時
            this.dateTimeDateAccess.Enabled = this.checkBoxDateAccess.Checked;
            this.textBoxTimeAccess.Enabled = this.checkBoxTimeAccess.Checked;
            this.buttonTimeAccess.Enabled = (this.checkBoxDateAccess.Checked || this.checkBoxTimeAccess.Checked);
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
            this.dateTimeDateCreate.Value = current;
            this.textBoxTimeCreate.Text = time;
            this.dateTimeDateAccess.Value = current;
            this.textBoxTimeAccess.Text = time;

            this.checkBoxDateUpdate.Checked = true;
            this.checkBoxDateCreate.Checked = true;
            this.checkBoxDateAccess.Checked = true;
            this.checkBoxTimeUpdate.Checked = true;
            this.checkBoxTimeCreate.Checked = true;
            this.checkBoxTimeAccess.Checked = true;
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
            this.dateTimeDateCreate.Value = current;
            this.textBoxTimeCreate.Text = time;
            this.dateTimeDateAccess.Value = current;
            this.textBoxTimeAccess.Text = time;

            this.checkBoxDateUpdate.Checked = true;
            this.checkBoxDateCreate.Checked = true;
            this.checkBoxDateAccess.Checked = true;
            this.checkBoxTimeUpdate.Checked = true;
            this.checkBoxTimeCreate.Checked = true;
            this.checkBoxTimeAccess.Checked = true;
        }

        //=========================================================================================
        // 機　能：更新時刻からの反映ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonTimeUpdate_Click(object sender, EventArgs evt) {
            if (this.checkBoxDateUpdate.Checked) {
                this.checkBoxDateCreate.Checked = true;
                this.checkBoxDateAccess.Checked = true;
                DateTime value = this.dateTimeDateUpdate.Value;
                this.dateTimeDateCreate.Value = value;
                this.dateTimeDateAccess.Value = value;
            }
            if (this.checkBoxTimeUpdate.Checked) {
                this.checkBoxTimeCreate.Checked = true;
                this.checkBoxTimeAccess.Checked = true;
                string value = this.textBoxTimeUpdate.Text;
                this.textBoxTimeCreate.Text = value;
                this.textBoxTimeAccess.Text = value;
            }
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：作成時刻からの反映ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonTimeCreate_Click(object sender, EventArgs evt) {
            if (this.checkBoxDateCreate.Checked) {
                this.checkBoxDateUpdate.Checked = true;
                this.checkBoxDateAccess.Checked = true;
                DateTime value = this.dateTimeDateCreate.Value;
                this.dateTimeDateUpdate.Value = value;
                this.dateTimeDateAccess.Value = value;
            }
            if (this.checkBoxTimeCreate.Checked) {
                this.checkBoxTimeUpdate.Checked = true;
                this.checkBoxTimeAccess.Checked = true;
                string value = this.textBoxTimeCreate.Text;
                this.textBoxTimeUpdate.Text = value;
                this.textBoxTimeAccess.Text = value;
            }
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：アクセス時刻からの反映ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonTimeAccess_Click(object sender, EventArgs evt) {
            if (this.checkBoxDateAccess.Checked) {
                this.checkBoxDateUpdate.Checked = true;
                this.checkBoxDateCreate.Checked = true;
                DateTime value = this.dateTimeDateAccess.Value;
                this.dateTimeDateUpdate.Value = value;
                this.dateTimeDateCreate.Value = value;
            }
            if (this.checkBoxTimeAccess.Checked) {
                this.checkBoxTimeUpdate.Checked = true;
                this.checkBoxTimeCreate.Checked = true;
                string value = this.textBoxTimeAccess.Text;
                this.textBoxTimeUpdate.Text = value;
                this.textBoxTimeCreate.Text = value;
            }
            EnableUIItem();
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
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_renameSelectedFileInfo = null;
            RenameSelectedFileInfo.WindowsRenameInfo renameInfo = new RenameSelectedFileInfo.WindowsRenameInfo();

            // ファイル名
            RenameSelectedFileInfo.ModifyFileNameInfo fileNameInfo = m_commonImpl.GetFileNameInfo();
            renameInfo.ModifyFileNameInfo = fileNameInfo;

            // 属性
            renameInfo.AttributeReadonly = CommonImpl.GetAttributeFromUI(this.checkBoxReadOnly);
            renameInfo.AttributeHidden = CommonImpl.GetAttributeFromUI(this.checkBoxHidden);
            renameInfo.AttributeArchive = CommonImpl.GetAttributeFromUI(this.checkBoxArchive);
            renameInfo.AttributeSystem = CommonImpl.GetAttributeFromUI(this.checkBoxSystem);

            // 更新日時
            bool success;
            DateInfo dateInfo;
            TimeInfo timeInfo;

            success = CommonImpl.GetTimestamp(this.checkBoxDateUpdate, this.checkBoxTimeUpdate, this.dateTimeDateUpdate, this.textBoxTimeUpdate, out dateInfo, out timeInfo);
            if (!success) {
                InfoBox.Warning(this, Resources.DlgRenameSel_ErrorUpdateTime);
                return;
            }
            renameInfo.UpdateDate = dateInfo;
            renameInfo.UpdateTime = timeInfo;

            success = CommonImpl.GetTimestamp(this.checkBoxDateCreate, this.checkBoxTimeCreate, this.dateTimeDateCreate, this.textBoxTimeCreate, out dateInfo, out timeInfo);
            if (!success) {
                InfoBox.Warning(this, Resources.DlgRenameSel_ErrorCreateTime);
                return;
            }
            renameInfo.CreateDate = dateInfo;
            renameInfo.CreateTime = timeInfo;

            success = CommonImpl.GetTimestamp(this.checkBoxDateAccess, this.checkBoxTimeAccess, this.dateTimeDateAccess, this.textBoxTimeAccess, out dateInfo, out timeInfo);
            if (!success) {
                InfoBox.Warning(this, Resources.DlgRenameSel_ErrorAccessTime);
                return;
            }
            renameInfo.AccessDate = dateInfo;
            renameInfo.AccessTime = timeInfo;
            
            // フォルダの処理方法
            renameInfo.TargetFolder = m_commonImpl.GetTargetFolder();

            // 組み合わせを確認
            success = CommonImpl.CheckRenameAndFolder(renameInfo.ModifyFileNameInfo.RenameModeFileBody, renameInfo.ModifyFileNameInfo.RenameModeFileExt, renameInfo.TargetFolder);
            if (!success) {
                InfoBox.Warning(this, Resources.DlgRenameSel_ErrorTargetFolder);
                return;
            }
            success = CommonImpl.CheckFileName(m_fileList, renameInfo.ModifyFileNameInfo);
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
        private void RenameSelectedFilesWindowsDialog_FormClosing(object sender, FormClosingEventArgs evt) {
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

        //=========================================================================================
        // クラス：WindowsとSSHで共通のリネーム処理の実装
        //=========================================================================================
        public class CommonImpl {
            // ファイル名/拡張子のコンボボックス項目
            private const int COMBOBOX_FILENAME_NONE = 0;                   // そのまま
            private const int COMBOBOX_FILENAME_TO_UPPER = 1;               // 大文字に変更
            private const int COMBOBOX_FILENAME_TO_LOWER = 2;               // 小文字に変更
            private const int COMBOBOX_FILENAME_TO_CAPITAL = 3;             // 先頭だけ大文字に変更
            private const int COMBOBOX_FILENAME_SEQUENCE_SPECIFY = 4;       // 連番の指定/指定の拡張子

            // フォルダの扱いコンボボックス項目
            private const int COMBOBOX_FOLDER_EXCLUDE_SUBFOLDER = 0;        // フォルダ自身だけ
            private const int COMBOBOX_FOLDER_SUBFOLDER_FILE_ONLY = 1;      // サブフォルダのファイルだけ
            private const int COMBOBOX_FOLDER_SUBFOLDER_AND_FOLDER = 2;     // フォルダ自身とサブフォルダの両方
            
            // 所有フォーム
            private Form m_parent;

            // ファイル名主部の入力用コンボボックス
            private ComboBox m_comboBoxNameBody;

            // ファイル名拡張子の入力用コンボボックス
            private ComboBox m_comboBoxNameExt;

            // フォルダの扱いの入力用コンボボックス
            private ComboBox m_comboBoxFolder;

            // コンボボックスの項目を更新中のときtrue
            private bool m_updateCombobox = false;

            // 編集中の連番の設定
            private RenameNumberingInfo m_numberingInfo;

            // 編集中の拡張子の設定
            private string m_extension = RenameSelectedFileInfo.DEFAULT_EXTENSION;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  親ウィンドウ
            // 　　　　[in]comboBoxNameBody  ファイル名主部の入力用コンボボックス
            // 　　　　[in]comboBoxNameExt   ファイル名拡張子の入力用コンボボックス
            // 　　　　[in]comboBoxFolder    フォルダの扱いの入力用コンボボックス
            // 戻り値：なし
            //=========================================================================================
            public CommonImpl(Form parent, ComboBox comboBoxNameBody, ComboBox comoboBoxNameExt, ComboBox comboBoxFolder) {
                m_parent = parent;
                m_comboBoxNameBody = comboBoxNameBody;
                m_comboBoxNameExt = comoboBoxNameExt;
                m_comboBoxFolder = comboBoxFolder;
                m_numberingInfo = RenameNumberingInfo.DefaultRenameSequenceUI();

                // ファイル名主部
                string[] nameBodyList = {
                    Resources.DlgRenameSel_FileNameNone,
                    Resources.DlgRenameSel_FileNameToUpper,
                    Resources.DlgRenameSel_FileNameToLower,
                    Resources.DlgRenameSel_FileNameToCapital,
                    ""
                };
                m_comboBoxNameBody.Items.AddRange(nameBodyList);
                m_comboBoxNameBody.SelectedIndex = COMBOBOX_FILENAME_NONE;
                m_comboBoxNameBody.SelectedIndexChanged += new EventHandler(comboBoxNameBody_SelectedIndexChanged);

                // ファイル名拡張子
                string[] nameExtList = {
                    Resources.DlgRenameSel_FileNameNone,
                    Resources.DlgRenameSel_FileNameToUpper,
                    Resources.DlgRenameSel_FileNameToLower,
                    Resources.DlgRenameSel_FileNameToCapital,
                    ""
                };
                m_comboBoxNameExt.Items.AddRange(nameExtList);
                m_comboBoxNameExt.SelectedIndex = COMBOBOX_FILENAME_NONE;
                m_comboBoxNameExt.SelectedIndexChanged += new EventHandler(comboBoxNameExt_SelectedIndexChanged);

                // フォルダの扱い
                string[] folderSelList = {
                    Resources.DlgRenameSel_FolderExcludeSubfolder,
                    Resources.DlgRenameSel_FolderSubfolderFileOnly,
                    Resources.DlgRenameSel_FolderSubfolderAndFolder
                };
                m_comboBoxFolder.Items.AddRange(folderSelList);
                m_comboBoxFolder.SelectedIndex = COMBOBOX_FOLDER_EXCLUDE_SUBFOLDER;

                UpdateComboboxNameBodySequence();
                UpdateComboboxNameExtension();
            }

            //=========================================================================================
            // 機　能：コンボボックスのファイル名主部の表示項目を更新する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void UpdateComboboxNameBodySequence() {
                ModifyFileInfoContext modifyCtx = new ModifyFileInfoContext();
                string fileName = RenameNumberingInfo.CreateSequenceString(m_numberingInfo, modifyCtx);
                string item = string.Format(Resources.DlgRenameSel_FileNameSequence, fileName);
                int index = m_comboBoxNameBody.SelectedIndex;
                m_comboBoxNameBody.Items.RemoveAt(COMBOBOX_FILENAME_SEQUENCE_SPECIFY);
                m_comboBoxNameBody.Items.Insert(COMBOBOX_FILENAME_SEQUENCE_SPECIFY, item);
                m_comboBoxNameBody.SelectedIndex = index;
            }

            //=========================================================================================
            // 機　能：コンボボックスのファイル名拡張子の表示項目を更新する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void UpdateComboboxNameExtension() {
                string item = string.Format(Resources.DlgRenameSel_FileNameSpecify, m_extension);
                int index = m_comboBoxNameExt.SelectedIndex;
                m_comboBoxNameExt.Items.RemoveAt(COMBOBOX_FILENAME_SEQUENCE_SPECIFY);
                m_comboBoxNameExt.Items.Insert(COMBOBOX_FILENAME_SEQUENCE_SPECIFY, item);
                m_comboBoxNameExt.SelectedIndex = index;
            }

            //=========================================================================================
            // 機　能：コンボボックスから大文字に変換の項目を選択する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void SelectToUpper() {
                m_comboBoxNameBody.SelectedIndex = COMBOBOX_FILENAME_TO_UPPER;
                m_comboBoxNameExt.SelectedIndex = COMBOBOX_FILENAME_TO_UPPER;
            }

            //=========================================================================================
            // 機　能：コンボボックスから小文字に変換の項目を選択する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void SelectToLower() {
                m_comboBoxNameBody.SelectedIndex = COMBOBOX_FILENAME_TO_LOWER;
                m_comboBoxNameExt.SelectedIndex = COMBOBOX_FILENAME_TO_LOWER;
            }

            //=========================================================================================
            // 機　能：コンボボックスから連番指定の項目を選択する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void SelectSequential() {
                m_comboBoxNameBody.SelectedIndex = COMBOBOX_FILENAME_NONE;
                m_comboBoxNameBody.SelectedIndex = COMBOBOX_FILENAME_SEQUENCE_SPECIFY;
            }

            //=========================================================================================
            // 機　能：ファイル名主部の選択項目が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void comboBoxNameBody_SelectedIndexChanged(object sender, EventArgs evt) {
                if (m_updateCombobox) {
                    return;
                }
                m_updateCombobox = true;
                try {
                    if (m_comboBoxNameBody.SelectedIndex == COMBOBOX_FILENAME_SEQUENCE_SPECIFY) {
                        // 連番の設定
                        RenameSelectedSequenceDialog dialog = new RenameSelectedSequenceDialog(m_numberingInfo);
                        DialogResult result = dialog.ShowDialog(m_parent);
                        if (result != DialogResult.OK) {
                            m_comboBoxNameBody.SelectedIndex = 0;
                            return;
                        }
                        m_numberingInfo = dialog.NumberingInfo;
                        UpdateComboboxNameBodySequence();
                    }
                } finally {
                    m_updateCombobox = false;
                }
            }

            //=========================================================================================
            // 機　能：ファイル名拡張子の選択項目が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void comboBoxNameExt_SelectedIndexChanged(object sender, EventArgs evt) {
                if (m_updateCombobox) {
                    return;
                }
                m_updateCombobox = true;
                try {
                    if (m_comboBoxNameExt.SelectedIndex == COMBOBOX_FILENAME_SEQUENCE_SPECIFY) {
                        // 指定の拡張子
                        RenameSelectedExtensionDialog dialog = new RenameSelectedExtensionDialog(m_extension);
                        DialogResult result = dialog.ShowDialog(m_parent);
                        if (result != DialogResult.OK) {
                            m_comboBoxNameExt.SelectedIndex = 0;
                            return;
                        }
                        m_extension = dialog.Extension;
                        UpdateComboboxNameExtension();
                    }
                } finally {
                    m_updateCombobox = false;
                }
            }

            //=========================================================================================
            // 機　能：UIの入力値からファイル名の変更情報を作成して返す
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public RenameSelectedFileInfo.ModifyFileNameInfo GetFileNameInfo() {
                RenameSelectedFileInfo.ModifyFileNameInfo fileNameInfo = new RenameSelectedFileInfo.ModifyFileNameInfo();
                RenameSelectedFileInfo.RenameMode[] comboboxItemList = new RenameSelectedFileInfo.RenameMode[] {
                    RenameSelectedFileInfo.RenameMode.None,
                    RenameSelectedFileInfo.RenameMode.ToUpper,
                    RenameSelectedFileInfo.RenameMode.ToLower,
                    RenameSelectedFileInfo.RenameMode.ToCapital,
                    RenameSelectedFileInfo.RenameMode.Specify,
                };
                int indexRenameBody = m_comboBoxNameBody.SelectedIndex;
                int indexRenameExt = m_comboBoxNameExt.SelectedIndex;

                fileNameInfo.RenameModeFileBody = comboboxItemList[indexRenameBody];
                fileNameInfo.RenameModeFileExt = comboboxItemList[indexRenameExt];
                if (fileNameInfo.RenameModeFileBody == RenameSelectedFileInfo.RenameMode.Specify) {
                    fileNameInfo.RenameFileBodyNumbering = m_numberingInfo;
                } else {
                    fileNameInfo.RenameFileBodyNumbering = null;
                }

                if (fileNameInfo.RenameModeFileExt == RenameSelectedFileInfo.RenameMode.Specify) {
                    fileNameInfo.RenameFileExtString = m_extension;
                } else {
                    fileNameInfo.RenameFileExtString = null;
                }

                return fileNameInfo;
            }

            //=========================================================================================
            // 機　能：処理対象でのフォルダの扱いの入力値を返す
            // 引　数：なし
            // 戻り値：フォルダの扱い
            //=========================================================================================
            public RenameSelectedFileInfo.TargetFolder GetTargetFolder() {
                int index = m_comboBoxFolder.SelectedIndex;
                switch (index) {
                    case 0:
                        return RenameSelectedFileInfo.TargetFolder.ExcludeSubfolder;
                    case 1:
                        return RenameSelectedFileInfo.TargetFolder.SubfolderFileOnly;
                    case 2:
                        return RenameSelectedFileInfo.TargetFolder.SubfolderAndFolder;
                }
                return null;
            }

            //=========================================================================================
            // 機　能：３ステートチェックボックスの値を取得する
            // 引　数：[in]checkbox  取得元のチェックボックスのUI
            // 戻り値：チェックボックスの入力値（true/false/null）
            //=========================================================================================
            public static BooleanFlag GetAttributeFromUI(CheckBox checkbox) {
                if (checkbox.CheckState == CheckState.Indeterminate) {
                    return null;
                } else if (checkbox.CheckState == CheckState.Unchecked) {
                    return new BooleanFlag(false);
                } else {
                    return new BooleanFlag(true);
                }
            }

            //=========================================================================================
            // 機　能：タイムスタンプの更新方法を取得する
            // 引　数：[in]checkBoxDate  日付指定のチェックボックスのUI
            // 　　　　[in]checkBoxTime  時刻指定のチェックボックスのUI
            // 　　　　[in]dateTime      日付指定のUI
            // 　　　　[in]textBox       時刻指定のUI
            // 　　　　[out]dateInfo     入力された日付を返す変数
            // 　　　　[out]timeInfo     入力された時刻を返す変数
            // 戻り値：正しく取得できたときtrue
            //=========================================================================================
            public static bool GetTimestamp(CheckBox checkBoxDate, CheckBox checkBoxTime, DateTimePicker dateTime, MaskedTextBox textBox, out DateInfo dateInfo, out TimeInfo timeInfo) {
                // 日付を変換
                if (checkBoxDate.Checked) {
                    DateTime dateValue = dateTime.Value;
                    dateInfo = new DateInfo(dateValue.Year, dateValue.Month, dateValue.Day);
                } else {
                    dateInfo = null;
                }

                // 時刻を変換
                if (checkBoxTime.Checked) {
                    timeInfo = null;
                    string timeValue = textBox.Text;
                    timeInfo = TimeInfo.ParseTimeInfo(timeValue);
                    if (timeInfo == null) {
                        return false;
                    }
                } else {
                    timeInfo = null;
                }
                return true;
            }

            //=========================================================================================
            // 機　能：名前の変更方法とフォルダの扱いの整合性がとれているかどうかを確認する
            // 引　数：[in]fileBody      ファイル名主部の変更方法
            // 　　　　[in]fileExt       ファイル名拡張子の変更方法
            // 　　　　[in]targetFolder  フォルダの扱い
            // 戻り値：正しく取得できたときtrue
            //=========================================================================================
            public static bool CheckRenameAndFolder(RenameSelectedFileInfo.RenameMode fileBody, RenameSelectedFileInfo.RenameMode fileExt, RenameSelectedFileInfo.TargetFolder targetFolder) {
                if (targetFolder.ModifySubfolderFile) {
                    if (fileBody == RenameSelectedFileInfo.RenameMode.Specify || fileExt == RenameSelectedFileInfo.RenameMode.Specify) {
                        return false;
                    }
                }
                return true;
            }

            //=========================================================================================
            // 機　能：ファイル名が正しく変換できるかどうかを確認する
            // 引　数：[in]fileList      現在のファイル一覧
            // 　　　　[in]fileNameInfo  ファイル名の更新方法
            // 戻り値：正しく変換できるときtrue
            //=========================================================================================
            public static bool CheckFileName(UIFileList fileList, RenameSelectedFileInfo.ModifyFileNameInfo fileNameInfo) {
                // 連番/指定指示がない場合は常に成功
                if (fileNameInfo.RenameModeFileBody != RenameSelectedFileInfo.RenameMode.Specify && fileNameInfo.RenameModeFileExt != RenameSelectedFileInfo.RenameMode.Specify) {
                    return true;
                }

                // 処理対象のHashを作成
                List<UIFile> srcFileList = fileList.MarkFiles;
                HashSet<string> srcFileHash = new HashSet<string>();
                foreach (UIFile srcFile in srcFileList) {
                    srcFileHash.Add(srcFile.FileName.ToLower());
                }

                // 変換後のファイルをチェック
                ModifyFileInfoContext modifyCtx = new ModifyFileInfoContext();
                foreach (UIFile srcFile in srcFileList) {
                    string srcFileName = srcFile.FileName.ToLower();
                    string destFileName = RenameSelectedFileInfoBackgroundTask.GetNewFileName(srcFile.FileName, fileNameInfo, modifyCtx).ToLower();
                    if (srcFileName != destFileName && srcFileHash.Contains(destFileName)) {
                        // 変換後のファイルが変換前にもある：重複の可能性
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
