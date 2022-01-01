using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Shell;
using ShellFiler.UI;
using ShellFiler.Util;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {
    
    //=========================================================================================
    // クラス：ファイル名一覧を取得するエミュレータエンジン
    //=========================================================================================
    public class ShellEngineGetFileList : IShellEmulatorEngine {
        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        // 一覧を取得するディレクトリ
        private string m_directory;

        // 行内解析の共通処理
        private ResultLineParser m_resultLineParser;

        // 一覧の取得結果
        private List<IFile> m_resultFileList = new List<IFile>();

        // ファイルの取得順序
        private int m_defaultOrder = 1;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent     所有オブジェクト
        // 　　　　[in]connection SSHの接続
        // 　　　　[in]directory  一覧を取得するディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineGetFileList(ShellCommandEmulator parent, SSHConnection connection, string directory) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            m_directory = directory;
            m_resultLineParser = new ResultLineParser(m_connection, this, m_shellCommandEmulator, m_connection.ShellCommandDictionary.ExpectGetFileList);
        }

        //=========================================================================================
        // 機　能：エンジンの処理を閉じる
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンド処理スレッドで実行される
        //=========================================================================================
        public void CloseEngine() {
        }

        //=========================================================================================
        // 機　能：コマンドラインに送信する文字列を取得する
        // 引　数：なし
        // 戻り値：送信するコマンドライン
        // メ　モ：バックグラウンド処理スレッドで実行される
        //=========================================================================================
        public string GetRequest() {
            ShellCommandDictionary dictionary = m_connection.ShellCommandDictionary;
            string strCommand = dictionary.GetCommandGetFileList(m_directory);
            return strCommand;
        }

        //=========================================================================================
        // 機　能：SSHからのデータが受信できたときの処理を行う
        // 引　数：[in]buffer   受信したデータのバッファ
        //         [in]remain   受信データを仮想画面に転送する内容（エラーのときnull）
        // 戻り値：エラーコード
        // メ　モ：ストリームスレッドで実行される
        //=========================================================================================
        public ShellEngineError OnReceive(byte[] buffer, out byte[] remain) {
            ShellEngineError error;
            List<ResultLineParser.ParsedLine> parsedLine;
            bool completed;
            error = m_resultLineParser.OnReceive(buffer, out parsedLine, out completed, out remain);
            if (error.IsFailed) {
                return error;
            }
            for (int i = 0; i < parsedLine.Count; i++) {
                ResultLineParser.ParsedLine line = parsedLine[i];
                ShellFile fileInfo;
                bool success = RetrieveFileInfo(line.LineString, line.ParsedToken, line.LineType, out fileInfo);
                if (!success) {
                    return ShellEngineError.Fail(FileOperationStatus.Fail, Resources.ShellError_LineParse_OSSPEC, line.LineString);
                }
                if (fileInfo == null) {
                    ;
                } else if (fileInfo.FileName == "..") {
                    fileInfo.DefaultOrder = 0;
                    m_resultFileList.Insert(0, fileInfo);
                } else {
                    fileInfo.DefaultOrder = m_defaultOrder++;
                    m_resultFileList.Add(fileInfo);
                }

            }
            if (completed) {
                m_shellCommandEmulator.RemoveEngine(FileOperationStatus.Success);
            }
            return ShellEngineError.Success();
        }

        //=========================================================================================
        // 機　能：SSHからの応答データからファイル情報1件分を取得する
        // 引　数：[in]line            解析対象の行
        // 　　　　[in]parseResult     解析の結果抽出したデータ
        // 　　　　[in]resultLineType  結果行の種類
        // 　　　　[out]fileInfo       取得したファイル情報を返す変数（スキップしたときnull）
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        public static bool RetrieveFileInfo(string line, List<ResultLineParser.ParsedToken> parseResult, OSSpecLineType resultLineType, out ShellFile fileInfo) {
            fileInfo = null;

            // 無意味な行なら次へ
            if ((resultLineType & OSSpecLineType.Comment) != 0) {
                return true;
            }

            // 内容を取得
            string fileName = null;
            int hardLink = -1;
            FileAttribute attribute = null;
            int permissions = -1;
            long fileSize = -1;
            DateTime modifiedDate = DateTime.MinValue;
            DateTime accessedDate = DateTime.MinValue;
            string owner = null;
            string group = null;
            string lnPath = null;

            for (int i = 0; i < parseResult.Count; i++) {
                OSSpecTokenType token = parseResult[i].TokenType;
                object resultValue = parseResult[i].ParsedData;
                if (token == OSSpecTokenType.LsFileName) {
                    fileName = (string)resultValue;
                } else if (token == OSSpecTokenType.LsAttr) {
                    FileAttributeLinux val = (FileAttributeLinux)resultValue;
                    attribute = val.FileAttribute;
                    permissions = val.Permissions;
                } else if (token == OSSpecTokenType.LsSize) {
                    fileSize = (long)resultValue;
                } else if (token == OSSpecTokenType.LsUpdTimeFull) {
                    modifiedDate = (DateTime)resultValue;
                } else if (token == OSSpecTokenType.LsAccTimeFull) {
                    accessedDate = (DateTime)resultValue;
                } else if (token == OSSpecTokenType.LsOwner) {
                    owner = (string)resultValue;
                } else if (token == OSSpecTokenType.LsGroup) {
                    group = (string)resultValue;
                } else if (token == OSSpecTokenType.LsLink) {
                    hardLink = (int)resultValue;
                } else if (token == OSSpecTokenType.LsLnPath) {
                    lnPath = (string)resultValue;
                }
            }

            // 内容の確認/調整
            if (fileName == null || attribute == null || fileSize == -1 || modifiedDate == DateTime.MinValue ||
                    owner == null || group == null) {
                Program.Abort("属性取得の制御エラーです。fileName={0}, attribute={1}, fileSize={2}, modifiedDate={3}, owner={4}, group={5}, hardLink={6}, line={7}",
                              fileName, attribute, fileSize, modifiedDate, owner, group, hardLink, line);
            }
            if (fileName.EndsWith("/")) {
                fileName = fileName.Substring(0, fileName.Length - 1);
            }
            if (fileName == ".") {
                return true;
            }
            fileName = GenericFileStringUtils.GetFileName(fileName, '/');
            if (fileName.StartsWith(".")) {
                attribute.IsHidden = true;
            }

            fileInfo = new ShellFile(fileName, attribute, permissions, fileSize, modifiedDate, owner, group, hardLink);
            if (accessedDate != DateTime.MinValue) {
                fileInfo.AccessDate = accessedDate;
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：一覧の取得結果
        //=========================================================================================
        public List<IFile> ResultFileList {
            get {
                return m_resultFileList;
            }
        }
    }
}
