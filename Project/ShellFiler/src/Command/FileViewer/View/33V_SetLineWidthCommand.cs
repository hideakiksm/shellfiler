using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.FileViewer;

namespace ShellFiler.Command.FileViewer.View {

    //=========================================================================================
    // クラス：コマンドを実行する
    // テキスト表示の画面幅を入力して指定します。
    //   書式 　 V_SetLineWidth()
    //   引数  　なし
    //   戻り値　設定を変更したときtrue
    //   対応Ver 0.0.1
    //=========================================================================================
    class V_SetLineWidthCommand : FileViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_SetLineWidthCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (!Abailable) {
                return false;
            }
            TextViewerLineBreakSetting setting = TextFileViewer.TextBufferLineInfo.LineBreakSetting;
            if (TextFileViewer.TextViewerComponent is TextViewerComponent) {
                // テキストビューアでの折り返し
                ViewerLineWidthDialog dialog = new ViewerLineWidthDialog(setting);
                DialogResult result = dialog.ShowDialog(TextFileViewer.Parent);
                if (result != DialogResult.OK) {
                    return false;
                }
                setting = dialog.Setting;
                Program.Document.UserGeneralSetting.TextViewerLineBreak = (TextViewerLineBreakSetting)(setting.Clone());
            } else if (TextFileViewer.TextViewerComponent is DumpViewerComponent) {
                // ダンプビューアでの折り返し
                ViewerDumpWidthDialog dialog = new ViewerDumpWidthDialog(setting);
                DialogResult result = dialog.ShowDialog(TextFileViewer.Parent);
                if (result != DialogResult.OK) {
                    return false;
                }
                setting = dialog.Setting;
                Program.Document.UserGeneralSetting.TextViewerLineBreak = (TextViewerLineBreakSetting)(setting.Clone());
            }
            bool success = TextFileViewer.TextViewerComponent.SetLineWidth(setting);
            if (!success) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_SetLineWidthCommand;
            }
        }
    }
}
