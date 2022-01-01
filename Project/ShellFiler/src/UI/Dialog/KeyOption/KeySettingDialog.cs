using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Properties;
using ShellFiler.Locale;
using ShellFiler.UI.ControlBar;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：キー設定変更ダイアログ
    // 　　　　シーンを外部から指定することで、ファイル一覧、ファイルビューア、グラフィックビューア
    // 　　　　の３つに対応する。
    //=========================================================================================
    public partial class KeySettingDialog : Form {
        // キー一覧の設定全体
        private KeySetting m_keySetting;

        // 対象となっているキー一覧
        private KeyItemSettingList m_keySettingList;

        // 変更の対象となっているシーン
        private CommandUsingSceneType m_keySettingScene;

        // コマンド一覧
        private CommandScene m_commandScene;

        // コマンドのツリーのノードをプログラムから設定しているときtrue
        private bool m_setCommandTreeNode = false;

        // 無効状態で選択中のブラシ
        private Brush m_disabledSelectionBrush;

        // キーノードのTag
        // ・カテゴリ：KeyNameUtils.KeyNameCategory（一括作成）
        // ・キー：KeyNameUtils.KeyNameItem（動的作成）
        // ・キー＋シフト：KeyState（動的作成）
        //
        // リストビューのTag
        // ・全項目：KeyItemSetting（一括作成）
        //
        // コマンドノードのTag
        // ・グループ：CommandGroup（一括作成）
        // ・機能：CommandApi（動的作成、m__classNameToTreeNodeでのインデックス）

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]keySetting   キー一覧の設定全体
        // 　　　　[in]commandSpec  コマンド仕様のXML解析結果
        // 　　　　[in]scene        変更の対象となっているシーン
        // 戻り値：なし
        //=========================================================================================
        public KeySettingDialog(KeySetting keySetting, CommandSpec commandSpec, CommandUsingSceneType scene) {
            InitializeComponent();
            m_keySetting = keySetting;
            m_keySettingScene = scene;
            string title = "";
            switch (scene) {
                case CommandUsingSceneType.FileList:
                    m_keySettingList = keySetting.FileListKeyItemList;
                    m_commandScene = commandSpec.FileList;
                    title = Resources.MenuName_FileListKeySetting;
                    break;
                case CommandUsingSceneType.FileViewer:
                    m_keySettingList = keySetting.FileViewerKeyItemList;
                    m_commandScene = commandSpec.FileViewer;
                    title = Resources.MenuName_FileViewerKeySetting;
                    break;
                case CommandUsingSceneType.GraphicsViewer:
                    m_keySettingList = keySetting.GraphicsViewerKeyItemList;
                    m_commandScene = commandSpec.GraphicsViewer;
                    title = Resources.MenuName_GraphicsViewerKeySetting;
                    break;
            }
            this.Text = title.Split(',')[0];
            
            this.treeViewAllKey.ImageList = UIIconManager.IconImageList;
            this.listViewDefinedKey.SmallImageList = UIIconManager.IconImageList;
            this.treeViewCommand.ImageList = UIIconManager.IconImageList;

            m_disabledSelectionBrush = new SolidBrush(Color.FromArgb(192, 192, 192));

            // 全キーのツリー（ルートノード）
            List<KeyNameUtils.KeyNameCategory> keyCategoryList = KeyNameUtils.KeyCategoryList;
            List<TreeNode> nodeListKey = new List<TreeNode>();
            foreach (KeyNameUtils.KeyNameCategory category in keyCategoryList) {
                TreeNode node = new TreeNode(category.CategoryName, (int)IconImageListID.Icon_KeySettingCategory, (int)IconImageListID.Icon_KeySettingCategory);
                node.Tag = category;
                nodeListKey.Add(node);
                node.Nodes.Add(new TreeNode());             // ダミー（「+」の表示用、BeforeExpandで削除）
            } 
            this.treeViewAllKey.Nodes.AddRange(nodeListKey.ToArray());

            // 定義済みキーのカラムを初期化
            // リストはタブ切り替え時に初期化
            ColumnHeader column = new ColumnHeader();
            column.Text = " ";
            column.Width = this.listViewDefinedKey.Width - 32;
            this.listViewDefinedKey.Columns.AddRange(new ColumnHeader[] {column});

            // コマンド一覧
            List<TreeNode> nodeListGroup = new List<TreeNode>();
            TreeNode nodeApiNone = new TreeNode(Resources.DlgKeySetting_ApiNone, (int)IconImageListID.Icon_KeySettingApiNone, (int)IconImageListID.Icon_KeySettingApiNone);
            nodeListGroup.Add(nodeApiNone);
            foreach (CommandGroup commandGroup in m_commandScene.CommandGroup) {
                TreeNode node = new TreeNode(commandGroup.GroupDisplayName, (int)IconImageListID.Icon_KeySettingApiGroup, (int)IconImageListID.Icon_KeySettingApiGroup);
                node.Tag = commandGroup;
                nodeListGroup.Add(node);
                node.Nodes.Add(new TreeNode());             // ダミー（「+」の表示用、BeforeExpandで削除）
            }
            this.treeViewCommand.Nodes.AddRange(nodeListGroup.ToArray());

#if FREE_VERSION
            // Freeware版
            this.labelFreeware.Text = Resources.Dlg_FreewareInfo;
            this.labelFreeware.BackColor = Color.LightYellow;
#endif
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void KeySettingDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            m_disabledSelectionBrush.Dispose();
            m_disabledSelectionBrush = null;
        }

//*****************************************************************************************
// 共通部
//*****************************************************************************************

        //=========================================================================================
        // 機　能：キー一覧のタブが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void tabControlKeyList_SelectedIndexChanged(object sender, EventArgs evt) {
            UpdateButtonEnabled();
            // 割り当て済みのキーの初回切り替えで初期化する
            if (this.tabControlKeyList.SelectedIndex == 1) {
                if (this.listViewDefinedKey.Items.Count == 0) {
                    InitializeDefinedKey();
                }
            }
        }

        //=========================================================================================
        // 機　能：キー一覧のタブコントロールの項目をオーナードローで描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void tabControlKeyList_DrawItem(object sender, DrawItemEventArgs evt) {
            TabControl tab = (TabControl)sender;
            string text = tab.TabPages[evt.Index].Text;

            StringFormat sf = new StringFormat();
            try {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                if ((evt.State & DrawItemState.Selected) == DrawItemState.Selected) {
                    // 選択状態
                    evt.Graphics.FillRectangle(SystemBrushes.Window, evt.Bounds);
                    evt.Graphics.DrawString(text, evt.Font, SystemBrushes.WindowText, evt.Bounds, sf);
                } else {
                    // 選択されていない状態
                    evt.Graphics.DrawString(text, evt.Font, SystemBrushes.WindowText, evt.Bounds, sf);
                }
            } finally {
                sf.Dispose();
            }
        }

//*****************************************************************************************
// すべてのキーのツリー
//*****************************************************************************************

        //=========================================================================================
        // 機　能：キー一覧のツリーが開かれようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewKeyList_BeforeExpand(object sender, TreeViewCancelEventArgs evt) {
            TreeNode targetNode = evt.Node;
            if (targetNode.Tag is KeyNameUtils.KeyNameCategory && targetNode.Nodes.Count == 1) {
                // キーカテゴリの初回の展開
                targetNode.Nodes.Clear();
                List<TreeNode> nodeList = new List<TreeNode>();
                KeyNameUtils.KeyNameCategory group = (KeyNameUtils.KeyNameCategory)(evt.Node.Tag);
                foreach (KeyNameUtils.KeyNameItem item in group.KeyNameItemList) {
                    TreeNode node = new TreeNode(item.DisplayName, (int)IconImageListID.Icon_KeySettingKeyGroup, (int)IconImageListID.Icon_KeySettingKeyGroup);
                    node.Tag = item;
                    node.Nodes.Add(new TreeNode());        // ダミー（「+」の表示用、BeforeExpandで削除）
                    nodeList.Add(node);
                }
                targetNode.Nodes.AddRange(nodeList.ToArray());
            } else if (evt.Node.Tag is KeyNameUtils.KeyNameItem && targetNode.Nodes.Count == 1) {
                // キーグループの初回の展開
                targetNode.Nodes.Clear();
                KeyNameUtils.KeyNameItem keyNameItem = (KeyNameUtils.KeyNameItem)(evt.Node.Tag);
                List<TreeNode> nodeList = new List<TreeNode>();
                nodeList.Add(CreateTreeNodeKeyState(keyNameItem, false, false, false, TwoStrokeType.None));
                nodeList.Add(CreateTreeNodeKeyState(keyNameItem, true,  false, false, TwoStrokeType.None));
                nodeList.Add(CreateTreeNodeKeyState(keyNameItem, false, true,  false, TwoStrokeType.None));
                nodeList.Add(CreateTreeNodeKeyState(keyNameItem, true,  true,  false, TwoStrokeType.None));
                nodeList.Add(CreateTreeNodeKeyState(keyNameItem, false, false, true,  TwoStrokeType.None));
                nodeList.Add(CreateTreeNodeKeyState(keyNameItem, true,  false, true,  TwoStrokeType.None));
                nodeList.Add(CreateTreeNodeKeyState(keyNameItem, false, true,  true,  TwoStrokeType.None));
                nodeList.Add(CreateTreeNodeKeyState(keyNameItem, true,  true,  true,  TwoStrokeType.None));
                nodeList.Add(CreateTreeNodeKeyState(keyNameItem, false, false, false, TwoStrokeType.Key1));
                nodeList.Add(CreateTreeNodeKeyState(keyNameItem, false, false, false, TwoStrokeType.Key2));
                nodeList.Add(CreateTreeNodeKeyState(keyNameItem, false, false, false, TwoStrokeType.Key3));
                nodeList.Add(CreateTreeNodeKeyState(keyNameItem, false, false, false, TwoStrokeType.Key4));
                targetNode.Nodes.AddRange(nodeList.ToArray());
            }
        }

        //=========================================================================================
        // 機　能：シフト＋キーのレベルでの、ツリーのノードを作成する
        // 引　数：[in]keyNameItem  キー名
        // 　　　　[in]shift        シフトキーが押されているときのノードを作成する場合true
        // 　　　　[in]ctrl         Ctrlキーが押されているときのノードを作成する場合true
        // 　　　　[in]alt          Altキーが押されているときのノードを作成する場合true
        // 　　　　[in]twoStrok     ２ストロークキーの状態
        // 戻り値：作成したツリーノード
        //=========================================================================================
        private TreeNode CreateTreeNodeKeyState(KeyNameUtils.KeyNameItem keyNameItem, bool shift, bool ctrl, bool alt, TwoStrokeType twoStroke) {
            KeyState keyState;
            if (twoStroke == TwoStrokeType.None) {
                keyState = new KeyState(keyNameItem.KeyCode, shift, ctrl, alt);
            } else {
                keyState = new KeyState(keyNameItem.KeyCode, twoStroke);
            }
            string keyName = GetKeyDisplayName(keyState);
            TreeNode node = new TreeNode(keyName, (int)IconImageListID.Icon_KeySettingKey, (int)IconImageListID.Icon_KeySettingKey);
            node.Tag = keyState;
            return node;
        }

        //=========================================================================================
        // 機　能：シフト＋キーのレベルでの、ノードの表示名を作成する
        // 引　数：[in]keyState  目的のキー
        // 戻り値：ノードの表示名
        //=========================================================================================
        private string GetKeyDisplayName(KeyState keyState) {
            KeyItemSetting itemSetting = m_keySettingList.GetSettingFromKey(keyState);
            string keyName;
            if (itemSetting == null) {
                keyName = keyState.GetDisplayNameKey(m_keySettingList);
            } else {
                string commandName = itemSetting.ActionCommandMoniker.CreateActionCommand().UIResource.Hint;
                keyName = keyState.GetDisplayNameKey(m_keySettingList) + " : " + commandName;
            }
            return keyName;
        }

        //=========================================================================================
        // 機　能：すべてのキー一覧の項目をオーナードローで描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewKeyList_DrawNode(object sender, DrawTreeNodeEventArgs evt) {
            if (evt.Bounds.Width <= 0 || evt.Bounds.Height <= 0 || m_disabledSelectionBrush == null) {
                evt.DrawDefault = true;
                return;
            }
            TreeNode node = evt.Node;

            // ダブルバッファを用意
            DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, evt.Bounds.Width + 4, evt.Bounds.Height);
            doubleBuffer.SetDrawOrigin(-evt.Bounds.Left, -evt.Bounds.Top);
            Graphics g = doubleBuffer.DrawingGraphics;
           
            // 描画内容を決定
            string text = node.Text;
            Brush textBrush;
            Brush backBrush;
            if ((evt.State & TreeNodeStates.Focused) == TreeNodeStates.Focused) {
                textBrush = SystemBrushes.HighlightText;
                backBrush = SystemBrushes.Highlight;
            } else if ((evt.State & TreeNodeStates.Selected) == TreeNodeStates.Selected && !this.treeViewAllKey.Focused) {
                textBrush = SystemBrushes.WindowText;
                backBrush = m_disabledSelectionBrush;
            } else {
                textBrush = SystemBrushes.WindowText;
                backBrush = SystemBrushes.Window;
            }

            Font font = null;
            try {
                // 描画
                bool bold = IsAssignedKey(node);
                if (bold) {
                    Rectangle rect = new Rectangle(evt.Bounds.Left, evt.Bounds.Top, evt.Bounds.Width + 4, evt.Bounds.Height);
                    g.FillRectangle(backBrush, rect);
                    font = new Font(this.treeViewAllKey.Font, FontStyle.Bold);
                } else {
                    Rectangle rectOver = new Rectangle(evt.Bounds.Right, evt.Bounds.Top, 4, evt.Bounds.Height);
                    g.FillRectangle(SystemBrushes.Window, rectOver);
                    g.FillRectangle(backBrush, evt.Bounds);
                    font = new Font(this.treeViewAllKey.Font, FontStyle.Regular);
                }
                g.DrawString(text, font, textBrush, new Point(evt.Bounds.Left, evt.Bounds.Top + 2));

                // ダブルバッファの内容を描画
                doubleBuffer.FlushScreen(evt.Bounds.Left, evt.Bounds.Top);
            } finally {

                font.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：指定されたノードの配下に割り当て済みのキーがあるときtrueを返す
        // 引　数：[in]node  調べるUIのノード
        // 戻り値：配下に割り当て済みのキーがあるときtrue
        //=========================================================================================
        private bool IsAssignedKey(TreeNode node) {
            if (node.Tag is KeyNameUtils.KeyNameCategory) {
                // カテゴリ
                KeyNameUtils.KeyNameCategory category = (KeyNameUtils.KeyNameCategory)(node.Tag);
                foreach (KeyNameUtils.KeyNameItem item in category.KeyNameItemList) {
                    Keys key = item.KeyCode;
                    if (m_keySettingList.GetSettingCountFromKeyCode(key) > 0) {
                        return true;
                    }
                }
                return false;
            } else if (node.Tag is KeyNameUtils.KeyNameItem) {
                // キー
                KeyNameUtils.KeyNameItem item = (KeyNameUtils.KeyNameItem)(node.Tag);
                int count = m_keySettingList.GetSettingCountFromKeyCode(item.KeyCode);
                return (count > 0);
            } else if (node.Tag is KeyState) {
                // キー＋シフト：KeyState
                KeyState keyState = (KeyState)(node.Tag);
                KeyItemSetting setting = m_keySettingList.GetSettingFromKey(keyState);
                return (setting != null);
            } else {
                // ダミーノード
                return false;
            }
        }

        //=========================================================================================
        // 機　能：すべてのキー一覧の項目が選択されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewKeyList_AfterSelect(object sender, TreeViewEventArgs evt) {
            TreeNode node = evt.Node;
            ActionCommandMoniker moniker = null;                // 設定済みでないときnull
            if (node.Tag is KeyState) {
                // 末端レベルの場合は割り当てられているコマンドを選択
                KeyState keyState = (KeyState)(node.Tag);
                KeyItemSetting itemSetting = m_keySettingList.GetSettingFromKey(keyState);
                if (itemSetting != null) {
                    moniker = itemSetting.ActionCommandMoniker;
                }
                SelectCommand(moniker);
            } else if (node.ImageIndex == (int)IconImageListID.Icon_KeySettingKey) {
                // 末端レベルが割り当て済みでない場合は「割り当てなし」を表示
                SelectCommand(null);
            }
            UpdateButtonEnabled();
        }

//*****************************************************************************************
// 割り当て済みのキーのリストボックス
//*****************************************************************************************

        //=========================================================================================
        // 機　能：定義済みキーの一覧のUIを初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void InitializeDefinedKey() {
            // リストビューの項目
            List<KeyItemSetting> allSetting = GetAllKeyItemSetting();
            List<ListViewItem> itemList = new List<ListViewItem>();
            foreach (KeyItemSetting settingItem in allSetting) {
                if (settingItem.KeyState.IsMouse()) {
                    continue;
                }
                string keyName = GetKeyDisplayName(settingItem.KeyState);
                ListViewItem lvItem = new ListViewItem(keyName);
                lvItem.ImageIndex = (int)IconImageListID.Icon_KeySettingKey;
                lvItem.Tag = settingItem;
                itemList.Add(lvItem);
            }
            this.listViewDefinedKey.Items.AddRange(itemList.ToArray());
        }

        //=========================================================================================
        // 機　能：リストビューの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewDefinedKey_SelectedIndexChanged(object sender, EventArgs evt) {
            UpdateButtonEnabled();
            if(this.listViewDefinedKey.SelectedItems.Count != 1) {
                return;
            }
            KeyItemSetting setting = (KeyItemSetting)(this.listViewDefinedKey.SelectedItems[0].Tag);
            ActionCommandMoniker moniker = setting.ActionCommandMoniker;
            SelectCommand(moniker);
        }

        //=========================================================================================
        // 機　能：現状でのリストビューのすべての項目のキー設定を作成する
        // 引　数：なし
        // 戻り値：すべてのキーの設定
        //=========================================================================================
        private List<KeyItemSetting> GetAllKeyItemSetting() {
            List<KeyItemSetting> allSetting = new List<KeyItemSetting>();
            foreach (KeyItemSetting setting in m_keySettingList.AllKeySettingList) {
                allSetting.Add(setting);
            }
            KeyItemSettingSorter sorter = new KeyItemSettingSorter();
            sorter.SortKeySetting(allSetting);
            return allSetting;
        }

//*****************************************************************************************
// コマンド一覧のツリー
//*****************************************************************************************

        //=========================================================================================
        // 機　能：コマンドのノードが開かれようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewCommand_BeforeExpand(object sender, TreeViewCancelEventArgs evt) {
            TreeNode targetNode = evt.Node;
            if (targetNode.Tag is CommandGroup && targetNode.Nodes.Count == 1) {
                // コマンドグループの初回の展開
                targetNode.Nodes.Clear();
                List<TreeNode> nodeList = new List<TreeNode>();
                CommandGroup group = (CommandGroup)(evt.Node.Tag);
                foreach (CommandApi api in group.FunctionList) {
                    ActionCommand actionCommand;
                    string commandDisplayName = GetCommandDisplayName(api.Moniker, out actionCommand);
                    int imageIndex = (int)IconImageListID.Icon_KeySettingApi;
                    if (actionCommand.UIResource.IconIdLeft != IconImageListID.None) {
                        imageIndex = (int)actionCommand.UIResource.IconIdLeft;
                    }
                    TreeNode node = new TreeNode(commandDisplayName, imageIndex, imageIndex);
                    node.Tag = api;
                    nodeList.Add(node);
                }
                targetNode.Nodes.AddRange(nodeList.ToArray());
            }
        }

        //=========================================================================================
        // 機　能：コマンドのノードの表示名を作成する
        // 引　数：[in]moniker         作成するコマンドのモニカ
        // 　　　　[out]actionCommand  モニカから作成したコマンド（再利用のため）
        // 戻り値：ノードの表示名
        //=========================================================================================
        private string GetCommandDisplayName(ActionCommandMoniker moniker, out ActionCommand actionCommand) {
            actionCommand = moniker.CreateActionCommand();
            StringBuilder keyList = new StringBuilder();

            // 割り当て済みのキー一覧を作成
            List<KeyItemSetting> allSetting = GetAllKeyItemSetting();
            foreach (KeyItemSetting itemSetting in allSetting) {
                if (itemSetting.ActionCommandMoniker.CommandType.Equals(moniker.CommandType)) {
                    if (keyList.Length > 0) {
                        keyList.Append(", ");
                    }
                    keyList.Append(itemSetting.KeyState.GetDisplayName(this.m_keySettingList));
                }
            }

            // 表示名として「コマンド名 : キー一覧」を作成
            string assignedKey = StringUtils.MakeOmittedString(keyList.ToString(), 100);
            if (assignedKey != "") {
                assignedKey = " : " + assignedKey;
            }
            string commandDisplayName = actionCommand.UIResource.Hint + assignedKey;
            return commandDisplayName;
        }

        //=========================================================================================
        // 機　能：指定されたコマンドのノードを選択状態にする
        // 引　数：[in]moniker    選択するコマンドのモニカ（null:割り当てなしを選択）
        // 戻り値：なし
        //=========================================================================================
        private void SelectCommand(ActionCommandMoniker moniker) {
            // 割り当てなし
            if (moniker == null) {
                m_setCommandTreeNode = true;
                this.treeViewCommand.SelectedNode = this.treeViewCommand.Nodes[0];
                m_setCommandTreeNode = false;
                SetCommandExplanation(null, null);
                return;
            }

            // 指定されたクラスがないときはエラー(XMLがおかしい)
            string className = moniker.CommandType.FullName;
            if (!m_commandScene.ClassNameToApi.ContainsKey(className)) {
                InfoBox.Error(this, Resources.DlgKeySetting_CommandClassNotFound, className);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            // クラス名→API→親のグループとして、そのノードを開く
            CommandApi api = m_commandScene.ClassNameToApi[className];
            CommandGroup targetGroup = api.ParentGroup;
            foreach (TreeNode groupNode in this.treeViewCommand.Nodes) {
                CommandGroup groupTag = (CommandGroup)(groupNode.Tag);
                if (groupTag == targetGroup) {
                    // グループノード内をAPIノードで満たしてから探す
                    groupNode.Expand();
                    foreach (TreeNode apiNode in groupNode.Nodes) {
                        CommandApi apiTag = (CommandApi)(apiNode.Tag);
                        if (apiTag.CommandClassName == className) {
                            m_setCommandTreeNode = true;
                            this.treeViewCommand.SelectedNode = apiNode;
                            m_setCommandTreeNode = false;
                            SetCommandExplanation(apiTag, moniker.Parameter);
                            return;
                        }
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：コマンドの説明をUIに設定する
        // 引　数：[in]api        コマンドの詳細情報
        // 　　　　[in]paramList  現在コマンドに割り当てられているパラメータのリスト
        // 戻り値：なし
        //=========================================================================================
        private void SetCommandExplanation(CommandApi api, object[] paramList) {
            if (api == null) {
                this.textBoxExplanation.Text = Resources.DlgKeySetting_CommandNone;
            } else {
                string comment = KeySettingOptionDialog.CreateCommandExplanation(api, paramList);
                this.textBoxExplanation.Text = comment;
            }
        }

        //=========================================================================================
        // 機　能：コマンドのノードが選択されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewCommand_AfterSelect(object sender, TreeViewEventArgs evt) {
            if (m_setCommandTreeNode) {
                return;
            }
            UpdateButtonEnabled();
            if (this.treeViewCommand.SelectedNode.Tag is CommandApi) {
                // コマンドが選択されたとき
                CommandApi api = (CommandApi)(this.treeViewCommand.SelectedNode.Tag);
                object[] defaultParam = new object[api.ArgumentList.Count];
                for (int i = 0; i < defaultParam.Length; i++) {
                    defaultParam[i] = Resources.DlgKeySetting_ParameterDummy;
                }
                SetCommandExplanation(api, defaultParam);
            } else if (this.treeViewCommand.SelectedNode == this.treeViewCommand.Nodes[0]) {
                // 機能なしが選択されたとき
                this.textBoxExplanation.Text = Resources.DlgKeySetting_CommandNoneExplain;
            }
        }

        //=========================================================================================
        // 機　能：コマンド一覧の項目をオーナードローで描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewCommand_DrawNode(object sender, DrawTreeNodeEventArgs evt) {
            if (evt.Bounds.Width <= 0 || evt.Bounds.Height <= 0 || m_disabledSelectionBrush == null) {
                evt.DrawDefault = true;
                return;
            }

            // ダブルバッファを用意
            TreeNode node = evt.Node;
            DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, evt.Bounds.Width, evt.Bounds.Height);
            doubleBuffer.SetDrawOrigin(-evt.Bounds.Left, -evt.Bounds.Top);
            Graphics g = doubleBuffer.DrawingGraphics;

            // 描画の準備
            Brush textBrush, backBrush;
            if ((evt.State & TreeNodeStates.Focused) == TreeNodeStates.Focused) {
                textBrush = SystemBrushes.HighlightText;
                backBrush = SystemBrushes.Highlight;
            } else if ((evt.State & TreeNodeStates.Selected) == TreeNodeStates.Selected && !this.treeViewCommand.Focused) {
                textBrush = SystemBrushes.WindowText;
                backBrush = m_disabledSelectionBrush;
            } else {
                textBrush = SystemBrushes.WindowText;
                backBrush = SystemBrushes.Window;
            }

            // 描画
            string text = node.Text;
            g.FillRectangle(backBrush, evt.Bounds);
            g.DrawString(text, this.treeViewCommand.Font, textBrush, new Point(evt.Bounds.Left, evt.Bounds.Top + 2));
            
            // ダブルバッファの内容を反映
            doubleBuffer.FlushScreen(evt.Bounds.Left, evt.Bounds.Top);
        }

//*****************************************************************************************
// 全体
//*****************************************************************************************

        //=========================================================================================
        // 機　能：ボタンの有効/無効の状態を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateButtonEnabled() {
            bool keySelected;                               // キーを選択中のときtrue
            bool keyCommandExist;                           // キーに機能が割り当てられているときtrue
            ActionCommandMoniker keyMoniker = null;         // キーに割り当てられているコマンド
            ActionCommandMoniker commandMoniker = null;     // コマンドに割り当てられているコマンド
            if (this.tabControlKeyList.SelectedIndex == 0) {
                // すべてのキーを選択中
                if (this.treeViewAllKey.SelectedNode != null && this.treeViewAllKey.SelectedNode.Tag is KeyState) {
                    KeyState selectedKey = (KeyState)(this.treeViewAllKey.SelectedNode.Tag);
                    KeyItemSetting keySetting = m_keySettingList.GetSettingFromKey(selectedKey);
                    keySelected = true;
                    if (keySetting != null) {
                        keyCommandExist = true;
                        keyMoniker = keySetting.ActionCommandMoniker;
                    } else {
                        keyCommandExist = false;
                    }
                } else {
                    keySelected = false;
                    keyCommandExist = false;
                }
            } else {
                // 割り当て済みのキーを選択中
                if (this.listViewDefinedKey.SelectedItems.Count == 1) {
                    keySelected = true;
                    keyCommandExist = true;
                } else {
                    keySelected = false;
                    keyCommandExist = false;
                }
            }

            bool commandSelected;
            bool commandNoneSelected;
            if (this.treeViewCommand.SelectedNode != null && this.treeViewCommand.SelectedNode.Tag is CommandApi) {
                CommandApi api = (CommandApi)(this.treeViewCommand.SelectedNode.Tag);
                commandSelected = true;
                commandNoneSelected = false;
                commandMoniker = api.Moniker;
            } else if (this.treeViewCommand.SelectedNode == this.treeViewCommand.Nodes[0]) {
                commandSelected = true;
                commandNoneSelected = true;
            } else {
                commandSelected = false;
                commandNoneSelected = false;
            }

            bool assign;
            if (keySelected && commandSelected) {
                if (!keyCommandExist && commandNoneSelected) {
                    // キー：コマンド設定なし、コマンド：解除は無効
                    assign = false;
                } else {
                    // それ以外のキーとコマンドが選択されているときは有効
                    assign = true;
                }
            } else {
                assign = false;
            }

            this.buttonAssign.Enabled = assign;
            this.buttonRelease.Enabled = (keySelected && keyCommandExist);
        }

        //=========================================================================================
        // 機　能：割り当て解除ボタンがクリックされたの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonRelease_Click(object sender, EventArgs evt) {
            // 選択中のキーを取得
            KeyState selectedKey = GetSelectedKey();
            if (selectedKey == null) {
                return;
            }

            // 設定を削除
            KeyItemSetting oldItemSetting = m_keySettingList.GetSettingFromKey(selectedKey);
            m_keySettingList.DeleteSetting(selectedKey);
            if (oldItemSetting != null) {
                foreach (TreeNode nodeGroup in this.treeViewCommand.Nodes) {
                    bool breakAll = false;
                    foreach (TreeNode nodeCommand in nodeGroup.Nodes) {
                        if (nodeCommand.Tag == null) {
                            continue;
                        }
                        CommandApi api = (CommandApi)(nodeCommand.Tag);
                        if (api.Moniker.Equals(oldItemSetting.ActionCommandMoniker)) {
                            ActionCommand dummy;
                            nodeCommand.Text = GetCommandDisplayName(api.Moniker, out dummy);
                            breakAll = true;
                            break;
                        }
                    }
                    if (breakAll) {
                        break;
                    }
                }
            }

            // すべてのキーのUIを更新
            foreach (TreeNode nodeGroup in this.treeViewAllKey.Nodes) {
                bool breakAll = false;
                foreach (TreeNode nodeKeyGroup in nodeGroup.Nodes) {
                    foreach (TreeNode nodeKeyShift in nodeKeyGroup.Nodes) {
                        KeyState key = (KeyState)(nodeKeyShift.Tag);
                        if (selectedKey == key) {
                            string keyName = GetKeyDisplayName(selectedKey);
                            nodeKeyShift.Text = keyName;
                            breakAll = true;
                            break;
                        }
                    }
                }
                if (breakAll) {
                    break;
                }
            }

            // 割り当て済みのキー一覧のUIを更新
            foreach (ListViewItem lvItem in this.listViewDefinedKey.Items) {
                KeyItemSetting setting = (KeyItemSetting)(lvItem.Tag);
                if (selectedKey == setting.KeyState) {
                    this.listViewDefinedKey.Items.Remove(lvItem);
                    break;
                }
            }
            SelectCommand(null);
            UpdateButtonEnabled();
        }

        //=========================================================================================
        // 機　能：現在選択されているキーを返す
        // 引　数：なし
        // 戻り値：選択中のキー
        //=========================================================================================
        private KeyState GetSelectedKey() {
            KeyState selectedKey = null;
            if (this.tabControlKeyList.SelectedIndex == 0) {
                // すべてのキーを選択中
                if (this.treeViewAllKey.SelectedNode != null && this.treeViewAllKey.SelectedNode.Tag is KeyState) {
                    selectedKey = (KeyState)(this.treeViewAllKey.SelectedNode.Tag);
                }
            } else {
                // 割り当て済みのキーを選択中
                if (this.listViewDefinedKey.SelectedItems.Count == 1) {
                    KeyItemSetting keyItem = (KeyItemSetting)(this.listViewDefinedKey.SelectedItems[0].Tag);
                    selectedKey = keyItem.KeyState;
                }
            }
            return selectedKey;
        }

        //=========================================================================================
        // 機　能：割り当てボタンがクリックされたの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonAssign_Click(object sender, EventArgs evt) {
            // 「機能なし」の割り当ての場合
            if (this.treeViewCommand.SelectedNode == this.treeViewCommand.Nodes[0]) {
                buttonRelease_Click(null, null);
                return;
            }

            // 実行の準備
            KeyState selectedKey = GetSelectedKey();
            if (selectedKey == null) {
                return;
            }
            if (this.treeViewCommand.SelectedNode == null || !(this.treeViewCommand.SelectedNode.Tag is CommandApi)) {
                return;
            }
            CommandApi api = (CommandApi)(this.treeViewCommand.SelectedNode.Tag);
            ActionCommandMoniker newCommand = api.Moniker;
            ActionCommandMoniker oldCommand = null;
            KeyItemSetting itemSetting =  m_keySettingList.GetSettingFromKey(selectedKey);
            if (itemSetting != null) {
                oldCommand = itemSetting.ActionCommandMoniker;
            }

            // 条件の入力
            string dispFuncOld = null;
            if (itemSetting != null) {
                dispFuncOld = itemSetting.DisplayName;
            }
            KeySettingOptionDialog settingOptionDialog = new KeySettingOptionDialog(api, m_commandScene.CommandSceneType, selectedKey, oldCommand, newCommand, dispFuncOld);
            DialogResult result = settingOptionDialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }
            object[] param = settingOptionDialog.ParamList;
            ActionCommandOption option = settingOptionDialog.CommandOption;
            string dispFunc = settingOptionDialog.DisplayNameFunctionBar;

            // キーの入れ替え
            if (oldCommand != null) {
                m_keySettingList.DeleteSetting(selectedKey);
            }
            KeyItemSetting newItemSetting = new KeyItemSetting(selectedKey, new ActionCommandMoniker(option, newCommand.CommandType, param), dispFunc);
            m_keySettingList.AddSetting(newItemSetting);

            // コマンド一覧のUIに反映
            Type newCommandType = newCommand.CommandType;
            Type oldCommandType = null;
            if (oldCommand != null) {
                oldCommandType = oldCommand.CommandType;
            }
            foreach (TreeNode nodeGroup in this.treeViewCommand.Nodes) {
                foreach (TreeNode nodeCommand in nodeGroup.Nodes) {
                    if (nodeCommand.Tag == null) {
                        continue;
                    }
                    CommandApi nodeCommandApi = (CommandApi)(nodeCommand.Tag);
                    Type apiType = api.Moniker.CommandType;
                    if (apiType.Equals(newCommandType) || apiType.Equals(oldCommandType)) {
                        ActionCommand dummy;
                        nodeCommand.Text = GetCommandDisplayName(nodeCommandApi.Moniker, out dummy);
                    }
                }
            }

            // すべてのキーのUIに反映
            foreach (TreeNode nodeGroup in this.treeViewAllKey.Nodes) {
                bool breakAll = false;
                foreach (TreeNode nodeKeyGroup in nodeGroup.Nodes) {
                    foreach (TreeNode nodeKeyShift in nodeKeyGroup.Nodes) {
                        KeyState key = (KeyState)(nodeKeyShift.Tag);
                        if (selectedKey == key) {
                            string keyName = GetKeyDisplayName(selectedKey);
                            nodeKeyShift.Text = keyName;
                            breakAll = true;
                            break;
                        }
                    }
                }
                if (breakAll) {
                    break;
                }
            }

            // 割り当て済みのキー一覧のUIを更新
            KeyItemSettingSorter sorter = new KeyItemSettingSorter();
            int lvItemCount = this.listViewDefinedKey.Items.Count;
            for (int i = 0; i < lvItemCount; i++) {
                ListViewItem lvItem = this.listViewDefinedKey.Items[i];
                KeyItemSetting settingItem = (KeyItemSetting)(lvItem.Tag);
                int comp = sorter.CompareKey(settingItem.KeyState, selectedKey);
                if (comp == 0) {
                    lvItem.Text = GetKeyDisplayName(settingItem.KeyState);
                    lvItem.Tag = newItemSetting;
                    break;
                } else if (comp == 1) {
                    string keyName = GetKeyDisplayName(selectedKey);
                    ListViewItem newItem = new ListViewItem(keyName);
                    newItem.ImageIndex = (int)IconImageListID.Icon_KeySettingKey;
                    newItem.Tag = newItemSetting;
                    this.listViewDefinedKey.Items.Insert(i, newItem);
                    break;
                }
            }
        }

        //=========================================================================================
        // 機　能：規定値に戻すボタンがクリックされたの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDefault_Click(object sender, EventArgs evt) {
            // 戻してよいか確認
            KeySettingResetConfirmDialog confirmDialog = new KeySettingResetConfirmDialog();
            confirmDialog.InitializeForKeySetting(m_keySettingScene);
            DialogResult result = confirmDialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }

            // リセットを実行
            switch (m_keySettingScene) {
                case CommandUsingSceneType.FileList:
                    m_keySetting.InitializeFileListKeySetting();
                    break;
                case CommandUsingSceneType.FileViewer:
                    m_keySetting.InitializeFileViewerKeySetting();
                    break;
                case CommandUsingSceneType.GraphicsViewer:
                    m_keySetting.InitializeGraphicsViewerKeySetting();
                    break;
            }

            // 処理完了
            InfoBox.Information(this, Resources.DlgKeySetting_ResetDone);

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：キャンセルボタンがクリックされたの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCancel_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
