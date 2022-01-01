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
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.FileList {

    //=========================================================================================
    // クラス：コマンドを実行する
    // インクリメンタルサーチにより、対象パスから入力されたファイル名を検索します。
    //   書式 　 IncrementalSearch()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class IncrementalSearchCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public IncrementalSearchCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            FileIncrementalSearchDialog dialog = new FileIncrementalSearchDialog();
            FileListViewCommandUtils.MoveDialogOuterArea(FileListViewTarget, dialog);
            dialog.FileListView = FileListViewTarget;
            dialog.ShowDialog(Program.MainWindow);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.IncrementalSearchCommand;
            }
        }
    }
}
