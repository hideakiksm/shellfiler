using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Management;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;
using ShellFiler.Virtual;

namespace ShellFiler.UI.StateList {

    //=========================================================================================
    // クラス：状態一覧パネル
    //=========================================================================================
    public partial class StateListPanel : ListView {
        // タスクノードの実装
        private TaskNodeImpl m_taskNodeImpl;

        // SSHノードの実装
        private SSHNodeImpl m_sshNodeImpl;

        // 一時領域終了待ちノードの実装
        private TemporaryNodeImpl m_temporaryNodeImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public StateListPanel() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
            // ノードの初期化
            this.treeView.ImageList = UIIconManager.IconImageList;
            this.treeView.ShowNodeToolTips = true;

            m_taskNodeImpl = new TaskNodeImpl(this);
            m_sshNodeImpl = new SSHNodeImpl(this);
            m_temporaryNodeImpl = new TemporaryNodeImpl(this, components);
        }

        //=========================================================================================
        // 機　能：項目を描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void treeView_DrawNode(object sender, DrawTreeNodeEventArgs evt) {
            if (evt.Bounds.Width <= 0 || evt.Bounds.Height <= 0) {
                return;
            }
            TreeNode node = evt.Node;
            if (node == m_taskNodeImpl.RootNodeTaskManager || node == m_sshNodeImpl.RootNodeSSHSession || node == m_temporaryNodeImpl.RootNodeEdit) {
                // ルート項目
                TreeViewRenderer renderer = new TreeViewRenderer(this);
                renderer.DrawTitleItem(evt);
            } else if (node == m_taskNodeImpl.NoListTaskManager || node == m_sshNodeImpl.NoListSSHSession || node == m_temporaryNodeImpl.NoListEdit) {
                // 「項目なし」の項目
                TreeViewRenderer renderer = new TreeViewRenderer(this);
                renderer.DrawDisableItem(evt);
            } else if (node.Parent == m_taskNodeImpl.RootNodeTaskManager) {
                // タスク項目
                if (node == m_taskNodeImpl.WaitingTaskNode) {
                    evt.DrawDefault = true;
                } else {
                    Point origin = new Point(evt.Bounds.X, evt.Bounds.Y);
                    bool isFocus = ((evt.State & TreeNodeStates.Focused) == TreeNodeStates.Focused);
                    TreeViewRenderer renderer = new TreeViewRenderer(this);
                    renderer.DrawTaskItem(evt.Graphics, origin, node, isFocus);
                }
            } else {
                // 通常の項目（オーナードロー対象外）
                evt.DrawDefault = true;
            }
        }
        
        //=========================================================================================
        // 機　能：ツリーの項目がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt) {
            TreeNode node = evt.Node;
            if (node == null) {
                return;
            }

            this.treeView.SelectedNode = node;

            if (node == m_taskNodeImpl.RootNodeTaskManager || node.Parent == m_taskNodeImpl.RootNodeTaskManager || node.Parent == m_taskNodeImpl.WaitingTaskNode) {
                m_taskNodeImpl.OnNodeMouseClick(node);
            } else if (node == m_sshNodeImpl.RootNodeSSHSession || node.Parent == m_sshNodeImpl.RootNodeSSHSession) {
                m_sshNodeImpl.OnNodeMouseClick(node);
            } else if (node == m_temporaryNodeImpl.RootNodeEdit || node.Parent == m_temporaryNodeImpl.RootNodeEdit) {
                m_temporaryNodeImpl.OnNodeMouseClick(node);
            }
        }

        //=========================================================================================
        // 機　能：ツリー項目のコンテキストメニューを表示する
        // 引　数：[in]node     ツリーの項目
        // 　　　　[in]menuList メニューの項目一覧
        // 戻り値：なし
        //=========================================================================================
        public void ShowNodeItemMenu(TreeNode node, StateMenu[] menuList, EventHandler eventHandler) {
            // コンテキストメニューを表示
            ContextMenuStrip cms = new ContextMenuStrip();
            for (int i = 0; i < menuList.Length; i++) {
                if (menuList[i].MenuName == null) {
                    cms.Items.Add(new ToolStripSeparator());
                } else {
                    ToolStripItem item = new ToolStripMenuItem(menuList[i].MenuName);
                    item.Tag = node;
                    item.Enabled = menuList[i].Enabled;
                    item.Click += eventHandler;
                    cms.Items.Add(item);
                }
            }
            ContextMenuStrip = cms;
            ContextMenuStrip.Show(this, new Point(node.Bounds.Left + 10, node.Bounds.Bottom));
            ContextMenuStrip = null;
        }

        //=========================================================================================
        // 機　能：ノードが縮小されようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeView_BeforeCollapse(object sender, TreeViewCancelEventArgs evt) {
            evt.Cancel = true;
        }

        //=========================================================================================
        // 機　能：キーが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeView_KeyDown(object sender, KeyEventArgs evt) {
            if (evt.KeyCode == Keys.Escape || evt.KeyCode == Keys.Up && evt.Control == true) {
                if (Program.Document.CurrentTabPage.IsCursorLeft) {
                    Program.MainWindow.LeftFileListView.Focus();
                } else {
                    Program.MainWindow.RightFileListView.Focus();
                }
                evt.SuppressKeyPress = true;
                evt.Handled = true;
            }
        }

        //=========================================================================================
        // 機　能：フォーカスを受け取ったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeView_Enter(object sender, EventArgs evt) {
            Program.MainWindow.OnActivateStateListPanel(true);
            this.treeView.Invalidate();
        }

        //=========================================================================================
        // 機　能：フォーカスを失ったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeView_Leave(object sender, EventArgs evt) {
            Program.MainWindow.OnActivateStateListPanel(false);
            this.treeView.Invalidate();
        }

        //=========================================================================================
        // 機　能：状態一覧パネルをアクティブにする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ActivateStateList() {
            this.treeView.Focus();
        }

        //=========================================================================================
        // プロパティ：ツリーのUI
        //=========================================================================================
        public TreeView TreeView {
            get {
                return this.treeView;
            }
        }
    }
}
 