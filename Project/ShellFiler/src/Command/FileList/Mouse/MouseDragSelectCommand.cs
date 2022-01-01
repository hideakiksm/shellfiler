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
    // マウスでファイルを選択します。クリックしたファイルやディレクトリ以外はマークが解除されます。
    //   書式 　 MouseDragSelect()
    //   引数  　なし
    //   戻り値　なし
    //=========================================================================================
    class MouseDragSelectCommand : AbstractMouseMarkCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MouseDragSelectCommand() {
            MarkAction = MouseMarkAction.MarkFirstOnly;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.MouseDragSelectCommand;
            }
        }
    }
}
