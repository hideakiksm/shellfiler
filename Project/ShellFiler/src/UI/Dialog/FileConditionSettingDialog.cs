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
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Condition;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイル転送条件設定ダイアログ
    //=========================================================================================
    public partial class FileConditionSettingDialog : Form {
        // 編集対象の設定（呼び出し元のClone）
        private FileConditionSetting m_setting;

        // 設定項目のリスト
        private List<FileConditionItem> m_conditionItemList;

        // 転送元のファイルシステム
        private FileSystemID m_fileSystem;

        // 最後に選択されていた項目の表示名
        private string m_lastSelectedItem = "";

        // ツールチップ
        private ToolTip m_toolTipOption = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting       編集対象の設定（呼び出し元のClone）
        // 　　　　[in]fileSystemId  転送元のファイルシステム
        // 　　　　[in]initialName   初期状態で選択されている項目の表示名
        // 戻り値：なし
        //=========================================================================================
        public FileConditionSettingDialog(FileConditionSetting setting, FileSystemID fileSystemId, string initialName) {
            InitializeComponent();
            m_setting = setting;
            m_fileSystem = fileSystemId;

            // ツールチップ
            m_toolTipOption = new ToolTip();
            m_toolTipOption.InitialDelay = 500;
            m_toolTipOption.ReshowDelay = 1000;
            m_toolTipOption.AutoPopDelay = 3000;
            m_toolTipOption.ShowAlways = true;

            m_conditionItemList = setting.GetAllSettingItemUI(fileSystemId);
            string[] items = new string[m_conditionItemList.Count];
            int selectedIndex = 0;
            for (int i = 0; i < m_conditionItemList.Count; i++) {
                items[i] = CreateListBoxDisplayName(m_conditionItemList[i]);
                if (m_conditionItemList[i].DisplayName == initialName) {
                    selectedIndex = i;
                }
            }
            this.listBoxQuickSetting.Items.AddRange(items);
            this.listBoxQuickSetting.SelectedIndex = selectedIndex;

            FileSystemID fileSystemIdOpposite;
            if (FileSystemID.IsWindows(fileSystemId)) {
                fileSystemIdOpposite = FileSystemID.SFTP;
            } else if (FileSystemID.IsSSH(fileSystemId)) {
                fileSystemIdOpposite = FileSystemID.Windows;
            } else {
                fileSystemIdOpposite = FileSystemID.None;
                FileSystemID.NotSupportError(fileSystemId);
            }
            this.labelMessage1.Text = string.Format(this.labelMessage1.Text, fileSystemId.DisplayName);
            this.labelMessage2.Text = string.Format(this.labelMessage2.Text, fileSystemIdOpposite.DisplayName);

            EnableUIItem();

#if FREE_VERSION
            // Freeware版
            this.labelFreeware.Text = Resources.Dlg_FreewareInfo;
            this.labelFreeware.BackColor = Color.LightYellow;
#endif
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            int firstDefinedIndex = 0;
            for (int i = 0; i < m_conditionItemList.Count; i++) {
                if (m_conditionItemList[i].IsDefined()) {
                    firstDefinedIndex = i;
                    break;
                }
            }

            int index = this.listBoxQuickSetting.SelectedIndex;
            this.buttonEdit.Enabled = (index != -1);
            this.buttonDelete.Enabled = (index != -1) && (!(m_conditionItemList[index].IsDefined()));
            this.buttonUp.Enabled = (index != -1) && (index != 0) && (index < firstDefinedIndex);
            this.buttonDown.Enabled = (index != -1) && (index < firstDefinedIndex - 1);
        }

        //=========================================================================================
        // 機　能：リストボックスで選択中の項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxQuickSetting_SelectedIndexChanged(object sender, EventArgs evt) {
            EnableUIItem();

            int index = this.listBoxQuickSetting.SelectedIndex;
            if (index == -1) {
                return;
            }

            // 表示
            Rectangle rectItem = this.listBoxQuickSetting.GetItemRectangle(index);
            Point ptItem = new Point(rectItem.Left + 50, rectItem.Bottom + 16);
            string message = StringUtils.ConbineLine(m_conditionItemList[index].ToDisplayString(), "\r\n");
            m_toolTipOption.Show(message, this.listBoxQuickSetting, ptItem, 3000);
        }

        //=========================================================================================
        // 機　能：追加ボタンがクリックされたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonAdd_Click(object sender, EventArgs evt) {
            InsertNewItem(null);
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：編集ボタンがクリックされたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonEdit_Click(object sender, EventArgs evt) {
            int index = this.listBoxQuickSetting.SelectedIndex;
            if (index == -1) {
                return;
            }

            if (m_conditionItemList[index].IsDefined()) {
                // 定義済み項目の編集は追加扱い
                InsertNewItem(m_conditionItemList[index]);
            } else {
                // 使用中の項目がないか確認
                bool inUse = CheckInUse(m_conditionItemList[index].DisplayName);
                if (inUse) {
                    DialogResult result = InfoBox.Question(this, MessageBoxButtons.OKCancel, Resources.DlgFileCondition_QuickItemEditInUse, m_conditionItemList[index].DisplayName);
                    if(result != DialogResult.OK) {
                        return;
                    }
                }

                // 編集
                EditItem(index);
                this.listBoxQuickSetting.Items[index] = m_conditionItemList[index].DisplayName;
            }
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：指定された表示名の項目がシステム内で使用されているかどうかを返す
        // 引　数：[in]dispName  表示名
        // 戻り値：使用中のときtrue
        //=========================================================================================
        private bool CheckInUse(string dispName) {
            int tabCount = Program.Document.TabPageList.Count;
            for (int i = 0; i < tabCount; i++) {
                TabPageInfo tabInfo = Program.Document.TabPageList.AllList[i];
                if (tabInfo.LeftFileList.FileListFilterMode != null) {
                    bool inUse = CheckInUseFilterMode(dispName, tabInfo.LeftFileList.FileListFilterMode.ConditionList);
                    if (inUse) {
                        return true;
                    }
                }
                if (tabInfo.RightFileList.FileListFilterMode != null) {
                    bool inUse = CheckInUseFilterMode(dispName, tabInfo.RightFileList.FileListFilterMode.ConditionList);
                    if (inUse) {
                        return true;
                    }
                }
            }
            return false;
        }

        //=========================================================================================
        // 機　能：指定された表示名の項目が条件一覧中で使用されているかどうかを返す
        // 引　数：[in]dispName  表示名
        // 　　　　[in]itemList  確認対象の条件一覧
        // 戻り値：使用中のときtrue
        //=========================================================================================
        private bool CheckInUseFilterMode(string dispName, List<FileConditionItem> itemList) {
            for (int i = 0; i < itemList.Count; i++) {
                if (itemList[i].DisplayName != dispName) {
                    continue;
                }
                if (FileSystemID.IsWindows(m_fileSystem) && itemList[i] is FileConditionItemWindows) {
                    return true;
                } else if (FileSystemID.IsSSH(m_fileSystem) && itemList[i] is FileConditionItemSSH) {
                    return true;
                }
            }
            return false;
        }

        //=========================================================================================
        // 機　能：新しい項目を追加する
        // 引　数：[in]setting  追加する際のコピー元になる転送設定（nullのとき新規項目として追加）
        // 戻り値：なし
        //=========================================================================================
        private void InsertNewItem(FileConditionItem setting) {
            // 新しい項目を作成
            FileConditionItem newSetting = FileConditionSetting.CreateConditionItemFrom(setting, m_fileSystem);
            if (newSetting == null) {
                InfoBox.Warning(this, Resources.DlgFileCondition_InsertNotSupportFileSystem);
                return;
            }
            if (setting != null) {
                DialogResult resultInsert = InfoBox.Question(this, MessageBoxButtons.OKCancel, Resources.DlgFileCondition_QuickCannotEditDefinedItem);
                if (resultInsert != DialogResult.OK) {
                    return;
                }
            }
            if (setting != null) {
                newSetting.DisplayName = CreateUniqueName(newSetting.DisplayName);
            } else {
                newSetting.DisplayName = CreateUniqueName(Resources.DlgFileCondition_QuickNewDisplayName);
            }

            // 編集
            if (FileSystemID.IsWindows(m_fileSystem)) {
                // Windows用の項目
                FileConditionWindowsDialog dialog = new FileConditionWindowsDialog((FileConditionItemWindows)newSetting, m_conditionItemList, -1);
                DialogResult result = dialog.ShowDialog(this);
                if (result != DialogResult.OK) {
                    return;
                }
            } else if (FileSystemID.IsSSH(m_fileSystem)) {
                // SSH用の項目
                FileConditionSSHDialog dialog = new FileConditionSSHDialog((FileConditionItemSSH)newSetting, m_conditionItemList, -1);
                DialogResult result = dialog.ShowDialog(this);
                if (result != DialogResult.OK) {
                    return;
                }
            } else {
                FileSystemID.NotSupportError(m_fileSystem);
            }

            // 登録
            for (int i = 0; i < m_conditionItemList.Count; i++) {
                if (m_conditionItemList[i].IsDefined()) {
                    m_conditionItemList.Insert(i, newSetting);
                    this.listBoxQuickSetting.Items.Insert(i, CreateListBoxDisplayName(newSetting));
                    return;
                }
            }
            Program.Abort("定義済み項目が見つからなかったため、編集した設定を追加できませんでした。");
        }

        //=========================================================================================
        // 機　能：既存の項目を編集する
        // 引　数：[in]index   編集対象のクイック設定のインデックス
        // 戻り値：なし
        //=========================================================================================
        private void EditItem(int index) {
            // 編集
            FileConditionItem resultCondition = EditCondition(this, m_conditionItemList, index, m_fileSystem);
            if (resultCondition == null) {
                return;
            }

            // 登録
            m_conditionItemList[index] = resultCondition;
            this.listBoxQuickSetting.Items[index] = CreateListBoxDisplayName(resultCondition);
        }

        //=========================================================================================
        // 機　能：項目を編集する
        // 引　数：[in]parent     親フォーム
        // 　　　　[in]condList   編集対象の条件のリスト
        // 　　　　[in]index      編集対象の中での位置
        // 　　　　[in]fileSystem 対象のファイルシステム
        // 戻り値：編集された条件（キャンセルしたときnull）
        //=========================================================================================
        public static FileConditionItem EditCondition(Form parent, List<FileConditionItem> condList, int index, FileSystemID fileSystem) {
            FileConditionItem newItem = null;
            if (FileSystemID.IsWindows(fileSystem)) {
                // Windows用の項目
                FileConditionItemWindows winCondition = (FileConditionItemWindows)(condList[index].Clone());
                FileConditionWindowsDialog dialog = new FileConditionWindowsDialog(winCondition, condList, index);
                DialogResult result = dialog.ShowDialog(parent);
                if (result != DialogResult.OK) {
                    return null;
                }
                newItem = winCondition;
            } else if (FileSystemID.IsSSH(fileSystem)) {
                // SSH用の項目
                FileConditionItemSSH sshCondition = (FileConditionItemSSH)(condList[index].Clone());
                FileConditionSSHDialog dialog = new FileConditionSSHDialog(sshCondition, condList, index);
                DialogResult result = dialog.ShowDialog(parent);
                if (result != DialogResult.OK) {
                    return null;
                }
                newItem = sshCondition;
            } else {
                FileSystemID.NotSupportError(fileSystem);
            }
            return newItem;
        }

        //=========================================================================================
        // 機　能：条件設定からリストボックス表示用の文字列を作成する
        // 引　数：[in]item  作成する設定
        // 戻り値：表示用の文字列
        //=========================================================================================
        private string CreateListBoxDisplayName(FileConditionItem item) {
            string dispItem;
            if (item.IsDefined()) {
                dispItem = string.Format(Resources.DlgFileCondition_QuickDefinedItem, item.DisplayName);
            } else {
                dispItem = item.DisplayName;
            }
            return dispItem;
        }

        //=========================================================================================
        // 機　能：指定された表示名が設定全体で一意となる名前を作成する
        // 引　数：[in]name   表示名
        // 戻り値：一意となる名前
        //=========================================================================================
        private string CreateUniqueName(string name) {
            string newName = name;
            int suffix = 1;
            while (true) {
                bool found = false;
                for (int i = 0; i < m_conditionItemList.Count; i++) {
                    if (m_conditionItemList[i].DisplayName == newName) {
                        found = true;
                    }
                }
                if (found) {
                    suffix++;
                    newName = name + "(" + suffix + ")";
                } else {
                    break;
                }
            }
            return newName;
        }

        //=========================================================================================
        // 機　能：削除ボタンがクリックされたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            int index = this.listBoxQuickSetting.SelectedIndex;
            if (index == -1) {
                return;
            }
            if (m_conditionItemList[index].IsDefined()) {
                return;
            }
            DialogResult result = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgFileCondition_QuickConfirmDelete, m_conditionItemList[index].DisplayName);
            if (result != DialogResult.Yes) {
                return;
            }
            // 使用中の項目がないか確認
            bool inUse = CheckInUse(m_conditionItemList[index].DisplayName);
            if (inUse) {
                result = InfoBox.Question(this, MessageBoxButtons.OKCancel, Resources.DlgFileCondition_QuickItemDeleteInUse, m_conditionItemList[index].DisplayName);
                if(result != DialogResult.OK) {
                    return;
                }
            }
            m_conditionItemList.RemoveAt(index);
            this.listBoxQuickSetting.Items.RemoveAt(index);
            this.listBoxQuickSetting.SelectedIndex = index;
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：上へボタンがクリックされたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonUp_Click(object sender, EventArgs evt) {
            int index = this.listBoxQuickSetting.SelectedIndex;
            if (index == -1 || index == 0) {
                return;
            }
            if (m_conditionItemList[index].IsDefined()) {
                return;
            }
            FileConditionItem temp = m_conditionItemList[index];
            m_conditionItemList[index] = m_conditionItemList[index - 1];
            m_conditionItemList[index - 1] = temp;
            object tempItem = this.listBoxQuickSetting.Items[index];
            this.listBoxQuickSetting.Items[index] = this.listBoxQuickSetting.Items[index - 1];
            this.listBoxQuickSetting.Items[index - 1] = tempItem;
            this.listBoxQuickSetting.SelectedIndex = index - 1;
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：下へボタンがクリックされたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDown_Click(object sender, EventArgs evt) {
            int index = this.listBoxQuickSetting.SelectedIndex;
            if (index == -1 || index >= this.listBoxQuickSetting.Items.Count - 1) {
                return;
            }
            if (m_conditionItemList[index].IsDefined() || m_conditionItemList[index + 1].IsDefined()) {
                return;
            }
            FileConditionItem temp = m_conditionItemList[index];
            m_conditionItemList[index] = m_conditionItemList[index + 1];
            m_conditionItemList[index + 1] = temp;
            object tempItem = this.listBoxQuickSetting.Items[index];
            this.listBoxQuickSetting.Items[index] = this.listBoxQuickSetting.Items[index + 1];
            this.listBoxQuickSetting.Items[index + 1] = tempItem;
            this.listBoxQuickSetting.SelectedIndex = index + 1;
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (FileSystemID.IsWindows(m_fileSystem)) {
                // Windows
                m_setting.UserSettingWindows.Clear();
                for (int i = 0; i < m_conditionItemList.Count; i++) {
                    if (!m_conditionItemList[i].IsDefined()) {
                        m_setting.UserSettingWindows.Add((FileConditionItemWindows)m_conditionItemList[i]);
                    }
                }
            } else if (FileSystemID.IsSSH(m_fileSystem)) {
                // SSH
                m_setting.UserSettingSSH.Clear();
                for (int i = 0; i < m_conditionItemList.Count; i++) {
                    if (!m_conditionItemList[i].IsDefined()) {
                        m_setting.UserSettingSSH.Add((FileConditionItemSSH)m_conditionItemList[i]);
                    }
                }
            } else if (FileSystemID.IsVirtual(m_fileSystem)) {
                ;
            } else {
                FileSystemID.NotSupportError(m_fileSystem);
            }
            int index = this.listBoxQuickSetting.SelectedIndex;
            m_lastSelectedItem = m_conditionItemList[index].DisplayName;

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：最後に選択されていた項目の表示名
        //=========================================================================================
        public string LastSelectedItem {
            get {
                return m_lastSelectedItem = "";
            }
        }
    }
}