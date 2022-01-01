using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.MonitoringViewer;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.MonitoringViewer;

namespace ShellFiler.Command.Terminal.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ターミナルの表示内容をファイルに保存します。
    //   書式 　 T_PasteClipboard()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class T_PasteClipboardCommand : TerminalActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public T_PasteClipboardCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (TerminalPanel.IsMouseDrag) {
                return null;
            }

            // クリップボードの内容を確認
            IDataObject data = Clipboard.GetDataObject();
            string text = null;
            if (data.GetDataPresent(DataFormats.OemText)) {
                text = (string)data.GetData(DataFormats.OemText, false);
            }
            if (text == null) {
                return null;
            }

            // 送信
            text = StringUtils.ConvertCrLfToLf(text);
            TerminalPanel.SendCommand(text);

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.T_PasteClipboardCommand;
            }
        }
    }
}
