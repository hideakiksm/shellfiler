using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Document;
using ShellFiler.Document.SSH;
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
    // クラス：SSH関連のノードの処理を実装するクラス
    //=========================================================================================
    class SSHNodeImpl {
        // 所有パネル
        private StateListPanel m_parent;

        // SSHファイルシステム（対応していないときnull）
        private SFTPFileSystem m_sshFileSystem = null;

        // トップレベルノード(SSHセッション)
        private TreeNode m_rootNodeSSHSession;

        // 項目なしのノード(SSHセッション)
        private TreeNode m_noListSSHSession;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent     所有パネル
        // 戻り値：なし
        //=========================================================================================
        public SSHNodeImpl(StateListPanel parent) {
            m_parent = parent;

            // 管理用ノードの作成
            m_rootNodeSSHSession = new TreeNode(Resources.StateView_SSHConnection);
            m_parent.TreeView.Nodes.Add(m_rootNodeSSHSession);
            m_noListSSHSession = new TreeNode(Resources.StateView_NoSSHConnection);
            m_rootNodeSSHSession.Nodes.Add(m_noListSSHSession);
            m_rootNodeSSHSession.ToolTipText = Resources.StateView_SSHConnectionHint;
            m_rootNodeSSHSession.Expand();

            // イベントを設定
            m_sshFileSystem = Program.Document.FileSystemFactory.SFTPFileSystem;
            if (m_sshFileSystem != null) {
                SSHConnectionManager manager = Program.Document.FileSystemFactory.SSHConnectionManager;
                manager.ConnectionChanged += new SSHConnectionManager.ConnectionChangedEventHandler(this.ConnectionManager_StateChanged);
            }
        }

        //=========================================================================================
        // 機　能：SSH接続一覧のTreeViewノードを作成する
        // 引　数：[in]info  接続の情報
        // 戻り値：TreeViewの項目
        //=========================================================================================
        private TreeNode CreateSSHConnectionNode(UIConnectionPoolInfo info) {
            string userServer = SSHUtils.CreateUserServerShort(info.User, info.Server, info.PortNo);
            TreeNode node = new TreeNode(userServer); 
            node.ImageIndex = (int)IconImageListID.Icon_BackgroundTaskSSH;
            node.SelectedImageIndex = node.ImageIndex;
            node.Tag = info;
            return node;
        }



        //=========================================================================================
        // 機　能：SSHコネクションマネージャの状態が変わった時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void ConnectionManager_StateChanged(object sender, SSHConnectionManager.ConnectionChangedEventArgs evt) {
            ConnectionManager_StateChangedUIDelegate delg = new ConnectionManager_StateChangedUIDelegate(ConnectionManager_StateChangedUI);
            BaseThread.InvokeProcedureByMainThread(delg, sender, evt);
        }
        delegate void ConnectionManager_StateChangedUIDelegate(object sender, SSHConnectionManager.ConnectionChangedEventArgs evt);
        private void ConnectionManager_StateChangedUI(object sender, SSHConnectionManager.ConnectionChangedEventArgs evt) {
            if (m_sshFileSystem == null) {
                return;
            }

            if (evt.EventType == SSHConnectionManager.ConnectionChangedEventType.ConnectionPoolAdded) {
                OnConnectionPoolAdded(evt);
            } else if (evt.EventType == SSHConnectionManager.ConnectionChangedEventType.ConnectionPoolDeleted) {
                OnConnectionPoolDeleted(evt);
            } else if (evt.EventType == SSHConnectionManager.ConnectionChangedEventType.CountChanged) {
                OnCountChanged(evt);
            } else if (evt.EventType == SSHConnectionManager.ConnectionChangedEventType.StateReset) {
                OnStateReset(evt);
            }
        }

        //=========================================================================================
        // 機　能：ConnectionPoolに新しいユーザー名/サーバーの組み合わせが登録されたときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnConnectionPoolAdded(SSHConnectionManager.ConnectionChangedEventArgs evt) {
            if (m_rootNodeSSHSession.Nodes.Count == 1 && m_rootNodeSSHSession.Nodes[0] == m_noListSSHSession) {
                m_rootNodeSSHSession.Nodes.RemoveAt(0);
            }
            TreeNode item = CreateSSHConnectionNode(evt.PoolInfo);
            m_rootNodeSSHSession.Nodes.Add(item);
            m_rootNodeSSHSession.Expand();
        }

        //=========================================================================================
        // 機　能：ConnectionPoolからユーザー名/サーバーの組み合わせが削除されたときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnConnectionPoolDeleted(SSHConnectionManager.ConnectionChangedEventArgs evt) {
            foreach (TreeNode node in m_rootNodeSSHSession.Nodes) {
                if (node == m_noListSSHSession) {
                    continue;
                }
                UIConnectionPoolInfo info = (UIConnectionPoolInfo)(node.Tag);
                if (info.Server == evt.Server && info.User == evt.User && info.PortNo == evt.PortNo) {
                    m_rootNodeSSHSession.Nodes.Remove(node);
                    break;
                }
            }
            if (m_rootNodeSSHSession.Nodes.Count == 0) {
                m_rootNodeSSHSession.Nodes.Add(m_noListSSHSession);
            }
            m_rootNodeSSHSession.Expand();

            // 表示中であれば初期フォルダへ
            if (FileSystemID.IsSSH(Program.Document.CurrentTabPage.LeftFileList.FileSystem.FileSystemId)) {
                string filePath = Program.Document.CurrentTabPage.LeftFileList.DisplayDirectoryName;
                SSHProtocolType protocol;
                string user, server, path;
                int portNo;
                SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
                if (evt.Server == server && evt.User == user && evt.PortNo == portNo) {
                    ChdirCommand.ChangeDirectory(Program.MainWindow.LeftFileListView, new ChangeDirectoryParam.Direct(UIFileList.InitialFolder));
                }
            }
            if (FileSystemID.IsSSH(Program.Document.CurrentTabPage.RightFileList.FileSystem.FileSystemId)) {
                string filePath = Program.Document.CurrentTabPage.RightFileList.DisplayDirectoryName;
                SSHProtocolType protocol;
                string user, server, path;
                int portNo;
                SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
                if (evt.Server == server && evt.User == user && evt.PortNo == portNo) {
                    ChdirCommand.ChangeDirectory(Program.MainWindow.RightFileListView, new ChangeDirectoryParam.Direct(UIFileList.InitialFolder));
                }
            }
        }

        //=========================================================================================
        // 機　能：使用中の接続数に変化が生じたときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnCountChanged(SSHConnectionManager.ConnectionChangedEventArgs evt) {
        }

        //=========================================================================================
        // 機　能：情報に大幅な変化が生じたため、再構築の必要があるときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnStateReset(SSHConnectionManager.ConnectionChangedEventArgs evt) {
            m_rootNodeSSHSession.Nodes.Clear();
            SSHConnectionManager manager = Program.Document.FileSystemFactory.SSHConnectionManager;
            List<UIConnectionPoolInfo> infoList = manager.UIConnectionPoolList;
            foreach (UIConnectionPoolInfo info in infoList) {
                TreeNode node = CreateSSHConnectionNode(info);
                m_rootNodeSSHSession.Nodes.Add(node);
            }
            if (m_rootNodeSSHSession.Nodes.Count == 0) {
                m_rootNodeSSHSession.Nodes.Add(m_noListSSHSession);
            }
            m_rootNodeSSHSession.Expand();
        }



        //=========================================================================================
        // 機　能：ツリーの項目がクリックされたときの処理を行う
        // 引　数：[in]node  クリックされたノード
        // 戻り値：なし
        //=========================================================================================
        public void OnNodeMouseClick(TreeNode node) {
            StateMenu[] menuItemList = null;
            if (node != m_noListSSHSession && node.Parent == m_rootNodeSSHSession) {
                // 接続中のSSHセッション
                menuItemList = new StateMenu[] {
                    new StateMenu(true, Resources.StateView_MenuSSHFolder),
                    new StateMenu(true, Resources.StateView_MenuSSHHomeSFTP),
                    new StateMenu(true, Resources.StateView_MenuSSHHomeSSH),
                    new StateMenu(true, Resources.StateView_MenuSSHClose),
                    new StateMenu(true, null),
                    new StateMenu(true, Resources.StateView_MenuSSHCloseAll),
                    new StateMenu(true, Resources.StateView_MenuSSHOpen),
                };
            } else if (node == m_rootNodeSSHSession) {
                if (node.Nodes.Count > 0 && node.Nodes[0] != m_noListSSHSession) {
                    // 有効なSSHセッションがある状態のSSHセッション
                    menuItemList = new StateMenu[] {
                        new StateMenu(true, Resources.StateView_MenuSSHOpen),
                        new StateMenu(true, Resources.StateView_MenuSSHCloseAll),
                    };
                } else {
                    // 有効なSSHセッションがない状態のSSHセッション
                    menuItemList = new StateMenu[] {
                        new StateMenu(true, Resources.StateView_MenuSSHOpen),
                        new StateMenu(false, Resources.StateView_MenuSSHCloseAll),
                    };
                }
            }
            
            if (menuItemList != null) {
                m_parent.ShowNodeItemMenu(node, menuItemList, new EventHandler(ContextMenuItem_Click));
            }
        }

        //=========================================================================================
        // 機　能：コンテキストメニューの項目がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void ContextMenuItem_Click(object sender, EventArgs evt) {
            ToolStripItem menuItem = (ToolStripItem)sender;
            TreeNode node = (TreeNode)(menuItem.Tag);
            string menuText = menuItem.Text;
            if (menuText == Resources.StateView_MenuSSHFolder) {
                // SSH：直前のフォルダを表示
                UIConnectionPoolInfo info = (UIConnectionPoolInfo)(node.Tag);
                string prevPath = Program.Document.FileSystemFactory.SSHConnectionManager.GetPreviousPath(info.User, info.Server, info.PortNo);
                if (prevPath != null) {
                    Program.MainWindow.OnAddressBarCommand(prevPath);
                }
            } else if (menuText == Resources.StateView_MenuSSHHomeSFTP) {
                // SSH:SFTPでホームdirを表示
                UIConnectionPoolInfo info = (UIConnectionPoolInfo)(node.Tag);
                string path = SSHUtils.CreateUserServer(SSHProtocolType.SFTP, info.User, info.Server, info.PortNo);
                Program.MainWindow.OnAddressBarCommand(path + ":~/");
            } else if (menuText == Resources.StateView_MenuSSHHomeSSH) {
                // SSH:SSHシェルでホームdirを表示
                UIConnectionPoolInfo info = (UIConnectionPoolInfo)(node.Tag);
                string path = SSHUtils.CreateUserServer(SSHProtocolType.SSHShell, info.User, info.Server, info.PortNo);
                Program.MainWindow.OnAddressBarCommand(path + ":~/");
            } else if (menuText == Resources.StateView_MenuSSHClose) {
                // SSH:このセッションを切断
                UIConnectionPoolInfo info = (UIConnectionPoolInfo)(node.Tag);
                TaskManagerDialog.DisconnectSSHSession(info, Program.MainWindow);
            } else if (menuText == Resources.StateView_MenuSSHCloseAll) {
                // SSH:すべてのセッションを切断
                TaskManagerDialog.DisconnectAllSSHSession(Program.MainWindow);
            } else if (menuText == Resources.StateView_MenuSSHOpen) {
                // SSH:新規セッション
                Program.MainWindow.OnUICommand(UICommandSender.StateListPanel, UICommandItem.StateList_NoSSH);
            }
        }    

        //=========================================================================================
        // プロパティ：トップレベルノード(SSHセッション)
        //=========================================================================================
        public TreeNode RootNodeSSHSession {
            get {
                return m_rootNodeSSHSession;
            }
        }

        //=========================================================================================
        // プロパティ：項目なしのノード(SSHセッション)
        //=========================================================================================
        public TreeNode NoListSSHSession {
            get {
                return m_noListSSHSession;
            }
        }
    }
}
