using System;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.FileList;

namespace ShellFiler.Command.FileList.FileOperation {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マークされたファイルを、方式を指定して削除します。
    //   書式 　 DeleteEx()
    //   引数  　なし
    //   戻り値　bool:削除をバックグラウンドで開始したときtrue、削除を開始できなかったときfalseを返します。
    //   対応Ver 1.1.0
    //=========================================================================================
    class DeleteExCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DeleteExCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            return DeleteCommand.DeleteExecute(FileListViewTarget, FileListViewOpposite, true, true);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.DeleteExCommand;
            }
        }
    }
}
