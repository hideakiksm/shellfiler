using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：仮想フォルダ内ファイルの対応付け情報
    //=========================================================================================
    public class VirtualArchiveFileMapping {
        // 元のファイル（E:\dir\arc.zip\dir2\file.txt）
        private string m_displayFilePath;

        // アーカイブ内ローカル表現、先頭の「\」はなし（dir2\file.txt）
        private string m_archiveLocalPath;

        // 作成したときtrue
        private bool m_extracted;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]displayFilePath   元のファイル
        // 　　　　[in]archiveLocalPath  アーカイブ内ローカル表現
        // 戻り値：ステータス
        //=========================================================================================
        public VirtualArchiveFileMapping(string displayFilePath, string archiveLocalPath) {
            m_displayFilePath = displayFilePath;
            m_archiveLocalPath = archiveLocalPath;
            m_extracted = false;
        }

        //=========================================================================================
        // プロパティ：元のファイル（E:\dir\arc.zip\dir2\file.txt）
        //=========================================================================================
        public string DisplayFilePath {
            get {
                return m_displayFilePath;
            }
        }

        //=========================================================================================
        // プロパティ：アーカイブ内ローカル表現、先頭の「\」はなし（dir2\file.txt）
        //=========================================================================================
        public string ArchiveLocalPath {
            get {
                return m_archiveLocalPath;
            }
        }

        //=========================================================================================
        // プロパティ：作成したときtrue
        //=========================================================================================
        public bool Extracted {
            get {
                return m_extracted;
            }
            set {
                m_extracted = value;
            }
        }
    }
}
