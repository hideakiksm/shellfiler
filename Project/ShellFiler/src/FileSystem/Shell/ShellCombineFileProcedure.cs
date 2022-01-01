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
    // クラス：ファイルを結合するプロシージャ
    //=========================================================================================
    class ShellCombineFileProcedure : ShellProcedure {
        // SSH内部処理のコントローラ
        private ShellProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellCombineFileProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new ShellProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]srcPathList     転送元ファイル名のフルパス
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]taskLogger      ログ出力クラス
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(List<string> srcPathList, string destFilePath, ITaskLogger taskLogger) {
            bool success;
            FileOperationStatus status = m_controler.Initialize(srcPathList[0], true, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            destFilePath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, destFilePath);
            SSHProtocolType destProtocol;
            string destUser, destServer, destPath;
            int destPortNo;
            success = SSHUtils.SeparateUserServer(destFilePath, out destProtocol, out destUser, out destServer, out destPortNo, out destPath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            ShellCommandDictionary dictionary = m_controler.Connection.ShellCommandDictionary;
            ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;

            // 上書き確認
            ShellEngineGetFileInfo engine = new ShellEngineGetFileInfo(emulator, m_controler.Connection, destPath);
            status = emulator.Execute(m_controler.Context, engine);
            if (!status.Succeeded) {
                return status;
            }
            if (engine.ResultFileInfo != null) {
                // すでに存在している場合は終わり
                taskLogger.LogFileOperationStart(FileOperationType.CombineFile, destFilePath, false);
                return taskLogger.LogFileOperationEnd(FileOperationStatus.AlreadyExists);
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
                    ShellUtils.DeleteFile(m_controler, destPath);
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
        private FileOperationStatus AppendFile(string srcFilePath, string destLocalPath, bool firstFile) {
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
            {
                ShellCommandDictionary dictionary = m_controler.Connection.ShellCommandDictionary;
                string strCommand = dictionary.GetAppendFileCommand(srcPath, destLocalPath, firstFile);
                List<OSSpecLineExpect> expect = dictionary.ExpectAppendFile;
                ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;
                ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    return status;
                }
                return FileOperationStatus.Success;
            }
        }
    }
}
