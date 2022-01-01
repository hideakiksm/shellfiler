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
    // カーソル上のファイルのコンテキストメニューを表示します。
    //   書式 　 ContextMenu()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class ContextMenuCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ContextMenuCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override object Execute() {
            ShowContextMenu(FileListViewTarget, ContextMenuPosition.OnFile);
            return null;
        }

        //=========================================================================================
        // 機　能：コンテキストメニューを表示する
        // 引　数：[in]fileListView  対象パスのファイル一覧
        // 　　　　[in]menuPos       コンテキストメニューを表示する位置
        // 戻り値：なし
        //=========================================================================================
        public static void ShowContextMenu(FileListView fileListView, ContextMenuPosition menuPos) {
            if (fileListView.FileList.Files.Count == 0) {
                return;
            }
            if (FileSystemID.IsWindows(fileListView.FileList.FileSystem.FileSystemId)) {
                UIFile file = fileListView.FileList.Files[fileListView.FileListViewComponent.CursorLineNo];
                string fileName = fileListView.FileList.DisplayDirectoryName + file.FileName;
                fileListView.ExplorerMenu(fileName, menuPos);
            } else if (FileSystemID.IsVirtual(fileListView.FileList.FileSystem.FileSystemId)) {
                string tempFile;
                bool success = PrepareVirtualFolderCommand.ReadyForVirtualFolderCursor(fileListView, out tempFile);
                if (!success) {
                    return;
                }
                fileListView.ExplorerMenu(tempFile, menuPos);
            }
            return;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ContextMenuCommand;
            }
        }
    }
}
