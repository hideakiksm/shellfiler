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
using ShellFiler.UI.FileList;
using ShellFiler.UI.Dialog;
using ShellFiler.FileTask;

namespace ShellFiler.Command.FileList.FileList {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ソートをクリアします。
    //   書式 　 SortClear()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class SortClearCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SortClearCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // ソート方法をダイアログで入力
            FileListSortMode sortMode;
            bool isLeft = Program.Document.CurrentTabPage.IsCursorLeft;
            if (isLeft) {
                sortMode = Program.Document.CurrentTabPage.LeftFileList.SortMode;
            } else {
                sortMode = Program.Document.CurrentTabPage.RightFileList.SortMode;
            }
            sortMode.SortOrder1 = FileListSortMode.Method.NoSort;
            sortMode.SortOrder2 = FileListSortMode.Method.NoSort;
            sortMode.SortDirection1 = FileListSortMode.Direction.Normal;
            sortMode.SortDirection2 = FileListSortMode.Direction.Normal;
            sortMode.TopDirectory = true;
            sortMode.Capital = false;

            // UIを更新
            SortMenuCommand.RefreshUI(isLeft);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.SortClearCommand;
            }
        }
    }
}
