using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;

namespace ShellFiler.FileTask.Provider {

    //=========================================================================================
    // クラス：ダミーの転送元ファイルの表現（SRCではファイルシステムだけを認識）
    //=========================================================================================
    class FileProviderSrcDummy : IFileProviderSrc {
        // 転送元ディレクトリのファイルシステム
        private IFileSystem m_fileSystem;

        // ファイル一覧のコンテキスト情報
        private IFileListContext m_fileListContext;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileSystem   ファイルシステム
        // 　　　　[in]fileListCtx  ファイル一覧のコンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public FileProviderSrcDummy(IFileSystem fileSystem, IFileListContext fileListCtx) {
            m_fileSystem = fileSystem;
            m_fileListContext = fileListCtx;
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
                return m_fileListContext;
            }
            set {
                m_fileListContext = value;
            }
        }

        //=========================================================================================
        // プロパティ：転送元ファイルとディレクトリの数
        //=========================================================================================
        public int SrcItemCount {
            get {
                return 0;
            }
        }

        //=========================================================================================
        // 機　能：転送元のフルパスを返す
        // 引　数：[in]index    インデックス
        // 戻り値：転送元のファイル情報
        //=========================================================================================
        public SimpleFileDirectoryPath GetSrcPath(int index) {
            return null;
        }
    }
}
