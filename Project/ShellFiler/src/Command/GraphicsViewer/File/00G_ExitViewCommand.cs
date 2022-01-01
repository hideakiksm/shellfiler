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

namespace ShellFiler.Command.GraphicsViewer.File {

    //=========================================================================================
    // クラス：コマンドを実行する
    // グラフィックビューアを終了します。
    //   書式 　 G_ExitView()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_ExitViewCommand : GraphicsViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_ExitViewCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (GraphicsViewerPanel.GraphicsViewerForm.TopMost) {
                GraphicsViewerPanel.GraphicsViewerForm.FullScreen(false, false);
                return null;
            } else {
                GraphicsViewerPanel.GraphicsViewerForm.Close();
                return null;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_ExitViewCommand;
            }
        }
    }
}
