using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：汎用ファイルシステムでの属性
    //=========================================================================================
    public class FileAttribute : ICloneable {
        private const int ATTRIBUTE_SYMLINK   = 0x10000000;         // 属性：シンボリックリンク
        private const int ATTRIBUTE_EXEC      = 0x20000000;         // 属性：実行可能
        private const int ATTRIBUTE_LINKERROR = 0x40000000;         // 属性：リンク切れ

        public const int CHMOD_PERMISSION_MASK = 0x01ff; // chmodで指定するパーミッション
		public const int S_IRUSR = 0x0100; // read by owner
		public const int S_IWUSR = 0x0080; // write by owner
		public const int S_IXUSR = 0x0040; // execute/search by owner

		public const int S_IRGRP = 0x0020; // read by group
		public const int S_IWGRP = 0x0010; // write by group
		public const int S_IXGRP = 0x0008; // execute/search by group

		public const int S_IROTH = 0x0004; // read by others
		public const int S_IWOTH = 0x0002; // write by others
		public const int S_IXOTH = 0x0001; // execute/search by others

        // 属性の値
        private FileAttributes m_attribute;

        // ソート順
        private int m_sortOrder;

        // 文字列表記（Windowsで未設定のときnull）
        private string m_attributeString;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileAttribute() {
            m_attribute = FileAttributes.Normal;
            m_sortOrder = 0;
        }

        //=========================================================================================
        // 機　能：Windowsファイルシステムの属性からファイル属性を取得する
        // 引　数：[in]attr       Windowsファイルシステムのファイル属性
        // 　　　　[in]isSynLink  シンボリックリンクのときtrue
        // 戻り値：IFileのファイル属性
        //=========================================================================================
        public static FileAttribute FromFileInfo(FileAttributes attr, bool isSymLink) {
            // 属性を変換
            FileAttribute objAttr = new FileAttribute();
            objAttr.m_attribute = attr;
            objAttr.m_sortOrder = (int)(attr & (FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System | FileAttributes.Directory | FileAttributes.Archive));
            objAttr.m_attributeString = null;
            if (isSymLink) {
                objAttr.IsSymbolicLink = true;
            }
            return objAttr;
        }

        //=========================================================================================
        // 機　能：Linuxのパーミッションからファイル属性を取得する
        // 引　数：[in]fileName     ファイル名
        // 　　　　[in]permissions  パーミッションの数値
        // 　　　　[in]isDir        ディレクトリのときtrue
        // 　　　　[in]isLink       シンボリックリンクのときtrue
        // 戻り値：IFileのファイル属性
        //=========================================================================================
        public static FileAttribute FromLinuxPermissions(string fileName, int permissions, bool isDir, bool isLink) {
            // 属性を変換
            int sortOrder = permissions;
            if (isDir) {
                sortOrder += 1000;
            }
            if (isLink) {
                sortOrder += 2000;
            }
            int personal = (permissions / 100) % 10;
            bool isExec = ((personal & 1) != 0);
            bool isHidden = ((fileName.Length >= 2) && fileName.StartsWith("."));

            string attr = "";
            if (isLink) {
                attr += "s";
            } else if (isDir) {
                attr += "d";
            } else {
                attr += "-";
            }
            if ((permissions & S_IRUSR) != 0) {
                attr += "r";
            } else {
                attr += "-";
            }
            if ((permissions & S_IWUSR) != 0) {
                attr += "w";
            } else {
                attr += "-";
            }
            if ((permissions & S_IXUSR) != 0) {
                attr += "x";
            } else {
                attr += "-";
            }
            if ((permissions & S_IRGRP) != 0) {
                attr += "r";
            } else {
                attr += "-";
            }
            if ((permissions & S_IWGRP) != 0) {
                attr += "w";
            } else {
                attr += "-";
            }
            if ((permissions & S_IXGRP) != 0) {
                attr += "x";
            } else {
                attr += "-";
            }
            if ((permissions & S_IROTH) != 0) {
                attr += "r";
            } else {
                attr += "-";
            }
            if ((permissions & S_IWOTH) != 0) {
                attr += "w";
            } else {
                attr += "-";
            }
            if ((permissions & S_IXOTH) != 0) {
                attr += "x";
            } else {
                attr += "-";
            }

            // オブジェクトに変換
            FileAttribute objAttr = new FileAttribute();
            objAttr.IsExecutable = isExec;
            objAttr.IsDirectory = isDir;
            objAttr.IsSymbolicLink = isLink;
            objAttr.IsHidden = isHidden;
            objAttr.IsReadonly = ((permissions & S_IWUSR) == 0);
            objAttr.m_sortOrder = sortOrder;
            objAttr.m_attributeString = attr;
            return objAttr;
        }

        //=========================================================================================
        // 機　能：Linuxの文字列パーミッションからファイル属性を取得する
        // 引　数：[in]strAttr   属性の文字列表現
        // 　　　　[in]attr      ファイル属性（ディレクトリと読み込み専用は設定済み）
        // 　　　　[in]isExec    実行可能のときtrue
        // 　　　　[in]isLink    シンボリックリンクのときtrue
        // 　　　　[in]sortOrder ソート順
        // 戻り値：IFileのファイル属性
        // メ　モ：IsHiddenは別に設定
        //=========================================================================================
        public static FileAttribute FromLinuxStringPermissions(string strPermission, FileAttributes attr, bool isExec, bool isLink, int sortOrder) {
            // オブジェクトに変換
            FileAttribute objAttr = new FileAttribute();
            objAttr.m_attribute = attr;
            objAttr.IsExecutable = isExec;
            objAttr.IsSymbolicLink = isLink;
            objAttr.m_sortOrder = sortOrder;
            objAttr.m_attributeString = strPermission;
            return objAttr;
        }

        //=========================================================================================
        // 機　能：圧縮ファイルの属性からファイル属性を取得する
        // 引　数：[in]isDir   ディレクトリのときtrue
        // 戻り値：IFileのファイル属性
        //=========================================================================================
        public static FileAttribute FromArchive(bool isDir) {
            // 属性を変換
            FileAttributes attrValue;
            string attrString;
            if (isDir) {
                attrValue = FileAttributes.Directory;
                attrString = "---D-";
            } else {
                attrValue = FileAttributes.Normal;
                attrString = "-----";
            }

            // オブジェクトに変換
            FileAttribute objAttr = new FileAttribute();
            objAttr.m_attribute = attrValue;
            objAttr.m_sortOrder = (int)attrValue;
            objAttr.m_attributeString = attrString;
            return objAttr;
        }

        //=========================================================================================
        // 機　能：ファイル属性からWindowsの属性文字列を作成する
        // 引　数：なし
        // 戻り値：Windowsの属性文字列
        //=========================================================================================
        private string CreateWindowsAttributeString() {
            StringBuilder attrString = new StringBuilder(5);
            if ((m_attribute & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                attrString.Append('R');
            } else {
                attrString.Append('-');
            }

            if ((m_attribute & FileAttributes.Hidden) == FileAttributes.Hidden) {
                attrString.Append('H');
            } else {
                attrString.Append('-');
            }

            if ((m_attribute & FileAttributes.System) == FileAttributes.System) {
                attrString.Append('S');
            } else {
                attrString.Append('-');
            }

            if ((m_attribute & FileAttributes.Directory) == FileAttributes.Directory) {
                attrString.Append('D');
            } else {
                attrString.Append('-');
            }

            if ((m_attribute & FileAttributes.Archive) == FileAttributes.Archive) {
                attrString.Append('A');
            } else {
                attrString.Append('-');
            }
            return attrString.ToString();
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
        // プロパティ：ディレクトリ属性のときtrue
        //=========================================================================================
        public bool IsDirectory {
            set {
                if (value) {
                    m_attribute |= FileAttributes.Directory;
                } else {
                    m_attribute &= ~FileAttributes.Directory;
                }
            }
            get {
                return ((m_attribute & FileAttributes.Directory) == FileAttributes.Directory);
            }
        }

        //=========================================================================================
        // プロパティ：アーカイブ属性のときtrue
        //=========================================================================================
        public bool IsArchive {
            set {
                if (value) {
                    m_attribute |= FileAttributes.Archive;
                } else {
                    m_attribute &= ~FileAttributes.Archive;
                }
            }
            get {
                return ((m_attribute & FileAttributes.Archive) == FileAttributes.Archive);
            }
        }

        //=========================================================================================
        // プロパティ：読み込み専用属性のときtrue
        //=========================================================================================
        public bool IsReadonly {
            set {
                if (value) {
                    m_attribute |= FileAttributes.ReadOnly;
                } else {
                    m_attribute &= ~FileAttributes.ReadOnly;
                }
            }
            get {
                return ((m_attribute & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
            }
        }

        //=========================================================================================
        // プロパティ：隠し属性のときtrue
        //=========================================================================================
        public bool IsHidden {
            set {
                if (value) {
                    m_attribute |= FileAttributes.Hidden;
                } else {
                    m_attribute &= ~FileAttributes.Hidden;
                }
            }
            get {
                return ((m_attribute & FileAttributes.Hidden) == FileAttributes.Hidden);
            }
        }

        //=========================================================================================
        // プロパティ：システム属性のときtrue
        //=========================================================================================
        public bool IsSystem {
            set {
                if (value) {
                    m_attribute |= FileAttributes.System;
                } else {
                    m_attribute &= ~FileAttributes.System;
                }
            }
            get {
                return ((m_attribute & FileAttributes.System) == FileAttributes.System);
            }
        }

        //=========================================================================================
        // プロパティ：シンボリックリンク属性のときtrue
        //=========================================================================================
        public bool IsSymbolicLink {
            set {
                if (value) {
                    m_attribute = (FileAttributes)((int)m_attribute | ATTRIBUTE_SYMLINK);
                } else {
                    m_attribute = (FileAttributes)((int)m_attribute & ~ATTRIBUTE_SYMLINK);
                }
            }
            get {
                return (((int)m_attribute & ATTRIBUTE_SYMLINK) == ATTRIBUTE_SYMLINK);
            }
        }

        //=========================================================================================
        // プロパティ：実行可能属性のときtrue
        //=========================================================================================
        public bool IsExecutable {
            set {
                if (value) {
                    m_attribute = (FileAttributes)((int)m_attribute | ATTRIBUTE_EXEC);
                } else {
                    m_attribute = (FileAttributes)((int)m_attribute & ~ATTRIBUTE_EXEC);
                }
            }
            get {
                return (((int)m_attribute & ATTRIBUTE_EXEC) == ATTRIBUTE_EXEC);
            }
        }

        //=========================================================================================
        // プロパティ：リンク切れのときtrue
        //=========================================================================================
        public bool IsLinkError {
            set {
                if (value) {
                    m_attribute = (FileAttributes)((int)m_attribute | ATTRIBUTE_LINKERROR);
                } else {
                    m_attribute = (FileAttributes)((int)m_attribute & ~ATTRIBUTE_LINKERROR);
                }
            }
            get {
                return (((int)m_attribute & ATTRIBUTE_LINKERROR) == ATTRIBUTE_LINKERROR);
            }
        }

        //=========================================================================================
        // プロパティ：ソート順
        //=========================================================================================
        public int SortOrder {
            get {
                return m_sortOrder;
            }
        }

        //=========================================================================================
        // プロパティ：属性の文字列表現
        //=========================================================================================
        public string AttributeString {
            set {
                m_attributeString = value;
            }
            get {
                if (m_attributeString == null) {
                    m_attributeString = CreateWindowsAttributeString();
                }
                return m_attributeString;
            }
        }
    }
}
