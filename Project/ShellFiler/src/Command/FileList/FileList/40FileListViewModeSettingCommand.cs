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
    // ファイル一覧での詳細/サムネイル表示モードを選択する
    //   書式 　 FileListViewModeSetting()
    //   引数  　なし
    //   戻り値　bool:ファイル一覧フィルターを変更したときはtrue、ダイアログをキャンセルしたときはfalseを返します。
    //   対応Ver 2.2.0
    //=========================================================================================
    class FileListViewModeSettingCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileListViewModeSettingCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            return SortMenuCommand.FileListOptionMenu(FileListOptionDialog.OptionPage.ViewMode, FileListViewTarget);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.FileListViewModeSettingCommand;
            }
        }
    }
}
