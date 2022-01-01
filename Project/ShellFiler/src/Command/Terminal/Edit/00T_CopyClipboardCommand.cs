using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Log;
using ShellFiler.Terminal.UI;

namespace ShellFiler.Command.Terminal.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 選択範囲をクリップボードにコピーします。
    //   書式 　 T_CopyClipboard()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class T_CopyClipboardCommand : TerminalActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public T_CopyClipboardCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (TerminalPanel.IsMouseDrag || TerminalPanel.SelectionRange == null) {
                return null;
            }

            // 選択されたテキストを取得
            string selectedText;
            bool createAll;
            TerminalPanel.GetSelectedText(false, "\n", out selectedText, out createAll);
            if (!createAll) {
                InfoBox.Information(TerminalPanel.DialogParentForm, Resources.Msg_TerminalCopyPart);
            }

            // 画面を点滅
            TerminalPanel.FlashSelection();

            // クリップボードに設定
            Clipboard.SetDataObject(selectedText, true);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.T_CopyClipboardCommand;
            }
        }
    }
}
