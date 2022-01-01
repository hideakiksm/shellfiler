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
    // クラス：Gitによるファイルの追加をバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class GitAddBackgroundTask : AbstractFileBackgroundTask {
        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider  転送元ファイルの情報
        // 　　　　[in]destProvider 転送先ファイルの情報
        // 　　　　[in]refreshUi    作業完了時のUI更新方法
        // 戻り値：なし
        //=========================================================================================
        public GitAddBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi) : base(srcProvider, destProvider, refreshUi) {
            // 転送元
            string srcShort = BackgroundTaskPathInfo.CreateShortTextFileProviderSrc(FileProviderSrc, null);
            string srcDetail = "";

            // 転送先
            string destShort = "";
            string destDetail = "";

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
                AddMarkFiles();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：マークされたファイルを追加する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void AddMarkFiles() {
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

                LogFileOperationStart(FileOperationType.AddFile, srcPath, false);
                status = GitAdd(srcPath);
                LogFileOperationEnd(status, null);

                if (IsCancel) {
                    return;
                }
            }
        }

        //=========================================================================================
        // 機　能：git addコマンドを実行する
        // 引　数：[in]srcPath   転送元ファイルのフルパス
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus GitAdd(string srcPath) {
            string srcFile = GenericFileStringUtils.GetFileName(srcPath);
            string currentDir = GenericFileStringUtils.GetDirectoryName(srcPath);


            string command = "git add " + srcFile;
            RetrieveFileDataTargetShellExecuteDummy dataTarget = new RetrieveFileDataTargetShellExecuteDummy();
            FileOperationStatus status = FileProviderSrc.SrcFileSystem.RemoteExecute(RequestContext, currentDir, command, null, true, true, dataTarget);
            return status;
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.GitAdd;
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
