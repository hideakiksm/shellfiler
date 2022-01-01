using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.FileTask.Management;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.Properties;
using ShellFiler.Virtual;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ローカルによるファイルの編集結果をバックグラウンドでアップロードするクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    // 　　　　転送元は必ずWindowsファイルシステムとなる
    //=========================================================================================
    class LocalUploadBackgroundTask : AbstractFileBackgroundTask {
        // ダウンロード用のテンポラリ空間
        private LocalExecuteTemporarySpace m_temporarySpace;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider   転送元ファイルの情報（Windowsファイルシステム）
        // 　　　　[in]destProvider  転送先ファイルの情報
        // 　　　　[in]refreshUi     作業完了時のUI更新方法
        // 　　　　[in]space         作業領域の情報
        // 戻り値：なし
        //=========================================================================================
        public LocalUploadBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, LocalExecuteTemporarySpace space) : base(srcProvider, destProvider, refreshUi) {
            m_temporarySpace = space;
            CreateBackgroundTaskPathInfo();
        }

        //=========================================================================================
        // 機　能：転送元/転送先の情報を作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CreateBackgroundTaskPathInfo() {
            // 転送元
            string srcShort = "temp";
            string srcDetail = FileProviderSrc.GetSrcPath(0).FilePath;

            // 転送先
            string destShort = GenericFileStringUtils.GetFileName(FileProviderDest.DestDirectoryName);
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
            SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(0);
            FileProviderSrc.SrcFileSystem.BeginFileOperation(RequestContext, pathInfo.FilePath);
            FileProviderDest.DestFileSystem.BeginFileOperation(RequestContext, FileProviderDest.DestDirectoryName);

            try {
                StartExecute();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                if (FailCount == 0) {
                    Program.Document.TemporaryManager.DeleteLocalExecuteSpace(m_temporarySpace);
                    m_temporarySpace = null;
                }
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：アップロードを開始する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void StartExecute() {
            for (int i = 0; i < FileProviderSrc.SrcItemCount; i++) {
                SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(i);
                string srcPath = pathInfo.FilePath;
                if (!pathInfo.IsDirectory) {
                    // ファイルをコピー
                    string destPath = FileProviderDest.DestDirectoryName;
                    CopyFile(srcPath, destPath);
                    if (IsCancel) {
                        return;
                    }
                } else {
                    // ディレクトリをコピー
                    string destPath = FileProviderDest.DestDirectoryName;
                    if (!FileBackgroundTaskUtil.AllowTransfer(FileProviderSrc.SrcFileSystem, srcPath, destPath)) {
                        // 転送関係が異常
                        LogFileOperationStart(FileOperationType.Download, srcPath, true);
                        LogFileOperationEnd(FileOperationStatus.SrcDest);
                    } else {
                        string markDir = FileProviderSrc.SrcFileSystem.GetFileName(srcPath);
                        string destTarget = FileProviderDest.DestFileSystem.CombineFilePath(destPath, markDir);
                        CopyDirectory(srcPath, destTarget);
                    }
                    if (IsCancel) {
                        return;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイルをコピーする
        // 引　数：[in]srcFilePath  転送元パス名(C:\SRCBASE\MARKFILE.txt)
        // 　　　　[in]destPath     転送先パス名(D:\DESTBASE\)
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus CopyFile(string srcFilePath, string destPath) {
            string srcFileName = FileProviderSrc.SrcFileSystem.GetFileName(srcFilePath);
            string destFilePath = FileProviderDest.DestFileSystem.CombineFilePath(destPath, srcFileName);
            LogFileOperationStart(FileOperationType.Upload, srcFilePath, false);

            // 最終更新時刻を比較
            try {
                DateTime writeTime = File.GetLastWriteTime(srcFilePath);
                if (writeTime <= m_temporarySpace.StartTime) {
                    return LogFileOperationEnd(FileOperationStatus.Skip);
                }
            } catch (Exception) {
                return LogFileOperationEnd(FileOperationStatus.ErrorGetAttr);
            }

            // ファイルをコピー
            List<LocalFileInfo> fileList= m_temporarySpace.LocalFileList;
            bool overrite = false;
            foreach (LocalFileInfo fileInfo in fileList) {
                if (string.Compare(fileInfo.FilePath, srcFilePath, true) == 0) {
                    overrite = true;            // ダウンロードしたファイルは上書き
                }
            }
            FileOperationStatus status = FileSystemToFileSystem.CopyFile(RequestContext, srcFilePath, null, destFilePath, overrite, null, null, new FileProgressEventHandler(ProgressEventHandler));
            if (status.Succeeded) {
                status = FileOperationStatus.SuccessUpload;
            }
            LogFileOperationEnd(status);
            return status;
        }

        //=========================================================================================
        // 機　能：ディレクトリをコピーする
        // 引　数：[in]srcPath    転送元パス名(C:\SRCBASE\MARKDIR)
        // 　　　　[in]destPath   転送先パス名(D:\DESTBASE\MARKDIR)
        // 戻り値：ステータス
        //=========================================================================================
        private void CopyDirectory(string srcPath, string destPath) {
            FileOperationStatus status;

            // ディレクトリを作成
            LogFileOperationStart(FileOperationType.MakeDir, destPath, true);
            status = FileBackgroundTaskUtil.DestCreateDirectory(RequestContext, FileProviderDest.DestFileSystem, destPath);
            LogFileOperationEnd(status);
            if (!status.Succeeded) {
                return;
            }

            // ディレクトリをキューに入れる
            List<string> fileList = new List<string>();
            List<string> dirList = new List<string>();
            {
                List<IFile> files = null;
                status = FileProviderSrc.SrcFileSystem.GetFileList(RequestContext, srcPath, out files);
                if (status != FileOperationStatus.Success) {
                    LogFileOperationStart(FileOperationType.Download, srcPath, true);
                    LogFileOperationEnd(FileOperationStatus.CanNotAccess);
                    return;
                }
                foreach (IFile file in files) {
                    if (file.FileName == "." || file.FileName == "..") {
                        continue;
                    }
                    if (!file.Attribute.IsDirectory || file.Attribute.IsSymbolicLink) {
                        fileList.Add(file.FileName);
                    } else {
                        dirList.Add(file.FileName);
                    }
                    if (IsCancel) {
                        return;
                    }
                }
            }

            // ファイルをコピー
            foreach (string fileName in fileList) {
                string srcFilePath = FileProviderSrc.SrcFileSystem.CombineFilePath(srcPath, fileName);
                CopyFile(srcFilePath, destPath);
                if (IsCancel) {
                    return;
                }
            }

            // ディレクトリを再帰的にコピー
            foreach (string dirName in dirList) {
                string srcDirPath = FileProviderSrc.SrcFileSystem.CombineFilePath(srcPath, dirName);
                string destDirPath = FileProviderDest.DestFileSystem.CombineFilePath(destPath, dirName);
                CopyDirectory(srcDirPath, destDirPath);
                if (IsCancel) {
                    return;
                }
            }
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.LocalUpload;
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
