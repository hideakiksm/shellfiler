using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.ComponentModel;
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
    // クラス：作業領域の終了待ちノードの処理を実装するクラス
    //=========================================================================================
    class TemporaryNodeImpl {
        // 編集開始からプロセス終了まででプロセスが多重起動防止で終了したと見なす時間[s]
        private const int PROCESS_CLOSE_ON_EDIT_MIN_TIME = 1;

        // 所有パネル
        private StateListPanel m_parent;

        // 終了状態の監視クラス
        private EditEndWatcher m_editEndWatcher;

        // トップレベルノード(編集終了待ち)
        private TreeNode m_rootNodeEdit;

        // 項目なしのノード(編集終了待ち)
        private TreeNode m_noListEdit;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent     所有パネル
        // 　　　　[in]components タイマー初期化用のコンポーネント
        // 戻り値：なし
        //=========================================================================================
        public TemporaryNodeImpl(StateListPanel parent, IContainer components) {
            m_parent = parent;
            m_editEndWatcher = new EditEndWatcher(components);

            // 管理用ノードの作成
            m_rootNodeEdit = new TreeNode(Resources.StateView_EditNow);
            m_parent.TreeView.Nodes.Add(m_rootNodeEdit);
            m_noListEdit = new TreeNode(Resources.StateView_NoEditNow);
            m_rootNodeEdit.Nodes.Add(m_noListEdit);
            m_rootNodeEdit.ToolTipText = Resources.StateView_EditNowHint;
            m_rootNodeEdit.Expand();

            // イベントを設定
            Program.Document.TemporaryManager.TemporaryChanged += new TemporaryManager.TemporaryChangedEventHandler(TemporaryManager_TemporaryChanged);
        }

        //=========================================================================================
        // 機　能：編集中一覧のTreeViewノードを作成する
        // 引　数：[in]info  作業ディレクトリの情報
        // 戻り値：TreeViewの項目
        //=========================================================================================
        private TreeNode CreateTemporaryLocalExecuteListNode(LocalExecuteTemporarySpace info) {
            TreeNode node = new TreeNode(info.ProgramNameHintShort);
            node.ImageIndex = (int)info.DisplayNameInfo.IconImageIndex;
            node.SelectedImageIndex = node.ImageIndex;
            node.ToolTipText = info.ProgramNameHintShort;
            node.Tag = info;
            return node;
        }


        
        //=========================================================================================
        // 機　能：テンポラリマネージャの状態が変わった時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void TemporaryManager_TemporaryChanged(object sender, TemporaryManager.TemporaryChangedEventArgs evt) {
            TemporaryManager_TemporaryChangedUIDelegate delg = new TemporaryManager_TemporaryChangedUIDelegate(TemporaryManager_TemporaryChangedUI);
            BaseThread.InvokeProcedureByMainThread(delg, sender, evt);
        }
        delegate void TemporaryManager_TemporaryChangedUIDelegate(object sender, TemporaryManager.TemporaryChangedEventArgs evt);
        private void TemporaryManager_TemporaryChangedUI(object sender, TemporaryManager.TemporaryChangedEventArgs evt) {
            if (evt.EventType == TemporaryManager.TemporaryChangedEventType.LocalExecuteAdded) {
                OnLocalExecuteAdded(evt);
            } else if (evt.EventType == TemporaryManager.TemporaryChangedEventType.LocalExecuteDeleted) {
                OnLocalExecuteDeleted(evt);
            }
        }

        //=========================================================================================
        // 機　能：ローカル実行のテンポラリが追加されたときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnLocalExecuteAdded(TemporaryManager.TemporaryChangedEventArgs evt) {
            if (m_rootNodeEdit.Nodes.Count == 1 && m_rootNodeEdit.Nodes[0] == m_noListEdit) {
                m_rootNodeEdit.Nodes.RemoveAt(0);
            }
            TreeNode item = CreateTemporaryLocalExecuteListNode(evt.LocalExecuteTemporarySpace);
            m_rootNodeEdit.Nodes.Add(item);
            m_rootNodeEdit.Expand();
        }

        //=========================================================================================
        // 機　能：ローカル実行のテンポラリが削除されたときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnLocalExecuteDeleted(TemporaryManager.TemporaryChangedEventArgs evt) {
            foreach (TreeNode node in m_rootNodeEdit.Nodes) {
                if (node == m_noListEdit) {
                    continue;
                }
                LocalExecuteTemporarySpace info = (LocalExecuteTemporarySpace)(node.Tag);
                if (info == evt.LocalExecuteTemporarySpace) {
                    m_rootNodeEdit.Nodes.Remove(node);
                    break;
                }
            }
            if (m_rootNodeEdit.Nodes.Count == 0) {
                m_rootNodeEdit.Nodes.Add(m_noListEdit);
            }
            m_rootNodeEdit.Expand();
        }



        //=========================================================================================
        // 機　能：ツリーの項目がクリックされたときの処理を行う
        // 引　数：[in]node  クリックされたノード
        // 戻り値：なし
        //=========================================================================================
        public void OnNodeMouseClick(TreeNode node) {
            StateMenu[] menuItemList = null;
            if (node != m_noListEdit && node.Parent == m_rootNodeEdit) {
                // 終了待ちの編集ファイル
                menuItemList = new StateMenu[] {
                    new StateMenu(true, Resources.StateView_MenuEditUpdate),
                    new StateMenu(true, Resources.StateView_MenuEditClose),
                    new StateMenu(true, Resources.StateView_MenuEditWorkDir),
                    new StateMenu(true, null),
                    new StateMenu(true, Resources.StateView_MenuEditCloseAll)
                };
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
            // 終了待ち
            // アップロード中のとき、更新を回避
            BackgroundTaskManager taskMan = Program.Document.BackgroundTaskManager;
            if (taskMan.IsVirtualUploading()) {
                InfoBox.Information(Program.MainWindow, Resources.StateView_EditCannotModify);
                return;
            }

            LocalExecuteTemporarySpace space = (LocalExecuteTemporarySpace)(node.Tag);
            if (menuText == Resources.StateView_MenuEditUpdate) {
                // 終了待ち:編集結果を反映
                List<LocalFileInfo> modifiedList;
                space.CheckModified(out modifiedList);
                if (modifiedList.Count == 0) {
                    DialogResult result = InfoBox.Question(Program.MainWindow, MessageBoxButtons.YesNo, Resources.StateView_EditUpdateNotModify);
                    if (result != DialogResult.Yes) {
                        return;
                    }
                    Program.Document.TemporaryManager.DeleteLocalExecuteSpace(space);
                } else {
                    space.Dirty = true;
                }
            } else if (menuText == Resources.StateView_MenuEditClose) {
                // 終了待ち:編集結果を破棄
                List<LocalFileInfo> modifiedList;
                space.CheckModified(out modifiedList);
                if (modifiedList.Count == 0) {
                    Program.Document.TemporaryManager.DeleteLocalExecuteSpace(space);
                } else {
                    DialogResult result = InfoBox.Question(Program.MainWindow, MessageBoxButtons.YesNo, Resources.StateView_EditDeleteConfirm, modifiedList.Count);
                    if (result != DialogResult.Yes) {
                        return;
                    }
                    Program.Document.TemporaryManager.DeleteLocalExecuteSpace(space);
                }
            } else if (menuText == Resources.StateView_MenuEditWorkDir) {
                // 終了待ち:作業フォルダを表示
                Program.MainWindow.OnAddressBarCommand(space.VirtualDirectory);
                InfoBox.Information(Program.MainWindow, Resources.StateView_EditWorkdir);
            } else if (menuText == Resources.StateView_MenuEditCloseAll) {
                // 終了待ち:すべての編集結果を破棄
                bool deletedAll= Program.Document.TemporaryManager.DeleteAllLocalExecuteTemporarySpace();
                if (deletedAll) {
                    InfoBox.Information(Program.MainWindow, Resources.StateView_EditDeleteAll);
                } else {
                    InfoBox.Information(Program.MainWindow, Resources.StateView_EditDeletePart);
                }
            }
        }    

        //=========================================================================================
        // プロパティ：トップレベルノード(編集終了待ち)
        //=========================================================================================
        public TreeNode RootNodeEdit {
            get {
                return m_rootNodeEdit;
            }
        }

        //=========================================================================================
        // プロパティ：項目なしのノード(編集終了待ち)
        //=========================================================================================
        public TreeNode NoListEdit {
            get {
                return m_noListEdit;
            }
        }

        //=========================================================================================
        // クラス：終了状態の監視クラス
        //=========================================================================================
        private class EditEndWatcher {
            // 監視用の500msタイマー
            private Timer m_timerWatch;

            // 現在開いているLocalExecuteEditEndDialog（開いていないときはnull）
            private LocalExecuteEditEndDialog m_currentLocalExecuteDialog = null;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]components タイマー初期化用のコンポーネント
            // 戻り値：なし
            //=========================================================================================
            public EditEndWatcher(IContainer components) {
                m_timerWatch = new Timer(components);
                m_timerWatch.Interval = 500;
                m_timerWatch.Tick += new EventHandler(this.timerWatch_Tick);
                m_timerWatch.Start();
            }

            //=========================================================================================
            // 機　能：タイマーを処理する
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            void timerWatch_Tick(object sender, EventArgs evt) {
                // アップロード中のとき、更新を回避
                BackgroundTaskManager taskMan = Program.Document.BackgroundTaskManager;
                if (taskMan.IsVirtualUploading()) {
                    return;
                }

                // 更新を確認
                DateTime current = DateTime.Now;
                TemporaryManager manager = Program.Document.TemporaryManager;
                LocalExecuteTemporarySpace space = manager.GetFirstDirtyLocalExecuteSpace();
                while (space != null) {
                    TimeSpan spent = current - space.StartTime;
                    if (spent.Seconds <= PROCESS_CLOSE_ON_EDIT_MIN_TIME) {
                        // 短時間でプロセスが終了したときは対処方法を通知してそのまま
                        string message = string.Format(Resources.StateView_ProcessClosedImmediately, space.ProgramNameHintShort);
                        LocalExecuteProcessErrorDialog dialog = new LocalExecuteProcessErrorDialog(space.DisplayNameInfo, space.ProgramNameHintShort, message, space.LocalFileList);
                        dialog.ShowDialog(Program.MainWindow);
                    } else {
                        List<LocalFileInfo> modifiedList;
                        bool success = space.CheckModified(out modifiedList);
                        if (modifiedList.Count > 0) {
                            // プロセス終了時にファイルが更新されているときはアップロード
                            Upload(space, modifiedList);
                        } else if (!success) {
                            // プロセス終了時にファイルアクセスできないものがあったときは通知
                            string message = string.Format(Resources.StateView_ProcessClosedCannotAccess, space.ProgramNameHintShort);
                            LocalExecuteProcessErrorDialog dialog = new LocalExecuteProcessErrorDialog(space.DisplayNameInfo, space.ProgramNameHintShort, message, space.LocalFileList);
                            dialog.ShowDialog(Program.MainWindow);
                        } else {
                            // プロセス終了時にファイルが更新されていないときは終了
                            manager.DeleteLocalExecuteSpace(space);
                        }
                    }
                    space = manager.GetFirstDirtyLocalExecuteSpace();
                }
            }

            //=========================================================================================
            // 機　能：アップロード処理を実行する
            // 引　数：[in]space         作業領域の情報
            // 　　　　[in]modifiedList  更新されたファイルの一覧
            // 戻り値：なし
            //=========================================================================================
            private void Upload(LocalExecuteTemporarySpace space, List<LocalFileInfo> modifiedList) {
                // タイマーで再入してきたときは更新して終了
                if (m_currentLocalExecuteDialog != null && m_currentLocalExecuteDialog.TemporarySpace == space) {
                    m_currentLocalExecuteDialog.UpdateModifiedList(modifiedList);
                    return;
                }

                // アップロードしてよいか確認
                LocalExecuteEditEndDialog dialog = new LocalExecuteEditEndDialog(space, space.LocalFileList, modifiedList);
                m_currentLocalExecuteDialog = dialog;
                dialog.ShowDialog(Program.MainWindow);
                m_currentLocalExecuteDialog = null;
                if (dialog.Result == LocalExecuteEditEndDialog.UpdateResult.Cancel) {
                    return;
                } else if (dialog.Result == LocalExecuteEditEndDialog.UpdateResult.Delete) {
                    Program.Document.TemporaryManager.DeleteLocalExecuteSpace(space);
                    return;
                }

                // 仮想ディレクトリ内のファイル一覧を作成
                List<SimpleFileDirectoryPath> srcPath = new List<SimpleFileDirectoryPath>();
                try {
                    DirectoryInfo directoryInfo = new DirectoryInfo(space.VirtualDirectory);
                    FileSystemInfo[] infoList = directoryInfo.GetFileSystemInfos();
                    foreach (FileSystemInfo info in infoList) {
                        if (info is FileInfo) {
                            srcPath.Add(new SimpleFileDirectoryPath(info.FullName, false, false));
                        } else {
                            srcPath.Add(new SimpleFileDirectoryPath(info.FullName, true, false));
                        }
                    }
                } catch (Exception e) {
                    InfoBox.Warning(Program.MainWindow, Resources.StateView_ProcessClosedBrokenDir, space.VirtualDirectory, e.Message);
                    Program.Document.TemporaryManager.DeleteLocalExecuteSpace(space);
                    return;
                }

                // バックグラウンドタスクでアップロード
                RefreshUITarget uiTarget = new RefreshUITarget(Program.MainWindow.LeftFileListView, Program.MainWindow.RightFileListView, RefreshUITarget.RefreshMode.RefreshBoth, RefreshUITarget.RefreshOption.None);
                FileSystemFactory factory = Program.Document.FileSystemFactory;

                IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileSystemID.Windows);
                IFileProviderSrc srcProvider = new FileProviderSrcSpecified(srcPath, srcFileSystem, null);
                IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(space.RemoteFileSystemId);
                IFileProviderDest destProvider = new FileProviderDestSimple(space.RemoteDirectory, destFileSystem, space.FileListContext);

                LocalUploadBackgroundTask task = new LocalUploadBackgroundTask(srcProvider, destProvider, uiTarget, space);
                Program.Document.BackgroundTaskManager.StartFileTask(task, true);
            }
        }
    }
}
