using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.FileOperation {

    //=========================================================================================
    // クラス：ファイルまたはフォルダを2重化する
    // ファイルまたはディレクトリの、名前または属性を変更します。
    //   書式 　 Duplicate()
    //   引数  　なし
    //   戻り値　bool:名前の変更をバックグラウンドで開始したときtrue、変更を開始できなかったときfalseを返します。
    //   対応Ver 2.1.0
    //=========================================================================================
    class DuplicateCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DuplicateCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (FileListViewTarget.FileList.Files.Count == 0) {
                return false;
            }
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }

            UIFile file = FileListViewTarget.FileList.Files[FileListComponentTarget.CursorLineNo];
            if (file.FileName == "..") {
                return false;
            }

            // ダイアログで新しい名前を入力
            DuplicateDialog dialog = new DuplicateDialog(file.Attribute.IsDirectory, file.FileName, FileListViewTarget.FileList.FileSystem.FileSystemId, FileListViewTarget.FileList.Files);
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK) {
                return false;
            }
            DuplicateFileInfo fileInfo = dialog.DuplicateFileInfoModified;

            // バックグラウンドタスクで名前の変更を実行
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshTarget, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            List<SimpleFileDirectoryPath> srcProviderFileList = new List<SimpleFileDirectoryPath>();
            srcProviderFileList.Add(new SimpleFileDirectoryPath(FileListViewTarget.FileList.DisplayDirectoryName + file.FileName, file.Attribute.IsDirectory, false));
            IFileProviderSrc srcProvider = new FileProviderSrcSpecified(srcProviderFileList, srcFileSystem, null);
            IFileProviderDest destProvider = new FileProviderDestSimple(FileListViewTarget.FileList.DisplayDirectoryName, srcFileSystem, null);
            AttributeSetMode attributeSetMode = fileInfo.AttributeSetMode;
            CopyMoveDeleteOption option = new CopyMoveDeleteOption(CopyMoveDeleteOption.CreateDefaultSameFileOperation(srcFileSystem.FileSystemId), null, null, attributeSetMode, null, fileInfo, false);
            DuplicateFileBackgroundTask duplicateTask = new DuplicateFileBackgroundTask(srcProvider, destProvider, uiTarget, option);
            Program.Document.BackgroundTaskManager.StartFileTask(duplicateTask, true);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.DuplicateCommand;
            }
        }
    }
}
