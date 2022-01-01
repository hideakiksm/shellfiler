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
using ShellFiler.FileTask;
using ShellFiler.Properties;

namespace ShellFiler.Command.FileList.ChangeDir {

    //=========================================================================================
    // クラス：コマンドを実行する
    // カーソル位置のファイルに対して強制的に仮想フォルダへの切り替えを試みます。
    //   書式 　 ChdirVirtualFolder()
    //   引数  　なし
    //   戻り値　bool:フォルダの変更に成功または変更を開始できたときtrue、変更できなかったときfalseを返します。
    //   対応Ver 1.2.0
    //=========================================================================================
    class ChdirVirtualFolderCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ChdirVirtualFolderCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            int lineNo = FileListComponentTarget.CursorLineNo;
            UIFile file = FileListViewTarget.FileList.Files[lineNo];

            if (file.FileName == "..") {
                return ChdirParentCommand.ChangeDirectoryParent(FileListViewTarget);
            } else {
                string baseDir = FileListViewTarget.FileList.DisplayDirectoryName;
                string targetDir = baseDir + file.FileName;
                return ChdirCommand.ChangeDirectory(FileListViewTarget, new ChangeDirectoryParam.Direct(targetDir));
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ChdirVirtualFolderCommand;
            }
        }
    }
}
