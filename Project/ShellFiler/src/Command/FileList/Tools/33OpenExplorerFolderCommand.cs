using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask;
using ShellFiler.Util;
using ShellFiler.UI;
using System.IO;

namespace ShellFiler.Command.FileList.Tools {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 現在のフォルダをエクスプローラで開きます。
    //   書式 　 OpenExplorerFolder()
    //   引数  　なし
    //   戻り値　bool:実行に成功したときtrue、実行できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class OpenExplorerFolderCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public OpenExplorerFolderCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            bool success = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!success) {
                return false;
            }

            string folder = FileListViewTarget.FileList.DisplayDirectoryName;

            if (Configuration.Current.FileListCursorOpenFolder) {
                int lineNo = FileListComponentTarget.CursorLineNo;
                string cursorFolderName = FileListViewTarget.FileList.Files[lineNo].FileName;
                folder = System.IO.Path.Combine(folder, cursorFolderName);
            }

            IFileListContext fileListCtx = FileListViewTarget.FileList.FileListContext;
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            FileOperationStatus status = FileListViewTarget.FileList.FileSystem.OpenShellFile(folder, folder, true, fileListCtx);
            if (status.Failed) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_AssociatedCannotOpen);
                return false;
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.OpenExplorerFolderCommand;
            }
        }
    }
}
