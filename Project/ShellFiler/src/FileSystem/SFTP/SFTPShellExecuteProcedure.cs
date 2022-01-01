using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using ShellFiler.Properties;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileViewer;
using ShellFiler.Util;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：SFTPでシェルコマンドを実行するプロシージャ
    //=========================================================================================
    class SFTPShellExecuteProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPShellExecuteProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]dirName     カレントディレクトリ名
        // 　　　　[in]command     コマンドライン
        // 　　　　[in]relayErrLog 標準エラーの結果をログ出力するときtrue
        // 　　　　[in]dataTarget  取得データの格納先
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(string dirName, string command, bool relayErrLog, IRetrieveFileDataTarget dataTarget) {
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            string filePath = SSHUtils.CompleteSFTPDirectory(m_controler.Connection, dirName);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // コマンドを実行
            try {
                Stream stdOut = new BridgeLogOutStream(dataTarget);
                Stream stdErr;
                if (relayErrLog) {
                    stdErr = new BridgeLogOutStream(dataTarget);
                } else {
                    stdErr = new BridgeStdErrStream(dataTarget, m_controler.Connection.AuthenticateSetting.Encoding, true);
                }
                ChannelCommandExec exec = new ChannelCommandExec(m_controler.Connection);
                status = exec.ExecuteByStream(m_controler.Context.SFTPRequestContext, command, stdOut, stdErr, dataTarget);
                if (status != FileOperationStatus.Success) {
                    return status;
                }
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
            if (m_controler.IsCanceled) {
                return FileOperationStatus.Canceled;
            }
            return FileOperationStatus.Success;
        }
    }
}
