using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ShellFiler.Api;

namespace ShellFiler.Locale {

    //=========================================================================================
    // クラス：エンコーディングの種類を表す列挙子として使用するクラス
    //=========================================================================================
    public class EncodingType {
        // 判別可能なエンコードの数
        public static int MAX_CODE_TYPE = 7;

        public static readonly EncodingType BINARY  = new EncodingType(0, "DUMP",      null,                              null);                               // バイナリ
        public static readonly EncodingType UNICODE = new EncodingType(1, "UNICODE",   new EncodingTypeChecker.Unicode(), Encoding.Unicode);                   // UNICODE
        public static readonly EncodingType JIS     = new EncodingType(2, "JIS",       new EncodingTypeChecker.Jis(),     Encoding.GetEncoding(50220));        // JIS
        public static readonly EncodingType EUC     = new EncodingType(3, "EUC-JP",    new EncodingTypeChecker.EucJp(),   Encoding.GetEncoding(51932));        // EUC
        public static readonly EncodingType SJIS    = new EncodingType(4, "SHIFT-JIS", new EncodingTypeChecker.SJis(),    Encoding.GetEncoding(932));          // SJIS
        public static readonly EncodingType UTF8    = new EncodingType(5, "UTF-8",     new EncodingTypeChecker.Utf8(),    Encoding.UTF8);                      // UTF-8
        public static readonly EncodingType UNKNOWN = new EncodingType(6, "",          null,                              null);                               // 不明

        // すべての値の列挙
        private static EncodingType[] s_allValue = {
            BINARY, UNICODE, JIS, EUC, SJIS, UTF8, UNKNOWN
        };

        // テキストエンコーディングとして有効なすべての値
        private static EncodingType[] s_allTextValue = {
            SJIS, UTF8, EUC, JIS, UNICODE
        };

        // 順序数
        private int m_order;

        // 表記名
        private string m_displayName;

        // この文字コードであることを確認するオブジェクト
        private EncodingTypeChecker m_encodingTypeChecker;

        // エンコーディング
        private Encoding m_encoding;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]order     順序数
        // 　　　　[in]display   表記名
        // 　　　　[in]checker   この文字コードであることを確認するオブジェクト
        // 　　　　[in]encoding  使用するEncoding
        // 戻り値：なし
        //=========================================================================================
        public EncodingType(int order, string display, EncodingTypeChecker checker, Encoding encoding) {
            m_order = order;
            m_displayName = display;
            m_encodingTypeChecker = checker;
            m_encoding = encoding;
        }

        //=========================================================================================
        // 機　能：文字列に対応するエンコードを返す
        // 引　数：[in]strEncoding  エンコードの文字列表現
        // 戻り値：エンコード（対応するものがない場合はnull）
        //=========================================================================================
        public static EncodingType FromString(string strEncoding) {
            EncodingType[] encodingTypeList = EncodingType.AllTextValue;
            foreach (EncodingType encodingType in encodingTypeList) {
                if (strEncoding == encodingType.DisplayName) {
                    return encodingType;
                }
            }
            return null;
        }

        //=========================================================================================
        // プロパティ：すべての値
        //=========================================================================================
        public static EncodingType[] AllValue {
            get {
                return s_allValue;
            }
        }

        //=========================================================================================
        // プロパティ：テキストエンコーディングとして有効なすべての値
        //=========================================================================================
        public static EncodingType[] AllTextValue {
            get {
                return s_allTextValue;
            }
        }

        //=========================================================================================
        // プロパティ：表記名
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
        }

        //=========================================================================================
        // プロパティ：この文字コードであることを確認するオブジェクト
        //=========================================================================================
        public EncodingTypeChecker EncodingTypeChecker {
            get {
                return m_encodingTypeChecker;
            }
        }

        //=========================================================================================
        // プロパティ：エンコーディング
        //=========================================================================================
        public Encoding Encoding {
            get {
                return m_encoding;
            }
        }

        //=========================================================================================
        // プロパティ：Windowsでのデフォルトエンコーディング
        //=========================================================================================
        public static Encoding WindowsDefaultEncoding {
            get {
                return  Encoding.GetEncoding(932);
            }
        }

        //=========================================================================================
        // プロパティ：HTTPリクエストで利用可能なエンコーディング
        //=========================================================================================
        public static EncodingType[] HttpRequestEncoding {
            get {
                EncodingType[] result = { UTF8, SJIS, EUC };
                return result;
            }
        }
    }
}
