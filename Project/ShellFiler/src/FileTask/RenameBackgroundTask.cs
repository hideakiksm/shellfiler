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
    class RenameBackgroundTask : AbstractFileBackgroundTask {
        // 変更前のフルパス
        private string m_fullPath;

        // 入力前の名前変更情報
        private RenameFileInfo m_orgRenameInfo;

        // 入力後の名前変更情報
        private RenameFileInfo m_newRenameInfo;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider   転送元ファイルの情報
        // 　　　　[in]destProvider  転送先ファイルの情報
        // 　　　　[in]refreshUi     作業完了時のUI更新方法
        // 　　　　[in]fullPath      変更前のフルパス
        // 　　　　[in]orgRenameInfo 入力前の名前変更情報
        // 　　　　[in]newRenameInfo 入力後の名前変更情報
        // 戻り値：なし
        //=========================================================================================
        public RenameBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, string fullPath, RenameFileInfo orgRenameInfo, RenameFileInfo newRenameInfo) : base(srcProvider, destProvider, refreshUi) {
            m_fullPath = fullPath;
            m_orgRenameInfo = orgRenameInfo;
            m_newRenameInfo = newRenameInfo;
            CreateBackgroundTaskPathInfo();
        }
        
        //=========================================================================================
        // 機　能：転送元/転送先の情報を作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CreateBackgroundTaskPathInfo() {
            // 転送元
            string srcShort = m_orgRenameInfo.FileName;
            string srcDetail = m_orgRenameInfo.FileName + Resources.DlgTaskMan_Arrow + m_newRenameInfo.FileName;

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
            FileOperationType type;
            if (m_orgRenameInfo.IsDirectory) {
                type = FileOperationType.RenameDir;
            } else {
                type = FileOperationType.RenameFile;
            }
            LogFileOperationStart(type, m_fullPath, false);
            FileOperationStatus status = FileProviderSrc.SrcFileSystem.Rename(RequestContext, m_fullPath, m_orgRenameInfo, m_newRenameInfo);
            LogFileOperationEnd(status);
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
