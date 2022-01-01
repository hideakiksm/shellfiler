using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：名前の変更を行うProcedure
    //=========================================================================================
    class RenameProcedure {
        // ファイル操作ユーティリティ
        private WindowsFileFolderSelectUtils m_windowsFileUtils;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public RenameProcedure() {
        }

        //=========================================================================================
        // 機　能：名前と属性を変更する
        // 引　数：[in]path       属性を変更するファイルやディレクトリのフルパス
        // 　　　　[in]orgInfo    変更前のファイル情報
        // 　　　　[in]newInfo    変更後のファイル情報
        // 戻り値：ステータス（成功のときSuccessDelDir）
        //=========================================================================================
        public FileOperationStatus Execute(string path, RenameFileInfo orgInfo, RenameFileInfo newInfo) {
            RenameFileInfo.WindowsRenameInfo orgWin = orgInfo.WindowsInfo;
            RenameFileInfo.WindowsRenameInfo newWin = newInfo.WindowsInfo;
            string baseDir = Path.GetDirectoryName(path);
            if (!baseDir.EndsWith("\\")) {
                baseDir += "\\";
            }
            m_windowsFileUtils = new WindowsFileFolderSelectUtils(orgInfo.IsDirectory);
            if (orgInfo.IsDirectory) {
                return Rename(baseDir, orgInfo.WindowsInfo, newInfo.WindowsInfo);
            } else {
                return Rename(baseDir, orgInfo.WindowsInfo, newInfo.WindowsInfo);
            }
        }

        //=========================================================================================
        // 機　能：ディレクトリの名前と属性を変更する
        // 引　数：[in]baseDir    属性を変更するディレクトリが存在するディレクトリのフルパス
        // 　　　　[in]orgInfo    変更前のファイル情報
        // 　　　　[in]newInfo    変更後のファイル情報
        // 戻り値：ステータス（成功のときSuccessDelDir）
        //=========================================================================================
        public FileOperationStatus Rename(string baseDir, RenameFileInfo.WindowsRenameInfo orgWin, RenameFileInfo.WindowsRenameInfo newWin) {
            try {
                // 旧属性を取得
                string oldPath = baseDir + orgWin.FileName;
                FileAttributes oldAttr = m_windowsFileUtils.GetAttributes(oldPath);
                FileAttributes orgAttr = oldAttr;

                // ファイル名／属性を変更可能にする
                try {
                    if ((oldAttr & FileAttributes.ReadOnly) != 0 || (oldAttr & FileAttributes.System) != 0 || (oldAttr & FileAttributes.Hidden) != 0) {
                        oldAttr &= ~FileAttributes.ReadOnly;
                        oldAttr &= ~FileAttributes.System;
                        oldAttr &= ~FileAttributes.Hidden;
                        m_windowsFileUtils.SetAttributes(oldPath, oldAttr);
                    }
                } catch (Exception) {
                    return FileOperationStatus.ErrorSetAttr;
                }

                // 属性変更を実行
                FileOperationStatus status = ExecuteRename(baseDir, orgWin, newWin, oldAttr);

                // 失敗時は元に戻す
                if (!status.Succeeded) {
                    try {
                        File.SetAttributes(oldPath, orgAttr);
                    } catch (Exception) {
                        // 元に戻せない場合はそのまま失敗
                    }
                }
                return status;
            } catch (Exception) {
                // はじめの属性取得に失敗したときは何もせずに失敗
                return FileOperationStatus.ErrorSetAttr;
            }
        }

        //=========================================================================================
        // 機　能：ディレクトリの名前と属性を変更する（処理本体）
        // 引　数：[in]baseDir    属性を変更するディレクトリが存在するディレクトリのフルパス
        // 　　　　[in]orgInfo    変更前のファイル情報
        // 　　　　[in]newInfo    変更後のファイル情報
        // 　　　　[in]oldAttr    処理開始時点のファイル属性
        // 戻り値：ステータス（成功のときSuccessDelDir）
        //=========================================================================================
        public FileOperationStatus ExecuteRename(string baseDir, RenameFileInfo.WindowsRenameInfo orgWin, RenameFileInfo.WindowsRenameInfo newWin, FileAttributes oldAttr) {
            string oldPath = baseDir + orgWin.FileName;
            string newPath = baseDir + newWin.FileName;

            // ファイル名が異なるときは変更
            if (orgWin.FileName != newWin.FileName) {
                FileOperationStatus status = m_windowsFileUtils.RenameFile(orgWin.FileName, newWin.FileName, baseDir);
                if (!status.Succeeded) {
                    return status;
                }
            }

            // タイムスタンプが異なるときは変更
            try {
                if (orgWin.CreationDate != newWin.CreationDate) {
                    m_windowsFileUtils.SetCreationTime(newPath, newWin.CreationDate);
                }
                if (orgWin.ModifiedDate != newWin.ModifiedDate) {
                    m_windowsFileUtils.SetLastWriteTime(newPath, newWin.ModifiedDate);
                }
                if (orgWin.AccessDate != newWin.AccessDate) {
                    m_windowsFileUtils.SetLastAccessTime(newPath, newWin.AccessDate);
                }
            } catch (Exception) {
                return FileOperationStatus.ErrorSetTime;
            }

            // 属性が異なるときは変更
            try {
                if (oldAttr != newWin.FileAttributes) {
                    m_windowsFileUtils.SetAttributes(newPath, newWin.FileAttributes);
                }
            } catch (Exception) {
                return FileOperationStatus.ErrorSetAttr;
            }
            return FileOperationStatus.Success;
        }
    }
}
