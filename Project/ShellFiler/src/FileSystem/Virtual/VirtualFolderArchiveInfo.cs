using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Locale;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.FileSystem.Virtual {

    //=========================================================================================
    // クラス：仮想ディレクトリのアーカイブ1つ分の情報
    //=========================================================================================
    public class VirtualFolderArchiveInfo {
        // この仮想フォルダの表示パス（最後に「/」または「\」がつく）
        // user@server:/home/user/data/file1.zip/dir1/file2.tar.gz/
        private string m_displayPathArchive;

        // はじめにアクセスするアーカイブのファイル名（m_resourceManage==trueのときは最後に削除）
        // C:\Temp\Virtual\001\a01.tar.gz
        private string m_archiveFile;

        // m_archiveFileのファイルをこのクラス内でライフサイクル管理するときtrue
        private bool m_resourceManage;

        // 内部ファイルがあるとき、そのアーカイブファイル名（最後に削除、2重に圧縮されていないときはnull）
        // C:\Temp\Virtual\001\a02.tar
        private string m_internalArchiveFile;

        // アーカイブアクセスで使用するパスワード（"":パスワード空、null:パスワードなし/不明）
        private string m_password;

        // Clone()せず、Clone()相当の動きをしている
        // テンポラリを置くことは不可

        //=========================================================================================
        // 機　能：Windowsのアーカイブファイルから仮想フォルダの情報を作成する
        // 引　数：[in]archivePath   アーカイブファイルのファイルパス
        // 戻り値：なし
        //=========================================================================================
        public static VirtualFolderArchiveInfo FromWindowsArchive(string archivePath) {
            VirtualFolderArchiveInfo archive = new VirtualFolderArchiveInfo();
            archive.m_displayPathArchive = archivePath;
            archive.m_archiveFile = GenericFileStringUtils.RemoveLastDirectorySeparator(archivePath);
            archive.m_resourceManage = false;
            archive.m_internalArchiveFile = null;
            archive.m_password = null;
            return archive;
        }

        //=========================================================================================
        // 機　能：SSHのアーカイブファイルから仮想フォルダの情報を作成する
        // 引　数：[in]dispArchivePath   アーカイブファイルの表示パス（user@server:/dir/file.zip）
        // 　　　　[in]localArchivePath  ローカルに用意したアーカイブファイルのパス（C:\...\TEMP\file.zip）
        // 戻り値：なし
        //=========================================================================================
        public static VirtualFolderArchiveInfo FromSSHArchive(string dispArchivePath, string localArchivePath) {
            VirtualFolderArchiveInfo archive = new VirtualFolderArchiveInfo();
            archive.m_displayPathArchive = dispArchivePath;
            archive.m_archiveFile = localArchivePath;
            archive.m_resourceManage = true;
            archive.m_internalArchiveFile = null;
            archive.m_password = null;
            return archive;
        }

        //=========================================================================================
        // 機　能：仮想フォルダ内のアーカイブファイルから仮想フォルダの情報を作成する
        // 引　数：[in]baseFileSystem   一番外側のアーカイブにアクセスできるファイルシステム
        // 　　　　[in]virtualItem      一番外側のアーカイブの情報
        // 戻り値：なし
        //=========================================================================================
        public static VirtualFolderArchiveInfo FromVirtualArchive(string dispPath, string archiveFile, string password) {
            VirtualFolderArchiveInfo archive = new VirtualFolderArchiveInfo();
            archive.m_displayPathArchive = dispPath;
            archive.m_archiveFile = archiveFile;
            archive.m_resourceManage = true;
            archive.m_internalArchiveFile = null;
            archive.m_password = password;
            return archive;
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private VirtualFolderArchiveInfo() {
        }

        //=========================================================================================
        // 機　能：後始末を行う
        // 引　数：[in]dir  調べるディレクトリ名
        // 戻り値：処理できるときtrue
        //=========================================================================================
        public void Dispose() {
            if (m_resourceManage) {
                try {
                    File.Delete(m_internalArchiveFile);
                } catch (Exception) {
                }
            }
            if (m_internalArchiveFile != null) {
                try {
                    File.Delete(m_internalArchiveFile);
                } catch (Exception) {
                }
            }
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public VirtualFolderArchiveInfo CloneArchiveInfo() {
            return this;
        }

        //=========================================================================================
        // 機　能：この仮想フォルダで処理できるディレクトリかどうかを調べる
        // 引　数：[in]dir  調べるディレクトリ名
        // 戻り値：処理できるときtrue
        //=========================================================================================
        public bool IsIncludedAbsolutePath(string dir) {
            string dirLower = dir.ToLower().Replace('/', '\\');
            string arcLower = m_displayPathArchive.ToLower().Replace('/', '\\');
            dirLower = GenericFileStringUtils.CompleteDirectoryName(dirLower, "\\");
            if (dirLower.StartsWith(arcLower)) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：ライフサイクルが管理状態にある一時ファイルのリストを返す
        // 引　数：なし
        // 戻り値：一時ファイルのリスト（最後が「\」のものはディレクトリ、それ以外はファイル）
        //=========================================================================================
        public List<string> GetManagedTemporaryList() {
            List<string> result = new List<string>();
            if (m_resourceManage) {
                result.Add(m_archiveFile);
            }
            if (m_internalArchiveFile != null) {
                result.Add(m_internalArchiveFile);
            }
            return result;
        }

        //=========================================================================================
        // プロパティ：この仮想フォルダの表示パス（最後に「/」または「\」がつく）
        //=========================================================================================
        public string DisplayPathArchive {
            get {
                return m_displayPathArchive;
            }
        }

        //=========================================================================================
        // プロパティ：はじめにアクセスするアーカイブのファイル名
        //=========================================================================================
        public string ArchiveFile {
            get {
                return m_archiveFile;
            }
        }

        //=========================================================================================
        // プロパティ：内部ファイルがあるとき、そのアーカイブファイル名（最後に削除、2重に圧縮されていないときはnull）
        //=========================================================================================
        public string InnerArchiveFile {
            get {
                return m_internalArchiveFile;
            }
            set {
                m_internalArchiveFile = value;
            }
        }

        //=========================================================================================
        // プロパティ：実際にアクセスするアーカイブファイル
        //=========================================================================================
        public string RealArchiveFile {
            get {
                if (m_internalArchiveFile != null) {
                    return m_internalArchiveFile;
                } else {
                    return m_archiveFile;
                }
            }
        }

        //=========================================================================================
        // プロパティ：アーカイブアクセスで使用するパスワード（"":パスワード空、null:パスワードなし/不明）
        //=========================================================================================
        public string Password {
            get {
                return m_password;
            }
            set {
                m_password = value;
            }
        }
    }
}
