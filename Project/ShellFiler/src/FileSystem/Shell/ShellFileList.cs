using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.UI.Dialog;

namespace ShellFiler.FileSystem.Shell {

    //=========================================================================================
    // クラス：ファイル一覧の情報
    //=========================================================================================
    public class ShellFileList : IFileList {
        // ファイルシステム
        private ShellFileSystem m_fileSystem;

        // 現在のディレクトリのフルパス
        private string m_directoryFullPath;

        // ファイル一覧
        private List<IFile> m_fileList = new List<IFile>();

        // ファイルリストが左ウィンドウで表示されるときtrue
        private bool m_isLeftWindow;
        
        // ボリューム情報
        private VolumeInfo m_volumeInfo = null;

        // SSHファイルシステムでのファイル一覧のコンテキスト情報
        private ShellFileListContext m_fileListContext;

        // ファイル再読込の世代番号
        private int m_loadingGeneration;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileSystem   ファイルシステム
        // 　　　　[in]directory    一覧を作成するディレクトリ
        // 　　　　[in]isLeftWindow 左ウィンドウに表示する一覧のときtrue
        // 　　　　[in]fileListCtx  使用中のファイル一覧のコンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellFileList(IFileSystem fileSystem, string directory, bool isLeftWindow, IFileListContext fileListCtx) {
            // ディレクトリを変更
            m_fileSystem = (ShellFileSystem)fileSystem;
            m_directoryFullPath = m_fileSystem.CompleteDirectoryName(m_fileSystem.GetFullPath(directory));
            m_fileList.Clear();
            m_isLeftWindow = isLeftWindow;
            m_loadingGeneration = Program.Document.FileSystemFactory.GetNextLoadingGeneration();
            m_fileListContext = (ShellFileListContext)fileListCtx;
        }

        //=========================================================================================
        // 機　能：一覧を取得する
        // 引　数：[in]chdirMode    ディレクトリ変更のモード
        // 戻り値：一覧取得のステータス
        //=========================================================================================
        public ChangeDirectoryStatus GetFileList(ChangeDirectoryParam chdirMode) {
            SSHProtocolType protocol;
            string user, server, local;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(m_directoryFullPath, out protocol, out user, out server, out portNo, out local);
            if (!success) {
                return ChangeDirectoryStatus.Failed;
            }

            SSHConnectionManager manager = Program.Document.FileSystemFactory.SSHConnectionManager;
            SSHUserAuthenticateSetting settingList = Program.Document.SSHUserAuthenticateSetting;
            bool isTemp;
            SSHUserAuthenticateSettingItem setting = settingList.ReferAuthenticateSetting(server, user, portNo, out isTemp);
            if (setting == null) {
                // 設定がない場合はプールに接続があるか調べる
                bool existInPool = manager.IsExistConnectionInPool(server, user, portNo);
                if (!existInPool) {
                    // プールにもない場合は新規接続として情報を入力
                    setting = SSHUtils.InputUserAuthenticateInfo(user, server, local, portNo);
                    if (setting == null) {
                        return ChangeDirectoryStatus.Failed;
                    }
                }
            } else {
                // 再接続なしで接続処理中なら重複の可能性があるため、念のため失敗
                if (isTemp) {
                    bool loading = Program.Document.TabPageList.CheckLoadingFileList();
                    if (loading) {
                        return ChangeDirectoryStatus.Failed;
                    }
                }
            }
            return ChangeDirectoryStatus.Loading;
        }

        //=========================================================================================
        // 機　能：並列読み込みを開始する
        // 引　数：[in]chdirMode   ディレクトリ変更のモード
        // 戻り値：なし
        //=========================================================================================
        public void StartLoading(ChangeDirectoryParam chdirMode) {
            FileOperationRequestContext context = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(BackgroundTaskID.NullId, FileSystemID.SSHShell, FileSystemID.None, m_fileListContext, null, null);
            m_fileSystem.GetUIFileList(context, this, m_directoryFullPath, m_isLeftWindow, chdirMode);
        }

        //=========================================================================================
        // プロパティ：一覧取得に使用するファイルシステム
        //=========================================================================================
        public IFileSystem FileSystem {
            get {
                return m_fileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧を返す
        //=========================================================================================
        public List<IFile> Files {
            get {
                return m_fileList;
            }
            set {
                m_fileList = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリ名（最後は必ずセパレータ）
        //=========================================================================================
        public string DirectoryName {
            get {
                return m_directoryFullPath;
            }
            set {
                m_directoryFullPath = value;
            }
        }

        //=========================================================================================
        // プロパティ：左ウィンドウで表示される一覧のときtrue
        //=========================================================================================
        public bool IsLeftWindow {
            get {
                return m_isLeftWindow;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル再読込の世代番号
        //=========================================================================================
        public int LoadingGeneration {
            get {
                return m_loadingGeneration;
            }
        }

        //=========================================================================================
        // プロパティ：ボリューム情報
        //=========================================================================================
        public VolumeInfo VolumeInfo {
            get {
                return m_volumeInfo;
            }
            set {
                m_volumeInfo = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルシステムごとのコンテキスト情報（一覧取得前はnull）
        //=========================================================================================
        public IFileListContext FileListContext {
            get {
                return m_fileListContext;
            }
            set {
                m_fileListContext = (ShellFileListContext)value;
            }
        }
    }
}
