using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document.Setting;
using ShellFiler.Util;
using ShellFiler.FileTask.DataObject;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.Virtual {

    //=========================================================================================
    // クラス：パスの各要素がファイルまたはフォルダとして実在するかどうかのフラグ
    // dir1\dir2\dir3でdir2がアーカイブファイルのとき、
    //     m_subDirectoryList = {{"dir1",Folder}, {"dir1\dir2",File}, {"dir1\dir2\dir3",Unknown}}
    //=========================================================================================
    class SubPathExistFlag {
        // サブディレクトリのリスト
        private List<SubDirectory> m_subDirectoryList = new List<SubDirectory>();
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]dispPath    一覧を取得しようとしているパスの表示形式 user@server:/dir/arc.zip/dir2/dir3/
        // 　　　　[in]arcPath     dispPath上のアーカイブのパスの表示形式   user@server:/dir/arc.zip/
        // 戻り値：なし
        //=========================================================================================
        public SubPathExistFlag(string dispPath, string arcPath) {
            dispPath = GenericFileStringUtils.RemoveLastDirectorySeparator(dispPath);
            arcPath = GenericFileStringUtils.RemoveLastDirectorySeparator(arcPath);
            string subPath = dispPath.Substring(arcPath.Length);
            if (subPath.Length > 1 && (subPath[0] == '/' || subPath[0] == '\\')) {
                subPath = subPath.Substring(1);
            }
            string[] subDirList = subPath.Split('/', '\\');
            string path = "";
            string pathLowEn = "\\";                        // 比較用、\xxx\xxx\形式
            for (int i = 0; i < subDirList.Length; i++) {
                if (i != 0) {
                    path = path + "\\";
                }
                path = path + subDirList[i];
                pathLowEn = pathLowEn + subDirList[i].ToLower() + "\\";
                m_subDirectoryList.Add(new SubDirectory(path, pathLowEn));
            }
        }

        //=========================================================================================
        // 機　能：存在しているパスの情報を設定する
        // 引　数：[in]arcPathLowEn  存在していたパス（すべて小文字、\xxx\xxx\形式）
        // 　　　　[in]isDir         ディレクトリのときtrue
        // 　　　　[in]contentsIndex 圧縮ファイルのインデックス
        // 戻り値：なし
        //=========================================================================================
        public void SetExistContents(string arcPathLowEn, bool isDir, int contentsIndex) {
            for (int i = 0; i < m_subDirectoryList.Count; i++) {
                if (m_subDirectoryList[i].PathCompareLowEn == arcPathLowEn) {
                    if (isDir) {
                        m_subDirectoryList[i].FileFolderType = FileFolderType.Folder;
                    } else {
                        m_subDirectoryList[i].FileFolderType = FileFolderType.File;
                    }
                    m_subDirectoryList[i].ContentsIndex = contentsIndex;
                    break;
                }
            }
        }

        //=========================================================================================
        // 機　能：存在しているパスの情報を設定する
        // 引　数：[out]subVirtualDispPath  仮想フォルダにできる表示用パス名
        // 　　　　[out]subVirtualIndex     仮想フォルダにできるファイルのあるインデックス
        // 戻り値：サブフォルダに仮想フォルダがある可能性があるときtrue
        //=========================================================================================
        public bool GetSubVirtualPath(out string subVirtualDispPath, out int subVirtualIndex) {
            subVirtualDispPath = null;
            subVirtualIndex = -1;
            for (int i = m_subDirectoryList.Count - 1; i >= 0; i--) {
                FileFolderType type = m_subDirectoryList[i].FileFolderType;
                if (type == FileFolderType.File) {
                    // パスの右端がファイルなら成功
                    subVirtualDispPath = m_subDirectoryList[i].Path;
                    subVirtualIndex = m_subDirectoryList[i].ContentsIndex;
                    return true;
                } else if (type == FileFolderType.Folder) {
                    return false;
                }
            }
            return false;
        }

        //=========================================================================================
        // クラス：サブディレクトリの状態
        //=========================================================================================
        private class SubDirectory {
            // パス名、\Xxx\XXX形式
            private string m_path;

            // 比較用パス名、すべて小文字\xxx\xxx\形式
            private string m_pathLowEn;

            // フォルダの種類
            private FileFolderType m_fileFolderType;

            // 格納ファイルの圧縮ファイル内のインデックス（未設定のとき-1）
            private int m_contentsIndex;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]path           パス名、\Xxx\XXX形式
            // 　　　　[in]pathLowEn      比較用パス名、すべて小文字\xxx\xxx\形式
            // 戻り値：なし
            //=========================================================================================
            public SubDirectory(string path, string pathLowEn) {
                m_path = path;
                m_pathLowEn = pathLowEn;
                m_fileFolderType = FileFolderType.Unknown;
                m_contentsIndex = -1;
            }

            //=========================================================================================
            // プロパティ：パス名
            //=========================================================================================
            public string Path {
                get {
                    return m_path;
                }
            }

            //=========================================================================================
            // プロパティ：比較用パス名、すべて小文字\xxx\xxx\形式
            //=========================================================================================
            public string PathCompareLowEn {
                get {
                    return m_pathLowEn;
                }
            }

            //=========================================================================================
            // プロパティ：格納ファイルの圧縮ファイル内のインデックス（未設定のとき-1）
            //=========================================================================================
            public int ContentsIndex {
                get {
                    return m_contentsIndex;
                }
                set{
                    m_contentsIndex = value;
                }
            }

            //=========================================================================================
            // プロパティ：フォルダの種類
            //=========================================================================================
            public FileFolderType FileFolderType {
                get {
                    return m_fileFolderType;
                }
                set {
                    m_fileFolderType = value;
                }
            }
        }

        //=========================================================================================
        // 列挙子：ファイルとフォルダの種類
        //=========================================================================================
        private enum FileFolderType {
            File,                           // 種別はファイル（アーカイブファイルの可能性）
            Folder,                         // 種別はフォルダ
            Unknown,                        // 種別は不明（未処理）
        }
    }
}
