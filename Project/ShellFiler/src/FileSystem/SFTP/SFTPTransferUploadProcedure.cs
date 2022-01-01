using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Properties;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask.FileFilter;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：ファイルをサーバにアップロードするプロシージャ
    //=========================================================================================
    class SFTPTransferUploadProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        // コンテキスト情報
        private FileOperationRequestContext m_context;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPTransferUploadProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
            m_context = context;
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]progress      進捗状態を通知するdelegate
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(string srcFilePath, string destFilePath, FileProgressEventHandler progress) {
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            string filePath = destFilePath;
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            try {
                ChannelSftp channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);

                // コピーを実行
                ChannelProgress monitor = new ChannelProgress(progress);
                try {
                    channel.put(srcFilePath, path, monitor, ChannelSftp.OVERWRITE);
                } catch (SftpException) {
                    try {
                        channel.rm(path);
                    } catch (Exception) {
                    }
                    return FileOperationStatus.Fail;
                }
                if (monitor.Canceled || m_controler.IsCanceled) {
                    try {
                        channel.rm(path);
                    } catch (Exception) {
                    }
                    return FileOperationStatus.Canceled;
                }
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
            return FileOperationStatus.SuccessCopy;
        }
    }
}
