using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.GraphicsViewer;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.GraphicsViewer;

namespace ShellFiler.Command.GraphicsViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // グラフィックビューアでのマーク一覧を表示します。
    //   書式 　 G_MarkFileList()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.0.0
    //=========================================================================================
    class G_MarkFileListCommand : GraphicsViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_MarkFileListCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            SlideShowMarkResult markResult = GraphicsViewerPanel.GetMarkResult();
            if (markResult == null) {
                InfoBox.Information(GraphicsViewerPanel.GraphicsViewerForm, Resources.Msg_GraphicsViewerNoMarkResult);
            } else {
                MarkFileListDialog dialog = new MarkFileListDialog(markResult);
                dialog.ShowDialog(GraphicsViewerPanel.GraphicsViewerForm);
            }
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_MarkFileListCommand;
            }
        }
    }
}
