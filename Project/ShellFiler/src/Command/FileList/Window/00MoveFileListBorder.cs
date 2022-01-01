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

namespace ShellFiler.Command.FileList.Window {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ファイル一覧の境界を右に{0}ピクセルだけ移動します。
    //   書式 　 MoveFileListBorder(int pixel)
    //   引数  　pixel:移動するピクセル数（右側がプラス）
    // 　　　　　pixel-default:30
    // 　　　　　pixel-range:-99999,99999
    //   戻り値　int:新しい境界位置をピクセル単位で返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class MoveFileListBorderCommand : FileListActionCommand {
        // 移動するピクセル数（右側がプラス）
        private int m_pixel;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MoveFileListBorderCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_pixel = (int)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            return Program.MainWindow.MoveFileListBorder(m_pixel);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.MoveFileListBorderCommand;
            }
        }
    }
}
