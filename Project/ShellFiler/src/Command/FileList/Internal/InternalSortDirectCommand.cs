using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.UI.Dialog;
using ShellFiler.FileTask;

namespace ShellFiler.Command.FileList.Internal {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ソート方法を直接指定して、対象パスのファイル一覧のソート方法を指定します。
    //   書式 　 InternalSortDirect()
    //   引数  　なし
    //   戻り値　なし
    //=========================================================================================
    class InternalSortDirectCommand : FileListActionCommand {
        // ソートする項目
        private FileListSortMode.Method m_sortMethod;
        
        // ソートの向き
        private FileListSortMode.Direction m_sortDirection;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public InternalSortDirectCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_sortMethod = (FileListSortMode.Method)param[0];
            m_sortDirection = (FileListSortMode.Direction)param[1];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // ソート方法をダイアログで入力
            FileListSortMode sortMode;
            bool isLeft = Program.Document.CurrentTabPage.IsCursorLeft;
            if (isLeft) {
                sortMode = Program.Document.CurrentTabPage.LeftFileList.SortMode;
            } else {
                sortMode = Program.Document.CurrentTabPage.RightFileList.SortMode;
            }
            sortMode.SortOrder1 = m_sortMethod;
            sortMode.SortOrder2 = FileListSortMode.ModifySort2BySort1(m_sortMethod, sortMode.SortOrder2);
            sortMode.SortDirection1 = m_sortDirection;
            sortMode.SortDirection2 = FileListSortMode.Direction.Normal;
            sortMode.TopDirectory = true;
            sortMode.Capital = false;

            // UIを更新
            SortMenuCommand.RefreshUI(isLeft);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.SortClearCommand;
            }
        }
    }
}
