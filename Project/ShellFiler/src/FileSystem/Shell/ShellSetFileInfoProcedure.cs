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
    // クラス：ファイル情報を取得するプロシージャ
    //=========================================================================================
    class ShellSetFileInfoProcedure : ShellProcedure {
        // SSH内部処理のコントローラ
        private ShellProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellSetFileInfoProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new ShellProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：ファイルを取得する
        // 引　数：[in]srcFileInfo  転送元のファイル情報
        // 　　　　[in]filePath     ファイルパス
        // 戻り値：ステータスコード（成功のときSuccess、存在しないときはSuccessでfileInfo=null）
        //=========================================================================================
        public FileOperationStatus Execute(ShellFile srcFileInfo, string filePath) {
            FileOperationStatus status = m_controler.Initialize(filePath, false, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            filePath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, filePath);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            ShellCommandDictionary dictionary = m_controler.Connection.ShellCommandDictionary;
            ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;

            // chmodコマンドを実行
            if (srcFileInfo.Permissions != -1) {
                string strCommand = dictionary.GetCommandSetPermissions(path, srcFileInfo.Permissions);
                List<OSSpecLineExpect> expect = dictionary.ExpectSetPermissions;
                ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    return status;
                }
            }

            // touchコマンドを実行
            {
                string strCommand = dictionary.GetCommandSetFileTime(path, srcFileInfo.ModifiedDate);
                List<OSSpecLineExpect> expect = dictionary.ExpectSetFileTime;
                ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    return status;
                }
            }

            return FileOperationStatus.Success;
        }
    }
}
