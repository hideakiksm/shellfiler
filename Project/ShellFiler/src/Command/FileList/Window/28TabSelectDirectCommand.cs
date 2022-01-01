using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.Window {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 指定された番号のタブを{0}番目（0から開始）のタブに選択します。
    //   書式 　 TabSelectDirect(int index)
    //   引数  　index:選択するタブのインデックス（0～9）
    // 　　　　　index-default:0
    // 　　　　　index-range:0,9
    //   戻り値　bool:タブを選択したときtrue、該当するタブがなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class TabSelectDirectCommand : FileListActionCommand {
        // 選択するタブのインデックス（0～9）
        private int m_index;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TabSelectDirectCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_index = (int)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (m_index < 0 || Program.Document.TabPageList.Count <= m_index) {
                return false;
            }

            Program.MainWindow.TabControlImpl.SelectTabDirect(m_index);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.TabSelectDirectCommand;
            }
        }
    }
}
