using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;

namespace ShellFiler.FileTask.Provider {

    //=========================================================================================
    // クラス：単純なファイル転送先情報
    //         転送先は1つのディレクトリ名で表現される
    //=========================================================================================
    class FileProviderDestSimple : IFileProviderDest {
        // 転送先のファイルシステム
        private IFileSystem m_fileSystem;

        // 転送先のディレクトリ
        private string m_destDirectoryName;

        // ファイル一覧のコンテキスト情報
        private IFileListContext m_fileListContext;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]destDirName  転送先のディレクトリ名
        // 　　　　[in]fileSystem   転送先のファイルシステム
        // 　　　　[in]fileListCtx  ファイル一覧のコンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public FileProviderDestSimple(string destDirName, IFileSystem fileSystem, IFileListContext fileListCtx) {
            m_destDirectoryName = destDirName;
            m_fileSystem = fileSystem;
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
    }
}
