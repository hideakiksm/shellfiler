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
using ShellFiler.FileSystem.Shell;
using ShellFiler.Properties;
using ShellFiler.MonitoringViewer;
using ShellFiler.MonitoringViewer.ProcessMonitor;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.Provider;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.SSH {

    //=========================================================================================
    // クラス：コマンドを実行する
    // SSHの接続先のホストで、操作対象のユーザーの切り替え（su）を行います。
    //   書式 　 SSHChangeUser()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class SSHChangeUserCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SSHChangeUserCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 処理開始条件をチェック
            bool canUseSSH = BackgroundTaskCommandUtil.CheckSSHSupport(FileListViewTarget.FileList.FileSystem.FileSystemId, true, FileSystemID.SSHShell);
            if (!canUseSSH) {
                return null;
            }
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(ShellExecuteBackgroundTask));
            if (!canStart) {
                return null;
            }

            // 接続中ユーザーのユーザー名@サーバー名を取得
            TerminalShellChannelID channelId = ((ShellFileListContext)(FileListViewTarget.FileList.FileListContext)).TerminalShellChannelId;
            List<TerminalShellChannel> channelList = Program.Document.FileSystemFactory.SSHConnectionManager.GetAuthorizedTerminalChannel(null);
            string userServer = "";
            for (int i = 0; i < channelList.Count; i++) {
                if (channelList[i].ID == channelId) {
                    userServer = channelList[i].ActiveUserServer;
                    break;
                }
            }

            // 入力
            ShellFileListContext context = (ShellFileListContext)(FileListViewTarget.FileList.FileListContext);
            string rootUserName = context.ShellCommandDictionary.ValueRootUserName;
            string directory = FileListViewTarget.FileList.DisplayDirectoryName;
            SSHChangeUserDialog dialog = new SSHChangeUserDialog(directory, userServer, rootUserName, context.ShellCommandDictionary);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return null;
            }

            // ユーザーを切り替え
            return ChdirCommand.ChangeDirectory(FileListViewTarget, new ChangeDirectoryParam.SSHChangeUser(directory, dialog.ChangeUserInfo));
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.SSHChangeUserCommand;
            }
        }
    }
}
