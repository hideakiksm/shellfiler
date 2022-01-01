using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Properties;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：ファイルシステムのID
    //=========================================================================================
    public class FileSystemID {
        public static readonly FileSystemID Windows      = new FileSystemID(0, "Windows",      Resources.FileSystem_Windows);      // Windowsファイルシステム
        public static readonly FileSystemID WindowsAdmin = new FileSystemID(1, "WindowsAdmin", Resources.FileSystem_Windows);      // Windowsファイルシステム（管理者モード）
        public static readonly FileSystemID SFTP         = new FileSystemID(2, "SSH",          Resources.FileSystem_SSH);          // SSH(SFTP)
        public static readonly FileSystemID SSHShell     = new FileSystemID(3, "SSHShell",     Resources.FileSystem_SSHShell);     // SSH(シェル)
        public static readonly FileSystemID Virtual      = new FileSystemID(4, "Virtual",      Resources.FileSystem_Virtual);      // 仮想ディレクトリ
        public static readonly FileSystemID None         = new FileSystemID(5, "None",         "");                                // なし

        // 数値表現（ソート用）
        private int m_intId;

        // 文字列表現
        private string m_stringId;

        // 表示用文字列
        private string m_displayName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]intId        数値表現（ソート用）
        // 　　　　[in]stringId     文字列表現
        // 　　　　[in]displayName  表示用文字列
        // 戻り値：なし
        //=========================================================================================
        public FileSystemID(int intId, string stringId, string displayName) {
            m_intId = intId;
            m_stringId = stringId;
            m_displayName = displayName;
        }
        
        //=========================================================================================
        // 機　能：比較する
        // 引　数：[in]other  比較対象
        // 戻り値：等しいときtrue
        //=========================================================================================
        public override bool Equals(object other) {
            return this.m_stringId == ((FileSystemID)other).m_stringId;
        }

        //=========================================================================================
        // 機　能：ハッシュコードを計算する
        // 引　数：なし
        // 戻り値：ハッシュコード
        //=========================================================================================
        public override int GetHashCode() {
            return m_stringId.GetHashCode();
        }

        //=========================================================================================
        // 機　能：Windowsファイルシステムのときtrueを返す
        // 引　数：[in]id   ファイルシステムのID
        // 戻り値：Windowsファイルシステムのときtrue
        //=========================================================================================
        public static bool IsWindows(FileSystemID id) {
            if (id == Windows || id == WindowsAdmin) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：SSHファイルシステムのときtrueを返す
        // 引　数：[in]id   ファイルシステムのID
        // 戻り値：SSHファイルシステムのときtrue
        //=========================================================================================
        public static bool IsSSH(FileSystemID id) {
            if (id == SFTP || id == SSHShell) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：仮想ディレクトリファイルシステムのときtrueを返す
        // 引　数：[in]id   ファイルシステムのID
        // 戻り値：仮想ディレクトリファイルシステムのときtrue
        //=========================================================================================
        public static bool IsVirtual(FileSystemID id) {
            if (id == Virtual) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：ファイルシステムサポート漏れのエラーを表示する
        // 引　数：[in]id   ファイルシステムのID
        // 戻り値：なし
        //=========================================================================================
        public static void NotSupportError(FileSystemID id) {
            Program.Abort("ファイルシステム{0}の実装が不足しています。", id);
        }

        //=========================================================================================
        // 機　能：ファイルパスの大文字小文字を無視すべきかどうかを返す
        // 引　数：[in]id   ファイルシステムのID
        // 戻り値：大文字小文字を無視するときtrue
        //=========================================================================================
        public static bool IgnoreCaseFolderPath(FileSystemID id) {
            if (id == Windows || id == WindowsAdmin) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：文字列表現を対応するIDに変換する
        // 引　数：[in]strId       文字列表現
        // 　　　　[in]enableNone  Noneが有効なときtrue
        // 戻り値：ID（該当するものがないときnull）
        //=========================================================================================
        public static FileSystemID FromString(string strId, bool enableNone) {
            if (strId == Windows.StringId) {
                return Windows;
            } else if (strId == WindowsAdmin.StringId) {
                return WindowsAdmin;
            } else if (strId == SFTP.StringId) {
                return SFTP;
            } else if (strId == SSHShell.StringId) {
                return SSHShell;
            } else if (strId == Virtual.StringId) {
                return Virtual;
            } else if (strId == None.StringId && enableNone) {
                return None;
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：指定されたデータ型を扱うときにファイルパスの大文字小文字を無視すべきかどうかを返す
        // 引　数：[in]type  データ型
        // 戻り値：大文字小文字を無視するときtrue
        //=========================================================================================
        public static bool IgnoreCaseFolderPathFromIFileType(Type type) {
            if (type == typeof(WindowsFile)) {
                return true;
            } else if (type == typeof(SFTPFile)) {
                return false;
            } else if (type == typeof(ShellFile)) {
                return false;
            } else if  (type == typeof(VirtualFile)) {
                return true;
            } else {
                Program.Abort("想定外の型が指定されました。\n{0}", type.FullName);
                return false;
            }
        }

        //=========================================================================================
        // プロパティ：数値表現（ソート用）
        //=========================================================================================
        public int IntId {
            get {
                return m_intId;
            }
        }

        //=========================================================================================
        // プロパティ：文字列表現
        //=========================================================================================
        public string StringId {
            get {
                return m_stringId;
            }
        }

        //=========================================================================================
        // プロパティ：表示用文字列
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
        }
    }
}
