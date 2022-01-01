using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;

namespace ShellFiler.Document.SSH {

    //=========================================================================================
    // クラス：SSHファイルシステムのユーティリティ
    //=========================================================================================
    public class SSHUtils {
        // SSHのデフォルトポート番号
        public const int SSH_DEFAULT_PORT = 22;

        // TELNETのデフォルトポート番号
        public const int TELNET_DEFAULT_PORT = 23;

        // suパスのセパレータ
        public const char SU_PATH_SEPARATOR = '/';

        //=========================================================================================
        // 機　能：フルパスディレクトリをサーバ名、ユーザ名、ディレクトリパスに分解する
        // 引　数：[in]fullPath   フルパスディレクトリ名（protocol:user/suPath@server(23):/path/file.txt）
        // 　　　　[out]protocol  プロトコルを返す変数への参照
        // 　　　　[out]user      ユーザー名を返す変数への参照
        // 　　　　[out]server    サーバー名を返す変数への参照
        // 　　　　[out]path      ディレクトリパスを返す変数への参照
        // 戻り値：正しく分解できたときtrue
        //=========================================================================================
        public static bool SeparateUserServer(string fullPath, out SSHProtocolType protocol, out string user, out string server, out int port, out string path) {
            // 構成要素を分解
            protocol = SSHProtocolType.None;
            user = "";
            server = "";
            port = SSH_DEFAULT_PORT;
            path = "";
            string[] protocolUserPath = fullPath.Split(new char[] {':'}, 3);
            if (protocolUserPath.Length != 3) {
                return false;
            }
            string[] userServer = protocolUserPath[1].Split(new char[] {'@'}, 2);
            if (userServer.Length != 2) {
                return false;
            }
            user = userServer[0];
            
            // 内容を取得
            if (protocolUserPath[0] == "sftp") {
                protocol = SSHProtocolType.SFTP;
            } else if (protocolUserPath[0] == "ssh") {
                protocol = SSHProtocolType.SSHShell;
            } else {
                return false;
            }
            path = protocolUserPath[2];

            // サーバを取得
            string serverPart = userServer[1];
            Regex regex2 = new Regex("[a-zA-Z0-9\\.]+\\([0-9]+\\)");
            if (regex2.IsMatch(serverPart)) {
                string[] serverPort = serverPart.Split(new char[] {'('}, 2);
                serverPort[1] = serverPort[1].Substring(0, serverPort[1].Length - 1);
                server = serverPort[0];
                if (!int.TryParse(serverPort[1], out port)) {
                    return false;
                }
                if (port == 0 || port >= 65536) {
                    return false;
                }
            } else {
                server = serverPart;
                port = SSH_DEFAULT_PORT;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：指定されたユーザーがスーパーユーザーかどうかを調べる
        // 引　数：[in]userServer    現在のユーザー（ユーザー名@サーバー名）
        // 　　　　[in]rootUserName  ルートのユーザー名
        // 戻り値：スーパーユーザーのときtrue
        //=========================================================================================
        public static bool IsSuperUser(string userServer, string rootUserName) {
            string[] userAndServer = userServer.Split('@');
            if (rootUserName == userAndServer[0]) {
                return true;
            } else {
                return false;
            }
        }
       
        //=========================================================================================
        // 機　能：このファイルシステムのパス同士で、同じサーバ空間のパスかどうかを調べる
        // 引　数：[in]path1  パス１
        // 　　　　[in]path2  パス２
        // 戻り値：パス１とパス２が同じサーバ空間にあるときtrue
        //=========================================================================================
        public static bool IsSameServerSpace(string path1, string path2) {
            bool success;
            SSHProtocolType protocol1;
            string user1, server1, local1;
            int portNo1;
            success = SSHUtils.SeparateUserServer(path1, out protocol1, out user1, out server1, out portNo1, out local1);
            if (!success) {
                return false;
            }
            SSHProtocolType protocol2;
            string user2, server2, local2;
            int portNo2;
            success = SSHUtils.SeparateUserServer(path2, out protocol2, out user2, out server2, out portNo2, out local2);
            if (!success) {
                return false;
            }
            if (protocol1 != protocol2 || user1 != user2 || server1 != server2 || portNo1 != portNo2) {
                return false;
            } else {
                return true;
            }
        }

        //=========================================================================================
        // 機　能：パスとファイルを連結する
        // 引　数：[in]dir  ディレクトリ名
        // 　　　　[in]file ファイル名
        // 戻り値：連結したファイル名
        //=========================================================================================
        public static string CombineFilePath(string dir, string file) {
            if (file.StartsWith("~/")) {
                // ホームディレクトリ以下の絶対パス表現
                SSHProtocolType protocol;
                string user, server, local;
                int portNo;
                bool success = SSHUtils.SeparateUserServer(dir, out protocol, out user, out server, out portNo, out local);
                if (success) {
                    return SSHUtils.CreateUserServer(protocol, user, server, portNo) + ":" + file;
                }
            }
            // 通常の連結
            if (dir.EndsWith("/")) {
                return dir + file;
            } else {
                return dir + "/" + file;
            }
        }
        
        //=========================================================================================
        // 機　能：ディレクトリ名の最後を'\'または'/'にする
        // 引　数：[in]dir  ディレクトリ名
        // 戻り値：'\'または'/'を補完したディレクトリ名
        //=========================================================================================
        public static string CompleteDirectoryName(string dir) {
            if (dir.EndsWith("/")) {
                return dir;
            } else {
                return dir + "/";
            }
        }

        //=========================================================================================
        // 機　能：指定されたパス名をルートとそれ以外に分割する
        // 引　数：[in]path   パス名
        // 　　　　[out]root  ルート部分を返す文字列（最後はセパレータ）
        // 　　　　[out]sub   サブディレクトリ部分を返す文字列
        // 戻り値：なし
        //=========================================================================================
        public static void SplitRootPath(string path, out string root, out string sub) {
            SSHProtocolType protocol;
            string user, server, local;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(path, out protocol, out user, out server, out portNo, out local);
            if (!success) {
                Program.Abort("パス名の解析エラーです。{0}", local);
            }
            root = SSHUtils.CreateUserServer(protocol, user, server, portNo) + ":/";
            sub = local.Substring(1);
        }

        //=========================================================================================
        // 機　能：指定されたパス名のホームディレクトリを取得する
        // 引　数：[in]path  パス名
        // 戻り値：ホームディレクトリ（取得できないときnull）
        //=========================================================================================
        public static string GetHomePath(string path) {
            SSHProtocolType protocol;
            string user, server, local;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(path, out protocol, out user, out server, out portNo, out local);
            if (!success) {
                return null;
            }
            string home = SSHUtils.CreateUserServer(protocol, user, server, portNo) + ":~/";
            return home;
        }

        //=========================================================================================
        // 機　能：ファイルパスからファイル名を返す
        // 引　数：[in]filePath  ファイルパス
        // 戻り値：ファイルパス中のファイル名
        //=========================================================================================
        public static string GetFileName(string filePath) {
            SSHProtocolType protocol;
            string user, server, local;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out local);
            if (!success) {
                return "";
            }
            return GenericFileStringUtils.GetFileName(local, '/');
        }

        //=========================================================================================
        // 機　能：指定されたパスからディレクトリ名部分を返す
        // 引　数：[in]fullPath  パス名
        // 戻り値：パス名のディレクトリ部分
        //=========================================================================================
        public static string GetDirectoryName(string fullPath) {
            SSHProtocolType protocol;
            string user, server, local;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(fullPath, out protocol, out user, out server, out portNo, out local);
            if (!success) {
                return "";
            }
            return GenericFileStringUtils.GetDirectoryName(local, '/');
        }

        //=========================================================================================
        // 機　能：サーバー名、ユーザー名からユーザー名@サーバー名の文字列を作成する
        // 引　数：[in]user    ユーザー名を返す変数への参照
        // 　　　　[in]server  サーバー名を返す変数への参照
        // 　　　　[in]port    ポート番号
        // 戻り値：ユーザー名@サーバー名（user@server）
        //=========================================================================================
        public static string CreateUserServerShort(string user, string server, int port) {
            if (port == 22) {
                return user + "@" + server;
            } else {
                return user + "@" + server + "(" + port + ")";
            }
        }

        //=========================================================================================
        // 機　能：プロトコル、サーバー名、ユーザー名からユーザー名@サーバー名の文字列を作成する
        // 引　数：[in]protocol プロトコル
        // 　　　　[in]user     ユーザー名
        // 　　　　[in]server   サーバー名
        // 　　　　[in]port     ポート番号
        // 戻り値：ユーザー名@サーバー名（protocol:user/suPath@server）
        //=========================================================================================
        public static string CreateUserServer(SSHProtocolType protocol, string user, string server, int port) {
            if (port == 22) {
                return protocol.FolderProtocol + ":" + user + "@" + server;
            } else {
                return protocol.FolderProtocol + ":" + user + "@" + server + "(" + port + ")";
            }
        }

        //=========================================================================================
        // 機　能：SSHの絶対パス表現かどうかを調べる
        // 引　数：[in]allowProtocol  許可されるプロトコル（Allも可能）
        // 　　　　[in]directory      ディレクトリ名
        // 戻り値：絶対パスのときtrue(trueでも実際にファイルアクセスできるかどうかは不明)
        //=========================================================================================
        public static bool IsAbsolutePath(SSHProtocolType allowProtocol, string directory) {
            // user@server:\形式
            string protocolRegex = null;
            if (allowProtocol == SSHProtocolType.SFTP) {
                protocolRegex = "sftp";
            } else if (allowProtocol == SSHProtocolType.SSHShell) {
                protocolRegex = "ssh";
            } else if (allowProtocol == SSHProtocolType.All) {
                protocolRegex = "(sftp|ssh)";
            } else {
                Program.Abort("不明なプロトコルです。");
            }
            Regex regex1 = new Regex(@"^" + protocolRegex + @":[a-zA-Z0-9_\.\-]+\@[a-zA-Z0-9\.\-]+\:");
            Regex regex2 = new Regex(@"^" + protocolRegex + @":[a-zA-Z0-9_\.\-]+\@[a-zA-Z0-9\.\-]+\([0-9]+\):");
            if (!regex1.IsMatch(directory) && !regex2.IsMatch(directory)) {
                return false;
            }

            // フルパスディレクトリを分解
            SSHProtocolType protocol;
            string user, server, local;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(directory, out protocol, out user, out server, out portNo, out local);
            return success;
        }

        //=========================================================================================
        // 機　能：フルパスの文字列を返す
        // 引　数：[in]orgPath    未整理のフルパス（「..」などを含む）
        // 戻り値：フルパスディレクトリ名
        //=========================================================================================
        public static string GetFullPath(string orgPath) {
            SSHProtocolType protocol;
            string user, server, local;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(orgPath, out protocol, out user, out server, out portNo, out local);
            if (!success) {
                return orgPath;
            }
            string[] dirList = local.Split('/');
            if (dirList.Length == 1) {
                return orgPath;
            }
            bool isHome = (dirList[0] == "~");

            // ディレクトリを「/」で区切ったときに「.」「..」を削除
            string[] targetDir = new string[dirList.Length];
            int targetDirCount = 1;
            targetDir[0] = dirList[0];
            for (int i = 1; i < dirList.Length; i++) {
                if (dirList[i] == "." || dirList[i] == "") {
                    ;
                } else if (dirList[i] == "..") {
                    if (targetDirCount >= 2) {          // /dir1/..のような場合、/
                        targetDirCount--;
                    } else if (isHome) {                // ~/..のような場合、~/..
                        targetDir[targetDirCount] = dirList[i];
                        targetDirCount++;
                    }
                } else {
                    targetDir[targetDirCount] = dirList[i];
                    targetDirCount++;
                }
            }

            // 1つにつなげる
            StringBuilder resultPath = new StringBuilder();
            for (int i = 0; i < targetDirCount; i++) {
                resultPath.Append(targetDir[i]);
                if (targetDirCount == 1 || i != targetDirCount - 1)  {
                    resultPath.Append('/');
                }
            }
            string result = CreateUserServer(protocol, user, server, portNo) + ":" + resultPath.ToString();
            return result;
        }
        
        //=========================================================================================
        // 機　能：SFTPでディレクトリ名を補完する
        // 引　数：[in]connection   接続
        // 　　　　[in]directory    フルパスのディレクトリ
        // 戻り値：補完済みのフルパスディレクトリ
        //=========================================================================================
        public static string CompleteSFTPDirectory(SSHConnection connection, string directory) {
            // フルパスディレクトリを分解
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(directory, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // ホームディレクトリの場合は補完
            if (directory.EndsWith("/")) {
                path = SSHUtils.CompleteHomeDir(path, connection.SFTPHomeDirectory);
                path = SSHUtils.GetFullPath(path);
                path = GenericFileStringUtils.CompleteDirectoryName(path, "/");
            } else {
                path = SSHUtils.CompleteHomeDir(path, connection.SFTPHomeDirectory);
                path = SSHUtils.GetFullPath(path);
            }

            // ShellFiler形式に戻す
            directory = SSHUtils.CreateUserServer(protocol, user, server, portNo) + ":" + path;
            return directory;
        }
                
        //=========================================================================================
        // 機　能：SFTPでディレクトリ名を補完する
        // 引　数：[in]connection   接続
        // 　　　　[in]directory    フルパスのディレクトリ
        // 戻り値：補完済みのフルパスディレクトリ
        //=========================================================================================
        public static string CompleteShellDirectory(TerminalShellChannel channel, string directory) {
            // フルパスディレクトリを分解
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(directory, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // ホームディレクトリの場合は補完
            if (directory.EndsWith("/")) {
                path = SSHUtils.CompleteHomeDir(path, channel.HomeDirectory);
                path = SSHUtils.GetFullPath(path);
                path = GenericFileStringUtils.CompleteDirectoryName(path, "/");
            } else {
                path = SSHUtils.CompleteHomeDir(path, channel.HomeDirectory);
                path = SSHUtils.GetFullPath(path);
            }

            // ShellFiler形式に戻す
            directory = SSHUtils.CreateUserServer(protocol, user, server, portNo) + ":" + path;
            return directory;
        }

        //=========================================================================================
        // 機　能：ホームディレクトリを補完する
        // 引　数：[in]path    ディレクトリパス（~/path/file.txt）
        // 　　　　[in]homeDir ホームディレクトリ名（/home/taro/）
        // 戻り値：フルパスディレクトリ名（/home/taro/path/file.txt）
        //=========================================================================================
        private static string CompleteHomeDir(string path, string homeDir) {
            if (path == "~") {
                return homeDir;
            } else if (path.StartsWith("~/")) {
                if (!homeDir.EndsWith("/")) {
                    homeDir = homeDir + "/";
                }
                string retDir = homeDir + path.Substring(2);
                return retDir;
            } else {
                return path;
            }
        }

        //=========================================================================================
        // 機　能：パーミッションの８進数表現を10進数表現に変換する
        // 引　数：[in]permissions  ８進数表現
        // 戻り値：10進数表現
        //=========================================================================================
        public static int PermissionOctToDec(int permissions) {
            return ((permissions >> 6) & 0x7) * 100 + ((permissions >> 3) & 0x7) * 10 + (permissions & 0x7);
        }

        //=========================================================================================
        // 機　能：パーミッションの10進数表現を８進数表現に変換する
        // 引　数：[in]permissions  10進数表現
        // 戻り値：８進数表現
        //=========================================================================================
        public static int PermissionDecToOct(int permissions) {
            return (((permissions / 100) % 10) << 6) | (((permissions / 10) % 10) << 3) | (permissions % 10);
        }

        //=========================================================================================
        // 機　能：同じSSHセッションで処理できるパスかどうかを返す
        // 引　数：[in]srcPath   転送元パス
        // 　　　　[in]destPath  転送先パス
        // 戻り値：同じセッションで処理できるときtrue
        //=========================================================================================
        public static bool IsSameSSHSession(string srcPath, string destPath) {
            bool success;

            // 転送元を分解
            SSHProtocolType srcProtocol;
            string srcUser, srcServer, srcDir;
            int srcPortNo;
            success = SSHUtils.SeparateUserServer(srcPath, out srcProtocol, out srcUser, out srcServer, out srcPortNo, out srcDir);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // 転送先を分解
            SSHProtocolType destProtocol;
            string destUser, destServer, destDir;
            int destPortNo;
            success = SSHUtils.SeparateUserServer(destPath, out destProtocol, out destUser, out destServer, out destPortNo, out destDir);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // 同一ならOK
            if (srcProtocol == destProtocol && srcUser == destUser && srcServer == destServer && srcPortNo == destPortNo) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：パス表現からユーザ名とサーバ部分を返す（"sftp:user@server(20):~/dir" → sftp, "user@server(20)"）
        // 引　数：[in]path         元のパス
        // 　　　　[out]protocol    プロトコルを返す変数
        // 　　　　[out]userServer  ユーザー名@サーバー名の部分を返す変数
        // 戻り値：パス表現のユーザ名とサーバ部分
        //=========================================================================================
        public static void GetUserServerPart(string path, out SSHProtocolType protocol, out string userServer) {
            protocol = SSHProtocolType.None;
            userServer = null;
            string[] list = path.Split(new char[] {':'}, 3);
            if (list.Length != 3) {
                Program.Abort("パス名の管理（':'の数）が想定外です。");
                return;
            }
            if (list[0] == "sftp") {
                protocol = SSHProtocolType.SFTP;
            } else if (list[0] == "ssh") {
                protocol = SSHProtocolType.SSHShell;
            } else {
                Program.Abort("パス名の管理（プロトコル）が想定外です。");
                return;
            }
            userServer = list[1];
        }

        //=========================================================================================
        // 機　能：接続先の認証情報を入力して登録する
        // 引　数：[in]user   ユーザー名
        // 　　　　[in]server サーバー名
        // 　　　　[in]path   パス名
        // 　　　　[in]port   ポート番号
        // 戻り値：入力した認証情報
        //=========================================================================================
        public static SSHUserAuthenticateSettingItem InputUserAuthenticateInfo(string user, string server, string path, int port) {
            // 入力したディレクトリから認証情報を初期化
            SSHUserAuthenticateSetting authDatabase = Program.Document.SSHUserAuthenticateSetting;
            SSHUserAuthenticateSettingItem authSetting = new SSHUserAuthenticateSettingItem();
            authSetting.DisplayName = SSHUtils.CreateUserServerShort(user, server, port);
            authSetting.ServerName = server;
            authSetting.UserName = user;
            authSetting.Password = null;
            authSetting.Encoding = Encoding.UTF8;

            // 設定の残りを初期化
            SSHConnectionDialog dialog = new SSHConnectionDialog(authSetting, SSHConnectionDialog.ConnectMode.UserSererFixed);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return null;
            }

            // 保存指定のときはディスク保存
            authSetting = dialog.AuthenticateSetting;
            if (dialog.AutoRetry) {
                Program.Document.SSHUserAuthenticateSetting.AddSetting(authSetting);
                authDatabase.SaveData();
            } else {
                Program.Document.SSHUserAuthenticateSetting.AddTemporarySetting(authSetting);
            }
            return authSetting;
        }

        //=========================================================================================
        // 機　能：ファイルの存在を確認する
        // 引　数：[in]fileSystem  ファイルシステム
        // 　　　　[in]context     コンテキスト情報
        // 　　　　[in]filePath    ファイルパス
        // 　　　　[in]isTarget   対象パスの一覧のときtrue、反対パスのときfalse
        // 　　　　[in]isFile      ファイルの存在を調べるときtrue、フォルダはfalse、両方はnull
        // 　　　　[out]isExist    ファイルが存在するときtrueを返す領域への参照
        // 戻り値：ステータス（成功のときSuccess、存在しないときはSuccessでisExist=false）
        //=========================================================================================
        public static FileOperationStatus CheckFileExistImpl(IFileSystem fileSystem, FileOperationRequestContext context, string filePath, bool isTarget, BooleanFlag isFile, out bool isExist) {
            isExist = false;
            IFile fileInfo;
            FileOperationStatus status = fileSystem.GetFileInfo(context, filePath, isTarget, out fileInfo);
            if (status == FileOperationStatus.FileNotFound) {
                isExist = false;
                return FileOperationStatus.Success;
            }
            if (status != FileOperationStatus.Success) {
                return status;
            }
            if (fileInfo == null) {
                isExist = false;
            } else {
                if (isFile == null) {
                    isExist = true;
                } else if (isFile.Value) {
                    isExist = (fileInfo.Attribute.IsDirectory == false);
                } else {
                    isExist = (fileInfo.Attribute.IsDirectory == true);
                }
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ファイル一覧のコンテキストからシェルコマンドの取得クラスを取得する
        // 引　数：[in]fileListContext  ファイル一覧のコンテキスト情報
        // 戻り値：シェルコマンドの取得クラス
        //=========================================================================================
        public static ShellCommandDictionary GetShellCommandDictionary(IFileListContext fileListContext) {
            ShellCommandDictionary commandDic = null;
            if (fileListContext == null) {
                Program.Abort("FileListContextが設定されていません。");
            } else if (fileListContext is SFTPFileListContext) {
                commandDic = ((SFTPFileListContext)(fileListContext)).ShellCommandDictionary;
            } else if (fileListContext is ShellFileListContext) {
                commandDic = ((ShellFileListContext)(fileListContext)).ShellCommandDictionary;
            } else if (fileListContext == null) {
                Program.Abort("FileListContextが未知の型です:{0}", fileListContext.GetType().Name);
            }
            return commandDic;
        }

        //=========================================================================================
        // 機　能：ファイル一覧のコンテキストからエンコード種別を取得する
        // 引　数：[in]fileListContext  ファイル一覧のコンテキスト情報
        // 戻り値：エンコード種別
        //=========================================================================================
        public static Encoding GetEncoding(IFileListContext fileListContext) {
            Encoding encoding = null;
            if (fileListContext == null) {
                Program.Abort("FileListContextが設定されていません。");
            } else if (fileListContext is SFTPFileListContext) {
                encoding = ((SFTPFileListContext)(fileListContext)).Encoding;
            } else if (fileListContext is ShellFileListContext) {
                encoding = ((ShellFileListContext)(fileListContext)).Encoding;
            } else if (fileListContext == null) {
                Program.Abort("FileListContextが未知の型です:{0}", fileListContext.GetType().Name);
            }
            return encoding;
        }
    }
}