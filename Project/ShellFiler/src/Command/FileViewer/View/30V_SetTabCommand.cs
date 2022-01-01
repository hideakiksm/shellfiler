using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileViewer.View {

    //=========================================================================================
    // クラス：コマンドを実行する
    // タブ幅を{0}文字にセットします。
    //   書式 　 V_SetTab(int tab)
    //   引数  　tab:タブ幅（2～16）
    // 　　　　　tab-default:4
    // 　　　　　tab-range:2,16
    //   戻り値　タブ幅を変更したときtrue
    //   対応Ver 0.0.1
    //=========================================================================================
    class V_SetTabCommand : FileViewerActionCommand {
        // タブ幅
        private int m_tab;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_SetTabCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_tab = (int)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (!Abailable) {
                return false;
            }
            if (m_tab < 2 || m_tab > 16) {
                return false;
            }
            return TextFileViewer.TextViewerComponent.SetTab(m_tab);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_SetTabCommand;
            }
        }
    }
}
