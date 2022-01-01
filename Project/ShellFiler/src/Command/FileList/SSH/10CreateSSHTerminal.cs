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
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.UI;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.UI.Dialog.Terminal;

namespace ShellFiler.Command.FileList.SSH {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 表示中のパスのSSHターミナル（コンソール）を新しく開きます。
    //   書式 　 CreateSSHTerminal()
    //   引数  　なし
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class CreateSSHTerminalCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CreateSSHTerminalCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            OpenNewTerminal(FileListViewTarget);
            return null;
        }

        //=========================================================================================
        // 機　能：ターミナルを開く
        // 引　数：[in]fileListView   対象パスのファイル一覧
        // 戻り値：ターミナルを開いたときtrue、キャンセルしたときfalse
        //=========================================================================================
        public static bool OpenNewTerminal(FileListView fileListView) {
            bool canUseSSH = BackgroundTaskCommandUtil.CheckSSHSupport(fileListView.FileList.FileSystem.FileSystemId, true, null);
            if (!canUseSSH) {
                return false;
            }

            string directory = fileListView.FileList.DisplayDirectoryName;
            bool success = OpenTerminal(CreateSSHTerminalCommand.OpenMode.NewCheckExisting, directory);

            return success;
        }

        //=========================================================================================
        // 機　能：ターミナルを開く
        // 引　数：[in]openMode   開くターミナルを選択する方法
        // 　　　　[in]directory  開く対象のSSHディレクトリ（openModeのみnull可ですべての接続）
        // 戻り値：ターミナルを開いたときtrue、キャンセルしたときfalse
        //=========================================================================================
        public static bool OpenTerminal(OpenMode openMode, string directory) {
            // コンソールを選択
            string userServer;
            if (directory == null) {
                userServer = null;
            } else {
                SSHProtocolType protocol;
                SSHUtils.GetUserServerPart(directory, out protocol, out userServer);
            }
            List<TerminalShellChannel> channelList = Program.Document.FileSystemFactory.SSHConnectionManager.GetAuthorizedTerminalChannel(directory);
            TerminalShellChannel channel;
            ConsoleScreen console;
            switch (openMode) {
                case OpenMode.IntegrateFirst:
                    if (channelList.Count == 0) {
                        channel = null;
                        console = null;
                    } else {
                        channel = channelList[0];
                        console = channel.ConsoleScreen;
                    }
                    break;
                case OpenMode.AlwaysNew:
                    channel = null;
                    console = null;
                    break;
                case OpenMode.SelectExisting: {
                    if (channelList.Count == 0) {
                        InfoBox.Warning(Program.MainWindow, Resources.Msg_NotFoundSSHTerminal);
                        return false;
                    } else {
                        TerminalSelectConsoleDialog dialog = new TerminalSelectConsoleDialog(null, false, channelList);
                        DialogResult result = dialog.ShowDialog(Program.MainWindow);
                        if (result != DialogResult.OK) {
                            return false;
                        }
                        dialog.GetResult(out userServer, out channel, out console);
                    }
                    break;
                }
                case OpenMode.NewCheckExisting: {
                    bool isAllAssigned = CheckAllAssignedWindow(channelList);
                    if (channelList.Count == 0 || isAllAssigned) {
                        channel = null;
                        console = null;
                    } else {
                        TerminalSelectConsoleDialog dialog = new TerminalSelectConsoleDialog(userServer, true, channelList);
                        DialogResult result = dialog.ShowDialog(Program.MainWindow);
                        if (result != DialogResult.OK) {
                            return false;
                        }
                        string userServerBackup = userServer;
                        dialog.GetResult(out userServer, out channel, out console);
                        if (console == null) {
                            userServer = userServerBackup;
                        }
                    }
                    break;
                }
                default:
                    return false;
            }

            // 新規コンソール
            if (console == null) {
                if (channelList.Count > TerminalShellChannel.MAX_CONSOLE_COUNT) {
                    InfoBox.Warning(Program.MainWindow, Resources.Msg_TooMaySSHTerminal);
                    return false;
                }
                string uniqueName = Program.Document.FileSystemFactory.SSHConnectionManager.CreateUniqueConsoleName(userServer, false);
                console = new ConsoleScreen(userServer, uniqueName, DateTime.Now);
            }

            // フォームを開く
            TerminalForm form = Program.WindowManager.GetTerminalForm(console);
            if (form == null) {
                form = new TerminalForm(userServer, console, channel);
                form.Show();
            } else {
                form.BringToFront();
            }
            return true;
        }

        //=========================================================================================
        // 機　能：シェルにターミナルが割り当てられているかどうかを確認する
        // 引　数：[in]channelList  シェルチャネルの一覧
        // 戻り値：すべてのシェルチャネルが割り当てられているときtrue、未割り当てがあるときfalse
        //=========================================================================================
        private static bool CheckAllAssignedWindow(List<TerminalShellChannel> channelList) {
            for (int i = 0; i < channelList.Count; i++) {
                if (channelList[i].ForFileSystem) {
                    continue;
                }
                ConsoleScreen console = channelList[i].ConsoleScreen;
                TerminalForm form = Program.WindowManager.GetTerminalForm(console);
                if (form == null) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.CreateSSHTerminalCommand;
            }
        }

        //=========================================================================================
        // プロパティ：コンソールを開くモード
        //=========================================================================================
        public enum OpenMode {
            IntegrateFirst,             // 既存のコンソールがあれば初めに開かれたものを開く。初回の場合は新規
            AlwaysNew,                  // 常に新しいシェルを開く
            SelectExisting,             // 開かれているシェルをダイアログで選択して開く。初回の場合はエラー
            NewCheckExisting,           // 新規シェルを開くが、ウィンドウがない接続済みのシェルがある場合は選択ダイアログを開く
        }
    }
}
