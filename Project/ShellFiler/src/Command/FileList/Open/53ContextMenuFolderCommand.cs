using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.FileTask;
using ShellFiler.Properties;

namespace ShellFiler.Command.FileList.Open {

    //=========================================================================================
    // クラス：コマンドを実行する
    // カレントフォルダのコンテキストメニューを表示します。
    //   書式 　 ContextMenuFolder()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class ContextMenuFolderCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ContextMenuFolderCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override object Execute() {
            if (!FileSystemID.IsWindows(FileListViewTarget.FileList.FileSystem.FileSystemId)) {
                return null;
            }
            string folder = FileListViewTarget.FileList.DisplayDirectoryName;
            FileListViewTarget.ExplorerMenu(folder, ContextMenuPosition.FileListTop);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ContextMenuFolderCommand;
            }
        }
    }
}
