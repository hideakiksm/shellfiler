using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.MonitoringViewer;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.MonitoringViewer;

namespace ShellFiler.Command.FileViewer.View {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ファイル一覧ウィンドウをアクティブにします。
    //   書式 　 V_ReturnFileList()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class V_ReturnFileListCommand : FileViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_ReturnFileListCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // delegateしてキー入力をキャンセル
            Program.MainWindow.BeginInvoke(new ExecutePostDelegate(ExecutePost));
            return null;
        }
        public delegate void ExecutePostDelegate();
        public static void ExecutePost() {
            Program.MainWindow.Activate();
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_ReturnFileListCommand;
            }
        }
    }
}
