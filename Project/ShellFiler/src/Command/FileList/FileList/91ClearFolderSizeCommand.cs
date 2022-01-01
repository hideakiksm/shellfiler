using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.FileList {

    //=========================================================================================
    // クラス：コマンドを実行する
    // フォルダ配下のサイズを取得した結果を破棄し、メモリを解放します。
    //   書式 　 ClearFolderSize()
    //   引数  　なし
    //   戻り値　bool:削除したときtrue、削除不要のときfalseを返します。
    //   対応Ver 1.3.0
    //=========================================================================================
    class ClearFolderSizeCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ClearFolderSizeCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            bool deleted = ClearFolderSize();
            if (deleted) {
                InfoBox.Information(Program.MainWindow, Resources.Msg_ClearFolderSize);
            } else {
                InfoBox.Information(Program.MainWindow, Resources.Msg_ClearFolderSizeUnnecessary);
            }
            return deleted;
        }

        //=========================================================================================
        // 機　能：削除を実行する
        // 引　数：なし
        // 戻り値：削除したときtrue、削除不要のときfalse
        //=========================================================================================
        public static bool ClearFolderSize() {
            if (Program.Document.RetrieveFolderSizeResult == null) {
                return false;
            }
            Program.Document.RetrieveFolderSizeResult = null;
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ClearFolderSizeCommand;
            }
        }
    }
}
