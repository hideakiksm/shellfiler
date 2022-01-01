using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.Window {

    //=========================================================================================
    // クラス：コマンドを実行する
    // バックグラウンドマネージャを開きます。
    //   書式 　 TaskManager()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class TaskManagerCommand : FileListActionCommand {
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TaskManagerCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            TaskManagerDialog dialog = new TaskManagerDialog();
            dialog.ShowDialog(Program.MainWindow);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.TaskManagerCommand;
            }
        }
    }
}
