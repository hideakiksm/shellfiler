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
    // クラス：ファイルとフォルダの操作を選択的に実行するユーティリティ
    //=========================================================================================
    class WindowsFileFolderSelectUtils {
        // ディレクトリを操作の対象とするときtrue
        private bool m_isDirectory;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]isDir   ディレクトリを操作の対象とするときtrue
        // 戻り値：なし
        //=========================================================================================
        public WindowsFileFolderSelectUtils(bool isDir) {
            m_isDirectory = isDir;
        }

        //=========================================================================================
        // 機　能：ディレクトリの名前を変更する
        // 引　数：[in]oldFile   旧ファイル名
        // 　　　　[in]newFile   新ファイル名
        // 　　　　[in]baseDir   ディレクトリが存在するディレクトリのフルパス
        // 戻り値：ステータス（成功のときSuccessDelDir）
        //=========================================================================================
        public FileOperationStatus RenameFile(string oldFile, string newFile, string baseDir) {
            string oldPath = baseDir + oldFile;
            string newPath = baseDir + newFile;
            try {
                // 実ファイルで存在確認
                if (Directory.Exists(oldPath)) {
                    if (m_isDirectory) {
                        DirectoryInfo di = new DirectoryInfo(oldPath);
                        oldFile = di.Name;
                    } else {
                        return FileOperationStatus.FailedAlr;
                    }
                } else if (File.Exists(oldPath)) {
                    if (m_isDirectory) {
                        return FileOperationStatus.FailedAlr;
                    } else {
                        FileInfo fi = new FileInfo(oldPath);
                        oldFile = fi.Name;
                    }
                }
                if (oldFile == newFile) {
                    return FileOperationStatus.Success;
                }
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }

            if (oldFile.ToLower() == newFile.ToLower()) {
                // 大文字と小文字だけが違う場合は別の名前を経由
                string tempPath = baseDir + newFile + "$SF$TEMP$";
                try {
                    if (Directory.Exists(tempPath) || File.Exists(tempPath)) {
                        return FileOperationStatus.AlrDir;
                    }
                    Move(oldPath, tempPath);
                    Move(tempPath, newPath);
                } catch (Exception) {
                    return FileOperationStatus.ErrorRename;
                }
            } else {
                // 名前を変更
                try {
                    Move(oldPath, newPath);
                } catch (Exception) {
                    return FileOperationStatus.ErrorRename;
                }
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：属性を取得する
        // 引　数：[in]path    対象ファイルまたはディレクトリのパス
        // 戻り値：属性
        //=========================================================================================
        public void SetAttributes(string path, FileAttributes attr) {
            File.SetAttributes(path, attr);
        }

        //=========================================================================================
        // 機　能：属性を取得する
        // 引　数：[in]path    対象ファイルまたはディレクトリのパス
        // 戻り値：属性
        //=========================================================================================
        public FileAttributes GetAttributes(string path) {
            return File.GetAttributes(path);
        }

        //=========================================================================================
        // 機　能：属性を足し合わせる
        // 引　数：[in]path    対象ファイルまたはディレクトリのパス
        // 　　　　[in]attr    設定する属性
        // 戻り値：属性
        //=========================================================================================
        public void MergeAttributes(string path, FileAttribute attr) {
            FileAttributes target = File.GetAttributes(path);
            if (attr.IsReadonly) {
                target |= FileAttributes.ReadOnly;
            } else {
                target &= ~FileAttributes.ReadOnly;
            }
            if (attr.IsSystem) {
                target |= FileAttributes.System;
            } else {
                target &= ~FileAttributes.System;
            }
            if (attr.IsArchive) {
                target |= FileAttributes.Archive;
            } else {
                target &= ~FileAttributes.Archive;
            }
            if (attr.IsHidden) {
                target |= FileAttributes.Hidden;
            } else {
                target &= ~FileAttributes.Hidden;
            }
            File.SetAttributes(path, target);
        }

        //=========================================================================================
        // 機　能：作成日時を設定する
        // 引　数：[in]path    対象ファイルまたはディレクトリのパス
        // 　　　　[in]time    設定する日時
        // 戻り値：なし
        //=========================================================================================
        public void SetCreationTime(string path, DateTime time) {
            if (m_isDirectory) {
                Directory.SetCreationTime(path, time);
            } else {
                File.SetCreationTime(path, time);
            }
        }

        //=========================================================================================
        // 機　能：作成日時を取得する
        // 引　数：[in]path    対象ファイルまたはディレクトリのパス
        // 戻り値：作成日時
        //=========================================================================================
        public DateTime GetCreationTime(string path) {
            if (m_isDirectory) {
                return Directory.GetCreationTime(path);
            } else {
                return File.GetCreationTime(path);
            }
        }

        //=========================================================================================
        // 機　能：更新日時を設定する
        // 引　数：[in]path    対象ファイルまたはディレクトリのパス
        // 　　　　[in]time    設定する日時
        // 戻り値：なし
        //=========================================================================================
        public void SetLastWriteTime(string path, DateTime time) {
            if (m_isDirectory) {
                Directory.SetLastWriteTime(path, time);
            } else {
                File.SetLastWriteTime(path, time);
            }
        }

        //=========================================================================================
        // 機　能：更新日時を取得する
        // 引　数：[in]path    対象ファイルまたはディレクトリのパス
        // 戻り値：更新日時
        //=========================================================================================
        public DateTime GetLastWriteTime(string path) {
            if (m_isDirectory) {
                return Directory.GetLastWriteTime(path);
            } else {
                return File.GetLastWriteTime(path);
            }
        }

        //=========================================================================================
        // 機　能：最終アクセス日時を設定する
        // 引　数：[in]path    対象ファイルまたはディレクトリのパス
        // 　　　　[in]time    設定する日時
        // 戻り値：なし
        //=========================================================================================
        public void SetLastAccessTime(string path, DateTime time) {
            if (m_isDirectory) {
                Directory.SetLastAccessTime(path, time);
            } else {
                File.SetLastAccessTime(path, time);
            }
        }

        //=========================================================================================
        // 機　能：最終アクセス日時を取得する
        // 引　数：[in]path    対象ファイルまたはディレクトリのパス
        // 戻り値：最終アクセス日時
        //=========================================================================================
        public DateTime GetLastAccessTime(string path) {
            if (m_isDirectory) {
                return Directory.GetLastAccessTime(path);
            } else {
                return File.GetLastAccessTime(path);
            }
        }

        //=========================================================================================
        // 機　能：ファイルまたはディレクトリを移動する
        // 引　数：[in]oldPath  旧ファイル名
        // 　　　　[in]newPath  新ファイル名
        // 戻り値：なし
        //=========================================================================================
        public void Move(string oldPath, string newPath) {
            if (m_isDirectory) {
                Directory.Move(oldPath, newPath);
            } else {
                File.Move(oldPath, newPath);
            }
        }
    }
}
