using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Properties;
using ShellFiler.Virtual;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：SFTPのlsコマンドによりUI用のファイル一覧をバックグラウンドで取得するプロシージャ
    //=========================================================================================
    class SFTPGetUIFileListProcedure : AbstractSFTPBackgroundProcedure {

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in,out]arg    リクエスト/レスポンス
        // 戻り値：ステータスコード
        //=========================================================================================
        public override FileOperationStatus Execute(AbstractProcedureArg aarg) {
            SFTPGetUIFileListArg arg = (SFTPGetUIFileListArg)aarg;
            IFileList resultFileList;
            VirtualFolderInfo resultVirtualFolder;
            FileOperationStatus status = ExecuteRequest(SSHConnection, arg, out resultFileList, out resultVirtualFolder);
            if (SSHConnection.Closed) {
                status = FileOperationStatus.Canceled;
            }
            arg.Status = status;
            if (status.Succeeded) {
                NotifyTaskEnd(arg.FileList.IsLeftWindow, status, resultFileList, resultVirtualFolder, arg.ChangeDirectoryMode);
            }
            return status;
        }

        //=========================================================================================
        // 機　能：コマンドを実際に実行する
        // 引　数：[in]connection           接続
        // 　　　　[in]arg                  リクエスト/レスポンス
        // 　　　　[out]resultFileList      結果のファイル一覧
        // 　　　　[out]resultVirtualFolder 結果として使用する仮想フォルダ情報を返す変数（仮想フォルダを利用しないときnull）
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus ExecuteRequest(SSHConnection connection, SFTPGetUIFileListArg arg, out IFileList resultFileList, out VirtualFolderInfo resultVirtualFolder) {
            FileOperationStatus status;
            resultFileList = null;
            resultVirtualFolder = null;

            // 準備
            SFTPProcedureControler controler = new SFTPProcedureControler(connection, arg.RequestContext);
            status = controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }
            string directory = SSHUtils.CompleteSFTPDirectory(connection, arg.Directory);           // 接続後に補完
            resultFileList = arg.FileList;
            ChangeDirectoryParam chdirMode = arg.ChangeDirectoryMode;

            // フルパスディレクトリを分解
            SSHProtocolType protocol;
            string user, server, local;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(directory, out protocol, out user, out server, out portNo, out local);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }
            if (local != "/" && local.EndsWith("/")) {
                local = local.Substring(0, local.Length - 1);
            }

            // 指定ディレクトリが存在するか確認
            bool isDir;
            status = IsDirectory(controler, local, out isDir);
            if (controler.IsCanceled) {
                return FileOperationStatus.Canceled;
            }
            if (status == FileOperationStatus.FileNotFound || (status.Succeeded && !isDir)) {
                // ないか、ファイルなら仮想ディレクトリとして処理
                IFileList virtualFileList;
                status = GetFileListFromArchiveFile(controler, directory, local, chdirMode, (SFTPFileList)resultFileList, out virtualFileList, out resultVirtualFolder);
                if (!status.Succeeded) {
                    return status;
                }
                resultFileList = virtualFileList;
            } else if (status.Succeeded) {
                // ディレクトリならlsを実行
                status = GetFileListFromDirectory(controler, directory, (SFTPFileList)resultFileList);
                if (!status.Succeeded) {
                    return status;
                }
                connection.PreviousDirectory = arg.Directory;
            } else {
                // 確認に失敗
                return status;
            }
            return status;
        }

        //=========================================================================================
        // 機　能：指定ディレクトリが存在するかどうかを返す
        // 引　数：[in]controler SSH接続のコントローラ
        // 　　　　[in]path      調べるディレクトリ（ローカルパス）
        // 　　　　[out]isDir    ディレクトリのときtrueを返す変数
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus IsDirectory(SFTPProcedureControler controler, string path, out bool isDir) {
            isDir = false;

            // 属性を取得
            try {
                ChannelSftp channel = controler.Context.SFTPRequestContext.GetChannelSftp(controler.Connection);
                SftpATTRS attrs = channel.stat(path);
                isDir = attrs.isDir();
                return FileOperationStatus.Success;
            } catch (SftpException) {
                return FileOperationStatus.FileNotFound;
            } catch (Exception e) {
                return controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
        }

        //=========================================================================================
        // 機　能：指定ディレクトリ内のファイル一覧を返す
        // 引　数：[in]controler         SSH接続のコントローラ
        // 　　　　[in]directory         調べるディレクトリ
        // 　　　　[out]fileList         取得したファイル一覧を返す変数
        // 　　　　[out]volumeInfo       取得したボリューム情報を返す変数
        // 　　　　[out]fileListContext  ファイル一覧に応じたコンテキスト情報を返す変数
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus GetFileListFromDirectory(SFTPProcedureControler controler, string directory, SFTPFileList sshFileList) {
            FileOperationStatus status;
            sshFileList.Files = null;
            sshFileList.DirectoryName = directory;

            // ファイル一覧を取得
            List<IFile> fileList;
            status = ChannelLsHelper.ExecLs(controler, directory, out fileList);
            if (!status.Succeeded) {
                return status;
            }

            // 成功時はファイル一覧に設定、実行はUIからなのでキャッシュしない
            // コネクションが切れたときは終了（App終了時のみ切れるはずなので、通知しない）
            if (controler.Connection.Closed) {
                return FileOperationStatus.Canceled;
            }

            // ボリューム情報を取得
            VolumeInfo volumeInfo;
            SFTPGetVolumeInfoProcedure procedure = new SFTPGetVolumeInfoProcedure(controler.Connection, controler.Context);
            status = procedure.Execute(directory, out volumeInfo);
            if (!status.Succeeded) {
                return status;
            }

            sshFileList.Files = fileList;
            sshFileList.VolumeInfo = volumeInfo;
            sshFileList.FileListContext = new SFTPFileListContext(controler.Connection.ShellCommandDictionary, controler.Connection.AuthenticateSetting.Encoding);

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：アーカイブから一覧を取得する
        // 引　数：[in]controler          SSH接続のコントローラ
        // 　　　　[in]directory          調べるディレクトリ（ユーザ名＋サーバ名＋ローカルパス）
        // 　　　　[in]path               調べるディレクトリ（SSH内ローカルパス）
        // 　　　　[in]chdirMode          フォルダ変更のパラメータ
        // 　　　　[in]sshFileList        リクエストされたSSHのファイル一覧情報
        // 　　　　[out]virtualFileList   仮想フォルダのファイル一覧情報を返す変数
        // 　　　　[out]virtualFolderInfo 新しい一覧の仮想フォルダ情報を返す変数
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus GetFileListFromArchiveFile(SFTPProcedureControler controler, string directory, string path, ChangeDirectoryParam chdirMode, SFTPFileList sshFileList, out IFileList virtualFileList, out VirtualFolderInfo virtualFolderInfo) {
            FileOperationStatus status = FileOperationStatus.Fail;
            virtualFileList = null;
            virtualFolderInfo = null;

            // 対象ディレクトリを分解
            string[] subDirList = GenericFileStringUtils.SplitSubDirectoryList(path);

            // 後ろのパス区切りから順にアーカイブの存在チェック
            string arcPath = null;
            for (int i = subDirList.Length - 1; i >= 1; i--) {
                string checkArcPath = StringUtils.CombineStringArray(subDirList, 0, i + 1, "/");
                bool isDir;
                status = IsDirectory(controler, checkArcPath, out isDir);
                if (status == FileOperationStatus.FileNotFound) {
                    ;       // 見つからない場合は次へ
                } else if (!status.Succeeded) {
                    return status;
                } else if (!isDir) {
                    arcPath = checkArcPath;
                    break;
                } else {
                    return FileOperationStatus.Fail;
                }
            }
            if (arcPath == null) {
                return FileOperationStatus.Fail;
            }

            // アーカイブを確認
            if (!Program.Document.ArchiveFactory.IsSupportFileList()) {
                return FileOperationStatus.Fail;
            }

            // アーカイブをローカルにダウンロード
            VirtualFolderTemporaryDirectory tempDir = Program.Document.TemporaryManager.VirtualManager.CreateVirtualFolder();
            VirtualFolderInfo virtualFolder = new VirtualFolderInfo(Program.Document.FileSystemFactory.SFTPFileSystem, tempDir, sshFileList.FileListContext);
            AutoTaskLogger logger = new AutoTaskLogger();
            try {
                logger.LogFileOperationStart(FileOperationType.Download, GenericFileStringUtils.GetFileName(arcPath), false);

                string tempRoot = virtualFolder.TemporaryInfo.TemporaryDirectoryArchive;
                string loadedArcFile = Program.Document.TemporaryManager.GetTemporaryFileInFolder(tempRoot, "File") + GenericFileStringUtils.GetExtensionAll(arcPath);

                SSHProtocolType protocol;
                string userServer;
                SSHUtils.GetUserServerPart(directory, out protocol, out userServer);
                string arcDisplayPath = protocol.FolderProtocol + ":" + userServer + ":" + arcPath;
                SFTPDownloadFileProcedure downProc = new SFTPDownloadFileProcedure(controler.Connection, controler.Context);
                status = downProc.Execute(controler.Context, arcDisplayPath, loadedArcFile, logger.Progress);
                if (status == FileOperationStatus.SuccessCopy) {
                    status = FileOperationStatus.SuccessDownload;
                }
                logger.LogFileOperationEnd(status);

                if (!status.Succeeded) {
                    return status;
                }

                // 次にアクセスする仮想フォルダを決定
                Program.Document.TemporaryManager.VirtualManager.BeginUsingVirtualFolder(virtualFolder, VirtualFolderTemporaryDirectory.UsingType.FileListLoading);
                VirtualFolderArchiveInfo virtualArchive = VirtualFolderArchiveInfo.FromSSHArchive(arcDisplayPath + "/", loadedArcFile);
                virtualFolder.AddVirtualArchive(virtualArchive);

                // リクエスト
                GetVirtualUIFileListProcedure procGetVirtualList = new GetVirtualUIFileListProcedure();
                GetVirtualUIFileListArg arg = new GetVirtualUIFileListArg(controler.Context, virtualFolder, directory, sshFileList.IsLeftWindow, chdirMode, sshFileList.LoadingGeneration);
                VirtualFileList fileList;
                status = procGetVirtualList.ExecuteSubProcess(arg, logger, out fileList, out virtualFolderInfo);
                virtualFileList = fileList;
                if (!status.Succeeded) {
                    return status;
                }
            } finally {
                if (logger.LogCount > 0) {
                    LogLineSimple log;
                    if (status.Succeeded) {
                        log = new LogLineSimple(Resources.Log_VirtualFolderSuccess);
                    } else {
                        log = new LogLineSimple(LogColor.Error, Resources.Log_VirtualFolderFailed);
                    }
                    Program.LogWindow.RegistLogLine(log);
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
            SFTPGetUIFileListArg arg = (SFTPGetUIFileListArg)aarg;
            if (aarg.Status == FileOperationStatus.Canceled) {
                arg.Status = FileOperationStatus.Canceled;
            } else if (e != null) {
                arg.Status = FileOperationStatus.Fail;
            } else if (arg.Status.Succeeded) {
                arg.Status = FileOperationStatus.Fail;
            }
            NotifyTaskEnd(arg.FileList.IsLeftWindow, arg.Status, arg.FileList, null, arg.ChangeDirectoryMode);
        }

        //=========================================================================================
        // 機　能：一覧取得タスクをUIに通知する
        // 引　数：[in]isLeftWindow       左側のウィンドウに対して通知するときtrue
        // 　　　　[in]status             一覧取得のステータス
        // 　　　　[in]fileList           ファイル一覧（失敗のときはnull）
        // 　　　　[in]nextVirtualFolder  次に使用する仮想フォルダ情報（エラーのとき処理途中の情報、仮想dir以外はnull）
        // 　　　　[in]chdirMode          ディレクトリ変更のパラメータ
        // 戻り値：なし
        //=========================================================================================
        public static void NotifyTaskEnd(bool isLeftWindow, FileOperationStatus status, IFileList fileList, VirtualFolderInfo nextVirtualFolder, ChangeDirectoryParam chdirMode) {
            // メインスレッドにdelegateして一覧を切り替え
            BaseThread.InvokeProcedureByMainThread(new NotifyTaskEndDelegate(NotifyTaskEndUI), isLeftWindow, status, fileList, nextVirtualFolder, chdirMode);
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
