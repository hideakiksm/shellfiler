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
    // クラス：画像を取得する処理を行う
    //=========================================================================================
    class VirtualRetrieveImageProcedure {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public VirtualRetrieveImageProcedure() {
        }

        //=========================================================================================
        // 機　能：処理を実行する
        // 引　数：[in]context  コンテキスト情報
        // 　　　　[in]filePath  読み込み対象のファイルパス
        // 　　　　[in]maxSize   読み込む最大サイズ
        // 　　　　[out]image    読み込んだ画像を返す変数
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext context, string filePath, long maxSize, out BufferedImage image) {
            FileOperationStatus status;
            image = null;

            string tempFile;
            status = ExtractRuntime.ExtractVirtualStoreFile(context, filePath, out tempFile);
            if (!status.Succeeded) {
                return status;
            }

            WindowsRetrieveImageProcedure procedure = new WindowsRetrieveImageProcedure();
            status = procedure.Execute(context, tempFile, maxSize, out image);
            if (!status.Succeeded) {
                return status;
            }

            return FileOperationStatus.Success;
        }
    }
}
