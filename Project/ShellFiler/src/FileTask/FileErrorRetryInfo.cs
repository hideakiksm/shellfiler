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
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.Management;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイル操作での再試行情報
    //=========================================================================================
    public class FileErrorRetryInfo {
        // 実行しているタスクの種類
        private BackgroundTaskType m_taskType;

        // 転送元のファイルシステム
        private FileSystemID m_srcFileSystem;

        // 転送先のファイルシステム
        private FileSystemID m_destFileSystem;

        // 転送先ディレクトリのルート
        private string m_destDiretoryRoot;

        // 再試行するAPIの一覧
        private List<IRetryInfo> m_retryApiList = new List<IRetryInfo>();

        // 再試行するファイルの一覧
        private List<SimpleFileDirectoryPath> m_retryFileList = new List<SimpleFileDirectoryPath>();

        // 再試行するファイル一覧の重複情報
        private HashSet<SimpleFileDirectoryPath> m_retryFileListHash = new HashSet<SimpleFileDirectoryPath>();

        // ファイル操作のオプション（ショートカット作成のときnull）
        private CopyMoveDeleteOption m_option;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]taskType       実行しているタスクの種類
        // 　　　　[in]srcFileSystem  転送元のファイルシステム
        // 　　　　[in]destFileSystem 転送先のファイルシステム
        // 　　　　[in]destDir        転送先ディレクトリのルート
        // 　　　　[in]option         ファイル操作のオプション（ショートカット作成のときnull）
        // 戻り値：なし
        //=========================================================================================
        public FileErrorRetryInfo(BackgroundTaskType taskType, FileSystemID srcFileSystem, FileSystemID destFileSystem, string destDir, CopyMoveDeleteOption option) {
            m_taskType = taskType;
            m_srcFileSystem = srcFileSystem;
            m_destFileSystem = destFileSystem;
            m_destDiretoryRoot = destDir;
            m_option = option;
        }

        //=========================================================================================
        // 機　能：再試行するAPIを登録する
        // 引　数：[in]api  再試行対象のファイル
        // 戻り値：なし
        //=========================================================================================
        public void AddRetryApiInfo(IRetryInfo api) {
            SimpleFileDirectoryPath apiPath = api.SrcMarkObjectPath;
            if (!m_retryFileListHash.Contains(apiPath)) {
                m_retryApiList.Add(api);
                m_retryFileListHash.Add(apiPath);
            }
        }

        //=========================================================================================
        // 機　能：再試行するファイルを登録する
        // 引　数：[in]file  再試行対象のファイル
        // 戻り値：なし
        //=========================================================================================
        public void AddRetryFileInfo(SimpleFileDirectoryPath file) {
            if (!m_retryFileListHash.Contains(file)) {
                m_retryFileList.Add(file);
                m_retryFileListHash.Add(file);
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
        // プロパティ：再試行するAPIの一覧
        //=========================================================================================
        public List<IRetryInfo> RetryApiList {
            get {
                return m_retryApiList;
            }
        }

        //=========================================================================================
        // プロパティ：再試行するファイルの一覧
        //=========================================================================================
        public List<SimpleFileDirectoryPath> RetryFileList {
            get {
                return m_retryFileList;
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
