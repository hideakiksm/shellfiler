using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Util;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // クラス：イメージの読み込みクラス
    //=========================================================================================
    public class ImageLoader {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ImageLoader() {
        }

        //=========================================================================================
        // 機　能：メモリ上のバッファからイメージを読み込む
        // 引　数：[in]buffer     バッファ
        // 　　　　[out]image     読み込んだ画像を返す変数
        // 　　　　[out]colorBits 1ピクセルあたりのビット数
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus LoadImage(byte[] buffer, out BufferedImage image, out int colorBits) {
            image = null;
            colorBits = -1;
            try {
                image = new BufferedImage();
                image.Stream = new MemoryStream(buffer);
                image.Image = Image.FromStream(image.Stream, false, false);
            } catch (OutOfMemoryException) {
                return FileOperationStatus.ErrorOutOfMemory;
            } catch (Exception) {
                return FileOperationStatus.ErrorConvertImage;
            }
            colorBits = Image.GetPixelFormatSize(image.Image.PixelFormat);
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：読み込み可能なファイルかどうかを返す
        // 引　数：[in]fileName  ファイル名
        // 戻り値：読み込み可能なファイルのときtrue
        //=========================================================================================
        public static bool IsTargetFile(string fileName) {
            string fileLower = fileName.ToLower();
            if (fileLower.EndsWith(".bmp") || fileLower.EndsWith(".jpg") || fileLower.EndsWith(".png") || fileLower.EndsWith(".gif")) {
                return true;
            } else {
                return false;
            }
        }
    }
}
