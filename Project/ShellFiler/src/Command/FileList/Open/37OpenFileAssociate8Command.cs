using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;

namespace ShellFiler.Command.FileList.Open {

    //=========================================================================================
    // クラス：コマンドを実行する
    // カーソル位置のファイルをShellFilerで定義された関連付け設定8により開きます。
    //   書式 　 OpenFileAssociate8()
    //   引数  　なし
    //   戻り値　bool:実行に成功したときtrue、実行できなかったときfalseを返します。
    //   対応Ver 1.0.0
    //=========================================================================================
    class OpenFileAssociate8Command : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public OpenFileAssociate8Command() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            return OpenFileAssociate1Command.OpenFileAssociate(this, 7);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.OpenFileAssociate8Command;
            }
        }
    }
}
