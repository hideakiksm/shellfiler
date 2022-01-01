using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Management;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;
using ShellFiler.Util;
using ShellFiler.Properties;
using ShellFiler.FileTask.DataObject;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：外部アプリケーションをバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class ShellExecuteBackgroundTask : AbstractFileBackgroundTask {
        // 実行時のカレントとするパス
        private string m_targetPath;

        // 実行するコマンドライン
        private string m_command;

        // エラー発生時の期待値
        private List<OSSpecLineExpect> m_errorExpect;

        // 標準出力をログに中継するときtrue
        private bool m_relayOutLog;

        // 標準エラー出力をログに中継するときtrue（falseのときエラー処理）
        private bool m_relayErrLog;
        
        // 取得データの格納先
        private IRetrieveFileDataTarget m_retrieveDataTarget;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider   転送元ファイルの情報
        // 　　　　[in]destProvider  転送先ファイルの情報
        // 　　　　[in]refreshUi     作業完了時のUI更新方法
        // 　　　　[in]targetPath    実行時のカレントとするパス
        // 　　　　[in]command       実行するコマンドライン
        // 　　　　[in]errorExpect   エラー発生時の期待値
        // 　　　　[in]relayOutLog   標準出力をログに中継するときtrue
        // 　　　　[in]relayErrLog   標準エラー出力をログに中継するときtrue
        // 　　　　[in]dataTarget    取得データの格納先
        // 戻り値：なし
        //=========================================================================================
        public ShellExecuteBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, string targetPath, string command, List<OSSpecLineExpect> errorExpect, bool relayOutLog, bool relayErrLog, IRetrieveFileDataTarget dataTarget) : base(srcProvider, destProvider, refreshUi) {
            m_targetPath = targetPath;
            m_command = command;
            m_errorExpect = errorExpect;
            m_relayOutLog = relayOutLog;
            m_relayErrLog = relayErrLog;
            m_retrieveDataTarget = dataTarget;
            CreateBackgroundTaskPathInfo();
        }

        //=========================================================================================
        // 機　能：転送元/転送先の情報を作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CreateBackgroundTaskPathInfo() {
            // 転送元
            string program;
            string argument;
            GenericFileStringUtils.SplitCommandLine(m_command, out program, out argument);
            string srcShort = GenericFileStringUtils.GetFileName(program);
            string srcDetail = m_command;

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
            FileProviderSrc.SrcFileSystem.BeginFileOperation(RequestContext, m_targetPath);

            try {
                StartExecute();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：シェルプログラムの実行を開始する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void StartExecute() {
            FileOperationStatus status = FileProviderSrc.SrcFileSystem.RemoteExecute(RequestContext, m_targetPath, m_command, m_errorExpect, m_relayOutLog, m_relayErrLog, m_retrieveDataTarget);
            if (!status.Succeeded) {
                string program;
                string argument;
                GenericFileStringUtils.SplitCommandLine(m_command, out program, out argument);    
                string programFile = GenericFileStringUtils.GetFileName(program);

                LogLineSimple logLine = new LogLineSimple(LogColor.Error, Resources.ShellExec_ProcessError, programFile);
                Program.LogWindow.RegistLogLine(logLine);
                m_retrieveDataTarget.AddCompleted(RetrieveDataLoadStatus.Failed, logLine.Message);
            }
            m_retrieveDataTarget.FireEvent(true);
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.ShellExecute;
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
