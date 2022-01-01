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
    // 左右両方のパスともに対象パスのカーソル位置のフォルダに移動します。反対パスに同名フォルダがない場合は何もしません。
    //   書式 　 BothChdirCursorFolder()
    //   引数  　なし
    //   戻り値　bool:対象パスのフォルダの変更に成功または変更を開始できたときtrue、変更できなかったときfalseを返します。
    //   対応Ver 2.3.0
    //=========================================================================================
    class BothChdirCursorFolderCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public BothChdirCursorFolderCommand() {
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
                ChdirParentCommand.ChangeDirectoryParent(FileListViewOpposite);
                return ChdirParentCommand.ChangeDirectoryParent(FileListViewTarget);
            } else {
                int oppFileIndex = FileListViewOpposite.FileList.GetFileIndex(file.FileName);
                if (oppFileIndex != -1) {
                    UIFile oppFile = FileListViewOpposite.FileList.Files[oppFileIndex];
                    if (oppFile.Attribute.IsDirectory) {
                        string oppBaseDir = FileListViewOpposite.FileList.DisplayDirectoryName;
                        string oppTargetDir = oppBaseDir + file.FileName;
                        ChdirCommand.ChangeDirectory(FileListViewOpposite, new ChangeDirectoryParam.Direct(oppTargetDir));
                    }
                }
                string currBaseDir = FileListViewTarget.FileList.DisplayDirectoryName;
                string currTargetDir = currBaseDir + file.FileName;
                return ChdirCommand.ChangeDirectory(FileListViewTarget, new ChangeDirectoryParam.Direct(currTargetDir));
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.BothChdirCursorFolderCommand;
            }
        }
    }
}
