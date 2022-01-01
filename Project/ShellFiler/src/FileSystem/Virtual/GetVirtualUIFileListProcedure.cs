using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;
using ShellFiler.FileTask.DataObject;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.Virtual {

    //=========================================================================================
    // クラス：仮想フォルダでUI用のファイル一覧をバックグラウンドで取得するプロシージャ
    //=========================================================================================
    class GetVirtualUIFileListProcedure : AbstractBackgroundProcedure {
        // 仮想フォルダのファイル一覧
        private VirtualFileList m_fileList = null;

        // 次に使用する仮想フォルダ情報（エラー時は処理途中の展開結果が入っている）
        private VirtualFolderInfo m_nextVirtualFolder;

        // ログ出力
        private ITaskLogger m_logger = new AutoTaskLogger();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public GetVirtualUIFileListProcedure() {
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in,out]arg    リクエスト/レスポンス
        // 戻り値：ステータスコード
        //=========================================================================================
        public override FileOperationStatus Execute(AbstractProcedureArg aarg) {
            // パラメータを初期化
            GetVirtualUIFileListArg arg = (GetVirtualUIFileListArg)aarg;
            string displayDirectory = arg.DisplayDirectory;
            bool isLeftWindow = arg.IsLeftWindow;
            int loadingGeneration = arg.LoadingGeneration;
            m_nextVirtualFolder = arg.VirtualFolder;

            // 一覧取得
            FileOperationStatus status = GetFileList(m_nextVirtualFolder, displayDirectory, isLeftWindow, loadingGeneration);
            
            // ログ出力
            if (m_logger.LogCount > 0) {
                LogLineSimple log;
                if (status.Succeeded) {
                    log = new LogLineSimple(Resources.Log_VirtualFolderSuccess);
                } else {
                    log = new LogLineSimple(LogColor.Error, Resources.Log_VirtualFolderFailed);
                }
                Program.LogWindow.RegistLogLine(log);
            }

            // 通知
            aarg.Status = status;
            if (status.Succeeded) {
                NotifyTaskEnd(arg, m_fileList, m_nextVirtualFolder);
            }

            return status;
        }

        //=========================================================================================
        // 機　能：他の機能の一部としてコマンドを実行する
        // 引　数：[in,out]arg     リクエスト/レスポンス
        // 　　　　[in]logger      ログ出力機能
        // 　　　　[out]fileList   仮想フォルダのファイル一覧
        // 　　　　[out]nextFolder 次に使用する仮想フォルダ情報（エラー時は処理途中の展開結果が入っている）
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus ExecuteSubProcess(AbstractProcedureArg aarg, ITaskLogger logger, out VirtualFileList fileList, out VirtualFolderInfo nextFolder) {
            // パラメータを初期化
            m_logger = logger;

            GetVirtualUIFileListArg arg = (GetVirtualUIFileListArg)aarg;
            string displayDirectory = arg.DisplayDirectory;
            bool isLeftWindow = arg.IsLeftWindow;
            int loadingGeneration = arg.LoadingGeneration;
            m_nextVirtualFolder = arg.VirtualFolder;

            // 一覧取得
            FileOperationStatus status = GetFileList(m_nextVirtualFolder, displayDirectory, isLeftWindow, loadingGeneration);
            aarg.Status = status;
            fileList = m_fileList;
            nextFolder = m_nextVirtualFolder;
            return status;
        }

        //=========================================================================================
        // 機　能：ファイル一覧取得の機能を実行する
        // 引　数：[in]virtualFolder     仮想フォルダの情報（新しい位置を設定して戻る）
        // 　　　　[in]displayDirectory  目的の表示ディレクトリ
        // 　　　　[in]isLeftWindow      左側のファイル一覧のときtrue
        // 　　　　[in]loadingGeneration ファイル一覧の世代
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus GetFileList(VirtualFolderInfo virtualFolder, string displayDirectory, bool isLeftWindow, int loadingGeneration) {
            FileOperationStatus status;
            string clipboardString = OSUtils.GetClipboardString();

            // SubPathExistFlagを作成
            //   user@server:/home/user/data/file1.tar.gz/dir1/file2.zip/dir2/
            //   subPath = dir1/file2.zip/dir2/
            SubPathExistFlag subPath = new SubPathExistFlag(displayDirectory, virtualFolder.MostInnerArchive.DisplayPathArchive);

            // 目的の一覧が取得できるか、エラーになるまでアーカイブを変えながらループ
            while (true) {
                VirtualFolderArchiveInfo virtualArchive = virtualFolder.MostInnerArchive;
                string realArcFile = virtualArchive.RealArchiveFile;
                string password = virtualArchive.Password;
                
                string tempPath = virtualFolder.TemporaryInfo.TemporaryDirectoryArchive;
                ArchiveAutoPasswordSetting autoPasswordSetting = Program.Document.UserSetting.ArchiveAutoPasswordSetting;
                ArchivePasswordSource passwordSource = new ArchivePasswordSource(autoPasswordSetting, realArcFile, clipboardString, password);
                IArchiveVirtualFileList archiveList = Program.Document.ArchiveFactory.CreateVirtualFileList(realArcFile, tempPath, passwordSource);
                try {
                    List<IFile> virtualFiles;
                    status = GetVirtualFileList(archiveList, virtualFolder, virtualArchive, displayDirectory, subPath, out virtualFiles);
                    if (status.Succeeded && virtualFiles != null) {
                        // 取得成功
                        IFileSystem fileSystem = Program.Document.FileSystemFactory.VirtualFileSystem;
                        m_fileList = VirtualFileList.CreateForExchangeFileSystem(fileSystem, displayDirectory, isLeftWindow, loadingGeneration, virtualFolder, virtualFiles);
                        return FileOperationStatus.Success;
                    } else if (status.Succeeded && virtualFiles == null) {
                        // 処理は成功したが目的のフォルダがない→途中のアーカイブを展開
                        string subVirtualPath;
                        int subVirtualIndex;
                        bool existSubVirtual = subPath.GetSubVirtualPath(out subVirtualPath, out subVirtualIndex);
                        if (!existSubVirtual) {
                            m_logger.LogFileOperationStart(FileOperationType.OpenArc, displayDirectory, false);
                            return m_logger.LogFileOperationEnd(FileOperationStatus.Fail);
                        }
                        string displayPath = virtualArchive.DisplayPathArchive + subVirtualPath;
                        status = ExtractInnerArchive(archiveList, subVirtualIndex, displayPath, virtualFolder);
                        if (!status.Succeeded) {
                            return status;
                        }
                    } else {
                        // 圧縮ファイルのオープン失敗などで処理が失敗した
                        return status;
                    }
                } finally {
                    archiveList.Dispose();
                }
            }
        }

        //=========================================================================================
        // 機　能：内部のアーカイブを展開する
        // 引　数：[in]archiveList    アーカイブからの一覧取得機能
        // 　　　　[in]subIndex       展開するアーカイブのファイルインデックス
        // 　　　　[in]dispSubPath    展開するアーカイブの表示用ディレクトリ
        // 　　　　[in]virtualFolder  仮想フォルダの情報（新しい位置を設定して戻る）
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus ExtractInnerArchive(IArchiveVirtualFileList archiveList, int subIndex, string dispSubPath, VirtualFolderInfo virtualFolder) {
            m_logger.LogFileOperationStart(FileOperationType.ExtractFile, dispSubPath, false);
            string tempRoot = virtualFolder.TemporaryInfo.TemporaryDirectoryArchive;
            string destTempFile = Program.Document.TemporaryManager.GetTemporaryFileInFolder(tempRoot, "File") + GenericFileStringUtils.GetExtensionAll(dispSubPath);

            FileOperationStatus status = archiveList.Extract(subIndex, destTempFile, m_logger.Progress);
            m_logger.LogFileOperationEnd(status);
            if (!status.Succeeded) {
                return status;
            }
            if (archiveList.UsedPasswordDisplayName != null) {
                string message = string.Format(Resources.Log_ExtractPassword, archiveList.UsedPasswordDisplayName);
                Program.LogWindow.RegistLogLineHelper(message);
            }
            virtualFolder.MostInnerArchive.Password = archiveList.UsedPassword;

            virtualFolder.AddVirtualArchive(VirtualFolderArchiveInfo.FromVirtualArchive(dispSubPath + "\\", destTempFile, null));
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ファイル一覧を取得する
        // 引　数：[in]archiveList    アーカイブからの一覧取得機能
        // 　　　　[in]virtualFolder  仮想フォルダの情報
        // 　　　　[in]directory      一覧を作成するディレクトリ
        // 　　　　[in]subPath        サブディレクトリの情報
        // 　　　　[out]resultList    結果のファイル一覧を返す変数（目的のフォルダがないときはnull）
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus GetVirtualFileList(IArchiveVirtualFileList archiveList, VirtualFolderInfo virtualFolder, VirtualFolderArchiveInfo virtualArchive, string directory, SubPathExistFlag subPath, out List<IFile> resultList) {
            FileOperationStatus status;
            resultList = null;

            // アーカイブを開く
            string innerArcName;
            string arcName = GenericFileStringUtils.RemoveLastDirectorySeparator(virtualArchive.RealArchiveFile);
            status = archiveList.Open(arcName, out innerArcName, m_logger);
            if (!status.Succeeded) {
                m_logger.LogFileOperationStart(FileOperationType.OpenArc, arcName, false);
                m_logger.LogFileOperationEnd(status);
                return status;
            }
            if (virtualArchive.InnerArchiveFile != null && innerArcName == null) {
                ;           // 内部アーカイブを元に開いたときは、さらにその内部アーカイブが戻ってこなくてもそのまま
            } else {
                virtualArchive.InnerArchiveFile = innerArcName;
            }

            // 一覧を取得
            string displayArchive = virtualFolder.MostInnerArchive.DisplayPathArchive;
            directory = GenericFileStringUtils.RemoveLastDirectorySeparator(directory);
            displayArchive = GenericFileStringUtils.RemoveLastDirectorySeparator(displayArchive);
            string targetInnerPath = directory.Substring(displayArchive.Length).Replace('/', '\\');         // アーカイブ中のこのパスを一覧取得
            if (targetInnerPath.StartsWith("\\")) {
                targetInnerPath = targetInnerPath.Substring(1);
            }
            string targetInnerPathLowEn = "\\" + targetInnerPath.ToLower() + "\\";

            UniqueVirtualFileArray fileList = new UniqueVirtualFileArray();

            // 親フォルダの情報
            if (targetInnerPath == "") {
                try {
                    FileInfo fileInfo = new FileInfo(archiveList.ArchiveFileName);
                    fileList.Add(new VirtualFile("..", fileInfo.LastWriteTime, fileInfo.Length, true));
                } catch (Exception) {
                    fileList.Add(new VirtualFile("..", DateTime.MinValue, 0, true));
                }
            } else {
                fileList.Add(null);                 // 親ディレクトリの場所を予約
            }

            // 対象となるファイルを検索
            VirtualFile parentInfo = null;
            int fileCount = archiveList.GetFileCount();
            for (int i = 0; i < fileCount; i++) {
                // ファイル一覧に取り込み
                IArchiveContentsFileInfo arcFileInfo = archiveList.GetFileInfo(i);
                string arcFilePathLow = arcFileInfo.FilePath.ToLower();         // 先頭と末尾に「\」なし
                string arcFilePathLowEn = "\\" + arcFilePathLow + "\\";
                bool isArcDir = arcFileInfo.IsDirectory;
                if (arcFilePathLowEn == targetInnerPathLowEn && isArcDir) {
                    parentInfo = new VirtualFile("..", arcFileInfo.LastWriteTime, 0, true);
                } else {
                    int lastEn = arcFilePathLow.LastIndexOf('\\');
                    if (lastEn != -1) {
                        // ルート以外の一覧
                        string arcFilePathParentEn = "\\" + arcFilePathLow.Substring(0, lastEn) + "\\";
                        if (arcFilePathParentEn == targetInnerPathLowEn) {
                            // 見つかった一覧
                            fileList.Add(new VirtualFile(arcFileInfo));
                        } else if (arcFilePathLowEn.StartsWith(targetInnerPathLowEn) && arcFilePathLowEn.Length > targetInnerPathLowEn.Length) {
                            // 突然現れた途中のパス
                            // \dir1\を検索中のとき、\dir1\dir2\file\が出現したらdir2をディレクトリとして記憶
                            string fileName = arcFilePathLowEn.Substring(targetInnerPathLowEn.Length);
                            fileName = GenericFileStringUtils.FirstFolder(fileName);
                            fileList.Add(new VirtualFile(fileName, DateTime.MinValue, 0, true));
                        } else if (targetInnerPath == "") {
                            // 突然現れたルートのパス
                            // \\を検索中のとき、\dir1\dir2\file\が出現したらdir1をディレクトリとして記憶
                            string fileName = arcFilePathLowEn.Substring(1);
                            fileName = GenericFileStringUtils.FirstFolder(fileName);
                            fileList.Add(new VirtualFile(fileName, DateTime.MinValue, 0, true));
                        }
                    } else if (targetInnerPath == "") {
                        // ルートの一覧
                        fileList.Add(new VirtualFile(arcFileInfo));
                    }
                }

                // 存在位置を記憶
                subPath.SetExistContents(arcFilePathLowEn, isArcDir, i);
            }

            // 結果を変換
            resultList = new List<IFile>();
            foreach (VirtualFile file in fileList.FileList) {
                resultList.Add(file);
            }

            // 親ディレクトリ未設定の場合、設定
            if (resultList[0] == null) {
                if (parentInfo == null && resultList.Count == 1) {          // 目的のフォルダがない
                    resultList = null;
                } else if (parentInfo == null) {                            // 通常ないが、親だけが見つからなかった→回避
                    resultList[0] = new VirtualFile("..", DateTime.MinValue, 0, true);
                } else {                                                    // 成功
                    resultList[0] = parentInfo;
                }
            }

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：処理中に発生した例外またはエラーを処理する
        // 引　数：[in]aarg  リクエスト/レスポンス
        // 　　　　[in]e     例外（エラー通知の場合はnull）
        // 戻り値：なし
        //=========================================================================================
        public override void ExecuteOnException(AbstractProcedureArg aarg, Exception e) {
            GetVirtualUIFileListArg arg = (GetVirtualUIFileListArg)aarg;
            if (aarg.Status == FileOperationStatus.Canceled) {
                arg.Status = FileOperationStatus.Canceled;
            } else if (e != null) {
                arg.Status = FileOperationStatus.Fail;
            } else if (arg.Status.Succeeded) {
                arg.Status = FileOperationStatus.Fail;
            }
            NotifyTaskEnd(arg, null, m_nextVirtualFolder);
        }

        //=========================================================================================
        // 機　能：一覧取得タスクをUIに通知する
        // 引　数：[in]arg                リクエスト/レスポンス
        // 　　　　[in]fileList           次に使用するファイル一覧（エラーのときnull）
        // 　　　　[in]nextVirtualFolder  次に使用する仮想フォルダ情報（エラーのとき処理途中の情報）
        // 戻り値：なし
        //=========================================================================================
        public static void NotifyTaskEnd(GetVirtualUIFileListArg arg, IFileList fileList, VirtualFolderInfo nextVirtualFolder) {
            // メインスレッドにdelegateして一覧を切り替え
            BaseThread.InvokeProcedureByMainThread(new NotifyTaskEndDelegate(NotifyTaskEndUI), arg.IsLeftWindow, arg.Status, fileList, nextVirtualFolder, arg.ChangeDirectoryMode);
        }
        private delegate void NotifyTaskEndDelegate(bool isLeftWindow, FileOperationStatus status, IFileList fileList, VirtualFolderInfo nextVirtualFolder, ChangeDirectoryParam chdirMode);
        private static void NotifyTaskEndUI(bool isLeftWindow, FileOperationStatus status, IFileList fileList, VirtualFolderInfo nextVirtualFolder, ChangeDirectoryParam chdirMode) {
            if (isLeftWindow) {
                Program.MainWindow.LeftFileListView.OnNotifyUIFileListTaskEnd(status, fileList, nextVirtualFolder, chdirMode);
            } else {
                Program.MainWindow.RightFileListView.OnNotifyUIFileListTaskEnd(status, fileList, nextVirtualFolder, chdirMode);
            }
        }
    }
}
