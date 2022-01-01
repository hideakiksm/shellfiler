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
    // クラス：ファイルの一覧を取得する処理を行う
    //=========================================================================================
    class VirtualGetFileListProcedure {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public VirtualGetFileListProcedure() {
        }

        //=========================================================================================
        // 機　能：処理を実行する
        // 引　数：[in]context     コンテキスト情報
        // 　　　　[in]directory   取得ディレクトリ
        // 　　　　[out]fileList   ファイル一覧を取得する変数への参照
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext context, string directory, out List<IFile> fileList) {
            FileOperationStatus status;
            fileList = null;
            List<IFile> result = new List<IFile>();

            // 仮想フォルダを用意
            VirtualFolderInfo virtualFolder = null;
            IFileListContext fileListContext = context.SrcFileListContext;
            if (fileListContext is VirtualFileListContext) {
                virtualFolder = ((VirtualFileListContext)fileListContext).VirtualFolderInfo;
            }
            if (virtualFolder == null) {
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

            // ファイル一覧を取得
            string localDir = ExtractRuntime.GetArchiveFileInfo(directory, virtualFolder.MostInnerArchive);
            List<int> fileIdList, dirIdList, partDirIdList;
            int parentId;
            bool exist = EnumFileId(localDir, fileCopySrcMaster, out parentId, out fileIdList, out dirIdList, out partDirIdList);
            if (!exist) {
                return FileOperationStatus.FileNotFound;
            }

            // ファイル情報に変換
            List<IFile> resultFileList = new List<IFile>();
            status = GetParentFile(archiveExtract, parentId, resultFileList);
            if (!status.Succeeded) {
                return status;
            }
            status = GetFileList(archiveExtract, localDir, fileIdList, true, resultFileList);
            if (!status.Succeeded) {
                return status;
            }
            status = GetFileList(archiveExtract, localDir, dirIdList, true, resultFileList);
            if (!status.Succeeded) {
                return status;
            }
            status = GetFileList(archiveExtract, localDir, partDirIdList, false, resultFileList);
            if (!status.Succeeded) {
                return status;
            }

            fileList = resultFileList;
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：指定されたディレクトリ直下のファイルとディレクトリの一覧を返す
        // 引　数：[in]targetDir         対象ディレクトリのアーカイブ内表現（最後の「\」なし）
        // 　　　　[in]fileCopySrcMaster アーカイブ内の転送元一覧情報
        // 　　　　[out]parentId         親フォルダのアーカイブ内インデックスを返す変数（親を不明な状態で返すとき-1）
        // 　　　　[out]fileIdList       ファイルのアーカイブ内インデックスを返す変数
        // 　　　　[out]dirIdList        ディレクトリのアーカイブ内インデックスを返す変数
        // 　　　　[out]pathDirIdList    ディレクトリ（子のファイルやディレクトリの一部として構成）のアーカイブ内インデックスを返す変数
        // 戻り値：対象ディレクトリが見つかったときtrue
        //=========================================================================================
        public bool EnumFileId(string targetDir, VirtualFileCopySrcMaster fileCopySrcMaster, out int parentId, out List<int> fileIdList, out List<int> dirIdList, out List<int> pathDirIdList) {
            parentId = -1;
            fileIdList = null;
            dirIdList = null;
            pathDirIdList = null;

            // 準備
            Dictionary<string, int> fileNameToId = new Dictionary<string, int>();
            Dictionary<string, int> dirNameToId = new Dictionary<string, int>();
            Dictionary<string, PathDirNameData> pathDirNameToId = new Dictionary<string, PathDirNameData>();
            string targetDirLower = targetDir.ToLower() + "\\";

            // ターゲット自身
            Dictionary<string, int> dirNameLowerToIndex = fileCopySrcMaster.DirectoryNameLowerToIndex;
            bool existTargetDirectory = dirNameLowerToIndex.ContainsKey(targetDir.ToLower());

            // 親
            if (targetDir == "") {
                parentId = -1;
            } else {
                string parentDir = GenericFileStringUtils.GetDirectoryName(targetDir).ToLower();
                if (parentDir == "") {
                    parentId = -1;
                } else if (dirNameLowerToIndex.ContainsKey(parentDir)) {
                    parentId = dirNameLowerToIndex[parentDir];
                } else {
                    parentId = -1;
                }
            }

            // ファイル一覧を確認
            Dictionary<string, int> fileNameLowerToIndex = fileCopySrcMaster.FileNameLowerToIndex;
            foreach (string filePathLower in fileNameLowerToIndex.Keys) {
                if (!filePathLower.StartsWith(targetDirLower)) {
                    continue;
                }
                existTargetDirectory = true;
                // targetDir="dir1\dir2"
                // filePathLower="dir1\dir2\dir3\file"
                // →subPath="dir3\file"
                string subPath = filePathLower.Substring(targetDirLower.Length);
                string[] subPathList = subPath.Split('\\');
                string childName = subPathList[0];
                if (subPathList.Length == 1) {
                    // ファイルを発見
                    fileNameToId.Add(childName, fileNameLowerToIndex[filePathLower]);
                } else if (!dirNameToId.ContainsKey(childName)) {
                    // 未登録のフォルダを発見
                    PathDirNameData pathDirNameData = new PathDirNameData(childName, subPathList.Length, fileNameLowerToIndex[filePathLower]);
                    if (!pathDirNameToId.ContainsKey(childName)) {
                        pathDirNameToId.Add(childName, pathDirNameData);
                    } else {
                        PathDirNameData existing = pathDirNameToId[childName];
                        if (existing.Depth > pathDirNameData.Depth) {
                            // 階層が浅いものを登録
                            pathDirNameToId[childName] = pathDirNameData;
                        }
                    }
                }
            }

            // フォルダ一覧を確認
            foreach (string dirPathLower in dirNameLowerToIndex.Keys) {
                if (!dirPathLower.StartsWith(targetDirLower)) {
                    continue;
                }
                existTargetDirectory = true;
                // targetDir="dir1\dir2"
                // dirPathLower="dir1\dir2\dir3\dir4"
                // →subPath="dir3\dir4"
                string subPath = dirPathLower.Substring(targetDirLower.Length);
                string[] subPathList = subPath.Split('\\');
                string childName = subPathList[0];
                if (subPathList.Length == 1) {
                    // 目的のフォルダを発見
                    dirNameToId.Add(childName, dirNameLowerToIndex[dirPathLower]);
                } else if (!dirNameToId.ContainsKey(childName)) {
                    // 未登録のフォルダを発見
                    PathDirNameData pathDirNameData = new PathDirNameData(childName, subPathList.Length, dirNameLowerToIndex[dirPathLower]);
                    if (!pathDirNameToId.ContainsKey(childName)) {
                        pathDirNameToId.Add(childName, pathDirNameData);
                    } else {
                        PathDirNameData existing = pathDirNameToId[childName];
                        if (existing.Depth > pathDirNameData.Depth) {
                            // 階層が浅いものを登録
                            pathDirNameToId[childName] = pathDirNameData;
                        }
                    }
                }
            }

            // 存在していたか？
            if (!existTargetDirectory) {
                return false;
            }

            // 結果に変換
            fileIdList = new List<int>();
            foreach (int index in fileNameToId.Values) {
                fileIdList.Add(index);
            }
            fileIdList.Sort();

            dirIdList = new List<int>();
            foreach (int index in dirNameToId.Values) {
                dirIdList.Add(index);
            }
            dirIdList.Sort();

            pathDirIdList = new List<int>();
            foreach (string child in pathDirNameToId.Keys) {
                if (dirNameToId.ContainsKey(child)) {
                    continue;
                }
                pathDirIdList.Add(pathDirNameToId[child].ArchiveIndex);
            }
            pathDirIdList.Sort();

            return true;
        }

        //=========================================================================================
        // 機　能：親フォルダの情報を作成してファイル一覧に追加する
        // 引　数：[in]archiveExtract    アーカイブへのアクセスインターフェース
        // 　　　　[in]parentIndex       親フォルダのアーカイブ内インデックス（親が不明なとき-1）
        // 　　　　[in]resultFileList    結果を返すファイル一覧
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        private FileOperationStatus GetParentFile(IArchiveVirtualExtract archiveExtract, int parentIndex, List<IFile> resultFileList) {
            FileOperationStatus status;
            if (parentIndex == -1) {
                resultFileList.Add(new VirtualFile("..", DateTime.MinValue, 0, true));
            } else {
                string filePath;
                DateTime updateTime;
                long fileSize;
                bool isDir;
                status = archiveExtract.GetFileInfo(parentIndex, out filePath, out updateTime, out fileSize, out isDir);
                if (!status.Succeeded) {
                    return status;
                }
                resultFileList.Add(new VirtualFile("..", updateTime, 0, true));
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ファイルまたはフォルダの情報を作成してファイル一覧に追加する
        // 引　数：[in]archiveExtract    アーカイブへのアクセスインターフェース
        // 　　　　[in]targetDir         対象ディレクトリのアーカイブ内表現（最後の「\」なし）
        // 　　　　[in]archiveIdxList    作成するファイルやディレクトリのアーカイブ内インデックス配列
        // 　　　　[in]leafNode          ディレクトリ構成末端の情報を作成するときtrue、途中の情報を作成するときfalse
        // 　　　　[in]resultFileList    結果を返すファイル一覧
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        private FileOperationStatus GetFileList(IArchiveVirtualExtract archiveExtract, string targetDir, List<int> archiveIdxList, bool leafNode, List<IFile> resultFileList) {
            FileOperationStatus status;
            for (int i = 0; i < archiveIdxList.Count; i++) {
                int index = archiveIdxList[i];
                string filePath;
                DateTime updateTime;
                long fileSize;
                bool isDir;
                status = archiveExtract.GetFileInfo(index, out filePath, out updateTime, out fileSize, out isDir);
                if (!status.Succeeded) {
                    return status;
                }
                string fileName;
                if (leafNode) {
                    fileName = GenericFileStringUtils.GetFileName(filePath);
                } else {
                    string fileNameSub = filePath.Substring(targetDir.Length);
                    fileName = (fileNameSub.Split('\\'))[0];
                    isDir = true;
                    fileSize = 0;
                }
                resultFileList.Add(new VirtualFile(fileName, updateTime, fileSize, isDir));
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // クラス：アーカイブ内のパス表現中に含まれるディレクトリをディレクトリ情報として扱うためのデータ構造
        // 　　　　zipの仕様で、dir1\dir2\fileのうち、dir1エントリがない場合にdir1\dir2やdir1\dir2\file
        // 　　　　からdir1の情報を作成する目的
        //=========================================================================================
        private class PathDirNameData {
            // 直下の子ディレクトリの名前
            // 例 dir3
            public string ChildDirectoryName;

            // 子ディレクトリから、アーカイブ内インデックスのパスの深さ
            // 例 dir1\dir2\dir3\dir4\file1.txt で、dir1\dir2を検索中なら3
            public int Depth;

            // アーカイブ内のインデックス
            public int ArchiveIndex;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]childDirectoryName   直下の子ディレクトリの名前
            // 　　　　[in]depth                子ディレクトリから、アーカイブ内インデックスのパスの深さ
            // 　　　　[in]archiveIndex         アーカイブ内のインデックス
            // 戻り値：ステータス（成功のときSuccess）
            //=========================================================================================
            public PathDirNameData(string childDirectoryName, int depth, int archiveIndex) {
                ChildDirectoryName = childDirectoryName;
                Depth = depth;
                ArchiveIndex = archiveIndex;
            }
        }
    }
}
