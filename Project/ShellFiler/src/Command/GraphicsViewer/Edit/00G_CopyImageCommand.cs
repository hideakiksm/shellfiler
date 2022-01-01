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

namespace ShellFiler.Command.GraphicsViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // グラフィックビューアの画像をクリップボードにコピーします
    //   書式 　 G_CopyImage()
    //   引数  　なし
    //   戻り値　コピーできたときtrue
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_CopyImageCommand : GraphicsViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_CopyImageCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (!Abailable) {
                return false;
            }
            return GraphicsViewerPanel.CopyImage();
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_CopyImageCommand;
            }
        }
    }
}
