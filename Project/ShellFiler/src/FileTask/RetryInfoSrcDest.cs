using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイルの転送元と転送先を持つ再試行情報（コピー、移動）
    //=========================================================================================
    public class RetryInfoSrcDest : IRetryInfo {
        // エラーが発生した操作
        private FileOperationApiType m_operationApiType;
        
        // 再試行用のマークされたファイルのパス情報
        private SimpleFileDirectoryPath m_srcMarkObjectPath;

        // 転送元
        private string m_srcFilePath;

        // 転送先
        private string m_destFilePath;

        //=========================================================================================
        // 機　能：コンストラクタ（APIが明確なため再試行OKとして登録する場合）
        // 引　数：[in]operationApi   エラーが発生した操作
        // 　　　　[in]srcFilePath    転送元
        // 　　　　[in]destFilePath   転送先
        // 戻り値：なし
        //=========================================================================================
        public RetryInfoSrcDest(FileOperationApiType operationApi, string srcFilePath, string destFilePath) {
            m_operationApiType = operationApi;
            m_srcFilePath = srcFilePath;
            m_destFilePath = destFilePath;
        }

        //=========================================================================================
        // 機　能：ヒント文字列を取得する
        // 引　数：なし
        // 戻り値：ヒント文字列
        //=========================================================================================
        public string GetHintString() {
            if (m_destFilePath != null) {
                return string.Format(Resources.DlgFileErrList_ListViewSrcDestHint, m_srcFilePath, m_destFilePath);
            } else {
                return string.Format(Resources.DlgFileErrList_ListViewSrcHint, m_srcFilePath);
            }
        }

        //=========================================================================================
        // プロパティ：エラーが発生した操作
        //=========================================================================================
        public FileOperationApiType OperationApiType {
            get {
                return m_operationApiType;
            }
        }

        //=========================================================================================
        // プロパティ：再試行用のマークされたファイルのパス情報
        //=========================================================================================
        public SimpleFileDirectoryPath SrcMarkObjectPath {
            get {
                return m_srcMarkObjectPath;
            }
            set {
                m_srcMarkObjectPath = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：エラーが発生したファイルそのもの
        //=========================================================================================
        public string ErrorFilePath {
            get {
                return m_srcFilePath;
            }
        }

        //=========================================================================================
        // プロパティ：転送元
        //=========================================================================================
        public string SrcFilePath {
            get {
                return m_srcFilePath;
            }
        }

        //=========================================================================================
        // プロパティ：転送先
        //=========================================================================================
        public string DestFilePath {
            get {
                return m_destFilePath;
            }
        }
    }
}
