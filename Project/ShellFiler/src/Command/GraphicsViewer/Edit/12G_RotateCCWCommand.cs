using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.GraphicsViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.GraphicsViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 表示中の画像を反時計回りに90度に回転した画像にします。
    //   書式 　 G_RotateCCW()
    //   引数  　なし
    //   戻り値　実行できたときtrue
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_RotateCCWCommand : GraphicsViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_RotateCCWCommand() {
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
            ImageInfo image = GraphicsViewerPanel.CurrentImage;
            if (image == null || image.Image == null) {
                return false;
            }
            image.OriginalImage.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
            GraphicsViewerPanel.ResetCurrentImageUI(false);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_RotateCCWCommand;
            }
        }
    }
}
