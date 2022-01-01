using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.FileSystem.SFTP;

namespace ShellFiler.FileSystem.Virtual {

    //=========================================================================================
    // クラス：仮想フォルダでのファイル１つ分の情報
    //=========================================================================================
    public class VirtualFile : SimpleFile {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]arcInfo   アーカイブ内ファイルの情報
        // 戻り値：なし
        //=========================================================================================
        public VirtualFile(IArchiveContentsFileInfo arcInfo) :
            base(arcInfo.FileName, arcInfo.LastWriteTime, FileAttribute.FromArchive(arcInfo.IsDirectory), arcInfo.FileSize) {
        }

        //=========================================================================================
        // 機　能：コンストラクタ（親ディレクトリ専用）
        // 引　数：[in]fileName   ファイル名
        // 　　　　[in]lastWrite  最終更新日時（不明なときMinValue）
        // 　　　　[in]fileSize   ファイルサイズ
        // 　　　　[in]isDir      ディレクトリのときtrue
        // 戻り値：なし
        //=========================================================================================
        public VirtualFile(string fileName, DateTime lastWrite, long fileSize, bool isDir) :
            base(fileName, lastWrite, FileAttribute.FromArchive(isDir), fileSize) {
        }
    }
}
