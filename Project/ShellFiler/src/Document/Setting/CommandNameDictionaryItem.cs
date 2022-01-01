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
    // クラス：コマンド名の辞書の項目
    //=========================================================================================
    public class CommandNameDictionaryItem {
        // コマンドラインの文字列
        private string m_commandString;
        
        // コマンドのフルパス
        private string m_fullPath;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]command  コマンドラインの文字列
        // 　　　　[in]fullPath コマンドのフルパス
        // 戻り値：なし
        //=========================================================================================
        public CommandNameDictionaryItem(string command, string fullPath) {
            m_commandString = command;
            m_fullPath = fullPath;
        }

        //=========================================================================================
        // プロパティ：コマンドラインの文字列
        //=========================================================================================
        public string CommandString {
            get {
                return m_commandString;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのフルパス
        //=========================================================================================
        public string FullPath {
            get {
                return m_fullPath;
            }
        }
    }
}
