using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.FileSystem.Virtual;

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：Windowsでのファイル１つ分の情報
    //=========================================================================================
    public class WindowsFile : SimpleFile {
        // 作成日時
        private DateTime m_creationDate;

        // アクセス日時
        private DateTime m_accessDate;

        // オリジナルの属性
        private FileAttributes m_winAttribute;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]winInfo   Windowsのファイル情報
        // 戻り値：なし
        //=========================================================================================
        public WindowsFile(FileInfo winInfo) :
            base(winInfo.Name, winInfo.LastWriteTime, FileAttribute.FromFileInfo(winInfo.Attributes, false), winInfo.Length) {
            m_creationDate = winInfo.CreationTime;
            m_accessDate = winInfo.LastAccessTime;
            m_winAttribute = winInfo.Attributes;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（親ディレクトリ専用）
        // 引　数：[in]winFile   Windowsのディレクトリ情報
        // 　　　　[in]winInfo   ディレクトリ名（nullのときwinInfoから取得）
        // 戻り値：なし
        //=========================================================================================
        public WindowsFile(DirectoryInfo winInfo, string name) :
            base(name != null ? name : winInfo.Name, winInfo.LastWriteTime, FileAttribute.FromFileInfo(winInfo.Attributes, false), -1) {
            m_creationDate = winInfo.CreationTime;
            m_accessDate = winInfo.LastAccessTime;
            m_winAttribute = winInfo.Attributes;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（属性コピー専用）
        // 引　数：[in]fileInfo   SFTPのファイル情報
        // 戻り値：なし
        //=========================================================================================
        public WindowsFile(SFTPFile fileInfo) : base(fileInfo.FileName, fileInfo.ModifiedDate, ResetSSHAttribute(fileInfo.Attribute), fileInfo.FileSize) {
            m_creationDate = fileInfo.ModifiedDate;
            m_accessDate = fileInfo.AccessDate;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（属性コピー専用）
        // 引　数：[in]fileInfo   SSHシェルのファイル情報
        // 戻り値：なし
        //=========================================================================================
        public WindowsFile(ShellFile fileInfo) : base(fileInfo.FileName, fileInfo.ModifiedDate, ResetSSHAttribute(fileInfo.Attribute), fileInfo.FileSize) {
            m_creationDate = fileInfo.ModifiedDate;
            m_accessDate = fileInfo.AccessDate;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（属性コピー専用）
        // 引　数：[in]fileInfo  仮想フォルダのファイル情報
        // 戻り値：なし
        //=========================================================================================
        public WindowsFile(VirtualFile fileInfo) : base(fileInfo.FileName, fileInfo.ModifiedDate, ResetSSHAttribute(fileInfo.Attribute), fileInfo.FileSize) {
            m_creationDate = fileInfo.ModifiedDate;
            m_accessDate = fileInfo.ModifiedDate;
        }

        //=========================================================================================
        // 機　能：SSHの属性をコピーするとき、不要な属性をクリアする
        // 引　数：[in]sshAttr  SSHの属性
        // 戻り値：IsDirectory以外をクリアした属性
        //=========================================================================================
        private static FileAttribute ResetSSHAttribute(FileAttribute sshAttr) {
            FileAttribute attr = new FileAttribute();
            attr.IsDirectory = sshAttr.IsDirectory;
            return attr;
        }

        //=========================================================================================
        // プロパティ：作成日時
        //=========================================================================================
        public DateTime CreationDate {
            get {
                return m_creationDate;
            }
        }

        //=========================================================================================
        // プロパティ：アクセス日時
        //=========================================================================================
        public DateTime AccessDate {
            get {
                return m_accessDate;
            }
        }

        //=========================================================================================
        // プロパティ：オリジナルの属性
        //=========================================================================================
        public FileAttributes WindowsAttributes {
            get {
                return m_winAttribute;
            }
        }
    }
}
