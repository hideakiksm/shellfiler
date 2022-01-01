using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Shell;
using ShellFiler.UI;
using ShellFiler.Util;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {
    
    //=========================================================================================
    // クラス：文字列中のエスケープシーケンスを外す処理の実装クラス
    //=========================================================================================
    public class EscapeSequenceEraser {

        //=========================================================================================
        // 機　能：エスケープを外す処理を実行する
        // 引　数：[in]org   元の文字列
        // 戻り値：エスケープシーケンスを外した文字列
        //=========================================================================================
        public static string Execute(string org) {
            StringBuilder result = new StringBuilder();
            int index = 0;
            while (index < org.Length) {
                if (org[index] != CharCode.CH_ESC) {
                    result.Append(org[index]);
                    index++;
                    continue;
                }
                index++;
                if (index >= org.Length) {
                    continue;
                }
                char ch = org[index];
                if (ch == '[' || ch == '$' || ch == '(') {
                    index++;
                } else if (ch == '*' || ch == '=' || ch == '>') {
                    index++;
                    continue;
                }
                if (index >= org.Length) {
                    continue;
                }
                while (index < org.Length) {
                    ch = org[index];
                    if ('0' <= ch && ch <= '9' || ch == ';' || ch == '>') {
                        index++;
                    } else if (ch == '@' || 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z') {
                        index++;
                        break;
                    } else {
                        index++;
                    }
                }
            }
            return result.ToString();
        }
    }
}
