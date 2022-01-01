using System;
using System.Collections.Generic;
using System.IO;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：ファイル操作ライブラリ
    //=========================================================================================
    public class FileUtils {

        //=========================================================================================
        // 機　能：指定されたフォルダに1バイト以上を超えるファイルが存在するかどうかを返す
        // 引　数：[in]path    フォルダ
        // 　　　　[out]exist  フォルダ内に1バイト以上のファイルが存在するときtrueを返す変数
        // 戻り値：成功したときtrue
        //=========================================================================================
        public static bool IsExistNotEmpty(string path, out bool exist) {
            try {
                List<string> dirQueue = new List<string>();
                dirQueue.Add(path);
                while (dirQueue.Count > 0) {
                    string target = dirQueue[0];
                    dirQueue.RemoveAt(0);
                    DirectoryInfo dirInfo = new DirectoryInfo(target);
                    FileInfo[] fileList = dirInfo.GetFiles();
                    foreach (FileInfo file in fileList) {
                        if (file.Length > 0) {
                            exist = true;
                            return true;
                        }
                    }
                    DirectoryInfo[] dirList = dirInfo.GetDirectories();
                    foreach (DirectoryInfo dir in dirList) {
                        if (dir.Name == "." || dir.Name == "..") {
                            continue;
                        }
                        dirQueue.Add(dir.FullName);
                    }
                }
            } catch (Exception) {
                exist = false;
                return false;
            }
            exist = false;
            return true;
        }

        //=========================================================================================
        // 機　能：指定されたディレクトリが存在するかどうかを返す
        // 引　数：[in]path   調べるディレクトリのパス
        // 戻り値：ディレクトリが存在するときtrue
        //=========================================================================================
        public static bool DirectoryExists(string path) {
            int dwAttr = Win32API.GetFileAttributes(path);
            if (dwAttr == -1) {
                return false;
            }
            if ((dwAttr & Win32API.FILE_ATTRIBUTE_DIRECTORY) == Win32API.FILE_ATTRIBUTE_DIRECTORY){
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：指定されたファイルが存在するかどうかを返す
        // 引　数：[in]path   調べるファイルのパス
        // 戻り値：ファイルが存在するときtrue
        //=========================================================================================
        public static bool FileExists(string path) {
            int dwAttr = Win32API.GetFileAttributes(path);
            if (dwAttr == -1) {
                return false;
            }
            if ((dwAttr & Win32API.FILE_ATTRIBUTE_DIRECTORY) == 0){
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：指定されたディレクトリ以下をすべてコピーする
        // 引　数：[in]srcPath   転送元のパス
        // 　　　　[in]destPath  転送先のパス
        // 戻り値：すべて成功したときtrue
        //=========================================================================================
        public static bool CopyDirectory(string srcPath, string destPath) {
            bool success = false;
            // コピー先のディレクトリがないときは作る
            if (!Directory.Exists(destPath)) {
                try {
                    Directory.CreateDirectory(destPath);
                } catch (Exception) {
                    success = false;
                }
            }
            if (destPath[destPath.Length - 1] != '\\') {
                destPath = destPath + "\\";
            }

            // ファイルをコピー
            string[] files = Directory.GetFiles(srcPath);
            foreach (string file in files) {
                try {
                    File.Copy(file, destPath + Path.GetFileName(file), true);
                } catch (Exception) {
                    success = false;
                }
            }

            // ディレクトリをコピー
            string[] dirs = Directory.GetDirectories(srcPath);
            foreach (string dir in dirs) {
                try {
                    success &= CopyDirectory(dir, destPath + Path.GetFileName(dir));
                } catch (Exception) {
                    success = false;
                }
            }
            return success;
        }

        //=========================================================================================
        // 機　能：指定されたディレクトリ以下をすべて削除する
        // 引　数：[in]path  削除するディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public static void DeleteDirectory(string path) {
            try {
                DeleteDirectory(new DirectoryInfo(path));
            } catch (Exception) {
            }
        }

        //=========================================================================================
        // 機　能：指定されたディレクトリ以下をすべて削除する
        // 引　数：[in]directoryInfo  削除するディレクトリ
        // 戻り値：なし
        //=========================================================================================
        private static void DeleteDirectory(DirectoryInfo directoryInfo) {
            // すべてのファイルの読み取り専用属性を解除する
            foreach (FileInfo fileInfo in directoryInfo.GetFiles()) {
                if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                    fileInfo.Attributes = FileAttributes.Normal;
                }
            }

            // サブディレクトリ内の読み取り専用属性を解除する (再帰)
            foreach (System.IO.DirectoryInfo subDirInfo in directoryInfo.GetDirectories()) {
                DeleteDirectory(subDirInfo);
            }

            // このディレクトリの読み取り専用属性を解除する
            if ((directoryInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                directoryInfo.Attributes = FileAttributes.Directory;
            }

            // このディレクトリを削除する
            directoryInfo.Delete(true);
        }

        //=========================================================================================
        // 機　能：指定されたファイルを強制的に削除する
        // 引　数：[in]path  削除するファイル
        // 戻り値：なし
        //=========================================================================================
        public static void ForceDeleteFile(string path) {
            try {
                FileInfo fileInfo = new FileInfo(path);
                if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                    fileInfo.Attributes = FileAttributes.Normal;
                }
            } catch (Exception) {
            }
            try {
                File.Delete(path);
            } catch (Exception) {
            }
        }

        //=========================================================================================
        // 機　能：ファイルを読み取り専用にする
        // 引　数：[in]path  読み取り専用にするファイル
        // 戻り値：なし
        //=========================================================================================
        public static void SetFileReadonly(string path) {
            try {
                FileInfo fileInfo = new FileInfo(path);
                if ((fileInfo.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly) {
                    fileInfo.Attributes |= FileAttributes.ReadOnly;
                }
            } catch (Exception) {
            }
        }
    }
}
