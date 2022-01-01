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
    // マークされたファイルをgit mvコマンドで移動します。
    //   書式 　 GitMove()
    //   引数  　なし
    //   戻り値　bool:移動をバックグラウンドで開始したときtrue、移動を開始できなかったときfalseを返します。
    //   対応Ver 3.0.0
    //=========================================================================================
    class GitMoveCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public GitMoveCommand() {
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
            string relativePath;
            bool canStart = IsSameDirectoryRootLeftRight(FileListViewTarget, FileListViewOpposite, out relativePath);
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(GitMoveBackgroundTask));
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewOpposite.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshBoth, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(FileListViewOpposite.FileList.FileSystem.FileSystemId);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // srcFileListContext:ライフサイクル管理
            IFileListContext destFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewOpposite);            // destFileListContext:ライフサイクル管理
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(FileListViewTarget.FileList, FileListViewTarget.FileList.MarkFiles, srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestSimple(FileListViewOpposite.FileList.DisplayDirectoryName, destFileSystem, destFileListContext);
            GitMoveBackgroundTask moveTask = new GitMoveBackgroundTask(srcProvider, destProvider, uiTarget, relativePath);
            Program.Document.BackgroundTaskManager.StartFileTask(moveTask, true);
            FileListViewTarget.FileListViewComponent.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return true;
        }

        //=========================================================================================
        // 機　能：左右が共通のルートディレクトリを持つかどうかを返す
        // 引　数：[in]target       対象パスのファイル一覧
        // 　　　　[in]opposite     反対パスのファイル一覧
        //         [out]relativeOpp 反対パスを相対表現にして返す変数
        // 戻り値：共通のルートを持つときtrue
        //=========================================================================================
        private bool IsSameDirectoryRootLeftRight(FileListView target, FileListView opposite, out string relativeOpp) {
            relativeOpp = null;

            // 条件を確認
            string srcPath = target.FileList.DisplayDirectoryName;
            string destPath = opposite.FileList.DisplayDirectoryName;
            if (srcPath == destPath) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_GitMvSameDir);
                return false;
            }
            if (target.FileList.FileSystem.FileSystemId != FileSystemID.Windows || opposite.FileList.FileSystem.FileSystemId != FileSystemID.Windows) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_GitMvNotWindows);
                return false;
            }

            // パスを確認
            IFileSystem fileSystem = target.FileList.FileSystem;
            string srcRootDir, srcSubDir, destRootDir, destSubDir;
            fileSystem.SplitRootPath(srcPath, out srcRootDir, out srcSubDir);
            fileSystem.SplitRootPath(destPath, out destRootDir, out destSubDir);
            if (srcRootDir != destRootDir) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_GitMvDifferentRoot);
                return false;
            }
            srcSubDir = GenericFileStringUtils.RemoveLastDirectorySeparator(srcSubDir);
            destSubDir = GenericFileStringUtils.RemoveLastDirectorySeparator(destSubDir);

            // ディレクトリを相対表現に変更
            // \data\root\sub1\sub2
            // \data\root\sub3\sub4
            // out:../../sub3/sub4
            string[] srcSubList = srcSubDir.Split('\\');
            string[] destSubList = destSubDir.Split('\\');
            int minFolderCount = Math.Min(srcSubList.Length, destSubList.Length);
            int idxSame = 0;
            for (int i = 0; i < minFolderCount; i++) {
                if (!srcSubList[i].Equals(destSubList[i], StringComparison.CurrentCultureIgnoreCase)) {
                    break;
                }
                idxSame++;
            }
            List<string> relative = new List<string>();
            for (int i = idxSame; i < srcSubList.Length; i++) {
                relative.Add("..");
            }
            for (int i = idxSame; i < destSubList.Length; i++) {
                relative.Add(destSubList[i]);
            }
            relativeOpp = string.Join("/", relative.ToArray());
            
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.GitMoveCommand;
            }
        }
    }
}
