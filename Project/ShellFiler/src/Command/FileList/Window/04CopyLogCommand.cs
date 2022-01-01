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

namespace ShellFiler.Command.FileList.Window {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ログの選択範囲をクリップボードにコピーします。
    //   書式 　 CopyLog()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.4.0
    //=========================================================================================
    class CopyLogCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CopyLogCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 選択されたテキストを取得
            string selectedText;
            bool createAll;
            bool selected = Program.MainWindow.LogWindow.GetSelectedText(out selectedText, out createAll);
            if (!selected) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CopyLogNotSelected);
                return null;
            } else if (!createAll) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CopyLogPart);
            }
            
            // 画面を点滅
            Program.MainWindow.LogWindow.FlashSelection();

            // クリップボードに設定
            Clipboard.SetDataObject(selectedText, true);

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.CopyLogCommand;
            }
        }
    }
}
