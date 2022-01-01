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
using ShellFiler.FileTask;

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：ファイルやディレクトリの属性を一括変更のルールで変更するプロシージャ
    //=========================================================================================
    class ModifyFileInfoProcedure {
        // ファイル操作ユーティリティ
        private WindowsFileFolderSelectUtils m_windowsFileUtils;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ModifyFileInfoProcedure() {
        }

        //=========================================================================================
        // 機　能：名前と属性を変更する
        // 引　数：[in]path       属性を変更するファイルやディレクトリのフルパス
        // 　　　　[in]renameInfo 変更ルール
        // 　　　　[in]modifyCtx  名前変更のコンテキスト情報
        // 戻り値：ステータス（成功のときSuccessDelDir）
        //=========================================================================================
        public FileOperationStatus Execute(string path, RenameSelectedFileInfo renameInfo, ModifyFileInfoContext modifyCtx) {
            string baseDir = Path.GetDirectoryName(path);
            if (!baseDir.EndsWith("\\")) {
                baseDir += "\\";
            }
            string fileName = Path.GetFileName(path);
            if (File.Exists(path)) {
                m_windowsFileUtils = new WindowsFileFolderSelectUtils(false);
                return ModifyFileInfo(fileName, baseDir, renameInfo.WindowsInfo, modifyCtx);
            } else if (Directory.Exists(path)) {
                m_windowsFileUtils = new WindowsFileFolderSelectUtils(true);
                return ModifyFileInfo(fileName, baseDir, renameInfo.WindowsInfo, modifyCtx);
            } else {
                return FileOperationStatus.FileNotFound;
            }
        }

        //=========================================================================================
        // 機　能：名前と属性を変更する
        // 引　数：[in]fileName   属性を変更するファイルやディレクトリのファイル名のみ
        // 　　　　[in]baseDir    属性を変更するディレクトリが存在するディレクトリのフルパス
        // 　　　　[in]renameInfo 変更ルール
        // 　　　　[in]modifyCtx  名前変更のコンテキスト情報
        // 戻り値：ステータス（成功のときSuccessDelDir）
        //=========================================================================================
        public FileOperationStatus ModifyFileInfo(string fileName, string baseDir, RenameSelectedFileInfo.WindowsRenameInfo renameInfo, ModifyFileInfoContext modifyCtx) {
            try {
                // 旧属性を取得
                string oldPath = baseDir + fileName;
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
                FileOperationStatus status = ExecuteModify(fileName, baseDir, renameInfo, modifyCtx, orgAttr, oldAttr);

                // 失敗時は元に戻す
                if (!status.Succeeded) {
                    try {
                        m_windowsFileUtils.SetAttributes(oldPath, orgAttr);
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
        // 機　能：名前と属性を変更する（処理本体）
        // 引　数：[in]fileName   属性を変更するファイルやディレクトリのファイル名のみ
        // 　　　　[in]baseDir    属性を変更するディレクトリが存在するディレクトリのフルパス
        // 　　　　[in]renameInfo 変更ルール
        // 　　　　[in]modifyCtx  名前変更のコンテキスト情報
        // 　　　　[in]orgAttr    元のファイル属性
        // 　　　　[in]oldAttr    処理開始時点のファイル属性
        // 戻り値：ステータス（成功のときSuccessDelDir）
        //=========================================================================================
        public FileOperationStatus ExecuteModify(string fileName, string baseDir, RenameSelectedFileInfo.WindowsRenameInfo renameInfo, ModifyFileInfoContext modifyCtx, FileAttributes orgAttr, FileAttributes oldAttr) {
            FileOperationStatus status;

            // ファイル名
            string newFileName = RenameSelectedFileInfoBackgroundTask.GetNewFileName(fileName, renameInfo.ModifyFileNameInfo, modifyCtx);
            string newPath = baseDir + newFileName;
            if (fileName != newFileName) {
                status = m_windowsFileUtils.RenameFile(fileName, newFileName, baseDir);
                if (!status.Succeeded) {
                    return status;
                }
            }

            // タイムスタンプ
            try {
                // 作成時刻
                if (renameInfo.CreateDate != null && renameInfo.CreateTime != null) {
                    DateTime orgDate = m_windowsFileUtils.GetCreationTime(newPath);
                    DateTimeInfo dateTime = new DateTimeInfo(renameInfo.CreateDate, renameInfo.CreateTime);
                    m_windowsFileUtils.SetCreationTime(newPath, dateTime.ToDateTime());
                } else if (renameInfo.CreateDate != null || renameInfo.CreateTime != null) {
                    DateTime orgDate = m_windowsFileUtils.GetCreationTime(newPath);
                    DateTimeInfo dateTime = new DateTimeInfo(orgDate);
                    dateTime.SetDate(renameInfo.CreateDate);
                    dateTime.SetTime(renameInfo.CreateTime);
                    m_windowsFileUtils.SetCreationTime(newPath, dateTime.ToDateTime());
                }

                // 更新時刻
                if (renameInfo.UpdateDate != null && renameInfo.UpdateTime != null) {
                    DateTime orgDate = m_windowsFileUtils.GetLastWriteTime(newPath);
                    DateTimeInfo dateTime = new DateTimeInfo(renameInfo.UpdateDate, renameInfo.UpdateTime);
                    m_windowsFileUtils.SetLastWriteTime(newPath, dateTime.ToDateTime());
                } else if (renameInfo.UpdateDate != null || renameInfo.UpdateTime != null) {
                    DateTime orgDate = m_windowsFileUtils.GetLastWriteTime(newPath);
                    DateTimeInfo dateTime = new DateTimeInfo(orgDate);
                    dateTime.SetDate(renameInfo.UpdateDate);
                    dateTime.SetTime(renameInfo.UpdateTime);
                    m_windowsFileUtils.SetLastWriteTime(newPath, dateTime.ToDateTime());
                }

                // アクセス時刻
                if (renameInfo.AccessDate != null && renameInfo.AccessTime != null) {
                    DateTime orgDate = m_windowsFileUtils.GetLastAccessTime(newPath);
                    DateTimeInfo dateTime = new DateTimeInfo(renameInfo.AccessDate, renameInfo.AccessTime);
                    m_windowsFileUtils.SetLastAccessTime(newPath, dateTime.ToDateTime());
                } else if (renameInfo.AccessDate != null || renameInfo.AccessDate != null) {
                    DateTime orgDate = m_windowsFileUtils.GetLastAccessTime(newPath);
                    DateTimeInfo dateTime = new DateTimeInfo(orgDate);
                    dateTime.SetDate(renameInfo.AccessDate);
                    dateTime.SetTime(renameInfo.AccessTime);
                    m_windowsFileUtils.SetLastAccessTime(newPath, dateTime.ToDateTime());
                }
            } catch (Exception) {
                return FileOperationStatus.ErrorSetTime;
            }

            // 属性が異なるときは変更
            try {
                FileAttributes targetAttr = orgAttr;
                if (renameInfo.AttributeReadonly != null) {
                    if (renameInfo.AttributeReadonly.Value == true) {
                        targetAttr |= FileAttributes.ReadOnly;
                    } else {
                        targetAttr &= ~FileAttributes.ReadOnly;
                    }
                }
                if (renameInfo.AttributeHidden != null) {
                    if (renameInfo.AttributeHidden.Value == true) {
                        targetAttr |= FileAttributes.Hidden;
                    } else {
                        targetAttr &= ~FileAttributes.Hidden;
                    }
                }
                if (renameInfo.AttributeArchive != null) {
                    if (renameInfo.AttributeArchive.Value == true) {
                        targetAttr |= FileAttributes.Archive;
                    } else {
                        targetAttr &= ~FileAttributes.Archive;
                    }
                }
                if (renameInfo.AttributeSystem != null) {
                    if (renameInfo.AttributeSystem.Value == true) {
                        targetAttr |= FileAttributes.System;
                    } else {
                        targetAttr &= ~FileAttributes.System;
                    }
                }

                if (targetAttr != oldAttr) {
                    m_windowsFileUtils.SetAttributes(newPath, targetAttr);
                }
            } catch (Exception) {
                return FileOperationStatus.ErrorSetAttr;
            }

            return FileOperationStatus.Success;
        }
    }
}
