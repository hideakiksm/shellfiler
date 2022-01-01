using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.FileSystem.Virtual;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem {

    //=========================================================================================
    // クラス：バックグラウンド処理用プロシージャの基底クラス
    //=========================================================================================
    abstract class AbstractBackgroundProcedure {

        //=========================================================================================
        // 機　能：DTOに対応するプロシージャを作成して返す
        // 引　数：[in]arg  リクエスト/レスポンス
        // 戻り値：プロシージャ（処理が不要な場合はnull）
        //=========================================================================================
        public static AbstractBackgroundProcedure CreateProcedure(AbstractProcedureArg arg) {
            AbstractBackgroundProcedure procedure = null;
            if (arg is NopArg) {
                ;
            } else if (arg is SFTPGetUIFileListArg) {
                procedure = new SFTPGetUIFileListProcedure();
            } else if (arg is ShellGetUIFileListArg) {
                procedure = new ShellGetUIFileListProcedure();
            } else if (arg is GetVirtualUIFileListArg) {
                procedure = new GetVirtualUIFileListProcedure();
            } else if (arg is InitializeTerminalArg) {
                procedure = new InitializeTerminalProcedure();
            }
            return procedure;
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in,out]arg    リクエスト/レスポンス
        // 戻り値：ステータスコード
        //=========================================================================================
        public abstract FileOperationStatus Execute(AbstractProcedureArg aarg);

        //=========================================================================================
        // 機　能：処理中に発生した例外またはエラーを処理する
        // 引　数：[in]aarg  リクエスト/レスポンス
        // 　　　　[in]e     例外（エラー通知の場合はnull）
        // 戻り値：なし
        //=========================================================================================
        public virtual void ExecuteOnException(AbstractProcedureArg aarg, Exception e) {
        }
    }
}
