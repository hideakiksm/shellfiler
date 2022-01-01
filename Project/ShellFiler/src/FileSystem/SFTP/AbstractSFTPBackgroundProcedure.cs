using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：バックグラウンド実行するSFTP用プロシージャの基底クラス
    //=========================================================================================
    abstract class AbstractSFTPBackgroundProcedure : AbstractBackgroundProcedure {
        // SSHの接続
        private SSHConnection m_connection;

        //=========================================================================================
        // プロパティ：SSHの接続
        //=========================================================================================
        public SSHConnection SSHConnection {
            get {
                return m_connection;
            }
            set {
                m_connection = value;
            }
        }
    }
}
