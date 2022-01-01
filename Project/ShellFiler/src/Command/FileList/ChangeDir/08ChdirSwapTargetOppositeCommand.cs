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

namespace ShellFiler.Command.FileList.ChangeDir {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 反対パスのフォルダを対象パスと同じにします。
    //   書式 　 ChdirSwapTargetOpposite()
    //   引数  　なし
    //   戻り値　bool:フォルダの変更に成功または変更を開始できたときtrue、変更できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ChdirSwapTargetOppositeCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ChdirSwapTargetOppositeCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            string dirTarget = FileListViewTarget.FileList.DisplayDirectoryName;
            string dirOpposite = FileListViewOpposite.FileList.DisplayDirectoryName;
            bool result1 = ChdirCommand.ChangeDirectory(FileListViewTarget, new ChangeDirectoryParam.Direct(dirOpposite));
            bool result2 = ChdirCommand.ChangeDirectory(FileListViewOpposite, new ChangeDirectoryParam.Direct(dirTarget));
            return result1 && result2;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ChdirSwapTargetOppositeCommand;
            }
        }
    }
}
