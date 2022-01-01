using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.Virtual;

namespace ShellFiler.FileTask.ArgumentConverter {

    //=========================================================================================
    // 列挙子：プログラム引数の解析エラー
    //=========================================================================================
    public class ParseError {
        public static readonly ParseError KeyInputNotClose  = new ParseError(Resources.ArgumentParse_ErrorKeyInputNotClose);      // 「$<」が「>」で閉じていません。
        public static readonly ParseError VariableNameEmpty = new ParseError(Resources.ArgumentParse_ErrorVariableNameEmpty);     // 「$」の後に変数名がありません。
        public static readonly ParseError DecoratorEmpty    = new ParseError(Resources.ArgumentParse_ErrorDecoratorEmpty);        // 「:」の後に修飾子がありません。
        public static readonly ParseError UnknownVariable   = new ParseError(Resources.ArgumentParse_ErrorUnknownVariable);       // 「$」の後に未知の変数名が記述されています。
        public static readonly ParseError UnknownDecorator  = new ParseError(Resources.ArgumentParse_ErrorUnknownDecorator);      // 「:」の後に未知の修飾子が記述されています。

        // エラーメッセージのテンプレート
        private string m_errorMessage;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]errorMessage  エラーメッセージのテンプレート
        // 戻り値：なし
        //=========================================================================================
        private ParseError(string errorMessage) {
            m_errorMessage = errorMessage;
        }

        //=========================================================================================
        // プロパティ：エラーメッセージのテンプレート
        //=========================================================================================
        public string ErrorMessage {
            get {
                return m_errorMessage;
            }
        }
    }
}
