using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileTask.DataObject {

    //=========================================================================================
    // クラス：ファイルシステムからの読み込み用のデータソース
    //=========================================================================================
    public class AccessibleFile : IRetrieveFileDataTarget, IFileViewerDataSource {
        // 読み込み中でこのサイズ以下の場合はチャンクが小さすぎるため待機する
        public const int MIN_PARSE_START_SIZE = 1024;

        // 読み込みバッファのデフォルトサイズ
        private const int BUFFER_DEFAULT_SIZE = 32768;

        // 対象ファイルのパス（ファイルではないとき、カレントパスにある代表ファイル名）
        private string m_filePath;
        
        // 対象ファイルがあるファイルシステム（None:ファイルシステム上にない）
        private FileSystemID m_targetFileSystem;

        // このファイルの表示名（null:ファイル名を使用）
        private string m_displayName;

        // 保存を推奨するときtrue
        private bool m_saveRequired;

        // デフォルトのタブ幅（-1:ファイル名から自動）
        private int m_defaultTab;

        // 最大ファイルサイズ
        private int m_maxFileSize;

        // アクセス可能かどうかのステータス
        private RetrieveDataLoadStatus m_status;

        // エラー情報
        private string m_errorInfo;

        // 読み込んだファイル内容のバッファ
        private byte[] m_buffer;

        // 読み込んだサイズ
        private int m_loadSize;

        // 高速読み込みするときtrue（スライドショー用）
        private bool m_fastRead;

        // 読み込み中でこのサイズ以下の場合はチャンクが小さすぎるため待機するサイズ
        private int m_minParseStartSize;

        // デフォルトとして使用するエンコーディング（null:不明のため先頭部分を解析）
        private EncodingType m_defaultEncoding;

        // 読み込み用バックグラウンドタスクのID（終了したときBackgroundTaskId.NullId）
        private BackgroundTaskID m_loadingTaskId = BackgroundTaskID.NullId;

        // データ状態に変化があったことを知らせるイベント
        public event AccessibleFileStatusChangedEventHandler StatusChangedEvent;

        // ステータスが変わったときtrue
        private bool m_statusChanged = false;

        // チャンクが読み込まれたときtrue
        private bool m_chunkLoaded = false;

        // 読み込み時間のターンアラウンドタイム（未設定のとき-1）
        private int m_loadingTurnAroundTime = -1;

        // 読み込み時間のレスポンスタイム（未設定のとき-1）
        private int m_loadingResponseTime = -1;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]filePath     対象ファイルのパス（ファイルではないとき、カレントパスにある代表ファイル名）
        // 　　　　[in]fileSystem   対象ファイルがあるファイルシステム
        // 　　　　[in]dispName     このファイルの表示名（null:ファイル名を使用）
        // 　　　　[in]saveRequired 保存を推奨するときtrue
        // 　　　　[in]defaultTab   デフォルトのタブ幅（-1:ファイル名から自動）
        // 　　　　[in]maxSize      最大ファイルサイズ
        // 　　　　[in]fastRead     高速読み込みするときtrue（スライドショー用）
        // 　　　　[in]minParse     読み込み中でこのサイズ以下の場合はチャンクが小さすぎるため待機するサイズ
        // 　　　　[in]defaultEnc   デフォルトとして使用するエンコーディング（null:不明のため先頭部分を解析）
        // 戻り値：なし
        //=========================================================================================
        public AccessibleFile(string filePath, FileSystemID fileSystem, string dispName, bool saveRequired, int defaultTab, int maxSize, bool fastRead, int minParse, EncodingType defaultEnc) {
            m_filePath = filePath;
            m_targetFileSystem = fileSystem;
            m_displayName = dispName;
            m_saveRequired = saveRequired;
            m_defaultTab = defaultTab;
            m_maxFileSize = maxSize;
            m_status = RetrieveDataLoadStatus.Loading;
            m_errorInfo = null;
            m_buffer = new byte[BUFFER_DEFAULT_SIZE];
            m_loadSize = 0;
            m_fastRead = fastRead;
            m_minParseStartSize = minParse;
            m_defaultEncoding = defaultEnc;
        }

        //=========================================================================================
        // 機　能：新しいデータを追加する
        // 引　数：[in]buffer  データの入ったバッファ
        // 　　　　[in]offset  buffer中のオフセット
        // 　　　　[in]length  データの長さ
        // 戻り値：なし
        // メ　モ：読み込みスレッドまたはその他外部の作業スレッドからの呼び出しを想定
        //=========================================================================================
        public void AddData(byte[] buffer, int offset, int length) {
            lock (this) {
                if (m_status != RetrieveDataLoadStatus.Loading) {
                    return;
                }

                // バッファの拡張が必要
                if (m_buffer.Length < m_maxFileSize && m_loadSize + length > m_buffer.Length) {
                    int newBufferSize = Math.Min(m_buffer.Length * 2 + length, m_maxFileSize);
                    byte[] newBuffer = new byte[newBufferSize];
                    Array.Copy(m_buffer, newBuffer, m_loadSize);
                    m_buffer = newBuffer;
                }

                // 内容を格納
                int writeLen = length;
                if (m_loadSize + length > m_maxFileSize) {
                    SetStatus(RetrieveDataLoadStatus.CompletedPart, null);
                    writeLen = m_maxFileSize - m_loadSize;
                }
                if (writeLen > 0) {
                    Array.Copy(buffer, offset, m_buffer, m_loadSize, writeLen);
                    m_loadSize += writeLen;
                    m_chunkLoaded = true;
                } else if (m_minParseStartSize == 0) {
                    m_chunkLoaded = true;
                }
            }
        }
        
        //=========================================================================================
        // 機　能：データの追加が終わったことを通知する
        // 引　数：[in]status    読み込み状況のステータス
        // 　　　　[in]errorInfo status=Failedのときエラー情報の文字列、それ以外はnull
        // 戻り値：なし
        // メ　モ：読み込みスレッドまたはその他外部の作業スレッドからの呼び出しを想定
        //=========================================================================================
        public void AddCompleted(RetrieveDataLoadStatus status, string errorInfo) {
            lock (this) {
                SetStatus(status, errorInfo);
            }
        }

        //=========================================================================================
        // 機　能：読み込み中のステータスを設定する
        // 引　数：[in]status    読み込み状況のステータス
        // 　　　　[in]errorInfo status=Failedのときエラー情報の文字列、それ以外はnull
        // 戻り値：なし
        //=========================================================================================
        private void SetStatus(RetrieveDataLoadStatus status, string errorInfo) {
            RetrieveDataLoadStatus oldStatus = m_status; 
            if (m_status.Priority < status.Priority) {
                m_statusChanged = true;
                m_status = status;
                if (m_errorInfo == null) {
                    m_errorInfo = errorInfo;
                }
            }
            if (oldStatus == RetrieveDataLoadStatus.Loading && m_status != RetrieveDataLoadStatus.Loading) {
                m_loadingTaskId = BackgroundTaskID.NullId;
            }
        }
        
        //=========================================================================================
        // 機　能：イベントを発行する
        // 引　数：[in]final   最後のイベント通知のときtrue
        // 戻り値：なし
        // メ　モ：読み込みスレッドからの呼び出しを想定
        //=========================================================================================
        public void FireEvent(bool final) {
            bool chunkLoaded;
            bool statusChanged;
            lock (this) {
                chunkLoaded = m_chunkLoaded;
                m_chunkLoaded = false;
                statusChanged = m_statusChanged;
                m_statusChanged = false;
            }
            if (StatusChangedEvent != null && (final || chunkLoaded || statusChanged)) {
                StatusChangedEvent(this, new AccessibleFileStatusChangedEventArgs(chunkLoaded, statusChanged));
            }
        }

        //=========================================================================================
        // 機　能：読み込み済みのバッファを返す
        // 引　数：[out]buffer    バッファのバイト配列を返す変数
        // 　　　　[out]readSize  読み込み済みのサイズを返す変数
        // 戻り値：なし
        //=========================================================================================
        public void GetBuffer(out byte[] buffer, out int readSize) {
            lock (this) {
                buffer = m_buffer;
                readSize = m_loadSize;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル名のフルパス（null:ファイル以外から読み込み、DisplayNameとDefaultTabは必須）
        //=========================================================================================
        public string FilePath {
            get {
                return m_filePath;
            }
        }

        //=========================================================================================
        // プロパティ：対象ファイルがあるファイルシステム（None:ファイルシステム上にない）
        //=========================================================================================
        public FileSystemID TargetFileSystem {
            get {
                return m_targetFileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：このファイルの表示名（null:ファイル名を使用）
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
        }

        //=========================================================================================
        // プロパティ：保存を推奨するときtrue
        //=========================================================================================
        public bool SaveRequired {
            get {
                return m_saveRequired && (m_loadSize > 0);
            }
            set {
                m_saveRequired = value;
            }
        }

        //=========================================================================================
        // プロパティ：デフォルトのタブ幅（-1:ファイル名から自動）
        //=========================================================================================
        public int DefaultTab {
            get {
                return m_defaultTab;
            }
        }

        //=========================================================================================
        // プロパティ：解析開始時の最小サイズ（リアルタイムに解析するとき0）
        //=========================================================================================
        public int MinParseStartSize {
            get {
                return m_minParseStartSize;
            }
        }

        //=========================================================================================
        // プロパティ：デフォルトとして使用するエンコーディング（null:不明のため先頭部分を解析）
        //=========================================================================================
        public EncodingType DefaultEncoding {
            get {
                return m_defaultEncoding;
            }
        }

        //=========================================================================================
        // プロパティ：ステータス
        //=========================================================================================
        public RetrieveDataLoadStatus Status {
            get {
                lock (this) {
                    return m_status;
                }
            }
        }

        //=========================================================================================
        // プロパティ：エラー情報
        //=========================================================================================
        public string ErrorInfo {
            get {
                lock (this) {
                    return m_errorInfo;
                }
            }
        }

        //=========================================================================================
        // プロパティ：読み込み用バックグラウンドタスクのID（終了したときBackgroundTaskId.NullId）
        //=========================================================================================
        public BackgroundTaskID LoadingTaskId {
            get {
                return m_loadingTaskId;
            }
            set {
                m_loadingTaskId = value;
            }
        }

        // 以下、読み込み用

        //=========================================================================================
        // プロパティ：高速読み込みするときtrue
        //=========================================================================================
        public bool FastRead {
            get {
                return m_fastRead;
            }
        }
        
        //=========================================================================================
        // プロパティ：最大ファイルサイズ
        //=========================================================================================
        public int MaxFileSize {
            get {
                return m_maxFileSize;
            }
        }

        //=========================================================================================
        // プロパティ：読み込み時間のターンアラウンドタイム（未設定のとき-1）
        //=========================================================================================
        public int LoadingTurnAroundTime {
            get {
                return m_loadingTurnAroundTime;
            }
            set {
                m_loadingTurnAroundTime = value;
            }
        }

        //=========================================================================================
        // プロパティ：読み込み時間のレスポンスタイム（未設定のとき-1）
        //=========================================================================================
        public int LoadingResponseTime {
            get {
                return m_loadingResponseTime;
            }
            set {
                m_loadingResponseTime = value;
            }
        }
    }
}
