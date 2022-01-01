using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイル操作でのエラー情報1件分の項目
    //=========================================================================================
    public class FileErrorInfoItem {
        // エラーが発生した操作（再試行不可能、再試行OKとして登録されているときnull）
        private FileOperationType m_fileOperarionType;
        
        // エラーの種類
        private FileOperationStatus m_resultStatus;

        // 再試行用のマークされたファイルのパス情報
        private SimpleFileDirectoryPath m_srcMarkObjectPath;

        // 再試行情報（再試行できないときnull）
        private IRetryInfo m_retryInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]operationType     エラーが発生した操作
        // 　　　　[in]resultStatus      エラーの種類
        // 　　　　[in]retryInfo         再試行情報（再試行できないときnull）
        // 戻り値：なし
        //=========================================================================================
        public FileErrorInfoItem(FileOperationType operationType, FileOperationStatus resultStatus, IRetryInfo retryInfo) {
            m_fileOperarionType = operationType;
            m_resultStatus = resultStatus;
            m_retryInfo = retryInfo;
        }

        //=========================================================================================
        // プロパティ：エラーが発生した操作
        //=========================================================================================
        public FileOperationType FileOperarionType {
            get {
                return m_fileOperarionType;
            }
        }

        //=========================================================================================
        // プロパティ：エラーの種類
        //=========================================================================================
        public FileOperationStatus ResultStatus {
            get {
                return m_resultStatus;
            }
        }

        //=========================================================================================
        // プロパティ：マーク階層
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
        // プロパティ：再試行情報（再試行できないときnull）
        //=========================================================================================
        public IRetryInfo RetryInfo {
            get {
                return m_retryInfo;
            }
        }
    }
}
