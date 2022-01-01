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

namespace ShellFiler.Command.Terminal.Console {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ファンクションキー{0}がシフト{1}で押されたことを通知します。
    //   書式 　 T_SendEscape(int funcNum, bool isModify)
    //   引数  　funcNum:ファンクションキーの番号
    // 　　　　　funcNum-default:1
    // 　　　　　funcNum-range:1,22
    //   　　  　isModify:シフトが押された状態のときtrue
    // 　　　　　isModify-default:1
    // 　　　　　isModify-range:1,22
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class T_SendKeyFunctionCommand : TerminalActionCommand {
	    private static string[] FUNCTIONKEY_MAP = { 
	    //     F1    F2    F3    F4    F5    F6    F7    F8    F9    F10   F11  F12
		      "11", "12", "13", "14", "15", "17", "18", "19", "20", "21", "23", "24",
        //     F13   F14   F15   F16   F17  F18   F19   F20   F21   F22
              "25", "26", "28", "29", "31", "32", "33", "34", "23", "24"
        };

        // ファンクションキーの番号
        private int m_funcNum;

        // シフトが押された状態のときtrue
        private bool m_isModify;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public T_SendKeyFunctionCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_funcNum = (int)param[0];
            m_isModify = (bool)param[1];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (m_funcNum < 1 || m_funcNum > 22) {
                return null;
            }
		    string tail;
		    if (m_funcNum >= 20) {
                if (!m_isModify) {
                    tail = "@";
                } else {
                    tail = "$";
                }
            } else {
                if (!m_isModify) {
                    tail = "^";
                } else {
                    tail = "~";
                }
            }
		    string command = ConsoleScreen.CH_ESC + "[" + FUNCTIONKEY_MAP[m_funcNum - 1] + tail;
            TerminalPanel.SendCommand(command);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.T_SendKeyFunctionCommand;
            }
        }
    }
}
