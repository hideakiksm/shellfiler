using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.Virtual;

namespace ShellFiler.Command.FileList.FileOperationEtc {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 差分表示ツールで対象パスと反対パスのファイルすべてを比較します。
    //   書式 　 DiffFolderCompare()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.1.0
    //=========================================================================================
    class DiffFolderCompareCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DiffFolderCompareCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 差分表示ツールのコマンドラインを取得
            string command = DiffMarkCommand.GetDiffToolCommand(true);
            if (command == null) {
                DiffToolDownloadDialog dialog = new DiffToolDownloadDialog();
                dialog.ShowDialog(Program.MainWindow);
                return null;
            }
            if (!FileSystemID.IsWindows(FileListViewTarget.FileList.FileSystem.FileSystemId) || !FileSystemID.IsWindows(FileListViewOpposite.FileList.FileSystem.FileSystemId)) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_DiffWindowsOnly);
                return null;
            }
            if (FileListViewTarget.FileList.DisplayDirectoryName == FileListViewOpposite.FileList.DisplayDirectoryName) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_DiffSameFolder);
                return null;
            }
            string diffCommand = DiffMarkCommand.GetDiffToolCommand(true);
            if (diffCommand == null) {
                DiffToolDownloadDialog dialog = new DiffToolDownloadDialog();
                dialog.ShowDialog(Program.MainWindow);
                return null;
            }
            string program, argBase;
            GenericFileStringUtils.SplitCommandLine(diffCommand, out program, out argBase);
            
            string argument = string.Format(argBase, FileListViewTarget.FileList.DisplayDirectoryName + " " + FileListViewOpposite.FileList.DisplayDirectoryName);
            LocalExecute(diffCommand, argument);
            return null;
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]program    実行するプログラム（ダミーの引数部を含む）
        // 　　　　[in]argument   プログラムへの引数
        // 戻り値：なし
        //=========================================================================================
        private void LocalExecute(string program, string argument) {
            string exeFilePath, dummy;
            GenericFileStringUtils.SplitCommandLine(program, out exeFilePath, out dummy);

            // 起動の準備
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = exeFilePath;
            psi.Arguments = argument;
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = false;
            psi.RedirectStandardError = false;
            psi.UseShellExecute = true;
            psi.CreateNoWindow = false;

            // プロセスを起動
            try {
                Process process = Process.Start(psi);
                process.Dispose();
            } catch (Exception) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_DiffFailed, program);
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.DiffFolderCompareCommand;
            }
        }
    }
}
