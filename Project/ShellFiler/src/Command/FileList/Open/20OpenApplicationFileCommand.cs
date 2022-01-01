using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Virtual;

namespace ShellFiler.Command.FileList.Open {

    //=========================================================================================
    // クラス：コマンドを実行する
    // カーソル位置のファイルまたはフォルダを関連付けにより実行します。
    //   書式 　 OpenApplicationFile()
    //   引数  　なし
    //   戻り値　bool:実行に成功したときtrue、実行できなかったときfalseを返します。
    //   対応Ver 1.0.0
    //=========================================================================================
    class OpenApplicationFileCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public OpenApplicationFileCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 仮想フォルダの場合は実行準備
            bool success = PrepareVirtualFolderCommand.ReadyForVirtualFolder(FileListViewTarget);
            if (!success) {
                return false;
            }

            // 実行
            int lineNo = FileListComponentTarget.CursorLineNo;
            string path = FileListViewTarget.FileList.DisplayDirectoryName + FileListViewTarget.FileList.Files[lineNo].FileName;
            IFileListContext fileListCtx = FileListViewTarget.FileList.FileListContext;

            FileSystemFactory factory = Program.Document.FileSystemFactory;
            FileOperationStatus status = FileListViewTarget.FileList.FileSystem.OpenShellFile(path, FileListViewTarget.FileList.DisplayDirectoryName, true, fileListCtx);
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
                return UIResource.OpenApplicationFileCommand;
            }
        }
    }
}
