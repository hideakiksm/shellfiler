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
    // クラス：ファイルのミラーコピーをバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class MirrorCopyBackgroundTask : AbstractFileBackgroundTask {
        // ミラーコピーのオプション
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
        // 　　　　[in]retryList    再試行するAPIのリスト（再試行しないときnull、nullのみサポート）
        // 戻り値：なし
        //=========================================================================================
        public MirrorCopyBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, CopyMoveDeleteOption option, List<IRetryInfo> retryList) : base(srcProvider, destProvider, refreshUi) {
            m_copyOption = option;

            // 再試行情報を作成
            ActivateFileErrorInfo(option);
            m_retryList = new List<RetryInfoSrcDest>();
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
                CopyMarkFiles();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
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
                    string destPath = FileProviderDest.DestDirectoryName;
                    CopyFile(null, srcPath, destPath);
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
                        string destTarget = FileProviderDest.DestFileSystem.CombineFilePath(destPath, markDir);
                        CopyDirectory(null, srcPath, destTarget);
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
        // 戻り値：フォルダ内に修正を加えたときtrue
        //=========================================================================================
        private bool CopyFile(IFile srcFileInfo, string srcFilePath, string destPath) {
            if (!CheckTargetName(srcFilePath, false)) {
                return false;
            }
            if (CheckSuspend()) {
                return false;
            }

            FileOperationStatus status;
            string srcFileName = FileProviderSrc.SrcFileSystem.GetFileName(srcFilePath);
            string destFilePath = FileProviderDest.DestFileSystem.CombineFilePath(destPath, srcFileName);

            // 属性を確認
            bool overwrite = false;
            MirrorCopyOption.MirrorCopyTransferMode transferMode = m_copyOption.MirrorCopyOption.TransferMode;
            if (transferMode == MirrorCopyOption.MirrorCopyTransferMode.OverwriteIfNewer || transferMode == MirrorCopyOption.MirrorCopyTransferMode.DifferenceAttribute) {
                // 属性比較ありの場合は属性を取得
                IFile destFileInfo = null;
                status = CheckFileAttribute(transferMode, srcFilePath, ref srcFileInfo, destFilePath, ref destFileInfo);
                if (status != FileOperationStatus.Success) {
                    return false;
                }
                overwrite = true;
            } else if (transferMode == MirrorCopyOption.MirrorCopyTransferMode.ForceOverwrite) {
                overwrite = true;
            }

            // ファイルのコピーを実行
            // これより下で失敗した場合のみAPIレベルでの再試行が可能
            LogFileOperationStart(FileOperationType.CopyFile, srcFilePath, false);
            status = FileSystemToFileSystem.CopyFile(RequestContext, srcFilePath, srcFileInfo, destFilePath, overwrite, m_copyOption.MirrorCopyOption.AttributeSetMode, null, new FileProgressEventHandler(ProgressEventHandler));

            // すでに存在する場合、ミラーコピーではスキップ
            if (status == FileOperationStatus.AlreadyExists) {
                status = FileOperationStatus.Skip;
            }

            // ステータスを更新
            LogFileOperationEnd(status);
            return true;
        }

        //=========================================================================================
        // 機　能：ディレクトリをコピーする
        // 引　数：[in]srcFileInfo      取得済みの転送元ファイル情報（まだ取得していないときはnull）
        // 　　　　[in]srcPath          転送元パス名(C:\SRCBASE\MARKDIR)
        // 　　　　[in]destPath         転送先パス名(D:\DESTBASE\MARKDIR)
        // 戻り値：ディレクトリに変更を行ったときnull
        //=========================================================================================
        private bool CopyDirectory(IFile srcFileInfo, string srcPath, string destPath) {
            FileOperationStatus status;
            if (!CheckTargetName(srcPath, true)) {
                return false;
            }
            if (CheckSuspend()) {
                return false;
            }

            bool folderModified = false;        // このフォルダに変更を加えたときtrue
            try {
                // ディレクトリを作成
                LogFileOperationStart(FileOperationType.MakeDir, destPath, true);
                status = FileBackgroundTaskUtil.DestCreateDirectory(RequestContext, FileProviderDest.DestFileSystem, destPath);
                LogFileOperationEnd(status);
                if (!status.Succeeded) {
                    return folderModified;
                }
                if (status != FileOperationStatus.AlreadyExists) {
                    folderModified = true;
                }

                // ディレクトリをキューに入れる
                List<IFile> fileList, dirList;
                status = BackgroundTaskCommandUtil.GetFileList(FileProviderSrc.SrcFileSystem, RequestContext, srcPath, out fileList, out dirList);
                if (!status.Succeeded) {
                    LogFileOperationStart(FileOperationType.CopyDir, srcPath, true);
                    LogFileOperationEnd(status);
                    return folderModified;
                }

                // 転送先の一覧を作成
                DestFolderList destFolderList = GetDestFileList(destPath);
                if (IsCancel) {
                    return folderModified;
                }

                // ファイルをコピー
                for (int i = 0; i < fileList.Count; i++) {
                    IFile file = fileList[i];
                    string srcFilePath = FileProviderSrc.SrcFileSystem.CombineFilePath(srcPath, file.FileName);
                    folderModified |= CopyFile(file, srcFilePath, destPath);
                    if (IsCancel) {
                        return folderModified;
                    }
                    fileList[i] = null;
                    destFolderList.SetFileUsed(file.FileName);
                }

                // ディレクトリを再帰的にコピー
                for (int i = 0; i < dirList.Count; i++) {
                    IFile file = dirList[i];
                    string srcDirPath = FileProviderSrc.SrcFileSystem.CombineFilePath(srcPath, file.FileName);
                    string destDirPath = FileProviderDest.DestFileSystem.CombineFilePath(destPath, file.FileName);
                    folderModified |= CopyDirectory(file, srcDirPath, destDirPath);
                    if (IsCancel) {
                        return folderModified;
                    }
                    dirList[i] = null;
                    destFolderList.SetFolderUsed(file.FileName);
                }

                // 転送先から未参照ファイルを削除
                folderModified |= DeleteDestFileFolder(destPath, destFolderList);
                if (IsCancel) {
                    return folderModified;
                }

                return folderModified;
            } finally {
                // 属性をコピー
                if (!IsCancel && folderModified) {
                    LogFileOperationStart(FileOperationType.CopyAttr, srcPath, true);
                    status = FileSystemToFileSystem.CopyFileInfo(RequestContext, true, srcPath, srcFileInfo, destPath, m_copyOption.MirrorCopyOption.AttributeSetMode);
                    LogFileOperationEnd(status);
                }
            }
        }

        //=========================================================================================
        // 機　能：転送先にあるファイル一覧を取得する
        // 引　数：[in]destPath    転送先パス名(D:\DESTBASE\MARKDIR)
        // 戻り値：転送先のファイル一覧
        //=========================================================================================
        private DestFolderList GetDestFileList(string destPath) {
            FileOperationStatus status;
            List<IFile> fileListAll;
            status = FileProviderDest.DestFileSystem.GetFileList(RequestContext, destPath, out fileListAll);
            if (!status.Succeeded) {
                // 対象パスが取得できない場合は転送先に削除候補がなし
                return null;
            }

            // 一覧を変換
            DestFolderList destFolderList = new DestFolderList(FileProviderDest.DestFileSystem.FileSystemId);
            for (int i = 0; i < fileListAll.Count; i++) {
                destFolderList.AddFile(fileListAll[i]);
            }
            return destFolderList;
        }

        //=========================================================================================
        // 機　能：転送先にしかないファイルやフォルダを削除する
        // 引　数：[in]destPath        転送先パス名(D:\DESTBASE\MARKDIR)
        // 　　　　[in]destFolderList  転送先のファイル一覧で、処理済みファイルをチェックしたもの
        // 戻り値：フォルダに変更を加えたときtrue
        //=========================================================================================
        private bool DeleteDestFileFolder(string destPath, DestFolderList destFolderList) {
            FileOperationStatus status;
            DeleteBackgroundTask.DeleteWorker worker = new DeleteBackgroundTask.DeleteWorker(
                    new DeleteBackgroundTask.TaskControler(this), FileProviderDest.DestFileSystem, RequestContext, false,
                    m_copyOption.MirrorCopyOption.DeleteFileOption, m_copyOption.MirrorCopyOption.UseRecycleBin);
            bool folderModified = false;
            
            // 未参照のファイルを削除
            List<IFile> unrefFile = destFolderList.GetUnrefFile();
            for (int i = 0 ; i < unrefFile.Count; i++) {
                string destFilePath = FileProviderDest.DestFileSystem.CombineFilePath(destPath, unrefFile[i].FileName);
                folderModified |= worker.DeleteFile(destFilePath, unrefFile[i], null);
                if (IsCancel) {
                    return folderModified;
                }
            }

            // 未参照のフォルダを削除
            List<IFile> unrefFolder = destFolderList.GetUnrefFolder();
            for (int i = 0 ; i < unrefFolder.Count; i++) {
                string destDirPath = FileProviderDest.DestFileSystem.CombineFilePath(destPath, unrefFolder[i].FileName);
                status = worker.DeleteDirectory(destDirPath, unrefFolder[i], null);
                LogFileOperationStart(FileOperationType.DeleteDir, destDirPath, true);
                LogFileOperationEnd(status);
                if (!status.Succeeded || status == FileOperationStatus.Skip) {
                    ;
                } else {
                    folderModified = true;
                }
                if (IsCancel) {
                    return folderModified;
                }
            }
            return folderModified;
        }

        //=========================================================================================
        // クラス：転送先の処理済みファイル一覧情報の管理クラス
        //=========================================================================================
        class DestFolderList {
            // 転送先のファイル一覧（Windowsは小文字、処理済みのものを削除）
            private Dictionary<string, int> m_destFileFlag = new Dictionary<string, int>();

            // 転送先のファイル一覧（オリジナル、処理済みのものはnull）
            private List<IFile> m_destFileListOrg = new List<IFile>();

            // 転送先のフォルダ一覧（Windowsは小文字、処理済みのものを削除）
            private Dictionary<string, int> m_destFolderFlag = new Dictionary<string, int>();

            // 転送先のフォルダ一覧（オリジナル、処理済みのものはnull）
            private List<IFile> m_destFolderListOrg = new List<IFile>();

            // 転送先で大文字小文字を無視するときtrue
            private bool m_destFileIgnoreCase;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]destFileSystem   転送先のファイルシステム
            // 戻り値：なし
            //=========================================================================================
            public DestFolderList(FileSystemID destFileSystem) {
                m_destFileIgnoreCase = FileSystemID.IgnoreCaseFolderPath(destFileSystem);
            }

            //=========================================================================================
            // 機　能：転送先の初期ファイルを追加して初期化する
            // 引　数：[in]file   転送先のファイル情報
            // 戻り値：なし
            //=========================================================================================
            public void AddFile(IFile file) {
                string fileName = file.FileName;
                if (fileName == ".." || fileName == ".") {
                    return;
                }
                if (m_destFileIgnoreCase) {
                    fileName = fileName.ToLower();
                }
                if (!file.Attribute.IsDirectory || file.Attribute.IsSymbolicLink) {
                    if (m_destFileFlag.ContainsKey(fileName)) {
                        return;
                    }
                    m_destFileFlag.Add(fileName, m_destFileListOrg.Count);
                    m_destFileListOrg.Add(file);
                } else {
                    if (m_destFolderFlag.ContainsKey(fileName)) {
                        return;
                    }
                    m_destFolderFlag.Add(fileName, m_destFolderListOrg.Count);
                    m_destFolderListOrg.Add(file);
                }
            }

            //=========================================================================================
            // 機　能：ファイルが使用されたマークを付ける
            // 引　数：[in]file   使用したファイルのファイル名
            // 戻り値：なし
            //=========================================================================================
            public void SetFileUsed(string file) {
                if (m_destFileIgnoreCase) {
                    file = file.ToLower();
                }
                if (m_destFileFlag.ContainsKey(file)) {
                    m_destFileListOrg[m_destFileFlag[file]] = null;
                }
            }

            //=========================================================================================
            // 機　能：フォルダが使用されたマークを付ける
            // 引　数：[in]file   使用したフォルダのフォルダ名
            // 戻り値：なし
            //=========================================================================================
            public void SetFolderUsed(string file) {
                if (m_destFileIgnoreCase) {
                    file = file.ToLower();
                }
                if (m_destFolderFlag.ContainsKey(file)) {
                    m_destFolderListOrg[m_destFolderFlag[file]] = null;
                }
            }

            //=========================================================================================
            // 機　能：未参照のファイル一覧を返す
            // 引　数：なし
            // 戻り値：未参照のファイル一覧
            //=========================================================================================
            public List<IFile> GetUnrefFile() {
                List<IFile> unrefFileList = new List<IFile>();
                for (int i = 0; i < m_destFileListOrg.Count; i++) {
                    if (m_destFileListOrg[i] != null) {
                        unrefFileList.Add(m_destFileListOrg[i]);
                    }
                }
                return unrefFileList;
            }

            //=========================================================================================
            // 機　能：未参照のフォルダ一覧を返す
            // 引　数：なし
            // 戻り値：未参照のフォルダ一覧
            //=========================================================================================
            public List<IFile> GetUnrefFolder() {
                List<IFile> unrefFolderList = new List<IFile>();
                for (int i = 0; i < m_destFolderListOrg.Count; i++) {
                    if (m_destFolderListOrg[i] != null) {
                        unrefFolderList.Add(m_destFolderListOrg[i]);
                    }
                }
                return unrefFolderList;
            }
        }

        //=========================================================================================
        // 機　能：ファイル属性を確認する
        // 引　数：[in]srcFilePath    転送元のファイル名(C:\SRCBASE\MARKFILE.txt)
        // 　　　　[ref]srcFileInfo   転送元の取得済みのファイル情報（まだ取得していないときはnull）
        // 　　　　[in]destFilePath   転送先のファイル名(C:\SRCBASE\MARKFILE.txt)
        // 　　　　[ref]destFileInfo  転送先の取得済みのファイル情報（まだ取得していないときはnull、転送先が存在しない場合はnullで戻る）
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus CheckFileAttribute(MirrorCopyOption.MirrorCopyTransferMode transferMode, string srcFilePath, ref IFile srcFileInfo, string destFilePath, ref IFile destFileInfo) {
            FileOperationStatus status;

            // 転送元の属性を取得
            if (srcFileInfo == null) {
                status = FileProviderSrc.SrcFileSystem.GetFileInfo(RequestContext, srcFilePath, true, out srcFileInfo);
                if (!status.Succeeded) {
                    LogFileOperationStart(FileOperationType.CopyFile, srcFilePath, false);
                    LogFileOperationEnd(status);
                    return status;
                }
            }

            // 転送先の属性を取得
            if (destFileInfo == null) {
                status = FileProviderDest.DestFileSystem.GetFileInfo(RequestContext, destFilePath, false, out destFileInfo);
                if (status == FileOperationStatus.FileNotFound) {
                    destFileInfo = null;
                } else if (!status.Succeeded) {
                    LogFileOperationStart(FileOperationType.CopyFile, destFilePath, false);
                    LogFileOperationEnd(status);
                    return status;
                }
            }

            // 属性を評価
            if (destFileInfo == null) {
                // 転送先にないため無条件にコピー
                return FileOperationStatus.Success;
            }

            // 属性を比較
            if (transferMode == MirrorCopyOption.MirrorCopyTransferMode.OverwriteIfNewer) {
                int compDate = BackgroundTaskCommandUtil.CompareFileDate(FileProviderSrc.SrcFileSystem.FileSystemId, FileProviderDest.DestFileSystem.FileSystemId, srcFileInfo.ModifiedDate, destFileInfo.ModifiedDate);
                if (compDate <= 0) {        // srcが古いときスキップ
                    LogFileOperationStart(FileOperationType.CopyFile, destFilePath, false);
                    LogFileOperationEnd(FileOperationStatus.Skip);
                    return FileOperationStatus.Skip;
                }
            } else if (transferMode == MirrorCopyOption.MirrorCopyTransferMode.DifferenceAttribute) {
                int compDate = BackgroundTaskCommandUtil.CompareFileDate(FileProviderSrc.SrcFileSystem.FileSystemId, FileProviderDest.DestFileSystem.FileSystemId, srcFileInfo.ModifiedDate, destFileInfo.ModifiedDate);
                if (compDate == 0 && srcFileInfo.FileSize == destFileInfo.FileSize) {       // 日付もサイズも同じときスキップ
                    LogFileOperationStart(FileOperationType.CopyFile, destFilePath, false);
                    LogFileOperationEnd(FileOperationStatus.Skip);
                    return FileOperationStatus.Skip;
                }
            } else {
                Program.Abort("transferModeが未定義です。{0}", transferMode);
            }

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ファイル名が処理対象のものかどうかを返す
        // 引　数：[in]srcPath   コピー元のファイル/ディレクトリ名(C:\SRCBASE\MARKFILE.txt)
        // 　　　　[in]isDir     ディレクトリを処理中のときtrue
        // 戻り値：処理を継続してよいときtrue
        //=========================================================================================
        private bool CheckTargetName(string filePath, bool isDir) {
            bool isTarget = m_copyOption.MirrorCopyOption.CheckTargetFile(filePath);
            if (isTarget) {
                return true;
            } else {
                FileOperationType logType = (isDir ? FileOperationType.CopyDir : FileOperationType.CopyFile);
                LogFileOperationStart(logType, filePath, isDir);
                LogFileOperationEnd(FileOperationStatus.Skip);
                return false;
            }
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.MirrorCopy;
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
