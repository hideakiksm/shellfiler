using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.COM;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Log.Logger;
using Nomad.Archive.SevenZip;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：7zでアーカイブを圧縮するときのコールバック
    //=========================================================================================
    public class SevenZipArchiveUpdateCallback : IProgress, IArchiveUpdateCallback, ICryptoGetTextPassword2 {
        // 作成するアーカイブの設定
        private ArchiveParameter m_archiveSetting;

        // ログ出力先のタスク
        private IBackgroundTask m_task;

        // 圧縮対象のファイルとフォルダのベースディレクトリ
        private string m_baseDirectory;

        // 圧縮対象のファイルとフォルダの一覧（ベースからの相対）
        private IList<ArchiveFileDirectoryInfo> m_fileList;

        // フォルダのサイズを0にするときtrue（tar圧縮時はtrue）
        private bool m_zeroFolderSize;

        // 進捗情報表示用のインターフェース
        private FileProgressEventHandler m_progress;

        // 現在処理中のファイルのインデックス
        private int m_currentFileIndex = -1;

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

        // 最新のタイムスタンプ
        private DateTime m_latestTimestamp = DateTime.MinValue;

        // 詳細情報のログ出力行
        private LogLineArchiveDetail m_logLineDetail = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]archiveSetting  作成するアーカイブの設定
        // 　　　　[in]task            ログ出力先のタスク
        // 　　　　[in]baseDirectory   圧縮対象のファイルとフォルダのベースディレクトリ
        // 　　　　[in]fileList        圧縮対象のファイルとフォルダの一覧（ベースからの相対）
        // 　　　　[in]zeroFolderSize  フォルダのサイズを0にするときtrue（tar圧縮時はtrue）
        // 　　　　[in]progress        進捗情報表示用のインターフェース
        // 戻り値：なし
        //=========================================================================================
        public SevenZipArchiveUpdateCallback(ArchiveParameter archiveSetting, IBackgroundTask task, string baseDirectory, List<ArchiveFileDirectoryInfo> fileList, bool zeroFolderSize, FileProgressEventHandler progress) {
            m_archiveSetting = archiveSetting;
            m_task = task;
            m_baseDirectory = baseDirectory;
            m_fileList = fileList;
            m_zeroFolderSize = zeroFolderSize;
            m_progress = progress;
            m_logLineDetail = new LogLineArchiveDetail();
            Program.LogWindow.RegistLogLine(m_logLineDetail);
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
            m_logLineDetail.TotalSize = m_totalFileSize;
            Program.LogWindow.RedrawLogLine(m_logLineDetail);
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
            if (m_currentFileIndex != index) {
                m_currentFileIndex = index;
                try {
                    m_currentFileInfo = new FileInfo(m_baseDirectory + m_fileList[index].FilePath);
                } catch (Exception) {
                    return Win32API.E_FAIL;
                }
            }
            switch (propID) {
                case ItemPropId.kpidPath:
                    Marshal.GetNativeVariantForObject(m_fileList[index].FilePath, value);
                    if (m_fileList[index].Size == ArchiveFileDirectoryInfo.UnknownFileSize) {
                        m_fileList[index].Size = m_currentFileInfo.Length;
                    }
                    if (!m_fileList[index].IsDirectory) {
                        m_totalFileSize += m_fileList[index].Size;
                    }
                    break;
                case ItemPropId.kpidIsFolder:
                    Marshal.GetNativeVariantForObject(m_fileList[index].IsDirectory, value);
                    break;
                case ItemPropId.kpidIsAnti:         // 7zの機能、展開時にだけ存在するファイルはtrue
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
                    if (m_latestTimestamp < m_currentFileInfo.LastWriteTime) {
                        m_latestTimestamp = m_currentFileInfo.LastWriteTime;
                    }
                    break;
                case ItemPropId.kpidSize:
                    if (m_fileList[index].IsDirectory) {
                        if (m_zeroFolderSize) {
                            Marshal.GetNativeVariantForObject((ulong)0, value);
                        } else {
                            Marshal.GetNativeVariantForObject((ulong)m_fileList[index].Size, value);
                        }
                    } else {
                        Marshal.GetNativeVariantForObject((ulong)m_currentFileInfo.Length, value);
                    }
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
            if (m_fileList[index].IsDirectory) {
                inStream = null;
            } else {
                m_logLineDetail.DetailFile = GenericFileStringUtils.GetFileName(m_fileList[index].FilePath);
                Program.LogWindow.RedrawLogLine(m_logLineDetail);
                try {
                    m_currentSourceStream = new FileStream(m_baseDirectory + m_fileList[index].FilePath, FileMode.Open, FileAccess.Read);
                } catch (Exception) {
                    inStream = null;
                    return Win32API.E_FAIL;
                }
                inStream = new InStreamTimedWrapper(m_currentSourceStream);
            }
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
            if (m_archiveSetting.Local7zOption.Encryption) {
                password = m_archiveSetting.Local7zOption.PasswordItem.Password;
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

        //=========================================================================================
        // プロパティ：進捗情報表示用のインターフェース
        //=========================================================================================
        public FileProgressEventHandler ProgressEventHandler {
            get {
                return m_progress;
            }
        }

        //=========================================================================================
        // プロパティ：詳細情報のログ出力行
        //=========================================================================================
        public LogLineArchiveDetail LogLineDetail {
            get {
                return m_logLineDetail;
            }
        }
    }
}
