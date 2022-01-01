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
    public partial class AssociateDetailDialog : Form {
        // 拡張子のセパレータ
        private const string EXT_SEPARATOR = ";";

        // キー一覧の設定全体
        private KeySetting m_keySetting;

        // コマンド仕様のXML解析結果
        private CommandScene m_commandScene;

        // コマンドのツリーのノードをプログラムから設定しているときtrue
        private bool m_setCommandTreeNode = false;

        // 編集対象の項目（最後に1度だけ内容を書き換えて戻る、途中の書き換えはオプション変更での新旧の比較ができなくなるため禁止）
        private AssociateKeySetting.AssociateInfo m_associateItem;

        // デフォルトの関連づけを編集するときtrue
        private bool m_isDefault;

        // コマンドノードのTag
        // ・グループ：CommandGroup（一括作成）
        // ・機能：CommandApi（動的作成、m__classNameToTreeNodeでのインデックス）

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]keySetting   キー一覧の設定全体
        // 　　　　[in]commandScene コマンド仕様のXML解析結果
        // 　　　　[in]assocItem    編集対象の項目（内容を書き換えて戻る）
        // 　　　　[in]isDefault    デフォルトの関連づけを編集するときtrue
        // 戻り値：なし
        //=========================================================================================
        public AssociateDetailDialog(KeySetting keySetting, CommandScene commandScene, AssociateKeySetting.AssociateInfo assocItem, bool isDefault) {
            InitializeComponent();
            m_keySetting = keySetting;
            m_commandScene = commandScene;
            m_associateItem = assocItem;
            m_isDefault = isDefault;
        }
        
        //=========================================================================================
        // 機　能：ダイアログが初期化されるときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        // メ　モ：ツリーの初期化はダイアログ作成後でないと成功しないため、コンストラクタの処理を移植。
        //=========================================================================================
        private void AssociateSettingDialog_Load(object sender, EventArgs evt) {
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

            // 拡張子
            string[] extList = {
                Resources.DlgAssocSetting_AssocItemFolderComboBox,
            };
            this.comboBoxExt.Items.AddRange(extList);
            if (m_associateItem.ExtList == null) {
                this.comboBoxExt.Text = Resources.DlgAssocSetting_AssocItemFolderComboBox;
            } else {
                this.comboBoxExt.Text = StringUtils.CombineStringArray(m_associateItem.ExtList, EXT_SEPARATOR);
            }

            // ファイルシステム
            string[] fileSystemList = {
                Resources.DlgAssocSetting_FileSystemWindows,
                Resources.DlgAssocSetting_FileSystemSSH,
                Resources.DlgAssocSetting_FileSystemAll,
            };
            this.comboBoxFileSystem.Items.AddRange(fileSystemList);
            this.comboBoxFileSystem.SelectedIndex = FileSystemToComboBoxIndex(m_associateItem.FileSystem);

            SelectCommand(m_associateItem.CommandMoniker);
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：UIの有効／無効状態を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            if (m_isDefault) {                                                  // デフォルトの関連付け
                this.comboBoxExt.Enabled = false;
                this.comboBoxFileSystem.Enabled = false;
            } else {
                if (m_associateItem.ExtList == null) {                          // フォルダ
                    this.comboBoxExt.Enabled = false;
                    this.comboBoxFileSystem.Enabled = true;
                } else {                                                        // 通常ファイル
                    this.comboBoxExt.Enabled = true;
                    this.comboBoxFileSystem.Enabled = true;
                }
            }

            TreeNode currentNode = this.treeViewCommand.SelectedNode;
            if (currentNode == this.treeViewCommand.Nodes[0] || currentNode != null && currentNode.Tag is CommandApi) {         // コマンド
                this.buttonAssign.Enabled = true;
            } else {                                                            // コマンドグループ
                this.buttonAssign.Enabled = false;
            }
        }

//*****************************************************************************************
// 表示用ヘルパー
//*****************************************************************************************
        
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
// コマンド一覧のツリー
//*****************************************************************************************

        //=========================================================================================
        // 機　能：コマンドのノードが開かれようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewCommand_BeforeExpand(object sender, TreeViewCancelEventArgs evt) {
            HashSet<string> commandSkip = new HashSet<string>();
            commandSkip.Add(typeof(OpenFileAssociate1Command).FullName);
            commandSkip.Add(typeof(OpenFileAssociate2Command).FullName);
            commandSkip.Add(typeof(OpenFileAssociate3Command).FullName);
            commandSkip.Add(typeof(OpenFileAssociate4Command).FullName);
            commandSkip.Add(typeof(OpenFileAssociate5Command).FullName);
            commandSkip.Add(typeof(OpenFileAssociate6Command).FullName);
            commandSkip.Add(typeof(OpenFileAssociate7Command).FullName);
            commandSkip.Add(typeof(OpenFileAssociate8Command).FullName);

            TreeNode targetNode = evt.Node;
            if (targetNode.Tag is CommandGroup && targetNode.Nodes.Count == 1) {
                // コマンドグループの初回の展開
                targetNode.Nodes.Clear();
                List<TreeNode> nodeList = new List<TreeNode>();
                CommandGroup group = (CommandGroup)(evt.Node.Tag);
                foreach (CommandApi api in group.FunctionList) {
                    if (commandSkip.Contains(api.Moniker.CommandType.FullName)) {
                        continue;
                    }
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
            } else if (this.treeViewCommand.SelectedNode == this.treeViewCommand.Nodes[0]) {
                // 機能なしが選択されたとき
                this.textBoxExplanation.Text = Resources.DlgKeySetting_CommandNoneExplain;
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
            ActionCommandMoniker newMoniker;
            if (this.treeViewCommand.SelectedNode == this.treeViewCommand.Nodes[0]) {
                // 「機能なし」の割り当ての場合
                // 削除の案内
                if (!m_isDefault) {
                    DialogResult result = InfoBox.Question(this, MessageBoxButtons.OKCancel, Resources.DlgAssocSetting_SelectDeleteOrRelease);
                    if (result != DialogResult.OK) {
                        return;
                    }
                }

                newMoniker = new ActionCommandMoniker(ActionCommandOption.None, typeof(NopCommand));
            } else if (this.treeViewCommand.SelectedNode == null || !(this.treeViewCommand.SelectedNode.Tag is CommandApi)) {
                // 無効ノードは無視
                return;
            } else {
                // 機能の割り当ての場合
                CommandApi api = (CommandApi)(this.treeViewCommand.SelectedNode.Tag);
                ActionCommandMoniker newCommand = api.Moniker;
                ActionCommandMoniker oldCommand = m_associateItem.CommandMoniker;

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

            // フォルダと拡張子
            string[] extList;
            string comboText = this.comboBoxExt.Text;
            if (comboText == Resources.DlgAssocSetting_AssocItemFolderComboBox) {       // <フォルダ>を選択
                extList = null;
            } else {                                                                    // 拡張子を選択
                comboText = comboText.ToLower();
                extList = comboText.Split(EXT_SEPARATOR[0]);
            }

            // 置き換え
            m_associateItem.CommandMoniker = newMoniker;
            m_associateItem.ExtList = extList;
            m_associateItem.FileSystem = ComboBoxIndexToFileSystem(this.comboBoxFileSystem.SelectedIndex);

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
    }
}
