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
    // クラス：SFTPでファイル/ディレクトリを削除するプロシージャ
    //=========================================================================================
    class SFTPDeleteProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPDeleteProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：ファイル/フォルダを削除する
        // 引　数：[in]filePath    削除するファイルのパス
        // 　　　　[in]isTarget    対象パスを削除するときtrue、反対パスのときfalse
        // 　　　　[in]flag        削除フラグ
        // 戻り値：ステータス（成功のときSuccessDelete）
        //=========================================================================================
        public FileOperationStatus Execute(string filePath, bool isTarget, DeleteFileFolderFlag flag) {
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            // フラグを読み込み
            bool isDirectory;
            if ((flag & DeleteFileFolderFlag.FILE) != 0) {
                isDirectory = false;
            } else if ((flag & DeleteFileFolderFlag.FOLDER) != 0) {
                isDirectory = true;
            } else {
                Program.Abort("削除モードが正しくありません。");
                return FileOperationStatus.ErrorDelete;
            }
            bool withAttr = ((flag & DeleteFileFolderFlag.CHANGE_ATTR) != 0);
            bool delDirect = ((flag & DeleteFileFolderFlag.RECYCLE_OR_RF) != 0);

            // パスを分解
            filePath = SSHUtils.CompleteSFTPDirectory(m_controler.Connection, filePath);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // 削除を実行
            if (!withAttr && isDirectory && delDirect) {
                status = DeleteDirectoryDirect(path);
            } else {
                status = DeleteFileDirectorySFTP(path, isDirectory, withAttr, delDirect);
            }
            return status;
        }

        //=========================================================================================
        // 機　能：SFTPを使ってファイルまたはディレクトリを削除する
        // 引　数：[in]path        削除するファイルまたはディレクトリのフルパス
        // 　　　　[in]isDirectory ディレクトリのときtrue
        // 　　　　[in]withAttr    属性変更を伴うときtrue
        // 　　　　[in]delDirect   rm -rfを使って削除するときtrue
        // 戻り値：なし
        //=========================================================================================
        private FileOperationStatus DeleteFileDirectorySFTP(string path, bool isDirectory, bool withAttr, bool delDirect) {
            string fileName = GenericFileStringUtils.GetFileName(path, '/');
            string basePath = GenericFileStringUtils.GetDirectoryName(path, '/');
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

                // 属性を変更
                if (withAttr) {
                    try {
                        channel.chmod(0x1ff, fileName);     // 777を指定
                        if (m_controler.IsCanceled) {
                            return FileOperationStatus.Canceled;
                        }
                    } catch (SftpException) {
                        return FileOperationStatus.ErrorSetAttr;
                    }
                }

                // 削除
                try {
                    if (delDirect) {
                        return DeleteDirectoryDirect(path);
                    } else if (isDirectory) {
                        channel.rmdir(fileName);
                    } else {
                        channel.rm(fileName);
                    }
                } catch (SftpException) {
                    return FileOperationStatus.ErrorDelete;
                }
                if (m_controler.IsCanceled) {
                    return FileOperationStatus.Canceled;
                }
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
            if (isDirectory) {
                return FileOperationStatus.SuccessDelDir;
            } else {
                return FileOperationStatus.SuccessDelete;
            }
        }

        //=========================================================================================
        // 機　能：rm -rfを使ってディレクトリを削除する
        // 引　数：[in]path        削除するファイルまたはディレクトリのフルパス
        // 戻り値：なし
        //=========================================================================================
        private FileOperationStatus DeleteDirectoryDirect(string path) {
            FileOperationStatus status;
            if (path == "/" || path.StartsWith("/ ")) {
                return FileOperationStatus.NotSupport;
            }
            try {
                ChannelSftp channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);
                string command = m_controler.Connection.ShellCommandDictionary.GetRmRfCommand(path);
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
