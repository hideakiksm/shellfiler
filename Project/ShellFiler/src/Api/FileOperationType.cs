using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：ファイル操作の種類
    //=========================================================================================
    public class FileOperationType {
        // 登録済み操作種別の一覧
        private static List<FileOperationType> s_typeList = new List<FileOperationType>();

        // 定数定義
        public static readonly FileOperationType CopyFile      = new FileOperationType(Resources.LogOperation_Copy);             // ファイルコピー
        public static readonly FileOperationType CopyDir       = new FileOperationType(Resources.LogOperation_CopyDir);          // ディレクトリをコピー
        public static readonly FileOperationType CopyAttr      = new FileOperationType(Resources.LogOperation_CopyAttr);         // 属性をコピー
        public static readonly FileOperationType DuplicateFile = new FileOperationType(Resources.LogOperation_Duplicate);        // ファイル２重化
        public static readonly FileOperationType MakeDir       = new FileOperationType(Resources.LogOperation_MakeDir);          // ディレクトリ作成
        public static readonly FileOperationType DeleteFile    = new FileOperationType(Resources.LogOperation_DeleteFile);       // ファイル削除
        public static readonly FileOperationType DeleteDir     = new FileOperationType(Resources.LogOperation_DeleteDir);        // ディレクトリ削除
        public static readonly FileOperationType MoveFile      = new FileOperationType(Resources.LogOperation_MoveFile);         // ファイル移動
        public static readonly FileOperationType MoveDir       = new FileOperationType(Resources.LogOperation_MoveDir);          // ディレクトリ移動
        public static readonly FileOperationType RenameDir     = new FileOperationType(Resources.LogOperation_RenameDir);        // ディレクトリ名を変更
        public static readonly FileOperationType RenameFile    = new FileOperationType(Resources.LogOperation_RenameFile);       // ファイル名を変更
        public static readonly FileOperationType Download      = new FileOperationType(Resources.LogOperation_Download);         // ダウンロード
        public static readonly FileOperationType Upload        = new FileOperationType(Resources.LogOperation_Upload);           // アップロード
        public static readonly FileOperationType Shortcut      = new FileOperationType(Resources.LogOperation_Shortcut);         // ショートカットの作成
        public static readonly FileOperationType OpenArc       = new FileOperationType(Resources.LogOperation_OpenArc);          // アーカイブのオープン
        public static readonly FileOperationType CreateArc     = new FileOperationType(Resources.LogOperation_CreateArc);        // アーカイブの作成
        public static readonly FileOperationType FixArc        = new FileOperationType(Resources.LogOperation_FixArc);           // アーカイブの固定
        public static readonly FileOperationType ExtractFile   = new FileOperationType(Resources.LogOperation_ExtractFile);      // ファイルの展開
        public static readonly FileOperationType ExtractDir    = new FileOperationType(Resources.LogOperation_ExtractDir);       // ディレクトリの展開
        public static readonly FileOperationType FolderSize    = new FileOperationType(Resources.LogOperation_FolderSize);       // フォルダサイズの取得
        public static readonly FileOperationType CombineFile   = new FileOperationType(Resources.LogOperation_CombineFile);      // ファイルの結合
        public static readonly FileOperationType SplitFile     = new FileOperationType(Resources.LogOperation_SplitFile);        // ファイルの分割
        public static readonly FileOperationType AddFile       = new FileOperationType(Resources.LogOperation_AddFile);          // ファイルの追加

        // ログ出力用文字列
        private String m_logString;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]logString   ログ出力用文字列
        // 戻り値：なし
        //=========================================================================================
        private FileOperationType(String logString) {
            this.m_logString = logString;
            s_typeList.Add(this);
        }

        //=========================================================================================
        // プロパティ：ログ出力用文字列
        //=========================================================================================
        public string LogString {
            get {
                return this.m_logString;
            }
        }

        //=========================================================================================
        // プロパティ：登録済み操作種別の一覧
        //=========================================================================================
        public static List<FileOperationType> TypeList {
            get {
                return s_typeList;
            }
        }
    }
}
