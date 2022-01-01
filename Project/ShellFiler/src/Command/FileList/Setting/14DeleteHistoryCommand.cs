using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.Option;

namespace ShellFiler.Command.FileList.Setting {

    //=========================================================================================
    // クラス：コマンドを実行する
    // すべての履歴情報を削除します。
    //   書式 　 DeleteHistory()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class DeleteHistoryCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DeleteHistoryCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            DialogResult result = DeleteHistoryProcedure.ConfirmDeleteHistory(Program.MainWindow, Resources.Option_PrivacyDeleteAll);
            if (result != DialogResult.Yes) {
                return null;
            }
            DeleteHistoryProcedure.DeleteFolderHistory();
            DeleteHistoryProcedure.DeleteCommandHistory();
            DeleteHistoryProcedure.DeleteViewerHistory();
            InfoBox.Information(Program.MainWindow, Resources.Option_PrivacyGeneralDeleteCompleted);

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.DeleteHistoryCommand;
            }
        }
    }
}
