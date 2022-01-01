using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;

namespace ShellFiler.FileTask.Provider {

    //=========================================================================================
    // クラス：マークファイルを含むファイル転送先情報
    //         転送先は1つのディレクトリ名で表現され、さらに転送先ファイル一覧中の特定のファイル
    //         を指定できる。
    //=========================================================================================
    class FileProviderDestMarked : IFileProviderDest, IFileProviderDestFiles {
        // 転送先のファイルシステム
        private IFileSystem m_fileSystem;

        // 転送先のディレクトリ
        private string m_destDirectoryName;

        // 転送元のファイルとディレクトリの一覧
        private List<SimpleFileDirectoryPath> m_pathList = new List<SimpleFileDirectoryPath>();

        // ファイル一覧のコンテキスト情報
        private IFileListContext m_fileListContext;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]destDirName  転送先のディレクトリ名
        // 　　　　[in]fileSystem   転送先のファイルシステム
        // 　　　　[in]pathList     転送元のファイルとディレクトリの一覧（destDirNameに含まれること）
        // 　　　　[in]fileListCtx  ファイル一覧のコンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public FileProviderDestMarked(string destDirName, IFileSystem fileSystem, List<SimpleFileDirectoryPath> pathList, IFileListContext fileListCtx) {
            m_destDirectoryName = destDirName;
            m_fileSystem = fileSystem;
            m_pathList = pathList;
            m_fileListContext = fileListCtx;
        }

        //=========================================================================================
        // プロパティ：転送先ディレクトリのファイルシステム
        //=========================================================================================
        public IFileSystem DestFileSystem {
            get {
                return m_fileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧のコンテキスト情報
        //=========================================================================================
        public IFileListContext FileListContext {
            get {
                return m_fileListContext;
            }
            set {
                m_fileListContext = value;
            }
        }

        //=========================================================================================
        // プロパティ：転送先ディレクトリ名
        //=========================================================================================
        public string DestDirectoryName {
            get {
                return m_destDirectoryName;
            }
        }

        //=========================================================================================
        // プロパティ：転送先ファイルとディレクトリの数
        //=========================================================================================
        public int DestItemCount {
            get {
                return m_pathList.Count;
            }
        }

        //=========================================================================================
        // 機　能：転送先のファイル情報を返す
        // 引　数：[in]index    インデックス
        // 戻り値：転送元のファイル情報
        //=========================================================================================
        public SimpleFileDirectoryPath GetDestPath(int index) {
            return m_pathList[index];
        }

        //=========================================================================================
        // 機　能：転送元プロバイダに変換して返す
        // 引　数：なし
        // 戻り値：転送元プロバイダにへ関した形
        //=========================================================================================
        public IFileProviderSrc GetSrcProvider() {
            return new SrcConverter(this);
        }

        //=========================================================================================
        // クラス：転送元へのコンバータ
        //=========================================================================================
        private class SrcConverter : IFileProviderSrc {
            // 親クラス（元の転送先プロバイダ）
            private FileProviderDestMarked m_parent;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  親クラス（元の転送先プロバイダ）
            // 戻り値：なし
            //=========================================================================================
            public SrcConverter(FileProviderDestMarked parent) {
                m_parent = parent;
            }

            //=========================================================================================
            // プロパティ：転送元ディレクトリのファイルシステム
            //=========================================================================================
            public IFileSystem SrcFileSystem {
                get {
                    return m_parent.m_fileSystem;
                }
            }
            
            //=========================================================================================
            // プロパティ：ファイル一覧のコンテキスト情報
            //=========================================================================================
            public IFileListContext FileListContext {
                get {
                    // 転送先からの変換のため、仮想フォルダはなし
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
                    return m_parent.m_pathList.Count;
                }
            }

            //=========================================================================================
            // 機　能：転送元のフルパスを返す
            // 引　数：[in]index    インデックス
            // 戻り値：転送元のファイル情報
            //=========================================================================================
            public SimpleFileDirectoryPath GetSrcPath(int index) {
                return m_parent.GetDestPath(index);
            }
        }
    }
}
