using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Windows;
using ShellFiler.Locale;
using ShellFiler.Properties;
using ShellFiler.UI.FileList;
using ShellFiler.Util;
using ShellFiler.Virtual;

namespace ShellFiler.FileSystem.Virtual {

    //=========================================================================================
    // クラス：ファイルアクセスのためファイルを準備する処理を行う
    //=========================================================================================
    class VirtualRetrieveFileChunkProcedure {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public VirtualRetrieveFileChunkProcedure() {
        }

        //=========================================================================================
        // 機　能：処理を実行する
        // 引　数：[in]context  コンテキスト情報
        // 　　　　[in]file     アクセスしたいファイルの情報
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext context, AccessibleFile file) {
            FileOperationStatus status;
            string tempFile;
            status = ExtractRuntime.ExtractVirtualStoreFile(context, file.FilePath, out tempFile);
            if (!status.Succeeded) {
                return status;
            }

            WindowsRetrieveFileChunkProcedure procedure = new WindowsRetrieveFileChunkProcedure();
            status = procedure.Execute(context, file, tempFile);
            if (!status.Succeeded) {
                return status;
            }

            return FileOperationStatus.Success;
        }
    }
}
