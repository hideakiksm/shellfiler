using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.FileList;
using ShellFiler.FileTask;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Virtual;
using ShellFiler.Properties;

namespace ShellFiler.Command.FileList.ChangeDir {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 仮想フォルダのとき、マークファイルをすべて展開して実行の準備を行います。
    //   書式 　 PrepareVirtualFolder()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.2.0
    //=========================================================================================
    class PrepareVirtualFolderCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public PrepareVirtualFolderCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override object Execute() {
            bool error = false;
            if (!FileSystemID.IsVirtual(FileListViewTarget.FileList.FileSystem.FileSystemId)) {
                error = true;
            }
            if (FileListViewTarget.FileList.Files.Count == 0) {
                error = true;
            }

            if (error) {
                VirtualPrepareErrorDialog dialog = new VirtualPrepareErrorDialog();
                dialog.ShowDialog(Program.MainWindow);
            } else {
                ReadyForVirtualFolder(FileListViewTarget);
            }

            return null;
        }

        //=========================================================================================
        // 機　能：仮想フォルダの実行準備を行う
        // 引　数：[in]fileListView   対象パスのファイル一覧
        // 戻り値：実行を継続してよいときtrue
        //=========================================================================================
        public static bool ReadyForVirtualFolder(FileListView fileListView) {
            if (!FileSystemID.IsVirtual(fileListView.FileList.FileSystem.FileSystemId)) {
                return true;
            }
            if (fileListView.FileList.Files.Count == 0) {
                return false;
            }

            // ファイルを展開
            List<string> fileList = BackgroundTaskCommandUtil.GetMarkAndCurrentFileList(fileListView);
            if (fileList.Count == 0) {
                return false;
            }
            VirtualExtractMarkedArg arg = new VirtualExtractMarkedArg(fileListView.FileList.FileListContext, fileList);
            FileOperationStatus status = VirtualExtractWaitDialog.Extract(arg);
            if (!status.Succeeded) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：カーソル上のファイルのみについて仮想フォルダの実行準備を行う
        // 引　数：[in]fileListView   対象パスのファイル一覧
        // 　　　　[out]tempFile      テンポラリに展開したファイルのパス名
        // 戻り値：実行を継続してよいときtrue
        //=========================================================================================
        public static bool ReadyForVirtualFolderCursor(FileListView fileListView, out string tempFile) {
            tempFile = null;
            if (!FileSystemID.IsVirtual(fileListView.FileList.FileSystem.FileSystemId)) {
                return true;
            }
            if (fileListView.FileList.Files.Count == 0) {
                return false;
            }

            // ファイルを展開
            string fileName = fileListView.FileList.DisplayDirectoryName + fileListView.FileList.Files[fileListView.FileListViewComponent.CursorLineNo].FileName;
            List<string> fileList = new List<string>();
            fileList.Add(fileName);
            VirtualExtractMarkedArg arg = new VirtualExtractMarkedArg(fileListView.FileList.FileListContext, fileList);
            FileOperationStatus status = VirtualExtractWaitDialog.Extract(arg);
            if (!status.Succeeded) {
                return false;
            }
            tempFile = arg.TempFileList[0];
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.PrepareVirtualFolderCommand;
            }
        }
    }
}
