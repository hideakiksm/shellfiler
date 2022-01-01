using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.UI.Log;

namespace ShellFiler.FileSystem.Transfer {

    //=========================================================================================
    // クラス：SFTP→SFTPでのファイル操作API
    //=========================================================================================
    class SFTPToSFTPFileSystem : IFileSystemToFileSystem {
        // 転送先のSSHファイルシステム
        private SFTPFileSystem m_sshFileSystem;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]sshFileSystem  転送元/転送先のSSHファイルシステム
        // 戻り値：なし
        //=========================================================================================
        public SFTPToSFTPFileSystem(SFTPFileSystem sshFileSystem) {
            m_sshFileSystem = sshFileSystem;
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
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus CopyFile(FileOperationRequestContext context, string srcFilePath, IFile srcFileInfoAttr, string destFilePath, bool overwrite, AttributeSetMode attrMode, FileFilterTransferSetting fileFilter, FileProgressEventHandler progress) {
            FileOperationStatus status;
            bool sameSession = SSHUtils.IsSameSSHSession(srcFilePath, destFilePath);
            if (sameSession && fileFilter == null) {
                // 同じセッションで処理
                bool attrCopy = false;
                if (attrMode != null) {
                    attrCopy = attrMode.IsSetAttribute(m_sshFileSystem.FileSystemId);
                }
                status = m_sshFileSystem.RemoteToRemoteFileTransfer(context, TransferModeType.CopyFile, srcFilePath, destFilePath, overwrite, attrCopy, fileFilter, progress);
                return status;
            } else {
                // 違うセッションで処理
                IFileSystemToFileSystem trans = Program.Document.FileSystemFactory.CreateTransferFileSystem(FileSystemID.SFTP, FileSystemID.SFTP, true);
                status = trans.CopyFile(context, srcFilePath, srcFileInfoAttr, destFilePath, overwrite, attrMode, fileFilter, progress);
                return status;
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
            FileOperationStatus status;
            bool sameSession = SSHUtils.IsSameSSHSession(srcFilePath, destFilePath);
            if (sameSession) {
                // 同じセッションで処理
                bool attrCopy = false;
                if (attrMode != null) {
                    attrCopy = attrMode.IsSetAttribute(m_sshFileSystem.FileSystemId);
                }
                status = m_sshFileSystem.RemoteToRemoteFileTransfer(context, TransferModeType.MoveFile, srcFilePath, destFilePath, overwrite, attrCopy, null, progress);
                return status;
            } else {
                // 違うセッションで処理
                IFileSystemToFileSystem trans = Program.Document.FileSystemFactory.CreateTransferFileSystem(FileSystemID.SFTP, FileSystemID.SFTP, true);
                status = trans.MoveFileDirectory(context, srcFilePath, srcFileInfoAttr, destFilePath, overwrite, attrMode, progress);
                return status;
            }
        }

        //=========================================================================================
        // 機　能：ディレクトリの直接コピー／移動をサポートするかどうかを確認する
        // 引　数：[in]srcDirPath   転送元ディレクトリ名のフルパス
        // 　　　　[in]destDirPath  転送先ディレクトリ名のフルパス
        // 　　　　[in]isCopy       コピーのときtrue、移動のときfalse
        // 戻り値：直接の移動ができるときtrue（trueになっても失敗することはある）
        //=========================================================================================
        public bool CanMoveDirectoryDirect(string srcDirPath, string destDirPath, bool isCopy) {
            bool sameSession = SSHUtils.IsSameSSHSession(srcDirPath, destDirPath);
            return sameSession;
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
            bool attrCopy = attrMode.IsSetAttribute(m_sshFileSystem.FileSystemId);
            return m_sshFileSystem.RemoteCopyDirectoryDirect(context, srcPath, destPath, attrCopy, progress);
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
            return m_sshFileSystem.CreateShortcut(context, srcFilePath, destFilePath, overwrite, type);
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
            bool copyAttr = attrMode.IsSetAttribute(m_sshFileSystem.FileSystemId);
            if (!copyAttr) {
                return FileOperationStatus.Success;
            }

            // 転送元の属性を取得
            if (srcFileInfo == null) {
                status = m_sshFileSystem.GetFileInfo(context, srcFilePath, true, out srcFileInfo);
                if (!status.Succeeded) {
                    return status;
                }
            }

            // 属性を設定
            status = m_sshFileSystem.SetFileInfo(context, srcFileInfo, destFilePath, copyAttr, copyAttr);
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
            bool sameSession = SSHUtils.IsSameSSHSession(srcFilePath[0], destFilePath);
            if (sameSession) {
                // 同じセッションで処理
                status = m_sshFileSystem.CombineFile(context, srcFilePath, destFilePath, taskLogger);
                return status;
            } else {
                // 違うセッションで処理
                IFileSystemToFileSystem trans = Program.Document.FileSystemFactory.CreateTransferFileSystem(FileSystemID.SFTP, FileSystemID.SFTP, true);
                status = trans.CombineFile(context, srcFilePath, destFilePath, taskLogger);
                return status;
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
            bool sameSession = SSHUtils.IsSameSSHSession(srcFilePath, destFolderPath);
            if (sameSession) {
                // 同じセッションで処理
                status = m_sshFileSystem.SplitFile(context, srcFilePath, destFolderPath, numberingInfo, splitInfo, taskLogger);
                return status;
            } else {
                // 違うセッションで処理
                IFileSystemToFileSystem trans = Program.Document.FileSystemFactory.CreateTransferFileSystem(FileSystemID.SFTP, FileSystemID.SFTP, true);
                status = trans.SplitFile(context, srcFilePath, destFolderPath, numberingInfo, splitInfo, taskLogger);
                return status;
            }
        }
    }
}
