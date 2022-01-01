using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.Properties;
using ShellFiler.UI.Log;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：ファイルを結合するプロシージャ
    //=========================================================================================
    class SFTPCombineFileProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPCombineFileProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]srcPathList     転送元ファイル名のフルパス
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]taskLogger      ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Execute(List<string> srcPathList, string destFilePath, ITaskLogger taskLogger) {
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            bool success;
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
                    taskLogger.LogFileOperationStart(FileOperationType.CombineFile, destFilePath, false);
                    return taskLogger.LogFileOperationEnd(FileOperationStatus.AlreadyExists);
                }
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }

            // 転送開始
            bool successAll = false;
            try {
                for (int i = 0; i < srcPathList.Count; i++) {
                    // 1ファイル結合
                    taskLogger.LogFileOperationStart(FileOperationType.CombineFile, srcPathList[i], false);
                    status = AppendFile(srcPathList[i], destPath, (i == 0));
                    taskLogger.LogFileOperationEnd(status);
                    
                    if (!status.Succeeded) {
                        break;
                    }
                    if (i == srcPathList.Count - 1) {
                        successAll = true;
                    }
                }
            } finally {
                if (!successAll) {
                    // 全件成功しなかった場合は転送先を削除
                    try {
                        ChannelSftp channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);
                        channel.rm(destPath);
                    } catch (Exception) {
                    }
                }
            }
            return FileOperationStatus.Success;
        }
        
        //=========================================================================================
        // 機　能：ファイル１件を結合する
        // 引　数：[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]destLocalPath   転送先ファイル名のフルパス（ローカルパス）
        // 　　　　[in]firstFile       先頭ファイルのときtrue
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus AppendFile(string srcFilePath, string destLocalPath, bool firstFile) {
            FileOperationStatus status;
            bool success;
            srcFilePath = SSHUtils.CompleteSFTPDirectory(m_controler.Connection, srcFilePath);
            SSHProtocolType srcProtocol;
            string srcUser, srcServer, srcPath;
            int srcPortNo;
            success = SSHUtils.SeparateUserServer(srcFilePath, out srcProtocol, out srcUser, out srcServer, out srcPortNo, out srcPath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // 結合を実行
            try {
                ChannelSftp channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);
                string command = m_controler.Connection.ShellCommandDictionary.GetAppendFileCommand(srcPath, destLocalPath, firstFile);
                ChannelCommandExec exec = new ChannelCommandExec(m_controler.Connection);
                byte[] dummy;
                status = exec.Execute(m_controler.Context.SFTPRequestContext, command, out dummy);
            } catch (SftpException) {
                return FileOperationStatus.Fail;
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
