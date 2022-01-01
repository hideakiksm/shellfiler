using System;
using System.Collections.Generic;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：SFTPリクエストのコンテキスト情報
    //=========================================================================================
    public class SFTPRequestContext {
        // 「ユーザー名@サーバー」から使用中SFTPチャネルへのmap
        private Dictionary<string, ChannelSftp> m_mapAuthToSftpChannel = new Dictionary<string, ChannelSftp>();

        // 使用中EXECチャネルのリスト
        private List<ChannelExec> m_listAuthToExecChannel = new List<ChannelExec>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SFTPRequestContext() {
        }

        //=========================================================================================
        // 機　能：コンテキスト情報を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            lock (this) {
                foreach (ChannelExec channel in m_listAuthToExecChannel) {
                    channel.disconnect();
                }
                m_listAuthToExecChannel.Clear();

                foreach (ChannelSftp channel in m_mapAuthToSftpChannel.Values) {
                    channel.disconnect();
                }
                m_mapAuthToSftpChannel.Clear();
            }
        }

        //=========================================================================================
        // 機　能：SFTPチャネルを返す
        // 引　数：[in]connection 接続
        // 　　　　[in]cache      キャッシュ情報
        // 戻り値：SFTPチャネル
        //=========================================================================================
        public ChannelSftp GetChannelSftp(SSHConnection connection) {
            lock (this) {
                string auth = SSHUtils.CreateUserServerShort(connection.AuthenticateSetting.UserName, connection.AuthenticateSetting.ServerName, connection.AuthenticateSetting.PortNo);
                if (m_mapAuthToSftpChannel.ContainsKey(auth)) {
                    // 作成済みのchannelを返す
                    return m_mapAuthToSftpChannel[auth];
                } else {
                    // なければ作成
                    ChannelSftp channel = (ChannelSftp)(connection.SSHSession.openChannel("sftp"));
                    channel.connect();
                    m_mapAuthToSftpChannel.Add(auth, channel);
                    return channel;
                }
            }
        }

        //=========================================================================================
        // 機　能：EXECチャネルを返す
        // 引　数：[in]connection 接続
        // 　　　　[in]cache      キャッシュ情報
        // 戻り値：EXECチャネル
        //=========================================================================================
        public ChannelExec GetChannelExec(SSHConnection connection) {
            lock (this) {
                ChannelExec channel = (ChannelExec)(connection.SSHSession.openChannel("exec"));
                m_listAuthToExecChannel.Add(channel);
                return channel;
            }
        }

        //=========================================================================================
        // 機　能：EXECチャネルをクローズする
        // 引　数：[in]channel  クローズするチャネル
        // 戻り値：なし
        //=========================================================================================
        public void CloseExecChannel(ChannelExec channel) {
            m_listAuthToExecChannel.Remove(channel);
            channel.disconnect();
        }
    }
}
