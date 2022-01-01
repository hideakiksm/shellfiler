using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;

namespace ShellFiler.FileSystem.Shell {

    //=========================================================================================
    // クラス：Shellファイルシステムでのファイル一覧のコンテキスト情報
    //=========================================================================================
    class ShellFileListContext : IFileListContext {
        // shellのコマンドを組み立てるクラス（初期化完了まではnull）
        private ShellCommandDictionary m_shellCommandDictionary = null;

        // エンコード方式（初期化完了まではnull）
        private Encoding m_encoding = null;

        // 使用するチャネルのID
        private TerminalShellChannelID m_terminalShellChannelId;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]commandDic  shellのコマンドを組み立てるクラス
        // 　　　　[in]encoding    エンコード方式
        // 　　　　[in]id          使用するチャネルのID
        // 戻り値：なし
        //=========================================================================================
        public ShellFileListContext(ShellCommandDictionary commandDic, Encoding encoding, TerminalShellChannelID id) {
            m_shellCommandDictionary = commandDic;
            m_encoding = encoding;
            m_terminalShellChannelId = id;
        }

        //=========================================================================================
        // 機　能：タブの複製時にクローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public IFileListContext CloneForTabCopy() {
            ShellFileListContext context = new ShellFileListContext(m_shellCommandDictionary, m_encoding, m_terminalShellChannelId);
            return context;
        }

        //=========================================================================================
        // 機　能：バックグラウンドタスク実行用にクローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public IFileListContext CloneForBackgroundTask() {
            ShellFileListContext context = new ShellFileListContext(m_shellCommandDictionary, m_encoding, m_terminalShellChannelId);
            return context;
        }

        //=========================================================================================
        // 機　能：実行時のファイルを取得する
        // 引　数：[in]path  元のファイル
        // 戻り値：仮想フォルダ等を変換した後のファイル
        //=========================================================================================
        public string GetExecuteLocalPath(string path) {
            return path;
        }

        //=========================================================================================
        // 機　能：実行時のファイル一覧を取得する
        // 引　数：[in]pathList  元のファイル一覧
        // 戻り値：仮想フォルダ等を変換した後のファイル一覧
        //=========================================================================================
        public List<string> GetExecuteLocalPathList(List<string> pathList) {
            return pathList;
        }

        //=========================================================================================
        // プロパティ：shellのコマンドを組み立てるクラス
        //=========================================================================================
        public ShellCommandDictionary ShellCommandDictionary {
            get {
                return m_shellCommandDictionary;
            }
        }

        //=========================================================================================
        // プロパティ：エンコード方式
        //=========================================================================================
        public Encoding Encoding {
            get {
                return m_encoding;
            }
        }

        //=========================================================================================
        // プロパティ：使用するチャネルのID
        //=========================================================================================
        public TerminalShellChannelID TerminalShellChannelId {
            get {
                return m_terminalShellChannelId;
            }
            set {
                m_terminalShellChannelId = value;
            }
        }
    }
}
