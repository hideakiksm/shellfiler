using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.Virtual;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.TerminalSession.CommandEmulator;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;

namespace ShellFiler.FileSystem.Shell {

    //=========================================================================================
    // クラス：ファイル情報を取得するプロシージャ
    //=========================================================================================
    class ShellRetrieveFileChunkProcedure : ShellProcedure {
        // 内部処理のコントローラ
        private ShellProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellRetrieveFileChunkProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new ShellProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：ファイルを取得する
        // 引　数：[in]file    アクセスしたいファイルの情報
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(AccessibleFile file) {
            FileOperationStatus status = m_controler.Initialize(file.FilePath, true, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            string filePath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, file.FilePath);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // headコマンドを実行
            ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;
            ShellEngineRetrieveChunk engine = new ShellEngineRetrieveChunk(emulator, m_controler.Connection, file, path);
            status = emulator.Execute(m_controler.Context, engine);
            if (!status.Succeeded) {
                return status;
            }
            return FileOperationStatus.Success;
        }
    }
}
