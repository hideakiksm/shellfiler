using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.Management;
using ShellFiler.Document.Setting;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイル削除をバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class DeleteBackgroundTask : AbstractFileBackgroundTask {
        // 削除操作のモード
        private DeleteFileOption m_deleteFileOption;

        // ごみ箱/rm -rfを使って削除するときtrue
        private bool m_deleteDirectoryDirect;

        // 削除のオプション
        private CopyMoveDeleteOption m_deleteOption;

        // 再試行するAPIのリスト（再試行しないとき空のリスト）
        private List<RetryInfoSrcDest> m_retryList;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider  転送元ファイルの情報
        // 　　　　[in]destProvider 転送先ファイルの情報
        // 　　　　[in]refreshUi    作業完了時のUI更新方法
        // 　　　　[in]deleteOpt    削除操作のモード
        // 　　　　[in]delDirDirect ごみ箱/rm -rfを使って削除するときtrue
        // 　　　　[in]option       削除のオプション
        // 　　　　[in]retryList    再試行するAPIのリスト（再試行しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public DeleteBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, DeleteFileOption deleteOpt, bool delDirDirect, CopyMoveDeleteOption option, List<IRetryInfo> retryList) :
                    base(srcProvider, destProvider, refreshUi) {
            m_deleteFileOption = deleteOpt;
            m_deleteDirectoryDirect = delDirDirect;
            m_deleteOption = option;

            // 再試行情報を作成
            ActivateFileErrorInfo(option);
            if (retryList == null) {
                m_retryList = new List<RetryInfoSrcDest>();
            } else {
                m_retryList = new List<RetryInfoSrcDest>();
                foreach (IRetryInfo info in retryList) {
                    m_retryList.Add((RetryInfoSrcDest)info);
                }
            }
            CreateBackgroundTaskPathInfo();
        }

        //=========================================================================================
        // 機　能：転送元/転送先の情報を作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CreateBackgroundTaskPathInfo() {
            // 転送元
            string srcShort = BackgroundTaskPathInfo.CreateShortTextFileProviderSrc(FileProviderSrc, m_retryList.ToArray());
            string srcDetail = BackgroundTaskPathInfo.CreateDetailTextFileProviderSrc(FileProviderSrc, m_retryList.ToArray());

            // 転送先
            string destShort = "";
            string destDetail = "";

            m_backgroundTaskPathInfo = new BackgroundTaskPathInfo(srcShort, srcDetail, destShort, destDetail);
        }

        //=========================================================================================
        // 機　能：タスクを実行する
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンドスレッドで実行する
        //=========================================================================================
        protected override void ExecuteTask() {
            string srcPath;
            if (m_retryList.Count > 0) {
                srcPath = m_retryList[0].SrcFilePath;
            } else {
                srcPath = FileProviderSrc.GetSrcPath(0).FilePath;
            }
            FileProviderSrc.SrcFileSystem.BeginFileOperation(RequestContext, srcPath);

            try {
                DeleteFilesRetryApi();
                DeleteMarkFiles();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：再試行対象のファイルをAPI単位で削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void DeleteFilesRetryApi() {
            DeleteWorker worker = new DeleteWorker(new TaskControler(this), FileProviderSrc.SrcFileSystem, RequestContext, true, m_deleteFileOption, m_deleteDirectoryDirect);
            int allCount = FileProviderSrc.SrcItemCount + m_retryList.Count;
            for (int i = 0; i < m_retryList.Count; i++) {
                if (CheckSuspend()) {
                    return;
                }
                m_backgroundTaskPathInfo.SetProgressCount(i + 1, allCount);
                FireBackgroundtaskPathInfoProgressEvent();

                RetryInfoSrcDest retryInfo = m_retryList[i];
                LogFileOperationSetMarkInfo(retryInfo.SrcMarkObjectPath);
                worker.DeleteFile(retryInfo.SrcFilePath, null, null);  // 前回DeleteFile失敗のため、無条件に再試行
                if (IsCancel) {
                    return;
                }
            }
        }

        //=========================================================================================
        // 機　能：マークされたファイルを削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void DeleteMarkFiles() {
            DeleteWorker worker = new DeleteWorker(new TaskControler(this), FileProviderSrc.SrcFileSystem, RequestContext, true, m_deleteFileOption, m_deleteDirectoryDirect);
            int allCount = FileProviderSrc.SrcItemCount + m_retryList.Count;
            for (int i = 0; i <FileProviderSrc.SrcItemCount; i++) {
                if (CheckSuspend()) {
                    return;
                }
                m_backgroundTaskPathInfo.SetProgressCount(m_retryList.Count + i + 1, allCount);
                FireBackgroundtaskPathInfoProgressEvent();

                SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(i);
                LogFileOperationSetMarkInfo(pathInfo);
                if (!pathInfo.IsDirectory || pathInfo.IsSymbolicLink) {
                    // ファイルを削除
                    worker.DeleteFile(pathInfo.FilePath, null, m_deleteOption.TransferCondition);
                    if (IsCancel) {
                        return;
                    }
                } else {
                    // ディレクトリを削除
                    FileOperationStatus status = worker.DeleteDirectory(pathInfo.FilePath, null, m_deleteOption.TransferCondition);
                    LogFileOperationStart(FileOperationType.DeleteDir, pathInfo.FilePath, true);
                    LogFileOperationEnd(status);
                    if (IsCancel) {
                        return;
                    }
                }
            }
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                if (m_deleteDirectoryDirect) {
                    return BackgroundTaskType.Delete;
                } else {
                    return BackgroundTaskType.DeleteNoRecycle;
                }
            }
        }

        //=========================================================================================
        // プロパティ：転送元/転送先の情報
        //=========================================================================================
        public override BackgroundTaskPathInfo PathInfo {
            get {
                return m_backgroundTaskPathInfo;
            }
        }

        //=========================================================================================
        // クラス：AbstractFileBackgroundTaskへのタスクメソッド中継クラス
        //=========================================================================================
        public class TaskControler {
            // 中継先のタスク
            private AbstractFileBackgroundTask m_task;

            public TaskControler(AbstractFileBackgroundTask task) {
                m_task = task;
            }

            public bool CheckSuspend() {
                return m_task.CheckSuspend();
            }

            public void LogFileOperationStart(FileOperationType operationType, string filePath, bool isDir) {
                m_task.LogFileOperationStart(operationType, filePath, isDir);
            }

            public FileOperationStatus LogFileOperationEnd(FileOperationStatus status) {
                return m_task.LogFileOperationEnd(status);
            }

            public FileOperationStatus LogFileOperationEnd(FileOperationStatus status, IRetryInfo retryInfo) {
                return m_task.LogFileOperationEnd(status, retryInfo);
            }

            public void SetCancel(CancelReason reason) {
                m_task.SetCancel(reason);
            }

            public bool IsCancel {
                get {
                    return m_task.IsCancel;
                }
            }
        }

        //=========================================================================================
        // クラス：削除処理の実行クラス（ミラーコピーとの共用のため）
        //=========================================================================================
        public class DeleteWorker {
            // 実行中のタスク
            private TaskControler m_task;

            // 操作対象のファイルシステム
            private IFileSystem m_fileSystem;

            // コンテキスト情報
            private FileOperationRequestContext m_requestContext;

            // 再試行をサポートするときtrue
            private bool m_supportRetry;

            // 削除操作のモード
            private DeleteFileOption m_deleteFileOption;

            // ごみ箱/rm -rfを使って削除するときtrue
            private bool m_deleteDirectoryDirect;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]task           実行中のタスク
            // 　　　　[in]fileSystem     操作対象のファイルシステム
            // 　　　　[in]requestContext コンテキスト情報
            // 　　　　[in]supportRetry   再試行をサポートするときtrue
            // 　　　　[in]deleteOption   削除操作のモード
            // 　　　　[in]deleteDirect   ごみ箱/rm -rfを使って削除するときtrue
            // 戻り値：なし
            //=========================================================================================
            public DeleteWorker(TaskControler task, IFileSystem fileSystem, FileOperationRequestContext requestContext, bool supportRetry, DeleteFileOption deleteOption, bool deleteDirect) {
                m_task = task;
                m_fileSystem = fileSystem;
                m_requestContext = requestContext;
                m_supportRetry = supportRetry;
                m_deleteFileOption = deleteOption;
                m_deleteDirectoryDirect = deleteDirect;
            }

            //=========================================================================================
            // 機　能：ファイルを削除する
            // 引　数：[in]filePath  削除対象のファイル名(C:\SRCBASE\MARKFILE.txt)
            // 　　　　[in]fileInfo  取得済みのファイル情報（まだ取得していないときはnull）
            // 　　　　[in]condition 削除条件（無条件に削除するときnull）
            // 戻り値：実際に削除したときtrue
            //=========================================================================================
            public bool DeleteFile(string filePath, IFile fileInfo, CompareCondition condition) {
                if (m_task.CheckSuspend()) {
                    return false;
                }

                m_task.LogFileOperationStart(FileOperationType.DeleteFile, filePath, false);

                // 属性により削除を確認
                // ファイル属性も取得
                bool changeAttr;
                FileOperationStatus status = CheckFileAttribute(filePath, fileInfo, condition, out changeAttr);
                if (status == FileOperationStatus.SkippedCondition) {
                    m_task.LogFileOperationEnd(FileOperationStatus.Skip);
                    return false;
                } else if (!status.Succeeded) {
                    m_task.LogFileOperationEnd(status, new RetryInfoSrcDest(FileOperationApiType.FileDelete, filePath, null));
                    return false;
                }

                // 削除を実行
                DeleteFileFolderFlag flag = DeleteFileFolderFlag.FILE;
                if (changeAttr) {
                    flag |= DeleteFileFolderFlag.CHANGE_ATTR;
                }
                if (m_deleteDirectoryDirect) {
                    flag |= DeleteFileFolderFlag.RECYCLE_OR_RF;
                }
                status = m_fileSystem.DeleteFileFolder(m_requestContext, filePath, true, flag);
                if (!status.Succeeded) {
                    if (m_supportRetry) {
                        m_task.LogFileOperationEnd(status, new RetryInfoSrcDest(FileOperationApiType.FileDelete, filePath, null));
                    } else {
                        m_task.LogFileOperationEnd(status, null);
                    }
                    return false;
                } else {
                    m_task.LogFileOperationEnd(status);
                    return true;
                }
            }

            //=========================================================================================
            // 機　能：ファイル属性を確認する
            // 引　数：[in]filePath    削除対象のファイル/ディレクトリ名(C:\SRCBASE\MARKFILE.txt)
            // 　　　　[in]fileInfo    取得済みのファイル情報（まだ取得していないときはnull）
            // 　　　　[in]condition   削除条件（無条件に削除するときnull）
            // 　　　　[out]changeAttr 削除前に属性の変更が必要なときtrue
            // 戻り値：ステータス（削除を開始してよいときSuccess、条件不一致で対象外となったときSkippedCondition、開始できないときはその理由）
            //=========================================================================================
            private FileOperationStatus CheckFileAttribute(string filePath, IFile fileInfo, CompareCondition condition, out bool changeAttr) {
                changeAttr = false;

                // 属性を取得
                if (fileInfo == null) {
                    FileOperationStatus status = m_fileSystem.GetFileInfo(m_requestContext, filePath, true, out fileInfo);
                    if (status == FileOperationStatus.FailLinkTarget) {
                        return FileOperationStatus.Success;
                    } else if (!status.Succeeded) {
                        return status;
                    }
                }
                
                // 削除条件を確認
                if (condition != null) {
                    bool target = TargetConditionComparetor.IsTarget(condition, fileInfo, true);
                    if (!target) {
                        return FileOperationStatus.SkippedCondition;
                    }
                }

                // 確認
                if (fileInfo.Attribute.IsReadonly || fileInfo.Attribute.IsSystem) {
                    if (!m_deleteFileOption.DeleteSpecialAttrAll) {
                        DialogResult result = ShowDeleteConfirmDialog(filePath, fileInfo.Attribute);
                        switch (result) {
                            case DialogResult.Cancel:
                                m_task.SetCancel(CancelReason.User);
                                return FileOperationStatus.Canceled;
                            case DialogResult.Ignore:
                                m_deleteFileOption.DeleteSpecialAttrAll = true;
                                break;
                            case DialogResult.No:
                                return FileOperationStatus.NoDel;
                            case DialogResult.None:
                                return FileOperationStatus.Canceled;
                        }
                    }
     
                    changeAttr = true;
                }

                return FileOperationStatus.Success;
            }

            //=========================================================================================
            // 機　能：削除してよいかどうかの確認を行う
            // 引　数：[in]srcFilePath  転送元パス名(C:\SRCBASE\MARKFILE.txt)
            // 　　　　[in]attribute    ファイルの属性
            // 戻り値：ダイアログの応答
            //=========================================================================================
            private DialogResult ShowDeleteConfirmDialog(string srcFilePath, FileAttribute attribute) {
                object result;
                bool success = BaseThread.InvokeFunctionByMainThread(new ShowDeleteConfirmDialogDelegate(ShowDeleteConfirmDialogUI), out result, srcFilePath, attribute);
                if (!success) {
                    return DialogResult.None;
                }
                return (DialogResult)result;
            }
            private delegate DialogResult ShowDeleteConfirmDialogDelegate(string srcFilePath, FileAttribute attribute);
            private static DialogResult ShowDeleteConfirmDialogUI(string srcFilePath, FileAttribute attribute) {
                DeleteConfirmDialog dialog = new DeleteConfirmDialog();
                dialog.FileName = srcFilePath;
                dialog.FileAttribute = attribute;
                return dialog.ShowDialog(Program.MainWindow);
            }

            //=========================================================================================
            // 機　能：ディレクトリを削除する
            // 引　数：[in]dirPath   削除ディレクトリ名(C:\SRCBASE\MARKDIR)
            // 　　　　[in]fileInfo  取得済みのファイル情報（まだ取得していないときはnull）
            // 　　　　[in]condition 削除条件（無条件に削除するときnull）
            // 戻り値：ステータス
            //=========================================================================================
            public FileOperationStatus DeleteDirectory(string dirPath, IFile fileInfo, CompareCondition condition) {
                if (m_task.CheckSuspend()) {
                    return FileOperationStatus.Canceled;
                }
                FileOperationStatus status;

                // ディレクトリ一覧を取得
                List<IFile> fileList, dirList;
                status = BackgroundTaskCommandUtil.GetFileList(m_fileSystem, m_requestContext, dirPath, out fileList, out dirList);
                if (!status.Succeeded) {
                    m_task.LogFileOperationStart(FileOperationType.DeleteDir, dirPath, true);
                    m_task.LogFileOperationEnd(status);
                    return status;
                }

                // ディレクトリを削除してよいかどうかを確認
                status = IsStartDeleteDir(dirPath, fileList, dirList);
                if (status != null) {
                    return status;
                }

                // 属性により削除を確認
                // フォルダの情報も取得
                bool changeAttr;
                status = CheckFileAttribute(dirPath, fileInfo, condition, out changeAttr);
                if (!status.Succeeded) {
                    return status;
                } else if (status != FileOperationStatus.SkippedCondition) {
                    condition = null;                // このフォルダが条件に該当する場合は、配下を無条件に削除する
                }

                // ディレクトリを削除
                if (m_deleteDirectoryDirect && condition == null) {
                    // ごみ箱で削除
                    DeleteFileFolderFlag flag = DeleteFileFolderFlag.FOLDER | DeleteFileFolderFlag.RECYCLE_OR_RF;
                    if (changeAttr) {
                        flag |= DeleteFileFolderFlag.CHANGE_ATTR;
                    }
                    status = m_fileSystem.DeleteFileFolder(m_requestContext, dirPath, true, flag);
                } else {
                    // 直接削除
                    bool deleteAll = true;
                    for (int i = 0; i < fileList.Count; i++) {
                        IFile file = fileList[i];
                        string srcFilePath = m_fileSystem.CombineFilePath(dirPath, file.FileName);
                        bool deleted = DeleteFile(srcFilePath, file, condition);
                        if (m_task.IsCancel) {
                            return FileOperationStatus.Canceled;
                        }
                        if (!deleted) {
                            deleteAll = false;
                        }
                        fileList[i] = null;
                    }
                    fileList = null;

                    // ディレクトリを再帰的に削除
                    for (int i = 0; i < dirList.Count; i++) {
                        IFile file = dirList[i];
                        string srcDirPath = m_fileSystem.CombineFilePath(dirPath, file.FileName);
                        status = DeleteDirectory(srcDirPath, file, condition);
                        m_task.LogFileOperationStart(FileOperationType.DeleteDir, srcDirPath, true);
                        m_task.LogFileOperationEnd(status);
                        if (m_task.IsCancel) {
                            return FileOperationStatus.Canceled;
                        }
                        if (!status.Succeeded || status == FileOperationStatus.Skip) {
                            deleteAll = false;
                        }
                        dirList[i] = null;
                    }

                    // 自分自身を削除
                    if (deleteAll && condition == null) {
                        DeleteFileFolderFlag flag = DeleteFileFolderFlag.FOLDER;
                        if (changeAttr) {
                            flag |= DeleteFileFolderFlag.CHANGE_ATTR;
                        }
                        status = m_fileSystem.DeleteFileFolder(m_requestContext, dirPath, true, flag);
                    } else {
                        status = FileOperationStatus.Skip;
                    }
                }
                return status;
            }

            //=========================================================================================
            // 機　能：ディレクトリを削除してよいかどうかを確認する
            // 引　数：[in]dirPath  削除ディレクトリ名(C:\SRCBASE\MARKDIR)
            // 　　　　[in]fileList 指定ディレクトリ以下のファイル一覧
            // 　　　　[in]dirList  指定ディレクトリ以下のディレクトリ一覧
            // 戻り値：ステータス（削除を開始してよいときnull、開始できないときはその理由）
            //=========================================================================================
            private FileOperationStatus IsStartDeleteDir(string dirPath, List<IFile> fileList, List<IFile> dirList) {
                // 連続実行時は継続
                if (m_deleteFileOption.DeleteDirectoryAll) {
                    return null;
                }

                // 配下にファイルかディレクトリがあれば確認
                bool existFile = (fileList.Count > 0 || dirList.Count > 0);
                if (!existFile) {
                    return null;
                }

                // ユーザに削除の確認
                FileAttribute attrDir = new FileAttribute();
                attrDir.IsDirectory = true;
                DialogResult result = ShowDeleteConfirmDialog(dirPath, attrDir);
                switch (result) {
                    case DialogResult.Cancel:
                        m_task.SetCancel(CancelReason.User);
                        return FileOperationStatus.Canceled;
                    case DialogResult.Ignore:
                        m_deleteFileOption.DeleteDirectoryAll = true;
                        break;
                    case DialogResult.No:
                        return FileOperationStatus.NoDel;
                    case DialogResult.None:
                        return FileOperationStatus.Canceled;
                }

                return null;
            }
        }
    }
}
