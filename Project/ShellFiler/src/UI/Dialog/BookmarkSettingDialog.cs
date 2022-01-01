using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ブックマークの編集ダイアログ
    //=========================================================================================
    public partial class BookmarkSettingDialog : Form {
        // 編集中の設定（コンストラクタとは別インスタンス）
        private BookmarkSetting m_setting;

        // フォームの入力結果（入力完了時以外はnull）
        private BookmarkSetting m_formResult = null;

        // ショートカットの文字からm_shortcutItemsのインデックスへのMap
        private Dictionary<char, int> m_shortcutCharToIndex = new Dictionary<char, int>();

        // ファイル一覧でのカレントフォルダ
        private string m_currentPath;

        // 現在選択中のツリーのノード
        private TreeNode m_currentNode;

        // ツリーが更新中のときtrue
        private bool m_treeUpdate = true;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public BookmarkSettingDialog(BookmarkSetting setting, string current) {
            InitializeComponent();
            m_currentPath = current;
            m_setting = (BookmarkSetting)(setting.Clone());
            for (int i = 0; i < BookmarkItem.SHORTCUT_ITEMS.Length; i++) {
                m_shortcutCharToIndex.Add(BookmarkItem.SHORTCUT_ITEMS[i][0], i);
            }

            // イメージリストを追加
            FileIconManager iconManager = Program.Document.FileIconManager;
            FileIcon icon = iconManager.GetFileIcon(iconManager.DefaultFolderIconId, FileIconID.NullId, FileListViewIconSize.IconSize16);
            this.treeViewSetting.ImageList = UIIconManager.IconImageList;

            // ツリーのノードを作成
            foreach (BookmarkGroup group in m_setting.BookmarkGroupList) {
                TreeNode groupNode = CreateGroupNode(group);
                this.treeViewSetting.Nodes.Add(groupNode);
                foreach (BookmarkItem item in group.ItemList) {
                    TreeNode itemNode = CreateItemNode(item);
                    groupNode.Nodes.Add(itemNode);
                }
                groupNode.Expand();
            }
            m_treeUpdate = false;
        }

        //=========================================================================================
        // 機　能：グループのノードを作成する
        // 引　数：[in]group  グループの情報
        // 戻り値：グループのツリーノード
        //=========================================================================================
        private TreeNode CreateGroupNode(BookmarkGroup group) {
            TreeNode nodeGroup = new TreeNode(group.GroupName, (int)IconImageListID.Icon_BookmarkGroup, (int)IconImageListID.Icon_BookmarkGroup);
            nodeGroup.Tag = group;
            return nodeGroup;
        }

        //=========================================================================================
        // 機　能：フォルダ項目のノードを作成する
        // 引　数：[in]group  フォルダ項目の情報
        // 戻り値：フォルダ項目のツリーノード
        //=========================================================================================
        private TreeNode CreateItemNode(BookmarkItem item) {
            string itemDisp = CreateItemNodeName(item);
            TreeNode nodeItem = new TreeNode(itemDisp, (int)IconImageListID.Icon_BookmarkItem, (int)IconImageListID.Icon_BookmarkItem);
            nodeItem.Tag = item;
            nodeItem.ToolTipText = item.Directory;
            return nodeItem;
        }

        //=========================================================================================
        // 機　能：フォルダ項目の表示名を作成する
        // 引　数：[in]item   フォルダ項目の情報
        // 戻り値：フォルダ項目の表示名
        //=========================================================================================
        private string CreateItemNodeName(BookmarkItem item) {
            return item.DisplayName + "(" + item.ShortCut + ")";
        }

        //=========================================================================================
        // 機　能：現在選択中のグループと項目のインデックスを取得する
        // 引　数：[out]groupIndex   選択中のグループのインデックスを返す変数
        // 　　　　[out]folderIndex  選択中のフォルダのインデックスを返す変数（グループ選択中は-1）
        // 戻り値：なし
        //=========================================================================================
        private void GetCurrentIndex(out int groupIndex, out int folderIndex) {
            groupIndex = 0;
            folderIndex = 0;
            TreeNode selected = this.treeViewSetting.SelectedNode;

            // グループのノードを走査
            for (int i = 0; i < this.treeViewSetting.Nodes.Count; i++) {
                TreeNode groupNode = this.treeViewSetting.Nodes[i];
                if (groupNode == selected) {
                    groupIndex = i;
                    folderIndex = -1;
                    return;
                }

                // フォルダのノードを走査
                for (int j = 0; j < groupNode.Nodes.Count; j++) {
                    if (groupNode.Nodes[j] == selected) {
                        groupIndex = i;
                        folderIndex = j;
                        return;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：フォルダ項目の親となるブックマークのグループを返す
        // 引　数：[in]checkItem  フォルダ項目
        // 戻り値：フォルダ項目の親となるグループ
        //=========================================================================================
        private BookmarkGroup GetBookmarkGroupFromBookmarkItem(BookmarkItem checkItem) {
            foreach (BookmarkGroup group in m_setting.BookmarkGroupList) {
                foreach (BookmarkItem item in group.ItemList) {
                    if (item == checkItem) {
                        return group;
                    }
                }
            }
            return null;
        }

        //=========================================================================================
        // 機　能：一意のグループ名を作成する
        // 引　数：[in]groupNameDefault  元のグループ名
        // 戻り値：番号付けにより一意になったグループ名
        //=========================================================================================
        private string CreateUniqueGroupName(string groupNameDefault) {
            int groupIndex = 1;
            while (true) {
                string groupName;
                if (groupNameDefault == null) {
                    if (groupIndex == 1) {
                        groupName = Resources.DlgBookmark_NewGroupName;
                    } else {
                        groupName = string.Format(Resources.DlgBookmark_NewGroupName2, groupIndex);
                    }
                } else {
                    if (groupIndex == 1) {
                        groupName = groupNameDefault;
                    } else {
                        groupName = groupNameDefault + " " + groupIndex;
                    }
                }
                bool foundSameGroup = false;
                foreach (BookmarkGroup existGroup in m_setting.BookmarkGroupList) {
                    if (existGroup.GroupName == groupName) {
                        foundSameGroup = true;
                        break;
                    }
                }
                if (!foundSameGroup) {
                    return groupName;
                }
                groupIndex++;
            }
        }

        //=========================================================================================
        // 機　能：一意のフォルダ表示名を作成する
        // 引　数：[in]group        現在選択中のグループ
        // 　　　　[in]currentPath  現在のファイル一覧でのパス名
        // 戻り値：フォルダ名から抽出した一意のフォルダ表示名
        //=========================================================================================
        private string CreateUniqueFolderName(BookmarkGroup group, string currentPath) {
            string fileName = GenericFileStringUtils.GetFileName(GenericFileStringUtils.RemoveLastDirectorySeparator(currentPath));
            int nameIndex = 1;
            while (true) {
                string itemName;
                if (nameIndex == 1) {
                    itemName = fileName;
                } else {
                    itemName = fileName + " " + nameIndex;
                }
                bool foundSameItem = false;
                foreach (BookmarkItem bookmarkItem in group.ItemList) {
                    if (bookmarkItem.DisplayName == itemName) {
                        foundSameItem = true;
                    }
                }
                if (!foundSameItem) {
                    return itemName;
                }
                nameIndex++;
            }
        }

        //=========================================================================================
        // 機　能：一意のフォルダショートカットキーを作成する
        // 引　数：[in]group        現在選択中のグループ
        // 　　　　[in]currentPath  現在のファイル一覧でのパス名
        // 戻り値：フォルダ名から抽出した一意のショートカット
        //=========================================================================================
        private char CreateUniqueFolderShortcut(BookmarkGroup group, string currentPath) {
            // 既存ショートカットの一覧を作成
            bool[] existShortcut = new bool[BookmarkItem.SHORTCUT_ITEMS.Length];
            foreach (BookmarkItem bookmarkItem in group.ItemList) {
                if (m_shortcutCharToIndex.ContainsKey(bookmarkItem.ShortCut)) {
                    existShortcut[m_shortcutCharToIndex[bookmarkItem.ShortCut]] = true;
                }
            }

            // ショートカットの初期値候補を決定
            string fileName = GenericFileStringUtils.GetFileName(GenericFileStringUtils.RemoveLastDirectorySeparator(currentPath));
            string upper = fileName.ToUpper();
            char shortcut = 'A';
            if (fileName.Length > 0 && m_shortcutCharToIndex.ContainsKey(upper[0])) {
                shortcut = upper[0];
            }

            // 新しいショートカットを評価
            int shortcutIndex = m_shortcutCharToIndex[shortcut];
            if (!existShortcut[shortcutIndex]) {
                return shortcut;
            } else {
                while (true) {
                    shortcutIndex = (shortcutIndex + 1) % BookmarkItem.SHORTCUT_ITEMS.Length;
                    if (!existShortcut[shortcutIndex]) {
                        return BookmarkItem.SHORTCUT_ITEMS[shortcutIndex][0];
                    }
                }
            }
        }
        
        //=========================================================================================
        // 機　能：ショートカットからそのインデックスを作成する
        // 引　数：[in]key    ショートカットキー
        // 戻り値：ショートカットキーに対応するインデックス
        //=========================================================================================
        private int ShortcutToIndex(char key) {
            for (int i = 0; i < BookmarkItem.SHORTCUT_ITEMS.Length; i++) {
                if (key == BookmarkItem.SHORTCUT_ITEMS[i][0]) {
                    return i;
                }
            }
            return 0;
        }

        //=========================================================================================
        // 機　能：ボタンの有効/無効状態を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateButtonState() {
            int currentGroupIndex, currentFolderIndex;
            GetCurrentIndex(out currentGroupIndex, out currentFolderIndex);
            if (currentFolderIndex == -1) {
                // グループを選択中
                if (currentGroupIndex == 0) {
                    this.buttonUp.Enabled = false;
                } else {
                    this.buttonUp.Enabled = true;
                }
                if (currentGroupIndex == m_setting.BookmarkGroupList.Count - 1) {
                    this.buttonDown.Enabled = false;
                } else {
                    this.buttonDown.Enabled = true;
                }
            } else {
                // フォルダ項目を選択中
                if (currentGroupIndex == 0 && currentFolderIndex == 0) {
                    this.buttonUp.Enabled = false;
                } else if (currentFolderIndex == 0) {
                    this.buttonUp.Enabled = false;
                } else {
                    this.buttonUp.Enabled = true;
                }
                int groupCount = m_setting.BookmarkGroupList.Count;
                int folderCount = m_setting.BookmarkGroupList[currentGroupIndex].ItemList.Count;
                if (currentGroupIndex == groupCount - 1 && currentFolderIndex == folderCount - 1) {
                    this.buttonDown.Enabled = false;
                } else if (currentFolderIndex == folderCount - 1) {
                    this.buttonDown.Enabled = false;
                } else {
                    this.buttonDown.Enabled = true;
                }
            }
        }

        //=========================================================================================
        // 機　能：グループの新規作成ボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNewGroup_Click(object sender, EventArgs evt) {
            // グループ数の上限をチェック
            if (m_setting.BookmarkGroupList.Count >= BookmarkGroup.MAX_GROUP_COUNT) {
                InfoBox.Warning(this, Resources.DlgBookmark_TooManyGroups);
                return;
            }

            // 新しいグループを作成
            string groupName = CreateUniqueGroupName(null);
            BookmarkGroup groupItem = new BookmarkGroup();
            groupItem.GroupName = groupName;
            TreeNode groupNode = CreateGroupNode(groupItem);

            // グループを登録
            int currentGroupIndex, dummy;
            GetCurrentIndex(out currentGroupIndex, out dummy);
            currentGroupIndex++;
            m_setting.InsertGroup(currentGroupIndex, groupItem);
            this.treeViewSetting.Nodes.Insert(currentGroupIndex, groupNode);
            this.treeViewSetting.SelectedNode = groupNode;
        }

        //=========================================================================================
        // 機　能：フォルダの新規作成ボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNewFolder_Click(object sender, EventArgs evt) {
            // 現在のグループのフォルダ数の上限をチェック
            int currentGroupIndex, currentFolderIndex;
            GetCurrentIndex(out currentGroupIndex, out currentFolderIndex);
            if (m_setting.BookmarkGroupList[currentGroupIndex].ItemList.Count >= BookmarkGroup.MAX_FOLDER_COUNT) {
                InfoBox.Warning(this, Resources.DlgBookmark_TooManyFolders);
                return;
            }

            // 新しいフォルダを作成
            BookmarkGroup groupItem = m_setting.BookmarkGroupList[currentGroupIndex];
            string folderName = CreateUniqueFolderName(groupItem, m_currentPath);
            char shortcut = CreateUniqueFolderShortcut(groupItem, m_currentPath);
            BookmarkItem folderItem = new BookmarkItem(shortcut, folderName, m_currentPath);
            TreeNode folderNode = CreateItemNode(folderItem);

            // グループを登録
            int insertPos;
            if (currentFolderIndex == -1) {
                insertPos = m_setting.BookmarkGroupList[currentGroupIndex].ItemList.Count;
            } else {
                insertPos = currentFolderIndex + 1;
            }
            m_setting.BookmarkGroupList[currentGroupIndex].InsertDirectory(insertPos, folderItem);
            this.treeViewSetting.Nodes[currentGroupIndex].Nodes.Insert(insertPos, folderNode);
            this.treeViewSetting.SelectedNode = folderNode;
        }

        //=========================================================================================
        // 機　能：削除ボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            if (m_currentNode == null) {
                return;
            }
            if (m_currentNode.Tag is BookmarkGroup) {
                // グループを削除
                BookmarkGroup group = (BookmarkGroup)(m_currentNode.Tag);

                // 最後の1グループは削除できない
                if (m_setting.BookmarkGroupList.Count <= 1) {
                    InfoBox.Warning(this, Resources.DlgBookmark_CannotDeleteAll);
                    return;
                }

                // 項目がある場合は確認
                if (group.ItemList.Count > 0) {
                    DialogResult result = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgBookmark_DeleteGroup, group.GroupName);
                    if (result != DialogResult.Yes) {
                        return;
                    }
                }

                m_setting.DeleteGroup(group);
                this.treeViewSetting.Nodes.Remove(m_currentNode);
            } else {
                // 項目を削除
                BookmarkItem item = (BookmarkItem)(m_currentNode.Tag);
                this.treeViewSetting.Nodes.Remove(m_currentNode);
                BookmarkGroup group = GetBookmarkGroupFromBookmarkItem(item);
                group.DeleteItem(item);
            }
        }

        //=========================================================================================
        // 機　能：上へボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonUp_Click(object sender, EventArgs evt) {
            bool success = RetrieveUIValue();
            if (!success) {
                return;
            }
            TreeNode node = this.treeViewSetting.SelectedNode;
            m_treeUpdate = true;
            try {
                int currentGroupIndex, currentFolderIndex;
                GetCurrentIndex(out currentGroupIndex, out currentFolderIndex);
                if (node.Tag is BookmarkGroup) {
                    // グループをまとめて上に移動
                    m_setting.SwapGroup(currentGroupIndex, currentGroupIndex - 1);
                    this.treeViewSetting.Nodes.RemoveAt(currentGroupIndex);
                    this.treeViewSetting.Nodes.Insert(currentGroupIndex - 1, node);
                    UpdateButtonState();
                } else {
                    // フォルダ項目を上に移動
                    m_setting.BookmarkGroupList[currentGroupIndex].SwapItem(currentFolderIndex, currentFolderIndex - 1);
                    this.treeViewSetting.Nodes[currentGroupIndex].Nodes.RemoveAt(currentFolderIndex);
                    this.treeViewSetting.Nodes[currentGroupIndex].Nodes.Insert(currentFolderIndex - 1, node);
                }
            } finally {
                this.treeViewSetting.SelectedNode = node;
                m_treeUpdate = false;
            }
            UpdateButtonState();
        }

        //=========================================================================================
        // 機　能：下へボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDown_Click(object sender, EventArgs evt) {
            bool success = RetrieveUIValue();
            if (!success) {
                return;
            }
            TreeNode node = this.treeViewSetting.SelectedNode;
            m_treeUpdate = true;
            try {
                int currentGroupIndex, currentFolderIndex;
                GetCurrentIndex(out currentGroupIndex, out currentFolderIndex);
                if (node.Tag is BookmarkGroup) {
                    // グループをまとめて下に移動
                    m_setting.SwapGroup(currentGroupIndex, currentGroupIndex + 1);
                    this.treeViewSetting.Nodes.RemoveAt(currentGroupIndex);
                    this.treeViewSetting.Nodes.Insert(currentGroupIndex + 1, node);
                    UpdateButtonState();
                } else {
                    // フォルダ項目を下に移動
                    m_setting.BookmarkGroupList[currentGroupIndex].SwapItem(currentFolderIndex, currentFolderIndex + 1);
                    this.treeViewSetting.Nodes[currentGroupIndex].Nodes.RemoveAt(currentFolderIndex);
                    this.treeViewSetting.Nodes[currentGroupIndex].Nodes.Insert(currentFolderIndex + 1, node);
                }
            } finally {
                this.treeViewSetting.SelectedNode = node;
                m_treeUpdate = false;
            }
            UpdateButtonState();
        }

        //=========================================================================================
        // 機　能：フォルダの参照ボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonFolderRef_Click(object sender, EventArgs evt) {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = this.textBoxFolder.Text;
            DialogResult dr = fbd.ShowDialog(this);
            if (dr == DialogResult.OK) {
                this.textBoxFolder.Text = fbd.SelectedPath;
            }
        }

        //=========================================================================================
        // 機　能：初期値の挿入ボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonInitial_Click(object sender, EventArgs evt) {
            // グループ数の上限をチェック
            if (m_setting.BookmarkGroupList.Count >= BookmarkGroup.MAX_GROUP_COUNT) {
                InfoBox.Warning(this, Resources.DlgBookmark_TooManyGroups);
                return;
            }

            // デフォルトの定義を作成して追加
            BookmarkGroup group = BookmarkSetting.CreateDefaultBookmarkGroup();
            group.GroupName = CreateUniqueGroupName(group.GroupName);
            m_setting.AddGroup(group);
            TreeNode groupNode = CreateGroupNode(group);
            this.treeViewSetting.Nodes.Add(groupNode);
            foreach (BookmarkItem item in group.ItemList) {
                TreeNode itemNode = CreateItemNode(item);
                groupNode.Nodes.Add(itemNode);
            }
            groupNode.Expand();
            this.treeViewSetting.SelectedNode = groupNode;
        }
        
        //=========================================================================================
        // 機　能：ツリーが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewSetting_BeforeCollapse(object sender, TreeViewCancelEventArgs evt) {
            evt.Cancel = true;
        }

        //=========================================================================================
        // 機　能：ツリーの項目が選択されようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewSetting_BeforeSelect(object sender, TreeViewCancelEventArgs evt) {
            if (m_treeUpdate) {
                return;
            }

            bool success = RetrieveUIValue();
            if (!success) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // 機　能：ツリーの項目が選択されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewSetting_AfterSelect(object sender, TreeViewEventArgs evt) {
            if (m_treeUpdate) {
                return;
            }
            TreeNode node = this.treeViewSetting.SelectedNode;
            if (node.Tag is BookmarkGroup) {
                BookmarkGroup group = (BookmarkGroup)(node.Tag);
                this.textBoxDispName.Text = group.GroupName;
                this.textBoxFolder.Text = "";
                this.textBoxFolder.Enabled = false;
                this.buttonFolderRef.Enabled = false;
                if (m_currentNode != node) {
                    this.comboBoxShortcut.Items.Clear();
                    this.comboBoxShortcut.Items.Add("");
                    this.comboBoxShortcut.SelectedIndex = 0;
                    m_currentNode = node;
                }
                this.comboBoxShortcut.Enabled = false;
            } else {
                BookmarkItem item = (BookmarkItem)(node.Tag);
                this.textBoxDispName.Text = item.DisplayName;
                this.textBoxFolder.Text = item.Directory;
                this.textBoxFolder.Enabled = true;
                this.buttonFolderRef.Enabled = true;
                if (m_currentNode != node) {
                    this.comboBoxShortcut.Items.Clear();
                    this.comboBoxShortcut.Items.Add(item.ShortCut.ToString());
                    this.comboBoxShortcut.SelectedIndex = 0;
                    m_currentNode = node;
                }
                this.comboBoxShortcut.Enabled = true;
            }
            UpdateButtonState();
        }

        //=========================================================================================
        // 機　能：ショートカットのコンボボックスのドロップダウンが開かれたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxShortcut_DropDown(object sender, EventArgs evt) {
            // フォルダの項目だけを処理
            TreeNode node = this.treeViewSetting.SelectedNode;
            if (node.Tag is BookmarkGroup) {
                return;
            }

            // 現在の情報を取得
            BookmarkItem item = (BookmarkItem)(node.Tag);
            int currentGroupIndex, currentFolderIndex;
            GetCurrentIndex(out currentGroupIndex, out currentFolderIndex);
            BookmarkGroup groupItem = m_setting.BookmarkGroupList[currentGroupIndex];

            // 既存ショートカットの一覧を作成
            bool[] existShortcut = new bool[BookmarkItem.SHORTCUT_ITEMS.Length];
            Array.Clear(existShortcut, 0, existShortcut.Length);
            foreach (BookmarkItem bookmarkItem in groupItem.ItemList) {
                if (m_shortcutCharToIndex.ContainsKey(bookmarkItem.ShortCut)) {
                    existShortcut[m_shortcutCharToIndex[bookmarkItem.ShortCut]] = true;
                }
            }

            // 項目の一覧を作成
            List<string> shortcutDropdown = new List<string>();
            int currentShortcutIndex = ShortcutToIndex(item.ShortCut);
            int selectIndex = 0;
            for (int i = 0; i < BookmarkItem.SHORTCUT_ITEMS.Length; i++) {
                if (i == currentShortcutIndex) {
                    selectIndex = shortcutDropdown.Count;
                    shortcutDropdown.Add(BookmarkItem.SHORTCUT_ITEMS[i]);
                } else if (!existShortcut[i]) {
                    shortcutDropdown.Add(BookmarkItem.SHORTCUT_ITEMS[i]);
                }
            }

            // UIに反映
            this.comboBoxShortcut.Items.Clear();
            this.comboBoxShortcut.Items.AddRange(shortcutDropdown.ToArray());
            this.comboBoxShortcut.SelectedIndex = selectIndex;
        }

        //=========================================================================================
        // 機　能：UIから値を取得する
        // 引　数：なし
        // 戻り値：値の取得に成功したときtrue、エラーのため再入力が必要なときfalse
        //=========================================================================================
        private bool RetrieveUIValue() {
            if (m_currentNode == null) {
                return true;
            }
            if (m_currentNode.Tag is BookmarkGroup) {
                // グループの情報を取り込む
                BookmarkGroup group = (BookmarkGroup)(m_currentNode.Tag);
                bool duplicate = IsDuplicateGroupName(group, this.textBoxDispName.Text);
                if (duplicate) {
                    InfoBox.Warning(this, Resources.DlgBookmark_DuplicateGroupName);
                    return false;
                }
                if (this.textBoxDispName.Text == "") {
                    InfoBox.Warning(this, Resources.DlgBookmark_InvalidGroupName);
                    return false;
                }
                group.GroupName = this.textBoxDispName.Text;
                m_currentNode.Text = group.GroupName;
            } else {
                // フォルダ項目の情報を取り込む
                BookmarkItem item = (BookmarkItem)(m_currentNode.Tag);
                bool duplicate = IsDuplicateItemName(item, this.textBoxDispName.Text);
                if (duplicate) {
                    InfoBox.Warning(this, Resources.DlgBookmark_DuplicateItemName);
                    return false;
                }
                if (this.textBoxDispName.Text == "") {
                    InfoBox.Warning(this, Resources.DlgBookmark_InvalidFilePath);
                    return false;
                }
                FileSystemID fileSystemId = Program.Document.FileSystemFactory.GetFileSystemFromRootPath(this.textBoxFolder.Text);
                if (fileSystemId == FileSystemID.None) {
                    InfoBox.Warning(this, Resources.DlgBookmark_UnknownFilePath);
                    return false;
                }
                string shortcutComboboxStr = (string)(this.comboBoxShortcut.Items[this.comboBoxShortcut.SelectedIndex]);
                item.DisplayName = this.textBoxDispName.Text;
                item.Directory = this.textBoxFolder.Text;
                item.ShortCut = shortcutComboboxStr[0];
                m_currentNode.Text = CreateItemNodeName(item);
            }
            return true;
        }

        //=========================================================================================
        // 機　能：グループ名が重複しているかどうかを返す
        // 引　数：[in]targetGroup  チェック対象のグループ(除外するグループ)
        // 　　　　[in]groupName    新しいグループ名
        // 戻り値：グループ名が重複しているときtrue
        //=========================================================================================
        private bool IsDuplicateGroupName(BookmarkGroup targetGroup, string groupName) {
            foreach (BookmarkGroup group in m_setting.BookmarkGroupList) {
                if (group != targetGroup) {
                    if (group.GroupName == groupName) {
                        return true;
                    }
                }
            }
            return false;
        }

        //=========================================================================================
        // 機　能：フォルダの表示名が重複しているかどうかを返す
        // 引　数：[in]targetItem   チェック対象のフォルダ項目（除外するフォルダ項目）
        // 　　　　[in]itemName     新しいフォルダの表示名
        // 戻り値：フォルダの表示名が重複しているときtrue
        //=========================================================================================
        private bool IsDuplicateItemName(BookmarkItem targetItem, string itemName) {
            BookmarkGroup group = GetBookmarkGroupFromBookmarkItem(targetItem);         // 削除時以外は必ず見つかるはず
            if (group == null) {
                return false;
            }
            foreach (BookmarkItem item in group.ItemList) {
                if (item != targetItem) {
                    if (item.DisplayName == itemName) {
                        return true;
                    }
                }
            }
            return false;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            bool success = RetrieveUIValue();
            if (!success) {
                m_formResult = null;
                return;
            }

            m_formResult = m_setting;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void BookmarkSettingDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && m_formResult == null) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // プロパティ：ブックマークの設定
        //=========================================================================================
        public BookmarkSetting BookmarkSetting {
            get {
                return m_setting;
            }
        }
    }
}
