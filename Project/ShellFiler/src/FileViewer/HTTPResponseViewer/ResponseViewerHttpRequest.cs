using System;
using System.Collections.Generic;
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
    public class ResponseViewerHttpRequest : ICloneable {
        // GET/HEADメソッドでのContents-Typeデフォルト
        public const string CONTENTS_TYPE_GET = "text/html";
        
        // PUTメソッドでのContents-Typeデフォルト
        public const string CONTENTS_TYPE_POST = "application/x-www-form-urlencoded; charset=UTF-8";
 
        // リクエストURL
        private string m_requestUrl;

        // プロキシサーバーのURL（null:使用しない）
        private string m_proxyUrl;

        // HTTPヘッダ（Content-Lengthを自動設定するときは値がnull）
        private List<KeyValuePair<string, string>> m_httpHeader;

        // HTTPリクエストのメソッド
        private string m_requestMethod;

        // HTTPボディのテキスト（テキスト入力モード以外はnull）
        private string m_httpBodyText;

        // HTTPボディのテキストエンコード方式（テキスト入力モード以外はnull）
        private EncodingType m_httpBodyEncodingType;
        
        // HTTPボディのファイル名（ファイル参照モード以外はnull）
        private string m_httpBodyFileName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ResponseViewerHttpRequest() {
        }
        
        //=========================================================================================
        // 機　能：デフォルト値を作成して返す
        // 引　数：なし
        // 戻り値：デフォルト値
        //=========================================================================================
        public static ResponseViewerHttpRequest GetDefaultValue() {
            ResponseViewerHttpRequest result = new ResponseViewerHttpRequest();
            result.m_requestUrl = "http://localhost:8080/";
            result.m_proxyUrl = null;
            result.m_httpHeader = new List<KeyValuePair<string, string>>();
            result.m_httpHeader.Add(new KeyValuePair<string, string>("Accept", "text/html, */*"));
            result.m_httpHeader.Add(new KeyValuePair<string, string>("Content-Type", CONTENTS_TYPE_POST));
            result.m_httpHeader.Add(new KeyValuePair<string, string>("Content-Length", null));
            result.m_httpHeader.Add(new KeyValuePair<string, string>("User-Agent", "ShellFiler HttpRequestViewer"));
            result.m_httpHeader.Add(new KeyValuePair<string, string>("Cache-Control", "no-cache"));
            result.m_requestMethod = "POST";
            result.m_httpBodyText = "";
            result.m_httpBodyEncodingType = EncodingType.UTF8;
            return result;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            ResponseViewerHttpRequest obj = (ResponseViewerHttpRequest)(MemberwiseClone());
            obj.m_httpHeader = new List<KeyValuePair<string, string>>();
            foreach (KeyValuePair<string, string> pair in m_httpHeader) {
                obj.m_httpHeader.Add(new KeyValuePair<string, string>(pair.Key, pair.Value));
            }
            return obj;
        }

        //=========================================================================================
        // プロパティ：リクエストURL
        //=========================================================================================
        public string RequestUrl {
            get {
                return m_requestUrl;
            }
            set {
                m_requestUrl = value;
            }
        }

        //=========================================================================================
        // プロパティ：プロキシサーバーのURL（null:使用しない）
        //=========================================================================================
        public string ProxyUrl {
            get {
                return m_proxyUrl;
            }
            set {
                m_proxyUrl = value;
            }
        }

        //=========================================================================================
        // プロパティ：HTTPヘッダ（Content-Lengthを自動設定するときは値がnull）
        //=========================================================================================
        public List<KeyValuePair<string, string>> HttpHeader {
            get {
                return m_httpHeader;
            }
            set {
                m_httpHeader = value;
            }
        }

        //=========================================================================================
        // プロパティ：HTTPリクエストのメソッド
        //=========================================================================================
        public string RequestMethod {
            get {
                return m_requestMethod;
            }
            set {
                m_requestMethod = value;
            }
        }

        //=========================================================================================
        // プロパティ：HTTPボディのテキスト（テキスト入力モード以外はnull）
        //=========================================================================================
        public string HttpBodyText {
            get {
                return m_httpBodyText;
            }
            set {
                m_httpBodyText = value;
            }
        }

        //=========================================================================================
        // プロパティ：HTTPボディのテキストエンコード方式（テキスト入力モード以外はnull）
        //=========================================================================================
        public EncodingType HttpBodyEncodingType {
            get {
                return m_httpBodyEncodingType;
            }
            set {
                m_httpBodyEncodingType = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：HTTPボディのファイル名（ファイル参照モード以外はnull）
        //=========================================================================================
        public string HttpBodyFileName {
            get {
                return m_httpBodyFileName;
            }
            set {
                m_httpBodyFileName = value;
            }
        }
    }
}
