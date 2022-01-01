using System;
using System.Collections.Generic;
using System.Text;

using ShellFiler.Api;

namespace ShellFiler.FileSystem {

    //=========================================================================================
    // クラス：ファイル１つ分の情報
    //=========================================================================================
    public class SimpleFile : IFile {
        // ファイル名
        private string m_fileName;

        // 拡張子
        private string m_extension;

        // 更新日時
        private DateTime m_modifiedDate;

        // 属性
        private FileAttribute m_attribute;

        // ファイルサイズ
        private long m_fileSize;
        
        // デフォルトのソート順
        private int m_defaultOrder;

        // 表示用拡張子の位置とする「.」の直後の文字のインデックス（-1:拡張子を分離表示しない）
        private int m_displayExtensionPos;

        // Cloneをサポート

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileName      ファイル名
        // 　　　　[in]modifiedDate  更新日時
        // 　　　　[in]attribute     属性
        // 　　　　[in]fileSize      ファイルサイズ
        // 戻り値：なし
        //=========================================================================================
        public SimpleFile(string fileName, DateTime modifiedDate, FileAttribute attribute, long fileSize) {
            m_fileName = fileName;
            m_extension = GenericFileStringUtils.GetExtensionLast(fileName);
            m_modifiedDate = modifiedDate;
            m_attribute = attribute;
            if (attribute.IsDirectory) {
                m_fileSize = -1;
            } else {
                m_fileSize = fileSize;
            }
            m_defaultOrder = 0;
            if (m_extension.Length > 4 || m_extension.Length == 0) {
                m_displayExtensionPos = -1;
            } else {
                m_displayExtensionPos = m_fileName.Length - m_extension.Length;
            }
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
        // プロパティ：ファイル名（パス名は含まない）
        //=========================================================================================
        public string FileName {
            get {
                return m_fileName;
            }
        }

        //=========================================================================================
        // プロパティ：拡張子
        //=========================================================================================
        public string Extension {
            get {
                return m_extension;
            }
        }

        //=========================================================================================
        // プロパティ：表示用拡張子の位置とする「.」の直後の文字のインデックス（-1:拡張子を分離表示しない）
        //=========================================================================================
        public int DisplayExtensionPos {
            get {
                return m_displayExtensionPos;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルの更新時刻
        //=========================================================================================
        public DateTime ModifiedDate {
            get {
                return m_modifiedDate;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル属性
        //=========================================================================================
        public FileAttribute Attribute {
            get {
                return m_attribute;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルサイズ
        //=========================================================================================
        public long FileSize {
            get {
                return m_fileSize;
            }
            set {
                // フォルダのみの想定
                m_fileSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：デフォルトのソート順
        //=========================================================================================
        public int DefaultOrder {
            get {
                return m_defaultOrder;
            }
            set {
                m_defaultOrder = value;
            }
        }
    }
}
