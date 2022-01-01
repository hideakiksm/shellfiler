using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Management;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;
using ShellFiler.Util;
using ShellFiler.Properties;
using ShellFiler.Virtual;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ローカルによるファイルの編集をバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class LocalExecuteBackgroundTask : AbstractFileBackgroundTask {
        // 実行するプログラム
        private string m_programFile;

        // プログラム引数を取得するためのdelegate
        private GetProgramArgumentDelegate m_programArgumentDelegate;
        
        // 表示名
        private TemporarySpaceDisplayName m_displayNameInfo;

        // ダウンロード用のテンポラリ空間（転送元）
        private LocalExecuteTemporarySpace m_srcTemporarySpace = null;

        // ダウンロード用のテンポラリ空間（転送先）
        private LocalExecuteTemporarySpace m_destTemporarySpace = null;

        // 対象パス 編集対象のディレクトリのルート（仮想ディレクトリのルートに対応するリモートディレクトリ）
        private string m_targetDirectoryRoot;

        // 反対パス 編集対象のディレクトリのルート（仮想ディレクトリのルートに対応するリモートディレクトリ、使用しない場合はnull）
        private string m_oppositeDirectoryRoot;
        
        // テンポラリ空間で終了待ちのプロセス（null:なし）
        private Process m_temporarySpaceProcess = null;

        // コマンド引数を得るためのdelegate
        // srcFileList:対象ファイルパスから集めたファイルパスの一覧
        // destFileList:反対ファイルパスから集めたファイルパスの一覧（反対パスを使用しない場合はnull）
        public delegate string GetProgramArgumentDelegate(List<string> srcFileList, List<string> destFileList);

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider   転送元ファイルの情報
        // 　　　　[in]destProvider  転送先ファイルの情報
        // 　　　　[in]refreshUi     作業完了時のUI更新方法
        // 　　　　[in]programFile   実行するプログラム
        // 　　　　[in]argDelegate   コマンド引数を得るためのdelegate
        // 　　　　[in]nameInfo      表示名の情報
        // 　　　　[in]targetDir     対象パス 編集対象のディレクトリ
        // 　　　　[in]oppositeDir   反対パス 編集対象のディレクトリ（使用しない場合はnull）
        // 戻り値：なし
        //=========================================================================================
        public LocalExecuteBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi,
                                          string programFile, GetProgramArgumentDelegate argDelegate,
                                          TemporarySpaceDisplayName nameInfo, string targetDir, string oppositeDir) : base(srcProvider, destProvider, refreshUi) {
            m_programFile = programFile;
            m_programArgumentDelegate = argDelegate;
            m_displayNameInfo = nameInfo;
            m_targetDirectoryRoot =  targetDir;
            m_oppositeDirectoryRoot = oppositeDir;
            CreateBackgroundTaskPathInfo();
        }

        //=========================================================================================
        // 機　能：転送元/転送先の情報を作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CreateBackgroundTaskPathInfo() {
            // 転送元
            string commandFile = GenericFileStringUtils.GetFileName(m_programFile);
            string srcShort = commandFile;
            string srcDetail = m_programFile;

            // 転送先
            string destShort = "";
            string destDetail = "";

            m_backgroundTaskPathInfo = new BackgroundTaskPathInfo(srcShort, srcDetail, destShort, destDetail);
        }

        //=========================================================================================
        // 機　能：タスクを実行する
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンドスレッドで実行する
        //=========================================================================================
        protected override void ExecuteTask() {
            SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(0);
            FileProviderSrc.SrcFileSystem.BeginFileOperation(RequestContext, pathInfo.FilePath);
            FileProviderDest.DestFileSystem.BeginFileOperation(RequestContext, FileProviderDest.DestDirectoryName);

            bool success = false;
            try {
                success = StartExecute();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                if (!success) {
                    if (m_srcTemporarySpace != null) {
                        m_srcTemporarySpace.Dispose();
                        m_srcTemporarySpace = null;
                    }
                    if (m_destTemporarySpace != null) {
                        m_destTemporarySpace.Dispose();
                        m_destTemporarySpace = null;
                    }
                    if (m_temporarySpaceProcess != null) {
                        m_temporarySpaceProcess.Dispose();
                        m_temporarySpaceProcess = null;
                    }
                } else {
                    Program.Document.TemporaryManager.AttachLocalExecuteSpace(m_srcTemporarySpace, m_destTemporarySpace, m_temporarySpaceProcess);
                    m_srcTemporarySpace = null;
                    m_destTemporarySpace = null;
                    m_temporarySpaceProcess = null;
                }
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：ローカル実行を開始する
        // 引　数：なし
        // 戻り値：処理に成功したときtrue
        //=========================================================================================
        private bool StartExecute() {
            // 対象パスの情報を収集
            List<string> srcFileList = new List<string>();
            FileOperationStatus status;
            if (FileProviderSrc.SrcFileSystem.LocalExecuteDownloadRequired) {
                // リモートの場合はダウンロード
                // 領域を準備
                FileSystemID remoteSysId = FileProviderSrc.SrcFileSystem.FileSystemId;
                m_srcTemporarySpace = Program.Document.TemporaryManager.CreateLocalExecuteSpace(m_programFile, m_displayNameInfo, m_targetDirectoryRoot, remoteSysId, FileProviderSrc.FileListContext);
                IFileSystem winFileSystem = Program.Document.FileSystemFactory.CreateFileSystemForFileOperation(FileSystemID.Windows);
                FileProviderDestSimple destProvider = new FileProviderDestSimple(m_srcTemporarySpace.VirtualDirectory, winFileSystem, new DummyFileListContext());
                ResetFileProvider(FileProviderSrc, destProvider);

                // ダウンロード
                List<LocalFileInfo> downloadedFile = new List<LocalFileInfo>();
                LocalDownloadControler controler = new LocalDownloadControler(this, RequestContext, FileProviderSrc, FileProviderDest, FileSystemToFileSystem, new FileProgressEventHandler(new FileProgressEventHandler.EventHandlerDelegate(ProgressEventHandler)));
                status = controler.DownloadMarkFiles(downloadedFile);
                if (!status.Succeeded) {
                    return false;
                }
                m_srcTemporarySpace.AddLocalFileList(downloadedFile);

                // ルートの一覧を作成
                for (int i = 0; i < FileProviderSrc.SrcItemCount; i++) {
                    SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(i);
                    string srcFileName = FileProviderSrc.SrcFileSystem.GetFileName(pathInfo.FilePath);
                    string destFilePath = FileProviderDest.DestFileSystem.CombineFilePath(FileProviderDest.DestDirectoryName, srcFileName);
                    srcFileList.Add(destFilePath);
                }
            } else {
                // ローカル実行の場合はファイル名を整理
                for (int i = 0; i < FileProviderSrc.SrcItemCount; i++) {
                    SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(i);
                    srcFileList.Add(pathInfo.FilePath);
                }
            }

            // 反対パスの情報を収集
            List<string> destFileList = null;
            if (FileProviderDestOrg is IFileProviderDestFiles) {
                destFileList = new List<string>();
                IFileProviderDestFiles destProviderFiles = (IFileProviderDestFiles)FileProviderDestOrg;
                if (FileProviderDestOrg.DestFileSystem.LocalExecuteDownloadRequired) {
                    // リモートの場合はダウンロード
                    // 領域を準備
                    IFileProviderSrc srcProvider = destProviderFiles.GetSrcProvider();
                    FileSystemID remoteSysId = FileProviderDestOrg.DestFileSystem.FileSystemId;
                    m_destTemporarySpace = Program.Document.TemporaryManager.CreateLocalExecuteSpace(m_programFile, m_displayNameInfo, m_oppositeDirectoryRoot, remoteSysId, FileProviderDestOrg.FileListContext);
                    IFileSystem winFileSystem = Program.Document.FileSystemFactory.CreateFileSystemForFileOperation(FileSystemID.Windows);
                    FileProviderDestSimple destProvider = new FileProviderDestSimple(m_destTemporarySpace.VirtualDirectory, winFileSystem, new DummyFileListContext());
                    ResetFileProvider(srcProvider, destProvider);

                    // ダウンロード
                    List<LocalFileInfo> downloadedFile = new List<LocalFileInfo>();
                    LocalDownloadControler controler = new LocalDownloadControler(this, RequestContext, FileProviderSrc, FileProviderDest, FileSystemToFileSystem, new FileProgressEventHandler(new FileProgressEventHandler.EventHandlerDelegate(ProgressEventHandler)));
                    status = controler.DownloadMarkFiles(downloadedFile);
                    if (!status.Succeeded) {
                        return false;
                    }
                    m_destTemporarySpace.AddLocalFileList(downloadedFile);

                    // ルートの一覧を作成
                    for (int i = 0; i < FileProviderSrc.SrcItemCount; i++) {
                        string srcPath = FileProviderSrc.GetSrcPath(i).FilePath;
                        string srcFileName = FileProviderSrc.SrcFileSystem.GetFileName(srcPath);
                        string destFilePath = FileProviderDest.DestFileSystem.CombineFilePath(FileProviderDest.DestDirectoryName, srcFileName);
                        destFileList.Add(destFilePath);
                    }
                } else {
                    // ローカル実行の場合はファイル名を整理
                    for (int i = 0; i < destProviderFiles.DestItemCount; i++) {
                        destFileList.Add(destProviderFiles.GetDestPath(i).FilePath);
                    }
                }
            }

            // ローカルでコマンドを実行
            string currentDir = m_targetDirectoryRoot;
            if(FileProviderSrc.FileListContext != null) {
                currentDir = FileProviderSrc.FileListContext.GetExecuteLocalPath(currentDir);
            } else if (m_srcTemporarySpace != null) {
                currentDir = m_srcTemporarySpace.VirtualDirectory;
            }

            string programArgument = m_programArgumentDelegate(srcFileList, destFileList);
            status = LocalExecute(m_programFile, programArgument, currentDir);
            if (!status.Succeeded) {
                return false;
            }

            return true;
        }
 
        //=========================================================================================
        // 機　能：ローカルでコマンドを実行する
        // 引　数：[in]program  実行するプログラム
        // 　　　　[in]argument プログラム引数
        // 　　　　[in]current  カレントパス
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        private FileOperationStatus LocalExecute(string program, string argument, string current) {
            // 起動の準備
            string programFile = GenericFileStringUtils.GetFileName(program);
            string currentDir = GenericFileStringUtils.GetDirectoryName(program);
            if (currentDir == program) {
                currentDir = current;
            }
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = program;
            psi.WorkingDirectory = currentDir;
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = false;
            psi.RedirectStandardError = false;
            psi.UseShellExecute = true;
            psi.CreateNoWindow = true;
            psi.Arguments = argument;

            // プロセスを起動
            try {
                Process process = OSUtils.StartProcess(psi, currentDir);
                Program.LogWindow.RegistLogLineHelper(string.Format(Resources.ShellExec_ProcessStart2, programFile));
                if (m_srcTemporarySpace == null && m_destTemporarySpace == null) {
                    process.Dispose();
                } else {
                    m_temporarySpaceProcess = process;
                }
            } catch (Exception) {
                LogLineSimple logLine = new LogLineSimple(LogColor.Error, Resources.ShellExec_ProcessError, program);
                Program.LogWindow.RegistLogLine(logLine);
                return FileOperationStatus.Fail;
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.LocalExecute;
            }
        }

        //=========================================================================================
        // プロパティ：転送元/転送先の情報
        //=========================================================================================
        public override BackgroundTaskPathInfo PathInfo {
            get {
                return m_backgroundTaskPathInfo;
            }
        }
    }
}
