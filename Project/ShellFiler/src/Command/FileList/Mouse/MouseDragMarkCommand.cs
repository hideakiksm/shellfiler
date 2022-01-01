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

namespace ShellFiler.Command.FileList.Mouse {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マウスでファイルのマーク状態を反転します。
    // クリック対象のマーク状態を反転し、その後のドラッグした範囲のファイルをはじめのファイルと
    // 同じマーク状態にします。
    //   書式 　 MouseDragMark()
    //   引数  　なし
    //   戻り値　なし
    //=========================================================================================
    class MouseDragMarkCommand : AbstractMouseMarkCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MouseDragMarkCommand() {
            MarkAction = MouseMarkAction.RevertSelect;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.MouseDragMarkCommand;
            }
        }
    }
}
