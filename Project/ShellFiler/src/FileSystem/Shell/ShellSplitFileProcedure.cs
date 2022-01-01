using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.GraphicsViewer;
using ShellFiler.Virtual;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.TerminalSession.CommandEmulator;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;

namespace ShellFiler.FileSystem.Shell {

    //=========================================================================================
    // クラス：ファイルを分割するプロシージャ
    //=========================================================================================
    class ShellSplitFileProcedure : ShellProcedure {
        // SSH内部処理のコントローラ
        private ShellProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellSplitFileProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new ShellProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：ファイルを分割する
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]destFolderPath  転送先フォルダ名のフルパス（最後は「\」）
        // 　　　　[in]numberingInfo   ファイルの連番の命名規則
        // 　　　　[in]splitInfo       ファイルの分割方法
        // 　　　　[in]taskLogger      ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Execute(string srcFilePath, string destFolderPath, RenameNumberingInfo numberingInfo, SplitFileInfo splitInfo, ITaskLogger taskLogger) {
            bool success;
            FileOperationStatus status = m_controler.Initialize(srcFilePath, true, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            srcFilePath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, srcFilePath);
            SSHProtocolType srcProtocol;
            string srcUser, srcServer, srcPath;
            int srcPortNo;
            success = SSHUtils.SeparateUserServer(srcFilePath, out srcProtocol, out srcUser, out srcServer, out srcPortNo, out srcPath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            destFolderPath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, destFolderPath);
            SSHProtocolType destProtocol;
            string destUser, destServer, destPath;
            int destPortNo;
            success = SSHUtils.SeparateUserServer(destFolderPath, out destProtocol, out destUser, out destServer, out destPortNo, out destPath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            string destTempPathSuffix;
            status = SplitFile(srcPath, destPath, out destTempPathSuffix, numberingInfo, splitInfo, taskLogger);

            // 削除
            if (destTempPathSuffix != null) {
                status = ShellUtils.DeleteFile(m_controler, destTempPathSuffix + "*");
                if (!status.Succeeded) {
                    taskLogger.LogFileOperationStart(FileOperationType.SplitFile, destPath, false);
                    taskLogger.LogFileOperationEnd(FileOperationStatus.FailDeleteTemp);
                    Program.MainWindow.LogWindow.RegistLogLine(new LogLineSimple(Resources.Log_SSHSplitTempDelete, destTempPathSuffix + "*"));
                    return FileOperationStatus.FailDeleteTemp;
                }
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
            ShellCommandDictionary dictionary = m_controler.Connection.ShellCommandDictionary;
            ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;
            
            // テンポラリの存在確認
            string fileSuffixForCheck = "_sftemp_" + DateTime.Now.Ticks + "_";
            string destTempPathSuffixForCheck = destPath + fileSuffixForCheck;
            {
                ShellEngineGetFileList getFileListEngine = new ShellEngineGetFileList(emulator, m_controler.Connection, destPath);
                status = emulator.Execute(m_controler.Context, getFileListEngine);
                if (!status.Succeeded) {
                    taskLogger.LogFileOperationStart(FileOperationType.SplitFile, srcPath, false);
                    return taskLogger.LogFileOperationEnd(FileOperationStatus.CanNotAccess);
                }
                int count = GenericFileStringUtils.CountFileStartsWith(getFileListEngine.ResultFileList, fileSuffixForCheck, false);
                if (count > 0) {
                    taskLogger.LogFileOperationStart(FileOperationType.SplitFile, srcPath, false);
                    taskLogger.LogFileOperationEnd(FileOperationStatus.Fail);
                    Program.MainWindow.LogWindow.RegistLogLine(new LogLineSimple(Resources.Log_SSHSplitTemp, destTempPathSuffix + "*"));
                    return FileOperationStatus.Fail;
                }
            }

            // 転送元の属性取得
            long srcLength;
            {
                ShellEngineGetFileInfo engine = new ShellEngineGetFileInfo(emulator, m_controler.Connection, srcPath);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded || engine.ResultFileInfo == null) {
                    taskLogger.LogFileOperationStart(FileOperationType.SplitFile, srcPath, false);
                    return taskLogger.LogFileOperationEnd(FileOperationStatus.CanNotAccess);
                }
                srcLength = engine.ResultFileInfo.FileSize;
            }
            long oneFileSize;
            int totalFileCount;
            success = splitInfo.GetOneFileSize(srcLength, FileSystemID.SSHShell, FileSystemID.SSHShell, out oneFileSize, out totalFileCount);
            if (!success) {
                taskLogger.LogFileOperationStart(FileOperationType.SplitFile, srcPath, false);
                return taskLogger.LogFileOperationEnd(FileOperationStatus.TooManyFiles);
            }

            // テンポラリに分割
            {
                string strCommand = dictionary.GetSplitFileCommand(oneFileSize, srcPath, destTempPathSuffixForCheck);
                List<OSSpecLineExpect> expect = dictionary.ExpectSplitFile;
                ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    taskLogger.LogFileOperationStart(FileOperationType.SplitFile, srcPath, false);
                    return taskLogger.LogFileOperationEnd(FileOperationStatus.Fail);
                }
                destTempPathSuffix = destTempPathSuffixForCheck;
            }

            // テンポラリを固定
            string destFileTemplate = destPath + GenericFileStringUtils.GetFileName(srcPath);
            ModifyFileInfoContext modifyCtx = new ModifyFileInfoContext();
            for (int i = 0; i < totalFileCount; i++) {
                string oldFileName = string.Format("{0}{1:00}", destTempPathSuffix, i);
                string newFileName = RenameNumberingInfo.CreateSequenceFileName(destFileTemplate, numberingInfo, modifyCtx);
                    
                string strCommand = dictionary.GetCommandRename(oldFileName, newFileName);
                List<OSSpecLineExpect> expect = dictionary.ExpectRename;
                ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                taskLogger.LogFileOperationStart(FileOperationType.SplitFile, newFileName, false);
                status = emulator.Execute(m_controler.Context, engine);
                taskLogger.LogFileOperationEnd(status);
                if (!status.Succeeded) {
                    return status;
                }
            }
            destTempPathSuffix = null;

            return FileOperationStatus.Success;
        }
    }
}
