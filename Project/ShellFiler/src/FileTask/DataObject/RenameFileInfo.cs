using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.UI.Dialog;

namespace ShellFiler.FileTask.DataObject {

    //=========================================================================================
    // クラス：リネーム対象のファイル情報
    //=========================================================================================
    public class RenameFileInfo {
        // 対象がディレクトリのときtrue
        private bool m_isDirectory;

        // 対象ファイルシステムがWindowsのときのリネーム情報
        private WindowsRenameInfo m_windowsInfo;

        // 対象ファイルシステムがSSHのときのリネーム情報
        private SSHRenameInfo m_sshInfo;

        //=========================================================================================
        // 機　能：リネーム対象のファイル情報を作成して返す
        // 引　数：[in]fileSystem  対象のファイルシステム
        // 　　　　[in]file        リネーム対象のファイル
        // 戻り値：リネーム対象のファイル情報
        //=========================================================================================
        public static RenameFileInfo CreateRenameFileInfo(FileSystemID fileSystem, IFile file) {
            RenameFileInfo fileInfo;
            if (FileSystemID.IsWindows(fileSystem)) {
                fileInfo = new RenameFileInfo();
                fileInfo.m_isDirectory = file.Attribute.IsDirectory;
                fileInfo.m_windowsInfo = new WindowsRenameInfo((WindowsFile)file);
                fileInfo.m_sshInfo = null;
            } else if (fileSystem == FileSystemID.SFTP) {
                fileInfo = new RenameFileInfo();
                fileInfo.m_isDirectory = file.Attribute.IsDirectory;
                fileInfo.m_windowsInfo = null;
                fileInfo.m_sshInfo = new SSHRenameInfo((SFTPFile)file);
            } else if (fileSystem == FileSystemID.SSHShell) {
                fileInfo = new RenameFileInfo();
                fileInfo.m_isDirectory = file.Attribute.IsDirectory;
                fileInfo.m_windowsInfo = null;
                fileInfo.m_sshInfo = new SSHRenameInfo((ShellFile)file);
            } else if (FileSystemID.IsVirtual(fileSystem)) {
                fileInfo = null;
            } else {
                FileSystemID.NotSupportError(fileSystem);
                fileInfo = null;
            }
            return fileInfo;
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private RenameFileInfo() {
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]file  対象のファイル情報
        // 戻り値：なし
        //=========================================================================================
        public RenameFileInfo(WindowsRenameInfo info) {
            m_isDirectory = ((info.FileAttributes & FileAttributes.Directory) != 0);
            m_windowsInfo = info;
            m_sshInfo = null;
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]file  対象のファイル情報
        // 戻り値：なし
        //=========================================================================================
        public RenameFileInfo(SSHRenameInfo info) {
            m_isDirectory = info.IsDirectory;
            m_windowsInfo = null;
            m_sshInfo = info;
        }

        //=========================================================================================
        // 機　能：リネームダイアログを作成する
        // 引　数：[in]fileSystemId   編集対象のファイルシステム
        // 戻り値：リネームダイアログ
        //=========================================================================================
        public IRenameFileDialog CreateRenameDialog(FileSystemID fileSystemId) {
            IRenameFileDialog result = null;
            if (m_windowsInfo != null) {
                result = new RenameWindowsDialog(this);
            } else {
                result = new RenameSSHDialog(this, (fileSystemId == FileSystemID.SSHShell));
            }
            return result;
        }

        //=========================================================================================
        // 機　能：内容の変更が発生しているかどうかを確認する
        // 引　数：[in]other  比較対象の変更情報
        // 戻り値：変更が発生しているときtrue
        //=========================================================================================
        public bool IsModified(RenameFileInfo other) {
            if (m_windowsInfo != null) {
                return m_windowsInfo.IsModified(other.m_windowsInfo);
            } else {
                return m_sshInfo.IsModified(other.m_sshInfo);
            }
        }

        //=========================================================================================
        // プロパティ：ファイル名
        //=========================================================================================
        public string FileName {
            get {
                if (m_windowsInfo != null) {
                    return m_windowsInfo.FileName;
                } else {
                    return m_sshInfo.FileName;
                }
            }
        }

        //=========================================================================================
        // プロパティ：対象がディレクトリのときtrue
        //=========================================================================================
        public bool IsDirectory {
            get {
                return m_isDirectory;
            }
        }

        //=========================================================================================
        // プロパティ：Windows用のリネーム情報
        //=========================================================================================
        public WindowsRenameInfo WindowsInfo {
            get {
                return m_windowsInfo;
            }
        }

        //=========================================================================================
        // プロパティ：SSH用のリネーム情報
        //=========================================================================================
        public SSHRenameInfo SSHInfo {
            get {
                return m_sshInfo;
            }
        }

        //=========================================================================================
        // クラス：リネームダイアログを表示するためのインターフェース
        //=========================================================================================
        public interface IRenameFileDialog {

            //=========================================================================================
            // 機　能：編集ダイアログを表示する
            // 引　数：[in]parent  親ウィンドウ
            // 戻り値：ダイアログの結果
            //=========================================================================================
            DialogResult ShowRenameDialog(Form parent);

            //=========================================================================================
            // プロパティ：編集対象のリネーム情報（編集前）
            //=========================================================================================
            RenameFileInfo OriginalRenameFileInfo {
                get;
            }

            //=========================================================================================
            // プロパティ：編集対象のリネーム情報（編集結果）
            //=========================================================================================
            RenameFileInfo ModifiedRenameFileInfo {
                get;
            }
        }

        //=========================================================================================
        // クラス：Window用のリネーム対象のファイル情報
        //=========================================================================================
        public class WindowsRenameInfo {
            // ファイル名
            private string m_fileName;

            // Windowsの属性
            private FileAttributes m_winAttribute;

            // 更新時刻
            private DateTime m_modifiedDate;

            // 作成時刻
            private DateTime m_creationDate;

            // アクセス時刻
            private DateTime m_accessDate;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public WindowsRenameInfo() {
            }

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]winFile  ファイル情報
            // 戻り値：なし
            //=========================================================================================
            public WindowsRenameInfo(WindowsFile winFile) {
                m_fileName = winFile.FileName;
                m_winAttribute = winFile.WindowsAttributes;
                m_modifiedDate = winFile.ModifiedDate;
                m_creationDate = winFile.CreationDate;
                m_accessDate = winFile.AccessDate;
            }

            //=========================================================================================
            // 機　能：内容の変更が発生しているかどうかを確認する
            // 引　数：[in]other  比較対象の変更情報
            // 戻り値：変更が発生しているときtrue
            //=========================================================================================
            public bool IsModified(WindowsRenameInfo other) {
                if (m_fileName != other.m_fileName) {
                    return true;
                }
                if (m_winAttribute != other.m_winAttribute) {
                    return true;
                }
                if (m_modifiedDate != other.m_modifiedDate) {
                    return true;
                }
                if (m_creationDate != other.m_creationDate) {
                    return true;
                }
                if (m_accessDate != other.m_accessDate) {
                    return true;
                }
                return false;
            }

            //=========================================================================================
            // プロパティ：ファイル名
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
            // プロパティ：Windowsの属性
            //=========================================================================================
            public FileAttributes FileAttributes {
                get {
                    return m_winAttribute;
                }
                set {
                    m_winAttribute = value;
                }
            }

            //=========================================================================================
            // プロパティ：更新時刻
            //=========================================================================================
            public DateTime ModifiedDate {
                get {
                    return m_modifiedDate;
                }
                set {
                    m_modifiedDate = value;
                }
            }

            //=========================================================================================
            // プロパティ：作成時刻
            //=========================================================================================
            public DateTime CreationDate {
                get {
                    return m_creationDate;
                }
                set {
                    m_creationDate = value;
                }
            }

            //=========================================================================================
            // プロパティ：アクセス時刻
            //=========================================================================================
            public DateTime AccessDate {
                get {
                    return m_accessDate;
                }
                set {
                    m_accessDate = value;
                }
            }
        }

        //=========================================================================================
        // クラス：SSH用のリネーム対象のファイル情報
        //=========================================================================================
        public class SSHRenameInfo {
            // ファイル名
            private string m_fileName;

            // ディレクトリのときtrue
            private bool m_isDirectory;

            // パーミッション（chmod入力用のみ）
            private int m_permissions;

            // 更新時刻
            private DateTime m_modifiedDate;

            // アクセス時刻
            private DateTime m_accessDate;

            // オーナー
            private string m_owner;

            // グループ
            private string m_group;

            
            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public SSHRenameInfo() {
            }

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]sshFile  ファイル情報
            // 戻り値：なし
            //=========================================================================================
            public SSHRenameInfo(SFTPFile sshFile) {
                m_fileName = sshFile.FileName;
                m_permissions = sshFile.Permissions & FileAttribute.CHMOD_PERMISSION_MASK;
                m_modifiedDate = sshFile.ModifiedDate;
                m_accessDate = sshFile.AccessDate;
                m_owner = sshFile.Owner;
                m_group = sshFile.Group;
            }

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]sshFile  ファイル情報
            // 戻り値：なし
            //=========================================================================================
            public SSHRenameInfo(ShellFile sshFile) {
                m_fileName = sshFile.FileName;
                m_permissions = sshFile.Permissions & FileAttribute.CHMOD_PERMISSION_MASK;
                m_modifiedDate = sshFile.ModifiedDate;
                m_accessDate = sshFile.AccessDate;
                m_owner = sshFile.Owner;
                m_group = sshFile.Group;
            }

            //=========================================================================================
            // 機　能：内容の変更が発生しているかどうかを確認する
            // 引　数：[in]other  比較対象の変更情報
            // 戻り値：変更が発生しているときtrue
            //=========================================================================================
            public bool IsModified(SSHRenameInfo other) {
                if (m_fileName != other.m_fileName) {
                    return true;
                }
                if (m_permissions != other.m_permissions) {
                    return true;
                }
                if (m_modifiedDate != other.m_modifiedDate) {
                    return true;
                }
                if (m_accessDate != other.m_accessDate) {
                    return true;
                }
                if (m_owner != other.m_owner) {
                    return true;
                }
                if (m_group != other.m_group) {
                    return true;
                }
                return false;
            }

            //=========================================================================================
            // プロパティ：ファイル名
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
            // プロパティ：ディレクトリのときtrue
            //=========================================================================================
            public bool IsDirectory {
                get {
                    return m_isDirectory;
                }
                set {
                    m_isDirectory = value;
                }
            }
            
            //=========================================================================================
            // プロパティ：パーミッション（chmod入力用のみ）
            //=========================================================================================
            public int Permissions {
                get {
                    return m_permissions;
                }
                set {
                    m_permissions = value;
                }
            }

            //=========================================================================================
            // プロパティ：更新時刻
            //=========================================================================================
            public DateTime ModifiedDate {
                get {
                    return m_modifiedDate;
                }
                set {
                    m_modifiedDate = value;
                }
            }

            //=========================================================================================
            // プロパティ：アクセス日時
            //=========================================================================================
            public DateTime AccessDate {
                get {
                    return m_accessDate;
                }
                set {
                    m_accessDate = value;
                }
            }

            //=========================================================================================
            // プロパティ：オーナー（ls時のみ有効）
            //=========================================================================================
            public string Owner {
                get {
                    return m_owner;
                }
                set {
                    m_owner = value;
                }
            }

            //=========================================================================================
            // プロパティ：グループ（ls時のみ有効）
            //=========================================================================================
            public string Group {
                get {
                    return m_group;
                }
                set {
                    m_group = value;
                }
            }
        }
    }
}
