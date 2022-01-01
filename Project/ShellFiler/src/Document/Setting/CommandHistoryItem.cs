using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileViewer;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：コマンドの入力履歴の項目
    //=========================================================================================
    public class CommandHistoryItem {
        // コマンドラインの文字列
        private string m_commandString;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]command  コマンドラインの文字列
        // 戻り値：なし
        //=========================================================================================
        public CommandHistoryItem(string command) {
            m_commandString = command;
        }

        //=========================================================================================
        // 機　能：コマンドライン項目が等しいときtrueを返す
        // 引　数：[in]other   コマンドライン項目
        // 戻り値：なし
        //=========================================================================================
        public bool EqualsCommand(CommandHistoryItem other) {
            if (m_commandString == other.m_commandString) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドラインの文字列
        //=========================================================================================
        public string CommandString {
            get {
                return m_commandString;
            }
        }
    }
}
