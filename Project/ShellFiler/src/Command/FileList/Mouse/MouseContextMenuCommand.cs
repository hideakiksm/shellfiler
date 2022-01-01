using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList.Open;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;

namespace ShellFiler.Command.FileList.Mouse {

    //=========================================================================================
    // クラス：コマンドを実行する
    // クリック対象にマウスカーソルを移動し、そのファイルのコンテキストメニューを表示します。
    //   書式 　 MouseContextMenu()
    //   引数  　なし
    //   戻り値　なし
    //=========================================================================================
    class MouseContextMenuCommand : AbstractMouseActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MouseContextMenuCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            FileListComponentTarget.OnMouseDown(this);
            return null;
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override void OnMouseMove() {
            FileListComponentTarget.OnMouseMove(this);
        }

        //=========================================================================================
        // 機　能：マウスのボタンが離されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override void OnMouseUp() {
            FileListComponentTarget.OnMouseUp(this);
            ContextMenuCommand.ShowContextMenu(FileListViewTarget, ContextMenuPosition.OnMouse);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.MouseContextMenuCommand;
            }
        }
    }
}
