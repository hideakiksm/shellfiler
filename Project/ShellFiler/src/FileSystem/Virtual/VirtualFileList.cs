using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.UI.Dialog;
using ShellFiler.Virtual;

namespace ShellFiler.FileSystem.Virtual {

    //=========================================================================================
    // クラス：ファイル一覧の情報
    //=========================================================================================
    public class VirtualFileList : IFileList {
        // ファイルシステム
        private VirtualFileSystem m_fileSystem;

        // 現在のディレクトリのフルパス
        private string m_directoryFullPath;

        // ファイル一覧
        private List<IFile> m_fileList = new List<IFile>();

        // ファイルリストが左ウィンドウで表示されるときtrue
        private bool m_isLeftWindow;
        
        // ボリューム情報
        private VolumeInfo m_volumeInfo = null;

        // 仮想フォルダファイルシステムでのファイル一覧のコンテキスト情報
        private VirtualFileListContext m_fileListContext;

        // ファイル再読込の世代番号
        private int m_loadingGeneration;

        //=========================================================================================
        // 機　能：他のファイルシステムと差し替える形で新規に作成する
        // 引　数：[in]fileSystem        仮想フォルダファイルシステム
        // 　　　　[in]directory         一覧を作成するディレクトリ
        // 　　　　[in]isLeftWindow      左ウィンドウに表示する一覧のときtrue
        // 　　　　[in]loadingGeneration IFileListでの読み込みの世代
        // 　　　　[in]virtualFolder     仮想フォルダの情報
        // 　　　　[in]baseFileListCtx   元のファイルシステムでのコンテキスト情報
        // 　　　　[in]fileList          設定するファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public static VirtualFileList CreateForExchangeFileSystem(IFileSystem fileSystem, string directory, bool isLeftWindow, int loadingGeneration, VirtualFolderInfo virtualFolder, List<IFile> fileList) {
            VirtualFileList list = new VirtualFileList();
            list.m_fileSystem = (VirtualFileSystem)fileSystem;
            list.m_directoryFullPath = fileSystem.CompleteDirectoryName(fileSystem.GetFullPath(directory));
            list.m_fileList.Clear();
            list.m_fileList.AddRange(fileList);
            list.m_isLeftWindow = isLeftWindow;
            list.m_fileListContext = new VirtualFileListContext(virtualFolder);
            list.m_loadingGeneration = loadingGeneration;
            return list;
        }

        //=========================================================================================
        // 機　能：すでに存在する仮想フォルダから作成する
        // 引　数：[in]fileSystem        仮想フォルダファイルシステム
        // 　　　　[in]directory         一覧を作成するディレクトリ
        // 　　　　[in]isLeftWindow      左ウィンドウに表示する一覧のときtrue
        // 　　　　[in]virtualFolder     仮想フォルダの情報
        // 戻り値：仮想フォルダ情報
        //=========================================================================================
        public static VirtualFileList CreateFromExistingVirtualFolder(IFileSystem fileSystem, string directory, bool isLeftWindow, VirtualFolderInfo virtualFolder) {
            VirtualFileList list = new VirtualFileList();
            list.m_fileSystem = (VirtualFileSystem)fileSystem;
            list.m_directoryFullPath = fileSystem.CompleteDirectoryName(fileSystem.GetFullPath(directory));
            list.m_fileList.Clear();
            list.m_isLeftWindow = isLeftWindow;
            list.m_fileListContext = new VirtualFileListContext(virtualFolder);
            list.m_loadingGeneration = Program.Document.FileSystemFactory.GetNextLoadingGeneration();
            return list;
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private VirtualFileList() {
        }

        //=========================================================================================
        // 機　能：一覧を取得する
        // 引　数：[in]chdirMode    ディレクトリ変更のモード
        // 戻り値：一覧取得のステータス
        //=========================================================================================
        public ChangeDirectoryStatus GetFileList(ChangeDirectoryParam chdirMode) {
            return ChangeDirectoryStatus.Loading;
        }

        //=========================================================================================
        // 機　能：並列読み込みを開始する
        // 引　数：[in]chdirMode   ディレクトリ変更のモード
        // 戻り値：なし
        //=========================================================================================
        public void StartLoading(ChangeDirectoryParam chdirMode) {
            // 仮想ディレクトリでは結果を非同期通知で戻す
            // 次にアクセスする仮想フォルダを決定
            VirtualFolderInfo nextVirtualFolder = m_fileListContext.VirtualFolderInfo.CloneForUpdate();
            Program.Document.TemporaryManager.VirtualManager.BeginUsingVirtualFolder(nextVirtualFolder, VirtualFolderTemporaryDirectory.UsingType.FileListLoading);
            VirtualFolderArchiveInfo nextArchive = nextVirtualFolder.GetVirtualFolderItem(m_directoryFullPath);
            nextVirtualFolder.DeleteSubVirtualArchive(nextArchive);

            // リクエスト
            FileOperationRequestContext context = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(BackgroundTaskID.NullId, FileSystemID.Virtual, FileSystemID.None, null, null, null);
            GetVirtualUIFileListArg arg = new GetVirtualUIFileListArg(context, nextVirtualFolder, m_directoryFullPath, m_isLeftWindow, chdirMode, m_loadingGeneration);
            Program.Document.UIRequestBackgroundThread.Request(arg, m_directoryFullPath);
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
            set {
                m_fileList = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリ名（最後は必ずセパレータ）
        //=========================================================================================
        public string DirectoryName {
            get {
                return m_directoryFullPath;
            }
            set {
                m_directoryFullPath = value;
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
            set {
                m_volumeInfo = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルシステムごとのコンテキスト情報（一覧取得前はnull）
        //=========================================================================================
        public IFileListContext FileListContext {
            get {
                return m_fileListContext;
            }
            set {
                m_fileListContext = (VirtualFileListContext)value;
            }
        }
    }
}

// 仮想ディレクトリ対応の擬似コード
// class UIFileList {
//     public void ChangeDirectory() {
//         IFileList fileList = FileSystemFactory.CreateFileList(this);
//         fileList.GetFileList();
//         if (failed) return;
//         if (loading) {
//             fileList.StartLoading();                // 並列読みを開始
//             return;
//         }
//         StoreFileInfo(fileList);
//     }
//     public void StoreFileInfo(IFileList fileList) {
//         swap List<IFile>
//     }
// }
// 
// class FileSystemFactory {
//     public IFileList CreateFileList(UIFileList oldFileList, string newDir) {
//         if (oldFileList == null) {
//             return new WindowsFileList;
//         }
//         if (oldFileList.FileSystem.IsAbsolutePath(newPath)) {
//             return oldFileList.FileSystem.CreateFileListExisting(newDir);
//         } else if (s_windowsFileSystem.IsAbsolutePath(newPath)) {
//             return new WindowsFileList();
//         } else if (s_sshFileSystem.IsAbsolutePath(newPath)) {
//             return new SSHFileList();
//         } else {
//             string dir = oldFileList.Directory + newDir;
//             if (oldFileList.IsAbsolutePath(dir)) {
//                 return oldFileList.FileSystem.CreateFileListExisting(newDir);
//             }
//         }
//         return null;
//     }
// }
// 
// class WindowsFileSystem {
//     public IFileList CreateFileListFromExisting() {
//         return new WindowsFileList();
//     }
//     public void GetUIFileListVirtual() {
//         GetVirtualUIFileListArg arg = new GetVirtualUIFileListArg(context, null, directory, archiveFile, archiveFile, isLeftWindow, chdirMode);
//         Program.Document.UIRequestBackgroundThread.Request(arg, archiveFile);
//     }
// }
// 
// class WindowsFileList {
//     private List<IFile> m_fileList = new List<IFile>();
//     public void GetFileList() {
//         if (Directory.Exists(m_directoryFullPath)) {
//             GetFileListFromDirectory();
//         } else {
//             GetFileListFromArchiveFile();
//         }
//     }
//     private GetFileListFromDirectory() {
//         m_fileList = Windowsのファイル一覧
//     }
//     private GetFileListFromArchiveFile() {
//         GenericFileStringUtils.SplitWindowsSubDirectoryList(m_directoryFullPath, out root, out subDirList);
//         string arcPath = null;
//         for (subDirList 後ろから) {
//             subDir = 先頭からループ位置まで連結
//             if (File.Exists(subDir)) {    // 仮想Dirの圧縮ファイルを発見
//                 arcPath = subDir;
//                 break;
//             } else if (Directory.Exists(subDir)) {
//                 エラー
//             }
//         }
//         if (arcPath == null) {
//             return failed;
//         }
//         m_virtualFolderRequest = new WindowsVirtualFolderRequest(null, arcPath);          // StartLoading()へ
//     }
//     public void StartLoading(ChangeDirectoryParam chdirMode) {
//         if (m_virtualFolderRequest != null) {
//             (WindowsFileSystem)m_fileSystem.GetUIFileListVirtual()
//         }
//     }
// }
// class GetVirtualUIFileListProcedure() {
//     public Execute(VirtualFolder virtualFolder, IFileSystem orgFileSystem, string directory, string directoryArchive, string realArchive) {
//         if (virtualFolder == null) {
//             // user@server:/home/user/data/file1.tar.gz/dir1/file2.zip/dir2/
//             // subPath = dir1/file2.zip/dir2/
//             virtualFolder = new VirtualFolder(orgFileSystem)
//             virtualFolder.AddVirtualItem(new VirtualFolderItem(realArchive, directoryArchive));
//         } else {
//         }
//         SubPahList subPathList = ...;   {"dir1", "dir1/file2.zip", "dir1/file2.zip/dir2"}
//         while (true) {
//             string arcFile = virtualFolder.GetLastVirtualItem().GetArchiveFile();
//             IArchiveFileList archive = ArchiveFactory.CreateFileList(archivePath);
//             List<IFile> virtualFiles;
//             status = GetVirtualFileList(archive, virtualFolder, directory, directoryArchive, realArchive, SubPathList, out virtualFiles);
//             if (!staus.Succeeded) {
//                 // 失敗した
//                 return failed;
//             } else if (status.Succeeded && virtualFolderItems != null) {
//                 // 一覧取得でき、指定パスがあった
//                 resultIFileList = new VirtualFileList(virtualFolder, virtualFiles);
//                 return success;
//             } else if (!SubPathList.LastFile == null) {
//                 // 一覧取得できたが指定パスがなかった
//                 return failed;
//             }
//             string nextArcFile = subPathList.LastFile;
//             ExtractArcFile(nextArcFile, ...);
//             virtualFolder.Add(new VirtualFolderItem(nextArcFile, ...));
//             archive.Dispose();
//         }
//     }
//     private GetVirtualFileList(IArchiveFileList archive, VirtualFolder virtualFolder, string directory, string directoryArchive, string realArchive, SubPathList subPathList) {
//         archive.Open(virtualItem);
//         if (failed) {
//             return failed;
//         }
//         archive.GetFileList(subPathList);
//         if (failed) {
//             return failed;
//         }
//     }
// }
// 
// 
// 
// // SSH
// // C:\Temp\001\Virtual\a.zip = user@server:/home/user/data/file1.zip/
// // C:\Temp\002\Virtual\a.zip = user@server:/home/user/data/file1.zip/dir1/file2.zip/
// class VirtualFolder {
//     IFileSystem BaseFileSystem;         // SSH
//     List<VirtualFolderItem> VirtualFolderItemList;
// }
// class VirtualFolderItem {
//     string ArchiveFile;                 // C:\Temp\Virtual\001\a.tar.gz
//     string InternalArchiveFile;         // C:\Temp\Virtual\001\a.tar
//     string displayPathArchive;          // user@server:/home/user/data/file1.tar.gz/dir1/file2.zip/dir2/
//     bool PasswordSpecified;
//     string Password;
// 
//     public Dispose() {
//         if (InternalArchiveFile != null) {
//             DeleteFile(InternalArchiveFile);
//         }
//     }
//     public string GetArchiveFile() {
//         if (InternalArchiveFile == null) {
//             return ArchiveFile;
//         } else {
//             return InternalArchiveFile;
//         }
//     }
// }
