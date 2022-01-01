using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：SFTP内部処理のコントローラ（2つのセッションを使うもの）
    //=========================================================================================
    class SFTPProcedureDualControler {
        // 転送元の接続
        private SSHConnection m_srcConnection;

        // 転送先の接続
        private SSHConnection m_destConnection;

        // コンテキスト情報
        private FileOperationRequestContext m_context;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcConnection  転送元の接続
        // 　　　　[in]destConnection 転送先の接続
        // 　　　　[in]context        コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPProcedureDualControler(SSHConnection srcConnection, SSHConnection destConnection, FileOperationRequestContext context) {
            m_srcConnection = srcConnection;
            m_destConnection = destConnection;
            m_context = context;
        }

        //=========================================================================================
        // 機　能：プロシージャの実行に必要な準備を行う
        // 引　数：なし
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Initialize() {
            FileOperationStatus status;
            status = m_srcConnection.ConnectServer();
            if (!status.Succeeded) {
                return status;
            }
            status = m_destConnection.ConnectServer();
            if (!status.Succeeded) {
                return status;
            }
            return status;
        }

        //=========================================================================================
        // プロパティ：キャンセルされたときtrue
        //=========================================================================================
        public bool IsCanceled {
            get {
                if (m_srcConnection.Closed) {
                    return true;
                }
                if (m_destConnection.Closed) {
                    return true;
                }
                return false;
            }
        }

        //=========================================================================================
        // プロパティ：転送元の接続
        //=========================================================================================
        public SSHConnection SrcConnection {
            get {
                return m_srcConnection;
            }
        }
        
        //=========================================================================================
        // プロパティ：転送先の接続
        //=========================================================================================
        public SSHConnection DestConnection {
            get {
                return m_destConnection;
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
