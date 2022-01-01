using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.FileTask;
using ShellFiler.FileViewer;

namespace ShellFiler.Command.FileList.Tools {

    //=========================================================================================
    // クラス：コマンドを実行する
    // カーソル上のファイルをファイルビューアで開きます。
    //   書式 　 FileViewer()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class FileViewerCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileViewerCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 処理開始条件をチェック
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(RetrieveFileBackgroundTask));
            if (!canStart) {
                return null;
            }
            if (FileListViewTarget.FileList.Files.Count == 0) {
                return null;
            }
            if (FileListViewTarget.FileList.Files[FileListComponentTarget.CursorLineNo].Attribute.IsDirectory) {
                return null;
            }

            // 読み込み対象を準備
            IFileSystem srcFileSystem = FileListViewTarget.FileList.FileSystem;
            string filePath = FileListViewTarget.FileList.DisplayDirectoryName + FileListViewTarget.FileList.Files[FileListComponentTarget.CursorLineNo].FileName;
            int maxSize = Configuration.Current.TextViewerMaxFileSize;
            AccessibleFile accessibleFile = new AccessibleFile(filePath, srcFileSystem.FileSystemId, null, false, -1, maxSize, false, AccessibleFile.MIN_PARSE_START_SIZE, null);

            // バックグラウンドタスクを準備
            IFileListContext fileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // virtualInfo:ライフサイクル管理
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            IFileProviderSrc srcProvider = new FileProviderSrcDummy(srcFileSystem, fileListContext);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            RetrieveFileBackgroundTask task = new RetrieveFileBackgroundTask(srcProvider, destProvider, uiTarget, accessibleFile);
            accessibleFile.LoadingTaskId = task.TaskId;

            // ファイルビューアを開く
            FileViewerForm fileViewer = new FileViewerForm(FileViewerForm.ViewerMode.Auto, accessibleFile, task.TaskId);
            fileViewer.Show();

            // 読み込み開始
            Program.Document.BackgroundTaskManager.StartFileTask(task, true);

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.FileViewerCommand;
            }
        }
    }
}
