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
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Command.GraphicsViewer.File {

    //=========================================================================================
    // クラス：コマンドを実行する
    // スライドショーを開始し、次の画像を表示します。最後の画像でグラフィックビューアを閉じるかどうかは{0}とします。
    //   書式 　 G_SlideShowNext(bool closeLast)
    //   引数  　closeLast:最後の画像で終了
    // 　　　　　closeLast-default:true
    // 　　　　　closeLast-range:
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_SlideShowNextCommand : GraphicsViewerActionCommand {
        // 最後の画像で終了するときtrue
        private bool m_closeLast;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_SlideShowNextCommand() {
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
            ExecutePrevNext(GraphicsViewerPanel, true, m_closeLast);
            return null;
        }

        //=========================================================================================
        // 機　能：直前または直後の画像に移動する
        // 引　数：[in]panel     グラフィックビューアのパネル
        // 　　　　[in]forward   前方方向に向かって進むときtrue
        // 　　　　[in]closeLast 最後の画像より進めようとしたときビューアを閉じる場合はtrue
        // 戻り値：実行結果
        //=========================================================================================
        public static void ExecutePrevNext(GraphicsViewerPanel panel, bool forward, bool closeLast) {
            if (panel.GraphicsViewerForm.GraphicsViewerParameter.GraphicsViewerMode == GraphicsViewerMode.ClipboardViewer) {
                InfoBox.Information(panel.GraphicsViewerForm, Resources.Msg_GraphicsViewerCannotStartSlideShow);
                return;
            }

            bool last = panel.RequestSlideShowNext(forward);
            if (closeLast && last) {
                panel.GraphicsViewerForm.Close();
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_SlideShowNextCommand;
            }
        }
    }
}
