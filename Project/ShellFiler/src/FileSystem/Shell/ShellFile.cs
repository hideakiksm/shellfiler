using System;
using System.Collections.Generic;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Virtual;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.Shell {

    //=========================================================================================
    // クラス：ファイル１つ分の情報
    //=========================================================================================
    public class ShellFile : SimpleFile {
        // アクセス日時（不明なときDateTime.MinValue）
        private DateTime m_accessDate = DateTime.MinValue;
        
        // パーミッション（3桁の8進数）
        private int m_permissions = -1;
        
        // オーナー
        private string m_owner;

        // グループ
        private string m_group;

        // シンボリックリンクの参照先（リンクで取得成功時のみ有効、それ以外はnull）
        private string m_linkTarget = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileName      ファイル名
        // 　　　　[in]attribute     解析済みの属性
        // 　　　　[in]fileSize      ファイルサイズ
        // 　　　　[in]modifiedDate  更新日時
        // 　　　　[in]owner         オーナー
        // 　　　　[in]group         グループ
        // 　　　　[in]hardLink      ハードリンクの数
        // 戻り値：なし
        //=========================================================================================
        public ShellFile(string fileName, FileAttribute attribute, int permissions, long fileSize, DateTime modifiedDate, string owner, string group, int hardLink) : base(fileName, modifiedDate, attribute, fileSize) {
            m_permissions = permissions;
            m_owner = owner;
            m_group = group;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（属性コピー専用）
        // 引　数：[in]fileInfo  Windowsのファイル属性
        // 戻り値：なし
        //=========================================================================================
        public ShellFile(WindowsFile fileInfo) : base(fileInfo.FileName, fileInfo.ModifiedDate, fileInfo.Attribute, fileInfo.FileSize) {
            m_accessDate = fileInfo.AccessDate;
            m_permissions = -1;         // 不明
        }

        //=========================================================================================
        // 機　能：コンストラクタ（属性コピー専用）
        // 引　数：[in]fileInfo  SFTPのファイル属性
        // 戻り値：なし
        //=========================================================================================
        public ShellFile(SFTPFile fileInfo) : base(fileInfo.FileName, fileInfo.ModifiedDate, fileInfo.Attribute, fileInfo.FileSize) {
            m_accessDate = fileInfo.ModifiedDate;
            m_permissions = fileInfo.Permissions;
            m_owner = fileInfo.Owner;
            m_group = fileInfo.Group;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（属性コピー専用）
        // 引　数：[in]fileInfo  仮想フォルダのファイル属性
        // 戻り値：なし
        //=========================================================================================
        public ShellFile(VirtualFile fileInfo) : base(fileInfo.FileName, fileInfo.ModifiedDate, fileInfo.Attribute, fileInfo.FileSize) {
            m_accessDate = fileInfo.ModifiedDate;
            m_permissions = -1;         // 不明
        }

        //=========================================================================================
        // 機　能：シンボリックリンクの参照先を設定する
        // 引　数：[in]target       参照先の相対パス
        // 　　　　[in]existTarget  参照先が存在するときtrue
        // 　　　　[in]isDirectory  参照先がディレクトリのときtrue
        // 戻り値：なし
        //=========================================================================================
        public void SetLinkTarget(string target, bool existTarget, bool isDirectory) {
            m_linkTarget = target;
            this.Attribute.IsLinkError = !existTarget;
            this.Attribute.IsDirectory = isDirectory;
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
        // プロパティ：オリジナルの属性（ディレクトリやシンボリックリンクも含む、属性の設定APIでWindowsFileから移行して不明なとき-1）
        //=========================================================================================
        public int Permissions {
            get {
                return m_permissions;
            }
        }

        //=========================================================================================
        // プロパティ：オーナー（ls時のみ有効）
        //=========================================================================================
        public string Owner {
            get {
                return m_owner;
            }
        }

        //=========================================================================================
        // プロパティ：グループ（ls時のみ有効）
        //=========================================================================================
        public string Group {
            get {
                return m_group;
            }
        }

        //=========================================================================================
        // プロパティ：属性オーナー読み込み可能のときtrue
        //=========================================================================================
        public bool AttributeOwnerRead {
            get {
                return ((m_permissions & FileAttribute.S_IRUSR) != 0);
            }
        }

        //=========================================================================================
        // プロパティ：属性オーナー書き込み可能のときtrue
        //=========================================================================================
        public bool AttributeOwnerWrite {
            get {
                return ((m_permissions & FileAttribute.S_IWUSR) != 0);
            }
        }

        //=========================================================================================
        // プロパティ：属性オーナー実行可能のときtrue
        //=========================================================================================
        public bool AttributeOwnerExecute {
            get {
                return ((m_permissions & FileAttribute.S_IXUSR) != 0);
            }
        }

        //=========================================================================================
        // プロパティ：属性グループ読み込み可能のときtrue
        //=========================================================================================
        public bool AttributeGroupRead {
            get {
                return ((m_permissions & FileAttribute.S_IRGRP) != 0);
            }
        }

        //=========================================================================================
        // プロパティ：属性グループ書き込み可能のときtrue
        //=========================================================================================
        public bool AttributeGroupWrite {
            get {
                return ((m_permissions & FileAttribute.S_IWGRP) != 0);
            }
        }

        //=========================================================================================
        // プロパティ：属性グループ実行可能のときtrue
        //=========================================================================================
        public bool AttributeGroupExecute {
            get {
                return ((m_permissions & FileAttribute.S_IXGRP) != 0);
            }
        }

        //=========================================================================================
        // プロパティ：属性他人読み込み可能のときtrue
        //=========================================================================================
        public bool AttributeOtherRead {
            get {
                return ((m_permissions & FileAttribute.S_IROTH) != 0);
            }
        }

        //=========================================================================================
        // プロパティ：属性他人書き込み可能のときtrue
        //=========================================================================================
        public bool AttributeOtherWrite {
            get {
                return ((m_permissions & FileAttribute.S_IWOTH) != 0);
            }
        }

        //=========================================================================================
        // プロパティ：属性他人実行可能のときtrue
        //=========================================================================================
        public bool AttributeOtherExecute {
            get {
                return ((m_permissions & FileAttribute.S_IXOTH) != 0);
            }
        }
    }
}
