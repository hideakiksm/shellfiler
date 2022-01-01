using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.Management;
using ShellFiler.FileTask.Provider;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイルの結合をバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class CombineFileBackgroundTask : AbstractFileBackgroundTask {
        // 結合済みファイル名
        private string m_combineFileName;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider      転送元ファイルの情報
        // 　　　　[in]destProvider     転送先ファイルの情報
        // 　　　　[in]refreshUi        作業完了時のUI更新方法
        // 　　　　[in]combineFileName  結合済みファイル名
        // 戻り値：なし
        //=========================================================================================
        public CombineFileBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, string combineFileName) : base(srcProvider, destProvider, refreshUi) {
            CreateBackgroundTaskPathInfo();
            m_combineFileName = combineFileName;
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
                CopyMarkFiles();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：マークされたファイルをコピーする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CopyMarkFiles() {
            List<string> srcFilePathList = new List<string>();
            for (int i = 0; i <FileProviderSrc.SrcItemCount; i++) {
                SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(i);
                srcFilePathList.Add(pathInfo.FilePath);
            }
            FileSystemToFileSystem.CombineFile(RequestContext, srcFilePathList, m_combineFileName, this);
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.CombineFile;
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
