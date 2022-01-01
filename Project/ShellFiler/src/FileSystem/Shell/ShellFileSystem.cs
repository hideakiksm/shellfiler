using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.Properties;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.Locale;
using ShellFiler.UI.FileList;
using ShellFiler.UI.Log;

namespace ShellFiler.FileSystem.Shell {

    //=========================================================================================
    // クラス：SSHシェルでのファイル操作API
    //=========================================================================================
    public class ShellFileSystem : IFileSystem {
        // このクラスはsingletonで動作する
        // 接続の管理クラス
        private SSHConnectionManager m_connectionManager;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]manager  接続の管理クラス
        // 戻り値：なし
        //=========================================================================================
        public ShellFileSystem(SSHConnectionManager manager) {
            m_connectionManager = manager;
        }

        //=========================================================================================
        // 機　能：後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
        }

        //=========================================================================================
        // 機　能：ファイル操作を開始する
        // 引　数：[in]context   コンテキスト情報
        // 　　　　[in]dirRoot   ルートディレクトリを含むディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public void BeginFileOperation(FileOperationRequestContext context, string dirRoot) {
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(context, dirRoot, out connection);
            if (status != FileOperationStatus.Success) {
                context.SetCancel(CancelReason.User);
            }
            connection.CompleteRequest();
        }

        //=========================================================================================
        // 機　能：ファイル操作を終了する
        // 引　数：[in]context      コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public void EndFileOperation(FileOperationRequestContext context) {
            m_connectionManager.EndFileOperation(context);
            context.Dispose();
        }

        //=========================================================================================
        // 機　能：バックグラウンドタスクにファイル一覧取得のリクエストを行う
        // 引　数：[in]context      コンテキスト情報
        // 　　　　[in]fileList     ファイル一覧を取得した結果を返すオブジェクト
        // 　　　　[in]directory    一覧を作成するディレクトリ
        // 　　　　[in]isLeftWindow 左ウィンドウに表示する一覧のときtrue
        // 　　　　[in]chdirMode    ディレクトリ変更のモード
        // 戻り値：なし（MainWindowFormのdelegateで結果を通知）
        //=========================================================================================
        public void GetUIFileList(FileOperationRequestContext context, ShellFileList fileList, string directory, bool isLeftWindow, ChangeDirectoryParam chdirMode) {
            ShellGetUIFileListArg arg = new ShellGetUIFileListArg(context, fileList, directory, chdirMode);
            Program.Document.UIRequestBackgroundThread.Request(arg, directory);
        }

        //=========================================================================================
        // 機　能：このファイルシステムの新しいファイル一覧を作成する
        // 引　数：[in]directory     一覧を作成するディレクトリ
        // 　　　　[in]isLeftWindow  左画面の一覧を作成するときtrue
        // 　　　　[in]fileListCtx   使用中のファイル一覧のコンテキスト情報
        // 戻り値：ファイル一覧（作成できなかったときnull）
        //=========================================================================================
        public IFileList CreateFileListFromExisting(string directory, bool isLeftWindow, IFileListContext fileListCtx) {
            ShellFileList fileList = new ShellFileList(this, directory, isLeftWindow, fileListCtx);
            return fileList;
        }

        //=========================================================================================
        // 機　能：ファイル一覧を取得する
        // 引　数：[in]context     コンテキスト情報
        // 　　　　[in]directory   取得ディレクトリ
        // 　　　　[out]fileList   ファイル一覧を取得する変数への参照
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus GetFileList(FileOperationRequestContext context, string directory, out List<IFile> fileList) {
            fileList = null;
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(context, directory, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellGetFileListProcedure procedure = new ShellGetFileListProcedure(connection, context);
                status = procedure.Execute(directory, out fileList);
            } finally {
                connection.CompleteRequest();
            }
            return status;
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
            // 仮想フォルダ/SSHで同じ実装
            return GenericFileStringUtils.ExtractFileIcon(filePath, isDir, tryReal, width, height);
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
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(context, srcLogicalPath, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellTransferDownloadProcedure procedure = new ShellTransferDownloadProcedure(connection, context);
                status = procedure.Execute(srcLogicalPath, destPhysicalPath, srcFileInfo, progress);
            } finally {
                connection.CompleteRequest();
            }
            return status;
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
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(context, destLogicalPath, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellTransferUploadProcedure procedure = new ShellTransferUploadProcedure(connection, context);
                status = procedure.Execute(srcPhysicalPath, destLogicalPath, progress);
            } finally {
                connection.CompleteRequest();
            }
            return status;
        }

        //=========================================================================================
        // 機　能：ファイルの情報を返す
        // 引　数：[in]context    コンテキスト情報
        // 　　　　[in]filePath   ファイルパス
        // 　　　　[in]isTarget   対象パスの一覧のときtrue、反対パスのときfalse
        // 　　　　[out]fileInfo  ファイルの情報（失敗したときはnull）
        // 戻り値：ステータス（成功のときSuccess、存在しないときはSuccessでfileInfo=null）
        //=========================================================================================
        public FileOperationStatus GetFileInfo(FileOperationRequestContext context, string filePath, bool isTarget, out IFile fileInfo) {
            fileInfo = null;
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(context, filePath, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellGetFileInfoProcedure procedure = new ShellGetFileInfoProcedure(connection, context);
                status = procedure.Execute(filePath, isTarget, out fileInfo);
            } finally {
                connection.CompleteRequest();
            }
            return status;
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
            if (baseAttr || allAttr) {
                ;
            } else {
                return FileOperationStatus.Success;
            }
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(context, destFilePath, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellFile sshFile = (ShellFile)srcFileInfo;
                ShellSetFileInfoProcedure procedure = new ShellSetFileInfoProcedure(connection, context);
                status = procedure.Execute(sshFile, destFilePath);
            } finally {
                connection.CompleteRequest();
            }
            return status;
        }

        //=========================================================================================
        // 機　能：ファイルの存在を確認する
        // 引　数：[in]context   コンテキスト情報
        // 　　　　[in]filePath  ファイルパス
        // 　　　　[in]isFile    ファイルの存在を調べるときtrue、フォルダはfalse、両方はnull
        // 　　　　[in]isTarget  対象パスの一覧のときtrue、反対パスのときfalse
        // 　　　　[out]isExist  ファイルが存在するときtrueを返す領域への参照
        // 戻り値：ステータス（成功のときSuccess、存在しないときはSuccessでisExist=false）
        //=========================================================================================
        public FileOperationStatus CheckFileExist(FileOperationRequestContext context, string filePath, bool isTarget, BooleanFlag isFile, out bool isExist) {
            FileOperationStatus status = SSHUtils.CheckFileExistImpl(this, context, filePath, isTarget, isFile, out isExist);
            return status;
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
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(context, basePath, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellCreateDirectoryProcedure procedure = new ShellCreateDirectoryProcedure(connection, context);
                status = procedure.Execute(basePath, newName, isTarget);
            } finally {
                connection.CompleteRequest();
            }
            return status;
        }

        //=========================================================================================
        // 機　能：リモート同士でファイルを転送する
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]attrCopy      属性コピーを行うときtrue
        // 　　　　[in]progress      進捗状態を通知するdelegate（サポートしないときnull）
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus RemoteCopyDirectoryDirect(FileOperationRequestContext context, string srcFilePath, string destFilePath, bool attrCopy, FileProgressEventHandler progress) {
            FileOperationStatus status = FileOperationStatus.CopyRetry;

            // 転送元と転送先が同じSSHコネクションで処理できるか？
            bool sameSession = SSHUtils.IsSameSSHSession(srcFilePath, destFilePath);
            if (sameSession) {
                // 同じセッションで処理
                SSHConnection connection;
                status = m_connectionManager.GetSSHConnection(context, srcFilePath, out connection);
                if (status != FileOperationStatus.Success) {
                    return status;
                }

                // コピー/移動を実行する
                try {
                    ShellRemoteFileTransferProcedure procedure = new ShellRemoteFileTransferProcedure(connection, context);
                    status = procedure.Execute(TransferModeType.CopyDirectory, srcFilePath, destFilePath, false, attrCopy, progress);
                } finally {
                    connection.CompleteRequest();
                }

                // 転送先にファイルがある場合は個別転送が必要
                if (status == FileOperationStatus.AlreadyExists) {
                    return FileOperationStatus.CopyRetry;
                }
            }
            return status;
        }

        //=========================================================================================
        // 機　能：ファイルを結合する
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]taskLogger      ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus CombineFile(FileOperationRequestContext context, List<string> srcFilePath, string destFilePath, ITaskLogger taskLogger) {
            FileOperationStatus status;

            SSHConnection connection;
            status = m_connectionManager.GetSSHConnection(context, srcFilePath[0], out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }

            try {
                ShellCombineFileProcedure procedure = new ShellCombineFileProcedure(connection, context);
                status = procedure.Execute(srcFilePath, destFilePath, taskLogger);
            } finally {
                connection.CompleteRequest();
            }

            return status;
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

            SSHConnection connection;
            status = m_connectionManager.GetSSHConnection(context, srcFilePath, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }

            try {
                ShellSplitFileProcedure procedure = new ShellSplitFileProcedure(connection, context);
                status = procedure.Execute(srcFilePath, destFolderPath, numberingInfo, splitInfo, taskLogger);
            } finally {
                connection.CompleteRequest();
            }

            return status;
        }

        //=========================================================================================
        // 機　能：ショートカットを作成する
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]overwrite     上書きするときtrue
        // 　　　　[in]type          ショートカットの種類
        // 戻り値：エラーコード
        //=========================================================================================
        public FileOperationStatus CreateShortcut(FileOperationRequestContext context, string srcFilePath, string destFilePath, bool overwrite, ShortcutType type) {
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(context, destFilePath, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                TransferModeType transferMode;
                if (type == ShortcutType.SymbolicLink) {
                    transferMode = TransferModeType.SymbolicLink;
                } else {
                    transferMode = TransferModeType.HardLink;
                }
                ShellRemoteFileTransferProcedure procedure = new ShellRemoteFileTransferProcedure(connection, context);
                status = procedure.Execute(transferMode, srcFilePath, destFilePath, overwrite, false, FileProgressEventHandler.Dummy);
            } finally {
                connection.CompleteRequest();
            }
            return status;
        }

        //=========================================================================================
        // 機　能：リモート同士でファイルを転送する
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]transferMode    ファイル転送のモード
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]overwrite       上書きするときtrue
        // 　　　　[in]attrCopy        属性コピーを行うときtrue
        // 　　　　[in]fileFilter      転送時に使用するフィルター（使用しないときはnull）
        // 　　　　[in]progress        進捗状態を通知するdelegate（サポートしないときnull）
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus RemoteToRemoteFileTransfer(FileOperationRequestContext context, TransferModeType transferMode, string srcFilePath, string destFilePath, bool overwrite, bool attrCopy, FileFilterTransferSetting fileFilter, FileProgressEventHandler progress) {
            FileOperationStatus status = FileOperationStatus.Fail;

            // 同じセッションで処理
            SSHConnection connection;
            status = m_connectionManager.GetSSHConnection(context, srcFilePath, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }

            // コピー/移動を実行する
            // 属性コピー時、srcFileInfoAttrは無視
            try {
                ShellRemoteFileTransferProcedure procedure = new ShellRemoteFileTransferProcedure(connection, context);
                status = procedure.Execute(transferMode, srcFilePath, destFilePath, overwrite, attrCopy, progress);
            } finally {
                connection.CompleteRequest();
            }
            return status;
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
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(context, filePath, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellDeleteProcedure procedure = new ShellDeleteProcedure(connection, context);
                status = procedure.Execute(filePath, isTarget, flag);
            } finally {
                connection.CompleteRequest();
            }
            return status;
        }

        //=========================================================================================
        // 機　能：ファイルの名前と属性を変更する
        // 引　数：[in]cache      キャッシュ情報
        // 　　　　[in]path       属性を変更するファイルやディレクトリのフルパス
        // 　　　　[in]orgInfo    変更前のファイル情報
        // 　　　　[in]newInfo    変更後のファイル情報
        // 戻り値：ステータス（成功のときSuccessDelDir）
        //=========================================================================================
        public FileOperationStatus Rename(FileOperationRequestContext cache, string path, RenameFileInfo orgInfo, RenameFileInfo newInfo) {
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(cache, path, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellRenameProcedure procedure = new ShellRenameProcedure(connection, cache);
                status = procedure.Execute(path, orgInfo, newInfo);
            } finally {
                connection.CompleteRequest();
            }
            return status;
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
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(cache, path, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellModifyFileInfoProcedure procedure = new ShellModifyFileInfoProcedure(connection, cache);
                status = procedure.Execute(path, renameInfo, modifyCtx);
            } finally {
                connection.CompleteRequest();
            }
            return status;
        }

        //=========================================================================================
        // 機　能：画像を読み込む
        // 引　数：[in]cache     キャッシュ情報
        // 　　　　[in]filePath  読み込み対象のファイルパス
        // 　　　　[in]maxSize   読み込む最大サイズ
        // 　　　　[out]image    読み込んだ画像を返す変数
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus RetrieveImage(FileOperationRequestContext cache, string filePath, long maxSize, out BufferedImage image) {
            image = null;
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(cache, filePath, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellRetrieveImageProcedure procedure = new ShellRetrieveImageProcedure(connection, cache);
                status = procedure.Execute(filePath, maxSize, out image);
            } finally {
                connection.CompleteRequest();
            }
            return status;
        }

        //=========================================================================================
        // 機　能：ファイルアクセスのためファイルを準備する（チャンクで読み込み）
        // 引　数：[in]cache   キャッシュ情報
        // 　　　　[in]file    アクセスしたいファイルの情報
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus RetrieveFileChunk(FileOperationRequestContext cache, AccessibleFile file) {
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(cache, file.FilePath, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellRetrieveFileChunkProcedure procedure = new ShellRetrieveFileChunkProcedure(connection, cache);
                status = procedure.Execute(file);
            } finally {
                connection.CompleteRequest();
            }
            return status;
        }
    
        //=========================================================================================
        // 機　能：リモートでコマンドを実行する
        // 引　数：[in]context     コンテキスト情報
        // 　　　　[in]dirName     カレントディレクトリ名
        // 　　　　[in]command     コマンドライン
        // 　　　　[in]errorExpect エラーの期待値
        // 　　　　[in]relayOutLog 標準出力の結果をログ出力するときtrue
        // 　　　　[in]relayErrLog 標準エラーの結果をログ出力するときtrue
        // 　　　　[in]dataTarget  取得データの格納先
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus RemoteExecute(FileOperationRequestContext context, string dirName, string command, List<OSSpecLineExpect> errorExpect, bool relayOutLog, bool relayErrLog, IRetrieveFileDataTarget dataTarget) {
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(context, dirName, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellShellExecuteProcedure procedure = new ShellShellExecuteProcedure(connection, context);
                status = procedure.Execute(dirName, command, errorExpect, dataTarget);
            } finally {
                connection.CompleteRequest();
            }
            return status;
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
            return FileOperationStatus.Skip;
        }

        //=========================================================================================
        // 機　能：指定したフォルダ以下のファイルサイズ合計を取得する
        // 引　数：[in]context     コンテキスト情報
        // 　　　　[in]directory   対象ディレクトリのルート
        // 　　　　[in]condition   取得条件
        // 　　　　[in]result      取得結果を返す変数
        // 　　　　[in]progress    進捗状態を通知するdelegate
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus RetrieveFolderSize(FileOperationRequestContext context, string directory, RetrieveFolderSizeCondition condition, RetrieveFolderSizeResult result, FileProgressEventHandler progress) {
            SSHConnection connection;
            FileOperationStatus status = m_connectionManager.GetSSHConnection(context, directory, out connection);
            if (status != FileOperationStatus.Success) {
                return status;
            }
            try {
                ShellRetrieveFolderSizeProcedure procedure = new ShellRetrieveFolderSizeProcedure(connection, context, condition, result);
                status = procedure.Execute(directory);
            } finally {
                connection.CompleteRequest();
            }
            return status;
        }

        //=========================================================================================
        // 機　能：このファイルシステムのパス同士で、同じサーバ空間のパスかどうかを調べる
        // 引　数：[in]path1  パス１
        // 　　　　[in]path2  パス２
        // 戻り値：パス１とパス２が同じサーバ空間にあるときtrue
        //=========================================================================================
        public bool IsSameServerSpace(string path1, string path2) {
            bool same = SSHUtils.IsSameServerSpace(path1, path2);
            return same;
        }

        //=========================================================================================
        // 機　能：パスとファイルを連結する
        // 引　数：[in]dir  ディレクトリ名
        // 　　　　[in]file ファイル名
        // 戻り値：連結したファイル名
        //=========================================================================================
        public string CombineFilePath(string dir, string file) {
            string path = SSHUtils.CombineFilePath(dir, file);
            return path;
        }

        //=========================================================================================
        // 機　能：ディレクトリ名の最後を'\'または'/'にする
        // 引　数：[in]dir  ディレクトリ名
        // 戻り値：'\'または'/'を補完したディレクトリ名
        //=========================================================================================
        public string CompleteDirectoryName(string dir) {
            string comp = SSHUtils.CompleteDirectoryName(dir);
            return comp;
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
                return SSHUtils.IsAbsolutePath(SSHProtocolType.SSHShell, directory);
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
            SSHUtils.SplitRootPath(path, out root, out sub);
        }

        //=========================================================================================
        // 機　能：指定されたパス名のホームディレクトリを取得する
        // 引　数：[in]path  パス名
        // 戻り値：ホームディレクトリ（取得できないときnull）
        //=========================================================================================
        public string GetHomePath(string path) {
            string home = SSHUtils.GetHomePath(path);
            return home;
        }

        //=========================================================================================
        // 機　能：ファイルパスからファイル名を返す
        // 引　数：[in]filePath  ファイルパス
        // 戻り値：ファイルパス中のファイル名
        //=========================================================================================
        public string GetFileName(string filePath) {
            string name = SSHUtils.GetFileName(filePath);
            return name;
        }

        //=========================================================================================
        // 機　能：指定されたパス名の絶対パス表現を取得する
        // 引　数：[in]path  パス名
        // 戻り値：絶対パス
        //=========================================================================================
        public string GetFullPath(string path) {
            return SSHUtils.GetFullPath(path);
        }

        //=========================================================================================
        // 機　能：指定されたパスからディレクトリ名部分を返す
        // 引　数：[in]fullPath  パス名
        // 戻り値：パス名のディレクトリ部分
        //=========================================================================================
        public string GetDirectoryName(string fullPath) {
            return GenericFileStringUtils.GetDirectoryName(fullPath, '/');
        }

        //=========================================================================================
        // 機　能：パスの区切り文字を返す
        // 引　数：[in]fileListCtx   ファイル一覧のコンテキスト情報
        // 戻り値：パスの区切り文字
        //=========================================================================================
        public string GetPathSeparator(IFileListContext fileListCtx) {
            return "/";
        }

        //=========================================================================================
        // プロパティ：ファイルシステムID
        //=========================================================================================
        public FileSystemID FileSystemId {
            get {
                return FileSystemID.SSHShell;
            }
        }

        //=========================================================================================
        // プロパティ：サポートするショートカットの種類
        //=========================================================================================
        public ShortcutType[] SupportedShortcutType {
            get {
                ShortcutType[] list = new ShortcutType[2];
                list[0] = ShortcutType.SymbolicLink;
                list[1] = ShortcutType.HardLink;
                return list;
            }
        }

        //=========================================================================================
        // プロパティ：ローカル実行の際、ダウンロードとアップロードが必要なときtrue
        //=========================================================================================
        public bool LocalExecuteDownloadRequired {
            get {
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：表示の際の項目一覧
        //=========================================================================================
        public FileListHeaderItem[] FileListHeaderItemList {
            get {
                FileListHeaderItem[] list = new FileListHeaderItem[4];
                list[0] = new FileListHeaderItem(FileListHeaderItem.FileListHeaderItemId.FileName,     Resources.FileListItemFileName,   "WWWWWWWWWW.WWW", true);    // ファイル名
                list[1] = new FileListHeaderItem(FileListHeaderItem.FileListHeaderItemId.FileSize,     Resources.FileListItemSize,       "999.999W",       false);   // ファイルサイズ
                list[2] = new FileListHeaderItem(FileListHeaderItem.FileListHeaderItemId.ModifiedTime, Resources.FileListItemUpdateDate, "99/99/99 99:99", false);   // 更新日時
                list[3] = new FileListHeaderItem(FileListHeaderItem.FileListHeaderItemId.Attribute,    Resources.FileListItemAttr,       "drwxrwxrwx",     false);   // 属性
                return list;
            }
        }

        //=========================================================================================
        // プロパティ：通常使用するエンコード方式
        //=========================================================================================
        public EncodingType DefaultEncoding {
            get {
                return EncodingType.UTF8;
            }
        }
    }
}
