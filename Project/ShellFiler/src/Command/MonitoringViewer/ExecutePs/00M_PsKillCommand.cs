using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.OSSpec;
using ShellFiler.Command.MonitoringViewer.Edit;
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

namespace ShellFiler.Command.MonitoringViewer.ExecutePs {

    //=========================================================================================
    // クラス：コマンドを実行する
    // psの結果から、選択中のプロセスを終了します。
    //   書式 　 M_PsKill()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class M_PsKillCommand : MonitoringViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public M_PsKillCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            KillCommand(MonitoringViewer, false);
            return null;
        }

        //=========================================================================================
        // 機　能：プロセスを終了する
        // 引　数：[in]viewer   モニタリングビューア
        // 　　　　[in]force    強制終了するときtrue
        // 戻り値：なし
        //=========================================================================================
        public static void KillCommand(IMonitoringViewer viewer, bool force) {
            // 処理開始条件をチェック
            if (!viewer.Available) {
                return;
            }
            if (viewer.Mode != MonitoringViewerMode.PS) {
                return;
            }
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(ShellExecuteBackgroundTask));
            if (!canStart) {
                return;
            }

            // PIDを取得
            int pid;
            string pidCommand;
            bool success = GetPid(viewer.MatrixData, viewer.MatrixDataView.CursorLine, out pid, out pidCommand);
            if (!success) {
                InfoBox.Warning(viewer.MonitoringViewerForm, Resources.MonitorProcess_KillNotFoundPid);
                return;
            }

            // 確認
            KillConfirmDialog confirmDialog = new KillConfirmDialog(pid, pidCommand, force);
            DialogResult result = confirmDialog.ShowDialog(viewer.MonitoringViewerForm);
            if (result != DialogResult.Yes) {
                return;
            }

            // 準備
            IconImageListID uiIcon;
            string uiDispName;
            if (force) {
                uiIcon = UIResource.M_PsForceTerminateCommand.IconIdLeft;
                uiDispName = UIResource.M_PsForceTerminateCommand.Hint;
            } else {
                uiIcon = UIResource.M_PsKillCommand.IconIdLeft;
                uiDispName = UIResource.M_PsKillCommand.Hint;
            }
            MonitoringCommandWaitDialog waitDialog = new MonitoringCommandWaitDialog(uiIcon, uiDispName);

            CommandRetryInfo retryInfo = viewer.RetryInfo;
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(retryInfo.FileSystemId);
            ShellExecuteRelayMode relayMode = ShellExecuteRelayMode.RelayFileViewer;
            string command = retryInfo.CommandDictionary.GetCommandKill(pid, force);
            List<OSSpecLineExpect> expect = retryInfo.CommandDictionary.ExpectKillProcess;

            LinuxCommandResult dataTarget = new LinuxCommandResult(new LinuxCommandResult.OnCommandCompletedDelegate(waitDialog.OnCommandCompleted));
            bool relayStdout = true;
            bool relayStderr = false;

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(null, null, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            IFileProviderSrc srcProvider = new FileProviderSrcDummy(srcFileSystem, retryInfo.FileListContext);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            ShellExecuteBackgroundTask execTask = new ShellExecuteBackgroundTask(srcProvider, destProvider, uiTarget, retryInfo.TargetPathName, command, expect, relayStdout, relayStderr, dataTarget);
            
            // モニタリングビューアを開く
            BackgroundTaskID taskId = execTask.TaskId;
            waitDialog.LoadingTaskId = taskId;
            dataTarget.LoadingTaskId = taskId;

            // タスク開始
            Program.Document.BackgroundTaskManager.StartFileTask(execTask, true);
            waitDialog.ShowDialog(viewer.MonitoringViewerForm);

            // 実行後に自動更新
            M_RefreshCommand.Refresh(viewer);
        }

        //=========================================================================================
        // 機　能：データ中の選択行からPIDの値を取得する
        // 引　数：[in]data        データ
        // 　　　　[in]cursorLine  選択中の行
        // 　　　　[out]pid        pidを返す変数(取得できなかったとき-1)
        // 　　　　[out]command    コマンドを返す変数(取得できなかったとき"")
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        private static bool GetPid(MatrixData data, int cursorLine, out int pid, out string command) {
            pid = -1;
            command = "";

            // PIDカラムを探す
            List<MatrixData.HeaderKind> headerList = data.Header;
            int indexPid = -1;
            int indexCommand = -1;
            for (int i = 0; i < headerList.Count; i++) {
                if (headerList[i].DisplayName == PsHeaderKind.PID.DisplayName) {
                    indexPid = i;
                }
                if (headerList[i].DisplayName == PsHeaderKind.COMMAND.DisplayName) {
                    indexCommand = i;
                }
            }
            if (indexPid == -1 || indexCommand == -1) {
                return false;
            }

            // PIDのデータを取得
            List<string> line = data.LineListSorted[cursorLine].ValueList;
            if (line.Count <= indexPid) {
                return false;
            }
            if (line.Count <= indexCommand) {
                return false;
            }
            command = line[indexCommand];
            if (int.TryParse(line[indexPid], out pid)) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.M_PsKillCommand;
            }
        }
    }
}
