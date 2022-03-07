using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：メニュー項目項目一覧の編集ダイアログ
    //=========================================================================================
    public partial class MenuListSettingDialog : Form {
        // オーナードロー 先頭項目の水平方向のマージン
        public const int CX_TOP_MARGIN = 8;

        // オーナードロー 通常項目の水平方向のマージン
        public const int CX_MENU_MARGIN = 8;

        // オーナードロー 通常項目の左側にあるアイコン描画領域の幅
        public const int CX_LEFT_AREA = 24;

        // オーナードロー 階層メニューのインデント幅
        public const int CX_INDENT_SIZE = 16;
        
        // ショートカットキーの表示名
        public static string[] SHORTCUT_ITEMS = new string[] {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
            "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            Resources.DlgMenuSetting_ShortcutNone,
        };

        // コマンド仕様のXML解析結果
        private CommandScene m_commandScene;
        
        // 編集対象のメニュー項目（このインスタンスを編集して返す）
        private MenuItemCustomRoot m_menuItemCustomRoot;

        // 親階層のメニュー一覧（重複チェック用）
        private List<MenuItemSetting> m_parentMenuList;

        // ショートカットキーのコンボボックス
        private LasyComboBoxImpl m_comboboxImplShortcut;

        // ルートノード
        private TreeNode m_rootNode;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]commandScene    コマンド仕様のXML解析結果
        // 　　　　[in]custom          編集対象のメニュー項目（このインスタンスを編集して返す）
        // 　　　　[in]parentMenuList  親階層のメニュー一覧（重複チェック用）
        // 戻り値：なし
        //=========================================================================================
        public MenuListSettingDialog(CommandScene commandScene, MenuItemCustomRoot custom, List<MenuItemSetting> parentMenuList) {
            InitializeComponent();
            m_commandScene = commandScene;
            m_menuItemCustomRoot = custom;
            m_parentMenuList = parentMenuList;
            
            // ルート
            this.textBoxRootMenu.Text = custom.MenuSetting.ItemNameInput;
            int shortcutIndex = ShortcutToIndex(custom.MenuSetting.ShortcutKey);
            m_comboboxImplShortcut = new LasyComboBoxImpl(this.comboBoxShortcut, SHORTCUT_ITEMS, shortcutIndex);
            m_comboboxImplShortcut.SelectedIndexChanged += new EventHandler(comboBoxShortcut_SelectedIndexChanged);
            comboBoxShortcut_SelectedIndexChanged(null, null);

            // ツリーを初期化
            m_rootNode = new TreeNode();
            m_rootNode.Tag = new MenuNodeTag(NodeType.Root, custom.MenuSetting, 0);
            this.treeView.Nodes.Add(m_rootNode);
            m_rootNode.Expand();
            InitializeMenuTree(m_rootNode, custom.MenuSetting, 1);

            // フォーカス
            this.treeView.SelectedNode = m_rootNode;
            this.ActiveControl = this.treeView;

            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：メニューのツリーを初期化する
        // 引　数：[in]rootNode  ルートノード
        // 　　　　[in]rootMenu  ルートノードに対するメニュー設定（以下にポップアップとして編集対象）
        // 　　　　[in]depth     メニューの深さ（再帰）
        // 戻り値：なし
        //=========================================================================================
        private void InitializeMenuTree(TreeNode rootNode, MenuItemSetting rootMenu, int depth) {
            List<MenuItemSetting> subMenuList = rootMenu.SubMenuList;
            if (subMenuList.Count == 0) {
                // 項目なし
                TreeNode dummyNode = new TreeNode();
                dummyNode.Tag = new MenuNodeTag(NodeType.Dummy, null, depth);
                rootNode.Nodes.Add(dummyNode);
            } else {
                // 項目あり
                for (int i = 0; i < subMenuList.Count; i++) {
                    TreeNode subMenuNode = new TreeNode();
                    if (subMenuList[i].Type == MenuItemSetting.ItemType.MenuItem) {
                        subMenuNode.Tag = new MenuNodeTag(NodeType.MenuItem, subMenuList[i], depth);
                    } else if (subMenuList[i].Type == MenuItemSetting.ItemType.Separator) {
                        subMenuNode.Tag = new MenuNodeTag(NodeType.Separator, subMenuList[i], depth);
                    } else {
                        subMenuNode.Tag = new MenuNodeTag(NodeType.SubMenu, subMenuList[i], depth);
                        InitializeMenuTree(subMenuNode, subMenuList[i], depth + 1);
                        subMenuNode.Expand();
                    }
                    rootNode.Nodes.Add(subMenuNode);
                }
                TreeNode dummyNode = new TreeNode();
                dummyNode.Tag = new MenuNodeTag(NodeType.Last, null, depth);
                rootNode.Nodes.Add(dummyNode);
            }
        }

        //=========================================================================================
        // 機　能：ショートカットキーの設定値をコンボボックスのインデックスに変換する
        // 引　数：[in]shortcut  ショートカットキーの設定値
        // 戻り値：コンボボックスのインデックス
        //=========================================================================================
        public static int ShortcutToIndex(char shortcut) {
            for (int i = 0; i < SHORTCUT_ITEMS.Length - 1; i++) {
                if (SHORTCUT_ITEMS[i][0] == shortcut) {
                    return i;
                }
            }
            return SHORTCUT_ITEMS.Length - 1;
        }

        //=========================================================================================
        // 機　能：コンボボックスのインデックスをショートカットキーの設定値に変換する
        // 引　数：[in]index  コンボボックスのインデックス
        // 戻り値：ショートカットキーの設定値
        //=========================================================================================
        public static char IndexToShortcut(int index) {
            if (index == SHORTCUT_ITEMS.Length - 1) {
                return MenuItemSetting.SHORTCUT_KEY_NONE;
            } else {
                return SHORTCUT_ITEMS[index][0];
            }
        }

        //=========================================================================================
        // 機　能：ショートカットキーの設定がユニークかどうかを返す
        // 引　数：[in]selectedIndex  コンボボックスのインデックス
        // 　　　　[in]parentList     親階層のメニュー一覧
        // 　　　　[in]editTarget     編集中のメニュー項目（新規作成のときnull）
        // 戻り値：設定がユニークなときtrue
        //=========================================================================================
        public static bool CheckShortcutUnique(int selectedIndex, List<MenuItemSetting> parentList, MenuItemSetting editTarget) {
            char shortcut = IndexToShortcut(selectedIndex);
            
            // ショートカットなしは重複なし
            if (shortcut == MenuItemSetting.SHORTCUT_KEY_NONE) {
                return true;
            }

            // 重複を確認
            for (int i = 0; i < parentList.Count; i++) {
                MenuItemSetting parentItem = parentList[i];
                if (parentItem != editTarget) {
                    if (parentItem.Type == MenuItemSetting.ItemType.MenuItem || parentItem.Type == MenuItemSetting.ItemType.SubMenu) {
                        char parentShortcut = parentItem.ShortcutKey;
                        if (shortcut == parentShortcut) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            TreeNode node = this.treeView.SelectedNode;
            MenuNodeTag tag = (MenuNodeTag)(node.Tag);
            switch (tag.NodeType) {
                case NodeType.Root:
                    // メニューのルート（メインメニューに常時表示される項目）
                    this.buttonAddItem.Enabled = true;
                    this.buttonAddGroup.Enabled = true;
                    this.buttonBorder.Enabled = true;
                    this.buttonEdit.Enabled = false;
                    this.buttonDelete.Enabled = false;
                    this.buttonUp.Enabled = false;
                    this.buttonDown.Enabled = false;
                    break;
                case NodeType.MenuItem:
                    // 通常のメニュー項目
                    this.buttonAddItem.Enabled = true;
                    this.buttonAddGroup.Enabled = true;
                    this.buttonBorder.Enabled = true;
                    this.buttonEdit.Enabled = true;
                    this.buttonDelete.Enabled = true;
                    this.buttonUp.Enabled = (node != m_rootNode.Nodes[0]);
                    this.buttonDown.Enabled = !IsLastNode(node);
                    break;
                case NodeType.Separator:
                    // メニュー項目の区切り線
                    this.buttonAddItem.Enabled = true;
                    this.buttonAddGroup.Enabled = true;
                    this.buttonBorder.Enabled = true;
                    this.buttonEdit.Enabled = false;
                    this.buttonDelete.Enabled = true;
                    this.buttonUp.Enabled = (node != m_rootNode.Nodes[0]);
                    this.buttonDown.Enabled = !IsLastNode(node);
                    break;
                case NodeType.SubMenu:
                    // サブメニューを持つメニュー項目
                    this.buttonAddItem.Enabled = true;
                    this.buttonAddGroup.Enabled = true;
                    this.buttonBorder.Enabled = true;
                    this.buttonEdit.Enabled = true;
                    this.buttonDelete.Enabled = true;
                    this.buttonUp.Enabled = (node != m_rootNode.Nodes[0]);
                    this.buttonDown.Enabled = !IsLastNode(node);
                    break;
                case NodeType.Dummy:
                    // 項目がないときに「項目がありません」を表示するための項目
                    this.buttonAddItem.Enabled = true;
                    this.buttonAddGroup.Enabled = true;
                    this.buttonBorder.Enabled = true;
                    this.buttonEdit.Enabled = false;
                    this.buttonDelete.Enabled = false;
                    this.buttonUp.Enabled = false;
                    this.buttonDown.Enabled = false;
                    break;
                case NodeType.Last:
                    // 最終の項目
                    this.buttonAddItem.Enabled = true;
                    this.buttonAddGroup.Enabled = true;
                    this.buttonBorder.Enabled = true;
                    this.buttonEdit.Enabled = false;
                    this.buttonDelete.Enabled = false;
                    this.buttonUp.Enabled = false;
                    this.buttonDown.Enabled = false;
                    break;
            }
        }

        //=========================================================================================
        // 機　能：下方向に移動できる位置にあるノードかどうかを返す
        // 引　数：[in]targetNode  調べるノード
        // 戻り値：ノードが最終位置のときtrue
        //=========================================================================================
        private bool IsLastNode(TreeNode targetNode) {
            TreeNode current = m_rootNode;
            while (true) {
                TreeNode next = null;
                for (int i = current.Nodes.Count - 1; i >= 0; i--) {
                    TreeNode childNode = current.Nodes[i];
                    MenuNodeTag childTag = (MenuNodeTag)(childNode.Tag);
                    if (childTag.NodeType == NodeType.MenuItem) {
                        if (targetNode == childNode) {
                            return true;
                        } else {
                            return false;
                        }
                    } else if (childTag.NodeType == NodeType.SubMenu) {
                        if (targetNode == childNode) {
                            return true;
                        }
                        if (childNode.Nodes.Count == 1) {
                            return false;
                        }
                        next = current.Nodes[i];
                        break;
                    }
                }
                if (next == null) {
                    return false;
                }
                current = next;
            }
        }

        //=========================================================================================
        // 機　能：ツリーの項目が選択された直後の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeView_AfterSelect(object sender, TreeViewEventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：ツリーの項目の再描画が発生したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeView_DrawNode(object sender, DrawTreeNodeEventArgs evt) {
            if (evt.Bounds.Height == 0) {
                return;
            }
            DoubleBuffer buffer = new DoubleBuffer(evt.Graphics, this.treeView.Width, evt.Bounds.Height);
            MenuGraphics g = new MenuGraphics(buffer.DrawingGraphics);
            try {
                g.Graphics.FillRectangle(g.MenuBackBrush, buffer.DrawingRectangle);
                DrawNode(g, evt, buffer.DrawingRectangle);
            } finally {
                g.Dispose();
                buffer.FlushScreen(0, evt.Bounds.Top);
            }
        }

        //=========================================================================================
        // 機　能：ツリーの項目を描画する
        // 引　数：[in]g       グラフィックス
        // 　　　　[in]evt     描画イベント
        // 　　　　[in]rcItem  描画対象の項目の領域
        // 戻り値：なし
        //=========================================================================================
        private void DrawNode(MenuGraphics g, DrawTreeNodeEventArgs evt, Rectangle rcItem) {
            MenuNodeTag tag = (MenuNodeTag)(evt.Node.Tag);
            Brush textBrush, backBrush;
            bool isSelect;
            if ((evt.State & TreeNodeStates.Focused) == TreeNodeStates.Focused) {
                textBrush = SystemBrushes.HighlightText;
                backBrush = SystemBrushes.Highlight;
                isSelect = true;
            } else if ((evt.State & TreeNodeStates.Selected) == TreeNodeStates.Selected && !this.treeView.Focused) {
                textBrush = SystemBrushes.WindowText;
                backBrush = SystemBrushes.ControlDark;
                isSelect = true;
            } else {
                textBrush = SystemBrushes.WindowText;
                backBrush = SystemBrushes.Window;
                isSelect = false;
            }

            int cxItem = this.treeView.Width;
            int cyItem = evt.Bounds.Height;
            int yFont = (cyItem - this.treeView.Font.Height) / 2 + 1;
            if (tag.NodeType == NodeType.Root) {
                // メニューのルートを描画
                // （メインメニューに常時表示される項目）
                string topMenuName = m_menuItemCustomRoot.MenuSetting.ItemName.Replace("&", "");
                int cxTop = (int)(GraphicsUtils.MeasureString(g.Graphics, this.treeView.Font, topMenuName)) + CX_TOP_MARGIN;
                Rectangle rcMenuTop = new Rectangle(0, 0, cxTop + CX_TOP_MARGIN * 2, cyItem - 2);
                if (isSelect) {
                    g.Graphics.FillRectangle(backBrush, rcItem);
                } else {
                    g.Graphics.FillRectangle(g.MenuTopBrush, rcMenuTop);
                }
                g.Graphics.DrawLine(g.BorderPen, 0, 0, cxTop + CX_TOP_MARGIN * 2, 0);
                g.Graphics.DrawLine(g.BorderPen, 0, 0, 0, cyItem);
                g.Graphics.DrawLine(g.BorderPen, cxTop + CX_TOP_MARGIN * 2, 0, cxTop + CX_TOP_MARGIN * 2, cyItem);
                g.Graphics.DrawLine(g.BorderPen, cxTop + CX_TOP_MARGIN * 2, cyItem - 1, cxItem, cyItem - 1);
                g.Graphics.DrawString(topMenuName, this.treeView.Font, textBrush, new Point(CX_TOP_MARGIN, yFont));
            } else {
                // ルート以外の項目を描画
                // 領域の背景と垂直の枠線
                if (isSelect) {
                    g.Graphics.FillRectangle(backBrush, rcItem);
                }
                int indent = tag.Depth;
                for (int i = 0; i < indent; i++) {
                    if (tag.NodeType == NodeType.Dummy && i == indent - 1) {            // 「項目がありません」
                        Rectangle rcLeftArea = new Rectangle(1 + i * CX_INDENT_SIZE, 0, CX_LEFT_AREA, cyItem);
                        if (!isSelect) {
                            g.Graphics.FillRectangle(g.MenuBackBrush, rcLeftArea);
                        }
                        g.Graphics.DrawLine(g.BorderPen, rcLeftArea.Left - 1, 0, rcLeftArea.Left - 1, rcLeftArea.Bottom);
                    } else if (tag.NodeType == NodeType.Last && i == indent - 1) {      // 最後の項目の一番内側
                        Rectangle rcLeftArea1 = new Rectangle(1 + i * CX_INDENT_SIZE, 0, CX_LEFT_AREA, cyItem / 2);
                        if (!isSelect) {
                            g.Graphics.FillRectangle(g.LeftAreaBrush(rcLeftArea1), rcLeftArea1);
                        }
                        g.Graphics.DrawLine(g.BorderPen, rcLeftArea1.Left - 1, 0, rcLeftArea1.Left - 1, rcLeftArea1.Bottom);
                    } else {                                                            // 通常項目
                        Rectangle rcLeftArea = new Rectangle(1 + i * CX_INDENT_SIZE, 0, CX_LEFT_AREA, cyItem);
                        if (!isSelect) {
                            g.Graphics.FillRectangle(g.LeftAreaBrush(rcLeftArea), rcLeftArea);
                        }
                        g.Graphics.DrawLine(g.BorderPen, rcLeftArea.Left - 1, 0, rcLeftArea.Left - 1, rcLeftArea.Bottom);
                    }
                }

                // 水平の枠線
                if (tag.NodeType == NodeType.SubMenu) {
                    g.Graphics.DrawLine(g.BorderPen, indent * CX_INDENT_SIZE, cyItem - 1, cxItem - 1, cyItem - 1);
                } else if (tag.NodeType == NodeType.Last) {
                    g.Graphics.DrawLine(g.BorderPen, (indent - 1) * CX_INDENT_SIZE, cyItem / 2, cxItem - 1, cyItem / 2);
                } else if (tag.NodeType == NodeType.Dummy) {
                    g.Graphics.DrawLine(g.BorderPen, (indent - 1) * CX_INDENT_SIZE, cyItem - 1, cxItem - 1, cyItem - 1);
                }

                // 項目の内容
                int xPos = (indent - 1) * CX_INDENT_SIZE + CX_LEFT_AREA + CX_MENU_MARGIN;
                string menuName = null;                     // nullのとき境界線
                int idIcon = 0;                             // 0のときアイコンなし
                if (tag.NodeType == NodeType.MenuItem) {
                    // 通常のメニュー項目
                    menuName = tag.MenuSetting.ItemName.Replace("&", "");
                    idIcon = (int)(tag.MenuSetting.UIResource.IconIdLeft);
                } else if (tag.NodeType == NodeType.Separator) {
                    // メニュー項目の区切り線
                } else if (tag.NodeType == NodeType.SubMenu) {
                    // サブメニューを持つメニュー項目
                    menuName = tag.MenuSetting.ItemName.Replace("&", "");
                } else if (tag.NodeType == NodeType.Dummy) {
                    menuName = Resources.DlgMenuSetting_NoMenuItem;
                } else if (tag.NodeType == NodeType.Last) {
                    menuName = "";
                }
                if (menuName == null) {
                    // 境界線
                    int yPosBar = cyItem / 2;
                    g.Graphics.DrawLine(g.SeparatorLightPen, xPos, yPosBar, cxItem - 2, yPosBar);
                    g.Graphics.DrawLine(g.SeparatorDarkPen, xPos, yPosBar + 1, cxItem - 2, yPosBar + 1);
                } else {
                    // 項目
                    g.Graphics.DrawString(menuName, this.treeView.Font, textBrush, new Point(xPos, yFont));
                    if (idIcon != 0) {
                        int xPosIcon = xPos - CX_LEFT_AREA - CX_MENU_MARGIN + (CX_LEFT_AREA - UIIconManager.CxDefaultIcon) / 2;
                        int yPosIcon = (cyItem - UIIconManager.CyDefaultIcon) / 2;
                        UIIconManager.IconImageList.Draw(g.Graphics, new Point(xPosIcon, yPosIcon), idIcon);
                    }

                    // ポップアップの三角
                    if (tag.NodeType == NodeType.SubMenu) {
                        Bitmap bmp = UIIconManager.ButtonFace_PopupMenu;
                        int xPosTriangle = xPos + (int)(GraphicsUtils.MeasureString(g.Graphics, this.treeView.Font, menuName)) + CX_MENU_MARGIN * 2;
                        int yPosTriangle = (cyItem - bmp.Height) / 2;
                        g.Graphics.DrawImage(bmp, xPosTriangle, yPosTriangle, bmp.Size.Width, bmp.Size.Height);
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：ツリーの項目が閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeView_BeforeCollapse(object sender, TreeViewCancelEventArgs evt) {
            evt.Cancel = true;
        }

        //=========================================================================================
        // 機　能：メニュー項目の追加ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonAddItem_Click(object sender, EventArgs evt) {
            TreeNode currentNode = this.treeView.SelectedNode;
            if (currentNode == null) {
                return;
            }
            MenuNodeTag currentTag = (MenuNodeTag)(currentNode.Tag);
            int currentNodeIndex = currentNode.Index;
            TreeNode parentNode = currentNode.Parent;
            List<MenuItemSetting> parentList;
            if (currentNode == m_rootNode) {
                parentList = m_menuItemCustomRoot.MenuSetting.SubMenuList;
                parentNode = m_rootNode;
            } else {
                parentList = ((MenuNodeTag)(parentNode.Tag)).MenuSetting.SubMenuList;
            }

            MenuItemDialog dialog = new MenuItemDialog(m_commandScene, null, MenuItemSetting.SHORTCUT_KEY_NONE, parentList, null);
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }

            TreeNode newNode = new TreeNode();
            MenuItemSetting newMenu = new MenuItemSetting(dialog.ResultCommand, dialog.ResultShortcutKey, dialog.ResultItemName);
            MenuNodeTag newNodeTag = new MenuNodeTag(NodeType.MenuItem, newMenu, currentTag.Depth);
            newNode.Tag = newNodeTag;

            switch (currentTag.NodeType) {
                case NodeType.Root:                         // メニューのルート（メインメニューに常時表示される項目）
                    if (((MenuNodeTag)(currentNode.Nodes[0].Tag)).NodeType == NodeType.Dummy) {
                        currentNode.Nodes.Clear();
                    }
                    newNodeTag.Depth++;
                    m_menuItemCustomRoot.MenuSetting.SubMenuList.Insert(0, newMenu);
                    currentNode.Nodes.Insert(0, newNode);
                    if (currentNode.Nodes.Count == 1) {
                        TreeNode newNodeLast = new TreeNode();
                        newNodeLast.Tag = new MenuNodeTag(NodeType.Last, null, newNodeTag.Depth);
                        currentNode.Nodes.Add(newNodeLast);
                    }
                    break;
                case NodeType.MenuItem:                     // 通常のメニュー項目
                case NodeType.Separator:                    // メニュー項目の区切り線
                case NodeType.SubMenu:                      // サブメニューを持つメニュー項目
                    currentNode.Parent.Nodes.Insert(currentNodeIndex + 1, newNode);
                    parentList.Insert(currentNodeIndex + 1, newMenu);
                    break;
                case NodeType.Last:                         // 最終の項目
                    currentNode.Parent.Nodes.Insert(currentNodeIndex - 1, newNode);
                    parentList.Insert(currentNodeIndex - 1, newMenu);
                    break;
                case NodeType.Dummy: {                      // 項目がないときに「項目がありません」を表示するための項目
                    TreeNode lastNode = new TreeNode();
                    lastNode.Tag = new MenuNodeTag(NodeType.Last, null, currentTag.Depth);
                    parentNode.Nodes.Clear();
                    parentNode.Nodes.Add(newNode);
                    parentNode.Nodes.Add(lastNode);
                    parentList.Add(newMenu);
                    break;
                }
            }
            this.treeView.SelectedNode = newNode;
        }

        //=========================================================================================
        // 機　能：ポップアップグループの追加ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonAddGroup_Click(object sender, EventArgs evt) {
            TreeNode currentNode = this.treeView.SelectedNode;
            if (currentNode == null) {
                return;
            }
            MenuNodeTag currentTag = (MenuNodeTag)(currentNode.Tag);
            int currentNodeIndex = currentNode.Index;
            TreeNode parentNode = currentNode.Parent;
            List<MenuItemSetting> parentList;
            if (currentNode == m_rootNode) {
                parentList = m_menuItemCustomRoot.MenuSetting.SubMenuList;
                parentNode = m_rootNode;
            } else {
                parentList = ((MenuNodeTag)(parentNode.Tag)).MenuSetting.SubMenuList;
            }

            MenuGroupDialog dialog = new MenuGroupDialog(Resources.DlgMenuSetting_NewGroupName, MenuItemSetting.SHORTCUT_KEY_NONE, parentList, null);
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }

            TreeNode newNode = new TreeNode();
            MenuItemSetting newMenu = new MenuItemSetting(dialog.GroupName, dialog.ShortcutKey);
            MenuNodeTag newNodeTag = new MenuNodeTag(NodeType.SubMenu, newMenu, currentTag.Depth);
            newNode.Tag = newNodeTag;

            TreeNode newNodeDummy = new TreeNode();
            MenuNodeTag newNodeDummyTag = new MenuNodeTag(NodeType.Dummy, null, currentTag.Depth + 1);
            newNodeDummy.Tag = newNodeDummyTag;
            newNode.Nodes.Add(newNodeDummy);

            switch (currentTag.NodeType) {
                case NodeType.Root:                         // メニューのルート（メインメニューに常時表示される項目）
                    if (((MenuNodeTag)(currentNode.Nodes[0].Tag)).NodeType == NodeType.Dummy) {
                        currentNode.Nodes.Clear();
                    }
                    newNodeTag.Depth++;
                    newNodeDummyTag.Depth++;
                    m_menuItemCustomRoot.MenuSetting.SubMenuList.Insert(0, newMenu);
                    currentNode.Nodes.Insert(0, newNode);
                    if (currentNode.Nodes.Count == 1) {
                        TreeNode newNodeLast = new TreeNode();
                        newNodeLast.Tag = new MenuNodeTag(NodeType.Last, null, newNodeTag.Depth);
                        currentNode.Nodes.Add(newNodeLast);
                    }
                    break;
                case NodeType.MenuItem:                     // 通常のメニュー項目
                case NodeType.Separator:                    // メニュー項目の区切り線
                case NodeType.SubMenu:                      // サブメニューを持つメニュー項目
                    currentNode.Parent.Nodes.Insert(currentNodeIndex + 1, newNode);
                    parentList.Insert(currentNodeIndex + 1, newMenu);
                    break;
                case NodeType.Last:                         // 最終の項目
                    currentNode.Parent.Nodes.Insert(currentNodeIndex - 1, newNode);
                    parentList.Insert(currentNodeIndex - 1, newMenu);
                    break;
                case NodeType.Dummy: {                      // 項目がないときに「項目がありません」を表示するための項目
                    TreeNode lastNode = new TreeNode();
                    lastNode.Tag = new MenuNodeTag(NodeType.Last, null, currentTag.Depth);
                    parentNode.Nodes.Clear();
                    parentNode.Nodes.Add(newNode);
                    parentNode.Nodes.Add(lastNode);
                    parentList.Add(newMenu);
                    break;
                }
            }
            this.treeView.SelectedNode = newNodeDummy;
            newNode.Expand();
        }

        //=========================================================================================
        // 機　能：境界線項目の追加ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonBorder_Click(object sender, EventArgs evt) {
            TreeNode currentNode = this.treeView.SelectedNode;
            if (currentNode == null) {
                return;
            }
            MenuNodeTag currentTag = (MenuNodeTag)(currentNode.Tag);
            int currentNodeIndex = currentNode.Index;
            TreeNode parentNode = currentNode.Parent;
            List<MenuItemSetting> parentList;
            if (currentNode == m_rootNode) {
                parentList = m_menuItemCustomRoot.MenuSetting.SubMenuList;
            } else {
                parentList = ((MenuNodeTag)(parentNode.Tag)).MenuSetting.SubMenuList;
            }

            TreeNode newNode = new TreeNode();
            MenuItemSetting newMenu = new MenuItemSetting(MenuItemSetting.ItemType.Separator);
            MenuNodeTag newNodeTag = new MenuNodeTag(NodeType.Separator, newMenu, currentTag.Depth);
            newNode.Tag = newNodeTag;

            switch (currentTag.NodeType) {
                case NodeType.Root:                         // メニューのルート（メインメニューに常時表示される項目）
                    if (((MenuNodeTag)(currentNode.Nodes[0].Tag)).NodeType == NodeType.Dummy) {
                        currentNode.Nodes.Clear();
                    }
                    newNodeTag.Depth++;
                    m_menuItemCustomRoot.MenuSetting.SubMenuList.Insert(0, newMenu);
                    currentNode.Nodes.Insert(0, newNode);
                    if (currentNode.Nodes.Count == 1) {
                        TreeNode newNodeLast = new TreeNode();
                        newNodeLast.Tag = new MenuNodeTag(NodeType.Last, null, newNodeTag.Depth);
                        currentNode.Nodes.Add(newNodeLast);
                    }
                    break;
                case NodeType.MenuItem:                     // 通常のメニュー項目
                case NodeType.Separator:                    // メニュー項目の区切り線
                case NodeType.SubMenu:                      // サブメニューを持つメニュー項目
                    currentNode.Parent.Nodes.Insert(currentNodeIndex + 1, newNode);
                    parentList.Insert(currentNodeIndex + 1, newMenu);
                    break;
                case NodeType.Last:                         // 最終の項目
                    currentNode.Parent.Nodes.Insert(currentNodeIndex, newNode);
                    parentList.Insert(currentNodeIndex, newMenu);
                    break;
                case NodeType.Dummy: {                      // 項目がないときに「項目がありません」を表示するための項目
                    TreeNode lastNode = new TreeNode();
                    lastNode.Tag = new MenuNodeTag(NodeType.Last, null, currentTag.Depth);
                    parentNode.Nodes.Clear();
                    parentNode.Nodes.Add(newNode);
                    parentNode.Nodes.Add(lastNode);
                    parentList.Add(newMenu);
                    break;
                }
            }
            this.treeView.SelectedNode = newNode;
            newNode.Expand();
        }

        //=========================================================================================
        // 機　能：項目の編集ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonEdit_Click(object sender, EventArgs evt) {
            TreeNode currentNode = this.treeView.SelectedNode;
            if (currentNode == null) {
                return;
            }
            MenuNodeTag currentTag = (MenuNodeTag)(currentNode.Tag);
            MenuItemSetting currentMenu = currentTag.MenuSetting;
            List<MenuItemSetting> parentList = null;
            if (currentNode != m_rootNode) {
                parentList = ((MenuNodeTag)(currentNode.Parent.Tag)).MenuSetting.SubMenuList;
            }

            switch (currentTag.NodeType) {
                case NodeType.Root:                         // メニューのルート（メインメニューに常時表示される項目）
                    break;
                case NodeType.MenuItem: {                   // 通常のメニュー項目
                    MenuItemDialog dialog = new MenuItemDialog(m_commandScene, currentMenu.ItemNameInput, currentMenu.ShortcutKey, parentList, currentMenu);
                    DialogResult result = dialog.ShowDialog(this);
                    if (result != DialogResult.OK) {
                        return;
                    }
                    currentTag.MenuSetting.ReplaceMenuSettingItem(dialog.ResultCommand, dialog.ResultShortcutKey, dialog.ResultItemName);
                    break;
                }
                case NodeType.Separator:                    // メニュー項目の区切り線
                    break;
                case NodeType.SubMenu: {                    // サブメニューを持つメニュー項目
                    MenuGroupDialog dialog = new MenuGroupDialog(currentMenu.ItemNameInput, currentMenu.ShortcutKey, parentList, currentMenu);
                    DialogResult result = dialog.ShowDialog(this);
                    if (result != DialogResult.OK) {
                        return;
                    }
                    currentTag.MenuSetting.ReplaceMenuSettingItem(dialog.GroupName, dialog.ShortcutKey);
                    break;
                }
                case NodeType.Last:                         // 最終の項目
                    break;
                case NodeType.Dummy: {                      // 項目がないときに「項目がありません」を表示するための項目
                    break;
                }
            }
            this.treeView.Invalidate();
        }

        //=========================================================================================
        // 機　能：項目の削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            TreeNode currentNode = this.treeView.SelectedNode;
            if (currentNode == null) {
                return;
            }
            MenuNodeTag currentTag = (MenuNodeTag)(currentNode.Tag);
            int currentNodeIndex = currentNode.Index;
            TreeNode parentNode = currentNode.Parent;
            List<MenuItemSetting> parentList;
            if (currentNode == m_rootNode) {
                parentList = m_menuItemCustomRoot.MenuSetting.SubMenuList;
            } else {
                parentList = ((MenuNodeTag)(parentNode.Tag)).MenuSetting.SubMenuList;
            }

            bool deletePopupAll = true;
            if (currentTag.NodeType == NodeType.SubMenu) {
                if (currentNode.Nodes.Count >= 2) {
                    DialogResult yesNo = InfoBox.Question(this, MessageBoxButtons.YesNoCancel, Resources.DlgMenuSetting_DeletePopup);
                    if (yesNo == DialogResult.Cancel) {
                        return;
                    }
                    deletePopupAll = (yesNo == DialogResult.Yes);
                }
            }

            switch (currentTag.NodeType) {
                case NodeType.Root:                         // メニューのルート（メインメニューに常時表示される項目）
                    break;
                case NodeType.MenuItem:                     // 通常のメニュー項目
                case NodeType.Separator:                    // メニュー項目の区切り線
                case NodeType.SubMenu:                      // サブメニューを持つメニュー項目
                    if (currentTag.NodeType == NodeType.SubMenu && !deletePopupAll) {
                        DeletePopupNodeAndMoveParent(currentNode, parentList);
                    } else {
                        parentNode.Nodes.Remove(currentNode);
                        parentList.Remove(currentTag.MenuSetting);
                        if (parentNode.Nodes.Count == 1 && ((MenuNodeTag)(parentNode.Nodes[0].Tag)).NodeType == NodeType.Last) {
                            parentNode.Nodes.Clear();
                            TreeNode newNodeDummy = new TreeNode();
                            newNodeDummy.Tag = new MenuNodeTag(NodeType.Dummy, null, currentTag.Depth);
                            parentNode.Nodes.Add(newNodeDummy);
                        }
                        if (currentNodeIndex == 0) {
                            this.treeView.SelectedNode = parentNode.Nodes[currentNodeIndex];
                        } else {
                            this.treeView.SelectedNode = parentNode.Nodes[currentNodeIndex - 1];
                        }
                    }
                    break;
                case NodeType.Dummy:                        // 項目がないときに「項目がありません」を表示するための項目
                case NodeType.Last:                         // 最終の項目
                    break;
            }
        }

        //=========================================================================================
        // 機　能：指定されたノード配下にある項目を親の階層に移動し、指定されたノードを削除する
        // 引　数：[in]currentNode   削除対象のノード
        // 　　　　[in]parentList    親階層のメニュー項目
        // 戻り値：なし
        //=========================================================================================
        private void DeletePopupNodeAndMoveParent(TreeNode currentNode, List<MenuItemSetting> parentList) {
            TreeNode parentNode = currentNode.Parent;
            MenuNodeTag currentTag = (MenuNodeTag)(currentNode.Tag);
            int parentIndex = currentNode.Index;
            for (int i = currentNode.Nodes.Count - 1; i >= 0; i--) {
                TreeNode childNode = currentNode.Nodes[i];
                MenuNodeTag childTag = (MenuNodeTag)(currentNode.Nodes[i].Tag);
                parentNode.Nodes.Remove(childNode);
                if (childTag.NodeType == NodeType.Last || childTag.NodeType == NodeType.Dummy) {
                    ;
                } else {
                    parentNode.Nodes.Insert(parentIndex, childNode);
                    parentList.Insert(parentIndex, childTag.MenuSetting);
                    ModifyChildDepth(childNode, -1);
                }
            }
            parentNode.Nodes.Remove(currentNode);
            parentList.Remove(currentTag.MenuSetting);
        }

        //=========================================================================================
        // 機　能：指定された項目以下にある項目の階層情報を変更する
        // 引　数：[in]node      変更対象のノードのルート
        // 　　　　[in]increase  階層の増分
        // 戻り値：なし
        //=========================================================================================
        private void ModifyChildDepth(TreeNode node, int increase) {
            MenuNodeTag tag = (MenuNodeTag)node.Tag;
            tag.Depth += increase;
            for (int i = 0; i < node.Nodes.Count; i++) {
                ModifyChildDepth(node.Nodes[i], increase);
            }
        }

        //=========================================================================================
        // 機　能：上へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonUp_Click(object sender, EventArgs evt) {
            TreeNode currentNode = this.treeView.SelectedNode;
            if (currentNode == null) {
                return;
            }
            MenuNodeTag currentTag = (MenuNodeTag)(currentNode.Tag);
            NodeType currentNodeType = currentTag.NodeType;
            if (currentNodeType == NodeType.Root || currentNodeType == NodeType.Last || currentNodeType == NodeType.Dummy) {
                return;
            }
            int currentNodeIndex = currentNode.Index;
            TreeNode prevNode = currentNode.PrevNode;
            TreeNode parentNode = currentNode.Parent;
            List<MenuItemSetting> parentList = ((MenuNodeTag)(parentNode.Tag)).MenuSetting.SubMenuList;
            
            // 上限ノード
            if (currentNode.Parent == m_rootNode && currentNode.Index == 0) {
                return;
            }

            // 対象を一度削除
            parentNode.Nodes.Remove(currentNode);
            parentList.Remove(currentTag.MenuSetting);

            if (parentNode.Nodes.Count == 1 && ((MenuNodeTag)(parentNode.Nodes[0].Tag)).NodeType == NodeType.Last) {
                // 最後の1個が消えた
                parentNode.Nodes.Clear();
                TreeNode newNodeDummy = new TreeNode();
                newNodeDummy.Tag = new MenuNodeTag(NodeType.Dummy, null, currentTag.Depth);
                parentNode.Nodes.Add(newNodeDummy);
            }
            if (currentNodeIndex > 0) {
                // まだ残っている
                MenuNodeTag prevTag = (MenuNodeTag)(prevNode.Tag);
                if (prevTag.NodeType == NodeType.SubMenu) {
                    // 直前のポップアップの最後に追加
                    TreeNode target = GetLastPopupMenuParent(prevNode);
                    MenuNodeTag targetTag = (MenuNodeTag)(target.Tag);
                    ModifyChildDepth(currentNode, 1);
                    if (target.Nodes.Count == 1) {
                        // 子ノードのダミーを削除して追加
                        TreeNode lastNode = new TreeNode();
                        lastNode.Tag = new MenuNodeTag(NodeType.Last, null, currentTag.Depth);
                        target.Nodes.Clear();
                        target.Nodes.Add(currentNode);
                        target.Nodes.Add(lastNode);
                        targetTag.MenuSetting.SubMenuList.Add(currentTag.MenuSetting);
                    } else {
                        // Lastノードの手前に追加
                        target.Nodes.Insert(target.Nodes.Count - 1, currentNode);
                        targetTag.MenuSetting.SubMenuList.Add(currentTag.MenuSetting);
                    }
                } else {
                    parentNode.Nodes.Insert(currentNodeIndex - 1, currentNode);
                    parentList.Insert(currentNodeIndex - 1, currentTag.MenuSetting);
                }
            } else {
                // 上の階層に移動
                int parentIndex = parentNode.Index;
                parentNode.Parent.Nodes.Insert(parentIndex, currentNode);
                if (parentNode.Parent == null) {
                    m_menuItemCustomRoot.MenuSetting.SubMenuList.Insert(parentIndex, currentTag.MenuSetting);
                    ModifyChildDepth(currentNode, -1);
                } else {
                    List<MenuItemSetting> parentParentList = ((MenuNodeTag)(parentNode.Parent.Tag)).MenuSetting.SubMenuList;
                    parentParentList.Insert(parentIndex, currentTag.MenuSetting);
                    ModifyChildDepth(currentNode, -1);
                }
            }
            this.treeView.SelectedNode = currentNode;
        }

        //=========================================================================================
        // 機　能：指定されたツリーノードの親階層の最後の項目を返す（ポップアップに対応）
        // 引　数：[in]target  対象の項目
        // 戻り値：対象項目の親階層の最後にある項目
        //=========================================================================================
        private TreeNode GetLastPopupMenuParent(TreeNode target) {
            while (true) {
                if (target.Nodes.Count == 1) {
                    // 子ノードはダミー
                    return target;
                } else {
                    // 子ノードの最後をチェック
                    TreeNode targetLastChild = target.Nodes[target.Nodes.Count - 2];
                    MenuNodeTag targetLastChildTag = (MenuNodeTag)(targetLastChild.Tag);
                    if (targetLastChildTag.NodeType == NodeType.SubMenu) {
                        // 最後がポップアップの場合は子を継続
                        target = targetLastChild;
                    } else {
                        return target;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：下へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDown_Click(object sender, EventArgs evt) {
            TreeNode currentNode = this.treeView.SelectedNode;
            if (currentNode == null) {
                return;
            }
            MenuNodeTag currentTag = (MenuNodeTag)(currentNode.Tag);
            NodeType currentNodeType = currentTag.NodeType;
            if (currentNodeType == NodeType.Root || currentNodeType == NodeType.Last || currentNodeType == NodeType.Dummy) {
                return;
            }
            int currentNodeIndex = currentNode.Index;
            TreeNode nextNode = currentNode.NextNode;
            TreeNode parentNode = currentNode.Parent;
            List<MenuItemSetting> parentList = ((MenuNodeTag)(parentNode.Tag)).MenuSetting.SubMenuList;
            int parentNodeCount = parentNode.Nodes.Count;

            // 下限ノード
            if (IsLastNode(currentNode)) {
                return;
            }

            // 対象を一度削除
            parentNode.Nodes.Remove(currentNode);
            parentList.Remove(currentTag.MenuSetting);

            if (parentNode.Nodes.Count == 1 && ((MenuNodeTag)(parentNode.Nodes[0].Tag)).NodeType == NodeType.Last) {
                // 最後の1個が消えた
                parentNode.Nodes.Clear();
                TreeNode newNodeDummy = new TreeNode();
                newNodeDummy.Tag = new MenuNodeTag(NodeType.Dummy, null, currentTag.Depth);
                parentNode.Nodes.Add(newNodeDummy);
            }
            if (currentNodeIndex < parentNodeCount - 2) {
                // まだ残っている
                MenuNodeTag nextTag = (MenuNodeTag)(nextNode.Tag);
                if (nextTag.NodeType == NodeType.SubMenu) {
                    // 直前のポップアップの最後に追加
                    TreeNode target = GetFirstPopupMenuParent(nextNode);
                    MenuNodeTag targetTag = (MenuNodeTag)(target.Tag);
                    ModifyChildDepth(currentNode, 1);
                    if (target.Nodes.Count == 1) {
                        // 子ノードのダミーを削除して追加
                        TreeNode lastNode = new TreeNode();
                        lastNode.Tag = new MenuNodeTag(NodeType.Last, null, currentTag.Depth);
                        target.Nodes.Clear();
                        target.Nodes.Add(currentNode);
                        target.Nodes.Add(lastNode);
                        targetTag.MenuSetting.SubMenuList.Add(currentTag.MenuSetting);
                    } else {
                        // 先頭ノードに追加
                        target.Nodes.Insert(0, currentNode);
                        targetTag.MenuSetting.SubMenuList.Insert(0, currentTag.MenuSetting);
                    }
                } else {
                    parentNode.Nodes.Insert(currentNodeIndex + 1, currentNode);
                    parentList.Insert(currentNodeIndex + 1, currentTag.MenuSetting);
                }
            } else {
                // 上の階層に移動
                int parentIndex = parentNode.Index;
                parentNode.Parent.Nodes.Insert(parentIndex + 1, currentNode);
                if (parentNode.Parent == null) {
                    m_menuItemCustomRoot.MenuSetting.SubMenuList.Insert(parentIndex + 1, currentTag.MenuSetting);
                    ModifyChildDepth(currentNode, -1);
                } else {
                    List<MenuItemSetting> parentParentList = ((MenuNodeTag)(parentNode.Parent.Tag)).MenuSetting.SubMenuList;
                    parentParentList.Insert(parentIndex + 1, currentTag.MenuSetting);
                    ModifyChildDepth(currentNode, -1);
                }
            }
            this.treeView.SelectedNode = currentNode;
        }

        //=========================================================================================
        // 機　能：指定されたツリーノードの親階層の最初の項目を返す（ポップアップに対応）
        // 引　数：[in]target  対象の項目
        // 戻り値：対象項目の親階層の最初にある項目
        //=========================================================================================
        private TreeNode GetFirstPopupMenuParent(TreeNode target) {
            while (true) {
                if (target.Nodes.Count == 1) {
                    // 子ノードはダミー
                    return target;
                } else {
                    // 子ノードの最初をチェック
                    TreeNode targetFirstChild = target.Nodes[0];
                    MenuNodeTag targetFirstChildTag = (MenuNodeTag)(targetFirstChild.Tag);
                    if (targetFirstChildTag.NodeType == NodeType.SubMenu) {
                        // 最初がポップアップの場合は子を継続
                        target = targetFirstChild;
                    } else {
                        return target;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：テストボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonTest_Click(object sender, EventArgs evt) {
            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.DummyForTest, new DummyMenuTarget());
            menuImpl.AddItemsFromSetting(cms, cms.Items, m_menuItemCustomRoot.MenuSetting.SubMenuList, Program.Document.KeySetting.FileListKeyItemList, false, null);
            Point pos = new Point(this.buttonTest.Left, this.buttonTest.Bottom);
            this.ContextMenuStrip = cms;
            this.ContextMenuStrip.Show(this, pos);
            this.ContextMenuStrip = null;
        }

        //=========================================================================================
        // 機　能：ルートメニューの項目名が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxRootMenu_TextChanged(object sender, EventArgs evt) {
            m_menuItemCustomRoot.MenuSetting.ItemNameInput = this.textBoxRootMenu.Text;
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
            bool unique = MenuListSettingDialog.CheckShortcutUnique(index, m_parentMenuList, m_menuItemCustomRoot.MenuSetting);
            this.labelShortcutWarning.Visible = !unique;

            char shortcut = IndexToShortcut(index);
            m_menuItemCustomRoot.MenuSetting.ShortcutKey = shortcut;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：編集結果のカスタマイズメニュー項目
        //=========================================================================================
        public MenuItemCustomRoot ResultMenuItem {
            get {
                return m_menuItemCustomRoot;
            }
        }

        //=========================================================================================
        // ポップアップメニューの表示テストを行うときのコマンドの送信先
        //=========================================================================================
        public class DummyMenuTarget : IUICommandTarget {

            //=========================================================================================
            // 機　能：UIでのコマンドが発生したことを通知する
            // 引　数：[in]sender  イベント発生原因の送信元の種別
            // 　　　　[in]item    発生したイベントの項目
            // 戻り値：なし
            //=========================================================================================
            public void OnUICommand(UICommandSender sender, UICommandItem item) {
            }
        }

        //=========================================================================================
        // プロパティ：メニュー項目のツリーのTagに設定する値
        //=========================================================================================
        private class MenuNodeTag {
            // メニュー項目のノードの種類
            public NodeType NodeType;

            // メニューの設定（この内容がマスター、Last/Dummyのときnull）
            public MenuItemSetting MenuSetting;

            // 階層の深さ
            public int Depth;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]nodeType      メニュー項目のノードの種類
            // 　　　　[in]menuSetting   メニューの設定
            // 　　　　[in]depth         階層の深さ
            // 戻り値：なし
            //=========================================================================================
            public MenuNodeTag(NodeType nodeType, MenuItemSetting menuSetting, int depth) {
                NodeType = nodeType;
                MenuSetting = menuSetting;
                Depth = depth;
            }
        }

        //=========================================================================================
        // プロパティ：メニュー項目のノードの種類
        //=========================================================================================
        private enum NodeType {
            Root,                   // メニューのルート（メインメニューに常時表示される項目）
            MenuItem,               // 通常のメニュー項目
            Separator,              // メニュー項目の区切り線
            SubMenu,                // サブメニューを持つメニュー項目
            Dummy,                  // 項目がないときに「項目がありません」を表示するための項目
            Last,                   // 最終の項目
        }
    }
}
