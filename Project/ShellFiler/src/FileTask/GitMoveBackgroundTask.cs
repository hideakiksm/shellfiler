using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.Management;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：Gitによるファイルの移動をバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class GitMoveBackgroundTask : AbstractFileBackgroundTask {
        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        // 転送元から見た転送先ディレクトリの相対パス
        private string m_relativeDest;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider  転送元ファイルの情報
        // 　　　　[in]destProvider 転送先ファイルの情報
        // 　　　　[in]refreshUi    作業完了時のUI更新方法
        // 　　　　[in]relativeDest 転送元から見た転送先ディレクトリの相対パス
        // 戻り値：なし
        //=========================================================================================
        public GitMoveBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, string relativeDest) : base(srcProvider, destProvider, refreshUi) {
            m_relativeDest = relativeDest;

            // 転送元
            string srcShort = BackgroundTaskPathInfo.CreateShortTextFileProviderSrc(FileProviderSrc, null);
            string srcDetail = BackgroundTaskPathInfo.CreateDetailTextFileProviderSrc(FileProviderSrc, null);

            // 転送先
            string destShort = FileProviderDest.DestFileSystem.GetFileName(GenericFileStringUtils.TrimLastSeparator(FileProviderDest.DestDirectoryName));
            string destDetail = FileProviderDest.DestDirectoryName;

            m_backgroundTaskPathInfo = new BackgroundTaskPathInfo(srcShort, srcDetail, destShort, destDetail);
        }

        //=========================================================================================
        // 機　能：タスクを実行する
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンドスレッドで実行する
        //=========================================================================================
        protected override void ExecuteTask() {
            string srcPath = FileProviderSrc.GetSrcPath(0).FilePath;

            FileProviderSrc.SrcFileSystem.BeginFileOperation(RequestContext, srcPath);
            FileProviderDest.DestFileSystem.BeginFileOperation(RequestContext, FileProviderDest.DestDirectoryName);

            try {
                MoveMarkFiles();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：マークされたファイルを移動する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void MoveMarkFiles() {
            FileOperationStatus status;
            int allCount = FileProviderSrc.SrcItemCount;
            for (int i = 0; i <FileProviderSrc.SrcItemCount; i++) {
                if (CheckSuspend()) {
                    return;
                }
                m_backgroundTaskPathInfo.SetProgressCount(i + 1, allCount);
                FireBackgroundtaskPathInfoProgressEvent();

                SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(i);
                LogFileOperationSetMarkInfo(pathInfo);
                string srcPath = pathInfo.FilePath;

                // ファイルを移動
                string destPath = FileProviderDest.DestDirectoryName;
                LogFileOperationStart(FileOperationType.MoveFile, srcPath, false);
                status = GitMove(srcPath, destPath);
                LogFileOperationEnd(status, null);

                if (IsCancel) {
                    return;
                }
            }
        }

        //=========================================================================================
        // 機　能：git mvコマンドを実行する
        // 引　数：[in]srcPath   転送元ファイルのフルパス
        // 　　　　[in]destPath  転送先のフルパス
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus GitMove(string srcPath, string destPath) {
            string srcFile = GenericFileStringUtils.GetFileName(srcPath);
            string currentDir = GenericFileStringUtils.GetDirectoryName(srcPath);

            string newName = GetNewName(srcFile);
            if (newName == null) {
                return FileOperationStatus.Canceled;
            }

            string command = "git mv " + srcFile + " " + m_relativeDest + "/" + newName;
            RetrieveFileDataTargetShellExecuteDummy dataTarget = new RetrieveFileDataTargetShellExecuteDummy();
            FileOperationStatus status = FileProviderSrc.SrcFileSystem.RemoteExecute(RequestContext, currentDir, command, null, true, true, dataTarget);
            return status;
        }
        
        //=========================================================================================
        // 機　能：新しいファイル名を入力する
        // 引　数：[in]fileName  現在のファイル名
        // 戻り値：新しいファイル名（null:キャンセル）
        //=========================================================================================
        private string GetNewName(string fileName) {
            object result;
            bool success = BaseThread.InvokeFunctionByMainThread(new GetNewNameDelegate(GetNewNameUI), out result, fileName);
            if (!success) {
                return null;
            }
            return (string)result;
        }
        private delegate string GetNewNameDelegate(string fileName);
        private static string GetNewNameUI(string fileName) {
            GitRenameDialog dialog = new GitRenameDialog(fileName);
            dialog.ShowDialog(Program.MainWindow);
            return dialog.NewFileName;
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.GitMove;
            }
        }

        //=========================================================================================
        // プロパティ：転送元/転送先の情報
        //=========================================================================================
        public override BackgroundTaskPathInfo PathInfo {
            get {
                return m_backgroundTaskPathInfo;
            }
        }
    }
}
