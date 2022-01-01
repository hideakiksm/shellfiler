using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Command.FileList.SSH;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;

namespace ShellFiler.Command.FileList.Tools {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 対象パスをカレントとしてcmd.exeを開きます。
    //   書式 　 ShellCommand()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class ShellCommandCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ShellCommandCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            OpenShellCommand(FileListViewTarget, false);
            return null;
        }

        //=========================================================================================
        // 機　能：シェルウィンドウを開く
        // 引　数：[in]fileListView   対象パスのファイル一覧
        // 　　　　[in]powerShell     PowerShellのときtrue、cmd.exeのときfalse
        // 戻り値：なし
        //=========================================================================================
        public static void OpenShellCommand(FileListView fileListView, bool powerShell) {
            if (!powerShell && Configuration.Current.TerminalShellCommandSSH && FileSystemID.IsSSH(fileListView.FileList.FileSystem.FileSystemId)) {
                // SSHのターミナルを開く
                CreateSSHTerminalCommand.OpenNewTerminal(fileListView);
                return;
            } else {
                // Windowsのコマンドプロンプトを開く
                string command;
                if (powerShell) {
                    command = GetPowerShellPath();
                    if (command == null) {
                        InfoBox.Warning(Program.MainWindow, Resources.Msg_PowerShellCommandFailed);
                        return;
                    }
                } else {
                    command = "cmd.exe";
                }
                string current;
                if (FileSystemID.IsWindows(fileListView.FileList.FileSystem.FileSystemId)) {
                    current = fileListView.FileList.DisplayDirectoryName;
                } else {
                    current = UIFileList.InitialFolder;
                }
                LocalExecute(command, current);
            }
        }

        //=========================================================================================
        // 機　能：WindowsPowerShellのパス名を返す
        // 引　数：なし
        // 戻り値：実行ファイルのパス名
        //=========================================================================================
        private static string GetPowerShellPath() {
            RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\PowerShell\1\PowerShellEngine", false);
            if (regkey == null) {
                return null;
            }
            string pathValue = (string)regkey.GetValue("ApplicationBase");
            if (pathValue == null) {
                return null;
            }
            string path = Path.Combine(pathValue, "powershell.exe");
            if (!File.Exists(path)) {
                return null;
            }
            return path;
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]program    実行するプログラム
        // 　　　　[in]directory  作業ディレクトリ
        // 戻り値：なし
        //=========================================================================================
        private static void LocalExecute(string program, string directory) {
            // 起動の準備
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = program;
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = false;
            psi.RedirectStandardError = false;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = false;
            psi.Arguments = "";
            psi.WorkingDirectory = directory;

            // プロセスを起動
            try {
                Process process = Process.Start(psi);
                process.Dispose();
            } catch (Exception) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_ShellCommandError, program);
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ShellCommandCommand;
            }
        }
    }
}
