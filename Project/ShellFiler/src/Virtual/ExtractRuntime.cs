using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Locale;
using ShellFiler.Properties;
using ShellFiler.UI.FileList;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;
using ShellFiler.Util;

namespace ShellFiler.Virtual {

    //=========================================================================================
    // クラス：ファイル展開のランタイム処理を行うクラス
    //=========================================================================================
    class ExtractRuntime {

        //=========================================================================================
        // 機　能：指定されたファイルを展開する
        // 引　数：[in]context    コンテキスト情報
        // 　　　　[in]dispFile   展開するファイルの表示パス（E:\dir\arc.zip\dir2\file.txt）
        // 　　　　[in]tempFile   一時フォルダへの展開結果を返す変数
        // 戻り値：ステータス
        //=========================================================================================
        public static FileOperationStatus ExtractVirtualStoreFile(FileOperationRequestContext context, string dispFile, out string tempFile) {
            tempFile = null;
            List<string> targetNameList = new List<string>();
            targetNameList.Add(dispFile);
            List<string> tempFileList;
            FileOperationStatus status = ExtractVirtualStoreMultiFiles(context, targetNameList, out tempFileList);
            if (!status.Succeeded) {
                return status;
            }
            tempFile = tempFileList[0];
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：指定されたファイルを展開する
        // 引　数：[in]context        コンテキスト情報
        // 　　　　[in]dispFileList   展開するファイルの表示パスのリスト（E:\dir\arc.zip\dir2\file.txt）
        // 　　　　[out]tempFileList  一時フォルダへの展開結果を返す変数
        // 戻り値：ステータス
        //=========================================================================================
        public static FileOperationStatus ExtractVirtualStoreMultiFiles(FileOperationRequestContext context, List<string> dispFileList, out List<string> tempFileList) {
            ITaskLogger logger = new AutoTaskLogger();
            FileOperationStatus status = ExtractVirtualStoreMultiFilesExec(context, logger, dispFileList, out tempFileList);
            if (logger.LogCount > 0) {
                LogLineSimple log;
                if (status.Succeeded) {
                    log = new LogLineSimple(Resources.Log_VirtualExtractSuccess);
                } else {
                    log = new LogLineSimple(LogColor.Error, Resources.Log_VirtualExtractFailed);
                }
                Program.LogWindow.RegistLogLine(log);
            }
            return status;
        }

        //=========================================================================================
        // 機　能：指定されたファイルを展開する（実処理）
        // 引　数：[in]context        コンテキスト情報
        // 　　　　[in]logger         ログの出力先
        // 　　　　[in]dispFileList   展開するファイルの表示パスのリスト（E:\dir\arc.zip\dir2\file.txt）
        // 　　　　[out]tempFileList  一時フォルダへの展開結果を返す変数
        // 戻り値：ステータス
        //=========================================================================================
        public static FileOperationStatus ExtractVirtualStoreMultiFilesExec(FileOperationRequestContext context, ITaskLogger logger, List<string> dispFileList, out List<string> tempFileList) {
            FileOperationStatus status;

            // 仮想フォルダを用意
            tempFileList = null;
            IFileListContext fileListContext = context.SrcFileListContext;
            VirtualFolderInfo virtualFolder = null;
            if (fileListContext is VirtualFileListContext) {
                virtualFolder = ((VirtualFileListContext)fileListContext).VirtualFolderInfo;
            }
            if (virtualFolder == null) {
                Program.Abort("仮想フォルダ情報がセットされていません。");
            }

            VirtualFolderArchiveInfo virtualArchive = virtualFolder.MostInnerArchive;
            string virtualExecRoot = Program.Document.TemporaryManager.VirtualManager.CreateVirtualExecuteFolder(virtualFolder);
            if (virtualExecRoot == null) {
                logger.LogFileOperationStart(FileOperationType.OpenArc, virtualArchive.DisplayPathArchive, false);
                return logger.LogFileOperationEnd(FileOperationStatus.ErrorMkDir);
            }

            // アーカイブを開く
            string password = virtualArchive.Password;
            string realArcFile = virtualArchive.RealArchiveFile;
            string clipboardString = OSUtils.GetClipboardString();
            ArchiveAutoPasswordSetting autoPasswordSetting = Program.Document.UserSetting.ArchiveAutoPasswordSetting;
            ArchivePasswordSource passwordSource = new ArchivePasswordSource(autoPasswordSetting, realArcFile, clipboardString, password);
            Form parentDialog = context.ParentDialog;
            IArchiveVirtualExtract archiveExtract = Program.Document.ArchiveFactory.CreateVirtualExtract(realArcFile, passwordSource, parentDialog);
            try {
                status = archiveExtract.Open();
                if (status.Failed) {
                    logger.LogFileOperationStart(FileOperationType.OpenArc, virtualArchive.DisplayPathArchive, false);
                    return logger.LogFileOperationEnd(status);
                }
                virtualArchive.Password = archiveExtract.UsedPassword;

                // 転送元と転送先を決定
                List<VirtualArchiveFileMapping> mappingList = GetArchiveFileInfoList(dispFileList, virtualArchive);

                // 目的のファイルを展開
                BackgroundWaitCallback waitCallback = context.VirtualRequestContext.VirtualExtractDialogCallback;
                status = archiveExtract.ExtractTemporary(mappingList, virtualExecRoot, logger, waitCallback);
                if (!status.Succeeded) {
                    return status;
                }
                tempFileList = GetResultFileInfo(mappingList, virtualExecRoot, logger);
                if (tempFileList == null) {
                    return FileOperationStatus.FileNotFound;
                }

                // パスワードのログを出力
                if (archiveExtract.UsedPasswordDisplayName != null) {
                    string message = string.Format(Resources.Log_ExtractPassword, archiveExtract.UsedPasswordDisplayName);
                    Program.LogWindow.RegistLogLineHelper(message);
                }
            } finally {
                archiveExtract.Dispose();
            }

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：要求されたファイル一覧を仮想フォルダのマッピング情報に変換する
        // 引　数：[in]dispFileList    展開するファイルの表示パスのリスト（E:\dir\arc.zip\dir2\file.txt）
        // 　　　　[in]virtualArchive  アーカイブ情報
        // 戻り値：作成したマッピング
        //=========================================================================================
        public static List<VirtualArchiveFileMapping> GetArchiveFileInfoList(List<string> dispFileList, VirtualFolderArchiveInfo virtualArchive) {
            string dispArcFile = virtualArchive.DisplayPathArchive;
            string realArcFile = virtualArchive.RealArchiveFile;
            List<VirtualArchiveFileMapping> result = new List<VirtualArchiveFileMapping>();
            for (int i = 0; i < dispFileList.Count; i++) {
                string dispFile = dispFileList[i];
                string archiveLocalPath = dispFile.Substring(dispArcFile.Length);     // 先頭の「\」はなし
                archiveLocalPath = archiveLocalPath.Replace('/', '\\');
                result.Add(new VirtualArchiveFileMapping(dispFile, archiveLocalPath));
            }
            return result;
        }

        //=========================================================================================
        // 機　能：要求されたファイル1件をアーカイブ内のファイルパスに変換する
        // 引　数：[in]dispFile        展開するファイルの表示パス（E:\dir\arc.zip\dir2\file.txt）
        // 　　　　[in]virtualArchive  アーカイブ情報
        // 戻り値：作成したマッピング
        //=========================================================================================
        public static string GetArchiveFileInfo(string dispFile, VirtualFolderArchiveInfo virtualArchive) {
            string dispArcFile = virtualArchive.DisplayPathArchive;
            string archiveLocalPath = dispFile.Substring(dispArcFile.Length);     // 先頭の「\」はなし
            archiveLocalPath = archiveLocalPath.Replace('/', '\\');
            return archiveLocalPath;
        }

        //=========================================================================================
        // 機　能：要求されたファイル一覧を仮想フォルダのマッピング情報に変換する
        // 引　数：[in]dispFileList    展開するファイルの表示パスのリスト（E:\dir\arc.zip\dir2\file.txt）
        // 　　　　[in]virtualArchive  アーカイブ情報
        // 戻り値：作成したマッピング
        //=========================================================================================
        private static List<string> GetResultFileInfo(List<VirtualArchiveFileMapping> mapping, string virtualExecRoot, ITaskLogger logger) {
            bool notFound = false;
            List<string> result = new List<string>();
            for (int i = 0; i < mapping.Count; i++) {
                if (mapping[i].Extracted) {
                    result.Add(virtualExecRoot + mapping[i].ArchiveLocalPath);
                } else {
                    notFound = true;
                    string file = GenericFileStringUtils.GetFileName(mapping[i].ArchiveLocalPath);
                    logger.LogFileOperationStart(FileOperationType.ExtractFile, file, false);
                    logger.LogFileOperationEnd(FileOperationStatus.FileNotFound);
                }
            }
            if (notFound) {
                return null;
            }
            return result;
        }
    }
}
