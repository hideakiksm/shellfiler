using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.FileSystem.SFTP;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：接続のための認証情報の項目
    //=========================================================================================
    public class SSHUserAuthenticateSettingItem {
        // 表示名
        private string m_displayName = "";

        // サーバ名
        private string m_serverName = "";

        // ユーザ名
        private string m_userName = "";

        // 認証方式を公開鍵暗号方式にするときtrue
        private bool m_keyAuthentication = false;

        // パスワード、または、パスフレーズ(必要時に入力するときnull)
        private string m_password = null;

        // 秘密鍵のファイル名（パスワード認証のときnull）
        private string m_privateKeyFilePath = null;

        // ポート番号
        private int m_sshPortNo = 22;
        
        // 接続タイムアウト[ms]
        private int m_timeout = 5000;

        // エンコーディング方式
        private Encoding m_encoding = Encoding.UTF8;

        // OSの種類
        private OSType m_osType = OSType.DefaultOS;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SSHUserAuthenticateSettingItem() {
        }

        //=========================================================================================
        // 機　能：エンコーディングをインデックスに変換する
        // 引　数：[in]encoding  エンコーディング
        // 戻り値：インデックス
        //=========================================================================================
        public static int EncodingToIndex(Encoding encoding) {
            if (encoding == Encoding.UTF8) {
                return 0;
            } else if (encoding == Encoding.GetEncoding(51932)) {
                return 1;
            } else if (encoding == Encoding.GetEncoding(932)) {
                return 2;
            } else if (encoding == Encoding.GetEncoding(28591)) {
                return 3;
            } else {
                return 0;
            }
        }

        //=========================================================================================
        // 機　能：インデックスをエンコーディングに変換する
        // 引　数：[in]index  インデックス
        // 戻り値：エンコーディング
        //=========================================================================================
        public static Encoding IndexToEncoding(int index) {
            switch (index) {
                case 0:
                    return Encoding.UTF8;
                case 1:
                    return Encoding.GetEncoding(51932);
                case 2:
                    return Encoding.GetEncoding(932);
                case 3:
                    return Encoding.GetEncoding(28591);
                default:
                    return Encoding.UTF8;
            }
        }

        //=========================================================================================
        // 機　能：エンコーディングを名前に変換する
        // 引　数：[in]encoding  エンコーディング
        // 戻り値：エンコーディング名（変換できない場合はnull）
        //=========================================================================================
        public static string EncodingToName(Encoding encoding) {
            if (encoding == Encoding.UTF8) {
                return "UTF-8";
            } else if (encoding == Encoding.GetEncoding(51932)) {
                return "EUC-JP";
            } else if (encoding == Encoding.GetEncoding(932)) {
                return "SHIFT-JIS";
            } else if (encoding == Encoding.GetEncoding(28591)) {
                return "ISO-8859-1";
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：エンコーディング名をエンコーディングに変換する
        // 引　数：[in]name  エンコーディング名
        // 戻り値：エンコーディング（変換できない場合はnull）
        //=========================================================================================
        public static Encoding NameToEncoding(string name) {
            if (name == "UTF-8") {
                return Encoding.UTF8;
            } else if (name == "EUC-JP") {
                return Encoding.GetEncoding(51932);
            } else if (name =="SHIFT-JIS") {
                return Encoding.GetEncoding(932);
            } else if (name =="ISO-8859-1") {
                return Encoding.GetEncoding(28591);
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：OS種別をOSの種類名に変換する
        // 引　数：[in]name  OS種別
        // 戻り値：OSの種類名（不明なときnull）
        //=========================================================================================
        public static string OSTypeToName(OSType osType) {
            if (osType == OSType.DefaultOS) {
                return "Default";
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：OSの種類名をOS種別に変換する
        // 引　数：[in]name  OSの種類名
        // 戻り値：OS種別（不明なときnull）
        //=========================================================================================
        public static OSType NameToOSType(string name) {
            if (name == "Default") {
                return OSType.DefaultOS;
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：ディレクトリ名を返す
        // 引　数：[in]protocol   使用するプロトコル
        // 戻り値：ディレクトリ名
        //=========================================================================================
        public string GetDirectoryName(SSHProtocolType protocol) {
            return SSHUtils.CreateUserServer(protocol, m_userName, m_serverName, m_sshPortNo);
        }

        //=========================================================================================
        // プロパティ：表示名
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
            set {
                m_displayName = value;
            }
        }

        //=========================================================================================
        // プロパティ：サーバ名
        //=========================================================================================
        public string ServerName {
            get {
                return m_serverName;
            }
            set {
                m_serverName = value;
            }
        }

        //=========================================================================================
        // プロパティ：ユーザ名
        //=========================================================================================
        public string UserName {
            get {
                return m_userName;
            }
            set {
                m_userName = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：認証方式を公開鍵暗号方式にするときtrue
        //=========================================================================================
        public bool KeyAuthentication {
            get {
                return m_keyAuthentication;
            }
            set {
                m_keyAuthentication = value;
            }
        }

        //=========================================================================================
        // プロパティ：パスワード、または、パスフレーズ(必要時に入力するときnull)
        //=========================================================================================
        public string Password {
            get {
                return m_password;
            }
            set {
                m_password = value;
            }
        }

        //=========================================================================================
        // プロパティ：秘密鍵のファイル名（パスワード認証のときnull）
        //=========================================================================================
        public string PrivateKeyFilePath {
            get {
                return m_privateKeyFilePath;
            }
            set {
                m_privateKeyFilePath = value;
            }
        }

        //=========================================================================================
        // プロパティ：ポート番号
        //=========================================================================================
        public int PortNo {
            get {
                return m_sshPortNo;
            }
            set {
                m_sshPortNo = value;
            }
        }

        //=========================================================================================
        // プロパティ：接続タイムアウト[ms]
        //=========================================================================================
        public int Timeout {
            get {
                return m_timeout;
            }
            set {
                m_timeout = value;
            }
        }

        //=========================================================================================
        // プロパティ：エンコーディング方式
        //=========================================================================================
        public Encoding Encoding {
            get {
                return m_encoding;
            }
            set {
                m_encoding = value;
            }
        }

        //=========================================================================================
        // プロパティ：OSの種類
        //=========================================================================================
        public OSType TargetOS {
            get {
                return m_osType;
            }
            set {
                m_osType = value;
            }
        }

        //=========================================================================================
        // 列挙子：OSの種類
        //=========================================================================================
        public class OSType {
            public static readonly OSType DefaultOS = new OSType();              // デフォルト
        }
    }
}
