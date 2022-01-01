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

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：フィルター付きでファイルをコピーするプロシージャ
    //=========================================================================================
    class FilterCopyProcedure {
        // 転送元ファイルサイズ
        private long m_srcFileSize;

        // 転送先ファイルサイズ
        private long m_destFileSize;

        // 進捗状態を通知するdelegate
        private FileProgressEventHandler m_progress;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FilterCopyProcedure() {
        }

        //=========================================================================================
        // 機　能：ファイルをコピーする
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]overwrite     上書きするときtrue
        // 　　　　[in]fileFilter    転送時に使用するフィルター
        // 　　　　[in]progress      進捗状態を通知するdelegate
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext context, string srcFilePath, string destFilePath, bool overwrite, FileFilterTransferSetting fileFilter, FileProgressEventHandler progress) {
            m_progress = progress;

            // 上書きを確認
            bool exist = false;
            try {
                FileAttributes attr = File.GetAttributes(destFilePath);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
                    return FileOperationStatus.CanNotAccess;
                }
                exist = true;
            } catch (FileNotFoundException) {
            } catch (DirectoryNotFoundException) {
            }

            if (!overwrite && exist) {
                return FileOperationStatus.AlreadyExists;
            }

            // 準備
            try {
                m_srcFileSize = new FileInfo(srcFilePath).Length;
                m_destFileSize = m_srcFileSize;
            } catch (Exception) {
                return FileOperationStatus.FileNotFound;
            }
            ProgressEvent(false, 0);

            if (m_srcFileSize > int.MaxValue) {
                return FileOperationStatus.TooBigFile;
            }

            // 転送元
            FileOperationStatus status;
            try {
                status = FilterCopy(context, srcFilePath, destFilePath, fileFilter);
            } catch (OutOfMemoryException) {
                return FileOperationStatus.ErrorOutOfMemory;
            }

            return status;
        }

        //=========================================================================================
        // 機　能：ファイルにフィルターを適用する
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]fileFilter    転送時に使用するフィルター
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus FilterCopy(FileOperationRequestContext context, string srcFilePath, string destFilePath, FileFilterTransferSetting fileFilter) {
            // 転送元を読み込み
            FileOperationStatus status;
            byte[] srcData;
            try {
                MemoryStream memory = new MemoryStream((int)m_srcFileSize);
                long transfered = 0;
                FileStream inputStream = new FileStream(srcFilePath, FileMode.Open, FileAccess.Read);
                try {
                    byte[] readBuffer = new byte[65536];
                    while (true) {
                        int readSize = inputStream.Read(readBuffer, 0, readBuffer.Length);
                        if (readSize == 0) {
                            break;
                        }
                        transfered += readSize;
                        ProgressEvent(false, transfered);
                        memory.Write(readBuffer, 0, readSize);
                    }
                } finally {
                    inputStream.Close();
                }
                srcData = memory.ToArray();
                memory.Close();
                memory.Dispose();
            } catch (FileNotFoundException) {
                return FileOperationStatus.FileNotFound;
            } catch (SecurityException) {
                return FileOperationStatus.CanNotAccess;
            } catch (OutOfMemoryException e) {
                throw e;
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }

            // フィルターを適用
            FileFilterManager filterManager = Program.Document.FileFilterManager;
            byte[] destData;
            status = filterManager.ConvertWithTransferSetting(srcFilePath, srcData, out destData, fileFilter, context.CancelEvent);
            srcData = null;
            if (status != FileOperationStatus.Success) {
                return status;      // Skipの場合はここで終わる
            }
            m_destFileSize = destData.Length;

            // 転送先に書き込み
            try {
                FileStream outputStream = new FileStream(destFilePath, FileMode.Create, FileAccess.Write);
                try {
                    const int WRITE_SIZE = 65536;
                    for (int i = 0; i < destData.Length; i += WRITE_SIZE) {
                        int chunkSize = Math.Min(WRITE_SIZE, destData.Length - i);
                        outputStream.Write(destData, i, chunkSize);
                        ProgressEvent(true, i + chunkSize);
                    }
                } finally {
                    outputStream.Close();
                }
            } catch (SecurityException) {
                return FileOperationStatus.CanNotAccess;
            } catch (OutOfMemoryException e) {
                throw e;
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：進捗状況を通知する
        // 引　数：[in]isDest      転送先を処理中のときtrue、転送元を処理中のときfalse
        // 　　　　[in]transfered  転送元、転送先それぞれで転送したバイト数
        // 戻り値：なし
        //=========================================================================================
        private void ProgressEvent(bool isDest, long transfered) {
            long transArg;
            if (isDest) {
                transArg = m_srcFileSize + transfered;
            } else {
                transArg = transfered;
            }
            FileProgressEventArgs args = new FileProgressEventArgs(m_srcFileSize + m_destFileSize, transArg);
            m_progress.SetProgress(this, args);
        }
    }
}
