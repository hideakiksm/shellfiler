    //   対応Ver 2.0.0
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Terminal;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.UI;

namespace ShellFiler.Command.Terminal.View {

    //=========================================================================================
    // クラス：コマンドを実行する
    // バックログを下に{0}行だけスクロールします。
    //   書式 　 T_ScrollDown(int line)
    //   引数  　line:スクロールする行数
    // 　　　　　line-default:1
    // 　　　　　line-range:1,999999
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class T_ScrollDownCommand : TerminalActionCommand {
        // スクロールする行数
        private int m_line;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public T_ScrollDownCommand() {
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
            if (!TerminalPanel.VisibleBackLog) {
                TerminalPanel.VisibleBackLog = true;
            }
            TerminalPanel.ScrollLog(false, m_line);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.T_ScrollDownCommand;
            }
        }
    }
}
