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
using ShellFiler.FileTask.Management;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ディレクトリの作成をバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class MakeDirectoryBackgroundTask : AbstractFileBackgroundTask {
        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider  転送元ファイルの情報
        // 　　　　[in]destProvider 転送先ファイルの情報
        // 　　　　[in]refreshUi    作業完了時のUI更新方法
        // 戻り値：なし
        //=========================================================================================
        public MakeDirectoryBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi) : base(srcProvider, destProvider, refreshUi) {
            CreateBackgroundTaskPathInfo();
        }

        //=========================================================================================
        // 機　能：転送元/転送先の情報を作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CreateBackgroundTaskPathInfo() {
            // 転送元
            string srcDirPath = FileProviderSrc.GetSrcPath(0).FilePath;
            string srcShort = srcDirPath;
            string srcDetail = srcDirPath;

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
            SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(0);
            FileProviderSrc.SrcFileSystem.BeginFileOperation(RequestContext, pathInfo.FilePath);

            try {
                StartMakeDirectory();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：ディレクトリ作成を開始する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void StartMakeDirectory() {
            string srcDirPath = FileProviderSrc.GetSrcPath(0).FilePath;
            string filePath = FileProviderSrc.SrcFileSystem.GetDirectoryName(srcDirPath);
            string newName = FileProviderSrc.SrcFileSystem.GetFileName(srcDirPath);

            FileOperationStatus status = FileProviderSrc.SrcFileSystem.CreateDirectory(RequestContext, filePath, newName, true);
            LogFileOperationStart(FileOperationType.MakeDir, srcDirPath, true);
            LogFileOperationEnd(status);
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.MakeDir;
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
