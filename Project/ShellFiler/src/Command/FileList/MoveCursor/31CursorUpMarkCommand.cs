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

namespace ShellFiler.Command.FileList.MoveCursor {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マークしながらカーソルを上に{0}行だけ移動します。
    //   書式 　 CursorUpMark(int line)
    //   引数  　line:移動する行数
    // 　　　　　line-default:1
    // 　　　　　line-range:1,999999
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class CursorUpMarkCommand : FileListActionCommand {
        // 移動する行数
        private int m_line;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CursorUpMarkCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_line = (int)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            FileListComponentTarget.CursorUp(m_line, MarkOperation.Mark);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.CursorUpMarkCommand;
            }
        }
    }
}
