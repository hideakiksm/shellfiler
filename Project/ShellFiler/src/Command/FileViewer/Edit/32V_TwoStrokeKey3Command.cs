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
using ShellFiler.UI.Dialog.FileViewer;
using ShellFiler.UI.Dialog.KeyOption;

namespace ShellFiler.Command.FileViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ２ストロークキーの3つ目の入力を開始します。
    //   書式 　 V_TwoStrokeKey3()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class V_TwoStrokeKey3Command : FileViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_TwoStrokeKey3Command() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            Program.Document.CommandFactory.StartTwoStrokeKey(CommandUsingSceneType.FileViewer, TextFileViewer, TwoStrokeType.Key3);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_TwoStrokeKey3Command;
            }
        }
    }
}