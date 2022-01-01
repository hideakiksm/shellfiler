using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Terminal.TerminalSession;

namespace ShellFiler.Document.SSH {

    //=========================================================================================
    // クラス：SSHサーバとのコネクション管理
    //=========================================================================================
    public class SSHConnectionManager {
        // シーケンシャルコネクションの一覧（「ユーザー名@サーバー名」から接続へのMap、必要になる都度エントリを作成）
        private Dictionary<string, ConnectionPool> m_connectionMap = new Dictionary<string, ConnectionPool>();

        // コネクションに変化が生じたときの通知用delegate
        public delegate void ConnectionChangedEventHandler(object sender, ConnectionChangedEventArgs evt); 

        // コネクションに変化が生じたときに通知するイベント
        public event ConnectionChangedEventHandler ConnectionChanged; 

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SSHConnectionManager() {
            ConnectionChanged += new ConnectionChangedEventHandler(this.ConnectionManager_StateChanged);
        }
        
        //=========================================================================================
        // 機　能：後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            CloseAll();
        }

        //=========================================================================================
        // 機　能：指定された条件に相当する接続を取得する
        // 引　数：[in]cache     キャッシュ情報
        // 　　　　[in]diretory  フルパスディレクトリ名
        // 　　　　[in]result    取得した接続（未接続状態の可能性あり）
        // 戻り値：接続に成功したときSuccess
        //=========================================================================================
        public FileOperationStatus GetSSHConnection(FileOperationRequestContext cache, string directory, out SSHConnection result) {
            result = null;

            // 接続先フルパス名から認証情報を取得
            SSHProtocolType protocol;
            string user, server, local;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(directory, out protocol, out user, out server, out portNo, out local);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }
            
            // 登録済みの認証情報を取得
            SSHUserAuthenticateSettingItem authSetting;
            bool isNewPool = false;         // プールに新しいエントリが増えたときtrue
            bool isCountChanged = false;    // 使用中カウントが増えたときtrue
            SSHConnection connection;       // 使用するコネクション
            ConnectionPool connectionPool;  // 使用するコネクションプール
            lock (this) {
                // コネクションプールの準備
                string userServer = SSHUtils.CreateUserServerShort(user, server, portNo);
                if (m_connectionMap.ContainsKey(userServer)) {
                    connectionPool = m_connectionMap[userServer];
                } else {
                    connectionPool = new ConnectionPool(user, server, portNo);
                    isNewPool = true;
                }

                // 接続
                isCountChanged = !connectionPool.ContainsSequentialInUse(cache.BackgroundTaskId);
                connection = connectionPool.GetConnection(cache.BackgroundTaskId);
                if (connection == null) {
                    SSHUserAuthenticateSetting setting = Program.Document.SSHUserAuthenticateSetting;
                    setting.LoadData();
                    bool isTempAuth;
                    authSetting = setting.GetUserAuthenticateSetting(server, user, portNo, out isTempAuth);
                    if (authSetting == null) {
                        throw new SfException(SfException.SSHNotFoundAuthSetting);
                    }
                    connection = new SSHConnection(this, authSetting, isTempAuth);
                    connectionPool.SetConnection(connection, cache.BackgroundTaskId);
                }

                // コネクションを記憶
                if (isNewPool) {
                    m_connectionMap.Add(userServer, connectionPool);
                }
                connection.StartRequest();
            }

            // イベントを通知
            if (isNewPool) {
                ConnectionChangedEventArgs args = new ConnectionChangedEventArgs(ConnectionChangedEventType.ConnectionPoolAdded, user, server, portNo, new UIConnectionPoolInfo(connectionPool));
                ConnectionChanged(this, args);
            } else if (isCountChanged) {
                ConnectionChangedEventArgs args = new ConnectionChangedEventArgs(ConnectionChangedEventType.CountChanged, user, server, portNo, new UIConnectionPoolInfo(connectionPool));
                ConnectionChanged(this, args);
            }
            result = connection;
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：指定された条件に相当する接続を取得する
        // 引　数：[in]directory  取得するユーザー名とパスワードを含むディレクトリ（nullのときすべて）
        // 戻り値：接続済みのターミナルのリスト
        //=========================================================================================
        public List<TerminalShellChannel> GetAuthorizedTerminalChannel(string directory) {
            lock (this) {
                if (directory != null) {
                    SSHProtocolType protocol;
                    string userServer;
                    SSHUtils.GetUserServerPart(directory, out protocol, out userServer);
                    if (m_connectionMap.ContainsKey(userServer)) {
                        ConnectionPool connectionPool = m_connectionMap[userServer];
                        return connectionPool.Connection.TerminalChannel;     // nullもあり
                    }
                    return new List<TerminalShellChannel>();
                } else {
                    List<TerminalShellChannel> result = new List<TerminalShellChannel>();
                    foreach (ConnectionPool connectionPool in m_connectionMap.Values) {
                        result.AddRange(connectionPool.Connection.TerminalChannel);
                    }
                    return result;
                }
            }
        }

        //=========================================================================================
        // 機　能：ユニークなコンソール名を作成して返す
        // 引　数：[in]baseName    基本となる名前
        // 　　　　[in]fileSystem  ファイルシステムで使用するコンソールのときtrue
        // 戻り値：ユニークなコンソール名
        //=========================================================================================
        public string CreateUniqueConsoleName(string baseName, bool fileSystem) {
            lock (this) {
                for (int i = 1; i <= 1000; i++) {
                    // 名前を作成
                    string name = "";
                    if (fileSystem) {
                        name = "SSH:";
                    }
                    if (i == 1) {
                        name += baseName;
                    } else {
                        name += baseName + "(" + i + ")";
                    }

                    // 既存のセッション中のコンソールにnameがあるか？
                    bool found = false;
                    foreach (ConnectionPool pool in m_connectionMap.Values) {
                        foreach (TerminalShellChannel channel in pool.Connection.TerminalChannel) {
                            if (channel.ConsoleScreen.DisplayName == name) {
                                found = true;
                                break;
                            }
                        }
                        if (found) {
                            break;
                        }
                    }
                    if (!found) {
                        return name;
                    }
                }
            }
            return baseName;
        }

        //=========================================================================================
        // 機　能：指定されたユーザー名とサーバーで最後に表示したディレクトリを取得する
        // 引　数：[in]user    ユーザー名を返す変数への参照
        // 　　　　[in]server  サーバー名を返す変数への参照
        // 　　　　[in]port    ポート番号
        // 戻り値：最後に表示したディレクトリ（取得できないときnull）
        //=========================================================================================
        public string GetPreviousPath(string user, string server, int port) {
            string prevDir = null;
            lock (this) {
                string userServer = SSHUtils.CreateUserServerShort(user, server, port);
                if (m_connectionMap.ContainsKey(userServer)) {
                    ConnectionPool connectionPool = m_connectionMap[userServer];
                    if (connectionPool.Connection != null) {
                        prevDir = connectionPool.Connection.PreviousDirectory;
                    }
                }
            }
            return prevDir;
        }

        //=========================================================================================
        // 機　能：指定されたユーザー名とサーバーに対する接続情報を返す
        // 引　数：[in]user    ユーザー名を返す変数への参照
        // 　　　　[in]server  サーバー名を返す変数への参照
        // 　　　　[in]port    ポート番号
        // 戻り値：接続情報（取得できないときnull）
        //=========================================================================================
        public UIConnectionPoolInfo GetConnectionInfo(string user, string server, int port) {
            lock (this) {
                string userServer = SSHUtils.CreateUserServerShort(user, server, port);
                if (m_connectionMap.ContainsKey(userServer)) {
                    ConnectionPool connectionPool = m_connectionMap[userServer];
                    return new UIConnectionPoolInfo(connectionPool);
                }
            }
            return null;
        }

        //=========================================================================================
        // 機　能：作業解放時にコネクションを解放する
        // 引　数：[in]cache     キャッシュ情報
        // 戻り値：なし
        //=========================================================================================
        public void EndFileOperation(FileOperationRequestContext cache) {
            lock (this) {
                foreach (ConnectionPool pool in m_connectionMap.Values) {
                    pool.FreeConnection(cache.BackgroundTaskId);
                    ConnectionChangedEventArgs args = new ConnectionChangedEventArgs(ConnectionChangedEventType.CountChanged, pool.User, pool.Server, pool.PortNo, new UIConnectionPoolInfo(pool));
                    ConnectionChanged(this, args);
                }
            }
        }

        //=========================================================================================
        // 機　能：指定された接続をクローズする
        // 引　数：[in]server   対象サーバ
        // 　　　　[in]user     ユーザ
        // 戻り値：なし
        //=========================================================================================
        public void CloseServer(string server, string user, int port) {
            lock (this) {
                string userServer = SSHUtils.CreateUserServerShort(user, server, port);
                if (m_connectionMap.ContainsKey(userServer)) {
                    ConnectionPool connectionPool = m_connectionMap[userServer];
                    connectionPool.CloseServer();
                    m_connectionMap.Remove(userServer);
                    ConnectionChangedEventArgs args = new ConnectionChangedEventArgs(ConnectionChangedEventType.ConnectionPoolDeleted, user, server, port, null);
                    ConnectionChanged(this, args);
                }
            }
        }

        //=========================================================================================
        // 機　能：すべての接続をクローズする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CloseAll() {
            lock (this) {
                List<string> keyList = new List<string>();
                foreach (string key in m_connectionMap.Keys) {
                    keyList.Add(key);
                }
                foreach (string userServer in keyList) {
                    ConnectionPool connectionPool = m_connectionMap[userServer];
                    connectionPool.CloseServer();
                    m_connectionMap.Remove(userServer);
                }
                ConnectionChangedEventArgs args = new ConnectionChangedEventArgs(ConnectionChangedEventType.StateReset, null, null, -1, null);
                ConnectionChanged(this, args);
            }
        }

        //=========================================================================================
        // 機　能：指定されたサーバーへの接続がプールにあることを確認する
        // 引　数：[in]server    サーバー名
        // 　　　　[in]user      ユーザー名
        // 　　　　[in]port      ポート番号
        // 戻り値：プールに接続があるときtrue
        //=========================================================================================
        public bool IsExistConnectionInPool(string server, string user, int port) {
            lock (this) {
                string userServer = SSHUtils.CreateUserServerShort(user, server, port);
                if (m_connectionMap.ContainsKey(userServer)) {
                    ConnectionPool connectionPool = m_connectionMap[userServer];
                    connectionPool.ModifyLastAccessedTime();
                    return true;
                } else {
                    return false;
                }
            }
        }

        //=========================================================================================
        // 機　能：SSHコネクションマネージャの状態が変わった時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void ConnectionManager_StateChanged(object sender, SSHConnectionManager.ConnectionChangedEventArgs evt) {
            // ConnectionChanged==null防止のダミー（最低1つのイベントを登録）
        }

        //=========================================================================================
        // プロパティ：接続リスト
        //=========================================================================================
        public List<UIConnectionPoolInfo> UIConnectionPoolList {
            get {
                List<UIConnectionPoolInfo> result = new List<UIConnectionPoolInfo>();
                lock (this) {
                    foreach (ConnectionPool list in m_connectionMap.Values) {
                        result.Add(new UIConnectionPoolInfo(list));
                    }
                }
                return result;
            }
        }

        //=========================================================================================
        // クラス：コネクション状態変化のイベント引数
        //=========================================================================================
        public class ConnectionChangedEventArgs {
            // 変化の種類
            private ConnectionChangedEventType m_eventType;

            // 変化したプールのユーザー名（StateResetのときはnull）
            private string m_user;

            // 変化したプールのサーバー名（StateResetのときはnull）
            private string m_server;

            // 変化したプールのポート番号（StateResetのときは-1）
            private int m_port;

            // コネクションプールのUI用情報（ConnectionPoolDeleted/StateResetのときはnull）
            private UIConnectionPoolInfo m_poolInfo;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]eventType    変化の種類
            // 　　　　[in]user         ユーザー名
            // 　　　　[in]server       サーバー名
            // 　　　　[in]port         ポート番号
            // 　　　　[in]poolInfo     コネクションプールのUI用情報
            // 戻り値：なし
            //=========================================================================================
            public ConnectionChangedEventArgs(ConnectionChangedEventType eventType, string user, string server, int port, UIConnectionPoolInfo poolInfo) {
                m_eventType = eventType;
                m_user = user;
                m_server = server;
                m_port = port;
                m_poolInfo = poolInfo;
            }
            
            //=========================================================================================
            // プロパティ：変化の種類
            //=========================================================================================
            public ConnectionChangedEventType EventType {
                get {
                    return m_eventType;
                }
            }

            //=========================================================================================
            // プロパティ：変化したプールのユーザー名（StateResetのときはnull）
            //=========================================================================================
            public string User {
                get {
                    return m_user;
                }
            }

            //=========================================================================================
            // プロパティ：変化したプールのサーバー名（StateResetのときはnull）
            //=========================================================================================
            public string Server {
                get {
                    return m_server;
                }
            }

            //=========================================================================================
            // プロパティ：変化したプールのポート番号（StateResetのときは-1）
            //=========================================================================================
            public int PortNo {
                get {
                    return m_port;
                }
            }

            //=========================================================================================
            // プロパティ：コネクションプールのUI用情報（ConnectionPoolDeleted/StateResetのときはnull）
            //=========================================================================================
            public UIConnectionPoolInfo PoolInfo {
                get {
                    return m_poolInfo;
                }
            }
        }

        //=========================================================================================
        // 列挙子：コネクション状態変化の種類
        //=========================================================================================
        public enum ConnectionChangedEventType {
            ConnectionPoolAdded,        // ConnectionPoolに新しいユーザー名/サーバーの組み合わせが登録された
            ConnectionPoolDeleted,      // ConnectionPoolからユーザー名/サーバーの組み合わせが削除された
            CountChanged,               // 使用中の接続数に変化が生じた
            StateReset,                 // 情報に大幅な変化が生じたため、再構築の必要がある
        }
    }
}
