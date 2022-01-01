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
    // マウスでファイルを選択します。エクスプローラ風に前回クリック位置との間を連続してマークします。
    //   書式 　 MouseExplorerSelect()
    //   引数  　なし
    //   戻り値　なし
    //=========================================================================================
    class MouseExplorerSelectCommand : AbstractMouseMarkCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MouseExplorerSelectCommand() {
            MarkAction = MouseMarkAction.ExplorerMark;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.MouseExplorerSelectCommand;
            }
        }
    }
}
