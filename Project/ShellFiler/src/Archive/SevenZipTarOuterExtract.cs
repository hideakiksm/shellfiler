using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Nomad.Archive.SevenZip;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：tarアーカイブの外部の展開を行うクラス
    //=========================================================================================
    public class SevenZipTarOuterExtract {
        // アーカイブファイル名（展開前）
        private string m_srcArchiveFileName;

        // アーカイブファイル名（展開後）
        private string m_destArchiveFileName;

        // 7z.dllのライブラリ（外部管理）
        private SevenZipFormat m_sevenZipLib = null;

        // アーカイブインタフェース（null:開いていない）
        private IInArchive m_archive = null;
        
        // アーカイブの読み込み用ストリーム（null:開いていない）
        private InStreamWrapper m_archiveStream = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]sevenZipLib  7z.dllのライブラリ
        // 　　　　[in]arcName      アーカイブファイル名
        // 　　　　[in]destArc      内部のアーカイブファイルを作成するファイル名（nullのとき自動）
        // 戻り値：なし
        //=========================================================================================
        public SevenZipTarOuterExtract(SevenZipFormat sevenZipLib, string arcName, string destArc) {
            m_sevenZipLib = sevenZipLib;
            m_srcArchiveFileName = arcName;
            if (destArc == null) {
                m_destArchiveFileName = Program.Document.TemporaryManager.GetTemporaryFile();
            } else {
                m_destArchiveFileName = destArc;
            }
        }

        //=========================================================================================
        // 機　能：破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            CloseArchive();
            if (m_destArchiveFileName != null) {
                try {
                    File.Delete(m_destArchiveFileName);
                } catch (Exception) {
                }
            }
        }

        //=========================================================================================
        // 機　能：アーカイブをクローズする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CloseArchive() {
            if (m_archiveStream != null) {
                m_archiveStream.Dispose();
                m_archiveStream = null;
            }
            if (m_archive != null) {
                Marshal.ReleaseComObject(m_archive);
                m_archive = null;
            }
        }
        
        //=========================================================================================
        // 機　能：内部ファイルのライフサイクル管理をデタッチする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DetachInnerFile() {
            m_destArchiveFileName = null;
        }

        //=========================================================================================
        // 機　能：tarアーカイブを準備する
        // 引　数：[in]outerType  外側の圧縮形式
        // 　　　　[in]progress   進捗情報表示用のインターフェース
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus PrepareTarArchive(KnownSevenZipFormat outerType, FileProgressEventHandler progress) {
            // アーカイブを作成
            m_archive = m_sevenZipLib.CreateInArchive(SevenZipUtils.GetClassIdFromKnownFormat(outerType));
            if (m_archive == null) {
                return FileOperationStatus.ErrorOpen;
            }

            // アーカイブを開く
            m_archiveStream = new InStreamWrapper(File.OpenRead(m_srcArchiveFileName));
            SevenZipArchiveOpenCallback openCallback = SevenZipArchiveOpenCallback.NewInstance(null);
            ulong checkPos = 128 * 1024;
            if (m_archive.Open(m_archiveStream, ref checkPos, openCallback) != 0) {
                return FileOperationStatus.ErrorOpen;
            }

            // ファイルを展開
            int fileCount = (int)(m_archive.GetNumberOfItems());
            if (fileCount != 1) {
                return FileOperationStatus.ArcDataError;
            }

            // 展開を実行
            FileOperationStatus status = FileOperationStatus.Fail;
            TarExtractCallback callback = new TarExtractCallback(m_destArchiveFileName, progress);
            try {
                m_archive.Extract(new uint[] { 0 }, 1, 0, callback);
                status = callback.ResultStatus;
            } finally {
                callback.Dispose();
            }

            CloseArchive();

            return status;
        }
        
        //=========================================================================================
        // プロパティ：アーカイブファイル名（展開後）
        //=========================================================================================
        public string DestFilePath {
            get {
                return m_destArchiveFileName;
            }
        }

        //=========================================================================================
        // クラス：tarの格納ファイル1個を展開するためのcallback
        //=========================================================================================
        private class TarExtractCallback : IProgress, IArchiveExtractCallback {
            // 展開先のファイル名
            private string m_destFileName;
            // キャンセルされたときtrue
            private bool m_canceled = false;

            // ファイルアクセス用のストリーム
            private OutStreamWrapper m_fileStream;

            // 処理結果のステータス
            private FileOperationStatus m_fileOperationStatus = FileOperationStatus.Fail;

            // 展開処理の進捗状況の通知先
            private FileProgressEventHandler m_progress;
            
            // 処理対象の合計サイズ
            private long m_totalFileSize = -1;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]destFileName  展開先のファイル名
            // 　　　　[in]progress      展開処理の進捗状況の通知先
            // 戻り値：なし
            //=========================================================================================
            public TarExtractCallback(string destFileName, FileProgressEventHandler progress) {
                m_destFileName = destFileName;
                m_progress = progress;
            }

            //=========================================================================================
            // 機　能：ファイル単位の後始末を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Dispose() {
                if (m_fileStream != null) {
                    m_fileStream.Dispose();
                    m_fileStream = null;
                }
            }

            //=========================================================================================
            // 機　能：合計サイズを設定する
            // 引　数：[in]total  合計サイズ
            // 戻り値：なし
            //=========================================================================================
            public int SetTotal(ulong total) {
                m_totalFileSize = (long)(total);
                return 0;
            }

            //=========================================================================================
            // 機　能：処理済みサイズを設定する
            // 引　数：[in,out]completeValue  処理済みサイズ
            // 戻り値：なし
            //=========================================================================================
            public int SetCompleted(ref ulong completeValue) {
                if (m_totalFileSize != -1) {
                    FileProgressEventArgs evt = new FileProgressEventArgs(m_totalFileSize, (long)completeValue);
                    m_progress.SetProgress(this, evt);
                    if (evt.Cancel) {
                        m_canceled = true;
                        m_fileOperationStatus = FileOperationStatus.Canceled;
                        return Win32API.E_FAIL;
                    }
                }
                return 0;
            }

            //=========================================================================================
            // 機　能：ストリームを取得する
            // 引　数：[in]index          取得するファイル位置
            // 　　　　[out]outStream     ストリームを返す変数
            // 　　　　[in]askExtractMode モード
            // 戻り値：ステータス
            //=========================================================================================
            public int GetStream(uint index, out ISequentialOutStream outStream, AskMode askExtractMode) {
                if ((index != 0) || (askExtractMode != AskMode.kExtract)) {
                    outStream = null;
                    return 0;
                }
                try {
                    m_fileStream = new OutStreamWrapper(File.Create(m_destFileName));
                    outStream = m_fileStream;
                    return 0;
                } catch (Exception) {
                    outStream = null;
                    m_fileOperationStatus = FileOperationStatus.Fail;
                    return Win32API.E_FAIL;
                }
            }

            //=========================================================================================
            // 機　能：ファイル操作を準備する
            // 引　数：[in]askExtractMode モード
            // 戻り値：なし
            //=========================================================================================
            public void PrepareOperation(AskMode askExtractMode) {
            }

            //=========================================================================================
            // 機　能：結果を通知する
            // 引　数：[in]result  結果
            // 戻り値：なし
            //=========================================================================================
            public void SetOperationResult(OperationResult result) {
                if (m_canceled) {
                    m_fileOperationStatus = FileOperationStatus.Canceled;
                } else if (result == OperationResult.kOK) {
                    m_fileOperationStatus = FileOperationStatus.Success;
                } else if (result == OperationResult.kCRCError) {
                    m_fileOperationStatus = FileOperationStatus.ArcCrcError;
                } else if (result == OperationResult.kDataError) {
                    m_fileOperationStatus = FileOperationStatus.ArcDataError;
                } else {
                    m_fileOperationStatus = FileOperationStatus.Fail;
                }
            }

            //=========================================================================================
            // プロパティ：処理結果のステータス
            //=========================================================================================
            public FileOperationStatus ResultStatus {
                get {
                    return m_fileOperationStatus;
                }
            }
        }
    }
}
