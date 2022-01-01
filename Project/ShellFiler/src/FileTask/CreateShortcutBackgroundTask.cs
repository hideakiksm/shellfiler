using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Document;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Management;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ショートカットの作成をバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class CreateShortcutBackgroundTask : AbstractFileBackgroundTask {
        // 作成するショートカットの種類
        private ShortcutType m_shortcutType;

        // 同名ファイルを発見したときの動作
        private SameFileOperation m_sameFileOperation;

        // 再試行するAPIのリスト（再試行しないとき空のリスト）
        private List<RetryInfoSrcDest> m_retryList;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider  転送元ファイルの情報
        // 　　　　[in]destProvider 転送先ファイルの情報
        // 　　　　[in]refreshUi    作業完了時のUI更新方法
        // 　　　　[in]shortcutType ショートカットの種類
        // 　　　　[in]retryList    再試行するAPIのリスト（再試行しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public CreateShortcutBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, ShortcutType shortcutType, List<IRetryInfo> retryList) : base(srcProvider, destProvider, refreshUi) {
            m_shortcutType = shortcutType;
            m_sameFileOperation = SameFileOperation.CreateWithDefaultConfig(destProvider.DestFileSystem.FileSystemId);
            m_sameFileOperation.AllApply = false;

            // 再試行情報を作成
            ActivateFileErrorInfo(null);
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
            string destShort = FileProviderDest.DestFileSystem.GetFileName(GenericFileStringUtils.TrimLastSeparator(FileProviderDest.DestDirectoryName));
            string destDetail = FileProviderDest.DestDirectoryName;

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
            FileProviderDest.DestFileSystem.BeginFileOperation(RequestContext, FileProviderDest.DestDirectoryName);

            try {
                LinkFilesRetryApi();
                LinkMarkFiles();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：再試行対象のファイルのショートカットを作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void LinkFilesRetryApi() {
            int allCount = FileProviderSrc.SrcItemCount + m_retryList.Count;
            for (int i = 0; i < m_retryList.Count; i++) {
                if (CheckSuspend()) {
                    return;
                }
                m_backgroundTaskPathInfo.SetProgressCount(i + 1, allCount);
                FireBackgroundtaskPathInfoProgressEvent();

                RetryInfoSrcDest retryInfo = m_retryList[i];
                LogFileOperationSetMarkInfo(retryInfo.SrcMarkObjectPath);
                CreateShortcut(retryInfo.SrcFilePath, retryInfo.DestFilePath);
                if (IsCancel) {
                    return;
                }
            }
        }

        //=========================================================================================
        // 機　能：マークされたファイルのショートカットを作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void LinkMarkFiles() {
            int allCount = FileProviderSrc.SrcItemCount + m_retryList.Count;
            for (int i = 0; i <FileProviderSrc.SrcItemCount; i++) {
                if (CheckSuspend()) {
                    return;
                }
                m_backgroundTaskPathInfo.SetProgressCount(m_retryList.Count + i + 1, allCount);
                FireBackgroundtaskPathInfoProgressEvent();

                SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(i);
                LogFileOperationSetMarkInfo(pathInfo);
                string srcPath = pathInfo.FilePath;
                string destPath = FileProviderDest.DestDirectoryName;
                CreateShortcut(srcPath, destPath);
                if (IsCancel) {
                    return;
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイルをコピーする
        // 引　数：[in]srcFilePath  転送元パス名(C:\SRCBASE\MARKFILE.txt)
        // 　　　　[in]destPath     転送先パス名(D:\DESTBASE\)
        // 戻り値：なし
        //=========================================================================================
        private void CreateShortcut(string srcFilePath, string destPath) {
            // ショートカットを作成
            string srcFileName = FileProviderSrc.SrcFileSystem.GetFileName(srcFilePath);
            string destFileName;
            if (m_shortcutType == ShortcutType.WindowsShortcut) {
                destFileName = GenericFileStringUtils.GetFileNameBody(srcFileName) + ".lnk";
            } else {
                destFileName = srcFileName;
            }
            string destFilePath = FileProviderDest.DestFileSystem.CombineFilePath(destPath, destFileName);
            LogFileOperationStart(FileOperationType.Shortcut, srcFilePath, false);
            FileOperationStatus status = FileSystemToFileSystem.CreateShortcut(RequestContext, srcFilePath, destFilePath, false, m_shortcutType);

            // すでに存在する場合は同名ファイルの処理
            if (status == FileOperationStatus.AlreadyExists) {
                SameNameFileTransfer.TransferDelegate operation = delegate(FileOperationRequestContext context, string srcFile, string destFile, bool overwrite) {
                    FileOperationStatus transStatus = FileOperationStatus.Fail;
                    if (m_shortcutType == ShortcutType.WindowsShortcut) {
                        transStatus = FileSystemToFileSystem.CreateShortcut(context, srcFile, destFile, overwrite, ShortcutType.WindowsShortcut);
                    } else if (m_shortcutType == ShortcutType.SymbolicLink) {
                        transStatus = FileSystemToFileSystem.CreateShortcut(context, srcFile, destFile, overwrite, ShortcutType.SymbolicLink);
                    } else if (m_shortcutType == ShortcutType.HardLink) {
                        transStatus = FileSystemToFileSystem.CreateShortcut(context, srcFile, destFile, overwrite, ShortcutType.HardLink);
                    }
                    return transStatus;
                };
                SameNameFileTransfer sameFile = new SameNameFileTransfer(this, RequestContext, FileProviderSrc, FileProviderDest, SameNameFileTransfer.TransferMode.LinkSameFile, m_sameFileOperation, operation);
                SameNameTargetFileDetail fileDetail = new SameNameTargetFileDetail(RequestContext, FileProviderSrc.SrcFileSystem, FileProviderDest.DestFileSystem, srcFilePath, destFilePath);
                try {
                    status = sameFile.TransferSameFile(fileDetail);
                    if (status == FileOperationStatus.Canceled) {
                        SetCancel(CancelReason.User);
                    }
                    m_sameFileOperation = sameFile.SameFileOperation;
                } finally {
                    fileDetail.Dispose();
                }
            }

            // ステータスを更新
            if (!status.Succeeded) {
                LogFileOperationEnd(status, new RetryInfoSrcDest(FileOperationApiType.FileCopy, srcFilePath, destPath));
            } else {
                LogFileOperationEnd(status);
            }
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.CreateShortcut;
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
    }
}
