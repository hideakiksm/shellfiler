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
using ShellFiler.FileTask.Provider;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.Properties;
using ShellFiler.Virtual;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ローカル実行でのダウンロードをサポートするためのクラス。
    //=========================================================================================
    class LocalDownloadControler {
        // オーナーとなるタスク
        private AbstractFileBackgroundTask m_task;

        // リクエストコンテキスト
        private FileOperationRequestContext m_context;

        // ダウンロードの転送元ファイル一覧
        private IFileProviderSrc m_fileSrc;

        // ダウンロードの転送先ファイル一覧
        private IFileProviderDest m_fileDest;

        // ダウンロード用の転送ファイルシステム
        private IFileSystemToFileSystem m_transer;

        // 進捗表示用のインターフェース
        private FileProgressEventHandler m_progress;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]task      オーナーとなるタスク
        // 　　　　[in]context   リクエストコンテキスト
        // 　　　　[in]fileSrc   ダウンロードの転送元ファイル一覧
        // 　　　　[in]fileDest  ダウンロードの転送先ファイル一覧
        // 　　　　[in]transfer  ダウンロード用の転送ファイルシステム
        // 　　　　[in]progress  進捗表示用のインターフェース
        // 戻り値：なし
        //=========================================================================================
        public LocalDownloadControler(AbstractFileBackgroundTask task, FileOperationRequestContext context, IFileProviderSrc fileSrc, IFileProviderDest fileDest, IFileSystemToFileSystem transfer, FileProgressEventHandler progress) {
            m_task = task;
            m_context = context;
            m_fileSrc = fileSrc;
            m_fileDest = fileDest;
            m_transer = transfer;
            m_progress = progress;
        }

        //=========================================================================================
        // 機　能：ダウンロードする
        // 引　数：[in]localFileList  転送したローカルファイル情報の一覧を格納するリスト
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus DownloadMarkFiles(List<LocalFileInfo> localFileList) {
            FileOperationStatus status;
            for (int i = 0; i < m_fileSrc.SrcItemCount; i++) {
                SimpleFileDirectoryPath pathInfo = m_fileSrc.GetSrcPath(i);
                string srcPath = pathInfo.FilePath;
                if (!pathInfo.IsDirectory) {
                    // ファイルをコピー
                    string destPath = m_fileDest.DestDirectoryName;
                    status = CopyFile(srcPath, destPath, localFileList);
                    if (!status.Succeeded) {
                        return status;
                    }
                    if (m_task.IsCancel) {
                        return FileOperationStatus.Canceled;
                    }
                } else {
                    // ディレクトリをコピー
                    string destPath = m_fileDest.DestDirectoryName;
                    if (!FileBackgroundTaskUtil.AllowTransfer(m_fileSrc.SrcFileSystem, srcPath, destPath)) {
                        // 転送関係が異常
                        m_task.LogFileOperationStart(FileOperationType.Download, srcPath, true);
                        return m_task.LogFileOperationEnd(FileOperationStatus.SrcDest);
                    }
                    string markDir = m_fileSrc.SrcFileSystem.GetFileName(srcPath);
                    string destTarget = m_fileDest.DestFileSystem.CombineFilePath(destPath, markDir);
                    status = CopyDirectory(srcPath, destTarget, localFileList);
                    if (!status.Succeeded) {
                        return status;
                    }
                    if (m_task.IsCancel) {
                        return FileOperationStatus.Canceled;
                    }
                }
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ファイルをコピーする
        // 引　数：[in]srcFilePath    転送元パス名(C:\SRCBASE\MARKFILE.txt)
        // 　　　　[in]destPath       転送先パス名(D:\DESTBASE\)
        // 　　　　[in]localFileList  転送したローカルファイル情報の一覧を格納するリスト
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus CopyFile(string srcFilePath, string destPath, List<LocalFileInfo> localFileList) {
            string srcFileName = m_fileSrc.SrcFileSystem.GetFileName(srcFilePath);
            string destFilePath = m_fileDest.DestFileSystem.CombineFilePath(destPath, srcFileName);
            m_task.LogFileOperationStart(FileOperationType.Download, srcFilePath, false);
            FileOperationStatus status = m_transer.CopyFile(m_context, srcFilePath, null, destFilePath, false, null, null, m_progress);
            if (status.Succeeded) {
                try {
                    DateTime writeTime = File.GetLastWriteTime(destFilePath);
                    localFileList.Add(new LocalFileInfo(destFilePath, writeTime));
                    status = FileOperationStatus.SuccessDownload;
                } catch (Exception) {
                    status = FileOperationStatus.ErrorGetAttr;
                }
            }
            m_task.LogFileOperationEnd(status);
            return status;
        }

        //=========================================================================================
        // 機　能：ディレクトリをコピーする
        // 引　数：[in]srcPath        転送元パス名(C:\SRCBASE\MARKDIR)
        // 　　　　[in]destPath       転送先パス名(D:\DESTBASE\MARKDIR)
        // 　　　　[in]localFileList  転送したローカルファイル情報の一覧を格納するリスト
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus CopyDirectory(string srcPath, string destPath, List<LocalFileInfo> localFileList) {
            FileOperationStatus status;

            // ディレクトリを作成
            m_task.LogFileOperationStart(FileOperationType.MakeDir, destPath, true);
            status = FileBackgroundTaskUtil.DestCreateDirectory(m_context, m_fileDest.DestFileSystem, destPath);
            m_task.LogFileOperationEnd(status);
            if (!status.Succeeded) {
                return status;
            }

            // ディレクトリをキューに入れる
            List<string> fileList = new List<string>();
            List<string> dirList = new List<string>();
            {
                List<IFile> files = null;
                status = m_fileSrc.SrcFileSystem.GetFileList(m_context, srcPath, out files);
                if (status != FileOperationStatus.Success) {
                    m_task.LogFileOperationStart(FileOperationType.Download, srcPath, true);
                    return m_task.LogFileOperationEnd(FileOperationStatus.CanNotAccess);
                }
                foreach (IFile file in files) {
                    if (file.FileName == "." || file.FileName == "..") {
                        continue;
                    }
                    if (file.Attribute.IsDirectory) {
                        dirList.Add(file.FileName);
                    } else {
                        fileList.Add(file.FileName);
                    }
                    if (m_task.IsCancel) {
                        return FileOperationStatus.Canceled;
                    }
                }
            }

            // ファイルをコピー
            foreach (string fileName in fileList) {
                string srcFilePath = m_fileSrc.SrcFileSystem.CombineFilePath(srcPath, fileName);
                status = CopyFile(srcFilePath, destPath, localFileList);
                if (!status.Succeeded) {
                    return status;
                }
                if (m_task.IsCancel) {
                    return FileOperationStatus.Canceled;
                }
            }

            // ディレクトリを再帰的にコピー
            foreach (string dirName in dirList) {
                string srcDirPath = m_fileSrc.SrcFileSystem.CombineFilePath(srcPath, dirName);
                string destDirPath = m_fileDest.DestFileSystem.CombineFilePath(destPath, dirName);
                status = CopyDirectory(srcDirPath, destDirPath, localFileList);
                if (!status.Succeeded) {
                    return status;
                }
                if (m_task.IsCancel) {
                    return FileOperationStatus.Canceled;
                }
            }
            return FileOperationStatus.Success;
        }
    }
}
