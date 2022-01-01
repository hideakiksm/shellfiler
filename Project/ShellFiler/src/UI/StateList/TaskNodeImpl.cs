using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Management;
using ShellFiler.Util;
using ShellFiler.Locale;

namespace ShellFiler.UI.StateList {

    //=========================================================================================
    // クラス：タスク関連のノードの処理を実装するクラス
    //=========================================================================================
    class TaskNodeImpl {
        // 所有パネル
        private StateListPanel m_parent;

        // 実行中のタスクのトップレベルノード
        private TreeNode m_rootNode;

        // 実行中のタスクの項目なしのノード
        private TreeNode m_noListNode;

        // 待機中タスクのノード
        private TreeNode m_waitingTaskNode;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent     所有パネル
        // 戻り値：なし
        //=========================================================================================
        public TaskNodeImpl(StateListPanel parent) {
            m_parent = parent;

            // 管理用ノードの作成
            m_rootNode = new TreeNode(Resources.StateView_RunningTask);
            m_parent.TreeView.Nodes.Add(m_rootNode);

            m_noListNode = new TreeNode(Resources.StateView_NoRunningTask);
            m_rootNode.Nodes.Add(m_noListNode);
            m_rootNode.ToolTipText = Resources.StateView_RunningTaskHint;
            m_rootNode.Expand();

            m_waitingTaskNode = new TreeNode(Resources.StateView_WaitingTask);
            m_waitingTaskNode.ImageIndex = (int)(IconImageListID.Icon_TaskWait);
            m_waitingTaskNode.SelectedImageIndex = m_waitingTaskNode.ImageIndex;
            m_waitingTaskNode.ToolTipText = Resources.StateView_WaitingTaskHint;

            // イベントを設定
            Program.Document.BackgroundTaskManager.TaskChanged += new BackgroundTaskManager.TaskChangedEventHandler(this.TaskManager_StateChanged);
        }

        //=========================================================================================
        // 機　能：タスク一覧のTreeViewノードを作成する
        // 引　数：[in]info  タスクの情報
        // 戻り値：TreeViewの項目
        //=========================================================================================
        private TreeNode CreateTaskListNode(FileTaskDisplayInfo info) {
            string text, hint;
            CreateTaskNodeText(info, out text, out hint);
            TreeNode node = new TreeNode(text);
            node.ImageIndex = (int)info.BackgroundTaskType.DialogIconId;
            node.SelectedImageIndex = node.ImageIndex;
            node.ToolTipText = hint;
            node.Tag = info;
            return node;
        }

        //=========================================================================================
        // 機　能：タスクの表示名を作成する
        // 引　数：[in]info   タスクの情報
        // 　　　　[out]text  タスクの表示名を返す文字列
        // 　　　　[out]hint  ヒントとして表示する転送元/転送先の情報を返す変数
        // 戻り値：なし
        //=========================================================================================
        public static void CreateTaskNodeText(FileTaskDisplayInfo info, out string text, out string hint) {
            hint = info.PathInfo.SrcShort;
            if (info.PathInfo.DestShort != "") {
                hint += Resources.DlgTaskMan_Arrow + info.PathInfo.DestShort;
            }

            if (info.PathInfo.ProgressAll == 0) {
                text = info.BackgroundTaskType.DisplayName + " " + hint;
            } else {
                text = string.Format(Resources.StateView_TaskDisplayName, info.BackgroundTaskType.DisplayName, info.PathInfo.ProgressCurrent, info.PathInfo.ProgressAll) + " " + hint;
            }
        }



        //=========================================================================================
        // 機　能：バックグラウンドタスクマネージャの状態が変わった時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TaskManager_StateChanged(object sender, BackgroundTaskManager.TaskChangedEventArgs evt) {
            if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.TaskAdded) {
                OnTaskAdded(evt);
            } else if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.TaskDeleted) {
                OnTaskDeleted(evt);
            } else if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.TaskResumed) {
                OnTaskResumed(evt);
            } else if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.TaskSuspended) {
                OnTaskSuspended(evt);
            } else if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.ProgressChanged) {
                OnProgressChanged(evt);
            } else if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.WaitingPriorityPlus) {
                OnWaitingPriorityPlus(evt);
            } else if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.WaitingPriorityMinus) {
                OnWaitingPriorityMinus(evt);
            }
        }

        //=========================================================================================
        // 機　能：タスクが追加されたときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnTaskAdded(BackgroundTaskManager.TaskChangedEventArgs evt) {
            // ノードなしの説明があれば削除
            if (m_rootNode.Nodes.Count == 1 && m_rootNode.Nodes[0] == m_noListNode) {
                m_rootNode.Nodes.RemoveAt(0);
            }

            FileTaskDisplayInfo dispInfo = evt.DisplayInfo;
            if (dispInfo.RunningType == BackgroundTaskRunningType.Waiting) {
                // 待機中ノードの追加
                if (!m_rootNode.Nodes.Contains(m_waitingTaskNode)) {
                    m_rootNode.Nodes.Add(m_waitingTaskNode);
                    m_rootNode.Expand();
                }
                TreeNode newNode = CreateTaskListNode(evt.DisplayInfo);
                m_waitingTaskNode.Nodes.Add(newNode);
                m_waitingTaskNode.Expand();
            } else {
                // 実行中ノードの追加
                TreeNode newNode = CreateTaskListNode(evt.DisplayInfo);
                if (m_rootNode.Nodes.Contains(m_waitingTaskNode)) {
                    m_rootNode.Nodes.Insert(m_rootNode.Nodes.Count - 1, newNode);
                } else {
                    m_rootNode.Nodes.Add(newNode);
                }
                m_rootNode.Expand();
            }
        }

        //=========================================================================================
        // 機　能：タスクが削除されたときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnTaskDeleted(BackgroundTaskManager.TaskChangedEventArgs evt) {
            FileTaskDisplayInfo reqDispInfo = evt.DisplayInfo;
            int index = GetTaskNodeIndex(m_rootNode, reqDispInfo.TaskId);
            if (index != -1) {
                // 実行中タスクから削除
                m_rootNode.Nodes.RemoveAt(index);
            }
            if (m_rootNode.Nodes.Count == 0) {
                m_rootNode.Nodes.Add(m_noListNode);
            }
            m_rootNode.Expand();
        }

        //=========================================================================================
        // 機　能：タスクが再開されたときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnTaskResumed(BackgroundTaskManager.TaskChangedEventArgs evt) {
            FileTaskDisplayInfo reqDispInfo = evt.DisplayInfo;
            int index = GetTaskNodeIndex(m_waitingTaskNode, reqDispInfo.TaskId);
            if (index == -1) {
                return;
            }

            // 待機中タスクから削除
            TreeNode node = m_waitingTaskNode.Nodes[index];
            m_waitingTaskNode.Nodes.RemoveAt(index);
            if (m_waitingTaskNode.Nodes.Count == 0) {
                m_rootNode.Nodes.Remove(m_waitingTaskNode);
            }

            // 実行中タスクに追加
            if (m_rootNode.Nodes.Contains(m_waitingTaskNode)) {
                m_rootNode.Nodes.Insert(m_rootNode.Nodes.Count - 1, node);
            } else {
                m_rootNode.Nodes.Add(node);
            }
            node.Tag = evt.DisplayInfo;
            m_rootNode.Expand();
        }

        //=========================================================================================
        // 機　能：タスクが中断されたときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnTaskSuspended(BackgroundTaskManager.TaskChangedEventArgs evt) {
            FileTaskDisplayInfo reqDispInfo = evt.DisplayInfo;
            int index = GetTaskNodeIndex(m_rootNode, reqDispInfo.TaskId);
            if (index == -1) {
                return;
            }

            // 実行中タスクから削除
            TreeNode node = m_rootNode.Nodes[index];
            m_rootNode.Nodes.RemoveAt(index);

            // 待機中タスクに追加
            if (!m_rootNode.Nodes.Contains(m_waitingTaskNode)) {
                m_rootNode.Nodes.Add(m_waitingTaskNode);
                m_rootNode.Expand();
            }
            m_waitingTaskNode.Nodes.Add(node);
            node.Tag = evt.DisplayInfo;
            m_waitingTaskNode.Expand();
        }

        //=========================================================================================
        // 機　能：指定されたタスクIDのツリーノードのインデックスを返す
        // 引　数：[in]node  調べるルートとなるノード
        // 　　　　[in]evt   イベントの詳細
        // 戻り値：rootNodeに対するツリーノードのインデックス
        //=========================================================================================
        private int GetTaskNodeIndex(TreeNode node, BackgroundTaskID taskId) {
            int count = node.Nodes.Count;
            for (int i = 0; i < count; i++) {
                TreeNode subNode = node.Nodes[i];
                if (subNode == m_noListNode || subNode == m_waitingTaskNode) {
                    continue;
                }
                FileTaskDisplayInfo itemDispInfo = (FileTaskDisplayInfo)(subNode.Tag);
                if (itemDispInfo.TaskId == taskId) {
                    return i;
                }
            }
            return -1;
        }

        //=========================================================================================
        // 機　能：タスクの進捗状態が変わったときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnProgressChanged(BackgroundTaskManager.TaskChangedEventArgs evt) {
            FileTaskDisplayInfo dispInfo = evt.DisplayInfo;
            int index = GetTaskNodeIndex(m_rootNode, dispInfo.TaskId);
            if (index == -1) {
                return;
            }

            Graphics graphics = m_parent.TreeView.CreateGraphics();
            try {
                TreeNode node = m_rootNode.Nodes[index];
                Point origin = new Point(0, node.Bounds.Top);
                bool isSelect = (node == m_parent.TreeView.SelectedNode && m_parent.TreeView.Focused);
                TreeViewRenderer renderer = new TreeViewRenderer(m_parent);
                renderer.DrawTaskItem(graphics, origin, node, isSelect);
            } finally {
                graphics.Dispose();
            }
        }
        
        //=========================================================================================
        // 機　能：待機中タスクの優先度が上がったときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnWaitingPriorityPlus(BackgroundTaskManager.TaskChangedEventArgs evt) {
            FileTaskDisplayInfo dispInfo = evt.DisplayInfo;
            int index = GetTaskNodeIndex(m_waitingTaskNode, dispInfo.TaskId);
            if (index == -1 || index == 0) {
                return;
            }
            TreeNode node = m_waitingTaskNode.Nodes[index];
            m_waitingTaskNode.Nodes.RemoveAt(index);
            m_waitingTaskNode.Nodes.Insert(index - 1, node);
            m_waitingTaskNode.Expand();
        }

        //=========================================================================================
        // 機　能：待機中タスクの優先度が下がったときの処理を行う
        // 引　数：[in]evt   イベントの詳細
        // 戻り値：なし
        //=========================================================================================
        private void OnWaitingPriorityMinus(BackgroundTaskManager.TaskChangedEventArgs evt) {
            FileTaskDisplayInfo dispInfo = evt.DisplayInfo;
            int index = GetTaskNodeIndex(m_waitingTaskNode, dispInfo.TaskId);
            if (index == -1 || index == m_waitingTaskNode.Nodes.Count - 1) {
                return;
            }
            TreeNode node = m_waitingTaskNode.Nodes[index];
            m_waitingTaskNode.Nodes.RemoveAt(index);
            m_waitingTaskNode.Nodes.Insert(index + 1, node);
            m_waitingTaskNode.Expand();
        }



        //=========================================================================================
        // 機　能：ツリーの項目がクリックされたときの処理を行う
        // 引　数：[in]node  クリックされたノード
        // 戻り値：なし
        //=========================================================================================
        public void OnNodeMouseClick(TreeNode node) {
            StateMenu[] menuItemList = null;
            if (node != m_noListNode && node != m_waitingTaskNode && node.Parent == m_rootNode) {
                // 実行中のタスク
                menuItemList = new StateMenu[] {
                    new StateMenu(true, Resources.StateView_MenuTaskCancel),
                    new StateMenu(true, Resources.StateView_MenuTaskSuspend),
                    new StateMenu(true, null),
                    new StateMenu(true, Resources.StateView_MenuTaskCancelAll),
                    new StateMenu(true, Resources.StateView_MenuTaskBackground),
                };
            } else if (node.Parent == m_waitingTaskNode) {
                // 待機中のタスク
                bool waitingFirst = (m_waitingTaskNode.Nodes.Count > 0 && m_waitingTaskNode.Nodes[0] == node);
                bool waitingLast = (m_waitingTaskNode.Nodes.Count > 0 && m_waitingTaskNode.Nodes[m_waitingTaskNode.Nodes.Count - 1] == node);
                menuItemList = new StateMenu[] {
                    new StateMenu(true, Resources.StateView_MenuTaskCancel),
                    new StateMenu(true, Resources.StateView_MenuTaskResume),
                    new StateMenu(!waitingFirst, Resources.StateView_MenuTaskPriorityPlus),
                    new StateMenu(!waitingLast, Resources.StateView_MenuTaskPriorityMinus),
                    new StateMenu(true, null),
                    new StateMenu(true, Resources.StateView_MenuTaskCancelAll),
                    new StateMenu(true, Resources.StateView_MenuTaskBackground),
                };
            } else if (node == m_rootNode) {
                if (node.Nodes.Count > 0 && node.Nodes[0] != m_noListNode) {
                    // 有効なタスクがある状態のバックグラウンドタスク
                    menuItemList = new StateMenu[] {
                        new StateMenu(true, Resources.StateView_MenuTaskCancelAll),
                        new StateMenu(true, Resources.StateView_MenuTaskBackground),
                    };
                } else {
                    // 有効なタスクがない状態のバックグラウンドタスク
                    menuItemList = new StateMenu[] {
                        new StateMenu(false, Resources.StateView_MenuTaskCancelAll),
                        new StateMenu(true, Resources.StateView_MenuTaskBackground),
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
            if (menuText == Resources.StateView_MenuTaskCancel) {
                // このタスクを中止
                FileTaskDisplayInfo info = (FileTaskDisplayInfo)(node.Tag);
                Program.Document.BackgroundTaskManager.CancelBackgroundTask(info.TaskId, false);
            } else if (menuText == Resources.StateView_MenuTaskCancelAll) {
                // すべてのタスクを中止
                BackgroundTaskManager taskMan = Program.Document.BackgroundTaskManager;
                List<FileTaskDisplayInfo> runningList, waitingList;
                taskMan.GetDisplayInfoList(out runningList, out waitingList);
                foreach (FileTaskDisplayInfo dispInfo in runningList) {
                    Program.Document.BackgroundTaskManager.CancelBackgroundTask(dispInfo.TaskId, false);
                }
                foreach (FileTaskDisplayInfo dispInfo in waitingList) {
                    Program.Document.BackgroundTaskManager.CancelBackgroundTask(dispInfo.TaskId, false);
                }
            } else if (menuText == Resources.StateView_MenuTaskBackground) {
                // バックグラウンドマネージャ
                Program.MainWindow.OnUICommand(UICommandSender.StateListPanel, UICommandItem.StateList_NoTask);
            } else if (menuText == Resources.StateView_MenuTaskSuspend) {
                // 中断
                FileTaskDisplayInfo info = (FileTaskDisplayInfo)(node.Tag);
                BackgroundTaskManager manager = Program.Document.BackgroundTaskManager;
                IBackgroundTask task = manager.GetTaskFromTaskId(info.TaskId);
                if (task == null) {
                    return;
                }
                BackgroundTaskRunningType runType = manager.GetRunningTypeFromActiveTask(task);
                if (runType != BackgroundTaskRunningType.Waitable) {
                    InfoBox.Warning(Program.MainWindow, Resources.StateView_NotWaitableTask);
                    return;
                }
                manager.SetSuspendState(task, true);
            } else if (menuText == Resources.StateView_MenuTaskResume) {
                // 再開
                FileTaskDisplayInfo info = (FileTaskDisplayInfo)(node.Tag);
                BackgroundTaskManager manager = Program.Document.BackgroundTaskManager;
                IBackgroundTask task = manager.GetTaskFromTaskId(info.TaskId);
                if (task == null) {
                    return;
                }
                BackgroundTaskRunningType runType = manager.GetRunningTypeFromActiveTask(task);
                if (runType != BackgroundTaskRunningType.Waiting) {
                    return;
                }
                manager.SetSuspendState(task, false);
            } else if (menuText == Resources.StateView_MenuTaskPriorityPlus) {
                // 優先度+1
                FileTaskDisplayInfo info = (FileTaskDisplayInfo)(node.Tag);
                Program.Document.BackgroundTaskManager.ChangeWaitingPriority(info.TaskId, true);
            } else if (menuText == Resources.StateView_MenuTaskPriorityMinus) {
                // 優先度-1
                FileTaskDisplayInfo info = (FileTaskDisplayInfo)(node.Tag);
                Program.Document.BackgroundTaskManager.ChangeWaitingPriority(info.TaskId, false);
            }
        }

        //=========================================================================================
        // プロパティ：トップレベルノード(実行中のタスク)
        //=========================================================================================
        public TreeNode RootNodeTaskManager {
            get {
                return m_rootNode;
            }
        }

        //=========================================================================================
        // プロパティ：項目なしのノード(実行中のタスク)
        //=========================================================================================
        public TreeNode NoListTaskManager {
            get {
                return m_noListNode;
            }
        }

        //=========================================================================================
        // プロパティ：待機中タスクのノード
        //=========================================================================================
        public TreeNode WaitingTaskNode {
            get {
                return m_waitingTaskNode;
            }
        }
    }
}
