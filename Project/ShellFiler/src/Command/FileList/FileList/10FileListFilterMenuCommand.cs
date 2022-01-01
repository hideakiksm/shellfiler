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
    // ファイル一覧フィルターの選択ダイアログを表示して、対象パスのフィルターを入力します。
    //   書式 　 FileListFilterMenu()
    //   引数  　なし
    //   戻り値　bool:ファイル一覧フィルターを変更したときはtrue、ダイアログをキャンセルしたときはfalseを返します。
    //   対応Ver 1.1.0
    //=========================================================================================
    class FileListFilterMenuCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileListFilterMenuCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            return SortMenuCommand.FileListOptionMenu(FileListOptionDialog.OptionPage.Filter, FileListViewTarget);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.FileListFilterMenuCommand;
            }
        }
    }
}
