using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Command.FileList;
using ShellFiler.GraphicsViewer;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Command.GraphicsViewer.File {

    //=========================================================================================
    // クラス：コマンドを実行する
    // グラフィックビューアを終了します。スライドショーの場合は前へ進めます。
    //   書式 　 G_ExitViewOrPrevSlide()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_ExitViewOrPrevSlideCommand : GraphicsViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_ExitViewOrPrevSlideCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (GraphicsViewerPanel.GraphicsViewerForm.SlideShowMode) {
                G_SlideShowNextCommand.ExecutePrevNext(GraphicsViewerPanel, false, true);
            } else {
                if (GraphicsViewerPanel.GraphicsViewerForm.TopMost) {
                    GraphicsViewerPanel.GraphicsViewerForm.FullScreen(false, false);
                    return false;
                } else {
                    GraphicsViewerPanel.GraphicsViewerForm.Close();
                    return true;
                }
            }
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_ExitViewOrPrevSlideCommand;
            }
        }
    }
}
