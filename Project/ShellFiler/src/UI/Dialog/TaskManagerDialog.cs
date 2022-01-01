using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.SSH;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Management;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：タスクマネージャ
    //=========================================================================================
    public partial class TaskManagerDialog : Form {
        // バックグラウンドタスクページ
        private BackgroundTaskPage m_backgroundTaskPage;

        // SSH接続ページ
        private SSHConnectionPage m_sshConnectionPage;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TaskManagerDialog() {
            InitializeComponent();
            m_backgroundTaskPage = new BackgroundTaskPage(this);
            m_sshConnectionPage = new SSHConnectionPage(this);
        }
    
        //=========================================================================================
        // 機　能：SSHのセッションを切断する
        // 引　数：[in]info    切断対象のセッション情報
        // 　　　　[in]parent  メッセージボックスの親となるコントロール
        // 戻り値：なし
        //=========================================================================================
        public static void DisconnectSSHSession(UIConnectionPoolInfo info, Form parent) {
            // 使用中の接続数を確認
            HashSet<BackgroundTaskID> inUseTask = new HashSet<BackgroundTaskID>();
            foreach (BackgroundTaskID taskId in info.InUseTaskIdList) {
                inUseTask.Add(taskId);
            }

            // 使用中のものがあれば了解を得る
            bool closeAll = false;
            if (inUseTask.Count > 0) {
                DialogResult result = InfoBox.Message(parent, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, Resources.DlgTaskMan_CloseInUse);
                if (result != DialogResult.Yes) {
                    return;
                }
            }

            // 切断
            if (closeAll) {
                foreach (BackgroundTaskID taskId in inUseTask) {
                    Program.Document.BackgroundTaskManager.CancelBackgroundTask(taskId, false);
                }
            }
            SSHConnectionManager manager = Program.Document.FileSystemFactory.SSHConnectionManager;
            manager.CloseServer(info.Server, info.User, info.PortNo);

            // 表示中であれば初期フォルダへ
            if (FileSystemID.IsSSH(Program.Document.CurrentTabPage.LeftFileList.FileSystem.FileSystemId)) {
                string filePath = Program.Document.CurrentTabPage.LeftFileList.DisplayDirectoryName;
                SSHProtocolType protocol;
                string user, server, path;
                int portNo;
                SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
                if (info.Server == server && info.User == user && info.PortNo == portNo) {
                    ChdirCommand.ChangeDirectory(Program.MainWindow.LeftFileListView, new ChangeDirectoryParam.Direct(UIFileList.InitialFolder));
                }
            }
            if (FileSystemID.IsSSH(Program.Document.CurrentTabPage.RightFileList.FileSystem.FileSystemId)) {
                string filePath = Program.Document.CurrentTabPage.RightFileList.DisplayDirectoryName;
                SSHProtocolType protocol;
                string user, server, path;
                int portNo;
                SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
                if (info.Server == server && info.User == user && info.PortNo == portNo) {
                    ChdirCommand.ChangeDirectory(Program.MainWindow.RightFileListView, new ChangeDirectoryParam.Direct(UIFileList.InitialFolder));
                }
            }
        }

        //=========================================================================================
        // 機　能：SSHのセッションをすべて切断する
        // 引　数：[in]parent  メッセージボックスの親となるコントロール
        // 戻り値：なし
        //=========================================================================================
        public static void DisconnectAllSSHSession(Form parent) {
            // 使用中の接続数を確認
            SSHConnectionManager manager = Program.Document.FileSystemFactory.SSHConnectionManager;
            List<UIConnectionPoolInfo> infoList = manager.UIConnectionPoolList;
            HashSet<BackgroundTaskID> inUseTask = new HashSet<BackgroundTaskID>();
            foreach (UIConnectionPoolInfo info in infoList) {
                foreach (BackgroundTaskID taskId in info.InUseTaskIdList) {
                    inUseTask.Add(taskId);
                }
            }

            // 使用中のものがあれば了解を得る
            bool closeAll = false;
            if (inUseTask.Count > 0) {
                DialogResult result = InfoBox.Message(parent, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, Resources.DlgTaskMan_CloseInUse);
                if (result != DialogResult.Yes) {
                    return;
                }
            }

            // 切断
            if (closeAll) {
                foreach (BackgroundTaskID taskId in inUseTask) {
                    Program.Document.BackgroundTaskManager.CancelBackgroundTask(taskId, false);
                }
            }
            manager.CloseAll();

            // 表示中であれば初期フォルダへ
            if (FileSystemID.IsSSH(Program.Document.CurrentTabPage.LeftFileList.FileSystem.FileSystemId)) {
                ChdirCommand.ChangeDirectory(Program.MainWindow.LeftFileListView, new ChangeDirectoryParam.Direct(UIFileList.InitialFolder));
            }
            if (FileSystemID.IsSSH(Program.Document.CurrentTabPage.RightFileList.FileSystem.FileSystemId)) {
                ChdirCommand.ChangeDirectory(Program.MainWindow.RightFileListView, new ChangeDirectoryParam.Direct(UIFileList.InitialFolder));
            }
        }

        //=========================================================================================
        // クラス：バックグラウンドタスクページ
        //=========================================================================================
        private class BackgroundTaskPage {
            // 所有ダイアログ
            private TaskManagerDialog m_parent;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  所有ダイアログ
            // 戻り値：なし
            //=========================================================================================
            public BackgroundTaskPage(TaskManagerDialog parent) {
                m_parent = parent;

                // タスク一覧ヘッダを追加
                ColumnHeader runColumnType = new ColumnHeader();
                runColumnType.Text = Resources.DlgTaskMan_ListType;
                runColumnType.Width = 100;
                ColumnHeader runColumnFrom = new ColumnHeader();
                runColumnFrom.Text = Resources.DlgTaskMan_ListFrom;
                runColumnFrom.Width = 150;
                ColumnHeader runColumnTo = new ColumnHeader();
                runColumnTo.Text = Resources.DlgTaskMan_ListTo;
                runColumnTo.Width = 150;
                m_parent.listViewTask.Columns.AddRange(new ColumnHeader[] {runColumnType, runColumnFrom, runColumnTo});

                ColumnHeader waitColumnType = new ColumnHeader();
                waitColumnType.Text = Resources.DlgTaskMan_ListType;
                waitColumnType.Width = 100;
                ColumnHeader waitColumnFrom = new ColumnHeader();
                waitColumnFrom.Text = Resources.DlgTaskMan_ListFrom;
                waitColumnFrom.Width = 150;
                ColumnHeader waitColumnTo = new ColumnHeader();
                waitColumnTo.Text = Resources.DlgTaskMan_ListTo;
                waitColumnTo.Width = 150;
                m_parent.listViewWait.Columns.AddRange(new ColumnHeader[] {waitColumnType, waitColumnFrom, waitColumnTo});

                // タスク一覧を追加
                BackgroundTaskManager taskMan = Program.Document.BackgroundTaskManager;
                List<FileTaskDisplayInfo> runningList, waitingList;
                taskMan.GetDisplayInfoList(out runningList, out waitingList);
                m_parent.listViewTask.SmallImageList = UIIconManager.IconImageList;
                for (int i = 0; i < runningList.Count; i++) {
                    ListViewItem item = CreateTaskListViewItem(runningList[i]);
                    m_parent.listViewTask.Items.Add(item);
                }

                m_parent.listViewWait.SmallImageList = UIIconManager.IconImageList;
                for (int i = 0; i < waitingList.Count; i++) {
                    ListViewItem item = CreateTaskListViewItem(waitingList[i]);
                    m_parent.listViewWait.Items.Add(item);
                }

                // その他初期化
                EnableUIItem();

                // イベントを登録
                Program.Document.BackgroundTaskManager.TaskChanged += new BackgroundTaskManager.TaskChangedEventHandler(this.TaskManager_StateChanged);
                m_parent.FormClosed += new FormClosedEventHandler(TaskManagerDialog_FormClosed);
                m_parent.buttonSuspend.Click += new EventHandler(buttonSuspend_Click);
                m_parent.buttonCancelWait.Click += new EventHandler(buttonCancelWait_Click);
                m_parent.buttonResume.Click += new EventHandler(buttonResume_Click);
                m_parent.buttonCancel.Click += new EventHandler(buttonCancel_Click);
                m_parent.buttonCacnelAll.Click += new EventHandler(buttonCancelAll_Click);
                m_parent.buttonPriorityPlus.Click += new EventHandler(buttonPriorityPlus_Click);
                m_parent.buttonPriorityMinus.Click += new EventHandler(buttonPriorityMinus_Click);
                m_parent.listViewTask.SelectedIndexChanged += new EventHandler(listView_SelectedIndexChanged);
                m_parent.listViewWait.SelectedIndexChanged += new EventHandler(listView_SelectedIndexChanged);
                m_parent.ActiveControl = m_parent.buttonCacnelAll;
            }
            
            //=========================================================================================
            // 機　能：フォームが閉じられたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void TaskManagerDialog_FormClosed(object sender, FormClosedEventArgs evt) {
                Program.Document.BackgroundTaskManager.TaskChanged -= new BackgroundTaskManager.TaskChangedEventHandler(this.TaskManager_StateChanged);
            }

            //=========================================================================================
            // 機　能：タスク一覧のListView項目を作成する
            // 引　数：[in]info  タスクの情報
            // 戻り値：ListViewの項目
            //=========================================================================================
            private ListViewItem CreateTaskListViewItem(FileTaskDisplayInfo info) {
                string[] itemString = {info.BackgroundTaskType.DisplayName, info.PathInfo.SrcShort, info.PathInfo.DestShort};
                ListViewItem item = new ListViewItem(itemString);
                item.ImageIndex = (int)info.BackgroundTaskType.DialogIconId;
                item.ToolTipText = info.PathInfo.SrcDetail;
                item.Tag = info;
                return item;
            }
            
            //=========================================================================================
            // 機　能：UIの有効/無効を切り替える
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EnableUIItem() {
                // すべて中止
                if (Program.Document.BackgroundTaskManager.Count > 0) {
                    m_parent.buttonCacnelAll.Enabled = true;
                } else {
                    m_parent.buttonCacnelAll.Enabled = false;
                }

                // 実行中タスクの中止/中断
                FileTaskDisplayInfo itemTask = GetSelectedItem(m_parent.listViewTask);
                if (itemTask != null && itemTask.Running) {
                    m_parent.buttonCancel.Enabled = true;
                    if (itemTask.RunningType == BackgroundTaskRunningType.Waitable) {
                        m_parent.buttonSuspend.Enabled = true;
                    } else {
                        m_parent.buttonSuspend.Enabled = false;
                    }
                } else {
                    m_parent.buttonCancel.Enabled = false;
                    m_parent.buttonSuspend.Enabled = false;
                }

                // 待機中タスクの中止/再開/優先度
                FileTaskDisplayInfo itemWait = GetSelectedItem(m_parent.listViewWait);
                if (itemWait != null && itemWait.Running) {
                    m_parent.buttonCancelWait.Enabled = true;
                    m_parent.buttonResume.Enabled = true;
                    if (itemWait == m_parent.listViewWait.Items[0].Tag) {
                        m_parent.buttonPriorityPlus.Enabled = false;
                    } else {
                        m_parent.buttonPriorityPlus.Enabled = true;
                    }
                    if (itemWait == m_parent.listViewWait.Items[m_parent.listViewWait.Items.Count - 1].Tag) {
                        m_parent.buttonPriorityMinus.Enabled = false;
                    } else {
                        m_parent.buttonPriorityMinus.Enabled = true;
                    }
                } else {
                    m_parent.buttonCancelWait.Enabled = false;
                    m_parent.buttonResume.Enabled = false;
                    m_parent.buttonPriorityPlus.Enabled = false;
                    m_parent.buttonPriorityMinus.Enabled = false;
                }
            }

            //=========================================================================================
            // 機　能：バックグラウンドタスク実行中のキャンセルボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonCancel_Click(object sender, EventArgs evt) {
                FileTaskDisplayInfo info = GetSelectedItem(m_parent.listViewTask);
                if (info == null) {
                    return;
                }
                Program.Document.BackgroundTaskManager.CancelBackgroundTask(info.TaskId, false);
            }

            //=========================================================================================
            // 機　能：バックグラウンドタスク待機中のキャンセルボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonCancelWait_Click(object sender, EventArgs evt) {
                FileTaskDisplayInfo info = GetSelectedItem(m_parent.listViewWait);
                if (info == null) {
                    return;
                }
                Program.Document.BackgroundTaskManager.CancelBackgroundTask(info.TaskId, false);
            }

            //=========================================================================================
            // 機　能：選択中の項目を返す
            // 引　数：[in]listView   確認対象のリストビュー
            // 戻り値：選択中の項目（選択されていないときnull）
            //=========================================================================================
            private FileTaskDisplayInfo GetSelectedItem(ListView listView) {
                ListView.SelectedListViewItemCollection itemCollection = listView.SelectedItems;
                if (itemCollection.Count == 0) {
                    return null;
                }
                ListViewItem item = itemCollection[0];

                FileTaskDisplayInfo info = (FileTaskDisplayInfo)item.Tag;
                return info;
            }

            //=========================================================================================
            // 機　能：指定されたタスクIDに対応するリストビューの項目を返す
            // 引　数：[in]taskId   確認対象のタスクID
            // 　　　　[out]isWait  実行中タスクのときfalse、待機中タスクのときtrueを返す変数
            // 　　　　[out]item    リストビューの項目を返す変数（見つからないときnull）
            // 戻り値：なし
            //=========================================================================================
            private void GetTaskItem(BackgroundTaskID taskId, out bool isWait, out ListViewItem item) {
                foreach (ListViewItem lvItem in m_parent.listViewTask.Items) {
                    BackgroundTaskID itemTaskId = ((FileTaskDisplayInfo)(lvItem.Tag)).TaskId;
                    if (taskId == itemTaskId) {
                        isWait = false;
                        item = lvItem;
                        return;
                    }
                }
                foreach (ListViewItem lvItem in m_parent.listViewWait.Items) {
                    BackgroundTaskID itemTaskId = ((FileTaskDisplayInfo)(lvItem.Tag)).TaskId;
                    if (taskId == itemTaskId) {
                        isWait = true;
                        item = lvItem;
                        return;
                    }
                }
                isWait = false;
                item = null;
            }

            //=========================================================================================
            // 機　能：バックグラウンドタスクの一時停止ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonSuspend_Click(object sender, EventArgs evt) {
                FileTaskDisplayInfo info = GetSelectedItem(m_parent.listViewTask);
                if (info == null) {
                    return;
                }
                BackgroundTaskManager manager = Program.Document.BackgroundTaskManager;
                IBackgroundTask task = manager.GetTaskFromTaskId(info.TaskId);
                if (task == null) {
                    return;
                }
                BackgroundTaskRunningType runType = manager.GetRunningTypeFromActiveTask(task);
                if (runType != BackgroundTaskRunningType.Waitable) {
                    return;
                }
                manager.SetSuspendState(task, true);
            }

            //=========================================================================================
            // 機　能：バックグラウンドタスクの再開ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonResume_Click(object sender, EventArgs evt) {
                FileTaskDisplayInfo info = GetSelectedItem(m_parent.listViewWait);
                if (info == null) {
                    return;
                }
                BackgroundTaskManager manager = Program.Document.BackgroundTaskManager;
                IBackgroundTask task = manager.GetTaskFromTaskId(info.TaskId);
                if (task == null) {
                    return;
                }
                BackgroundTaskRunningType runType = manager.GetRunningTypeFromActiveTask(task);
                if (runType != BackgroundTaskRunningType.Waiting) {
                    return;
                }
                BackgroundTaskRunningType nextRunType = manager.GetRunningType(task.GetType(), true);
                if (nextRunType != BackgroundTaskRunningType.Waitable) {
                    InfoBox.Warning(m_parent, Resources.DlgTaskMan_TaskResumeFailed);
                    return;
                }
                manager.SetSuspendState(task, false);
            }

            //=========================================================================================
            // 機　能：バックグラウンドタスクのすべてキャンセルボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonCancelAll_Click(object sender, EventArgs evt) {
                BackgroundTaskManager taskMan = Program.Document.BackgroundTaskManager;
                List<FileTaskDisplayInfo> runningList, waitingList;
                taskMan.GetDisplayInfoList(out runningList, out waitingList);
                foreach (FileTaskDisplayInfo dispInfo in runningList) {
                    Program.Document.BackgroundTaskManager.CancelBackgroundTask(dispInfo.TaskId, false);
                }
                foreach (FileTaskDisplayInfo dispInfo in waitingList) {
                    Program.Document.BackgroundTaskManager.CancelBackgroundTask(dispInfo.TaskId, false);
                }
            }

            //=========================================================================================
            // 機　能：バックグラウンドタスクの優先度プラスのボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonPriorityPlus_Click(object sender, EventArgs evt) {
                FileTaskDisplayInfo info = GetSelectedItem(m_parent.listViewWait);
                if (info == null) {
                    return;
                }
                Program.Document.BackgroundTaskManager.ChangeWaitingPriority(info.TaskId, true);
            }

            //=========================================================================================
            // 機　能：バックグラウンドタスクの優先度マイナスのボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonPriorityMinus_Click(object sender, EventArgs evt) {
                FileTaskDisplayInfo info = GetSelectedItem(m_parent.listViewWait);
                if (info == null) {
                    return;
                }
                Program.Document.BackgroundTaskManager.ChangeWaitingPriority(info.TaskId, false);
            }

            //=========================================================================================
            // 機　能：バックグラウンドタスクのリストボックスの選択が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void listView_SelectedIndexChanged(object sender, EventArgs evt) {
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：バックグラウンドタスクマネージャの状態が変わった時の処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void TaskManager_StateChanged(object sender, BackgroundTaskManager.TaskChangedEventArgs evt) {
                if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.TaskAdded) {
                    // リストに項目を追加
                    OnTaskManagerTaskAdded(evt);
                } else if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.TaskDeleted) {
                    // リスト項目を削除状態に変更
                    OnTaskManagerTaskDeleted(evt);
                } else if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.TaskSuspended) {
                    // 実行中→待機中に変更
                    OnTaskManagerTaskSuspended(evt);
                } else if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.TaskResumed) {
                    // 待機中→実行中に変更
                    OnTaskManagerTaskResumed(evt);
                } else if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.WaitingPriorityPlus) {
                    // 待機中優先度：１つ高く
                    OnTaskManagerWaitingPriorityChange(evt, true);
                } else if (evt.EventType == BackgroundTaskManager.TaskChangedEventType.WaitingPriorityMinus) {
                    // 待機中優先度：１つ低く
                    OnTaskManagerWaitingPriorityChange(evt, false);
                }
            }

            //=========================================================================================
            // 機　能：リストに項目を追加する
            // 引　数：[in]evt   StateChangedイベントの詳細
            // 戻り値：なし
            //=========================================================================================
            private void OnTaskManagerTaskAdded(BackgroundTaskManager.TaskChangedEventArgs evt) {
                m_parent.listViewTask.Items.Add(CreateTaskListViewItem(evt.DisplayInfo));
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：リスト項目を削除状態に変更する
            // 引　数：[in]evt   StateChangedイベントの詳細
            // 戻り値：なし
            //=========================================================================================
            private void OnTaskManagerTaskDeleted(BackgroundTaskManager.TaskChangedEventArgs evt) {
                FileTaskDisplayInfo reqDispInfo = evt.DisplayInfo;
                int count = m_parent.listViewTask.Items.Count;
                for (int i = 0; i < count; i++) {
                    FileTaskDisplayInfo itemDispInfo = (FileTaskDisplayInfo)(m_parent.listViewTask.Items[i].Tag);
                    if (itemDispInfo.TaskId == reqDispInfo.TaskId) {
                        FileTaskDisplayInfo itemInfo = (FileTaskDisplayInfo)(m_parent.listViewTask.Items[i].Tag);
                        itemInfo.Running = false;
                        m_parent.listViewTask.Items[i].SubItems[0].Text = Resources.DlgTaskMan_ListFinish + itemDispInfo.BackgroundTaskType.DisplayName;
                        m_parent.listViewTask.Items[i].ImageIndex = (int)IconImageListID.Icon_BackgroundTaskFinish;
                        EnableUIItem();
                        break;
                    }
                }
            }

            //=========================================================================================
            // 機　能：実行中→待機中に変更する
            // 引　数：[in]evt   StateChangedイベントの詳細
            // 戻り値：なし
            //=========================================================================================
            private void OnTaskManagerTaskSuspended(BackgroundTaskManager.TaskChangedEventArgs evt) {
                bool isWait;
                ListViewItem item;
                GetTaskItem(evt.DisplayInfo.TaskId, out isWait, out item);
                if (item == null || isWait) {
                    Program.Abort("実行中→待機中への変更で対象となる項目が見つかりません。");
                }
                SelectNeighbor(m_parent.listViewTask, item);
                m_parent.listViewTask.Items.Remove(item);
                m_parent.listViewWait.Items.Add(item);
                item.Selected = true;
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：待機中→実行中に変更する
            // 引　数：[in]evt   StateChangedイベントの詳細
            // 戻り値：なし
            //=========================================================================================
            private void OnTaskManagerTaskResumed(BackgroundTaskManager.TaskChangedEventArgs evt) {
                bool isWait;
                ListViewItem item;
                GetTaskItem(evt.DisplayInfo.TaskId, out isWait, out item);
                if (item == null || !isWait) {
                    Program.Abort("待機中→実行中への変更で対象となる項目が見つかりません。");
                }
                SelectNeighbor(m_parent.listViewWait, item);
                m_parent.listViewWait.Items.Remove(item);
                m_parent.listViewTask.Items.Add(item);
                item.Selected = true;
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：待機中優先度：１つ高く/低くする
            // 引　数：[in]evt   StateChangedイベントの詳細
            // 　　　　[in]plus  優先度を1つ上げるときtrue、1つ下げるときfalse
            // 戻り値：なし
            //=========================================================================================
            private void OnTaskManagerWaitingPriorityChange(BackgroundTaskManager.TaskChangedEventArgs evt, bool plus) {
                // 対象のインデックスを確認
                if (m_parent.listViewWait.Items.Count <= 1) {
                    return;
                }
                int index = -1;
                for (int i = 0; i < m_parent.listViewWait.Items.Count; i++) {
                    FileTaskDisplayInfo itemInfo = (FileTaskDisplayInfo)(m_parent.listViewWait.Items[i].Tag);
                    if (itemInfo.TaskId == evt.DisplayInfo.TaskId) {
                        index = i;
                    }
                }
                if (index == -1) {
                    return;
                }

                if (plus) {
                    // 1つ高く
                    if (index == 0) {
                        return;
                    }
                    ListViewItem lvItem = m_parent.listViewWait.Items[index];
                    m_parent.listViewWait.Items.RemoveAt(index);
                    m_parent.listViewWait.Items.Insert(index - 1, lvItem);
                } else {
                    // 1つ低く
                    if (index == m_parent.listViewWait.Items.Count - 1) {
                        return;
                    }
                    ListViewItem lvItem = m_parent.listViewWait.Items[index];
                    m_parent.listViewWait.Items.RemoveAt(index);
                    m_parent.listViewWait.Items.Insert(index + 1, lvItem);
                }
            }

            //=========================================================================================
            // 機　能：指定された項目が削除されたときのため、近くの項目を選択状態にする
            // 引　数：[in]listView   対象となるリストビュー
            // 　　　　[in]item       削除予定の項目
            // 戻り値：なし
            //=========================================================================================
            private void SelectNeighbor(ListView listView, ListViewItem item) {
                if (listView.Items.Count <= 1) {
                    return;
                }
                if (listView.Items[listView.Items.Count - 1] == item) {
                    listView.Items[listView.Items.Count - 2].Selected = true;
                } else {
                    bool selectNext = false;
                    foreach (ListViewItem lvItem in listView.Items) {
                        if (selectNext) {
                            lvItem.Selected = true;
                            return;
                        } else if (lvItem == item) {
                            selectNext = true;
                        }
                    }
                }
            }
        }
        
        //=========================================================================================
        // クラス：SSH接続ページ
        //=========================================================================================
        private class SSHConnectionPage {
            // 所有ダイアログ
            private TaskManagerDialog m_parent;

            // SSHファイルシステム（null:サポートしていない）
            private SFTPFileSystem m_sshFileSystem = null;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  所有ダイアログ
            // 戻り値：なし
            //=========================================================================================
            public SSHConnectionPage(TaskManagerDialog parent) {
                m_parent = parent;

                // SSH通信ヘッダを追加
                ColumnHeader columnSsh = new ColumnHeader();
                columnSsh.Text = Resources.DlgTaskMan_ListSSH;
                columnSsh.Width = 200;
                ColumnHeader columnSshConnection = new ColumnHeader();
                columnSshConnection.Text = Resources.DlgTaskMan_ListSSHType;
                columnSshConnection.Width = 100;
                ColumnHeader columnSshInUse = new ColumnHeader();
                columnSshInUse.Text = Resources.DlgTaskMan_ListSSHInUse;
                columnSshInUse.Width = 100;
                m_parent.listViewSSH.Columns.AddRange(new ColumnHeader[] {columnSsh, columnSshConnection, columnSshInUse});

                // SSH通信一覧状態変更
                m_sshFileSystem = Program.Document.FileSystemFactory.SFTPFileSystem;
                if (m_sshFileSystem == null) {
                    // SSHをサポートしていない場合
                    m_parent.buttonSSHDisconnect.Enabled = false;
                    m_parent.listViewSSH.Enabled = false;
                } else {
                    // 通信状態一覧を取得
                    SSHConnectionManager manager = Program.Document.FileSystemFactory.SSHConnectionManager;
                    List<UIConnectionPoolInfo> infoList = manager.UIConnectionPoolList;
                    foreach (UIConnectionPoolInfo info in infoList) {
                        ListViewItem item = CreateSSHListViewItem(info);
                        m_parent.listViewSSH.Items.Add(item);
                    }
                    OnSSHStateChange();
                }

                // イベントを登録
                if (m_sshFileSystem != null) {
                    SSHConnectionManager manager = Program.Document.FileSystemFactory.SSHConnectionManager;
                    manager.ConnectionChanged += new SSHConnectionManager.ConnectionChangedEventHandler(this.ConnectionManager_StateChanged);
                }
                m_parent.FormClosed += new FormClosedEventHandler(TaskManagerDialog_FormClosed);
                m_parent.buttonDisconnectAll.Click += new System.EventHandler(this.buttonDisconnectAll_Click);
                m_parent.buttonSSHDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            }

            //=========================================================================================
            // 機　能：フォームが閉じられたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void TaskManagerDialog_FormClosed(object sender, FormClosedEventArgs evt) {
                if (m_sshFileSystem != null) {
                    SSHConnectionManager manager = Program.Document.FileSystemFactory.SSHConnectionManager;
                    manager.ConnectionChanged -= new SSHConnectionManager.ConnectionChangedEventHandler(this.ConnectionManager_StateChanged);
                }
            }

            //=========================================================================================
            // 機　能：SSH接続一覧のListView項目を作成する
            // 引　数：[in]info  接続の情報
            // 戻り値：ListViewの項目
            //=========================================================================================
            private ListViewItem CreateSSHListViewItem(UIConnectionPoolInfo info) {
                string userServer = SSHUtils.CreateUserServerShort(info.User, info.Server, info.PortNo);
                string count = "SFTP";
                string inUse = (info.AllInUseCount).ToString();
                string[] itemString = {userServer, count, inUse};
                ListViewItem item = new ListViewItem(itemString);
                item.ImageIndex = (int)IconImageListID.Icon_BackgroundTaskSSH;
                item.Tag = info;
                return item;
            }

            //=========================================================================================
            // 機　能：SSHのリストボックスの状態が変わったときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void OnSSHStateChange() {
                if (m_parent.listViewSSH.Items.Count > 0) {
                    m_parent.buttonDisconnectAll.Enabled = true;
                } else {
                    m_parent.buttonDisconnectAll.Enabled = false;
                }
                ListViewItem item = m_parent.listViewSSH.FocusedItem;
                if (item == null) {
                    m_parent.buttonSSHDisconnect.Enabled = false;
                } else {
                    m_parent.buttonSSHDisconnect.Enabled = true;
                }
            }

            //=========================================================================================
            // 機　能：SSHの切断ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonDisconnect_Click(object sender, EventArgs evt) {
                if (m_sshFileSystem == null) {
                    return;
                }
                ListView.SelectedListViewItemCollection itemCollection = m_parent.listViewSSH.SelectedItems;
                if (itemCollection.Count == 0) {
                    return;
                }
                ListViewItem item = itemCollection[0];
                UIConnectionPoolInfo info = (UIConnectionPoolInfo)(item.Tag);
                DisconnectSSHSession(info, m_parent);
            }


            //=========================================================================================
            // 機　能：SSHのすべて切断ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonDisconnectAll_Click(object sender, EventArgs evt) {
                if (m_sshFileSystem == null) {
                    return;
                }

                DisconnectAllSSHSession(m_parent);
            }
            
            //=========================================================================================
            // 機　能：SSHのリストボックスの選択が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void listViewSSH_SelectedIndexChanged(object sender, EventArgs evt) {
                OnSSHStateChange();
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
                    ListViewItem item = CreateSSHListViewItem(evt.PoolInfo);
                    m_parent.listViewSSH.Items.Add(item);
                    OnSSHStateChange();
                } else if (evt.EventType == SSHConnectionManager.ConnectionChangedEventType.ConnectionPoolDeleted) {
                    foreach (ListViewItem item in m_parent.listViewSSH.Items) {
                        UIConnectionPoolInfo info = (UIConnectionPoolInfo)(item.Tag);
                        if (info.Server == evt.Server && info.User == evt.User && info.PortNo == evt.PortNo) {
                            m_parent.listViewSSH.Items.Remove(item);
                            break;
                        }
                    }
                    OnSSHStateChange();
                } else if (evt.EventType == SSHConnectionManager.ConnectionChangedEventType.CountChanged) {
                    int count = m_parent.listViewSSH.Items.Count;
                    for (int i = 0; i < count; i++) {
                        UIConnectionPoolInfo info = (UIConnectionPoolInfo)(m_parent.listViewSSH.Items[i].Tag);
                        if (info.Server == evt.Server && info.User == evt.User && info.PortNo == evt.PortNo) {
                            m_parent.listViewSSH.Items[i] = CreateSSHListViewItem(evt.PoolInfo);
                            break;
                        }
                    }
                    OnSSHStateChange();
                } else if (evt.EventType == SSHConnectionManager.ConnectionChangedEventType.StateReset) {
                    int count = m_parent.listViewSSH.Items.Count;
                    for (int i = count - 1; i >= 0; i--) {
                        m_parent.listViewSSH.Items.RemoveAt(i);
                    }
                    SSHConnectionManager manager = Program.Document.FileSystemFactory.SSHConnectionManager;
                    List<UIConnectionPoolInfo> infoList = manager.UIConnectionPoolList;
                    foreach (UIConnectionPoolInfo info in infoList) {
                        ListViewItem item = CreateSSHListViewItem(info);
                        m_parent.listViewSSH.Items.Add(item);
                    }
                    OnSSHStateChange();
                }
            }
        }
    }
}
