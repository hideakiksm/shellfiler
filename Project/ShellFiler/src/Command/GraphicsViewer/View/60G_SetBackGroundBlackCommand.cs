using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.GraphicsViewer.View {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 背景を黒にします。
    //   書式 　 G_SetBackGroundBlack()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.0.0
    //=========================================================================================
    class G_SetBackGroundBlackCommand : GraphicsViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_SetBackGroundBlackCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            G_ToggleBackGroundColorCommand.SetColor(true);
            GraphicsViewerPanel.ResetColor();
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_SetBackGroundBlackCommand;
            }
        }
    }
}
