using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：パスヒストリの項目情報
    //=========================================================================================
    public class PathHistoryItem : ICloneable {
        // ディレクトリ（最後の"\"または"/"を含まない）
        private string m_directory;

        // 最後のファイル名
        private string m_fileName;

        // ファイルシステムのID
        private FileSystemID m_fileSystemId;

        // シーケンス番号
        private int m_sequenceNo;

        // 次に発行するシーケンス番号
        private static int s_nextSequenceNo = 0;

        // Cloneをサポート

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]directory  ディレクトリ
        // 　　　　[in]fileName   ファイル名
        // 　　　　[in]fileSystemId ファイルシステムのID
        // 戻り値：なし
        //=========================================================================================
        public PathHistoryItem(string directory, string fileName, FileSystemID fileSystemId) {
            if (directory.EndsWith("\\") || directory.EndsWith("/")) {
                m_directory = directory.Substring(0, directory.Length - 1);
            } else {
                m_directory = directory;
            }
            m_fileName = fileName;
            m_fileSystemId = fileSystemId;
            m_sequenceNo = Interlocked.Add(ref s_nextSequenceNo, 1);
        }
        
        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            return MemberwiseClone();
        }

        //=========================================================================================
        // プロパティ：ディレクトリ（最後の"\"または"/"を含まない）
        //=========================================================================================
        public string Directory {
            get {
                return m_directory;
            }
        }

        //=========================================================================================
        // プロパティ：最後のファイル名
        //=========================================================================================
        public string FileName {
            get {
                return m_fileName;
            }
            set {
                m_fileName = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルシステムのID
        //=========================================================================================
        public FileSystemID FileSystemID {
            get {
                return m_fileSystemId;
            }
        }

        //=========================================================================================
        // プロパティ：シーケンス番号
        //=========================================================================================
        public int SequenceNo {
            get {
                return m_sequenceNo;
            }
        }
    }
}
