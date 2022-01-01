using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.MoveCursor {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 対象パスのファイルとフォルダ全体について、指定された条件でマーク状態を操作します。
    //   書式 　 MarkWithConditions()
    //   引数  　なし
    //   戻り値　int:変更した件数を返します。ダイアログをキャンセルしたときは-1を返します。
    //   対応Ver 1.1.0
    //=========================================================================================
    class MarkWithConditionsCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MarkWithConditionsCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // ダイアログで条件を入力
            MarkWithConditionsDialog dialog = new MarkWithConditionsDialog(FileListViewTarget.FileList.FileSystem.FileSystemId);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return -1;
            }

            // マーク処理を実行
            CompareCondition condition = new CompareCondition(dialog.ResultConditionList);
            MarkAllFileMode operation = dialog.MarkMode;
            int count = FileListViewTarget.FileListViewComponent.MarkAllFile(operation, true, condition);

            return count;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.MarkWithConditionsCommand;
            }
        }
    }
}
