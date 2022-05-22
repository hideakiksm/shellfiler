using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
    // クラス：方式を指定して削除開始時の確認ダイアログ
    //=========================================================================================
    public partial class DeleteExStartDialog : Form {
        // ダイアログ一覧処理の実装
        private DeleteExStartDialog.ListImpl m_listImpl;

        // ダイアログ削除処理の実装
        private DeleteExStartDialog.DeleteImpl m_deleteImpl;

        // 転送条件の入力の実装
        private DeleteExStartDialog.ConditionImpl m_conditionImpl;
        
        // 編集前のファイル転送条件の設定
        private FileConditionSetting m_settingOld;

        // 待機状態のタスクを作成するときtrue
        private bool m_suspededTask;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]delDirect   ごみ箱/rm -rfを使って削除するときtrue
        // 　　　　[in]fileSystem  対象のファイルシステム
        // 戻り値：なし
        //=========================================================================================
        public DeleteExStartDialog(bool delDirect, FileSystemID fileSystem) {
            InitializeComponent();

            m_settingOld = Program.Document.FileConditionSetting;
            m_settingOld.LoadSetting();
            FileConditionSetting setting = (FileConditionSetting)(m_settingOld.Clone());

            m_listImpl = new DeleteExStartDialog.ListImpl(this.listViewTarget, this.labelMessage, Resources.DlgOperationStart_Delete);
            m_deleteImpl = new DeleteExStartDialog.DeleteImpl(delDirect, fileSystem, checkBoxDirectory, checkBoxAttr, checkBoxWithRecycle);
            m_conditionImpl = new DeleteExStartDialog.ConditionImpl(
                        this, setting, setting.TransferConditionDialogInfo, fileSystem,
                        this.checkBoxCondition, this.radioButtonSetting, this.radioButtonWild,
                        this.checkedListCondition, this.buttonSetting,  this.textBoxWildCard);
            this.pictureBoxIcon.Image = UIIconManager.IconImageList.Images[(int)(IconImageListID.FileList_Delete)];
        }

        //=========================================================================================
        // 機　能：ファイル一覧から初期化する
        // 引　数：[in]fileList  確認対象のファイルリスト
        // 戻り値：なし
        //=========================================================================================
        public void InitializeByMarkFile(UIFileList fileList) {
            m_listImpl.InitializeByMarkFile(fileList.MarkFiles);
            m_deleteImpl.InitializeByMarkFile(fileList);
        }

        //=========================================================================================
        // 機　能：再試行情報から初期化する
        // 引　数：[in]retryInfo  確認対象の再試行情報
        // 戻り値：なし
        //=========================================================================================
        public void InitializeByRetryInfo(FileErrorRetryInfo retryInfo) {
            m_listImpl.InitializeByRetryInfo(retryInfo);
            m_deleteImpl.InitializeByRetryInfo(retryInfo);
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_deleteImpl.OnOK();
            bool success = m_conditionImpl.OnOk(FileConditionTarget.FileOnly);
            if (!success) {
                return;
            }
            m_suspededTask = this.checkBoxSuspend.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void DeleteStartDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            if (!FileConditionSetting.EqualsConfig(m_settingOld, m_conditionImpl.FileConditionSetting)) {
                m_conditionImpl.FileConditionSetting.SaveSetting();
                Program.Document.FileConditionSetting = m_conditionImpl.FileConditionSetting;
            }
            m_listImpl.OnFormClosed();
            m_deleteImpl.OnFormClosed();
        }

        //=========================================================================================
        // プロパティ：削除操作のモード
        //=========================================================================================
        public DeleteFileOption DeleteFileOption {
            get {
                return m_deleteImpl.DeleteFileOption;
            }
        }

        //=========================================================================================
        // プロパティ：ごみ箱を使って削除するときtrue
        //=========================================================================================
        public bool DeleteWithRecycle {
            get {
                return m_deleteImpl.DeleteWithRecycle;
            }
        }
        
        //=========================================================================================
        // プロパティ：入力した転送条件（無条件に転送するときnull）
        //=========================================================================================
        public CompareCondition TransferCondition {
            get {
                if (m_conditionImpl.ResultConditionList != null) {
                    return new CompareCondition(m_conditionImpl.ResultConditionList);
                } else {
                    return null;
                }
            }
        }

        //=========================================================================================
        // プロパティ：待機状態のタスクを作成するときtrue
        //=========================================================================================
        public bool SuspededTask {
            get {
                return m_suspededTask;
            }
        }

        //=========================================================================================
        // クラス：ダイアログのファイル一覧の実装
        //=========================================================================================
        public class ListImpl {
            // アイコン用のイメージリスト
            private ImageList m_imageList;

            // アイコンID→イメージリストのインデックスのmap
            private Dictionary<FileIconID, int> m_mapIconIdToImageListIndex = new Dictionary<FileIconID,int>();

            // リストビュー
            private ListView m_listViewTarget;

            // メッセージのラベル（設定しないときnull）
            private Label m_labelMessage;

            // ファイル操作の文字列表現（設定しないときnull）
            private string m_dispOperation;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]listViewTarget  リストビュー
            // 　　　　[in]labelMessage    メッセージのラベル（設定しないときnull）
            // 　　　　[in]dispOperation   ファイル操作の文字列表現（設定しないときnull）
            // 戻り値：なし
            //=========================================================================================
            public ListImpl(ListView listViewTarget, Label labelMessage, string dispOperation) {
                m_listViewTarget = listViewTarget;
                m_labelMessage = labelMessage;
                m_dispOperation = dispOperation;

                // カラムを初期化
                ColumnHeader column = new ColumnHeader();
                column.Text = Resources.DlgDeleteStart_Column;
                column.Width = -1;
                m_listViewTarget.Columns.AddRange(new ColumnHeader[] {column});

                // イメージリストを初期化
                m_imageList = new ImageList();
                m_imageList.ColorDepth = ColorDepth.Depth32Bit;
                m_imageList.ImageSize = new Size(UIIconManager.CxDefaultIcon, UIIconManager.CyDefaultIcon);
                m_listViewTarget.SmallImageList = m_imageList;
            }

            //=========================================================================================
            // 機　能：ファイル一覧から初期化する
            // 引　数：[in]fileList  確認対象のファイルリスト
            // 戻り値：なし
            //=========================================================================================
            public void InitializeByMarkFile(List<UIFile> fileList) {
                m_listViewTarget.Items.Clear();
                int fileCount = 0;
                int dirCount = 0;
                foreach(UIFile file in fileList) {
                    ListViewItem item = new ListViewItem(FormUtils.CreateListViewString(file.FileName));
                    if (m_mapIconIdToImageListIndex.ContainsKey(file.FileIconId)) {
                        item.ImageIndex = m_mapIconIdToImageListIndex[file.FileIconId];
                    } else {
                        FileIcon icon = Program.Document.FileIconManager.GetFileIcon(file.FileIconId, file.DefaultFileIconId, FileListViewIconSize.IconSize16);
                        m_mapIconIdToImageListIndex.Add(file.FileIconId, m_imageList.Images.Count);
                        item.ImageIndex = m_imageList.Images.Count;
                        m_imageList.Images.Add(icon.IconImage);
                    }
                    m_listViewTarget.Items.Add(item);
                    if (file.Attribute.IsDirectory) {
                        dirCount++;
                    } else {
                        fileCount++;
                    }
                }

                if (m_labelMessage != null) {
                    string message = string.Format(Resources.DlgOperationStart_Message, fileCount, dirCount, m_dispOperation);
                    m_labelMessage.Text = message;
                }
            }

            //=========================================================================================
            // 機　能：再試行情報から初期化する
            // 引　数：[in]retryInfo  確認対象の再試行情報
            // 戻り値：なし
            //=========================================================================================
            public void InitializeByRetryInfo(FileErrorRetryInfo retryInfo) {
                int fileCount = 0;
                int folderCount = 0;
                m_imageList.Images.Add(UIIconManager.IconOperationFailed);
                foreach(IRetryInfo info in retryInfo.RetryApiList) {
                    ListViewItem item = new ListViewItem(FormUtils.CreateListViewString(info.SrcMarkObjectPath.FilePath));
                    item.ImageIndex = 0;
                    m_listViewTarget.Items.Add(item);
                    fileCount++;
                }
                foreach(SimpleFileDirectoryPath info in retryInfo.RetryFileList) {
                    ListViewItem item = new ListViewItem(FormUtils.CreateListViewString(info.FilePath));
                    item.ImageIndex = 0;
                    m_listViewTarget.Items.Add(item);
                    if (info.IsDirectory) {
                        folderCount++;
                    } else {
                        fileCount++;
                    }
                }

                // メッセージ
                if (m_labelMessage != null) {
                    string message = string.Format(Resources.DlgOperationRetryStart_Message, fileCount, folderCount, m_dispOperation);
                    m_labelMessage.Text = message;
                }
            }

            //=========================================================================================
            // 機　能：フォームが閉じられたときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnFormClosed() {
                m_imageList.Dispose();
            }
        }

        //=========================================================================================
        // クラス：ダイアログの削除処理の実装
        //=========================================================================================
        public class DeleteImpl {
            // 削除操作のモード
            private DeleteFileOption m_deleteFileOption = new DeleteFileOption();
            
            // ごみ箱/rm -rfを使って削除するときtrue
            private bool m_deleteDirectoryDirect;

            // チェックボックス：ディレクトリを確認なしで削除
            private CheckBox m_checkBoxDirectory;

            // チェックボックス：属性付きファイルを確認なしで削除
            private CheckBox m_checkBoxAttr;

            // チェックボックス：ごみ箱ありで削除
            private CheckBox m_checkBoxWithRecycle;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]delDirect            ごみ箱/rm -rfを使って削除するときtrue
            // 　　　　[in]fileSystem           対象パスのファイルシステム
            // 　　　　[in]checkBoxDirectory    チェックボックス：ディレクトリを確認なしで削除
            // 　　　　[in]checkBoxAttr         チェックボックス：属性付きファイルを確認なしで削除
            // 　　　　[in]checkBoxWithRecycle  チェックボックス：ごみ箱ありで削除
            // 戻り値：なし
            //=========================================================================================
            public DeleteImpl(bool delDirect, FileSystemID fileSystem, CheckBox checkBoxDirectory, CheckBox checkBoxAttr, CheckBox checkBoxWithRecycle) {
                m_deleteDirectoryDirect = delDirect;
                m_checkBoxDirectory = checkBoxDirectory;
                m_checkBoxAttr = checkBoxAttr;
                m_checkBoxWithRecycle = checkBoxWithRecycle;

                // 削除の設定を準備
                if (Configuration.Current.DeleteFileOptionDefault != null) {
                    m_deleteFileOption = (DeleteFileOption)(Configuration.Current.DeleteFileOptionDefault.Clone());
                } else {
                    m_deleteFileOption = (DeleteFileOption)(Program.Document.UserGeneralSetting.DeleteFileOption.Clone());
                }

                // チェックボックス
                m_checkBoxDirectory.Checked = m_deleteFileOption.DeleteDirectoryAll;
                m_checkBoxAttr.Checked = m_deleteFileOption.DeleteSpecialAttrAll;
                m_checkBoxWithRecycle.Checked = m_deleteDirectoryDirect;
                if (FileSystemID.IsWindows(fileSystem)) {
                    m_checkBoxWithRecycle.Text = Resources.DlgDeleteConfirm_DeleteRecycleBin;
                } else if (FileSystemID.IsSSH(fileSystem)) {
                    m_checkBoxWithRecycle.Text = Resources.DlgDeleteConfirm_DeleteDirectoryRmRf;
                } else {
                    FileSystemID.NotSupportError(fileSystem);
                }
            }

            //=========================================================================================
            // 機　能：ファイル一覧から初期化する
            // 引　数：[in]fileList  確認対象のファイルリスト
            // 戻り値：なし
            //=========================================================================================
            public void InitializeByMarkFile(UIFileList fileList) {
            }

            //=========================================================================================
            // 機　能：再試行情報から初期化する
            // 引　数：[in]retryInfo  確認対象の再試行情報
            // 戻り値：なし
            //=========================================================================================
            public void InitializeByRetryInfo(FileErrorRetryInfo retryInfo) {
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnOK() {
                m_deleteFileOption.DeleteDirectoryAll = m_checkBoxDirectory.Checked;
                m_deleteFileOption.DeleteSpecialAttrAll = m_checkBoxAttr.Checked;
                m_deleteDirectoryDirect = m_checkBoxWithRecycle.Checked;
            }

            //=========================================================================================
            // 機　能：フォームが閉じられたときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnFormClosed() {
            }

            //=========================================================================================
            // プロパティ：削除操作のモード
            //=========================================================================================
            public DeleteFileOption DeleteFileOption {
                get {
                    return m_deleteFileOption;
                }
            }

            //=========================================================================================
            // プロパティ：ごみ箱を使って削除するときtrue
            //=========================================================================================
            public bool DeleteWithRecycle {
                get {
                    return m_deleteDirectoryDirect;
                }
            }
        }

        //=========================================================================================
        // プロパティ：転送条件の入力の実装
        //=========================================================================================
        public class ConditionImpl {
            // 親ダイアログ
            private Form m_parent;

            // ファイル転送条件の設定
            private FileConditionSetting m_setting;

            // 選択中の項目名一覧のリスト（入力された値に書き換えて戻る）
            private FileConditionDialogInfo m_dialogInfo;

            // 条件設定のリスト（対象のファイルシステムのみ）
            private List<FileConditionItem> m_conditionSettingList;

            // ダイアログの結果として入力した転送条件（新しいインスタンス）
            private List<FileConditionItem> m_resultConditionList;

            // 条件を使用するかどうかのチェックボックス
            private CheckBox m_checkBoxCondition;

            // 定義済み設定を使用する際のラジオボタン
            private RadioButton m_radioButtonSetting;
            
            // ワイルドカードを使用する際のラジオボタン
            private RadioButton m_radioButtonWild;
            
            // 条件の選択用リストボックス
            private CheckedListBox m_checkedListCondition;

            // 条件の設定用ボタン
            private Button m_buttonSetting;

            // ワイルドカード入力領域
            private TextBox m_textBoxWildCard;

            // 対象パスのファイルシステム
            private FileSystemID m_fileSystemId;

            // ツールチップ
            private ToolTip m_toolTipOption = null;

            // EnableUIItemの追加機能のAPI型
            public delegate void EnableUIItemDelegate();

            // EnableUIItemの追加機能
            public EnableUIItemDelegate EnableUIItemHandler = null;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent             親ダイアログ
            // 　　　　[in]setting            ファイル転送条件の設定
            // 　　　　[in]dialogInfo         入力の初期値（入力された値に書き換えて戻る）
            // 　　　　[in]fileSystem         対象パスのファイルシステム
            // 　　　　[in]checkBoxCondition  条件を使用するかどうかのチェックボックス
            // 　　　　[in]radioButtonSetting 定義済み設定を使用する際のラジオボタン
            // 　　　　[in]radioButtonWild    ワイルドカードを使用する際のラジオボタン
            // 　　　　[in]comboBoxCondition  条件の選択用リストボックス
            // 　　　　[in]buttonSetting      条件の設定用ボタン
            // 　　　　[in]textBoxWildCard    ワイルドカード入力領域
            // 戻り値：なし
            //=========================================================================================
            public ConditionImpl(Form parent, FileConditionSetting setting, FileConditionDialogInfo dialogInfo, FileSystemID fileSystem,
                        CheckBox checkBoxCondition, RadioButton radioButtonSetting, RadioButton radioButtonWild,
                        CheckedListBox checkedListCondition, Button buttonSetting, TextBox textBoxWildCard) {
                m_parent = parent;
                m_setting = setting;
                m_dialogInfo = dialogInfo;
                m_fileSystemId = fileSystem;
                m_checkBoxCondition = checkBoxCondition;
                m_radioButtonSetting = radioButtonSetting;
                m_radioButtonWild = radioButtonWild;
                m_checkedListCondition = checkedListCondition;
                m_buttonSetting = buttonSetting;
                m_textBoxWildCard = textBoxWildCard;

                // ダイアログの初期値
                if (dialogInfo.ConditionMode) {
                    m_radioButtonSetting.Checked = true;
                } else {
                    m_radioButtonWild.Checked = true;
                }
                m_textBoxWildCard.Text = dialogInfo.WildCard;

                // ツールチップ
                m_toolTipOption = new ToolTip();
                m_toolTipOption.InitialDelay = 500;
                m_toolTipOption.ReshowDelay = 1000;
                m_toolTipOption.AutoPopDelay = 3000;
                m_toolTipOption.ShowAlways = true;

                // 条件一覧を初期化
                ResetCondition();
                EnableUIItem();

                m_checkedListCondition.SelectedIndexChanged += new EventHandler(checkedListCondition_SelectedIndexChanged);
                m_buttonSetting.Click += new EventHandler(buttonSetting_Click);
                m_checkedListCondition.KeyDown += new KeyEventHandler(DialogItem_KeyDown);
                m_textBoxWildCard.KeyDown += new KeyEventHandler(DialogItem_KeyDown);
                m_radioButtonSetting.KeyDown += new KeyEventHandler(DialogItem_KeyDown);
                m_radioButtonSetting.Enter += new EventHandler(RadioButton_Enter);
                m_radioButtonSetting.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                m_radioButtonWild.KeyDown += new KeyEventHandler(DialogItem_KeyDown);
                m_radioButtonWild.Enter += new EventHandler(RadioButton_Enter);
                m_radioButtonWild.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                if (m_checkBoxCondition != null) {
                    m_checkBoxCondition.CheckedChanged += new EventHandler(CheckBoxCondition_CheckedChanged);
                    m_checkBoxCondition.Checked = false;
                }
            }

            //=========================================================================================
            // 機　能：UIの有効/無効を切り替える
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void EnableUIItem() {
                if (m_checkBoxCondition == null || m_checkBoxCondition.Checked) {
                    m_radioButtonWild.Enabled = true;
                    m_radioButtonSetting.Enabled = true;
                    if (m_radioButtonSetting.Checked) {
                        m_checkedListCondition.Enabled = true;
                        m_buttonSetting.Enabled = true;
                        m_textBoxWildCard.Enabled = false;
                    } else {
                        m_checkedListCondition.Enabled = false;
                        m_buttonSetting.Enabled = false;
                        m_textBoxWildCard.Enabled = true;
                    }
                } else {
                    m_radioButtonWild.Enabled = false;
                    m_radioButtonSetting.Enabled = false;
                    m_checkedListCondition.Enabled = false;
                    m_buttonSetting.Enabled = false;
                    m_textBoxWildCard.Enabled = false;
                }
                if (EnableUIItemHandler != null) {
                    EnableUIItemHandler();
                }
            }

            //=========================================================================================
            // 機　能：キーが押されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void DialogItem_KeyDown(object sender, KeyEventArgs evt) {
                if (evt.KeyCode == Keys.Down &&
                            (sender == m_checkedListCondition && m_checkedListCondition.SelectedIndex == m_checkedListCondition.Items.Count - 1
                             || sender == m_radioButtonSetting)) {
                    m_radioButtonWild.Checked = true;
                    HideToolHint();
                    m_parent.ActiveControl = m_textBoxWildCard;
                    m_textBoxWildCard.Focus();
                    evt.Handled = true;
                    evt.SuppressKeyPress = true;
                } else if (evt.KeyCode == Keys.Up && (sender == m_textBoxWildCard || sender == m_radioButtonWild)) {
                    m_radioButtonSetting.Checked = true;
                    m_parent.ActiveControl = m_checkedListCondition;
                    m_checkedListCondition.Focus();
                    evt.Handled = true;
                    evt.SuppressKeyPress = true;
                }
            }

            //=========================================================================================
            // 機　能：条件一覧の選択項目が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void checkedListCondition_SelectedIndexChanged(object sender, EventArgs evt) {
                int index = m_checkedListCondition.SelectedIndex;
                if (index == -1) {
                    return;
                }

                // 表示
                Rectangle rectItem = m_checkedListCondition.GetItemRectangle(index);
                Point ptItem = new Point(rectItem.Left + 50, rectItem.Bottom + 16);
                string message = StringUtils.ConbineLine(m_conditionSettingList[index].ToDisplayString(), "\r\n");
                m_toolTipOption.Show(message, m_checkedListCondition, ptItem, 3000);
            }

            //=========================================================================================
            // 機　能：条件一覧を初期化する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void ResetCondition() {
                m_conditionSettingList = m_setting.GetAllSettingItemUI(m_fileSystemId);
                HashSet<string> selectedItems = ArrayUtils.ListToHash<string>(m_dialogInfo.SelectedConditionList);

                m_checkedListCondition.Items.Clear();
                for (int i = 0; i < m_conditionSettingList.Count; i++) {
                    m_checkedListCondition.Items.Add(m_conditionSettingList[i].DisplayName);
                    if (selectedItems.Contains(m_conditionSettingList[i].DisplayName)) {
                        m_checkedListCondition.SetItemChecked(i, true);
                        if (m_checkedListCondition.SelectedIndex == -1) {
                            m_checkedListCondition.SelectedIndex = i;
                        }
                    }
                }
                if (m_checkedListCondition.SelectedIndex == -1) {
                    m_checkedListCondition.SelectedIndex = 0;
                }
            }

            //=========================================================================================
            // 機　能：設定ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonSetting_Click(object sender, EventArgs evt) {
                if (FileSystemID.IsVirtual(m_fileSystemId)) {
                    InfoBox.Warning(m_parent, Resources.DlgDeleteStart_VirtualConditionNotSupport);
                    return;
                }

                FileConditionSetting newSetting = (FileConditionSetting)(m_setting.Clone());
                string initialDisplayName = m_conditionSettingList[m_checkedListCondition.SelectedIndex].DisplayName;
                FileConditionSettingDialog dialog = new FileConditionSettingDialog(newSetting, m_fileSystemId, initialDisplayName);
                dialog.ShowDialog(m_parent);
                if (FileConditionSetting.EqualsConfig(m_setting, newSetting)) {
                    return;
                }

                // 項目を差し替える
                m_setting = newSetting;
                m_setting.SaveSetting();
                ResetCondition();
            }

            //=========================================================================================
            // 機　能：条件を使用するかどうかのチェックボックスがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void CheckBoxCondition_CheckedChanged(object sender, EventArgs eve) {
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：ワイルドカードのラジオボタンがフォーカスを受け取ったときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void RadioButton_Enter(object sender, EventArgs evt) {
                if (sender == m_radioButtonSetting) {
                    if (!m_radioButtonSetting.Checked) {
                        m_radioButtonSetting.Checked = true;
                    } else {
                        m_parent.ActiveControl = m_checkedListCondition;
                    }
                } else if (sender == m_radioButtonWild) {
                    if (!m_radioButtonWild.Checked) {
                        m_radioButtonWild.Checked = true;
                    } else {
                        m_parent.ActiveControl = m_textBoxWildCard;
                    }
                }
            }

            //=========================================================================================
            // 機　能：条件指定/ワイルドカードのラジオボタンが変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void RadioButton_CheckedChanged(object sender, EventArgs evt) {
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：[in]wildFileFolder  ワイルドカードでのファイル／フォルダの対象区分
            // 戻り値：ダイアログを閉じてよいときtrue
            //=========================================================================================
            public bool OnOk(FileConditionTarget wildFileFolder) {
                if (m_checkBoxCondition == null || m_checkBoxCondition.Checked) {
                    // 転送条件を取得
                    if (m_radioButtonSetting.Checked) {
                        // 条件指定によるモード
                        m_resultConditionList = new List<FileConditionItem>();
                        List<string> conditionNameList = new List<string>();
                        for (int i = 0; i < m_checkedListCondition.Items.Count; i++) {
                            if (m_checkedListCondition.GetItemChecked(i)) {
                                m_resultConditionList.Add((FileConditionItem)(m_conditionSettingList[i].Clone()));
                                conditionNameList.Add(m_conditionSettingList[i].DisplayName);
                            }
                        }
                        if (m_resultConditionList.Count == 0) {
                            InfoBox.Warning(m_parent, Resources.DlgDeleteStart_ErrorCondTarget);
                            return false;
                        }

                        m_dialogInfo.ConditionMode = true;
                        m_dialogInfo.SelectedConditionList.Clear();
                        m_dialogInfo.SelectedConditionList.AddRange(conditionNameList);
                    } else {
                        // ワイルドカード入力による指定
                        string wildCard = m_textBoxWildCard.Text;
                        if (wildCard == "") {
                            InfoBox.Warning(m_parent, Resources.DlgCond_EmptyWildCard);
                            return false;
                        }
                        FileConditionItemDefined item = FileConditionItemDefined.CreateFromWildCard(wildCard, wildFileFolder);
                        if (item == null) {
                            InfoBox.Warning(m_parent, Resources.DlgCond_BadWildCard);
                            return false;
                        }
                        m_resultConditionList = new List<FileConditionItem>();
                        m_resultConditionList.Add(item);

                        m_dialogInfo.ConditionMode = false;
                        m_dialogInfo.WildCard = wildCard;
                    }
                } else {
                    // 条件なし（無条件に転送）
                    m_resultConditionList = null;
                }
                return true;
            }

            //=========================================================================================
            // 機　能：ツールヒントを非表示にする
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void HideToolHint() {
                m_toolTipOption.Hide(m_checkedListCondition);
            }

            //=========================================================================================
            // プロパティ：ダイアログの結果として入力した転送条件（新しいインスタンス、チェックボックスありで無条件に転送するときnull）
            //=========================================================================================
            public List<FileConditionItem> ResultConditionList {
                get {
                    return m_resultConditionList;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル転送条件の設定
            //=========================================================================================
            public FileConditionSetting FileConditionSetting {
                get {
                    return m_setting;
                }
            }
        }
    }
}
