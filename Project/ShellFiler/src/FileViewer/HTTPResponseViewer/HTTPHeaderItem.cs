using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileTask.DataObject;

namespace ShellFiler.FileViewer.HTTPResponseViewer {

    //=========================================================================================
    // クラス：HTTPヘッダの既定値
    //=========================================================================================
    public class HTTPHeaderItem {
        // すべての項目
        private static List<HTTPHeaderItem> s_allItems = new List<HTTPHeaderItem>();

        // ヘッダ名から項目へのMap
        private static Dictionary<string, HTTPHeaderItem> s_nameToHeader = new Dictionary<string, HTTPHeaderItem>();

        // 項目の定義
        public static readonly HTTPHeaderItem Accept             = new HTTPHeaderItem("Accept");
        public static readonly HTTPHeaderItem AcceptCharset      = new HTTPHeaderItem("Accept-Charset");
        public static readonly HTTPHeaderItem AcceptEncoding     = new HTTPHeaderItem("Accept-Encoding");
        public static readonly HTTPHeaderItem AcceptLanguage     = new HTTPHeaderItem("Accept-Language");
        public static readonly HTTPHeaderItem Allow              = new HTTPHeaderItem("Allow");
        public static readonly HTTPHeaderItem Authorization      = new HTTPHeaderItem("Authorization");
        public static readonly HTTPHeaderItem CacheControl       = new HTTPHeaderItem("Cache-Control");
        public static readonly HTTPHeaderItem Connection         = new HTTPHeaderItem("Connection");
        public static readonly HTTPHeaderItem ContentEncoding    = new HTTPHeaderItem("Content-Encoding");
        public static readonly HTTPHeaderItem ContentLanguage    = new HTTPHeaderItem("Content-Language");
        public static readonly HTTPHeaderItem ContentLength      = new HTTPHeaderItem("Content-Length");
        public static readonly HTTPHeaderItem ContentLocation    = new HTTPHeaderItem("Content-Location");
        public static readonly HTTPHeaderItem ContentMD5         = new HTTPHeaderItem("Content-MD5");
        public static readonly HTTPHeaderItem ContentRange       = new HTTPHeaderItem("Content-Range");
        public static readonly HTTPHeaderItem ContentType        = new HTTPHeaderItem("Content-Type");
        public static readonly HTTPHeaderItem Date               = new HTTPHeaderItem("Date");
        public static readonly HTTPHeaderItem Expect             = new HTTPHeaderItem("Expect");
        public static readonly HTTPHeaderItem Expires            = new HTTPHeaderItem("Expires");
        public static readonly HTTPHeaderItem From               = new HTTPHeaderItem("From");
        public static readonly HTTPHeaderItem Host               = new HTTPHeaderItem("Host");
        public static readonly HTTPHeaderItem IfMatch            = new HTTPHeaderItem("If-Match");
        public static readonly HTTPHeaderItem IfModifiedSince    = new HTTPHeaderItem("If-Modified-Since");
        public static readonly HTTPHeaderItem IfNoneMatch        = new HTTPHeaderItem("If-None-Match");
        public static readonly HTTPHeaderItem IfRange            = new HTTPHeaderItem("If-Range");
        public static readonly HTTPHeaderItem IfUnmodifiedSince  = new HTTPHeaderItem("If-Unmodified-Since");
        public static readonly HTTPHeaderItem LastModified       = new HTTPHeaderItem("Last-Modified");
        public static readonly HTTPHeaderItem MaxForwards        = new HTTPHeaderItem("Max-Forwards");
        public static readonly HTTPHeaderItem Pragma             = new HTTPHeaderItem("Pragma");
        public static readonly HTTPHeaderItem ProxyAuthorization = new HTTPHeaderItem("Proxy-Authorization");
        public static readonly HTTPHeaderItem Range              = new HTTPHeaderItem("Range");
        public static readonly HTTPHeaderItem Referer            = new HTTPHeaderItem("Referer");
        public static readonly HTTPHeaderItem TE                 = new HTTPHeaderItem("TE");
        public static readonly HTTPHeaderItem Trailer            = new HTTPHeaderItem("Trailer");
        public static readonly HTTPHeaderItem TransferEncoding   = new HTTPHeaderItem("Transfer-Encoding");
        public static readonly HTTPHeaderItem Upgrade            = new HTTPHeaderItem("Upgrade");
        public static readonly HTTPHeaderItem UserAgent          = new HTTPHeaderItem("User-Agent");
        public static readonly HTTPHeaderItem Via                = new HTTPHeaderItem("Via");
        public static readonly HTTPHeaderItem Warning            = new HTTPHeaderItem("Warning");

        // HTTPヘッダ名
        private string m_httpHeaderName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]headerName   HTTPヘッダ名
        // 戻り値：なし
        //=========================================================================================
        private HTTPHeaderItem(string headerName) {
            m_httpHeaderName = headerName;
            s_allItems.Add(this);
            s_nameToHeader.Add(headerName, this);
        }

        //=========================================================================================
        // 機　能：HTTPヘッダ名に対するオブジェクトを返す
        // 引　数：[in]headerName   HTTPヘッダ名
        // 戻り値：オブジェクト（対応するものがないときはnull）
        //=========================================================================================
        public static HTTPHeaderItem GetHeaderItemFromName(string headerName) {
            if (s_nameToHeader.ContainsKey(headerName)) {
                return s_nameToHeader[headerName];
            } else {
                return null;
            }
        }
        
        //=========================================================================================
        // プロパティ：HTTPヘッダ名
        //=========================================================================================
        public string HTTPHeaderName {
            get {
                return m_httpHeaderName;
            }
        }

        //=========================================================================================
        // プロパティ：すべての項目
        //=========================================================================================
        public static List<HTTPHeaderItem> AllItems {
            get {
                return s_allItems;
            }
        }
    }
}
