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
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイルの移動をバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class MoveBackgroundTask : AbstractFileBackgroundTask {
        // 移動のオプション
        private CopyMoveDeleteOption m_moveOption;

        // 再試行するAPIのリスト（再試行しないとき空のリスト）
        private List<RetryInfoSrcDest> m_retryList;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider  転送元ファイルの情報
        // 　　　　[in]destProvider 転送先ファイルの情報
        // 　　　　[in]refreshUi    作業完了時のUI更新方法
        // 　　　　[in]option       コピーと移動のオプション
        // 　　　　[in]retryList    再試行するAPIのリスト（再試行しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public MoveBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, CopyMoveDeleteOption option, List<IRetryInfo> retryList) : base(srcProvider, destProvider, refreshUi) {
            m_moveOption = option;

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
                MoveFilesRetryApi();
                MoveMarkFiles();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：再試行対象のファイルをAPI単位で移動する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void MoveFilesRetryApi() {
            FileOperationStatus status;
            int allCount = FileProviderSrc.SrcItemCount + m_retryList.Count;
            for (int i = 0; i < m_retryList.Count; i++) {
                if (CheckSuspend()) {
                    return;
                }
                m_backgroundTaskPathInfo.SetProgressCount(i + 1, allCount);
                FireBackgroundtaskPathInfoProgressEvent();

                RetryInfoSrcDest retryInfo = m_retryList[i];
                LogFileOperationSetMarkInfo(retryInfo.SrcMarkObjectPath);

                // 属性を取得
                IFile fileInfo;
                status = FileProviderSrc.SrcFileSystem.GetFileInfo(RequestContext, retryInfo.SrcFilePath, true, out fileInfo);
                if (!status.Succeeded) {
                    LogFileOperationStart(FileOperationType.MoveFile, retryInfo.SrcFilePath, false);
                    LogFileOperationEnd(status);
                    continue;
                }

                // ファイルを移動
                bool dummyModified = false;
                List<string> mkdirPendingList = new List<string>();
                MoveFile(fileInfo, retryInfo.SrcFilePath, retryInfo.DestFilePath, null, mkdirPendingList, ref dummyModified);
                if (IsCancel) {
                    return;
                }
            }
        }

        //=========================================================================================
        // 機　能：マークされたファイルを移動する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void MoveMarkFiles() {
            FileOperationStatus status;
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
                bool isFile = (!pathInfo.IsDirectory || pathInfo.IsSymbolicLink);

                // 属性を取得
                IFile fileInfo;
                status = FileProviderSrc.SrcFileSystem.GetFileInfo(RequestContext, srcPath, true, out fileInfo);
                if (!status.Succeeded) {
                    if (isFile) {
                        LogFileOperationStart(FileOperationType.MoveFile, srcPath, false);
                        LogFileOperationEnd(status);
                    } else {
                        LogFileOperationStart(FileOperationType.MoveDir, srcPath, true);
                        LogFileOperationEnd(status);
                    }
                    continue;
                }

                // 転送関係を確認
                string destPath = FileProviderDest.DestDirectoryName;
                bool isSame = CheckFileSrcDest(srcPath, destPath, isFile);
                if (isSame) {
                    continue;
                }

                if (isFile) {
                    // ファイルを移動
                    List<string> mkdirPendingList = new List<string>();
                    bool dummyModified = false;
                    MoveFile(fileInfo, srcPath, destPath, m_moveOption.TransferCondition, mkdirPendingList, ref dummyModified);
                    if (IsCancel) {
                        return;
                    }
                } else {
                    // ディレクトリを移動
                    if (!FileBackgroundTaskUtil.AllowTransfer(FileProviderSrc.SrcFileSystem, srcPath, destPath)) {
                        // 転送関係が異常
                        LogFileOperationStart(FileOperationType.MoveDir, srcPath, true);
                        LogFileOperationEnd(FileOperationStatus.SrcDest);
                    } else {
                        string markDir = FileProviderSrc.SrcFileSystem.GetFileName(pathInfo.FilePath);
                        string destTarget = FileProviderDest.DestFileSystem.CombineFilePath(destPath, markDir);
                        List<string> mkdirPendingList = new List<string>();
                        MoveDirectory(fileInfo, srcPath, destTarget, m_moveOption.TransferCondition, mkdirPendingList);
                    }
                    if (IsCancel) {
                        return;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：転送元と転送先が同じかどうかを調べる
        // 引　数：[in]srcPath   転送元ファイルのパス（ファイル名）
        // 　　　　[in]destPath  転送先ファイルのパス（フォルダ名）
        // 　　　　[in]isFile    ファイルのときtrue
        // 戻り値：同名のときtrue
        //=========================================================================================
        private bool CheckFileSrcDest(string srcPath, string destPath, bool isFile) {
            // 比較
            srcPath = FileProviderSrc.SrcFileSystem.GetDirectoryName(srcPath);
            if (srcPath != destPath) {
                // 違っていれば小文字にして比較
                if (FileSystemID.IgnoreCaseFolderPath(FileProviderSrc.SrcFileSystem.FileSystemId)) {
                    srcPath = srcPath.ToLower();
                }
                if (FileSystemID.IgnoreCaseFolderPath(FileProviderDest.DestFileSystem.FileSystemId)) {
                    destPath = destPath.ToLower();
                }
                if (srcPath != destPath) {
                    return false;
                }
            }

            // 同名の移動
            if (isFile) {
                LogFileOperationStart(FileOperationType.MoveFile, srcPath, false);
                LogFileOperationEnd(FileOperationStatus.SrcDest);
            } else {
                LogFileOperationStart(FileOperationType.MoveDir, srcPath, true);
                LogFileOperationEnd(FileOperationStatus.SrcDest);
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルを移動する
        // 引　数：[in]srcFileInfo      取得済みの転送元ファイル情報
        // 　　　　[in]srcFilePath      転送元パス名(C:\SRCBASE\MARKFILE.txt)
        // 　　　　[in]destPath         転送先パス名(D:\DESTBASE\)
        // 　　　　[in]condition        削除条件（無条件に削除するときnull）
        // 　　　　[in]mkdirPendingList ディレクトリ作成を保留しているディレクトリのリスト
        // 　　　　[in,out]folderModify 転送先フォルダ内のコンテンツが変更されたときtrueを返す変数
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus MoveFile(IFile srcFileInfo, string srcFilePath, string destPath, CompareCondition condition, List<string> mkdirPendingList, ref bool folderModify) {
            if (CheckSuspend()) {
                return FileOperationStatus.Canceled;
            }

            FileOperationStatus status;
            string srcFileName = FileProviderSrc.SrcFileSystem.GetFileName(srcFilePath);
            string destFilePath = FileProviderDest.DestFileSystem.CombineFilePath(destPath, srcFileName);

            // 属性を確認
            if (condition != null) {
                status = CheckFileAttribute(srcFilePath, srcFileInfo, condition, mkdirPendingList, ref folderModify);
                if (status == FileOperationStatus.SkippedCondition || !status.Succeeded) {
                    return status;
                }
            }

            // ファイルの移動を実行
            // これより下で失敗した場合のみAPIレベルでの再試行が可能
            LogFileOperationStart(FileOperationType.MoveFile, srcFilePath, false);
            status = FileSystemToFileSystem.MoveFileDirectory(RequestContext, srcFilePath, srcFileInfo, destFilePath, false, m_moveOption.AttributeSetMode, new FileProgressEventHandler(ProgressEventHandler));

            // すでに存在する場合は同名ファイルの処理
            if (status == FileOperationStatus.AlreadyExists) {
                SameNameFileTransfer.TransferDelegate operation = delegate(FileOperationRequestContext context, string srcFile, string destFile, bool overwrite) {
                    status = FileSystemToFileSystem.MoveFileDirectory(context, srcFile, srcFileInfo, destFile, overwrite, m_moveOption.AttributeSetMode, new FileProgressEventHandler(ProgressEventHandler));
                    return status;
                };
                SameNameFileTransfer sameFile = new SameNameFileTransfer(this, RequestContext, FileProviderSrc, FileProviderDest, SameNameFileTransfer.TransferMode.MoveSameFile, m_moveOption.SameFileOperation, operation);
                SameNameTargetFileDetail fileDetail = new SameNameTargetFileDetail(RequestContext, FileProviderSrc.SrcFileSystem, FileProviderDest.DestFileSystem, srcFilePath, destFilePath);
                try {
                    status = sameFile.TransferSameFile(fileDetail);
                    if (status == FileOperationStatus.Canceled) {
                        SetCancel(CancelReason.User);
                    }
                    m_moveOption.SameFileOperation = sameFile.SameFileOperation;
                } finally {
                    fileDetail.Dispose();
                }
            }

            // ステータスを更新
            if (!status.Succeeded) {
                LogFileOperationEnd(status, new RetryInfoSrcDest(FileOperationApiType.FileMove, srcFilePath, destPath));
            } else {
                LogFileOperationEnd(status);
                if (status != FileOperationStatus.NoMove) {
                    folderModify = true;
                }
            }
            return status;
        }
        
        //=========================================================================================
        // 機　能：ディレクトリを移動する
        // 引　数：[in]srcFileInfo      取得済みの転送元ファイル情報
        // 　　　　[in]srcPath          転送元パス名(C:\SRCBASE\MARKDIR)
        // 　　　　[in]destPath         転送先パス名(D:\DESTBASE\MARKDIR)
        // 　　　　[in]condition        削除条件（無条件に削除するときnull）
        // 　　　　[in]mkdirPendingList ディレクトリ作成を保留しているディレクトリのリスト
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus MoveDirectory(IFile srcFileInfo, string srcPath, string destPath, CompareCondition condition, List<string> mkdirPendingList) {
            if (CheckSuspend()) {
                return FileOperationStatus.Canceled;
            }
            FileOperationStatus status = FileOperationStatus.Fail;

            bool folderModified = false;        // このフォルダまたはその配下コンテンツが更新されたときtrue（trueのとき、更新時刻をコピー）
            bool isPending = false;             // 条件付き転送でサブディレクトリの作成を保留にしているときtrue
            try {
                if (condition != null) {
                    // フォルダの移動の可否が不明なとき
                    isPending = true;
                    mkdirPendingList.Add(destPath);

                    // フォルダの情報を取得
                    status = CheckFileAttribute(srcPath, srcFileInfo, condition, mkdirPendingList, ref folderModified);
                    if (!status.Succeeded) {
                        return status;
                    } else if (status != FileOperationStatus.SkippedCondition) {
                        condition = null;                // このフォルダが条件に該当する場合は、配下を無条件に移動する
                    }
                } else {
                    // フォルダを移動してよいとき
                    // まずは直接移動してみる
                    if (FileSystemToFileSystem.CanMoveDirectoryDirect(srcPath, destPath, false)) {
                        status = FileSystemToFileSystem.MoveFileDirectory(RequestContext, srcPath, srcFileInfo, destPath, false, m_moveOption.AttributeSetMode, new FileProgressEventHandler(ProgressEventHandler));
                        if (status != FileOperationStatus.AlreadyExists && status.Succeeded) {
                            // 成功すればそれでOK
                            LogFileOperationStart(FileOperationType.MoveDir, srcPath, true);
                            LogFileOperationEnd(status);
                            status = FileOperationStatus.Success;
                            return status;
                        }
                    }

                    // ディレクトリを作成
                    LogFileOperationStart(FileOperationType.MakeDir, destPath, true);
                    status = FileBackgroundTaskUtil.DestCreateDirectory(RequestContext, FileProviderDest.DestFileSystem, destPath);
                    LogFileOperationEnd(status);
                    if (!status.Succeeded) {
                        return status;
                    }
                    if (status != FileOperationStatus.AlreadyExists) {
                        folderModified = true;
                    }
                }

                // ディレクトリをキューに入れる
                List<IFile> fileList, dirList;
                status = BackgroundTaskCommandUtil.GetFileList(FileProviderSrc.SrcFileSystem, RequestContext, srcPath, out fileList, out dirList);
                if (!status.Succeeded) {
                    LogFileOperationStart(FileOperationType.MoveDir,  srcPath, true);
                    LogFileOperationEnd(status);
                    return status;
                }

                // ファイルを移動
                bool moveAll = true;
                for (int i = 0; i < fileList.Count; i++) {
                    IFile file = fileList[i];
                    string srcFilePath = FileProviderSrc.SrcFileSystem.CombineFilePath(srcPath, file.FileName);
                    status = MoveFile(file, srcFilePath, destPath, condition, mkdirPendingList, ref folderModified);
                    if (IsCancel) {
                        status = FileOperationStatus.Canceled;
                        return status;
                    }
                    if (!status.Succeeded || status == FileOperationStatus.Skip || status == FileOperationStatus.NoMove) {
                        moveAll = false;
                    }
                    fileList[i] = null;
                }
                fileList = null;

                // ディレクトリを再帰的に移動
                for (int i = 0; i < dirList.Count; i++) {
                    IFile dir = dirList[i];
                    string srcDirPath = FileProviderSrc.SrcFileSystem.CombineFilePath(srcPath, dir.FileName);
                    string destDirPath = FileProviderDest.DestFileSystem.CombineFilePath(destPath, dir.FileName);
                    status = MoveDirectory(dir, srcDirPath, destDirPath, condition, mkdirPendingList);
                    if (IsCancel) {
                        status = FileOperationStatus.Canceled;
                        return status;
                    }
                    if (!status.Succeeded || status == FileOperationStatus.Skip || status == FileOperationStatus.NoMove) {
                        moveAll = false;
                    }
                    dirList[i] = null;
                }
                dirList = null;

                // 移動後、自分自身を削除
                if (moveAll && condition == null) {
                    status = FileProviderSrc.SrcFileSystem.DeleteFileFolder(RequestContext, srcPath, true, DeleteFileFolderFlag.FOLDER | DeleteFileFolderFlag.CHANGE_ATTR);
                    LogFileOperationStart(FileOperationType.DeleteDir, destPath, true);
                    LogFileOperationEnd(status);
                } else {
                    status = FileOperationStatus.Skip;
                }
                return status;
            } finally {
                if (isPending && mkdirPendingList.Count > 0) {
                    mkdirPendingList.RemoveAt(mkdirPendingList.Count - 1);
                    LogFileOperationStart(FileOperationType.MoveDir, srcPath, true);
                    LogFileOperationEnd(FileOperationStatus.Skip);
                }
                // 属性をコピー
                if (!IsCancel && folderModified) {
                    LogFileOperationStart(FileOperationType.CopyAttr, srcPath, true);
                    status = FileSystemToFileSystem.CopyFileInfo(RequestContext, true, srcPath, srcFileInfo, destPath, m_moveOption.AttributeSetMode);
                    LogFileOperationEnd(status);
                }
            }
        }
        
        //=========================================================================================
        // 機　能：ファイル属性を確認する
        // 引　数：[in]filePath    削除対象のファイル/ディレクトリ名(C:\SRCBASE\MARKFILE.txt)
        // 　　　　[in]fileInfo    取得済みのファイル情報
        // 　　　　[in]condition   削除条件（無条件に削除するときnull）
        // 　　　　[in]mkdirPendingList ディレクトリ作成を保留しているディレクトリのリスト
        // 　　　　[in,out]modified     フォルダ内データが更新されたときtrueを返す変数
        // 戻り値：ステータス（削除を開始してよいときSuccess、条件不一致で対象外となったときSkippedCondition、開始できないときはその理由）
        //=========================================================================================
        private FileOperationStatus CheckFileAttribute(string filePath, IFile fileInfo, CompareCondition condition, List<string> mkdirPendingList, ref bool modified) {
            FileOperationStatus status;
            FileOperationType logType = (fileInfo.Attribute.IsDirectory ? FileOperationType.CopyDir : FileOperationType.CopyFile);
            
            // 削除条件を確認
            if (condition != null) {
                bool target = TargetConditionComparetor.IsTarget(condition, fileInfo, true);
                if (!target) {
                    if (!fileInfo.Attribute.IsDirectory) {          // ファイルのみスキップログを出力
                        LogFileOperationStart(FileOperationType.CopyFile, filePath, false);
                        LogFileOperationEnd(FileOperationStatus.Skip);
                    }
                    return FileOperationStatus.SkippedCondition;
                }
                condition = null;                                   // 呼び出しもとでもnullの設定が必要
            }

            // 保留中のフォルダ作成を実行
            if (condition == null) {
                for (int i = 0; i < mkdirPendingList.Count; i++) {
                    LogFileOperationStart(FileOperationType.MakeDir, mkdirPendingList[i], true);
                    status = FileBackgroundTaskUtil.DestCreateDirectory(RequestContext, FileProviderDest.DestFileSystem, mkdirPendingList[i]);
                    LogFileOperationEnd(status);
                    modified = true;
                    if (!status.Succeeded) {
                        return status;
                    }
                }
                mkdirPendingList.Clear();
            }

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.Move;
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
