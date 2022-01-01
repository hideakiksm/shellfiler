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
    // スライドショーを開始し、前の画像を表示します。最後の画像でグラフィックビューアを閉じるかどうかは{0}とします。
    //   書式 　 G_SlideShowPrev(bool closeLast)
    //   引数  　closeLast:最後の画像で終了
    // 　　　　　closeLast-default:true
    // 　　　　　closeLast-range:
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_SlideShowPrevCommand : GraphicsViewerActionCommand {
        // 最後の画像で終了するときtrue
        private bool m_closeLast;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_SlideShowPrevCommand() {
        }
        
        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_closeLast = (bool)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            G_SlideShowNextCommand.ExecutePrevNext(GraphicsViewerPanel, false, m_closeLast);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_SlideShowPrevCommand;
            }
        }
    }
}
