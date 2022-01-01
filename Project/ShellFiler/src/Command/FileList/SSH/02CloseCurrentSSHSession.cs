using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.SSH;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.FileTask;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.SSH {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 表示中のパスのSSHセッションを閉じます。
    //   書式 　 LogoutCurrentSSHSession()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class LogoutCurrentSSHSessionCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public LogoutCurrentSSHSessionCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            bool canUseSSH = BackgroundTaskCommandUtil.CheckSSHSupport(FileListViewTarget.FileList.FileSystem.FileSystemId, true, null);
            if (!canUseSSH) {
                return null;
            }

            string directory = FileListViewTarget.FileList.DisplayDirectoryName;
            SSHProtocolType protocol;
            string user, server, local;
            int portNo;
            SSHUtils.SeparateUserServer(directory, out protocol, out user, out server, out portNo, out local);
            Program.Document.FileSystemFactory.SSHConnectionManager.GetConnectionInfo(user, server, portNo);
            TaskManagerDialog.DisconnectAllSSHSession(Program.MainWindow);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.LogoutCurrentSSHSessionCommand;
            }
        }
    }
}
