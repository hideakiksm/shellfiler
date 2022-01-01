using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.Command;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.Properties;
using ShellFiler.MonitoringViewer;
using ShellFiler.MonitoringViewer.ProcessMonitor;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.SSH {

    //=========================================================================================
    // クラス：コマンドを実行する
    // SSHの接続先のホストで、実行中のプロセスの一覧を表示します。
    //   書式 　 SSHProcessMonitor()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class SSHProcessMonitorCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SSHProcessMonitorCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 処理開始条件をチェック
            bool canUseSSH = BackgroundTaskCommandUtil.CheckSSHSupport(FileListViewTarget.FileList.FileSystem.FileSystemId, true, null);
            if (!canUseSSH) {
                return null;
            }
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(ShellExecuteBackgroundTask));
            if (!canStart) {
                return false;
            }

            // 準備
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            ShellExecuteRelayMode relayMode = ShellExecuteRelayMode.RelayFileViewer;
            ShellCommandDictionary commandDic = SSHUtils.GetShellCommandDictionary(FileListViewTarget.FileList.FileListContext);
            Encoding encoding = SSHUtils.GetEncoding(FileListViewTarget.FileList.FileListContext);
            string command = commandDic.GetCommandGetProcessList();
            List<OSSpecLineExpect> expect = commandDic.ExpectGetProcessList;
            LinuxSpaceSeparateValueStore dataTarget = new LinuxSpaceSeparateValueStore(MonitoringViewerMode.PS, encoding);
            bool relayStdout = true;

            // タスクを登録
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            IFileProviderSrc srcProvider = new FileProviderSrcDummy(srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            ShellExecuteBackgroundTask execTask = new ShellExecuteBackgroundTask(srcProvider, destProvider, uiTarget, FileListViewTarget.FileList.DisplayDirectoryName, command, expect, relayStdout, false, dataTarget);
            
            // モニタリングビューアを開く
            // retryInfoへのsrcFileListContextのセットはVirtualのライフサイクル管理が正常に動作しない。
            // 実際にはCloneによるコピーが必要だが、Shellファイルシステムではコピーで十分
            CommandRetryInfo retryInfo = new CommandRetryInfo(commandDic, FileListViewTarget.FileList.FileSystem.FileSystemId, srcFileListContext, FileListViewTarget.FileList.DisplayDirectoryName, MonitoringViewerMode.PS, encoding);
            BackgroundTaskID taskId = execTask.TaskId;
            ProcessMonitoringViewer viewer = new ProcessMonitoringViewer(dataTarget, retryInfo);
            MonitoringViewerForm monitoringViewer = new MonitoringViewerForm(viewer, taskId);
            dataTarget.LoadingTaskId = taskId;
            dataTarget.Viewer = viewer;
            monitoringViewer.Show();

            // タスク開始
            Program.Document.BackgroundTaskManager.StartFileTask(execTask, true);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.SSHProcessMonitorCommand;
            }
        }
    }
}
