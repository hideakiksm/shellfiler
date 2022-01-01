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
using ShellFiler.FileTask;
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
    // クラス：SSHシェルでファイルやディレクトリの属性を一括変更のルールにより変更するプロシージャ
    //=========================================================================================
    class ShellModifyFileInfoProcedure : ShellProcedure {
        // SSH内部処理のコントローラ
        private ShellProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellModifyFileInfoProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new ShellProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]oldFilePath   変更対象のファイル名
        // 　　　　[in]modInfo       変更ルール
        // 　　　　[in]modifyCtx     名前変更のコンテキスト情報
        // 戻り値：ステータスコード（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(string oldFilePath, RenameSelectedFileInfo modInfo, ModifyFileInfoContext modifyCtx) {
            RenameSelectedFileInfo.SSHRenameInfo renameInfo = modInfo.SSHInfo;
            FileOperationStatus status = m_controler.Initialize(oldFilePath, true, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            oldFilePath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, oldFilePath);
            SSHProtocolType protocol;
            string user, server, filePath;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(oldFilePath, out protocol, out user, out server, out portNo, out filePath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }
            string basePath = GenericFileStringUtils.GetDirectoryName(filePath, '/');
            if (!basePath.EndsWith("/")) {
                basePath += "/";
            }
            string fileName = GenericFileStringUtils.GetFileName(filePath);

            ShellCommandDictionary dictionary = m_controler.Connection.ShellCommandDictionary;
            ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;
            ShellFile orgFileInfo = null;

            // ファイル名を変更
            {
                // ファイル名
                string newFileName = RenameSelectedFileInfoBackgroundTask.GetNewFileName(fileName, renameInfo.ModifyFileNameInfo, modifyCtx);
                if (fileName != newFileName) {
                    string newFilePath = SSHUtils.CombineFilePath(basePath, newFileName);
                    string strCommand = dictionary.GetCommandRename(filePath, newFilePath);
                    List<OSSpecLineExpect> expect = dictionary.ExpectRename;
                    ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                    status = emulator.Execute(m_controler.Context, engine);
                    if (!status.Succeeded) {
                        return status;
                    }
                    filePath = newFilePath;
                }
            }

            // パーミッションを変更
            if (renameInfo.AttributeOtherRead != null || renameInfo.AttributeOtherWrite != null || renameInfo.AttributeOwnerExecute != null ||
                    renameInfo.AttributeGroupRead != null || renameInfo.AttributeGroupWrite != null || renameInfo.AttributeGroupExecute != null ||
                    renameInfo.AttributeOtherRead != null || renameInfo.AttributeOtherWrite != null || renameInfo.AttributeOtherExecute != null) {
                status = GetFileAttribute(filePath, ref orgFileInfo);
                if (!status.Succeeded) {
                    return status;
                }
                int oldPermissions = orgFileInfo.Permissions;
                int modPermissions = RenameSelectedFileInfo.SSHRenameInfo.ModifyPermissions(oldPermissions, renameInfo);
                if (oldPermissions != modPermissions) {
                    string strCommand = dictionary.GetCommandSetPermissions(filePath, modPermissions);
                    List<OSSpecLineExpect> expect = dictionary.ExpectSetPermissions;
                    ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                    status = emulator.Execute(m_controler.Context, engine);
                    if (!status.Succeeded) {
                        return status;
                    }
                }
            }

            // 更新時刻を変更
            if (renameInfo.UpdateDate != null || renameInfo.UpdateTime != null) {
                DateTime modifiedDate;
                if (renameInfo.UpdateDate != null && renameInfo.UpdateTime != null) {
                    modifiedDate = new DateTime(renameInfo.UpdateDate.Year, renameInfo.UpdateDate.Month, renameInfo.UpdateDate.Day,
                                                renameInfo.UpdateTime.Hour, renameInfo.UpdateTime.Minute, renameInfo.UpdateTime.Second);
                } else {
                    status = GetFileAttribute(filePath, ref orgFileInfo);
                    if (!status.Succeeded) {
                        return status;
                    }
                    DateTimeInfo dateTimeInfo = new DateTimeInfo(orgFileInfo.ModifiedDate);
                    dateTimeInfo.SetDate(renameInfo.UpdateDate);
                    dateTimeInfo.SetTime(renameInfo.UpdateTime);
                    modifiedDate = dateTimeInfo.ToDateTime();
                }
                string strCommand = dictionary.GetCommandSetModifiedTime(filePath, modifiedDate);
                List<OSSpecLineExpect> expect = dictionary.ExpectSetFileTime;
                ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    return status;
                }
            }
            
            // アクセス時刻を変更
            if (renameInfo.AccessDate != null || renameInfo.AccessTime != null) {
                DateTime accessDate;
                if (renameInfo.AccessDate != null && renameInfo.AccessTime != null) {
                    accessDate = new DateTime(renameInfo.AccessDate.Year, renameInfo.AccessDate.Month, renameInfo.AccessDate.Day,
                                                renameInfo.AccessTime.Hour, renameInfo.AccessTime.Minute, renameInfo.AccessTime.Second);
                } else {
                    status = GetFileAttribute(filePath, ref orgFileInfo);
                    if (!status.Succeeded) {
                        return status;
                    }
                    DateTimeInfo dateTimeInfo = new DateTimeInfo(orgFileInfo.AccessDate);
                    dateTimeInfo.SetDate(renameInfo.AccessDate);
                    dateTimeInfo.SetTime(renameInfo.AccessTime);
                    accessDate = dateTimeInfo.ToDateTime();
                }
                string strCommand = dictionary.GetCommandSetAccessedTime(filePath, accessDate);
                List<OSSpecLineExpect> expect = dictionary.ExpectSetFileTime;
                ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    return status;
                }
            }

            // オーナーを変更
            if (renameInfo.Owner != null || renameInfo.Group != null) {
                string strCommand = null;
                List<OSSpecLineExpect> expect = null;
                if (renameInfo.Owner != null && renameInfo.Group != null) {
                    strCommand = dictionary.GetChangeOwnerCommand(filePath, renameInfo.Owner, renameInfo.Group);
                    expect = dictionary.ExpectChangeOwner;
                } else if (renameInfo.Owner != null) {
                    strCommand = dictionary.GetChangeOwnerCommand(filePath, renameInfo.Owner, null);
                    expect = dictionary.ExpectChangeOwner;
                } else if (renameInfo.Group != null) {
                    strCommand = dictionary.GetChangeOwnerCommand(filePath, null, renameInfo.Group);
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

        //=========================================================================================
        // 機　能：ファイル属性を取得する
        // 引　数：[in]filePath       属性の変更対象のパス
        // 　　　　[in,out]fileInfo   ファイルの属性（未取得の場合はnullで取得状態にして返る）
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus GetFileAttribute(string filePath, ref ShellFile fileInfo) {
            FileOperationStatus status;
            if (fileInfo != null) {
                return FileOperationStatus.Success;
            }

            // statコマンドを実行
            ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;
            ShellEngineGetFileInfo engine = new ShellEngineGetFileInfo(emulator, m_controler.Connection, filePath);
            status = emulator.Execute(m_controler.Context, engine);
            if (!status.Succeeded) {
                return status;
            }
            fileInfo = engine.ResultFileInfo;

            return FileOperationStatus.Success;
        }
    }
}
