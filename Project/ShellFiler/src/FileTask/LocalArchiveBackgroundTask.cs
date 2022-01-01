using System.IO;
using System.Collections.Generic;
using ShellFiler.Api;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Management;
using ShellFiler.FileSystem;
using ShellFiler.Virtual;
using ShellFiler.Archive;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ローカルによるファイルの展開をバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class LocalArchiveBackgroundTask : AbstractFileBackgroundTask {
        // 動作モード
        private OperationMode m_operationMode;

        // 対象のディレクトリのルート（圧縮ファイルのルートになるディレクトリ）
        private string m_targetDirectoryRoot; 

        // 展開先パスのモード
        private ExtractPathMode m_extractPathMode;

        // 圧縮で使用する設定（展開のときnull）
        private ArchiveParameter m_archiveSetting;

        // 転送元の作業ディレクトリ（直接参照のときはnull）
        private LocalTemporaryDirectory m_srcTemporaryDir = null;

        // 転送先の作業ディレクトリ（直接参照のときはnull）
        private LocalTemporaryDirectory m_destTemporaryDir = null;

        // クリップボードの文字列（ない場合と圧縮時はnull）
        private string m_clipboardString;

        // 同名ファイルを発見したときの動作
        private SameFileOperation m_sameFileOperation;

        // FileProviderDestOrgにある小文字化された転送先ファイル名の一覧（初期化されていないときnull）
        private HashSet<string> m_providerDestOrgFileNameListLower = null;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]operationMode   動作モード
        // 　　　　[in]srcProvider     転送元ファイルの情報
        // 　　　　[in]destProvider    転送先ファイルの情報
        // 　　　　[in]refreshUi       作業完了時のUI更新方法
        // 　　　　[in]targetRoot      対象のディレクトリのルート（圧縮ファイルのルートになるディレクトリ）
        // 　　　　[in]archiveSetting  圧縮で使用する設定（展開時はnull）
        // 　　　　[in]clipboardString クリップボードの文字列（ない場合と圧縮時はnull）
        // 戻り値：なし
        //=========================================================================================
        public LocalArchiveBackgroundTask(OperationMode operationMode, IFileProviderSrc srcProvider, IFileProviderDest destProvider,
                    RefreshUITarget refreshUi, string targetRoot, ExtractPathMode extractPathMode, ArchiveParameter archiveSetting, string clipboardString) :
                    base(srcProvider, destProvider, refreshUi) {
            m_operationMode = operationMode;
            m_targetDirectoryRoot = targetRoot;
            m_extractPathMode = extractPathMode;
            m_archiveSetting = archiveSetting;
            m_clipboardString = clipboardString;
            m_sameFileOperation = SameFileOperation.CreateWithDefaultConfig(destProvider.DestFileSystem.FileSystemId);
            m_sameFileOperation.AllApply = false;
            CreateBackgroundTaskPathInfo();
        }

        //=========================================================================================
        // 機　能：転送元/転送先の情報を作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CreateBackgroundTaskPathInfo() {
            // 転送元
            string srcShort = BackgroundTaskPathInfo.CreateShortTextFileProviderSrc(FileProviderSrcOrg, null);
            string srcDetail = BackgroundTaskPathInfo.CreateDetailTextFileProviderSrc(FileProviderSrcOrg, null);

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
            // タスク開始
            SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(0);
            FileProviderSrc.SrcFileSystem.BeginFileOperation(RequestContext, pathInfo.FilePath);
            FileProviderDest.DestFileSystem.BeginFileOperation(RequestContext, FileProviderDest.DestDirectoryName);

            try {
                // 展開を実行
                StartExecute();

                // 処理完了
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                if (m_srcTemporaryDir != null) {
                    m_srcTemporaryDir.Dispose();
                    m_srcTemporaryDir = null;
                }
                if (m_destTemporaryDir != null) {
                    m_destTemporaryDir.Dispose();
                    m_destTemporaryDir = null;
                }
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
                FileProviderDest.DestFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：ローカル実行を開始する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void StartExecute() {
            // 実行可能状態を調べる
            bool support = Program.Document.ArchiveFactory.IsSupportExtract();
            if (!support) {
                ArchiveUtils.ShowExtractError();
                return;
            }

            // 必要に応じてテンポラリにダウンロード
            List<SimpleFileDirectoryPath> fileList = new List<SimpleFileDirectoryPath>();
            bool success = PrepareExtractArchive(out fileList);
            if (!success) {
                return;
            }

            // 圧縮/展開を実行
            if (m_operationMode == OperationMode.Extract) {
                for (int i = 0; i <fileList.Count; i++) {
                    if (CheckSuspend()) {
                        return;
                    }
                    m_backgroundTaskPathInfo.SetProgressCount(i + 1, fileList.Count);
                    FireBackgroundtaskPathInfoProgressEvent();

                    SimpleFileDirectoryPath fileName = fileList[i];
                    Extract(fileName);
                }
                m_backgroundTaskPathInfo.SetProgressCount(0, 0);
                FixDataExtract();
            } else {
                if (CheckSuspend()) {
                    return;
                }
                Archive(fileList);
            }
        }

        //=========================================================================================
        // 機　能：ファイルの圧縮と展開の準備を行う（作業ディレクトリにダウンロード）
        // 引　数：[in]fileList  サブディレクトリまで含めた対象ファイルとディレクトリの一覧
        // 戻り値：処理に成功したときtrue
        //=========================================================================================
        private bool PrepareExtractArchive(out List<SimpleFileDirectoryPath> fileList) {
            FileOperationStatus status;

            // リモートの場合、仮想領域を用意
            {
                IFileProviderSrc srcProvider = FileProviderSrc;
                IFileProviderDest destProvider = FileProviderDest;
                if (FileProviderSrc.SrcFileSystem.LocalExecuteDownloadRequired) {
                    FileSystemID remoteSysId = FileProviderSrc.SrcFileSystem.FileSystemId;
                    m_srcTemporaryDir = Program.Document.TemporaryManager.CreateTemporaryDirectory();
                    m_targetDirectoryRoot = m_srcTemporaryDir.VirtualDirectory;
                }
                if (FileProviderDest.DestFileSystem.LocalExecuteDownloadRequired) {
                    FileSystemID remoteSysId = FileProviderDest.DestFileSystem.FileSystemId;
                    m_destTemporaryDir = Program.Document.TemporaryManager.CreateTemporaryDirectory();
                    IFileSystem destFileSystem = Program.Document.FileSystemFactory.CreateFileSystemForFileOperation(FileSystemID.Windows);
                    destProvider = new FileProviderDestSimple(m_destTemporaryDir.VirtualDirectory, destFileSystem, new DummyFileListContext());
                }
                ResetFileProvider(srcProvider, destProvider);
            }

            // 準備
            fileList = new List<SimpleFileDirectoryPath>();
            if (FileProviderSrc.SrcFileSystem.LocalExecuteDownloadRequired) {
                // リモートの場合はダウンロード
                List<LocalFileInfo> downloadedFile = new List<LocalFileInfo>();
                IFileSystem destFileSystem = Program.Document.FileSystemFactory.CreateFileSystemForFileOperation(FileSystemID.Windows);
                IFileProviderDest destProviderVirtual = new FileProviderDestSimple(m_srcTemporaryDir.VirtualDirectory, destFileSystem, new DummyFileListContext());
                LocalDownloadControler controler = new LocalDownloadControler(this, RequestContext, FileProviderSrc, destProviderVirtual, FileSystemToFileSystem, new FileProgressEventHandler(new FileProgressEventHandler.EventHandlerDelegate(ProgressEventHandler)));
                status = controler.DownloadMarkFiles(downloadedFile);
                if (!status.Succeeded) {
                    return false;
                }
                // ルートの一覧を作成
                for (int i = 0; i < FileProviderSrc.SrcItemCount; i++) {
                    SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(i);
                    string srcFileName = FileProviderSrc.SrcFileSystem.GetFileName(pathInfo.FilePath);
                    string destFilePath = destProviderVirtual.DestFileSystem.CombineFilePath(destProviderVirtual.DestDirectoryName, srcFileName);
                    fileList.Add(new SimpleFileDirectoryPath(destFilePath, pathInfo.IsDirectory, pathInfo.IsSymbolicLink));
                }
            } else {
                // ローカル実行の場合はファイル名を整理
                for (int i = 0; i < FileProviderSrc.SrcItemCount; i++) {
                    SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(i);
                    fileList.Add((SimpleFileDirectoryPath)(pathInfo.Clone()));
                }
            }
            return true;
        }



        //=========================================================================================
        // 機　能：ファイルをすべて展開する
        // 引　数：[in]fileInfo  アーカイブファイルの情報
        // 戻り値：なし
        //=========================================================================================
        private void Extract(SimpleFileDirectoryPath fileInfo) {
            FileOperationStatus status;

            // 処理開始
            string arcName = fileInfo.FilePath;
            LogFileOperationStart(FileOperationType.OpenArc, arcName, true);
            if (fileInfo.IsDirectory) {
                LogFileOperationEnd(FileOperationStatus.Skip);
                return;
            }

            // アーカイブを開く
            ArchiveAutoPasswordSetting autoPasswordSetting = Program.Document.UserSetting.ArchiveAutoPasswordSetting;
            ArchivePasswordSource passwordSource = new ArchivePasswordSource(autoPasswordSetting, arcName, m_clipboardString, null);
            IArchiveExtract archive = Program.Document.ArchiveFactory.CreateExtract(arcName, passwordSource);
            try {
                status = archive.Open(new FileProgressEventHandler(ProgressEventHandler));
                if (!status.Succeeded) {
                    LogFileOperationEnd(status);
                    return;
                }

                // 展開先を決定
                string rootFileName = archive.GetRootFileNameIfUnique();
                string destPathRoot = FileProviderDest.DestDirectoryName;
                string destPath;
                status = PrepareDestPath(arcName, rootFileName, destPathRoot, out destPath);
                if (status.Failed) {
                    LogFileOperationEnd(status);
                    return;
                }
                LogFileOperationEnd(status);

                // 展開先ディレクトリを作成
                if (destPathRoot != destPath) {
                    LogFileOperationStart(FileOperationType.MakeDir, destPath, true);
                    status = FileBackgroundTaskUtil.DestCreateDirectory(RequestContext, FileProviderDest.DestFileSystem, GenericFileStringUtils.RemoveLastDirectorySeparator(destPath, "\\"));
                    LogFileOperationEnd(status);
                    if (!status.Succeeded) {
                        return;
                    }
                }

                // 構成ファイルの数だけループして展開
                int fileCount = archive.GetFileCount();
                for (int i = 0; i < fileCount; i++) {
                    ExtractFile(archive, i, fileCount, destPath);
                    if (IsCancel) {
                        break;
                    }
                }

                // 自動処理のログ出力
                if (destPathRoot != destPath) {
                    string autoFolderName = GenericFileStringUtils.GetFileName(GenericFileStringUtils.RemoveLastDirectorySeparator(destPath, "\\"));
                    string message = string.Format(Resources.Log_ExtractAutoFolder, autoFolderName);
                    Program.LogWindow.RegistLogLineHelper(message);
                }
                if (archive.UsedPasswordDisplayName != null) {
                    string message = string.Format(Resources.Log_ExtractPassword, archive.UsedPasswordDisplayName);
                    Program.LogWindow.RegistLogLineHelper(message);
                }
            } finally {
                archive.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：転送先パスを必要に応じて作成して用意する
        // 引　数：[in]arcName       アーカイブファイル名
        // 　　　　[in]rootFileName  書庫内ルートファイル名（複数のルートがあるときはnull）
        // 　　　　[in]destPathRoot  転送先のルートとなるディレクトリ（反対パス/作業ディレクトリ名）
        // 　　　　[out]newDirPath   転送先ディレクトリを返す変数
        // 戻り値：ステータス
        //=========================================================================================
        FileOperationStatus PrepareDestPath(string arcName, string rootFileName, string destPathRoot, out string destPath) {
            FileOperationStatus status;
            if (m_extractPathMode == ExtractPathMode.Direct) {
                destPath = destPathRoot;
                return FileOperationStatus.Success;
            }

            // 転送先のファイル一覧を作成
            HashSet<string> fileNameListLower = new HashSet<string>();
            status = GetDestFileList(fileNameListLower);
            if (!status.Succeeded) {
                destPath = null;
                return status;
            }

            // ディレクトリを作成
            string arcFileName = GenericFileStringUtils.GetFileName(arcName);
            if (m_extractPathMode == ExtractPathMode.AlwaysNewDirectory) {
                // 常にディレクトリを作成する
                return CreateDestPathName(destPathRoot, arcFileName, fileNameListLower, out destPath);
            } else {
                // 混ざる心配があるときだけディレクトリを作成する
                if (rootFileName == null) {
                    return CreateDestPathName(destPathRoot, arcFileName, fileNameListLower, out destPath);
                } else if (fileNameListLower.Contains(rootFileName)) {
                    return CreateDestPathName(destPathRoot, arcFileName, fileNameListLower, out destPath);
                } else {
                    destPath = destPathRoot;
                    return FileOperationStatus.Success;
                }
            }
        }

        //=========================================================================================
        // 機　能：転送先パスの一覧を作成する
        // 引　数：[in]fileNameListLower  転送先のファイル名一覧を作成する変数
        // 戻り値：ステータス
        // メ　モ：転送先のパス一覧は、対象パスと作業ディレクトリのファイル名のみを小文字化したリスト
        //=========================================================================================
        private FileOperationStatus GetDestFileList(HashSet<string> fileNameListLower) {
            FileOperationStatus status;

            // 元のディレクトリのファイル一覧を作成
            if (m_providerDestOrgFileNameListLower != null) {
                // 2つ目以降の圧縮ファイルを展開する場合は前回の転送先一覧を使う
                foreach (string fileName in m_providerDestOrgFileNameListLower) {
                    fileNameListLower.Add(fileName.ToLower());
                }
            } else {
                // 1つめのの圧縮ファイルを展開する場合は一覧を作成
                List<IFile> fileList = new List<IFile>();
                status = FileProviderDestOrg.DestFileSystem.GetFileList(RequestContext, FileProviderDestOrg.DestDirectoryName, out fileList);
                if (!status.Succeeded) {
                    return status;
                }
                m_providerDestOrgFileNameListLower = new HashSet<string>();
                foreach (IFile file in fileList) {
                    if (file.FileName != "..") {
                        string fileNameLower = file.FileName.ToLower();
                        fileNameListLower.Add(fileNameLower);
                        m_providerDestOrgFileNameListLower.Add(fileNameLower);
                    }
                }
            }

            // 転送先がテンポラリのときは一覧を作成
            if (FileProviderDestOrg != FileProviderDest) {
                List<IFile> fileList = new List<IFile>();
                status = FileProviderDest.DestFileSystem.GetFileList(RequestContext, FileProviderDest.DestDirectoryName, out fileList);
                if (!status.Succeeded) {
                    return status;
                }
                foreach (IFile file in fileList) {
                    if (file.FileName != "..") {
                        fileNameListLower.Add(file.FileName.ToLower());
                    }
                }
            }

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：転送先パス名を作成する
        // 引　数：[in]destRoot           転送先のルートとなるディレクトリ（反対パス/作業ディレクトリ名）
        // 　　　　[in]newDirName         新しいディレクトリ名の候補（アーカイブファイル名）
        // 　　　　[in]existNameListLower 反対パスと作業ディレクトリ内にある既存ファイル一覧
        // 　　　　[out]newDirPath        作成したディレクトリ名を返す変数（作成していない場合は既存のパス）
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus CreateDestPathName(string destRoot, string newDirName, HashSet<string> existNameListLower, out string newDirPath) {
            // ユニークなファイル名を作成
            SameNameFileTransfer.CheckFileExistsDelegate checkExist = delegate(string file, out bool exist) {
                exist = (existNameListLower.Contains(file.ToLower()));
                return true;
            };
            string newDirFile = SameNameFileTransfer.CreateUniqueFileName(newDirName, SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber, true, checkExist);
            if (newDirFile == null) {
                newDirPath = null;
                return FileOperationStatus.Fail;
            }
            newDirPath = destRoot + newDirFile + "\\";

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ファイルまたはフォルダ1件を展開する
        // 引　数：[in]archive   アーカイブ
        // 　　　　[in]index     展開するファイルまたはフォルダのインデックス
        // 　　　　[in]fileCount 全ファイル数
        // 　　　　[in]destPath  対象ディレクトリのパス名
        // 戻り値：なし
        //=========================================================================================
        private void ExtractFile(IArchiveExtract archive, int index, int fileCount, string destPath) {
            FileOperationStatus status;

            // ファイルを展開
            IArchiveContentsFileInfo fileInfo = archive.GetFileInfo(index);
            try {
                FileOperationType type;
                if (fileInfo.IsDirectory) {
                    type = FileOperationType.ExtractDir;
                } else {
                    type = FileOperationType.ExtractFile;
                }
                if (fileInfo.FileName == null && fileCount == 1) {
                    string fileName = GenericFileStringUtils.GetFileName(archive.ArchiveFileName);
                    fileName = GenericFileStringUtils.GetFileNameBody(fileName);
                    fileInfo.FileName = fileName;
                }
                LogFileOperationStart(type, fileInfo.FileName, false);

                status = archive.Extract(fileInfo, destPath, false, new FileProgressEventHandler(ProgressEventHandler));
                if (status == FileOperationStatus.AlreadyExists) {
                    // 同名のファイルがあった場合
                    SameNameFileTransfer.TransferDelegate operation = delegate(FileOperationRequestContext context, string srcFile, string destFile, bool overwrite) {
                        fileInfo.FileName = GenericFileStringUtils.GetFileName(destFile);
                        status = archive.Extract(fileInfo, destPath, overwrite, new FileProgressEventHandler(ProgressEventHandler));
                        return status;
                    };
                    string destFilePath = destPath + fileInfo.FilePath;
                    SameNameFileTransfer sameFile = new SameNameFileTransfer(this, RequestContext, FileProviderSrc, FileProviderDest, SameNameFileTransfer.TransferMode.ExtractSameFile, m_sameFileOperation, operation);
                    SameNameTargetFileDetail fileDetail = new SameNameTargetFileDetail(RequestContext, archive, fileInfo, FileProviderDest.DestFileSystem, destFilePath);
                    status = sameFile.TransferSameFile(fileDetail);
                    if (status == FileOperationStatus.Canceled) {
                        SetCancel(CancelReason.User);
                    }
                    m_sameFileOperation = sameFile.SameFileOperation;
                }
                LogFileOperationEnd(status);
            } finally {
                fileInfo.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：展開結果を固定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void FixDataExtract() {
            // 作業ディレクトリの内容をはじめの転送先フォルダに確定
            if (FileProviderDestOrg.DestFileSystem.LocalExecuteDownloadRequired) {
                IFileSystem srcFileSystemTemp = Program.Document.FileSystemFactory.CreateFileSystemForFileOperation(FileSystemID.Windows);
                FileProviderSrcWindowsAll srcProviderTemp = new FileProviderSrcWindowsAll();
                bool success = srcProviderTemp.ReadFileList(FileProviderDest.DestFileSystem, m_destTemporaryDir.VirtualDirectory);
                if (!success) {
                    LogLineSimple log = new LogLineSimple(LogColor.Error, Resources.Log_ExtractError, m_destTemporaryDir.VirtualDirectory);
                    Program.LogWindow.RegistLogLine(log);
                    FailCount = 1;
                    SuccessCount = 0;
                    SkipCount = 0;
                } else {
                    ResetFileProvider(srcProviderTemp, FileProviderDestOrg);
                    LocalUploadControler uploadControler = new LocalUploadControler(this, RequestContext, srcProviderTemp, FileProviderDestOrg, FileSystemToFileSystem, new FileProgressEventHandler(new FileProgressEventHandler.EventHandlerDelegate(ProgressEventHandler)));
                    uploadControler.UploadFiles();
                }
            }
        }



        //=========================================================================================
        // 機　能：ファイルをすべて圧縮する
        // 引　数：[in]fileList      圧縮対象のファイル情報
        // 戻り値：なし
        //=========================================================================================
        private void Archive(List<SimpleFileDirectoryPath> fileList) {
            string archivePath = Program.Document.TemporaryManager.GetTemporaryFile();
            LogFileOperationStart(FileOperationType.CreateArc, m_archiveSetting.FileName, true);
            FileOperationStatus status = ArchiveCreateTemporary(fileList, ref archivePath);
            LogFileOperationEnd(status);
            if (status.Succeeded) {
                FixDataArchive(archivePath);
            }
        }

        //=========================================================================================
        // 機　能：アーカイブファイルをテンポラリ上に作成する
        // 引　数：[in]fileList         圧縮対象のファイル情報
        // 　　　　[in,out]arcFilePath  アーカイブファイルのフルパス名（途中で名前が変わったときは更新）
        // 戻り値：ステータス
        // メ　モ：tar.gzなどは、1回目の圧縮は指定パスに、2回目の圧縮はテンポラリパスに作成し、
        // 　　　　2回目の圧縮ファイル名を引数値に更新して返す。
        //=========================================================================================
        private FileOperationStatus ArchiveCreateTemporary(List<SimpleFileDirectoryPath> fileList, ref string arcFilePath) {
            // アーカイブを開く
            FileOperationStatus status = FileOperationStatus.Fail;
            IArchiveCreate archive = Program.Document.ArchiveFactory.CreateArchive(m_archiveSetting, arcFilePath);
            try {
                if (archive == null) {
                    status = FileOperationStatus.Fail;
                    return status;
                }
                status = archive.CreateNew();
                if (!status.Succeeded) {
                    return status;
                }
                if (IsCancel) {
                    return status;
                }

                // 対象の一覧を取得
                LogFileOperationStateChange(FileOperationStatus.ArcReading);
                List<ArchiveFileDirectoryInfo> allFileList;
                status = GetAllFileList(fileList, out allFileList);
                if (!status.Succeeded) {
                    return status;
                }

                // 指定ファイルまたはディレクトリを圧縮
                status = archive.UpdateFiles(this, m_targetDirectoryRoot, allFileList, new FileProgressEventHandler(ProgressEventHandler));
                if (!status.Succeeded) {
                    return status;
                }
            } finally {
                status = archive.CloseArchive(status);
                arcFilePath = archive.ArchiveFileNameFinal;
            }
            return status;
        }

        //=========================================================================================
        // 機　能：指定されたディレクトリ名一覧の配下にある全ファイルのリストを作成する
        // 引　数：[in]fielList       取得対象のファイルとディレクトリの一覧
        // 　　　　[out]allFileList   ディレクトリ内の構成ファイル一覧を返す変数
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus GetAllFileList(List<SimpleFileDirectoryPath> fileList, out List<ArchiveFileDirectoryInfo> allFileList) {
            FileOperationStatus status;
            allFileList = new List<ArchiveFileDirectoryInfo>();
            foreach (SimpleFileDirectoryPath file in fileList) {
                if (!file.IsDirectory) {
                    // ファイル
                    string fileName = GenericFileStringUtils.GetFileName(file.FilePath);
                    allFileList.Add(new ArchiveFileDirectoryInfo(fileName, false, ArchiveFileDirectoryInfo.UnknownFileSize));
                } else {
                    // ディレクトリ
                    string dirName = GenericFileStringUtils.GetFileName(file.FilePath);
                    ArchiveFileDirectoryInfo dirInfo = new ArchiveFileDirectoryInfo(dirName, true, 0);
                    allFileList.Add(dirInfo);

                    // サブディレクトリ
                    long subSize;
                    string directory = GenericFileStringUtils.GetDirectoryName(file.FilePath);
                    List<ArchiveFileDirectoryInfo> subFileList;
                    status = GetAllFileListDirectory(file.FilePath, directory, out subFileList, out subSize);
                    if (!status.Succeeded) {
                        return status;
                    }
                    if (IsCancel) {
                        return FileOperationStatus.Canceled;
                    }
                    allFileList.AddRange(subFileList);
                    dirInfo.Size = subSize;
                }
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ディレクトリ以下の全ファイルのリストを作成する
        // 引　数：[in]directory      取得対象のディレクトリ
        // 　　　　[in]baseDirectory  ファイル一覧で表示されていたディレクトリ（圧縮時のルート）
        // 　　　　[out]subFileList   ディレクトリ内の構成ファイル一覧を返す変数
        // 　　　　[out]subSize       ディレクトリ内のファイルサイズの合計
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus GetAllFileListDirectory(string directory, string baseDirectory, out List<ArchiveFileDirectoryInfo> subFileList, out long subSize) {
            FileOperationStatus status;
            subFileList = new List<ArchiveFileDirectoryInfo>();
            subSize = 0;

            // ディレクトリをキューに入れる
            // ファイル名はいったんパス情報なし
            List<ArchiveFileDirectoryInfo> fileList = new List<ArchiveFileDirectoryInfo>();
            List<ArchiveFileDirectoryInfo> dirList = new List<ArchiveFileDirectoryInfo>();
            {
                List<IFile> files = null;
                status = FileProviderSrc.SrcFileSystem.GetFileList(RequestContext, directory, out files);
                if (status != FileOperationStatus.Success) {
                    return status;
                }
                foreach (IFile file in files) {
                    if (file.FileName == "." || file.FileName == "..") {
                        continue;
                    }
                    if (!file.Attribute.IsDirectory) {
                        fileList.Add(new ArchiveFileDirectoryInfo(file.FileName, false, file.FileSize));
                    } else {
                        dirList.Add(new ArchiveFileDirectoryInfo(file.FileName, true, 0));
                    }
                    if (IsCancel) {
                        return FileOperationStatus.Canceled;
                    }
                }
            }

            // ファイルをコピー
            // ベースディレクトリ以下の部分パスに変換しながら格納
            foreach (ArchiveFileDirectoryInfo file in fileList) {
                string srcFilePath = FileProviderSrc.SrcFileSystem.CombineFilePath(directory, file.FilePath);
                string subPath = srcFilePath.Substring(baseDirectory.Length);
                subFileList.Add(new ArchiveFileDirectoryInfo(subPath, false, file.Size));
                subSize += file.Size;
            }

            // ディレクトリを再帰的にコピー
            foreach (ArchiveFileDirectoryInfo dir in dirList) {
                // ディレクトリ自身
                string srcDirPath = FileProviderSrc.SrcFileSystem.CombineFilePath(directory, dir.FilePath);
                string subPath = srcDirPath.Substring(baseDirectory.Length);
                ArchiveFileDirectoryInfo dirInfo = new ArchiveFileDirectoryInfo(subPath, true, 0);
                subFileList.Add(dirInfo);

                // サブディレクトリ
                List<ArchiveFileDirectoryInfo> childList;
                long childSize;
                status = GetAllFileListDirectory(srcDirPath, baseDirectory, out childList, out childSize);
                if (!status.Succeeded) {
                    return status;
                }
                if (IsCancel) {
                    return FileOperationStatus.Canceled;
                }
                subFileList.AddRange(childList);
                dirInfo.Size = childSize;
                subSize += childSize;
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：アーカイブの圧縮結果を固定する
        // 引　数：[in]archivePath  テンポラリに作成したアーカイブ
        // 戻り値：なし
        //=========================================================================================
        private void FixDataArchive(string archivePath) {
            // FileProviderを再設定
            // テンポラリ→はじめの転送先
            IFileSystem srcFileSystemTemp = Program.Document.FileSystemFactory.CreateFileSystemForFileOperation(FileSystemID.Windows);
            FileProviderSrcSpecified srcProviderTemp = new FileProviderSrcSpecified(new List<SimpleFileDirectoryPath>(), srcFileSystemTemp, null);
            ResetFileProvider(srcProviderTemp, FileProviderDestOrg);

            // ファイルをコピー
            string destFilePath = FileProviderDest.DestFileSystem.CombineFilePath(FileProviderDest.DestDirectoryName, m_archiveSetting.FileName);
            LogFileOperationStart(FileOperationType.FixArc, m_archiveSetting.FileName, false);
            FileOperationStatus status = FileSystemToFileSystem.CopyFile(RequestContext, archivePath, null, destFilePath, false, null, null, new FileProgressEventHandler(ProgressEventHandler));
            if (status == FileOperationStatus.AlreadyExists) {
                // 同名ファイルを処理
                SameNameFileTransfer.TransferDelegate operation = delegate(FileOperationRequestContext context, string srcFile, string destFile, bool overwrite) {
                    status = FileSystemToFileSystem.CopyFile(context, srcFile, null, destFile, overwrite, null, null, new FileProgressEventHandler(ProgressEventHandler));
                    return status;
                };
                SameNameFileTransfer sameFile = new SameNameFileTransfer(this, RequestContext, FileProviderSrc, FileProviderDest, SameNameFileTransfer.TransferMode.CopySameFile, m_sameFileOperation, operation);
                SameNameTargetFileDetail fileDetail = new SameNameTargetFileDetail(RequestContext, FileProviderSrc.SrcFileSystem, FileProviderDest.DestFileSystem, archivePath, destFilePath);
                try {
                    status = sameFile.TransferSameFile(fileDetail);
                    m_sameFileOperation = sameFile.SameFileOperation;
                } finally {
                    fileDetail.Dispose();
                }
            }

            // 結果ステータスを表示
            if (status == FileOperationStatus.SuccessCopy) {
                if (FileProviderDest.DestFileSystem.LocalExecuteDownloadRequired) {
                    LogFileOperationEnd(FileOperationStatus.SuccessUpload);
                } else {
                    LogFileOperationEnd(FileOperationStatus.SuccessCopy);
                }
            } else {
                LogFileOperationEnd(status);
            }
        }


        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                if (m_operationMode == OperationMode.Extract) {
                    return BackgroundTaskType.LocalExtract;
                } else {
                    return BackgroundTaskType.LocalArchive;
                }
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

        //=========================================================================================
        // プロパティ：動作モード
        //=========================================================================================
        public OperationMode Operation {
            get {
                return m_operationMode;
            }
        }

        //=========================================================================================
        // 列挙子：動作モード
        //=========================================================================================
        public enum OperationMode {
            Archive,                    // 圧縮
            Extract,                    // 展開
        }
    }
}
