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
using ShellFiler.UI.FileList;

namespace ShellFiler.Command.FileList.ChangeDir {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 親フォルダに変更します。
    //   書式 　 ChdirParent()
    //   引数  　なし
    //   戻り値　bool:フォルダの変更に成功または変更を開始できたときtrue、変更できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ChdirParentCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ChdirParentCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            return ChangeDirectoryParent(FileListViewTarget);
        }

        //=========================================================================================
        // 機　能：親ディレクトリに変更する
        // 引　数：[in]fileListTarget  対象パスのファイル一覧
        // 戻り値：フォルダが変更できたとき/変更が開始できたときはtrue、変更できないときfalse
        //=========================================================================================
        public static bool ChangeDirectoryParent(FileListView fileListTarget) {
            string current = fileListTarget.FileList.DisplayDirectoryName;
            current = GenericFileStringUtils.TrimLastSeparator(current);
            current = fileListTarget.FileList.FileSystem.GetFileName(current);
            return ChdirCommand.ChangeDirectory(fileListTarget, new ChangeDirectoryParam.ChdirToParent(current));
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ChdirParentCommand;
            }
        }
    }
}
