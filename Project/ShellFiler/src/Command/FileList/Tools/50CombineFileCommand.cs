using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.UI.Dialog;
using ShellFiler.FileViewer;
using ShellFiler.FileViewer.HTTPResponseViewer;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.DataObject;
using ShellFiler.GraphicsViewer;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.Locale;

namespace ShellFiler.Command.FileList.Tools {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マークファイルを１つのファイルに結合します。
    //   書式 　 CombineFile()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.1.0
    //=========================================================================================
    class CombineFileCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CombineFileCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // 処理開始条件をチェック
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(CombineFileBackgroundTask));
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewOpposite.FileList.FileSystem.FileSystemId, false);
            if (!canStart) {
                return false;
            }

            // マーク状態をチェック
            if (FileListViewTarget.FileList.Files.Count == 0) {
                return null;
            }
            if (FileListViewTarget.FileList.MarkedDirectoryCount == 1) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CombineDirectoryMarked);
                return null;
            }
            if (FileListViewTarget.FileList.MarkedFileCount <= 1) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CombineFileCount);
                return null;
            }

            // 仮想フォルダの場合は実行準備
            bool success = PrepareVirtualFolderCommand.ReadyForVirtualFolder(FileListViewTarget);
            if (!success) {
                return false;
            }

            // 結合条件を入力
            CombineFileDialog dialog = new CombineFileDialog(FileListViewTarget.FileList, FileListViewOpposite.FileList); 
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return null;
            }
            List<UIFile> markFiles = dialog.FileList;
            string combineFilePath = FileListViewOpposite.FileList.DisplayDirectoryName + dialog.CombineFileName;

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshOpposite, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(FileListViewOpposite.FileList.FileSystem.FileSystemId);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
            IFileListContext destFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewOpposite);            // fileListContext:ライフサイクル管理
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(FileListViewTarget.FileList, markFiles, srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestSimple(FileListViewOpposite.FileList.DisplayDirectoryName, destFileSystem, destFileListContext);
            CombineFileBackgroundTask combineTask = new CombineFileBackgroundTask(srcProvider, destProvider, uiTarget, combineFilePath);
            Program.Document.BackgroundTaskManager.StartFileTask(combineTask, true);
            FileListViewTarget.FileListViewComponent.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.CombineFileCommand;
            }
        }
    }
}
