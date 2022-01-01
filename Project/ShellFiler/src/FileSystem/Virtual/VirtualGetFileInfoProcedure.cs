using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.FileFilter;
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
    // クラス：ファイル情報を取得する処理を行う
    //=========================================================================================
    class VirtualGetFileInfoProcedure {
        // 処理中に使用したテンポラリファイル名のリスト
        private List<string> m_temporaryFileList = new List<string>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public VirtualGetFileInfoProcedure() {
        }

        //=========================================================================================
        // 機　能：処理を実行する
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]directory     ファイルパス
        // 　　　　[out]fileInfo     ファイルの情報（失敗したときはnull）
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext context, string directory, out IFile fileInfo) {
            FileOperationStatus status;
            fileInfo = null;

            // 仮想フォルダを用意
            IFileListContext fileListContext = context.SrcFileListContext;
            VirtualFolderInfo virtualFolder = null;
            if (fileListContext is VirtualFileListContext) {
                virtualFolder = ((VirtualFileListContext)fileListContext).VirtualFolderInfo;
            } else {
                Program.Abort("仮想フォルダ情報がセットされていません。");
            }

            // まだの場合はアーカイブを開く
            IArchiveVirtualExtract archiveExtract;
            VirtualFileCopySrcMaster fileCopySrcMaster;
            context.VirtualRequestContext.GetVirtualFileCopySrcContext(out archiveExtract, out fileCopySrcMaster);
            if (archiveExtract == null) {
                status = VirtualProcedureUtils.OpenArchiveForFileCopy(context, virtualFolder, out archiveExtract, out fileCopySrcMaster);
                if (!status.Succeeded) {
                    return status;
                }
                context.VirtualRequestContext.SetVirtualFileCopySrcContext(archiveExtract, fileCopySrcMaster);
            }

            // アーカイブ内インデックスを取得
            VirtualFolderArchiveInfo virtualArchive = virtualFolder.MostInnerArchive;
            string arcPath = ExtractRuntime.GetArchiveFileInfo(directory, virtualArchive);
            int index;
            bool indexSubPath;
            bool found = GetArchiveIndex(arcPath, fileCopySrcMaster, out index, out indexSubPath);
            if (!found) {
                return FileOperationStatus.FileNotFound;
            }

            // 情報を取得
            string filePath;
            DateTime updateTime;
            long fileSize;
            bool isDir;
            status = archiveExtract.GetFileInfo(index, out filePath, out updateTime, out fileSize, out isDir);
            if (!status.Succeeded) {
                return status;
            }
            string fileName = GenericFileStringUtils.GetFileName(arcPath);
            if (indexSubPath) {
                fileInfo = new VirtualFile(fileName, updateTime, 0, true);
            } else {
                fileInfo = new VirtualFile(fileName, updateTime, fileSize, isDir);
            }

            return FileOperationStatus.SuccessCopy;
        }

        //=========================================================================================
        // 機　能：指定されたファイルまたはディレクトリのアーカイブ内インデックスを返す
        // 引　数：[in]targetFile        対象のファイルまたはディレクトリのアーカイブ内表現（最後の「\」なし）
        // 　　　　[in]fileCopySrcMaster アーカイブ内の転送元一覧情報
        // 　　　　[out]index            アーカイブ内インデックス
        // 　　　　[out]subPath          完全一致のものはなく、部分一致したインデックスを返したときtrue
        // 戻り値：ファイルが見つかったときtrue
        //=========================================================================================
        public bool GetArchiveIndex(string targetFile, VirtualFileCopySrcMaster fileCopySrcMaster, out int index, out bool subPath) {
            // そのまま確認
            index = fileCopySrcMaster.CheckContainsDirectory(targetFile);
            if (index != -1) {
                subPath = false;
                return true;
            }
            index = fileCopySrcMaster.CheckContainsFile(targetFile);
            if (index != -1) {
                subPath = false;
                return true;
            }

            // フォルダ一覧のサブパスにヒットするか確認
            index = -1;                                 // 見つかったアーカイブ内インデックス
            int indexDepth = int.MaxValue;              // indexに対するアーカイブ内パスの深さ（最も浅いものを優先）
            string targetDirLower = targetFile.ToLower() + "\\";
            Dictionary<string, int> dirNameLowerToIndex = fileCopySrcMaster.DirectoryNameLowerToIndex;
            foreach (string dirPathLower in dirNameLowerToIndex.Keys) {
                if (!dirPathLower.StartsWith(targetDirLower)) {
                    continue;
                }
                int depth = StringUtils.GetCharCount(dirPathLower, '\\');
                if (depth < indexDepth) {
                    index = dirNameLowerToIndex[dirPathLower];
                    indexDepth = depth;
                }
            }

            // ファイル一覧を確認
            Dictionary<string, int> fileNameLowerToIndex = fileCopySrcMaster.FileNameLowerToIndex;
            foreach (string filePathLower in fileNameLowerToIndex.Keys) {
                if (!filePathLower.StartsWith(targetDirLower)) {
                    continue;
                }
                int depth = StringUtils.GetCharCount(filePathLower, '\\');
                if (depth < indexDepth) {
                    index = fileNameLowerToIndex[filePathLower];
                    indexDepth = depth;
                }
            }

            if (index == -1) {
                subPath = false;
                return false;
            } else {
                subPath = true;
                return true;
            }
        }

        //=========================================================================================
        // 機　能：一時ファイルを削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void DeleteTempFile() {
            for (int i = 0; i < m_temporaryFileList.Count; i++) {
                try {
                    File.Delete(m_temporaryFileList[i]);
                } catch (Exception) {
                }
            }
        }
    }
}
