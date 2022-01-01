using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.FileViewer;

namespace ShellFiler.Command.FileViewer.Cursor {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ダイアログで指定された行に移動します。ダイアログに入力された数値の初期値を{0}にします。
    //   書式 　 V_JumpDirect(int initial)
    //   引数  　initial:初期状態の入力（-1のとき、現在行）
    // 　　　　　initial-default:1
    // 　　　　　initial-range:0,9999999
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class V_JumpDirectCommand : FileViewerActionCommand {
        // 初期状態の入力
        private int m_initialInput;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_JumpDirectCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_initialInput = (int)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (!Abailable) {
                return null;
            }
            
            Form form = TextFileViewer.FileViewerForm;
            if (ViewerComponent is TextViewerComponent) {
                TextViewerComponent textViewer = (TextViewerComponent)ViewerComponent;
                int lineNo = ViewerComponent.VisibleTopLine;
                ViewerJumpLineDialog dialog = new ViewerJumpLineDialog(lineNo, m_initialInput);
                Point rightBottom = TextFileViewer.PointToScreen(new Point(TextFileViewer.Right, TextFileViewer.Bottom));
                dialog.Left = rightBottom.X - dialog.Width - ViewerJumpLineDialog.DIALOG_POSITION_MARGIN - SystemInformation.VerticalScrollBarWidth;
                dialog.Top = rightBottom.Y - dialog.Height - ViewerJumpLineDialog.DIALOG_POSITION_MARGIN - SystemInformation.HorizontalScrollBarHeight;
                DialogResult result = dialog.ShowDialog(form);
                if (result == DialogResult.OK) {
                    lineNo = dialog.LineNo;
                    textViewer.MoveSpecifiedPhysicalLineNoOnTop(lineNo);
                }
            } else {
                DumpViewerComponent dumpViewer = (DumpViewerComponent)ViewerComponent;
                int address = dumpViewer.Address;
                ViewerJumpAddressDialog dialog = new ViewerJumpAddressDialog(address, m_initialInput);
                Point rightBottom = TextFileViewer.PointToScreen(new Point(TextFileViewer.Right, TextFileViewer.Bottom));
                dialog.Left = rightBottom.X - dialog.Width - ViewerJumpAddressDialog.DIALOG_POSITION_MARGIN - SystemInformation.VerticalScrollBarWidth;
                dialog.Top = rightBottom.Y - dialog.Height - ViewerJumpAddressDialog.DIALOG_POSITION_MARGIN - SystemInformation.HorizontalScrollBarHeight;
                DialogResult result = dialog.ShowDialog(form);
                if (result == DialogResult.OK) {
                    address = dialog.Address;
                    dumpViewer.MoveSpecifiedAddressOnTop(address);
                }
            }
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_JumpDirectCommand;
            }
        }
    }
}
