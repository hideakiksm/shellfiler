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
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.FileList;
using ShellFiler.FileTask;

namespace ShellFiler.Command.FileList.FileOperationEtc {

    //=========================================================================================
    // クラス：コマンドを実行する
    // カーソル上のファイルをgit mvコマンドでリネームします。
    //   書式 　 GitRename()
    //   引数  　なし
    //   戻り値　bool:移動をバックグラウンドで開始したときtrue、移動を開始できなかったときfalseを返します。
    //   対応Ver 3.0.0
    //=========================================================================================
    class GitRenameCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public GitRenameCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // マークを確認
            bool markOk = BackgroundTaskCommandUtil.CheckMarkState(FileListViewTarget, false);
            if (!markOk) {
                return false;
            }

            // 処理開始条件をチェック
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(GitRenameBackgroundTask));
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }

            // ファイル名を入力
            UIFile file = FileListViewTarget.FileList.Files[FileListComponentTarget.CursorLineNo];
            if (file.FileName == "..") {
                return false;
            }
            string fullPath = FileListViewTarget.FileList.DisplayDirectoryName + file.FileName;

            string orgFileName = GenericFileStringUtils.GetFileName(fullPath);
            GitRenameDialog dialog = new GitRenameDialog(orgFileName);
            DialogResult dialogResult = dialog.ShowDialog(Program.MainWindow);
            if (dialogResult != DialogResult.OK) {
                return false;
            }
            string newFileName = dialog.NewFileName;

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshTarget, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // srcFileListContext:ライフサイクル管理
            IFileProviderSrc srcProvider = new FileProviderSrcDummy(srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            GitRenameBackgroundTask task = new GitRenameBackgroundTask(srcProvider, destProvider, uiTarget, fullPath, orgFileName, newFileName);
            Program.Document.BackgroundTaskManager.StartFileTask(task, true);
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.GitRenameCommand;
            }
        }
    }
}
