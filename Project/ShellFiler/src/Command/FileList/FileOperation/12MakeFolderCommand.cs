using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.FileTask;

namespace ShellFiler.Command.FileList.FileOperation {

    //=========================================================================================
    // クラス：コマンドを実行する
    // フォルダ作成のダイアログを表示して、入力された名前のフォルダを作成します。
    //   書式 　 MakeFolder()
    //   引数  　なし
    //   戻り値　bool:フォルダの作成をバックグラウンドで開始したときtrue、作成を開始できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class MakeFolderCommand : FileListActionCommand {
        // 作成するディレクトリのフルパス
        private string m_targetDirectory;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MakeFolderCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }

            // ディレクトリ名を入力
            string current = FileListViewTarget.FileList.DisplayDirectoryName;
            MakeDirectoryDialog dialog = new MakeDirectoryDialog();
            dialog.Initialize(current, srcFileSystem);
            DialogResult dialogResult = dialog.ShowDialog(Program.MainWindow);
            if (dialogResult == DialogResult.Cancel) {
                return false;
            }
            string newDirName = dialog.NewDirectoryName;
            bool isMoveCurrent = dialog.IsMoveCurrent;
            List<SimpleFileDirectoryPath> dirNameList = new List<SimpleFileDirectoryPath>();
            dirNameList.Add(new SimpleFileDirectoryPath(current + newDirName, true, false));
            RefreshUITarget uiTarget;
            if (isMoveCurrent) {
                m_targetDirectory = current + newDirName;
                uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            } else {
                m_targetDirectory = null;
                uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshTarget, RefreshUITarget.RefreshOption.None);
            }

            // 処理開始条件をチェック
            canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(MakeDirectoryBackgroundTask));
            if (!canStart) {
                return false;
            }

            // タスクを登録
            uiTarget.PreRefreshEvent += new RefreshUITarget.PreRefreshDelegate(UITarget_PreRefresh);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
            IFileProviderSrc srcProvider = new FileProviderSrcSpecified(dirNameList, srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            MakeDirectoryBackgroundTask task = new MakeDirectoryBackgroundTask(srcProvider, destProvider, uiTarget);
            Program.Document.BackgroundTaskManager.StartFileTask(task, true);
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);

            return true;
        }

        //=========================================================================================
        // 機　能：処理完了後、ディレクトリを切り替える
        // 引　数：[in]successCount  成功件数
        // 　　　　[in]skipCount     スキップ件数
        // 　　　　[in]failCount     失敗件数
        // 戻り値：なし
        // メ　モ：ユーザーインターフェーススレッドで実行される
        //=========================================================================================
        public void UITarget_PreRefresh(int successCount, int skipCount, int failCount) {
            if (failCount == 0 && m_targetDirectory != null) {
                ChdirCommand.ChangeDirectory(FileListViewTarget, new ChangeDirectoryParam.Direct(m_targetDirectory));
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.MakeFolderCommand;
            }
        }
    }
}
