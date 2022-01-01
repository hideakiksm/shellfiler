using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Command.FileList.ChangeDir {

    //=========================================================================================
    // クラス：コマンドを実行する
    // パスヒストリを利用して直前のディレクトリに変更します。
    //   書式 　 PathHistoryNext()
    //   引数  　なし
    //   戻り値　bool:フォルダの変更に成功または変更を開始できたときtrue、変更できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class PathHistoryNextCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public PathHistoryNextCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            PathHistory pathHistory = FileListViewTarget.FileList.PathHistory;
            if (pathHistory.CurrentIndex >= pathHistory.HistoryList.Count - 1) {
                return false;
            }
            PathHistoryItem item = pathHistory.HistoryList[pathHistory.CurrentIndex + 1];
            return ChdirCommand.ChangeDirectory(FileListViewTarget, new ChangeDirectoryParam.PathHistoryNext(item));
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.PathHistoryNextCommand;
            }
        }
    }
}
