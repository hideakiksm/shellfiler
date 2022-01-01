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
    // クラス：メニュー項目を入力するダイアログ
    //=========================================================================================
    public partial class MenuItemDialog : Form {
        // コマンド仕様のXML解析結果
        private CommandScene m_commandScene;

        // コマンドのツリーのノードをプログラムから設定しているときtrue
        private bool m_setCommandTreeNode = false;

        // 入力した項目名（カスタマイズしないときnull）
        private string m_itemName;

        // 入力したショートカットキー
        private char m_shortcutKey;

        // 入力した実行コマンド
        private ActionCommandMoniker m_resultCommand;

        // 親階層のメニュー一覧
        private List<MenuItemSetting> m_parentMenuList;

        // 編集中のメニュー項目（新規作成のときnull）
        private MenuItemSetting m_editTargetMenu;

        // ショートカットキーのコンボボックスの実装
        private LasyComboBoxImpl m_comboboxImplShortcut;

        // 現在選択中の機能
        private ActionCommandMoniker m_currentApi;

        // コマンドノードのTag
        // ・グループ：CommandGroup（一括作成）
        // ・機能：CommandApi（動的作成、m__classNameToTreeNodeでのインデックス）

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]commandScene  コマンド仕様のXML解析結果
        // 　　　　[in]itemName      入力した項目名の初期値（カスタマイズしないときnull）
        // 　　　　[in]shortcutKey   ショートカットキーの初期値
        // 　　　　[in]parentList    親階層のメニュー一覧（重複チェック用）
        // 　　　　[in]editTarget    編集中のメニュー項目（新規作成のときnull）
        // 戻り値：なし
        //=========================================================================================
        public MenuItemDialog(CommandScene commandScene, string itemName, char shortcutKey, List<MenuItemSetting> parentList, MenuItemSetting editTarget) {
            InitializeComponent();
            m_commandScene = commandScene;
            m_parentMenuList = parentList;
            m_editTargetMenu = editTarget;

            if (editTarget != null) {
                m_currentApi = editTarget.ActionCommandMoniker;
            } else {
                m_currentApi = new ActionCommandMoniker(ActionCommandOption.None, typeof(NopCommand));
            }

            // 項目名とショートカット
            if (itemName == null) {
                this.checkBoxItemName.Checked = false;
                if (editTarget == null) {
                    this.textBoxItemName.Text = "";
                } else {
                    this.textBoxItemName.Text = editTarget.UIResource.Hint;
                }
            } else {
                this.checkBoxItemName.Checked = true;
                this.textBoxItemName.Text = itemName;
            }
            int shortcutIndex = MenuListSettingDialog.ShortcutToIndex(shortcutKey);
            m_comboboxImplShortcut = new LasyComboBoxImpl(this.comboBoxShortcut, MenuListSettingDialog.SHORTCUT_ITEMS, shortcutIndex);
            m_comboboxImplShortcut.SelectedIndexChanged += new EventHandler(comboBoxShortcut_SelectedIndexChanged);
            comboBoxShortcut_SelectedIndexChanged(this, null);
        }

        //=========================================================================================
        // 機　能：ダイアログが読み込まれたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MenuItemDialog_Load(object sender, EventArgs evt) {
            this.treeViewCommand.ImageList = UIIconManager.IconImageList;

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

            SelectCommand(m_currentApi);
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：メニュー項目名をUIに設定する
        // 引　数：[in]moniker   選択中コマンドのモニカ
        // 戻り値：なし
        //=========================================================================================
        private void SetItemName(ActionCommandMoniker moniker) {
            if (!this.checkBoxItemName.Checked) {
                ActionCommand actionCommand = moniker.CreateActionCommand();
                string commandDisplayName = actionCommand.UIResource.Hint;
                this.textBoxItemName.Text = commandDisplayName;
            }
        }

        //=========================================================================================
        // 機　能：UIの有効／無効状態を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            // メニュー
            this.textBoxItemName.Enabled = this.checkBoxItemName.Checked;

            // コマンド
            TreeNode currentNode = this.treeViewCommand.SelectedNode;
            if (currentNode == this.treeViewCommand.Nodes[0] || currentNode != null && currentNode.Tag is CommandApi) {         // コマンド
                this.buttonAssign.Enabled = true;
            } else {                                                            // コマンドグループ
                this.buttonAssign.Enabled = false;
            }
        }

        //=========================================================================================
        // 機　能：ショートカットキーのコンボボックスの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxShortcut_SelectedIndexChanged(object sender, EventArgs evt) {
            if (m_comboboxImplShortcut == null) {
                return;
            }
            int index = m_comboboxImplShortcut.SelectedIndex;
            bool unique = MenuListSettingDialog.CheckShortcutUnique(index, m_parentMenuList, m_editTargetMenu);
            this.labelShortcutWarning.Visible = !unique;
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
                    ActionCommand actionCommand = api.Moniker.CreateActionCommand();
                    string commandDisplayName = actionCommand.UIResource.Hint;
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
            if (this.treeViewCommand.SelectedNode.Tag is CommandApi) {
                // コマンドが選択されたとき
                CommandApi api = (CommandApi)(this.treeViewCommand.SelectedNode.Tag);
                object[] defaultParam = new object[api.ArgumentList.Count];
                for (int i = 0; i < defaultParam.Length; i++) {
                    defaultParam[i] = Resources.DlgKeySetting_ParameterDummy;
                }
                SetCommandExplanation(api, defaultParam);
                SetItemName(api.Moniker);
            } else if (this.treeViewCommand.SelectedNode == this.treeViewCommand.Nodes[0]) {
                // 機能なしが選択されたとき
                this.textBoxExplanation.Text = Resources.DlgKeySetting_CommandNoneExplain;
                SetItemName(new ActionCommandMoniker(ActionCommandOption.None, typeof(NopCommand)));
            }
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：コマンド一覧の項目をオーナードローで描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewCommand_DrawNode(object sender, DrawTreeNodeEventArgs evt) {
            if (evt.Bounds.Width <= 0 || evt.Bounds.Height <= 0) {
                evt.DrawDefault = true;
                return;
            }

            // ダブルバッファを用意
            TreeNode node = evt.Node;
            DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, evt.Bounds.Width, evt.Bounds.Height);
            doubleBuffer.SetDrawOrigin(-evt.Bounds.Left, -evt.Bounds.Top);
            KeySettingGraphics g = new KeySettingGraphics(doubleBuffer.DrawingGraphics);
            try {
                // 描画の準備
                Brush textBrush, backBrush;
                if ((evt.State & TreeNodeStates.Focused) == TreeNodeStates.Focused) {
                    textBrush = SystemBrushes.HighlightText;
                    backBrush = SystemBrushes.Highlight;
                } else if ((evt.State & TreeNodeStates.Selected) == TreeNodeStates.Selected && !this.treeViewCommand.Focused) {
                    textBrush = SystemBrushes.WindowText;
                    backBrush = g.DisabledSelectionBrush;
                } else {
                    textBrush = SystemBrushes.WindowText;
                    backBrush = SystemBrushes.Window;
                }

                // 描画
                string text = node.Text;
                g.Graphics.FillRectangle(backBrush, evt.Bounds);
                g.Graphics.DrawString(text, this.treeViewCommand.Font, textBrush, new Point(evt.Bounds.Left, evt.Bounds.Top + 2));
                
            } finally {
                g.Dispose();
            }

            // ダブルバッファの内容を反映
            doubleBuffer.FlushScreen(evt.Bounds.Left, evt.Bounds.Top);
        }

        //=========================================================================================
        // 機　能：割り当てボタンがクリックされたの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonAssign_Click(object sender, EventArgs evt) {
            // 項目名
            if (this.checkBoxItemName.Checked) {
                m_itemName = this.textBoxItemName.Text;
                if (m_itemName.Trim().Length == 0) {
                    InfoBox.Warning(this, Resources.DlgMenuSetting_NoMenuItemName);
                    return;
                }
            } else {
                m_itemName = null;
            }

            // ショートカット
            m_shortcutKey = MenuListSettingDialog.IndexToShortcut(m_comboboxImplShortcut.SelectedIndex);

            // コマンド
            ActionCommandMoniker newMoniker;
            if (this.treeViewCommand.SelectedNode == this.treeViewCommand.Nodes[0]) {
                // 「機能なし」の割り当ての場合
                newMoniker = new ActionCommandMoniker(ActionCommandOption.None, typeof(NopCommand));
            } else if (this.treeViewCommand.SelectedNode == null || !(this.treeViewCommand.SelectedNode.Tag is CommandApi)) {
                // 無効ノードは無視
                return;
            } else {
                // 機能の割り当ての場合
                CommandApi api = (CommandApi)(this.treeViewCommand.SelectedNode.Tag);
                ActionCommandMoniker newCommand = api.Moniker;
                ActionCommandMoniker oldCommand = m_currentApi;

                // 条件の入力
                KeySettingOptionDialog settingOptionDialog = new KeySettingOptionDialog(api, m_commandScene.CommandSceneType, null, oldCommand, newCommand, null);
                DialogResult result = settingOptionDialog.ShowDialog(this);
                if (result != DialogResult.OK) {
                    return;
                }
                object[] param = settingOptionDialog.ParamList;
                ActionCommandOption option = settingOptionDialog.CommandOption;
                newMoniker = new ActionCommandMoniker(option, newCommand.CommandType, param);
            }
            m_resultCommand = newMoniker;

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：指定されたコマンドのノードを選択状態にする
        // 引　数：[in]moniker    選択するコマンドのモニカ（null:割り当てなしを選択）
        // 戻り値：なし
        //=========================================================================================
        private void SelectCommand(ActionCommandMoniker moniker) {
            // 割り当てなし
            if (moniker == null || moniker.CommandType == typeof(NopCommand)) {
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
                        if (apiTag != null && apiTag.CommandClassName == className) {
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
        // プロパティ：入力した項目名（カスタマイズしないときnull）
        //=========================================================================================
        public string ResultItemName {
            get {
                return m_itemName;
            }
        }

        //=========================================================================================
        // プロパティ：入力したショートカットキー
        //=========================================================================================
        public char ResultShortcutKey {
            get {
                return m_shortcutKey;
            }
        }

        //=========================================================================================
        // プロパティ：入力した実行コマンド
        //=========================================================================================
        public ActionCommandMoniker ResultCommand {
            get {
                return m_resultCommand;
            }
        }
    }
}
