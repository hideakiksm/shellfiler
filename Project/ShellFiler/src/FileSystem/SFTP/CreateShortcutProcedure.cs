using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.Properties;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：ショートカットを作成するプロシージャ
    //=========================================================================================
    class CreateShortcutProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public CreateShortcutProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]overwrite     上書きするときtrue
        // 　　　　[in]progress      進捗状態を通知するdelegate
        // 　　　　[in]type          ショートカットの種類
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(string srcFilePath, string destFilePath, bool overwrite, ShortcutType type) {
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            SSHProtocolType protocol;
            string user, server, srcLocalPath, destLocalPath;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(srcFilePath, out protocol, out user, out server, out portNo, out srcLocalPath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }
            success = SSHUtils.SeparateUserServer(destFilePath, out protocol, out user, out server, out portNo, out destLocalPath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            try {
                ChannelSftp channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);

                // 上書き確認
                if (!overwrite) {
                    // 上書きしない場合
                    bool alreadyExist = false;
                    try {
                        channel.stat(destLocalPath);
                        alreadyExist = true;
                    } catch (SftpException) {
                        // 存在しない場合はここを通る
                    }
                    if (m_controler.IsCanceled) {
                        return FileOperationStatus.Canceled;
                    }
                    if (alreadyExist) {
                        return FileOperationStatus.AlreadyExists;
                    }
                }

                // リンクを作成
                string command;
                if (type == ShortcutType.SymbolicLink) {
                    command = m_controler.Connection.ShellCommandDictionary.GetLnSymbolicCommand(srcLocalPath, destLocalPath);
                } else {
                    command = m_controler.Connection.ShellCommandDictionary.GetLnHardCommand(srcLocalPath, destLocalPath);
                }
                byte[] dummy;
                ChannelCommandExec exec = new ChannelCommandExec(m_controler.Connection);
                status = exec.Execute(m_controler.Context.SFTPRequestContext, command, out dummy);
                if (status != FileOperationStatus.Success) {
                    return status;
                }
            } catch (Exception e) {
                m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
                return FileOperationStatus.FailedConnect;
            }
            return FileOperationStatus.SuccessCopy;
        }
    }
}
