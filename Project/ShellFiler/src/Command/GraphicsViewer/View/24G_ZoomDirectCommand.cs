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
    // 拡大率を指定された値{0}に設定します。
    //   書式 　 G_ZoomDirect(int zoom)
    //   引数  　zoom:%単位での拡大率
    // 　　　　　zoom-default:100
    // 　　　　　zoom-range:6,1600
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_ZoomDirectCommand : GraphicsViewerActionCommand {
        // ズームする割合
        private int m_zoom;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_ZoomDirectCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_zoom = (int)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            GraphicsViewerPanel.ChangeZoomRatioDirect((float)m_zoom / 100.0f, null);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_ZoomDirectCommand;
            }
        }
    }
}
