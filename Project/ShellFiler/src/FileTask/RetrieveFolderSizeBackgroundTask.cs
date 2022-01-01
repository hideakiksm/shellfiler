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
using ShellFiler.Document.Setting;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：フォルダサイズの取得をバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class RetrieveFolderSizeBackgroundTask : AbstractFileBackgroundTask {
        // フォルダサイズの取得条件
        private RetrieveFolderSizeCondition m_folderSizeCondition;

        // フォルダサイズの取得結果の格納先
        private RetrieveFolderSizeResult m_retrieveFolderSizeResult;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider  転送元ファイルの情報
        // 　　　　[in]destProvider 転送先ファイルの情報
        // 　　　　[in]refreshUi    作業完了時のUI更新方法
        // 　　　　[in]sizeCond     フォルダサイズの取得条件
        // 　　　　[in]folderResult フォルダサイズの取得結果を格納する先
        // 戻り値：なし
        //=========================================================================================
        public RetrieveFolderSizeBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, RetrieveFolderSizeCondition sizeCond, RetrieveFolderSizeResult folderSizeResult) :
                    base(srcProvider, destProvider, refreshUi) {
            m_folderSizeCondition = sizeCond;
            m_retrieveFolderSizeResult = folderSizeResult;
            CreateBackgroundTaskPathInfo();
        }

        //=========================================================================================
        // 機　能：転送元/転送先の情報を作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CreateBackgroundTaskPathInfo() {
            // 転送元
            string srcShort = BackgroundTaskPathInfo.CreateShortTextFileProviderSrc(FileProviderSrc, null);
            string srcDetail = BackgroundTaskPathInfo.CreateDetailTextFileProviderSrc(FileProviderSrc, null);

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

            try {
                RetrievtMarkFodlers();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：マークされたフォルダの情報を取得する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void RetrievtMarkFodlers() {
            int allCount = FileProviderSrc.SrcItemCount;
            for (int i = 0; i <FileProviderSrc.SrcItemCount; i++) {
                if (CheckSuspend()) {
                    return;
                }
                m_backgroundTaskPathInfo.SetProgressCount(i + 1, allCount);
                FireBackgroundtaskPathInfoProgressEvent();

                SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(i);
                if (pathInfo.IsDirectory && !pathInfo.IsSymbolicLink) {
                    LogFileOperationStart(FileOperationType.FolderSize, pathInfo.FilePath, true);
                    FileOperationStatus status = FileProviderSrc.SrcFileSystem.RetrieveFolderSize(RequestContext, pathInfo.FilePath, m_folderSizeCondition, m_retrieveFolderSizeResult, new FileProgressEventHandler(ProgressEventHandler));
                    LogFileOperationEnd(status);
                    if (IsCancel) {
                        return;
                    }
                }
            }

            m_retrieveFolderSizeResult.FinalAddResult();
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.RetrieveFolderSize;
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
