using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.FileTask;
using ShellFiler.Properties;

namespace ShellFiler.Command.FileList.FileOperationEtc {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ファイルを展開します。
    //   書式 　 Extract()
    //   引数  　なし
    //   戻り値　bool:展開をバックグラウンドで開始したときtrue、展開を開始できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ExtractCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ExtractCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // マークを確認
            bool markOk = BackgroundTaskCommandUtil.CheckMarkState(FileListViewTarget, Configuration.Current.MarklessUnpack);
            if (!markOk) {
                return false;
            }

            // 処理開始条件をチェック
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(LocalArchiveBackgroundTask));
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewOpposite.FileList.FileSystem.FileSystemId, false);
            if (!canStart) {
                return false;
            }

            string clipString = OSUtils.GetClipboardString();

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshOpposite, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(FileListViewOpposite.FileList.FileSystem.FileSystemId);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
            IFileListContext destFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewOpposite);            // fileListContext:ライフサイクル管理
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(FileListViewTarget.FileList, FileListViewTarget.FileList.MarkFiles, srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestSimple(FileListViewOpposite.FileList.DisplayDirectoryName, destFileSystem, destFileListContext);
            string targetDir = FileListViewTarget.FileList.DisplayDirectoryName;
            ExtractPathMode extractPathMode = Configuration.Current.ArchiveExtractPathMode;
            LocalArchiveBackgroundTask unpackTask = new LocalArchiveBackgroundTask(LocalArchiveBackgroundTask.OperationMode.Extract, srcProvider, destProvider, uiTarget, targetDir, extractPathMode, null, clipString);
            Program.Document.BackgroundTaskManager.StartFileTask(unpackTask, true);
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ExtractCommand;
            }
        }
    }
}
