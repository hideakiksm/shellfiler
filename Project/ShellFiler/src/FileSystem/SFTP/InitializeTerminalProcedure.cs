using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Properties;
using ShellFiler.Virtual;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.UI;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：ターミナルの初期化をバックグラウンドで実行するプロシージャ
    //=========================================================================================
    class InitializeTerminalProcedure : AbstractSFTPBackgroundProcedure {

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in,out]arg    リクエスト/レスポンス
        // 戻り値：ステータスコード
        //=========================================================================================
        public override FileOperationStatus Execute(AbstractProcedureArg aarg) {
            InitializeTerminalArg arg = (InitializeTerminalArg)aarg;
            TerminalShellChannel resultTerminal;
            string errorDetail;
            FileOperationStatus status = ExecuteRequest(SSHConnection, arg.RequestContext, arg.ConsoleScreen, out resultTerminal, out errorDetail);
            if (SSHConnection.Closed) {
                status = FileOperationStatus.Canceled;
            }
            arg.Status = status;
            NotifyTaskEnd(arg, status, errorDetail);
            return status;
        }

        //=========================================================================================
        // 機　能：コマンドを実際に実行する
        // 引　数：[in]connection       接続
        // 　　　　[in]context          コンテキスト情報
        // 　　　　[in]console          コンソール画面
        // 　　　　[out]resultTerminal  結果のターミナル接続を返す変数
        // 　　　　[out]errorDetail     詳細メッセージ
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus ExecuteRequest(SSHConnection connection, FileOperationRequestContext context, ConsoleScreen console, out TerminalShellChannel resultTerminal, out string errorDetail) {
            resultTerminal = null;
            errorDetail = null;
            FileOperationStatus status;

            // 準備
            SFTPProcedureControler controler = new SFTPProcedureControler(connection, context);
            status = controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            // 接続
            resultTerminal = new TerminalShellChannel(connection, console, false);
            status = resultTerminal.Connect(out errorDetail);
            if (!status.Succeeded) {
                return status;
            }
            if (controler.IsCanceled) {
                return FileOperationStatus.Canceled;
            }
            connection.SetTerminalChannel(resultTerminal);
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：処理中に発生した例外またはエラーを処理する
        // 引　数：[in]aarg  リクエスト/レスポンス
        // 　　　　[in]e     例外（エラー通知の場合はnull）
        // 戻り値：なし
        //=========================================================================================
        public override void ExecuteOnException(AbstractProcedureArg aarg, Exception e) {
            InitializeTerminalArg arg = (InitializeTerminalArg)aarg;
            if (aarg.Status == FileOperationStatus.Canceled) {
                arg.Status = FileOperationStatus.Canceled;
            } else if (e != null) {
                arg.Status = FileOperationStatus.Fail;
            } else if (arg.Status.Succeeded) {
                arg.Status = FileOperationStatus.Fail;
            }
            string message;
            if (e == null) {
                message = "";
            } else {
                message = e.Message;
            }
            NotifyTaskEnd(arg, arg.Status, message);
        }

        //=========================================================================================
        // 機　能：一覧取得タスクをUIに通知する
        // 引　数：[in]status      接続のステータス
        // 　　　　[in]channel     接続結果（失敗のステータスのときnull）
        // 　　　　[in]errorDetail 詳細エラー情報
        // 戻り値：なし
        //=========================================================================================
        private void NotifyTaskEnd(InitializeTerminalArg arg, FileOperationStatus status, string errorDetail) {
            arg.ConsoleScreen.NotifyConnect(status, errorDetail);
        }
    }
}
