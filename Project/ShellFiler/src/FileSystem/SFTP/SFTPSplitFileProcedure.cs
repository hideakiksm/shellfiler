using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：ファイルを結合するプロシージャ
    //=========================================================================================
    class SFTPSplitFileProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPSplitFileProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]destFolderPath  転送先フォルダ名のフルパス（最後は「/」）
        // 　　　　[in]numberingInfo   ファイルの連番の命名規則
        // 　　　　[in]splitInfo       ファイルの分割方法
        // 　　　　[in]taskLogger      ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext context, string srcFilePath, string destFolderPath, RenameNumberingInfo numberingInfo, SplitFileInfo splitInfo, ITaskLogger taskLogger) {
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

            destFolderPath = SSHUtils.CompleteSFTPDirectory(m_controler.Connection, destFolderPath);
            SSHProtocolType destProtocol;
            string destUser, destServer, destPath;
            int destPortNo;
            success = SSHUtils.SeparateUserServer(destFolderPath, out destProtocol, out destUser, out destServer, out destPortNo, out destPath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            try {
                string destTempPathSuffix;
                status = SplitFile(srcPath, destPath, out destTempPathSuffix, numberingInfo, splitInfo, taskLogger);

                // 削除
                if (destTempPathSuffix != null) {
                    try {
                        ChannelSftp channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);
                        channel.rm(destTempPathSuffix + "*");
                    } catch (SftpException) {
                        taskLogger.LogFileOperationStart(FileOperationType.SplitFile, destPath, false);
                        taskLogger.LogFileOperationEnd(FileOperationStatus.FailDeleteTemp);
                        Program.MainWindow.LogWindow.RegistLogLine(new LogLineSimple(Resources.Log_SSHSplitTempDelete, destTempPathSuffix + "*"));
                        return FileOperationStatus.FailDeleteTemp;
                    }
                }
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
            return status;
        }

        //=========================================================================================
        // 機　能：指定されたファイルを分割する
        // 引　数：[in]srcPath             転送元ファイルのローカルパス
        // 　　　　[in]destPath            転送先フォルダ名のローカルパス（最後は「/」）
        // 　　　　[in]destTempPathSuffix  テンポラリへの分割実施後、そのパスのサフィックスを返す変数（分割未実施のときnull）
        // 　　　　[in]numberingInfo       ファイルの連番の命名規則
        // 　　　　[in]splitInfo           ファイルの分割方法
        // 　　　　[in]taskLogger          ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus SplitFile(string srcPath, string destPath, out string destTempPathSuffix, RenameNumberingInfo numberingInfo, SplitFileInfo splitInfo, ITaskLogger taskLogger) {
            bool success;
            FileOperationStatus status;
            destTempPathSuffix = null;
            
            // テンポラリの存在確認
            ChannelSftp channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);
            string destTempPathSuffixForCheck = destPath + "_sftemp_" + DateTime.Now.Ticks + "_";
            try {
                ArrayList sftpList = channel.ls(destTempPathSuffixForCheck + "*");
                if (sftpList != null && sftpList.Count > 0) {
                    taskLogger.LogFileOperationStart(FileOperationType.SplitFile, srcPath, false);
                    taskLogger.LogFileOperationEnd(FileOperationStatus.Fail);
                    Program.MainWindow.LogWindow.RegistLogLine(new LogLineSimple(Resources.Log_SSHSplitTemp, destTempPathSuffix + "*"));
                    return FileOperationStatus.Fail;
                }
            } catch (SftpException) {
                taskLogger.LogFileOperationStart(FileOperationType.SplitFile, destPath, false);
                return taskLogger.LogFileOperationEnd(FileOperationStatus.CanNotAccess);
            }

            // 転送元の属性取得
            long srcLength;
            try {
                SftpATTRS attr = channel.stat(srcPath);
                srcLength = attr.getSize();
            } catch (SftpException) {
                taskLogger.LogFileOperationStart(FileOperationType.SplitFile, srcPath, false);
                return taskLogger.LogFileOperationEnd(FileOperationStatus.CanNotAccess);
            }
            long oneFileSize;
            int totalFileCount;
            success = splitInfo.GetOneFileSize(srcLength, FileSystemID.SFTP, FileSystemID.SFTP, out oneFileSize, out totalFileCount);
            if (!success) {
                taskLogger.LogFileOperationStart(FileOperationType.SplitFile, srcPath, false);
                return taskLogger.LogFileOperationEnd(FileOperationStatus.TooManyFiles);
            }

            // テンポラリに分割
            try {
                string command = m_controler.Connection.ShellCommandDictionary.GetSplitFileCommand(oneFileSize, srcPath, destTempPathSuffixForCheck);
                ChannelCommandExec exec = new ChannelCommandExec(m_controler.Connection);
                byte[] dummy;
                status = exec.Execute(m_controler.Context.SFTPRequestContext, command, out dummy);
                destTempPathSuffix = destTempPathSuffixForCheck;
            } catch (SftpException) {
                taskLogger.LogFileOperationStart(FileOperationType.SplitFile, srcPath, false);
                return taskLogger.LogFileOperationEnd(FileOperationStatus.Fail);
            }
            if (m_controler.Connection.Closed) {
                taskLogger.LogFileOperationStart(FileOperationType.SplitFile, srcPath, false);
                return taskLogger.LogFileOperationEnd(FileOperationStatus.Canceled);
            }

            // テンポラリを固定
            string destFileTemplate = destPath + GenericFileStringUtils.GetFileName(srcPath);
            ModifyFileInfoContext modifyCtx = new ModifyFileInfoContext();
            for (int i = 0; i < totalFileCount; i++) {
                string oldFileName = string.Format("{0}{1:00}", destTempPathSuffix, i);
                string newFileName = RenameNumberingInfo.CreateSequenceFileName(destFileTemplate, numberingInfo, modifyCtx);
                try {
                    taskLogger.LogFileOperationStart(FileOperationType.SplitFile, newFileName, false);
                    channel.rename(oldFileName, newFileName);
                    taskLogger.LogFileOperationEnd(FileOperationStatus.Success);
                } catch (SftpException) {
                    return taskLogger.LogFileOperationEnd(FileOperationStatus.ErrorRename);
                }
            }

            return FileOperationStatus.Success;
        }
    }
}
