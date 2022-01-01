using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask;
using ShellFiler.Command.FileList.Tools;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.FileList;
using ShellFiler.Virtual;

namespace ShellFiler.Command.FileList.Open {

    //=========================================================================================
    // クラス：コマンドを実行する
    // カーソル位置のファイルを実行します。実行ファイル以外はファイルビューアを起動して表示します。
    //   書式 　 ExecuteOrViewer()
    //   引数  　なし
    //   戻り値　bool:実行を開始したときtrue、実行を開始できなかったときfalseを返します。
    //   対応Ver 1.0.0
    //=========================================================================================
    class ExecuteOrViewerCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ExecuteOrViewerCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // ディレクトリの場合は失敗
            int lineNo = FileListComponentTarget.CursorLineNo;
            UIFile file = FileListViewTarget.FileList.Files[lineNo];
            if (file.Attribute.IsDirectory) {
                return false;
            }
            
            // 仮想フォルダの場合は実行準備
            bool success = PrepareVirtualFolderCommand.ReadyForVirtualFolder(FileListViewTarget);
            if (!success) {
                return false;
            }

            // ファイルの場合は試しに開いてみる
            string dir = FileListViewTarget.FileList.DisplayDirectoryName;
            string fullPath = dir + file.FileName;
            IFileListContext fileListCtx = FileListViewTarget.FileList.FileListContext;
            FileOperationStatus status = FileListViewTarget.FileList.FileSystem.OpenShellFile(fullPath, dir, true, fileListCtx);
            if (status == FileOperationStatus.Skip) {
                // 開けない場合はビューアを開く
                FileViewerCommand command = new FileViewerCommand();
                command.InitializeFromParent(this, true, ActionCommandOption.None);
                command.Execute();
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ExecuteOrViewerCommand;
            }
        }
    }
}
