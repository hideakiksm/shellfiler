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
    // クラス：画像を取得する処理を行う
    //=========================================================================================
    class WindowsRetrieveImageProcedure {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public WindowsRetrieveImageProcedure() {
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
            image = null;
            try {
                if (!File.Exists(filePath)) {
                    return FileOperationStatus.FileNotFound;
                }

                FileInfo fi = new FileInfo(filePath);
                long fileSize = fi.Length;
                if (fileSize > maxSize) {
                    return FileOperationStatus.ErrorTooLarge;
                }
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }
            try {
                image = new BufferedImage();
                image.Stream = new MemoryStream(File.ReadAllBytes(filePath));
                image.Image = Image.FromStream(image.Stream, false, false);
//                image = new Bitmap(stream);
            } catch (Exception) {
                image.Dispose();
                image = null;
                return FileOperationStatus.ErrorConvertImage;
            }
            return FileOperationStatus.Success;
        }
    }
}
