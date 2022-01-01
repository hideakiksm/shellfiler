using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Security;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：ファイル操作の結果として出力する文字列の種類
    //=========================================================================================
    public class FileOperationStatus {
        // 登録済みステータスの一覧
        private static List<FileOperationStatus> s_statusList = new List<FileOperationStatus>();

        // 定数定義
        public static readonly FileOperationStatus Null              = new FileOperationStatus(LogLevel.Null,  StatusType.NoCount, "");                                      // メッセージなし
        public static readonly FileOperationStatus Processing        = new FileOperationStatus(LogLevel.Null,  StatusType.NoCount, "");                                      // 処理中のメッセージなし
        public static readonly FileOperationStatus ArcReading        = new FileOperationStatus(LogLevel.Null,  StatusType.NoCount, Resources.LogSts_ArcReading);             // アーカイブ読み込み中
        public static readonly FileOperationStatus Success           = new FileOperationStatus(LogLevel.Info,  StatusType.Success, Resources.LogSts_Success);                // 成功しました
        public static readonly FileOperationStatus SuccessCopy       = new FileOperationStatus(LogLevel.Info,  StatusType.Success, Resources.LogSts_SuccessCopy);            // コピーしました
        public static readonly FileOperationStatus SuccessMove       = new FileOperationStatus(LogLevel.Info,  StatusType.Success, Resources.LogSts_SuccessMove);            // 移動しました
        public static readonly FileOperationStatus SuccessDelete     = new FileOperationStatus(LogLevel.Info,  StatusType.Success, Resources.LogSts_SuccessDelete);          // 削除しました
        public static readonly FileOperationStatus SuccessMkDir      = new FileOperationStatus(LogLevel.Info,  StatusType.Success, Resources.LogSts_SuccessMkDir);           // ディレクトリを作成
        public static readonly FileOperationStatus SuccessDelDir     = new FileOperationStatus(LogLevel.Info,  StatusType.Success, Resources.LogSts_SuccessDelDir);          // ディレクトリを削除
        public static readonly FileOperationStatus SuccessDownload   = new FileOperationStatus(LogLevel.Info,  StatusType.Success, Resources.LogSts_SuccessDownload);        // ダウンロードしました
        public static readonly FileOperationStatus SuccessUpload     = new FileOperationStatus(LogLevel.Info,  StatusType.Success, Resources.LogSts_SuccessUpload);          // アップロードしました
        public static readonly FileOperationStatus MoveRetry         = new FileOperationStatus(LogLevel.Info,  StatusType.NoCount, Resources.LogSts_MoveRetry);              // コピーと削除で移動
        public static readonly FileOperationStatus CopyRetry         = new FileOperationStatus(LogLevel.Info,  StatusType.NoCount, Resources.LogSts_CopyRetry);              // 配下を個別にコピー
        public static readonly FileOperationStatus NoCopy            = new FileOperationStatus(LogLevel.Info,  StatusType.Skip,    Resources.LogSts_NoCopy);                 // コピーしません
        public static readonly FileOperationStatus NoMove            = new FileOperationStatus(LogLevel.Info,  StatusType.Skip,    Resources.LogSts_NoMove);                 // 移動しません
        public static readonly FileOperationStatus NoDel             = new FileOperationStatus(LogLevel.Info,  StatusType.Skip,    Resources.LogSts_NoDel);                  // 削除しません
        public static readonly FileOperationStatus NoNew             = new FileOperationStatus(LogLevel.Info,  StatusType.Skip,    Resources.LogSts_NoNew);                  // 転送先が最新
        public static readonly FileOperationStatus NoShortcut        = new FileOperationStatus(LogLevel.Info,  StatusType.Skip,    Resources.LogSts_NoShortcut);             // 作成しません
        public static readonly FileOperationStatus NoExtract         = new FileOperationStatus(LogLevel.Info,  StatusType.Skip,    Resources.LogSts_NoExtract);              // 展開しません
        public static readonly FileOperationStatus AlrDir            = new FileOperationStatus(LogLevel.Info,  StatusType.Skip,    Resources.LogSts_AlrDir);                 // ディレクトリ作成済み
        public static readonly FileOperationStatus AlreadyExists     = new FileOperationStatus(LogLevel.Info,  StatusType.Skip,    Resources.LogSts_AlreadyExists);          // すでに存在します
        public static readonly FileOperationStatus Canceled          = new FileOperationStatus(LogLevel.Info,  StatusType.Skip,    Resources.LogSts_Canceled);               // キャンセルしました
        public static readonly FileOperationStatus Skip              = new FileOperationStatus(LogLevel.Info,  StatusType.Skip,    Resources.LogSts_Skipped);                // スキップしました
        public static readonly FileOperationStatus SkippedFilter     = new FileOperationStatus(LogLevel.Info,  StatusType.Skip,    Resources.LogSts_SkippedFilter);          // 対象外のためスキップ
        public static readonly FileOperationStatus SkippedCondition  = new FileOperationStatus(LogLevel.Info,  StatusType.Skip,    Resources.LogSts_SkippedCondition);       // 条件不一致でスキップ
        public static readonly FileOperationStatus Fail              = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_Failed);                 // すでに存在します
        public static readonly FileOperationStatus FailedAlr         = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FailedAlr);              // 失敗しました
        public static readonly FileOperationStatus FailLinkTarget    = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FailLinkTarget);         // リンク切れです
        public static readonly FileOperationStatus SrcDest           = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_SrcDest);                // 転送関係が異常です
        public static readonly FileOperationStatus ErrorGetAttr      = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorGetAttr);           // 属性を取得できません
        public static readonly FileOperationStatus ErrorSetAttr      = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorSetAttr);           // 属性を設定できません
        public static readonly FileOperationStatus ErrorSetTime      = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorSetTime);           // 時刻を設定できません
        public static readonly FileOperationStatus ErrorRename       = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorRename);            // 名前を変更できません
        public static readonly FileOperationStatus ErrorFileOpen     = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorFileOpen);          // ファイルオープン失敗
        public static readonly FileOperationStatus ErrorTooComplex   = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorTooComplex);        // 名前が複雑すぎます
        public static readonly FileOperationStatus ErrorTooLarge     = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorTooLarge);          // ファイルが大きすぎます
        public static readonly FileOperationStatus ErrorOpen         = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorOpen);              // ファイルを開けません
        public static readonly FileOperationStatus ErrorDelete       = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorDelete);            // 削除できません
        public static readonly FileOperationStatus ErrorChmod        = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorChmod);             // オーナー変更できません
        public static readonly FileOperationStatus DiskFull          = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_DiskFull);               // ディスクフルです
        public static readonly FileOperationStatus ErrorMkDir        = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorMkDir);             // ディレクトリ作成失敗
        public static readonly FileOperationStatus FileNotFound      = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FileNotFound);           // ファイルがありません
        public static readonly FileOperationStatus NotSupport        = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_NotSupport);             // サポート外の操作です
        public static readonly FileOperationStatus CanNotAccess      = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_CanNotAccess);           // アクセスできません
        public static readonly FileOperationStatus SharingViolation  = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_SharingViolation);       // 共有違反です
        public static readonly FileOperationStatus FailedConnect     = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ConnectServer);          // サーバへの接続に失敗
        public static readonly FileOperationStatus ArcUnknown        = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ArcUnknown);             // 未対応の圧縮形式
        public static readonly FileOperationStatus ArcCrcError       = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ArcCrcError);            // CRCエラーです
        public static readonly FileOperationStatus ArcDataError      = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ArcDataError);           // データエラーです
        public static readonly FileOperationStatus ArcBadPassword    = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ArcBadPassword);         // パスワードが不一致
        public static readonly FileOperationStatus ErrorConvertImage = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorConvertImage);      // 画像の変換不可
        public static readonly FileOperationStatus ErrorOutOfMemory  = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorOutOfMemory);       // メモリ不足
        public static readonly FileOperationStatus ErrorFilterCode   = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ErrorFilterCode);        // フィルターでエラー
        public static readonly FileOperationStatus FilterUnknownChar = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FilterUnknownChar);      // 文字のコード変換不可
        public static readonly FileOperationStatus TooBigFile        = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_TooBigFile);             // ファイルが大きすぎます
        public static readonly FileOperationStatus FailProcessStart  = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FailProcessStart);       // プロセス起動に失敗
        public static readonly FileOperationStatus FailProcessError  = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FailProcessError);       // 外部プロセスでエラー
        public static readonly FileOperationStatus FailWriteTemp     = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FailWriteTemp);          // 作業ファイル出力失敗
        public static readonly FileOperationStatus FailReadTemp      = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FailReadTemp);           // 作業ファイル入力失敗
        public static readonly FileOperationStatus FilterUnexpected  = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FilterUnexpected);       // 想定外のデータ形式
        public static readonly FileOperationStatus FailSetting       = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FailSetting);            // 設定が不適切
        public static readonly FileOperationStatus VirtualMkDir      = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_VirtualMkDir);           // 作業領域の作成失敗
        public static readonly FileOperationStatus VirtualArcOpen    = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_VirtualArcOpen);         // アーカイブを開けません
        public static readonly FileOperationStatus FailFormat        = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FailFormat);             // 想定外の結果形式
        public static readonly FileOperationStatus FailedChangeUser  = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FailedChangeUser);       // ユーザー切り替えに失敗
        public static readonly FileOperationStatus FailedExit        = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FailedExit);             // exitに失敗
        public static readonly FileOperationStatus ShellDiffCrc      = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_ShellDiffCrc);           // CRC不一致
        public static readonly FileOperationStatus FailRead          = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FailRead);               // 読み込めません
        public static readonly FileOperationStatus FailDeleteTemp    = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_FailDeleteTemp);         // 後処理に失敗
        public static readonly FileOperationStatus TooManyFiles      = new FileOperationStatus(LogLevel.Error, StatusType.Fail,    Resources.LogSts_TooManyFiles);           // ファイル数が多すぎます

        // エラーの重要度
        private LogLevel m_level;

        // ステータスの種類
        private StatusType m_statusType;

        // ステータスを表すメッセージ
        private string m_message;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]level      エラーの重要度
        // 　　　　[in]statusType ステータスの種類
        // 　　　　[in]message    ステータスを表すメッセージ
        // 戻り値：なし
        //=========================================================================================
        private FileOperationStatus(LogLevel level, StatusType statusType, String message) {
            m_level = level;
            m_statusType = statusType;
            m_message = message;
            s_statusList.Add(this);
        }

        //=========================================================================================
        // 機　能：例外からステータスを初期化する
        // 引　数：[in]exception      発生した例外
        // 　　　　[in]defaultStatus  デフォルトのステータス
        // 戻り値：なし
        //=========================================================================================
        public static FileOperationStatus FromException(Exception exception, FileOperationStatus defaultStatus) {
            FileOperationStatus status;
            if (exception is SecurityException) {
                status = FileOperationStatus.CanNotAccess;
            } else {
                status = defaultStatus;
            }
            return status;
        }

        //=========================================================================================
        // プロパティ：エラーの重要度
        //=========================================================================================
        public LogLevel Level {
            get {
                return m_level;
            }
        }

        //=========================================================================================
        // プロパティ：エラーの重要度
        //=========================================================================================
        public StatusType Type {
            get {
                return m_statusType;
            }
        }

        //=========================================================================================
        // プロパティ：ステータスを表すメッセージ
        //=========================================================================================
        public string Message {
            get {
                return m_message;
            }
        }

        //=========================================================================================
        // プロパティ：操作が成功したときtrue
        //=========================================================================================
        public bool Succeeded {
            get {
                if (this == FileOperationStatus.Canceled) {
                    return false;
                }
                return (m_level == LogLevel.Info);
            }
        }

        //=========================================================================================
        // プロパティ：スキップしたときtrue
        //=========================================================================================
        public bool Skipped {
            get {
                if (this == FileOperationStatus.Canceled) {
                    return true;
                }
                return (m_statusType == StatusType.Skip);
            }
        }

        //=========================================================================================
        // プロパティ：操作が失敗したときtrue
        //=========================================================================================
        public bool Failed {
            get {
                return (m_level == LogLevel.Error);
            }
        }
                
        //=========================================================================================
        // プロパティ：登録済みステータスの一覧
        //=========================================================================================
        public static List<FileOperationStatus> StatusList {
            get {
                return s_statusList;
            }
        }

        //=========================================================================================
        // 列挙子：ログレベル
        //=========================================================================================
        public enum LogLevel {
            // その他
            Null,
            // 情報レベル
            Info,
            // エラーレベル
            Error,
        };

        //=========================================================================================
        // 列挙子：ステータスの種類
        //=========================================================================================
        public enum StatusType {
            // 未分類
            NoCount,
            // 成功
            Success,
            // スキップ
            Skip,
            // 失敗
            Fail,
        };
    }
}
