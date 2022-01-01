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
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.UI.FileList;
using ShellFiler.Properties;

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：ファイルアクセスのためファイルを準備する処理を行う
    //=========================================================================================
    class WindowsRetrieveFileChunkProcedure {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public WindowsRetrieveFileChunkProcedure() {
        }

        //=========================================================================================
        // 機　能：処理を実行する
        // 引　数：[in]context   コンテキスト情報
        // 　　　　[in]file      アクセスしたいファイルの情報
        // 　　　　[in]fileName  実際にアクセスするファイル名
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext cache, AccessibleFile file, string fileName) {
            try {
                if (!File.Exists(fileName)) {
                    return FileOperationStatus.FileNotFound;
                }
                FileInfo fileInfo = new FileInfo(fileName);
                int bufferSize = (int)Math.Min(fileInfo.Length, 512 * 1024);
                byte[] readBuffer = new byte[bufferSize];

                FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                try {
                    long length = Math.Min((long)(file.MaxFileSize + 1), stream.Length);
                    while (stream.Position < length) {
                        int readSize = stream.Read(readBuffer, 0, readBuffer.Length);
                        if (BaseThread.CurrentThread.IsCancel) { 
                            return FileOperationStatus.Canceled;
                        }
                        if (readSize > 0) {
                            file.AddData(readBuffer, 0, readSize);
                            file.FireEvent(false);
                            if (!file.FastRead) {
                                Thread.Sleep(10);
                            }
                        }
                    }
                } finally {
                    stream.Close();
                }
                file.AddCompleted(RetrieveDataLoadStatus.CompletedAll, null);
                file.FireEvent(true);
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }
            return FileOperationStatus.Success;
        }
    }
}
