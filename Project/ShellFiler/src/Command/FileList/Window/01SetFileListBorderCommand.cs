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
    // ファイル一覧の境界を{0}%の位置に設定します。
    //   書式 　 SetFileListBorderRatio(int pixel)
    //   引数  　pixel:設定する割合（0～100）
    // 　　　　　pixel-default:30
    // 　　　　　pixel-range:0,100
    //   戻り値　int:新しい境界位置をピクセル単位で返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class SetFileListBorderRatioCommand : FileListActionCommand {
        // 設定する割合（0～100）
        private int m_ratio;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SetFileListBorderRatioCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_ratio = (int)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            return Program.MainWindow.SetFileListBorderRatio(m_ratio);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.SetFileListBorderRatioCommand;
            }
        }
    }
}
