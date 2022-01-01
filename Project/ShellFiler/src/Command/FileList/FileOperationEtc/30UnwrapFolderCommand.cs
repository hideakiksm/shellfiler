using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Condition;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.FileList;
using ShellFiler.FileTask;
using ShellFiler.Properties;

namespace ShellFiler.Command.FileList.FileOperationEtc {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マークされたファイルとフォルダを、反対パスのフラットな階層にコピーします。
    //   書式 　 UnwrapFolder()
    //   引数  　なし
    //   戻り値　bool:コピーをバックグラウンドで開始したときtrue、コピーを開始できなかったときfalseを返します。
    //   対応Ver 2.2.1
    //=========================================================================================
    class UnwrapFolderCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public UnwrapFolderCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            return CopyExecute(FileListViewTarget, FileListViewOpposite, false);
        }

        //=========================================================================================
        // 機　能：コピーを実行する
        // 引　数：[in]fileListViewTarget   対象パスのファイル一覧
        // 　　　　[in]fileListViewOpposite 反対パスのファイル一覧
        // 　　　　[in]exMode               拡張モードでコピーするときtrue
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public static bool CopyExecute(FileListView fileListViewTarget, FileListView fileListViewOpposite, bool exMode) {
            // マークを確認
            bool markOk = BackgroundTaskCommandUtil.CheckMarkState(fileListViewTarget, Configuration.Current.MarklessCopy);
            if (!markOk) {
                return false;
            }

            // 処理開始条件をチェック
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(CopyBackgroundTask));
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
                CopyExStartDialog dialog = new CopyExStartDialog(fileListViewTarget.FileList.FileSystem.FileSystemId, fileListViewOpposite.FileList.FileSystem.FileSystemId);
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
            RefreshUITarget uiTarget = new RefreshUITarget(fileListViewTarget, fileListViewOpposite, RefreshUITarget.RefreshMode.RefreshOpposite, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(fileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(fileListViewOpposite.FileList.FileSystem.FileSystemId);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(fileListViewTarget);               // fileListContext:ライフサイクル管理
            IFileListContext destFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(fileListViewOpposite);            // fileListContext:ライフサイクル管理
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(fileListViewTarget.FileList, fileListViewTarget.FileList.MarkFiles, srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestSimple(fileListViewOpposite.FileList.DisplayDirectoryName, destFileSystem, destFileListContext);
            CopyMoveDeleteOption option = new CopyMoveDeleteOption(CopyMoveDeleteOption.CreateDefaultSameFileOperation(destFileSystem.FileSystemId), null, condition, attributeSetMode, null, null, true);
            CopyBackgroundTask copyTask = new CopyBackgroundTask(srcProvider, destProvider, uiTarget, option, null);
            Program.Document.BackgroundTaskManager.StartFileTask(copyTask, startTask);
            fileListViewTarget.FileListViewComponent.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.UnwrapFolderCommand;
            }
        }
    }
}