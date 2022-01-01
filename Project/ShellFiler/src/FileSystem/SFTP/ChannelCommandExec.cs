using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.Properties;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：ファイル１つ分の情報
    //=========================================================================================
    class ChannelCommandExec {
        // 接続
        private SSHConnection m_connection;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]command    実行するコマンド
        // 戻り値：なし
        //=========================================================================================
        public ChannelCommandExec(SSHConnection connection) {
            m_connection = connection;
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]command   実行するコマンド
        // 　　　　[out]output   出力結果を返すバッファへの参照
        // 戻り値：なし
        //=========================================================================================
        public FileOperationStatus Execute(SFTPRequestContext cache, string command, out byte[] output) {
            output = null;
            MemoryStream errs = new MemoryStream();
            MemoryStream outs = new MemoryStream();
            try {
    			ChannelExec channel = cache.GetChannelExec(m_connection);
                try {
                    channel.setCommand(command);
                    channel.setOutputStream(outs);
                    channel.setErrStream(errs);
			        channel.connect();
                    while (!channel.isEOF()) {
                        if (BaseThread.CurrentThread.IsCancel) {
                            return FileOperationStatus.Canceled;
                        }
                        Thread.Sleep(10);
                    }
                } finally {
                    cache.CloseExecChannel(channel);
                }
            } catch (Exception e) {
                return m_connection.OnException(e, Resources.Log_SSHExecFail);
            }
            errs.Flush();
            errs.Close();
            outs.Flush();
            outs.Close();
            output = outs.ToArray();
            byte[] errorMessage = errs.ToArray();
            if (errorMessage.Length == 0) {
                return FileOperationStatus.Success;
            } else {
                return FileOperationStatus.Fail;
            }
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]command   実行するコマンド
        // 　　　　[in]stdOut    標準出力で使用するストリーム
        // 　　　　[in]stdErr    標準エラー出力で使用するストリーム
        // 　　　　[in]fireEvent ストリームのイベントを発火するインターフェース（不要のときnull）
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus ExecuteByStream(SFTPRequestContext cache, string command, Stream stdOut, Stream stdErr, IRetrieveFileDataTarget fireEvent) {
            try {
                ChannelExec channel = cache.GetChannelExec(m_connection);
                try {
                    channel.setCommand(command);
                    channel.setOutputStream(stdOut);
                    channel.setErrStream(stdErr);
                    channel.connect();
                    while (!channel.isEOF()) {
                        if (BaseThread.CurrentThread.IsCancel) {
                            return FileOperationStatus.Canceled;
                        }
                        if (fireEvent != null) {
                            fireEvent.FireEvent(false);
                        }
                        Thread.Sleep(10);
                    }
                    if (fireEvent != null) {
                        fireEvent.FireEvent(false);
                    }
                } finally {
                    cache.CloseExecChannel(channel);
                }
                stdOut.Close();
                stdErr.Close();
            } catch (Exception e) {
                return m_connection.OnException(e, Resources.Log_SSHExecFail);
            }
            return FileOperationStatus.Success;
        }
    }
}
