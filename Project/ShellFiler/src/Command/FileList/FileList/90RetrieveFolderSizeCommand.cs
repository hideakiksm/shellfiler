using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.FileList {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マーク中のフォルダ配下のサイズの合計を取得します。
    //   書式 　 RetrieveFolderSize()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class RetrieveFolderSizeCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public RetrieveFolderSizeCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // マークを確認
            bool markOk = BackgroundTaskCommandUtil.CheckMarkState(FileListViewTarget, Configuration.Current.MarklessFodlerSize);
            if (!markOk) {
                return null;
            }

            // 処理開始条件をチェック
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return null;
            }

            // 条件を入力
            RetrieveFolderSizeCondition sizeCondition = new RetrieveFolderSizeCondition();
            RetrieveFolderSizeDialog dialog  = new RetrieveFolderSizeDialog(FileListViewTarget, FileListViewOpposite, sizeCondition);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return null;
            }
            sizeCondition = dialog.ResultSizeInfo;
            RefreshUITarget.RefreshMode refreshMode;
            if (sizeCondition.UseCache) {
                refreshMode = RefreshUITarget.RefreshMode.FolderSizeBoth;
            } else {
                refreshMode = RefreshUITarget.RefreshMode.FolderSizeBothAndClear;
            }

            // 処理開始条件をチェック
            canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(DeleteBackgroundTask));
            if (!canStart) {
                return null;
            }

            // 結果の準備
            string basePath = FileListViewTarget.FileList.DisplayDirectoryName;
            FileSystemID fileSystem = FileListViewTarget.FileList.FileSystem.FileSystemId;
            RetrieveFolderSizeResult folderSizeResult = new RetrieveFolderSizeResult(fileSystem, basePath, Configuration.Current.RetrieveFolderSizeKeepLowerDepth, Configuration.Current.RetrieveFolderSizeKeepLowerCount);
            Program.Document.RetrieveFolderSizeResult = folderSizeResult;

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, refreshMode, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(FileListViewTarget.FileList, FileListViewTarget.FileList.MarkFiles, srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            RetrieveFolderSizeBackgroundTask task = new RetrieveFolderSizeBackgroundTask(srcProvider, destProvider, uiTarget, sizeCondition, folderSizeResult);
            Program.Document.BackgroundTaskManager.StartFileTask(task, true);
            FileListViewTarget.FileListViewComponent.MarkAllFile(MarkAllFileMode.ClearAll, true, null);

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.RetrieveFolderSizeCommand;
            }
        }
    }
}
