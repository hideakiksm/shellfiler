using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.ChangeDir {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ダイアログでドライブを選択してカレントを変更します。
    //   書式 　 ChdirDrive()
    //   引数  　なし
    //   戻り値　bool:フォルダの変更に成功または変更を開始できたときtrue、変更できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ChdirDriveCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ChdirDriveCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            return ChdirDriveCommand.ChangeDirectory(FileListViewTarget, LogInDirectoryDialog.LogInDirectoryPage.LogInDrive);
        }

        //=========================================================================================
        // 機　能：フォルダ変更ダイアログを表示する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public static object ChangeDirectory(FileListView fileListView, LogInDirectoryDialog.LogInDirectoryPage dialogMode) {
            // フォルダ変更ダイアログを開く
            string directory = fileListView.FileList.DisplayDirectoryName;
            LogInDirectoryDialog dialog = new LogInDirectoryDialog(dialogMode, directory);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return false;
            }

            // SSH一時ログインの場合はダイアログを開く
            if (dialog.SSHTemporaryLogin) {
                SSHConnectionDialog dialogSsh = new SSHConnectionDialog(null, SSHConnectionDialog.ConnectMode.TempNewConnection);
                result = dialogSsh.ShowDialog(Program.MainWindow);
                if (result != DialogResult.OK) {
                    return false;
                }
                SSHUserAuthenticateSettingItem authSetting = dialogSsh.AuthenticateSetting;
                SSHUserAuthenticateSetting authDatabase = Program.Document.SSHUserAuthenticateSetting;
                authDatabase.AddTemporarySetting(authSetting);

                SSHProtocolType protocol;
                string dummyUser, dummyServer, dummyPath;
                int dummyPort;
                SSHUtils.SeparateUserServer(directory, out protocol, out dummyUser, out dummyServer, out dummyPort, out dummyPath);
                
                directory = authSetting.GetDirectoryName(protocol) + ":~/";
            } else {
                directory = dialog.TargetDirectory;
            }

            // 指定フォルダに変更
            if (directory.StartsWith("ssh:")) {
                // SSHシェルファイルシステム、かつ、新規チャネルのとき
                ChdirCommand.ChangeDirectory(fileListView, new ChangeDirectoryParam.DirectSshShell(directory, dialog.ShellNewChannel));
            } else {
                ChdirCommand.ChangeDirectory(fileListView, new ChangeDirectoryParam.Direct(directory));
            }

            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ChdirDriveCommand;
            }
        }
    }
}
