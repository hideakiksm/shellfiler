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

namespace ShellFiler.Command.FileViewer.View {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ファイルビューアを終了します。
    //   書式 　 V_ExitView()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class V_ExitViewCommand : FileViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_ExitViewCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (TextFileViewer.FileViewerForm.TopMost) {
                TextFileViewer.FileViewerForm.FullScreen(false, false);
                return null;
            } else {
                try {
                    TextFileViewer.FileViewerForm.Close();
                } catch (InvalidOperationException) {
                    // 例外「CreateHandle() の実行中は値 Dispose() を呼び出せません。」を回避
                }
                return null;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_ExitViewCommand;
            }
        }
    }
}
