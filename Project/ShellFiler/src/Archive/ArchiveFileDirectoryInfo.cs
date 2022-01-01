using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;

namespace ShellFiler.FileTask.Provider {

    //=========================================================================================
    // クラス：アーカイブ対象のファイルまたはディレクトリの情報
    //=========================================================================================
    public class ArchiveFileDirectoryInfo {
        // ファイルサイズが不明な場合（ディレクトリは必ず指定）
        public const long UnknownFileSize = -1;

        // ファイルパス
        private string m_filePath;
        
        // ディレクトリのときtrue
        private bool m_isDirectory;

        // ファイルサイズ、ディレクトリ配下サイズの合計
        private long m_size;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]filePath    ファイルパス
        // 　　　　[in]isDirectory ディレクトリのときtrue
        // 　　　　[in]size        ファイルサイズ、ディレクトリ配下サイズの合計
        // 戻り値：なし
        //=========================================================================================
        public ArchiveFileDirectoryInfo(string filePath, bool isDirectory, long size) {
            m_filePath = filePath;
            m_isDirectory = isDirectory;
            m_size = size;
        }
        
        //=========================================================================================
        // プロパティ：ファイルパス
        //=========================================================================================
        public string FilePath {
            get {
                return m_filePath;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリのときtrue
        //=========================================================================================
        public bool IsDirectory {
            get {
                return m_isDirectory;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルサイズ、ディレクトリ配下サイズの合計（UnknownFileSizeは取得が必要）
        //=========================================================================================
        public long Size {
            get {
                return m_size;
            }
            set {
                m_size = value;
            }
        }
    }
}
