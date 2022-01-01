using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.Management;
using ShellFiler.FileTask.Provider;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイルコピーをバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class CopyBackgroundTask : AbstractFileBackgroundTask {
        // コピーのオプション
        private CopyMoveDeleteOption m_copyOption;

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
        public CopyBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, CopyMoveDeleteOption option, List<IRetryInfo> retryList) : base(srcProvider, destProvider, refreshUi) {
            m_copyOption = option;

            // 再試行情報を作成
            if (m_copyOption.DuplicateFileInfo == null) {
                // ２重化のときは実行しない
                ActivateFileErrorInfo(option);
            }
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
                CopyFilesRetryApi();
                CopyMarkFiles();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：再試行対象のファイルをAPI単位でコピーする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CopyFilesRetryApi() {
            for (int i = 0; i < m_retryList.Count; i++) {
                if (CheckSuspend()) {
                    return;
                }
                m_backgroundTaskPathInfo.SetProgressCount(i + 1, m_retryList.Count);
                FireBackgroundtaskPathInfoProgressEvent();

                RetryInfoSrcDest retryInfo = m_retryList[i];
                LogFileOperationSetMarkInfo(retryInfo.SrcMarkObjectPath);
                List<string> mkdirPendingList = new List<string>();
                CopyFile(null, retryInfo.SrcFilePath, retryInfo.DestFilePath, m_copyOption.TransferCondition, mkdirPendingList, true);
                if (IsCancel) {
                    return;
                }
            }
        }

        //=========================================================================================
        // 機　能：マークされたファイルをコピーする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CopyMarkFiles() {
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
                if (!pathInfo.IsDirectory) {
                    // ファイルをコピー
                    List<string> mkdirPendingList = new List<string>();
                    string destPath = FileProviderDest.DestDirectoryName;
                    CopyFile(null, srcPath, destPath, m_copyOption.TransferCondition, mkdirPendingList, true);
                    if (IsCancel) {
                        return;
                    }
                } else {
                    // ディレクトリをコピー
                    string destPath = FileProviderDest.DestDirectoryName;
                    if (!FileBackgroundTaskUtil.AllowTransfer(FileProviderSrc.SrcFileSystem, srcPath, destPath)) {
                        // 転送関係が異常
                        LogFileOperationStart(FileOperationType.CopyFile, srcPath, true);
                        LogFileOperationEnd(FileOperationStatus.SrcDest);
                    } else {
                        string markDir = FileProviderSrc.SrcFileSystem.GetFileName(srcPath);
                        string destTarget;
                        if (m_copyOption.UnwrapFolder) {
                            // フォルダ階層をフラットにする
                            destTarget = destPath;
                        } else if (m_copyOption.DuplicateFileInfo == null) {
                            // ディレクトリコピー
                            destTarget = FileProviderDest.DestFileSystem.CombineFilePath(destPath, markDir);
                        } else {
                            // ディレクトリの２重化
                            destTarget = FileProviderDest.DestFileSystem.CombineFilePath(destPath, m_copyOption.DuplicateFileInfo.NewFileName);
                        }
                        List<string> mkdirPendingList = new List<string>();
                        CopyDirectory(null, srcPath, destTarget, m_copyOption.TransferCondition, mkdirPendingList);
                    }
                    if (IsCancel) {
                        return;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイルをコピーする
        // 引　数：[in]srcFileInfo      取得済みの転送元ファイル情報（まだ取得していないときはnull）
        // 　　　　[in]srcFilePath      転送元パス名(C:\SRCBASE\DIR\MARKFILE.txt)
        // 　　　　[in]destPath         転送先パス名(D:\DESTBASE\)
        // 　　　　[in]condition        削除条件（無条件に削除するときnull）
        // 　　　　[in]mkdirPendingList ディレクトリ作成を保留しているディレクトリのリスト
        // 　　　　[in]rootFile         ルートレベルの位置にあるファイルであるときtrue
        // 戻り値：フォルダ内に修正を加えたときtrue
        //=========================================================================================
        private bool CopyFile(IFile srcFileInfo, string srcFilePath, string destPath, CompareCondition condition, List<string> mkdirPendingList, bool rootLevel) {
            if (CheckSuspend()) {
                return false;
            }

            FileOperationStatus status;
            string srcFileName = FileProviderSrc.SrcFileSystem.GetFileName(srcFilePath);
            string destFilePath;
            if (rootLevel && m_copyOption.DuplicateFileInfo != null) {
                destFilePath = FileProviderDest.DestFileSystem.CombineFilePath(destPath, m_copyOption.DuplicateFileInfo.NewFileName);
            } else {
                destFilePath = FileProviderDest.DestFileSystem.CombineFilePath(destPath, srcFileName);
            }

            // 属性を確認
            bool folderModify = false;
            if (condition != null) {
                // 転送条件ありの場合は属性を取得
                status = CheckFileAttribute(srcFilePath, false, ref srcFileInfo, condition, mkdirPendingList, ref folderModify);
                if (status == FileOperationStatus.SkippedCondition) {
                    return false;           // ここで戻る場合は修正を加えていない
                } else if (!status.Succeeded) {
                    return true;
                }
            }

            // ファイルのコピーを実行
            // これより下で失敗した場合のみAPIレベルでの再試行が可能
            FileOperationType fileOperationType = (m_copyOption.DuplicateFileInfo == null) ? FileOperationType.CopyFile : FileOperationType.DuplicateFile;
            LogFileOperationStart(fileOperationType, srcFilePath, false);
            status = FileSystemToFileSystem.CopyFile(RequestContext, srcFilePath, srcFileInfo, destFilePath, false, m_copyOption.AttributeSetMode, m_copyOption.FileFilter, new FileProgressEventHandler(ProgressEventHandler));

            // すでに存在する場合は同名ファイルの処理
            if (status == FileOperationStatus.AlreadyExists) {
                SameNameFileTransfer.TransferDelegate operation = delegate(FileOperationRequestContext context, string srcFile, string destFile, bool overwrite) {
                    status = FileSystemToFileSystem.CopyFile(context, srcFile, srcFileInfo, destFile, overwrite, m_copyOption.AttributeSetMode, m_copyOption.FileFilter, new FileProgressEventHandler(ProgressEventHandler));
                    return status;
                };
                SameNameFileTransfer sameFile = new SameNameFileTransfer(this, RequestContext, FileProviderSrc, FileProviderDest, SameNameFileTransfer.TransferMode.CopySameFile, m_copyOption.SameFileOperation, operation);
                SameNameTargetFileDetail fileDetail = new SameNameTargetFileDetail(RequestContext, FileProviderSrc.SrcFileSystem, FileProviderDest.DestFileSystem, srcFilePath, destFilePath);
                try {
                    status = sameFile.TransferSameFile(fileDetail);
                    if (status == FileOperationStatus.Canceled) {
                        SetCancel(CancelReason.User);
                    }
                } finally {
                    fileDetail.Dispose();
                }
                m_copyOption.SameFileOperation = sameFile.SameFileOperation;
            }

            // ステータスを更新
            if (!status.Succeeded) {
                LogFileOperationEnd(status, new RetryInfoSrcDest(FileOperationApiType.FileCopy, srcFilePath, destPath));
            } else {
                LogFileOperationEnd(status);
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ディレクトリをコピーする
        // 引　数：[in]srcFileInfo      取得済みの転送元ファイル情報（まだ取得していないときはnull）
        // 　　　　[in]srcPath          転送元パス名(C:\SRCBASE\MARKDIR)
        // 　　　　[in]destPath         転送先パス名(D:\DESTBASE\MARKDIR)
        // 　　　　[in]condition        削除条件（無条件に削除するときnull）
        // 　　　　[in]mkdirPendingList ディレクトリ作成を保留しているディレクトリのリスト
        // 戻り値：ディレクトリに変更を行ったときnull
        //=========================================================================================
        private bool CopyDirectory(IFile srcFileInfo, string srcPath, string destPath, CompareCondition condition, List<string> mkdirPendingList) {
            FileOperationStatus status;
            if (CheckSuspend()) {
                return false;
            }

            bool isPending = false;             // 条件付き転送でサブディレクトリの作成を保留にしているときtrue
            bool folderModified = false;        // このフォルダに変更を加えたときtrue
            // Dir1
            //   Dir2
            //     File1
            // File1を転送することがわかるまで、Dir1とDir2の作成は保留になる。
            // 再帰1回に対して、フォルダ1個をmkdirPendingListに追加し、条件を満たしているファイルがあった場合は
            // 全フォルダを作成後、mkdirPendingListをクリアする。
            // 条件を満たしていなかった場合はその再帰ループで登録したディレクトリをmkdirPendingListから除去する。
            try {
                if (condition != null) {
                    // フォルダのコピーの可否が不明なとき、フォルダ作成の予約
                    isPending = true;
                    mkdirPendingList.Add(destPath);

                    // フォルダの情報を取得
                    status = CheckFileAttribute(srcPath, true, ref srcFileInfo, condition, mkdirPendingList, ref folderModified);
                    if (!status.Succeeded) {
                        return folderModified;
                    } else if (status != FileOperationStatus.SkippedCondition) {
                        condition = null;                // このフォルダが条件に該当する場合は、配下を無条件にコピーする
                    }
                } else {
                    // 一括コピー（条件がnullのときのみ可能）
                    bool supported = DirectRecursiveCopy(srcPath, destPath, ref folderModified);
                    if (supported) {
                        // ここで戻る場合、AttributeSetModeの設定の仕様により属性コピーを行う必要はない
                        return folderModified;
                    }

                    // フォルダをコピーしてよいとき
                    // ディレクトリを作成
                    if (!m_copyOption.UnwrapFolder) {
                        LogFileOperationStart(FileOperationType.MakeDir, destPath, true);
                        status = FileBackgroundTaskUtil.DestCreateDirectory(RequestContext, FileProviderDest.DestFileSystem, destPath);
                        LogFileOperationEnd(status);
                        if (!status.Succeeded) {
                            return folderModified;
                        }
                        if (status != FileOperationStatus.AlreadyExists) {
                            folderModified = true;
                        }
                    }
                }

                // ディレクトリをキューに入れる
                List<IFile> fileList, dirList;
                status = BackgroundTaskCommandUtil.GetFileList(FileProviderSrc.SrcFileSystem, RequestContext, srcPath, out fileList, out dirList);
                if (!status.Succeeded) {
                    LogFileOperationStart(FileOperationType.CopyDir, srcPath, true);
                    LogFileOperationEnd(status);
                    return folderModified;
                }

                // ファイルをコピー
                for (int i = 0; i < fileList.Count; i++) {
                    IFile file = fileList[i];
                    string srcFilePath = FileProviderSrc.SrcFileSystem.CombineFilePath(srcPath, file.FileName);
                    folderModified |= CopyFile(file, srcFilePath, destPath, condition, mkdirPendingList, false);
                    if (IsCancel) {
                        return folderModified;
                    }
                    fileList[i] = null;
                }

                // ディレクトリを再帰的にコピー
                for (int i = 0; i < dirList.Count; i++) {
                    IFile file = dirList[i];
                    string srcDirPath = FileProviderSrc.SrcFileSystem.CombineFilePath(srcPath, file.FileName);
                    string destDirPath;
                    if (m_copyOption.UnwrapFolder) {
                        // フォルダ階層をフラットにする
                        destDirPath = destPath;
                    } else {
                        // 通常コピー
                        destDirPath = FileProviderDest.DestFileSystem.CombineFilePath(destPath, file.FileName);
                    }
                    folderModified |= CopyDirectory(file, srcDirPath, destDirPath, condition, mkdirPendingList);
                    if (IsCancel) {
                        return folderModified;
                    }
                    dirList[i] = null;
                }

                if (m_copyOption.UnwrapFolder) {
                    folderModified = false;
                }
                return folderModified;
            } finally {
                if (isPending && mkdirPendingList.Count > 0) {
                    mkdirPendingList.RemoveAt(mkdirPendingList.Count - 1);
                    LogFileOperationStart(FileOperationType.CopyDir, srcPath, true);
                    LogFileOperationEnd(FileOperationStatus.Skip);
                }
                // 属性をコピー
                if (!IsCancel && folderModified && !m_copyOption.UnwrapFolder) {
                    LogFileOperationStart(FileOperationType.CopyAttr, srcPath, true);
                    status = FileSystemToFileSystem.CopyFileInfo(RequestContext, true, srcPath, srcFileInfo, destPath, m_copyOption.AttributeSetMode);
                    LogFileOperationEnd(status);
                }
            }
        }

        //=========================================================================================
        // 機　能：フォルダ内のファイルを一括転送する
        // 引　数：[in]srcPath             コピー元フォルダ
        // 　　　　[in]destPath            コピー先フォルダ
        // 　　　　[in,out]folderModified  フォルダが更新されたときtrueを返す変数
        // 戻り値：処理を継続してよいときtrue
        //=========================================================================================
        private bool DirectRecursiveCopy(string srcPath, string destPath, ref bool folderModified) {
            // 一括転送許可かどうかを確認
            if (m_copyOption.FileFilter != null) {
                return false;
            }

            // 一括コピー可能かどうかを確認
            if (!FileSystemToFileSystem.CanMoveDirectoryDirect(srcPath, destPath, true)) {
                return false;
            }

            // 一括転送開始
            LogFileOperationStart(FileOperationType.CopyDir, srcPath, true);
            FileOperationStatus status = FileSystemToFileSystem.CopyDirectoryDirect(RequestContext, srcPath, destPath, m_copyOption.AttributeSetMode, new FileProgressEventHandler(ProgressEventHandler));
            LogFileOperationEnd(status);
            if (status == FileOperationStatus.CopyRetry) {
                return false;
            }
            folderModified = true;
            return true;
        }
        
        //=========================================================================================
        // 機　能：ファイル属性を確認する
        // 引　数：[in]filePath         削除対象のファイル/ディレクトリ名(C:\SRCBASE\MARKFILE.txt)
        // 　　　　[in]isDir            ディレクトリを削除するときtrue
        // 　　　　[in]fileInfo         取得済みのファイル情報（まだ取得していないときはnull）
        // 　　　　[in]condition        削除条件（無条件に削除するときnull）
        // 　　　　[in]mkdirPendingList ディレクトリ作成を保留しているディレクトリのリスト
        // 　　　　[in,out]modified     フォルダ内データが更新されたときtrueを返す変数
        // 戻り値：ステータス（削除を開始してよいときSuccess、条件不一致で対象外となったときSkippedCondition、開始できないときはその理由）
        //=========================================================================================
        private FileOperationStatus CheckFileAttribute(string filePath, bool isDir, ref IFile fileInfo, CompareCondition condition, List<string> mkdirPendingList, ref bool modified) {
            FileOperationStatus status;
            FileOperationType logType = (isDir ? FileOperationType.CopyDir : FileOperationType.CopyFile);

            // 属性を取得
            if (fileInfo == null) {
                status = FileProviderSrc.SrcFileSystem.GetFileInfo(RequestContext, filePath, true, out fileInfo);
                if (!status.Succeeded) {
                    LogFileOperationStart(logType, filePath, isDir);
                    LogFileOperationEnd(status);
                    return status;
                }
            }

            // 削除条件を確認
            if (condition != null) {
                bool target = TargetConditionComparetor.IsTarget(condition, fileInfo, true);
                if (!target) {
                    if (!isDir) {           // ファイルのみスキップログを出力
                        LogFileOperationStart(FileOperationType.CopyFile, filePath, false);
                        LogFileOperationEnd(FileOperationStatus.Skip);
                    }
                    return FileOperationStatus.SkippedCondition;
                }
                condition = null;           // 呼び出しもとでもnullの設定が必要
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
                return BackgroundTaskType.Copy;
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
