using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.Command;
using ShellFiler.Command.FileViewer;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;

namespace ShellFiler.MonitoringViewer {

    //=========================================================================================
    // クラス：一覧コマンドを再試行するための情報
    //=========================================================================================
    public class CommandRetryInfo {
        // コマンドラインの生成クラス
        private ShellCommandDictionary m_commandDictionary;

        // 対象パスのファイルシステムのID
        private FileSystemID m_fileSystemId;

        // ファイル一覧のコンテキスト情報
        private IFileListContext m_fileListContext;

        // 対象パスのフォルダ名
        private string m_targetPathName;

        // データの解析ルール
        private MonitoringViewerMode m_parseType;

        // データ解析時の文字列エンコード
        private Encoding m_encoding;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]commandDic      コマンドラインの生成クラス
        // 　　　　[in]fileSystemId    対象パスのファイルシステムのID
        // 　　　　[in]fileListContext ファイル一覧のコンテキスト情報
        // 　　　　[in]targetPathName  対象パスのフォルダ名
        // 　　　　[in]parseType       データの解析ルール
        // 　　　　[in]encoding        データ解析時の文字列エンコード
        // 戻り値：なし
        //=========================================================================================
        public CommandRetryInfo(ShellCommandDictionary commandDic, FileSystemID fileSystemId, IFileListContext fileListContext, string targetPathName, MonitoringViewerMode parseType, Encoding encoding) {
            m_commandDictionary = commandDic;
            m_fileSystemId = fileSystemId;
            m_fileListContext = fileListContext;
            m_targetPathName = targetPathName;
            m_parseType = parseType;
            m_encoding = encoding;
        }

        //=========================================================================================
        // プロパティ：コマンドラインの生成クラス
        //=========================================================================================
        public ShellCommandDictionary CommandDictionary {
            get {
                return m_commandDictionary;
            }
        }

        //=========================================================================================
        // プロパティ：対象パスのファイルシステムのID
        //=========================================================================================
        public FileSystemID FileSystemId {
            get {
                return m_fileSystemId;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧のコンテキスト情報
        //=========================================================================================
        public IFileListContext FileListContext {
            get {
                return m_fileListContext;
            }
        }

        //=========================================================================================
        // プロパティ：対象パスのフォルダ名
        //=========================================================================================
        public string TargetPathName {
            get {
                return m_targetPathName;
            }
        }

        //=========================================================================================
        // プロパティ：データの解析ルール
        //=========================================================================================
        public MonitoringViewerMode ParseType {
            get {
                return m_parseType;
            }
        }

        //=========================================================================================
        // プロパティ：データ解析時の文字列エンコード
        //=========================================================================================
        public Encoding Encoding {
            get {
                return m_encoding;
            }
        }
    }
}
