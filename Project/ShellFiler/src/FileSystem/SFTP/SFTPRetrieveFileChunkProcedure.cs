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
    // クラス：ファイル情報を取得するプロシージャ
    //=========================================================================================
    class SFTPRetrieveFileChunkProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPRetrieveFileChunkProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：ファイルを取得する
        // 引　数：[in]file    アクセスしたいファイルの情報
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(AccessibleFile file) {
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            string filePath = file.FilePath;
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // headコマンドを実行
            try {
                Stream stdOut = new BridgeStdOutStream(file, file.FastRead);
                Stream stdErr = new BridgeStdErrStream(file, m_controler.Connection.AuthenticateSetting.Encoding, file.FastRead);
                string command = m_controler.Connection.ShellCommandDictionary.GetCommandGetFileHead(path, file.MaxFileSize + 1);
                ChannelCommandExec exec = new ChannelCommandExec(m_controler.Connection);
                status = exec.ExecuteByStream(m_controler.Context.SFTPRequestContext, command, stdOut, stdErr, file);
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
