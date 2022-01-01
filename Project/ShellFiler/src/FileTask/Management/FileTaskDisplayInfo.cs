using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.UI;
using ShellFiler.FileTask;

namespace ShellFiler.FileTask.Management {
    
    //=========================================================================================
    // クラス：ファイル操作の表示名のエントリ
    //=========================================================================================
    public class FileTaskDisplayInfo {
        // タスクのID
        private BackgroundTaskID m_taskId;

        // タスクの種類
        private BackgroundTaskType m_operationType;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_pathInfo;

        // 実行中のときtrue
        private bool m_running;

        // タスクの実行状態（進捗表示イベントTaskChangedEventType.ProgressChangedの場合はNone）
        private BackgroundTaskRunningType m_runningType;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]taskId   タスクのID
        // 　　　　[in]type     タスクの種類
        // 　　　　[in]pathInfo 転送中/転送先のパス情報
        // 　　　　[in]running  実行中のときtrue（終了したかどうかの判断）
        // 　　　　[in]runType  タスクの実行状態（進捗表示イベントTaskChangedEventType.ProgressChangedの場合はNone）
        // 戻り値：なし
        //=========================================================================================
        public FileTaskDisplayInfo(BackgroundTaskID taskId, BackgroundTaskType type, BackgroundTaskPathInfo pathInfo, bool running, BackgroundTaskRunningType runType) {
            m_taskId = taskId;
            m_operationType = type;
            m_pathInfo = pathInfo;
            m_running = running;
            m_runningType = runType;
        }

        //=========================================================================================
        // プロパティ：タスクのID
        //=========================================================================================
        public BackgroundTaskID TaskId {
            get {
                return m_taskId;
            }
        }

        //=========================================================================================
        // プロパティ：タスクの種類
        //=========================================================================================
        public BackgroundTaskType BackgroundTaskType {
            get {
                return m_operationType;
            }
        }

        //=========================================================================================
        // プロパティ：転送元/転送先の情報
        //=========================================================================================
        public BackgroundTaskPathInfo PathInfo {
            get {
                return m_pathInfo;
            }
        }

        //=========================================================================================
        // プロパティ：実行中のときtrue
        //=========================================================================================
        public bool Running {
            get {
                return m_running;
            }
            set {
                m_running = value;
            }
        }


        //=========================================================================================
        // プロパティ：タスクの実行状態（進捗表示イベントTaskChangedEventType.ProgressChangedの場合はNone）
        //=========================================================================================
        public BackgroundTaskRunningType RunningType {
            get {
                return m_runningType;
            }
        }
    }
}
