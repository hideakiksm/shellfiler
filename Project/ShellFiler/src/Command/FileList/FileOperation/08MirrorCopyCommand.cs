using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.FileTask;
using ShellFiler.Properties;

namespace ShellFiler.Command.FileList.FileOperation {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ミラーコピーを行います。反対パスにファイルやフォルダをコピーしますが、反対パスにしかないファイルは削除します。
    //   書式 　 MirrorCopy()
    //   引数  　なし
    //   戻り値　bool:ミラーコピーをバックグラウンドで開始したときtrue、コピーを開始できなかったときfalseを返します。
    //   対応Ver 1.3.0
    //=========================================================================================
    class MirrorCopyCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MirrorCopyCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // マークを確認
            bool markOk = BackgroundTaskCommandUtil.CheckMarkState(FileListViewTarget, Configuration.Current.MarklessCopy);
            if (!markOk) {
                return false;
            }

            // 処理開始条件をチェック
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(MirrorCopyBackgroundTask));
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

            // 条件を入力
            AttributeSetMode attributeSetMode = (AttributeSetMode)(Configuration.Current.TransferAttributeSetMode.Clone());
            MirrorCopyDialog dialog = new MirrorCopyDialog(attributeSetMode, FileListViewTarget.FileList.FileSystem.FileSystemId, FileListViewOpposite.FileList.FileSystem.FileSystemId);
            dialog.InitializeByMarkFile(FileListViewTarget.FileList, FileListViewOpposite.FileList);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return false;
            }
            MirrorCopyOption mirrorOption = dialog.MirrorCopyOption;

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshOpposite, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(FileListViewOpposite.FileList.FileSystem.FileSystemId);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
            IFileListContext destFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewOpposite);            // fileListContext:ライフサイクル管理
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(FileListViewTarget.FileList, FileListViewTarget.FileList.MarkFiles, srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestSimple(FileListViewOpposite.FileList.DisplayDirectoryName, destFileSystem, destFileListContext);
            CopyMoveDeleteOption option = new CopyMoveDeleteOption(null, null, null, null, mirrorOption, null, false);
            MirrorCopyBackgroundTask copyTask = new MirrorCopyBackgroundTask(srcProvider, destProvider, uiTarget, option, null);
            Program.Document.BackgroundTaskManager.StartFileTask(copyTask, true);
            FileListViewTarget.FileListViewComponent.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.MirrorCopyCommand;
            }
        }
    }
}
