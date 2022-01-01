using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.Management;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイル操作でのエラー情報
    //=========================================================================================
    public class FileErrorInfo {
        // 登録できるエラー情報の上限
        private const int MAX_ERROR_INFO_COUNT = 10;

        // バックグラウンドタスクのID
        private BackgroundTaskID m_taskId;

        // 実行しているタスクの種類
        private BackgroundTaskType m_taskType;

        // 転送元のファイルシステム
        private FileSystemID m_srcFileSystem;

        // 転送先のファイルシステム
        private FileSystemID m_destFileSystem;

        // 転送先ディレクトリのルート
        private string m_destDiretoryRoot;

        // 登録したエラー情報の数
        private int m_errorInfoCount = 0;

        // エラーの一覧
        private List<FileErrorInfoItem> m_errorList = new List<FileErrorInfoItem>();

        // ファイル操作のオプション（ショートカット作成のときnull）
        private CopyMoveDeleteOption m_option;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]taskId         バックグラウンドタスクのID
        // 　　　　[in]taskType       実行しているタスクの種類
        // 　　　　[in]srcFileSystem  転送元のファイルシステム
        // 　　　　[in]destFileSystem 転送先のファイルシステム
        // 　　　　[in]destDir        転送先ディレクトリのルート
        // 　　　　[in]option         ファイル操作のオプション（ショートカット作成のときnull）
        // 戻り値：なし
        //=========================================================================================
        public FileErrorInfo(BackgroundTaskID taskId, BackgroundTaskType taskType, FileSystemID srcFileSystem, FileSystemID destFileSystem, string destDir, CopyMoveDeleteOption option) {
            m_taskId = taskId;
            m_taskType = taskType;
            m_srcFileSystem = srcFileSystem;
            m_destFileSystem = destFileSystem;
            m_destDiretoryRoot = destDir;
            m_option = option;
        }

        //=========================================================================================
        // 機　能：エラー情報を登録する
        // 引　数：[in]errorInfo   エラー情報
        // 戻り値：なし
        //=========================================================================================
        public void AddFileErrorInfo(FileErrorInfoItem errorInfo) {
            m_errorInfoCount++;
            if (m_errorList.Count < MAX_ERROR_INFO_COUNT) {
                m_errorList.Add(errorInfo);
            }
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクのID
        //=========================================================================================
        public BackgroundTaskID TaskId {
            get {
                return m_taskId;
            }
        }

        //=========================================================================================
        // プロパティ：実行しているタスクの種類
        //=========================================================================================
        public BackgroundTaskType TaskType {
            get {
                return m_taskType;
            }
        }

        //=========================================================================================
        // プロパティ：転送元のファイルシステム
        //=========================================================================================
        public FileSystemID SrcFileSystem {
            get {
                return m_srcFileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：転送先のファイルシステム
        //=========================================================================================
        public FileSystemID DestFileSystem {
            get {
                return m_destFileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：転送先ディレクトリのルート
        //=========================================================================================
        public string DestDiretoryRoot {
            get {
                return m_destDiretoryRoot;
            }
        }

        //=========================================================================================
        // プロパティ：エラーが多すぎて登録しきれないときtrue
        //=========================================================================================
        public bool TooManyError {
            get {
                return (m_errorInfoCount > m_errorList.Count);
            }
        }

        //=========================================================================================
        // プロパティ：登録したエラー情報の数
        //=========================================================================================
        public int ErrorInfoCount {
            get {
                return m_errorInfoCount;
            }
        }

        //=========================================================================================
        // プロパティ：エラーの一覧
        //=========================================================================================
        public List<FileErrorInfoItem> ErrorList {
            get {
                return m_errorList;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル操作のオプション（ショートカット作成のときnull）
        //=========================================================================================
        public CopyMoveDeleteOption Option {
            get {
                return m_option;
            }
        }
    }
}
