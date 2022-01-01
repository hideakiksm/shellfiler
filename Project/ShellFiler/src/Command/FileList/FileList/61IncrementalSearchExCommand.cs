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
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.FileList {

    //=========================================================================================
    // クラス：コマンドを実行する
    // インクリメンタルサーチにより、対象パスのうち、{0}から始まるファイル名を検索します。
    //   書式 　 IncrementalSearchEx(string keyword)
    //   引数  　keyword:検索するファイルの先頭文字列
    //           keyword-default:
    // 　　　　　keyword-range:
    //   戻り値　bool:対象となるファイルが見つかったときtrue、見つからなかったときfalseを返します。
    //   対応Ver 1.3.0
    //=========================================================================================
    class IncrementalSearchExCommand : FileListActionCommand {
        // 検索するファイルの先頭文字列
        private string m_keyword;


        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public IncrementalSearchExCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        // メ　モ：[0]:検索するファイルの先頭文字列
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_keyword = (string)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            bool success = FileListViewTarget.FileListViewComponent.IncrementalSearch(m_keyword, true, IncrementalSearchOperation.MoveDown);
            return success;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.IncrementalSearchExCommand;
            }
        }
    }
}
