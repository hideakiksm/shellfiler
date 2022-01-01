using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Properties;
using ShellFiler.Virtual;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.TerminalSession.CommandEmulator;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;

namespace ShellFiler.FileSystem.Shell {

    //=========================================================================================
    // クラス：SFTPのlsコマンドによりUI用のファイル一覧をバックグラウンドで取得するプロシージャ
    //=========================================================================================
    class ShellGetUIFileListProcedure : AbstractShellBackgroundProcedure {

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in,out]arg    リクエスト/レスポンス
        // 戻り値：ステータスコード
        //=========================================================================================
        public override FileOperationStatus Execute(AbstractProcedureArg aarg) {
            ShellGetUIFileListArg arg = (ShellGetUIFileListArg)aarg;
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
        private FileOperationStatus ExecuteRequest(SSHConnection connection, ShellGetUIFileListArg arg, out IFileList resultFileList, out VirtualFolderInfo resultVirtualFolder) {
            resultFileList = null;
            resultVirtualFolder = null;
            FileOperationStatus status;

            // 準備
            ShellProcedureControler.InitializeMode initMode = ShellProcedureControler.InitializeMode.GenericOperation;
            ChangeDirectoryParam chdirMode = arg.ChangeDirectoryMode;
            if (chdirMode is ChangeDirectoryParam.DirectSshShell) {
                bool newChannel = ((ChangeDirectoryParam.DirectSshShell)chdirMode).NewChannel;
                if (newChannel) {
                    initMode = ShellProcedureControler.InitializeMode.LoginWithNewChannel;
                } else {
                    initMode = ShellProcedureControler.InitializeMode.LoginWithExistingChannel;
                }
            }
            ShellProcedureControler controler = new ShellProcedureControler(connection, arg.RequestContext);
            status = controler.Initialize(arg.Directory, true, initMode);
            if (!status.Succeeded) {
                return status;
            }
            string directory = SSHUtils.CompleteShellDirectory(controler.TerminalChannel, arg.Directory);           // 接続後に補完
            resultFileList = arg.FileList;

            // ユーザー変更
            if (chdirMode is ChangeDirectoryParam.SSHChangeUser) {
                // ユーザーを変更
                ChangeDirectoryParam.SSHChangeUser chdirUser = (ChangeDirectoryParam.SSHChangeUser)chdirMode;
                if (chdirUser.ChangeUserInfo.ChangeMode == SSHChangeUserInfo.ChangeUserMode.Su) {
                    status = ChangeUser(controler, chdirUser);
                    if (!status.Succeeded) {
                        return FileOperationStatus.FailedChangeUser;
                    }
                } else if (chdirUser.ChangeUserInfo.ChangeMode == SSHChangeUserInfo.ChangeUserMode.Exit) {
                    status = ExitUser(controler);
                    if (!status.Succeeded) {
                        return FileOperationStatus.FailedExit;
                    }
                } else {
                    Program.Abort("ユーザー変更のモードが想定外です。");
                    return FileOperationStatus.Fail;
                }

                // カレントを取得
                status = GetCurrentDirectory(controler, ref directory);
                if (!status.Succeeded) {
                    return status;
                }
            }

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
            IFile srcFileInfo;
            status = GetFileInfo(controler, local, out srcFileInfo);
            if (controler.IsCanceled) {
                return FileOperationStatus.Canceled;
            }
            bool isDir = srcFileInfo.Attribute.IsDirectory;

            if (status == FileOperationStatus.FileNotFound || (status.Succeeded && !isDir)) {
                // ないか、ファイルなら仮想ディレクトリとして処理
                IFileList virtualFileList;
                status = GetFileListFromArchiveFile(controler, directory, local, chdirMode, srcFileInfo, (ShellFileList)resultFileList, out virtualFileList, out resultVirtualFolder);
                if (!status.Succeeded) {
                    return status;
                }
                resultFileList = virtualFileList;
            } else if (status.Succeeded) {
                // ディレクトリならlsを実行
                status = GetFileListFromDirectory(controler, directory, local, (ShellFileList)resultFileList);
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
        // 引　数：[in]controler  SSH接続のコントローラ
        // 　　　　[in]path       調べるディレクトリ（ローカルパス、最後は「/」なし）
        // 　　　　[out]fileInfo  ファイルの属性の取得結果を返す変数
        // 戻り値：ステータス（ファイルがない場合はFileNotFound）
        //=========================================================================================
        private FileOperationStatus GetFileInfo(ShellProcedureControler controler, string path, out IFile fileInfo) {
            FileOperationStatus status;
            fileInfo = null;

            // 属性を取得
            ShellCommandEmulator emulator = controler.TerminalChannel.ShellCommandEmulator;
            ShellEngineGetFileInfo engine = new ShellEngineGetFileInfo(emulator, controler.Connection, path);
            status = emulator.Execute(controler.Context, engine);
            if (!status.Succeeded) {
                return status;
            }
            if (engine.ResultFileInfo == null) {
                return FileOperationStatus.FileNotFound;
            }
            fileInfo = engine.ResultFileInfo;

            // シンボリックリンクを解決
            if (fileInfo.Attribute.IsSymbolicLink) {
                string baseDir = GenericFileStringUtils.GetDirectoryName(path);
                List<IFile> fileInfoList = new List<IFile>();
                fileInfoList.Add(fileInfo);
                status = ShellSymbolicLinkResolver.SetSymbolicLinkFileList(controler, baseDir, fileInfoList);
                if (!status.Succeeded) {
                    return status;
                }
            }

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：指定ディレクトリ内のファイル一覧を返す
        // 引　数：[in]controler         SSH接続のコントローラ
        // 　　　　[in]directory         調べるディレクトリ（ShellFiler形式）
        // 　　　　[in]localPath         調べるディレクトリ（ローカルパス）
        // 　　　　[out]fileList         取得したファイル一覧を返す変数
        // 　　　　[out]volumeInfo       取得したボリューム情報を返す変数
        // 　　　　[out]fileListContext  ファイル一覧に応じたコンテキスト情報を返す変数
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus GetFileListFromDirectory(ShellProcedureControler controler, string directory, string localPath, ShellFileList sshFileList) {
            FileOperationStatus status;
            sshFileList.Files = null;
            sshFileList.DirectoryName = directory;

            // ファイル一覧を取得
            localPath = GenericFileStringUtils.CompleteDirectoryName(localPath, "/");
            ShellCommandEmulator emulator = controler.TerminalChannel.ShellCommandEmulator;
            ShellEngineGetFileList getFileListEngine = new ShellEngineGetFileList(emulator, controler.Connection, localPath);
            status = emulator.Execute(controler.Context, getFileListEngine);
            if (!status.Succeeded) {
                return status;
            }
            List<IFile> fileList = getFileListEngine.ResultFileList;
            status = ShellSymbolicLinkResolver.SetSymbolicLinkFileList(controler, localPath, fileList);
            if (!status.Succeeded) {
                return status;
            }

            // ボリューム情報を取得
            VolumeInfo volumeInfo;
            ShellEngineGetVolumeInfo getVolumeInfoEngine = new ShellEngineGetVolumeInfo(emulator, controler.Connection, localPath);
            status = emulator.Execute(controler.Context, getVolumeInfoEngine);
            if (status.Succeeded) {
                volumeInfo = getVolumeInfoEngine.ResultVolumeInfo;
            } else {
                volumeInfo = null;
            }

            // 結果を返す
            ShellCommandDictionary commandDic = controler.Connection.ShellCommandDictionary;
            Encoding encoding = controler.Connection.AuthenticateSetting.Encoding;
            TerminalShellChannelID shellId = controler.TerminalChannel.ID;
            sshFileList.Files = fileList;
            sshFileList.VolumeInfo = volumeInfo;
            sshFileList.FileListContext = new ShellFileListContext(commandDic, encoding, shellId);

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：アーカイブから一覧を取得する
        // 引　数：[in]controler          SSH接続のコントローラ
        // 　　　　[in]directory          調べるディレクトリ（ユーザ名＋サーバ名＋ローカルパス）
        // 　　　　[in]path               調べるディレクトリ（SSH内ローカルパス）
        // 　　　　[in]chdirMode          フォルダ変更のパラメータ
        // 　　　　[in]srcFileInfo        転送元ファイルの情報
        // 　　　　[in]sshFileList        リクエストされたSSHのファイル一覧情報
        // 　　　　[out]virtualFileList   仮想フォルダのファイル一覧情報を返す変数
        // 　　　　[out]virtualFolderInfo 新しい一覧の仮想フォルダ情報を返す変数
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus GetFileListFromArchiveFile(ShellProcedureControler controler, string directory, string path, ChangeDirectoryParam chdirMode, IFile srcFileInfo, ShellFileList shellFileList, out IFileList virtualFileList, out VirtualFolderInfo virtualFolderInfo) {
            FileOperationStatus status = FileOperationStatus.Fail;
            virtualFileList = null;
            virtualFolderInfo = null;

            // 対象ディレクトリを分解
            string[] subDirList = GenericFileStringUtils.SplitSubDirectoryList(path);

            // 後ろのパス区切りから順にアーカイブの存在チェック
            string arcPath = null;
            for (int i = subDirList.Length - 1; i >= 1; i--) {
                string checkArcPath = StringUtils.CombineStringArray(subDirList, 0, i + 1, "/");
                IFile file;
                status = GetFileInfo(controler, checkArcPath, out file);
                if (status == FileOperationStatus.FileNotFound) {
                    ;       // 見つからない場合は次へ
                } else if (!status.Succeeded) {
                    return status;
                } else if (!file.Attribute.IsDirectory) {
                    arcPath = checkArcPath;
                    srcFileInfo = file;
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
            VirtualFolderInfo virtualFolder = new VirtualFolderInfo(Program.Document.FileSystemFactory.ShellFileSystem, tempDir, shellFileList.FileListContext);
            AutoTaskLogger logger = new AutoTaskLogger();
            try {
                logger.LogFileOperationStart(FileOperationType.Download, GenericFileStringUtils.GetFileName(arcPath), false);

                string tempRoot = virtualFolder.TemporaryInfo.TemporaryDirectoryArchive;
                string loadedArcFile = Program.Document.TemporaryManager.GetTemporaryFileInFolder(tempRoot, "File") + GenericFileStringUtils.GetExtensionAll(arcPath);

                SSHProtocolType protocol;
                string userServer;
                SSHUtils.GetUserServerPart(directory, out protocol, out userServer);
                string arcDisplayPath = protocol.FolderProtocol + ":" + userServer + ":" + arcPath;
                ShellTransferDownloadProcedure downProc = new ShellTransferDownloadProcedure(controler.Connection, controler.Context);
                status = downProc.Execute(arcDisplayPath, loadedArcFile, srcFileInfo, logger.Progress);
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
                GetVirtualUIFileListArg arg = new GetVirtualUIFileListArg(controler.Context, virtualFolder, directory, shellFileList.IsLeftWindow, chdirMode, shellFileList.LoadingGeneration);
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
        // 機　能：ディレクトリを取得する
        // 引　数：[in]controler      SSH接続のコントローラ
        // 　　　　[in,out]directory  現在のディレクトリ（パス部分をSSHから取得して返す）
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus GetCurrentDirectory(ShellProcedureControler controler, ref string directory) {
            FileOperationStatus status;

            // フルパスディレクトリを分解
            SSHProtocolType protocol;
            string user, server, local;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(directory, out protocol, out user, out server, out portNo, out local);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // ディレクトリを取得
            ShellCommandEmulator emulator = controler.TerminalChannel.ShellCommandEmulator;
            ShellEngineGetCurrentDirectory engine = new ShellEngineGetCurrentDirectory(emulator, controler.Connection);
            status = emulator.Execute(controler.Context, engine);
            if (!status.Succeeded) {
                return status;
            }
            local = engine.CurrentDirectory;

            // 元の形式に整形
            directory = SSHUtils.CreateUserServer(protocol, user, server, portNo) + ":" + local;

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ユーザーを切り替える
        // 引　数：[in]controler  SSH接続のコントローラ
        // 　　　　[in]chdirUser  ユーザー変更の条件
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus ChangeUser(ShellProcedureControler controler, ChangeDirectoryParam.SSHChangeUser chdirUser) {
            FileOperationStatus status;
            ShellCommandDictionary dictionary = controler.Connection.ShellCommandDictionary;
            ShellCommandEmulator emulator = controler.TerminalChannel.ShellCommandEmulator;
            Encoding encoding = controler.Connection.AuthenticateSetting.Encoding;
            SSHChangeUserInfo changeUserInfo = chdirUser.ChangeUserInfo;

            string strCommand = dictionary.GetChangeLoginUserCommand(changeUserInfo.UserName, changeUserInfo.UseLoginShell);
            List<OSSpecLineExpect> expect = dictionary.ExpectChangeLoginUser;
            ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, controler.Connection, strCommand, expect);

            ShellEngineInternalExecute.RemainDataHookDelegate dataHook = delegate(List<ResultLineParser.ParsedLine> parsedLine, ref bool completed, ref byte[] remain) {
                if (!changeUserInfo.IsSuperUserNow) {
                    // 一般ユーザーからの変更
                    // 「Password:」に続いてパスワードとプロンプト変更を送信する
                    if (remain.Length == 0) {
                        return ShellEngineError.Success();
                    }
                    string[] expectPassword = dictionary.ValueChangeUserPrompt;
                    for (int i = 0; i < expectPassword.Length; i++) {
                        byte[] expectBytes = encoding.GetBytes(expectPassword[i]);
                        bool same = ArrayUtils.CompareByteArray(expectBytes, 0, expectBytes.Length, remain, 0, remain.Length);
                        if (!same) {
                            continue;
                        }
                        // パスワード+LF+プロンプト変更コマンドを入力
                        string strPromptCommand = dictionary.GetCommandChangePrompt(emulator.PromptStringSend);
                        byte[] password = encoding.GetBytes(changeUserInfo.Password + emulator.ReturnCharSend + strPromptCommand + emulator.ReturnCharSend);
                        engine.SendData(password);
                        remain = new byte[0];
                        engine.SetDataHook(null);
                        break;
                    }
                    return ShellEngineError.Success();
                } else {
                    // スーパーユーザーからの変更
                    // プロンプトがなくてもLF+プロンプト変更コマンドを入力
                    string strPromptCommand = dictionary.GetCommandChangePrompt(emulator.PromptStringSend);
                    byte[] password = encoding.GetBytes(emulator.ReturnCharSend + strPromptCommand + emulator.ReturnCharSend);
                    engine.SendData(password);
                    engine.SetDataHook(null);
                    return ShellEngineError.Success();
                }                        
            };

            // 入力処理を実行
            engine.SetDataHook(dataHook); 
            status = emulator.Execute(controler.Context, engine);
            if (!status.Succeeded) {
                return status;
            }

            return FileOperationStatus.Success;
        }
        
        //=========================================================================================
        // 機　能：現在の操作ユーザーから抜ける
        // 引　数：[in]controler  SSH接続のコントローラ
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus ExitUser(ShellProcedureControler controler) {
            FileOperationStatus status;
            ShellCommandDictionary dictionary = controler.Connection.ShellCommandDictionary;
            ShellCommandEmulator emulator = controler.TerminalChannel.ShellCommandEmulator;
            Encoding encoding = controler.Connection.AuthenticateSetting.Encoding;

            string strCommand = dictionary.GetExitCommand();
            List<OSSpecLineExpect> expect = dictionary.ExpectExit;
            ShellEngineInternalExecute engine = new ShellEngineInternalExecute(emulator, controler.Connection, strCommand, expect);

            ShellEngineInternalExecute.RemainDataHookDelegate dataHook = delegate(List<ResultLineParser.ParsedLine> parsedLine, ref bool completed, ref byte[] remain) {
                // プロンプト変更コマンドを送信
                string strPromptCommand = dictionary.GetCommandChangePrompt(emulator.PromptStringSend);
                byte[] bytesComand = encoding.GetBytes(strPromptCommand + emulator.ReturnCharSend);
                engine.SendData(bytesComand);
                return ShellEngineError.Success();
            };
            
            status = emulator.Execute(controler.Context, engine);
            if (!status.Succeeded) {
                return status;
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
            ShellGetUIFileListArg arg = (ShellGetUIFileListArg)aarg;
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
