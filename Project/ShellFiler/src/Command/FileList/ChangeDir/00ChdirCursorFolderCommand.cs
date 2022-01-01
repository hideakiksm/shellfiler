using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Command.FileList.ChangeDir {

    //=========================================================================================
    // クラス：コマンドを実行する
    // カーソル位置のフォルダに移動します。
    //   書式 　 ChdirCursorFolder()
    //   引数  　なし
    //   戻り値　bool:フォルダの変更に成功または変更を開始できたときtrue、変更できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ChdirCursorFolderCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ChdirCursorFolderCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
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
                return UIResource.ChdirCursorFolderCommand;
            }
        }
    }
}
