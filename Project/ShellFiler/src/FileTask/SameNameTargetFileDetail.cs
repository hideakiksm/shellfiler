using System;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.UI;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：同名ファイルの詳細情報
    //=========================================================================================
    public class SameNameTargetFileDetail {
        // 転送元ファイルパス(C:\SRCBASE\MARKFILE.txt)
        private string m_srcFilePath;

        // 転送先ファイルパス(D:\DESTBASE\MARKFILE.txt)
        private string m_destFilePath;

        // 転送元ファイルシステムのID
        private FileSystemID m_srcFileSystemId;

        // 転送先ファイルシステムのID
        private FileSystemID m_destFileSystemId;

        // 完全な情報取得に成功したときtrue
        private bool m_successFullInfo;

        // 転送元ファイルの更新時刻（取得できないときMinValue）
        private DateTime m_srcLastWriteTime;

        // 転送先ファイルの更新時刻（取得できないときMinValue）
        private DateTime m_destLastWriteTime;

        // 転送元ファイルのサイズ（取得できないとき-1）
        private long m_srcFileSize;

        // 転送先ファイルのサイズ（取得できないとき-1）
        private long m_destFileSize;

        // 転送元ファイルのアイコン（取得できないときnull）
        private Icon m_srcIcon;
        
        // 転送先ファイルのアイコン（取得できないときnull）
        private Icon m_destIcon;

        // 転送元に対して実ファイルのアイコンを展開するときtrue
        private bool m_srcIconExtractReal;

        // アイコン取得用の転送元ファイルシステム
        private IFileSystem m_srcFileSystemForIcon;

        // アイコン取得用の転送先ファイルシステム
        private IFileSystem m_destFileSystemForIcon;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]context        コンテキスト情報
        // 　　　　[in]srcFileSystem  転送元のファイルシステム
        // 　　　　[in]destFileSystem 転送先のファイルシステム
        // 　　　　[in]srcFilePath    転送元のファイル名（C:\DIR1\SRCFILE.txt）
        // 　　　　[in]destFilePath   転送先のファイル名（C:\DIR2\DESTFILE.txt）
        // 戻り値：なし
        //=========================================================================================
        public SameNameTargetFileDetail(FileOperationRequestContext context, IFileSystem srcFileSystem, IFileSystem destFileSystem, string srcFilePath, string destFilePath) {
            m_srcFileSystemId = srcFileSystem.FileSystemId;
            m_srcFilePath = srcFilePath;
            m_destFilePath = destFilePath;
            m_destFileSystemId = destFileSystem.FileSystemId;

            IFile srcFile = null;
            FileOperationStatus srcStatus = srcFileSystem.GetFileInfo(context, srcFilePath, true, out srcFile);
            IFile destFile = null;
            FileOperationStatus destStatus = destFileSystem.GetFileInfo(context, destFilePath, false, out destFile);
            if (srcStatus.Succeeded && destStatus.Succeeded) {
                m_successFullInfo = true;
                m_srcLastWriteTime = srcFile.ModifiedDate;
                m_destLastWriteTime = destFile.ModifiedDate;
                m_srcFileSize = srcFile.FileSize;
                m_destFileSize = destFile.FileSize;
            } else {
                m_successFullInfo = false;
                m_srcLastWriteTime = DateTime.MinValue;
                m_destLastWriteTime = DateTime.MinValue;
                m_srcFileSize = -1;
                m_destFileSize = -1;
            }
            m_srcIconExtractReal = true;
            m_srcFileSystemForIcon = srcFileSystem;
            m_destFileSystemForIcon = destFileSystem;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（展開処理用）
        // 引　数：[in]context        コンテキスト情報
        // 　　　　[in]archive        アーカイブ
        // 　　　　[in]srcFileInfo    転送先の圧縮内ファイル情報
        // 　　　　[in]destFileSystem 転送先のファイルシステム
        // 　　　　[in]destFilePath   転送先のファイル名（C:\DIR2\DESTFILE.txt）
        // 戻り値：なし
        //=========================================================================================
        public SameNameTargetFileDetail(FileOperationRequestContext context, IArchiveExtract archive, IArchiveContentsFileInfo srcFileInfo, IFileSystem destFileSystem, string destFilePath) {
            m_srcFilePath = archive.ArchiveFileName + "\\" + srcFileInfo.FilePath;
            m_destFilePath = destFilePath;

            IFile destFile = null;
            FileOperationStatus destStatus = destFileSystem.GetFileInfo(context, destFilePath, false, out destFile);
            if (destStatus.Succeeded && srcFileInfo.LastWriteTime != DateTime.MinValue && srcFileInfo.FileSize >= 0) {
                m_successFullInfo = true;
                m_srcLastWriteTime = srcFileInfo.LastWriteTime;
                m_destLastWriteTime = destFile.ModifiedDate;
                m_srcFileSize = srcFileInfo.FileSize;
                m_destFileSize = destFile.FileSize;
            } else {
                m_successFullInfo = false;
                m_srcLastWriteTime = DateTime.MinValue;
                m_destLastWriteTime = DateTime.MinValue;
                m_srcFileSize = -1;
                m_destFileSize = -1;
            }
            m_srcIconExtractReal = true;
            m_srcFileSystemForIcon = destFileSystem;
            m_destFileSystemForIcon = destFileSystem;
        }

        //=========================================================================================
        // 機　能：破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_srcIcon != null) {
                m_srcIcon.Dispose();
                m_srcIcon = null;
            }
            if (m_destIcon != null) {
                m_destIcon.Dispose();
                m_destIcon = null;
            }
        }

        //=========================================================================================
        // 機　能：アイコンを取得する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ExtractIcon() {
            if (m_srcIconExtractReal) {
                m_srcIcon = m_srcFileSystemForIcon.ExtractFileIcon(m_srcFilePath, false, true, UIIconManager.CX_LARGE_ICON, UIIconManager.CY_LARGE_ICON);
                m_destIcon = m_destFileSystemForIcon.ExtractFileIcon(m_destFilePath, false, true, UIIconManager.CX_LARGE_ICON, UIIconManager.CY_LARGE_ICON);
            } else {
                m_srcIcon = m_srcFileSystemForIcon.ExtractFileIcon(m_srcFilePath, false, false, UIIconManager.CX_LARGE_ICON, UIIconManager.CY_LARGE_ICON);     // 転送先で代用
                m_destIcon = m_destFileSystemForIcon.ExtractFileIcon(m_destFilePath, false, true, UIIconManager.CX_LARGE_ICON, UIIconManager.CY_LARGE_ICON);
            }
        }

        //=========================================================================================
        // プロパティ：転送元ファイルパス(C:\SRCBASE\MARKFILE.txt)
        //=========================================================================================
        public string SrcFilePath {
            get {
                return m_srcFilePath;
            }
        }

        //=========================================================================================
        // プロパティ：転送先ファイルパス(D:\DESTBASE\MARKFILE.txt)
        //=========================================================================================
        public string DestFilePath {
            get {
                return m_destFilePath;
            }
        }

        //=========================================================================================
        // プロパティ：転送元ファイルシステムのID
        //=========================================================================================
        public FileSystemID SrcFileSystemId {
            get {
                return m_srcFileSystemId;
            }
        }

        //=========================================================================================
        // プロパティ：転送先ファイルシステムのID
        //=========================================================================================
        public FileSystemID DestFileSystemId {
            get {
                return m_destFileSystemId;
            }
        }

        //=========================================================================================
        // プロパティ：完全な情報取得に成功したときtrue
        //=========================================================================================
        public bool SuccessFullInfo {
            get {
                return m_successFullInfo;
            }
        }

        //=========================================================================================
        // プロパティ：転送元ファイルの更新時刻
        //=========================================================================================
        public DateTime SrcLastWriteTime {
            get {
                return m_srcLastWriteTime;
            }
        }

        //=========================================================================================
        // プロパティ：転送先ファイルの更新時刻
        //=========================================================================================
        public DateTime DestLastWriteTime {
            get {
                return m_destLastWriteTime;
            }
        }

        //=========================================================================================
        // プロパティ：転送元ファイルのサイズ
        //=========================================================================================
        public long SrcFileSize {
            get {
                return m_srcFileSize;
            }
        }

        //=========================================================================================
        // プロパティ：転送先ファイルのサイズ
        //=========================================================================================
        public long DestFileSize {
            get {
                return m_destFileSize;
            }
        }

        //=========================================================================================
        // プロパティ：転送元ファイルのアイコン（取得できないときnull）
        //=========================================================================================
        public Icon SrcIcon {
            get {
                return m_srcIcon;
            }
        }

        //=========================================================================================
        // プロパティ：転送先ファイルのアイコン（取得できないときnull）
        //=========================================================================================
        public Icon DestIcon {
            get {
                return m_destIcon;
            }
        }
    }
}
