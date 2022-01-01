using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.FileList;
using ShellFiler.FileTask;

namespace ShellFiler.Command.FileList.FileOperation {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マークされたファイルを反対パスに移動します。
    //   書式 　 Move()
    //   引数  　なし
    //   戻り値　bool:移動をバックグラウンドで開始したときtrue、移動を開始できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class MoveCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MoveCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            return MoveExecute(FileListViewTarget, FileListViewOpposite, false);
        }

        //=========================================================================================
        // 機　能：移動を実行する
        // 引　数：[in]fileListViewTarget   対象パスのファイル一覧
        // 　　　　[in]fileListViewOpposite 反対パスのファイル一覧
        // 　　　　[in]exMode               拡張モードでコピーするときtrue
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public static bool MoveExecute(FileListView fileListViewTarget, FileListView fileListViewOpposite, bool exMode) {
            // マークを確認
            bool markOk = BackgroundTaskCommandUtil.CheckMarkState(fileListViewTarget, Configuration.Current.MarklessMove);
            if (!markOk) {
                return false;
            }

            // 処理開始条件をチェック
            if (IsSameDirectoryLeftRight(fileListViewTarget, fileListViewOpposite)) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_MoveSourceDestIsSame);
                return false;
            }
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(MoveBackgroundTask));
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, fileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, fileListViewOpposite.FileList.FileSystem.FileSystemId, false);
            if (!canStart) {
                return false;
            }

            // 条件を入力
            bool startTask = true;
            CompareCondition condition = null;
            AttributeSetMode attributeSetMode = (AttributeSetMode)(Configuration.Current.TransferAttributeSetMode.Clone());
            if (exMode) {
                MoveExStartDialog dialog = new MoveExStartDialog(fileListViewTarget.FileList.FileSystem.FileSystemId, fileListViewOpposite.FileList.FileSystem.FileSystemId);
                dialog.InitializeByMarkFile(fileListViewTarget.FileList, fileListViewOpposite.FileList);
                DialogResult result = dialog.ShowDialog(Program.MainWindow);
                if (result != DialogResult.OK) {
                    return false;
                }
                condition = dialog.TransferCondition;
                attributeSetMode = dialog.AttributeSetMode;
                startTask = !dialog.SuspededTask;
            }

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(fileListViewTarget, fileListViewOpposite, RefreshUITarget.RefreshMode.RefreshBoth, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(fileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(fileListViewOpposite.FileList.FileSystem.FileSystemId);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(fileListViewTarget);               // fileListContext:ライフサイクル管理
            IFileListContext destFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(fileListViewOpposite);            // fileListContext:ライフサイクル管理
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(fileListViewTarget.FileList, fileListViewTarget.FileList.MarkFiles, srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestSimple(fileListViewOpposite.FileList.DisplayDirectoryName, destFileSystem, destFileListContext);
            CopyMoveDeleteOption option = new CopyMoveDeleteOption(CopyMoveDeleteOption.CreateDefaultSameFileOperation(destFileSystem.FileSystemId), null, condition, attributeSetMode, null, null, false);
            MoveBackgroundTask moveTask = new MoveBackgroundTask(srcProvider, destProvider, uiTarget, option, null);
            Program.Document.BackgroundTaskManager.StartFileTask(moveTask, startTask);
            fileListViewTarget.FileListViewComponent.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return true;
        }

        //=========================================================================================
        // 機　能：左右が同じディレクトリかどうかを返す
        // 引　数：[in]target   対象パスのファイル一覧
        // 　　　　[in]opposite 反対パスのファイル一覧
        // 戻り値：左右が同じディレクトリのときtrue
        //=========================================================================================
        private static bool IsSameDirectoryLeftRight(FileListView target, FileListView opposite) {
            string srcPath = target.FileList.DisplayDirectoryName;
            string destPath = opposite.FileList.DisplayDirectoryName;
            if (srcPath == destPath) {
                return true;
            }
            // 違っていれば小文字にして比較
            if (FileSystemID.IgnoreCaseFolderPath(target.FileList.FileSystem.FileSystemId)) {
                srcPath = srcPath.ToLower();
            }
            if (FileSystemID.IgnoreCaseFolderPath(opposite.FileList.FileSystem.FileSystemId)) {
                destPath = destPath.ToLower();
            }
            if (srcPath == destPath) {
                return true;
            }

            return false;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.MoveCommand;
            }
        }
    }
}
