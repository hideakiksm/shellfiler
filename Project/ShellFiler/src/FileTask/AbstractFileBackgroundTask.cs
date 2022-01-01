using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Management;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイル操作のバックグラウンドタスクとなる基底クラス
    //=========================================================================================
    public abstract class AbstractFileBackgroundTask : BaseThread, IBackgroundTask, ITaskLogger {
        // 転送元ファイル一覧
        private IFileProviderSrc m_fileProviderSrc;

        // 転送元ファイル一覧（初期状態）
        private IFileProviderSrc m_fileProviderSrcOrg;

        // 転送先ファイル一覧
        private IFileProviderDest m_fileProviderDest;

        // 転送先ファイル一覧（初期状態）
        private IFileProviderDest m_fileProviderDestOrg;

        // 作業完了時のUI更新方法
        private RefreshUITarget m_refreshUiTarget;

        // 転送用ファイルシステム
        private IFileSystemToFileSystem m_fileSystemToFileSystem;

        // コンテキスト情報
        private FileOperationRequestContext m_requestContext;

        // エラー発生時の情報（エラー情報を扱っていないときnull）
        private FileErrorInfo m_fileErrorInfo = null;

        // このタスクのID
        private BackgroundTaskID m_taskId;

        // キャンセル状態
        private CancelFlag m_cancelFlag = new CancelFlag();

        // Suspend状態（スレッド開始まではnull）
        private SuspendFlag m_suspendFlag = null;

        // 成功したファイル操作の数
        private int m_successCount;

        // スキップしたファイル操作の数
        private int m_skipCount;

        // 失敗したファイル操作の数
        private int m_failCount;

        // 現在処理中のファイルに対するログ内容
        private FileLogLineManager m_currentLogLineFile = new FileLogLineManager();

        // 現在処理中のファイルに対する再試行用のマークファイル処理開始の情報
        private SimpleFileDirectoryPath m_currentLogRetryMarkPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider  転送元ファイルの情報
        // 　　　　[in]destProvider 転送先ファイルの情報
        // 　　　　[in]refreshUi    作業完了時のUI更新方法
        // 戻り値：なし
        //=========================================================================================
        public AbstractFileBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi) : base("FileBackgroundTask", 1) {
            m_fileProviderSrc = srcProvider;
            m_fileProviderSrcOrg = srcProvider;
            m_fileProviderDest = destProvider;
            m_fileProviderDestOrg = destProvider;
            m_refreshUiTarget = refreshUi;
            m_fileSystemToFileSystem = Program.Document.FileSystemFactory.CreateTransferFileSystem(srcProvider.SrcFileSystem.FileSystemId, destProvider.DestFileSystem.FileSystemId, false);
            m_taskId = BackgroundTaskID.NextId();
            m_requestContext = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(m_taskId, m_fileProviderSrc.SrcFileSystem.FileSystemId, m_fileProviderDest.DestFileSystem.FileSystemId, m_fileProviderSrc.FileListContext, m_fileProviderDest.FileListContext, null);
            m_successCount = 0;
            m_skipCount = 0;
            m_failCount = 0;
        }

        //=========================================================================================
        // 機　能：タスクを開始する
        // 引　数：[in]suspend  Suspend状態で起動するときtrue
        // 戻り値：なし
        //=========================================================================================
        public void StartTask(bool suspend) {
            m_suspendFlag = new SuspendFlag(suspend);
            StartThread();          // スレッド開始
        }

        //=========================================================================================
        // 機　能：処理を中断または再開する
        // 引　数：[in]suspend  Suspend状態にするときtrue
        // 戻り値：なし
        //=========================================================================================
        public void SetSuspendState(bool suspend) {
            m_suspendFlag.SetSuspend(suspend);
        }

        //=========================================================================================
        // 機　能：FileProviderを再設定する
        // 引　数：[in]srcProvider  転送元ファイルの情報
        // 　　　　[in]destProvider 転送先ファイルの情報
        // 戻り値：なし
        //=========================================================================================
        public void ResetFileProvider(IFileProviderSrc srcProvider, IFileProviderDest destProvider) {
            m_fileProviderSrc = srcProvider;
            m_fileProviderDest = destProvider;
            m_fileSystemToFileSystem = Program.Document.FileSystemFactory.CreateTransferFileSystem(srcProvider.SrcFileSystem.FileSystemId, destProvider.DestFileSystem.FileSystemId, false);
            m_requestContext.ResetFileProvider(srcProvider, destProvider);
        }

        //=========================================================================================
        // 機　能：ファイル操作時のエラー情報を有効にする
        // 引　数：[in]option   オプション（ショートカットの作成のときnull）
        // 戻り値：なし
        //=========================================================================================
        protected void ActivateFileErrorInfo(CopyMoveDeleteOption option) {
            FileSystemID srcId = m_fileProviderSrcOrg.SrcFileSystem.FileSystemId;
            FileSystemID destId = m_fileProviderDestOrg.DestFileSystem.FileSystemId;
            string destDir = m_fileProviderDestOrg.DestDirectoryName;
            m_fileErrorInfo = new FileErrorInfo(m_taskId, BackgroundTaskType, srcId, destId, destDir, option);
        }

        //=========================================================================================
        // 機　能：スレッドの入り口
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンドスレッドで実行する
        //=========================================================================================
        protected override void ThreadEntry() {
            try {
                CheckSuspend();
                ExecuteTask();
            } catch (Exception e) {
                string taskName = "???";
                try {
                    taskName = BackgroundTaskType.DisplayName;
                } catch (Exception) {
                }
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionTask, taskName);
            }
            // バックグラウンドタスク稼働時はメインウィンドウを閉じる操作を禁止しているため、
            // 呼び出しは成功するはず。
            // BaseThread.InvokeProcedureByMainThreadはJoinThreadと競合するため、ここでは使用できない。
            Program.MainWindow.BeginInvoke(new OnNotifyFileTaskEndDelegate(OnNotifyFileTaskEndUI), m_taskId);
        }
        private delegate void OnNotifyFileTaskEndDelegate(BackgroundTaskID taskId);
        private static void OnNotifyFileTaskEndUI(BackgroundTaskID taskId) {
            try {
                Program.Document.BackgroundTaskManager.OnNotifyFileTaskEnd(taskId);
            } catch (Exception e) {
                Program.Abort("UIスレッドでの処理OnNotifyFileTaskEndUIの途中でエラーが発生しました。\r\n" + e.ToString());
            }
        }

        //=========================================================================================
        // 機　能：タスクを実行する
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンドスレッドで実行する
        //=========================================================================================
        abstract protected void ExecuteTask();
        
        //=========================================================================================
        // 機　能：Suspend状態にあるとき待機する
        // 引　数：なし
        // 戻り値：処理を中断するときtrue
        //=========================================================================================
        public bool CheckSuspend() {
            if (IsCancel) {
                return false;
            }
            bool resume = m_suspendFlag.WaitWhileSuspend(m_cancelFlag.Event);
            return !resume;
        }
        
        //=========================================================================================
        // 機　能：進捗状態に更新があったときのイベントを発行する
        // 引　数：[in]taskId   更新があったタスクのID
        // 戻り値：なし
        // メ　モ：delegateによりユーザインターフェーススレッドで実行される
        //=========================================================================================
        protected void FireBackgroundtaskPathInfoProgressEvent() {
            Program.Document.BackgroundTaskManager.FireBackgroundtaskPathInfoProgressEvent(m_taskId);
        }

        //=========================================================================================
        // 機　能：再試行用のマークファイル処理開始の情報を設定する
        // 引　数：[in]pathInfo   マークファイルのパス情報
        // 戻り値：なし
        // メ　モ：ActivateFileErrorInfo()を実行していないタスクでは、呼び出す必要がない
        //=========================================================================================
        public void LogFileOperationSetMarkInfo(SimpleFileDirectoryPath pathInfo) {
            m_currentLogRetryMarkPathInfo = pathInfo;
        }
                
        //=========================================================================================
        // 機　能：ファイル操作開始時のログを出力する
        // 引　数：[in]operationType  ファイル操作の種類
        // 　　　：[in]filePath       ファイルパス
        // 　　　：[in]isDir          ファイルのときtrue、ディレクトリのときfalse
        // 戻り値：なし
        //=========================================================================================
        public void LogFileOperationStart(FileOperationType operationType, string filePath, bool isDir) {
            // ファイルの場合はファイル名だけ出力
            if (!isDir) {
                filePath = GenericFileStringUtils.GetFileName(filePath);
            } else {
                filePath = GenericFileStringUtils.RemoveLastDirectorySeparator(filePath);
            }
            m_currentLogLineFile.LogFileOperationStart(operationType, filePath);
        }

        //=========================================================================================
        // 機　能：ファイル操作終了時のログを出力する
        // 引　数：[in]status     出力するステータス
        // 戻り値：渡されたstatus
        //=========================================================================================
        public FileOperationStatus LogFileOperationEnd(FileOperationStatus status) {
            return LogFileOperationEnd(status, null);
        }

        //=========================================================================================
        // 機　能：ファイル操作終了時のログを出力する
        // 引　数：[in]status     出力するステータス
        // 　　　　[in]retryInfo  再試行情報(再試行できない場合、Failのステータス以外ではnull)
        // 戻り値：渡されたstatus
        //=========================================================================================
        public FileOperationStatus LogFileOperationEnd(FileOperationStatus status, IRetryInfo retryInfo) {
            // ログに追加
            m_currentLogLineFile.LogFileOperationEnd(status);

            // 件数をカウント
            switch (status.Type) {
                case FileOperationStatus.StatusType.Success:
                    SuccessCount++;
                    break;
                case FileOperationStatus.StatusType.Skip:
                    SkipCount++;
                    break;
                case FileOperationStatus.StatusType.Fail:
                    if (m_fileErrorInfo != null) {
                        FileErrorInfoItem errorItem = new FileErrorInfoItem(m_currentLogLineFile.OperationType, status, retryInfo);
                        errorItem.SrcMarkObjectPath = m_currentLogRetryMarkPathInfo;
                        if (errorItem.RetryInfo != null) {
                            errorItem.RetryInfo.SrcMarkObjectPath = m_currentLogRetryMarkPathInfo;
                        }
                        m_fileErrorInfo.AddFileErrorInfo(errorItem);
                    }
                    FailCount++;
                    break;
            }

            // 特定のエラーでタスク停止
            if (status == FileOperationStatus.Canceled) {
                if (!(m_cancelFlag.IsCancel)) {
                    SetCancel(CancelReason.User);
                }
            } else if (status == FileOperationStatus.DiskFull) {
                SetCancel(CancelReason.DiskFull);
            } else if (status == FileOperationStatus.FailedConnect) {
                SetCancel(CancelReason.Connection);
            }
            return status;
        }

        //=========================================================================================
        // 機　能：ファイル操作終了時のログのみを出力する
        // 引　数：[in]status  出力するステータス
        // 戻り値：なし
        //=========================================================================================
        public void LogFileOperationStateChange(FileOperationStatus status) {
            m_currentLogLineFile.LogFileOperationStateChange(status);
        }

        //=========================================================================================
        // 機　能：処理完了時のログを出力する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        protected void DisplayCompletedLog() {
            LogLine log = null;
            if (m_cancelFlag.CancelReason == CancelReason.User) {
                // キャンセル
                log = new LogLineSimple(Resources.Log_FinalCanceled, SuccessCount, SkipCount, FailCount);
            } else if (m_cancelFlag.CancelReason == CancelReason.DiskFull) {
                // ディスクフルのため中止
                log = new LogLineSimple(LogColor.Error, Resources.Log_FinalDiskFull, SuccessCount, SkipCount, FailCount);
            } else if (m_cancelFlag.CancelReason == CancelReason.Connection) {
                // 接続エラーのため中止
                log = new LogLineSimple(LogColor.Error, Resources.Log_FinalConnectionError, SuccessCount, SkipCount, FailCount);
            } else if (this is CopyBackgroundTask) {
                // コピー完了
                if (FailCount > 0) {
                    log = new LogLineTaskError(m_taskId, Resources.Log_FinalCopyFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalCopySuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is MoveBackgroundTask) {
                // 移動完了
                if (FailCount > 0) {
                    log = new LogLineTaskError(m_taskId, Resources.Log_FinalMoveFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalMoveSuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is CreateShortcutBackgroundTask) {
                // ショートカット作成完了
                if (FailCount > 0) {
                    log = new LogLineTaskError(m_taskId, Resources.Log_FinalShortcutFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalShortcutSuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is MirrorCopyBackgroundTask) {
                // ミラーコピー完了
                if (FailCount > 0) {
                    log = new LogLineTaskError(m_taskId, Resources.Log_FinalMirrorCopyFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalMirrorCopySuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is DeleteBackgroundTask) {
                // 削除完了
                if (FailCount > 0) {
                    log = new LogLineTaskError(m_taskId, Resources.Log_FinalDeleteFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalDeleteSuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is MakeDirectoryBackgroundTask) {
                // ディレクトリ作成完了
                if (FailCount > 0) {
                    log = new LogLineSimple(LogColor.Error, Resources.Log_FinalMkDirFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalMkDirSuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is RenameBackgroundTask) {
                // 名前変更完了
                if (FailCount > 0) {
                    log = new LogLineSimple(LogColor.Error, Resources.Log_FinalRenameFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalRenameSuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is RenameSelectedFileInfoBackgroundTask) {
                // 名前変更完了
                if (FailCount > 0) {
                    log = new LogLineSimple(LogColor.Error, Resources.Log_FinalRenameSelectedFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalRenameSelectedSuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is LocalUploadBackgroundTask) {
                // アップロード完了
                if (FailCount > 0) {
                    log = new LogLineSimple(LogColor.Error, Resources.Log_FinalUploadFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalUploadSuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is LocalExecuteBackgroundTask) {
                // ダウンロード完了
                if (FileProviderSrcOrg.SrcFileSystem.LocalExecuteDownloadRequired) {
                    if (FailCount > 0) {
                        log = new LogLineSimple(LogColor.Error, Resources.Log_FinalDownloadFail, SuccessCount, SkipCount, FailCount);
                    } else {
                        log = new LogLineSimple(Resources.Log_FinalDownloadSuccess, SuccessCount, SkipCount, FailCount);
                    }
                }
            } else if (this is LocalArchiveBackgroundTask) {
                LocalArchiveBackgroundTask arcTask = (LocalArchiveBackgroundTask)this;
                if (arcTask.Operation == LocalArchiveBackgroundTask.OperationMode.Extract) {
                    // 展開完了
                    if (FailCount > 0) {
                        log = new LogLineSimple(LogColor.Error, Resources.Log_FinalExtractFail, SuccessCount, SkipCount, FailCount);
                    } else {
                        log = new LogLineSimple(Resources.Log_FinalExtractSuccess, SuccessCount, SkipCount, FailCount);
                    }
                } else {
                    // 圧縮完了
                    if (FailCount > 0) {
                        log = new LogLineSimple(LogColor.Error, Resources.Log_FinalArchiveFail, SuccessCount, SkipCount, FailCount);
                    } else {
                        log = new LogLineSimple(Resources.Log_FinalArchiveSuccess, SuccessCount, SkipCount, FailCount);
                    }
                }
            } else if (this is RetrieveFolderSizeBackgroundTask) {
                // フォルダサイズの取得完了
                if (FailCount > 0) {
                    log = new LogLineSimple(LogColor.Error, Resources.Log_FinalRetrieveFolderSizeFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalRetrieveFolderSizeSuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is DuplicateFileBackgroundTask) {
                // ファイル２重化完了
                if (FailCount > 0) {
                    log = new LogLineSimple(LogColor.Error, Resources.Log_FinalDuplicateFileFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalDuplicateFileSuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is CombineFileBackgroundTask) {
                // ファイル結合完了
                if (FailCount > 0) {
                    log = new LogLineSimple(LogColor.Error, Resources.Log_FinalCombineFileFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalCombineFileSuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is GitMoveBackgroundTask) {
                // Git Mv（移動）移動完了
                if (FailCount > 0) {
                    log = new LogLineTaskError(m_taskId, Resources.Log_FinalMoveFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalMoveSuccess, SuccessCount, SkipCount, FailCount);
                }
            } else if (this is GitMoveBackgroundTask) {
                // Git Mv（リネーム）移動完了
                if (FailCount > 0) {
                    log = new LogLineTaskError(m_taskId, Resources.Log_FinalRenameFail, SuccessCount, SkipCount, FailCount);
                } else {
                    log = new LogLineSimple(Resources.Log_FinalRenameSuccess, SuccessCount, SkipCount, FailCount);
                }
            }

            if (log != null) {
                Program.LogWindow.RegistLogLine(log);
            }

            // キャンセル時はエラー情報なしの扱い
            if (!(log is LogLineTaskError)) {
                m_fileErrorInfo = null;
            }
        }
        
        //=========================================================================================
        // 機　能：UIをリフレッシュする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        protected void RefreshUI() {
            m_refreshUiTarget.RefreshDirectory(SuccessCount, SkipCount, FailCount);
        }

        //=========================================================================================
        // 機　能：ファイル転送の状態を通知する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void ProgressEventHandler(object sender, FileProgressEventArgs evt) {
            m_currentLogLineFile.ProgressEventHandler(sender, evt);
            if (IsCancel) {
                evt.Cancel = true;
            }
        }
        
        //=========================================================================================
        // 機　能：ファイル転送のタスクをキャンセルする
        // 引　数：[in]reason  キャンセルした理由
        // 戻り値：なし
        //=========================================================================================
        public virtual void SetCancel(CancelReason reason) {
            m_cancelFlag.SetCancel(reason);
            m_requestContext.SetCancel(reason);
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        abstract public BackgroundTaskType BackgroundTaskType {
            get;
        }

        //=========================================================================================
        // プロパティ：転送元/転送先の情報
        //=========================================================================================
        abstract public BackgroundTaskPathInfo PathInfo {
            get;
        }

        //=========================================================================================
        // プロパティ：コンテキスト情報
        //=========================================================================================
        public FileOperationRequestContext RequestContext {
            get {
                return m_requestContext;
            }
        }

        //=========================================================================================
        // プロパティ：エラー発生時の情報（エラー情報を扱っていないときnull）
        //=========================================================================================
        public FileErrorInfo FileErrorInfo {
            get {
                return m_fileErrorInfo;
            }
        }

        //=========================================================================================
        // プロパティ：転送元ファイル一覧
        //=========================================================================================
        public IFileProviderSrc FileProviderSrc {
            get {
                return m_fileProviderSrc;
            }
        }

        //=========================================================================================
        // プロパティ：転送元ファイル一覧（初期状態）
        //=========================================================================================
        public IFileProviderSrc FileProviderSrcOrg {
            get {
                return m_fileProviderSrcOrg;
            }
        }

        //=========================================================================================
        // プロパティ：転送先ファイル一覧
        //=========================================================================================
        public IFileProviderDest FileProviderDest {
            get {
                return m_fileProviderDest;
            }
        }

        //=========================================================================================
        // プロパティ：転送先ファイル一覧（初期状態）
        //=========================================================================================
        public IFileProviderDest FileProviderDestOrg {
            get {
                return m_fileProviderDestOrg;
            }
        }

        //=========================================================================================
        // プロパティ：転送用ファイルシステム
        //=========================================================================================
        public IFileSystemToFileSystem FileSystemToFileSystem {
            get {
                return m_fileSystemToFileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：タスクID
        //=========================================================================================
        public BackgroundTaskID TaskId {
            get {
                return m_taskId;
            }
        }

        //=========================================================================================
        // プロパティ：処理を中止するときtrue
        //=========================================================================================
        public override bool IsCancel {
            get {
                if (base.IsCancel) {
                    return true;
                }
                return m_cancelFlag.IsCancel;
            }
        }

        //=========================================================================================
        // プロパティ：成功した数
        //=========================================================================================
        public int SuccessCount {
            get {
                return m_successCount;
            }
            set {
                m_successCount = value;
            }
        }

        //=========================================================================================
        // プロパティ：スキップした数
        //=========================================================================================
        public int SkipCount {
            get {
                return m_skipCount;
            }
            set {
                m_skipCount = value;
            }
        }

        //=========================================================================================
        // プロパティ：失敗した数
        //=========================================================================================
        public int FailCount {
            get {
                return m_failCount;
            }
            set {
                m_failCount = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログ出力した件数
        //=========================================================================================
        public int LogCount {
            get {
                return m_currentLogLineFile.LogCount;
            }
        }

        //=========================================================================================
        // プロパティ：進捗表示用のハンドラ
        //=========================================================================================
        public FileProgressEventHandler Progress {
            get {
                return m_currentLogLineFile.Progress;
            }
        }
    }
}
