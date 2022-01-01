using System;
using System.Collections.Generic;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.Shell;
using ShellFiler.FileSystem.Virtual;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：ファイル１つ分の情報
    //=========================================================================================
    public class SFTPFile : SimpleFile {
        // アクセス日時
        private DateTime m_accessDate;

        // アクセス許可（不明なとき-1、10進数での8進数値3桁）
        private int m_permissions;
        
        // オーナー
        private string m_owner;

        // グループ
        private string m_group;

        // シンボリックリンクの参照先（リンクで取得成功時のみ有効、それ以外はnull）
        private string m_linkTarget = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileName  ファイル名
        // 　　　　[in]attr      SFTPの属性
        // 　　　　[in]lsLong    lsのlongNameの結果（属性取得のみの場合はnullでオーナーとグループは不明）
        // 戻り値：なし
        //=========================================================================================
        public SFTPFile(string fileName, SftpATTRS attr, string lsLong) : base(fileName, SSHToDateTime(attr.getMTime()), FileAttribute.FromLinuxPermissions(fileName, attr.getPermissions(), attr.isDir(), attr.isLink()), attr.getSize()) {
            m_permissions = attr.getPermissions();
            m_accessDate = SSHToDateTime(attr.getMTime());
            if (lsLong == null) {
                m_owner = Resources.SSHFileInfoOwnerUnknown;
                m_owner = Resources.SSHFileInfoGroupUnknown;
            } else {
                string[] lsLine = StringUtils.SeparateBySpace(lsLong);
                if (lsLine.Length >= 9) {
                    m_owner = lsLine[2];
                    m_group = lsLine[3];
                } else {
                    m_owner = Resources.SSHFileInfoOwnerUnknown;
                    m_owner = Resources.SSHFileInfoGroupUnknown;
                }
            }
        }

        //=========================================================================================
        // 機　能：コンストラクタ（属性コピー専用）
        // 引　数：[in]winFile   Windowsのディレクトリ情報
        // 　　　　[in]winInfo   ディレクトリ名（nullのときwinInfoから取得）
        // 戻り値：なし
        //=========================================================================================
        public SFTPFile(WindowsFile fileInfo) : base(fileInfo.FileName, fileInfo.ModifiedDate, fileInfo.Attribute, fileInfo.FileSize) {
            m_accessDate = fileInfo.AccessDate;
            m_permissions = -1;         // 不明
        }
        
        //=========================================================================================
        // 機　能：コンストラクタ（属性コピー専用）
        // 引　数：[in]fileInfo  SSHシェルのファイル属性
        // 戻り値：なし
        //=========================================================================================
        public SFTPFile(ShellFile fileInfo) : base(fileInfo.FileName, fileInfo.ModifiedDate, fileInfo.Attribute, fileInfo.FileSize) {
            m_accessDate = fileInfo.ModifiedDate;
            m_permissions = fileInfo.Permissions;
            m_owner = fileInfo.Owner;
            m_group = fileInfo.Group;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（属性コピー専用）
        // 引　数：[in]winFile   Windowsのディレクトリ情報
        // 　　　　[in]winInfo   ディレクトリ名（nullのときwinInfoから取得）
        // 戻り値：なし
        //=========================================================================================
        public SFTPFile(VirtualFile fileInfo) : base(fileInfo.FileName, fileInfo.ModifiedDate, fileInfo.Attribute, fileInfo.FileSize) {
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
            if (isDirectory) {
                this.FileSize = -1;
            }
            this.Attribute.IsLinkError = !existTarget;
            this.Attribute.IsDirectory = isDirectory;
        }

        //=========================================================================================
        // 機　能：SSHの時刻表現をDateTimeに変換する
        // 引　数：[in]time  SSHの時刻表現
        // 戻り値：DateTimeの表現
        //=========================================================================================
        public static DateTime SSHToDateTime(int time) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(time + TimeZoneInfo.Local.BaseUtcOffset.Hours * 60 * 60);
        }

        //=========================================================================================
        // 機　能：DateTimeをSSHの時刻表現に変換する
        // 引　数：[in]time  DateTimeの表現
        // 戻り値：SSHの時刻表現
        //=========================================================================================
        public static int DateTimeToSSH(DateTime time) {
            DateTime startOfEpoch = new DateTime(1970, 1, 1);
            int second = (int)(time - startOfEpoch).TotalSeconds;
            second -= TimeZoneInfo.Local.BaseUtcOffset.Hours * 60 * 60;
            return second;
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
