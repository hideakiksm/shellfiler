using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.FileSystem;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：仮想フォルダ内ファイルの展開情報
    // 　　　　アーカイブ内のファイル全部の一覧を指定 → 指定ファイルの有無をチェック
    //=========================================================================================
    public class VirtualFileCopySrcMaster {
        // アーカイブ内のファイルパスからアーカイブ内インデックスへのMap
        // {{"dir2\File.txt", 1}, {"dir1\File.txt", 0}}
        private Dictionary<string, int> m_fileNameToIndex = new Dictionary<string, int>();

        // アーカイブ内のファイルパスの小文字表現からアーカイブ内インデックスへのMap
        private Dictionary<string, int> m_fileNameLowerToIndex = new Dictionary<string, int>();

        // アーカイブ内のディレクトリパスからアーカイブ内インデックスへのMap
        private Dictionary<string, int> m_directoryNameToIndex = new Dictionary<string, int>();

        // アーカイブ内のディレクトリパスの小文字表現からアーカイブ内インデックスへのMap
        private Dictionary<string, int> m_directoryNameLowerToIndex = new Dictionary<string, int>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public VirtualFileCopySrcMaster() {
        }

        //=========================================================================================
        // 機　能：アーカイブ内のファイルパスを追加する
        // 引　数：[in]arcPath   アーカイブ内のファイルパス
        // 　　　　[in]isDir     ディレクトリのときtrue
        // 　　　　[in]index     アーカイブ内インデックス
        // 戻り値：なし
        //=========================================================================================
        public void AddLocalPath(string arcPath, bool isDir, int index) {
            if (!isDir) {
                if (!m_fileNameToIndex.ContainsKey(arcPath)) {
                    m_fileNameToIndex.Add(arcPath, index);
                }
                string arcPathLower = arcPath.ToLower();
                if (!m_fileNameLowerToIndex.ContainsKey(arcPathLower)) {
                    m_fileNameLowerToIndex.Add(arcPathLower, index);
                }
            } else {
                if (!m_directoryNameToIndex.ContainsKey(arcPath)) {
                    m_directoryNameToIndex.Add(arcPath, index);
                }
                string arcPathLower = arcPath.ToLower();
                if (!m_directoryNameLowerToIndex.ContainsKey(arcPathLower)) {
                    m_directoryNameLowerToIndex.Add(arcPathLower, index);
                }
            }
        }

        //=========================================================================================
        // 機　能：指定されたアーカイブ内ファイルのインデックスを返す
        // 引　数：[in]filePath   アーカイブ内ファイルのローカルパス
        // 戻り値：アーカイブ内のインデックス（アーカイブ内にないとき-1）
        //=========================================================================================
        public int CheckContainsFile(string filePath) {
            // そのままのファイル名で確認
            if (m_fileNameToIndex.ContainsKey(filePath)) {
                int idxResult = m_fileNameToIndex[filePath];
                return idxResult;
            }

            // 小文字化したファイル名で確認
            string filePathLower = filePath.ToLower();
            if (m_fileNameLowerToIndex.ContainsKey(filePathLower)) {
                int idxResult = m_fileNameLowerToIndex[filePathLower];
                return idxResult;
            }
            return -1;
        }

        //=========================================================================================
        // 機　能：指定されたアーカイブ内ディレクトリのインデックスを返す
        // 引　数：[in]filePath   アーカイブ内ディレクトリのローカルパス
        // 戻り値：アーカイブ内のインデックス（アーカイブ内にないとき-1）
        //=========================================================================================
        public int CheckContainsDirectory(string filePath) {
            // そのままのファイル名で確認
            if (m_directoryNameToIndex.ContainsKey(filePath)) {
                int idxResult = m_directoryNameToIndex[filePath];
                return idxResult;
            }

            // 小文字化したファイル名で確認
            string filePathLower = filePath.ToLower();
            if (m_directoryNameLowerToIndex.ContainsKey(filePathLower)) {
                int idxResult = m_directoryNameLowerToIndex[filePathLower];
                return idxResult;
            }
            return -1;
        }

        //=========================================================================================
        // プロパティ：アーカイブ内のファイルパスの小文字表現からアーカイブ内インデックスへのMap
        //=========================================================================================
        public Dictionary<string, int> FileNameLowerToIndex {
            get {
                return m_fileNameLowerToIndex;
            }
        }

        //=========================================================================================
        // プロパティ：アーカイブ内のディレクトリパスの小文字表現からアーカイブ内インデックスへのMap
        //=========================================================================================
        public Dictionary<string, int> DirectoryNameLowerToIndex {
            get {
                return m_directoryNameLowerToIndex;
            }
        }
    }
}
