using System;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.FileOperation {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マークされたファイルをごみ箱を使わずに削除します。
    //   書式 　 HighspeedDelete()
    //   引数  　なし
    //   戻り値　bool:削除をバックグラウンドで開始したときtrue、削除を開始できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class HighspeedDeleteCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public HighspeedDeleteCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            return DeleteCommand.DeleteExecute(FileListViewTarget, FileListViewOpposite, false, true);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.HighspeedDeleteCommand;
            }
        }
    }
}
