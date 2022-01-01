using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Reflection;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document;
using ShellFiler.Document.SSH;
using ShellFiler.FileSystem.Transfer;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.FileSystem.Virtual;

namespace ShellFiler.FileSystem {

    //=========================================================================================
    // クラス：Windowsでのファイル一覧の情報
    //=========================================================================================
    class FileSystemFactory {
        // 次の再読込世代の番号
        private static int s_loadingGeneration = 0;

        // 接続の管理クラス（SSHモジュールが読み込めないときnull）
        private static SSHConnectionManager s_sshConnectionManager = null;

        // ダミーのファイルシステム
        private static DummyFileSystem s_dummyFileSystem = null;
        
        // Windows用ファイルシステム
        private static WindowsFileSystem s_windowsFileSystem = null;

        // SSH用SFTPファイルシステム（SSHモジュールが読み込めないときnull）
        private static SFTPFileSystem s_sftpFileSystem = null;

        // SSH用シェルファイルシステム（SSHモジュールが読み込めないときnull）
        private static ShellFileSystem s_shellFileSystem = null;

        // 仮想フォルダ用ファイルシステム（サポートしていない場合はnull）
        private static VirtualFileSystem s_virtualFileSystem = null;

        // ダミーのファイル転送システム
        private static DummyTransferSystem s_dummyTransferSystem = null;

        // Windows→Windows用のファイル転送システム
        private static WindowsToWindowsFileSystem s_windowsToWindowsFileSystem = null;

        // SFTP→SFTP用のファイル転送システム（SSHモジュールが読み込めないときnull）
        private static SFTPToSFTPFileSystem s_sshToSshFileSystem = null;

        // SSHシェル→SSHシェル用のファイル転送システム（SSHモジュールが読み込めないときnull）
        private static ShellToShellFileSystem s_shellToShellFileSystem = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileSystemFactory() {
            s_dummyFileSystem = new DummyFileSystem();
            s_dummyTransferSystem = new DummyTransferSystem();

            s_windowsFileSystem = new WindowsFileSystem();
            s_windowsToWindowsFileSystem = new WindowsToWindowsFileSystem(s_windowsFileSystem);

            try {
                string dllPath = Path.Combine(Program.InstallPath, "Tamir.SharpSSH.dll");
                if (File.Exists(dllPath)) {
                    s_sshConnectionManager = new SSHConnectionManager();
                    s_sftpFileSystem = new SFTPFileSystem(s_sshConnectionManager);
                    s_sshToSshFileSystem = new SFTPToSFTPFileSystem(s_sftpFileSystem);
                    s_shellFileSystem = new ShellFileSystem(s_sshConnectionManager);
                    s_shellToShellFileSystem = new ShellToShellFileSystem(s_shellFileSystem);
                }
            } catch {
                // 失敗時、s_sshFileSystemはnullのまま
            }

            if (Program.Document.ArchiveFactory.IsSupportExtract() && Program.Document.ArchiveFactory.IsSupportFileList()) {
                SFTPFileSystem sftpFile = s_sftpFileSystem;
                if (sftpFile == null) {
                    sftpFile = new SFTPFileSystem(null);
                }
                ShellFileSystem shellFile = s_shellFileSystem;
                if (shellFile == null) {
                    shellFile = new ShellFileSystem(null);
                }
                s_virtualFileSystem = new VirtualFileSystem(s_windowsFileSystem, sftpFile, shellFile);
            }
        }

        //=========================================================================================
        // 機　能：ファイルシステム間の転送用システムを作成する
        // 引　数：[in]srcId        転送元ファイルシステムのID
        // 　　　　[in]destId       転送先ファイルシステムのID
        // 　　　　[in]forceDownUp  ダウンロードとアップロードで処理するときtrue
        // 戻り値：転送用システム
        // メ　モ：マルチスレッド対応
        //=========================================================================================
        public IFileSystemToFileSystem CreateTransferFileSystem(FileSystemID srcId, FileSystemID destId, bool forceDownUp) {
            if (srcId == FileSystemID.None || destId == FileSystemID.None) {
                return s_dummyTransferSystem;
            } else if (srcId == FileSystemID.Windows && destId == FileSystemID.Windows) {
                return s_windowsToWindowsFileSystem;
            } else if (srcId == FileSystemID.Windows && destId == FileSystemID.SFTP) {
                return new DownloadUploadFileSystem(s_windowsFileSystem, s_sftpFileSystem);
            } else if (srcId == FileSystemID.Windows && destId == FileSystemID.SSHShell) {
                return new DownloadUploadFileSystem(s_windowsFileSystem, s_shellFileSystem);

            } else if (srcId == FileSystemID.SFTP && destId == FileSystemID.Windows) {
                return new DownloadUploadFileSystem(s_sftpFileSystem, s_windowsFileSystem);
            } else if (srcId == FileSystemID.SFTP && destId == FileSystemID.SFTP) {
                if (forceDownUp) {
                    return new DownloadUploadFileSystem(s_sftpFileSystem, s_sftpFileSystem);
                } else {
                    return s_sshToSshFileSystem;
                }
            } else if (srcId == FileSystemID.SFTP && destId == FileSystemID.SSHShell) {
                return new DownloadUploadFileSystem(s_sftpFileSystem, s_shellFileSystem);

            } else if (srcId == FileSystemID.SSHShell && destId == FileSystemID.Windows) {
                return new DownloadUploadFileSystem(s_shellFileSystem, s_windowsFileSystem);
            } else if (srcId == FileSystemID.SSHShell && destId == FileSystemID.SFTP) {
                return new DownloadUploadFileSystem(s_shellFileSystem, s_sftpFileSystem);
            } else if (srcId == FileSystemID.SSHShell && destId == FileSystemID.SSHShell) {
                if (forceDownUp) {
                    return new DownloadUploadFileSystem(s_shellFileSystem, s_shellFileSystem);
                } else {
                    return s_shellToShellFileSystem;
                }

            } else if (srcId == FileSystemID.Virtual && destId == FileSystemID.Windows) {
                return new DownloadUploadFileSystem(s_virtualFileSystem, s_windowsFileSystem);
            } else if (srcId == FileSystemID.Virtual && destId == FileSystemID.SFTP) {
                return new DownloadUploadFileSystem(s_virtualFileSystem, s_sftpFileSystem);
            } else if (srcId == FileSystemID.Virtual && destId == FileSystemID.SSHShell) {
                return new DownloadUploadFileSystem(s_virtualFileSystem, s_shellFileSystem);
            } else if (destId == FileSystemID.Virtual) {           // DiffOppositeなど
                return s_dummyTransferSystem;
            } else {
                Program.Abort("認識していないファイルシステム{0}→{1}が使用されています。", srcId.StringId, destId.StringId);
                return null;
            }
        }

        //=========================================================================================
        // 機　能：ファイル操作用のファイルシステムを作成する
        // 引　数：[in]id   ファイルシステムのID
        // 戻り値：ファイルシステム
        // メ　モ：マルチスレッド対応
        //=========================================================================================
        public IFileSystem CreateFileSystemForFileOperation(FileSystemID id) {
            if (id == FileSystemID.Windows) {
                return s_windowsFileSystem;
            } else if (id == FileSystemID.SFTP) {
                return s_sftpFileSystem;
            } else if (id == FileSystemID.SSHShell) {
                return s_shellFileSystem;
            } else if (id == FileSystemID.Virtual) {
                return s_virtualFileSystem;
            } else {
                return s_dummyFileSystem;
            }
        }

        //=========================================================================================
        // 機　能：ファイル操作用のデフォルトファイルシステムを作成する
        // 引　数：なし
        // 戻り値：Windowsファイルシステム
        //=========================================================================================
        public IFileSystem CreateDefaultFileSystemForFileList() {
            return s_windowsFileSystem;
        }

        //=========================================================================================
        // 機　能：ディレクトリ名からファイル一覧を作成する
        // 引　数：[in]oldFileList   現在のファイル一覧（初回のときnull）
        // 　　　　[in]oldDirName    現在のディレクトリ（絶対パス、初回のときnull）
        // 　　　　[in]newDirName    変更先ディレクトリ
        // 　　　　[in]leftWindow    左ウィンドウで表示されるときtrue
        // 戻り値：作成したファイル一覧（エラーが発生したときnull）
        //=========================================================================================
        public IFileList CreateFileList(UIFileList oldFileList, string newDirName, bool leftWindow) {
            IFileList newFileList = null;
            if (oldFileList == null) {
                // 初回起動時
                // Windowsの絶対表現のみ
                if (!GenericFileStringUtils.IsWindowsAbsolutePath(newDirName)) {
                    return null;
                }
                FileOperationRequestContext cache = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(BackgroundTaskID.NullId, FileSystemID.Windows, FileSystemID.None, null, null, null);
                newFileList = new WindowsFileList(cache, s_windowsFileSystem, newDirName, leftWindow);
                return newFileList;
            }
            IFileListContext oldFileListCtx = oldFileList.FileListContext;
            IFileSystem oldFileSystem = oldFileList.FileSystem;
            if (oldFileSystem.IsAbsolutePath(newDirName, oldFileListCtx)) {
                // 旧ファイルシステム内での継続
                newFileList = oldFileSystem.CreateFileListFromExisting(newDirName, leftWindow, oldFileListCtx);
            } else if (s_windowsFileSystem.IsAbsolutePath(newDirName, null)) {
                // Windowsの新規
                newDirName = s_windowsFileSystem.GetFullPath(newDirName);
                FileOperationRequestContext cache = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(BackgroundTaskID.NullId, FileSystemID.Windows, FileSystemID.None, null, null, null);
                newFileList = new WindowsFileList(cache, s_windowsFileSystem, newDirName, leftWindow);
            } else if (s_sftpFileSystem != null && s_sftpFileSystem.IsAbsolutePath(newDirName, null)) {
                // SFTPの新規
                newDirName = s_sftpFileSystem.GetFullPath(newDirName);
                newFileList = new SFTPFileList(s_sftpFileSystem, newDirName, leftWindow);
            } else if (s_shellFileSystem != null && s_shellFileSystem.IsAbsolutePath(newDirName, null)) {
                // SSHShellの新規
                newDirName = s_shellFileSystem.GetFullPath(newDirName);
                newFileList = new ShellFileList(s_shellFileSystem, newDirName, leftWindow, null);
            } else {
                // 旧ファイルシステム内での相対
                string combinedDir = oldFileSystem.CombineFilePath(oldFileList.DisplayDirectoryName, newDirName);
                newDirName = oldFileSystem.GetFullPath(combinedDir);
                if (oldFileSystem.IsAbsolutePath(newDirName, oldFileListCtx)) {
                    newFileList = oldFileSystem.CreateFileListFromExisting(newDirName, leftWindow, oldFileListCtx);
                } else if (s_windowsFileSystem.IsAbsolutePath(newDirName, null)) {
                    // 仮想フォルダ→Windowsの新規
                    FileOperationRequestContext cache = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(BackgroundTaskID.NullId, FileSystemID.Windows, FileSystemID.None, null, null, null);
                    newFileList = new WindowsFileList(cache, s_windowsFileSystem, newDirName, leftWindow);
                } else if (s_sftpFileSystem != null && s_sftpFileSystem.IsAbsolutePath(newDirName, null)) {
                    // 仮想フォルダ→SFTPの新規
                    newFileList = new SFTPFileList(s_sftpFileSystem, newDirName, leftWindow);
                } else if (s_sftpFileSystem != null && s_shellFileSystem.IsAbsolutePath(newDirName, null)) {
                    // 仮想フォルダ→SSHシェルの新規
                    VirtualFolderInfo virtualInfo = ((VirtualFileListContext)(oldFileList.FileListContext)).VirtualFolderInfo;
                    IFileListContext context = null;
                    if (virtualInfo.BaseFileListContext is ShellFileListContext) {
                        context = virtualInfo.BaseFileListContext;
                    }
                    newFileList = new ShellFileList(s_shellFileSystem, newDirName, leftWindow, context);
                }
            }
            return newFileList;
        }

        //=========================================================================================
        // 機　能：パスの構成を解析する
        // 引　数：[in]directory     解析対象のディレクトリ
        // 　　　　[out]parsedPath   パスの構成要素（[0]:ルート）
        // 　　　　[out]separator    パスのセパレータ
        // 　　　　[out]fileSystemId 解析結果のファイルシステムのID
        // 戻り値：なし
        //=========================================================================================
        public void ParsePath(string directory, out List<string> parsedPath, out string separator, out FileSystemID fileSystemId) {
            IFileSystem fileSystem = null;
            if (s_windowsFileSystem.IsAbsolutePath(directory, null)) {
                fileSystem = s_windowsFileSystem;
            } else if (s_sftpFileSystem != null && s_sftpFileSystem.IsAbsolutePath(directory, null)) {
                fileSystem = s_sftpFileSystem;
            } else {
                Program.Abort("パス形式{0}が未知の形式です。", directory);
            }

            separator = fileSystem.GetPathSeparator(null);
            string root, sub;
            fileSystem.SplitRootPath(directory, out root, out sub);
            if (root.EndsWith(separator)) {
                root = root.Substring(0, root.Length - 1);
            }
            if (sub.EndsWith(separator)) {
                sub = sub.Substring(0, sub.Length - 1);
            }
            parsedPath = new List<string>();
            parsedPath.Add(root);
            if (sub != "") {
                parsedPath.AddRange(sub.Split(separator[0]));
            }
            fileSystemId = fileSystem.FileSystemId;
        }
        
        //=========================================================================================
        // 機　能：指定パスに対するファイルシステムを取得する
        // 引　数：[in]directory     解析対象のディレクトリ
        // 戻り値：ファイルシステムのID（対象外の形式のときNone）
        //=========================================================================================
        public FileSystemID GetFileSystemFromRootPath(string directory) {
            if (s_windowsFileSystem.IsAbsolutePath(directory, null)) {
                return s_windowsFileSystem.FileSystemId;
            } else if (s_sftpFileSystem != null && s_sftpFileSystem.IsAbsolutePath(directory, null)) {
                return s_sftpFileSystem.FileSystemId;
            } else {
                return FileSystemID.None;
            }
        }

        //=========================================================================================
        // 機　能：ファイル操作用のコンテキストオブジェクトを作成する
        // 引　数：[in]taskId           バックグラウンドタスクのID
        // 　　　　[in]srcFileSystem    転送元ファイルシステムのID
        // 　　　　[in]destFileSystem   転送先ファイルシステムのID（null：転送先の概念がない）
        // 　　　　[in]srcFileListCtx   転送元の仮想フォルダの情報（null:仮想フォルダを使用しない）
        // 　　　　[in]destFileListCtx  転送先の仮想フォルダの情報（null:仮想フォルダを使用しない）
        // 　　　　[in]cancelEvent      外部指定のキャンセルイベント（外部指定せず、内部で新規に作成するときnull）
        // 戻り値：なし
        //=========================================================================================
        public FileOperationRequestContext CreateFileOperationRequestContext(BackgroundTaskID taskId, FileSystemID srcFileSystem, FileSystemID destFileSystem, IFileListContext srcFileListCtx, IFileListContext destFileListCtx, ManualResetEvent cancelEvent) {
            return new FileOperationRequestContext(taskId, srcFileSystem, destFileSystem, srcFileListCtx, destFileListCtx, cancelEvent);
        }

        //=========================================================================================
        // 機　能：後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            s_windowsFileSystem.Dispose();
            s_dummyFileSystem.Dispose();
            if (s_sftpFileSystem != null) {
                s_sftpFileSystem.Dispose();
            }
            if (s_shellFileSystem != null) {
                s_shellFileSystem.Dispose();
            }
            if (s_virtualFileSystem != null) {
                s_virtualFileSystem.Dispose();
            }
            if (s_sshConnectionManager != null) {
                s_sshConnectionManager.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：ルートディレクトリを補完する
        // 引　数：[in]drive  ドライブ／サーバー名
        // 戻り値：ルートディレクトリ（補完できないときnull）
        //=========================================================================================
        public string CompleteRootDir(string drive) {
            if (s_windowsFileSystem.IsAbsolutePath(drive + "\\", null)) {
                return drive + "\\";
            } else if (s_sftpFileSystem != null && s_sftpFileSystem.IsAbsolutePath(drive + "/", null)) {
                return drive + "/";
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：次の再読込世代の番号を発行する
        // 引　数：なし
        // 戻り値：一意の再読込世代番号
        //=========================================================================================
        public int GetNextLoadingGeneration() {
            return Interlocked.Add(ref s_loadingGeneration, 1);
        }

        //=========================================================================================
        // プロパティ：SSH接続の管理クラス（SSHモジュールが読み込めないときnull）
        //=========================================================================================
        public SSHConnectionManager SSHConnectionManager {
            get {
                return s_sshConnectionManager;
            }
        }

        //=========================================================================================
        // プロパティ：Windows用ファイルシステム
        //=========================================================================================
        public WindowsFileSystem WindowsFileSystem {
            get {
                return s_windowsFileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：SFTPファイルシステム（サポートしていない場合はnull）
        //=========================================================================================
        public SFTPFileSystem SFTPFileSystem {
            get {
                return s_sftpFileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：SSHシェルファイルシステム（サポートしていない場合はnull）
        //=========================================================================================
        public ShellFileSystem ShellFileSystem {
            get {
                return s_shellFileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：仮想フォルダ用ファイルシステム（サポートしていない場合はnull）
        //=========================================================================================
        public VirtualFileSystem VirtualFileSystem {
            get {
                return s_virtualFileSystem;
            }
        }
    }
}
