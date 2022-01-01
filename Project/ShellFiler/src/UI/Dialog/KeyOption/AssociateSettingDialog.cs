using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Command.FileList.Open;
using ShellFiler.Command.FileList.Internal;
using ShellFiler.Properties;
using ShellFiler.Locale;
using ShellFiler.UI.ControlBar;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：関連付け設定ダイアログ
    //=========================================================================================
    public partial class AssociateSettingDialog : Form {
        // メニューの項目（Tag）
        private const string MENUITEM_ORIGINAL = "Original";
        private const string MENUITEM_DEFAULT = "Default";
        private const string MENUITEM_COPYFROM = "CopyFrom";

        // 拡張子のセパレータ
        private const string EXT_SEPARATOR = ";";

        // キー一覧の設定全体
        private KeySetting m_keySetting;

        // 関連付け設定の元の状態
        private AssociateSetting m_assocSettingOrg;

        // コマンド一覧
        private CommandScene m_commandScene;

        // 現在のタブページのインデックス
        private int m_currentPageIndex = -1;

        // リストボックスのItem
        // ・関連付け項目[0]：AssocListBoxItem.Item(0)
        // ・関連付け項目[1]：AssocListBoxItem.Item(1)
        // ・関連付け項目[2]：AssocListBoxItem.Item(2)
        // ・関連付けデフォルト：AssocListBoxItem.Default

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]keySetting   キー一覧の設定全体
        // 　　　　[in]commandSpec  コマンド仕様のXML解析結果
        // 戻り値：なし
        //=========================================================================================
        public AssociateSettingDialog(KeySetting keySetting, CommandSpec commandSpec) {
            InitializeComponent();
            m_keySetting = keySetting;
            m_assocSettingOrg = (AssociateSetting)(keySetting.AssociateSetting.Clone());
            m_commandScene = commandSpec.FileList;
        }

        //=========================================================================================
        // 機　能：ダイアログが初期化されるときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        // メ　モ：ツリーの初期化はダイアログ作成後でないと成功しないため、コンストラクタの処理を移植。
        //=========================================================================================
        private void AssociateSettingDialog_Load(object sender, EventArgs evt) {
            this.panelPage.Location = new Point(this.panelPage.Location.X + 16, this.panelPage.Location.Y);

            // タブページ
            AssociateSetting assocSetting = m_keySetting.AssociateSetting;
            for (int i = 0; i < assocSetting.AssocSettingList.Count; i++) {
                this.tabControl.TabPages[i].Text = GetTabName(i);
            }

            // 関連付け設定ごとの初期化
            m_currentPageIndex = 0;
            InitializeAssociate(m_currentPageIndex);

#if FREE_VERSION
            // Freeware版
            this.labelFreeware.Text = Resources.Dlg_FreewareInfo;
            this.labelFreeware.BackColor = Color.LightYellow;
#endif
        }

        //=========================================================================================
        // 機　能：指定された関連付け設定でUIを初期化する
        // 引　数：[in]index  関連付け設定のインデックス（0～7）
        // 戻り値：なし
        //=========================================================================================
        private void InitializeAssociate(int index) {
            AssociateKeySetting assocKey = m_keySetting.AssociateSetting.AssocSettingList[index];
            this.textBoxName.Text = GetTabName(index);
            if (assocKey == null) {
                this.textBoxKeyList.Text = GetKeyNameList(index);
                this.listBoxAssoc.Items.Clear();
            } else {
                this.textBoxKeyList.Text = GetKeyNameList(index);
                this.listBoxAssoc.Items.Clear();
                for (int i = 0; i < assocKey.AssociateExtList.Count; i++) {
                    this.listBoxAssoc.Items.Add(AssocListBoxItem.Item(i));
                }
                this.listBoxAssoc.Items.Add(AssocListBoxItem.Default);
                this.listBoxAssoc.SelectedIndex = 0;
            }
            listBoxAssoc_SelectedIndexChanged(null, null);
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：UIの有効／無効状態を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            AssociateKeySetting assocKey = m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex];
            this.textBoxName.Text = GetTabName(m_currentPageIndex);
            this.textBoxKeyList.Enabled = true;
            this.listBoxAssoc.Enabled = true;
            this.buttonAdd.Enabled = true;
            this.buttonEdit.Enabled = true;
            int extIndex = this.listBoxAssoc.SelectedIndex;
            AssocListBoxItem itemType = (AssocListBoxItem)(this.listBoxAssoc.Items[extIndex]);
            if (itemType == AssocListBoxItem.Default) {
                this.buttonDelete.Enabled = false;
            } else {
                this.buttonDelete.Enabled = true;
            }
        }
        
        //=========================================================================================
        // 機　能：タブのページが切り替えられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void tabControl_SelectedIndexChanged(object sender, EventArgs evt) {
            // 現在のページの入力値を検証
            bool success = ValidateSetting();
            if (!success) {
                this.tabControl.SelectedIndexChanged -= new EventHandler(tabControl_SelectedIndexChanged);
                this.tabControl.SelectedIndex = m_currentPageIndex;
                this.tabControl.SelectedIndexChanged += new EventHandler(tabControl_SelectedIndexChanged);
                return;
            }

            // ページを切り替え
            m_currentPageIndex = this.tabControl.SelectedIndex;
            InitializeAssociate(m_currentPageIndex);
        }
        
        //=========================================================================================
        // 機　能：ダイアログの入力値が正しいかどうかを確認する
        // 引　数：なし
        // 戻り値：入力値が正しいときtrue
        //=========================================================================================
        private bool ValidateSetting() {
            AssociateKeySetting currentSetting = m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex];

            // 名前
            if (currentSetting.DislayName == "") {
                InfoBox.Warning(this, Resources.DlgAssocSetting_ValidateDisplayNameNone);
                return false;
            }
            if (currentSetting.DislayName.IndexOfAny(new char[] {'(', ')', '&'}) != -1) {
                InfoBox.Warning(this, Resources.DlgAssocSetting_ValidateDisplayNameChar);
                return false;
            }

            // 関連付け（フォルダ）
            int indexFolderWindows = -1;                        // Windowsのフォルダの設定が行われた設定のインデックス（-1:なし）
            int indexFolderSSH = -1;                            // SSHのフォルダの設定が行われた設定のインデックス（-1:なし）
            for (int i = 0; i < currentSetting.AssociateExtList.Count; i++) {
                if (currentSetting.AssociateExtList[i].ExtList != null) {
                    continue;
                }
                FileSystemID fileSystem = currentSetting.AssociateExtList[i].FileSystem;
                int indexMulti = -1;                            // 見つけた重複設定のインデックス（-1:なし）
                if (fileSystem == FileSystemID.None) {
                    if (indexFolderWindows != -1) {
                        indexMulti = indexFolderWindows;
                    } else if (indexFolderSSH != -1) {
                        indexMulti = indexFolderSSH;
                    }
                    indexFolderWindows = i;
                    indexFolderSSH = i;
                } else if (fileSystem == FileSystemID.Windows) {
                    if (indexFolderWindows != -1) {
                        indexMulti = indexFolderWindows;
                    }
                    indexFolderWindows = i;
                } else if (fileSystem == FileSystemID.SFTP) {
                    if (indexFolderSSH != -1) {
                        indexMulti = indexFolderSSH;
                    }
                    indexFolderSSH = i;
                }
                if (indexMulti != -1) {
                    this.listBoxAssoc.SelectedIndex = i;
                    InfoBox.Warning(this, Resources.DlgAssocSetting_ValidateMultiFolderFileSystem, i + 1, indexMulti);
                    return false;
                }
            }

            // 関連付け（自身の重複）
            for (int i = 0; i < currentSetting.AssociateExtList.Count; i++) {
                string[] extList = currentSetting.AssociateExtList[i].ExtList;
                if (extList == null) {
                    continue;
                }

                // 空のチェック
                if (extList.Length == 0) {
                    this.listBoxAssoc.SelectedIndex = i;
                    InfoBox.Warning(this, Resources.DlgAssocSetting_ValidateExtListNone, i + 1);
                    return false;
                }
                for (int j = 0; j < extList.Length; j++) {
                    if (extList[j] == "") {
                        this.listBoxAssoc.SelectedIndex = i;
                        InfoBox.Warning(this, Resources.DlgAssocSetting_ValidateExtListNone, i + 1);
                        return false;
                    }
                }

                // 重複の確認
                Dictionary<string, int> extToExtPos = new Dictionary<string, int>();
                for (int j = 0; j < extList.Length; j++) {
                    if (extToExtPos.ContainsKey(extList[j])) {
                        this.listBoxAssoc.SelectedIndex = i;
                        InfoBox.Warning(this, Resources.DlgAssocSetting_ValidateExtSeftMulti, i + 1, extList[j]);
                        return false;
                    }
                    extToExtPos.Add(extList[j], j);
                }
            }


            // 関連付け
            Dictionary<KeyValuePair<FileSystemID, string>, int> extToIndex = new Dictionary<KeyValuePair<FileSystemID, string>, int>();
            for (int i = 0; i < currentSetting.AssociateExtList.Count; i++) {
                string[] extList = currentSetting.AssociateExtList[i].ExtList;
                if (extList == null) {
                    continue;
                }

                // 重複のチェック
                FileSystemID fileSystem = currentSetting.AssociateExtList[i].FileSystem;
                for (int j = 0; j < extList.Length; j++) {
                    FileSystemID[] fileSystemList;
                    if (fileSystem == FileSystemID.Windows) {
                        fileSystemList = new FileSystemID[] { FileSystemID.Windows };
                    } else if (fileSystem == FileSystemID.SFTP) {
                        fileSystemList = new FileSystemID[] { FileSystemID.SFTP };
                    } else {
                        fileSystemList = new FileSystemID[] { FileSystemID.Windows, FileSystemID.SFTP };
                    }
                    foreach (FileSystemID checkSys in fileSystemList) {
                        KeyValuePair<FileSystemID, string> sysExt = new KeyValuePair<FileSystemID, string>(checkSys, extList[j]);
                        if (extToIndex.ContainsKey(sysExt)) {
                            int prevIndex = extToIndex[sysExt];
                            this.listBoxAssoc.SelectedIndex = i;
                            InfoBox.Warning(this, Resources.DlgAssocSetting_ValidateExtListMulti, i + 1, prevIndex + 1, extList[j]);
                            return false;
                        }
                        extToIndex.Add(sysExt, i);
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：オプションボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOption_Click(object sender, EventArgs evt) {
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add(CreateOptionMenuItem(Resources.DlgAssocSetting_MenuOriginal, MENUITEM_ORIGINAL));
            cms.Items.Add(CreateOptionMenuItem(Resources.DlgAssocSetting_MenuDefault, MENUITEM_DEFAULT));
            cms.Items.Add(new ToolStripSeparator());
            for (int i = 0; i < m_keySetting.AssociateSetting.AssocSettingList.Count; i++) {
                string dispName = string.Format(Resources.DlgAssocSetting_MenuCopyFrom, m_keySetting.AssociateSetting.AssocSettingList[i].DislayName, (i + 1).ToString());
                string tag = MENUITEM_COPYFROM + i;
                ToolStripMenuItem menuItem = CreateOptionMenuItem(dispName, tag);
                menuItem.Enabled = (i != m_currentPageIndex);
                cms.Items.Add(menuItem);
            }
            Point pos = new Point(this.buttonOption.Location.X + this.buttonOption.Size.Width, this.buttonOption.Location.Y);
            pos = this.PointToClient(this.panelPage.PointToScreen(pos));
            this.ContextMenuStrip = cms;
            this.ContextMenuStrip.Show(this, pos);
        }

        //=========================================================================================
        // 機　能：オプションメニューの項目を作成する
        // 引　数：[in]menuName 項目名
        // 　　　　[in]tag      Tagプロパティに設定する内部情報
        // 戻り値：なし
        //=========================================================================================
        private ToolStripMenuItem CreateOptionMenuItem(string menuItem, string tag) {
            ToolStripMenuItem item = new ToolStripMenuItem(menuItem, null, new EventHandler(this.PopupMenuItem_Click));
            item.Tag = tag;
            return item;
        }

        //=========================================================================================
        // 機　能：オプションメニューの項目がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void PopupMenuItem_Click(object sender, EventArgs evt) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            string tag = (string)(menuItem.Tag);
            if (tag == MENUITEM_ORIGINAL) {
                m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex] = (AssociateKeySetting)(m_assocSettingOrg.AssocSettingList[m_currentPageIndex].Clone());
                InitializeAssociate(m_currentPageIndex);
            } else if (tag == MENUITEM_DEFAULT) {
                AssociateKeySetting keySetting = m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex];
                KeySetting.InitializeAssociateDefault(keySetting, m_currentPageIndex, true);
                InitializeAssociate(m_currentPageIndex);
            } else {
                int fromIndex = int.Parse(tag.Substring(MENUITEM_COPYFROM.Length));
                string oldName = m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex].DislayName;
                m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex] = (AssociateKeySetting)(m_keySetting.AssociateSetting.AssocSettingList[fromIndex].Clone());
                m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex].DislayName = oldName;
                InitializeAssociate(m_currentPageIndex);
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            bool success = ValidateSetting();
            if (!success) {
                return;
            }

            // 項目をソート
            foreach (AssociateKeySetting keySetting in m_keySetting.AssociateSetting.AssocSettingList) {
                keySetting.SortSetting();
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        
        //=========================================================================================
        // 機　能：規定値に戻すボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonReset_Click(object sender, EventArgs evt) {
            // 戻してよいか確認
            KeySettingResetConfirmDialog confirmDialog = new KeySettingResetConfirmDialog();
            confirmDialog.InitializeForAssociate();
            DialogResult result = confirmDialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }

            // リセットを実行
            for (int i = 0; i < m_keySetting.AssociateSetting.AssocSettingList.Count; i++) {
                AssociateKeySetting keySetting = m_keySetting.AssociateSetting.AssocSettingList[i];
                KeySetting.InitializeAssociateDefault(keySetting, i, true);
            }
            InitializeAssociate(m_currentPageIndex);

            // 処理完了
            InfoBox.Information(this, Resources.DlgKeySetting_AssociateResetDone);

            DialogResult = DialogResult.OK;
            Close();
        }

//*****************************************************************************************
// 表示用ヘルパー
//*****************************************************************************************

        //=========================================================================================
        // 機　能：指定された関連付け設定が割り当てられているキーの表示名を返す
        // 引　数：[in]index  関連付け設定のインデックス（0～7）
        // 戻り値：キーの表示名
        //=========================================================================================
        private string GetKeyNameList(int id) {
            string commandClass = UIResource.OpenFileAssociateItems[id].CommandType.FullName;
            List<KeyItemSetting> keySettingList = m_keySetting.FileListKeyItemList.GetSettingFromCommandClass(commandClass);

            if (keySettingList == null) {
                return Resources.DlgAssocSetting_KeySettingListNone;
            } else {
                StringBuilder keyName = new StringBuilder();
                foreach (KeyItemSetting keySetting in keySettingList) {
                    if (keyName.Length > 0) {
                        keyName.Append(", ");
                    }
                    keyName.Append(keySetting.KeyState.GetDisplayName(m_keySetting.FileListKeyItemList));
                }

                return keyName.ToString();
            }
        }

        //=========================================================================================
        // 機　能：指定された関連付け設定の項目名を返す
        // 引　数：[in]index  関連付け設定のインデックス（0～7）
        // 戻り値：関連付け設定の項目名
        //=========================================================================================
        private string GetTabName(int index) {
            string tabName;
            if (m_keySetting.AssociateSetting.AssocSettingList[index] == null) {
                List<UIResource> assocUI = UIResource.OpenFileAssociateItems;
                tabName = assocUI[index].Hint;
            } else {
                tabName = m_keySetting.AssociateSetting.AssocSettingList[index].DislayName;
            }
            return tabName;
        }

        //=========================================================================================
        // 機　能：拡張子のリストを返す
        // 引　数：[in]typeIndex  関連付け項目の拡張子単位のインデックス
        // 戻り値：拡張子のリストの「;」区切り表示用文字列
        //=========================================================================================
        private string GetExtList(int typeIndex) {
            string[] extList = m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex].AssociateExtList[typeIndex].ExtList;
            string extDisp = StringUtils.CombineStringArray(extList, EXT_SEPARATOR);
            return extDisp;
        }
        
        //=========================================================================================
        // 機　能：ファイルシステムを表示用文字列に変換する
        // 引　数：[in]fileSystem  ファイルシステム
        // 戻り値：表示用文字列
        //=========================================================================================
        private string FileSystemToDisplayString(FileSystemID fileSystem) {
            string result;
            if (fileSystem == FileSystemID.Windows) {
                result = Resources.DlgAssocSetting_FileSystemWindows;
            } else if (fileSystem == FileSystemID.SFTP) {
                result = Resources.DlgAssocSetting_FileSystemSSH;
            } else {
                result = Resources.DlgAssocSetting_FileSystemAll;
            }
            return result;
        }

        //=========================================================================================
        // 機　能：ファイルシステムをコンボボックスの項目のインデックスに変換する
        // 引　数：[in]fileSystem  ファイルシステム
        // 戻り値：コンボボックスの項目のインデックス
        //=========================================================================================
        private int FileSystemToComboBoxIndex(FileSystemID fileSystem) {
            int index;
            if (fileSystem == FileSystemID.Windows) {
                index = 0;
            } else if (fileSystem == FileSystemID.SFTP) {
                index = 1;
            } else {
                index = 2;
            }
            return index;
        }

        //=========================================================================================
        // 機　能：コンボボックスの項目のインデックスをファイルシステムに変換する
        // 引　数：[in]index  コンボボックスの項目のインデックス
        // 戻り値：ファイルシステム
        //=========================================================================================
        private FileSystemID ComboBoxIndexToFileSystem(int index) {
            switch (index) {
                case 0:
                    return FileSystemID.Windows;
                case 1:
                    return FileSystemID.SFTP;
                default:
                    return FileSystemID.None;
            }
        }

//*****************************************************************************************
// リストボックス
//*****************************************************************************************

        //=========================================================================================
        // 機　能：追加ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonAdd_Click(object sender, EventArgs evt) {
            AssociateKeySetting assocKey = m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex];
            int extIndex = this.listBoxAssoc.SelectedIndex;
            AssocListBoxItem itemType = (AssocListBoxItem)(this.listBoxAssoc.Items[extIndex]);

            // 初期値を準備
            int listCount = this.listBoxAssoc.Items.Count;
            AssociateKeySetting.AssociateInfo assocItem = new AssociateKeySetting.AssociateInfo();
            assocItem.CommandMoniker = new ActionCommandMoniker(ActionCommandOption.None, typeof(NopCommand));
            assocItem.ExtList = new string[] {".txt"};
            assocItem.FileSystem = FileSystemID.None;

            // 入力
            AssociateDetailDialog dialog = new AssociateDetailDialog(m_keySetting, m_commandScene, assocItem, false);
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }

            // データを登録
            assocKey.AssociateExtList.Add(assocItem);

            // UIの書き換え
            this.listBoxAssoc.Items.Insert(listCount - 1, AssocListBoxItem.Item(listCount - 1));        // 追加は常にitemType.Indexの最後へ
            this.listBoxAssoc.SelectedIndex = listCount - 1;

            this.listBoxAssoc.Invalidate();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：編集ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonEdit_Click(object sender, EventArgs evt) {
            AssociateKeySetting assocKey = m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex];
            int extIndex = this.listBoxAssoc.SelectedIndex;
            AssocListBoxItem itemType = (AssocListBoxItem)(this.listBoxAssoc.Items[extIndex]);

            // 初期値を準備
            int listCount = this.listBoxAssoc.Items.Count;
            bool isDefault;
            AssociateKeySetting.AssociateInfo assocItem = new AssociateKeySetting.AssociateInfo();
            if (itemType == AssocListBoxItem.Default) {
                assocItem.CommandMoniker = assocKey.GetDefaultCommand();
                assocItem.ExtList = null;
                assocItem.FileSystem = FileSystemID.None;
                isDefault = true;
            } else {
                assocItem = (AssociateKeySetting.AssociateInfo)(assocKey.AssociateExtList[itemType.Index].Clone());
                isDefault = false;
            }

            // 入力
            AssociateDetailDialog dialog = new AssociateDetailDialog(m_keySetting, m_commandScene, assocItem, isDefault);
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }

            // データを登録
            if (itemType == AssocListBoxItem.Default) {
                assocKey.SetDefaultCommand(assocItem.CommandMoniker);
            } else {
                assocKey.AssociateExtList[itemType.Index] = assocItem;
            }
            this.listBoxAssoc.Invalidate();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            AssociateKeySetting assocKey = m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex];

            int extIndex = this.listBoxAssoc.SelectedIndex;
            AssocListBoxItem itemType = (AssocListBoxItem)(this.listBoxAssoc.Items[extIndex]);
            if (itemType == AssocListBoxItem.Default) {
                return;
            }

            // UIの書き換え
            int listCount = this.listBoxAssoc.Items.Count;
            if (this.listBoxAssoc.SelectedIndex == listCount - 2) {
                this.listBoxAssoc.SelectedIndex = this.listBoxAssoc.SelectedIndex + 1;
            }
            this.listBoxAssoc.Items.RemoveAt(listCount - 2);        // 削除は常にitemType.Indexの最後から

            // データの書き換え
            int assocExtIndex = itemType.Index;
            assocKey.AssociateExtList.RemoveAt(assocExtIndex);

            this.listBoxAssoc.Invalidate();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：リストボックスの項目を描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxAssoc_DrawItem(object sender, DrawItemEventArgs evt) {
            int index = evt.Index;
            if (index == -1) {
                return;
            }

            // 描画
            DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, evt.Bounds.Width, evt.Bounds.Height);
            doubleBuffer.SetDrawOrigin(-evt.Bounds.Left, -evt.Bounds.Top);
            OwnerDrawListBoxGraphics g = new OwnerDrawListBoxGraphics(doubleBuffer.DrawingGraphics, evt.Bounds.Top, evt.Bounds.Height);

            Brush backBrush;
            Pen borderPen;
            
            // 背景色を決定
            if ((evt.State & DrawItemState.Focus) == DrawItemState.Focus || (evt.State & DrawItemState.Selected) == DrawItemState.Selected) {
                backBrush = g.MarkBackBrush;
                borderPen = g.BorderPen;
            } else {
                backBrush = SystemBrushes.Window;
                borderPen = g.BorderPen;
            }

            // 描画
            int cx = evt.Bounds.Width;
            int cy = evt.Bounds.Height;
            bool drawBottom = false;
            if (index == this.listBoxAssoc.Items.Count - 1) {
                cy--;
                drawBottom = true;
            }
            Rectangle rect = new Rectangle(evt.Bounds.Left, evt.Bounds.Top, evt.Bounds.Width - 1, cy);
            g.Graphics.FillRectangle(backBrush, rect);
            g.Graphics.DrawLine(borderPen, new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top));
            g.Graphics.DrawLine(borderPen, new Point(rect.Left, rect.Top), new Point(rect.Left, rect.Bottom));
            g.Graphics.DrawLine(borderPen, new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom));
            if (drawBottom) {
                g.Graphics.DrawLine(borderPen, new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Bottom));
            }
            
            // 描画対象の情報を取得
            string dispName;
            AssociateKeySetting assocSetting = m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex];
            ActionCommandMoniker moniker;
            AssocListBoxItem itemType = (AssocListBoxItem)(this.listBoxAssoc.Items[index]);
            if (itemType == AssocListBoxItem.Default) {                     // デフォルトの関連付け
                dispName = Resources.DlgAssocSetting_AssocItemDefault;
                moniker = assocSetting.GetDefaultCommand();
            } else {
                string[] extList = assocSetting.AssociateExtList[itemType.Index].ExtList;
                string strFileSystem = FileSystemToDisplayString(assocSetting.AssociateExtList[itemType.Index].FileSystem);
                if (extList == null) {                                      // フォルダに対する関連付け
                    dispName = string.Format(Resources.DlgAssocSetting_AssocItemFolder, strFileSystem);
                } else {                                                    // 一般の関連付け
                    string strExtList = GetExtList(itemType.Index);
                    dispName = string.Format(Resources.DlgAssocSetting_AssocItemExt, strFileSystem, strExtList);
                }
                moniker = assocSetting.AssociateExtList[itemType.Index].CommandMoniker;
            }
            ActionCommand command;
            if (moniker == null) {
                command = new NopCommand();
            } else {
                command = (ActionCommand)(moniker.CommandType.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
            }
            string dispCommand = string.Format(Resources.DlgAssocSetting_AssocItemCommand, command.UIResource.Hint);

            // 出力
            int x = evt.Bounds.Left + 4;
            int y = evt.Bounds.Top + 6;
            g.Graphics.DrawString(dispName, this.listBoxAssoc.Font, SystemBrushes.WindowText, new Point(x, y));
            y += 16;
            g.Graphics.DrawString(dispCommand, this.listBoxAssoc.Font, SystemBrushes.WindowText, new Point(x + 8, y));

            doubleBuffer.FlushScreen(evt.Bounds.Left, evt.Bounds.Top);
        }
        
        //=========================================================================================
        // 機　能：リストボックスの高さを取得する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxAssoc_MeasureItem(object sender, MeasureItemEventArgs evt) {
            evt.ItemHeight = 40;
        }

        //=========================================================================================
        // 機　能：リストボックスの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxAssoc_SelectedIndexChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

//*****************************************************************************************
// 各種入力の変更
//*****************************************************************************************

        //=========================================================================================
        // 機　能：項目名のテキストが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxName_TextChanged(object sender, EventArgs evt) {
            AssociateKeySetting assocSetting = m_keySetting.AssociateSetting.AssocSettingList[m_currentPageIndex];
            assocSetting.DislayName = this.textBoxName.Text;
        }

        //=========================================================================================
        // クラス：リストボックスの項目のobjectに登録する内容
        //=========================================================================================
        private class AssocListBoxItem {
            public static readonly AssocListBoxItem Default =  new AssocListBoxItem(-1);        // デフォルトの関連付け
            
            // 対応する関連付け項目のインデックス（デフォルトの関連付けのとき-1）
            public int Index;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]index  対応する関連付け項目のインデックス（デフォルトの関連付けのとき-1）
            // 戻り値：なし
            //=========================================================================================
            private AssocListBoxItem(int index) {
                Index = index;
            }

            //=========================================================================================
            // 機　能：フォルダ/拡張子の項目を返す
            // 引　数：[in]index  対応する関連付け項目のインデックス（デフォルトの関連付けのとき-1）
            // 戻り値：リストボックスに格納する項目
            //=========================================================================================
            public static AssocListBoxItem Item(int index) {
                return new AssocListBoxItem(index);
            }
        }
    }
}
