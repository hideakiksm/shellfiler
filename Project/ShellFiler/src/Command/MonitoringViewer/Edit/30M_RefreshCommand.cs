using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.MonitoringViewer;
using ShellFiler.MonitoringViewer.ProcessMonitor;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.MonitoringViewer;

namespace ShellFiler.Command.MonitoringViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 最新の状態に更新します。
    //   書式 　 M_Refresh()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class M_RefreshCommand : MonitoringViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public M_RefreshCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            Refresh(MonitoringViewer);
            return null;
        }

        //=========================================================================================
        // 機　能：一覧を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public static void Refresh(IMonitoringViewer viewer) {
            // 処理開始条件をチェック
            if (!viewer.Available) {
                return;
            }
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(ShellExecuteBackgroundTask));
            if (!canStart) {
                return;
            }

            // 準備
            CommandRetryInfo retryInfo = viewer.RetryInfo;
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(retryInfo.FileSystemId);
            ShellExecuteRelayMode relayMode = ShellExecuteRelayMode.RelayFileViewer;
            ShellCommandDictionary commandDic = retryInfo.CommandDictionary;
            string command = commandDic.GetCommandGetProcessList();
            List<OSSpecLineExpect> expect = commandDic.ExpectGetProcessList;
            LinuxSpaceSeparateValueStore dataTarget = new LinuxSpaceSeparateValueStore(retryInfo.ParseType, retryInfo.Encoding);
            bool relayStdout = true;
            bool relayStderr = false;

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(null, null, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            IFileProviderSrc srcProvider = new FileProviderSrcDummy(srcFileSystem, retryInfo.FileListContext);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            ShellExecuteBackgroundTask execTask = new ShellExecuteBackgroundTask(srcProvider, destProvider, uiTarget, retryInfo.TargetPathName, command, expect, relayStdout, relayStderr, dataTarget);
            
            // モニタリングビューアを開く
            BackgroundTaskID taskId = execTask.TaskId;
            viewer.Reload(dataTarget);
            dataTarget.LoadingTaskId = taskId;
            dataTarget.Viewer = viewer;

            // タスク開始
            Program.Document.BackgroundTaskManager.StartFileTask(execTask, true);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.M_RefreshCommand;
            }
        }
    }
}
