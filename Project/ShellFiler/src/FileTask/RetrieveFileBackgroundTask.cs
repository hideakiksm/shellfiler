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
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.Management;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.Properties;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイルの読み込みをバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class RetrieveFileBackgroundTask : AbstractFileBackgroundTask {
        // ファイルアクセスの結果を返すオブジェクト
        private AccessibleFile m_accessibleFile;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider   転送元ファイルの情報
        // 　　　　[in]destProvider  転送先ファイルの情報
        // 　　　　[in]refreshUi     作業完了時のUI更新方法
        // 　　　　[in]accessWrapper ファイルアクセスの結果を返すオブジェクト
        // 戻り値：なし
        //=========================================================================================
        public RetrieveFileBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, AccessibleFile accessWrapper)
            : base(srcProvider, destProvider, refreshUi) {
            m_accessibleFile = accessWrapper;
            CreateBackgroundTaskPathInfo();
        }

        //=========================================================================================
        // 機　能：転送元/転送先の情報を作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CreateBackgroundTaskPathInfo() {
            // 転送元
            string dispName = m_accessibleFile.DisplayName;
            string srcShort, srcDetail;
            if (dispName == null) {
                srcShort = GenericFileStringUtils.GetFileName(m_accessibleFile.FilePath);
                srcDetail = m_accessibleFile.FilePath;
            } else {
                srcShort = StringUtils.MakeOmittedString(dispName, 16);
                srcDetail = dispName;
            }

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
            FileProviderSrc.SrcFileSystem.BeginFileOperation(RequestContext, m_accessibleFile.FilePath);

            try {
                StartLoad();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：ファイルの取得を開始する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void StartLoad() {
            FileOperationStatus status = FileProviderSrc.SrcFileSystem.RetrieveFileChunk(RequestContext, m_accessibleFile);
            if (!status.Succeeded) {
                string message = status.Message;
                if (status == FileOperationStatus.Fail) {
                    message = Resources.FileViewer_CanNotRead;
                }
                m_accessibleFile.AddCompleted(RetrieveDataLoadStatus.Failed, message);
            }
            m_accessibleFile.FireEvent(true);
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.Retrieve;
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
