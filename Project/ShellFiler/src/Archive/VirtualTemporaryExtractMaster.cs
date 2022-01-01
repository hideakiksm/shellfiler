using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：仮想フォルダ内ファイルの展開情報
    // 　　　　展開するファイル一覧を指定→アーカイブ内ファイルの有無をチェック
    //=========================================================================================
    class VirtualTemporaryExtractMaster {
        // 展開するファイル
        private readonly List<VirtualArchiveFileMapping> m_fileNameList;

        // 展開するファイルパスから要求されたファイル内インデックスへのMap
        // {{"dir2/File.txt", 1}, {"dir1/File.txt", 0}}
        private readonly Dictionary<string, int> m_fileNameToIndex = new Dictionary<string, int>();

        // 展開するファイルパスの小文字表現から要求されたファイル内インデックスへのMap
        private readonly Dictionary<string, int> m_fileNameLowerToIndex = new Dictionary<string, int>();

        // これまでに展開したファイル数
        private int m_extractCount = 0;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileNameList  展開するファイル
        // 戻り値：なし
        //=========================================================================================
        public VirtualTemporaryExtractMaster(List<VirtualArchiveFileMapping> fileNameList) {
            m_fileNameList = fileNameList;

            // 初期化
            for (int i = 0; i < fileNameList.Count; i++) {
                string fileName = fileNameList[i].ArchiveLocalPath;
                if (!m_fileNameToIndex.ContainsKey(fileName)) {
                    m_fileNameToIndex.Add(fileName, i);
                }
                string fileNameLower = fileName.ToLower();
                if (!m_fileNameLowerToIndex.ContainsKey(fileNameLower)) {
                    m_fileNameLowerToIndex.Add(fileNameLower, i);
                }
            }
        }

        //=========================================================================================
        // 機　能：指定されたアーカイブ内ファイルが展開対象になっているかどうかをチェックする
        // 引　数：[in]filePath   アーカイブ内のローカルパス
        // 戻り値：展開対象になっているときtrue
        //=========================================================================================
        public bool CheckContains(string filePath) {
            // そのままのファイル名で確認
            if (m_fileNameToIndex.ContainsKey(filePath)) {
                int idxResult = m_fileNameToIndex[filePath];
                if (!m_fileNameList[idxResult].Extracted) {
                    m_extractCount++;
                }
                m_fileNameList[idxResult].Extracted = true;
                return true;
            }

            // 小文字化したファイル名で確認
            string filePathLower = filePath.ToLower();
            if (m_fileNameLowerToIndex.ContainsKey(filePathLower)) {
                int idxResult = m_fileNameLowerToIndex[filePathLower];
                m_fileNameList[idxResult].Extracted = true;
                if (!m_fileNameList[idxResult].Extracted) {
                    m_extractCount++;
                }
                return true;
            }

            // ファイル名の途中のパス
            string[] pathLowerList = filePathLower.Split('\\');
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < pathLowerList.Length - 1; j++) {
                if (j > 0) {
                    sb.Append('\\');
                }
                sb.Append(pathLowerList[j]);
                string path = sb.ToString();
                if (m_fileNameLowerToIndex.ContainsKey(path)) {
                    int idxResult = m_fileNameLowerToIndex[path];
                    if (!m_fileNameList[idxResult].Extracted) {
                        m_extractCount++;
                    }
                    m_fileNameList[idxResult].Extracted = true;
                    return true;
                }
            }
            return false;
        }

        //=========================================================================================
        // プロパティ：これまでに展開したファイル数
        //=========================================================================================
        public int ExtractCount {
            get {
                return m_extractCount;
            }
        }

        //=========================================================================================
        // プロパティ：全ファイル数
        //=========================================================================================
        public int AllFileCount {
            get {
                return m_fileNameList.Count;
            }
        }
    }
}
