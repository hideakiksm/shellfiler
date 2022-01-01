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
    // クラス：SSHシェルでファイルやディレクトリの属性を変更するプロシージャ
    //=========================================================================================
    class ShellRenameProcedure : ShellProcedure {
        // SSH内部処理のコントローラ
        private ShellProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellRenameProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new ShellProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]oldFilePath   変更対象のファイル名
        // 　　　　[in]originalInfo  変更前のファイル情報
        // 　　　　[in]newInfo       変更後のファイル情報
        // 戻り値：ステータスコード（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(string oldFilePath, RenameFileInfo originalInfo, RenameFileInfo newInfo) {
            FileOperationStatus status = m_controler.Initialize(oldFilePath, true, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            oldFilePath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, oldFilePath);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(oldFilePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }
            path = GenericFileStringUtils.GetDirectoryName(path, '/');
            if (!path.EndsWith("/")) {
                path += "/";
            }

            ShellCommandDictionary dictionary = m_controler.Connection.ShellCommandDictionary;
            ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;

            // ファイル名を変更
            string filePath;
            {
                string oldFileName = SSHUtils.CombineFilePath(path, originalInfo.SSHInfo.FileName);
                string newFileName = SSHUtils.CombineFilePath(path, newInfo.SSHInfo.FileName);
                filePath = oldFileName;
                if (oldFileName != newFileName) {
                    filePath = newFileName;
                    string strCommand = dictionary.GetCommandRename(oldFileName, newFileName);
                    List<OSSpecLineExpect> expect = dictionary.ExpectRename;
                    ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                    status = emulator.Execute(m_controler.Context, engine);
                    if (!status.Succeeded) {
                        return status;
                    }
                }
            }

            // パーミッションを変更
            if (originalInfo.SSHInfo.Permissions != newInfo.SSHInfo.Permissions) {
                string strCommand = dictionary.GetCommandSetPermissions(filePath, newInfo.SSHInfo.Permissions);
                List<OSSpecLineExpect> expect = dictionary.ExpectSetPermissions;
                ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    return status;
                }
            }

            // 更新時刻を変更
            if (originalInfo.SSHInfo.ModifiedDate != newInfo.SSHInfo.ModifiedDate) {
                string strCommand = dictionary.GetCommandSetModifiedTime(filePath, newInfo.SSHInfo.ModifiedDate);
                List<OSSpecLineExpect> expect = dictionary.ExpectSetFileTime;
                ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    return status;
                }
            }

            // アクセス時刻を変更
            if (originalInfo.SSHInfo.AccessDate != newInfo.SSHInfo.AccessDate) {
                string strCommand = dictionary.GetCommandSetAccessedTime(filePath, newInfo.SSHInfo.AccessDate);
                List<OSSpecLineExpect> expect = dictionary.ExpectSetFileTime;
                ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    return status;
                }
            }

            // オーナーを変更
            {
                string strCommand = null;
                List<OSSpecLineExpect> expect = null;
                string oldOwner = originalInfo.SSHInfo.Owner;
                string newOwner = newInfo.SSHInfo.Owner;
                string oldGroup = originalInfo.SSHInfo.Group;
                string newGroup = newInfo.SSHInfo.Group;
                if (oldOwner != newOwner && oldGroup == newGroup) {
                    strCommand = dictionary.GetChangeOwnerCommand(filePath, newOwner, null);
                    expect = dictionary.ExpectChangeOwner;
                } else if (oldOwner == newOwner && oldGroup != newGroup) {
                    strCommand = dictionary.GetChangeOwnerCommand(filePath, null, newGroup);
                    expect = dictionary.ExpectChangeOwner;
                } else if (oldOwner != newOwner && oldGroup != newGroup) {
                    strCommand = dictionary.GetChangeOwnerCommand(filePath, newOwner, newGroup);
                    expect = dictionary.ExpectChangeOwner;
                }
                if (strCommand != null) {
                    ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                    status = emulator.Execute(m_controler.Context, engine);
                    if (!status.Succeeded) {
                        return status;
                    }
                }
            }

            return FileOperationStatus.Success;
        }
    }
}
