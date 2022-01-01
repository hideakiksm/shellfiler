using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.FileSystem.SFTP;

namespace ShellFiler.FileSystem.Virtual {

    //=========================================================================================
    // クラス：ファイル名を一意とする仮想フォルダ内ファイルの配列
    //=========================================================================================
    public class UniqueVirtualFileArray {
        // ファイル名からファイルインデックスへのマップ
        Dictionary<string, int> m_fileNameToIndex = new Dictionary<string, int>();

        // ファイル名の一覧
        List<VirtualFile> m_fileList = new List<VirtualFile>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public UniqueVirtualFileArray() {
        }

        //=========================================================================================
        // 機　能：ファイルを追加する
        // 引　数：[in]file  追加するファイル（null可）
        // 戻り値：なし
        //=========================================================================================
        public void Add(VirtualFile file) {
            if (file == null) {
                m_fileList.Add(null);
                return;
            }
            string fileName = file.FileName.ToLower();
            if (m_fileNameToIndex.ContainsKey(fileName)) {
                if (file.ModifiedDate != DateTime.MinValue) {
                    // 日付が有効な場合は正規ファイルのため差し替え
                    int index = m_fileNameToIndex[fileName];
                    m_fileList[index] = file;
                }
            } else {
                int index = m_fileList.Count;
                m_fileNameToIndex.Add(fileName, index);
                m_fileList.Add(file);
            }
        }

        //=========================================================================================
        // プロパティ：ファイル名の一覧
        //=========================================================================================
        public List<VirtualFile> FileList {
            get {
                return m_fileList;
            }
        }
   }
}