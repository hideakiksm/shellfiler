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
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ローカル実行でのアップロードをサポートするためのクラス。
    //=========================================================================================
    class LocalUploadControler {
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
        
        // 同名ファイルを発見したときの動作
        private SameFileOperation m_sameFileOperation;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]task      オーナーとなるタスク
        // 　　　　[in]fileSrc   ダウンロードの転送元ファイル一覧
        // 　　　　[in]fileDest  ダウンロードの転送先ファイル一覧
        // 　　　　[in]transfer  ダウンロード用の転送ファイルシステム
        // 　　　　[in]progress  進捗表示用のインターフェース
        // 戻り値：なし
        //=========================================================================================
        public LocalUploadControler(AbstractFileBackgroundTask task, FileOperationRequestContext context, IFileProviderSrc fileSrc, IFileProviderDest fileDest, IFileSystemToFileSystem transfer, FileProgressEventHandler progress) {
            m_task = task;
            m_context = context;
            m_fileSrc = fileSrc;
            m_fileDest = fileDest;
            m_transer = transfer;
            m_progress = progress;
            m_sameFileOperation = SameFileOperation.CreateWithDefaultConfig(fileDest.DestFileSystem.FileSystemId);
            m_sameFileOperation.AllApply = false;
        }

        //=========================================================================================
        // 機　能：アップロードする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void UploadFiles() {
            for (int i = 0; i < m_fileSrc.SrcItemCount; i++) {
                SimpleFileDirectoryPath pathInfo = m_fileSrc.GetSrcPath(i);
                string srcPath = pathInfo.FilePath;
                if (!pathInfo.IsDirectory) {
                    // ファイルをコピー
                    string destPath = m_fileDest.DestDirectoryName;
                    CopyFile(srcPath, destPath);
                    if (m_task.IsCancel) {
                        return;
                    }
                } else {
                    // ディレクトリをコピー
                    string destPath = m_fileDest.DestDirectoryName;
                    if (!FileBackgroundTaskUtil.AllowTransfer(m_fileSrc.SrcFileSystem, srcPath, destPath)) {
                        // 転送関係が異常
                        m_task.LogFileOperationStart(FileOperationType.CopyFile, srcPath, true);
                        m_task.LogFileOperationEnd(FileOperationStatus.SrcDest);
                    } else {
                        string markDir = m_fileSrc.SrcFileSystem.GetFileName(srcPath);
                        string destTarget = m_fileDest.DestFileSystem.CombineFilePath(destPath, markDir);
                        CopyDirectory(srcPath, destTarget);
                    }
                    if (m_task.IsCancel) {
                        return;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイルをコピーする
        // 引　数：[in]srcFilePath  転送元パス名(C:\SRCBASE\MARKFILE.txt)
        // 　　　　[in]destPath     転送先パス名(D:\DESTBASE\)
        // 戻り値：なし
        //=========================================================================================
        private void CopyFile(string srcFilePath, string destPath) {
            string srcFileName = m_fileSrc.SrcFileSystem.GetFileName(srcFilePath);
            string destFilePath = m_fileDest.DestFileSystem.CombineFilePath(destPath, srcFileName);
            m_task.LogFileOperationStart(FileOperationType.CopyFile, srcFilePath, false);
            FileOperationStatus status = m_transer.CopyFile(m_context, srcFilePath, null, destFilePath, false, null, null, m_progress);
            if (status == FileOperationStatus.AlreadyExists) {
                SameNameFileTransfer.TransferDelegate operation = delegate(FileOperationRequestContext context, string srcFile, string destFile, bool overwrite) {
                    status = m_transer.CopyFile(context, srcFile, null, destFile, overwrite, null, null, m_progress);
                    return status;
                };
                SameNameFileTransfer sameFile = new SameNameFileTransfer(m_task, m_context, m_fileSrc, m_fileDest, SameNameFileTransfer.TransferMode.CopySameFile, m_sameFileOperation, operation);
                SameNameTargetFileDetail fileDetail = new SameNameTargetFileDetail(m_context, m_fileSrc.SrcFileSystem, m_fileDest.DestFileSystem, srcFilePath, destFilePath);
                try {
                    status = sameFile.TransferSameFile(fileDetail);
                    m_sameFileOperation = sameFile.SameFileOperation;
                } finally {
                    fileDetail.Dispose();
                }
            }
            if (status == FileOperationStatus.SuccessCopy) {
                m_task.LogFileOperationEnd(FileOperationStatus.SuccessUpload);
            } else {
                m_task.LogFileOperationEnd(status);
            }
        }

        //=========================================================================================
        // 機　能：ディレクトリをコピーする
        // 引　数：[in]srcPath    転送元パス名(C:\SRCBASE\MARKDIR)
        // 　　　　[in]destPath   転送先パス名(D:\DESTBASE\MARKDIR)
        // 戻り値：なし
        //=========================================================================================
        private void CopyDirectory(string srcPath, string destPath) {
            FileOperationStatus status;
            
            // ディレクトリを作成
            m_task.LogFileOperationStart(FileOperationType.MakeDir, destPath, true);
            status = FileBackgroundTaskUtil.DestCreateDirectory(m_context, m_fileDest.DestFileSystem, destPath);
            m_task.LogFileOperationEnd(status);
            if (!status.Succeeded) {
                return;
            }

            // ディレクトリをキューに入れる
            List<string> fileList = new List<string>();
            List<string> dirList = new List<string>();
            {
                List<IFile> files = null;
                status = m_fileSrc.SrcFileSystem.GetFileList(m_context, srcPath, out files);
                if (status != FileOperationStatus.Success) {
                    m_task.LogFileOperationStart(FileOperationType.CopyFile, srcPath, true);
                    m_task.LogFileOperationEnd(FileOperationStatus.CanNotAccess);
                    return;
                }
                foreach (IFile file in files) {
                    if (file.FileName == "." || file.FileName == "..") {
                        continue;
                    }
                    if (!file.Attribute.IsDirectory) {
                        fileList.Add(file.FileName);
                    } else {
                        dirList.Add(file.FileName);
                    }
                    if (m_task.IsCancel) {
                        return;
                    }
                }
            }

            // ファイルをコピー
            foreach (string fileName in fileList) {
                string srcFilePath = m_fileSrc.SrcFileSystem.CombineFilePath(srcPath, fileName);
                CopyFile(srcFilePath, destPath);
                if (m_task.IsCancel) {
                    return;
                }
            }

            // ディレクトリを再帰的にコピー
            foreach (string dirName in dirList) {
                string srcDirPath = m_fileSrc.SrcFileSystem.CombineFilePath(srcPath, dirName);
                string destDirPath = m_fileDest.DestFileSystem.CombineFilePath(destPath, dirName);
                CopyDirectory(srcDirPath, destDirPath);
                if (m_task.IsCancel) {
                    return;
                }
            }
        }
    }
}
