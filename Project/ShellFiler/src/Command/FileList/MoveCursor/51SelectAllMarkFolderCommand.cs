using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Command.FileList.MoveCursor {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 対象パスのフォルダ全体について、マーク状態を反転します。ファイルはそのままです。
    //   書式 　 SelectAllMarkFolder()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class SelectAllMarkFolderCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SelectAllMarkFolderCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.SelectAllFolder, true, null);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.SelectAllMarkFolderCommand;
            }
        }
    }
}
