using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.UI.Log;
using ShellFiler.Virtual;

namespace ShellFiler.FileSystem.Transfer {

    //=========================================================================================
    // クラス：ダウンロードとアップロードによるファイル操作API
    //=========================================================================================
    class DownloadUploadFileSystem : IFileSystemToFileSystem {
        // 転送元のファイルシステム
        private IFileSystem m_srcFileSystem;

        // 転送先のファイルシステム
        private IFileSystem m_destFileSystem;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcFileSystem     転送元のファイルシステム
        // 　　　　[in]destFileSystem    転送先のファイルシステム
        // 戻り値：なし
        //=========================================================================================
        public DownloadUploadFileSystem(IFileSystem srcFileSystem, IFileSystem destFileSystem) {
            m_srcFileSystem = srcFileSystem;
            m_destFileSystem = destFileSystem;
        }

        //=========================================================================================
        // 機　能：ファイルをコピーする
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]srcFileInfoAttr 属性コピーを行うとき、srcFilePathの属性（まだ取得できていないときnull）
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]overwrite       上書きするときtrue
        // 　　　　[in]attrMode        属性をコピーするかどうかの設定（属性をコピーしないときnull）
        // 　　　　[in]fileFilter      転送時に使用するフィルター（使用しないときはnull）
        // 　　　　[in]progress        進捗状態を通知するdelegate
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus CopyFile(FileOperationRequestContext context, string srcFilePath, IFile srcFileInfoAttr, string destFilePath, bool overwrite, AttributeSetMode attrMode, FileFilterTransferSetting fileFilter, FileProgressEventHandler progress) {
            FileOperationStatus status = Transfer(context, TransferModeType.CopyFile, srcFilePath, srcFileInfoAttr, destFilePath, overwrite, attrMode, fileFilter, progress);
            return status;
        }

        //=========================================================================================
        // 機　能：ファイルをコピーまたは移動する
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]transferType    転送（コピー/移動）の種類
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]srcFileInfoAttr 属性コピーを行うとき、srcFilePathの属性（まだ取得できていないときnull）
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]overwrite       上書きするときtrue
        // 　　　　[in]attrMode        属性をコピーするかどうかの設定（属性をコピーしないときnull）
        // 　　　　[in]fileFilter      転送時に使用するフィルター（使用しないときはnull）
        // 　　　　[in]progress        進捗状態を通知するdelegate
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Transfer(FileOperationRequestContext context, TransferModeType transferType, string srcFilePath, IFile srcFileInfoAttr, string destFilePath, bool overwrite, AttributeSetMode attrMode, FileFilterTransferSetting fileFilter, FileProgressEventHandler progress) {
            FileOperationStatus status;

            // 転送条件を確認
            string srcToPath;               // ダウンロード先、フィルタリング元
            string destFromPath;            // フィルタリング先、アップロード元
            bool baseAttr;                  // 基本属性を転送するときtrue
            bool allAttr;                   // すべての属性を転送するときtrue
            TempFileHolder tempHolder = new TempFileHolder();
            CheckTransferCondition((fileFilter != null), srcFilePath, destFilePath, attrMode, tempHolder, out srcToPath, out destFromPath, out baseAttr, out allAttr, progress);

            // 属性を取得
            if (srcFileInfoAttr == null) {
                // 属性転送予定のときか、SSHシェルのとき（転送量計算でファイルサイズが必要）、取得
                if ((baseAttr || allAttr) || (m_srcFileSystem.FileSystemId == FileSystemID.SSHShell)) {
                    status = m_srcFileSystem.GetFileInfo(context, srcFilePath, true, out srcFileInfoAttr);
                    if (!status.Succeeded) {
                        return status;
                    }
                }
            }

            // 展開＋アップロード
            try  {
                // 上書き確認
                if (!overwrite) {
                    bool exist;
                    status = m_destFileSystem.CheckFileExist(context, destFilePath, false, null, out exist);
                    if (!status.Succeeded) {
                        return status;
                    }
                    if (exist) {
                        return FileOperationStatus.AlreadyExists;
                    }
                }

                // 転送元を標準位置に取得
                if (!FileSystemID.IsWindows(m_srcFileSystem.FileSystemId)) {
                    status = m_srcFileSystem.TransferDownload(context, srcFilePath, destFilePath, srcToPath, srcFileInfoAttr, progress);
                    if (!status.Succeeded || status.Skipped) {
                        return status;
                    }
                }

                // フィルターを適用
                if (fileFilter != null) {
                    status = FileFilterLogic.ApplyFileFilter(srcFilePath, srcToPath, destFromPath, fileFilter, context.CancelEvent);
                    if (status != FileOperationStatus.Success) {
                        return status;      // Skipの場合はここで終わる
                    }
                }

                // 転送先を確定
                if (!FileSystemID.IsWindows(m_destFileSystem.FileSystemId)) {
                    status = m_destFileSystem.TransferUpload(context, srcFilePath, destFilePath, destFromPath, progress);
                    if (!status.Succeeded || status.Skipped) {
                        return status;
                    }
                }

                // 転送元を削除
                if (transferType == TransferModeType.MoveFile) {
                    DeleteFileFolderFlag flag = DeleteFileFolderFlag.FILE | DeleteFileFolderFlag.CHANGE_ATTR;
                    status = m_srcFileSystem.DeleteFileFolder(context, srcFilePath, true, flag);
                    if (!status.Succeeded) {
                        // 失敗時は巻き戻ししない
                        return status;
                    }
                }

                // 属性を設定
                if (baseAttr || allAttr) {
                    IFile destAttr = ConvertFileAttribute(srcFileInfoAttr);
                    status = m_destFileSystem.SetFileInfo(context, destAttr, destFilePath, baseAttr, allAttr);
                    if (!status.Succeeded) {
                        return status;
                    }
                }
            } finally {
                tempHolder.DeleteAll();
            }

            return FileOperationStatus.SuccessCopy;
        }

        //=========================================================================================
        // 機　能：ファイルシステム間でのファイル属性の変換を行う
        // 引　数：[in]fromFile   転送元ファイルシステムでのファイル属性
        // 戻り値：転送先ファイルシステムでのファイル属性
        //=========================================================================================
        public IFile ConvertFileAttribute(IFile fromFile) {
            FileSystemID srcId = m_srcFileSystem.FileSystemId;
            FileSystemID destId = m_destFileSystem.FileSystemId;
            if (srcId == FileSystemID.Windows) {
                if (destId == FileSystemID.Windows) {
                    return fromFile;
                } else if (destId == FileSystemID.SFTP) {
                    return new SFTPFile((WindowsFile)fromFile);
              } else if (destId == FileSystemID.SSHShell) {
                    return new ShellFile((WindowsFile)fromFile);
                } else {
                    FileSystemID.NotSupportError(destId);
                    return null;
                }
            } else if (srcId == FileSystemID.SFTP) {
                if (destId == FileSystemID.Windows) {
                    return new WindowsFile((SFTPFile)fromFile);
                } else if (destId == FileSystemID.SFTP) {
                    return fromFile;
              } else if (destId == FileSystemID.SSHShell) {
                  return new ShellFile((SFTPFile)fromFile);
                } else {
                    FileSystemID.NotSupportError(destId);
                    return null;
                }
            } else if (srcId == FileSystemID.Virtual) {
                if (destId == FileSystemID.Windows) {
                    return new WindowsFile((VirtualFile)fromFile);
                } else if (destId == FileSystemID.SFTP) {
                    return new SFTPFile((VirtualFile)fromFile);
              } else if (destId == FileSystemID.SSHShell) {
                  return new ShellFile((VirtualFile)fromFile);
                } else {
                    FileSystemID.NotSupportError(destId);
                    return null;
                }
          } else if (srcId == FileSystemID.SSHShell) {
                if (destId == FileSystemID.Windows) {
                    return new WindowsFile((ShellFile)fromFile);
                } else if (destId == FileSystemID.SFTP) {
                    return new SFTPFile((ShellFile)fromFile);
                } else if (destId == FileSystemID.SSHShell) {
                    return fromFile;
                } else {
                    FileSystemID.NotSupportError(destId);
                    return null;
                }
          } else {
                // destId == FileSystemID.Virtualを含む（転送先がVirtualになることはない）
                FileSystemID.NotSupportError(destId);
                return null; 
            }
        }

        //=========================================================================================
        // 機　能：ファイル転送時の転送条件を確認する
        // 引　数：[in]isFilter      ファイルフィルターを使用するときtrue
        // 　　　　[in]srcFilePath   転送先のファイルパス
        // 　　　　[in]destFilePath  転送先のファイルパス
        // 　　　　[in]attrMode      属性を転送するかどうかのモード（属性転送しないときnull）
        // 　　　　[in]tempHolder    テンポラリフォルダの管理クラス（ファイルコピーせず、属性だけのときnull）
        // 　　　　[out]srcTo        ダウンロード先のパス名を返す変数
        // 　　　　[out]destFrom     アップロード元のパス名を返す変数
        // 　　　　[out]baseAttr     基本属性を転送するときtrueを返す変数
        // 　　　　[out]allAttr      すべての属性を転送するときtrueを返す変数
        // 　　　　[in]progress      進捗状態を通知するdelegate
        // 戻り値：なし
        //=========================================================================================
        private void CheckTransferCondition(bool isFilter, string srcFilePath, string destFilePath, AttributeSetMode attrMode, TempFileHolder tempHolder, out string srcTo, out string destFrom, out bool baseAttr, out bool allAttr, FileProgressEventHandler progress) {
            // filter  from     to                download       filter        upload          baseAttr  allAttr
            // No      Windows  Windows     －       －  －        －  －        －  －           －       －
            // No      Windows  SSH         srcPath  ×  srcPath   ×  srcPath   ○  destPath     △       △
            // No      SSH      Windows     srcPath  ○  destPath  ×  destPath  ×  destPath     ○       △
            // No      SSH      SSH         srcPath  ○  temp1     ×  temp1     ○  destPath     △       △   転送量×2
            // Yes     Windows  Windows     －       －  －        －  －        －  －
            // Yes     Windows  SSH         srcPath  ×  srcPath   ○  temp1     ○  destPath     △       △
            // Yes     SSH      Windows     srcPath  ○  temp1     ○  destPath  ×  destPath     ○       △
            // Yes     SSH      SSH         srcPath  ○  temp1     ○  temp2     ○  destPath     △       △   転送量×2
            // ○：取得
            // ×：不要
            // △：設定により取得
            bool srcWin = FileSystemID.IsWindows(m_srcFileSystem.FileSystemId);
            bool srcSsh = !srcWin;
            bool destWin = FileSystemID.IsWindows(m_destFileSystem.FileSystemId);
            bool destSsh = !destWin;
            bool filterYes = isFilter;
            bool filterNo = !isFilter;
            int sizeMultiply = 1;

            bool attrOn = false;               // 表の○：属性転送してよいときtrue
            bool attrOpt = false;              // 表の△：属性転送する設定のときtrue
            if (attrMode != null) {
                attrOn = true;
                attrOpt = attrMode.IsSetAttribute(m_destFileSystem.FileSystemId);
            }

            if (srcWin && destWin) {
                srcTo = null;
                destFrom = null;
                baseAttr = false;
                allAttr = false;
                Program.Abort("ファイル転送の制御エラーです。");
            } else if (filterNo && srcWin && destSsh) {
                srcTo = srcFilePath;    destFrom = srcFilePath;     baseAttr = attrOpt; allAttr = attrOpt;
            } else if (filterNo && srcSsh && destWin) {
                srcTo = destFilePath;   destFrom = destFilePath;    baseAttr = attrOn;  allAttr = attrOpt;
            } else if (filterNo && srcSsh && destSsh) {
                string temp1 = (tempHolder != null) ? tempHolder.CreateNew() : null;
                srcTo = temp1;          destFrom = temp1;           baseAttr = attrOpt; allAttr = attrOpt;
                sizeMultiply = 2;

            } else if (filterYes && srcWin && destSsh) {
                string temp1 = (tempHolder != null) ? tempHolder.CreateNew() : null;
                srcTo = destFilePath;   destFrom = temp1;           baseAttr = attrOpt; allAttr = attrOpt;
            } else if (filterYes && srcSsh && destWin) {
                string temp1 = (tempHolder != null) ? tempHolder.CreateNew() : null;
                srcTo = temp1;          destFrom = destFilePath;    baseAttr = attrOn;  allAttr = attrOpt;
            } else if (filterYes && srcSsh && destSsh) {
                string temp1 = (tempHolder != null) ? tempHolder.CreateNew() : null;
                string temp2 = (tempHolder != null) ? tempHolder.CreateNew() : null;
                srcTo = temp1;          destFrom = temp2;           baseAttr = attrOpt; allAttr = attrOpt;
                sizeMultiply = 2;
            } else {
                srcTo = null;
                destFrom = null;
                baseAttr = false;
                allAttr = false;
                Program.Abort("プログラムの制御エラーです。");
            }
            if (sizeMultiply != 1 && progress != null) {
                progress.SetMultiply(sizeMultiply, 0);
            }
        }

        //=========================================================================================
        // 機　能：ファイルを移動する
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]srcFileInfoAttr 属性コピーを行うとき、srcFilePathの属性（属性をコピーしないときnull）
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]overwrite       上書きするときtrue
        // 　　　　[in]attrMode        属性をコピーするかどうかの設定（属性をコピーしないときnull）
        // 　　　　[in]progress        進捗状態を通知するdelegate
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus MoveFileDirectory(FileOperationRequestContext context, string srcFilePath, IFile srcFileInfoAttr, string destFilePath, bool overwrite, AttributeSetMode attrMode, FileProgressEventHandler progress) {
            if (FileSystemID.IsVirtual(m_srcFileSystem.FileSystemId)) {
                return FileOperationStatus.Fail;
            }
            FileOperationStatus status = Transfer(context, TransferModeType.MoveFile, srcFilePath, srcFileInfoAttr, destFilePath, overwrite, attrMode, null, progress);
            return status;
        }

        //=========================================================================================
        // 機　能：ディレクトリの直接コピー／移動をサポートするかどうかを確認する
        // 引　数：[in]srcDirPath   転送元ディレクトリ名のフルパス
        // 　　　　[in]destDirPath  転送先ディレクトリ名のフルパス
        // 　　　　[in]isCopy       コピーのときtrue、移動のときfalse
        // 戻り値：直接の移動ができるときtrue（trueになっても失敗することはある）
        //=========================================================================================
        public bool CanMoveDirectoryDirect(string srcDirPath, string destDirPath, bool isCopy) {
            return false;
        }

        //=========================================================================================
        // 機　能：ディレクトリの直接コピーを行う
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]srcDirPath    転送元ディレクトリ名のフルパス
        // 　　　　[in]destDirPath   転送先ディレクトリ名のフルパス
        // 　　　　[in]attrMode      属性をコピーするかどうかの設定（属性をコピーしないときnull）
        // 　　　　[in]progress      進捗状態を通知するdelegate
        // 戻り値：ステータス（CopyRetryのとき再試行が必要）
        //=========================================================================================
        public FileOperationStatus CopyDirectoryDirect(FileOperationRequestContext context, string srcPath, string destPath, AttributeSetMode attrMode, FileProgressEventHandler progress) {
            return FileOperationStatus.CopyRetry;
        }

        //=========================================================================================
        // 機　能：ショートカットを作成する
        // 引　数：[in]cache         キャッシュ情報
        // 　　　　[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]overwrite     上書きするときtrue
        // 　　　　[in]type          ショートカットの種類
        // 戻り値：エラーコード
        //=========================================================================================
        public FileOperationStatus CreateShortcut(FileOperationRequestContext cache, string srcFilePath, string destFilePath, bool overwrite, ShortcutType type) {
            // 異なるファイルシステム間はサポートしない
            return FileOperationStatus.Fail;
        }

        //=========================================================================================
        // 機　能：ファイル属性をコピーする
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]isDir         ディレクトリを転送するときtrue
        // 　　　　[in]srcFilePath   転送先のフルパス
        // 　　　　[in]srcFileInfo   転送元のファイル情報（まだ取得できていないときnull）
        // 　　　　[in]destFilePath  転送先のフルパス
        // 　　　　[in]attrMode      設定する属性
        // 戻り値：エラーコード
        //=========================================================================================
        public FileOperationStatus CopyFileInfo(FileOperationRequestContext context, bool isDir, string srcFilePath, IFile srcFileInfo, string destFilePath, AttributeSetMode attrMode) {
            FileOperationStatus status;

            // 条件を決定
            string srcToPath;               // ダウンロード先、フィルタリング元
            string destFromPath;            // フィルタリング先、アップロード元
            bool baseAttr;                  // 基本属性を転送するときtrue
            bool allAttr;                   // すべての属性を転送するときtrue
            CheckTransferCondition(false, srcFilePath, destFilePath, attrMode, null, out srcToPath, out destFromPath, out baseAttr, out allAttr, null);
            if (baseAttr || allAttr) {
                ;
            } else {
                return FileOperationStatus.Success;
            }

            // 転送元の属性を取得
            if (srcFileInfo == null) {
                status = m_srcFileSystem.GetFileInfo(context, srcFilePath, true, out srcFileInfo);
                if (!status.Succeeded) {
                    return status;
                }
            }

            // 属性を設定
            IFile destAttr = ConvertFileAttribute(srcFileInfo);
            status = m_destFileSystem.SetFileInfo(context, destAttr, destFilePath, baseAttr, allAttr);
            return status;
        }

        //=========================================================================================
        // 機　能：ファイルを結合する
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]srcFileList     転送元ファイル名のフルパス
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]taskLogger      ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus CombineFile(FileOperationRequestContext context, List<string> srcFileList, string destFilePath, ITaskLogger taskLogger) {
            FileOperationStatus status;

            // 転送条件を確認
            List<string> srcToList;         // ダウンロード先、結合元
            string destFromPath;            // 結合先、アップロード元
            TempFileHolder tempHolder = new TempFileHolder();
            CheckCombineCondition(srcFileList, destFilePath, tempHolder, out srcToList, out destFromPath);

            try  {
                // 上書き確認
                bool exist;
                status = m_destFileSystem.CheckFileExist(context, destFilePath, false, null, out exist);
                if (!status.Succeeded) {
                    return status;
                }
                if (exist) {
                    // すでに存在している場合は終わり
                    taskLogger.LogFileOperationStart(FileOperationType.CombineFile, destFilePath, false);
                    return taskLogger.LogFileOperationEnd(FileOperationStatus.AlreadyExists);
                }

                // 転送元を標準位置に取得
                if (!FileSystemID.IsWindows(m_srcFileSystem.FileSystemId)) {
                    for (int i = 0; i < srcFileList.Count; i++) {
                        taskLogger.LogFileOperationStart(FileOperationType.Download, srcFileList[i], false);
                        status = m_srcFileSystem.TransferDownload(context, srcFileList[i], destFilePath, srcToList[i], null, taskLogger.Progress);
                        taskLogger.LogFileOperationEnd(status);
                        if (!status.Succeeded || status.Skipped) {
                            return status;
                        }
                    }
                }

                // 結合
                CombineFileProcedure procedure = new CombineFileProcedure();
                procedure.Execute(context, srcToList, destFromPath, taskLogger);

                // 転送先を確定
                if (!FileSystemID.IsWindows(m_destFileSystem.FileSystemId)) {
                    taskLogger.LogFileOperationStart(FileOperationType.Upload, destFilePath, false);
                    status = m_destFileSystem.TransferUpload(context, destFilePath, destFilePath, destFromPath, taskLogger.Progress);
                    taskLogger.LogFileOperationEnd(status);
                    if (!status.Succeeded || status.Skipped) {
                        return status;
                    }
                }
            } finally {
                tempHolder.DeleteAll();
            }

            return FileOperationStatus.SuccessCopy;
        }

        //=========================================================================================
        // 機　能：ファイル結合時の転送条件を確認する
        // 引　数：[in]srcFileList   転送先のファイルパス
        // 　　　　[in]destFilePath  転送先のファイルパス
        // 　　　　[in]tempHolder    テンポラリフォルダの管理クラス
        // 　　　　[out]srcToList    ダウンロード先のパス名を返す変数
        // 　　　　[out]destFrom     アップロード元のパス名を返す変数
        // 戻り値：なし
        //=========================================================================================
        private void CheckCombineCondition(List<string> srcFileList, string destFilePath, TempFileHolder tempHolder, out List<string> srcToList, out string destFrom) {
            // from     to               download        combine       upload
            // Windows  Windows     －       －  －        －  －        －  －
            // Windows  SSH         srcPath  ×  srcPath   ×  temp1     ○  destPath
            // SSH      Windows     srcPath  ○  temp1..N  ×  destPath  ×  destPath
            // SSH      SSH         srcPath  ○  temp1..N  ×  tempN+1   ○  destPath
            // ○：取得
            // ×：不要
            bool srcWin = FileSystemID.IsWindows(m_srcFileSystem.FileSystemId);
            bool srcSsh = !srcWin;
            bool destWin = FileSystemID.IsWindows(m_destFileSystem.FileSystemId);
            bool destSsh = !destWin;

            if (srcWin && destWin) {
                srcToList = null;
                destFrom = null;
                Program.Abort("ファイル転送の制御エラーです。");
            } else if (srcWin && destSsh) {
                srcToList = srcFileList;
                destFrom = tempHolder.CreateNew();
            } else if (srcSsh && destWin) {
                srcToList = new List<string>();
                for (int i = 0; i < srcFileList.Count; i++) {
                    srcToList.Add(tempHolder.CreateNew());
                }
                destFrom = destFilePath;
            } else if (srcSsh && destSsh) {
                srcToList = new List<string>();
                for (int i = 0; i < srcFileList.Count; i++) {
                    srcToList.Add(tempHolder.CreateNew());
                }
                destFrom = tempHolder.CreateNew();
            } else {
                srcToList = null;
                destFrom = null;
                Program.Abort("プログラムの制御エラーです。");
            }
        }

        //=========================================================================================
        // 機　能：ファイルを分割する
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]destFolderPath  転送先フォルダ名のフルパス（最後は「\」）
        // 　　　　[in]numberingInfo   ファイルの連番の命名規則
        // 　　　　[in]splitInfo       ファイルの分割方法
        // 　　　　[in]taskLogger      ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus SplitFile(FileOperationRequestContext context, string srcFilePath, string destFolderPath, RenameNumberingInfo numberingInfo, SplitFileInfo splitInfo, ITaskLogger taskLogger) {
            FileOperationStatus status;

            // 転送条件を確認
            string srcToPath;               // ダウンロード先、分割元
            string destFromPath;            // 結合先、アップロード元
            TempFileHolder tempHolder = new TempFileHolder();
            CheckSplitCondition(srcFilePath, destFolderPath, tempHolder, out srcToPath, out destFromPath);

            try  {
                // 転送元を標準位置に取得
                if (!FileSystemID.IsWindows(m_srcFileSystem.FileSystemId)) {
                    taskLogger.LogFileOperationStart(FileOperationType.Download, srcFilePath, false);
                    status = m_srcFileSystem.TransferDownload(context, srcFilePath, destFolderPath, srcToPath, null, taskLogger.Progress);
                    taskLogger.LogFileOperationEnd(status);
                    if (!status.Succeeded || status.Skipped) {
                        return status;
                    }
                }

                // 分割先を決定
//                long fileLength = WindowsFileUtils.GetFileLength(srcToPath);
//                if (fileLength == -1) {
//                    taskLogger.LogFileOperationStart(FileOperationType.CombineFile, srcFilePath, false);
//                    return taskLogger.LogFileOperationEnd(FileOperationStatus.CanNotAccess);
//                }
//                long dummyOneFileSize;
//                int fileCount;
//                bool success = splitInfo.GetOneFileSize(fileLength, m_srcFileSystem.FileSystemId, m_destFileSystem.FileSystemId, out dummyOneFileSize, out fileCount);
//                if (!success) {
//                    taskLogger.LogFileOperationStart(FileOperationType.CombineFile, srcFilePath, false);
//                    return taskLogger.LogFileOperationEnd(FileOperationStatus.CanNotAccess);
//                }
//                List<string> destFromList;      // 結合先、アップロード元
//                CheckSplitCondition(srcFilePath, tempHolder, out srcToList);

//                // 上書き確認
//                bool exist;
//                status = m_destFileSystem.CheckFileExist(context, destFilePath, false, null, out exist);
//                if (!status.Succeeded) {
//                    return status;
//                }
//                if (exist) {
//                    // すでに存在している場合は終わり
//                    taskLogger.LogFileOperationStart(FileOperationType.CombineFile, destFilePath, false);
//                    return taskLogger.LogFileOperationEnd(FileOperationStatus.AlreadyExists);
//                }

                // 結合
                SplitFileProcedure.SplitDestTempHolder splitDestTempHolder;
                if (destFromPath != destFolderPath) {
                    splitDestTempHolder = new SplitFileProcedure.SplitDestTempHolder(tempHolder);
                } else {
                    splitDestTempHolder = null;
                }
                SplitFileProcedure procedure = new SplitFileProcedure();
                procedure.Execute(context, srcToPath, srcFilePath, destFolderPath, numberingInfo, splitInfo, splitDestTempHolder, taskLogger);

                // 転送先を確定
                if (!FileSystemID.IsWindows(m_destFileSystem.FileSystemId)) {
                    foreach (RenameFilePathInfo filePathInfo in splitDestTempHolder.RenameFilePathList) {
                        taskLogger.LogFileOperationStart(FileOperationType.Upload, filePathInfo.NewName, false);
                        string uploadPath = destFolderPath + filePathInfo.NewName;
                        status = m_destFileSystem.TransferUpload(context, filePathInfo.OriginalName, uploadPath, filePathInfo.OriginalName, taskLogger.Progress);
                        taskLogger.LogFileOperationEnd(status);
                        if (!status.Succeeded || status.Skipped) {
                            return status;
                        }
                    }
                }
            } finally {
                tempHolder.DeleteAll();
            }

            return FileOperationStatus.SuccessCopy;
        }

        //=========================================================================================
        // 機　能：ファイル分割時の転送条件を確認する
        // 引　数：[in]srcFilePath   転送先のファイルパス
        // 　　　　[in]destFilePath  転送先のファイルパス
        // 　　　　[in]tempHolder    テンポラリフォルダの管理クラス
        // 　　　　[out]srcToList    ダウンロード先のパス名を返す変数
        // 　　　　[out]destFromPath アップロード元のパス名を返す変数
        // 戻り値：なし
        //=========================================================================================
        private void CheckSplitCondition(string srcFilePath, string destFilePath, TempFileHolder tempHolder, out string srcTo, out string destFromPath) {
            // from     to               download        split         upload
            // Windows  Windows     －       －  －       －  －        －  －
            // Windows  SSH         srcPath  ×  srcPath  ×  temp1..N     ○  destPath
            // SSH      Windows     srcPath  ○  temp1    ×  destPath     ×  destPath
            // SSH      SSH         srcPath  ○  temp1    ×  temp2..N+1   ○  destPath
            // ○：取得
            // ×：不要
            bool srcWin = FileSystemID.IsWindows(m_srcFileSystem.FileSystemId);
            bool srcSsh = !srcWin;
            bool destWin = FileSystemID.IsWindows(m_destFileSystem.FileSystemId);
            bool destSsh = !destWin;

            if (srcWin && destWin) {
                srcTo = null;
                destFromPath = null;
                Program.Abort("ファイル転送の制御エラーです。");
            } else if (srcWin && destSsh) {
                srcTo = srcFilePath;
                destFromPath = tempHolder.GetTempFolderPath();
            } else if (srcSsh && destWin) {
                srcTo = tempHolder.CreateNew();
                destFromPath = destFilePath;
            } else if (srcSsh && destSsh) {
                srcTo = tempHolder.CreateNew();
                destFromPath = tempHolder.GetTempFolderPath();
            } else {
                srcTo = null;
                destFromPath = null;
                Program.Abort("プログラムの制御エラーです。");
            }
        }
    }
}
