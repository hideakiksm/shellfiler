using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.Virtual;

namespace ShellFiler.Command.FileList.FileOperationEtc {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 差分表示ツールで反対パスのファイルまたはディレクトリと比較します。
    //   書式 　 DiffOpposite()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.1.0
    //=========================================================================================
    class DiffOppositeCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DiffOppositeCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 差分表示ツールのコマンドラインを取得
            string command = DiffMarkCommand.GetDiffToolCommand(false);
            if (command == null) {
                DiffToolDownloadDialog dialog = new DiffToolDownloadDialog();
                dialog.ShowDialog(Program.MainWindow);
                return null;
            }

            // ファイル構成をチェック
            List<UIFile> fileList = FileListViewTarget.FileList.MarkFiles;
            if (fileList.Count == 0) {
                FileListComponentTarget.Mark(false);
            }
            fileList = FileListViewTarget.FileList.MarkFiles;
            if (fileList.Count == 0) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_DiffNoMark);
                return null;
            } else if (fileList.Count >= 2) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_DiffTooManyOpposite);
                return null;
            }

            // 反対パスとの関係をチェック
            UIFile fileTarget = fileList[0];
            UIFile fileOppositeName = null;             // 反対パスの同名ファイル
            UIFile fileOppositeMark = null;             // 反対パスのマークファイル（マークが1件の場合）
            foreach (UIFile file in FileListViewOpposite.FileList.Files) {
                if (string.Compare(fileTarget.FileName, file.FileName, true) == 0) {
                    fileOppositeName = file;
                    break;
                }
            }
            if (FileListViewOpposite.FileList.MarkFiles.Count == 1) {
                foreach (UIFile file in FileListViewOpposite.FileList.Files) {
                    if (file.Marked) {
                        fileOppositeMark = file;
                    }
                }
            }

            // 仮想フォルダの場合は実行準備
            bool success = PrepareVirtualFolderCommand.ReadyForVirtualFolder(FileListViewTarget);
            if (!success) {
                return false;
            }
            success = PrepareVirtualFolderCommand.ReadyForVirtualFolder(FileListViewOpposite);
            if (!success) {
                return false;
            }

            // 状態を確認
            UIFile fileOpposite;                        // 比較対象のファイル
            if (fileOppositeName == null && fileOppositeMark == null) {
                // 反対パスに該当なし
                InfoBox.Warning(Program.MainWindow, Resources.Msg_DiffNoOpposite);
                return null;
            } else if (fileOppositeName != null && fileOppositeMark != null) {
                // 反対パスに同名ファイルもマークファイルもある
                string targetPath = FileListViewTarget.FileList.DisplayDirectoryName + fileTarget.FileName;
                string oppNamePath = FileListViewOpposite.FileList.DisplayDirectoryName + fileOppositeName.FileName;
                string oppMarkPath = FileListViewOpposite.FileList.DisplayDirectoryName + fileOppositeMark.FileName;
                if (oppNamePath == oppMarkPath) {
                    fileOpposite = fileOppositeName;
                } else {
                    DiffToolOppositeTargetDialog dialog = new DiffToolOppositeTargetDialog(targetPath, oppNamePath, oppMarkPath);
                    DialogResult result = dialog.ShowDialog(Program.MainWindow);
                    if (result != DialogResult.OK) {
                        return null;
                    }
                    if (dialog.SelectName) {
                        fileOpposite = fileOppositeName;
                    } else {
                        fileOpposite = fileOppositeMark;
                    }
                }
            } else if (fileOppositeName != null) {
                // 反対パスに同名ファイルがある
                fileOpposite = fileOppositeName;
            } else {
                // 反対パスにマークファイルがある
                fileOpposite = fileOppositeMark;
            }
            if (fileOpposite.Attribute.IsDirectory != fileTarget.Attribute.IsDirectory) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_DiffFileDirOpposite);
                return null;
            }

            // 反対パスのファイルリストを作成
            List<SimpleFileDirectoryPath> destFileList = new List<SimpleFileDirectoryPath>();
            destFileList.Add(new SimpleFileDirectoryPath(FileListViewOpposite.FileList.DisplayDirectoryName + fileOpposite.FileName, fileOpposite.Attribute.IsDirectory, fileOpposite.Attribute.IsSymbolicLink));

            // 表示名を作成
            List<UIFile> markedList = FileListViewTarget.FileList.MarkFiles;
            string dispName = string.Format(Resources.LocalExec_DisplayNameDiffOpposite, fileTarget.FileName);
            TemporarySpaceDisplayName nameInfo = new TemporarySpaceDisplayName(dispName, IconImageListID.FileList_DiffOpposite);

            // コマンドラインを作成
            // command="C:\…\DiffTool.exe {0}"
            // program="C:\…\DiffTool.exe"
            // argument="{0}";
            IFileListContext srcFileListCtx = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // srcFileListCtx:ライフサイクル管理
            IFileListContext destFileListCtx = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewOpposite);            // destFileListCtx:ライフサイクル管理
            string program, argument;
            GenericFileStringUtils.SplitCommandLine(command, out program, out argument);    
            LocalExecuteBackgroundTask.GetProgramArgumentDelegate argDelegate = delegate(List<string> srcPathList, List<string> destPathList) {
                List<string> allFiles = new List<string>();
                allFiles.AddRange(srcFileListCtx.GetExecuteLocalPathList(srcPathList));
                allFiles.AddRange(destFileListCtx.GetExecuteLocalPathList(destPathList));
                string fileListCommand = GenericFileStringUtils.CreateCommandFiles(allFiles, false);
                return string.Format(argument, fileListCommand);
            };

            // バックグラウンドで開始
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(FileListViewOpposite.FileList.FileSystem.FileSystemId);
            string targetDir = FileListViewTarget.FileList.DisplayDirectoryName;
            string oppositeDir = FileListViewOpposite.FileList.DisplayDirectoryName;
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(FileListViewTarget.FileList, FileListViewTarget.FileList.MarkFiles, srcFileSystem, destFileListCtx);
            IFileProviderDest destProvider = new FileProviderDestMarked(oppositeDir, destFileSystem, destFileList, destFileListCtx);
            LocalExecuteBackgroundTask task = new LocalExecuteBackgroundTask(srcProvider, destProvider, uiTarget, program, argDelegate, nameInfo, targetDir, oppositeDir);
            Program.Document.BackgroundTaskManager.StartFileTask(task, true);
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.DiffOppositeCommand;
            }
        }
    }
}
