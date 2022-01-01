using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Terminal.UI;
using ShellFiler.Terminal.TerminalSession;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：ターミナルの初期化をバックグラウンドで実行するリクエスト/レスポンス
    //=========================================================================================
    public class InitializeTerminalArg : AbstractProcedureArg {
        // コンソール画面
        private ConsoleScreen m_consoleScreen; 

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]context  コンテキスト情報
        // 　　　　[in]console  コンソール画面
        // 戻り値：なし
        //=========================================================================================
        public InitializeTerminalArg(FileOperationRequestContext context, ConsoleScreen console) : base(context) {
            m_consoleScreen = console;
        }

        //=========================================================================================
        // プロパティ：このリクエストを非同期に実行するときtrue（戻りを待たなくてよいときtrue）
        //=========================================================================================
        public override bool IsAsyncRequest {
            get {
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：コンソール画面
        //=========================================================================================
        public ConsoleScreen ConsoleScreen {
            get {
                return m_consoleScreen;
            }
        }
    }
}
