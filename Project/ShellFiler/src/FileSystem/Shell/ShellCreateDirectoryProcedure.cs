using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
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
    // クラス：ディレクトリを作成するプロシージャ
    //=========================================================================================
    class ShellCreateDirectoryProcedure : ShellProcedure {
        // SSH内部処理のコントローラ
        private ShellProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellCreateDirectoryProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new ShellProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：ディレクトリを作成する
        // 引　数：[in]basePath   ディレクトリを作成する場所のパス
        // 　　　　[in]newName    作成するディレクトリ名
        // 　　　　[in]isTarget   対象パスの一覧のときtrue、反対パスのときfalse
        // 戻り値：ステータスコード（成功のときSuccess、存在しないときはSuccessでfileInfo=null）
        //=========================================================================================
        public FileOperationStatus Execute(string basePath, string newName, bool isTarget) {
            FileOperationStatus status = m_controler.Initialize(basePath, isTarget, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            basePath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, basePath);
            string srcFullPath = SSHUtils.CombineFilePath(basePath, newName);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(srcFullPath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            ShellCommandDictionary dictionary = m_controler.Connection.ShellCommandDictionary;
            ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;

            // mkdirコマンドを実行
            string strCommand = dictionary.GetCommandMakeDirectory(path);
            List<OSSpecLineExpect> expect = dictionary.ExpectMakeDirectory;
            ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
            status = emulator.Execute(m_controler.Context, engine);
            if (!status.Succeeded) {
                return status;
            }

            return FileOperationStatus.SuccessMkDir;
        }
    }
}
