using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.COM;
using Nomad.Archive.SevenZip;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Log.Logger;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：tarアーカイブの外部の圧縮を行うクラス
    //=========================================================================================
    public class SevenZipTarOuterCreate {
        // 作成するアーカイブの設定
        private ArchiveParameter m_archiveParameter;

        // 圧縮前のアーカイブファイル名
        private string m_srcFileName;

        // 圧縮後のアーカイブファイル名
        private string m_destFileName;

        // 7z.dllのライブラリ（所有オブジェクトから引き継ぎ、ライフサイクル管理しない）
        private SevenZipFormat m_sevenZipLib;

        // アーカイブインタフェース（null:開いていない）
        private IOutArchive m_archive = null;

        // アーカイブの書き込み用ストリーム（null:開いていない）
        private OutStreamWrapper m_archiveStream = null;

        // 2回目の圧縮形式（GZIPまたはBZIP2）
        private KnownSevenZipFormat m_format2;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]sevenZipLib      7z.dllのライブラリ
        // 　　　　[in]archiveParameter 作成するアーカイブの設定
        // 　　　　[in]srcFileName      圧縮前のアーカイブファイル名
        // 　　　　[in]destFileName     圧縮後のアーカイブファイル名
        // 　　　　[in]format2          2回目の圧縮形式（GZIPまたはBZIP2）
        // 戻り値：作成用インターフェース
        //=========================================================================================
        public SevenZipTarOuterCreate(SevenZipFormat sevenZipLib, ArchiveParameter archiveParameter, string srcFileName, string destFileName, KnownSevenZipFormat format2) {
            m_sevenZipLib = sevenZipLib;
            m_archiveParameter = archiveParameter;
            m_srcFileName = srcFileName;
            m_destFileName = destFileName;
            m_format2 = format2;
        }

        //=========================================================================================
        // 機　能：アーカイブを閉じる
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CloseArchive() {
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
        // 機　能：変換処理を行う
        // 引　数：[in]progress  進捗情報表示用のインターフェース
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Convert(FileProgressEventHandler progress, LogLineArchiveDetail logLineDetail) {
            FileOperationStatus status;

            // ファイルの種別を判定
            m_archive = m_sevenZipLib.CreateOutArchive(SevenZipUtils.GetClassIdFromKnownFormat(m_format2));
            if (m_archive == null) {
                return FileOperationStatus.ErrorOpen;
            }

            // オプションを設定する
            SevenZipCreateSetOption.SetOuterArchiveOption(m_archive, m_archiveParameter);

            // アーカイブを開く
            try {
                m_archiveStream = new OutStreamWrapper(File.Create(m_destFileName));
            } catch (Exception) {
                return FileOperationStatus.ErrorOpen;
            }

            // 圧縮する
            TarUpdateCallback updateCallback = new TarUpdateCallback(m_archiveParameter, this, progress, logLineDetail);
            status = updateCallback.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            long arcFileSize = -1;
            status = FileOperationStatus.Fail;
            try {
                status = UpdateItems(updateCallback, out arcFileSize);
            } finally {
                // 取得したファイルサイズを通知して終了
                if (status.Succeeded) {
                    updateCallback.Completed(true, arcFileSize);
                } else {
                    updateCallback.Completed(false, 0);
                }
            }
            return status;
        }

        //=========================================================================================
        // 機　能：格納ファイルの更新を行う
        // 引　数：[in]updateCallback   格納ファイル圧縮用のコールバック
        // 　　　　[out]arcFileSize     アーカイブの最終サイズ
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus UpdateItems(TarUpdateCallback updateCallback, out long arcFileSize) {
            // 圧縮を実行
            arcFileSize = -1;
            try {
                m_archive.UpdateItems(m_archiveStream, 1, updateCallback);
            } catch (COMException) {
                if (updateCallback.Status == FileOperationStatus.Canceled) {
                    return FileOperationStatus.Canceled;
                } else {
                    return FileOperationStatus.Fail;
                }
            }
            if (!updateCallback.Status.Succeeded) {
                return updateCallback.Status;
            }

            // アーカイブを閉じる
            if (m_archiveStream != null) {
                m_archiveStream.Dispose();
                m_archiveStream = null;
            }

            // ファイルサイズを取得
            try {
                FileInfo fileInfo = new FileInfo(m_destFileName);
                arcFileSize = fileInfo.Length;
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // クラス：tarの2重目を圧縮するときのコールバック
        //=========================================================================================
        public class TarUpdateCallback : IProgress, IArchiveUpdateCallback, ICryptoGetTextPassword2 {
            // 作成するアーカイブの設定
            private ArchiveParameter m_archiveParameter;

            // 所有オブジェクト
            private SevenZipTarOuterCreate m_parent;

            // 進捗情報表示用のインターフェース
            private FileProgressEventHandler m_progress;

            // 詳細情報のログ出力行
            private LogLineArchiveDetail m_logLineDetail = null;

            // 現在処理中のファイルの情報
            private FileInfo m_currentFileInfo = null;

            // 現在処理中のファイルを読み込むストリーム（null:開いていない）
            private Stream m_currentSourceStream = null;

            // 処理対象の合計サイズ
            private long m_totalFileSize = 0;

            // キャンセルされたときtrue
            private bool m_canceled = false;

            // すべての処理が成功したときtrue
            private bool m_successAll = true;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]archiveSetting  作成するアーカイブの設定
            // 　　　　[in]parent          このコールバックを所有するオブジェクト
            // 　　　　[in]progress        進捗情報表示用のインターフェース
            // 　　　　[in]logLineDetail   詳細情報のログ出力行
            // 戻り値：なし
            //=========================================================================================
            public TarUpdateCallback(ArchiveParameter archiveSetting, SevenZipTarOuterCreate parent, FileProgressEventHandler progress, LogLineArchiveDetail logLineDetail) {
                m_archiveParameter = archiveSetting;
                m_parent = parent;
                m_progress = progress;
                m_logLineDetail = logLineDetail;
            }

            //=========================================================================================
            // 機　能：初期化する
            // 引　数：なし
            // 戻り値：ステータス
            //=========================================================================================
            public FileOperationStatus Initialize() {
                try {
                    m_currentFileInfo = new FileInfo(m_parent.m_srcFileName);
                    m_totalFileSize = m_currentFileInfo.Length;
                } catch (Exception) {
                    return FileOperationStatus.Fail;
                }
                return FileOperationStatus.Success;
            }

            //=========================================================================================
            // 機　能：圧縮完了時の処理を行う
            // 引　数：[in]success      作成に成功したときtrue
            // 　　　　[in]arcFileSize  アーカイブのファイルサイズ
            // 戻り値：なし
            //=========================================================================================
            public void Completed(bool success, long arcFileSize) {
                if (success) {
                    m_logLineDetail.SetCompletedMessage(arcFileSize);
                } else {
                    m_logLineDetail.SetCancelMessage();
                }
                Program.LogWindow.RedrawLogLine(m_logLineDetail);
            }

            //=========================================================================================
            // 機　能：進捗状態として合計サイズを設定する
            // 引　数：[in]total  合計サイズ
            // 戻り値：COMエラーコード
            //=========================================================================================
            public int SetTotal(ulong total) {
                return 0;
            }

            //=========================================================================================
            // 機　能：進捗状態として処理済みサイズを設定する
            // 引　数：[in,out]completeValue  処理済みサイズ
            // 戻り値：COMエラーコード
            //=========================================================================================
            public int SetCompleted(ref ulong completeValue) {
                m_logLineDetail.CompletedSize = (long)completeValue;
                Program.LogWindow.RedrawLogLine(m_logLineDetail);

                FileProgressEventArgs evt = new FileProgressEventArgs(m_totalFileSize, (long)completeValue);
                m_progress.SetProgress(this, evt);
                if (evt.Cancel) {
                    m_canceled = true;
                    m_successAll = false;
                    return Win32API.E_FAIL;
                }
                return 0;
            }

            //=========================================================================================
            // 機　能：項目の情報を取得する
            // 引　数：[in]index           インデックス
            // 　　　　[out]newData        新しいデータのとき1
            // 　　　　[out]newProperties  新しいプロパティのとき1
            // 　　　　[out]indexInArchive アーカイブにないとき-1
            // 戻り値：COMエラーコード
            //=========================================================================================
            public void GetUpdateItemInfo(int index, out int newData, out int newProperties, out uint indexInArchive) {
                newData = 1;
                newProperties = 1;
                indexInArchive = 0xFFFFFFFF;
            }

            //=========================================================================================
            // 機　能：格納ファイルのプロパティを取得する
            // 引　数：[in]index   インデックス
            // 　　　　[in]propID  取得するプロパティ
            // 　　　　[in]value   取得した値を返すVARIANT
            // 戻り値：COMエラーコード
            //=========================================================================================
            public int GetProperty(int index, ItemPropId propID, IntPtr value) {
                switch (propID) {
                    case ItemPropId.kpidPath: {
                        string fileName = GenericFileStringUtils.GetFileName(m_parent.m_archiveParameter.FileName);
                        fileName = GenericFileStringUtils.GetFileNameBody(fileName);
                        if (!(fileName.ToLower().EndsWith(".tar"))) {
                            fileName += ".tar";
                        }
                        Marshal.GetNativeVariantForObject(fileName, value);
                        break;
                    }
                    case ItemPropId.kpidIsFolder:
                        Marshal.GetNativeVariantForObject(false, value);
                        break;
                    case ItemPropId.kpidIsAnti:
                        Marshal.GetNativeVariantForObject(false, value);
                        break;
                    case ItemPropId.kpidCreationTime:
                        Win32API.GetTimeProperty(m_currentFileInfo.CreationTime, value);
                        break;
                    case ItemPropId.kpidLastAccessTime:
                        Win32API.GetTimeProperty(m_currentFileInfo.LastAccessTime, value);
                        break;
                    case ItemPropId.kpidLastWriteTime:
                        Win32API.GetTimeProperty(m_currentFileInfo.LastWriteTime, value);
                        break;
                    case ItemPropId.kpidSize:
                        Marshal.GetNativeVariantForObject((ulong)m_currentFileInfo.Length, value);
                        break;
                    default:
                        Marshal.WriteInt16(value, (short)VarEnum.VT_EMPTY);
                        break;
                }
                return 0;
            }

            //=========================================================================================
            // 機　能：格納ファイルのデータにアクセスするためのストリームを取得する
            // 引　数：[in]index     インデックス
            // 　　　　[out]inStream アクセス用のストリーム
            // 戻り値：COMエラーコード
            //=========================================================================================
            public int GetStream(int index, out ISequentialInStream inStream) {
                try {
                    m_currentSourceStream = new FileStream(m_parent.m_srcFileName, FileMode.Open, FileAccess.Read);
                } catch (Exception) {
                    inStream = null;
                    return Win32API.E_FAIL;
                }
                inStream = new InStreamTimedWrapper(m_currentSourceStream);
                return 0;
            }

            //=========================================================================================
            // 機　能：格納結果を通知する
            // 引　数：[in]result  格納結果のステータス
            // 戻り値：なし
            //=========================================================================================
            public void SetOperationResult(int result) {
                if (m_currentSourceStream != null) {
                    m_currentSourceStream.Close();
                    m_currentSourceStream = null;
                }
                if (m_canceled || result != 0) {
                    m_successAll = false;
                }
            }

            //=========================================================================================
            // 機　能：パスワードを取得する
            // 引　数：[in]isDefined   パスワードが定義されているときtrueを返す変数
            // 　　　　[in]password    パスワードの文字列
            // 戻り値：なし
            //=========================================================================================
            public void CryptoGetTextPassword2(out bool isDefined, out string password) {
                if (m_archiveParameter.Local7zOption.Encryption) {
                    password = m_archiveParameter.Local7zOption.PasswordItem.Password;
                    isDefined = true;
                } else {
                    password = null;
                    isDefined = false;
                }
            }

            //=========================================================================================
            // プロパティ：実行結果のステータス
            //=========================================================================================
            public FileOperationStatus Status {
                get {
                    if (m_canceled) {
                        return FileOperationStatus.Canceled;
                    } else if (m_successAll) {
                        return FileOperationStatus.Success;
                    } else {
                        return FileOperationStatus.Fail;
                    }
                }
            }
        }
    }
}
