﻿using System;
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
    // クラス：クリップボードからの読み込み用のデータソース
    //=========================================================================================
    public class ClipboardDataSource : IFileViewerDataSource {
        // クリップボードデータのバッファ
        public byte[] m_dataBuffer;

        // データ状態に変化があったことを知らせるイベント
        public event AccessibleFileStatusChangedEventHandler StatusChangedEvent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]text     クリップボードのテキスト
        // 戻り値：なし
        //=========================================================================================
        public ClipboardDataSource(string text) {
            m_dataBuffer = DefaultEncoding.Encoding.GetBytes(text);
        }

        //=========================================================================================
        // 機　能：読み込み完了のイベントを発行する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void FireEvent() {
            StatusChangedEvent(this, new AccessibleFileStatusChangedEventArgs(true, true));
        }

        //=========================================================================================
        // 機　能：読み込み済みのバッファを返す
        // 引　数：[out]buffer    バッファのバイト配列を返す変数
        // 　　　　[out]readSize  読み込み済みのサイズを返す変数
        // 戻り値：なし
        //=========================================================================================
        public void GetBuffer(out byte[] buffer, out int readSize) {
            buffer = m_dataBuffer;
            readSize = m_dataBuffer.Length;
        }

        //=========================================================================================
        // プロパティ：ファイル名のフルパス（null:ファイル以外から読み込み、DisplayNameとDefaultTabは必須）
        //=========================================================================================
        public string FilePath {
            get {
                return null;
            }
        }

        //=========================================================================================
        // プロパティ：対象ファイルがあるファイルシステム（None:ファイルシステム上にない）
        //=========================================================================================
        public FileSystemID TargetFileSystem {
            get {
                return FileSystemID.None;
            }
        }

        //=========================================================================================
        // プロパティ：このファイルの表示名（null:ファイル名を使用）
        //=========================================================================================
        public string DisplayName {
            get {
                return Resources.FileViewer_ClipboardDisplayName;
            }
        }

        //=========================================================================================
        // プロパティ：保存を推奨するときtrue
        //=========================================================================================
        public bool SaveRequired {
            get {
                return false;
            }
            set {
            }
        }

        //=========================================================================================
        // プロパティ：デフォルトのタブ幅（-1:ファイル名から自動）
        //=========================================================================================
        public int DefaultTab {
            get {
                return 8;
            }
        }

        //=========================================================================================
        // プロパティ：解析開始時の最小サイズ（リアルタイムに解析するとき0）
        //=========================================================================================
        public int MinParseStartSize {
            get {
                return 0;
            }
        }

        //=========================================================================================
        // プロパティ：デフォルトとして使用するエンコーディング（null:不明のため先頭部分を解析）
        //=========================================================================================
        public EncodingType DefaultEncoding {
            get {
                return EncodingType.UTF8;
            }
        }

        //=========================================================================================
        // プロパティ：ステータス
        //=========================================================================================
        public RetrieveDataLoadStatus Status {
            get {
                return RetrieveDataLoadStatus.CompletedAll;
            }
        }

        //=========================================================================================
        // プロパティ：エラー情報
        //=========================================================================================
        public string ErrorInfo {
            get {
                return "";
            }
        }

        //=========================================================================================
        // プロパティ：読み込み用バックグラウンドタスクのID（終了したときBackgroundTaskId.NullId）
        //=========================================================================================
        public BackgroundTaskID LoadingTaskId {
            get {
                return BackgroundTaskID.NullId;
            }
            set {
            }
        }

        //=========================================================================================
        // プロパティ：読み込み時間のターンアラウンドタイム（未設定のとき-1）
        //=========================================================================================
        public int LoadingTurnAroundTime {
            get {
                return -1;
            }
            set {
            }
        }

        //=========================================================================================
        // プロパティ：読み込み時間のレスポンスタイム（未設定のとき-1）
        //=========================================================================================
        public int LoadingResponseTime {
            get {
                return -1;
            }
            set {
            }
        }
    }
}
