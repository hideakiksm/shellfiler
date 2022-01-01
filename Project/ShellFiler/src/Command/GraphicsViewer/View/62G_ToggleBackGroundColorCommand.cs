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
    // 背景を白と黒の間で交互に切り替えます。
    //   書式 　 G_ToggleBackGroundColor()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.0.0
    //=========================================================================================
    class G_ToggleBackGroundColorCommand : GraphicsViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_ToggleBackGroundColorCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (GraphicsViewerPanel.BackgroundColor.R == 255 &&
                GraphicsViewerPanel.BackgroundColor.G == 255 &&
                GraphicsViewerPanel.BackgroundColor.B == 255) {
                SetColor(true);
            } else {
                SetColor(false);
            }
            GraphicsViewerPanel.ResetColor();
            return null;
        }

        //=========================================================================================
        // 機　能：色を設定する
        // 引　数：[in]isBlack  黒背景にするときtrue
        // 戻り値：実行結果
        //=========================================================================================
        public static void SetColor(bool isBlack) {
            if (isBlack) {
                Configuration.Current.GraphicsViewerBackColor = Color.FromArgb(0, 0, 0);
                Configuration.Current.GraphicsViewerTextColor = Color.FromArgb(255, 255, 255);
                Configuration.Current.GraphicsViewerTextShadowColor = Color.FromArgb(64, 64, 64);
                Configuration.Current.GraphicsViewerLoadingTextColor = Color.FromArgb(32, 64, 192);
                Configuration.Current.GraphicsViewerLoadingTextShadowColor = Color.FromArgb(8, 16, 48);
            } else {
                Configuration.Current.GraphicsViewerBackColor = Color.FromArgb(255, 255, 255);
                Configuration.Current.GraphicsViewerTextColor = Color.FromArgb(48, 48, 48);
                Configuration.Current.GraphicsViewerTextShadowColor = Color.FromArgb(200, 200, 200);
                Configuration.Current.GraphicsViewerLoadingTextColor = Color.FromArgb(16, 32, 64);
                Configuration.Current.GraphicsViewerLoadingTextShadowColor = Color.FromArgb(128, 192, 240);
            }
        }


        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_ToggleBackGroundColorCommand;
            }
        }
    }
}
