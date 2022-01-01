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
    // クラス：リモート同士でファイルをコピー／移動するプロシージャ
    //=========================================================================================
    class ShellRemoteFileTransferProcedure : ShellProcedure {
        // SSH内部処理のコントローラ
        private ShellProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellRemoteFileTransferProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new ShellProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]transferMode    ファイル転送のモード（リンク系も有効）
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]overwrite       上書きするときtrue
        // 　　　　[in]attrCopy        属性コピーを行うときtrue
        // 　　　　[in]progress        進捗状態を通知するdelegate
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(TransferModeType transferMode, string srcFilePath, string destFilePath, bool overwrite, bool attrCopy, FileProgressEventHandler progress) {
            FileOperationStatus status = m_controler.Initialize(srcFilePath, true, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            bool success;
            srcFilePath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, srcFilePath);
            SSHProtocolType srcProtocol;
            string srcUser, srcServer, srcPath;
            int srcPortNo;
            success = SSHUtils.SeparateUserServer(srcFilePath, out srcProtocol, out srcUser, out srcServer, out srcPortNo, out srcPath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
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
            if (!overwrite) {
                // 上書きしない場合
                ShellEngineGetFileInfo engine = new ShellEngineGetFileInfo(emulator, m_controler.Connection, destPath);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    return status;
                }
                if (engine.ResultFileInfo != null) {
                    return FileOperationStatus.AlreadyExists;
                }
            }

            // コピー/移動を実行
            {
                progress.SetProgress(this, new FileProgressEventArgs(1, 0));
                string strCommand;
                List<OSSpecLineExpect> expect;
                dictionary.GetCopyMoveLinkCommand(transferMode, srcPath, destPath, attrCopy, out strCommand, out expect);
                ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    return status;
                }
                progress.SetProgress(this, new FileProgressEventArgs(1, 1));
            }
            return status;
        }
    }
}
