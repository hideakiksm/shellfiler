using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.FileTask;
using ShellFiler.Properties;

namespace ShellFiler.Command.FileList.Open {

    //=========================================================================================
    // クラス：コマンドを実行する
    // カーソル上のファイルのプロパティを表示します。
    //   書式 　 ShowProperty()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class ShowPropertyCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ShowPropertyCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override object Execute() {
            ShowProperty(FileListViewTarget);
            return null;
        }

        //=========================================================================================
        // 機　能：プロパティを表示する
        // 引　数：[in]fileListView  対象パスのファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public static void ShowProperty(FileListView fileListView) {
            if (fileListView.FileList.Files.Count == 0) {
                return;
            }
            if (FileSystemID.IsWindows(fileListView.FileList.FileSystem.FileSystemId)) {
                UIFile file = fileListView.FileList.Files[fileListView.FileListViewComponent.CursorLineNo];
                string fileName = fileListView.FileList.DisplayDirectoryName + file.FileName;
                Win32API.ShowProperty(fileListView.Handle, fileName);
            } else if (FileSystemID.IsVirtual(fileListView.FileList.FileSystem.FileSystemId)) {
                string tempFile;
                bool success = PrepareVirtualFolderCommand.ReadyForVirtualFolderCursor(fileListView, out tempFile);
                if (!success) {
                    return;
                }
                Win32API.ShowProperty(fileListView.Handle, tempFile);
            }
            return;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ShowPropertyCommand;
            }
        }
    }
}
