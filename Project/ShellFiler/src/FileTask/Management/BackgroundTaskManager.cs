using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.Util;
using ShellFiler.FileTask;

namespace ShellFiler.FileTask.Management {

    //=========================================================================================
    // クラス：バックグラウンドタスクの管理クラス
    // 待機可能なバックグラウンドタスク（4個まで同時実行可能）
    // ・CopyBackgroundTask
    // ・CreateShortcutBackgroundTask
    // ・DeleteBackgroundTask
    // ・LocalArchiveBackgroundTask
    // ・MoveBackgroundTask
    // など
    //
    // 待機不可能なバックグラウンドタスク（10個まで同時実行可能）
    // ・LocalExecuteBackgroundTask
    // ・LocalUploadBackgroundTask
    // ・HttpResponseViewerBackgroundTask
    // ・RetrieveFileBackgroundTask
    // ・ShellExecuteBackgroundTask
    // など
    //
    // 多重実行可能なバックグラウンドタスク
    // ・GraphicsViewerBackgroundTask
    //=========================================================================================
    public class BackgroundTaskManager {
        // 記憶しているエラー情報の最大数
        public const int MAX_ERROR_INFO_TASK_COUNT = 10;

        // 待機可能なバックグラウンドタスク
        private List<IBackgroundTask> m_waitableBackgroundTaskList = new List<IBackgroundTask>();

        // 待機中のバックグラウンドタスク
        private List<IBackgroundTask> m_waitingBackgroundTaskList = new List<IBackgroundTask>();

        // 待機不可能なバックグラウンドタスク
        private List<IBackgroundTask> m_limitedBackgroundtaskList = new List<IBackgroundTask>();

        // 多重実行可能なバックグラウンドタスク
        private List<IBackgroundTask> m_unlimitedBackgroundTaskList = new List<IBackgroundTask>();

        // タスクIDからタスクへのMap
        private Dictionary<BackgroundTaskID, IBackgroundTask> m_mapIdToTask = new Dictionary<BackgroundTaskID, IBackgroundTask>();

        // グラフィックビューアのタスク管理
        private GraphicsViewerTaskManager m_graphicsViewerTaskManager;

        // エラー情報
        private List<FileErrorInfo> m_fileErrorInfo = new List<FileErrorInfo>();

        // タスクに変化が生じたときの通知用delegate
        public delegate void TaskChangedEventHandler(object sender, TaskChangedEventArgs evt); 

        // タスクに変化が生じたときに通知するイベント
        public event TaskChangedEventHandler TaskChanged; 

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public BackgroundTaskManager() {
            m_graphicsViewerTaskManager = new GraphicsViewerTaskManager(this);
        }
        
        //=========================================================================================
        // 機　能：ファイル操作のタスクを開始する
        // 引　数：[in]task       登録するタスク
        // 　　　　[in]isRunning  実行中のタスクを作成するときtrue
        // 戻り値：なし
        // メ　モ：ユーザインターフェーススレッドで実行される
        //=========================================================================================
        public void StartFileTask(IBackgroundTask task, bool isRunning) {
            // 登録
            bool suspend = false;
            BackgroundTaskRunningType taskType = GetRunningType(task.GetType(), isRunning);
            switch (taskType) {
                case BackgroundTaskRunningType.Waitable:
                    m_waitableBackgroundTaskList.Add(task);
                    break;
                case BackgroundTaskRunningType.Waiting:
                    m_waitingBackgroundTaskList.Add(task);
                    suspend = true;
                    break;
                case BackgroundTaskRunningType.WaitingOver:
                    Program.Abort("タスク管理に問題があります。count={0}", m_waitableBackgroundTaskList.Count);
                    break;
                case BackgroundTaskRunningType.Unlimited:
                    m_limitedBackgroundtaskList.Add(task);
                    break;
                case BackgroundTaskRunningType.LimitedOver:
                    Program.Abort("タスク管理に問題があります。count={0}", m_limitedBackgroundtaskList.Count);
                    break;
                case BackgroundTaskRunningType.Limited:
                    m_unlimitedBackgroundTaskList.Add(task);
                    break;
            }
            m_mapIdToTask.Add(task.TaskId, task);

            // ファイルエラーを登録
            if (task is AbstractFileBackgroundTask) {
                AbstractFileBackgroundTask fileTask = (AbstractFileBackgroundTask)task;
                if (fileTask.FileErrorInfo != null) {
                    m_fileErrorInfo.Add(fileTask.FileErrorInfo);
                    if (m_fileErrorInfo.Count > MAX_ERROR_INFO_TASK_COUNT) {
                        m_fileErrorInfo.RemoveAt(0);
                    }
                }
            }

            // タスクを開始
            task.StartTask(suspend);

            // タスク開始を通知
            if (TaskChanged != null) {
                BackgroundTaskType type = task.BackgroundTaskType;
                BackgroundTaskPathInfo pathInfo = task.PathInfo;
                FileTaskDisplayInfo info = new FileTaskDisplayInfo(task.TaskId, type, pathInfo, true, taskType);
                TaskChangedEventArgs arg = new TaskChangedEventArgs(TaskChangedEventType.TaskAdded, info);
                TaskChanged(this, arg);
            }
        }
        
        //=========================================================================================
        // 機　能：タスク終了の通知をバックグラウンドタスクから受け取ったときの処理を行う
        // 引　数：[in]taskId   受け取ったタスクのID
        // 戻り値：なし
        // メ　モ：delegateによりユーザインターフェーススレッドで実行される
        //=========================================================================================
        public void OnNotifyFileTaskEnd(BackgroundTaskID taskId) {
            // スレッドを終了
            IBackgroundTask task;
            BackgroundTaskRunningType taskRun;
            TerminateAndJoin(taskId, out task, out taskRun);
            m_mapIdToTask.Remove(taskId);
            if (task == null) {
                return;
            }

            // グラフィックビューアなら通知
            if (task is GraphicsViewerBackgroundTask) {
                m_graphicsViewerTaskManager.OnTerminateTask();
            }

            // エラーダイアログを開く
            if (task is AbstractFileBackgroundTask) {
                AbstractFileBackgroundTask fileTask = (AbstractFileBackgroundTask)task;
                if (task != null && fileTask.FileErrorInfo != null) {
                    if (fileTask.FileErrorInfo.ErrorInfoCount > 0) {
                        Program.MainWindow.ShowFileErrorDialog(fileTask.FileErrorInfo);
                    }
                } else if (task != null && fileTask.FailCount > 0) {
                    InfoBox.Information(Program.MainWindow, Resources.Msg_BackgroundTaskFailed, task.BackgroundTaskType.DisplayName);
                }
            }

            // タスク終了を通知
            if (TaskChanged != null) {
                // タイミングの関係で停止中のタスクが終了したときは一度Resumeしてから終了を通知する
                if (taskRun == BackgroundTaskRunningType.Waiting) {
                    FileTaskDisplayInfo infoResume = new FileTaskDisplayInfo(task.TaskId, task.BackgroundTaskType, task.PathInfo, false, BackgroundTaskRunningType.Waitable);
                    TaskChangedEventArgs argResume = new TaskChangedEventArgs(TaskChangedEventType.TaskResumed, infoResume);
                    TaskChanged(this, argResume);
                }
                FileTaskDisplayInfo info = new FileTaskDisplayInfo(task.TaskId, task.BackgroundTaskType, task.PathInfo, false, BackgroundTaskRunningType.None);
                TaskChangedEventArgs arg = new TaskChangedEventArgs(TaskChangedEventType.TaskDeleted, info);
                TaskChanged(this, arg);
            }

            // 待機中タスクを再開
            if (taskRun == BackgroundTaskRunningType.Waitable && m_waitingBackgroundTaskList.Count > 0) {
                IBackgroundTask resumeTask = m_waitingBackgroundTaskList[0];
                SetSuspendState(resumeTask, false);
            }
        }

        //=========================================================================================
        // 機　能：タスクの一時停止状態を変更する
        // 引　数：[in]task       対象のタスク
        // 　　　　[in]isSuspend  suspend状態にするときtrue、resumeするときfalse
        // 戻り値：なし
        //=========================================================================================
        public void SetSuspendState(IBackgroundTask task, bool isSuspend) {
            if (isSuspend) {
                // 一時停止
                if (!m_waitableBackgroundTaskList.Contains(task)) {
                    return;
                }
                m_waitableBackgroundTaskList.Remove(task);
                m_waitingBackgroundTaskList.Add(task);
                task.SetSuspendState(true);
            } else {
                // 再開
                if (!m_waitingBackgroundTaskList.Contains(task)) {
                    return;
                }
                m_waitingBackgroundTaskList.Remove(task);
                m_waitableBackgroundTaskList.Add(task);
                task.SetSuspendState(false);
            }

            // 通知
            if (TaskChanged != null) {
                TaskChangedEventType eventType;
                if (isSuspend) {
                    eventType = TaskChangedEventType.TaskSuspended;
                } else {
                    eventType = TaskChangedEventType.TaskResumed;
                }
                FileTaskDisplayInfo info = new FileTaskDisplayInfo(task.TaskId, task.BackgroundTaskType, task.PathInfo, false, BackgroundTaskRunningType.None);
                TaskChangedEventArgs arg = new TaskChangedEventArgs(eventType, info);
                TaskChanged(this, arg);
            }
        }

        //=========================================================================================
        // 機　能：待機中タスクの優先度を変更する
        // 引　数：[in]taskId   変更対象のタスクのID
        // 　　　　[in]plus     １つ優先度を上げるときtrue、１つ優先度を下げるときfalse
        // 戻り値：なし
        //=========================================================================================
        public void ChangeWaitingPriority(BackgroundTaskID taskId, bool plus) {
            // 変更対象を決定
            if (m_waitingBackgroundTaskList.Count == 1) {
                return;
            }
            int index = -1;
            for (int i = 0; i < m_waitingBackgroundTaskList.Count; i++) {
                if (taskId == m_waitingBackgroundTaskList[i].TaskId) {
                    index = i;
                    break;
                }
            }
            if (index == -1) {
                return;
            }

            TaskChangedEventType eventType;
            IBackgroundTask task = m_waitingBackgroundTaskList[index];
            FileTaskDisplayInfo info = new FileTaskDisplayInfo(task.TaskId, task.BackgroundTaskType, task.PathInfo, true, BackgroundTaskRunningType.Waiting);
            if (plus) {
                // 優先度をより高く（リストの前へ）
                if (index == 0) {
                    return;
                }
                IBackgroundTask temp = m_waitingBackgroundTaskList[index - 1];
                m_waitingBackgroundTaskList[index - 1] = m_waitingBackgroundTaskList[index];
                m_waitingBackgroundTaskList[index] = temp;
                eventType = TaskChangedEventType.WaitingPriorityPlus;
            } else {
                // 優先度をより低く（リストの後へ）
                if (index == m_waitingBackgroundTaskList.Count - 1) {
                    return;
                }
                IBackgroundTask temp = m_waitingBackgroundTaskList[index + 1];
                m_waitingBackgroundTaskList[index + 1] = m_waitingBackgroundTaskList[index];
                m_waitingBackgroundTaskList[index] = temp;
                eventType = TaskChangedEventType.WaitingPriorityMinus;
            }

            // 変更を通知
            if (TaskChanged != null) {
                TaskChangedEventArgs arg = new TaskChangedEventArgs(eventType, info);
                TaskChanged(this, arg);
            }
        }

        //=========================================================================================
        // 機　能：タスクを終了する
        // 引　数：[in]taskId   受け取ったタスクのID
        // 　　　　[out]task    終了したタスク
        // 　　　　[out]taskRun 終了したタスクの実行状態
        // 戻り値：なし
        //=========================================================================================
        private void TerminateAndJoin(BackgroundTaskID taskId, out IBackgroundTask task, out BackgroundTaskRunningType taskRun) {
            task = null;
            taskRun = BackgroundTaskRunningType.LimitedOver;
            for (int i=0; i<m_waitableBackgroundTaskList.Count; i++) {
                task = m_waitableBackgroundTaskList[i];
                if (task.TaskId == taskId) {
                    task.JoinThread();
                    m_waitableBackgroundTaskList.RemoveAt(i);
                    taskRun = BackgroundTaskRunningType.Waitable;
                    return;
                }
            }

            for (int i=0; i<m_waitingBackgroundTaskList.Count; i++) {
                task = m_waitingBackgroundTaskList[i];
                if (task.TaskId == taskId) {
                    task.JoinThread();
                    m_waitingBackgroundTaskList.RemoveAt(i);
                    taskRun = BackgroundTaskRunningType.Waiting;
                    return;
                }
            }

            for (int i=0; i<m_limitedBackgroundtaskList.Count; i++) {
                task = m_limitedBackgroundtaskList[i];
                if (task.TaskId == taskId) {
                    task.JoinThread();
                    m_limitedBackgroundtaskList.RemoveAt(i);
                    taskRun = BackgroundTaskRunningType.Limited;
                }
            }

            for (int i=0; i<m_unlimitedBackgroundTaskList.Count; i++) {
                task = m_unlimitedBackgroundTaskList[i];
                if (task.TaskId == taskId) {
                    task.JoinThread();
                    m_unlimitedBackgroundTaskList.RemoveAt(i);
                    taskRun = BackgroundTaskRunningType.Unlimited;
                }
            }
        }

        //=========================================================================================
        // 機　能：タスクIDに対応するタスクを取得する
        // 引　数：[in]taskId   タスクのID
        // 戻り値：タスク（対応するものがない場合はnull）
        //=========================================================================================
        public IBackgroundTask GetTaskFromTaskId(BackgroundTaskID taskId) {
            if (m_mapIdToTask.ContainsKey(taskId)) {
                return m_mapIdToTask[taskId];
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：進捗状態に更新があったときのイベントを発行する
        // 引　数：[in]taskId   更新があったタスクのID
        // 戻り値：なし
        // メ　モ：delegateによりユーザインターフェーススレッドで実行される
        //=========================================================================================
        public void FireBackgroundtaskPathInfoProgressEvent(BackgroundTaskID taskId) {
            BaseThread.InvokeProcedureByMainThread(new FireBackgroundtaskPathInfoProgressEventDelegate(FireBackgroundtaskPathInfoProgressEventUI), taskId);
        }
        delegate void FireBackgroundtaskPathInfoProgressEventDelegate(BackgroundTaskID taskId);
        private void FireBackgroundtaskPathInfoProgressEventUI(BackgroundTaskID taskId) {
            // スレッドを検索
            IBackgroundTask task = GetTaskFromTaskId(taskId);
            if (task == null) {
                return;
            }

            // イベントを発行
            if (TaskChanged != null && task != null) {
                FileTaskDisplayInfo info = new FileTaskDisplayInfo(task.TaskId, task.BackgroundTaskType, task.PathInfo, false, BackgroundTaskRunningType.None);
                TaskChangedEventArgs arg = new TaskChangedEventArgs(TaskChangedEventType.ProgressChanged, info);
                TaskChanged(this, arg);
            }
        }

        //=========================================================================================
        // 機　能：ファイル操作をキャンセルする
        // 引　数：[in]taskId   キャンセルするタスクのID
        // 　　　　[in]waitJoin バックグラウンドタスクの終了を待つときtrue
        // 戻り値：なし
        // メ　モ：ユーザインターフェーススレッドで実行される
        //=========================================================================================
        public void CancelBackgroundTask(BackgroundTaskID taskId, bool waitJoin) {
            IBackgroundTask task = GetTaskFromTaskId(taskId);
            if (task == null) {
                return;
            }
            task.SetCancel(CancelReason.User);
            if (waitJoin) {
                task.JoinThread();
            }
        }
                
        //=========================================================================================
        // 機　能：仮想ディレクトリの結果をアップロード中かどうか調べる
        // 引　数：なし
        // 戻り値：アップロード中のときtrue
        //=========================================================================================
        public bool IsVirtualUploading() {
            foreach (IBackgroundTask task in m_mapIdToTask.Values) {
                if (task is LocalUploadBackgroundTask) {
                    return true;
                }
            }
            return false;
        }

        //=========================================================================================
        // 機　能：バックグラウンド処理で発生した詳細エラー情報を取得する
        // 引　数：[in]taskId  バックグラウンドタスクのID
        // 戻り値：詳細エラー情報（エラー情報が削除済みのときnull）
        //=========================================================================================
        public FileErrorInfo GetBackgroundDetailErrorInfo(BackgroundTaskID taskId) {
            foreach (FileErrorInfo errorInfo in m_fileErrorInfo) {
                if (errorInfo.TaskId == taskId) {
                    return errorInfo;
                }
            }
            return null;
        }

        //=========================================================================================
        // 機　能：バックグラウンド処理で発生した詳細エラー情報をクリアする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearBackgroundDetailErrorInfo() {
            m_fileErrorInfo.Clear();
        }

        //=========================================================================================
        // 機　能：実行中のタスクの状態を返す
        // 引　数：[in]task  調べるタスク
        // 戻り値：タスクの状態（該当するものがないときLimitedOver）
        //=========================================================================================
        public BackgroundTaskRunningType GetRunningTypeFromActiveTask(IBackgroundTask task) {
            if (m_waitableBackgroundTaskList.Contains(task)) {
                return BackgroundTaskRunningType.Waitable;
            } else if (m_waitingBackgroundTaskList.Contains(task)) {
                return BackgroundTaskRunningType.Waiting;
            } else if (m_limitedBackgroundtaskList.Contains(task)) {
                return BackgroundTaskRunningType.Limited;
            } else if (m_unlimitedBackgroundTaskList.Contains(task)) {
                return BackgroundTaskRunningType.Unlimited;
            } else {
                return BackgroundTaskRunningType.LimitedOver;
            }
        }

        //=========================================================================================
        // 機　能：バックグラウンドタスクの種類を返す
        // 引　数：[in]task       調べるタスク
        // 　　　　[in]isRunning  実行中のタスクを作成するときtrue
        // 戻り値：タスクの種類
        //=========================================================================================
        public BackgroundTaskRunningType GetRunningType(Type task, bool isRunning) {
            if (task == typeof(CopyBackgroundTask) ||
                    task == typeof(CreateShortcutBackgroundTask) ||
                    task == typeof(DeleteBackgroundTask) ||
                    task == typeof(LocalArchiveBackgroundTask) ||
                    task == typeof(MoveBackgroundTask) ||
                    task == typeof(MirrorCopyBackgroundTask) ||
                    task == typeof(RenameSelectedFileInfoBackgroundTask) ||
                    task == typeof(RetrieveFolderSizeBackgroundTask) ||
                    task == typeof(DuplicateFileBackgroundTask) ||
                    task == typeof(CombineFileBackgroundTask) ||
                    task == typeof(SplitFileBackgroundTask) ||
                    task == typeof(GitAddBackgroundTask) ||
                    task == typeof(GitMoveBackgroundTask) ||
                    task == typeof(GitRenameBackgroundTask)) {
                // 待機可能
                if (isRunning) {
                    if (m_waitableBackgroundTaskList.Count < Configuration.MaxBackgroundTaskWaitableCount) {
                        return BackgroundTaskRunningType.Waitable;
                    } else {
                        return BackgroundTaskRunningType.Waiting;
                    }
                } else {
                    if (m_waitableBackgroundTaskList.Count == 0) {
                        return BackgroundTaskRunningType.Waitable;
                    } else {
                        return BackgroundTaskRunningType.Waiting;
                    }
                }
            } else if (task == typeof(LocalExecuteBackgroundTask) ||
                    task == typeof(LocalUploadBackgroundTask) ||
                    task == typeof(HttpResponseViewerBackgroundTask) ||
                    task == typeof(RetrieveFileBackgroundTask) ||
                    task == typeof(ShellExecuteBackgroundTask) ||
                    task == typeof(MakeDirectoryBackgroundTask) ||
                    task == typeof(RenameBackgroundTask)) {
                // 待機不可能
                if (m_limitedBackgroundtaskList.Count < Configuration.MaxBackgroundTaskLimitedCount) {
                    return BackgroundTaskRunningType.Limited;
                } else {
                    return BackgroundTaskRunningType.LimitedOver;
                }
            } else if (task == typeof(GraphicsViewerBackgroundTask)) {
                // 多重実行可能
                return BackgroundTaskRunningType.Unlimited;
            } else {
                // 不明
                Program.Abort("不明なタスクタイプです。", task.GetType().FullName);
                return BackgroundTaskRunningType.Limited;
            }
        }
        
        //=========================================================================================
        // 機　能：表示用情報を取得する
        // 引　数：[out]runningList  実行中のタスク一覧
        // 　　　　[out]waitingList  待機中のタスク一覧
        // 戻り値：なし
        //=========================================================================================
        public void GetDisplayInfoList(out List<FileTaskDisplayInfo> runningList, out List<FileTaskDisplayInfo> waitingList) {
            // 実行中
            runningList = new List<FileTaskDisplayInfo>();
            foreach (IBackgroundTask task in m_waitableBackgroundTaskList) {
                BackgroundTaskType type = task.BackgroundTaskType;
                BackgroundTaskPathInfo pathInfo = task.PathInfo;
                runningList.Add(new FileTaskDisplayInfo(task.TaskId, type, pathInfo, true, BackgroundTaskRunningType.Waitable));
            }
            foreach (IBackgroundTask task in m_limitedBackgroundtaskList) {
                BackgroundTaskType type = task.BackgroundTaskType;
                BackgroundTaskPathInfo pathInfo = task.PathInfo;
                runningList.Add(new FileTaskDisplayInfo(task.TaskId, type, pathInfo, true, BackgroundTaskRunningType.Limited));
            }
            foreach (IBackgroundTask task in m_unlimitedBackgroundTaskList) {
                BackgroundTaskType type = task.BackgroundTaskType;
                BackgroundTaskPathInfo pathInfo = task.PathInfo;
                runningList.Add(new FileTaskDisplayInfo(task.TaskId, type, pathInfo, true, BackgroundTaskRunningType.Unlimited));
            }

            // 待機中
            waitingList = new List<FileTaskDisplayInfo>();
            foreach (IBackgroundTask task in m_waitingBackgroundTaskList) {
                BackgroundTaskType type = task.BackgroundTaskType;
                BackgroundTaskPathInfo pathInfo = task.PathInfo;
                waitingList.Add(new FileTaskDisplayInfo(task.TaskId, type, pathInfo, true, BackgroundTaskRunningType.Waiting));
            }
        }

        //=========================================================================================
        // プロパティ：タスクの数
        //=========================================================================================
        public int Count {
            get {
                return m_mapIdToTask.Count;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアのタスク管理
        //=========================================================================================
        public GraphicsViewerTaskManager GraphicsViewerTaskManager {
            get {
                return m_graphicsViewerTaskManager;
            }
        }

        //=========================================================================================
        // クラス：タスク状態変化のイベント引数
        //=========================================================================================
        public class TaskChangedEventArgs {
            // イベントの種類
            private TaskChangedEventType m_eventType;
            
            // 表示名の情報
            private FileTaskDisplayInfo m_displayInfo;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]eventType    イベントの種類
            // 　　　　[in]displayInfo  表示名の情報
            // 戻り値：なし
            //=========================================================================================
            public TaskChangedEventArgs(TaskChangedEventType eventType, FileTaskDisplayInfo displayInfo) {
                m_eventType = eventType;
                m_displayInfo = displayInfo;
            }

            //=========================================================================================
            // プロパティ：イベントの種類
            //=========================================================================================
            public TaskChangedEventType EventType {
                get {
                    return m_eventType;
                }
            }

            //=========================================================================================
            // プロパティ：表示名の情報
            //=========================================================================================
            public FileTaskDisplayInfo DisplayInfo {
                get {
                    return m_displayInfo;
                }
            }
        }

        //=========================================================================================
        // 列挙子：タスク状態変化のイベント引数
        //=========================================================================================
        public enum TaskChangedEventType {
            TaskAdded,                          // タスクが追加された
            TaskDeleted,                        // タスクが削除された
            TaskResumed,                        // タスクが再開された
            TaskSuspended,                      // タスクが中断された
            ProgressChanged,                    // 進捗状態が変わった
            WaitingPriorityPlus,                // 待機中タスクの優先度が上がった
            WaitingPriorityMinus,               // 待機中タスクの優先度が下がった
        }
    }
}
