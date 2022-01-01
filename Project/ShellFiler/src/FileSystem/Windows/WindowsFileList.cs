using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.DataObject;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Virtual;

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：ファイル一覧の情報
    //=========================================================================================
    class WindowsFileList : IFileList {
        // キャッシュ情報
        private FileOperationRequestContext m_cache;

        // ファイルシステム
        private WindowsFileSystem m_fileSystem;

        // 現在のディレクトリのフルパス
        private string m_directoryFullPath;

        // 仮想ディレクトリの遅延読み込みのリクエスト（仮想ディレクトリを使用しないときはnull）
        private WindowsVirtualFolderRequest m_virtualFolderRequest = null;

        // ファイル一覧
        private List<IFile> m_fileList = new List<IFile>();

        // ボリューム情報
        private VolumeInfo m_volumeInfo = null;

        // ファイルリストが左ウィンドウで表示されるときtrue
        private bool m_isLeftWindow;

        // Windowsファイルシステムでのファイル一覧のコンテキスト情報
        private WindowsFileListContext m_fileListContext;

        // ファイル再読込の世代番号
        private int m_loadingGeneration;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]cache        キャッシュ情報
        // 　　　　[in]fileSystem   ファイルシステム
        // 　　　　[in]directory    一覧を作成するディレクトリ
        // 　　　　[in]isLeftWindow 左ウィンドウに表示する一覧のときtrue
        // 戻り値：なし
        //=========================================================================================
        public WindowsFileList(FileOperationRequestContext cache, IFileSystem fileSystem, string directory, bool isLeftWindow) {
            m_cache = cache;
            m_fileSystem = (WindowsFileSystem)fileSystem;
            m_directoryFullPath = fileSystem.CompleteDirectoryName(fileSystem.GetFullPath(directory));
            m_fileList.Clear();
            m_isLeftWindow = isLeftWindow;
            m_fileListContext = new WindowsFileListContext();
            m_loadingGeneration = Program.Document.FileSystemFactory.GetNextLoadingGeneration();
        }
        
        //=========================================================================================
        // 機　能：一覧を取得する
        // 引　数：[in]chdirMode    ディレクトリ変更のモード
        // 戻り値：一覧取得のステータス
        //=========================================================================================
        public ChangeDirectoryStatus GetFileList(ChangeDirectoryParam chdirMode) {
            m_virtualFolderRequest = null;

            if (Directory.Exists(m_directoryFullPath)) {
                // ディレクトリ一覧
                return GetFileListFromDirectory(chdirMode);
            } else {
                // アーカイブをチェック
                return GetFileListFromArchiveFile(chdirMode);
            }
        }
        
        //=========================================================================================
        // 機　能：ディレクトリから一覧を取得する
        // 引　数：[in]chdirMode    ディレクトリ変更のモード
        // 戻り値：一覧取得のステータス
        //=========================================================================================
        private ChangeDirectoryStatus GetFileListFromDirectory(ChangeDirectoryParam chdirMode) {
            // ディレクトリを取得
            List<IFile> fileList = null;
            FileOperationStatus status = m_fileSystem.GetFileList(m_cache, m_directoryFullPath, out fileList);
            if (status == FileOperationStatus.Success) {
                m_fileList = fileList;
            } else {
                return ChangeDirectoryStatus.Failed;
            }

            // ボリューム情報を取得
            VolumeInfo volumeInfo = null;
            status = m_fileSystem.GetVolumeInfo(m_cache, m_directoryFullPath, out volumeInfo);
            if (status == FileOperationStatus.Success) {
                m_volumeInfo = volumeInfo;
            } else {
                return ChangeDirectoryStatus.Failed;
            }
            return ChangeDirectoryStatus.Success;
        }
        
        //=========================================================================================
        // 機　能：アーカイブから一覧を取得する
        // 引　数：[in]chdirMode    ディレクトリ変更のモード
        // 戻り値：一覧取得のステータス
        //=========================================================================================
        private ChangeDirectoryStatus GetFileListFromArchiveFile(ChangeDirectoryParam chdirMode) {
            // 対象ディレクトリを分解
            string root;
            string[] subDirList;
            bool success = GenericFileStringUtils.SplitWindowsSubDirectoryList(m_directoryFullPath, out root, out subDirList);
            if (!success) {
                return ChangeDirectoryStatus.Failed;
            }

            // 後ろのパス区切りから順にアーカイブの存在チェック
            string arcPath = null;
            for (int i = subDirList.Length - 1; i >= 0; i--) {
                string checkArcPath = root + StringUtils.CombineStringArray(subDirList, 0, i + 1, "\\");
                if (File.Exists(checkArcPath)) {
                    arcPath = checkArcPath;
                    break;
                } else if (Directory.Exists(checkArcPath)) {
                    return ChangeDirectoryStatus.Failed;
                }
            }
            if (arcPath == null) {
                return ChangeDirectoryStatus.Failed;
            }

            // アーカイブを確認
            if (!Program.Document.ArchiveFactory.IsSupportFileList()) {
                ArchiveDownloadDialog helpDialog = new ArchiveDownloadDialog();
                helpDialog.ShowDialog(Program.MainWindow);
                return ChangeDirectoryStatus.ArcNotInstalled;
            }
            m_virtualFolderRequest = new WindowsVirtualFolderRequest(arcPath);          // StartLoading()へ
            return ChangeDirectoryStatus.Loading;
        }

        //=========================================================================================
        // 機　能：並列読み込みを開始すする
        // 引　数：[in]chdirMode   ディレクトリ変更のモード
        // 戻り値：なし
        //=========================================================================================
        public void StartLoading(ChangeDirectoryParam chdirMode) {
            // Windowsでは初期化時にすぐ読み込むが、仮想ディレクトリのみ遅延処理
            // 仮想ディレクトリでは結果を非同期通知で戻す
            if (m_virtualFolderRequest != null) {
                // 次にアクセスする仮想フォルダを決定
                VirtualFolderTemporaryDirectory tempDir = Program.Document.TemporaryManager.VirtualManager.CreateVirtualFolder();
                VirtualFolderInfo virtualFolder = new VirtualFolderInfo(this.m_fileSystem, tempDir, m_fileListContext);
                Program.Document.TemporaryManager.VirtualManager.BeginUsingVirtualFolder(virtualFolder, VirtualFolderTemporaryDirectory.UsingType.FileListLoading);
                VirtualFolderArchiveInfo virtualArchive = VirtualFolderArchiveInfo.FromWindowsArchive(m_virtualFolderRequest.ArchivePath + "\\");
                virtualFolder.AddVirtualArchive(virtualArchive);

                // リクエスト
                FileOperationRequestContext context = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(BackgroundTaskID.NullId, FileSystemID.Windows, FileSystemID.None, null, null, null);
                GetVirtualUIFileListArg arg = new GetVirtualUIFileListArg(context, virtualFolder, m_directoryFullPath, m_isLeftWindow, chdirMode, m_loadingGeneration);
                Program.Document.UIRequestBackgroundThread.Request(arg, m_directoryFullPath);
            }
        }

        //=========================================================================================
        // プロパティ：一覧取得に使用するファイルシステム
        //=========================================================================================
        public IFileSystem FileSystem {
            get {
                return m_fileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧を返す
        //=========================================================================================
        public List<IFile> Files {
            get {
                return m_fileList;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリ名（最後は必ずセパレータ）
        //=========================================================================================
        public string DirectoryName {
            get {
                return m_directoryFullPath;
            }
        }
        
        //=========================================================================================
        // プロパティ：左ウィンドウで表示される一覧のときtrue
        //=========================================================================================
        public bool IsLeftWindow {
            get {
                return m_isLeftWindow;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル再読込の世代番号
        //=========================================================================================
        public int LoadingGeneration {
            get {
                return m_loadingGeneration;
            }
        }

        //=========================================================================================
        // プロパティ：ボリューム情報
        //=========================================================================================
        public VolumeInfo VolumeInfo {
            get {
                return m_volumeInfo;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルシステムごとのコンテキスト情報（一覧取得前はnull）
        //=========================================================================================
        public IFileListContext FileListContext {
            get {
                return m_fileListContext;
            }
        }

        //=========================================================================================
        // クラス：Windowsファイル一覧内で仮想フォルダのリクエストを行うための情報
        //=========================================================================================
        private class WindowsVirtualFolderRequest {
            // Windowsファイルシステム上ではじめに見つかったアーカイブファイル
            private string m_archivePath;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]archivePath   Windowsファイルシステム上ではじめに見つかったアーカイブファイル
            // 戻り値：なし
            //=========================================================================================
            public WindowsVirtualFolderRequest(string archivePath) {
                m_archivePath = archivePath;
            }

            //=========================================================================================
            // プロパティ：Windowsファイルシステム上ではじめに見つかったアーカイブファイル
            //=========================================================================================
            public string ArchivePath {
                get {
                    return m_archivePath;
                }
            }
        }
    }
}
