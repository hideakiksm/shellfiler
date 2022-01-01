using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Document;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.Management;
using ShellFiler.FileTask.Provider;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイル情報の一括編集をバックグラウンドで実行するクラス
    //         baseにより作業スレッドで実行する。
    //         インスタンスは、BackgroundTaskManagerで管理される。
    //=========================================================================================
    class RenameSelectedFileInfoBackgroundTask : AbstractFileBackgroundTask {
        // ファイル情報の編集方法
        private RenameSelectedFileInfo m_renameInfo;

        // 属性変更のコンテキスト情報
        private ModifyFileInfoContext m_modifyContext;

        // 転送元/転送先の情報
        private BackgroundTaskPathInfo m_backgroundTaskPathInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcProvider  転送元ファイルの情報
        // 　　　　[in]destProvider 転送先ファイルの情報
        // 　　　　[in]refreshUi    作業完了時のUI更新方法
        // 　　　　[in]renameInfo   ファイル情報の編集方法
        // 戻り値：なし
        //=========================================================================================
        public RenameSelectedFileInfoBackgroundTask(IFileProviderSrc srcProvider, IFileProviderDest destProvider, RefreshUITarget refreshUi, RenameSelectedFileInfo renameInfo) : base(srcProvider, destProvider, refreshUi) {
            m_renameInfo = renameInfo;
            m_modifyContext = new ModifyFileInfoContext();
            CreateBackgroundTaskPathInfo();
        }

        //=========================================================================================
        // 機　能：転送元/転送先の情報を作成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CreateBackgroundTaskPathInfo() {
            // 転送元
            string srcShort = BackgroundTaskPathInfo.CreateShortTextFileProviderSrc(FileProviderSrc, null);
            string srcDetail = BackgroundTaskPathInfo.CreateDetailTextFileProviderSrc(FileProviderSrc, null);

            // 転送先
            string destShort = "";
            string destDetail = "";

            m_backgroundTaskPathInfo = new BackgroundTaskPathInfo(srcShort, srcDetail, destShort, destDetail);
        }

        //=========================================================================================
        // 機　能：タスクを実行する
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンドスレッドで実行する
        //=========================================================================================
        protected override void ExecuteTask() {
            string srcPath = FileProviderSrc.GetSrcPath(0).FilePath;
            FileProviderSrc.SrcFileSystem.BeginFileOperation(RequestContext, srcPath);

            try {
                ModifyAll();
                DisplayCompletedLog();
                RefreshUI();
            } finally {
                FileProviderSrc.SrcFileSystem.EndFileOperation(RequestContext);
            }
        }

        //=========================================================================================
        // 機　能：マークされたファイルを変更する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ModifyAll() {
            for (int i = 0; i < FileProviderSrc.SrcItemCount; i++) {
                if (CheckSuspend()) {
                    return;
                }
                SimpleFileDirectoryPath pathInfo = FileProviderSrc.GetSrcPath(i);
                LogFileOperationSetMarkInfo(pathInfo);
                string srcPath = pathInfo.FilePath;
                if (!pathInfo.IsDirectory) {
                    // ファイルの情報を変更
                    ModifyFileInfo(srcPath);
                    if (IsCancel) {
                        return;
                    }
                } else {
                    // ディレクトリの情報を変更
                    ModifyDirectoryInfo(srcPath);
                    if (IsCancel) {
                        return;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイルの属性を変更する
        // 引　数：[in]srcFilePath  対象ファイルのパス名(C:\SRCBASE\DIR\MARKFILE.txt)
        // 戻り値：なし
        //=========================================================================================
        private void ModifyFileInfo(string srcFilePath) {
            string srcFileName = FileProviderSrc.SrcFileSystem.GetFileName(srcFilePath);
            LogFileOperationStart(FileOperationType.RenameFile, srcFilePath, false);
            FileOperationStatus status = FileProviderSrc.SrcFileSystem.ModifyFileInfo(RequestContext, srcFilePath, m_renameInfo, m_modifyContext);
            LogFileOperationEnd(status);
        }

        //=========================================================================================
        // 機　能：ディレクトリ自身のの属性を変更する
        // 引　数：[in]srcFilePath  対象ファイルのパス名(C:\SRCBASE\DIR\MARKFILE.txt)
        // 戻り値：なし
        //=========================================================================================
        private void ModifyDirectorySelfInfo(string srcFilePath) {
            string srcFileName = FileProviderSrc.SrcFileSystem.GetFileName(srcFilePath);
            LogFileOperationStart(FileOperationType.RenameDir, srcFilePath, false);
            FileOperationStatus status = FileProviderSrc.SrcFileSystem.ModifyFileInfo(RequestContext, srcFilePath, m_renameInfo, m_modifyContext);
            LogFileOperationEnd(status);
        }

        //=========================================================================================
        // 機　能：ディレクトリの属性を変更する
        // 引　数：[in]srcPath    対象ディレクトリのパス名(C:\SRCBASE\MARKDIR)
        // 戻り値：なし
        //=========================================================================================
        private void ModifyDirectoryInfo(string srcPath) {
            if (CheckSuspend()) {
                return;
            }

            RenameSelectedFileInfo.TargetFolder targetMode = m_renameInfo.GetTargetDirectoryMode();
            if (targetMode.ModifySubfolderFile) {
                ModifySubfolder(srcPath);
            }
            if (targetMode.ModifyFolder) {
                ModifyDirectorySelfInfo(srcPath);
            }
        }

        //=========================================================================================
        // 機　能：ディレクトリ配下のオブジェクトの属性を変更する
        // 引　数：[in]srcPath    対象ディレクトリのパス名(C:\SRCBASE\MARKDIR)
        // 戻り値：なし
        //=========================================================================================
        private void ModifySubfolder(string srcPath) {
            if (CheckSuspend()) {
                return;
            }
            FileOperationStatus status;
            
            // ディレクトリをキューに入れる
            List<string> fileList = new List<string>();
            List<string> dirList = new List<string>();
            {
                List<IFile> files = null;
                status = FileProviderSrc.SrcFileSystem.GetFileList(RequestContext, srcPath, out files);
                if (status != FileOperationStatus.Success) {
                    LogFileOperationStart(FileOperationType.CopyFile, srcPath, true);
                    LogFileOperationEnd(FileOperationStatus.CanNotAccess);
                    return;
                }
                foreach (IFile file in files) {
                    if (file.FileName == "." || file.FileName == "..") {
                        continue;
                    }
                    if (!file.Attribute.IsDirectory) {
                        fileList.Add(file.FileName);
                    } else {
                        dirList.Add(file.FileName);
                    }
                    if (IsCancel) {
                        return;
                    }
                }
            }

            // ファイルの属性を変更
            foreach (string fileName in fileList) {
                string srcFilePath = FileProviderSrc.SrcFileSystem.CombineFilePath(srcPath, fileName);
                ModifyFileInfo(srcFilePath);
                if (IsCancel) {
                    return;
                }
            }

            // ディレクトリを再帰的に属性変更
            foreach (string dirName in dirList) {
                string srcDirPath = FileProviderSrc.SrcFileSystem.CombineFilePath(srcPath, dirName);
                ModifyDirectoryInfo(srcDirPath);
                if (IsCancel) {
                    return;
                }
            }
        }
        
        //=========================================================================================
        // 機　能：新しいファイル名を決める
        // 引　数：[in]fileName   現在のファイル名
        // 　　　　[in]renameInfo 変更ルール
        // 　　　　[in]modifyCtx  名前変更のコンテキスト情報
        // 戻り値：新しいファイル名（ファイル名を変更しないときnull）
        //=========================================================================================
        public static string GetNewFileName(string fileName, RenameSelectedFileInfo.ModifyFileNameInfo renameInfo, ModifyFileInfoContext modifyCtx) {
            // 変更しない
            if (renameInfo.RenameModeFileBody == RenameSelectedFileInfo.RenameMode.None && renameInfo.RenameModeFileExt == RenameSelectedFileInfo.RenameMode.None) {
                return fileName;
            }

            string ext = GenericFileStringUtils.GetExtensionLast(fileName);
            int bodyLen = fileName.Length - ext.Length;
            if (bodyLen > 0 && fileName[bodyLen - 1] == '.') {
                bodyLen--;
            }
            string body = fileName.Substring(0, bodyLen);

            string newBody = null;
            string newExt = null;

            // ファイル名主部
            if (renameInfo.RenameModeFileBody == RenameSelectedFileInfo.RenameMode.None) {
                newBody = body;
            } else if (renameInfo.RenameModeFileBody == RenameSelectedFileInfo.RenameMode.ToUpper) {
                newBody = body.ToUpper();
            } else if (renameInfo.RenameModeFileBody == RenameSelectedFileInfo.RenameMode.ToLower) {
                newBody = body.ToLower();
            } else if (renameInfo.RenameModeFileBody == RenameSelectedFileInfo.RenameMode.ToCapital) {
                newBody = StringUtils.ToCapital(body);
            } else if (renameInfo.RenameModeFileBody == RenameSelectedFileInfo.RenameMode.Specify) {
                newBody = RenameNumberingInfo.CreateSequenceString(renameInfo.RenameFileBodyNumbering, modifyCtx);
            }

            // 拡張子
            if (renameInfo.RenameModeFileExt == RenameSelectedFileInfo.RenameMode.None) {
                newExt = ext;
            } else if (renameInfo.RenameModeFileExt == RenameSelectedFileInfo.RenameMode.ToUpper) {
                newExt = ext.ToUpper();
            } else if (renameInfo.RenameModeFileExt == RenameSelectedFileInfo.RenameMode.ToLower) {
                newExt = ext.ToLower();
            } else if (renameInfo.RenameModeFileExt == RenameSelectedFileInfo.RenameMode.ToCapital) {
                newExt = StringUtils.ToCapital(ext);
            } else {
                newExt = renameInfo.RenameFileExtString;
            }

            string newFileName = newBody;
            if (newExt.Length > 0) {
                newFileName += "." + newExt;
            }
            return newFileName;
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクの種類
        //=========================================================================================
        public override BackgroundTaskType BackgroundTaskType {
            get {
                return BackgroundTaskType.RenameSelected;
            }
        }

        //=========================================================================================
        // プロパティ：転送元/転送先の情報
        //=========================================================================================
        public override BackgroundTaskPathInfo PathInfo {
            get {
                return m_backgroundTaskPathInfo;
            }
        }
    }
}
