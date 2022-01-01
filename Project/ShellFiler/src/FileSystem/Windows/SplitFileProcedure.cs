using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Util;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.Properties;
using ShellFiler.FileTask;
using ShellFiler.UI.Log;
using ShellFiler.Virtual;

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：ファイルを分割するプロシージャ
    //=========================================================================================
    class SplitFileProcedure {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SplitFileProcedure() {
        }

        //=========================================================================================
        // 機　能：ファイルを分割する
        // 引　数：[in]context              コンテキスト情報
        // 　　　　[in]srcFilePath          転送元ファイル名のフルパス
        // 　　　　[in]srcFilePathOrg       転送元ファイル名のフルパス（一時領域へのダウンロード時は元のファイル名）
        // 　　　　[in]destFolderPath       転送先フォルダ名のフルパス（最後は「\」）
        // 　　　　[in]numberingInfo        ファイルの連番の命名規則
        // 　　　　[in]splitInfo            ファイルの分割方法
        // 　　　　[in]splitDestTempHolder  転送先をテンポラリにするときの情報（テンポラリにしないときnull）
        // 　　　　[in]taskLogger           ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext context, string srcFilePath, string srcFilePathOrg, string destFolderPath, RenameNumberingInfo numberingInfo, SplitFileInfo splitInfo, SplitDestTempHolder splitDestTempHolder, ITaskLogger taskLogger) {
            FileOperationStatus status;
            
            // 転送元を用意
            FileStream inputStream;
            try {
                inputStream = new FileStream(srcFilePath, FileMode.Open, FileAccess.Read);
            } catch (Exception e) {
                taskLogger.LogFileOperationStart(FileOperationType.SplitFile, srcFilePathOrg, false);
                return taskLogger.LogFileOperationEnd(FileOperationStatus.FromException(e, FileOperationStatus.FailRead));
            }

            string destFileTemplate = destFolderPath + GenericFileStringUtils.GetFileName(srcFilePathOrg);
            ModifyFileInfoContext modifyCtx = new ModifyFileInfoContext();
            long fileLength = inputStream.Length;
            try {
                long oneFileSize;
                int fileCount;
                bool success = splitInfo.GetOneFileSize(fileLength, FileSystemID.Windows, FileSystemID.Windows, out oneFileSize, out fileCount);
                if (!success) {
                    taskLogger.LogFileOperationStart(FileOperationType.SplitFile, srcFilePathOrg, false);
                    return taskLogger.LogFileOperationEnd(FileOperationStatus.TooManyFiles);
                }
                for (int index = 0; index < fileCount; index++) {
                    string destFilePathFinal = RenameNumberingInfo.CreateSequenceFileName(destFileTemplate, numberingInfo, modifyCtx);
                    string destFilePath;
                    if (splitDestTempHolder != null) {
                        destFilePath = splitDestTempHolder.AddTemporaryMapping(GenericFileStringUtils.GetFileName(destFilePathFinal));
                    } else {
                        destFilePath = destFilePathFinal;
                    }

                    // 上書きを確認
                    bool exist = WindowsFileUtils.IsExistFile(destFilePath);
                    if (exist) {
                        taskLogger.LogFileOperationStart(FileOperationType.SplitFile, destFilePathFinal, false);
                        return taskLogger.LogFileOperationEnd(FileOperationStatus.AlreadyExists);
                    }

                    // 転送
                    status = SplitFile(context, inputStream, destFilePath, destFilePathFinal, oneFileSize, taskLogger);
                    if (!status.Succeeded) {
                        break;
                    }
                }
            } finally {
                inputStream.Close();
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ファイル１件を追加する
        // 引　数：[in]context           コンテキスト情報
        //         [in]inputStream       入力ファイル
        // 　　　　[in]destFilePath      転送先ファイル名のフルパス（物理的なファイル）
        // 　　　　[in]destFilePathFinal 転送先ファイルの最終的な格納先ファイル名
        // 　　　　[in]oneFileSize       ファイル１件分のサイズ
        // 　　　　[in]taskLogger        ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus SplitFile(FileOperationRequestContext context, FileStream inputStream, string destFilePath, string destFilePathFinal, long oneFileSize, ITaskLogger taskLogger) {
            // 転送を実行
            bool isDestWriting = false;
            try {
                taskLogger.LogFileOperationStart(FileOperationType.SplitFile, destFilePathFinal, false);
                FileStream outputStream = new FileStream(destFilePath, FileMode.CreateNew, FileAccess.Write);
                try {
                    long remainSize = oneFileSize;
                    byte[] readBuffer = new byte[65536];
                    while (true) {
                        int readRequestSize = (int)(Math.Min(remainSize, readBuffer.Length));
                        int readSize = 0;
                        if (readRequestSize > 0) {
                            readSize = inputStream.Read(readBuffer, 0, readRequestSize);
                            remainSize -= readSize;
                        }

                        isDestWriting = true;
                        outputStream.Write(readBuffer, 0, readSize);
                        isDestWriting = false;
                        ProgressEvent(taskLogger, oneFileSize, oneFileSize - remainSize);

                        if (readSize == 0 || remainSize == 0) {
                            break;
                        }
                        if (context.IsCancel) {
                            throw new SfCancelException();
                        }
                    }
                    taskLogger.LogFileOperationEnd(FileOperationStatus.Success);
                } finally {
                    outputStream.Close();
                }
            } catch (Exception e) {
                try {
                    File.Delete(destFilePath);
                } catch (Exception) {
                }
                if (e is SfCancelException) {
                    return taskLogger.LogFileOperationEnd(FileOperationStatus.Canceled);
                } else if (!isDestWriting) {
                    // 転送元でエラー
                    taskLogger.LogFileOperationEnd(FileOperationStatus.Canceled);
                    throw e;
                } else {
                    // 転送先でエラー
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

        //=========================================================================================
        // クラス：転送先をテンポラリとするときのファイル名対応情報
        //=========================================================================================
        public class SplitDestTempHolder {
            // テンポラリファイルの保持クラス
            private TempFileHolder m_tempFileHolder;

            // テンポラリファイルと実ファイル名との対応付け（テンポラリのファイルパス→ファイル名）
            private List<RenameFilePathInfo> m_listRenameFilePath = new List<RenameFilePathInfo>();

            //=========================================================================================
            // 機　能：進捗状況を表示する
            // 引　数：[in]taskLogger  ログ出力クラス
            // 　　　　[in]fileSize    ファイルサイズ
            // 　　　　[in]transfered  転送済みサイズ
            // 戻り値：なし
            //=========================================================================================
            public SplitDestTempHolder(TempFileHolder tempFileHolder) {
                m_tempFileHolder = tempFileHolder;
            }

            //=========================================================================================
            // 機　能：テンポラリファイルのマッピング情報を追加する
            // 引　数：[in]finalName  最終的なファイル名（リネーム後の名前、パスなし）
            // 戻り値：テンポラリファイルのファイルパス
            //=========================================================================================
            public string AddTemporaryMapping(string finalName) {
                // 分割ファイルとして最終的にfinalNameとするファイルは、
                // テンポラリフォルダ上でこのメソッドの戻り値のファイルパスとして作成する
                // 分割完了後、テンポラリから最終的なファイル名にアップロード
                string tempFilePath = m_tempFileHolder.CreateNew();
                RenameFilePathInfo renamePath = new RenameFilePathInfo(tempFilePath, finalName);
                m_listRenameFilePath.Add(renamePath);
                return tempFilePath;
            }

            //=========================================================================================
            // テンポラリファイルの保持クラス
            //=========================================================================================
            public TempFileHolder TempFileHolder {
                get {
                    return m_tempFileHolder;
                }
            }

            //=========================================================================================
            // テンポラリファイルと実ファイル名との対応付け
            //=========================================================================================
            public List<RenameFilePathInfo> RenameFilePathList {
                get {
                    return m_listRenameFilePath;
                }
            }
        }
    }
}
