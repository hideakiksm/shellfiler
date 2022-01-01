using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer.HTTPResponseViewer {

    //=========================================================================================
    // クラス：HTTPモードでのリクエスト条件
    //=========================================================================================
    public class ResponseViewerTcpRequest : ICloneable {
        // サーバー名
        private string m_serverName;

        // ポート番号
        private int m_portNo;

        // リクエストのテキスト（テキスト入力モード以外はnull）
        private string m_requestText;

        // リクエストのテキストのエンコーディング（テキスト入力モード以外はnull）
        private EncodingType m_requestTextEncoding;

        // リクエストのファイル名（ファイル参照モード以外はnull）
        private string m_requestFileName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ResponseViewerTcpRequest() {
        }

        //=========================================================================================
        // 機　能：デフォルト値を作成して返す
        // 引　数：なし
        // 戻り値：デフォルト値
        //=========================================================================================
        public static ResponseViewerTcpRequest GetDefaultValue() {
            ResponseViewerTcpRequest result = new ResponseViewerTcpRequest();
            result.m_serverName = "localhost";
            result.m_portNo = 8080;
            result.m_requestText = "";
            result.m_requestTextEncoding = EncodingType.UTF8;
            return result;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            return MemberwiseClone();
        }

        //=========================================================================================
        // プロパティ：サーバー名
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
        // プロパティ：ポート番号
        //=========================================================================================
        public int PortNo {
            get {
                return m_portNo;
            }
            set {
                m_portNo = value;
            }
        }

        //=========================================================================================
        // プロパティ：リクエストのテキスト（テキスト入力モード以外はnull）
        //=========================================================================================
        public string RequestText {
            get {
                return m_requestText;
            }
            set {
                m_requestText = value;
            }
        }

        //=========================================================================================
        // プロパティ：リクエストのテキストのエンコーディング（テキスト入力モード以外はnull）
        //=========================================================================================
        public EncodingType RequestTextEncodingType {
            get {
                return m_requestTextEncoding;
            }
            set {
                m_requestTextEncoding = value;
            }
        }

        //=========================================================================================
        // プロパティ：リクエストのファイル名（ファイル参照モード以外はnull）
        //=========================================================================================
        public string RequestFileName {
            get {
                return m_requestFileName;
            }
            set {
                m_requestFileName = value;
            }
        }
    }
}
