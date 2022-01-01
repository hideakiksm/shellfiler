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

namespace ShellFiler.Command.GraphicsViewer.View {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 画面を下に{0}ピクセルだけスクロールします。
    //   書式 　 G_ScrollDown(int pixel)
    //   引数  　pixel:スクロールするピクセル数
    // 　　　　　pixel-default:1
    // 　　　　　pixel-range:1,99999999
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_ScrollDownCommand : GraphicsViewerActionCommand {
        // スクロールするピクセル数
        private int m_pixel;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_ScrollDownCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_pixel = (int)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            GraphicsViewerPanel.ScrollView(0, m_pixel);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_ScrollDownCommand;
            }
        }
    }
}
