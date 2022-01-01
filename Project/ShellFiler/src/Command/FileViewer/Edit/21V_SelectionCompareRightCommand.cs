using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime;
using ShellFiler.Api;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileViewer;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 選択範囲を右側の比較対象として登録し、左右がそろった場合は差分表示ツールで比較します。
    //   書式 　 V_SelectionCompareRight()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.0.0
    //=========================================================================================
    class V_SelectionCompareRightCommand : FileViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_SelectionCompareRightCommand() {
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

            // 選択範囲のテキストを比較
            string text = V_SelectionCompareDisplayCommand.GetSelectionText(this.TextFileViewer);
            if (text == null) {
                return null;
            }

            // バッファに登録
            TextViewerComponent textModeViewer = (TextViewerComponent)(this.TextFileViewer.TextViewerComponent);
            FileViewerSelectionCompareBuffer buffer = Program.Document.FileViewerSelectionCompareBuffer;
            buffer.RightString = text;
            buffer.RightStartLineNum = textModeViewer.SelectionRange.StartLine;
            if (buffer.RightString == null) {
                InfoBox.Information(this.TextFileViewer.FileViewerForm, Resources.Msg_ViewerSelectionCompareTextRight);
                return null;
            }

            // 両方そろったら比較
            V_SelectionCompareDisplayCommand.FileCompare fileCompare = new V_SelectionCompareDisplayCommand.FileCompare(this.TextFileViewer);
            fileCompare.Execute();
            fileCompare.Dispose();

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_SelectionCompareRightCommand;
            }
        }
    }
}