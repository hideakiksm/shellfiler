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
    // 拡大率を最大の{0}/1000だけ増加するようにズームアウトします。
    //   書式 　 G_ZoomOutEx(int zoom)
    //   引数  　zoom:ズームする割合
    // 　　　　　zoom-default:100
    // 　　　　　zoom-range:1,1000
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_ZoomOutExCommand : GraphicsViewerActionCommand {
        // ズームする割合
        private int m_zoom;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_ZoomOutExCommand() {
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
            GraphicsViewerPanel.ChangeZoomRatioByZoomKeyDelta(-m_zoom, null);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_ZoomOutExCommand;
            }
        }
    }
}
