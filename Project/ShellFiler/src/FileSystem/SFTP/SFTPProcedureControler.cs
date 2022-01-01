using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：SFTP内部処理のコントローラ
    //=========================================================================================
    class SFTPProcedureControler {
        // 接続
        private SSHConnection m_connection;

        // コンテキスト情報
        private FileOperationRequestContext m_context;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPProcedureControler(SSHConnection connection, FileOperationRequestContext context) {
            m_connection = connection;
            m_context = context;
        }

        //=========================================================================================
        // 機　能：プロシージャの実行に必要な準備を行う
        // 引　数：なし
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Initialize() {
            FileOperationStatus status = m_connection.ConnectServer();
            return status;
        }

        //=========================================================================================
        // プロパティ：キャンセルされたときtrue
        //=========================================================================================
        public bool IsCanceled {
            get {
                return m_connection.Closed;
            }
        }

        //=========================================================================================
        // プロパティ：接続
        //=========================================================================================
        public SSHConnection Connection {
            get {
                return m_connection;
            }
        }
        
        //=========================================================================================
        // プロパティ：コンテキスト情報
        //=========================================================================================
        public FileOperationRequestContext Context {
            get {
                return m_context;
            }
        }
    }
}
