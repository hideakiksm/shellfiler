using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.FileTask;

namespace ShellFiler.Command.FileList.FileOperation {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マークされたファイルを、方式を指定して反対パスに移動します。
    //   書式 　 MoveEx()
    //   引数  　なし
    //   戻り値　bool:移動をバックグラウンドで開始したときtrue、移動を開始できなかったときfalseを返します。
    //   対応Ver 1.1.0
    //=========================================================================================
    class MoveExCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MoveExCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            return MoveCommand.MoveExecute(FileListViewTarget, FileListViewOpposite, true);
        }
        
        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.MoveExCommand;
            }
        }
    }
}
