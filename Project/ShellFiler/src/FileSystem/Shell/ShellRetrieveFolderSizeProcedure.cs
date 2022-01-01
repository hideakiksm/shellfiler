using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileViewer;
using ShellFiler.Util;
using ShellFiler.Properties;
using ShellFiler.GraphicsViewer;
using ShellFiler.Virtual;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.TerminalSession.CommandEmulator;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;

namespace ShellFiler.FileSystem.Shell {

    //=========================================================================================
    // クラス：フォルダサイズを取得するプロシージャ
    //=========================================================================================
    class ShellRetrieveFolderSizeProcedure : ShellProcedure {
        // SSH内部処理のコントローラ
        private ShellProcedureControler m_controler;

        // 取得条件
        private RetrieveFolderSizeCondition m_condition;

        // 取得結果を返す変数
        private RetrieveFolderSizeResult m_result;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 　　　　[in]condition  取得条件
        // 　　　　[in]result     取得結果を返す変数
        // 戻り値：なし
        //=========================================================================================
        public ShellRetrieveFolderSizeProcedure(SSHConnection connection, FileOperationRequestContext context, RetrieveFolderSizeCondition condition, RetrieveFolderSizeResult result) {
            m_controler = new ShellProcedureControler(connection, context);
            m_condition = condition;
            m_result = result;
        }

        //=========================================================================================
        // 機　能：フォルダサイズを取得する
        // 引　数：[in]condition   取得条件
        // 　　　　[in]result      取得結果を返す変数
        // 戻り値：ステータスコード（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(string directory) {
            FileOperationStatus status = m_controler.Initialize(directory, true, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            directory = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, directory);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(directory, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // ファイル一覧を取得
            ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;
            ShellEngineRetrieveFolderSize engine = new ShellEngineRetrieveFolderSize(emulator, m_controler.Connection, path, m_condition, m_result);
            status = emulator.Execute(m_controler.Context, engine);
            if (!status.Succeeded) {
                return status;
            }

            return FileOperationStatus.Success;
        }
    }
}
