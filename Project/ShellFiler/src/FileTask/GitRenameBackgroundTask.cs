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
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.Properties;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイルのリネームをバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class GitRenameBackgroundTask : AbstractFileBackgroundTask {
        // 変更前のフルパス
        private string m_fullPath;

        // 変更前の名前
        private string m_orgFileName;

        // 変更後の名前
        private string m_newFileName;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider   転送元ファイルの情報
        // 　　　　[in]destProvider  転送先ファイルの情報
        // 　　　　[in]refreshUi     作業完了時のUI更新方法
        // 　　　　[in]fullPath      変更前のフルパス
        // 　　　　[in]orgFileName   変更前の名前
        // 　　　　[in]newFileName   変更後の名前
        // 戻り値：なし
        //=========================================================================================
        public GitRenameBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, string fullPath, string orgFileName, string newFileName) : base(srcProvider, destProvider, refreshUi) {
            m_fullPath = fullPath;
            m_orgFileName = orgFileName;
            m_newFileName = newFileName;
            CreateBackgroundTaskPathInfo();
        }
        
        //=========================================================================================
        // 機　能：転送元/転送先の情報を作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CreateBackgroundTaskPathInfo() {
            // 転送元
            string srcShort = m_orgFileName;
            string srcDetail = m_orgFileName + Resources.DlgTaskMan_Arrow + m_newFileName;

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
            FileProviderSrc.SrcFileSystem.BeginFileOperation(RequestContext, m_fullPath);

            try {
                StartRename();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：ファイル名の変更を開始する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void StartRename() {
            FileOperationType type = FileOperationType.RenameFile;
            LogFileOperationStart(type, m_fullPath, false);

            string currentDir = GenericFileStringUtils.GetDirectoryName(m_fullPath);
            FileOperationStatus status = GitRename(currentDir, m_orgFileName, m_newFileName);
            LogFileOperationEnd(status);
        }

        //=========================================================================================
        // 機　能：git mvコマンドを実行する
        // 引　数：[in]orgFileName  元のファイル名
        // 引　数：[in]newFileName  新しいファイル名
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus GitRename(string currentDir, string orgFileName, string newFileName) {
            string command = "git mv " + orgFileName + " " + newFileName;
            RetrieveFileDataTargetShellExecuteDummy dataTarget = new RetrieveFileDataTargetShellExecuteDummy();
            FileOperationStatus status = FileProviderSrc.SrcFileSystem.RemoteExecute(RequestContext, currentDir, command, null, true, true, dataTarget);
            return status;
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.Rename;
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
