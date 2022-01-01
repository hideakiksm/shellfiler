using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileViewer.View {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ビューアのテキスト／ダンプモードを切り替えます。
    //   書式 　 V_ToggleViewMode()
    //   引数  　なし
    //   戻り値　テキストモードに切り替えたときtrue
    //   対応Ver 0.0.1
    //=========================================================================================
    class V_ToggleViewModeCommand : FileViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_ToggleViewModeCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (!Abailable) {
                return true;
            }
            if (TextFileViewer.TextViewerComponent is TextViewerComponent) {
                TextFileViewer.ChangeViewMode(false);
                return false;
            } else if (TextFileViewer.TextViewerComponent is DumpViewerComponent) {
                TextFileViewer.ChangeViewMode(true);
                return true;
            } else {
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_ToggleViewModeCommand;
            }
        }
    }
}
