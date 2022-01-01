using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.Properties;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：SFTPでディレクトリを作成するプロシージャ
    //=========================================================================================
    class SFPCreateDirectoryProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFPCreateDirectoryProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 　　　　[in]basePath   ディレクトリを作成する場所のバス
        // 　　　　[in]newName    作成するディレクトリ名
        // 戻り値：なし
        //=========================================================================================
        public FileOperationStatus Execute(string basePath, string newName) {
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            string filePath = SSHUtils.CompleteSFTPDirectory(m_controler.Connection, basePath);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            try {
                // 目的のディレクトリに変更
                ChannelSftp channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);
                try {
                    channel.cd(path);
                } catch (SftpException) {
                    return FileOperationStatus.FileNotFound;
                }
                if (m_controler.IsCanceled) {
                    return FileOperationStatus.Canceled;
                }

                // 存在確認
                try {
                    channel.stat(newName);
                    if (m_controler.IsCanceled) {
                        return FileOperationStatus.Canceled;
                    }
                    return FileOperationStatus.AlreadyExists;
                } catch (SftpException) {
                    // 存在しない場合はここを通る
                }

                // 作成
                try {
                    channel.mkdir(newName);
                } catch (SftpException) {
                    return FileOperationStatus.ErrorMkDir;
                }
                if (m_controler.IsCanceled) {
                    return FileOperationStatus.Canceled;
                }
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
            return FileOperationStatus.SuccessMkDir;
        }
    }
}
