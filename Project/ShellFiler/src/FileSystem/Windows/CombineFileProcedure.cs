using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Util;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.Properties;
using ShellFiler.FileTask;
using ShellFiler.UI.Log;

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：ファイルを結合するプロシージャ
    //=========================================================================================
    class CombineFileProcedure {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CombineFileProcedure() {
        }

        //=========================================================================================
        // 機　能：ファイルを結合する
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]srcPathList     転送元ファイル名のフルパス
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]taskLogger      ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext context, List<string> srcPathList, string destFilePath, ITaskLogger taskLogger) {
            FileOperationStatus status;

            // 上書きを確認
            bool exist = WindowsFileUtils.IsExistFile(destFilePath);
            if (exist) {
                taskLogger.LogFileOperationStart(FileOperationType.CombineFile, destFilePath, false);
                return taskLogger.LogFileOperationEnd(FileOperationStatus.AlreadyExists);
            }

            // 転送先を用意
            try {
                bool successAll = false;
                FileStream outputStream = new FileStream(destFilePath, FileMode.Create, FileAccess.Write);
                try {
                    for (int i = 0; i < srcPathList.Count; i++) {
                        status = AppendFile(context, srcPathList[i], outputStream, taskLogger);
                        if (!status.Succeeded) {
                            break;
                        }
                        if (i == srcPathList.Count - 1) {
                            successAll = true;
                        }
                    }
                } finally {
                    outputStream.Close();
                    if (!successAll) {
                        try {
                            File.Delete(destFilePath);
                        } catch (Exception) {
                        }
                    }
                }
            } catch (Exception e) {
                taskLogger.LogFileOperationStart(FileOperationType.CombineFile, destFilePath, false);
                return taskLogger.LogFileOperationEnd(FileOperationStatus.FromException(e, FileOperationStatus.Fail));
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ファイル１件を追加する
        // 引　数：[in]context         コンテキスト情報
        //         [in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]destStream      転送先ファイル名のフルパス
        // 　　　　[in]taskLogger      ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus AppendFile(FileOperationRequestContext context, string srcFilePath, FileStream destStream, ITaskLogger taskLogger) {
            // 転送を実行
            bool isDestWriting = false;
            try {
                taskLogger.LogFileOperationStart(FileOperationType.CombineFile, srcFilePath, false);
                long transfered = 0;
                FileStream inputStream = new FileStream(srcFilePath, FileMode.Open, FileAccess.Read);
                long fileSize = inputStream.Length;
                try {
                    ProgressEvent(taskLogger, fileSize, transfered);
                    byte[] readBuffer = new byte[65536];
                    while (true) {
                        int readSize = inputStream.Read(readBuffer, 0, readBuffer.Length);
                        if (readSize == 0) {
                            break;
                        }

                        isDestWriting = true;
                        destStream.Write(readBuffer, 0, readSize);
                        isDestWriting = false;
                     
                        transfered += readSize;
                        ProgressEvent(taskLogger, fileSize, transfered);

                        if (context.IsCancel) {
                            return taskLogger.LogFileOperationEnd(FileOperationStatus.Canceled);
                        }
                    }
                    taskLogger.LogFileOperationEnd(FileOperationStatus.Success);
                } finally {
                    inputStream.Close();
                }
            } catch (Exception e) {
                if (isDestWriting) {
                    // 転送先でエラー
                    return taskLogger.LogFileOperationEnd(FileOperationStatus.Canceled);
                    throw e;
                } else {
                    // 転送元でエラー
                    return taskLogger.LogFileOperationEnd(FileOperationStatus.FromException(e, FileOperationStatus.Fail));
                }
            }

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：進捗状況を表示する
        // 引　数：[in]taskLogger  ログ出力クラス
        // 　　　　[in]fileSize    ファイルサイズ
        // 　　　　[in]transfered  転送済みサイズ
        // 戻り値：なし
        //=========================================================================================
        private void ProgressEvent(ITaskLogger taskLogger, long fileSize, long transfered) {
            taskLogger.Progress.SetProgress(this, new FileProgressEventArgs(fileSize, transfered));
        }
    }
}
