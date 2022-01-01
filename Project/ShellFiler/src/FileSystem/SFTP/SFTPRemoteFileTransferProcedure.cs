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
    // クラス：リモートから別のリモートへファイルをコピー／移動するプロシージャ
    //=========================================================================================
    class SFTPRemoteFileTransferProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPRemoteFileTransferProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]transferMode    ファイル転送のモード
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]overwrite       上書きするときtrue
        // 　　　　[in]attrCopy        属性コピーを行うときtrue
        // 　　　　[in]progress        進捗状態を通知するdelegate
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(TransferModeType transferMode, string srcFilePath, string destFilePath, bool overwrite, bool attrCopy, FileProgressEventHandler progress) {
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            bool success;
            srcFilePath = SSHUtils.CompleteSFTPDirectory(m_controler.Connection, srcFilePath);
            SSHProtocolType srcProtocol;
            string srcUser, srcServer, srcPath;
            int srcPortNo;
            success = SSHUtils.SeparateUserServer(srcFilePath, out srcProtocol, out srcUser, out srcServer, out srcPortNo, out srcPath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            destFilePath = SSHUtils.CompleteSFTPDirectory(m_controler.Connection, destFilePath);
            SSHProtocolType destProtocol;
            string destUser, destServer, destPath;
            int destPortNo;
            success = SSHUtils.SeparateUserServer(destFilePath, out destProtocol, out destUser, out destServer, out destPortNo, out destPath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // 上書き確認
            try {
                if (!overwrite) {
                    // 上書きしない場合
                    bool alreadyExist = false;
                    try {
                        ChannelSftp channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);
                        channel.stat(destPath);
                        alreadyExist = true;
                    } catch (SftpException) {
                        // 存在しない場合はここを通る
                    }
                    if (m_controler.IsCanceled) {
                        return FileOperationStatus.Canceled;
                    }
                    if (alreadyExist) {
                        return FileOperationStatus.AlreadyExists;
                    }
                }
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }

            // コピー/移動を実行
            try {
                ChannelSftp channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);
                string command = null;
                if (transferMode == TransferModeType.CopyFile) {
                    command = m_controler.Connection.ShellCommandDictionary.GetCpCommand(srcPath, destPath, attrCopy);
                } else if (transferMode == TransferModeType.CopyDirectory) {
                    command = m_controler.Connection.ShellCommandDictionary.GetCpRCommand(srcPath, destPath, attrCopy);
                } else {
                    command = m_controler.Connection.ShellCommandDictionary.GetMvCommand(srcPath, destPath, attrCopy);
                }
                ChannelCommandExec exec = new ChannelCommandExec(m_controler.Connection);
                byte[] dummy;
                status = exec.Execute(m_controler.Context.SFTPRequestContext, command, out dummy);
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
            if (m_controler.Connection.Closed) {
                return FileOperationStatus.Canceled;
            }
            return status;
        }
    }
}
