using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ShellFiler.Properties;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask.FileFilter;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：ファイルをサーバからダウンロードするプロシージャ
    //=========================================================================================
    class SFTPDownloadFileProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPDownloadFileProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]overwrite     上書きするときtrue
        // 　　　　[in]progress      進捗状態を通知するdelegate
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext context, string srcFilePath, string destFilePath, FileProgressEventHandler progress) {
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            string filePath = SSHUtils.CompleteSFTPDirectory(m_controler.Connection, srcFilePath);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // コピーを実行
            ChannelSftp channel;
            try {
                channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }

            ChannelProgress monitor = new ChannelProgress(progress);
            try {
                channel.get(path, destFilePath, monitor, ChannelSftp.OVERWRITE);
            } catch (SftpException) {
                return FileOperationStatus.Fail;
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
            if (monitor.Canceled || m_controler.IsCanceled) {
                channel.rm(destFilePath);
                return FileOperationStatus.Canceled;
            }
            if (m_controler.Connection.Closed) {
                return FileOperationStatus.Canceled;
            }

            return FileOperationStatus.SuccessCopy;
        }
    }
}
