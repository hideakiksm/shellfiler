using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.Util;

namespace ShellFiler.Document.OSSpec {

    //=========================================================================================
    // クラス：shellのコマンドを組み立てるクラス
    //=========================================================================================
    public class ShellCommandDictionary {
        // プロンプトの変更コマンド
        // {0}  使用するプロンプト文字列 \XXXで8進数3桁が表す文字
        private string m_commandChangePrompt;

        // ファイル一覧の取得コマンド（ls）
        // {0}  ファイル一覧を取得するパス
        private string m_commandGetFileList;

        // ファイル一覧の取得の出力期待値
        private List<OSSpecLineExpect> m_expectGetFileList = new List<OSSpecLineExpect>();

        // ボリューム情報の取得コマンド（df）
        // {0}  ボリューム情報を取得するパス
        private string m_commandGetVolumeInfo;

        // ボリューム情報の取得の出力期待値
        private List<OSSpecLineExpect> m_expectGetVolumeInfo = new List<OSSpecLineExpect>();

        // ファイルの先頭部分の出力コマンド（head）
        // {0}  出力するファイル名
        // {1}  出力するサイズ[バイト]
        private string m_commandGetFileHead;

        // ファイルの先頭部分の出力期待値
        private List<OSSpecLineExpect> m_expectGetFileHead = new List<OSSpecLineExpect>();

        // 属性の取得コマンド
        // {0}  取得するファイル
        private string m_commandGetFileInfo;

        // 属性の取得の出力期待値
        private List<OSSpecLineExpect> m_expectGetFileInfo = new List<OSSpecLineExpect>();

        // ファイル時刻の設定のコマンド
        // {0}  設定する時刻
        // {1}  変更対象のファイル
        private string m_commandSetFileTime;

        // 最終更新時刻の設定のコマンド
        // {0}  設定する時刻
        // {1}  変更対象のファイル
        private string m_commandSetModifiedTime;

        // 最終アクセス時刻の設定のコマンド
        // {0}  設定する時刻
        // {1}  変更対象のファイル
        private string m_commandSetAccessedTime;

        // ファイル時刻の設定の出力期待値
        private List<OSSpecLineExpect> m_expectSetFileTime = new List<OSSpecLineExpect>();

        // ファイル時刻変更時の時刻指定フォーマット
        // CC:西暦上2桁
        // YY:西暦下2桁
        // MM:月
        // DD:日
        // hh:時
        // mm:分
        // ss:秒
        private string m_valueTouchTimeFormat = "CCYYMMDDhhmm.ss";

        // パーミッションの変更コマンド
        // {0}  変更するパーミッション（8進数3桁）
        // {1}  変更対象のファイル
        private string m_commandSetPermissions;

        // パーミッションの変更の出力期待値
        private List<OSSpecLineExpect> m_expectSetPermissions = new List<OSSpecLineExpect>();

        // ディレクトリの作成コマンド
        // {0}  作成するディレクトリ
        private string m_commandMakeDirectory;

        // ディレクトリの作成コマンドの期待値
        private List<OSSpecLineExpect> m_expectMakeDirectory = new List<OSSpecLineExpect>();

        // ディレクトリの再帰的削除コマンド
        // {0}  削除するディレクトリ
        private string m_commandDeleteDirectoryRecursive;

        // ディレクトリの再帰的削除コマンドの期待値
        private List<OSSpecLineExpect> m_expectDeleteDirectoryRecursive = new List<OSSpecLineExpect>();

        // ディレクトリの削除コマンド
        // {0}  削除するディレクトリ
        private string m_commandDeleteDirectory;

        // ディレクトリの削除コマンドの期待値
        private List<OSSpecLineExpect> m_expectDeleteDirectory = new List<OSSpecLineExpect>();

        // ファイルの削除コマンド
        // {0}  削除するディレクトリ
        private string m_commandDeleteFile;

        // ファイルの削除コマンドの期待値
        private List<OSSpecLineExpect> m_expectDeleteFile = new List<OSSpecLineExpect>();

        // ディレクトリを再帰的に削除できない場合のプロンプト
        private string m_valueDeleteDirectoryRecursivePrompt = "rm: *(yes/no)";
        
        // ディレクトリを再帰的に削除できない場合の応答
        private string m_valueDeleteDirectoryRecursiveAnswer = "n";
        
        // ディレクトリを削除できない場合のプロンプト
        private string m_valueDeleteDirectoryPrompt = "rmdir: *(yes/no)";

        // ディレクトリを削除できない場合の応答
        private string m_valueDeleteDirectoryAnswer = "n";

        // ファイルを削除できない場合のプロンプト
        private string m_valueDeleteFilePrompt = "rm: *(yes/no)";

        // ファイルを削除できない場合の応答
        private string m_valueDeleteFileAnswer = "n";

        // ファイルアップロードのコマンド
        // {0}  EOFのマーカー
        // {1}  転送先ファイル
        private string m_commandUpload;

        // ファイルアップロードの期待値
        private List<OSSpecLineExpect> m_expectUpload = new List<OSSpecLineExpect>();

        // ファイルアップロードのヒアドキュメントプロンプト
        private string m_valueUploadHearDocument = "> ";

        // ファイルアップロードのエンコード種別
        private ShellUploadEncoding m_valueUploadEncoding = ShellUploadEncoding.HexStream;

        // ファイルアップロードの1行で送信するサイズ
        private int m_valueUploadChunkSize = 1024;

        // ファイルダウンロードのコマンド
        // {0}  転送元ファイル
        private string m_commandDownload;

        // ファイルダウンロードの出力期待値
        private List<OSSpecLineExpect> m_expectDownload = new List<OSSpecLineExpect>();

        // ファイルのチェックサムを計算するコマンド
        private string m_commandComputeChecksum;

        // ファイルのチェックサムを計算するコマンドの期待値
        private List<OSSpecLineExpect> m_expectComputeChecksum = new List<OSSpecLineExpect>();

        // アップロード/ダウンロードの再試行回数
        private int m_valueUploadDownloadRetryCount = 3;

        // アップロード/ダウンロードでチェックサムの計算を行うときtrue
        private bool m_valueUploadDownloadUseCheckCksum = true;

        // リネームのコマンド
        // {0}  旧ファイル名のパス
        // {1}  新ファイル名のパス
        private string m_commandRename;
        
        // リネームの期待値
        private List<OSSpecLineExpect> m_expectRename = new List<OSSpecLineExpect>();

        // ファイルのオーナーユーザーを変更するコマンド
        // {0}  変更対象のファイル名
        // {1}  新しいオーナーユーザー
        // {2}  新しいオーナーグループ
        private string m_commandChangeOwnerUser;

        // ファイルのオーナーグループを変更するコマンド
        private string m_commandChangeOwnerGroup;

        // ファイルのオーナーユーザーとグループを変更するコマンド
        private string m_commandChangeOwnerUserGroup;

        // ファイルのオーナーユーザー変更の期待値
        private List<OSSpecLineExpect> m_expectChangeOwner = new List<OSSpecLineExpect>();

        // ファイルをコピーするコマンド（上書き確認なし、ファイルの単純なコピー）
        // {0}  転送元のパス名
        // {1}  転送先のパス名
        private string m_commandCopyFile;

        // ファイルをコピーするコマンド（上書き確認なし、ファイルの属性も伴うコピー）
        // {0}  転送元のパス名
        // {1}  転送先のパス名
        private string m_commandCopyFileAndAttr;

        // ファイルをコピーするコマンド（上書き確認なし、ディレクトリの単純なコピー）
        // {0}  転送元のパス名
        // {1}  転送先のパス名
        private string m_commandCopyDirectory;

        // ファイルをコピーするコマンド（上書き確認なし、ディレクトリの属性も伴うコピー）
        // {0}  転送元のパス名
        // {1}  転送先のパス名
        private string m_commandCopyDirectoryAndAttr;

        // ファイルコピーの期待値
        private List<OSSpecLineExpect> m_expectCopy = new List<OSSpecLineExpect>();

        // ファイルやディレクトリを移動するコマンド（上書き確認なし、単純な移動）
        // {0}  転送元のパス名
        // {1}  転送先のパス名
        private string m_commandMove;
        
        // ファイルやディレクトリを移動するコマンド（上書き確認なし、属性も伴う移動）
        // {0}  転送元のパス名
        // {1}  転送先のパス名
        private string m_commandMoveAndAttr;

        // ファイル移動の期待値
        private List<OSSpecLineExpect> m_expectMove = new List<OSSpecLineExpect>();

        // ファイルやディレクトリのリンクを作成するコマンド（シンボリックリンク）
        private string m_commandSymboricLink;

        // ファイルやディレクトリのリンクを作成するコマンド（ハードリンク）
        private string m_commandHardLink;

        // リンク作成の期待値
        private List<OSSpecLineExpect> m_expectLink = new List<OSSpecLineExpect>();

        // 操作ユーザーを切り替えるコマンド
        private string m_commandChangeLoginUser;

        // 操作ユーザーを切り替えるコマンド（ログインシェルを使う）
        private string m_commandChangeLoginUserShell;

        // 操作ユーザーを切り替えるコマンドの期待値
        private List<OSSpecLineExpect> m_expectChangeLoginUser = new List<OSSpecLineExpect>();

        // ルートユーザー名
        private string m_valueRootUserName = "root";

        // カレントユーザー変更のプロンプト
        private string[] m_valueChangeUserPrompt = new string[] {"パスワード: ", "Password: "};

        // ログインシェルを使って切り替える方をデフォルトにするときtrue
        private bool m_valueChangeUserLoginShell = true;

        // プロンプト文字列のユーザー名@サーバー名
        private string m_valuePromptUserServer = @"\\u@\\h";

        // 現在の操作ユーザーから抜けるコマンド
        private string m_commandExit;

        // 現在の操作ユーザーから抜けるコマンドの期待値
        private List<OSSpecLineExpect> m_expectExit = new List<OSSpecLineExpect>();

        // カレントディレクトリを取得するコマンド
        private string m_commandGetCurrentDirectory;

        // カレントディレクトリを取得するコマンドの期待値
        private List<OSSpecLineExpect> m_expectGetCurrentDirectory = new List<OSSpecLineExpect>();

        // ファイルの結合を行うコマンド（先頭ファイル）
        private string m_commandAppendFileFirst;
        
        // ファイルの結合を行うコマンド（２件目以降のファイル）
        private string m_commandAppendFileNext;
        
        // ファイルの結合を行うコマンドの期待値
        private List<OSSpecLineExpect> m_expectAppendFile;

        // ファイルの分割を行うコマンド
        private string m_commandSplitFile;

        // ファイルの分割を行うコマンドの期待値
        private List<OSSpecLineExpect> m_expectSplitFile;

        // シンボリックリンクのリンク先を調べるコマンド（cd後に実行）
        private string m_commandCheckSymbolicLink;

        // シンボリックリンクのリンク先を調べるコマンドの期待値（エラー定義のみ）
        private List<OSSpecLineExpect> m_expectCheckSymbolicLink = new List<OSSpecLineExpect>();

        // シンボリックリンクのリンク先の同時取得件数
        private int m_valueLinkTargetSameTimeCount = 10;

        // フォルダサイズの取得コマンド
        private string m_commandRetrieveFolderSize;

        // フォルダサイズの取得コマンドの期待値
        private List<OSSpecLineExpect> m_expectRetrieveFolderSize = new List<OSSpecLineExpect>();

        // 外部コマンド起動時に追加するコマンド
        private string m_commandShellExecutePrev;

        // 外部コマンド起動コマンドの期待値
        private List<OSSpecLineExpect> m_expectShellExecute = new List<OSSpecLineExpect>();

        // プロセス一覧の取得コマンド
        private string m_commandGetProcessList;

        // プロセス一覧の取得コマンドの期待値
        private List<OSSpecLineExpect> m_expectGetProcessList = new List<OSSpecLineExpect>();

        // ネットワーク状況一覧のコマンド
        private string m_commandNetStat;

        // ネットワーク状況一覧のコマンドの期待値
        private List<OSSpecLineExpect> m_expectNetStat = new List<OSSpecLineExpect>();

        // プロセスの終了コマンド
        // {0}  終了するプロセスのpid
        private string m_commandKillProcess;

        // プロセスの強制終了コマンド
        // {0}  終了するプロセスのpid
        private string m_commandKillProcessForce;

        // プロセスの強制終了コマンドの期待値
        private List<OSSpecLineExpect> m_expectKillProcess = new List<OSSpecLineExpect>();
        
        // zipの作成コマンド（書庫の時刻を格納ファイルの最新日時に合わせる）
        private string m_commandArchiveZipTime;

        // zipの作成コマンド（書庫の時刻は指定なし）
        private string m_commandArchiveZip;

        // zip作成コマンドの期待値
        private List<OSSpecLineExpect> m_expectArchiveZip = new List<OSSpecLineExpect>();

        // zipの圧縮レベルの指定オプション
        private string m_valueArchiveZipCompressionOption = "-{0}";

        // zipの圧縮レベルのデフォルト
        private int m_valueArchiveZipComplessionDefault = 6;

        // tar.gzの作成コマンド
        private string m_commandArchiveTarGz;

        // tar.gzの作成コマンドの期待値
        private List<OSSpecLineExpect> m_expectArchiveTarGz = new List<OSSpecLineExpect>();

        // tar.bz2の作成コマンド
        private string m_commandArchiveTarBz2;

        // tar.bz2の作成コマンドの期待値
        private List<OSSpecLineExpect> m_expectArchiveTarBz2 = new List<OSSpecLineExpect>();

        // tarの作成コマンド
        private string m_commandArchiveTar;

        // tarの作成コマンドの期待値
        private List<OSSpecLineExpect> m_expectArchiveTar = new List<OSSpecLineExpect>();
        
        // アーカイブの実行コマンド
        // {0}   実行ディレクトリ
        // {1}   アーカイブファイル名
        private string m_commandArchiveExecute;

        // アーカイブの実行コマンドの期待値
        private List<OSSpecLineExpect> m_expectArchiveExecute = new List<OSSpecLineExpect>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ShellCommandDictionary() {
        }

        //=========================================================================================
        // 機　能：ファイル名をコマンド形式に変換する
        // 引　数：[in]file  ファイル名
        // 戻り値：コマンド形式のファイル名
        //=========================================================================================
        private static string F(string file) {
            if (file.IndexOf(' ') == -1) {
                return file;
            } else {
                return "\"" + file + "\"";
            }
        }

        //=========================================================================================
        // 機　能：プロンプトの変更コマンド用に文字列を変換する
        // 引　数：[in]prompt   使用するプロンプト文字列
        // 戻り値：コマンドライン用のプロンプト文字列
        //=========================================================================================
        private static string EscapeCommandPrompt(string prompt) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < prompt.Length; i++) {
                char ch = prompt[i];
                if (ch < ' ') {
                    sb.Append(@"\\");
                    string oct = "00" + Convert.ToString((int)ch, 8);
                    sb.Append(oct.Substring(oct.Length - 3, 3));
                } else {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }


        //=========================================================================================
        // 機　能：日付をSetFileTimeコマンドの形式に変換する
        // 引　数：[in]date    変換する日付
        // 　　　　[in]format  touchコマンドのフォーマット
        // 戻り値：変換結果
        //=========================================================================================
        private static string DateToSetFileTimeFormat(DateTime date, string format) {
            // yyyy/MM/dd HH:mm:ss
            // CCYYMMDDhhmmss
            string strDate = date.ToString("yyyyMMddHHmmss");
            string[][] control = new string[][] {
                new string[] { "CC", strDate.Substring(0, 2) },
                new string[] { "YY", strDate.Substring(2, 2) },
                new string[] { "MM", strDate.Substring(4, 2) },
                new string[] { "DD", strDate.Substring(6, 2) },
                new string[] { "hh", strDate.Substring(8, 2) },
                new string[] { "mm", strDate.Substring(10, 2) },
                new string[] { "ss", strDate.Substring(12, 2) },
            };

            StringBuilder result = new StringBuilder();
            int index = 0;
            int length = format.Length;
            while (index < length) {
                bool found = false;
                if (index + 1 < length) {
                    for (int i = 0; i < control.Length; i++) {
                        string linuxFormatTemplate = control[i][0];
                        if (format[index] == linuxFormatTemplate[0] && format[index + 1] == linuxFormatTemplate[1]) {
                            result.Append(control[i][1]);
                            index += linuxFormatTemplate.Length;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found) {
                    result.Append(format[index]);
                    index++;
                }
            }
            return result.ToString();
        }

        //=========================================================================================
        // 機　能：プロンプトの変更コマンドを取得する
        // 引　数：[in]prompt   使用するプロンプト文字列
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandChangePrompt(string prompt) {
            string escapedPrompt = EscapeCommandPrompt(prompt);
            string result = string.Format(m_commandChangePrompt, escapedPrompt);
            return result;
        }
        
        //=========================================================================================
        // 機　能：ファイル一覧の取得コマンド（ls）を取得する
        // 引　数：[in]path  ファイル一覧を取得するパス
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandGetFileList(string path) {
            string result = string.Format(m_commandGetFileList, F(path));
            return result;
        }
        
        //=========================================================================================
        // 機　能：ボリューム情報の取得コマンド（df）を返す
        // 引　数：[in]path  ディスク情報を取得するパス
        // 戻り値：コマンドライン
        // メ　モ：ディスクのボリューム情報を得る
        //=========================================================================================
        public string GetCommandGetVolumeInfo(string path) {
            string result = string.Format(m_commandGetVolumeInfo, F(path));
            return result;
        }

        //=========================================================================================
        // 機　能：ファイルの先頭部分の出力コマンド（head）を返す
        // 引　数：[in]path   取得するファイルのパス
        // 　　　　[in]size   取得するサイズ
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandGetFileHead(string path, int size) {
            string result = string.Format(m_commandGetFileHead, F(path), size);
            return result;
        }

        //=========================================================================================
        // 機　能：属性の取得コマンドを返す
        // 引　数：[in]path   取得するファイルのパス
        // 　　　　[in]size   取得するサイズ
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandGetFileInfo(string path) {
            string result = string.Format(m_commandGetFileInfo, F(path));
            return result;
        }

        //=========================================================================================
        // 機　能：ファイル時刻(mtime/atime)の設定コマンドを返す
        // 引　数：[in]size   設定するファイルのパス
        // 　　　　[in]time   設定する時刻
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandSetFileTime(string path, DateTime time) {
            string result = string.Format(m_commandSetFileTime, DateToSetFileTimeFormat(time, m_valueTouchTimeFormat), F(path));
            return result;
        }

        //=========================================================================================
        // 機　能：ファイル更新時刻(mtime)の設定コマンドを返す
        // 引　数：[in]size   設定するファイルのパス
        // 　　　　[in]time   設定する時刻
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandSetModifiedTime(string path, DateTime time) {
            string result = string.Format(m_commandSetModifiedTime, DateToSetFileTimeFormat(time, m_valueTouchTimeFormat), F(path));
            return result;
        }

        //=========================================================================================
        // 機　能：ファイルアクセス時刻(atime)の設定コマンドを返す
        // 引　数：[in]path   設定するファイルのパス
        // 　　　　[in]time   設定する時刻
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandSetAccessedTime(string path, DateTime time) {
            string result = string.Format(m_commandSetAccessedTime, DateToSetFileTimeFormat(time, m_valueTouchTimeFormat), F(path));
            return result;
        }
 
        //=========================================================================================
        // 機　能：パーミッションの変更コマンドを返す
        // 引　数：[in]path          変更対象のファイル
        // 　　　　[in]permissions   変更するパーミッション（8進数3桁）
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandSetPermissions(string path, int permissions) {
            string strPermissions = "00" + Convert.ToString(permissions, 8);
            strPermissions = strPermissions.Substring(strPermissions.Length - 3, 3);
            string result = string.Format(m_commandSetPermissions, strPermissions, F(path));
            return result;
        }

        //=========================================================================================
        // 機　能：ディレクトリの作成コマンドを返す
        // 引　数：[in]path    作成するディレクトリ
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandMakeDirectory(string path) {
            string result = string.Format(m_commandMakeDirectory, F(path));
            return result;
        }

        //=========================================================================================
        // 機　能：ディレクトリを再帰的に削除するコマンドを返す
        // 引　数：[in]path    削除するディレクトリ
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandDeleteDirectoryRecursive(string path) {
            if (path == "/" || path.StartsWith("/ ")) {
                Program.Abort("ルートの削除は回避済みのはずです。");
                return null;
            }
            string result = string.Format(m_commandDeleteDirectoryRecursive, F(path));
            return result;
        }

        //=========================================================================================
        // 機　能：ディレクトリを削除するコマンドを返す
        // 引　数：[in]path    削除するディレクトリ
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandDeleteDirectory(string path) {
            if (path == "/" || path.StartsWith("/ ")) {
                Program.Abort("ルートの削除は回避済みのはずです。");
                return null;
            }
            string result = string.Format(m_commandDeleteDirectory, F(path));
            return result;
        }

        //=========================================================================================
        // 機　能：ファイルを削除するコマンドを返す
        // 引　数：[in]path    削除するファイル
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandDeleteFile(string path) {
            if (path == "/" || path.StartsWith("/ ")) {
                Program.Abort("ルートの削除は回避済みのはずです。");
                return null;
            }
            string result = string.Format(m_commandDeleteFile, F(path));
            return result;
        }

        //=========================================================================================
        // 機　能：ファイルアップロードのコマンドを返す
        // 引　数：[in]eof   EOFのマーカー
        // 　　　　[in]path  出力ファイル
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandUpload(string eof, string path) {
            string result = string.Format(m_commandUpload, eof, F(path));
            return result;
        }

        //=========================================================================================
        // 機　能：ファイルダウンロードのコマンドを返す
        // 引　数：[in]path  出力ファイル
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandDownload(string path) {
            string result = string.Format(m_commandDownload, F(path));
            return result;
        }

        //=========================================================================================
        // 機　能：ファイルのチェックサムを計算する
        // 引　数：[in]path  対象ファイル
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandComputeChecksum(string path) {
            string result = string.Format(m_commandComputeChecksum, F(path));
            return result;
        }

        //=========================================================================================
        // 機　能：ファイルダウンロードのコマンドを返す
        // 引　数：[in]oldPath  変更元ファイルのパス
        // 　　　　[in]newPath  変更先ファイルのパス
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandRename(string oldPath, string newPath) {
            string result = string.Format(m_commandRename, F(oldPath), F(newPath));
            return result;
        }

        //=========================================================================================
        // 機　能：ファイルのオーナー変更のコマンドを返す
        // 引　数：[in]filePath     変更対象のファイル名
        // 　　　　[in]ownerUser    新しいオーナーユーザー（変更しないときnull）
        // 　　　　[in]ownerGroup   新しいオーナーグループ（変更しないときnull）
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetChangeOwnerCommand(string filePath, string ownerUser, string ownerGroup) {
            if (ownerUser != null && ownerGroup != null) {
                string result = string.Format(m_commandChangeOwnerUserGroup, F(filePath), ownerUser, ownerGroup);
                return result;
            } else if (ownerUser != null) {
                string result = string.Format(m_commandChangeOwnerUser, F(filePath), ownerUser);
                return result;
            } else if (ownerGroup != null) {
                string result = string.Format(m_commandChangeOwnerGroup, F(filePath), "", ownerGroup);
                return result;
            } else {
                Program.Abort("オーナーの制御が異常です。");
                return null;
            }
        }
        
        //=========================================================================================
        // 機　能：コピー、移動、リンクのコマンドを返す
        // 引　数：[in]mode       コピーや移動のモード（リンク系も有効）
        // 　　　　[in]srcPath    転送元ファイルのフルパス
        // 　　　　[in]destPath   転送先ファイルのフルパス
        // 　　　　[in]attrCopy   属性コピーを行うときtrue
        // 　　　　[out]command   コマンドラインを返す変数
        // 　　　　[out]expect    期待値を返す変数
        // 戻り値：コマンドライン
        // メ　モ：上書きありのコピー
        //=========================================================================================
        public void GetCopyMoveLinkCommand(TransferModeType mode, string srcPath, string destPath, bool attrCopy, out string command, out List<OSSpecLineExpect> expect) {
            srcPath = F(srcPath);
            destPath = F(destPath);
            string template;
            switch (mode) {
                case TransferModeType.CopyFile:
                    if (attrCopy) {    
                        template = m_commandCopyFile;
                        expect = m_expectCopy;
                    } else {
                        template = m_commandCopyFileAndAttr;
                        expect = m_expectCopy;
                    }
                    break;
                case TransferModeType.CopyDirectory:
                    if (attrCopy) {    
                        template = m_commandCopyDirectory;
                        expect = m_expectCopy;
                    } else {
                        template = m_commandCopyDirectoryAndAttr;
                        expect = m_expectCopy;
                    }
                    break;
                case TransferModeType.MoveFile:
                    if (attrCopy) {    
                        template = m_commandMove;
                        expect = m_expectMove;
                    } else {
                        template = m_commandMoveAndAttr;
                        expect = m_expectMove;
                    }
                    break;
                case TransferModeType.SymbolicLink:
                    template = m_commandSymboricLink;
                    expect = m_expectLink;
                    break;
                case TransferModeType.HardLink:
                    template = m_commandHardLink;
                    expect = m_expectLink;
                    break;
                default:
                    Program.Abort("コピーモードの制御が想定外です。mode={0}", mode);
                    template = null;
                    expect = null;
                    break;
            }
            command = string.Format(template, srcPath, destPath);
        }
        
        //=========================================================================================
        // 機　能：操作ユーザーを切り替えるコマンドを返す
        // 引　数：[in]userName       ユーザー名
        // 　　　　[in]useLoginShell  ログインシェルを使用するときtrue
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetChangeLoginUserCommand(string userName, bool useLoginShell) {
            string command;
            if (useLoginShell) {
                command = string.Format(m_commandChangeLoginUserShell, userName);
            } else {
                command = string.Format(m_commandChangeLoginUser, userName);
            }
            return command;
        }

        //=========================================================================================
        // 機　能：現在の操作ユーザーから抜けるコマンドを返す
        // 引　数：なし
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetExitCommand() {
            return m_commandExit;
        }

        //=========================================================================================
        // 機　能：カレントディレクトリを取得するコマンドを返す
        // 引　数：なし
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandGetCurrentDirectory() {
            return m_commandGetCurrentDirectory;
        }

        //=========================================================================================
        // 機　能：ファイルの連結を行うコマンドを返す
        // 引　数：[in]srcPath    転送元ファイル
        // 　　　　[in]destPath   転送先ファイル
        // 　　　　[in]firstFile  先頭ファイルを処理するときtrue
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetAppendFileCommand(string srcPath, string destPath, bool firstFile) {
            string command;
            if (firstFile) {
                command = string.Format(m_commandAppendFileFirst, srcPath, destPath);
            } else {
                command = string.Format(m_commandAppendFileNext, srcPath, destPath);
            }
            return command;
        }

        //=========================================================================================
        // 機　能：ファイルの分割を行うコマンドを返す
        // 引　数：[in]splitSize  分割サイズ
        // 　　　　[in]srcPath    転送元ファイル
        // 　　　　[in]destPath   転送先ファイルのプレフィックス
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetSplitFileCommand(long size, string srcPath, string destPath) {
            string command = string.Format(m_commandSplitFile, size, srcPath, destPath);
            return command;
        }

        //=========================================================================================
        // 機　能：シンボリックリンクのリンク先取得コマンドを返す
        // 引　数：[in]basePath     取得するファイルのパス
        // 　　　　[in]fileList     取得するファイルの一覧
        // 　　　　[in]existsMarker リンク先が存在するかどうかのマーカーの一覧
        // 　　　　[in]dirMarker    ディレクトリかどうかのマーカーの一覧
        // 戻り値：コマンドライン
        // メ　モ：1行目にls -lの結果（「-> リンク先」）
        // 　　　　2行目に存在するかどうかのマーカーを返す
        // 　　　　3行目にディレクトリかどうかのマーカーを返す
        //=========================================================================================
        public string GetCommandCheckSymbolicLink(string basePath, List<string> fileList, List<string> existMarker, List<string> dirMarker) {
            StringBuilder result = new StringBuilder();
            result.Append("cd ").Append(basePath);
            for (int i = 0; i < fileList.Count; i++) {
                string file = fileList[i];
                result.Append(";");
                result.Append(string.Format(m_commandCheckSymbolicLink, F(file), existMarker[i], dirMarker[i]));
            }
            return result.ToString();
        }

        //=========================================================================================
        // 機　能：シンボリックリンクのリンク先取得コマンドを返す
        // 引　数：[in]path         取得するファイルのパス
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetCommandRetrieveFolderSize(string path) {
            string result = string.Format(m_commandRetrieveFolderSize, F(path));
            return result;
        }
        
        //=========================================================================================
        // 機　能：シェルコマンド実行のコマンドラインを成形する
        // 引　数：[in]command       コマンドラインの本体
        // 　　　　[in]currentDirSf  ShellFiler形式パス表現のカレントディレクトリ
        // 戻り値：実行するコマンド
        //=========================================================================================
        public string ModifyShellExecuteCommand(string command, string currentDirSf) {
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(currentDirSf, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            string result = string.Format(m_commandShellExecutePrev, F(path)) + " && " + command;
            return result;
        }

        //=========================================================================================
        // 機　能：プロセス一覧のコマンドを返す
        // 引　数：[in]separator  全体と個人の一覧の境界に使う文字列
        // 戻り値：プロセス一覧のコマンド
        //=========================================================================================
        public string GetCommandGetProcessList() {
            return m_commandGetProcessList;
        }

        //=========================================================================================
        // 機　能：接続先一覧のコマンドを返す
        // 引　数：なし
        // 戻り値：接続先一覧のコマンド
        //=========================================================================================
        public string GetCommandNetStat() {
            return m_commandNetStat;
        }

        //=========================================================================================
        // 機　能：プロセスの終了コマンドを返す
        // 引　数：[in]pid     終了するプロセスのpid
        // 　　　　[in]force   強制終了するときtrue
        // 戻り値：プロセス終了コマンド
        //=========================================================================================
        public string GetCommandKill(int pid, bool force) {
            string result;
            if (force) {
                result = string.Format(m_commandKillProcess, pid);
            } else {
                result = string.Format(m_commandKillProcessForce, pid);
            }
            return result;
        }

        //=========================================================================================
        // 機　能：zipの作成コマンドを返す
        // 引　数：[in]modifyTimestamp  書庫の時刻を格納ファイルの最新日時にあわせるときtrue
        // 　　　　[in]level            圧縮レベル（0～9）
        // 戻り値：コマンドライン（対応していないときは常にnullを返す）
        //=========================================================================================
        public string GetCommandArchiveZip(bool modifyTimestamp, int level) {
            string strCommand;
            if (modifyTimestamp) {
                strCommand = m_commandArchiveZip;
            } else {
                strCommand = m_commandArchiveZipTime;
            }
            if (level != m_valueArchiveZipComplessionDefault) {
                strCommand += " " + string.Format(m_valueArchiveZipCompressionOption, level);
            }
            return strCommand;
        }

        //=========================================================================================
        // 機　能：tar.gzの作成コマンドを返す
        // 引　数：なし
        // 戻り値：コマンドライン（対応していないときは常にnullを返す）
        //=========================================================================================
        public string GetCommandArchiveTarGz() {
            return m_commandArchiveTarGz;
        }

        //=========================================================================================
        // 機　能：tar.bz2の作成コマンドを返す
        // 引　数：なし
        // 戻り値：コマンドライン（対応していないときは常にnullを返す）
        //=========================================================================================
        public string GetCommandArchiveTarBz2() {
            return m_commandArchiveTarBz2;
        }

        //=========================================================================================
        // 機　能：tarの作成コマンドを返す
        // 引　数：なし
        // 戻り値：コマンドライン（対応していないときは常にnullを返す）
        //=========================================================================================
        public string GetCommandArchiveTar() {
            return m_commandArchiveTar;
        }

        //=========================================================================================
        // 機　能：アーカイブ用のコマンドを完成形にして返す
        // 引　数：[in]command   アーカイブコマンド
        // 　　　　[in]dir       実行するディレクトリ
        // 　　　　[in]arcName   書庫ファイル名
        // 　　　　[in]fileList  ファイル一覧
        // 戻り値：アーカイブ用のコマンド
        //=========================================================================================
        public string CompleteArchiveCommand(string command, string dir, string arcName, List<string> fileList) {
            StringBuilder files = new StringBuilder();
            bool first = true;
            foreach (string fileName in fileList) {
                if (first) {
                    first = false;
                } else {
                    files.Append(" ");
                }
                files.Append(F(fileName));
            }

            command = command.Trim();
            string result = string.Format(m_commandArchiveExecute, F(dir), F(arcName)) + " " + command + " " + F(arcName) + " " + files.ToString();
            return result;
        }
       
        //=========================================================================================
        // 機　能：アーカイブの実行コマンドの期待値を返す
        // 引　数：[in]expect  コマンド単体の実行期待値
        // 戻り値：コマンド全体の実行期待値
        //=========================================================================================
        public List<OSSpecLineExpect> GetExpectArchive(List<OSSpecLineExpect> expect) {
            List<OSSpecLineExpect> result = new List<OSSpecLineExpect>();
            result.AddRange(m_expectArchiveExecute);
            result.Add(new OSSpecLineExpect(expect[0].LineType | OSSpecLineType.OrPrev, expect[0].ColumnExpect.ToArray()));
            for (int i = 1; i< expect.Count; i++) {
                result.Add(expect[i]);
            }
            return result;
        }

 
        //=========================================================================================
        // 機　能：duコマンドを返す
        // 引　数：[in]path  ディレクトリ容量を取得するパス
        // 戻り値：コマンドライン
        // メ　モ：ディレクトリ容量を得る
        //=========================================================================================
        public string GetDuCommand(string path) {
            return "du -b " + F(path);
        }

        //=========================================================================================
        // 機　能：cpコマンドを返す
        // 引　数：[in]srcPath    転送元ファイルのフルパス
        // 　　　　[in]destPath   転送先ファイルのフルパス
        // 　　　　[in]attrCopy   属性コピーを行うときtrue
        // 戻り値：コマンドライン
        // メ　モ：上書きありのコピー
        //=========================================================================================
        public string GetCpCommand(string srcPath, string destPath, bool attrCopy) {
            if (attrCopy) {
                return "cp -p " + F(srcPath) + " " + F(destPath);
            } else {
                return "cp " + F(srcPath) + " " + F(destPath);
            }
        }

        //=========================================================================================
        // 機　能：cp -rコマンドを返す
        // 引　数：[in]srcPath    転送元ファイルのフルパス
        // 　　　　[in]destPath   転送先ファイルのフルパス
        // 　　　　[in]attrCopy   属性コピーを行うときtrue
        // 戻り値：コマンドライン
        // メ　モ：上書きありのコピー
        //=========================================================================================
        public string GetCpRCommand(string srcPath, string destPath, bool attrCopy) {
            if (attrCopy) {
                return "cp -r -p " + F(srcPath) + " " + F(destPath);
            } else {
                return "cp -r " + F(srcPath) + " " + F(destPath);
            }
        }

        //=========================================================================================
        // 機　能：mvコマンドを返す
        // 引　数：[in]srcPath    転送元ファイルのフルパス
        // 　　　　[in]destPath   転送先ファイルのフルパス
        // 　　　　[in]attrCopy   属性コピーを行うときtrue
        // 戻り値：コマンドライン
        // メ　モ：上書きありの移動
        //=========================================================================================
        public string GetMvCommand(string srcPath, string destPath, bool attrCopy) {
            return "mv -i " + F(srcPath) + " " + F(destPath);
        }

        //=========================================================================================
        // 機　能：rm -rfコマンドを返す
        // 引　数：[in]path   削除するディレクトリのフルパス
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetRmRfCommand(string path) {
            if (path == "/") {
                return "";          // 事前チェック済みのはず
            }
            return "rm -rf " + F(path);
        }
        
        //=========================================================================================
        // 機　能：chownコマンドを返す
        // 引　数：[in]owner  新しいオーナー（null:変更なし）
        // 　　　　[in]group  新しいグループ（null:変更なし）
        // 　　　　[in]path   変更するファイルのパス
        // 戻り値：コマンドライン
        // メ　モ：owner, grouとも名前による変更
        //=========================================================================================
        public string GetChownCommand(string owner, string group, string path) {
            if (owner != null && group != null) {
                return "chown " + owner + " " + F(path);
            } else if (owner == null && group != null) {
                return "chown :" + group + " " + F(path);
            } else {
                return "chown " + owner + ":" + group + " " + F(path);
            }
        }

        //=========================================================================================
        // 機　能：シンボリックリンクのリンク先取得コマンドを返す
        // 引　数：[in]path         取得するファイルのパス
        // 　　　　[in]fileList     取得するファイルの一覧
        // 　　　　[in]existsMarker リンク先が存在するかどうかのマーカーの一覧
        // 　　　　[in]dirMarker    ディレクトリかどうかのマーカーの一覧
        // 戻り値：コマンドライン
        // メ　モ：1行目にls -lの結果（「-> リンク先」）
        // 　　　　2行目に存在するかどうかのマーカーを返す
        // 　　　　3行目にディレクトリかどうかのマーカーを返す
        //=========================================================================================
        public string GetLsSymbolicLinkCommand(string basePath, List<string> fileList, List<string> existMarker, List<string> dirMarker) {
            StringBuilder result = new StringBuilder();
            result.Append("cd ").Append(basePath).Append(";");
            for (int i = 0; i < fileList.Count; i++) {
                if (i > 0) {
                    result.Append(";");
                }
                string file = fileList[i];
                result.Append("ls -l ").Append(F(file));
                result.Append(" && test -e ").Append(F(file)).Append(" && echo \"").Append(existMarker[i]).Append("\"");
                result.Append(" && test -d ").Append(F(file)).Append(" && echo \"").Append(dirMarker[i]).Append("\"");
            }
            return result.ToString();
        }

        //=========================================================================================
        // 機　能：シンボリックリンクの作成コマンドを返す
        // 引　数：[in]orgPath   元ファイルのパス
        // 　　　　[in]linkPath  作成するリンクファイルのパス
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetLnSymbolicCommand(string orgPath, string linkPath) {
            return "ln -s -f " + F(orgPath) + " " + F(linkPath);
        }

        //=========================================================================================
        // 機　能：ハードリンクの作成コマンドを返す
        // 引　数：[in]orgPath   元ファイルのパス
        // 　　　　[in]linkPath  作成するリンクファイルのパス
        // 戻り値：コマンドライン
        //=========================================================================================
        public string GetLnHardCommand(string orgPath, string linkPath) {
            return "ln -f " + F(orgPath) + " " + F(linkPath);
        }


        //=========================================================================================
        // 機　能：中断を行う際のキーを取得する
        // 引　数：なし
        // 戻り値：Ctrl+Cキーのコード
        //=========================================================================================
        public string GetBreakCode() {
            return "\x03";
        }

        //=========================================================================================
        // 機　能：実行を行う際のキーを取得する
        // 引　数：なし
        // 戻り値：Enterキーのコード
        //=========================================================================================
        public string GetEnterCode() {
            return "\n";
        }

        //=========================================================================================
        // プロパティ：プロンプトの変更コマンド
        //=========================================================================================
        public string CommandChangePrompt {
            get {
                return m_commandChangePrompt;
            }
            set {
                m_commandChangePrompt = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧の取得コマンド（ls）
        //=========================================================================================
        public string CommandGetFileList {
            get {
                return m_commandGetFileList;
            }
            set {
                m_commandGetFileList = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧の取得の出力期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectGetFileList {
            get {
                return m_expectGetFileList;
            }
            set {
                m_expectGetFileList = value;
            }
        }

        //=========================================================================================
        // プロパティ：ボリューム情報の取得コマンド（df）
        //=========================================================================================
        public string CommandGetVolumeInfo {
            get {
                return m_commandGetVolumeInfo;
            }
            set {
                m_commandGetVolumeInfo = value;
            }
        }

        //=========================================================================================
        // プロパティ：ボリューム情報の取得の出力期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectGetVolumeInfo {
            get {
                return m_expectGetVolumeInfo;
            }
            set {
                m_expectGetVolumeInfo = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルの先頭部分の出力コマンド（head）
        //=========================================================================================
        public string CommandGetFileHead {
            get {
                return m_commandGetFileHead;
            }
            set {
                m_commandGetFileHead = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルの先頭部分の出力期待値の出力期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectGetFileHead {
            get {
                return m_expectGetFileHead;
            }
            set {
                m_expectGetFileHead = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：属性の取得コマンド
        //=========================================================================================
        public string CommandGetFileInfo {
            get {
                return m_commandGetFileInfo;
            }
            set {
                m_commandGetFileInfo = value;
            }
        }

        //=========================================================================================
        // プロパティ：属性の取得の出力期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectGetFileInfo {
            get {
                return m_expectGetFileInfo;
            }
            set {
                m_expectGetFileInfo = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイル時刻の設定のコマンド
        //=========================================================================================
        public string CommandSetFileTime {
            get {
                return m_commandSetFileTime;
            }
            set {
                m_commandSetFileTime = value;
            }
        }

        //=========================================================================================
        // プロパティ：最終更新時刻の設定のコマンド
        //=========================================================================================
        public string CommandSetModifiedTime {
            get {
                return m_commandSetModifiedTime;
            }
            set {
                m_commandSetModifiedTime = value;
            }
        }

        //=========================================================================================
        // プロパティ：最終アクセス時刻の設定のコマンド
        //=========================================================================================
        public string CommandSetAccessedTime {
            get {
                return m_commandSetAccessedTime;
            }
            set {
                m_commandSetAccessedTime = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル時刻の設定の出力期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectSetFileTime {
            get {
                return m_expectSetFileTime;
            }
            set {
                m_expectSetFileTime = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル時刻変更時の時刻指定フォーマット
        //=========================================================================================
        public string ValueTouchTimeFormat {
            get {
                return m_valueTouchTimeFormat;
            }
            set {
                m_valueTouchTimeFormat = value;
            }
        }

        //=========================================================================================
        // プロパティ：パーミッションの変更コマンド
        //=========================================================================================
        public string CommandSetPermissions {
            get {
                return m_commandSetPermissions;
            }
            set {
                m_commandSetPermissions = value;
            }
        }

        //=========================================================================================
        // プロパティ：パーミッションの変更の出力期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectSetPermissions {
            get {
                return m_expectSetPermissions;
            }
            set {
                m_expectSetPermissions = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリの作成コマンド
        //=========================================================================================
        public string CommandMakeDirectory {
            get {
                return m_commandMakeDirectory;
            }
            set {
                m_commandMakeDirectory = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリの作成コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectMakeDirectory {
            get {
                return m_expectMakeDirectory;
            }
            set {
                m_expectMakeDirectory = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリの再帰的削除コマンド
        //=========================================================================================
        public string CommandDeleteDirectoryRecursive {
            get {
                return m_commandDeleteDirectoryRecursive;
            }
            set {
                m_commandDeleteDirectoryRecursive = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリの再帰的削除コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectDeleteDirectoryRecursive {
            get {
                return m_expectDeleteDirectoryRecursive;
            }
            set {
                m_expectDeleteDirectoryRecursive = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリの削除コマンド
        //=========================================================================================
        public string CommandDeleteDirectory {
            get {
                return m_commandDeleteDirectory;
            }
            set {
                m_commandDeleteDirectory = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリの削除コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectDeleteDirectory {
            get {
                return m_expectDeleteDirectory;
            }
            set {
                m_expectDeleteDirectory = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルの削除コマンド
        //=========================================================================================
        public string CommandDeleteFile {
            get {
                return m_commandDeleteFile;
            }
            set {
                m_commandDeleteFile = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルの削除コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectDeleteFile {
            get {
                return m_expectDeleteFile;
            }
            set {
                m_expectDeleteFile = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリを再帰的に削除できない場合のプロンプト
        //=========================================================================================
        public string ValueDeleteDirectoryRecursivePrompt {
            get {
                return m_valueDeleteDirectoryRecursivePrompt;
            }
            set {
                m_valueDeleteDirectoryRecursivePrompt = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリを再帰的に削除できない場合のプロンプト
        //=========================================================================================
        public string ValueDeleteDirectoryRecursiveAnswer {
            get {
                return m_valueDeleteDirectoryRecursiveAnswer;
            }
            set {
                m_valueDeleteDirectoryRecursiveAnswer = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリを削除できない場合のプロンプト
        //=========================================================================================
        public string ValueDeleteDirectoryPrompt {
            get {
                return m_valueDeleteDirectoryPrompt;
            }
            set {
                m_valueDeleteDirectoryPrompt = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリを削除できない場合の応答
        //=========================================================================================
        public string ValueDeleteDirectoryAnswer {
            get {
                return m_valueDeleteDirectoryAnswer;
            }
            set {
                m_valueDeleteDirectoryAnswer = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルを削除できない場合のプロンプト
        //=========================================================================================
        public string ValueDeleteFilePrompt {
            get {
                return m_valueDeleteFilePrompt;
            }
            set {
                m_valueDeleteFilePrompt = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルを削除できない場合の応答
        //=========================================================================================
        public string ValueDeleteFileAnswer {
            get {
                return m_valueDeleteFileAnswer;
            }
            set {
                m_valueDeleteFileAnswer = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルアップロードのコマンド
        //=========================================================================================
        public string CommandUpload {
            get {
                return m_commandUpload;
            }
            set {
                m_commandUpload = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルアップロードの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectUpload {
            get {
                return m_expectUpload;
            }
            set {
                m_expectUpload = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルアップロードのヒアドキュメントプロンプト
        //=========================================================================================
        public string ValueUploadHearDocument {
            get {
                return m_valueUploadHearDocument;
            }
            set {
                m_valueUploadHearDocument = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルアップロードのエンコード種別
        //=========================================================================================
        public ShellUploadEncoding ValueUploadEncoding {
            get {
                return m_valueUploadEncoding;
            }
            set {
                m_valueUploadEncoding = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルアップロードの1行で送信するサイズ
        //=========================================================================================
        public int ValueUploadChunkSize {
            get {
                return m_valueUploadChunkSize;
            }
            set {
                m_valueUploadChunkSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルダウンロードのコマンド
        //=========================================================================================
        public string CommandDownload {
            get {
                return m_commandDownload;
            }
            set {
                m_commandDownload = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルダウンロードの出力期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectDownload {
            get {
                return m_expectDownload;
            }
            set {
                m_expectDownload = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルのチェックサムを計算するコマンド
        //=========================================================================================
        public string CommandComputeChecksum {
            get {
                return m_commandComputeChecksum;
            }
            set {
                m_commandComputeChecksum = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルのチェックサムを計算するコマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectComputeChecksum {
            get {
                return m_expectComputeChecksum;
            }
            set {
                m_expectComputeChecksum = value;
            }
        }

        //=========================================================================================
        // プロパティ：アップロード/ダウンロードの再試行回数
        //=========================================================================================
        public int ValueUploadDownloadRetryCount {
            get {
                return m_valueUploadDownloadRetryCount;
            }
            set {
                m_valueUploadDownloadRetryCount = value;
            }
        }
        //=========================================================================================
        // プロパティ：アップロード/ダウンロードでチェックサムの計算を行うときtrue
        //=========================================================================================
        public bool ValueUploadDownloadUseCheckCksum {
            get {
                return m_valueUploadDownloadUseCheckCksum;
            }
            set {
                m_valueUploadDownloadUseCheckCksum = value;
            }
        }

        //=========================================================================================
        // プロパティ：リネームのコマンド
        //=========================================================================================
        public string CommandRename {
            get {
                return m_commandRename;
            }
            set {
                m_commandRename = value;
            }
        }

        //=========================================================================================
        // プロパティ：リネームの出力期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectRename {
            get {
                return m_expectRename;
            }
            set {
                m_expectRename = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルのオーナーユーザーを変更するコマンド
        //=========================================================================================
        public string CommandChangeOwnerUser {
            get {
                return m_commandChangeOwnerUser;
            }
            set {
                m_commandChangeOwnerUser = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルのオーナーグループを変更するコマンド
        //=========================================================================================
        public string CommandChangeOwnerGroup {
            get {
                return m_commandChangeOwnerGroup;
            }
            set {
                m_commandChangeOwnerGroup = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイルのオーナーユーザーとグループを変更するコマンド
        //=========================================================================================
        public string CommandChangeOwnerUserGroup {
            get {
                return m_commandChangeOwnerUserGroup;
            }
            set {
                m_commandChangeOwnerUserGroup = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルのオーナー変更の期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectChangeOwner {
            get {
                return m_expectChangeOwner;
            }
            set {
                m_expectChangeOwner = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイルをコピーするコマンド（上書き確認なし、ファイルの単純なコピー）
        //=========================================================================================
        public string CommandCopyFile {
            get {
                return m_commandCopyFile;
            }
            set {
                m_commandCopyFile = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイルをコピーするコマンドコマンド（上書き確認なし、ファイルの属性も伴うコピー）
        //=========================================================================================
        public string CommandCopyFileAndAttr {
            get {
                return m_commandCopyFileAndAttr;
            }
            set {
                m_commandCopyFileAndAttr = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイルをコピーするコマンド（上書き確認なし、ディレクトリの単純なコピー）
        //=========================================================================================
        public string CommandCopyDirectory {
            get {
                return m_commandCopyDirectory;
            }
            set {
                m_commandCopyDirectory = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイルをコピーするコマンド（上書き確認なし、ディレクトリの属性も伴うコピー）
        //=========================================================================================
        public string CommandCopyDirectoryAndAttr {
            get {
                return m_commandCopyDirectoryAndAttr;
            }
            set {
                m_commandCopyDirectoryAndAttr = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルコピーの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectCopy {
            get {
                return m_expectCopy;
            }
            set {
                m_expectCopy = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルやディレクトリを移動するコマンド（上書き確認なし、単純な移動）
        //=========================================================================================
        public string CommandMove {
            get {
                return m_commandMove;
            }
            set {
                m_commandMove = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルやディレクトリを移動するコマンド（上書き確認なし、属性も伴う移動）
        //=========================================================================================
        public string CommandMoveAndAttr {
            get {
                return m_commandMoveAndAttr;
            }
            set {
                m_commandMoveAndAttr = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル移動の期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectMove {
            get {
                return m_expectMove;
            }
            set {
                m_expectMove = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルやディレクトリのリンクを作成するコマンド（シンボリックリンク）
        //=========================================================================================
        public string CommandSymboricLink {
            get {
                return m_commandSymboricLink;
            }
            set {
                m_commandSymboricLink = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルやディレクトリのリンクを作成するコマンド（ハードリンク）
        //=========================================================================================
        public string CommandHardLink {
            get {
                return m_commandHardLink;
            }
            set {
                m_commandHardLink = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：リンク作成の期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectLink {
            get {
                return m_expectLink;
            }
            set {
                m_expectLink = value;
            }
        }

        //=========================================================================================
        // プロパティ：操作ユーザーを切り替えるコマンド
        //=========================================================================================
        public string CommandChangeLoginUser {
            get {
                return m_commandChangeLoginUser;
            }
            set {
                m_commandChangeLoginUser = value;
            }
        }

        //=========================================================================================
        // プロパティ：操作ユーザーを切り替えるコマンド（ログインシェルを使う）
        //=========================================================================================
        public string CommandChangeLoginUserShell {
            get {
                return m_commandChangeLoginUserShell;
            }
            set {
                m_commandChangeLoginUserShell = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：操作ユーザーを切り替えるコマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectChangeLoginUser {
            get {
                return m_expectChangeLoginUser;
            }
            set {
                m_expectChangeLoginUser = value;
            }
        }

        //=========================================================================================
        // プロパティ：ルートユーザー名
        //=========================================================================================
        public string ValueRootUserName {
            get {
                return m_valueRootUserName;
            }
            set {
                m_valueRootUserName = value;
            }
        }

        //=========================================================================================
        // プロパティ：カレントユーザー変更のプロンプト
        //=========================================================================================
        public string[] ValueChangeUserPrompt {
            get {
                return m_valueChangeUserPrompt;
            }
            set {
                m_valueChangeUserPrompt = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログインシェルを使って切り替える方をデフォルトにするときtrue
        //=========================================================================================
        public bool ValueChangeUserLoginShell {
            get {
                return m_valueChangeUserLoginShell;
            }
            set {
                m_valueChangeUserLoginShell = value;
            }
        }

        //=========================================================================================
        // プロパティ：プロンプト文字列のユーザー名@サーバー名
        //=========================================================================================
        public string ValuePromptUserServer {
            get {
                return m_valuePromptUserServer;
            }
            set {
                m_valuePromptUserServer = value;
            }
        }

        //=========================================================================================
        // プロパティ：現在の操作ユーザーから抜けるコマンド
        //=========================================================================================
        public string CommandExit {
            get {
                return m_commandExit;
            }
            set {
                m_commandExit = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：現在の操作ユーザーから抜けるコマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectExit {
            get {
                return m_expectExit;
            }
            set {
                m_expectExit = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：カレントディレクトリを取得するコマンド
        //=========================================================================================
        public string CommandGetCurrentDirectory {
            get {
                return m_commandGetCurrentDirectory;
            }
            set {
                m_commandGetCurrentDirectory = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：カレントディレクトリを取得するコマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectGetCurrentDirectory {
            get {
                return m_expectGetCurrentDirectory;
            }
            set {
                m_expectGetCurrentDirectory = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルの結合を行うコマンド（先頭ファイル）
        //=========================================================================================
        public string CommandAppendFileFirst {
            get {
                return m_commandAppendFileFirst;
            }
            set {
                m_commandAppendFileFirst = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイルの結合を行うコマンド（２件目以降のファイル）
        //=========================================================================================
        public string CommandAppendFileNext {
            get {
                return m_commandAppendFileNext;
            }
            set {
                m_commandAppendFileNext = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイルの結合を行うコマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectAppendFile {
            get {
                return m_expectAppendFile;
            }
            set {
                m_expectAppendFile = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイルの分割を行うコマンド
        //=========================================================================================
        public string CommandSplitFile {
            get {
                return m_commandSplitFile;
            }
            set {
                m_commandSplitFile = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイルの分割を行うコマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectSplitFile {
            get {
                return m_expectSplitFile;
            }
            set {
                m_expectSplitFile = value;
            }
        }

        //=========================================================================================
        // プロパティ：シンボリックリンクのリンク先を調べるコマンド（cd後に実行）
        //=========================================================================================
        public string CommandCheckSymbolicLink {
            get {
                return m_commandCheckSymbolicLink;
            }
            set {
                m_commandCheckSymbolicLink = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：シンボリックリンクのリンク先を調べるコマンドの期待値（エラー定義のみ）
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectCheckSymbolicLink {
            get {
                return m_expectCheckSymbolicLink;
            }
            set {
                m_expectCheckSymbolicLink = value;
            }
        }

        //=========================================================================================
        // プロパティ：シンボリックリンクのリンク先の同時取得件数
        //=========================================================================================
        public int ValueLinkTargetSameTimeCount {
            get {
                return m_valueLinkTargetSameTimeCount;
            }
            set {
                m_valueLinkTargetSameTimeCount = value;
            }
        }

        //=========================================================================================
        // プロパティ：フォルダサイズの取得コマンド
        //=========================================================================================
        public string CommandRetrieveFolderSize {
            get {
                return m_commandRetrieveFolderSize;
            }
            set {
                m_commandRetrieveFolderSize = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：フォルダサイズの取得コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectRetrieveFolderSize {
            get {
                return m_expectRetrieveFolderSize;
            }
            set {
                m_expectRetrieveFolderSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：外部コマンド起動時に追加するコマンド
        //=========================================================================================
        public string CommandShellExecutePrev {
            get {
                return m_commandShellExecutePrev;
            }
            set {
                m_commandShellExecutePrev = value;
            }
        }

        //=========================================================================================
        // プロパティ：外部コマンド起動コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectShellExecute {
            get {
                return m_expectShellExecute;
            }
            set {
                m_expectShellExecute = value;
            }
        }

        //=========================================================================================
        // プロパティ：プロセス一覧の取得コマンド
        //=========================================================================================
        public string CommandGetProcessList {
            get {
                return m_commandGetProcessList;
            }
            set {
                m_commandGetProcessList = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：プロセス一覧の取得コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectGetProcessList {
            get {
                return m_expectGetProcessList;
            }
            set {
                m_expectGetProcessList = value;
            }
        }

        //=========================================================================================
        // プロパティ：ネットワーク状況一覧のコマンド
        //=========================================================================================
        public string CommandNetStat {
            get {
                return m_commandNetStat;
            }
            set {
                m_commandNetStat = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：プロセス一覧の取得コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectNetStat {
            get {
                return m_expectNetStat;
            }
            set {
                m_expectNetStat = value;
            }
        }

        //=========================================================================================
        // プロパティ：プロセスの終了コマンド
        //=========================================================================================
        public string CommandKillProcess {
            get {
                return m_commandKillProcess;
            }
            set {
                m_commandKillProcess = value;
            }
        }

        //=========================================================================================
        // プロパティ：プロセスの強制終了コマンド
        //=========================================================================================
        public string CommandKillProcessForce {
            get {
                return m_commandKillProcessForce;
            }
            set {
                m_commandKillProcessForce = value;
            }
        }

        //=========================================================================================
        // プロパティ：プロセスの強制終了コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectKillProcess {
            get {
                return m_expectKillProcess;
            }
            set {
                m_expectKillProcess = value;
            }
        }

        //=========================================================================================
        // プロパティ：zipの作成コマンド（書庫の時刻を格納ファイルの最新日時に合わせる）
        //=========================================================================================
        public string CommandArchiveZipTime {
            get {
                return m_commandArchiveZipTime;
            }
            set {
                m_commandArchiveZipTime = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：zipの作成コマンド（書庫の時刻は指定なし）
        //=========================================================================================
        public string CommandArchiveZip {
            get {
                return m_commandArchiveZip;
            }
            set {
                m_commandArchiveZip = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：zip作成コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectArchiveZip {
            get {
                return m_expectArchiveZip;
            }
            set {
                m_expectArchiveZip = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：tar.gzの作成コマンド
        //=========================================================================================
        public string CommandArchiveTarGz {
            get {
                return m_commandArchiveTarGz;
            }
            set {
                m_commandArchiveTarGz = value;
            }
        }

        //=========================================================================================
        // プロパティ：tar.gzの作成コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectArchiveTarGz {
            get {
                return m_expectArchiveTarGz;
            }
            set {
                m_expectArchiveTarGz = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：tar.bz2の作成コマンド
        //=========================================================================================
        public string CommandArchiveTarBz2 {
            get {
                return m_commandArchiveTarBz2;
            }
            set {
                m_commandArchiveTarBz2 = value;
            }
        }

        //=========================================================================================
        // プロパティ：tar.bz2の作成コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectArchiveTarBz2 {
            get {
                return m_expectArchiveTarBz2;
            }
            set {
                m_expectArchiveTarBz2 = value;
            }
        }

        //=========================================================================================
        // プロパティ：tarの作成コマンド
        //=========================================================================================
        public string CommandArchiveTar {
            get {
                return m_commandArchiveTar;
            }
            set {
                m_commandArchiveTar = value;
            }
        }

        //=========================================================================================
        // プロパティ：tarの作成コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectArchiveTar {
            get {
                return m_expectArchiveTar;
            }
            set {
                m_expectArchiveTar = value;
            }
        }

        //=========================================================================================
        // プロパティ：アーカイブの実行コマンド
        //=========================================================================================
        public string CommandArchiveExecute {
            get {
                return m_commandArchiveExecute;
            }
            set {
                m_commandArchiveExecute = value;
            }
        }

        //=========================================================================================
        // プロパティ：アーカイブの実行コマンドの期待値
        //=========================================================================================
        public List<OSSpecLineExpect> ExpectArchiveExecute {
            get {
                return m_expectArchiveExecute;
            }
            set {
                m_expectArchiveExecute = value;
            }
        }

        //=========================================================================================
        // プロパティ：zipの圧縮レベルの指定オプション
        //=========================================================================================
        public string ValueArchiveZipCompressionOption {
            get {
                return m_valueArchiveZipCompressionOption;
            }
            set {
                m_valueArchiveZipCompressionOption = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：zipの圧縮レベルのデフォルト
        //=========================================================================================
        public int ValueArchiveZipComplessionDefault {
            get {
                return m_valueArchiveZipComplessionDefault;
            }
            set {
                m_valueArchiveZipComplessionDefault = value;
            }
        }

        //=========================================================================================
        // プロパティ：標準出力の結果解析の際、CR LF→LFの変換が必要なときtrue
        //=========================================================================================
        public bool ResultCrLfConvert {
            get {
                return true;
            }
        }
    }
}
