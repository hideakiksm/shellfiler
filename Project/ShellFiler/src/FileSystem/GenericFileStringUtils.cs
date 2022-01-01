using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Util;

namespace ShellFiler.FileSystem {

    //=========================================================================================
    // クラス：ファイル文字列に対する一般的なユーティリティ
    //=========================================================================================
    public class GenericFileStringUtils {
        // 「C:\」「D:\」にマッチする正規表現
        const string REG_FULLPATH_WITH_DRIVE = @"[a-zA-Z]:\\";

        // 「\\net\folder」にマッチする正規表現
        const string REG_FULLPATH_WITH_NETWORK = @"\\\\[a-zA-Z0-9\-\.]+\\[^\\]+";

        //=========================================================================================
        // 機　能：ファイルパスからファイル名を返す
        // 引　数：[in]filePath  ファイルパス
        // 戻り値：ファイルパス中のファイル名
        //=========================================================================================
        public static string GetFileName(string filePath) {
            int idxSlash = filePath.LastIndexOf('/');
            int idxEn = filePath.LastIndexOf('\\');
            int idx = Math.Max(idxSlash, idxEn);
            if (idx == -1) {
                return filePath;
            } else {
                return filePath.Substring(idx + 1);
            }
        }

        //=========================================================================================
        // 機　能：ファイル名から拡張子を取り除いた部分を返す
        // 引　数：[in]fileName  ファイル名
        // 戻り値：拡張子を除いたファイル名
        //=========================================================================================
        public static string GetFileNameBody(string fileName) {
            int indexExt = fileName.LastIndexOf('.');
            if (indexExt == -1) {
                return fileName;
            } else {
                int idxSlash = fileName.LastIndexOf('/');
                int idxEn = fileName.LastIndexOf('\\');
                int idxFileName = Math.Max(idxSlash, idxEn);
                if (idxFileName == -1) {
                    return fileName.Substring(0, indexExt);
                } else if (idxFileName > indexExt) {
                    return fileName;
                } else {
                    return fileName.Substring(0, indexExt);
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイル名から拡張子の位置を返す
        // 引　数：[in]fileName  ファイル名（フルパスは非対応）
        // 戻り値：拡張子を除いたファイル名
        //=========================================================================================
        public static int GetExtPosition(string fileName) {
            int indexExt = fileName.LastIndexOf('.');
            if (indexExt == -1) {
                return -1;
            } else {
                int idxSlash = fileName.LastIndexOf('/');
                int idxEn = fileName.LastIndexOf('\\');
                int idxFileName = Math.Max(idxSlash, idxEn);
                if (idxFileName == -1) {
                    return indexExt;
                } else if (idxFileName > indexExt) {
                    return -1;
                } else {
                    return indexExt;
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイルパスからファイル名を返す
        // 引　数：[in]filePath  ファイルパス
        // 　　　　[in]separator セパレータの文字
        // 戻り値：ファイルパス中のファイル名
        //=========================================================================================
        public static string GetFileName(string filePath, char separator) {
            int idxSlash = filePath.LastIndexOf(separator);
            if (idxSlash == -1) {
                return filePath;
            } else {
                return filePath.Substring(idxSlash + 1);
            }
        }

        //=========================================================================================
        // 機　能：指定されたパス名のWindows絶対パス表現を取得する
        // 引　数：[in]path  パス名
        // 戻り値：絶対パス
        //=========================================================================================
        public static string CleanupWindowsFullPath(string path) {
            int start = 0;          // pathのパス開始位置
            int minCount = 0;       // path[start]以下に必要な領域数
            bool isLastEn = path.EndsWith("\\");
            Regex regexDrive = new Regex("[a-zA-Z]:\\\\");
            Regex regexShare = new Regex("\\\\\\\\[a-zA-Z0-9\\-\\.]+\\\\");
            Match matchDrive = regexDrive.Match(path);
            Match matchShare = regexShare.Match(path);
            if (matchDrive.Success) {
                path = path.Substring(0, 1).ToUpper() + path.Substring(1);
                start = matchDrive.Length;
                minCount = 0;                   // ルート直下は空になる
            } else if (matchShare.Success) {
                start = matchShare.Length;
                minCount = 1;                   // 共有名が最低1個必要
            }
            string splitPath = path.Substring(start);
            string[] dirList = splitPath.Split('\\');
            if (dirList.Length == 1) {
                if (dirList[0] == "." || dirList[0] == "..") {
                    return path.Substring(0, start);
                } else {
                    return path;
                }
            }

            // ディレクトリを「\」で区切ったときに「.」「..」を削除
            string[] targetDir = new string[dirList.Length];
            int targetDirCount = 0;
            for (int i=0; i < minCount; i++) {
                targetDir[targetDirCount++] = dirList[i];
            }
            for (int i = minCount; i < dirList.Length; i++) {
                if (dirList[i] == "." || dirList[i] == "") {
                    ;
                } else if (dirList[i] == "..") {
                    if (targetDirCount > minCount) {
                        targetDirCount--;
                    }
                } else {
                    targetDir[targetDirCount] = dirList[i];
                    targetDirCount++;
                }
            }

            // 1つにつなげる
            StringBuilder resultPath = new StringBuilder();
            if (start != 0) {
                resultPath.Append(path.Substring(0, start));
            }
            for (int i = 0; i < targetDirCount; i++) {
                resultPath.Append(targetDir[i]);
                if (i != targetDirCount - 1 || isLastEn) {
                    resultPath.Append('\\');
                }
            }
            return resultPath.ToString();
        }

        //=========================================================================================
        // 機　能：指定されたパスからディレクトリ名部分を返す
        // 引　数：[in]path     パス名
        // 戻り値：パス名のディレクトリ部分（最後は「\」または「/」）
        //=========================================================================================
        public static string GetDirectoryName(string filePath) {
            int idxSlash = filePath.LastIndexOf('/');
            int idxEn = filePath.LastIndexOf('\\');
            int idx = Math.Max(idxSlash, idxEn);
            if (idx == -1) {
                return filePath;
            } else {
                return filePath.Substring(0, idx + 1);
            }
        }

        //=========================================================================================
        // 機　能：指定されたパスからディレクトリ名部分を返す
        // 引　数：[in]path      パス名
        // 　　　　[in]separator セパレータの文字
        // 戻り値：パス名のディレクトリ部分
        //=========================================================================================
        public static string GetDirectoryName(string path, char separator) {
            int idxSlash = path.LastIndexOf(separator);
            if (idxSlash == -1) {
                return path;
            } else {
                return path.Substring(0, idxSlash + 1);
            }
        }

        //=========================================================================================
        // 機　能：指定されたファイル名またはパス名から最後の拡張子部分を返す（a.tar.gz→gz）
        // 引　数：[in]file  ファイル名（パス名も可能）
        // 戻り値：拡張子（最後の「.」の次の文字から）、ない場合は空文字列
        //=========================================================================================
        public static string GetExtensionLast(string file) {
            int indexExt = file.LastIndexOf('.');
            if (indexExt > 0) {                // 0はUnix系の隠しファイル
                return file.Substring(indexExt + 1);
            } else {
                return "";
            }
        }

        //=========================================================================================
        // 機　能：指定されたファイル名またはパス名から拡張子部分のすべてを返す（a.tar.gz→.tar.gz）
        // 引　数：[in]path  ファイル名（パス名も可能）
        // 戻り値：拡張子（最初の「.」の文字から）、ない場合は空文字列
        //=========================================================================================
        public static string GetExtensionAll(string path) {
            string dispFile = GenericFileStringUtils.GetFileName(path);
            int idxExt = dispFile.IndexOf('.');
            if (idxExt != -1) {
                return dispFile.Substring(idxExt);
            } else {
                return "";
            }
        }

        //=========================================================================================
        // 機　能：指定されたパス名からドライブ名を返す
        // 引　数：[in]path  パス名
        // 戻り値：ドライブ名（ドライブ指定ではない場合、空文字列）
        //=========================================================================================
        public static string GetDriveName(string path) {
            if (path.Length < 2) {
                return "";
            }
            char ch0 = path[0];
            char ch1 = path[1];
            if (ch1 == ':' && (('A' <= ch1 && ch1 <= 'Z') || ('a' <= ch1 && ch1 <= 'z'))) {
                return path.Substring(0, 1);
            } else {
                return "";
            }
        }

        //=========================================================================================
        // 機　能：指定されたパス名の最後が区切りであればトリムする
        // 引　数：[in]path  パス名
        // 戻り値：区切りをトリムしたパス名
        //=========================================================================================
        public static string TrimLastSeparator(string path) {
            return path.TrimEnd('\\', '/');
        }

        //=========================================================================================
        // 機　能：コマンドラインをプログラムと引数に分離する
        // 引　数：[in]command   コマンドライン
        // 　　　　[out]program  プログラム名を返す変数への参照
        // 　　　　[out]argument 引数を返す変数への参照
        // 戻り値：なし
        //=========================================================================================
        public static void SplitCommandLine(string command, out string program, out string argument) {
            int index = 0;
            bool inQuote = false;
            while (index < command.Length) {
                char ch = command[index];
                if (ch == '\"') {
                    inQuote = !inQuote;
                } else if (ch == ' ') {
                    if (!inQuote) {
                        break;
                    }
                }
                index++;
            }
            program = command.Substring(0, index).Trim();
            argument = command.Substring(index).TrimStart();
            if (program.Length >= 3 && program[0] =='\"' && program[program.Length - 1] == '\"') {
                program = program.Substring(1, program.Length - 2);
            }
        }
        
        //=========================================================================================
        // 機　能：プログラムと引数をコマンドラインに結合する
        // 引　数：[in]program  プログラム名
        // 　　　　[in]argument 引数
        // 戻り値：コマンドライン
        //=========================================================================================
        public static string CombineCommandLine(string program, string argument) {
            string result;
            if (program.IndexOf(' ') != -1) {
                result = "\"" + program + "\"";
            } else {
                result = program;
            }
            if (argument.Length > 0) {
                result += " " + argument;
            }
            return result;
        }

        //=========================================================================================
        // 機　能：ディレクトリ名の最後を'\'または'/'にする
        // 引　数：[in]dir        ディレクトリ名
        // 　　　　[in]separator  セパレータの文字
        // 戻り値：'\'または'/'を補完したディレクトリ名
        //=========================================================================================
        public static string CompleteDirectoryName(string dir, string separator) {
            if (dir.EndsWith(separator)) {
                return dir;
            } else {
                return dir + separator;
            }
        }

        //=========================================================================================
        // 機　能：ディレクトリ名の最後から'\'または'/'を取り除く
        // 引　数：[in]dir        ディレクトリ名
        // 　　　　[in]separator  セパレータの文字
        // 戻り値：'\'または'/'を取り除いたディレクトリ名
        //=========================================================================================
        public static string RemoveLastDirectorySeparator(string dir, string separator) {
            if (dir.EndsWith(separator)) {
                return dir.Substring(0, dir.Length - separator.Length);
            } else {
                return dir;
            }
        }

        //=========================================================================================
        // 機　能：ディレクトリ名の最後から'\'または'/'を取り除く
        // 引　数：[in]dir        ディレクトリ名
        // 戻り値：'\'または'/'を取り除いたディレクトリ名
        // メ　モ：ログ出力等の簡易処理でのみ使用する。パスの連結では原則使用しない。
        //=========================================================================================
        public static string RemoveLastDirectorySeparator(string dir) {
            if (dir.EndsWith("\\")) {
                return dir.Substring(0, dir.Length - 1);
            } else if (dir.EndsWith("/")) {
                return dir.Substring(0, dir.Length - 1);
            } else {
                return dir;
            }
        }

        //=========================================================================================
        // 機　能：コマンド文字列からコマンドファイル名を取得する
        // 引　数：[in]command  コマンド文字列
        // 戻り値：コマンドのファイル名（解析できないときnull）
        //=========================================================================================
        public static string GetCommandFilePath(string command) {
            if (command.StartsWith("\"")) {
                int commandEnd = command.IndexOf('\"', 1);
                if (commandEnd == -1) {
                    return null;
                } else {
                    return command.Substring(1, commandEnd - 1);
                }
            } else {
                int commandEnd = command.IndexOf(' ');
                if (commandEnd == -1) {
                    return command;
                } else {
                    return command.Substring(0, commandEnd);
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイル名一覧をコマンドライン文字列に変換する
        // 引　数：[in]fileList    ファイル名一覧
        // 　　　　[in]quoteAlways 常に" "で囲むときtrue、空白がある場合だけ囲むときfalse
        // 戻り値：コマンドライン状態になったファイル一覧
        //=========================================================================================
        public static string CreateCommandFiles(List<string> fileList, bool quoteAlways) {
            StringBuilder sb = new StringBuilder();
            foreach (string file in fileList) {
                if (sb.Length > 0) {
                    sb.Append(' ');
                }
                if (quoteAlways || file.Contains(" ")) {
                    sb.Append('\"');
                    sb.Append(file);
                    sb.Append('\"');
                } else {
                    sb.Append(file);
                }
            }
            return sb.ToString();
        }

        //=========================================================================================
        // 機　能：最後の文字がセパレータかどうかを返す
        // 引　数：[in]path  パス名
        // 戻り値：最後がセパレータのとセきtrue
        //=========================================================================================
        public static bool IsLastSeparator(string path) {
            if (path.EndsWith("\\") || path.EndsWith("/")) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：パス名の構成をセパレータで分割する
        // 引　数：[in]path  パス名
        // 戻り値：セパレータで分割したパス名（"\\a\b\c"は{"","a","b","c"}になる）
        //=========================================================================================
        public static string[] SplitSubDirectoryList(string path) {
            return path.Split('\\', '/');
        }

        //=========================================================================================
        // 機　能：パス名の構成をセパレータで分割する
        // 引　数：[in]path  パス名
        // 　　　　[out]
        // 戻り値：セパレータで分割したパス名（"\\a\b\c"は{"","a","b","c"}になる）
        //=========================================================================================
        public static bool SplitWindowsSubDirectoryList(string path, out string root, out string[] subDirList) {
            // ?:\形式
            Regex regex1 = new Regex(REG_FULLPATH_WITH_DRIVE);
            if (regex1.IsMatch(path)) {
                root = path.Substring(0, 3);
                if (path.EndsWith("\\")) {
                    path = path.Substring(0, path.Length - 1);
                }
                if (path.Length <= 3) {
                    subDirList = new string[0];
                } else {
                    subDirList = path.Substring(3).Split('\\');
                }
                return true;
            }
            // \\xxx\形式
            Regex regex2 = new Regex(REG_FULLPATH_WITH_NETWORK);
            Match match = regex2.Match(path);
            if (match.Success) {
                root = path.Substring(0, match.Length);
                path = path.Substring(match.Length);
                if (path.EndsWith("\\")) {
                    path = path.Substring(0, path.Length - 1);
                }
                subDirList = path.Split('\\');
                return true;
            }
            root = null;
            subDirList = null;
            return false;
        }

        //=========================================================================================
        // 機　能：Windowsの絶対パス表現かどうかを調べる
        // 引　数：[in]dir  ディレクトリ名
        // 戻り値：絶対パスのときtrue(trueでも実際にファイルアクセスできるかどうかは不明)
        //=========================================================================================
        public static bool IsWindowsAbsolutePath(string dir) {
            // ?:\形式
            Regex regex1 = new Regex(REG_FULLPATH_WITH_DRIVE);
            if (regex1.IsMatch(dir)) {
                return true;
            }
            // \\xxx\形式
            Regex regex2 = new Regex(REG_FULLPATH_WITH_NETWORK);
            if (regex2.IsMatch(dir)) {
                string[] fileFolder = dir.Substring(2).Split('\\');
                if (fileFolder.Length >= 3 || fileFolder.Length == 2 && !dir.EndsWith("\\")) {
                    // \\  server\share\  が最小、「server\share\」を分解すると2以上になる
                    return true;
                }
            }
            return false;
        }

        //=========================================================================================
        // 機　能：ドロップされたファイルの一覧から共通のルートとなるディレクトリ名を得る
        // 引　数：[in]fileList  ドロップされたファイルの一覧
        // 戻り値：共通のルートフォルダ名（共通のルートがない場合はnull）
        //=========================================================================================
        public static string GetCommonRoot(List<SimpleFileDirectoryPath> fileList) {
            // 共通部分を大文字小文字同一視で確認
            string commonRoot = null;                   // 共通部分を小文字化したもの
            foreach (SimpleFileDirectoryPath file in fileList) {
                if (commonRoot == null) {
                    commonRoot = file.FilePath.ToLower();
                } else {
                    string lowerFile = file.FilePath.ToLower();
                    int minPos = Math.Min(commonRoot.Length, lowerFile.Length);
                    for (int i = 0; i < minPos; i++) {
                        if (lowerFile[i] != commonRoot[i]) {
                            minPos = i;
                        }
                    }
                    commonRoot = commonRoot.Substring(0, minPos);
                }
            }

            // 共通部分を取得（先頭ファイルの形式）
            commonRoot = fileList[0].FilePath.Substring(0, commonRoot.Length);
            commonRoot = GenericFileStringUtils.GetDirectoryName(commonRoot);
            if (commonRoot == "") {
                return null;
            }

            // ファイル名として妥当かを確認
            FileSystemID fileSystemId = Program.Document.FileSystemFactory.GetFileSystemFromRootPath(commonRoot);
            if (fileSystemId == FileSystemID.None) {
                return null;
            }

            return commonRoot;
        }

        //=========================================================================================
        // 機　能：ファイル名に空白を含む場合、" "で囲む
        // 引　数：[in]fileName  ファイル名（null可能）
        // 戻り値：" "を補完したファイル名（入力がnullのときnull）
        //=========================================================================================
        public static string CompleteQuoteFileName(string fileName) {
            if (fileName == null) {
                return null;
            } else {
                if (fileName.IndexOf(' ') != -1) {
                    fileName = "\"" + fileName + "\"";
                }
                return fileName;
            }
        }

        //=========================================================================================
        // 機　能：指定されたパスのうち、はじめのディレクトリを返す
        // 引　数：[in]filePath  ファイルパス（先頭は「\」以外）
        // 戻り値：はじめのディレクトリ（dir1\dir2\dir3のとき、dir1を返す）
        //=========================================================================================
        public static string FirstFolder(string filePath) {
            int idxEn = filePath.IndexOf('\\');
            if (idxEn <= 0) {
                return filePath;
            } else {
                return filePath.Substring(0, idxEn);
            }
        }

        //=========================================================================================
        // 機　能：ファイルアイコンを取得する
        // 引　数：[in]filePath  ファイルパス
        // 　　　　[in]isDir     ディレクトリのときtrue
        // 　　　　[in]tryReal   実ファイルを取得するときtrue
        // 　　　　[in]width     取得するアイコンの幅
        // 　　　　[in]height    取得するアイコンの高さ
        // 戻り値：アイコン（失敗したとき、デフォルトアイコンを使用するときnull）
        //=========================================================================================
        public static Icon ExtractFileIcon(string filePath, bool isDir, bool tryReal, int width, int height) {
            // 仮想フォルダ/SSHで同じ実装
            if (isDir) {
                filePath = Win32IconUtils.SampleFolderPath;
            }
            if (width == IconSize.Large32.CxIconSize && height == IconSize.Large32.CyIconSize) {
                return Win32IconUtils.GetFileIconExtension(filePath, true);
            } else if (width ==IconSize.Small16.CxIconSize && height == IconSize.Small16.CyIconSize) {
                return Win32IconUtils.GetFileIconExtension(filePath, false);
            } else {
                return new Icon(Win32IconUtils.GetFileIconExtension(filePath, true), new Size(width, height));
            }
        }
        
        //=========================================================================================
        // 機　能：指定された文字列から始まるファイル数を返す
        // 引　数：[in]fileList    ファイル一覧
        // 　　　　[in]prefix      目的のファイル名のプレフィックス
        // 　　　　[in]ignoreCase  ファイル名の大文字小文字を無視するときtrue
        // 戻り値：ファイル数
        //=========================================================================================
        public static int CountFileStartsWith(List<IFile> fileList, string prefix, bool ignoreCase) {
            int count = 0;
            if (ignoreCase) {
                for (int i = 0; i < fileList.Count; i++) {
                    if (fileList[i].FileName.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase)) {
                        count++;
                    }
                }
            } else {
                for (int i = 0; i < fileList.Count; i++) {
                    if (fileList[i].FileName.StartsWith(prefix)) {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
