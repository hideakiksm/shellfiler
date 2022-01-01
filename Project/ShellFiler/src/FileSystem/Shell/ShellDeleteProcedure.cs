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
    class ShellDeleteProcedure : ShellProcedure {
        // SSH内部処理のコントローラ
        private ShellProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellDeleteProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new ShellProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：ディレクトリを作成する
        // 引　数：[in]filePath    削除するファイルのパス
        // 　　　　[in]isTarget    対象パスを削除するときtrue、反対パスのときfalse
        // 　　　　[in]flag        削除フラグ
        // 戻り値：ステータスコード（成功のときSuccess、存在しないときはSuccessでfileInfo=null）
        //=========================================================================================
        public FileOperationStatus Execute(string filePath, bool isTarget, DeleteFileFolderFlag flag) {
            FileOperationStatus status = m_controler.Initialize(filePath, isTarget, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            // フラグを読み込み
            bool isDirectory;
            if ((flag & DeleteFileFolderFlag.FILE) != 0) {
                isDirectory = false;
            } else if ((flag & DeleteFileFolderFlag.FOLDER) != 0) {
                isDirectory = true;
            } else {
                Program.Abort("削除モードが正しくありません。");
                return FileOperationStatus.ErrorDelete;
            }
            bool withAttr = ((flag & DeleteFileFolderFlag.CHANGE_ATTR) != 0);
            bool delDirect = ((flag & DeleteFileFolderFlag.RECYCLE_OR_RF) != 0);

            // パスを分解
            filePath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, filePath);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            if (path == "/" || path.StartsWith("/ ")) {
                return FileOperationStatus.NotSupport;
            }

            ShellCommandDictionary dictionary = m_controler.Connection.ShellCommandDictionary;
            ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;

            // 削除を実行
            if (!withAttr && isDirectory && delDirect) {
                string strCommand = dictionary.GetCommandDeleteDirectoryRecursive(path);
                List<OSSpecLineExpect> expect = dictionary.ExpectDeleteDirectoryRecursive;
                string prompt = dictionary.ValueDeleteDirectoryRecursivePrompt;
                string answer = dictionary.ValueDeleteDirectoryRecursiveAnswer;
                status = ExecuteDeleteShellEngine(strCommand, expect, prompt, answer);
                if (!status.Succeeded) {
                    return status;
                }
            } else {
                // 属性を変更
                if (withAttr) {
                    string strCommand = dictionary.GetCommandSetPermissions(path, 0x1ff);
                    List<OSSpecLineExpect> expect = dictionary.ExpectSetPermissions;
                    ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, m_controler.Connection, strCommand, expect);
                    status = emulator.Execute(m_controler.Context, engine);
                    if (!status.Succeeded) {
                        return status;
                    }
                }

                // 削除
                if (delDirect) {
                    string strCommand = dictionary.GetCommandDeleteDirectoryRecursive(path);
                    List<OSSpecLineExpect> expect = dictionary.ExpectDeleteDirectoryRecursive;
                    string prompt = dictionary.ValueDeleteDirectoryRecursivePrompt;
                    string answer = dictionary.ValueDeleteDirectoryRecursiveAnswer;
                    status = ExecuteDeleteShellEngine(strCommand, expect, prompt, answer);
                    if (!status.Succeeded) {
                        return status;
                    }
                } else if (isDirectory) {
                    string strCommand = dictionary.GetCommandDeleteDirectory(path);
                    List<OSSpecLineExpect> expect = dictionary.ExpectDeleteDirectory;
                    string prompt = dictionary.ValueDeleteDirectoryPrompt;
                    string answer = dictionary.ValueDeleteDirectoryAnswer;
                    status = ExecuteDeleteShellEngine(strCommand, expect, prompt, answer);
                    if (!status.Succeeded) {
                        return status;
                    }
                } else {
                    string strCommand = dictionary.GetCommandDeleteFile(path);
                    List<OSSpecLineExpect> expect = dictionary.ExpectDeleteFile;
                    string prompt = dictionary.ValueDeleteFilePrompt;
                    string answer = dictionary.ValueDeleteFileAnswer;
                    status = ExecuteDeleteShellEngine(strCommand, expect, prompt, answer);
                    if (!status.Succeeded) {
                        return status;
                    }
                }
            }
            if (isDirectory) {
                return FileOperationStatus.SuccessDelDir;
            } else {
                return FileOperationStatus.SuccessDelete;
            }
        }

        //=========================================================================================
        // 機　能：ファイルまたはディレクトリをシェルエンジンで削除する
        // 引　数：[in]strCommand  実行するコマンドライン
        // 　　　　[in]expect      コマンドラインの期待値
        // 　　　　[in]prompt      削除確認のプロンプト
        // 　　　　[in]answer      削除確認への応答
        // 戻り値：ステータスコード（成功のときSuccess、存在しないときはSuccessでfileInfo=null）
        //=========================================================================================
        private FileOperationStatus ExecuteDeleteShellEngine(string strCommand, List<OSSpecLineExpect> expect, string prompt, string answer) {
            FileOperationStatus status;
            ShellCommandDictionary dictionary = m_controler.Connection.ShellCommandDictionary;
            ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;
            ShellEngineDelete engine = new ShellEngineDelete(emulator, m_controler.Connection, strCommand, expect, prompt, answer);
            status = emulator.Execute(m_controler.Context, engine);
            if (!status.Succeeded) {
                return status;
            }
            return FileOperationStatus.Success;
        }
    }
}
