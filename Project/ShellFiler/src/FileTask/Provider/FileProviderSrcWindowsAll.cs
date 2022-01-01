using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;

namespace ShellFiler.FileTask.Provider {

    //=========================================================================================
    // クラス：Windowsフォルダ内すべての一覧による転送元ファイルの表現
    //=========================================================================================
    class FileProviderSrcWindowsAll : IFileProviderSrc {
        // 転送元ディレクトリのファイルシステム
        private IFileSystem m_fileSystem;

        // 転送元のファイルとディレクトリの一覧
        private List<SimpleFileDirectoryPath> m_pathList = new List<SimpleFileDirectoryPath>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileProviderSrcWindowsAll() {
        }

        //=========================================================================================
        // 機　能：ファイルを読み込む
        // 引　数：[in]fileSystem  ファイルシステム（WindowsFileSystem）
        // 　　　　[in]directory   読み込むディレクトリ（最後は"\"）
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public bool ReadFileList(IFileSystem fileSystem, string directory) {
            m_fileSystem = fileSystem;
            try {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                FileSystemInfo[] infoList = directoryInfo.GetFileSystemInfos();
                foreach (FileSystemInfo info in infoList) {
                    string path = info.FullName;
                    bool isDir = (info is DirectoryInfo);
                    bool isLink = false;
                    m_pathList.Add(new SimpleFileDirectoryPath(path, isDir, isLink));
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }

        //=========================================================================================
        // プロパティ：転送元ディレクトリのファイルシステム
        //=========================================================================================
        public IFileSystem SrcFileSystem {
            get {
                return m_fileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧のコンテキスト情報
        //=========================================================================================
        public IFileListContext FileListContext {
            get {
                // Windowsのテンポラリすべてを取り込むため仮想フォルダはなし
                return new DummyFileListContext();
            }
            set {
            }
        }

        //=========================================================================================
        // プロパティ：転送元ファイルとディレクトリの数
        //=========================================================================================
        public int SrcItemCount {
            get {
                return m_pathList.Count;
            }
        }

        //=========================================================================================
        // 機　能：転送元のフルパスを返す
        // 引　数：[in]index    インデックス
        // 戻り値：転送元のファイル情報
        //=========================================================================================
        public SimpleFileDirectoryPath GetSrcPath(int index) {
            return m_pathList[index];
        }
    }
}
