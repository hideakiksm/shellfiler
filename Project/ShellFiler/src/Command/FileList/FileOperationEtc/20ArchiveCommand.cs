using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;

namespace ShellFiler.Command.FileList.FileOperationEtc {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ファイルを圧縮します。
    //   書式 　 Archive()
    //   引数  　なし
    //   戻り値　bool:圧縮をバックグラウンドで開始したときtrue、圧縮を開始できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ArchiveCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ArchiveCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // 圧縮のダウンロードの案内
            if (!Program.Document.ArchiveFactory.IsSupportArchive()) {
                ArchiveDownloadDialog helpDialog = new ArchiveDownloadDialog();
                helpDialog.ShowDialog(Program.MainWindow);
                return false;
            }

            // マークを確認
            bool markOk = BackgroundTaskCommandUtil.CheckMarkState(FileListViewTarget, Configuration.Current.MarklessPack);
            if (!markOk) {
                return false;
            }

            // 仮想フォルダは非対応
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewOpposite.FileList.FileSystem.FileSystemId, false);
            if (!canStart) {
                return false;
            }

            // ファイル名を決定
            List<UIFile> fileList = FileListViewTarget.FileList.MarkFiles;
            string arcFileBase;
            if (fileList.Count == 1) {
                arcFileBase = fileList[0].FileName;
            } else {
                UIFile cursorFile = FileListViewTarget.FileList.Files[FileListComponentTarget.CursorLineNo];
                if (cursorFile.FileName == "..") {
                    arcFileBase = fileList[0].FileName;
                } else {
                    arcFileBase = cursorFile.FileName;
                }
            }

            // 条件を入力
            ArchiveSetting option;
            if (Configuration.Current.ArchiveSettingDefault == null) {
                option = (ArchiveSetting)(Program.Document.UserGeneralSetting.ArchiveSetting).Clone();
            } else {
                option = (ArchiveSetting)(Configuration.Current.ArchiveSettingDefault).Clone();
            }
            string dirOpposite = FileListViewOpposite.FileList.DisplayDirectoryName;
            bool isRemote = FileListViewTarget.FileList.FileSystem.LocalExecuteDownloadRequired;
            bool sameSystem = BackgroundTaskCommandUtil.CheckSameFileSystem(FileListViewTarget, FileListViewOpposite);
            bool canRemoteArchive = (isRemote && sameSystem);
            ArchiveDialog dialog = new ArchiveDialog(option, arcFileBase, dirOpposite, canRemoteArchive, FileListViewTarget.FileList.FileListContext, FileListViewOpposite.FileList.Files);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return false;
            }
            ArchiveParameter archiveParameter = dialog.ResultArchiveParameter;
            Program.Document.UserGeneralSetting.ArchiveSetting = (ArchiveSetting)(dialog.ArchiveSetting.Clone());

            // 処理開始条件をチェック
            canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(LocalArchiveBackgroundTask));
            if (!canStart) {
                return false;
            }

            // タスクを登録
            if (archiveParameter.ExecuteMethod == ArchiveExecuteMethod.Local7z) {
                // ローカルでsevenzip.dllを使って圧縮
                RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshOpposite, RefreshUITarget.RefreshOption.None);
                FileSystemFactory factory = Program.Document.FileSystemFactory;
                IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
                IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(FileListViewOpposite.FileList.FileSystem.FileSystemId);
                IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
                IFileListContext destFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewOpposite);            // fileListContext:ライフサイクル管理
                IFileProviderSrc srcProvider = new FileProviderSrcMarked(FileListViewTarget.FileList, FileListViewTarget.FileList.MarkFiles, srcFileSystem, srcFileListContext);
                IFileProviderDest destProvider = new FileProviderDestSimple(FileListViewOpposite.FileList.DisplayDirectoryName, destFileSystem, destFileListContext);
                string targetDir = FileListViewTarget.FileList.DisplayDirectoryName;
                LocalArchiveBackgroundTask packTask = new LocalArchiveBackgroundTask(LocalArchiveBackgroundTask.OperationMode.Archive, srcProvider, destProvider, uiTarget, targetDir, ExtractPathMode.None, archiveParameter, null);
                Program.Document.BackgroundTaskManager.StartFileTask(packTask, true);
                FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            } else {
                // リモートで圧縮
                ShellCommandDictionary commandDic = SSHUtils.GetShellCommandDictionary(FileListViewTarget.FileList.FileListContext);
                Encoding encoding = SSHUtils.GetEncoding(FileListViewTarget.FileList.FileListContext);
                string srcPath = FileListViewTarget.FileList.DisplayDirectoryName;
                string destPath = FileListViewOpposite.FileList.DisplayDirectoryName;
                string command = RemoteShellCommandFactory.CreateCommand(archiveParameter, srcPath, destPath, commandDic, FileListViewTarget.FileList.MarkFiles);
                List<OSSpecLineExpect> commandExpect = archiveParameter.RemoteShellOption.CommandExpect;

                FileSystemFactory factory = Program.Document.FileSystemFactory;
                IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
                IRetrieveFileDataTarget dataTarget = new RetrieveFileDataTargetShellExecute(encoding);
                RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshOpposite, RefreshUITarget.RefreshOption.None);
                IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
                IFileProviderSrc srcProvider = new FileProviderSrcDummy(srcFileSystem, srcFileListContext);
                IFileProviderDest destProvider = new FileProviderDestDummy();
                ShellExecuteBackgroundTask execTask = new ShellExecuteBackgroundTask(srcProvider, destProvider, uiTarget, FileListViewTarget.FileList.DisplayDirectoryName, command, commandExpect, true, false, dataTarget);
                Program.Document.BackgroundTaskManager.StartFileTask(execTask, true);
                FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ArchiveCommand;
            }
        }
    }
}
