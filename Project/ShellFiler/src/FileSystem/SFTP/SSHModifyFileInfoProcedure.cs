using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.Properties;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：SFTPでファイルやディレクトリの属性を一括変更のルールにより変更するプロシージャ
    //=========================================================================================
    class SFTPModifyFileInfoProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPModifyFileInfoProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]path       属性を変更するファイルやディレクトリのフルパス
        // 　　　　[in]modInfo    変更ルール
        // 　　　　[in]modifyCtx  名前変更のコンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public FileOperationStatus Execute(string path, RenameSelectedFileInfo modInfo, ModifyFileInfoContext modifyCtx) {
            RenameSelectedFileInfo.SSHRenameInfo renameInfo = modInfo.SSHInfo;
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
            string fileName = GenericFileStringUtils.GetFileName(local);

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

                // ファイル名
                try {
                    string newFileName = RenameSelectedFileInfoBackgroundTask.GetNewFileName(fileName, renameInfo.ModifyFileNameInfo, modifyCtx);
                    if (fileName != newFileName) {
                        channel.rename(fileName, newFileName);
                    }
                    fileName = newFileName;
                } catch (SftpException) {
                    return FileOperationStatus.ErrorRename;
                }
                if (m_controler.IsCanceled) {
                    return FileOperationStatus.Canceled;
                }

                // パーミッションを変更
                SftpATTRS attrs = null;
                if (renameInfo.AttributeOtherRead != null || renameInfo.AttributeOtherWrite != null || renameInfo.AttributeOwnerExecute != null ||
                        renameInfo.AttributeGroupRead != null || renameInfo.AttributeGroupWrite != null || renameInfo.AttributeGroupExecute != null ||
                        renameInfo.AttributeOtherRead != null || renameInfo.AttributeOtherWrite != null || renameInfo.AttributeOtherExecute != null) {
                    attrs = channel.lstat(fileName);
                    int oldPermissions = attrs.getPermissions();
                    int modPermissions = RenameSelectedFileInfo.SSHRenameInfo.ModifyPermissions(oldPermissions, renameInfo);
                    if (oldPermissions != modPermissions) {
                        try {
                            channel.chmod(modPermissions, fileName);
                        } catch (SftpException) {
                            return FileOperationStatus.ErrorSetAttr;
                        }
                        if (m_controler.IsCanceled) {
                            return FileOperationStatus.Canceled;
                        }
                    }
                }

                // 時刻を変更
                if (renameInfo.UpdateDate != null || renameInfo.UpdateTime != null) {
                    try {
                        DateTime modifiedDate;
                        if (renameInfo.UpdateDate != null && renameInfo.UpdateTime != null) {
                            modifiedDate = new DateTime(renameInfo.UpdateDate.Year, renameInfo.UpdateDate.Month, renameInfo.UpdateDate.Day,
                                                        renameInfo.UpdateTime.Hour, renameInfo.UpdateTime.Minute, renameInfo.UpdateTime.Second);
                        } else {
                            if (attrs == null) {
                                attrs = channel.lstat(fileName);
                            }
                            DateTime orgDate = SFTPFile.SSHToDateTime(attrs.getMTime());
                            DateTimeInfo dateTimeInfo = new DateTimeInfo(orgDate);
                            dateTimeInfo.SetDate(renameInfo.UpdateDate);
                            dateTimeInfo.SetTime(renameInfo.UpdateTime);
                            modifiedDate = dateTimeInfo.ToDateTime();
                        }
                        int mtime = SFTPFile.DateTimeToSSH(modifiedDate);
                        channel.setMtime(fileName, mtime);
                    } catch (SftpException) {
                        return FileOperationStatus.ErrorSetAttr;
                    }
                }

                // オーナーを変更
                if (renameInfo.Owner != null || renameInfo.Group != null) {
                    try {
                        string command = m_controler.Connection.ShellCommandDictionary.GetChownCommand(renameInfo.Owner, renameInfo.Group, basePath + fileName);
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
                }
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
            return FileOperationStatus.Success;
        }
    }
}
