using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Properties;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask.DataObject;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：SFTPでファイルやディレクトリの属性を変更するプロシージャ
    //=========================================================================================
    class SFTPRenameProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPRenameProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]path          属性を変更するファイルやディレクトリのフルパス
        // 　　　　[in]originalInfo  変更前のファイル情報
        // 　　　　[in]newInfo       変更後のファイル情報
        // 戻り値：ステータスコード（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(string path, RenameFileInfo originalInfo, RenameFileInfo newInfo) {
            string pathAll = SSHUtils.CompleteSFTPDirectory(m_controler.Connection, path);
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            SSHProtocolType protocol;
            string user, server, local;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(pathAll, out protocol, out user, out server, out portNo, out local);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }
            string basePath = GenericFileStringUtils.GetDirectoryName(local, '/');
            if (!basePath.EndsWith("/")) {
                basePath += "/";
            }

            try {
                // 目的のディレクトリに変更
                ChannelSftp channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);
                try {
                    channel.cd(basePath);
                } catch (SftpException) {
                    return FileOperationStatus.FileNotFound;
                }
                if (m_controler.IsCanceled) {
                    return FileOperationStatus.Canceled;
                }

                // ファイル名を変更
                try {
                    string oldFileName = originalInfo.SSHInfo.FileName;
                    string newFileName = newInfo.SSHInfo.FileName;
                    if (oldFileName != newFileName) {
                        channel.rename(oldFileName, newFileName);
                    }
                } catch (SftpException) {
                    return FileOperationStatus.ErrorRename;
                }
                if (m_controler.IsCanceled) {
                    return FileOperationStatus.Canceled;
                }

                // パーミッションを変更
                if (originalInfo.SSHInfo.Permissions != newInfo.SSHInfo.Permissions) {
                    try {
                        channel.chmod(newInfo.SSHInfo.Permissions, newInfo.SSHInfo.FileName);
                    } catch (SftpException) {
                        return FileOperationStatus.ErrorSetAttr;
                    }
                    if (m_controler.IsCanceled) {
                        return FileOperationStatus.Canceled;
                    }
                }

                // 時刻を変更
                if (originalInfo.SSHInfo.ModifiedDate != newInfo.SSHInfo.ModifiedDate) {
                    try {
                        int mtime = SFTPFile.DateTimeToSSH(newInfo.SSHInfo.ModifiedDate);
                        channel.setMtime(newInfo.SSHInfo.FileName, mtime);
                    } catch (SftpException) {
                        return FileOperationStatus.ErrorSetAttr;
                    }
                }

                // オーナーを変更
                try {
                    string command = null;
                    string oldOwner = originalInfo.SSHInfo.Owner;
                    string newOwner = newInfo.SSHInfo.Owner;
                    string oldGroup = originalInfo.SSHInfo.Group;
                    string newGroup = newInfo.SSHInfo.Group;
                    if (oldOwner != newOwner && oldGroup == newGroup) {
                        command = m_controler.Connection.ShellCommandDictionary.GetChownCommand(newOwner, null, local);
                    } else if (oldOwner == newOwner && oldGroup != newGroup) {
                        command = m_controler.Connection.ShellCommandDictionary.GetChownCommand(null, newGroup, local);
                    } else if (oldOwner != newOwner && oldGroup != newGroup) {
                        command = m_controler.Connection.ShellCommandDictionary.GetChownCommand(newOwner, newGroup, local);
                    }
                    if (command != null) {
                        ChannelCommandExec exec = new ChannelCommandExec(m_controler.Connection);
                        byte[] dummy = null;
                        status = exec.Execute(m_controler.Context.SFTPRequestContext, command, out dummy);
                        if (!status.Succeeded) {
                            return FileOperationStatus.ErrorChmod;
                        }
                    }
                } catch (Exception e) {
                    return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
                }
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
            return FileOperationStatus.Success;
        }
    }
}
