using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.FileTask;
using ShellFiler.GraphicsViewer;

namespace ShellFiler.Command.FileList.Tools {

    //=========================================================================================
    // クラス：コマンドを実行する
    // スライドショーのマーク一覧の選択ダイアログを開きます。
    //   書式 　 SlideShowMarkResult()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.0.0
    //=========================================================================================
    class SlideShowMarkResultCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SlideShowMarkResultCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (Program.Document.SlideShowMarkResult == null) {
                InfoBox.Information(Program.MainWindow, Resources.Msg_GraphicsViewerNoMarkResult);
                return null;
            }

            Program.MainWindow.ShowSlideShowResult(false, Program.Document.SlideShowMarkResult);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.SlideShowMarkResultCommand;
            }
        }
    }
}
