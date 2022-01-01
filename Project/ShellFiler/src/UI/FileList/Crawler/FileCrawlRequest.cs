using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;

namespace ShellFiler.UI.FileList.Crawler {

    //=========================================================================================
    // クラス：ファイル一覧をクロールするためのリクエスト
    //=========================================================================================
    public class FileCrawlRequest {
        // クロール対象のファイル一覧（UIスレッド以外で読み込めない）
        private UIFileList m_fileList;

        // ファイル一覧
        private List<UIFile> m_fileListFiles;

        // ファイル一覧のID
        private UIFileListId m_uiFileListId;

        // ファイルシステムの種類
        private FileSystemID m_fileSystemId;

        // 一覧のディレクトリ名
        private string m_directoryName;

        // クロールによって取得する情報
        private CrawlType m_crawlType;

        // リクエストの追加パラメータ
        private object m_param;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileList   クロール対象のファイル一覧
        // 　　　　[in]crawlType  クロールによって取得する情報
        // 　　　　[in]param      リクエストの追加パラメータ
        // 戻り値：なし
        //=========================================================================================
        public FileCrawlRequest(UIFileList fileList, CrawlType crawlType, object param) {
            m_fileList = fileList;
            m_fileListFiles = fileList.Files;
            m_uiFileListId = fileList.UIFileListId;
            m_fileSystemId = fileList.FileSystem.FileSystemId;
            m_directoryName = fileList.DisplayDirectoryName;
            m_crawlType = crawlType;
            m_param = param;
        }

        //=========================================================================================
        // プロパティ：クロール対象のファイル一覧（UIスレッド以外で読み込めない）
        //=========================================================================================
        public UIFileList FileList {
            get {
                return m_fileList;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧
        //=========================================================================================
        public List<UIFile> FileListFiles {
            get {
                return m_fileListFiles;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧のID
        //=========================================================================================
        public UIFileListId UiFileListId {
            get {
                return m_uiFileListId;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルシステムの種類
        //=========================================================================================
        public FileSystemID FileSystemId {
            get {
                return m_fileSystemId;
            }
        }

        //=========================================================================================
        // プロパティ：一覧のディレクトリ名
        //=========================================================================================
        public string DirectoryName {
            get {
                return m_directoryName;
            }
        }

        //=========================================================================================
        // プロパティ：クロールによって取得する情報
        //=========================================================================================
        public CrawlType CrawlType {
            get {
                return m_crawlType;
            }
        }

        //=========================================================================================
        // プロパティ：リクエストの追加パラメータ
        //=========================================================================================
        public object Param {
            get {
                return m_param;
            }
        }
    }
}
