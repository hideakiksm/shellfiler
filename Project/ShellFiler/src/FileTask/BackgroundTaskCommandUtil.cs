using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask.Management;
using ShellFiler.Properties;
using ShellFiler.UI.FileList;
using ShellFiler.Util;
using ShellFiler.Virtual;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：バックグラウンドタスクのコマンドから使用するユーティリティ
    //=========================================================================================
    public class BackgroundTaskCommandUtil {

        //=========================================================================================
        // 機　能：タスクの開始条件をチェックする
        // 引　数：[in]classType   開始しようとしているタスクを実装しているクラスの型情報
        // 戻り値：タスクを開始してよいときtrue
        //=========================================================================================
        public static bool CheckTaskStart(Type classType) {
            // タスクがこれ以上実行できるか？
            BackgroundTaskRunningType type = Program.Document.BackgroundTaskManager.GetRunningType(classType, true);
            if (type == BackgroundTaskRunningType.Waiting) {
                DialogResult result = InfoBox.Question(Program.MainWindow, MessageBoxButtons.YesNo, Resources.Msg_TooManyWaitableBackgroundTask);
                if (result != DialogResult.Yes) {
                    return false;
                }
            } else if (type == BackgroundTaskRunningType.WaitingOver || type == BackgroundTaskRunningType.LimitedOver) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_TooManyBackgroundTask);
                return false;
            }

            // 更新中の一覧があれば使用不可
            bool loading = Program.Document.TabPageList.CheckLoadingFileList();
            if (loading) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_NowLoadingFileList);
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：同一のファイルシステムかつファイル空間かどうかをチェックする
        // 引　数：[in]fileListTarget    対象パスのファイル一覧
        // 　　　　[in]fileListOpposite  反対パスのファイル一覧
        // 戻り値：同一の空間のときtrue
        // メ　モ：戻り値がtrueのSSHの一覧同士では、対象パスと反対パスの間でコマンドの実行ができる
        //=========================================================================================
        public static bool CheckSameFileSystem(FileListView fileListTarget, FileListView fileListOpposite) {
            // 同じファイルシステムか？
            FileSystemID srcFileSystemId = fileListTarget.FileList.FileSystem.FileSystemId;
            FileSystemID destFileSystemId = fileListOpposite.FileList.FileSystem.FileSystemId;
            if (srcFileSystemId != destFileSystemId) {
                return false;
            }

            // 同じサーバー空間か？
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(srcFileSystemId);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(destFileSystemId);
            string srcPath = fileListTarget.FileList.DisplayDirectoryName;
            string destPath = fileListOpposite.FileList.DisplayDirectoryName;
            if (!srcFileSystem.IsSameServerSpace(srcPath, destPath)) {
                return false;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：フォルダ内のファイルとフォルダの情報を読み込む
        // 引　数：[in]fileSystem  一覧取得元のファイルシステム
        // 　　　　[in]context     コンテキスト情報
        // 　　　　[in]srcPath     一覧を読み込むファイルパス
        // 　　　　[out]fileList   ファイル一覧を読み込む変数
        // 　　　　[out]dirList    フォルダ一覧を読み込む変数
        // 戻り値：ステータス
        //=========================================================================================
        public static FileOperationStatus GetFileList(IFileSystem fileSystem, FileOperationRequestContext context, string srcPath, out List<IFile> fileList, out List<IFile> dirList) {
            FileOperationStatus status;
            fileList = new List<IFile>();
            dirList = new List<IFile>();

            List<IFile> files = null;
            status = fileSystem.GetFileList(context, srcPath, out files);
            if (status != FileOperationStatus.Success) {
                return FileOperationStatus.CanNotAccess;
            }
            foreach (IFile file in files) {
                if (file.FileName == "." || file.FileName == "..") {
                    continue;
                }
                if (!file.Attribute.IsDirectory || file.Attribute.IsSymbolicLink) {
                    fileList.Add(file);
                } else {
                    dirList.Add(file);
                }
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：フォルダ内のマーク状態を確認する
        // 引　数：[in]fileListView  対象フォルダのファイル一覧
        // 　　　　[in]markless      マークなし操作が許可状態のときtrue
        // 戻り値：処理を開始してよいときtrue
        //=========================================================================================
        public static bool CheckMarkState(FileListView fileListView, bool markless) {
            if (fileListView.FileList.Files.Count == 0) {
                return false;
            }
            if (markless) {
                // マークなし許可
                if (fileListView.FileList.MarkFiles.Count == 0) {
                    fileListView.FileListViewComponent.Mark(false);
                }
                if (fileListView.FileList.MarkFiles.Count == 0) {
                    return false;
                }
            } else {
                // マーク必須
                if (fileListView.FileList.MarkFiles.Count == 0) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：バックグラウンドタスク実行用に仮想フォルダ管理情報のクローンを作成する
        // 引　数：[in]fileListView  対象フォルダのファイル一覧
        // 戻り値：作成したクローン
        //=========================================================================================
        public static IFileListContext CloneFileListContext(FileListView fileListView) {
            IFileListContext fileListContext = fileListView.FileList.FileListContext;
            IFileListContext cloneInfo = fileListContext.CloneForBackgroundTask();
            if (cloneInfo is VirtualFileListContext) {
                VirtualFolderInfo cloneVirtualInfo = ((VirtualFileListContext)cloneInfo).VirtualFolderInfo;
                Program.Document.TemporaryManager.VirtualManager.BeginUsingVirtualFolder(cloneVirtualInfo, VirtualFolderTemporaryDirectory.UsingType.FileOperationRunning);
            }
            return cloneInfo;
        }

        //=========================================================================================
        // 機　能：仮想フォルダ実行禁止の機能について、対象フォルダが仮想フォルダかどうかを確認する
        // 引　数：[in]dialogParent  ダイアログの親になるフォーム
        // 　　　　[in]fileSystemId  対象フォルダのファイルシステム
        // 　　　　[in]isTarget      対象パスのときtrue、反対パスのときfalse
        // 戻り値：処理を継続してよいときtrue
        //=========================================================================================
        public static bool CheckAllowedNonVirtualFolder(Form dialogParent, FileSystemID fileSystemId, bool isTarget) {
            if (FileSystemID.IsVirtual(fileSystemId)) {
                if (isTarget) {
                    InfoBox.Warning(dialogParent, Resources.Msg_RejectFromVirtual);
                } else {
                    InfoBox.Warning(dialogParent, Resources.Msg_RejectFromVirtual2);
                }
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：マークファイルとカーソルファイルの一覧を取得する
        // 引　数：[in]fileListView  対象フォルダのファイル一覧
        // 戻り値：マークファイルとカーソルファイルの一覧
        //=========================================================================================
        public static List<string> GetMarkAndCurrentFileList(FileListView fileListView) {
            // 情報を取得
            List<UIFile> markUIFiles = fileListView.FileList.MarkFiles;
            int cursor = fileListView.FileListViewComponent.CursorLineNo;
            UIFile cursorUIFile = fileListView.FileList.Files[cursor];
            string dir = fileListView.FileList.DisplayDirectoryName;

            // 結果を作成
            List<string> result = new List<string>();
            if (cursorUIFile.FileName != "..") {
                result.Add(dir + cursorUIFile.FileName);
            }
            for (int i = 0; i < markUIFiles.Count; i++) {
                if (markUIFiles[i].FileName == "..") {
                    continue;
                }
                if (markUIFiles[i] != cursorUIFile) {
                    result.Add(dir + markUIFiles[i].FileName);
                }
            }
            return result;
        }

        //=========================================================================================
        // 機　能：SSHがサポートされているかどうかを確認する
        // 引　数：[in]fileSystem    対象パスのファイルシステム
        // 　　　　[in]checkCurrent  対象パスのチェックを行うときtrue
        // 　　　　[in]allowSystem   許可されているファイルシステム（SSH全般のときnull）
        // 戻り値：SSHがサポートされているときtrue
        //=========================================================================================
        public static bool CheckSSHSupport(FileSystemID fileSystem, bool checkCurrent, FileSystemID allowSystem) {
            if (Program.Document.FileSystemFactory.SFTPFileSystem == null) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_SharpSSHNotInstalled);
                return false;
            }
            if (checkCurrent) {
                if (allowSystem == null) {
                    if (!FileSystemID.IsSSH(fileSystem)) {
                        InfoBox.Warning(Program.MainWindow, Resources.Msg_NotSSHFolder);
                        return false;
                    }
                } else {
                    if (fileSystem != allowSystem) {
                        if (allowSystem == FileSystemID.SSHShell) {
                            InfoBox.Warning(Program.MainWindow, Resources.Msg_NotSSHShellFolder);
                            return false;
                        } else {
                            InfoBox.Warning(Program.MainWindow, Resources.Msg_NotSSHFolder);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルシステム間の相違を考慮した日付の比較を行う
        // 引　数：[in]fileSystem1   ファイルシステム1
        // 　　　　[in]fileSystem2   ファイルシステム2
        // 　　　　[in]date1         日付1
        // 　　　　[in]date2         日付2
        // 戻り値：日付の比較結果（-1/0/1）
        //=========================================================================================
        public static int CompareFileDate(FileSystemID fileSystem1, FileSystemID fileSystem2, DateTime date1, DateTime date2) {
            if (FileSystemID.IsWindows(fileSystem1) && FileSystemID.IsWindows(fileSystem2) ||
                    FileSystemID.IsSSH(fileSystem1) && FileSystemID.IsSSH(fileSystem2)) {
                if (date1 < date2) {
                    return -1;
                } else if (date1 > date2) {
                    return 1;
                } else {
                    return 0;
                }
            } else {
                long second1 = date1.Ticks / 10000000;
                long second2 = date2.Ticks / 10000000;
                if (second1 < second2) {
                    return -1;
                } else if (second1 > second2) {
                    return 1;
                } else {
                    return 0;
                }
            }
        }
    }
}
