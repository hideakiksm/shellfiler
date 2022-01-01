using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.GraphicsViewer.File;
using ShellFiler.Document;
using ShellFiler.GraphicsViewer;
using ShellFiler.Util;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.GraphicsViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 現在表示中のファイルをマークNo.1がセットされた状態にします。
    //   書式 　 G_MarkFile1()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.0.0
    //=========================================================================================
    class G_MarkFile1Command : GraphicsViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_MarkFile1Command() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            G_MarkFile1Command.MarkFile(this, 1);
            return null;
        }

        //=========================================================================================
        // 機　能：ファイルのマークを切り替える
        // 引　数：[in]command  実行中のコマンド
        // 　　　　[in]markNo   セットするマーク番号（0:クリア）
        // 戻り値：なし
        //=========================================================================================
        public static void MarkFile(GraphicsViewerActionCommand command, int markNo) {
            if (command.GraphicsViewerPanel.GraphicsViewerForm.GraphicsViewerParameter.GraphicsViewerMode == GraphicsViewerMode.ClipboardViewer) {
                InfoBox.Information(command.GraphicsViewerPanel.GraphicsViewerForm, Resources.Msg_GraphicsViewerCannotMark);
                return;
            }
            if (!command.Abailable) {
                return;
            }
            command.GraphicsViewerPanel.SetMark(markNo);
            G_SlideShowNextCommand.ExecutePrevNext(command.GraphicsViewerPanel, true, true);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_MarkFile1Command;
            }
        }
    }
}
