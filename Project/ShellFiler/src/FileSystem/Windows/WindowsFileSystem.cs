using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using ShellFiler.Api;
using ShellFiler.Locale;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.UI.FileList;
using ShellFiler.Properties;

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：Windows同士でのファイル操作API
    //=========================================================================================
    class WindowsFileSystem : IFileSystem {
        // テストで結果が空になるパス
        private const string TEST_PATH = "a:\\_:_";

        // このクラスはsingletonで動作する

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public WindowsFileSystem() {
        }

        //=========================================================================================
        // 機　能：ファイル操作を開始する
        // 引　数：[in]cache     キャッシュ情報
        // 　　　　[in]dirRoot   ルートディレクトリを含むディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public void BeginFileOperation(FileOperationRequestContext cache, string dirRoot) {
        }

        //=========================================================================================
        // 機　能：ファイル操作を終了する
        // 引　数：[in]cache     キャッシュ情報
        // 戻り値：なし
        //=========================================================================================
        public void EndFileOperation(FileOperationRequestContext cache) {
            cache.Dispose();
        }

        //=========================================================================================
        // 機　能：ファイル一覧を取得する
        // 引　数：[in]cache       キャッシュ情報
        // 　　　　[in]directory   取得ディレクトリ
        // 　　　　[out]fileList   ファイル一覧を取得する変数への参照
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus GetFileList(FileOperationRequestContext cache, string directory, out List<IFile> fileList) {
            if (directory == TEST_PATH) {
                fileList = new List<IFile>();
                return FileOperationStatus.Success;
            }
            fileList = new List<IFile>();
            try {
                if (!FileUtils.DirectoryExists(directory)) {
                    return FileOperationStatus.Fail;
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                // 親ディレクトリの情報を取得
                if (directoryInfo.Parent != null) {
                    WindowsFile file = new WindowsFile(directoryInfo.Parent, "..");
                    file.DefaultOrder = 0;
                    fileList.Add(file);
                }

                // ディレクトリ内のオブジェクトを取得
                int order = 1;
                FileSystemInfo[] infoList = directoryInfo.GetFileSystemInfos();
                foreach (FileSystemInfo info in infoList) {
                    IFile file;
                    if (info is FileInfo) {
                        FileInfo fileInfo = (FileInfo)info;
                        file = new WindowsFile(fileInfo);
                    } else {
                        file = new WindowsFile((DirectoryInfo)info, null);
                    }
                    file.DefaultOrder = order++;
                    fileList.Add(file);
                }
            } catch (Exception) {
                fileList = null;
                return FileOperationStatus.Fail;
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：このファイルシステムの新しいファイル一覧を作成する
        // 引　数：[in]directory     一覧を作成するディレクトリ
        // 　　　　[in]isLeftWindow  左画面の一覧を作成するときtrue
        // 　　　　[in]fileListCtx   使用中のファイル一覧のコンテキスト情報
        // 戻り値：ファイル一覧（作成できなかったときnull）
        //=========================================================================================
        public IFileList CreateFileListFromExisting(string directory, bool isLeftWindow, IFileListContext fileListCtx) {
            FileOperationRequestContext cache = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(BackgroundTaskID.NullId, FileSystemID.Windows, FileSystemID.None, null, null, null);
            return new WindowsFileList(cache, this, directory, isLeftWindow);
        }

        //=========================================================================================
        // 機　能：ファイルアイコンを取得する
        // 引　数：[in]filePath  ファイルパス
        // 　　　　[in]isDir     ディレクトリのときtrue
        // 　　　　[in]tryReal   実ファイルを取得するときtrue
        // 　　　　[in]width     取得するアイコンの幅
        // 　　　　[in]height    取得するアイコンの高さ
        // 戻り値：アイコン（失敗したとき、デフォルトアイコンを使用するときnull）
        //=========================================================================================
        public Icon ExtractFileIcon(string filePath, bool isDir, bool tryReal, int width, int height) {
            // ファイルに関連付けられたアイコンを取得
            if (tryReal) {
                Icon icon = null;
                if (width == IconSize.Large32.CxIconSize && height == IconSize.Large32.CyIconSize) {
                    icon = Win32IconUtils.GetFileIcon(filePath, true);
                } else if (width == IconSize.Small16.CxIconSize && height == IconSize.Small16.CyIconSize) {
                    return Win32IconUtils.GetFileIcon(filePath, false);
                } else {
                    icon = Win32IconUtils.GetFileIcon(filePath, true);
                    if (icon != null) {
                        icon = new Icon(icon, new Size(width, height));
                    }
                }
                if (icon != null) {
                    return icon;
                }
            }

            // 取得できない場合はデフォルト
            if (isDir) {
                filePath = Win32IconUtils.SampleFolderPath;
            }
            if (width == IconSize.Large32.CxIconSize && height == IconSize.Large32.CyIconSize) {
                return Win32IconUtils.GetFileIconExtension(filePath, true);
            } else if (width == IconSize.Small16.CxIconSize && height == IconSize.Small16.CyIconSize) {
                return Win32IconUtils.GetFileIconExtension(filePath, false);
            } else {
                return new Icon(Win32IconUtils.GetFileIconExtension(filePath, true), new Size(width, height));
            }
        }

        //=========================================================================================
        // 機　能：ファイル転送用に転送元ファイルをダウンロードする
        // 引　数：[in]context           コンテキスト情報
        // 　　　　[in]srcLogicalPath    転送元ファイルのファイルパス
        // 　　　　[in]destLogicalPath   転送先ファイルのファイルパス
        // 　　　　[in]destPhysicalPath  転送先ファイルとしてWindows上にダウンロードするときの物理パス
        // 　　　　[in]srcFileInfo       転送元のファイル情報
        // 　　　　[in]progress          進捗状態を通知するdelegate
        // 戻り値：ステータスコード（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus TransferDownload(FileOperationRequestContext context, string srcLogicalPath, string destLogicalPath, string destPhysicalPath, IFile srcFileInfo, FileProgressEventHandler progress) {
            Program.Abort("呼び出し予定のないメソッドTranserDownloadが呼び出されました。");
            return FileOperationStatus.Fail;
        }

        //=========================================================================================
        // 機　能：ファイル転送用に転送元ファイルをアップロードする
        // 引　数：[in]context           コンテキスト情報
        // 　　　　[in]srcLogicalPath    転送元ファイルのファイルパス
        // 　　　　[in]destLogicalPath   転送先ファイルのファイルパス
        // 　　　　[in]srcPhysicalPath   転送元ファイルとしてWindows上に用意されているファイルの物理パス
        // 　　　　[in]progress          進捗状態を通知するdelegate
        // 戻り値：ステータスコード（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus TransferUpload(FileOperationRequestContext context, string srcLogicalPath, string destLogicalPath, string srcPhysicalPath, FileProgressEventHandler progress) {
            Program.Abort("呼び出し予定のないメソッドTransferUploadが呼び出されました。");
            return FileOperationStatus.Fail;
        }

        //=========================================================================================
        // 機　能：ファイルの情報を返す
        // 引　数：[in]cache      キャッシュ情報
        // 　　　　[in]directory  ファイルパス
        // 　　　　[in]isTarget   対象パスの一覧のときtrue、反対パスのときfalse
        // 　　　　[out]fileInfo  ファイルの情報（失敗したときはnull）
        // 戻り値：ステータス（成功のときSuccess、存在しないときはSuccessでfileInfo=null）
        //=========================================================================================
        public FileOperationStatus GetFileInfo(FileOperationRequestContext cache, string directory, bool isTarget, out IFile fileInfo) {
            try {
                if (File.Exists(directory)) {
                    FileInfo windowsInfo = new FileInfo(directory);
                    fileInfo = new WindowsFile(windowsInfo);
                    return FileOperationStatus.Success;
                } else if (Directory.Exists(directory)) {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                    fileInfo = new WindowsFile(directoryInfo, null);
                    return FileOperationStatus.Success;
                } else {
                    fileInfo = null;
                    return FileOperationStatus.FileNotFound;
                }
            } catch (Exception) {
                fileInfo = null;
                return FileOperationStatus.Fail;
            }
        }

        //=========================================================================================
        // 機　能：ファイル属性を設定する
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]srcFileInfo   転送元のファイル情報
        // 　　　　[in]destFilePath  転送先のフルパス
        // 　　　　[in]baseAttr      属性の基本部分を設定するときtrue
        // 　　　　[in]allAttr       すべての属性を設定するときtrue
        // 戻り値：エラーコード
        //=========================================================================================
        public FileOperationStatus SetFileInfo(FileOperationRequestContext context, IFile srcFileInfo, string destFilePath, bool baseAttr, bool allAttr) {
            try {
                bool isDir = srcFileInfo.Attribute.IsDirectory;
                WindowsFileFolderSelectUtils fileUtils = new WindowsFileFolderSelectUtils(isDir);
                WindowsFile winFile = (WindowsFile)srcFileInfo;
                if (baseAttr) {
                    FileAttributes attrOrg = fileUtils.GetAttributes(destFilePath);
                    if ((attrOrg & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                        attrOrg &= ~FileAttributes.ReadOnly;
                        fileUtils.SetAttributes(destFilePath, attrOrg);
                    }
                    fileUtils.SetLastWriteTime(destFilePath, winFile.ModifiedDate);
                    if (allAttr) {
                        fileUtils.SetCreationTime(destFilePath, winFile.CreationDate);
                        fileUtils.SetLastAccessTime(destFilePath, winFile.AccessDate);
                    }
                    fileUtils.MergeAttributes(destFilePath, winFile.Attribute);
                } else if (allAttr) {
                    FileAttributes attrOrg = fileUtils.GetAttributes(destFilePath);
                    if ((attrOrg & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                        FileAttributes attrOff = attrOrg & ~FileAttributes.ReadOnly;
                        fileUtils.SetAttributes(destFilePath, attrOff);
                        fileUtils.SetCreationTime(destFilePath, winFile.CreationDate);
                        fileUtils.SetLastAccessTime(destFilePath, winFile.AccessDate);
                        fileUtils.SetAttributes(destFilePath, attrOrg);
                    } else {
                        fileUtils.SetCreationTime(destFilePath, winFile.CreationDate);
                        fileUtils.SetLastAccessTime(destFilePath, winFile.AccessDate);
                    }
                }
                return FileOperationStatus.Success;
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }
        }

        //=========================================================================================
        // 機　能：ファイルの存在を確認する
        // 引　数：[in]cache     コンテキスト情報
        // 　　　　[in]filePath  ファイルパス
        // 　　　　[in]isTarget  対象パスの一覧のときtrue、反対パスのときfalse
        // 　　　　[in]isFile    ファイルの存在を調べるときtrue、フォルダはfalse、両方はnull
        // 　　　　[out]isExist  ファイルが存在するときtrueを返す領域への参照
        // 戻り値：ステータス（成功のときSuccess、存在しないときはSuccessでisExist=false）
        //=========================================================================================
        public FileOperationStatus CheckFileExist(FileOperationRequestContext cache, string filePath, bool isTarget, BooleanFlag isFile, out bool isExist) {
            if (isFile == null) {
                isExist = File.Exists(filePath) | Directory.Exists(filePath);
            } else if (isFile.Value) {
                isExist = File.Exists(filePath);
            } else {
                isExist = Directory.Exists(filePath);
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ディレクトリを作成する
        // 引　数：[in]context    コンテキスト情報
        // 　　　　[in]basePath   ディレクトリを作成する場所のパス
        // 　　　　[in]newName    作成するディレクトリ名
        // 　　　　[in]isTarget   対象パスの一覧のときtrue、反対パスのときfalse
        // 戻り値：ステータス（成功のときSuccessMkDir）
        //=========================================================================================
        public FileOperationStatus CreateDirectory(FileOperationRequestContext context, string basePath, string newName, bool isTarget) {
            // 存在を確認
            string fullPath = CombineFilePath(basePath, newName);
            if (Directory.Exists(fullPath)) {
                return FileOperationStatus.AlreadyExists;
            }
            // ない場合は作成
            try {
                Directory.CreateDirectory(fullPath);
                return FileOperationStatus.SuccessMkDir;
            } catch (Exception) {
                return FileOperationStatus.ErrorMkDir;
            }
        }

        //=========================================================================================
        // 機　能：ボリューム情報を取得する
        // 引　数：[in]cache      キャッシュ情報
        // 　　　　[in]directory   取得するディレクトリ
        // 　　　　[out]volumeInfo ボリューム情報を返す変数への参照（取得できない場合はnull）
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus GetVolumeInfo(FileOperationRequestContext cache, string directory, out VolumeInfo volumeInfo) {
            if (directory == TEST_PATH) {
                volumeInfo = new VolumeInfo();
                return FileOperationStatus.Success;
            }
            volumeInfo = null;
            string root, sub;
            SplitRootPath(directory, out root, out sub);
            uint sectorPerCluster = 0;
            uint bytesPerSector = 0;
            uint numberOfFreeClusters = 0;
            uint totalNumberOfClusters = 0;
            bool success = Win32API.Win32GetDiskFreeSpace(root, ref sectorPerCluster, ref bytesPerSector, ref numberOfFreeClusters, ref totalNumberOfClusters);
            if (!success) {
                return FileOperationStatus.Fail;
            }
            
            long total = (long)sectorPerCluster * (long)bytesPerSector * (long)totalNumberOfClusters;
            long free = (long)sectorPerCluster * (long)bytesPerSector * (long)numberOfFreeClusters;
            long used = total - free;
            int ratio = -1;
            if (total > 0) {
                ratio = (int)Math.Max(0, Math.Min(100, (free / 1024 * 100 / (total / 1024))));
            }

            string volumeLabel = "";
            string diskInfo = "";
            Regex regex = new Regex("[a-zA-Z]:\\\\");
            if (regex.IsMatch(directory)) {
                try {
                    DriveInfo driveInfo = new DriveInfo(directory.Substring(0,1));
                    volumeLabel = driveInfo.VolumeLabel;
                    string driveFormat = driveInfo.DriveFormat;
                    string driveType = DriveTypeUtil.TypeToDisplayString(driveInfo.DriveType);
                    diskInfo = string.Format(Resources.VolumeInfoDrive, total, free, bytesPerSector, driveType, driveFormat);
                } catch (Exception) {
                    // 無視、空文字列のまま
                }
            } else {
                string driveType = DriveTypeUtil.TypeToDisplayString(DriveType.Network);
                volumeLabel = "";
                diskInfo = string.Format(Resources.VolumeInfoNetwork, total, free, bytesPerSector, driveType);
            }
            volumeInfo = new VolumeInfo();
            volumeInfo.UsedDiskSize = used;
            volumeInfo.ClusterSize = (long)sectorPerCluster;
            volumeInfo.FreeSize = free;
            volumeInfo.FreeRatio = ratio;
            volumeInfo.VolumeLabel = volumeLabel;
            volumeInfo.DriveEtcInfo = diskInfo;

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ファイル/フォルダを削除する
        // 引　数：[in]context     コンテキスト情報
        // 　　　　[in]filePath    削除するファイルのパス
        // 　　　　[in]isTarget    対象パスを削除するときtrue、反対パスのときfalse
        // 　　　　[in]flag        削除フラグ
        // 戻り値：ステータス（成功のときSuccessDelete）
        //=========================================================================================
        public FileOperationStatus DeleteFileFolder(FileOperationRequestContext context, string filePath, bool isTarget, DeleteFileFolderFlag flag) {
            try {
                if ((flag & DeleteFileFolderFlag.CHANGE_ATTR) != 0) {
                    FileAttributes attr = FileAttributes.Normal;
                    File.SetAttributes(filePath, attr);
                }

                if ((flag & DeleteFileFolderFlag.FILE) != 0) {
                    // ファイルを削除
                    if ((flag & DeleteFileFolderFlag.RECYCLE_OR_RF) != 0) {
                        // ごみ箱で削除
                        bool success = Win32API.DeleteRecycle(IntPtr.Zero, filePath);
                        if (!success) {
                            return FileOperationStatus.ErrorDelete;
                        }
                    } else {
                        // 通常削除
                        File.Delete(filePath);
                    }
                    return FileOperationStatus.SuccessDelete;
                } else if ((flag & DeleteFileFolderFlag.FOLDER) != 0) {
                    // フォルダを削除
                    if ((flag & DeleteFileFolderFlag.RECYCLE_OR_RF) != 0) {
                        // ごみ箱で削除
                        bool success = Win32API.DeleteRecycle(IntPtr.Zero, filePath);
                        if (!success) {
                            return FileOperationStatus.ErrorDelete;
                        }
                    } else {
                        // 通常削除
                        Directory.Delete(filePath, false);
                    }
                    return FileOperationStatus.SuccessDelDir;
                } else {
                    Program.Abort("削除モードが正しくありません。");
                    return FileOperationStatus.ErrorDelete;
                }
            } catch (Exception) {
                return FileOperationStatus.ErrorDelete;
            }
        }
        
        //=========================================================================================
        // 機　能：ファイルをコピーする
        // 引　数：[in]cache         キャッシュ情報
        // 　　　　[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]overwrite     上書きするときtrue
        // 　　　　[in]fileFilter    転送時に使用するフィルター（使用しないときはnull）
        // 　　　　[in]progress      進捗状態を通知するdelegate
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus CopyWindowsFile(FileOperationRequestContext cache, string srcFilePath, string destFilePath, bool overwrite, FileFilterTransferSetting fileFilter, FileProgressEventHandler progress) {
            FileOperationStatus status;
            if (fileFilter == null) {
                Win32CopyFileEx copyFileEx = new Win32CopyFileEx();
                copyFileEx.CopyProgress = progress;
                int errorCode = copyFileEx.CopyFile(srcFilePath, destFilePath, overwrite);
                status = Win32ToFileOperationStatus(errorCode, FileOperationStatus.SuccessCopy);
            } else {
                FilterCopyProcedure procedure = new FilterCopyProcedure();
                status = procedure.Execute(cache, srcFilePath, destFilePath, overwrite, fileFilter, progress);
            }

            return status;
        }

        //=========================================================================================
        // 機　能：ファイルを移動する
        // 引　数：[in]cache         キャッシュ情報
        // 　　　　[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]overwrite     上書きするときtrue
        // 　　　　[in]progress      進捗状態を通知するdelegate
        // 　　　　[out]attrSet      属性を設定したときtrueを返す変数
        // 戻り値：エラーコード
        //=========================================================================================
        public FileOperationStatus MoveWindowsFile(FileOperationRequestContext cache, string srcFilePath, string destFilePath, bool overwrite, FileProgressEventHandler progress, out bool attrSet) {
            attrSet = true;
            Win32CopyFileEx copyFileEx = new Win32CopyFileEx();
            copyFileEx.CopyProgress = progress;
            int errorCode = copyFileEx.MoveFile(srcFilePath, destFilePath, overwrite, false);
            if (errorCode == Win32API.ERROR_NOT_SAME_DEVICE) {
                errorCode = copyFileEx.MoveFile(srcFilePath, destFilePath, overwrite, true);
                attrSet = false;
            }
            FileOperationStatus status = Win32ToFileOperationStatus(errorCode, FileOperationStatus.SuccessMove);

            return status;
        }
        
        //=========================================================================================
        // 機　能：ショートカットを作成する
        // 引　数：[in]cache         キャッシュ情報
        // 　　　　[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]overwrite     上書きするときtrue
        // 戻り値：エラーコード
        //=========================================================================================
        public FileOperationStatus CreateShortcut(FileOperationRequestContext cache, string srcFilePath, string destFilePath, bool overwrite) {
            try {
                if (!overwrite) {
                    if (File.Exists(destFilePath) || Directory.Exists(destFilePath)) {
                        return FileOperationStatus.AlreadyExists;
                    }
                }
                bool success = Win32API.CreateShortcut(srcFilePath, destFilePath);
                if (!success) {
                    return FileOperationStatus.Fail;
                }
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：Win32のエラーコードをステータスコードに変換する
        // 引　数：[in]errorCode      Win32のエラーコード
        // 　　　　[in]successStatus  成功時に返すステータス
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus Win32ToFileOperationStatus(int errorCode, FileOperationStatus successStatus) {
            switch (errorCode) {
                case Win32API.SUCCESS:
                    return successStatus;
                case Win32API.ERROR_FILE_EXISTS:
                    return FileOperationStatus.AlreadyExists;
                case Win32API.ERROR_ALREADY_EXISTS:
                    return FileOperationStatus.AlreadyExists;
                case Win32API.ERROR_DISK_FULL:
                    return FileOperationStatus.DiskFull;
                case Win32API.ERROR_FILE_NOT_FOUND:
                case Win32API.ERROR_PATH_NOT_FOUND:
                    return FileOperationStatus.FileNotFound;
                case Win32API.ERROR_ACCESS_DENIED:
                    return FileOperationStatus.CanNotAccess;
                case Win32API.ERROR_SHARING_VIOLATION:
                    return FileOperationStatus.SharingViolation;
                case Win32API.ERROR_REQUEST_ABORTED:
                    return FileOperationStatus.Canceled;
                default:
                    return FileOperationStatus.Fail;
            }
        }
                
        //=========================================================================================
        // 機　能：ファイルの名前と属性を変更する
        // 引　数：[in]cache      キャッシュ情報
        // 　　　　[in]path       属性を変更するファイルやディレクトリのフルパス
        // 　　　　[in]orgInfo    変更前のファイル情報
        // 　　　　[in]newInfo    変更後のファイル情報
        // 戻り値：ステータス（成功のときSuccessRename）
        //=========================================================================================
        public FileOperationStatus Rename(FileOperationRequestContext cache, string path, RenameFileInfo orgInfo, RenameFileInfo newInfo) {
            RenameProcedure proc = new RenameProcedure();
            return proc.Execute(path, orgInfo, newInfo);
        }

        //=========================================================================================
        // 機　能：ファイルの名前と属性を一括変更のルールで変更する
        // 引　数：[in]cache      キャッシュ情報
        // 　　　　[in]path       属性を変更するファイルやディレクトリのフルパス
        // 　　　　[in]renameInfo 変更ルール
        // 　　　　[in]modifyCtx  名前変更のコンテキスト情報
        // 戻り値：ステータス（成功のときSuccessRename）
        //=========================================================================================
        public FileOperationStatus ModifyFileInfo(FileOperationRequestContext cache, string path, RenameSelectedFileInfo renameInfo, ModifyFileInfoContext modifyCtx) {
            ModifyFileInfoProcedure proc = new ModifyFileInfoProcedure();
            return proc.Execute(path, renameInfo, modifyCtx);
        }

        //=========================================================================================
        // 機　能：画像を読み込む
        // 引　数：[in]context   コンテキスト情報
        // 　　　　[in]filePath  読み込み対象のファイルパス
        // 　　　　[in]maxSize   読み込む最大サイズ
        // 　　　　[out]image    読み込んだ画像を返す変数
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus RetrieveImage(FileOperationRequestContext context, string filePath, long maxSize, out BufferedImage image) {
            WindowsRetrieveImageProcedure procedure = new WindowsRetrieveImageProcedure();
            FileOperationStatus status = procedure.Execute(context, filePath, maxSize, out image);
            return status;
        }

        //=========================================================================================
        // 機　能：ファイルアクセスのためファイルを準備する（チャンクで読み込み）
        // 引　数：[in]context  コンテキスト情報
        // 　　　　[in]file     アクセスしたいファイルの情報
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus RetrieveFileChunk(FileOperationRequestContext context, AccessibleFile file) {
            WindowsRetrieveFileChunkProcedure procedure = new WindowsRetrieveFileChunkProcedure();
            FileOperationStatus status = procedure.Execute(context, file, file.FilePath);
            return status;
        }

        //=========================================================================================
        // 機　能：リモートでコマンドを実行する
        // 引　数：[in]cache       キャッシュ情報
        // 　　　　[in]dirName     カレントディレクトリ名
        // 　　　　[in]command     コマンドライン
        // 　　　　[in]errorExpect エラーの期待値
        // 　　　　[in]relayOutLog 標準出力の結果をログ出力するときtrue
        // 　　　　[in]relayErrLog 標準エラーの結果をログ出力するときtrue
        // 　　　　[in]dataTarget  取得データの格納先
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus RemoteExecute(FileOperationRequestContext cache, string dirName, string command, List<OSSpecLineExpect> errorExpect, bool relayOutLog, bool relayErrLog, IRetrieveFileDataTarget dataTarget) {
            WindowsShellExecuteProcedure proc = new WindowsShellExecuteProcedure();
            return proc.Execute(cache, dirName, command, relayOutLog, dataTarget);
        }

        //=========================================================================================
        // 機　能：ファイルを関連づけ実行する
        // 引　数：[in]filePath      実行するファイルのローカルパス
        // 　　　　[in]currentDir    カレントパス
        // 　　　　[in]allFile       すべてのファイルを実行するときtrue、実行ファイルだけのときfalse
        // 　　　　[in]fileListCtx   ファイル一覧のコンテキスト情報
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus OpenShellFile(string filePath, string currentDir, bool allFile, IFileListContext fileListCtx) {
            try {
                string lower = filePath.ToLower();
                if (lower.EndsWith(".exe") || lower.EndsWith(".com") || allFile) {
                    Process process = OSUtils.ProcessStartCommandLine(filePath, currentDir);
                    if (process != null) {
                        process.Dispose();
                    }
                    return FileOperationStatus.Success;
                } else {
                    return FileOperationStatus.Skip;
                }
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }
        }

        //=========================================================================================
        // 機　能：指定したフォルダ以下のファイルサイズ合計を取得する
        // 引　数：[in]context     コンテキスト情報
        // 　　　　[in]directory   対象ディレクトリのルート
        // 　　　　[in]condition   取得条件
        // 　　　　[out]result     取得結果を返す変数
        // 　　　　[in]progress    進捗状態を通知するdelegate
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus RetrieveFolderSize(FileOperationRequestContext context, string directory, RetrieveFolderSizeCondition condition, RetrieveFolderSizeResult result, FileProgressEventHandler progress) {
            RetrieveFolderSizeProcedure proc = new RetrieveFolderSizeProcedure(context, condition, result, progress);
            return proc.Execute(directory);
        }

        //=========================================================================================
        // 機　能：このファイルシステムのパス同士で、同じサーバ空間のパスかどうかを調べる
        // 引　数：[in]path1  パス１
        // 　　　　[in]path2  パス２
        // 戻り値：パス１とパス２が同じサーバ空間にあるときtrue
        //=========================================================================================
        public bool IsSameServerSpace(string path1, string path2) {
            return true;
        }
 
        //=========================================================================================
        // 機　能：パスとファイルを連結する
        // 引　数：[in]dir  ディレクトリ名
        // 　　　　[in]file ファイル名
        // 戻り値：連結したファイル名
        //=========================================================================================
        public string CombineFilePath(string dir, string file) {
            return WindowsFileUtils.CombineFilePath(dir, file);
        }

        //=========================================================================================
        // 機　能：ディレクトリ名の最後を'\'または'/'にする
        // 引　数：[in]dir  ディレクトリ名
        // 戻り値：'\'または'/'を補完したディレクトリ名
        //=========================================================================================
        public string CompleteDirectoryName(string dir) {
            return GenericFileStringUtils.CompleteDirectoryName(dir, "\\");
        }

        //=========================================================================================
        // 機　能：このファイルシステムの絶対パス表現かどうかを調べる
        // 引　数：[in]directory     ディレクトリ名
        // 　　　　[in]fileListCtx   ファイル一覧のコンテキスト情報
        // 戻り値：絶対パスのときtrue(trueでも実際にファイルアクセスできるかどうかは不明)
        //=========================================================================================
        public bool IsAbsolutePath(string directory, IFileListContext fileListCtx) {
            VirtualFolderInfo virtualInfo = null;
            if (fileListCtx is VirtualFileListContext) {
                virtualInfo = ((VirtualFileListContext)fileListCtx).VirtualFolderInfo;
            }
            if (virtualInfo == null) {
                return GenericFileStringUtils.IsWindowsAbsolutePath(directory);
            } else {
                VirtualFolderArchiveInfo item = virtualInfo.GetVirtualFolderItem(directory);
                return (item != null);
            }
        }

        //=========================================================================================
        // 機　能：指定されたパス名をルートとそれ以外に分割する
        // 引　数：[in]path   パス名
        // 　　　　[out]root  ルート部分を返す文字列（最後はセパレータ）
        // 　　　　[out]sub   サブディレクトリ部分を返す文字列
        // 戻り値：なし
        //=========================================================================================
        public void SplitRootPath(string path, out string root, out string sub) {
            // ?:\形式
            Regex regex1 = new Regex("[a-zA-Z]:\\\\");
            if (regex1.IsMatch(path)) {
                root = path.Substring(0, 3);
                sub = path.Substring(3);
                return;
            }
            // \\xxx\形式
            Regex regex2 = new Regex("\\\\\\\\[a-zA-Z0-9\\.\\-]+\\\\");
            if (regex2.IsMatch(path)) {
                string[] fileFolder = path.Substring(2).Split('\\');
                if (fileFolder.Length >= 3) {
                    root = path.Substring(0, fileFolder[0].Length + fileFolder[1].Length + 4);
                    sub = path.Substring(root.Length);
                    // \\  server\share\  が最小、「server\share\」を分解すると2以上になる
                    return;
                } else if (fileFolder.Length == 2 && !path.EndsWith("\\")) {
                    root = path.Substring(0, fileFolder[0].Length + fileFolder[1].Length + 3);
                    sub = path.Substring(root.Length);
                    root += "\\";
                    // 「server\share」を分解すると2になる
                }
            }
            root = null;
            sub = null;
        }

        //=========================================================================================
        // 機　能：指定されたパス名のホームディレクトリを取得する
        // 引　数：[in]path  パス名
        // 戻り値：ホームディレクトリ（取得できないときnull）
        //=========================================================================================
        public string GetHomePath(string path) {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        //=========================================================================================
        // 機　能：ファイルパスからファイル名を返す
        // 引　数：[in]filePath  ファイルパス
        // 戻り値：ファイルパス中のファイル名
        //=========================================================================================
        public string GetFileName(string filePath) {
            return GenericFileStringUtils.GetFileName(filePath, '\\');
        }

        //=========================================================================================
        // 機　能：指定されたパス名の絶対パス表現を取得する
        // 引　数：[in]path  パス名
        // 戻り値：絶対パス
        //=========================================================================================
        public string GetFullPath(string path) {
            return GenericFileStringUtils.CleanupWindowsFullPath(path);
        }

        //=========================================================================================
        // 機　能：指定されたパスからディレクトリ名部分を返す
        // 引　数：[in]path     パス名
        // 戻り値：パス名のディレクトリ部分
        //=========================================================================================
        public string GetDirectoryName(string path) {
            return GenericFileStringUtils.GetDirectoryName(path, '\\');
        }
        
        //=========================================================================================
        // 機　能：パスの区切り文字を返す
        // 引　数：[in]fileListCtx   ファイル一覧のコンテキスト情報
        // 戻り値：パスの区切り文字
        //=========================================================================================
        public string GetPathSeparator(IFileListContext fileListCtx) {
            return "\\";
        }

        //=========================================================================================
        // 機　能：後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
        }
        
        //=========================================================================================
        // プロパティ：ファイルシステムID
        //=========================================================================================
        public FileSystemID FileSystemId {
            get {
                return FileSystemID.Windows;
            }
        }

        //=========================================================================================
        // プロパティ：サポートするショートカットの種類
        //=========================================================================================
        public ShortcutType[] SupportedShortcutType {
            get {
                ShortcutType[] list = new ShortcutType[1];
                list[0] = ShortcutType.WindowsShortcut;
                return list;
            }
        }

        //=========================================================================================
        // プロパティ：ローカル実行の際、ダウンロードとアップロードが必要なときtrue
        //=========================================================================================
        public bool LocalExecuteDownloadRequired {
            get {
                return false;
            }
        }

        //=========================================================================================
        // プロパティ：表示の際の項目一覧
        //=========================================================================================
        public FileListHeaderItem[] FileListHeaderItemList {
            get {
                FileListHeaderItem[] list = new FileListHeaderItem[3];
                list[0] = new FileListHeaderItem(FileListHeaderItem.FileListHeaderItemId.FileName,     Resources.FileListItemFileName,   "WWWWWWWWWW.WWW", true);      // ファイル名
                list[1] = new FileListHeaderItem(FileListHeaderItem.FileListHeaderItemId.FileSize,     Resources.FileListItemSize,       "999.999W",       false);     // ファイルサイズ
                list[2] = new FileListHeaderItem(FileListHeaderItem.FileListHeaderItemId.ModifiedTime, Resources.FileListItemUpdateDate, "99/99/99 99:99", false);     // 更新日時
                return list;
            }
        }

        //=========================================================================================
        // プロパティ：通常使用するエンコード方式
        //=========================================================================================
        public EncodingType DefaultEncoding {
            get {
                return EncodingType.SJIS;
            }
        }
    }
}
