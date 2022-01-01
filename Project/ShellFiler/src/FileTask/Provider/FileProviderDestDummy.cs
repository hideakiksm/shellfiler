using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;

namespace ShellFiler.FileTask.Provider {

    //=========================================================================================
    // クラス：ダミーのファイル転送先情報
    //=========================================================================================
    class FileProviderDestDummy : IFileProviderDest {
        // ダミーのファイルシステム
        private IFileSystem m_fileSystem;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileProviderDestDummy() {
            m_fileSystem = Program.Document.FileSystemFactory.CreateFileSystemForFileOperation(FileSystemID.None);
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
                return new DummyFileListContext();
            }
            set {
            }
        }

        //=========================================================================================
        // プロパティ：転送先ディレクトリ名
        //=========================================================================================
        public string DestDirectoryName {
            get {
                return null;
            }
        }
    }
}
