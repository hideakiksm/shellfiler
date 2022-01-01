using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.FileSystem;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：ファイルまたはディレクトリのパス表現
    //=========================================================================================
    public class SimpleFileDirectoryPath : ICloneable {
        // ファイルパス
        private string m_filePath;
        
        // ディレクトリのときtrue
        private bool m_isDirectory;

        // シンボリックリンクのときtrue
        private bool m_isSymbolicLink;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]filePath    ファイルパス
        // 　　　　[in]isDirectory ディレクトリのときtrue
        // 　　　　[in]isLink      シンボリックリンクのときtrue
        // 戻り値：なし
        //=========================================================================================
        public SimpleFileDirectoryPath(string filePath, bool isDirectory, bool isLink) {
            m_filePath = filePath;
            m_isDirectory = isDirectory;
            m_isSymbolicLink = isLink;
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
        // 機　能：オブジェクトが同じかどうかを判断する
        // 引　数：[in]obj   比較対象のオブジェクト
        // 戻り値：同じときtrue
        //=========================================================================================
        public override bool Equals(object obj) {
            if (obj == null || !(obj is SimpleFileDirectoryPath)) {
                return false;
            }
            SimpleFileDirectoryPath simple = (SimpleFileDirectoryPath)obj;
            if (m_filePath == simple.m_filePath && m_isDirectory == simple.m_isDirectory && m_isSymbolicLink == simple.m_isSymbolicLink) {
                return true;
            } else {
                return false;
            }
        }
        
        //=========================================================================================
        // 機　能：ハッシュコードを返す
        // 引　数：なし
        // 戻り値：ハッシュコード
        //=========================================================================================
        public override int GetHashCode() {
            int hashCode = m_filePath.GetHashCode();
            if (m_isDirectory) {
                hashCode ^= 0x55555555;
            }
            if (m_isSymbolicLink) {
                hashCode ^= 0x11111111;
            }
            return hashCode;
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
        // プロパティ：シンボリックリンクのときtrue
        //=========================================================================================
        public bool IsSymbolicLink {
            get {
                return m_isSymbolicLink;
            }
        }
    }
}
