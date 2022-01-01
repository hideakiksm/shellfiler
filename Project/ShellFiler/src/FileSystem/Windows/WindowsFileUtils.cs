using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.Properties;

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：ファイル操作のユーティリティ
    //=========================================================================================
    class WindowsFileUtils {

        //=========================================================================================
        // 機　能：ファイルが存在するかどうかを調べる
        // 引　数：[in]filePath   調査対象のファイルパス
        // 戻り値：ファイルが見つかったときtrue（ディレクトリが見つかったときfalse）
        //=========================================================================================
        public static bool IsExistFile(string filePath) {
            bool exist = false;
            try {
                File.GetAttributes(filePath);
                exist = true;
            } catch (FileNotFoundException) {
            } catch (DirectoryNotFoundException) {
            }
            return exist;
        }

        //=========================================================================================
        // 機　能：ファイルサイズを返す
        // 引　数：[in]filePath   調査対象のファイルパス
        // 戻り値：ファイルサイズ（取得できないとき-1）
        //=========================================================================================
        public static long GetFileLength(string filePath) {
            long fileSize = -1;
            try {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                try {
                    fileSize = stream.Length;
                } finally {
                    stream.Close();
                }
            } catch (FileNotFoundException) {
            } catch (DirectoryNotFoundException) {
            }
            return fileSize;
        }
 
        //=========================================================================================
        // 機　能：パスとファイルを連結する
        // 引　数：[in]dir  ディレクトリ名
        // 　　　　[in]file ファイル名
        // 戻り値：連結したファイル名
        //=========================================================================================
        public static string CombineFilePath(string dir, string file) {
            if (dir.EndsWith("\\")) {
                return dir + file;
            } else {
                return dir + "\\" + file;
            }
        }
    }
}
