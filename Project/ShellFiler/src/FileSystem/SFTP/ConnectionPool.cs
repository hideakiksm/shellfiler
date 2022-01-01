using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;

namespace ShellFiler.Document.SSH {

    //=========================================================================================
    // クラス：アカウント/サーバ単位でのコネクションプール
    //=========================================================================================
    public class ConnectionPool {
        // 接続先のユーザ名
        private string m_user;

        // 接続先のサーバ名
        private string m_server;

        // 接続先のポート番号
        private int m_portNo;

        // コネクション（null:なし）
        private SSHConnection m_connection = null;

        // プールしているコネクションを使用中のタスク
        private HashSet<BackgroundTaskID> m_connectionInUse = new HashSet<BackgroundTaskID>();

        // 最後にアクセスした日時
        private DateTime m_lastAccessedTime;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]user   接続先のユーザー名
        // 　　　　[in]server 接続先のサーバー名
        // 　　　　[in]portNo 接続先のポート番号
        // 戻り値：なし
        //=========================================================================================
        public ConnectionPool(string user, string server, int portNo) {
            m_user = user;
            m_server = server;
            m_portNo = portNo;
            m_lastAccessedTime = DateTime.Now;
        }
        
        //=========================================================================================
        // 機　能：直接コネクションを取得する
        // 引　数：[in]taskId  取得した接続を共有するタスクID
        // 戻り値：取得したコネクション（プールから取得できなかったときはnull）
        //=========================================================================================
        public SSHConnection GetConnection(BackgroundTaskID taskId) {
            m_lastAccessedTime = DateTime.Now;
            if (m_connection != null && m_connection.IsError) {
                m_connection = null;
            }
            if (m_connection != null) {
                if (taskId != BackgroundTaskID.NullId) {
                    m_connectionInUse.Add(taskId);
                }
                return m_connection;
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：新しいシーケンシャルコネクションを登録する
        // 引　数：[in]connection  登録する新しい接続
        // 　　　　[in]taskId      この接続を共有するタスクID
        // 戻り値：なし
        //=========================================================================================
        public void SetConnection(SSHConnection connection, BackgroundTaskID taskId) {
            m_connection = connection;
            if (taskId != BackgroundTaskID.NullId) {
                m_connectionInUse.Add(taskId);
            }
        }

        //=========================================================================================
        // 機　能：シーケンシャルコネクションの使用中タスクIDに指定タスクが含まれているかどうか確認する
        // 引　数：[in]taskId    接続を共有するタスクID
        // 戻り値：指定タスクが含まれていればtrue
        //=========================================================================================
        public bool ContainsSequentialInUse(BackgroundTaskID taskId) {
            return m_connectionInUse.Contains(taskId);
        }

        //=========================================================================================
        // 機　能：コネクションを解放してプールに戻す
        // 引　数：[in]taskId   解放するタスクのID
        // 戻り値：なし
        //=========================================================================================
        public void FreeConnection(BackgroundTaskID taskId) {
            // シーケンシャルコネクションは使用中フラグを消す
            if (m_connectionInUse.Contains(taskId)) {
                m_connectionInUse.Remove(taskId);
            }
            m_lastAccessedTime = DateTime.Now;
        }

        //=========================================================================================
        // 機　能：指定された接続をクローズする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CloseServer() {
            if (m_connection != null) {
                m_connection.DisposeConnection();
                m_connection = null;
            }
        }

        //=========================================================================================
        // 機　能：最後にアクセスした日時を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ModifyLastAccessedTime() {
            m_lastAccessedTime = DateTime.Now;
        }

        //=========================================================================================
        // プロパティ：プールしているコネクション（null:なし）
        //=========================================================================================
        public SSHConnection Connection {
            get {
                return m_connection;
            }
        }

        //=========================================================================================
        // プロパティ：接続先のユーザー名
        //=========================================================================================
        public string User {
            get {
                return m_user;
            }
        }

        //=========================================================================================
        // プロパティ：接続先のサーバー名
        //=========================================================================================
        public string Server {
            get {
                return m_server;
            }
        }

        //=========================================================================================
        // プロパティ：接続先のポート番号
        //=========================================================================================
        public int PortNo {
            get {
                return m_portNo;
            }
        }

        //=========================================================================================
        // プロパティ：使用中の接続数
        //=========================================================================================
        public int AllInUseCount {
            get {
                return m_connectionInUse.Count;
            }
        }

        //=========================================================================================
        // プロパティ：このサーバとの接続に関連するバックグラウンドタスクIDのリスト
        //=========================================================================================
        public List<BackgroundTaskID> InUseBackgroundTaskIdList {
            get {
                List<BackgroundTaskID> taskIdList = new List<BackgroundTaskID>();
                taskIdList.AddRange(m_connectionInUse);
                return taskIdList;
            }
        }
    }
}
