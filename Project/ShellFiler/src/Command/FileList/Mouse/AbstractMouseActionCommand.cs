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
    // クラス：マウスコマンドの基底クラス
    //=========================================================================================
    public abstract class AbstractMouseActionCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public AbstractMouseActionCommand() {
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        abstract public void OnMouseMove();

        //=========================================================================================
        // 機　能：マウスのボタンが離されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        abstract public void OnMouseUp();
    }
}
